using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Orders;
using MakerShop.Utility;
using MakerShop.Marketing;
using MakerShop.Users;
using MakerShop.Products;
using System.Web;


namespace MakerShop.Reporting.Trial
{
    /// <summary>
    /// DataSource class for various gateway reporting tasks
    /// </summary>
    public partial class ReportDataSource
    {
       


        /// <summary>
        /// Builds a trial charges breakdown summary report for a given period
        /// </summary>
        /// <param name="fromDate">Inclusive start date to consider</param>
        /// <param name="toDate">Inclusive end date to consider</param>
        /// <param name="promoCode">from ac_Affiliates</param>
        /// <param name="subAffiliate">from Order Table</param>
        /// <returns>A DataSet objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DataTable GetTrialChargesByDatePromoSubAffiliate(DateTime fromDate, DateTime endDate,
            string affiliateId, int affiliateMatch, string subAffiliate, int subAffiliateMatch, string sku, string groupBy, string paymentGatewayPrefix)
        {

            DataSet TrialReport = new DataSet();
            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand dataSetCommand = database.GetStoredProcCommand("rpt_TrialPaymentStatus"))
            {
                database.AddInParameter(dataSetCommand, "@StartDate", System.Data.DbType.DateTime, fromDate);
                database.AddInParameter(dataSetCommand, "@EndDate", System.Data.DbType.DateTime, endDate);
                database.AddInParameter(dataSetCommand, "@Affiliate", System.Data.DbType.String, affiliateId);
                database.AddInParameter(dataSetCommand, "@ExactAffiliate", System.Data.DbType.Int32, affiliateMatch);
                database.AddInParameter(dataSetCommand, "@Subaffiliate", System.Data.DbType.String, subAffiliate);
                database.AddInParameter(dataSetCommand, "@ExactSubaffiliate", System.Data.DbType.Int32, subAffiliateMatch);
                database.AddInParameter(dataSetCommand, "@Sku", System.Data.DbType.String, sku);
                database.AddInParameter(dataSetCommand, "@Groupby", System.Data.DbType.String, groupBy);
                database.AddInParameter(dataSetCommand, "@PaymentGatewayPrefix", System.Data.DbType.String, paymentGatewayPrefix);
               
                

                TrialReport = database.ExecuteDataSet(dataSetCommand);
            }

            if(TrialReport.Tables.Count >0)
                return TrialReport.Tables[0];

            return new DataTable();
        }



 

        /// <summary>
        /// Builds a trial charges breakdown summary report for a given period
        /// </summary>
        /// <param name="fromDate">Inclusive start date to consider</param>
        /// <param name="toDate">Inclusive end date to consider</param>
        /// <param name="promoCode">from ac_Affiliates</param>
        /// <param name="subAffiliate">from Order Table</param>
        /// <returns>A DataSet objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DataTable GetTransactionsByDatePromoSubAffiliate(int dateType, DateTime fromDate, DateTime endDate,
            string affiliateId, int affiliateMatch, string subAffiliate, int subAffiliateMatch, int reportType, string paymentTypeIds, string PaymentGatewayPrefix,
             string SKU, string country)
        {

            DataSet TransactionsReport = new DataSet();
            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand dataSetCommand = database.GetStoredProcCommand("rpt_Transactions"))
            {
                switch(dateType)
                {
                    case 1:
                    database.AddInParameter(dataSetCommand, "@StartOrderDate", System.Data.DbType.DateTime, fromDate);
                    database.AddInParameter(dataSetCommand, "@EndOrderDate", System.Data.DbType.DateTime, endDate);
                    break;

                   case 2:
                    database.AddInParameter(dataSetCommand, "@StartTransactionDate", System.Data.DbType.DateTime, fromDate);
                    database.AddInParameter(dataSetCommand, "@EndTransactionDate", System.Data.DbType.DateTime, endDate);
                    break;
                
                   case 3:
                    database.AddInParameter(dataSetCommand, "@StartPaymentDate", System.Data.DbType.DateTime, fromDate);
                    database.AddInParameter(dataSetCommand, "@EndPaymentDate", System.Data.DbType.DateTime, endDate);
                    break;   
                }


                database.AddInParameter(dataSetCommand, "@PaymentGatewayPrefix", System.Data.DbType.String, PaymentGatewayPrefix);
                database.AddInParameter(dataSetCommand, "@Affiliate", System.Data.DbType.String, affiliateId);
                database.AddInParameter(dataSetCommand, "@ExactAffiliate", System.Data.DbType.Int32, affiliateMatch);
                database.AddInParameter(dataSetCommand, "@Subaffiliate", System.Data.DbType.String, subAffiliate);
                database.AddInParameter(dataSetCommand, "@ExactSubaffiliate", System.Data.DbType.Int32, subAffiliateMatch);
                database.AddInParameter(dataSetCommand, "@reportType", System.Data.DbType.Int32, reportType);
                database.AddInParameter(dataSetCommand, "@PaymentTypeIds", System.Data.DbType.String, paymentTypeIds);
                database.AddInParameter(dataSetCommand, "@SKU", System.Data.DbType.String, SKU);
                database.AddInParameter(dataSetCommand, "@country", System.Data.DbType.String, country);

                TransactionsReport = database.ExecuteDataSet(dataSetCommand);
            }

            if (TransactionsReport.Tables.Count > 0)
                return TransactionsReport.Tables[0];

            return new DataTable();
        }


      
        /// <summary>
        /// Get SubAffiliates 
        /// </summary>
        /// <returns>A DataSet objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DataTable GetSubAffiliate()
        {

            DataSet SubAffiliates = new DataSet();
            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand dataSetCommand = database.GetStoredProcCommand("dll_SubAffiliate"))
            {
                SubAffiliates = database.ExecuteDataSet(dataSetCommand);
            }

            return SubAffiliates.Tables[0];
        }

        /// <summary>
        /// Get PromoCodes 
        /// </summary>
        /// <returns>A DataSet objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DataTable GetPromoCode()
        {
           // Marketing.AffiliateCollection
          // return Marketing.AffiliateDataSource.LoadForStore();
            DataSet PromoCodes = new DataSet();
            Database database = Token.Instance.Database;
            string sql = string.Format(@"SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;(SELECT value =-1, AffiliateId='All' ) UNION 
(SELECT DISTINCT value = ISNULL(AffiliateId,0) , AffiliateId = CAST(ISNULL(AffiliateId,0) AS varchar)  FROM dbo.ac_Orders) ORDER BY AffiliateId desc");

            using (System.Data.Common.DbCommand dataSetCommand = database.GetSqlStringCommand(sql))
            {
                PromoCodes = database.ExecuteDataSet(dataSetCommand);
            }

            return PromoCodes.Tables[0];
        }


        /// <summary>
        /// Get Product Sku
        /// </summary>
        /// <returns>A DataSet objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DataTable GetSku()
        {
            //ProductCollection 
           // return ProductDataSource.LoadForStore();

            DataSet Skus = new DataSet();
            Database database = Token.Instance.Database;
            string sql = string.Format(@"SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;(SELECT value='-1' , sku ='- All') UNION (SELECT Distinct value = sku, Sku FROM dbo.ac_Products
WHERE Sku is not null) ORDER BY Sku");

            using (System.Data.Common.DbCommand dataSetCommand = database.GetSqlStringCommand(sql))
            {
                Skus = database.ExecuteDataSet(dataSetCommand);
            }

            return Skus.Tables[0];
        }

    }
}
