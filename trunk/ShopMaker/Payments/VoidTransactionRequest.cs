using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Common;

namespace MakerShop.Payments
{
    /// <summary>
    /// Class that represents a request to void a previously authorized transaction
    /// </summary>
    public class VoidTransactionRequest : BaseTransactionRequest
    {
        private Transaction _AuthorizeTransaction;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payment">The Payment object associated with this request</param>
        /// <param name="remoteIP">Remote IP of the user initiating the request</param>
        public VoidTransactionRequest(Payment payment, string remoteIP) : base (payment, remoteIP)
        {
            this._AuthorizeTransaction = payment.Transactions.LastAuthorization;
            if (payment.Transactions.Count > 0)
            {
                LSDecimal authorizations = payment.Transactions.GetTotalAuthorized();
                LSDecimal captures = payment.Transactions.GetTotalCaptured();
                this.Amount = (authorizations - captures);
            }
        }

        /// <summary>
        /// Authorize transaction to be voided
        /// </summary>
        public Transaction AuthorizeTransaction
        {
            get { return _AuthorizeTransaction; }
        }

        /// <summary>
        /// Type of transaction: i.e; Void
        /// </summary>
        public override TransactionType TransactionType
        {
            get { return TransactionType.Void; }
        }

    }
}
