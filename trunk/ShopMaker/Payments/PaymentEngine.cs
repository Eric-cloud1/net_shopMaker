using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Common;
using MakerShop.Payments.Providers;
using MakerShop.Orders;
using MakerShop.Stores;
using MakerShop.Utility;
using MakerShop.Users;

namespace MakerShop.Payments
{
    /// <summary>
    /// Utility class for processing payments
    /// </summary>
    public static class PaymentEngine
    {
        #region Update
        /// <summary>
        /// Process an update request
        /// </summary>
        /// <param name="response">Details of the update transaction</param>
        public static void DoUpdate(NoCardTransactionResponse response)
        {
            DoUpdate(response, false);
        }

        /// <summary>
        /// Process an authorize request
        /// </summary>
        /// <param name="response">Details of the update transaction</param>
        /// <param name="async">If <b>true</b> update is processed asynchronously in a separate thread</param>
        private static void DoUpdate(NoCardTransactionResponse noCardResponse, bool async)
        {
            Transaction transaction;
            Payment payment = noCardResponse.Payment;
           
            if (!async)
            {
                if ((payment.PaymentStatus != PaymentStatus.AuthorizationPending))
                    throw new PaymentStatusException("This method can only be called when payment status is 'AuthorizationPending'.");
            }
            PaymentGateway gateway = null;
            if (payment.PaymentMethod != null)
            {
                gateway = payment.PaymentMethod.PaymentGateway;
            }
            if (gateway == null)
            {
                //manual processing
                transaction = CreateSuccessTransaction(noCardResponse, "0", "Manual Update Successful", noCardResponse.Response);
            }
            else
            {
                //process through a gateway.
                IPaymentProvider instance = gateway.GetInstance();
                if (instance != null)
                {
                    //TRY TO INSERT AN ORDER NOTE
                    try
                    {
                        string message = string.Format(Properties.Resources.UpdatePaymentRequest, noCardResponse.Amount);
                        OrderNote note = new OrderNote(payment.OrderId, Token.Instance.UserId, LocaleHelper.LocalNow, message, NoteType.SystemPrivate);
                        note.Save();
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("Could not add order note regarding update.", Logger.LogMessageType.Warn, ex);
                    }
                    //AUTHORIZE THE PAYMENT
                    try
                    {

                        transaction = instance.DoUpdate(noCardResponse);
                        
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("Provider threw exception in DoUpdate.", Logger.LogMessageType.Error, ex);
                        transaction = Transaction.CreateErrorTransaction(gateway.PaymentGatewayId, noCardResponse, string.Empty, ex.Message);
                    }
                    if (transaction == null)
                    {
                        //payment gateways should never return null transaction 
                        //instead they should return unsuccessful transaction
                        //CREATE A TRANSACTION TO RECORD THE ERROR
                        string message = "Class '" + instance.GetType().ToString() + "' returned a null Transaction response.";
                        Logger.Write(message, Logger.LogMessageType.Error);
                        transaction = Transaction.CreateErrorTransaction(gateway.PaymentGatewayId, noCardResponse, string.Empty, message);
                    }
                }
                else
                {
                    //gateway instance could not be loaded
                    string message = "Gateway '" + gateway.Name + "' instance could not be loaded.";
                    Logger.Write(message, Logger.LogMessageType.Error);
                    transaction = Transaction.CreateErrorTransaction(0, noCardResponse, string.Empty, message);
                }
            }
            RegisterTransaction(payment, transaction);

            payment.Save(false);
            TriggerAuthEvents(payment);
        }
        #endregion
        #region Authorize
        /// <summary>
        /// Process an authorize request
        /// </summary>
        /// <param name="request">Details of the authorize transaction</param>
        public static void DoAuthorize(AuthorizeTransactionRequest request)
        {
            DoAuthorize(request, false);
        }

