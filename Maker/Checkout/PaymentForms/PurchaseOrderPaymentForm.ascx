<%@ Control Language="C#" ClassName="PurchaseOrderPaymentForm" %>

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

    private string _ValidationGroup = "PurchaseOrder";
    public string ValidationGroup
    {
        get { return _ValidationGroup; }
        set { _ValidationGroup = value; }
    }

    private bool _ValidationSummaryVisible = true;
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
        PurchaseOrderNumber.ValidationGroup = _ValidationGroup;
        PurchaseOrderNumberRequired.ValidationGroup = _ValidationGroup;
        PurchaseOrderButton.ValidationGroup = _ValidationGroup;
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
            PurchaseOrderButton.Text = string.Format(PurchaseOrderButton.Text, method.Name);
        }
        else
        {
            this.Controls.Clear();
            Trace.Write(this.GetType().ToString(), "Output suppressed, invalid payment method provided.");
        }
        // DISABLE AUTOCOMPLETE
        PurchaseOrderNumber.Attributes.Add("autocomplete", "off");
    }

    private Payment GetPayment()
    {
        Payment payment = new Payment();
        payment.PaymentMethodId = _PaymentMethodId;
        if (this.PaymentAmount > 0) payment.Amount = this.PaymentAmount;
        else payment.Amount = Token.Instance.User.Basket.Items.TotalPrice();
        AccountDataDictionary instrumentBuilder = new AccountDataDictionary();
        PurchaseOrderNumber.Text = StringHelper.StripHtml(PurchaseOrderNumber.Text);
        instrumentBuilder["PurchaseOrderNumber"] = PurchaseOrderNumber.Text;
        payment.ReferenceNumber = PurchaseOrderNumber.Text;
        payment.AccountData = instrumentBuilder.ToString();
        return payment;
    }

    protected void PurchaseOrderButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
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
    }
</script>

<table class="paymentForm">
    <tr>
        <th class="caption" colspan="2">
            <asp:Label ID="Caption" runat="server" Text="Pay by {0}"></asp:Label>
        </th>
    </tr>
    <tr>
        <td class="content" colspan="2">
            <asp:Label ID="PurchaseOrderHelpText" runat="server" Text="Enter the purchase order number below."></asp:Label>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <asp:Label ID="PurchaseOrderNumberLabel" runat="server" Text="PO #:" AssociatedControlID="PurchaseOrderNumber"></asp:Label>
        </th>
        <td>
            <asp:TextBox ID="PurchaseOrderNumber" runat="server" MaxLength="50" ValidationGroup="PurchaseOrder"></asp:TextBox>
            <asp:RequiredFieldValidator ID="PurchaseOrderNumberRequired" runat="server" 
                ErrorMessage="You must enter the purchase order number." 
                ControlToValidate="PurchaseOrderNumber" Display="Static" Text="*" ValidationGroup="PurchaseOrder"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td class="submit" colspan="2">
            <asp:ValidationSummary runat="server" ID="ValidationSummary1" ValidationGroup="PurchaseOrder" />
            <asp:Button ID="PurchaseOrderButton" runat="server" Text="Pay by {0}" ValidationGroup="PurchaseOrder" OnClick="PurchaseOrderButton_Click" />
        </td>
    </tr>
</table>
