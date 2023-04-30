<%@ Control Language="C#" ClassName="CheckPaymentForm" EnableViewState="false" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>

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

    private string _ValidationGroup = "Check";
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

    private bool _AllowAmountEntry = false;
    public bool AllowAmountEntry
    {
        get { return _AllowAmountEntry; }
        set { _AllowAmountEntry = value; }
    }

    private LSDecimal _PaymentAmount = 0;
    public LSDecimal PaymentAmount
    {
        get { return _PaymentAmount; }
        set { _PaymentAmount = value; }
    }

    private void UpdateValidationOptions()
    {
        AccountHolder.ValidationGroup = _ValidationGroup;
        AccountHolderValidator.ValidationGroup = _ValidationGroup;
        BankName.ValidationGroup = _ValidationGroup;
        BankNameRequiredValidator.ValidationGroup = _ValidationGroup;
        RoutingNumber.ValidationGroup = _ValidationGroup;
        RoutingNumberValidator.ValidationGroup = _ValidationGroup;
        RoutingNumberValidator2.ValidationGroup = _ValidationGroup;
        SortCode.ValidationGroup = _ValidationGroup;
        SortCodeValidator.ValidationGroup = _ValidationGroup;
        AccountNumber.ValidationGroup = _ValidationGroup;
        AccountNumberValidator.ValidationGroup = _ValidationGroup;
        CheckButton.ValidationGroup = _ValidationGroup;
        ValidationSummary1.ValidationGroup = _ValidationGroup;
        ValidationSummary1.Visible = _ValidationSummaryVisible;
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        UpdateValidationOptions();
        trRoutingNumber.Visible = (Token.Instance.User.PrimaryAddress.CountryCode == "US");
        trSortCode.Visible = !trRoutingNumber.Visible;
    }

    private PaymentMethod _PaymentMethod;
    protected void Page_Load(object sender, EventArgs e)
    {
        _PaymentMethod = PaymentMethodDataSource.Load(this.PaymentMethodId);
        if (_PaymentMethod != null)
        {
            Caption.Text = string.Format(Caption.Text, _PaymentMethod.Name);
            CheckButton.Text = string.Format(CheckButton.Text, _PaymentMethod.Name);
            if (!Page.IsPostBack)
            {
                AccountHolder.Text = Token.Instance.User.PrimaryAddress.FullName;
            }
            trAmount.Visible = this.AllowAmountEntry;
            DisableAutoComplete();
        }
        else
        {
            this.Controls.Clear();
            Trace.Write(this.GetType().ToString(), "Output suppressed, invalid payment method provided.");
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (trAmount.Visible && string.IsNullOrEmpty(Amount.Text))
        {
            Amount.Text = GetPaymentAmount().ToString("F2");
        }
        CheckButton.OnClientClick = "if(Page_ClientValidate('" + this.ValidationGroup + "')){this.value='Processing...';this.onclick='return false;'}";
    }

    private LSDecimal GetPaymentAmount()
    {
        if (Page.IsPostBack)
        {
            if (trAmount.Visible && !string.IsNullOrEmpty(Amount.Text)) return AlwaysConvert.ToDecimal(Amount.Text);
            else if (this.PaymentAmount > 0) return this.PaymentAmount;
            else return Token.Instance.User.Basket.Items.TotalPrice();
        }
        else
        {
            if (this.PaymentAmount > 0) return this.PaymentAmount;
            return Token.Instance.User.Basket.Items.TotalPrice();
        }
    }

    private Payment GetPayment()
    {
        Payment payment = new Payment();
        payment.PaymentMethodId = _PaymentMethodId;
        payment.Amount = GetPaymentAmount();
        AccountDataDictionary instrumentBuilder = new AccountDataDictionary();
        instrumentBuilder["AccountHolder"] = StringHelper.StripHtml(AccountHolder.Text);
        instrumentBuilder["BankName"] = StringHelper.StripHtml(BankName.Text);
        instrumentBuilder["RoutingNumber"] = RoutingNumber.Text;
        instrumentBuilder["AccountNumber"] = AccountNumber.Text;
        payment.ReferenceNumber = Payment.GenerateReferenceNumber(AccountNumber.Text);
        payment.AccountData = instrumentBuilder.ToString();
        return payment;
    }

    protected void CheckButton_Click(object sender, EventArgs e)
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
        else CheckButton.Text = string.Format("Pay by {0}", _PaymentMethod.Name);
    }
    
    private void DisableAutoComplete()
    {
        AccountHolder.Attributes.Add("autocomplete", "off");
        BankName.Attributes.Add("autocomplete", "off");
        RoutingNumber.Attributes.Add("autocomplete", "off");
        AccountNumber.Attributes.Add("autocomplete", "off");
    }
