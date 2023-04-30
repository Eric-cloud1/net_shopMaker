using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using MakerShop.Catalog;
using MakerShop.Data;
using MakerShop.DigitalDelivery;
using MakerShop.Marketing;
using MakerShop.Messaging;
using MakerShop.Orders;
using MakerShop.Payments;
using MakerShop.Payments.Providers;
using MakerShop.Products;
using MakerShop.Reporting;
using MakerShop.Shipping;
using MakerShop.Stores;
using MakerShop.Taxes;
using MakerShop.Taxes.Providers;
using MakerShop.Users;
using MakerShop.Utility;
using MakerShop.Common;

public partial class Admin_Marketing_Affiliates_AffiliatePayment : System.Web.UI.Page
{

    private int _AffiliateId;
    private int _ParentAffiliateId;
    private int _OrderId;
    private int _OrderNumber;

    private OrderCollection _Orders;
    private Payment _Payment;
    private PaymentCollection _Payments;
    private Order _Order;

    protected void Page_Init(object sender, EventArgs e)
    {
        _AffiliateId = AlwaysConvert.ToInt(Request.QueryString["AffiliateId"]);
        _ParentAffiliateId = AlwaysConvert.ToInt(Request.QueryString["ParentAffiliateId"]);
      //  _Orders = OrderDataSource.LoadForAffiliate(_AffiliateId);

        _Payments = PaymentDataSource.LoadAllPaymentsByAffiliate(_AffiliateId);

        if (_Payments == null) Response.Redirect("Default.aspx");
       
        BindOrders();
    }