        /// <summary>
        /// Process an authorize request
        /// </summary>
        /// <param name="request">Details of the authorize transaction</param>
        /// <param name="async">If <b>true</b> request is processed asynchronously in a separate thread</param>
        private static void DoAuthorize(AuthorizeTransactionRequest request, bool async)
        {
            Transaction transaction;
            Payment payment = request.Payment;
            //Check for black list
            MakerShop.Validation.BlackListTypes blackListType;
            if (payment.PaymentType == PaymentTypes.Initial)
            {
                if (MakerShop.Validation.BlackListsDataSource.IsOnBlackList(payment, request.IsCart, out blackListType))
                {
                    payment.PaymentStatus = PaymentStatus.AuthorizationFailed;
                    payment.PaymentStatusReason = "Black List: " + blackListType.ToString();
                    payment.Order.Notes.Add(new MakerShop.Orders.OrderNote(payment.Order.OrderId, 3300107, MakerShop.Utility.LocaleHelper.LocalNow, "Black List", MakerShop.Orders.NoteType.SystemPublic));
                    payment.Order.Notes.Save();
                    payment.Save();
                    OrderSubscriptionsDataSource.CancelOrderSubscription(payment.Order.ParentOrderId, Token.Instance.User.UserName);

                    throw new MakerShop.Validation.BlackListException("Black List");
                }
            }
            if (!async)
            {
                if ((payment.PaymentStatus != PaymentStatus.Unprocessed))
                    throw new PaymentStatusException("This method can only be called when payment status is 'Unprocessed'.");
            }
            PaymentGateway gateway = null;
            if(payment.PaymentMethod != null) {
                gateway = payment.PaymentMethod.PaymentGateway;   
            }
            if (gateway == null)
            {
                //manual processing
                transaction = CreateSuccessTransaction(request, "0", "Manual Authorization Successful");
            }
            else
            {
                //process through a gateway.
                IPaymentProvider instance = gateway.GetInstance();
                if (instance != null)
                {
                    //TRY TO INSERT AN ORDER NOTE
                    try
                    {
                        string message = string.Format(Properties.Resources.AuthorizePaymentRequest, request.Amount);
                        OrderNote note = new OrderNote(payment.OrderId, Token.Instance.UserId, LocaleHelper.LocalNow, message, NoteType.SystemPrivate);
                        note.Save();
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("Could not add order note regarding authorization.", Logger.LogMessageType.Warn, ex);
                    }
                    //AUTHORIZE THE PAYMENT
                    //bool authorizeSucceeded = false;
                    try
                    {
                        if (request.Amount == 0)
                        {
                            transaction = new Transaction();
                            transaction.TransactionDate = DateTime.Now;
                            transaction.TransactionType = TransactionType.AuthorizeCapture;
                            transaction.PaymentId = request.Payment.PaymentId;
                            transaction.Amount = 0;
                            transaction.TransactionStatus = TransactionStatus.Successful;
                            transaction.PaymentGatewayId = request.Payment.PaymentGatewayId;
                            request.Payment.PaymentStatus = PaymentStatus.Captured;
                        }
                        else
                        {
                            OrderSubscriptions os = OrderSubscriptionsDataSource.Load(request.Payment.Order.ParentOrderId);
                            
                            //OrderSubscriptionPlanDetails ospd = OrderSubscriptionPlanDetailsDataSource.Load(request.Payment.Order.ParentOrderId, request.Payment.PaymentTypeId);
                            if (os == null)
                            {
                                foreach (OrderItem oi in request.Payment.Order.Items)
                                {
                                    if (oi.ProductId != 0)
                                    {
                                        Products.SubscriptionPlanDetails spd = Products.SubscriptionPlanDetailsDataSource.Load(oi.ProductId, payment.PaymentType);
                                        if (spd.DaysToCapture == 0)
                                            request.Capture = true;
                                        break;
                                    }
                                }
                                
                            }
                            else
                            {
                                if (os.DaysToCapture == 0)
                                    request.Capture = true;
                            }
                            transaction = instance.DoAuthorize(request);
                        }
                        //authorizeSucceeded = ((transaction != null) && (transaction.TransactionStatus != TransactionStatus.Failed));
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("Provider threw exception in DoAuthorize.", Logger.LogMessageType.Error, ex);
                        transaction = Transaction.CreateErrorTransaction(gateway.PaymentGatewayId, request, string.Empty, ex.Message);
                    }
                    if (transaction == null)
                    {
                        //payment gateways should never return null transaction 
                        //instead they should return unsuccessful transaction
                        //CREATE A TRANSACTION TO RECORD THE ERROR
                        string message = "Class '" + instance.GetType().ToString() + "' returned a null Transaction response.";
                        Logger.Write(message, Logger.LogMessageType.Error);
                        transaction = Transaction.CreateErrorTransaction(gateway.PaymentGatewayId, request, string.Empty, message);
                    }
                }
                else
                {
                    //gateway instance could not be loaded
                    string message = "Gateway '" + gateway.Name + "' instance could not be loaded.";
                    Logger.Write(message, Logger.LogMessageType.Error);
                    transaction = Transaction.CreateErrorTransaction(0, request, string.Empty, message);
                }
            }
            RegisterTransaction(payment, transaction);

            payment.Save();
            if (payment.PaymentStatus == PaymentStatus.AuthorizationFailed)
            {
                if (MakerShop.Validation.WhiteListsDataSource.IsOnWhiteList(payment))
                {
                    payment.PaymentStatus = PaymentStatus.Captured;
                    payment.PaymentStatusReason = "White List";
                    payment.Order.Notes.Add(new MakerShop.Orders.OrderNote(payment.Order.OrderId, 3300107, MakerShop.Utility.LocaleHelper.LocalNow, "White List", MakerShop.Orders.NoteType.SystemPublic));
                    payment.Order.Notes.Save();

                    payment.Save();
                }
                
            }
            
            TriggerAuthEvents(payment);
        }
        #endregion
        /// <summary>
        /// Processes an authorize recurring request
        /// </summary>
        /// <param name="request">Details of the authorize recurring request</param>
        public static void DoAuthorizeRecurring(AuthorizeRecurringTransactionRequest request)
        {
            AuthorizeRecurringTransactionResponse response;
            response = new AuthorizeRecurringTransactionResponse();

            Transaction transaction;
            Payment payment = request.Payment;
            if ((payment.PaymentStatus != PaymentStatus.Unprocessed))
                throw new PaymentStatusException("This method can only be called when payment status is 'Unprocessed'.");
            
            PaymentGateway gateway = null;
            if (payment.PaymentMethod != null)
            {
                gateway = payment.PaymentMethod.PaymentGateway;
            }
            
            if (gateway == null)
            {
                //manual processing
                transaction = CreateSuccessTransaction(request, "0", "Manual Recurring Authorization Successful");
                response.Status = transaction.TransactionStatus;
                response.AddTransaction(transaction);
            }
            else
            {
                //process through a gateway.
                IPaymentProvider instance = gateway.GetInstance();
                if (instance != null)
                {
                    //TRY TO INSERT AN ORDER NOTE
                    try
                    {
                        string message = string.Format(Properties.Resources.AuthorizePaymentRequest, request.Amount);
                        OrderNote note = new OrderNote(payment.OrderId, Token.Instance.UserId, LocaleHelper.LocalNow, message, NoteType.SystemPrivate);
                        note.Save();
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("Could not add order note regarding authorization.", Logger.LogMessageType.Warn, ex);
                    }
                    //AUTHORIZE THE PAYMENT
                    try
                    {
                        //transaction = instance.DoAuthorizeRecurring(request);
                        response = instance.DoAuthorizeRecurring(request);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("Provider threw exception in DoAuthorize.", Logger.LogMessageType.Error, ex);
                        transaction = Transaction.CreateErrorTransaction(gateway.PaymentGatewayId, request, string.Empty, ex.Message);
                        if (response == null) response = new AuthorizeRecurringTransactionResponse();
                        response.ClearTransactions();
                        response.Status = transaction.TransactionStatus;
                        response.AddTransaction(transaction);
                    }
                    if (response == null)
                    {
                        //payment gateways should never return null response 
                        //instead they should return unsuccessful response (status = falied)
                        //CREATE A TRANSACTION TO RECORD THE ERROR
                        string message = "Class '" + instance.GetType().ToString() + "' returned a null Response.";
                        Logger.Write(message, Logger.LogMessageType.Error);
                        transaction = Transaction.CreateErrorTransaction(gateway.PaymentGatewayId, request, string.Empty, message);
                        response = new AuthorizeRecurringTransactionResponse();
                        response.Status = transaction.TransactionStatus;
                        response.AddTransaction(transaction);
                    }
                }
                else
                {
                    //gateway instance could not be loaded
                    string message = "Gateway '" + gateway.Name + "' instance could not be loaded.";
                    Logger.Write(message, Logger.LogMessageType.Error);
                    transaction = Transaction.CreateErrorTransaction(0, request, string.Empty, message);
                    response.Status = transaction.TransactionStatus;
                    response.AddTransaction(transaction);
                }
            }

            RegisterRecurringTransactions(payment, response);
            payment.Save();
            TriggerAuthRecurringEvents(payment);
        }

        /// <summary>
        /// Completes a Pending Authorization
        /// </summary>
        /// <param name="payment"> The payment for which to complete the pending authorization.</param>
        /// <param name="paymentGatewayId">Id of the payment gateway used for processing</param>
        /// <param name="successful">Indicates whether the pending authorization succeeded or failed.</param>
        public static void ProcessAuthorizePending(Payment payment, int paymentGatewayId, bool successful)
        {
            if ((payment.PaymentStatus != PaymentStatus.AuthorizationPending))
                throw new PaymentStatusException("This method can only be called when payment status is 'AuthorizationPending'.");
            
            Transaction transaction = payment.Transactions.LastAuthorizationPending;
            if (transaction == null)
            {
                //no pending authorize transaction found. create a new transaction
                transaction = new Transaction();
                transaction.Amount = payment.Amount - payment.Transactions.GetTotalAuthorized();
                transaction.TransactionType = TransactionType.Authorize;
                transaction.PaymentGatewayId = paymentGatewayId;
            }
            else
            {
                //remove the transaction from the collection for correct calculations
                payment.Transactions.Remove(transaction);
            }

            transaction.TransactionStatus = successful ? TransactionStatus.Successful : TransactionStatus.Failed;

            ProcessAuthorizePending(payment, transaction);
        }

        /// <summary>
        /// Completes a Pending Authorization
        /// </summary>
        /// <param name="payment">The payment for which to complete the pending authorization.</param>
        /// <param name="transaction">The successful or failed authorization transaction</param>        
        public static void ProcessAuthorizePending(Payment payment, Transaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction", "'transaction' can't be null.");
            if (transaction.TransactionStatus == TransactionStatus.Pending)
            {
                throw new ArgumentOutOfRangeException("TransactionStatus must be either 'Successful' or 'Failed'", "transaction");
            }
            if (transaction.TransactionType != TransactionType.Authorize 
                && transaction.TransactionType != TransactionType.AuthorizeCapture)
            {
                throw new ArgumentException("TransactionType should be 'Authorize' or 'AuthorizeCatpure'", "transaction");
            }

            transaction.TransactionDate = LocaleHelper.LocalNow;
            payment.PaymentStatus = PaymentStatusHelper.GetStatusAfterTransaction(payment, transaction);
            payment.Transactions.Add(transaction);
            payment.Save();
            TriggerAuthEvents(payment);
        }

