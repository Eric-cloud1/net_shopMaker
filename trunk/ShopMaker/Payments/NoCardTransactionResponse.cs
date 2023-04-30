using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace MakerShop.Payments
{
    /// <summary>
    /// Class that represents a request for payment Authorization
    /// </summary>
    public class NoCardTransactionResponse : BaseTransactionRequest
    {
        private TransactionOrigin _TransactionOrigin;
        private string _Response;
    
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payment">The Payment for this transaction</param>
        /// <param name="remoteIP">The remote IP of the user initiating the request</param>
        public NoCardTransactionResponse(Payment payment, string remoteIP, string response)
            : base(payment, remoteIP)
        {
            this._TransactionOrigin = TransactionOrigin.Internet;
            this._Response = response;
        }

        /// <summary>
        /// The origin of the transaction
        /// </summary>
        public TransactionOrigin TransactionOrigin
        {
            get { return _TransactionOrigin; }
            set { _TransactionOrigin = value; }
        }

        public string Response
        {
            get { return _Response; }
        }

        /// <summary>
        /// Type of transaction: i.e; Authorize
        /// </summary>
        public override TransactionType TransactionType
        {
            get { return TransactionType.AuthorizeCapture; }
        }
    }
}
