using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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


        public static DataTable GetDailyBillingReport(DateTime fromDate, DateTime toDate,  bool showzero,  short groupby)
        {
            Database database = Token.Instance.Database;
            DataSet DailyBillingReport = new DataSet();
            

            using (System.Data.Common.DbCommand dataSetCommand = database.GetStoredProcCommand("rpt_Daily_Billing"))
            {
                database.AddInParameter(dataSetCommand, "@StartDate", System.Data.DbType.DateTime, fromDate);
                database.AddInParameter(dataSetCommand, "@EndDate", System.Data.DbType.DateTime, toDate);
                database.AddInParameter(dataSetCommand, "@showzero", System.Data.DbType.Boolean, showzero);
                database.AddInParameter(dataSetCommand, "@groupby", System.Data.DbType.Int16, groupby);



                DailyBillingReport = database.ExecuteDataSet(dataSetCommand);
            }

            return DailyBillingReport.Tables[0];
        }

        public static DataTable GetDailyBillingShortReport(DateTime fromDate, DateTime toDate, bool showzero, short groupby)
        {
            Database database = Token.Instance.Database;
            DataSet DailyBillingReport = new DataSet();


            using (System.Data.Common.DbCommand dataSetCommand = database.GetStoredProcCommand("rpt_Daily_Billing"))
            {
                database.AddInParameter(dataSetCommand, "@StartDate", System.Data.DbType.DateTime, fromDate);
                database.AddInParameter(dataSetCommand, "@EndDate", System.Data.DbType.DateTime, toDate);
                database.AddInParameter(dataSetCommand, "@showzero", System.Data.DbType.Boolean, showzero);
                database.AddInParameter(dataSetCommand, "@groupby", System.Data.DbType.Int16, groupby);



                DailyBillingReport = database.ExecuteDataSet(dataSetCommand);
            }

            return DailyBillingReport.Tables[0];
        }

 
    }
}