        /// <summary>
        /// Forces an authorize transaction on a payment. Marks the payment to authorized
        /// </summary>
        /// <param name="payment">The payment to force an authorize transaction on</param>
        /// <param name="paymentGatewayId">Id of the payment gateway used for processing</param>
        public static void ForceAuthorize(Payment payment, int paymentGatewayId)
        {
            ForceAuthorize(payment, paymentGatewayId, true);
        }

        /// <summary>
        /// Forces an authorize transaction on a payment. Marks the authorize transaction 
        /// successful or unsuccessful based on the value of parameter 'successful'.
        /// </summary>
        /// <param name="payment">The payment to force an authorize transaction on</param>
        /// <param name="paymentGatewayId">Id of the payment gateway used for processing</param>
        /// <param name="successful">Indicates whether the authorization succeeded or failed.</param>
        public static void ForceAuthorize(Payment payment, int paymentGatewayId, bool successful)
        {
            Transaction transaction = new Transaction();
            transaction.Amount = payment.Amount - payment.Transactions.GetTotalAuthorized();
            transaction.TransactionType = TransactionType.Authorize;
            transaction.TransactionStatus = successful ? TransactionStatus.Successful : TransactionStatus.Failed;
            transaction.TransactionDate = LocaleHelper.LocalNow;
            transaction.PaymentGatewayId = paymentGatewayId;
            payment.PaymentStatus = PaymentStatusHelper.GetStatusAfterTransaction(payment, transaction);
            payment.Transactions.Add(transaction);
            payment.Save();
        }

        /// <summary>
        /// Attempt to capture a previously authorized transaction.
        /// </summary>
        public static void DoCapture(CaptureTransactionRequest request)
        {
            DoCapture(request, false);
        }

        private static void DoCapture(CaptureTransactionRequest request, bool async)
        {
            Transaction transaction;
            Payment payment = request.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");

            //verify payment status
            if (payment.PaymentStatus == PaymentStatus.Unprocessed)
            {
                //A CAPTURE HAS BEEN REQUESTED WITHOUT AN AUTHORIZATION
                AuthorizeTransactionRequest authCaptureRequest = new AuthorizeTransactionRequest(request.Payment, request.RemoteIP);
                authCaptureRequest.Amount = request.Amount;
                authCaptureRequest.Capture = true;
                authCaptureRequest.CurrencyCode = request.CurrencyCode;
                //DoAuthorize(authCaptureRequest, async);
                DoAuthorize(authCaptureRequest, false);
            }

            if (!async)
            {
                if (payment.PaymentStatus != PaymentStatus.Authorized)
                    throw new PaymentStatusException("Capture can only be called when payment status is 'Authorized'");
            }

            Transaction lastAuthorization = payment.Transactions.LastAuthorization;
            if ((lastAuthorization != null) && (lastAuthorization.PaymentGateway != null))
            {
                IPaymentProvider instance = lastAuthorization.PaymentGateway.GetInstance();
                if (instance != null)
                {
                    //TRY TO INSERT AN ORDER NOTE
                    try
                    {
                        string message = string.Format(Properties.Resources.CapturePaymentRequest, request.Amount);
                        OrderNote note = new OrderNote(payment.OrderId, Token.Instance.UserId, LocaleHelper.LocalNow, message, NoteType.SystemPrivate);
                        note.Save();
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("Could not add order note regarding capture.", Logger.LogMessageType.Warn, ex);
                    }
                    //CAPTURE THE PAYMENT
                    try
                    {
                        transaction = instance.DoCapture(request);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("Provider threw exception in DoCapture.", Logger.LogMessageType.Error, ex);
                        transaction = Transaction.CreateErrorTransaction(instance.PaymentGatewayId, request, string.Empty, ex.Message);
                    }
                    if (transaction == null)
                    {
                        //payment gateways should never return null transaction 
                        //instead they should return unsuccessful transaction
                        //CREATE A TRANSACTION TO RECORD THE ERROR
                        string message = "Class '" + instance.GetType().ToString() + "' returned a null Transaction response.";
                        transaction = Transaction.CreateErrorTransaction(instance.PaymentGatewayId, request, string.Empty, message);
                    }
                }
                else
                {
                    //gateway instance could not be loaded
                    string message = "Gateway instance is null or could not be loaded.";
                    Logger.Write(message, Logger.LogMessageType.Error);
                    transaction = Transaction.CreateErrorTransaction(0, request, string.Empty, message);
                }
            }
            else
            {
                //no gateway, manual processing completed
                transaction = CreateSuccessTransaction(request, "0", "Manual Capture Successful");
            }

            RegisterTransaction(payment, transaction);
            //IF THIS IS THE FINAL CAPTURE, UPDATE PAYMENT AMOUNT TO MATCH TOTAL CAPTURED            
            if ((transaction.TransactionType == TransactionType.Capture)
                && (transaction.TransactionStatus == TransactionStatus.Successful))
            {
                payment.Amount = payment.Transactions.GetTotalCaptured();
            }
            payment.Save(false);
            TriggerCaptureEvents(payment, transaction);
        }

        /// <summary>
        /// Completes a Pending Capture
        /// </summary>
        /// <param name="payment">The payment object for which to complete the pending capture.</param>
        /// <param name="paymentGatewayId">Id of the payment gateway used for processing</param>
        /// <param name="successful">Indicates whether the pending capture succeeded or failed.</param>
        public static void ProcessCapturePending(Payment payment, int paymentGatewayId, bool successful)
        {
            if ((payment.PaymentStatus != PaymentStatus.CapturePending))
                throw new PaymentStatusException("This method can only be called when payment status is 'CapturePending'.");
            
            Transaction transaction = payment.Transactions.LastCapturePending;
            if (transaction == null)
            {
                //no pending capture transaction found. create a new transaction
                transaction = new Transaction();
                transaction.Amount = payment.Transactions.GetTotalAuthorized() - payment.Transactions.GetTotalCaptured();
                if (transaction.Amount <= 0) return;
                transaction.TransactionType = TransactionType.Capture;
                transaction.PaymentGatewayId = paymentGatewayId;
            }
            else
            {
                //remove the transaction from the collection for correct calculations
                payment.Transactions.Remove(transaction);
            }

            transaction.TransactionStatus = successful ? TransactionStatus.Successful : TransactionStatus.Failed;

            ProcessCapturePending(payment, transaction);
        }
        
        /// <summary>
        /// Completes a Pending Capture
        /// </summary>
        /// <param name="payment">The payment for which to complete the pending capture.</param>
        /// <param name="transaction">The new successufl or failed capture transaction.</param>
        public static void ProcessCapturePending(Payment payment, Transaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction", "'transaction' can't be null.");
            if (transaction.TransactionStatus == TransactionStatus.Pending)
            {
                throw new ArgumentOutOfRangeException("TransactionStatus must be either 'Successful' or 'Failed'", "transaction");
            }
            if (transaction.TransactionType != TransactionType.Capture
                && transaction.TransactionType != TransactionType.AuthorizeCapture
                && transaction.TransactionType != TransactionType.PartialCapture)
            {
                throw new ArgumentException("TransactionType should be either 'Capture', 'AuthorizeCapture', or 'PartialCatpure'", "transaction");
            }

            transaction.TransactionDate = LocaleHelper.LocalNow;
            payment.PaymentStatus = PaymentStatusHelper.GetStatusAfterTransaction(payment, transaction);
            payment.Transactions.Add(transaction);
            //IF THIS IS THE FINAL CAPTURE, UPDATE PAYMENT AMOUNT TO MATCH TOTAL CAPTURED            
            if ((transaction.TransactionType == TransactionType.Capture)
                && (transaction.TransactionStatus == TransactionStatus.Successful))
            {
                payment.Amount = payment.Transactions.GetTotalCaptured();
            }

            payment.Save();
            TriggerCaptureEvents(payment, transaction);
        }


