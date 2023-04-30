using System;
using System.Data;
using System.Data.Common;
using MakerShop.Common;
using MakerShop.Payments.Providers;
using MakerShop.Stores;
using MakerShop.Utility;
using MakerShop.Data;
using System.Text;

namespace MakerShop.Payments
{
    public partial class Payment
    {
        /// <summary>
        /// Decrypted account data
        /// </summary>
        public string AccountData
        {
            get { return EncryptionHelper.DecryptAES(this.EncryptedAccountData); }
            set { this.EncryptedAccountData = EncryptionHelper.EncryptAES(value); }
        }

        /// <summary>
        /// Indicates whether data is present.
        /// </summary>
        public bool HasAccountData
        {
            get { return (!string.IsNullOrEmpty(this.EncryptedAccountData)); }
        }

        /// <summary>
        /// Indicates the status of this payment.
        /// </summary>
        public PaymentStatus PaymentStatus
        {
            get { return (PaymentStatus)this.PaymentStatusId; }
            set { this.PaymentStatusId = (short)value; }
        }

        /// <summary>
        /// Indicates the paymentInstrument of this payment.
        /// </summary>
        public PaymentInstrument PaymentInstrument
        {
            get { return (PaymentInstrument)this.PaymentInstrumentId; }
            set { this.PaymentInstrumentId = (short)value; }
        }

        /// <summary>
        /// Indicates whether the payment is in a failed state.
        /// </summary>
        public bool IsFailed
        {
            get
            {
                return (this.PaymentStatus == PaymentStatus.AuthorizationFailed ||
                    this.PaymentStatus == PaymentStatus.CaptureFailed);
                //|| this.PaymentStatus == PaymentStatus.RefundFailed ||
                //this.PaymentStatus == PaymentStatus.VoidFailed);
            }
        }

        /// <summary>
        /// Indicates whether the payment is in a voidable state.
        /// </summary>
        public bool IsVoidable
        {
            get { return (PaymentStatusHelper.IsVoidable(this.PaymentStatus)); }
        }

        /// <summary>
        /// Indicates whether the payment is in a pending state.
        /// </summary>
        public bool IsPending
        {
            get
            {
                return (this.PaymentStatus == PaymentStatus.AuthorizationPending ||
                    this.PaymentStatus == PaymentStatus.CapturePending ||
                    this.PaymentStatus == PaymentStatus.RefundPending ||
                    this.PaymentStatus == PaymentStatus.VoidPending);
            }
        }

        /// <summary>
        /// Saves this payment object to the database
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save()
        {
            return Save(true);
        }

        /// <summary>
        /// Saves this payment object to the database
        /// <param name="triggerEvents">If true events are triggered on change of associated order's payment status</param>
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save(bool triggerEvents)
        {
            //save the payment method name for future reference
            if (string.IsNullOrEmpty(this.PaymentMethodName) && (this.PaymentMethod != null))
            {
                this.PaymentMethodName = this.PaymentMethod.Name;
            }
            //Check whether the payment is in a finalized state
            if (this.PaymentStatus == PaymentStatus.Captured || this.PaymentStatus == PaymentStatus.Completed || this.PaymentStatus == PaymentStatus.Refunded || this.PaymentStatus == PaymentStatus.Void)
            {
                //Payment is finalized.  Start tracking the date of completion if not done already.
                if (this.CompletedDate == DateTime.MinValue)
                {
                    this.CompletedDate = LocaleHelper.LocalNow;
                }
            }
            else
            {
                //this payment is not finalized, make sure a completion date is not set
                this.CompletedDate = DateTime.MinValue;
            }
            //get store settings to process payment
            StoreSettingCollection storeSettings = Store.GetCachedSettings();
            //if the payment has been completed, calculate whether the lifespan is elapsed
            if (this.CompletedDate > DateTime.MinValue)
            {
                //get the payment lifepsan
                Store store = Token.Instance.Store;
                if (LocaleHelper.LocalNow >= this.CompletedDate.Add(new TimeSpan(storeSettings.PaymentLifespan, 0, 0, 0)))
                {
                    //the payment lifespan is elapsed, remove any account data
                    //  this.AccountData = string.Empty;
                }
            }
            //ENSURE THAT CREDIT CARDS RESPECT CARD STORAGE SETTING
            if ((!string.IsNullOrEmpty(this.EncryptedAccountData)) && (this.PaymentMethod != null)
                && (this.PaymentMethod.IsCreditOrDebitCard()) && (!storeSettings.EnableCreditCardStorage))
            {
                // ENCRYPTIONXYZ this.EncryptedAccountData = string.Empty;
            }
            //ENSURE THAT CARD SECURITY CODE IS NEVER STORED
            if (!string.IsNullOrEmpty(this.EncryptedAccountData) && (this.AccountData.ToLowerInvariant().Contains("securitycode")))
            {
                AccountDataDictionary accountData = new AccountDataDictionary(this.AccountData);
                // ENCRYPTIONXYZ    accountData.Remove("SecurityCode");
                this.AccountData = accountData.ToString();
            }
            //ACTIVATE SUBSCRIPTIONS IF THIS PAYMENT IS COMPLETED (ARB PAYMENT)
            if ((this.PaymentStatus == PaymentStatus.Completed) && (this.Subscription != null) && (!this.Subscription.IsActive))
            {
                this.Subscription.Activate();
            }
            //CALL THE BASE SAVE METHOD
            SaveResult result = BaseSave();
            //TRIGGER UPDATE OF ORDER TOTAL PAYMENTS
            this.Order.RecalculatePaymentStatus(triggerEvents);
            //RETURN THE RESULT
            return result;
        }

