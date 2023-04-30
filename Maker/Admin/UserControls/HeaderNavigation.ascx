<%@ Control Language="C#" AutoEventWireup="true" CodeFile="HeaderNavigation.ascx.cs" Inherits="Admin_UserControls_HeaderNavigation" EnableViewState="false" %>
<asp:Panel ID="AdminNavigationHeaderPanel" runat="server" >
	<asp:HyperLink ID="DashboardLink" runat="server" NavigateUrl="~/Admin/Default.aspx" CssClass="dashboard" Text="Dashboard" Visible="true"></asp:HyperLink>
	<asp:HyperLink ID="OrdersLink" runat="server" NavigateUrl="~/Admin/Orders/Default.aspx" CssClass="orders" text="Orders" Visible="true"></asp:HyperLink>
	<asp:HyperLink ID="CatalogLink" runat="server" NavigateUrl="~/Admin/Catalog/Browse.aspx" CssClass="catalog" Text="Catalog" Visible="true"></asp:HyperLink>
	<asp:HyperLink ID="StoreLink" runat="server" NavigateUrl="~/Default.aspx" CssClass="stores" Text="Store" Visible="true"></asp:HyperLink>
	<asp:HyperLink ID="LogoutLink" runat="server" NavigateUrl="~/Logout.aspx" CssClass="logout" Text="Logout" Visible="true"></asp:HyperLink>
</asp:Panel>