        /// <summary>
        /// Forces a capture transaction on a payment. Marks the capture transaction 
        /// successful.
        /// </summary>
        /// <param name="payment">The payment to force an authorize transaction on</param>
        /// <param name="paymentGatewayId">Id of the payment gateway used for processing</param>
        public static void ForceCapture(Payment payment, int paymentGatewayId)
        {
            ForceCapture(payment, paymentGatewayId, true);
        }

        /// <summary>
        /// Forces a capture transaction on a payment. Marks the capture transaction 
        /// successful or unsuccessful based on the value of parameter 'successful'.
        /// </summary>
        /// <param name="payment">The payment to force an authorize transaction on</param>
        /// <param name="paymentGatewayId">Id of the payment gateway used for processing</param>
        /// <param name="successful">Indicates whether the capture succeeded or failed.</param>
        public static void ForceCapture(Payment payment, int paymentGatewayId, bool successful)
        {
            Transaction transaction = new Transaction();
            transaction.Amount = payment.Amount - payment.Transactions.GetTotalCaptured();
            transaction.TransactionType = TransactionType.Capture;
            transaction.TransactionStatus = successful ? TransactionStatus.Successful : TransactionStatus.Failed;
            transaction.TransactionDate = LocaleHelper.LocalNow;
            transaction.PaymentGatewayId = paymentGatewayId;
            payment.PaymentStatus = PaymentStatusHelper.GetStatusAfterTransaction(payment, transaction);
            payment.Transactions.Add(transaction);
            if (transaction.TransactionStatus == TransactionStatus.Successful)
            {
                payment.Amount = payment.Transactions.GetTotalCaptured();
            }
            payment.Save();
        }

        /// <summary>
        /// Forces a transaction on a payment. 
        /// </summary>
        /// <param name="payment">The payment to force a transaction on</param>
        /// <param name="transaction">The transaction to force</param>
        public static void ForceTransaction(Payment payment, Transaction transaction)
        {
            transaction.TransactionDate = LocaleHelper.LocalNow;
            payment.PaymentStatus = PaymentStatusHelper.GetStatusAfterTransaction(payment, transaction);
            payment.Transactions.Add(transaction);
            if (transaction.TransactionStatus == TransactionStatus.Successful)
            {
                LSDecimal totalCaptured;
                if (transaction.TransactionType == TransactionType.Refund 
                    || transaction.TransactionType == TransactionType.PartialRefund)
                {
                    totalCaptured = payment.Transactions.GetTotalCaptured();
                    if (totalCaptured > 0) payment.Amount = totalCaptured;
                    else payment.Amount = 0;
                }
                else if (transaction.TransactionType == TransactionType.Capture)
                {
                    payment.Amount = payment.Transactions.GetTotalCaptured();
                }
                else if (transaction.TransactionType == TransactionType.Void)
                {
                    totalCaptured = payment.Transactions.GetTotalCaptured();
                    if (totalCaptured > 0) payment.Amount = totalCaptured;
                    else payment.Amount = payment.Transactions.GetRemainingAuthorized();
                }
            }
            payment.Save();
        }

        /// <summary>
        /// Processes a void transaction request
        /// </summary>
        /// <param name="request">Details of the void transaction request</param>
        public static void DoVoid(VoidTransactionRequest request)
        {
            Payment payment = request.Payment;

            //VERIFY THE PAYMENT HAS A STATUS THAT CAN BE VOIDED
            if (!PaymentStatusHelper.IsVoidable(payment.PaymentStatus))
                throw new PaymentStatusException(String.Format("Void cannot be called when payment status is '{0}'.", payment.PaymentStatus.ToString()));

            Transaction transaction = null;
            LSDecimal capturedAmount = 0;

            if ((payment.PaymentStatus == PaymentStatus.Unprocessed) || (payment.PaymentStatus == PaymentStatus.AuthorizationFailed))
            {
                //THIS PAYMENT HAS NOT BEEN PROCESSED, SO WE CAN SIMPLY UPDATE IT TO VOID
                transaction = CreateSuccessTransaction(request, "0", "Manual Void Successful");
            }
            else
            {
                //FIND OUT HOW MUCH HAS BEEN CAPTURED
                capturedAmount = payment.Transactions.GetTotalCaptured();
                LSDecimal maxVoidAmount = payment.Transactions.GetTotalAuthorized() - capturedAmount;
                LSDecimal gatewayVoidAmount = (request.Amount > maxVoidAmount) ? maxVoidAmount : request.Amount;

                IPaymentProvider instance = getGatewayInstance(request);
                string message;

                //ONLY SEND THE TRANSACTION IF THE GATEWAY INSTANCE IS PRESENT AND A POSITIVE AMOUNT IS TO BE VOIDED
                if (instance == null)
                {
                    message = "Gateway instance is null or could not be loaded.";
                    transaction = Transaction.CreateErrorTransaction(0, request, string.Empty, message);
                }
                else
                {
                    bool voidSupported = (instance.SupportedTransactions & SupportedTransactions.Void) == SupportedTransactions.Void;
                    if (!voidSupported)
                    {
                        //FORCE THE VOID
                        transaction = PaymentEngine.ManualVoid(payment, instance.PaymentGatewayId);
                    }
                    else if (gatewayVoidAmount <= 0)
                    {
                        message = "The amount '" + gatewayVoidAmount + "' can not be voided.";
                        transaction = Transaction.CreateErrorTransaction(instance.PaymentGatewayId, request, string.Empty, message);
                    }
                    else
                    {
                        //process void
                        request.Amount = gatewayVoidAmount;
                        try
                        {
                            transaction = instance.DoVoid(request);
                        }
                        catch (Exception ex)
                        {
                            Logger.Write("Payment provider threw exception on DoVoid.", Logger.LogMessageType.Error, ex);
                            transaction = Transaction.CreateErrorTransaction(instance.PaymentGatewayId, request, string.Empty, ex.Message);
                        }
                        if (transaction == null)
                        {
                            //payment gateways should never return null transaction 
                            //instead they should return unsuccessful transaction
                            //CREATE A TRANSACTION TO RECORD THE ERROR
                            message = "Class '" + instance.GetType().ToString() + "' returned a null Transaction response.";
                            transaction = Transaction.CreateErrorTransaction(instance.PaymentGatewayId, request, string.Empty, message);
                        }
                    }
                }
            }

            RegisterTransaction(payment, transaction);
            //WE HAVE TO MAKE AN ADJUSTMENT HERE TO HANDLE THE POSSIBILITY OF A PARTIAL VOID
            if ((transaction.TransactionStatus == TransactionStatus.Successful) && (capturedAmount > 0))
            {
                //THIS WAS A PARTIAL VOID, UPDATE THE AMOUNT AND STATUS
                payment.Amount = capturedAmount;
                //status already updated in RegisterTransaction
                //payment.PaymentStatus = PaymentStatus.Captured;
            }
  
            payment.Save();
            TriggerVoidEvents(payment, transaction);
        }

