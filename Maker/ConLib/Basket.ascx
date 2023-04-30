<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Basket.ascx.cs" Inherits="ConLib_Basket" %>
<%-- 
<conlib>
<summary>Shows detail of items in shopping cart. And provide features like edit/remove items, different checkout options etc.</summary>
</conlib>
--%>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Src="~/ConLib/Utility/BasketItemDetail.ascx" TagName="BasketItemDetail" TagPrefix="uc" %>
<!--< % @  Register Src="../Checkout/Google/GoogleCheckoutButton.ascx" TagName="GoogleCheckoutButton" TagPrefix="uc" % >-->
<ajax:UpdatePanel ID="BasketPanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div style="padding-left:20px;">
        <asp:Label ID="OrderBelowMinimumAmountMessage" runat="server" Text="Your order does not yet meet the minimum value of {0:ulc}.  You must increase your purchase and click Recalculate, in order to checkout." SkinID="errorCondition" Visible="false" EnableViewState="false"></asp:Label>
        <asp:Label ID="OrderAboveMaximumAmountMessage" runat="server" Text="Your order exceeds the maximum value of {0:ulc}.  You must reduce your purchase and click Recalculate, in order to checkout." SkinID="errorCondition" Visible="false" EnableViewState="false"></asp:Label>
	    <asp:DataList ID="WarningMessageList" runat="server" EnableViewState="false" CssClass="errorCondition" >
	        <HeaderTemplate><ul></HeaderTemplate>
	        <ItemTemplate>
	            <li><%# Container.DataItem %></li>
	        </ItemTemplate>
	        <FooterTemplate></ul></FooterTemplate>
	    </asp:DataList>
	    </div>
		<div class="iner_frame" align="center">
            <asp:GridView ID="BasketGrid" runat="server" AutoGenerateColumns="False"
                ShowFooter="True" DataKeyNames="BasketItemId" OnRowCommand="BasketGrid_RowCommand"
                OnDataBound="BasketGrid_DataBound" Width="100%" CellPadding="4" CellSpacing="0"
                OnRowDataBound="BasketGrid_RowDataBound" RowStyle-CssClass="altodd" AlternatingRowStyle-CssClass="alteven" BorderColor="DarkGray" BorderStyle="Solid">
                <Columns>
                    <asp:TemplateField >
                        <HeaderStyle CssClass="columnHeader" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:PlaceHolder ID="ProductImagePanel" runat="server" Visible='<%#ShowProductImagePanel(Container.DataItem)%>' EnableViewState="false">
                                <asp:HyperLink ID="ThumbnailLink" runat="server" NavigateUrl='<%#Eval("Product.NavigateUrl") %>' EnableViewState="false">
                                    <asp:Image ID="Thumbnail" runat="server" AlternateText='<%# Eval("Product.Name") %>' ImageUrl='<%# Eval("Product.ThumbnailUrl") %>' EnableViewState="false" />
                                </asp:HyperLink>
                            </asp:PlaceHolder>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Item">
                        <HeaderStyle CssClass="columnHeader" HorizontalAlign="left" />
                        <ItemStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:PlaceHolder ID="ProductPanel" runat="server" Visible='<%#IsProduct(Container.DataItem)%>'>
                                <uc:BasketItemDetail id="BasketItemDetail1" runat="server" BasketItem='<%#(BasketItem)Container.DataItem%>' ShowAssets="true" ShowSubscription="true" LinkProducts="True" /><br />
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="ParentProductPanel" runat="server" Visible='<%#IsParentProduct(Container.DataItem)%>'>
                                <%#Eval("LastModifiedDate", "Item Added on {0:d}")%><br />
                                <asp:LinkButton ID="SaveItemButton" runat="server" CssClass="altoddButton" CommandName="SaveItem" CommandArgument='<%# Eval("BasketItemId") %>' Text="Move to wishlist"></asp:LinkButton>&nbsp;&nbsp;&nbsp;
                                <asp:LinkButton ID="DeleteItemButton" runat="server" CssClass="altoddButton" CommandName="DeleteItem" CommandArgument='<%# Eval("BasketItemId") %>' Text="Delete" OnClientClick="return confirm('Are you sure you want to remove this item from your cart?')"></asp:LinkButton><br /><br />
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="OtherPanel" runat="server" Visible='<%#((OrderItemType)Eval("OrderItemType") != OrderItemType.Product)%>' EnableViewState="false">
                                <%#Eval("Name")%>
                            </asp:PlaceHolder>
							<asp:PlaceHolder ID="CouponPanel" runat="server" Visible='<%#((OrderItemType)Eval("OrderItemType") == OrderItemType.Coupon)%>' EnableViewState="true">
								<br/>
                                <asp:LinkButton ID="DeleteCouponItemButton" runat="server" CommandName="DeleteCouponItem" CommandArgument='<%# Eval("BasketItemId") %>' Text="Delete" OnClientClick="return confirm('Are you sure you want to remove this coupon from your cart?')" EnableViewState="true"></asp:LinkButton>
                            </asp:PlaceHolder>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="SKU">
                        <HeaderStyle CssClass="columnHeader" />
                        <ItemStyle Width="120px" />
                        <ItemTemplate>
                            <%#ProductHelper.GetSKU(Container.DataItem)%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Tax">
                        <HeaderStyle CssClass="columnHeader" />
                        <ItemStyle HorizontalAlign="Center" Width="40px" />
                        <ItemTemplate>
                            <%#TaxHelper.GetTaxRate((BasketItem)Container.DataItem).ToString("0.####")%>%
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Each">
                        <HeaderStyle CssClass="columnHeader" />
                        <ItemStyle HorizontalAlign="right" Width="80px" />
                        <ItemTemplate>
                            <%#TaxHelper.GetShopPrice(Token.Instance.User.Basket, (BasketItem)Container.DataItem).ToString("ulc")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Qty">
                        <HeaderStyle CssClass="columnHeader" />
                        <ItemStyle HorizontalAlign="center" Width="80px" />
                        <ItemTemplate>
                            <asp:PlaceHolder ID="ProductQuantityPanel" runat="server" Visible='<%#IsParentProduct(Container.DataItem)%>'>
                                <cb:updowncontrol MinValue="0" id="Quantity" runat="server" MaxValue="32767" DownImageUrl="~/images/down.gif" UpImageUrl="~/images/up.gif" Columns="2" MaxLength="5" Text='<%# Eval("Quantity") %>' onFocus="this.select()"></cb:updowncontrol>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="OtherQuantityPanel" runat="server" Visible='<%#!IsParentProduct(Container.DataItem)%>' EnableViewState="false">
                                <%#Eval("Quantity")%>
                            </asp:PlaceHolder>							
                        </ItemTemplate>
                        <FooterStyle HorizontalAlign="right" VerticalAlign="top" CssClass="totalRow" />
                        <FooterTemplate>
                            <asp:Label ID="SubtotalLabel" runat="server" Text="Subtotal" SkinID="FieldHeader" EnableViewState="false"></asp:Label>
                        </FooterTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Price">
                        <HeaderStyle CssClass="columnHeader" />
                        <ItemStyle HorizontalAlign="right" Width="80px" />
                        <ItemTemplate>
                            <%#TaxHelper.GetShopExtendedPrice(Token.Instance.User.Basket, (BasketItem)Container.DataItem).ToString("ulc")%>
                        </ItemTemplate>
                        <FooterStyle HorizontalAlign="right" VerticalAlign="top" />
                        <FooterTemplate>
                            <asp:Label ID="Subtotal" runat="server" Text='<%# String.Format("{0:ulc}", GetBasketSubtotal()) %>' SkinID="FieldHeader" EnableViewState="false"></asp:Label>
                        </FooterTemplate>
                    </asp:TemplateField>
                </Columns>
                <RowStyle CssClass="altodd" />
                <AlternatingRowStyle CssClass="alteven" />
            </asp:GridView>
            <asp:Panel ID="EmptyBasketPanel" runat="server" CssClass="emptyBasketPanel" EnableViewState="false">
                <asp:Label ID="EmptyBasketMessage" runat="server" Text="Your cart is empty." CssClass="message" EnableViewState="false"></asp:Label>
            </asp:Panel>
            <div style="text-align:center; margin-top:10px; margin-bottom:10px;">
                <asp:LinkButton ID="KeepShoppingButton" runat="server" OnClick="KeepShoppingButton_Click" Text="&nbsp;&nbsp;Keep Shopping&nbsp;&nbsp;" SkinID="Button" EnableViewState="false"></asp:LinkButton>
                <asp:LinkButton ID="ClearBasketButton" runat="server" SkinID="Button" OnClientClick="return confirm('Are you sure you want to clear your cart?')" OnClick="ClearBasketButton_Click" Text="&nbsp;&nbsp;Clear Cart&nbsp;&nbsp;" EnableViewState="false"></asp:LinkButton>
                <asp:LinkButton ID="UpdateButton" runat="server" SkinID="Button" OnClick="UpdateButton_Click" Text="&nbsp;&nbsp;Recalculate&nbsp;&nbsp;" EnableViewState="false"></asp:LinkButton>
            </div>
            <div style="text-align:right; margin-top:10px; margin-bottom:10px; margin-right:5px;">
                <asp:ImageButton ID="CheckoutButton" runat="server" SkinID="CheckoutNow" OnClick="CheckoutButton_Click" Text="Checkout" EnableViewState="false"></asp:ImageButton>  
                <br />
            </div>
       <!--     <div style="text-align:right; margin-top:10px; margin-bottom:10px;">
                < u c : G o o g l e Checkou tButton id="Googl eCheckout Button" ru na t="ser ver" / >
            </div>
            -->
		</div>
    </ContentTemplate>
</ajax:UpdatePanel>