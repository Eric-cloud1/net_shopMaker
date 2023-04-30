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

namespace MakerShop.Orders.ChargeBack
{
    /// <summary>
    /// DataSource class for various gateway reporting tasks
    /// </summary>
    public partial class ReportDataSource
    {

        //TODO: replace
        /// <summary>
        /// Builds a product breakdown summary report for a given period
        /// </summary>
        /// <param name="fromDate">Inclusive start date to consider</param>
        /// <param name="toDate">Inclusive end date to consider</param>
        /// <param name="paymentGatewayId">If vendorId > 0 than includes products for this paymentGateway only. All gateways are included otherwise.</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A List of GatewayBreakdownSummary objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<ChargeBackDetails> GetChargeBacDetailskByDate(DateTime fromDate, DateTime toDate, string sort)
        {

            List<ChargeBackDetails> ChargeBackReport = new List<ChargeBackDetails>();
            DateTime toDate1 = DateTime.MinValue;
        
            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand updateCommand = database.GetStoredProcCommand("rpt_MidChageBackStatus"))
            {
                database.AddInParameter(updateCommand, "@StartDate", System.Data.DbType.DateTime, fromDate);
                database.AddInParameter(updateCommand, "@EndDate", System.Data.DbType.DateTime, toDate);
                database.AddInParameter(updateCommand, "@sortby", System.Data.DbType.String, sort);

                using (IDataReader dr = database.ExecuteReader(updateCommand))
                {
                    ChargeBackDetails chargeBack = null;
                    double result;
                    while (dr.Read())
                    {
                        chargeBack = new ChargeBackDetails();
                        //chargeBack.Name = dr["Name"].ToString();
                        //result = 0;
                        //double.TryParse(dr["Successful"].ToString(), out result);
                        //chargeBack.Successful = result;
                        //result = 0;
                        //double.TryParse(dr["ChargeBacks"].ToString(), out result);
                        //chargeBack.Chargeback = result;
                        //result = 0;
                        //double.TryParse(dr["auth"].ToString(), out result);
                        //chargeBack.Authorized = result;
                        ChargeBackReport.Add(chargeBack);

                    }
                    chargeBack = null;
                    dr.Close();
                }
            }
            return ChargeBackReport;
        }

    }
}