        /// <summary>
        /// Completes Pending Void transaction
        /// </summary>
        /// <param name="payment">The payment for which to complete the pending void.</param>
        /// <param name="paymentGatewayId">Id of the payment gateway used for processing.</param>
        /// <param name="successful">Indicates whether the pending void succeeded or failed.</param>
        public static void ProcessVoidPending(Payment payment, int paymentGatewayId, bool successful)
        {
            if ((payment.PaymentStatus != PaymentStatus.VoidPending))
                throw new PaymentStatusException("This method can only be called when payment status is 'VoidPending'.");

            Transaction transaction = payment.Transactions.LastVoidPending;
            if (transaction == null)
            {
                transaction = new Transaction();
                transaction.TransactionType = TransactionType.Void;
                transaction.PaymentGatewayId = paymentGatewayId;
                transaction.Amount = payment.Transactions.GetTotalAuthorized();
            }
            else
            {
                //remove the transaction from collection for correct calculations
                payment.Transactions.Remove(transaction);
            }

            transaction.TransactionStatus = successful ? TransactionStatus.Successful : TransactionStatus.Failed;

            ProcessVoidPending(payment, transaction);
        }

        /// <summary>
        /// Completes pending void transaction
        /// </summary>
        /// <param name="payment">The payment for which to complete the pending void.</param>
        /// <param name="transaction">The transaction to use for processing</param>
        public static void ProcessVoidPending(Payment payment, Transaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction", "'transaction' can't be null.");
            if (transaction.TransactionStatus == TransactionStatus.Pending)
            {
                throw new ArgumentOutOfRangeException("TransactionStatus must be either 'Successful' or 'Failed'", "transaction");
            }
            if (transaction.TransactionType != TransactionType.Void)
            {
                throw new ArgumentException("TransactionType should be 'Void'", "transaction");
            }
            
            LSDecimal capturedAmount = payment.Transactions.GetTotalCaptured();

            transaction.TransactionDate = LocaleHelper.LocalNow;            
            payment.PaymentStatus = PaymentStatusHelper.GetStatusAfterTransaction(payment, transaction);
            payment.Transactions.Add(transaction);
            
            bool successful = transaction.TransactionStatus == TransactionStatus.Successful;
            if (successful && capturedAmount > 0)
            {
                //NO MORE CAPTURES ARE POSSIBLE, UPDATE PAYMENT AMOUNT TO MATCH TOTAL CAPTURED
                payment.Amount = capturedAmount;
                payment.PaymentStatus = PaymentStatus.Captured;
            }
            payment.Save();
            TriggerVoidEvents(payment, transaction);
        }

        /// <summary>
        /// Forces a void transaction on a payment. Marks the void transaction 
        /// successful.
        /// </summary>
        /// <param name="payment">The payment to force a void transaction on</param>
        /// <param name="paymentGatewayId">Id of the payment gateway used for processing</param>
        public static void ForceVoid(Payment payment, int paymentGatewayId)
        {
            ForceVoid(payment, paymentGatewayId, true);
        }

        /// <summary>
        /// Forces a void transaction on a payment. Marks the void transaction 
        /// successful or unsuccessful based on the value of parameter 'successful'.
        /// </summary>
        /// <param name="payment">The payment to force a void transaction on</param>
        /// <param name="paymentGatewayId">Id of the payment gateway used for processing</param>
        /// <param name="successful">Indicates whether to mark transaction succeeded or failed.</param>
        public static void ForceVoid(Payment payment, int paymentGatewayId, bool successful)
        {
            Transaction transaction = new Transaction();
            LSDecimal capturedAmount = payment.Transactions.GetTotalCaptured();
            LSDecimal maxVoidAmount = payment.Transactions.GetTotalAuthorized() - capturedAmount;
            transaction.Amount = maxVoidAmount;
            transaction.TransactionType = TransactionType.Void;
            transaction.TransactionStatus = successful ? TransactionStatus.Successful : TransactionStatus.Failed;
            transaction.TransactionDate = LocaleHelper.LocalNow;
            transaction.PaymentGatewayId = paymentGatewayId;
            payment.PaymentStatus = PaymentStatusHelper.GetStatusAfterTransaction(payment, transaction);
            payment.Transactions.Add(transaction);
            if (transaction.TransactionStatus == TransactionStatus.Successful)
            {
                LSDecimal totalCaptured = payment.Transactions.GetTotalCaptured();
                if (totalCaptured > 0) payment.Amount = totalCaptured;
                else payment.Amount = payment.Transactions.GetRemainingAuthorized();                    
            }
            payment.Save();
        }

        private static Transaction ManualVoid(Payment payment, int paymentGatewayId)
        {
            Transaction transaction = new Transaction();
            LSDecimal capturedAmount = payment.Transactions.GetTotalCaptured();
            LSDecimal maxVoidAmount = payment.Transactions.GetTotalAuthorized() - capturedAmount;
            transaction.Amount = maxVoidAmount;
            transaction.TransactionType = TransactionType.Void;
            transaction.TransactionStatus = TransactionStatus.Successful;
            transaction.TransactionDate = LocaleHelper.LocalNow;
            transaction.PaymentGatewayId = paymentGatewayId;
            transaction.ResponseMessage = "Manual Void Successful";

            return transaction;
        }

        /// <summary>
        /// Processes a refund transaction request
        /// </summary>
        /// <param name="request">Details of the refund transaction request</param>
        public static void DoRefund(RefundTransactionRequest request)
        {
            //verify payment status
            Payment payment = request.Payment;
            if (payment.PaymentStatus != PaymentStatus.Captured)
                throw new PaymentStatusException("Credit/Refund can only be called when payment status is 'Captured'.");

            //GET THE CORRECT OBJECTS TO PREVENT SYNC ERRORS
            Order order = payment.Order;
            foreach (Payment p in order.Payments)
                if (p.PaymentId == payment.PaymentId) payment = p;

            LSDecimal capturedAmount = payment.Transactions.GetTotalCaptured();
            Transaction transaction;
            string message;

            Transaction lastCapture = payment.Transactions.LastCapture;
            if (lastCapture == null || lastCapture.PaymentGateway == null)
            {
                //manul transaction
                transaction = CreateSuccessTransaction(request, "0", "Manual Refund Successful");
            }
            else
            {
                //process through gateway
                IPaymentProvider instance = lastCapture.PaymentGateway.GetInstance();
                if (instance == null)
                {
                    message = "Gateway instance is null or could not be loaded";
                    transaction = Transaction.CreateErrorTransaction(0, request, string.Empty, message);
                }
                else
                {
                    if (request.Amount > capturedAmount) request.Amount = capturedAmount;
                    try
                    {
                        transaction = instance.DoRefund(request);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("Provider threw exception on DoRefund.", Logger.LogMessageType.Error, ex);
                        transaction = Transaction.CreateErrorTransaction(instance.PaymentGatewayId, request, string.Empty, ex.Message);
                    }
                    if (transaction == null)
                    {
                        //payment gateways should never return null transaction 
                        //instead they should return unsuccessful transaction
                        //CREATE A TRANSACTION TO RECORD THE ERROR
                        message = "Class '" + instance.GetType().ToString() + "' returned a null Transaction response.";
                        transaction = Transaction.CreateErrorTransaction(instance.PaymentGatewayId, request, string.Empty, message);
                    }
                }
            }

            RegisterTransaction(payment, transaction);
            if (transaction.TransactionStatus == TransactionStatus.Successful)
            {
                if (transaction.Amount == capturedAmount)
                {
                    //IF CREDIT WAS FOR THE ENTIRE PAYMENT, SET STATUS TO REFUNDED
                    payment.PaymentStatus = PaymentStatus.Refunded;
                    payment.Amount = 0;
                }
                else
                {
                    //THIS WAS ONLY A PARTIAL CREDIT, UPDATE THE PAYMENT TOTAL
                    payment.Amount = (capturedAmount - transaction.Amount);
                }
            }
            payment.Save(); 
            order.Save(true);
            
            TriggerRefundEvents(payment);

        }

