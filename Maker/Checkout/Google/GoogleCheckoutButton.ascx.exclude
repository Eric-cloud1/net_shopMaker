<%@ Control Language="C#" ClassName="GoogleCheckoutButton" CodeFile="GoogleCheckoutButton.ascx.cs" Inherits="Google_GoogleCheckoutButton" %>
<%@ Register TagPrefix="gc1" Namespace="MakerShop.Payments.Providers.GoogleCheckout.Checkout" Assembly="MakerShop.GoogleCheckout" %>
<gc1:GCheckoutButton id="GCheckoutButton" onclick="GCheckoutButton_Click" runat="server" EnableViewState="false" />
<asp:PlaceHolder ID="phWarnings" runat="server" Visible="false">
	<asp:DataList ID="GCWarningMessageList" runat="server" EnableViewState="false">
		<HeaderTemplate><ul></HeaderTemplate>
		<ItemTemplate>
			<li><%# Container.DataItem %></li>
		</ItemTemplate>
		<FooterTemplate></ul></FooterTemplate>
	</asp:DataList>
</asp:PlaceHolder>