</script>

<asp:Panel runat="server" DefaultButton="CheckButton">
<table class="paymentForm">
    <tr>
        <th class="caption" colspan="3">
            <asp:Label ID="Caption" runat="server" Text="Pay by {0}"></asp:Label>
        </th>
    </tr>
    <tr>
        <td class="content" colspan="3">
            <asp:Label ID="CheckHelpText" runat="server" Text="Enter your checking account information below."></asp:Label>
        </td>
    </tr>
    <tr id="trAmount" runat="server">
        <th class="rowHeader">
            <asp:Label ID="AmountLabel" runat="server" Text="Amount:"></asp:Label>
        </th>
        <td>
            <asp:TextBox ID="Amount" runat="server" Text="" Width="60px" MaxLength="10" ValidationGroup="Check"></asp:TextBox>
            <asp:RequiredFieldValidator ID="AmountRequired" runat="server" Text="*"
                ErrorMessage="Amount is required." Display="Static" ControlToValidate="Amount"
                ValidationGroup="Check"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <asp:Label ID="AccountHolderLabel" runat="server" Text="Account Holder:" AssociatedControlID="AccountHolder"></asp:Label>
	    </th>
        <td>
            <asp:TextBox id="AccountHolder" runat="server" MaxLength="50" ValidationGroup="Check"></asp:TextBox>
            <asp:RequiredFieldValidator ID="AccountHolderValidator" runat="server" ErrorMessage="You must enter the account holder name." 
                ControlToValidate="AccountHolder" Display="Static" Text="*" ValidationGroup="Check" ></asp:RequiredFieldValidator>
        </td>
        <td rowspan="5">
            <asp:Image ID="CheckHelpImage" runat="server" ImageUrl="~/images/PaymentInstruments/checkhelp.jpg" Width="250" />
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <asp:Label ID="BankNameLabel" runat="server" Text="Bank Name:" AssociatedControlID="BankName"></asp:Label>
	    </th>
        <td>
            <asp:TextBox id="BankName" runat="server" MaxLength="50" ValidationGroup="Check"></asp:TextBox>
            <asp:RequiredFieldValidator ID="BankNameRequiredValidator" runat="server" ErrorMessage="You must enter the bank name." 
                ControlToValidate="BankName" Display="Static" Text="*" ValidationGroup="Check" ></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr id="trRoutingNumber" runat="server">
        <th class="rowHeader">
            <asp:Label ID="RoutingNumberLabel" runat="server" Text="Routing Number:" AssociatedControlID="RoutingNumber"></asp:Label>
	    </th>
        <td>
            <asp:TextBox id="RoutingNumber" runat="server" MaxLength="9" ValidationGroup="Check"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RoutingNumberValidator2" runat="server" ErrorMessage="You must enter a valid routing number." 
                ControlToValidate="RoutingNumber" Display="Static" Text="*" ValidationGroup="Check" ></asp:RequiredFieldValidator>
            <cb:RoutingNumberValidator ID="RoutingNumberValidator" runat="server" ErrorMessage="You must enter a valid routing number."
                ControlToValidate="RoutingNumber" Display="Static" Text="*" ValidationGroup="Check" ></cb:RoutingNumberValidator>
        </td>
    </tr>
    <tr id="trSortCode" runat="server">
        <th class="rowHeader">
            <asp:Label ID="SortCodeLabel" runat="server" Text="Sort Code:" AssociatedControlID="SortCode"></asp:Label>
	    </th>
        <td>
            <asp:TextBox id="SortCode" runat="server" MaxLength="20" ValidationGroup="Check"></asp:TextBox>
            <asp:RequiredFieldValidator ID="SortCodeValidator" runat="server" ErrorMessage="You must enter a bank sort code or transit/routing number."
                ControlToValidate="SortCode" Display="Static" Text="*" ValidationGroup="Check"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <asp:Label ID="AccountNumberLabel" runat="server" Text="Account Number:" AssociatedControlID="AccountNumber"></asp:Label>
	    </th>
        <td>
            <asp:TextBox id="AccountNumber" runat="server" ValidationGroup="Check"></asp:TextBox>
            <asp:RequiredFieldValidator ID="AccountNumberValidator" runat="server" ErrorMessage="You must enter the account number." 
                ControlToValidate="AccountNumber" Display="Static" Text="*" ValidationGroup="Check" ></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td class="submit" colspan="2">
            <asp:ValidationSummary runat="server" ID="ValidationSummary1" ValidationGroup="Check" />
            <asp:Button ID="CheckButton" runat="server" Text="Pay by {0}" ValidationGroup="Check" OnClick="CheckButton_Click" EnableViewState="false"/>
        </td>
    </tr>
</table>
</asp:Panel>