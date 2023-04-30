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
    /// <summary>
    /// DataSource class for various gateway reporting tasks
    /// </summary>
    public partial class ReportDataSource
    {

        /// <summary>
        /// Builds a product breakdown summary report for a given period
        /// </summary>
        /// <param name="fromDate">Inclusive start date to consider</param>
        /// <param name="toDate">Inclusive end date to consider</param>
        /// <param name="paymentGatewayId">If vendorId > 0 than includes products for this paymentGateway only. All gateways are included otherwise.</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A List of GatewayBreakdownSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<ChargeBack> GetChargeBackByDate(DateTime fromDate, DateTime toDate, int paymentInstrumentId)
        {

            List<ChargeBack> ChargeBackReport = new List<ChargeBack>();
            DateTime toDate1 = DateTime.MinValue;
        
            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand updateCommand = database.GetStoredProcCommand("rpt_MidChageBackStatus"))
            {
                database.AddInParameter(updateCommand, "@StartDate", System.Data.DbType.DateTime, fromDate);
                database.AddInParameter(updateCommand, "@EndDate", System.Data.DbType.DateTime, toDate);
                database.AddInParameter(updateCommand, "@paymentId", System.Data.DbType.Int32, paymentInstrumentId);

                using (IDataReader dr = database.ExecuteReader(updateCommand))
                {
                    ChargeBack chargeBack = null;
                    decimal result;
                    while (dr.Read())
                    {
                        chargeBack = new ChargeBack();
                        chargeBack.Name = dr["Name"].ToString();
                        result = 0;
                        decimal.TryParse(dr["Successful"].ToString(), out result);
                        chargeBack.Successful = result;
                        result = 0;
                        decimal.TryParse(dr["ChargeBacks"].ToString(), out result);
                        chargeBack.Chargeback = result;
                        result = 0;
                        decimal.TryParse(dr["auth"].ToString(), out result);
                        chargeBack.Authorized = result;
                        ChargeBackReport.Add(chargeBack);

                    }
                    chargeBack = null;
                    dr.Close();
                }
            }
            return ChargeBackReport;
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
        public static List<ChargeBack> GetVPlexChargeBackByDate(DateTime fromDate, DateTime toDate, int paymentInstrumentId)
        {

            List<ChargeBack> ChargeBackVPlexReport = new List<ChargeBack>();
            DateTime toDate1 = DateTime.MinValue;

            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand updateCommand = database.GetStoredProcCommand("rpt_GatewayChageBacks"))
            {
                database.AddInParameter(updateCommand, "@StartDate", System.Data.DbType.DateTime, fromDate);
                database.AddInParameter(updateCommand, "@EndDate", System.Data.DbType.DateTime, toDate);
                database.AddInParameter(updateCommand, "@paymentId", System.Data.DbType.Int32, paymentInstrumentId);

                using (IDataReader dr = database.ExecuteReader(updateCommand))
                {
                    ChargeBack chargeBack = null;
                    decimal result;
                    while (dr.Read())
                    {
                        chargeBack = new ChargeBack();
                        chargeBack.Name = dr["Name"].ToString();
                        result = 0;
                        decimal.TryParse(dr["Successful"].ToString(), out result);
                        chargeBack.Successful = result;
                        result = 0;
                        decimal.TryParse(dr["ChargeBacks"].ToString(), out result);
                        chargeBack.Chargeback = result;
                        result = 0;
                        decimal.TryParse(dr["auth"].ToString(), out result);
                        chargeBack.Authorized = result;
                        ChargeBackVPlexReport.Add(chargeBack);

                    }
                    chargeBack = null;
                    dr.Close();
                }
            }
            return ChargeBackVPlexReport;
        }

    }
}
