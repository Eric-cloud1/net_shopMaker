<%@ Control Language="C#" ClassName="CreditCardPaymentForm" EnableViewState="false" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register assembly="wwhoverpanel" Namespace="Westwind.Web.Controls" TagPrefix="wwh" %>
<script runat="server">
    //DEFINE EVENTS TO TRIGGER FOR CHECKOUT
    public event CheckingOutEventHandler CheckingOut;
    public event CheckedOutEventHandler CheckedOut;

    private string _ValidationGroup = "CreditCard";
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
        Amount.ValidationGroup = _ValidationGroup;
        AmountRequired.ValidationGroup = _ValidationGroup;
        CardType.ValidationGroup = _ValidationGroup;
        CardTypeRequired.ValidationGroup = _ValidationGroup;
        CardName.ValidationGroup = _ValidationGroup;
        CardNameRequired.ValidationGroup = _ValidationGroup;
        CardNumber.ValidationGroup = _ValidationGroup;
        CardNumberValidator1.ValidationGroup = _ValidationGroup;
        ExpirationDropDownValidator1.ValidationGroup = _ValidationGroup;
        CardNumberValidator2.ValidationGroup = _ValidationGroup;
        ExpirationMonth.ValidationGroup = _ValidationGroup;
        MonthValidator.ValidationGroup = _ValidationGroup;
        ExpirationYear.ValidationGroup = _ValidationGroup;
        YearValidator.ValidationGroup = _ValidationGroup;
        SecurityCode.ValidationGroup = _ValidationGroup;
        SecurityCodeValidator.ValidationGroup = _ValidationGroup;
        SecurityCodeValidator2.ValidationGroup = _ValidationGroup;
        IssueNumber.ValidationGroup = _ValidationGroup;
        StartDateMonth.ValidationGroup = _ValidationGroup;
        StartDateMonth.ValidationGroup = _ValidationGroup;
        IntlDebitValidator1.ValidationGroup = _ValidationGroup;
        IntlDebitValidator2.ValidationGroup = _ValidationGroup;
        StartDateValidator1.ValidationGroup = _ValidationGroup;
        CreditCardButton.ValidationGroup = _ValidationGroup;
        ValidationSummary1.ValidationGroup = _ValidationGroup;
        ValidationSummary1.Visible = _ValidationSummaryVisible;
    }

    private string FormatCardNames(List<string> cardNames)
    {
        if (cardNames == null || cardNames.Count == 0) return string.Empty;
        if (cardNames.Count == 1) return cardNames[0];
        string formattedNames = string.Join(", ", cardNames.ToArray());
        int lastComma = formattedNames.LastIndexOf(", ");
        string leftSide = formattedNames.Substring(0, lastComma);
        string rightSide = formattedNames.Substring(lastComma + 1);
        return leftSide + ", and" + rightSide;
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        UpdateValidationOptions();
        //SET THE DEFAULT NAME
        CardName.Text = Token.Instance.User.PrimaryAddress.FullName;
        //POPULATE EXPIRATON DATE DROPDOWN
        int thisYear = LocaleHelper.LocalNow.Year;
        for (int i = 0; (i <= 10); i++)
        {
            ExpirationYear.Items.Add(new ListItem((thisYear + i).ToString()));
        }
        //POPULATE START DATE DROPDOWN
        for (int i = 1997; (i <= thisYear); i++)
        {
            StartDateYear.Items.Add(new ListItem(i.ToString()));
        }
        //LOAD AVAILABLE PAYMENT METHODS
        PaymentMethodCollection methods = StoreDataHelper.GetPaymentMethods(Token.Instance.UserId);
        List<string> creditCards = new List<string>();
        List<string> intlDebitCards = new List<string>();
        foreach (PaymentMethod method in methods)
        {
            if (method.IsCreditOrDebitCard())
            {
                CardType.Items.Add(new ListItem(method.Name, method.PaymentMethodId.ToString()));
                if (method.IsIntlDebitCard()) intlDebitCards.Add(method.Name);
                else creditCards.Add(method.Name);
            }
        }
        //HIDE THIS CONTROL IF THERE ARE NO CREDIT CARD PAYMENT METHODS
        if (CardType.Items.Count == 1)
        {
            Trace.Write(this.GetType().ToString(), "Output suppressed, no credit card payment methods detected.");
            this.Controls.Clear();
        }
        else
        {
            //SHOW OR HIDE INTL DEBIT FIELDS
            if (intlDebitCards.Count > 0)
            {
                trIntlCVV.Visible = true;
                if (creditCards.Count > 0)
                {
                    IntlCVVCredit.Visible = true;
                    IntlCVVCredit.Text = string.Format(IntlCVVCredit.Text, FormatCardNames(creditCards));
                }
                else IntlCVVCredit.Visible = false;
                IntlCVVDebit.Text = string.Format(IntlCVVDebit.Text, FormatCardNames(intlDebitCards));
                SecurityCodeValidator.Enabled = false;
                trIntlInstructions.Visible = true;
                IntlInstructions.Text = string.Format(IntlInstructions.Text, FormatCardNames(intlDebitCards));
                trIssueNumber.Visible = true;
                trStartDate.Visible = true;
            }
            else
            {
                trIntlCVV.Visible = false;
                trIntlInstructions.Visible = false;
                trIssueNumber.Visible = false;
                trStartDate.Visible = false;
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        trAmount.Visible = this.AllowAmountEntry;
        DisableAutoComplete();

        // PREVENT DOUBLE POSTING
        StringBuilder submitScript = new StringBuilder();
        submitScript.Append("if(document.getElementById('" + FormIsSubmitted.ClientID + "').value==1 || Page_ClientValidate('CreditCard') == false) { return false; } ");
        submitScript.Append("this.disabled = true; ");
        submitScript.Append("document.getElementById('" + FormIsSubmitted.ClientID + "').value=1;");
        //GetPostBackEventReference obtains a reference to a client-side script function that causes the server to post back to the page.
        submitScript.Append(Page.ClientScript.GetPostBackEventReference(CreditCardButton, string.Empty));
        submitScript.Append(";");
        submitScript.Append("return false;");
        this.CreditCardButton.Attributes.Add("onclick", submitScript.ToString());         
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (trAmount.Visible && string.IsNullOrEmpty(Amount.Text))
        {
            Amount.Text = GetPaymentAmount().ToString("F2");
        }
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
        payment.PaymentMethodId = AlwaysConvert.ToInt(CardType.SelectedValue);
        payment.Amount = GetPaymentAmount();
        AccountDataDictionary instrumentBuilder = new AccountDataDictionary();
        instrumentBuilder["AccountName"] = CardName.Text;
        instrumentBuilder["AccountNumber"] = CardNumber.Text;
        instrumentBuilder["ExpirationMonth"] = ExpirationMonth.SelectedItem.Value;
        instrumentBuilder["ExpirationYear"] = ExpirationYear.SelectedItem.Value;
        instrumentBuilder["SecurityCode"] = SecurityCode.Text;
        if (payment.PaymentMethod.IsIntlDebitCard())
        {
            if (IssueNumber.Text.Length > 0) instrumentBuilder["IssueNumber"] = IssueNumber.Text;
            if ((StartDateMonth.SelectedIndex > 0) && (StartDateYear.SelectedIndex > 0))
            {
                instrumentBuilder["StartDateMonth"] = StartDateMonth.SelectedItem.Value;
                instrumentBuilder["StartDateYear"] = StartDateYear.SelectedItem.Value;
            }
        }
        payment.ReferenceNumber = Payment.GenerateReferenceNumber(CardNumber.Text);
        payment.AccountData = instrumentBuilder.ToString();
        return payment;
    }

    private bool CustomValidation()
    {
        //if intl instructions are visible, we must validate additional rules
        if (trIntlInstructions.Visible)
        {
            PaymentMethod m = PaymentMethodDataSource.Load(AlwaysConvert.ToInt(CardType.SelectedValue));
            if (m != null)
            {
                if (m.IsIntlDebitCard())
                {
                    //INTERNATIONAL DEBIT CARD, ISSUE NUMBER OR START DATE REQUIRED
                    bool invalidIssueNumber = (!Regex.IsMatch(IssueNumber.Text, "\\d{1,2}"));
                    bool invalidStartDate = ((StartDateMonth.SelectedIndex == 0) || (StartDateYear.SelectedIndex == 0));
                    if (invalidIssueNumber && invalidStartDate)
                    {
                        IntlDebitValidator1.IsValid = false;
                        IntlDebitValidator2.IsValid = false;
                        return false;
                    }
                    //CHECK START DATE IS IN PAST
                    int selYear = AlwaysConvert.ToInt(StartDateYear.SelectedValue);
                    int curYear = DateTime.Now.Year;
                    if (selYear > curYear)
                    {
                        StartDateValidator1.IsValid = false;
                        return false;
                    }
                    else if (selYear == curYear)
                    {
                        int selMonth = AlwaysConvert.ToInt(StartDateMonth.SelectedValue);
                        int curMonth = DateTime.Now.Month;
                        if (selMonth > curMonth)
                        {
                            StartDateValidator1.IsValid = false;
                            return false;
                        }
                    }
                }
                else
                {
                    //CREDIT CARD, CVV IS REQUIRED
                    if (!Regex.IsMatch(SecurityCode.Text, "\\d{3,4}"))
                    {
                        SecurityCodeValidator2.IsValid = false;
                        return false;
                    }
                }
            }
        }
        return true;
    }
    
    protected void CreditCardButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid && CustomValidation())
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
                //PROCESS A CHECKOUT
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
        else CreditCardButton.Text = "Pay With Card";

        // IF NOT SUCCESSFULL / ENABLE THE CHECKOUT BUTTON
        CreditCardButton.Enabled = true;
        FormIsSubmitted.Value = "0";
    }

    private void DisableAutoComplete()
    {        
        CardNumber.Attributes.Add("autocomplete", "off");
        SecurityCode.Attributes.Add("autocomplete", "off");
        IssueNumber.Attributes.Add("autocomplete", "off");
    }

