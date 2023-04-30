<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RecentlyViewed.ascx.cs" Inherits="ConLib_RecentlyViewed" EnableViewState="false" %>
<%--
<conlib>
<summary>Displays a list or grid of recently viewed products.</summary>
<param name="Caption" default="Featured Items">Possible value can be any string.  Title of the control.</param>
<param name="MaxItems" default="5">Possible value cab be any integer greater then zero. Indicates that at maximum how many items can be shown.</param>
<param name="Orientation" default="HORIZONTAL">Possible values are 'HORIZONTAL' or 'VERTICAL'.  Indicates whether the contents will be displayed vertically or horizontally, In case of vertical orientation only one column will be displayed.</param>
<param name="Columns" default="3">Possible value can be any integer greater then zero. Indicates the number of columns, for 'VERTICAL' orientation there will always be a single column.</param>
<param name="ThumbnailPosition" default="TOP">Possible values are 'TOP' or 'LEFT'.  Indicates whether the product image will be displayed on top of product details or on the left.</param>
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
            <asp:DataList  ID="ProductList" runat="server" OnItemDataBound="ProductList_ItemDataBound" AlternatingItemStyle-CssClass="ProductItemViewOdd" Width="100%"
        SeparatorStyle-CssClass="itemSeparator" ItemStyle-VerticalAlign="bottom">
            <ItemStyle HorizontalAlign="center" VerticalAlign="middle" CssClass="ProductItemView" />

                <ItemTemplate>
                    <table width="100%" cellspacing="0" cellpadding="0" class="productsGrid">
                        <tr>
                            <td align="center" valign="top" class="thumbnail">
                                <asp:HyperLink ID="ThumbnailLink" runat="server" NavigateUrl='<%#Eval("NavigateUrl")%>'><asp:Image ID="Thumbnail" runat="server" SkinID="Thumbnail" ImageUrl='<%#GetThumbnailUrl(Eval("ThumbnailUrl"))%>' AlternateText='<%#Eval("ThumbnailAltText")%>' Visible='<%#!string.IsNullOrEmpty((String)Eval("ThumbnailUrl")) %>' /></asp:HyperLink>
		                        <asp:Literal ID="SingleRowLiteral" runat="server" Text="</td><td align='left' valign='bottom' class='details' width='100%'>" visible="false" EnableViewState="false"/>		        		        
		                        <asp:Literal ID="TwoRowsLiteral" runat="server" Text="</td></tr><tr><td class='PIVimage_desc' align='center'>" EnableViewState="false" />
		                        <p class="detailsInnerPara">
			                        <a href="<%# Page.ResolveClientUrl(Eval("NavigateUrl").ToString()) %>"><%#Eval("Name")%></a></b><br />
			                        <asp:PlaceHolder ID="phSku" runat="server" Visible='<%# (Eval("SKU").ToString().Length > 0) %>'><strong>SKU:</strong>&nbsp;&nbsp;<%#Eval("SKU")%></asp:PlaceHolder><br />
						            <asp:PlaceHolder ID="phPrice" runat="server" Visible='<%# !((bool)Eval("UseVariablePrice")) %>'>
    			                    <strong><asp:Literal ID="PriceLabel" runat="server" Text="Price: " /></strong>
    			                    <uc:ProductPrice ID="Price" runat="server" Product='<%#Container.DataItem%>' />
						             </asp:PlaceHolder>
					            </p>
						        <div style="margin-top:10px"><uc:AddToCartLink ID="Add2Cart" runat="server" ProductId='<%#Eval("ProductId")%>' /></div>				
				                <br />				    
                            </td>
                        </tr>            
                    </table> 	
                </ItemTemplate>
            </asp:DataList >
        </div>
    </div>
</asp:PlaceHolder>