    protected string AppendOrderId(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return "";
        }
       // url = url + "?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString();
        return url;
    }

    protected void BindOrders()
    {
        List<int> orders = new List<int>();
        List<int> gatewayIds = new List<int>();
        int i = 0;
        string gatewayName = "";
        if (Token.Instance.User.IsGateway)
        {
            foreach (MakerShop.Users.UserSetting us in MakerShop.Common.Token.Instance.User.Settings)
            {
                if (us.FieldName.ToUpper() == "Gateway".ToUpper())
                {
                    gatewayName = us.FieldValue;

                }
                else if (us.FieldName.ToUpper() == "GatewayId".ToUpper())
                {
                    foreach (string s in us.FieldValue.Split(','))
                    {
                        gatewayIds.Add(int.Parse(s));
                    }
                    break;
                }
            }
        }
        foreach (Payment p in _Payments)
        {
            if (p.PaymentMethodName.ToLower().Contains(gatewayName.ToLower()) || gatewayName == "")
            {
                if (gatewayIds.Contains(p.PaymentGatewayId) || (gatewayIds.Count == 0))
                {
                    i = OrderDataSource.LookupOrderNumber(p.OrderId);
                    if (!orders.Contains(i))
                        orders.Add(i);
                }
            }
        }

        OrderRepeater.DataSource = orders;
        OrderRepeater.DataBind();
    }

    protected void BindPayments()
    {
        //_Order = OrderDataSource.LoadParent(_OrderId);
    }

    protected string getOrderNumber(object dataItem)
    {
        return string.Format(@"Order # {0}", dataItem.ToString());

    }

    protected string getImageNumber(object dataItem)
    {
        return string.Format(@"img{0}", dataItem.ToString());

    }

    protected string FormatChargeBackDateDate(object dataItem)
    {
        Transaction a = (Transaction)dataItem;

        string _chargeBackDetails = string.Empty;


        if ((a.TransactionTypeId != 4) && (a.TransactionTypeId != 1))
            return _chargeBackDetails;


        if (a.ChargeBackCreateDate != new DateTime())
            _chargeBackDetails += string.Format(@"<br/>Created: {0:g}", a.ChargeBackCreateDate);

        if (a.ChargeBackDateInitiated != new DateTime())
            _chargeBackDetails += string.Format(@"<br/>Initiated: {0:d}", a.ChargeBackDateInitiated);

        if (_chargeBackDetails != string.Empty)
            _chargeBackDetails = string.Format(@"<font color=red>ChargeBack:{0}</font>", _chargeBackDetails);

        return _chargeBackDetails;

    }

    protected string FormatPaymentType(object dataItem)
    {
        Payment p = (Payment)dataItem;
        return p.PaymentType.ToString();
    }

    protected void BuildTaskMenu(DropDownList taskMenu, Payment payment)
    {
        taskMenu.Items.Clear();
        //ADD BLANK ITEM TO START
        taskMenu.Items.Add(new ListItem());
        if (ShowButton("Received", payment)) taskMenu.Items.Add(new ListItem("Mark as Received", "RECEIVED"));
        if (ShowButton("Authorize", payment)) taskMenu.Items.Add(new ListItem("Authorize Payment", "AUTHORIZE"));
        if (ShowButton("RetryAuth", payment)) taskMenu.Items.Add(new ListItem("Retry Authorization", "RETRY"));
        if (ShowButton("RetryCapture", payment)) taskMenu.Items.Add(new ListItem("Retry Capture", "RETRY"));
        if (ShowButton("Capture", payment)) taskMenu.Items.Add(new ListItem("Capture Payment", "CAPTURE"));
        if (ShowButton("Void", payment))
            if ((payment.PaymentMethod != null && (payment.PaymentMethod.IsCreditOrDebitCard() || payment.PaymentMethod.PaymentInstrument == PaymentInstrument.PayPal))
                && payment.PaymentStatus != PaymentStatus.Unprocessed)
                taskMenu.Items.Add(new ListItem("Void Authorization", "VOID"));
            else
                taskMenu.Items.Add(new ListItem("Void Payment", "VOID"));
        if (ShowButton("Refund", payment)) taskMenu.Items.Add(new ListItem("Issue Refund", "REFUND"));
        if (taskMenu.Items.Count > 1) taskMenu.Items.Add(new ListItem("---", String.Empty));
        taskMenu.Items.Add(new ListItem("Edit Payment", "EDIT"));
        taskMenu.Items.Add(new ListItem("Delete Payment", "DELETE"));
        PaymentStatus ps = payment.PaymentStatus;
        if ((ps == PaymentStatus.Captured) ||
          (ps == PaymentStatus.Completed) ||
         (ps == PaymentStatus.Refunded))
        {
            taskMenu.Items.Add(new ListItem("---", String.Empty));
            taskMenu.Items.Add(new ListItem("Charge Back", "CHARGEBACK"));

        }

        // REGISTER CLIENT SCRIPT FOR DELETE TASK
        string warnScript = String.Empty;
        warnScript += "function confirmDel(){";
        warnScript += " if( document.getElementById('" + taskMenu.ClientID.ToString() + "').value == 'DELETE'){";
        warnScript += "return confirm('This option will delete the payment and all transaction information, Are you sure you want to delete payments?');";
        warnScript += "}else return true;"; // END IF
        warnScript += "}"; // END Function

        this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "checkDelete", warnScript, true);

    }

    public string GetSkinAVS(string code)
    {
        return string.Empty;
    }

    public string GetSkinCVV(string code)
    {
        return string.Empty;
    }

    protected void PaymentRepeater_ItemDataBound(object source, RepeaterItemEventArgs e)
    {
        if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
        {
            DropDownList taskMenu = (DropDownList)e.Item.FindControl("PaymentAction");
            if (taskMenu != null) BuildTaskMenu(taskMenu, (Payment)e.Item.DataItem);
            if (Token.Instance.User.IsGateway)
            {
                ((Panel)e.Item.FindControl("pTask")).Visible = false;
                ((Panel)e.Item.FindControl("pAccountDetails")).Visible = false;

            }

        }
    }

    protected void OrderRepeater_ItemDataBound(object source, RepeaterItemEventArgs e)
    {
        RepeaterItem item = e.Item;

        if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
        {
            Repeater PaymentRepeater = (Repeater)item.FindControl("PaymentRepeater");
            int orderNumber = (int)item.DataItem;

            int orderId = OrderDataSource.LookupOrderId(orderNumber);
            PaymentCollection Orderpayments = GatewayFilter(PaymentDataSource.LoadForOrder(orderId));
            ///
            List<int> gatewayIds = new List<int>();
            string gatewayName = "";
            if (Token.Instance.User.IsGateway)
            {
                foreach (MakerShop.Users.UserSetting us in MakerShop.Common.Token.Instance.User.Settings)
                {
                    if (us.FieldName.ToUpper() == "Gateway".ToUpper())
                    {
                        gatewayName = us.FieldValue;

                    }
                    else if (us.FieldName.ToUpper() == "GatewayId".ToUpper())
                    {
                        foreach (string s in us.FieldValue.Split(','))
                        {
                            gatewayIds.Add(int.Parse(s));
                        }
                        break;
                    }
                }
            }
            PaymentCollection pc = new PaymentCollection();
            ///
            foreach (Payment p in Orderpayments)
            {
                if (p.PaymentMethodName.ToLower().Contains(gatewayName.ToLower()) || gatewayName == "")
                {
                    if (gatewayIds.Contains(p.PaymentGatewayId) || (gatewayIds.Count == 0))
                    {
                        if ((p.PaymentType == PaymentTypes.Commission) || (p.PaymentType == PaymentTypes.Commission_Agent) ||
                            (p.PaymentType == PaymentTypes.Commission_Company) || (p.PaymentType == PaymentTypes.Commission_Location)
                            || (p.PaymentType == PaymentTypes.Commission_Master_Agent) || (p.PaymentType == PaymentTypes.Commission_Master_Company))
                            pc.Add(p);
                    }
                }

            }
            PaymentRepeater.DataSource = pc;
            PaymentRepeater.DataBind();
        }
    }

    private PaymentCollection GatewayFilter(PaymentCollection payments)
    {
        if (Token.Instance.User.IsGateway)
            return payments;

        string gatewaySearch = "";
        foreach (MakerShop.Users.UserSetting us in MakerShop.Common.Token.Instance.User.Settings)
        {
            if (us.FieldName.ToUpper() == "Gateway".ToUpper())
            {
                gatewaySearch = us.FieldValue;
                break;
            }
        }

        PaymentCollection pc = new PaymentCollection();
        foreach (Payment p in payments)
        {
            if (p.PaymentGateway.Name.ToLower().Contains(gatewaySearch.ToLower()))
            {
                pc.Add(p);
            }
        }
        return pc;
    }

    protected void PaymentRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName.StartsWith("Do_"))
        {
            int paymentId = AlwaysConvert.ToInt(e.CommandArgument);        
            int index = _Payments.IndexOf(paymentId);

            if (index > -1)
            {
                Payment payment = _Payments[index];
                _Order = payment.Order;
                _OrderId = _Order.OrderId;
                _OrderNumber = _Order.OrderNumber;


                string whichCommand = e.CommandName.Substring(3);
                if (whichCommand == "Task")
                {
                    //FIND TASK MENU
                    DropDownList taskMenu = (DropDownList)e.Item.FindControl("PaymentAction");
                    whichCommand = taskMenu.SelectedValue;
                }

                //TAKE ACTION BASED ON COMMAND
                if (!string.IsNullOrEmpty(whichCommand))
                {
                    whichCommand = whichCommand.ToUpperInvariant();
                    switch (whichCommand)
                    {
                        case "RETRY":
                            //IF THIS WAS A FAILED CAPTURE, RESET PAYMENT STATUS TO AUTHORIZED AND REDIRECT TO CAPTURE PAGE
                            if (payment.PaymentStatus == PaymentStatus.CaptureFailed)
                            {
                                payment.UpdateStatus(PaymentStatus.Authorized);
                                Response.Redirect("CapturePayment.aspx?PaymentId=" + payment.PaymentId.ToString());
                            }
                            //THIS WAS A FAILED AUTHORIZATION, SHOW THE RETRY PANEL
                            Panel retryPanel = (Panel)PageHelper.RecursiveFindControl(e.Item, "RetryPanel");
                            retryPanel.Visible = true;
                            break;
                        case "SUBMITRETRY":
                            TextBox securityCode = (TextBox)PageHelper.RecursiveFindControl(e.Item, "RetrySecurityCode");
                            if (!string.IsNullOrEmpty(securityCode.Text))
                            {
                                AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);
                                accountData["SecurityCode"] = securityCode.Text;
                                payment.AccountData = accountData.ToString();
                            }
                            payment.PaymentStatus = MakerShop.Payments.PaymentStatus.Unprocessed;
                            payment.Authorize(false);
                            BindPayments();
                            break;
                        case "CANCELRETRY":
                            //NO ACTION REQUIRED, THE REBIND STEP WILL HIDE THE RETRY PANEL
                            BindPayments();
                            break;
                        case "EDIT":
                            Response.Redirect("EditPayment.aspx?PaymentId=" + payment.PaymentId.ToString());
                            break;
                        case "CHARGEBACK":
                            Response.Redirect("chargeback/PaymentChargeBack.aspx?OrderId=" + _OrderId.ToString() + "&PaymentId=" + payment.PaymentId.ToString());
                            break;
                        case "CAPTURE":
                            //REDIRECT TO CAPTURE FORM
                            Response.Redirect("CapturePayment.aspx?PaymentId=" + payment.PaymentId.ToString());
                            break;
                        case "DELETE":
                            string deleteNote = string.Format("A payment of type {0} and amount {1:c} is deleted.", payment.PaymentMethodName, payment.Amount);
                            _Order.Payments.DeleteAt(index);
                            _Order.Notes.Add(new OrderNote(_Order.OrderId, Token.Instance.UserId, LocaleHelper.LocalNow, deleteNote, NoteType.SystemPrivate));
                            _Order.Notes.Save();
                            BindPayments();
                            break;
                        case "AUTHORIZE":
                            payment.Authorize();
                            BindPayments();
                            break;
                        case "RECEIVED":
                            payment.PaymentStatus = PaymentStatus.Completed;
                            payment.Save();
                            BindPayments();
                            break;
                        case "REFUND":
                            Response.Redirect("RefundPayment.aspx?PaymentId=" + payment.PaymentId.ToString() + "&OrderId=" + _OrderId);
                            break;
                        case "VOID":
                            Response.Redirect("VoidPayment.aspx?PaymentId=" + payment.PaymentId.ToString() + "&OrderId=" + _OrderId);
                            break;
                        default:
                            throw new ArgumentException("Unrecognized command: " + whichCommand);
                    }
                }
            }
        }
    }

    public bool ShowButton(string buttonName, object dataItem)
    {
        Payment payment = (Payment)dataItem;
        switch (buttonName.ToUpperInvariant())
        {
            case "RETRYAUTH":
                // DISABLE FOR PHONE CALL PAYMENT METHOD
                if (payment.PaymentMethod != null && payment.PaymentMethod.PaymentInstrument == PaymentInstrument.PhoneCall) return false;
                return (payment.PaymentStatus == PaymentStatus.AuthorizationFailed);
            case "RETRYCAPTURE":
                // DISABLE FOR PHONE CALL PAYMENT METHOD
                if (payment.PaymentMethod != null && payment.PaymentMethod.PaymentInstrument == PaymentInstrument.PhoneCall) return false;
                return (payment.PaymentStatus == PaymentStatus.CaptureFailed);
            case "RECEIVED":
                return (payment.PaymentStatus == PaymentStatus.Unprocessed);
            case "AUTHORIZE":
                // DISABLE FOR PHONE CALL PAYMENT METHOD
                if (payment.PaymentMethod != null && payment.PaymentMethod.PaymentInstrument == PaymentInstrument.PhoneCall) return false;
                return (payment.PaymentStatus == PaymentStatus.Unprocessed);
            case "VOID":
                //Disable Void for Google Checkout AND PHONE CALL PAYMENT METHOD
                if (payment.PaymentMethod != null && (payment.PaymentMethod.PaymentInstrument == PaymentInstrument.GoogleCheckout || payment.PaymentMethod.PaymentInstrument == PaymentInstrument.PhoneCall))
                {
                    return false;
                }
                else
                {
                    //VOID SHOULD ONLY BE SHOWN IF THE PAYMENT IS UNPROCESSED OR IN AN AUTHORIZED STATE
                    return (payment.PaymentStatus == PaymentStatus.Unprocessed
                           || payment.PaymentStatus == PaymentStatus.AuthorizationFailed
                           || payment.PaymentStatus == PaymentStatus.Authorized
                           || payment.PaymentStatus == PaymentStatus.CaptureFailed);
                }
            case "CAPTURE":
                // DISABLE FOR PHONE CALL PAYMENT METHOD
                if (payment.PaymentMethod != null && payment.PaymentMethod.PaymentInstrument == PaymentInstrument.PhoneCall) return false;
                return (payment.PaymentStatus == PaymentStatus.Authorized);
            case "CANCEL":
                // DISABLE FOR PHONE CALL PAYMENT METHOD
                if (payment.PaymentMethod != null && payment.PaymentMethod.PaymentInstrument == PaymentInstrument.PhoneCall) return false;
                return (payment.PaymentStatus == PaymentStatus.Completed);
            case "REFUND":
                //SHOW REFUND IF THE PAYMENT WAS MADE WITHIN 60 DAYS
                //AND THE PAYMENT IS CAPTURED
                //AND THERE IS IS A POSTIVE TRANSACTION CAPTURE AMOUNT
                if (payment.CompletedDate != DateTime.MinValue)
                    return ((payment.CompletedDate > DateTime.UtcNow.AddDays(-120))
                       && (payment.PaymentStatus == PaymentStatus.Captured)
                       && (payment.Transactions.GetTotalCaptured() > 0)
                       && (payment.PaymentMethod == null || payment.PaymentMethod.PaymentInstrument != PaymentInstrument.GiftCertificate));
                else
                    return ((payment.PaymentDate > DateTime.UtcNow.AddDays(-120))
                           && (payment.PaymentStatus == PaymentStatus.Captured)
                           && (payment.Transactions.GetTotalCaptured() > 0)
                           && (payment.PaymentMethod == null || payment.PaymentMethod.PaymentInstrument != PaymentInstrument.GiftCertificate));
            case "DELETE":
                //BY DEFAULT DO NOT SHOW THE DELETE BUTTON
                return false;
            default:
                throw new ArgumentException("Invalid button name: '" + buttonName, buttonName);
        }
    }

    public bool ShowTransactionElement(string elementName, object dataItem)
    {
        Transaction transaction = (Transaction)dataItem;
        switch (elementName.ToUpperInvariant())
        {
            case "AVSCVV":
                return (transaction.TransactionType == TransactionType.Authorize || transaction.TransactionType == TransactionType.AuthorizeCapture || transaction.TransactionType == TransactionType.AuthorizeRecurring);
            default:
                throw new ArgumentException("Invalid element name: '" + elementName, elementName);
        }
    }

    protected bool isSuccessfulTransaction(Object obj)
    {
        if (obj is TransactionStatus)
        {
            return ((TransactionStatus)obj) == TransactionStatus.Successful;
        }
        return false;
    }

    protected bool isFailedTransaction(Object obj)
    {
        if (obj is TransactionStatus)
        {
            return ((TransactionStatus)obj) == TransactionStatus.Failed;
        }
        return false;
    }

    protected bool isPendingTransaction(Object obj)
    {
        if (obj is TransactionStatus)
        {
            return ((TransactionStatus)obj) == TransactionStatus.Pending;
        }
        return false;
    }

    protected string GetCvv(object dataItem)
    {
        Transaction transaction = (Transaction)dataItem;
        return StoreDataHelper.TranslateCVVCode(transaction.CVVResultCode) + " (" + transaction.CVVResultCode + ")";
    }

    protected string GetAvs(object dataItem)
    {
        Transaction transaction = (Transaction)dataItem;
        return StoreDataHelper.TranslateAVSCode(transaction.AVSResultCode) + " (" + transaction.AVSResultCode + ")";
    }


}