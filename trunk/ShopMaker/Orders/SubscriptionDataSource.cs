using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Search;
using MakerShop.Utility;

namespace MakerShop.Orders
{
    [DataObject(true)]
    public partial class SubscriptionDataSource
    {
        /// <summary>
        /// Loads the subscriptions associated with the specified order.
        /// </summary>
        /// <param name="orderId">The order ID to load subscriptions for.</param>
        /// <returns>A collection of subscriptions for the order.</returns>
        public static SubscriptionCollection LoadForOrder(int orderId)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + Subscription.GetColumnNames("S"));
            selectQuery.Append(" FROM ac_Subscriptions S INNER JOIN ac_OrderItems OI ON S.OrderItemId = OI.OrderItemId");
            selectQuery.Append(" WHERE OI.OrderId = @orderId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@orderId", System.Data.DbType.Int32, orderId);
            //EXECUTE THE COMMAND
            SubscriptionCollection results = new SubscriptionCollection();
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    Subscription subscription = new Subscription();
                    Subscription.LoadDataReader(subscription, dr);
                    results.Add(subscription);
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Deletes the subscriptions associated with the specified order.
        /// </summary>
        /// <param name="orderId">The order ID to delete subscriptions for.</param>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public static bool DeleteForOrder(int orderId)
        {
            // WE NEED TO USE THE Subscription.Delete METHOD
            // SO THAT SUBSCRIPTION EXPIRATION FOR USER GROUPS IS PROPERLY RECALCULATED
            SubscriptionCollection subscriptions = SubscriptionDataSource.LoadForOrder(orderId);
            if (subscriptions.Count > 0)
            {
                subscriptions.DeleteAll();
                return true;
            }
            else return false;
        }

        public static SubscriptionCollection LoadExpiredSubscriptions(int productId)
        {
            return LoadExpiredSubscriptions(productId, string.Empty);
        }

        public static SubscriptionCollection LoadExpiredSubscriptions(int productId, string sortExpression)
        {
            return LoadExpiredSubscriptions(productId, 0, 0, sortExpression);
        }

        public static SubscriptionCollection LoadExpiredSubscriptions(int productId, int maximumRows, int startRowIndex, string sortExpression)
        {
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Subscription.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Subscriptions");
            selectQuery.Append(" WHERE ProductId = @productId AND ExpirationDate <= @currentDate ");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
            database.AddInParameter(selectCommand, "@currentDate", System.Data.DbType.DateTime, DateTime.Now.ToUniversalTime());
            //EXECUTE THE COMMAND
            SubscriptionCollection subscriptions = new SubscriptionCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Subscription subscription = new Subscription();
                        Subscription.LoadDataReader(subscription, dr);
                        subscriptions.Add(subscription);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return subscriptions;
        }

