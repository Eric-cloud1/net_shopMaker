<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RelatedProducts.ascx.cs" Inherits="ConLib_RelatedProducts" EnableViewState="false" %>
<%--
<conlib>
<summary>Displays a list or grid of recently viewed products.</summary>
<param name="Caption" default="Featured Items">Possible value can be any string.  Title of the control.</param>
<param name="MaxItems" default="5">Possible value cab be any integer greater then zero. Indicates that at maximum how many items can be shown.</param>
<param name="Orientation" default="HORIZONTAL">Possible values are 'HORIZONTAL' or 'VERTICAL'.  Indicates whether the contents will be displayed vertically or horizontally, In case of vertical orientation only one column will be displayed.</param>
<param name="Columns" default="3">Possible value can be any integer greater then zero. Indicates the number of columns, for 'VERTICAL' orientation there will always be a single column.</param>
</conlib>
--%>
<%@ Register Src="~/ConLib/Utility/ProductPrice.ascx" TagName="ProductPrice" TagPrefix="uc" %>
<%@ Register Src="~/ConLib/AddToCartLink.ascx" TagName="AddToCartLink" TagPrefix="uc" %>
<asp:PlaceHolder ID="phContent" runat="server">
    <div class="section">
        <div class="header">
            <h2><asp:localize ID="phCaption" runat="server" Text="Recently Viewed"></asp:localize></h2>
        </div>
        <div class="content">
            <asp:DataList  ID="ProductList" runat="server" OnItemDataBound="ProductList_ItemDataBound" AlternatingItemStyle-CssClass="ProductItemViewOdd">
            <ItemStyle HorizontalAlign="center" VerticalAlign="middle" CssClass="ProductItemView" />

                <ItemTemplate>
                    <asp:HyperLink ID="ThumbnailLink" runat="server" NavigateUrl='<%# UrlGenerator.GetBrowseUrl(Container.DataItem) %>'>
		                <asp:Image ID="Thumbnail" runat="server" />
		            </asp:HyperLink>
		            <p class="image_desc">
			            <b><a href="<%# Page.ResolveClientUrl(Eval("NavigateUrl").ToString()) %>"><%#Eval("Name")%></a></b><br />
			            <asp:PlaceHolder ID="phSku" runat="server" Visible='<%# (Eval("SKU").ToString().Length > 0) %>'><strong>SKU:</strong>&nbsp;&nbsp;<%#Eval("SKU")%><br /></asp:PlaceHolder>
						<asp:PlaceHolder ID="phPrice" runat="server" Visible='<%# !((bool)Eval("UseVariablePrice")) %>'>
    			        <uc:ProductPrice ID="Price" runat="server" Product='<%#Container.DataItem%>' PriceLabel="<b>Price: </b>" />
						</asp:PlaceHolder>
						<div style="margin-top:5px"><uc:AddToCartLink ID="Add2Cart" runat="server" ProductId='<%#Eval("ProductId")%>' /></div>
	                </p>
                </ItemTemplate>
            </asp:DataList >
        </div>
    </div>
</asp:PlaceHolder>