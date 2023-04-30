<%@ Control Language="C#" ClassName="MailPaymentForm" %>

<script runat="server">
    //DEFINE EVENTS TO TRIGGER FOR CHECKOUT
    public event CheckingOutEventHandler CheckingOut;
    public event CheckedOutEventHandler CheckedOut;

    private int _PaymentMethodId;
    public int PaymentMethodId
    {
        get { return _PaymentMethodId; }
        set { _PaymentMethodId = value; }
    }

    private string _ValidationGroup = "MailFax";
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
        MailButton.ValidationGroup = _ValidationGroup;
        ValidationSummary1.ValidationGroup = _ValidationGroup;
        ValidationSummary1.Visible = _ValidationSummaryVisible;
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        UpdateValidationOptions();
    }
        
    protected void Page_Load(object sender, EventArgs e)
    {
        PaymentMethod method = PaymentMethodDataSource.Load(this.PaymentMethodId);
        if (method != null)
        {
            Caption.Text = string.Format(Caption.Text, method.Name);
            MailButton.Text = string.Format(MailButton.Text, method.Name);
        }
        else
        {
            this.Controls.Clear();
            Trace.Write(this.GetType().ToString(), "Output suppressed, invalid payment method provided.");
        }
    }

    private Payment GetPayment()
    {
        Payment payment = new Payment();
        payment.PaymentMethodId = _PaymentMethodId;
        if (this.PaymentAmount > 0) payment.Amount = this.PaymentAmount;
        else payment.Amount = Token.Instance.User.Basket.Items.TotalPrice();
        return payment;
    }
    
    protected void MailButton_Click(object sender, EventArgs e)
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
            CheckoutResponse checkoutResponse = Token.Instance.User.Basket.Checkout(checkoutRequest);
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
    }

</script>


<table class="paymentForm">
    <tr>
        <th class="caption">
            <asp:Label ID="Caption" runat="server" Text="Pay by {0}"></asp:Label>
        </th>
    </tr>
    <tr>
        <td class="content">
		    <p align="justify"><asp:Label ID="MailHelpText" runat="server" Text="Click below to submit your order.  You can then print an invoice and send in your payment."></asp:Label></p>
        </td>
    </tr>
    <tr>
        <td class="submit">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="MailFax" Visible="false" />
            <asp:Button ID="MailButton" runat="server" Text="Pay by {0}" OnClick="MailButton_Click" ValidationGroup="MailFax" />
        </td>
    </tr>
</table>