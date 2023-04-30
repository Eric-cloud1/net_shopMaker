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

namespace MakerShop.Reporting
{
    public partial class ReportDataSource
    {
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DataTable GetForecastAuthorizedReport(DateTime fromDate, DateTime endDate,
           string affiliateId, string subAffiliate, int subAffiliateMatch, short groupBy)
        {
            DataSet ForecastReport = new DataSet();
            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand dataSetCommand = database.GetStoredProcCommand("rpt_ForecastReport_Authorized"))
            {
                database.AddInParameter(dataSetCommand, "@StartDate", System.Data.DbType.DateTime, fromDate);
                database.AddInParameter(dataSetCommand, "@EndDate", System.Data.DbType.DateTime, endDate);
                database.AddInParameter(dataSetCommand, "@Affiliate", System.Data.DbType.String, affiliateId);
                database.AddInParameter(dataSetCommand, "@Subaffiliate", System.Data.DbType.String, subAffiliate);
                database.AddInParameter(dataSetCommand, "@ExactSubaffiliate", System.Data.DbType.Int32, subAffiliateMatch);
                database.AddInParameter(dataSetCommand, "@Groupby", System.Data.DbType.Int16, groupBy);
           

               

                ForecastReport = database.ExecuteDataSet(dataSetCommand);
            }

            return ForecastReport.Tables[0];
        }


        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DataTable GetForecastShortAuthorizedReport(DateTime fromDate, DateTime endDate,
           string affiliateId, string subAffiliate, int subAffiliateMatch, short groupBy)
        {
            DataSet ForecastReport = new DataSet();
            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand dataSetCommand = database.GetStoredProcCommand("rpt_ForecastReport_Authorized"))
            {
                database.AddInParameter(dataSetCommand, "@StartDate", System.Data.DbType.DateTime, fromDate);
                database.AddInParameter(dataSetCommand, "@EndDate", System.Data.DbType.DateTime, endDate);
                database.AddInParameter(dataSetCommand, "@Affiliate", System.Data.DbType.String, affiliateId);
                database.AddInParameter(dataSetCommand, "@Subaffiliate", System.Data.DbType.String, subAffiliate);
                database.AddInParameter(dataSetCommand, "@ExactSubaffiliate", System.Data.DbType.Int32, subAffiliateMatch);
                database.AddInParameter(dataSetCommand, "@Groupby", System.Data.DbType.Int16, groupBy);




                ForecastReport = database.ExecuteDataSet(dataSetCommand);
            }

            return ForecastReport.Tables[0];
        }
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DataTable GetForecastCapturedReport(DateTime fromDate, DateTime endDate,
           string affiliateId, string subAffiliate, int subAffiliateMatch, short groupBy)
        {
            DataSet ForecastReport = new DataSet();
            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand dataSetCommand = database.GetStoredProcCommand("rpt_ForecastReport_Captured"))
            {
                database.AddInParameter(dataSetCommand, "@StartDate", System.Data.DbType.DateTime, fromDate);
                database.AddInParameter(dataSetCommand, "@EndDate", System.Data.DbType.DateTime, endDate);
                database.AddInParameter(dataSetCommand, "@Affiliate", System.Data.DbType.String, affiliateId);
                database.AddInParameter(dataSetCommand, "@Subaffiliate", System.Data.DbType.String, subAffiliate);
                database.AddInParameter(dataSetCommand, "@ExactSubaffiliate", System.Data.DbType.Int32, subAffiliateMatch);
                database.AddInParameter(dataSetCommand, "@Groupby", System.Data.DbType.Int16, groupBy);




                ForecastReport = database.ExecuteDataSet(dataSetCommand);
            }

            return ForecastReport.Tables[0];
        }



        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DataTable GetForecastShortCapturedReport(DateTime fromDate, DateTime endDate,
           string affiliateId, string subAffiliate, int subAffiliateMatch, short groupBy)
        {
            DataSet ForecastReport = new DataSet();
            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand dataSetCommand = database.GetStoredProcCommand("rpt_ForecastReport_Captured"))
            {
                database.AddInParameter(dataSetCommand, "@StartDate", System.Data.DbType.DateTime, fromDate);
                database.AddInParameter(dataSetCommand, "@EndDate", System.Data.DbType.DateTime, endDate);
                database.AddInParameter(dataSetCommand, "@Affiliate", System.Data.DbType.String, affiliateId);
                database.AddInParameter(dataSetCommand, "@Subaffiliate", System.Data.DbType.String, subAffiliate);
                database.AddInParameter(dataSetCommand, "@ExactSubaffiliate", System.Data.DbType.Int32, subAffiliateMatch);
                database.AddInParameter(dataSetCommand, "@Groupby", System.Data.DbType.Int16, groupBy);




                ForecastReport = database.ExecuteDataSet(dataSetCommand);
            }

            return ForecastReport.Tables[0];
        }

        

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
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DataTable GetSku()
        {
            //ProductCollection 
            // return ProductDataSource.LoadForStore();

            DataSet Skus = new DataSet();
            Database database = Token.Instance.Database;
            string sql = string.Format(@"(SELECT sku ='---') UNION (SELECT Distinct Sku FROM dbo.ac_Products
WHERE Sku is not null) ORDER BY Sku");

            using (System.Data.Common.DbCommand dataSetCommand = database.GetSqlStringCommand(sql))
            {
                Skus = database.ExecuteDataSet(dataSetCommand);
            }

            return Skus.Tables[0];
        }
    }
}
