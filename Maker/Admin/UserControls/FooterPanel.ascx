<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FooterPanel.ascx.cs" Inherits="Admin_UserControls_FooterPanel" %>
<div id="footer">
    <p>
        <asp:HyperLink ID="LogoutLink" runat="server" NavigateUrl="~/logout.aspx" Text="Logout"></asp:HyperLink>
        &nbsp;|&nbsp;
        <asp:HyperLink ID="HomeLink" runat="server" NavigateUrl="~/Default.aspx" Text="Store"></asp:HyperLink>
        &nbsp;|&nbsp;
        <asp:HyperLink ID="CatalogLink" runat="server" NavigateUrl="~/Admin/Catalog/Browse.aspx" Text="Catalog"></asp:HyperLink>
        &nbsp;|&nbsp;
        <asp:HyperLink ID="OrdersLink" runat="server" NavigateUrl="~/Admin/Orders/Default.aspx" Text="Orders"></asp:HyperLink>
        &nbsp;|&nbsp;
        <asp:HyperLink ID="DashboardLink" runat="server" NavigateUrl="~/Admin/Default.aspx" Text="Dashboard"></asp:HyperLink>
        <br />
        <asp:Literal ID="ProductVersion" runat="server"></asp:Literal>
        <br />
        &copy; Copyright 2011 Viroplex. All rights reserved.
    </p>
</div>