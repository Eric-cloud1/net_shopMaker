<%@ Control Language="C#" ClassName="ZeroValuePaymentForm" %>

<script runat="server">
    //DEFINE EVENTS TO TRIGGER FOR CHECKOUT
    public event CheckingOutEventHandler CheckingOut;
    public event CheckedOutEventHandler CheckedOut;

    private string _ValidationGroup = "ZeroValue";
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

    private void UpdateValidationOptions()
    {
        CompleteButton.ValidationGroup = _ValidationGroup;
        ValidationSummary1.ValidationGroup = _ValidationGroup;
        ValidationSummary1.Visible = _ValidationSummaryVisible;
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        UpdateValidationOptions();
    }

    protected void CompleteButton_Click(object sender, EventArgs e)
    {
        bool checkOut = true;
        if (CheckingOut != null)
        {
            CheckingOutEventArgs c = new CheckingOutEventArgs();
            CheckingOut(this, c);
            checkOut = !c.Cancel;
        }
        if (checkOut)
        {
            Basket basket = Token.Instance.User.Basket;
            //PROCESS THE CHECKOUT
            CheckoutRequest checkoutRequest = new CheckoutRequest(null);
            CheckoutResponse checkoutResponse = basket.Checkout(checkoutRequest);
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
        <td class="content">
		    <p align="justify">
		        <asp:Localize ID="NoValueHelpText" runat="server" Text="There is no charge for your item(s).  Click below to complete your order." EnableViewState="False"></asp:Localize>
		    </p>
        </td>
    </tr>
    <tr>
        <td class="submit">
            <asp:ValidationSummary runat="server" ID="ValidationSummary1" ValidationGroup="ZeroValue" />
            <asp:Button ID="CompleteButton" runat="server" Text="Complete Order" OnClick="CompleteButton_Click" EnableViewState="false" ValidationGroup="ZeroValue" />
        </td>
    </tr>
</table>