        /// <summary>
        /// Updates payment status of this payment to the given status
        /// </summary>
        /// <param name="status">The payment status to set</param>
        public void UpdateStatus(PaymentStatus status)
        {
            string updateQuery = "UPDATE ac_Payments SET PaymentStatusId = @paymentStatusId WHERE PaymentId = @paymentId";
            Database database = Token.Instance.Database;
            using (System.Data.Common.DbCommand updateCommand = database.GetSqlStringCommand(updateQuery))
            {
                database.AddInParameter(updateCommand, "@paymentStatusId", System.Data.DbType.Int16, (short)status);
                database.AddInParameter(updateCommand, "@paymentId", System.Data.DbType.Int32, this.PaymentId);
                int recordsAffected = database.ExecuteNonQuery(updateCommand);
                if (recordsAffected > 0) _PaymentStatusId = (short)status;
            }
        }
        #region Update

        /// <summary>
        /// Updates this payment
        /// </summary>
        /// <param name="response">the string back from the gateway to use as the result</param>
        public virtual void Update(string response)
        {
            this.Update(false, response);
        }

        /// <summary>
        /// Authorizes this payment
        /// </summary>
        /// <param name="async">If <b>true</b> payment is authorized asynchronously</param>
        public virtual void Update(bool async, string response)
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            if (context == null)
            {
                if (string.IsNullOrEmpty(this.Order.RemoteIP))
                    throw new ArgumentException("You must specify remoteIP when HttpContext.Current is null.", "remoteIP");
                else
                    this.Update(async, this.Order.RemoteIP, response);

            }
            else
                this.Update(async, context.Request.UserHostAddress, response);
        }

        /// <summary>
        /// Update this payment
        /// </summary>
        /// <param name="async">If <b>true</b> payment is Updated asynchronously</param>
        /// <param name="remoteIP">Remote IP of the user initiating the request</param>
        public virtual void Update(bool async, string remoteIP, string response)
        {
            if (SubscriptionId == 0)
            {
                NoCardTransactionResponse nctResponse = new NoCardTransactionResponse(this, remoteIP, response);
                if (!async) PaymentEngine.DoUpdate(nctResponse);
                else PaymentEngine.AsyncDoUpdate(nctResponse);

            }
            else//should never be used
            {
                //AuthorizeRecurringTransactionRequest request = new AuthorizeRecurringTransactionRequest(this, this.Subscription.SubscriptionPlan, remoteIP);
                //PaymentEngine.DoAuthorizeRecurring(request);

            }

        }
        #endregion
        #region Authorize
        /// <summary>
        /// Authorizes this payment
        /// </summary>
        public virtual void Authorize()
        {
            this.Authorize(false);
        }

