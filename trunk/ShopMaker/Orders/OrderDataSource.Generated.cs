//Generated by DataSourceBaseGenerator

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;

namespace MakerShop.Orders
{
    /// <summary>
    /// DataSource class for Order objects
    /// </summary>
    public partial class OrderDataSource
    {
        /// <summary>
        /// Deletes a Order object from the database
        /// </summary>
        /// <param name="order">The Order object to delete</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static bool Delete(Order order)
        {
            return order.Delete();
        }

        /// <summary>
        /// Deletes a Order object with given id from the database
        /// </summary>
        /// <param name="orderId">Value of OrderId of the object to delete.</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        public static bool Delete(Int32 orderId)
        {
            Order order = new Order();
            if (order.Load(orderId)) return order.Delete();
            return false;
        }

        /// <summary>
        /// Inserts/Saves a Order object to the database.
        /// </summary>
        /// <param name="order">The Order object to insert</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Insert(Order order) { return order.Save(); }

        /// <summary>
        /// Loads a Order object for given Id from the database.
        /// </summary>
        /// <param name="orderId">Value of OrderId of the object to load.</param>
        /// <returns>If the load is successful the newly loaded Order object is returned. If load fails null is returned.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static Order Load(Int32 orderId)
        {
            return OrderDataSource.Load(orderId, true);
        }
        /// <summary>
        /// Loads a Order object for given Id from the database.
        /// </summary>
        /// <param name="orderId">Value of OrderId of the object to load.</param>
        /// <returns>If the load is successful the newly loaded Order object is returned. If load fails null is returned.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static Order LoadParent(Int32 orderId)
        {
            return OrderDataSource.LoadParent(orderId, true);
        }


        /// <summary>
        /// Loads a Order object for given Id from the database.
        /// </summary>
        /// <param name="orderId">Value of OrderId of the object to load.</param>
        /// <param name="useCache">If true tries to load object from cache first.</param>
        /// <returns>If the load is successful the newly loaded Order object is returned. If load fails null is returned.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static Order Load(Int32 orderId, bool useCache)
        {
            if (orderId == 0) return null;
            Order order = null;
            string key = "Order_" + orderId.ToString();
            if (useCache)
            {
                order = ContextCache.GetObject(key) as Order;
                if (order != null) return order;
            }
            order = new Order();
            if (order.Load(orderId))
            {
                if (useCache) ContextCache.SetObject(key, order);
                return order;
            }
            return null;
        }


        /// <summary>
        /// Loads a Order object for given Id from the database.
        /// </summary>
        /// <param name="orderId">Value of OrderId of the object to load.</param>
        /// <param name="useCache">If true tries to load object from cache first.</param>
        /// <returns>If the load is successful the newly loaded Order object is returned. If load fails null is returned.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static Order LoadParent(Int32 orderId, bool useCache)
        {
            if (orderId == 0) return null;
            Order order = null;
            string key = "Order_" + orderId.ToString();
            if (useCache)
            {
                order = ContextCache.GetObject(key) as Order;
                if (order != null) return order;
            }
            order = new Order();

            //set the parentId to check if Order has a Parent
            order.ParentOrderId = orderId;

            if (!order.IsParentOrder)
            {
                if (order.LoadParent(orderId))
                {
                    if (useCache) ContextCache.SetObject(key, order);
                    return order;
                }
            }
            else
            {
                if (order.Load(orderId))
                {
                    if (useCache) ContextCache.SetObject(key, order);
                    return order;
                }

            }
            return null;
        }

