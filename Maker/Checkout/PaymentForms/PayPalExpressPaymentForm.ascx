<%@ Control Language="C#" ClassName="PayPalExpressPaymentForm" %>
<%@ Import Namespace="MakerShop.Payments.Providers.PayPal" %>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        ExpressCheckoutSession paypalSession = ExpressCheckoutSession.Current;
        PayPalAccount.Text = paypalSession.Payer;
        PayPalButton.ImageUrl = string.Format(PayPalButton.ImageUrl, Request.Url.Scheme);
    }

    protected void PayPalButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/PayPalExpressCheckout.aspx?Action=DO");
    }
</script>

<table class="paymentForm">
    <tr>
        <th class="caption">
            <asp:Label ID="Caption" runat="server" Text="Pay with PayPal"></asp:Label>
        </th>
    </tr>
    <tr>
        <td class="content">
            <p align="justify"><asp:Label ID="PayPalExpressHelpText" runat="server" Text="Click below to place your order.  Your payment will be made automatically using the PayPal funding source(s) you have supplied.  If you change your mind and wish to use another form of payment, click the 'change' link below."></asp:Label></p><br />
            <div style="text-indent:20px">
                <asp:Label ID="PayPalAccountLabel" runat="server" Text="PayPal Account: " AssociatedControlID="PayPalAccount" SkinID="FieldHeader"></asp:Label>
                <asp:Label ID="PayPalAccount" runat="server" Text=""></asp:Label>
                <asp:HyperLink ID="ChangeLink" runat="server" Text="change" NavigateUrl="~/PayPalExpressCheckout.aspx?Action=CANCEL&ReturnURL=PAY"></asp:HyperLink>
            </div>
        </td>
    </tr>
    <tr>
        <td class="submit">
            <asp:ImageButton ID="PayPalButton" runat="server" ImageUrl="{0}://www.paypal.com/en_US/i/btn/x-click-but1.gif" CausesValidation="false" OnClick="PayPalButton_Click" />
        </td>
    </tr>
</table>