</script>

<table class="paymentForm">
    <tr>
        <th class="caption" colspan="2">
            <asp:Label ID="Caption" runat="server" Text="Pay With a Credit or Debit Card"></asp:Label>
        </th>
    </tr>
    <tr>
        <td class="pFcontent" colspan="2">
            <asp:ValidationSummary runat="server" ID="ValidationSummary1" ValidationGroup="CreditCard" />
        </td>
    </tr>
    <tr id="trAmount" runat="server">
        <th class="rowHeader">
            <asp:Label ID="AmountLabel" runat="server" Text="Amount:"></asp:Label>
        </th>
        <td>
            <asp:TextBox ID="Amount" runat="server" Text="" Width="60px" MaxLength="10" ValidationGroup="CreditCard"></asp:TextBox>
            <asp:RequiredFieldValidator ID="AmountRequired" runat="server" Text="*"
                ErrorMessage="Amount is required." Display="Static" ControlToValidate="Amount"
                ValidationGroup="CreditCard"></asp:RequiredFieldValidator>
            <asp:PlaceHolder ID="phAmount" runat="server"></asp:PlaceHolder>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <asp:Label ID="CardTypeLabel" runat="server" Text="Card Type:"></asp:Label>
        </th>
        <td>
            <asp:DropDownList ID="CardType" runat="server" DataTextField="Name" DataValueField="PaymentMethodId" ValidationGroup="CreditCard">
                <asp:ListItem></asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="CardTypeRequired" runat="server" Text="*"
                ErrorMessage="Card type is required." Display="Static" ControlToValidate="CardType"
                ValidationGroup="CreditCard"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <asp:Label ID="CardNameLabel" runat="server" Text="Name&nbsp;on&nbsp;Card:" AssociatedControlID="CardName"></asp:Label>
        </th>
        <td>
            <asp:TextBox ID="CardName" runat="server" MaxLength="50" ValidationGroup="CreditCard" AutoCompleteType="Disabled"></asp:TextBox>
            <asp:RequiredFieldValidator ID="CardNameRequired" runat="server" 
                ErrorMessage="You must enter the name as it appears on the card." 
                ControlToValidate="CardName" Display="Static" Text="*" ValidationGroup="CreditCard"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <asp:Label ID="CardNumberLabel" runat="server" Text="Card Number:" AssociatedControlID="CardNumber"></asp:Label>
        </th>
        <td>
            <asp:TextBox ID="CardNumber" runat="server" MaxLength="19" ValidationGroup="CreditCard"></asp:TextBox>
            <cb:CreditCardValidator ID="CardNumberValidator1" runat="server" 
                ControlToValidate="CardNumber" ErrorMessage="You must enter a valid card number."
                Display="Dynamic" Text="*" ValidationGroup="CreditCard"></cb:CreditCardValidator>
            <cb:RequiredRegularExpressionValidator ID="CardNumberValidator2" runat="server" ValidationExpression="\d{12,19}"
                ErrorMessage="Card number is required and should be between 12 and 19 digits (no dashes or spaces)." ControlToValidate="CardNumber"
                Display="Static" Text="*" Required="true" ValidationGroup="CreditCard"></cb:RequiredRegularExpressionValidator>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <asp:Label ID="ExpirationLabel" runat="server" Text="Expiration:" AssociatedControlID="ExpirationMonth"></asp:Label>
        </th>
        <td>
            <asp:dropdownlist ID="ExpirationMonth" runat="server" ValidationGroup="CreditCard">
                <asp:ListItem Text="Month" Value=""></asp:ListItem>
                <asp:ListItem Value="01">01</asp:ListItem>
                <asp:ListItem Value="02">02</asp:ListItem>
                <asp:ListItem Value="03">03</asp:ListItem>
                <asp:ListItem Value="04">04</asp:ListItem>
                <asp:ListItem Value="05">05</asp:ListItem>
                <asp:ListItem Value="06">06</asp:ListItem>
                <asp:ListItem Value="07">07</asp:ListItem>
                <asp:ListItem Value="08">08</asp:ListItem>
                <asp:ListItem Value="09">09</asp:ListItem>
                <asp:ListItem Value="10">10</asp:ListItem>
                <asp:ListItem Value="11">11</asp:ListItem>
                <asp:ListItem Value="12">12</asp:ListItem>
            </asp:dropdownlist>
            <asp:RequiredFieldValidator ID="MonthValidator" runat="server" ErrorMessage="You must select the expiration month." 
                ControlToValidate="ExpirationMonth" Display="Static" Text="*" ValidationGroup="CreditCard"></asp:RequiredFieldValidator>
            <asp:dropdownlist ID="ExpirationYear" Runat="server" ValidationGroup="CreditCard">
                <asp:ListItem Text="Year" Value=""></asp:ListItem>
            </asp:dropdownlist>
            <cb:expirationdropdownvalidator ID="ExpirationDropDownValidator1" runat="server"
                Display="Dynamic" errormessage="You must enter an expiration date in the future."
                monthcontroltovalidate="ExpirationMonth" yearcontroltovalidate="ExpirationYear"
                Text="*" ValidationGroup="CreditCard"></cb:expirationdropdownvalidator>
            <asp:RequiredFieldValidator ID="YearValidator" runat="server" ErrorMessage="You must select the expiration year." 
                ControlToValidate="ExpirationYear" Display="Static" Text="*" ValidationGroup="CreditCard"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr id="trIntlCVV" runat="server" visible="true">
        <td colspan="2">
            <asp:Literal ID="IntlCVVCredit" runat="server" Text="For {0} cards, the Verification Code is required.  "></asp:Literal>
            <asp:Literal ID="IntlCVVDebit" runat="server" Text="For {0} cards the Verification Code is optional - enter the number only if present on your card."></asp:Literal>
        </td>
    </tr>
    <tr>
        <th class="rowHeader" valign="top">
            <asp:Label ID="SecurityCodeLabel" runat="server" Text="Verification Code:" AssociatedControlID="SecurityCode"></asp:Label>
        </th>
        <td>
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td rowspan="2" valign="top" nowrap>
                        <asp:TextBox ID="SecurityCode" runat="server" Columns="4" MaxLength="4" ValidationGroup="CreditCard" AutoCompleteType="Disabled"></asp:TextBox>
                        <cb:RequiredRegularExpressionValidator ID="SecurityCodeValidator" runat="server" ValidationExpression="\d{3,4}"
                            ErrorMessage="Card security code should be a 3 or 4 digit number." ControlToValidate="SecurityCode"
                            Display="Static" Text="*" Required="true" ValidationGroup="CreditCard"></cb:RequiredRegularExpressionValidator>
                        <asp:CustomValidator ID="SecurityCodeValidator2" runat="server" Text="*" 
                            ErrorMessage="Card security code should be a 3 or 4 digit number." ValidationGroup="CreditCard"></asp:CustomValidator>
                    </td>
                    <td valign="top">
                        <a Name="cvv_visa" href="#cvv_visa" onmouseover='CVV_VISA_HoverLookupPanel.startCallback(event,"CVVVISA",null,null);' onmouseout='CVV_VISA_HoverLookupPanel.hide();'><asp:Image ID="CID" runat="server" ImageUrl="~/images/PaymentInstruments/cid.gif" hspace="4"  /></a>
                    </td>
                    <td valign="top">
                        <asp:Label ID="CIDHelpText" runat="server" Text="For Discover, MasterCard, and Visa, enter last three digits on the signature strip."></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <a Name="cvv_amex" href="#cvv_amex" onmouseover='CVV_AMEX_HoverLookupPanel.startCallback(event,"CVVAMES",null,null);' onmouseout='CVV_AMEX_HoverLookupPanel.hide();'><asp:Image ID="AmexCID" runat="server" ImageUrl="~/images/PaymentInstruments/cid_amex.gif" hspace="4" /></a>
                    </td>
                    <td valign="top">
                        <asp:Label ID="AmexCIDHelpText" runat="server" Text="For American Express, enter the four digits in small print on the front of the card."></asp:Label>
                    </td>
                </tr>
            </table>    
        </td>
    </tr>
    <tr id="trIntlInstructions" runat="server" visible="true">
        <td colspan="2">
            <asp:Literal ID="IntlInstructions" runat="server" Text="Issue number and/or Start Date only apply to {0} cards.  Enter the value(s) if present on your card."></asp:Literal>
        </td>
    </tr>
    <tr id="trIssueNumber" runat="server" visible="true">
        <th class="rowHeader">
            <asp:Label ID="IssueNumberLabel" runat="server" Text="Issue Number:" AssociatedControlID="IssueNumber" SkinID="fieldHeader"></asp:Label>
        </th>
        <td>
            <asp:TextBox ID="IssueNumber" runat="server" MaxLength="2" Width="40px"></asp:TextBox>
            <asp:CustomValidator ID="IntlDebitValidator1" runat="server" Text="*" ValidationGroup="CreditCard"></asp:CustomValidator>
        </td>
    </tr>
    <tr id="trStartDate" runat="server" visible="true">
        <th class="rowHeader">
            <asp:Label ID="StartDateLabel" runat="server" Text="OR Start Date: " AssociatedControlID="StartDateMonth" SkinID="fieldheader"></asp:Label>
        </th>
        <td>
            <asp:dropdownlist ID="StartDateMonth" runat="server" ValidationGroup="IntlCard">
                <asp:ListItem Text="Month" Value=""></asp:ListItem>
                <asp:ListItem Value="01">01</asp:ListItem>
                <asp:ListItem Value="02">02</asp:ListItem>
                <asp:ListItem Value="03">03</asp:ListItem>
                <asp:ListItem Value="04">04</asp:ListItem>
                <asp:ListItem Value="05">05</asp:ListItem>
                <asp:ListItem Value="06">06</asp:ListItem>
                <asp:ListItem Value="07">07</asp:ListItem>
                <asp:ListItem Value="08">08</asp:ListItem>
                <asp:ListItem Value="09">09</asp:ListItem>
                <asp:ListItem Value="10">10</asp:ListItem>
                <asp:ListItem Value="11">11</asp:ListItem>
                <asp:ListItem Value="12">12</asp:ListItem>
            </asp:dropdownlist>
            <asp:dropdownlist ID="StartDateYear" Runat="server" ValidationGroup="IntlCard">
                <asp:ListItem Text="Year" Value=""></asp:ListItem>
            </asp:dropdownlist>
            <asp:CustomValidator ID="IntlDebitValidator2" runat="server" Text="*" ErrorMessage="Issue number or start date is required." ValidationGroup="CreditCard"></asp:CustomValidator>
            <asp:CustomValidator ID="StartDateValidator1" runat="server" ErrorMessage="You cannot select a start date in the future." 
                ControlToValidate="StartDateYear" Display="Static" Text="*" ValidationGroup="CreditCard"></asp:CustomValidator>
        </td>
    </tr>
    <tr>
        <td class="submit" colspan="2">
            <asp:Button ID="CreditCardButton" runat="server" Text="Pay With Card" ValidationGroup="CreditCard" OnClick="CreditCardButton_Click"  />
            <asp:HiddenField runat="server"  ID="FormIsSubmitted" value="0" />
        </td>
    </tr>
</table>
<wwh:wwHoverPanel ID="CVV_AMEX_HoverLookupPanel"
    runat="server" 
    serverurl="~/Checkout/PaymentForms/cvv_amex.htm"
    Navigatedelay="1000"              
    scriptlocation="WebResource"
    style="display: none; background: white;" 
    panelopacity="0.89" 
    shadowoffset="8"
    shadowopacity="0.18"
    PostBackMode="None"
    AdjustWindowPosition="true">
</wwh:wwHoverPanel>
<wwh:wwHoverPanel ID="CVV_VISA_HoverLookupPanel"
    runat="server" 
    serverurl="~/Checkout/PaymentForms/cvv_visa.htm"
    Navigatedelay="1000"              
    scriptlocation="WebResource"
    style="display: none; background: white;" 
    panelopacity="0.89" 
    shadowoffset="8"
    shadowopacity="0.18"
    PostBackMode="None"
    AdjustWindowPosition="true">
</wwh:wwHoverPanel>