        /// <summary>
        /// Completes pending refund transaction
        /// </summary>
        /// <param name="payment">The payment for which to complete the pending refund.</param>
        /// <param name="paymentGatewayId">Id of the payment gateway used for processing.</param>
        /// <param name="successful">Indicates whether the pending refund succeeded or failed.</param>
        public static void ProcessRefundPending(Payment payment, int paymentGatewayId, bool successful)
        {
            if ((payment.PaymentStatus != PaymentStatus.RefundPending))
                throw new PaymentStatusException("This method can only be called when payment status is 'RefundPending'.");

            Transaction transaction = payment.Transactions.LastRefundPending;
            if (transaction == null)
            {
                transaction = new Transaction();
                transaction.Amount = payment.Transactions.GetTotalCaptured();
                transaction.TransactionType = TransactionType.Refund;
                transaction.PaymentGatewayId = paymentGatewayId;
            }
            else
            {
                //remove the transaction from collection for correct calculations
                payment.Transactions.Remove(transaction);
            }

            transaction.TransactionStatus = successful ? TransactionStatus.Successful : TransactionStatus.Failed;

            ProcessRefundPending(payment, transaction);
        }

        /// <summary>
        /// Completes a pending refund transaction
        /// </summary>
        /// <param name="payment">The payment for which to complete the pending refund.</param>
        /// <param name="transaction">The new successful or failed refund transaction.</param>
        public static void ProcessRefundPending(Payment payment, Transaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction", "'transaction' can't be null.");
            if (transaction.TransactionStatus == TransactionStatus.Pending)
            {
                throw new ArgumentOutOfRangeException("TransactionStatus must be either 'Successful' or 'Failed'", "transaction");
            }
            if (transaction.TransactionType != TransactionType.Refund
                && transaction.TransactionType != TransactionType.PartialRefund)
            {
                throw new ArgumentException("TransactionType should be 'Refund'", "transaction");
            }

            LSDecimal capturedAmount = payment.Transactions.GetTotalCaptured();

            transaction.TransactionDate = LocaleHelper.LocalNow;            
            payment.PaymentStatus = PaymentStatusHelper.GetStatusAfterTransaction(payment, transaction);
            payment.Transactions.Add(transaction);
                        
            if (transaction.TransactionStatus == TransactionStatus.Successful)
            {
                if (transaction.Amount == capturedAmount)
                {
                    //IF CREDIT WAS FOR THE ENTIRE PAYMENT, SET STATUS TO REFUNDED
                    payment.PaymentStatus = PaymentStatus.Refunded;
                    payment.Amount = 0;
                }
                else
                {
                    //THIS WAS ONLY A PARTIAL CREDIT, UPDATE THE PAYMENT TOTAL
                    payment.Amount = (capturedAmount - transaction.Amount);
                }
            }
            payment.Save();
            TriggerVoidEvents(payment, transaction);
        }

        /// <summary>
        /// Forces a refund transaction on a payment. Marks the refund transaction 
        /// successful.
        /// </summary>
        /// <param name="payment">The payment to force a refund transaction on</param>
        /// <param name="paymentGatewayId">Id of the payment gateway used for processing</param>
        public static void ForceRefund(Payment payment, int paymentGatewayId)
        {
            ForceRefund(payment, paymentGatewayId, true);
        }

        /// <summary>
        /// Forces a refund transaction on a payment. Marks the refund transaction 
        /// successful or unsuccessful based on the value of parameter 'successful'.
        /// </summary>
        /// <param name="payment">The payment to force a refund transaction on</param>
        /// <param name="paymentGatewayId">Id of the payment gateway used for processing</param>
        /// <param name="successful">Indicates whether the refund succeeded or failed.</param>
        public static void ForceRefund(Payment payment, int paymentGatewayId, bool successful)
        {
            Transaction transaction = new Transaction();
            transaction.Amount = payment.Transactions.GetTotalCaptured();
            transaction.TransactionType = TransactionType.Refund;
            transaction.TransactionStatus = successful ? TransactionStatus.Successful : TransactionStatus.Failed;
            transaction.TransactionDate = LocaleHelper.LocalNow;
            transaction.PaymentGatewayId = paymentGatewayId;
            payment.PaymentStatus = PaymentStatusHelper.GetStatusAfterTransaction(payment, transaction);
            payment.Transactions.Add(transaction);
            if (transaction.TransactionStatus == TransactionStatus.Successful)
            {
                LSDecimal capturedAmount = payment.Transactions.GetTotalCaptured();
                if (capturedAmount > 0) payment.Amount = capturedAmount;
                else payment.Amount = 0;                
            }
            payment.Save();
        }

        #region AsyncDoUpdate

        /// <summary>
        /// Processes an authorize transaction request asynchronously
        /// </summary>
        /// <param name="request">Details of the authorize transaction request</param>
        public static void AsyncDoUpdate(NoCardTransactionResponse response)
        {
            AsyncDoUpdateDelegate del = new AsyncDoUpdateDelegate(InternalAsyncDoUpdate);
            AsyncCallback cb = new AsyncCallback(AsyncDoUpdateCallback);
            Payment payment = response.Payment;
            //verify payment status
            if ((payment.PaymentStatus != PaymentStatus.Unprocessed))
            {
                throw new PaymentStatusException("This method can only be called when payment status is 'Unprocessed'.");
            }
            //SET THE ORDER STATUS TO PENDING
            try
            {
                payment.UpdateStatus(PaymentStatus.AuthorizationPending);
            }
            catch (Exception ex)
            {
                Logger.Write("Could not set pending payment status.", Logger.LogMessageType.Warn, ex);
            }
            IAsyncResult ar = del.BeginInvoke(Token.Instance.UserId, Token.Instance.StoreId, payment.PaymentId, payment.AccountData, response.RemoteIP, cb, null);
        }

        private static void AsyncDoUpdateCallback(IAsyncResult ar)
        {
            AsyncDoUpdateDelegate del = (AsyncDoUpdateDelegate)((System.Runtime.Remoting.Messaging.AsyncResult)ar).AsyncDelegate;
            try
            {
                del.EndInvoke(ar);
            }
            catch (Exception ex)
            {
                Logger.Write("Error", Logger.LogMessageType.Error, ex);
            }
        }

        private delegate void AsyncDoUpdateDelegate(int userId, int storeId, int paymentId, string paymentInstrumentData, string remoteIP);
        private static void InternalAsyncDoUpdate(int userId, int storeId, int paymentId, string paymentInstrumentData, string remoteIP)
        {
            //REINITIALIZE THE TOKEN WITH SAVED STORE/USER CONTEXT
            Store store = StoreDataSource.Load(storeId);
            if (store != null)
            {
                Token.Instance.InitStoreContext(store);
                User user = UserDataSource.Load(userId);
                Token.Instance.InitUserContext(user);

                //LOAD THE PAYMENT INSTANCE
                Payment payment = PaymentDataSource.Load(paymentId);
                if (payment != null)
                {
                    //SET PAYMENT INSTRUMENT DATA
                    payment.AccountData = paymentInstrumentData;
                    //CREATE THE REQUEST
                    AuthorizeTransactionRequest request = new AuthorizeTransactionRequest(payment, remoteIP);
                    //PROCESS THE AUTHORIZATION
                    PaymentEngine.DoAuthorize(request, true);
                }
            }
        }
        #endregion
        #region AsyncDoAuthorize

