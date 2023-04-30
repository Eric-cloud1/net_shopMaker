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
        public static DataTable GetCustomerExport(DateTime fromDate, DateTime toDate, string reportype, string duplicateremoval, string affiliateId, string subAffiliate, bool subAffiliateMatch)
        {
            DataSet customerexport = new DataSet();
            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand dataSetCommand = database.GetStoredProcCommand("rpt_ExportCustomers"))
            {
                database.AddInParameter(dataSetCommand, "@OrderStartDate", System.Data.DbType.DateTime, fromDate);
                database.AddInParameter(dataSetCommand, "@OrderEndDate", System.Data.DbType.DateTime, toDate);
                database.AddInParameter(dataSetCommand, "@ReportTypes", System.Data.DbType.String, reportype);
                database.AddInParameter(dataSetCommand, "@DupeRemoval", System.Data.DbType.String, duplicateremoval);
                database.AddInParameter(dataSetCommand, "@Affiliate", System.Data.DbType.String, affiliateId);
                database.AddInParameter(dataSetCommand, "@Subaffiliate", System.Data.DbType.String, subAffiliate);
                database.AddInParameter(dataSetCommand, "@ExactSubaffiliate", System.Data.DbType.Int32, subAffiliateMatch);


                customerexport = database.ExecuteDataSet(dataSetCommand);
            }

            return customerexport.Tables[0];
        }

    }
}
