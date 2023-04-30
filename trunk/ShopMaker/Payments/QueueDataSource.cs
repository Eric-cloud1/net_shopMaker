using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;


namespace MakerShop.Payments
{
    /// <summary>
    /// DataSource class for Queue objects
    /// </summary>
    public partial class QueueDataSource
    {
        /// <summary>
        /// Get all active queues in the system
        /// </summary>
        public static List<string> List()
        {
            List<string> ls = new List<string>();
            Database database = Token.Instance.Database;
            using (System.Data.Common.DbCommand cmd = database.GetStoredProcCommand("wsp_QueueList"))
            {
                using (IDataReader dr = database.ExecuteReader(cmd))
                {
                    while (dr.Read())
                    {
                        ls.Add(dr.GetString(0));
                    }
                    dr.Close();
                }
            }
            return ls;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Queue"></param>
        /// <returns>Table 1 is summary; Table 2 is details</returns>
        public static DataSet Report(string Queue, bool SummaryOnly, DateTime? EndDate)
        {
            List<string> ls = new List<string>();
            Database database = Token.Instance.Database;
            DataSet ds = null;

            using (DbCommand cmd = database.GetStoredProcCommand("wsp_QueueTransactions"))
            {
                if (!SummaryOnly) cmd.CommandTimeout = 1000 * 1200;

                database.AddInParameter(cmd, "@Queue", System.Data.DbType.AnsiString, Queue);
                database.AddInParameter(cmd, "@SummaryOnly", System.Data.DbType.Boolean, SummaryOnly);
                if (EndDate.HasValue)
                    database.AddInParameter(cmd, "@EndDate", System.Data.DbType.DateTime, EndDate.Value);
                ds = database.ExecuteDataSet(cmd);
            }
            return ds;
        }

        /// <summary>
        /// Clear out / Reset a queue, will remove all payment references to a queue
        /// </summary>
        /// <param name="Queue">Name of the queue</param>
        public static void Reset(string Queue)
        {
            Database database = Token.Instance.Database;

            using (DbCommand cmd = database.GetStoredProcCommand("wsp_QueueReset"))
            {
                database.AddInParameter(cmd, "@Queue", System.Data.DbType.AnsiString, Queue);
                database.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Update queue
        /// </summary>
        /// <param name="Queue">Name of the queue</param>
        public static void Update(string toQueue, string PaymentIds, DateTime PaymentDate)
        {
            Database database = Token.Instance.Database;

            using (DbCommand cmd = database.GetStoredProcCommand("wsp_QueueUpdate"))
            {
                database.AddInParameter(cmd, "@toQueue", System.Data.DbType.AnsiString, toQueue);
                database.AddInParameter(cmd, "@PaymentIds", System.Data.DbType.AnsiString, PaymentIds);
                database.AddInParameter(cmd, "@PaymentDate", System.Data.DbType.DateTime, PaymentDate);
                database.AddInParameter(cmd, "@CaptureMove", System.Data.DbType.Int32, false);
                database.ExecuteNonQuery(cmd);
            }
        }
        /// <summary>
        /// move queue
        /// </summary>
        /// <param name="Queue">Name of the queue</param>
        public static void Move(string fromQueue, string toQueue, int quantity, DateTime PaymentDate)
        {
            Database database = Token.Instance.Database;

            using (DbCommand cmd = database.GetStoredProcCommand("wsp_QueueUpdate"))
            {
                database.AddInParameter(cmd, "@fromQueue", System.Data.DbType.AnsiString, fromQueue);
                database.AddInParameter(cmd, "@toQueue", System.Data.DbType.AnsiString, toQueue);
                database.AddInParameter(cmd, "@quantity", System.Data.DbType.Int32, quantity);
                database.AddInParameter(cmd, "@PaymentDate", System.Data.DbType.DateTime, PaymentDate);
                database.AddInParameter(cmd, "@CaptureMove", System.Data.DbType.Int32, false);
                database.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// move queue
        /// </summary>
        /// <param name="Queue">Name of the queue</param>
        public static void MoveCapture(string fromQueue, string toQueue, DateTime PaymentDate)
        {
            Database database = Token.Instance.Database;

            using (DbCommand cmd = database.GetStoredProcCommand("wsp_QueueUpdate"))
            {
                database.AddInParameter(cmd, "@fromQueue", System.Data.DbType.AnsiString, fromQueue);
                database.AddInParameter(cmd, "@toQueue", System.Data.DbType.AnsiString, toQueue);
                database.AddInParameter(cmd, "@PaymentDate", System.Data.DbType.DateTime, PaymentDate);
                database.AddInParameter(cmd, "@CaptureMove", System.Data.DbType.Int32, true);
                database.ExecuteNonQuery(cmd);
            }
        }
    }

}