        /// <summary>
        /// Processes an authorize transaction request asynchronously
        /// </summary>
        /// <param name="request">Details of the authorize transaction request</param>
        public static void AsyncDoAuthorize(AuthorizeTransactionRequest request)
        {
            AsyncDoAuthorizeDelegate del = new AsyncDoAuthorizeDelegate(InternalAsyncDoAuthorize);
            AsyncCallback cb = new AsyncCallback(AsyncDoAuthorizeCallback);
            Payment payment = request.Payment;
            //verify payment status
            if ((payment.PaymentStatus != PaymentStatus.Unprocessed))
            {
                throw new PaymentStatusException("This method can only be called when payment status is 'Unprocessed'.");
            }
            //SET THE ORDER STATUS TO PENDING
            try
            {
                payment.UpdateStatus(PaymentStatus.AuthorizationPending);
            }
            catch (Exception ex)
            {
                Logger.Write("Could not set pending payment status.", Logger.LogMessageType.Warn, ex);
            }
            IAsyncResult ar = del.BeginInvoke(Token.Instance.UserId, Token.Instance.StoreId, payment.PaymentId, payment.AccountData, request.RemoteIP, cb, null);
        }

        private static void AsyncDoAuthorizeCallback(IAsyncResult ar)
        {
            AsyncDoAuthorizeDelegate del = (AsyncDoAuthorizeDelegate)((System.Runtime.Remoting.Messaging.AsyncResult)ar).AsyncDelegate;
            try
            {
                del.EndInvoke(ar);
            }
            catch (Exception ex)
            {
                Logger.Write("Error", Logger.LogMessageType.Error, ex);
            }
        }

        private delegate void AsyncDoAuthorizeDelegate(int userId, int storeId, int paymentId, string paymentInstrumentData, string remoteIP);
        private static void InternalAsyncDoAuthorize(int userId, int storeId, int paymentId, string paymentInstrumentData, string remoteIP)
        {
            //REINITIALIZE THE TOKEN WITH SAVED STORE/USER CONTEXT
            Store store = StoreDataSource.Load(storeId);
            if (store != null)
            {
                Token.Instance.InitStoreContext(store);
                User user = UserDataSource.Load(userId);
                Token.Instance.InitUserContext(user);

                //LOAD THE PAYMENT INSTANCE
                Payment payment = PaymentDataSource.Load(paymentId);
                if (payment != null)
                {
                    //SET PAYMENT INSTRUMENT DATA
                    payment.AccountData = paymentInstrumentData;
                    //CREATE THE REQUEST
                    AuthorizeTransactionRequest request = new AuthorizeTransactionRequest(payment, remoteIP);
                    //PROCESS THE AUTHORIZATION
                    PaymentEngine.DoAuthorize(request, true);
                }
            }
        }
        #endregion

        #region AsyncDoCapture

        /// <summary>
        /// Processes a capture transaction request asynchronously
        /// </summary>
        /// <param name="request">Details of the capture transaction request</param>
        public static void AsyncDoCapture(CaptureTransactionRequest request)
        {
            AsyncDoCaptureDelegate del = new AsyncDoCaptureDelegate(InternalAsyncDoCapture);
            AsyncCallback cb = new AsyncCallback(AsyncDoCaptureCallback);
            Payment payment = request.Payment;
            //verify payment status
            if ((payment.PaymentStatus != PaymentStatus.Authorized))
            {
                throw new PaymentStatusException("Capture can only be called when payment status is 'Authorized'");
            }
            //SET THE ORDER STATUS TO PENDING
            try
            {
                payment.UpdateStatus(PaymentStatus.CapturePending);
            }
            catch (Exception ex)
            {
                Logger.Write("Could not set pending payment status.", Logger.LogMessageType.Warn, ex);
            }
            IAsyncResult ar = del.BeginInvoke(Token.Instance.UserId, Token.Instance.StoreId, request.Clone(), cb, null);
        }

        private static void AsyncDoCaptureCallback(IAsyncResult ar)
        {
            AsyncDoCaptureDelegate del = (AsyncDoCaptureDelegate)((System.Runtime.Remoting.Messaging.AsyncResult)ar).AsyncDelegate;
            try
            {
                del.EndInvoke(ar);
            }
            catch (Exception ex)
            {
                Logger.Write("Error", Logger.LogMessageType.Error, ex);
            }
        }

        //private delegate void AsyncDoCaptureDelegate(int userId, int paymentId, string paymentInstrumentData, string remoteIP, LSDecimal captureAmount, bool isFinal);
        //private static void InternalAsyncDoCapture(int userId, int paymentId, string paymentInstrumentData, string remoteIP, LSDecimal captureAmount, bool isFinal)
        private delegate void AsyncDoCaptureDelegate(int userId, int storeId, CaptureTransactionRequest request);
        private static void InternalAsyncDoCapture(int userId, int storeId, CaptureTransactionRequest request)
        {
            //REINITIALIZE THE TOKEN WITH SAVED STORE/USER CONTEXT
            Store store = StoreDataSource.Load(storeId);
            if (store != null)
            {
                Token.Instance.InitStoreContext(store);
                User user = UserDataSource.Load(userId);
                Token.Instance.InitUserContext(user);
                /*
                //LOAD THE PAYMENT INSTANCE
                Payment payment = PaymentDataSource.Load(paymentId);
                if (payment != null)
                {
                    //SET PAYMENT INSTRUMENT DATA
                    payment.AccountData = paymentInstrumentData;
                    //CREATE THE REQUEST
                    CaptureTransactionRequest request = new CaptureTransactionRequest(payment, remoteIP);
                    //PROCESS THE AUTHORIZATION
                    PaymentEngine.DoCapture(request);
                }
                */
                PaymentEngine.DoCapture(request, true);
            }
        }
        #endregion

        private static IPaymentProvider getGatewayInstance(VoidTransactionRequest request)
        {
            Transaction lastAuthorization = request.Payment.Transactions.LastAuthorization;
            if ((lastAuthorization != null) && (lastAuthorization.PaymentGateway != null))
            {
                return lastAuthorization.PaymentGateway.GetInstance();
            }
            return null;
        }

        private static Transaction CreateSuccessTransaction(ITransactionRequest request, string responseCode, string responseMessage, string response)
        {
            Transaction transaction = new Transaction();
            transaction.Amount = request.Amount;
            transaction.RemoteIP = request.RemoteIP;
            transaction.ResponseCode = responseCode;
            transaction.ResponseMessage = responseMessage;
            transaction.TransactionStatus = TransactionStatus.Successful;
            transaction.TransactionType = request.TransactionType;
            transaction.AdditionalData = response;
            return transaction;
        }


        /// <summary>
        /// Creates a transaction to report success when manual processing is done.
        /// </summary>
        /// <param name="request">The type of the transaction request.</param>
        /// <param name="responseCode">The response code to set.</param>
        /// <param name="responseMessage">The response message to set.</param>
        /// <returns>A transaction object configured with the error message.</returns>
        private static Transaction CreateSuccessTransaction(ITransactionRequest request, string responseCode, string responseMessage)
        {
            return CreateSuccessTransaction(request, responseCode, responseMessage, string.Empty);
        }

