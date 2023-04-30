using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Common;
using MakerShop.Utility;

namespace MakerShop.Payments
{
    /// <summary>
    /// A class that represents a refund request
    /// </summary>
    public class RefundTransactionRequest : BaseTransactionRequest
    {
        private Transaction _CaptureTransaction;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payment">The Payment object</param>
        /// <param name="remoteIP">Remote Ip of the user initiating the request</param>
        public RefundTransactionRequest(Payment payment, string remoteIP) : base (payment, remoteIP)
        {
            if (!string.IsNullOrEmpty(payment.AccountData))
            {
                AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);
                this._CardNumber = accountData.GetValue("AccountNumber");
                this._ExpirationMonth = AlwaysConvert.ToInt(accountData.GetValue("ExpirationMonth"));
                this._ExpirationYear = AlwaysConvert.ToInt(accountData.GetValue("ExpirationYear"));
            }
            this._CaptureTransaction = payment.Transactions.LastCapture;
        }

        private string _CardNumber;
        /// <summary>
        /// Card Number
        /// </summary>
        public string CardNumber
        {
            get { return _CardNumber; }
            set { _CardNumber = value; }
        }

        private int _ExpirationMonth;
        /// <summary>
        /// Expiration month
        /// </summary>
        public int ExpirationMonth
        {
            get { return _ExpirationMonth; }
            set { _ExpirationMonth = value; }
        }

        private int _ExpirationYear;
        /// <summary>
        /// Expiration year
        /// </summary>
        public int ExpirationYear
        {
            get { return _ExpirationYear; }
            set { _ExpirationYear = value; }
        }

        /// <summary>
        /// The original capture transaction that is to be refunded
        /// </summary>
        public Transaction CaptureTransaction
        {
            get { return _CaptureTransaction; }
        }

        private LSDecimal _LastAmount = 0;
        private TransactionType _TransactionType = TransactionType.Refund;
        /// <summary>
        /// Transaction Type. Will either be TransactionType.Refund or TransactionType.PartialRefund
        /// </summary>
        public override TransactionType TransactionType
        {
            get 
            { 
                //CHECK TO SEE IF AMOUNT IS CHANGED SINCE LAST CALCULATION
                if (this.Amount != _LastAmount)
                {
                    //DETERMINE IF THIS IS A PARTIAL REFUND
                    LSDecimal captureAmount = this.Payment.Transactions.GetTotalCaptured();
                    _TransactionType = (this.Amount >= captureAmount) ? TransactionType.Refund : TransactionType.PartialRefund;
                    _LastAmount = this.Amount;
                }
                return _TransactionType;
            }
        }

        /// <summary>
        /// Name-Value pairs containg additional configuration properties
        /// </summary>
        public Dictionary<string, string> ExtendedProperties
        {
            get
            {
                return base.ExtendedProperties;
            }
        }


    }
}
