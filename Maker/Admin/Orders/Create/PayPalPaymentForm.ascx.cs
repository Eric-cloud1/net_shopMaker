using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Common;
using MakerShop.Orders;
using MakerShop.Payments;
using MakerShop.Users;
using MakerShop.Utility;

public partial class Admin_Orders_Create_PayPalPaymentForm : System.Web.UI.UserControl
{
    private int _UserId;
    private User _User;
    Basket _Basket;

    //DEFINE EVENTS TO TRIGGER FOR CHECKOUT
    public event CheckingOutEventHandler CheckingOut;
    public event CheckedOutEventHandler CheckedOut;

    private int _PaymentMethodId;
    public int PaymentMethodId
    {
        get { return _PaymentMethodId; }
        set { _PaymentMethodId = value; }
    }

    private string _ValidationGroup = "PayPal";
    public string ValidationGroup
    {
        get { return _ValidationGroup; }
        set { _ValidationGroup = value; }
    }

    private bool _ValidationSummaryVisible = false;
    public bool ValidationSummaryVisible
    {
        get { return _ValidationSummaryVisible; }
        set { _ValidationSummaryVisible = value; }
    }

    private LSDecimal _PaymentAmount = 0;
    public LSDecimal PaymentAmount
    {
        get { return _PaymentAmount; }
        set { _PaymentAmount = value; }
    }

    private void UpdateValidationOptions()
    {
        PayPalButton.ValidationGroup = _ValidationGroup;
        ValidationSummary1.ValidationGroup = _ValidationGroup;
        ValidationSummary1.Visible = _ValidationSummaryVisible;
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        // LOCATE THE USER THAT THE ORDER IS BEING PLACED FOR
        _UserId = AlwaysConvert.ToInt(Request.QueryString["UID"]);
        _User = UserDataSource.Load(_UserId);
        if (_User == null) return;
        _Basket = _User.Basket;

        UpdateValidationOptions();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        PaymentMethod method = PaymentMethodDataSource.Load(this.PaymentMethodId);
        if (method != null)
        {
            Caption.Text = string.Format(Caption.Text, method.Name);
        }
        else
        {
            this.Controls.Clear();
            Trace.Write(this.GetType().ToString(), "Output suppressed, invalid payment method provided.");
        }

        PayPalButton.ImageUrl = Request.Url.Scheme + "://www.paypal.com/en_US/i/btn/x-click-but1.gif";

        StringBuilder submitScript = new StringBuilder();
        submitScript.Append("if(document.getElementById('" + FormIsSubmitted.ClientID + "').value==1 || Page_ClientValidate('PayPal') == false) { return false; } ");
        submitScript.Append("this.disabled = true; ");
        submitScript.Append("document.getElementById('" + FormIsSubmitted.ClientID + "').value=1;");
        //GetPostBackEventReference obtains a reference to a client-side script function that causes the server to post back to the page.
        submitScript.Append(Page.ClientScript.GetPostBackEventReference(PayPalButton, string.Empty));
        submitScript.Append(";");
        submitScript.Append("return false;");
        this.PayPalButton.Attributes.Add("onclick", submitScript.ToString());
    }

    private Payment GetPayment()
    {
        Payment payment = new Payment();
        payment.PaymentMethodId = _PaymentMethodId;
        if (this.PaymentAmount > 0) payment.Amount = this.PaymentAmount;
        else payment.Amount = _Basket.Items.TotalPrice();
        return payment;
    }

    protected void PayPalButton_Click(object sender, EventArgs e)
    {
        //CREATE THE PAYMENT OBJECT
        Payment payment = GetPayment();
        //PROCESS CHECKING OUT EVENT
        bool checkOut = true;
        if (CheckingOut != null)
        {
            CheckingOutEventArgs c = new CheckingOutEventArgs(payment);
            CheckingOut(this, c);
            checkOut = !c.Cancel;
        }
        if (checkOut)
        {
            //PROCESS THE CHECKOUT
            CheckoutRequest checkoutRequest = new CheckoutRequest(payment);
            CheckoutResponse checkoutResponse = _Basket.Checkout(checkoutRequest);
            if (checkoutResponse.Success)
            {
                if (CheckedOut != null) CheckedOut(this, new CheckedOutEventArgs(checkoutResponse));
                Response.Redirect(NavigationHelper.GetReceiptUrl(checkoutResponse.OrderId));
            }
            else
            {
                List<string> warningMessages = checkoutResponse.WarningMessages;
                if (warningMessages.Count == 0)
                    warningMessages.Add("The order could not be submitted at this time.  Please try again later or contact us for assistance.");
                if (CheckedOut != null) CheckedOut(this, new CheckedOutEventArgs(checkoutResponse));
            }
        }

        // IF NOT SUCCESSFULL / ENABLE THE CHECKOUT BUTTON
        PayPalButton.Enabled = true;
        FormIsSubmitted.Value = "0";
    }

}
