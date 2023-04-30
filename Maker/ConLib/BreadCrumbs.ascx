<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BreadCrumbs.ascx.cs" Inherits="ConLib_BreadCrumbs" %>
<%--
<conlib>
<summary>Displays dynamic bread crumb links for the current page.</summary>
</conlib>
--%>
<div class="breadCrumbPanel noPrint">
    <asp:SiteMapPath ID="StoreBreadCrumbs" runat="server" SiteMapProvider="StoreSiteMap" EnableViewState="False">
        <NodeStyle CssClass="BreadCrumbs" />
        <CurrentNodeStyle Font-Underline="false" Font-Bold="true" />
        <PathSeparatorTemplate><asp:Localize ID="PathSeparator" runat="server" Text=" > "></asp:Localize></PathSeparatorTemplate>
    </asp:SiteMapPath>
</div>