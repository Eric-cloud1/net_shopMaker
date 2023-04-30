using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;

namespace MakerShop.Stores
{
    /// <summary>
    /// DataSource class for AuditEvent objects
    /// </summary>
    [DataObject(true)]
    public partial class AuditEventDataSource
    {
        /// <summary>
        /// Clears the audit event log
        /// </summary>
        public static void ClearLog()
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("DELETE FROM ac_AuditEvents WHERE StoreId = @storeId");
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.ExecuteScalar(selectCommand);
            Logger.Audit(AuditEventType.ClearAuditLog, true, string.Empty);
        }

        public static void AuditInfoBegin(string ip)
        {
            Database database = Token.Instance.Database;
            if (database.BeginAuditTransaction())
            {
                DbCommand cmd = database.GetStoredProcCommand("AuditDatabase_SetSessionInfo");
                if ((Token.Instance != null) && (Token.Instance.User != null))
                    database.AddInParameter(cmd, "@ApplicationUser", DbType.String, Token.Instance.User.UserName);
                else
                    database.AddInParameter(cmd, "@ApplicationUser", DbType.String, null);
                database.AddInParameter(cmd, "@LogicalAddress", DbType.String, ip);
                database.ExecuteNonQuery(cmd);
            }
        }
        public static void AuditInfoEnd()
        {

            Token.Instance.Database.EndAuditTransaction();
        }
    }
}
