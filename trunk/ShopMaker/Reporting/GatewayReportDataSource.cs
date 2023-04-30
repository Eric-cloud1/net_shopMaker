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

namespace MakerShop.Reporting.Gateway
{
    /// <summary>
    /// DataSource class for various gateway reporting tasks
    /// </summary>
    public partial class ReportDataSource
    {

     
        /// <summary>
        /// Builds a Gateway Performance summary report for a given period
        /// </summary>
        /// <param name="fromDate">Inclusive start date to consider</param>
        /// <param name="toDate">Inclusive end date to consider</param>
        /// <param name="promoCode">from ac_Affiliates</param>
        /// <param name="subAffiliate">from Order Table</param>
        /// <param name="ExactSubaffiliate">Match subaffiliate</param>
        /// <returns>A DataSet objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DataTable GetGatewayPerformanceByDatePromoSubAffiliate(DateTime fromDate, DateTime endDate,
            string affiliateId, string subAffiliate, int subAffiliateMatch, string paymentIds)
        {

            DataSet ConversionReport = new DataSet();
            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand dataSetCommand = database.GetStoredProcCommand("rpt_GatewayPerformance"))
            {
                database.AddInParameter(dataSetCommand, "@StartDate", System.Data.DbType.DateTime, fromDate);
                database.AddInParameter(dataSetCommand, "@EndDate", System.Data.DbType.DateTime, endDate);
                database.AddInParameter(dataSetCommand, "@Affiliate", System.Data.DbType.String, affiliateId);
                database.AddInParameter(dataSetCommand, "@Subaffiliate", System.Data.DbType.String, subAffiliate);
                database.AddInParameter(dataSetCommand, "@ExactSubaffiliate", System.Data.DbType.Int32, subAffiliateMatch);
                database.AddInParameter(dataSetCommand, "@paymentId", System.Data.DbType.String, paymentIds);



                ConversionReport = database.ExecuteDataSet(dataSetCommand);
            }

            if (ConversionReport.Tables.Count > 0)
                return ConversionReport.Tables[0];

