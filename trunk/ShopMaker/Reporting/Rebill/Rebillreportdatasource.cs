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
        public static DataTable GetRebillReport(DateTime fromDate, DateTime toDate, short groupby)
        {
            DataSet RebillReport = new DataSet();
            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand dataSetCommand = database.GetStoredProcCommand("rpt_RebillReport"))
            {
                database.AddInParameter(dataSetCommand, "@StartDate", System.Data.DbType.DateTime, fromDate);
                database.AddInParameter(dataSetCommand, "@EndDate", System.Data.DbType.DateTime, toDate);
                database.AddInParameter(dataSetCommand, "@groupby", System.Data.DbType.Int32, groupby);


                RebillReport = database.ExecuteDataSet(dataSetCommand);
            }

            return RebillReport.Tables[0];
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DataTable GetRebillAuthorizeReport(DateTime fromDate, DateTime toDate, short groupby)
        {
            DataSet RebillAuthorizeReport = new DataSet();
            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand dataSetCommand = database.GetStoredProcCommand("rpt_RebillAuthorize"))
            {
                database.AddInParameter(dataSetCommand, "@StartDate", System.Data.DbType.DateTime, fromDate);
                database.AddInParameter(dataSetCommand, "@EndDate", System.Data.DbType.DateTime, toDate);
                database.AddInParameter(dataSetCommand, "@groupby", System.Data.DbType.Int32, groupby);


                RebillAuthorizeReport = database.ExecuteDataSet(dataSetCommand);
            }

            return RebillAuthorizeReport.Tables[0];
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DataTable GetRebillShortAuthorizeReport(DateTime fromDate, DateTime toDate, short groupby)
        {
            DataSet RebillAuthorizeReport = new DataSet();
            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand dataSetCommand = database.GetStoredProcCommand("rpt_RebillAuthorize"))
            {
                database.AddInParameter(dataSetCommand, "@StartDate", System.Data.DbType.DateTime, fromDate);
                database.AddInParameter(dataSetCommand, "@EndDate", System.Data.DbType.DateTime, toDate);
                database.AddInParameter(dataSetCommand, "@groupby", System.Data.DbType.Int32, groupby);


                RebillAuthorizeReport = database.ExecuteDataSet(dataSetCommand);
            }

            return RebillAuthorizeReport.Tables[0];
        }




        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DataTable GetRebillCaptureReport(DateTime fromDate, DateTime toDate, short groupby)
        {
            DataSet RebillCaptureReport = new DataSet();
            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand dataSetCommand = database.GetStoredProcCommand("rpt_RebillCapture"))
            {
                database.AddInParameter(dataSetCommand, "@StartDate", System.Data.DbType.DateTime, fromDate);
                database.AddInParameter(dataSetCommand, "@EndDate", System.Data.DbType.DateTime, toDate);
                database.AddInParameter(dataSetCommand, "@groupby", System.Data.DbType.Int32, groupby);


                RebillCaptureReport = database.ExecuteDataSet(dataSetCommand);
            }

            return RebillCaptureReport.Tables[0];
        }


        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DataTable GetRebillShortCaptureReport(DateTime fromDate, DateTime toDate, short groupby)
        {
            DataSet RebillCaptureReport = new DataSet();
            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand dataSetCommand = database.GetStoredProcCommand("rpt_RebillCapture"))
            {
                database.AddInParameter(dataSetCommand, "@StartDate", System.Data.DbType.DateTime, fromDate);
                database.AddInParameter(dataSetCommand, "@EndDate", System.Data.DbType.DateTime, toDate);
                database.AddInParameter(dataSetCommand, "@groupby", System.Data.DbType.Int32, groupby);


                RebillCaptureReport = database.ExecuteDataSet(dataSetCommand);
            }

            return RebillCaptureReport.Tables[0];
        }

    }
}
