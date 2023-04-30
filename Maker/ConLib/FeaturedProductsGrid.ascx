<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FeaturedProductsGrid.ascx.cs" Inherits="Webparts_FeaturedProductsGrid" EnableViewState="false" %>
<%--
<conlib>
<summary>Displays featured items in a category.</summary>
<param name="Caption" default="Featured Items">Possible value can be any string.  Title of the control.</param>
<param name="Size" default="3">Possible value cab be any integer greater then zero. Indicates that at maximum how many items can be shown.</param>
<param name="Columns" default="2">Possible value cab be any integer greater then zero. Indicates that the grid will contain how much columns.</param>
<param name="IncludeOutOfStockItems" default="false">Possible values be true of false. Indicates that the grid will display out of sctock items or not.</param>
<param name="ThumbnailPosition" default="LEFT">Possible values are 'TOP' or 'LEFT'.  Indicates whether the product image will be displayed on top of product details or on the left.</param>
<param name="Orientation" default="HORIZONTAL">Possible values are 'HORIZONTAL' or 'VERTICAL'.  Indicates whether the contents will be displayed vertically or horizontally, In case of vertical orientation only one column will be displayed.</param>
</conlib>
--%>
<%@ Register Src="~/ConLib/Utility/ProductPrice.ascx" TagName="ProductPrice" TagPrefix="uc" %>
<%@ Register Src="~/ConLib/AddToCartLink.ascx" TagName="AddToCartLink" TagPrefix="uc" %>
<div class="pageHeader">
    <h1 class="heading"><asp:Localize ID="CaptionLabel" runat="server" Text="Featured Products"></asp:Localize></h1>
</div>
<asp:DataList ID="ProductList" runat="server" RepeatColumns="2" RepeatDirection="Horizontal" Width="100%" 
    OnItemDataBound="ProductList_ItemDataBound" DataKeyField="ProductId" ItemStyle-CssClass="rowSeparator" 
    SeparatorStyle-CssClass="itemSeparator" ItemStyle-VerticalAlign="bottom">
    <ItemTemplate>
	    <div class="featuredProductContainer">
        <table width="100%" cellspacing="0" cellpadding="0" class="productsGrid">
            <tr>
                <td align="center" valign="top" class="thumbnail">
                    <asp:HyperLink ID="ThumbnailLink" runat="server" NavigateUrl='<%#Eval("NavigateUrl")%>'><asp:Image ID="Thumbnail" runat="server" SkinID="Thumbnail" ImageUrl='<%#GetThumbnailUrl(Eval("ThumbnailUrl"))%>' AlternateText='<%#Eval("ThumbnailAltText")%>' Visible='<%#!string.IsNullOrEmpty((String)Eval("ThumbnailUrl")) %>' /></asp:HyperLink>
                    
                    <asp:Literal ID="SingleRowLiteral" runat="server" Text="</td><td align='left' valign='bottom' class='details' width='100%'>" visible="false" EnableViewState="false"/>		        		        
		            <asp:Literal ID="TwoRowsLiteral" runat="server" Text="</td></tr><tr><td class='PIVimage_desc' align='center'>" EnableViewState="false" />		            
		            
                    <div class="detailsInnerPara"><asp:HyperLink ID="NameLink" runat="server" CssClass="highlight" Text='<%# Eval("Name") %>' NavigateUrl='<%#Eval("NavigateUrl")%>'></asp:HyperLink><br />
                    <asp:Label ID="ManufacturerLabel" runat="server" Text='<%# Eval("Manufacturer.Name") %>'></asp:Label><br />
                    <div id="RatingImage" runat="server" visible='<%#Token.Instance.Store.Settings.ProductReviewEnabled != MakerShop.Users.UserAuthFilter.None %>'>
                        <asp:Image ID="Rating" runat="server" ImageUrl='<%#GetRatingImage(Container.DataItem)%>' AlternateText="Rating" /><br />
                    </div>
                    </div>
                    <p class="highlight">
					<uc:ProductPrice ID="ProductPrice" runat="server" Product='<%#Container.DataItem%>' PriceFormat="Price: {0:ulc}" BasePriceFormat='Price: <span class="msrp">{0:ulc}</span> '></uc:ProductPrice>
                    </p>
                    <div style="margin-top:10px"><uc:AddToCartLink ID="AddToCartLink1" runat="server" ProductId='<%#Eval("ProductId")%>' /></div>
            	</td>
            </tr>
        </table></div>
    </ItemTemplate>
    <SeparatorTemplate></SeparatorTemplate>
    <SeparatorStyle CssClass="itemSeperator" />
</asp:DataList>