        /// <summary>
        /// Counts the number of Order objects in result if retrieved using the given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the Order objects that should be loaded.</param>
        /// <returns>The number of Order objects matching the criteria</returns>
        public static int CountForCriteria(string sqlCriteria)
        {
            Database database = Token.Instance.Database;
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM ac_Orders" + whereClause);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of Order objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <returns>A collection of Order objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderCollection LoadForCriteria(string sqlCriteria)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of Order objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Order objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderCollection LoadForCriteria(string sqlCriteria, string sortExpression)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of Order objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of Order objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderCollection LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex)
        {
            return LoadForCriteria(sqlCriteria, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of Order objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Order objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderCollection  LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Order.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Orders");
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            selectQuery.Append(whereClause);
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            //EXECUTE THE COMMAND
            OrderCollection results = new OrderCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Order order = new Order();
                        Order.LoadDataReader(order, dr);
                        results.Add(order);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of Order objects for the given AffiliateId in the database.
        /// <param name="affiliateId">The given AffiliateId</param>
        /// </summary>
        /// <returns>The Number of Order objects for the given AffiliateId in the database.</returns>
        public static int CountForAffiliate(Int32 affiliateId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM ac_Orders WHERE AffiliateId = @affiliateId");
            database.AddInParameter(selectCommand, "@affiliateId", System.Data.DbType.Int32, NullableData.DbNullify(affiliateId));
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of Order objects for the given AffiliateId from the database
        /// </summary>
        /// <param name="affiliateId">The given AffiliateId</param>
        /// <returns>A collection of Order objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderCollection LoadForAffiliate(Int32 affiliateId)
        {
            return LoadForAffiliate(affiliateId, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of Order objects for the given AffiliateId from the database
        /// </summary>
        /// <param name="affiliateId">The given AffiliateId</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Order objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderCollection LoadForAffiliate(Int32 affiliateId, string sortExpression)
        {
            return LoadForAffiliate(affiliateId, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of Order objects for the given AffiliateId from the database
        /// </summary>
        /// <param name="affiliateId">The given AffiliateId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of Order objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderCollection LoadForAffiliate(Int32 affiliateId, int maximumRows, int startRowIndex)
        {
            return LoadForAffiliate(affiliateId, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of Order objects for the given AffiliateId from the database
        /// </summary>
        /// <param name="affiliateId">The given AffiliateId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Order objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderCollection LoadForAffiliate(Int32 affiliateId, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Order.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Orders");
            selectQuery.Append(" WHERE AffiliateId = @affiliateId");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@affiliateId", System.Data.DbType.Int32, NullableData.DbNullify(affiliateId));
            //EXECUTE THE COMMAND
            OrderCollection results = new OrderCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Order order = new Order();
                        Order.LoadDataReader(order, dr);
                        results.Add(order);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of Order objects for the given OrderStatusId in the database.
        /// <param name="orderStatusId">The given OrderStatusId</param>
        /// </summary>
        /// <returns>The Number of Order objects for the given OrderStatusId in the database.</returns>
        public static int CountForOrderStatus(Int32 orderStatusId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM ac_Orders WHERE OrderStatusId = @orderStatusId");
            database.AddInParameter(selectCommand, "@orderStatusId", System.Data.DbType.Int32, orderStatusId);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of Order objects for the given OrderStatusId from the database
        /// </summary>
        /// <param name="orderStatusId">The given OrderStatusId</param>
        /// <returns>A collection of Order objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderCollection LoadForOrderStatus(Int32 orderStatusId)
        {
            return LoadForOrderStatus(orderStatusId, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of Order objects for the given OrderStatusId from the database
        /// </summary>
        /// <param name="orderStatusId">The given OrderStatusId</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Order objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderCollection LoadForOrderStatus(Int32 orderStatusId, string sortExpression)
        {
            return LoadForOrderStatus(orderStatusId, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of Order objects for the given OrderStatusId from the database
        /// </summary>
        /// <param name="orderStatusId">The given OrderStatusId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of Order objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderCollection LoadForOrderStatus(Int32 orderStatusId, int maximumRows, int startRowIndex)
        {
            return LoadForOrderStatus(orderStatusId, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of Order objects for the given OrderStatusId from the database
        /// </summary>
        /// <param name="orderStatusId">The given OrderStatusId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Order objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderCollection LoadForOrderStatus(Int32 orderStatusId, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Order.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Orders");
            selectQuery.Append(" WHERE OrderStatusId = @orderStatusId");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@orderStatusId", System.Data.DbType.Int32, orderStatusId);
            //EXECUTE THE COMMAND
            OrderCollection results = new OrderCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Order order = new Order();
                        Order.LoadDataReader(order, dr);
                        results.Add(order);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of Order objects for the current store.
        /// </summary>
        /// <returns>The Number of Order objects in the current store.</returns>
        public static int CountForStore()
        {
            int storeId = Token.Instance.StoreId;
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM ac_Orders WHERE StoreId = @storeId");
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, storeId);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of Order objects for the current store from the database
        /// </summary>
        /// <returns>A collection of Order objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderCollection LoadForStore()
        {
            return LoadForStore(0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of Order objects for the current store from the database. Sorts using the given sort exrpression.
        /// </summary>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Order objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderCollection LoadForStore(string sortExpression)
        {
            return LoadForStore(0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of Order objects for the current store from the database.
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of Order objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderCollection LoadForStore(int maximumRows, int startRowIndex)
        {
            return LoadForStore(maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of Order objects for the current store from the database. Sorts using the given sort exrpression.
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Order objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderCollection LoadForStore(int maximumRows, int startRowIndex, string sortExpression)
        {
            int storeId = Token.Instance.StoreId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Order.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Orders");
            selectQuery.Append(" WHERE StoreId = @storeId");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, storeId);
            //EXECUTE THE COMMAND
            OrderCollection results = new OrderCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Order order = new Order();
                        Order.LoadDataReader(order, dr);
                        results.Add(order);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of Order objects for the given UserId in the database.
        /// <param name="userId">The given UserId</param>
        /// </summary>
        /// <returns>The Number of Order objects for the given UserId in the database.</returns>
        public static int CountForUser(Int32 userId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM ac_Orders WHERE UserId = @userId");
            database.AddInParameter(selectCommand, "@userId", System.Data.DbType.Int32, NullableData.DbNullify(userId));
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of Order objects for the given UserId from the database
        /// </summary>
        /// <param name="userId">The given UserId</param>
        /// <returns>A collection of Order objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderCollection LoadForUser(Int32 userId)
        {
            return LoadForUser(userId, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of Order objects for the given UserId from the database
        /// </summary>
        /// <param name="userId">The given UserId</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Order objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderCollection LoadForUser(Int32 userId, string sortExpression)
        {
            return LoadForUser(userId, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of Order objects for the given UserId from the database
        /// </summary>
        /// <param name="userId">The given UserId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of Order objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderCollection LoadForUser(Int32 userId, int maximumRows, int startRowIndex)
        {
            return LoadForUser(userId, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of Order objects for the given UserId from the database
        /// </summary>
        /// <param name="userId">The given UserId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Order objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderCollection LoadForUser(Int32 userId, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Order.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Orders");
            selectQuery.Append(" WHERE UserId = @userId");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@userId", System.Data.DbType.Int32, NullableData.DbNullify(userId));
            //EXECUTE THE COMMAND
            OrderCollection results = new OrderCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Order order = new Order();
                        Order.LoadDataReader(order, dr);
                        results.Add(order);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Updates/Saves the given Order object to the database.
        /// </summary>
        /// <param name="order">The Order object to update/save to database.</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Update)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Update(Order order) { return order.Save(); }

    }
}
