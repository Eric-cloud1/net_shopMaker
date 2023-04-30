<%@ Page Language="C#" MasterPageFile="~/Layouts/OneColumn.master" CodeFile="PayPalExpressCheckout.aspx.cs" Inherits="PayPalExpressCheckout" title="PayPal Express Checkout" EnableViewState="False" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <h1 class="heading">PayPal Express Checkout Failure</h1>
    </div>
    <div class="section">
		<br /><asp:BulletedList ID="ErrorList" runat="server"></asp:BulletedList><br />
	    <asp:HyperLink ID="RetryLink" runat="server" Text="Retry PayPal Request" NavigateUrl="PayPalExpressCheckout.aspx?action=RETRY" SkinID="Button"></asp:HyperLink>&nbsp;
	    <asp:HyperLink ID="CancelLink" runat="server" Text="Cancel Use of PayPal" NavigateUrl="PayPalExpressCheckout.aspx?action=CANCEL" SkinID="Button"></asp:HyperLink>
	    <asp:HyperLink ID="BasketLink" runat="server" Text="View Cart" NavigateUrl="PayPalExpressCheckout.aspx?action=CANCEL" SkinID="Button" Visible="false"></asp:HyperLink>
    </div>
</asp:Content>

