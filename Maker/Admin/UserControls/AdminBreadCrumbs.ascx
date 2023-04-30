<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AdminBreadCrumbs.ascx.cs" Inherits="Admin_UserControls_AdminBreadCrumbs" EnableViewState="false"%>
<div class="breadCrumbsPanel">
    <asp:SiteMapPath ID="BreadCrumbs" runat="server" OnItemDataBound="BreadCrumbs_ItemDataBound">
        <CurrentNodeStyle Font-Underline="false" Font-Bold="true" />
        <PathSeparatorTemplate>&nbsp;>&nbsp;</PathSeparatorTemplate>
    </asp:SiteMapPath>
</div>