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

namespace MakerShop.Reporting.Continuity
{
    public partial class ReportDataSource
    {

        /// <summary>
        /// Builds a Conversion Performance summary report for a given period
        /// </summary>
        /// <param name="fromDate">Inclusive start date to consider</param>
        /// <param name="toDate">Inclusive end date to consider</param>
        /// <param name="promoCode">from ac_Affiliates</param>
        /// <param name="subAffiliate">from Order Table</param>
        /// <param name="ExactSubaffiliate">Match subaffiliate</param>
        /// <returns>A DataSet objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DataTable GetConversionsPerformanceByDatePromoSubAffiliate(DateTime fromDate, DateTime endDate,
            string affiliateId,  string subAffiliate, int subAffiliateMatch)
        {

            DataSet ConversionReport = new DataSet();
            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand dataSetCommand = database.GetStoredProcCommand("rpt_ConversionPerformance"))
            {
                database.AddInParameter(dataSetCommand, "@StartDate", System.Data.DbType.DateTime, fromDate);
                database.AddInParameter(dataSetCommand, "@EndDate", System.Data.DbType.DateTime, endDate);
                database.AddInParameter(dataSetCommand, "@Affiliate", System.Data.DbType.String, affiliateId);
                database.AddInParameter(dataSetCommand, "@Subaffiliate", System.Data.DbType.String, subAffiliate);
                database.AddInParameter(dataSetCommand, "@ExactSubaffiliate", System.Data.DbType.Int32, subAffiliateMatch);


                ConversionReport = database.ExecuteDataSet(dataSetCommand);
            }

            if (ConversionReport.Tables.Count > 0)
                return ConversionReport.Tables[0];

            return new DataTable();
        }

    }
}
