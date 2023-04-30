<%@ Control Language="C#" CodeFile="FeaturedCategoryItems.ascx.cs" Inherits="ConLib_FeaturedCategoryItems" %>
<%--
<conlib>
<summary>Displays featured items in a category.</summary>
<param name="Caption" default="Featured Items">Possible value can be any string.  Title of the control.</param>
<param name="MaxItems" default="3">Possible value cab be any integer greater then zero. Indicates that at maximum how many items can be shown.</param>
<param name="Orientation" default="VERTICAL">Possible values are 'HORIZONTAL' or 'VERTICAL'.  Indicates whether the contents will be displayed vertically or horizontally, In case of vertical orientation only one column will be displayed.</param>
<param name="IncludeOutOfStockItems" default="false">Possible values be true of false. Indicates that the grid will display out of sctock items or not.</param>
</conlib>
--%>
<asp:PlaceHolder ID="phContent" runat="server">
    <div class="section">
        <div class="header">
            <h2><asp:localize ID="phCaption" runat="server" Text="Featured Items"></asp:localize></h2>
        </div>
        <div class="content">
            <asp:Repeater ID="ItemsRepeater" runat="server">
                <HeaderTemplate>
                    <table align="center" cellspacing="0" cellpadding="4">
                        <%= GetRow(true, true) %>
                </HeaderTemplate>
                <ItemTemplate>
                    <%= GetRow(true, false) %>
                    <td valign="top" align="center" width="<%= Width %>">
                        <asp:Literal ID="phThumbnail" runat="server" Text='<%#GetThumbnail(Container.DataItem)%>'></asp:Literal>
                        <b><a href="<%# Page.ResolveClientUrl(Eval("NavigateUrl").ToString()) %>"><%#Eval("Name")%></a></b>
                    </td>
                    <%= GetRow(false, false) %>
                </ItemTemplate>
                <FooterTemplate>
                        <%= GetRow(false, true) %>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:PlaceHolder>