        public static void RegisterTransaction(Payment payment, Transaction transaction)
        {
            //RECORD THE TRANSACTION AND UPDATE PAYMENT STATUS
            payment.PaymentStatus = PaymentStatusHelper.GetStatusAfterTransaction(payment, transaction);
            //ONLY UPDATE THE DATE IF IT WAS NOT OTHERWISE PROVIDED
            if (transaction.TransactionDate == System.DateTime.MinValue) transaction.TransactionDate = LocaleHelper.LocalNow;
            //MAKE SURE FIELDS DO NOT EXCEED LIMITS
            transaction.ProviderTransactionId = StringHelper.Truncate(transaction.ProviderTransactionId, 50);
            transaction.ResponseCode = StringHelper.Truncate(transaction.ResponseCode, 50);
            transaction.ResponseMessage = StringHelper.Truncate(transaction.ResponseMessage, 255);
            transaction.AuthorizationCode = StringHelper.Truncate(transaction.AuthorizationCode, 255);
            transaction.AVSResultCode = StringHelper.Truncate(transaction.AVSResultCode, 1);
            transaction.CVVResultCode = StringHelper.Truncate(transaction.CVVResultCode, 1);
            transaction.CAVResultCode = StringHelper.Truncate(transaction.CAVResultCode, 1);
            if (!string.IsNullOrEmpty(transaction.RemoteIP))
                transaction.RemoteIP = StringHelper.Truncate(transaction.RemoteIP, 39);
            if (!string.IsNullOrEmpty(transaction.Referrer))
                transaction.Referrer = StringHelper.Truncate(transaction.Referrer, 255);
            //ADD TRANSACTION TO PAYMENT
            transaction.PaymentId = payment.PaymentId;
            payment.Transactions.Add(transaction);
        }

        private static void RegisterRecurringTransactions(Payment payment, AuthorizeRecurringTransactionResponse response)
        {
            //RECORD THE TRANSACTIONS AND UPDATE PAYMENT STATUS
            //payment.PaymentStatus = PaymentStatusHelper.GetStatusAfterTransaction(payment, transaction);
            if (response.Status == TransactionStatus.Successful)
            {
                payment.PaymentStatus = PaymentStatus.Completed;
            }
            else
            {
                //can not retry reccuring transactions. set status to void.
                payment.PaymentStatus = PaymentStatus.Void;
            }
            foreach (Transaction transaction in response.Transactions)
            {
                //ONLY UPDATE THE DATE IF IT WAS NOT OTHERWISE PROVIDED
                if (transaction.TransactionDate == System.DateTime.MinValue) transaction.TransactionDate = LocaleHelper.LocalNow;
                transaction.PaymentId = payment.PaymentId;
                payment.Transactions.Add(transaction);
            }
        }

        private static void TriggerVoidEvents(Payment payment, Transaction transaction)
        {
            //TRIGGER EVENTS AS NEEDED
            switch (payment.PaymentStatus)
            {
                case PaymentStatus.Captured:
                    StoreEventEngine.PaymentVoidedPartial(payment, transaction.Amount);
                    break;
                case PaymentStatus.Void:
                    StoreEventEngine.PaymentVoided(payment);
                    break;
            }
        }

        public static void TriggerAuthEvents(Payment payment)
        {
            if (string.IsNullOrEmpty(payment.CustomerServicePhone))
            {
                payment.CustomerServicePhone = payment.PaymentGateway.PhoneNumber;
                payment.ChargeDescriptor = payment.PaymentGateway.BillingStatement;
                payment.Save();
            }
            //TRIGGER EVENTS AS NEEDED
            switch (payment.PaymentStatus)
            {
                case PaymentStatus.Authorized:
                    StoreEventEngine.PaymentAuthorized(payment);
                    break;
                case PaymentStatus.Captured:
                case PaymentStatus.Completed:
                    StoreEventEngine.PaymentCaptured(payment, payment.Amount);
                    break;
                case PaymentStatus.AuthorizationFailed:
                    StoreEventEngine.PaymentAuthorizationFailed(payment);
                    break;
                case PaymentStatus.AuthorizationPending:
                    //TODO
                    //StoreEventEngine.PaymentAuthorizationPending(payment);
                    break;
            }
        }

        private static void TriggerAuthRecurringEvents(Payment payment)
        {
            //TODO
        }
        private static void TriggerCaptureEvents(Payment payment, Transaction transaction)
        {
            //TRIGGER EVENTS AS NEEDED
            switch (payment.PaymentStatus)
            {
                case PaymentStatus.Authorized:
                    StoreEventEngine.PaymentCapturedPartial(payment, transaction.Amount);
                    break;
                case PaymentStatus.Captured:
                    StoreEventEngine.PaymentCaptured(payment, transaction.Amount);
                    break;
                case PaymentStatus.CaptureFailed:
                    StoreEventEngine.PaymentCaptureFailed(payment, transaction.Amount);
                    break;
                case PaymentStatus.CapturePending:
                    //TODO
                    //StoreEventEngine.PaymentCapturePending(payment, transaction);
                    break;
            }
        }

        private static void TriggerRefundEvents(Payment payment)
        {
            switch (payment.PaymentStatus)
            {
                case PaymentStatus.Refunded:
                    StoreEventEngine.PaymentRefunded(payment, payment.Amount);
                    break;
                //case PaymentStatus.RefundFailed:
                    //StoreEventEngine.PaymentRefundFailed(); //TODO
                //    break;
                case PaymentStatus.RefundPending:
                    //StoreEventEngine.PaymentRefundPending(); //TODO
                    break;
            }
        }

        private static PaymentMethod GetGiftCertificatePaymentMethod(int gatewayId)
        {
            PaymentMethodCollection payMethCol = PaymentMethodDataSource.LoadForCriteria("PaymentInstrumentId=" + (short)PaymentInstrument.GiftCertificate);
            //PaymentMethodCollection payMethCol = PaymentMethodDataSource.LoadForPaymentGateway(gatewayId);
            if (payMethCol != null && payMethCol.Count > 0)
            {
                foreach (PaymentMethod pm in payMethCol)
                {
                    if (pm.PaymentGatewayId == gatewayId)
                    {
                        return pm;
                    }
                }
                //not found yet. use the first one
                PaymentMethod pm1 = payMethCol[0];
                pm1.PaymentGatewayId = gatewayId;
                pm1.Save();
                return pm1;
            }
            return null;
        }

        /// <summary>
        /// Gets the payment method in AC for gift certificate type payments 
        /// </summary>
        /// <returns>The payment method for Gift Certificate payments</returns>
        public static PaymentMethod GetGiftCertificatePaymentMethod() 
        {
            //Get The Gift Certificate Payment Method.
            string classId = Misc.GetClassId(typeof(GiftCertificatePaymentProvider));
            int gatewayId = PaymentGatewayDataSource.GetPaymentGatewayIdByClassId(classId);

            GiftCertificatePaymentProvider provider = new GiftCertificatePaymentProvider();
            PaymentMethod payMethod;

            if (gatewayId == 0)
            {
                //Gift Certificate Payment Gateway is missing. Add it now
                PaymentGateway pg = new PaymentGateway();
                pg.ClassId = classId;
                pg.ConfigData = "";
                pg.Name = provider.Name;
                pg.Save();
                gatewayId = pg.PaymentGatewayId;
            }

            payMethod = GetGiftCertificatePaymentMethod(gatewayId);

            if (payMethod == null)
            {
                //Gift Certificate Payment Method is missing. Add it now
                payMethod = new PaymentMethod();
                payMethod.PaymentInstrument = PaymentInstrument.GiftCertificate;
                payMethod.Name = "Gift Certificate";
                payMethod.PaymentGatewayId = gatewayId;
                payMethod.Save();
            }

            return payMethod;
        }

    }
}
