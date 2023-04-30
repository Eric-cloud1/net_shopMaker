<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MoreCategoryItems.ascx.cs" Inherits="ConLib_MoreCategoryItems" %>
<%--
<conlib>
<summary>Displays other products in the same category.</summary>
<param name="Caption" default="Other Items In This Category">Possible value can be any string.  Title of the control.</param>
<param name="MaxItems" default="4">Possible value cab be any integer greater then zero. Indicates that at maximum how many items can be shown.</param>
<param name="Orientation" default="HORIZONTAL">Possible values are 'HORIZONTAL' or 'VERTICAL'.  Indicates whether the contents will be displayed vertically or horizontally, In case of vertical orientation only one column will be displayed.</param>
<param name="DisplayMode" default="SEQUENTIAL">Possible values are 'SEQUENTIAL' or 'RANDOM'.  Indicates whether the contents will be selected randomly or in sequence.</param>
</conlib>
--%>
<%@ Register Src="~/ConLib/AddToCartLink.ascx" TagName="AddToCartLink" TagPrefix="uc" %>
<asp:PlaceHolder ID="phContent" runat="server">
    <div class="section">
        <div class="header">
            <h2><asp:localize ID="phCaption" runat="server" Text="Other Items In This Category"></asp:localize></h2>
        </div>
        <div class="content" align="center">
            <asp:Repeater ID="ItemsRepeater" runat="server">
                <HeaderTemplate>
                    <table align="center" cellspacing="0" cellpadding="4">
                        <%= GetRow(true, true) %>
                </HeaderTemplate>
                <ItemTemplate>
                    <%= GetRow(true, false) %>
                    <td valign="bottom" align="center" width="<%= Width %>">
                        <asp:Literal ID="phThumbnail" runat="server" Text='<%#GetThumbnail(Container.DataItem)%>'></asp:Literal>
                        <b><a href="<%# Page.ResolveClientUrl(Eval("NavigateUrl").ToString()) %>"><%#Eval("Name")%></a></b>
                        <div style="margin-top:10px"><uc:AddToCartLink ID="Add2Cart" runat="server" ProductId='<%#Eval("ProductId")%>' /></div>
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
