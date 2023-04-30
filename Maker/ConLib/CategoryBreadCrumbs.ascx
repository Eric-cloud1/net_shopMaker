<%@ Control Language="C#" CodeFile="CategoryBreadCrumbs.ascx.cs" Inherits="ConLib_CategoryBreadCrumbs" EnableViewState="false" %>
<%--
<conlib>
<summary>Displays bread crumbs for the current category.</summary>
<param name="HideLastNode" default="false">Possible values are true or false.  Indicates whether the last node i.e. the name of current category will be shown or not.</param>
</conlib>
--%>
<div class="CategoryBreadCrumbs">
<asp:HyperLink ID="HomeLink" runat="server" NavigateUrl="" Text="Home"></asp:HyperLink>
<asp:Repeater ID="BreadCrumbsRepeater" runat="server">
    <HeaderTemplate><%= Separator %></HeaderTemplate>
    <ItemTemplate>
        <asp:HyperLink ID="BreadCrumbsLink" runat="server" NavigateUrl='<%#UrlGenerator.GetBrowseUrl((int)Eval("catalogNodeId"), (string)Eval("name"))%>' Text='<%#Eval("Name")%>'></asp:HyperLink>
    </ItemTemplate>
    <SeparatorTemplate><%= Separator %></SeparatorTemplate>
</asp:Repeater>
</div>