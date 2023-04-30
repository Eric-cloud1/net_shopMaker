<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SimilarProducts.ascx.cs" Inherits="ConLib_SimilarProducts" %>
<%--
<conlib>
<summary>Displays a list or grid of similar products.</summary>
<param name="Caption" default="Similar Products">Possible value can be any string.  Title of the control.</param>
<param name="Orientation" default="HORIZONTAL">Possible values are 'HORIZONTAL' or 'VERTICAL'.  Indicates whether the contents will be displayed vertically or horizontally, In case of vertical orientation only one column will be displayed.</param>
<param name="Columns" default="3">Possible value cab be any integer greater then zero. Indicates the number of columns, for 'VERTICAL' orientation there will always be a single column.</param>
</conlib>
--%>
<%@ Register Src="~/ConLib/Utility/ProductPrice.ascx" TagName="ProductPrice" TagPrefix="uc" %>
<div class="section">
    <div class="header">
        <h2><asp:Localize ID="CaptionLabel" runat="server" Text="Similar Products" /></h2>
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
                    <b><a href="<%# Page.ResolveClientUrl(Eval("NavigateUrl").ToString()) %>"><%#Eval("Name")%></a></b><br />
			        <asp:PlaceHolder ID="phSku" runat="server" Visible='<%# (Eval("SKU").ToString().Length > 0) %>'><strong>SKU:</strong>&nbsp;&nbsp;<%#Eval("SKU")%><br /></asp:PlaceHolder>
    			    <uc:ProductPrice ID="Price" runat="server" Product='<%#Container.DataItem%>' PriceLabel="<b>Price: </b>" /><br />	                
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