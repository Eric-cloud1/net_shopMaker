<%@ Control Language="C#" CodeFile="SimpleCategoryList.ascx.cs" Inherits="ConLib_SimpleCategoryList" EnableViewState="false" %>
<%--
<conlib>
<summary>A simple category list which shows the nested categories under a specific category.</summary>
<param name="CssClass" default="section">Css style sheet class.</param>
<param name="HeaderCssClass" default="header">Css style sheet class.</param>
<param name="HeaderText" default="Categories">Title Text for the header.</param>
<param name="ContentCssClass" default="content">Css style sheet class.</param>
</conlib>
--%>
<asp:Panel ID="MainPanel" runat="server" CssClass="section">
    <asp:Panel ID="HeaderPanel" runat="server" CssClass="header">
	    <h2 class="header"><asp:Localize ID="HeaderTextLabel" runat="server" Text="Categories"></asp:Localize></h2>
    </asp:Panel>
	<asp:Panel ID="ContentPanel" runat="server" CssClass="content">
        <asp:Repeater ID="CategoryList" runat="server">
            <HeaderTemplate>
                <ul class="category">
            </HeaderTemplate>
            <ItemTemplate>
                <li><asp:HyperLink ID="CategoryLink" runat="server"  Text='<%#Eval("Name")%>' NavigateUrl='<%#Eval("NavigateUrl")%>'></asp:HyperLink></li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </asp:Repeater>        
    </asp:Panel>
    <asp:Localize ID="NoSubCategoriesText" runat="server" Visible="false" EnableViewState="false" Text="There are no sub-categories in <strong>{0}</strong>."></asp:Localize>
</asp:Panel>