        /// <summary>
        /// Gets a count of subscriptions matching the given search criteria
        /// </summary>
        /// <param name="subscriptionPlanId">Specifies the subscription plan to match - pass 0 for all plans.</param>
        /// <param name="orderRange">Order number or range of orders to find.</param>
        /// <param name="userIdRange">User Id range of subscriptions to find.</param>
        /// <param name="firstName">The first name to match, by default a begins with match or use * wildcard to alter behavior.</param>
        /// <param name="lastName">The last name to match, by default a begins with match or use * wildcard to alter behavior.</param>
        /// <param name="email">The email name to match, by default a begins with match or use * wildcard to alter behavior.</param>
        /// <param name="expirationStart">Specifies to match subscriptions that expire on or after this date.  Pass DateTime.MinValue for no start date.</param>
        /// <param name="expirationEnd">Specifies to match subscriptions that expire on or before this date.  Pass DateTime.MaxValue for no end date.</param>
        /// <param name="active">True, false, or any to filter by active status.</param>
        /// <returns>A count of subscriptions matching the given search criteria</returns>
        public static int SearchCount(int subscriptionPlanId, string orderRange, string userIdRange, string firstName, string lastName, string email, DateTime expirationStart, DateTime expirationEnd, BitFieldState active)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = BuildSearchCommand(database, subscriptionPlanId, orderRange, userIdRange, firstName, lastName, email, expirationStart, expirationEnd, active, 0, 0, string.Empty, true);
            return AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Gets a subscription collection matching the given search criteria
        /// </summary>
        /// <param name="subscriptionPlanId">Specifies the subscription plan to match - pass 0 for all plans.</param>
        /// <param name="orderRange">Order number or range of orders to show.</param>
        /// <param name="firstName">The first name to match, by default a begins with match or use * wildcard to alter behavior.</param>
        /// <param name="lastName">The last name to match, by default a begins with match or use * wildcard to alter behavior.</param>
        /// <param name="email">The email name to match, by default a begins with match or use * wildcard to alter behavior.</param>
        /// <param name="expirationStart">Specifies to match subscriptions that expire on or after this date.  Pass DateTime.MinValue for no start date.</param>
        /// <param name="expirationEnd">Specifies to match subscriptions that expire on or before this date.  Pass DateTime.MaxValue for no end date.</param>
        /// <param param name="active">If true only active subscriptions will be included in results</param>
        /// <returns>A collection of subscriptions matching the given search criteria</returns>
        public static SubscriptionCollection Search(int subscriptionPlanId, string orderRange, string userIdRange, string firstName, string lastName, string email, DateTime expirationStart, DateTime expirationEnd, BitFieldState active)
        {
            return Search(subscriptionPlanId, orderRange, userIdRange, firstName, lastName, email, expirationStart, expirationEnd, active, 0, 0, string.Empty);
        }

        /// <summary>
        /// Gets a subscription collection matching the given search criteria
        /// </summary>
        /// <param name="subscriptionPlanId">Specifies the subscription plan to match - pass 0 for all plans.</param>
        /// <param name="orderRange">Order number or range of orders to show.</param>
        /// <param name="firstName">The first name to match, by default a begins with match or use * wildcard to alter behavior.</param>
        /// <param name="lastName">The last name to match, by default a begins with match or use * wildcard to alter behavior.</param>
        /// <param name="email">The email name to match, by default a begins with match or use * wildcard to alter behavior.</param>
        /// <param name="expirationStart">Specifies to match subscriptions that expire on or after this date.  Pass DateTime.MinValue for no start date.</param>
        /// <param name="expirationEnd">Specifies to match subscriptions that expire on or before this date.  Pass DateTime.MaxValue for no end date.</param>
        /// <param param name="active">If true only active subscriptions will be included in results</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A collection of subscriptions matching the given search criteria</returns>
        public static SubscriptionCollection Search(int subscriptionPlanId, string orderRange, string userIdRange, string firstName, string lastName, string email, DateTime expirationStart, DateTime expirationEnd, BitFieldState active, string sortExpression)
        {
            return Search(subscriptionPlanId, orderRange, userIdRange, firstName, lastName, email, expirationStart, expirationEnd, active, 0, 0, sortExpression);
        }

