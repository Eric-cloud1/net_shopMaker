using System;
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
    /// DataSource class for Transaction objects in the database
    /// </summary>
    [DataObject(true)]
    public partial class TransactionDataSource
    {
        /// <summary>
        /// Counts the number of Transactions for given payment gateway Id and the provider transaction reference
        /// </summary>
        /// <param name="paymentGatewayId">Id of the payment gateway</param>
        /// <param name="providerTransactionId">Reference of the provider's transaction</param>
        /// <returns>The number of Transactions for given payment gateway Id and the provider transaction reference</returns>
        public static int CountForProviderTransaction(int paymentGatewayId, string providerTransactionId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM ac_Transactions WHERE PaymentGatewayId = @paymentGatewayId AND ProviderTransactionId = @providerTransactionId");
            database.AddInParameter(selectCommand, "@paymentGatewayId", System.Data.DbType.Int32, paymentGatewayId);
            database.AddInParameter(selectCommand, "@providerTransactionId", System.Data.DbType.String, providerTransactionId);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }


        // *CCBILL* The format of the function is wrong. should be GetTransactionId not getTransactionId. But it really should be called GetTransactionIdForProviderTransaction
        // also missing the SET commands & NOLOCK...please move this into a stored procedure wsp_GetTransactionIdByProviderTransaction
        // Summary is also wrong

        /// <summary>
        /// Counts the number of Transactions for given payment gateway Id and the provider transaction reference
        /// </summary>
        /// <param name="paymentGatewayId">Id of the payment gateway</param>
        /// <param name="providerTransactionId">Reference of the provider's transaction</param>
        /// <returns>The number of Transactions for given payment gateway Id and the provider transaction reference</returns>
        public static int getTransactionId(string Gatewayname, string providerTransactionId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(@"select TransactionId 
from ac_Transactions t 
WHERE ProviderTransactionId =@ProviderTransactionId
AND paymentgatewayid in (select paymentgatewayid from ac_paymentgateways where name like ''+ @gatewayPrefix +'%')");
            database.AddInParameter(selectCommand, "@ProviderTransactionId", System.Data.DbType.String, providerTransactionId);
            database.AddInParameter(selectCommand, "@gatewayPrefix", System.Data.DbType.String, Gatewayname);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of Transactions for given payment gateway Id and the provider transaction reference
        /// </summary>
        /// <param name="paymentGatewayId">Id of the payment gateway</param>
        /// <param name="providerTransactionId">Reference of the provider's transaction</param>
        /// <returns>The loaded collection of Transactions</returns>
        public static TransactionCollection LoadForProviderTransaction(int paymentGatewayId, string providerTransactionId)
        {
            TransactionCollection Transactions = new TransactionCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + Transaction.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Transactions");
            selectQuery.Append(" WHERE PaymentGatewayId = @paymentGatewayId");
            selectQuery.Append(" AND ProviderTransactionId = @providerTransactionId");
            selectQuery.Append(" ORDER BY TransactionDate");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@paymentGatewayId", System.Data.DbType.Int32, paymentGatewayId);
            database.AddInParameter(selectCommand, "@providerTransactionId", System.Data.DbType.String, providerTransactionId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    Transaction transaction = new Transaction();
                    Transaction.LoadDataReader(transaction, dr);
                    Transactions.Add(transaction);
                }
                dr.Close();
            }
            return Transactions;
        }

        /// <summary>
        /// Loads a collection of Transactions for given payment gateway Id and the provider transaction reference
        /// </summary>
        /// <param name="paymentGatewayId">Id of the payment gateway</param>
        /// <returns>The loaded collection of Transactions</returns>
        public static TransactionCollection LoadForProviderTransaction(string providerTransactionId)
        {
            TransactionCollection Transactions = new TransactionCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + Transaction.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Transactions");
            selectQuery.Append(" WHERE ProviderTransactionId = @providerTransactionId");
            selectQuery.Append(" ORDER BY TransactionDate");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@providerTransactionId", System.Data.DbType.String, providerTransactionId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    Transaction transaction = new Transaction();
                    Transaction.LoadDataReader(transaction, dr);
                    Transactions.Add(transaction);
                }
                dr.Close();
            }
            return Transactions;
        }
    }
}
