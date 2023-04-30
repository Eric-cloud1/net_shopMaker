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
        public static DataTable GetReportSchedule(Int32 userId)
        {
            DataSet ReportSchedule = new DataSet();
            Database database = Token.Instance.Database;

            string sql = string.Format(@"SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;SELECT r.ReportID, [Description], [Procedure], r.UserId, myReport = case when xr.ReportID IS NOT NULL then 1 else 0 end
FROM(SELECT xs.ReportID, xs.[Description] ,  xs.RoleIds , xs.[Procedure] , 
u.UserId, g.RoleId FROM ac_UserGroups u , xm_ReportsSchedule xs,ac_GroupRoles g
WHERE g.RoleId IN (select ParsedValue FROM udf_ParseDelimitedText(',',xs.RoleIds))
AND g.GroupId = u.GroupId) r
LEFT JOIN  Xm_ReportUsers xr on xr.UserId = r.UserId
WHERE r.UserId = @UserId");


            using (System.Data.Common.DbCommand dataSetCommand = database.GetSqlStringCommand(sql))
            {
                database.AddInParameter(dataSetCommand, "@UserId", System.Data.DbType.Int32, userId);
            
                ReportSchedule = database.ExecuteDataSet(dataSetCommand);
            }

            if (ReportSchedule.Tables.Count > 0)
                return ReportSchedule.Tables[0];

            return new DataTable();

        }


        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static void SaveSchedule(Int32 userId, Int32 reportId, Int16 scheduleId, bool add)
        {
            Database database = Token.Instance.Database;
            string sql = string.Empty;
            int result = 0;

            if (add == true)
            {
                sql = string.Format(@"SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;SELECT ReportID FROM Xm_ReportUsers 
WHERE ReportID = @ReporId AND UserId = @UserId;"); //AND ReportScheduleType = @ScheduleId 

                using (System.Data.Common.DbCommand dataSetCommand = database.GetSqlStringCommand(sql))
                {
                    database.AddInParameter(dataSetCommand, "@UserId", System.Data.DbType.Int32, userId);
                    database.AddInParameter(dataSetCommand, "@ReporId", System.Data.DbType.Int32, reportId);
                    result = AlwaysConvert.ToInt(database.ExecuteScalar(dataSetCommand));
                }
           
                if (result == 0)
                {
                    sql = string.Format(@"SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;INSERT INTO Xm_ReportUsers (ReportID, ReportScheduleTypeId, UserId)
VALUES(@ReporId,@ScheduleId, @UserId );SELECT @@ROWCOUNT; ");

                    using (System.Data.Common.DbCommand dataSetCommand = database.GetSqlStringCommand(sql))
                    {
                        database.AddInParameter(dataSetCommand, "@UserId", System.Data.DbType.Int32, userId);
                        database.AddInParameter(dataSetCommand, "@ReporId", System.Data.DbType.Int32, reportId);
                        database.AddInParameter(dataSetCommand, "@ScheduleId", System.Data.DbType.Int16, scheduleId);
                        result = AlwaysConvert.ToInt(database.ExecuteScalar(dataSetCommand));
                    }
                }
                else
                {
                    sql = string.Format(@"SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;UPDATE Xm_ReportUsers 
SET  ReportScheduleTypeId = @ScheduleId
WHERE  ReportID = @ReporId AND UserId=@UserId;SELECT @@ROWCOUNT; ");

                     using (System.Data.Common.DbCommand dataSetCommand = database.GetSqlStringCommand(sql))
                     {
                         database.AddInParameter(dataSetCommand, "@UserId", System.Data.DbType.Int32, userId);
                         database.AddInParameter(dataSetCommand, "@ReporId", System.Data.DbType.Int32, reportId);
                         database.AddInParameter(dataSetCommand, "@ScheduleId", System.Data.DbType.Int16, scheduleId);
                         result = AlwaysConvert.ToInt(database.ExecuteScalar(dataSetCommand));
                     }
                }
            }
            else
            {
                sql = string.Format(@"SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;DELETE FROM Xm_ReportUsers WHERE ReportID = @ReporId 
AND UserId = @UserId;SELECT @@ROWCOUNT; ");

                using (System.Data.Common.DbCommand dataSetCommand = database.GetSqlStringCommand(sql))
                {
                    database.AddInParameter(dataSetCommand, "@UserId", System.Data.DbType.Int32, userId);
                    database.AddInParameter(dataSetCommand, "@ReporId", System.Data.DbType.Int32, reportId);
                    database.AddInParameter(dataSetCommand, "@ScheduleId", System.Data.DbType.Int16, scheduleId);

                    result = AlwaysConvert.ToInt(database.ExecuteScalar(dataSetCommand));
                }
            }
        }
    }
}
