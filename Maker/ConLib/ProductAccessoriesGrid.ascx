<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductAccessoriesGrid.ascx.cs" Inherits="Webparts_ProductAccessoriesGrid" EnableViewState="false" %>
<%--
<conlib>
<summary>Display product accessories of a product. This page will be displayed when a product that has accessories is added to cart.</summary>
<param name="Caption" default="Products Accessories for {0}">Possible value can be any string.  Title of the control.</param>
<param name="Size" default="6">Possible value cab be any integer greater then zero. Indicates that at maximum how many items can be shown.</param>
<param name="Columns" default="2">Possible value cab be any integer greater then zero. Indicates the number of columns.</param>
</conlib>
--%>
<%@ Register Src="~/ConLib/Utility/ProductPrice.ascx" TagName="ProductPrice" TagPrefix="uc" %>
<%@ Register Src="~/ConLib/AddToCartLink.ascx" TagName="AddToCartLink" TagPrefix="uc" %>
<div class="pageHeader">
    <h1 class="heading"><asp:Localize ID="CaptionLabel" runat="server" Text="Products Accessories for {0}"></asp:Localize></h1>
</div>
<div >
    <h2>
    <asp:Label runat="server" ID="InstructionText" Text="You just added '{0}' to your cart. Here are some
additional items you might consider.." ></asp:Label>
    </h2>
</div>
<asp:DataList ID="ProductList" runat="server" RepeatColumns="2" RepeatDirection="Horizontal" Width="100%" 
    OnItemDataBound="ProductList_ItemDataBound" DataKeyField="ProductId" ItemStyle-CssClass="rowSeparator" 
    SeparatorStyle-CssClass="itemSeparator">
    <ItemTemplate>
        <table width="100%" cellspacing="0" cellpadding="0" class="productsGrid">
            <tr>
                <td align="center" valign="top" class="thumbnail">
                    <asp:HyperLink ID="ThumbnailLink" runat="server" NavigateUrl='<%#GetNavigateUrl(Container.DataItem)%>' Visible='<%#HasThumbnail(Container.DataItem)%>'>
                    <asp:Image ID="Thumbnail" runat="server" SkinID="Thumbnail" ImageUrl='<%#GetThumbnailUrl(Container.DataItem)%>' AlternateText='<%#GetThumbnailAltText(Container.DataItem)%>' /></asp:HyperLink>
                </td>
                <td align="left" valign="top" class="details" width="100%">
                    <p class="detailsInnerPara"><asp:HyperLink ID="NameLink" runat="server" CssClass="highlight" Text='<%#GetName(Container.DataItem)%>' NavigateUrl='<%#GetNavigateUrl(Container.DataItem)%>'></asp:HyperLink><br />
                    <asp:Label ID="ManufacturerLabel" runat="server" Text='<%#GetManufacturerName(Container.DataItem) %>'></asp:Label><br />
                    <asp:Image ID="Rating" runat="server" ImageUrl='<%#GetRatingImage(Container.DataItem)%>' Visible='<%#Token.Instance.Store.Settings.ProductReviewEnabled != MakerShop.Users.UserAuthFilter.None %>'/><br /></p>
                    <p class="highlight"><uc:ProductPrice ID="Price" runat="server" Product='<%#Eval("ChildProduct")%>' PriceFormat="Price: {0:ulc}" BasePriceFormat='Price: <span class="msrp">{0:ulc}</span> ' ShowRetailPrice="True" RetailPriceFormat='List: <span class="msrp">{0:ulc}</span><br />' /></p><br />
                    <uc:AddToCartLink ID="Add2Cart" runat="server" Product='<%#Eval("ChildProduct")%>' />
                </td>
            </tr>
            <tr><td colspan="2"><br /></td></tr>
        </table>
    </ItemTemplate>
    <SeparatorTemplate></SeparatorTemplate>
    <SeparatorStyle CssClass="itemSeperator" />
</asp:DataList>
<div style="text-align:center; margin-top:10px; margin-bottom:10px;">
    <asp:HyperLink ID="KeepShoppingLink" runat="server" NavigateUrl="~/Default.aspx" Text="Keep Shopping" SkinID="Button"></asp:HyperLink>
</div>
