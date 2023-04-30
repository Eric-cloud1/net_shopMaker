using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;

namespace MakerShop.Products
{
    /// <summary>
    /// DataSource class for SubscriptionPlan objects
    /// </summary>
    [DataObject(true)]
    public partial class SubscriptionPlanDataSource
    {
        /// <summary>
        /// Counts the number of subscription plans in the current store
        /// </summary>
        /// <returns>The number of subscription plans in the current store</returns>
        public static int CountForStore()
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM ac_SubscriptionPlans SP INNER JOIN ac_Products P on SP.ProductId = P.ProductId WHERE P.StoreId = @storeId");
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of subscription plans for the current store
        /// </summary>
        /// <returns>A collection of SubscriptionPlan objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SubscriptionPlanCollection LoadForStore()
        {
            return LoadForStore(0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of subscription plans for the current store
        /// </summary>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A collection of SubscriptionPlan objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SubscriptionPlanCollection LoadForStore(string sortExpression)
        {
            return LoadForStore(0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of subscription plans for the current store
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <returns>A collection of SubscriptionPlan objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SubscriptionPlanCollection LoadForStore(int maximumRows, int startRowIndex)
        {
            return LoadForStore(maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of subscription plans for the current store
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A collection of SubscriptionPlan objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SubscriptionPlanCollection LoadForStore(int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + SubscriptionPlan.GetColumnNames("SP"));
            selectQuery.Append(" FROM ac_SubscriptionPlans SP INNER JOIN ac_Products P ON SP.ProductId = P.ProductId");
            selectQuery.Append(" WHERE P.StoreId = @storeId");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //EXECUTE THE COMMAND
            SubscriptionPlanCollection results = new SubscriptionPlanCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        SubscriptionPlan subscriptionPlan = new SubscriptionPlan();
                        SubscriptionPlan.LoadDataReader(subscriptionPlan, dr);
                        results.Add(subscriptionPlan);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

    }
}