            return new DataTable();
        }

        /// <summary>
        /// Builds a product breakdown summary report for a given period
        /// </summary>
        /// <param name="fromDate">Inclusive start date to consider</param>
        /// <param name="toDate">Inclusive end date to consider</param>
        /// <param name="paymentGatewayId">If vendorId > 0 than includes products for this paymentGateway only. All gateways are included otherwise.</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A List of GatewayBreakdownSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<GatewayBreakdownSummary> GetGatewayBreakdownSummary(DateTime fromDate, DateTime toDate, int paymentGatewayId, string sortExpression)
        {
            //GET INVALID ORDER STATUSES
            List<OrderStatus> reportStatuses = OrderStatusDataSource.GetReportStatuses();
            List<GatewayBreakdownSummary> breakdownReport = new List<GatewayBreakdownSummary>();

            DataSet RebillReport = new DataSet();
            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand dataSetCommand = database.GetStoredProcCommand("rpt_GatewayBreakdownSummary"))
            {

                if (fromDate != DateTime.MinValue)
                    database.AddInParameter(dataSetCommand, "@FD", System.Data.DbType.DateTime, fromDate);
                if (toDate != DateTime.MinValue)
                    database.AddInParameter(dataSetCommand, "@TD", System.Data.DbType.DateTime, toDate);
                if (paymentGatewayId > 0)
                    database.AddInParameter(dataSetCommand, "@PGI", System.Data.DbType.Int32, paymentGatewayId);

                if (!string.IsNullOrEmpty(sortExpression))
                    database.AddInParameter(dataSetCommand, "@orderBy", System.Data.DbType.String, sortExpression);

              

                //EXECUTE THE COMMAND
                using (IDataReader dr = database.ExecuteReader(dataSetCommand))
                {
                    GatewayBreakdownSummary breakdownSummary = null;
                    while (dr.Read())
                    {
                        breakdownSummary = new GatewayBreakdownSummary();


                        breakdownSummary.PaymentGatewayId = (int)dr["PaymentGatewayId"];
                        breakdownSummary.Name = dr["Name"].ToString();
                        breakdownSummary.TransactionStatus = dr["Transaction Status"].ToString();
                        breakdownSummary.Amount = AlwaysConvert.ToDecimal(dr["Amount"].ToString());
                        breakdownSummary.TransactionType = dr["Transaction Type"].ToString();
                        breakdownSummary.TransactionCount = dr["Transaction Count"].ToString();
                        breakdownReport.Add(breakdownSummary);

                    }
                    dr.Close();
                }
                return breakdownReport;
            }


        }


   



        /// <summary>
        /// Builds a product breakdown summary report for a given period
        /// </summary>
        /// <param name="fromDate">Inclusive start date to consider</param>
        /// <param name="toDate">Inclusive end date to consider</param>
        /// <param name="paymentGatewayId">If vendorId > 0 than includes products for this paymentGateway only. All gateways are included otherwise.</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A List of GatewayBreakdownSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DataTable GetTableGatewayBreakdownSummary(DateTime fromDate, DateTime toDate, int paymentGatewayId, string sortExpression, bool authorize, bool dateGroup)
        {
            Database database = Token.Instance.Database;
            DataSet ds = new DataSet();

                using (System.Data.Common.DbCommand dataSetCommand = database.GetStoredProcCommand("rpt_GatewayBreakdownSummary"))
                {

                    if (fromDate != DateTime.MinValue)
                        database.AddInParameter(dataSetCommand, "@FD", System.Data.DbType.DateTime, fromDate);
                    if (toDate != DateTime.MinValue)
                        database.AddInParameter(dataSetCommand, "@TD", System.Data.DbType.DateTime, toDate);
                    if (paymentGatewayId > 0)
                        database.AddInParameter(dataSetCommand, "@PGI", System.Data.DbType.Int32, paymentGatewayId);

                    if (!string.IsNullOrEmpty(sortExpression))
                        database.AddInParameter(dataSetCommand, "@orderBy", System.Data.DbType.String, sortExpression);

                    database.AddInParameter(dataSetCommand, "@Authorize", System.Data.DbType.Boolean, authorize);
                    database.AddInParameter(dataSetCommand, "@DateGroup", System.Data.DbType.Boolean, dateGroup);


                      ds = database.ExecuteDataSet(dataSetCommand);
                }

                if (ds.Tables.Count > 0)
                    return ds.Tables[0];

                return new DataTable();
            }


     




        /// <summary>
        /// Builds a Gateway Status summary report for a given period
        /// </summary>
        /// <param name="fromDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <param name="authorize">True only give authorize/sales, False only Captures</param>
        /// <returns>A DataSet objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DataTable GetGatewayStatusByDate(DateTime fromDate, DateTime endDate, bool authorize, bool dateGroup, short groupby, short dateType)
        {

            Database database = Token.Instance.Database;
            DataSet ds = new DataSet();
            using (System.Data.Common.DbCommand dataSetCommand = database.GetStoredProcCommand("rpt_GatewaySummary"))
            {
                database.AddInParameter(dataSetCommand, "@StartDate", System.Data.DbType.DateTime, fromDate);
                database.AddInParameter(dataSetCommand, "@EndDate", System.Data.DbType.DateTime, endDate);
                database.AddInParameter(dataSetCommand, "@Authorize", System.Data.DbType.Boolean, authorize);
                database.AddInParameter(dataSetCommand, "@DateGroup", System.Data.DbType.Boolean, dateGroup);
                database.AddInParameter(dataSetCommand, "@groupby", System.Data.DbType.Int32, groupby);  //gateway instrument Gateway (0) and Instrument (1)
                database.AddInParameter(dataSetCommand, "@DateType", System.Data.DbType.Int32, dateType); //lst datatype authorize date (1), capture date 0
                // database.AddInParameter(dataSetCommand, "@paymentTypeId", System.Data.DbType.String, paymentTypeIds);



                ds = database.ExecuteDataSet(dataSetCommand);
            }

            if (ds.Tables.Count > 0)
                return ds.Tables[0];

            return new DataTable();
        }



        /// <summary>
        /// Builds a Gateway Status summary report for a given period
        /// </summary>
        /// <param name="fromDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <param name="authorize">True only give authorize/sales, False only Captures</param>
        /// <returns>A DataSet objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DataTable GetGatewayStatusByDate_New(DateTime fromDate, DateTime endDate, bool authorize, bool dateGroup, short groupby, short dateType)
        {

            Database database = Token.Instance.Database;
            DataSet ds = new DataSet();
            using (System.Data.Common.DbCommand dataSetCommand = database.GetStoredProcCommand("rpt_GatewaySummary_new"))
            {
                database.AddInParameter(dataSetCommand, "@StartDate", System.Data.DbType.DateTime, fromDate);
                database.AddInParameter(dataSetCommand, "@EndDate", System.Data.DbType.DateTime, endDate);
                database.AddInParameter(dataSetCommand, "@Authorize", System.Data.DbType.Boolean, authorize);
                database.AddInParameter(dataSetCommand, "@DateGroup", System.Data.DbType.Boolean, dateGroup);
                database.AddInParameter(dataSetCommand, "@groupby", System.Data.DbType.Int32, groupby);
                database.AddInParameter(dataSetCommand, "@DateType", System.Data.DbType.Int32, dateType);
                // database.AddInParameter(dataSetCommand, "@paymentTypeId", System.Data.DbType.String, paymentTypeIds);



                ds = database.ExecuteDataSet(dataSetCommand);
            }

            if (ds.Tables.Count > 0)
                return ds.Tables[0];

            return new DataTable();
        }
   

    }
}
