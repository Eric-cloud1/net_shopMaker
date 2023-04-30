<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MiniBasket.ascx.cs" Inherits="ConLib_MiniBasket" %>
<%--
<conlib>
<summary>Display the contents of items in cart. Can be used in side bars.</summary>
<param name="AlternateControl" default="PopularProductsDialog.ascx">A control that will be displayed when cart is empty.</param>
</conlib>
--%>
<%@ Register Src="~/ConLib/Utility/MiniBasketItemDetail.ascx" TagName="MiniBasketItemDetail" TagPrefix="uc1" %>
<%@ Register Src="BasketShippingEstimate.ascx" TagName="BasketShippingEstimate" TagPrefix="uc1" %>
<%@ Register Src="PayPalExpressCheckoutButton.ascx" TagName="PayPalExpressCheckoutButton" TagPrefix="uc1" %>
<!--< % @  Register Src="~/Checkout/Google/GoogleCheckoutButton.ascx" TagName="GoogleCheckoutButton" TagPrefix="uc1" % > -->
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
    
<ajax:UpdatePanel ID="BasketPanel" runat="server" UpdateMode="Always">
    <ContentTemplate>
        <asp:Panel ID="MiniBasketHolder" runat="server">
        <div class="section">
            <div class="header">
                <h2><asp:localize ID="Caption" runat="server" Text="Your Cart"></asp:localize></h2>
            </div>
            <asp:Panel ID="ContentPanel" runat="server" CssClass="content">
                <asp:PlaceHolder ID="BasketTable" runat="server">
				    <div id="miniBasketMainBox">
                        <asp:Repeater ID="BasketRepeater" runat="server" OnItemCommand="BasketRepeater_ItemCommand" >
                            <ItemTemplate>
                                <div class="miniBasketItemBox">
                                    <asp:PlaceHolder ID="ProductImagePanel" runat="server" EnableViewState="false" Visible='<%# HasImage(Container.DataItem) %>'>
                                        <div class="miniBasketIconBox">								
                                            <asp:HyperLink ID="ThumbnailLink" runat="server" NavigateUrl='<%#Eval("Product.NavigateUrl")%>' EnableViewState="false">
                                            <asp:Image ID="Thumbnail" runat="server" AlternateText='<%# Eval("Product.Name") %>' ImageUrl='<%# GetIconUrl(Container.DataItem) %>' Width="50" Height="50" EnableViewState="false" />
                                            </asp:HyperLink>								
                                        </div>
                                    </asp:PlaceHolder>
                                    <div class="<%# (HasImage(Container.DataItem) ? "miniBasketItemTitleBox" : "miniBasketItemTitleNoIconBox") %>">
                                        <uc1:MiniBasketItemDetail ID="BasketItemDetail1" runat="server" BasketItem='<%#(BasketItem)Container.DataItem%>' LinkProducts="true" ShowAssets="false" ShowSubscription="false" />
                                    </div>
                                    <div class="miniBasketQtyDeleteLine" >
                                        <span style="float: left; margin-top:2px;">Qty:&nbsp;</span>
                                        <asp:PlaceHolder ID="ProductQuantityPanel" runat="server" >	
                                            <asp:TextBox ID="Quantity" runat="server" Text='<%# Eval("Quantity") %>' onFocus="this.select()" CssClass="miniBasketQtyInput" EnableViewState="false"> </asp:TextBox>
                                        </asp:PlaceHolder>								
                                        <asp:LinkButton ID="UpdateItemButton" runat="server" CommandName="UpdateItem" CommandArgument='<%#Eval("BasketItemId")%>' Text="Update" CausesValidation="false"></asp:LinkButton> &nbsp;
                                        <asp:LinkButton ID="DeleteItemButton" runat="server" CommandName="DeleteItem" CommandArgument='<%#Eval("BasketItemId")%>' Text="Delete" OnClientClick="return confirm('Are you sure you want to remove this item from your cart?')" CausesValidation="false"></asp:LinkButton>
                                    </div> 
                                    <span class="miniBasketPriceLabel">Price:&nbsp;</span>
                                    <span class="miniBasketPrice"><%#GetItemShopPrice((BasketItem)Container.DataItem).ToString("ulc")%></span>
                                </div>
                                <asp:HiddenField  Visible="false" ID="BasketItemId" runat="server" Value='<%#Eval("BasketItemId")%>' />
                            </ItemTemplate>
                        </asp:Repeater>
						<div id="miniBasketSubTotalBox">
							<asp:Panel ID="DiscountsPanel" runat="server" >
							<div>
							  <div id="tax_label">Discounts:</div>
							  <div id="tax_num">
								<b><asp:Literal ID="Discounts" runat="server" SkinID="BasketDialogPrice"></asp:Literal></b>
							  </div>
							</div>
							<div style="clear: both;"></div>
							</asp:Panel>
							<div>
							  <div id="sub_label">Subtotal:</div>
							  <div id="sub_num">
								<asp:Label ID="SubTotal" runat="server" SkinID="BasketDialogPrice"></asp:Label>
							  </div>
							</div>							
							<div style="clear: both;"></div>
							<div id="miniBasketCheckoutButtonBox">
							    <asp:ImageButton ID="CheckoutButton" runat="server" ToolTip="Checkout Now" SkinID="CheckoutNow" OnClick="CheckoutButton_Click" />
							</div>
							<div style="text-align:center">
							<div style="clear: both;"></div>
							<uc1:PayPalExpressCheckoutButton ID="PayPalExpressCheckoutButton" runat="server" ShowHeader="false" ShowDescription="false" PanelCSSClass="" />
							<div style="clear: both;"></div>
<!--							 < u c 1 : G o o g l e CheckoutButton ID="GoogleCheckoutButton" runat="s e r v  er" / >							-->
							 </div>
						</div>
						<div id="miniBasketShippingEstimateBox">
							<div>
							  <uc1:BasketShippingEstimate ID="BasketShippingEstimate1" runat="server" />
							</div>
						</div>
					</div>
                </asp:PlaceHolder>
                <asp:Panel ID="EmptyBasketPanel" runat="server" CssClass="emptyBasketDialogPanel" Visible="false">
                    <asp:Label ID="EmptyBasketMessage" runat="Server" Text="Empty" CssClass="message"></asp:Label>
                </asp:Panel>
            </asp:Panel>
        </div>
        </asp:Panel>
		<asp:Panel ID="AlternateControlPanel" runat="server">
		</asp:Panel>
    </ContentTemplate>
</ajax:UpdatePanel>