        /// <summary>
        /// Gets a subscription collection matching the given search criteria
        /// </summary>
        /// <param name="subscriptionPlanId">Specifies the subscription plan to match - pass 0 for all plans.</param>
        /// <param name="orderRange">Order number or range of orders to show.</param>
        /// <param name="firstName">The first name to match, by default a begins with match or use * wildcard to alter behavior.</param>
        /// <param name="lastName">The last name to match, by default a begins with match or use * wildcard to alter behavior.</param>
        /// <param name="email">The email name to match, by default a begins with match or use * wildcard to alter behavior.</param>
        /// <param name="expirationStart">Specifies to match subscriptions that expire on or after this date.  Pass DateTime.MinValue for no start date.</param>
        /// <param name="expirationEnd">Specifies to match subscriptions that expire on or before this date.  Pass DateTime.MaxValue for no end date.</param>
        /// <param param name="active">If true only active subscriptions will be included in results</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <returns>A collection of subscriptions matching the given search criteria</returns>
        public static SubscriptionCollection Search(int subscriptionPlanId, string orderRange, string userIdRange, string firstName, string lastName, string email, DateTime expirationStart, DateTime expirationEnd, BitFieldState active, int maximumRows, int startRowIndex)
        {
            return Search(subscriptionPlanId, orderRange, userIdRange, firstName, lastName, email, expirationStart, expirationEnd, active, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Gets a subscription collection matching the given search criteria
        /// </summary>
        /// <param name="subscriptionPlanId">Specifies the subscription plan to match - pass 0 for all plans.</param>
        /// <param name="orderRange">Order number or range of orders to show.</param>
        /// <param name="firstName">The first name to match, by default a begins with match or use * wildcard to alter behavior.</param>
        /// <param name="lastName">The last name to match, by default a begins with match or use * wildcard to alter behavior.</param>
        /// <param name="email">The email name to match, by default a begins with match or use * wildcard to alter behavior.</param>
        /// <param name="expirationStart">Specifies to match subscriptions that expire on or after this date.  Pass DateTime.MinValue for no start date.</param>
        /// <param name="expirationEnd">Specifies to match subscriptions that expire on or before this date.  Pass DateTime.MaxValue for no end date.</param>
        /// <param param name="active">If true only active subscriptions will be included in results</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A collection of subscriptions matching the given search criteria</returns>
        public static SubscriptionCollection Search(int subscriptionPlanId, string orderRange, string userIdRange, string firstName, string lastName, string email, DateTime expirationStart, DateTime expirationEnd, BitFieldState active, int maximumRows, int startRowIndex, string sortExpression)
        {
            // GENERATE THE SQL STATEMENT
            Database database = Token.Instance.Database;
            DbCommand selectCommand = BuildSearchCommand(database, subscriptionPlanId, orderRange, userIdRange, firstName, lastName, email, expirationStart, expirationEnd, active, maximumRows, startRowIndex, sortExpression, false);

            //EXECUTE THE COMMAND
            SubscriptionCollection subscriptions = new SubscriptionCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Subscription subscription = new Subscription();
                        Subscription.LoadDataReader(subscription, dr);
                        subscriptions.Add(subscription);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return subscriptions;
        }

        private static DbCommand BuildSearchCommand(Database database, int subscriptionPlanId, string orderRange, string userIdRange, string firstName, string lastName, string email, DateTime expirationStart, DateTime expirationEnd, BitFieldState active, int maximumRows, int startRowIndex, string sortExpression, bool count)
        {
            // INITIALIZE SELECT FIELDS AND JOINS
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (!count)
            {
                if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
                selectQuery.Append(" " + Subscription.GetColumnNames("S"));
            }
            else
            {
                selectQuery.Append(" COUNT(1) AS SubscriptionCount");
            }
            selectQuery.Append(" FROM (((((ac_Subscriptions S INNER JOIN ac_OrderItems OI ON S.OrderItemId = OI.OrderItemId)");
            selectQuery.Append(" INNER JOIN ac_SubscriptionPlans SP ON S.ProductId = SP.ProductId)");
            selectQuery.Append(" INNER JOIN ac_Orders O ON OI.OrderId = O.OrderId)");
            selectQuery.Append(" INNER JOIN ac_Users U ON S.UserId = U.UserId)");
            selectQuery.Append(" INNER JOIN ac_Addresses A ON U.PrimaryAddressId = A.AddressId)");
            selectQuery.Append(" WHERE U.StoreId = @storeId");
            if (active != BitFieldState.Any)
            {
                selectQuery.Append(" AND S.IsActive = " + (active == BitFieldState.True ? "1" : "0"));
            }

            // FILTER BY SUB PLAN
            if (subscriptionPlanId > 0)
            {
                selectQuery.Append(" AND S.ProductId = @subscriptionPlanId");
            }

            // FILTER BY USER ID
            IdRangeParser userRangeParser = new IdRangeParser("S.UserId", userIdRange, "u");
            selectQuery.Append(userRangeParser.GetSqlString("AND"));

            // FILTER BY USER NAME
            if (!string.IsNullOrEmpty(firstName))
            {
                selectQuery.Append(KeywordSearchHelper.PrepareSqlFilter("ac_Addresses", "A", "FirstName", "@firstName", ref firstName));
            }
            if (!string.IsNullOrEmpty(lastName))
            {
                selectQuery.Append(KeywordSearchHelper.PrepareSqlFilter("ac_Addresses", "A", "LastName", "@lastName", ref lastName));
            }
            if (!string.IsNullOrEmpty(email))
            {
                selectQuery.Append(KeywordSearchHelper.PrepareSqlFilter("ac_Users", "U", "LoweredEmail", "@email", ref email));
            }

            // FILTER BY ORDER NUMBER
            IdRangeParser orderRangeParser = new IdRangeParser("O.OrderNumber", orderRange, "o");
            selectQuery.Append(orderRangeParser.GetSqlString("AND"));

            // FILTER BY EXPIRATION
            if (expirationStart > DateTime.MinValue && expirationStart < DateTime.MaxValue)
            {
                selectQuery.Append(" AND S.ExpirationDate >= @expirationStart");
            }
            if (expirationEnd > DateTime.MinValue && expirationEnd < DateTime.MaxValue)
            {
                selectQuery.Append(" AND S.ExpirationDate <= @expirationEnd");
            }

            // ADD SORT EXPRESSION
            if (!count && !string.IsNullOrEmpty(sortExpression))
            {
                string loweredSortExpression = sortExpression.ToLowerInvariant();
                if (loweredSortExpression == "s.expirationdate")
                {
                    selectQuery.Append(" ORDER BY CASE WHEN S.ExpirationDate IS NULL THEN 1 ELSE 0 END, S.ExpirationDate");
                }
                else if (loweredSortExpression == "s.expirationdate desc")
                {
                    selectQuery.Append(" ORDER BY CASE WHEN S.ExpirationDate IS NULL THEN 0 ELSE 1 END, S.ExpirationDate DESC");
                }
                else selectQuery.Append(" ORDER BY " + sortExpression);
            }

            // BUILD THE COMMAND 
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);

            // FILTER BY SUB PLAN
            if (subscriptionPlanId > 0)
            {
                database.AddInParameter(selectCommand, "@subscriptionPlanId", DbType.Int32, subscriptionPlanId);
            }

            // FILTER BY ORDER NUMBER
            userRangeParser.AddParameters(database, selectCommand);

            // FILTER BY USER NAME
            if (!string.IsNullOrEmpty(firstName))
            {
                database.AddInParameter(selectCommand, "@firstName", DbType.String, firstName);
            }
            if (!string.IsNullOrEmpty(lastName))
            {
                database.AddInParameter(selectCommand, "@lastName", DbType.String, lastName);
            }
            if (!string.IsNullOrEmpty(email))
            {
                database.AddInParameter(selectCommand, "@email", DbType.String, email);
            }

            // FILTER BY ORDER NUMBER
            orderRangeParser.AddParameters(database, selectCommand);

            // FILTER BY EXPIRATION
            if (expirationStart > DateTime.MinValue && expirationStart < DateTime.MaxValue)
            {
                if (expirationStart.Kind == DateTimeKind.Local) expirationStart = LocaleHelper.FromLocalTime(expirationStart);
                database.AddInParameter(selectCommand, "@expirationStart", DbType.DateTime, expirationStart);
            }
            if (expirationEnd > DateTime.MinValue && expirationEnd < DateTime.MaxValue)
            {
                if (expirationEnd.Kind == DateTimeKind.Local) expirationEnd = LocaleHelper.FromLocalTime(expirationEnd);
                database.AddInParameter(selectCommand, "@expirationEnd", DbType.DateTime, expirationEnd);
            }

            // RETURN THE PREPARED COMMAND
            return selectCommand;
        }
    }
}