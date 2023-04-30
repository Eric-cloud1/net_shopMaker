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
    /// DataSource class for Payment objects
    /// </summary>
    [DataObject(true)]
    public partial class PaymentDataSource
    {
        /// <summary>
        /// This is used for the first order to setup the payments once the credit card info is received.
        /// </summary>
        /// <param name="p">Payment type with the credit card & order info</param>
        /// <param name="lastPaymentGatewayId">Incase the previous payments failed create new ones with different gateway</param>
        /// <param name="payments">The payments to process.</param>
        /// <returns>True no problems/ False issues</returns>
        public static bool SavePaymentInitial(Payment p, int? lastPaymentGatewayId, out List<Payment> payments)
        {
            payments = null;
            Database database = Token.Instance.Database;
            DataSet ds;
            using (DbCommand cmd = database.GetStoredProcCommand("wsp_Payment_Create_Initial"))
            {
                database.AddInParameter(cmd, "@OrderId", System.Data.DbType.Int32, p.OrderId);
                database.AddInParameter(cmd, "@PaymentInstrumentId", System.Data.DbType.Int16, p.PaymentInstrumentId);
                database.AddInParameter(cmd, "@EncryptedAccountData", System.Data.DbType.String, p.EncryptedAccountData);
                database.AddInParameter(cmd, "@ReferenceNumber", System.Data.DbType.String, p.ReferenceNumber);
                if (lastPaymentGatewayId.HasValue)
                    database.AddInParameter(cmd, "@LastPaymentGatewayId", System.Data.DbType.Int32, lastPaymentGatewayId.Value);

                 ds = database.ExecuteDataSet(cmd);
            }

            if (ds == null)
                return false;
            if (ds.Tables.Count == 0)
                return false;
            if (ds.Tables[0].Rows.Count == 0)
                return false;
            payments = new List<Payment>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                payments.Add(PaymentDataSource.Load(int.Parse(dr[0].ToString())));
            }
            return true;
        }
        
        /// <summary>
        /// This is used for the first order to setup the payments once the credit card info is received.
        /// </summary>
        /// <param name="p">Payment type with the credit card & order info, just used to move data around, not used as real object</param>
        /// <param name="lastPaymentGatewayId">Incase the previous payments failed create new ones with different gateway</param>
        /// <param name="payments">The payments to process.</param>
        /// <returns>True no problems/ False issues</returns>
        public static bool SavePaymentInitial(Payment p, int? lastPaymentGatewayId, bool Cart, out List<Payment> payments)
        {
            if (!Cart)
                return SavePaymentInitial(p, lastPaymentGatewayId, out payments);

            payments = null;
            Database database = Token.Instance.Database;
            DataSet ds;
            using (DbCommand cmd = database.GetStoredProcCommand("wsp_Payment_Create_Initial_Cart"))
            {
                database.AddInParameter(cmd, "@UserId", System.Data.DbType.Int32, p.Order.UserId);
                database.AddInParameter(cmd, "@PaymentInstrumentId", System.Data.DbType.Int16, p.PaymentInstrumentId);
                database.AddInParameter(cmd, "@EncryptedAccountData", System.Data.DbType.String, p.EncryptedAccountData);
                database.AddInParameter(cmd, "@ReferenceNumber", System.Data.DbType.String, p.ReferenceNumber);
                if (lastPaymentGatewayId.HasValue)
                    database.AddInParameter(cmd, "@LastPaymentGatewayId", System.Data.DbType.Int32, lastPaymentGatewayId.Value);

                ds = database.ExecuteDataSet(cmd);
            }

            if (ds == null)
                return false;
            if (ds.Tables.Count == 0)
                return false;
            if (ds.Tables[0].Rows.Count == 0)
                return false;
            payments = new List<Payment>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                payments.Add(PaymentDataSource.Load(int.Parse(dr[0].ToString())));
            }
            return true;
        }

        public static bool SavePaymentInitial(Payment p, string paymentGatewayPhone, out List<Payment> payments)
        {
            payments = null;
            Database database = Token.Instance.Database;
            DataSet ds;
            using (DbCommand cmd = database.GetStoredProcCommand("wsp_Payment_Create_Initial"))
            {
                database.AddInParameter(cmd, "@OrderId", System.Data.DbType.Int32, p.OrderId);
                database.AddInParameter(cmd, "@PaymentInstrumentId", System.Data.DbType.Int16, p.PaymentInstrumentId);
                database.AddInParameter(cmd, "@EncryptedAccountData", System.Data.DbType.String, p.EncryptedAccountData);
                database.AddInParameter(cmd, "@ReferenceNumber", System.Data.DbType.String, p.ReferenceNumber);
                database.AddInParameter(cmd, "@ProcessorPhone", System.Data.DbType.AnsiString, paymentGatewayPhone);

                ds = database.ExecuteDataSet(cmd);
            }

            if (ds == null)
                return false;
            if (ds.Tables.Count == 0)
                return false;
            if (ds.Tables[0].Rows.Count == 0)
                return false;
            payments = new List<Payment>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                payments.Add(PaymentDataSource.Load(int.Parse(dr[0].ToString())));
            }
            return true;
        }
        /// <summary>
        /// Loads a collection of Payment objects for the given OrderId and children from the database
        /// </summary>
        /// <param name="orderId">The given OrderId</param>
        /// <returns>A collection of Payment objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PaymentCollection LoadAllOrderFor(Int32 orderId)
        {
            return LoadAllOrderFor(orderId, 0, 0, string.Empty);
        }




        /// <summary>
        /// Loads a collection of Payment objects for the given OrderId and children from the database
        /// </summary>
        /// <param name="orderId">The given OrderId</param>
        /// <returns>A collection of Payment objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PaymentCollection LoadAllPaymentsByAffiliate(Int32 affiliateId)
        {
            return LoadAllOrderFor(affiliateId, 0, 0, string.Empty);
        }


        /// <summary>
        /// Loads a collection of Payment objects for the given OrderId and children from the database
        /// </summary>
        /// <param name="orderId">The given OrderId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Payment objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PaymentCollection LoadAllPaymentsByAffiliate(Int32 affiliateId, int maximumRows, int startRowIndex, string sortExpression)
        {
            //DEFAULT SORT EXPRESSION
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "PaymentDate DSC, PaymentTypeId ASC,  PaymentId ASC";
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT DISTINCT");
            if (maximumRows > 0) selectQuery.Append(" DISTINCT TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Payment.GetColumnNames("p"));
            //filter order for parent order
            selectQuery.Append(" FROM ac_Payments p JOIN ac_Orders o on o.OrderId = p.OrderId ");
            selectQuery.Append(" join en_PaymentTypes pt (NOLOCK) on pt.ParentTypeId = p.PaymentTypeId ");
            selectQuery.Append(" and pt.PaymentTypeDescription like '%commission%' ");
            selectQuery.Append(" where o.AffiliateId = @affiliateId");
            selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@affiliateId", System.Data.DbType.Int32, affiliateId);
            //EXECUTE THE COMMAND
            PaymentCollection results = new PaymentCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Payment payment = new Payment();
                        Payment.LoadDataReader(payment, dr);
                        results.Add(payment);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }



        /// <summary>
        /// Loads a collection of Payment objects for the given OrderId and children from the database
        /// </summary>
        /// <param name="orderId">The given OrderId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Payment objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PaymentCollection LoadAllOrderFor(Int32 orderId, int maximumRows, int startRowIndex, string sortExpression)
        {
            //DEFAULT SORT EXPRESSION
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "PaymentTypeId ASC, PaymentDate ASC, PaymentId ASC";
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT DISTINCT");
            if (maximumRows > 0) selectQuery.Append(" DISTINCT TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Payment.GetColumnNames("p"));
            //filter order for parent order
            selectQuery.Append(" FROM xm_OrderDetail od join xm_OrderDetail odP on od.ParentOrderId = odP.ParentOrderId ");
            selectQuery.Append(" JOIN ac_Orders o  ON o.OrderId = od.OrderId ");
            selectQuery.Append(" JOIN ac_Payments p on od.OrderId = p.OrderId");
            selectQuery.Append(" WHERE odP.OrderId = @orderId");
            selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@orderId", System.Data.DbType.Int32, orderId);
            //EXECUTE THE COMMAND
            PaymentCollection results = new PaymentCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Payment payment = new Payment();
                        Payment.LoadDataReader(payment, dr);
                        results.Add(payment);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

       
    }
    public enum PaymentTypes : short
    {
        UNKNOWN = 0,
        Initial = 1,
        Trial = 3,
        Recurring = 4,

        Commission = 20,
        Commission_Master_Company = 21,
        Commission_Company = 22,
        Commission_Master_Agent = 23,
        Commission_Agent = 24,
        Commission_Location = 25,
    }

}
