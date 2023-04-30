<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AffiliateMenu.ascx.cs" Inherits="Admin_Marketing_Affiliates_AffiliateMenu" %>
<asp:Panel ID="AffiliateMenuPanel" runat="server">

    <asp:HyperLink ID="Payments" runat="server" NavigateUrl="AffiliatePayment.aspx" Text="Payments" CssClass="contextMenuButton menu"></asp:HyperLink>
	<asp:HyperLink ID="AddAffiliate" runat="server" NavigateUrl="Default.aspx" Text="Add Affiliate" CssClass="contextMenuButton menu"></asp:HyperLink>
    <asp:HyperLink ID="EditAffiliate" runat="server" NavigateUrl="EditAffiliate.aspx" Text="Edit Affiliate" CssClass="contextMenuButton menu"></asp:HyperLink>
    <asp:HyperLink ID="Gateway" runat="server" NavigateUrl="EditGateway.aspx" Text="Gateway" CssClass="contextMenuButton"></asp:HyperLink>

</asp:Panel>