        /// <summary>
        /// Authorizes this payment
        /// </summary>
        /// <param name="async">If <b>true</b> payment is authorized asynchronously</param>
        public virtual void Authorize(bool async)
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            if (context == null)
            {
                if (string.IsNullOrEmpty(this.Order.RemoteIP))
                    throw new ArgumentException("You must specify remoteIP when HttpContext.Current is null.", "remoteIP");
                else
                    this.Authorize(async, this.Order.RemoteIP);

            }
            else
                this.Authorize(async, context.Request.UserHostAddress);
        }

        /// <summary>
        /// Authorizes this payment
        /// </summary>
        /// <param name="async">If <b>true</b> payment is authorized asynchronously</param>
        /// <param name="remoteIP">Remote IP of the user initiating the request</param>
        public virtual void Authorize(bool async, string remoteIP)
        {
            if (SubscriptionId == 0)
            {
                AuthorizeTransactionRequest request = new AuthorizeTransactionRequest(this, remoteIP);
                if (!async) PaymentEngine.DoAuthorize(request);
                else PaymentEngine.AsyncDoAuthorize(request);

            }
            else
            {
                AuthorizeRecurringTransactionRequest request = new AuthorizeRecurringTransactionRequest(this, this.Subscription.SubscriptionPlan, remoteIP);
                PaymentEngine.DoAuthorizeRecurring(request);

            }

        }
        #endregion
        /// <summary>
        /// Captures this payment
        /// </summary>
        public virtual void Capture()
        {

            this.Capture(this.Amount, true, false, this.Order.RemoteIP);
        }
        /// <summary>
        /// Captures this payment
        /// </summary>
        /// <param name="amount">The amount to capture</param>
        public virtual void Capture(LSDecimal amount)
        {

            this.Capture(amount, true);
        }

        /// <summary>
        /// Captures this payment
        /// </summary>
        /// <param name="amount">The amount to capture</param>
        /// <param name="final">If <b>true</b> this capture is considered to be the final capture</param>
        public virtual void Capture(LSDecimal amount, bool final)
        {
            this.Capture(amount, final, false);
        }

        /// <summary>
        /// Captures this payment
        /// </summary>
        /// <param name="amount">The amount to capture</param>
        /// <param name="final">If <b>true</b> this capture is considered to be the final capture</param>
        /// <param name="async">If <b>true</b> payment is captured asynchronously</param>
        public virtual void Capture(LSDecimal amount, bool final, bool async)
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            if (context == null) throw new ArgumentException("You must specify remoteIP when HttpContext.Current is null.", "remoteIP");
            this.Capture(amount, final, async, context.Request.UserHostAddress);
        }

        /// <summary>
        /// Captures this payment
        /// </summary>
        /// <param name="amount">The amount to capture</param>
        /// <param name="final">If <b>true</b> this capture is considered to be the final capture</param>
        /// <param name="async">If <b>true</b> payment is captured asynchronously</param>
        /// <param name="remoteIP">Remote IP of the user initiating the request</param>
        public virtual void Capture(LSDecimal amount, bool final, bool async, string remoteIP)
        {
            CaptureTransactionRequest request = new CaptureTransactionRequest(this, remoteIP);
            request.Amount = amount;
            request.IsFinal = final;
            if (!async) PaymentEngine.DoCapture(request);
            else PaymentEngine.AsyncDoCapture(request);


        }

        /// <summary>
        /// Refunds this payment
        /// </summary>
        /// <param name="remoteIp">Remote IP of the user initiating the request</param>
        public virtual void Refund(string remoteIp)
        {
            RefundTransactionRequest request = new RefundTransactionRequest(this, remoteIp);
            PaymentEngine.DoRefund(request);
        }

        /// <summary>
        /// Voids this payment
        /// </summary>
        public virtual void Void()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            if (context == null) throw new ArgumentException("You must specify remoteIP when HttpContext.Current is null.", "remoteIP");
            this.Void(context.Request.UserHostAddress);
        }

        /// <summary>
        /// Voids this payment
        /// </summary>
        /// <param name="remoteIP">Remote IP of the user initiating the request</param>
        public virtual void Void(string remoteIP)
        {
            VoidTransactionRequest request = new VoidTransactionRequest(this, remoteIP);
            PaymentEngine.DoVoid(request);
        }


        /// <summary>
        /// Creates a reference number from the given credit card number
        /// </summary>
        /// <param name="accountNumber">The credit card number to create reference for</param>
        /// <returns>A reference number from the given credit card number</returns>
        public static string GenerateReferenceNumber(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber)) return string.Empty;
            int length = accountNumber.Length;
            if (length < 5) return accountNumber;
            return ("x" + accountNumber.Substring((length - 4)));
        }

        public PaymentTypes PaymentType
        {
            set
            {
                this.PaymentTypeId = (short)value;
            }
            get
            {
                try
                {
                    return (PaymentTypes)this.PaymentTypeId;
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Updates productId to set the payment method. Should only be used for the first charge with value.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="paymentInstrument"></param>
        /// <param name="lastPaymentGatewayId">If you want a different paymentgateway than the one given pass the unwanted paymentgateway here</param>
        /// <returns></returns>

        public bool SetPaymentGatewayByProduct(int productId, PaymentTypes paymentType, PaymentInstrument paymentInstrument, int? lastPaymentGatewayId)
        {
            this.PaymentType = paymentType;
            this.PaymentInstrumentId = (short)paymentInstrument;
            this.PaymentMethodId = 0;
            this.PaymentGatewayId = 0;

            Database database = Token.Instance.Database;
            using (System.Data.Common.DbCommand cmd = database.GetStoredProcCommand("wsp_GetPaymentGatewayByProduct"))
            {
                database.AddInParameter(cmd, "@ProductID", System.Data.DbType.Int32, productId);
                database.AddInParameter(cmd, "@PaymentInstrumentId", System.Data.DbType.Int16, (short)paymentInstrument);

                if (lastPaymentGatewayId.HasValue)
                    database.AddInParameter(cmd, "@LastPaymentGatewayId", System.Data.DbType.Int32, lastPaymentGatewayId.Value);
                database.AddOutParameter(cmd, "@PaymentGatewayId", System.Data.DbType.Int32, 4);
                database.AddOutParameter(cmd, "@PaymentMethodId", System.Data.DbType.Int32, 4);
                database.ExecuteNonQuery(cmd);
                if (cmd.Parameters["@PaymentMethodId"].Value == DBNull.Value)
                    PaymentMethodId = 0;
                else
                    PaymentMethodId = (int)cmd.Parameters["@PaymentMethodId"].Value;
                if (cmd.Parameters["@PaymentGatewayId"].Value == DBNull.Value)
                    PaymentGatewayId = 0;
                else
                    PaymentGatewayId = (int)cmd.Parameters["@PaymentGatewayId"].Value;
            }
            /*
            using (System.Data.Common.DbCommand updateCommand = database.GetStoredProcCommand("wsp_ProductGateway"))
            {
                database.AddInParameter(updateCommand, "@ProductID", System.Data.DbType.Int32, productId);
                database.AddInParameter(updateCommand, "@PaymentTypeId", System.Data.DbType.Int16, (short)paymentType);
                database.AddInParameter(updateCommand, "@PaymentInstrumentId", System.Data.DbType.Int16, (short)paymentInstrument);
                database.AddOutParameter(updateCommand, "@PaymentMethodId", System.Data.DbType.Int32, 4);
                database.AddOutParameter(updateCommand, "@PaymentGatewayId", System.Data.DbType.Int32, 4);
                database.ExecuteNonQuery(updateCommand);
                if (updateCommand.Parameters["@PaymentMethodId"].Value == DBNull.Value)
                    PaymentMethodId = 0;
                else
                    PaymentMethodId = (int)updateCommand.Parameters["@PaymentMethodId"].Value;
                if (updateCommand.Parameters["@PaymentGatewayId"].Value == DBNull.Value)
                    PaymentGatewayId = 0;
                else
                    PaymentGatewayId = (int)updateCommand.Parameters["@PaymentGatewayId"].Value;
            }
            */

            if (PaymentMethodId == 0)
                throw new Exception("Payment Method not specified for product.");
            if (PaymentGatewayId == 0)
                throw new Exception("Gateway not specified for product.");
            if ((PaymentMethodId == -1) || (PaymentGatewayId == -1))
                return false;
            return true;
        }
        public void SetPaymentGatewayByProcessorPhone(string ProcessorPhone, PaymentTypes paymentType, PaymentInstrument paymentInstrument)
        {
            this.PaymentType = paymentType;
            this.PaymentInstrumentId = (short)paymentInstrument;
            this.PaymentMethodId = 0;
            this.PaymentGatewayId = 0;

            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand updateCommand = database.GetStoredProcCommand("wsp_GetPaymentGatewayByPhone"))
            {
                database.AddInParameter(updateCommand, "@ProcessorPhone", System.Data.DbType.AnsiString, ProcessorPhone);
                database.AddInParameter(updateCommand, "@PaymentInstrumentId", System.Data.DbType.Int16, (short)paymentInstrument);
                database.AddOutParameter(updateCommand, "@PaymentMethodId", System.Data.DbType.Int32, 4);
                database.AddOutParameter(updateCommand, "@PaymentGatewayId", System.Data.DbType.Int32, 4);
                database.ExecuteNonQuery(updateCommand);
                if (updateCommand.Parameters["@PaymentMethodId"].Value == DBNull.Value)
                    PaymentMethodId = 0;
                else
                    PaymentMethodId = (int)updateCommand.Parameters["@PaymentMethodId"].Value;
                if (updateCommand.Parameters["@PaymentGatewayId"].Value == DBNull.Value)
                    PaymentGatewayId = 0;
                else
                    PaymentGatewayId = (int)updateCommand.Parameters["@PaymentGatewayId"].Value;
            }
            if (PaymentMethodId == 0)
                throw new Exception("Payment Method not specified for product.");
            if (PaymentGatewayId == 0)
                throw new Exception("Gateway not specified for product.");

        }
        public static DataTable GetPendingCaptures(DateTime? PaymentDate, string queue)
        {
            Database database = Token.Instance.Database;
            DataTable dt = new DataTable();
            using (System.Data.Common.DbCommand cmd = database.GetStoredProcCommand("wsp_Payments_GetPendingCaptures"))
            {
                if (PaymentDate.HasValue)
                    database.AddInParameter(cmd, "@PaymentDate", DbType.DateTime, PaymentDate.Value);
                if (!string.IsNullOrEmpty(queue))
                    database.AddInParameter(cmd, "@Queue", DbType.String, queue);
                DataSet ds = database.ExecuteDataSet(cmd);
                if (ds.Tables.Count > 0)
                    dt = ds.Tables[0];

            }
            return dt;
        }
        public virtual void RetryDisable(bool capture)
        {
            RetryDisable(this.PaymentId, capture);
        }
        public static void RetryDisable(int paymentId, bool capture)
        {
            Database database = Token.Instance.Database;
            DataTable dt = new DataTable();
            using (System.Data.Common.DbCommand cmd = database.GetStoredProcCommand("wsp_PaymentRetryDisable"))
            {
                database.AddInParameter(cmd, "@PaymentId", DbType.Int32, paymentId);
                database.AddInParameter(cmd, "@capture", DbType.Boolean, capture);
                database.ExecuteNonQuery(cmd);
            }
        }
        public static DataTable GetPendingRefunds()
        {
            Database database = Token.Instance.Database;
            DataTable dt = new DataTable();
            using (System.Data.Common.DbCommand cmd = database.GetStoredProcCommand("wsp_Payments_GetPendingRefunds"))
            {

                DataSet ds = database.ExecuteDataSet(cmd);
                if (ds.Tables.Count > 0)
                    dt = ds.Tables[0];

            }
            return dt;
        }
        public static DataTable GetPendingVoids()
        {
            Database database = Token.Instance.Database;
            DataTable dt = new DataTable();
            using (System.Data.Common.DbCommand cmd = database.GetStoredProcCommand("wsp_Payments_GetPendingVoids"))
            {

                DataSet ds = database.ExecuteDataSet(cmd);
                if (ds.Tables.Count > 0)
                    dt = ds.Tables[0];

            }
            return dt;
        }
        public static DataTable GetPendingAuthorize(DateTime? PaymentDate, string queue)
        {
            Database database = Token.Instance.Database;
            DataTable dt = new DataTable();
            using (System.Data.Common.DbCommand cmd = database.GetStoredProcCommand("wsp_Payments_GetPendingAuthorize"))
            {
                if (PaymentDate.HasValue)
                    database.AddInParameter(cmd, "@PaymentDate", DbType.DateTime, PaymentDate.Value);
                if (!string.IsNullOrEmpty(queue))
                    database.AddInParameter(cmd, "@Queue", DbType.String, queue);
                DataSet ds = database.ExecuteDataSet(cmd);
                if (ds.Tables.Count > 0)
                    dt = ds.Tables[0];

            }
            return dt;
        }
    }
}
