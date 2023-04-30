<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PaymentPage.ascx.cs" Inherits="ConLib_PaymentPage" %>
<%--
<conlib>
<summary>Standard payment page, displays all of available billing options.</summary>
</conlib>
--%>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Src="~/Checkout/PaymentForms/CreditCardPaymentForm.ascx" TagName="CreditCardPaymentForm" TagPrefix="uc" %>
<%@ Register Src="~/Checkout/PaymentForms/CheckPaymentForm.ascx" TagName="CheckPaymentForm" TagPrefix="uc" %>
<%@ Register Src="~/Checkout/PaymentForms/PayPalExpressPaymentForm.ascx" TagName="PayPalExpressPaymentForm" TagPrefix="uc" %>
<%@ Register Src="~/Checkout/PaymentForms/PayPalPaymentForm.ascx" TagName="PayPalPaymentForm" TagPrefix="uc" %>
<%@ Register Src="~/Checkout/PaymentForms/MailPaymentForm.ascx" TagName="MailPaymentForm" TagPrefix="uc" %>
<%@ Register Src="~/Checkout/PaymentForms/PhoneCallPaymentForm.ascx" TagName="PhoneCallPaymentForm" TagPrefix="uc" %>
<%@ Register Src="~/Checkout/PaymentForms/PurchaseOrderPaymentForm.ascx" TagName="PurchaseOrderPaymentForm" TagPrefix="uc" %>
<%@ Register Src="~/Checkout/PaymentForms/GiftCertificatePaymentForm.ascx" TagName="GiftCertificatePaymentForm" TagPrefix="uc" %>
<%@ Register Src="~/ConLib/CheckoutProgress.ascx" TagName="CheckoutProgress" TagPrefix="uc" %>
<%@ Register Src="~/ConLib/BasketTotalSummary.ascx" TagName="BasketTotalSummary" TagPrefix="uc" %>
<%@ Register Src="~/ConLib/Utility/BasketItemDetail.ascx" TagName="BasketItemDetail" TagPrefix="uc" %>
<ajax:UpdatePanel ID="PaymentAjax" runat="server">
    <ContentTemplate>
        <asp:Panel ID="ConfirmAndPayPanel" runat="server">
            <div class="checkoutPageHeader">
                <uc:CheckoutProgress ID="CheckoutProgress1" runat="server" />
                <h1><asp:Localize ID="Caption" runat="server" Text="Confirm &amp; Pay"></asp:Localize></h1>
                <div class="content">
                    <asp:Label ID="ShippingMethodHelpText" runat="server" Text="Review your order, then enter your billing and payment and submit your order."></asp:Label>
                </div>
            </div>     
            <div class="section">
                <div class="header">
                    <h2><asp:Label ID="ConfirmOrderCaption" runat="server" Text="Confirm Order"></asp:Label></h2>
                </div>
                <div class="content">
                    <asp:Repeater ID="ShipmentRepeater" runat="server" OnItemDataBound="ShipmentRepeater_OnItemDataBound">
                        <ItemTemplate>
                            <table align="center" class="form  selectShippingMethod" cellpadding="0" cellspacing="0" border="1">
                                <tr>
                                    <th colspan="3" class="shipto">
                                        <asp:Label ID="ShipmentCaption" runat="server" Text="SHIPMENT INFORMATION" Visible='<%# (Token.Instance.User.Basket.Shipments.Count == 1) %>'></asp:Label>
                                        <asp:Label ID="ShipmentCaption2" runat="server" Text='<%# string.Format("SHIPMENT {0} OF {1}", ((BasketShipment)Container.DataItem).ShipmentNumber, Token.Instance.User.Basket.Shipments.Count) %>' Visible='<%# (Token.Instance.User.Basket.Shipments.Count > 1) %>'></asp:Label>
                                    </th>
                                </tr>
                                <tr>
                                    <th class="verticalText">
                                        S H I P &nbsp; T O
                                    </th>
                                    <td class="address">
                                        <%# Eval("Address") %><br />
                                        <asp:HyperLink ID="EditDestinationLink" runat="server" Text="change" NavigateUrl='<%# GetEditShipToLink() %>'></asp:HyperLink>
                                    </td>
                                    <td valign="top">
                                        <asp:Label ID="ShippingMethodLabel" runat="server" Text="Shipping Method:" SkinID="FieldHeader"></asp:Label>
                                        <asp:Label ID="ShippingMethod" runat="server" Text='<%#Eval("ShipMethod.Name") %>'></asp:Label>
                                        <asp:HyperLink ID="EditShippingMethod" runat="server" Text="change" NavigateUrl="~/Checkout/ShipMethod.aspx"></asp:HyperLink><br />
                                        <asp:Panel ID="ShipMessagePanel" runat="Server" Visible='<%# !string.IsNullOrEmpty((string)Eval("ShipMessage")) %>'>
                                            <br /><br />
                                            <asp:Label ID="ShipMessageLabel" runat="server" Text="Delivery Instructions:" SkinID="FieldHeader"></asp:Label>
                                            <asp:Label ID="ShipMessage" runat="server" Text='<%#Eval("ShipMessage")%>'></asp:Label>
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3" class="dataSheet">
                                        <asp:GridView ID="ShipmentItemsGrid" runat="server" Width="100%" AutoGenerateColumns="false" DataSource='<%#BasketHelper.GetShipmentItems(Container.DataItem)%>' GridLines="none" SkinID="ItemList" RowStyle-CssClass="odd" AlternatingRowStyle-CssClass="even">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Qty">
                                                    <HeaderStyle Width="50" HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="center" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="Quantity" runat="server" Text='<%#Eval("Quantity")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="SKU">
                                                    <HeaderStyle Width="80" HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="center" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="SKU" runat="server" Text='<%#ProductHelper.GetSKU(Container.DataItem)%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Item">
                                                    <ItemTemplate>
                                                        <uc:BasketItemDetail ID="BasketItemDetail1" runat="server" BasketItem='<%#(BasketItem)Container.DataItem%>' ShowAssets="True" LinkProducts="False" IgnoreKitShipment="false" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                                                    <HeaderTemplate>
                                                        <%#GetTaxHeader()%>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <%#TaxHelper.GetTaxRate((BasketItem)Container.DataItem).ToString("0.####")%>%
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Price">
                                                    <HeaderStyle HorizontalAlign="Right" />
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                    <ItemTemplate>
                                                        <%#TaxHelper.GetInvoicePrice(Token.Instance.User.Basket, (BasketItem)Container.DataItem).ToString("ulc")%>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </td>
                                </tr>
                            </table>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Panel ID="NonShippingItemsPanel" runat="server">
                        <table align="center" class="form" cellpadding="0" cellspacing="0" border="1">
                            <tr>
                                <th colspan="3">
                                    <asp:Label ID="NonShippingItemsCaption" runat="server" Text="NON SHIPPING ITEMS"></asp:Label>
                                </th>
                            </tr>
                            <tr>
                                <td colspan="3" class="dataSheet">
                                    <asp:GridView ID="NonShippingItemsGrid" runat="server" Width="100%" AutoGenerateColumns="false" GridLines="none" SkinID="ItemList" RowStyle-CssClass="odd" AlternatingRowStyle-CssClass="even">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Qty">
                                                <HeaderStyle Width="50" HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" />
                                                <ItemTemplate>
                                                    <asp:Label ID="Quantity" runat="server" Text='<%#Eval("Quantity")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                                <asp:TemplateField HeaderText="SKU">
                                                    <HeaderStyle Width="80" HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="center" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="SKU" runat="server" Text='<%#ProductHelper.GetSKU(Container.DataItem)%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Item">
                                                <ItemTemplate>
                                                    <uc:BasketItemDetail ID="BasketItemDetail1" runat="server" BasketItem='<%#(BasketItem)Container.DataItem%>' ShowAssets="True" LinkProducts="False" IgnoreKitShipment="false" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" Width="50px" />
                                                <HeaderTemplate>
                                                    <%#GetTaxHeader()%>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <%#TaxHelper.GetTaxRate((BasketItem)Container.DataItem).ToString("0.####")%>%
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Price">
                                                <HeaderStyle HorizontalAlign="Right" />
                                            <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                <ItemTemplate>
                                                    <%#TaxHelper.GetInvoicePrice(Token.Instance.User.Basket, (BasketItem)Container.DataItem).ToString("ulc")%>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </div>
            </div>
            <asp:Panel ID="PaymentFormPanel" runat="server" CssClass="section">
                <div class="header">
                    <h2><asp:Localize ID="PaymentMethodCaption" runat="server" Text="Payment Information"></asp:Localize></h2>
                </div>
                <div class="content">
                    <div class="paymentMethodSummary">
                        <%-- order summary is generated by BasketTotalSummary.ascx --%>
                        <uc:BasketTotalSummary ID="BasketTotalSummary1" runat="server" />
                        <asp:Label ID="CouponCodeLabel" runat="server" Text="Enter Coupon Code:" SkinID="FieldHeader"></asp:Label><br />
                        <asp:TextBox ID="CouponCode" runat="server" Width="110px" ValidationGroup="CouponCodeValidation"></asp:TextBox>
                        <asp:Button ID="ApplyCouponButton" runat="server" Text=">" OnClick="ApplyCouponButton_Click" ValidationGroup="CouponCodeValidation" /><br />
                        <asp:Label ID="InvalidCouponMessage" runat="server" Text="Invalid coupon code.<br />" Visible="false" SkinID="ErrorCondition"></asp:Label>
                        <asp:Label ID="NotCombineCouponRemoveMessage" runat="server" Text="The coupon {0} can not be combined with other coupons. Coupons {1} have been removed.<br />" Visible="false" SkinID="ErrorCondition" EnableViewState="false"></asp:Label>
                        <asp:Label ID="CombineCouponRemoveMessage" runat="server" Text="The coupon {0} can not be combined with coupon {1}. Coupon {2} has been removed.<br />" Visible="false" SkinID="ErrorCondition" EnableViewState="false"></asp:Label>
                        <asp:Label ID="ValidCouponMessage" runat="server" Text="Coupon accepted.<br />" Visible="false" SkinID="GoodCondition"></asp:Label><br />
                        
                        <%-- display the billing address --%>
                        <div class="summarySection" style="border-top:1px dashed #000000;">
                            <div class="summarySectionHeader" style="margin-top:20px;">
                                <h3><asp:Localize ID="BillingAddressCaption" runat="server" Text="Billing Address"></asp:Localize></h3>
                            </div>
                            <div class="summarySectionContent">
                                <asp:Label ID="BillingAddress" runat="server" Text=""></asp:Label><br />
                                <asp:HyperLink ID="EditBillingAddressLink" runat="server" Text="change" NavigateUrl="~/Checkout/EditBillAddress.aspx"></asp:HyperLink>
                            </div>
                        </div>
                    </div>
                    <div class="paymentMethodList">
                        <asp:PlaceHolder ID="phPaymentForms" runat="server"></asp:PlaceHolder>
                    </div>
                </div>
            </asp:Panel>
        </asp:Panel>
        <asp:Panel id="InvalidOrderAmountPanel" runat="server" visible="false" CssClass="checkoutPageHeader">
            <uc:CheckoutProgress ID="CheckoutProgress2" runat="server" />
            <h1><asp:Localize ID="InvalidAmountCaption" runat="server" Text="Invalid Order Amount"></asp:Localize></h1>
            <div class="content">
                <asp:Label ID="OrderBelowMinimumAmountMessage" runat="server" Text="Your order does not yet meet the minimum value of {0:ulc}.  You must increase your purchase in order to checkout."></asp:Label>
                <asp:Label ID="OrderAboveMaximumAmountMessage" runat="server" Text="Your order exceeds the maximum value of {0:ulc}.  You must reduce your purchase in order to checkout."></asp:Label>
                <br/><br/>
                <asp:HyperLink ID="BasketLink" SkinID="Button" runat="server" Text="My Cart" NavigateUrl="~/Basket.aspx"/>                    
            </div>
        </asp:Panel>
        <div id="TermsAndConditionsSection" runat="server" visible="false">
            <asp:Label ID="TermsAndConditions" runat="server" Text=""></asp:Label>
            <br/><br/>
            <asp:Button ID="AcceptTermsAndConditions" runat="server" Text="I Accept" OnClick="AcceptTermsAndConditions_Click"  /> 
        </div>
    </ContentTemplate>
</ajax:UpdatePanel>