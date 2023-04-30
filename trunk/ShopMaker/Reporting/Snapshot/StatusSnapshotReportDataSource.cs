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

namespace MakerShop.Reporting.StatusSnapshot
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
        public static List<StatusSnapshotDetails> GetPaymentTypeSnapshotByDate(DateTime fromDate, int summary)
        {

            List<StatusSnapshotDetails> snapshotReport = new List<StatusSnapshotDetails>();
            DateTime toDate1 = DateTime.MinValue;
        
            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand updateCommand = database.GetStoredProcCommand("rpt_SnapShotReport"))
            {
                database.AddInParameter(updateCommand, "@StartDate", System.Data.DbType.DateTime, fromDate);
                database.AddInParameter(updateCommand, "@Summary", System.Data.DbType.Int32, summary);
              

                using (IDataReader dr = database.ExecuteReader(updateCommand))
                {
                    StatusSnapshotDetails snapshot = null;
                  
                    while (dr.Read())
                    {
                        snapshot = new StatusSnapshotDetails();
                        snapshot.InitialAuthorized = dr["InitialAuthorized"].ToString();
                        snapshot.InitialCancel = dr["InitialCancel"].ToString();
                        snapshot.InitialCaptured = dr["InitialCaptured"].ToString();
                        snapshot.InitialFail = dr["InitialFail"].ToString();
                        snapshot.InitialRefund = dr["InitialRefund"].ToString();
                        snapshot.InitialShipped = dr["InitialShipped"].ToString();
                        snapshot.InitialVoid = dr["InitialVoid"].ToString();
                        //
                        snapshot.RebillAuthorized = dr["RebillAuthorized"].ToString();
                        snapshot.RebillCancel = dr["RebillCancel"].ToString();
                        snapshot.RebillCaptured = dr["RebillCaptured"].ToString();
                        snapshot.RebillFail = dr["RebillFail"].ToString();
                        snapshot.RebillRefund = dr["RebillRefund"].ToString();
                        snapshot.RebillShipped = dr["RebillShipped"].ToString();
                        snapshot.RebillVoid = dr["RebillVoid"].ToString();
                        //
                        snapshot.TrialAuthorized = dr["TrialAuthorized"].ToString();
                        snapshot.TrialCancel = dr["TrialCancel"].ToString();
                        snapshot.TrialCaptured = dr["TrialCaptured"].ToString();
                        snapshot.TrialFail = dr["TrialFail"].ToString();
                        snapshot.TrialRefund = dr["TrialRefund"].ToString();
                        snapshot.TrialShipped = dr["TrialShipped"].ToString();
                        snapshot.TrialVoid = dr["TrialVoid"].ToString();


                        //result = 0;
                        //double.TryParse(dr["Successful"].ToString(), out result);
                        //chargeBack.Successful = result;

                        snapshotReport.Add(snapshot);

                    }
                    snapshot = null;
                    dr.Close();
                }
            }
            return snapshotReport;
        }

    }
}
