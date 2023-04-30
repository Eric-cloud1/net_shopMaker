<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MyOrderPage.ascx.cs" Inherits="ConLib_MyOrderPage" %>
<%--
<conlib>
<summary>Display page to show details of an order like order items, shipping address, billing address etc.</summary>
<param name="AllowAddNote" default="true">If true, the customer can add notes to the order.  If false, the customer can only see notes added by the merchant.</param>
<param name="HandleFailedPayments" default="false">If true, the customer is redirected to an order payment page if the payment fails at checkout.</param>
</conlib>
--%>
<%@ Register Src="~/ConLib/OrderTotalSummary.ascx" TagName="OrderTotalSummary" TagPrefix="uc" %>
<%@ Register Src="~/ConLib/BreadCrumbs.ascx" TagName="BreadCrumbs" TagPrefix="uc" %>
<%@ Register Src="~/ConLib/Utility/OrderItemDetail.ascx" TagName="OrderItemDetail" TagPrefix="uc" %>
<%@ Register Src="~/ConLib/Utility/PayPalPayNowButton.ascx" TagName="PayPalPayNowButton" TagPrefix="uc" %>
<asp:PlaceHolder ID="BalanceDuePanel" runat="server" Visible="false" EnableViewState="false">
    <br />
    <asp:Label ID="BalanceDueMessage" runat="server" Text="** Your order has a balance of {0:lc} due.&nbsp&nbsp;<a href='{1}'><u>Pay Now</u></a>" SkinID="ErrorCondition"></asp:Label>
    <br /><br />
</asp:PlaceHolder>
<asp:PlaceHolder ID="OrderInvalidPanel" runat="server" Visible="false" EnableViewState="false">
    <br />
	<asp:Label ID="OrderInvalidMessage" runat="server" Text="** Your order has been cancelled or invalidated." SkinID="ErrorCondition"></asp:Label>
    <br /><br />
</asp:PlaceHolder>
<asp:Panel ID="PageHeaderPanel" runat="server" CssClass="pageHeader">
    <h1><asp:Localize ID="Caption" runat="server" Text="View Order #{0}"></asp:Localize></h1>
</asp:Panel>
<asp:Panel ID="OrderSummaryPanel" runat="server" SkinID="OrderSummaryPanel">
    <table class="orderSummaryTable">
        <tr>
            <th class="rowHeader">
                <asp:Label ID="OrderDateLabel" runat="server" Text="Order Date: "></asp:Label>
            </th>
            <td>
                <asp:Label ID="OrderDate" runat="server" Text=""></asp:Label>
            </td>
            <th class="rowHeader">
                <asp:Label ID="TotalChargesLabel" runat="server" Text="Order Total: "></asp:Label>
            </th>
            <td>
                <asp:Label ID="TotalCharges" runat="server" Text=""></asp:Label>
            </td>
            <th class="rowHeader">
                <asp:Label ID="BalanceLabel" runat="server" Text="Balance: "></asp:Label>
            </th>
            <td>
                <asp:Label ID="Balance" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <th class="rowHeader">
                <asp:Label ID="OrderStatusLabel" runat="server" Text="Status: "></asp:Label>
            </th>
            <td>
                <asp:Label ID="OrderStatus" runat="server" Text=""></asp:Label>
            </td>
            <th class="rowHeader">
                <asp:Label ID="TotalPaymentsLabel" runat="server" Text="Payments: "></asp:Label>
            </th>
            <td>
                <asp:Label ID="TotalPayments" runat="server" Text=""></asp:Label>
            </td>
			<th class="rowHeader">
                <asp:Label ID="UnprocessedPaymentsLabel" runat="server" Text="Unprocessed Payments: "></asp:Label>
            </th>
            <td>
                <asp:Label ID="UnprocessedPayments" runat="server" Text=""></asp:Label>				
            </td>
        </tr >
		<tr>
			<td colspan="6" align="right">
                <asp:HyperLink ID="PrintOrder" runat="server" Text="Printable Version" NavigateUrl="~/Members/PrintMyOrder.aspx"></asp:HyperLink>
            </td>
		</tr>
    </table>
</asp:Panel>
<asp:Repeater ID="ShipmentRepeater" runat="server" >
    <ItemTemplate>
        <table align="center" class="form" cellpadding="0" cellspacing="0" border="1">
            <tr>
                <th colspan="3">
                    <asp:Localize ID="ShipmentCaption" runat="server" Text="SHIPMENT INFORMATION" Visible='<%# (this.Order.Shipments.Count == 1) %>'></asp:Localize>
                    <asp:Localize ID="ShipmentCaption2" runat="server" Text='<%# string.Format("SHIPMENT {0} OF {1}", ((OrderShipment)Container.DataItem).ShipmentNumber, this.Order.Shipments.Count) %>' Visible='<%# (this.Order.Shipments.Count > 1) %>'></asp:Localize>
                </th>
            </tr>
            <tr>
                <th class="verticalText">
                    S<br />
                    H<br />
                    I<br />
                    P<br /><br />
                    T<br />
                    O<br />
                </th>
                <td class="address">
                    <%# GetShipToAddress(Container.DataItem) %>
                </td>
                <td valign="top" class="expand">
                    <asp:Label ID="ShipStatusLabel" runat="server" Text="Status:" SkinID="FieldHeader"></asp:Label>
                    <asp:Label ID="ShipStatus" runat="server" Text='<%#GetShipStatus(Container.DataItem)%>'></asp:Label><br />
                    <asp:Label ID="ShippingMethodLabel" runat="server" Text="Shipping Method:" SkinID="FieldHeader"></asp:Label>
                    <asp:Label ID="ShippingMethod" runat="server" Text='<%#Eval("ShipMethodName") %>'></asp:Label>
                    <asp:Panel ID="TrackingNumberPanel" runat="Server" Visible='<%#HasTrackingNumbers(Container.DataItem)%>'>
                        <br />         
                        <asp:Label ID="TrackingNumbersLabel" runat="server" Text="Tracking :" SkinID="FieldHeader"></asp:Label>
                        <asp:Repeater ID="TrackingRepeater" runat="server" DataSource='<%#Eval("TrackingNumbers")%>'>
                            <ItemTemplate>
                                <asp:HyperLink ID="TrackingNumberData" Target="_blank" runat="server" Text='<%#Eval("TrackingNumberData")%>' NavigateUrl='<%#GetTrackingUrl(Container.DataItem)%>'></asp:HyperLink>
                            </ItemTemplate>
                            <SeparatorTemplate>, </SeparatorTemplate>
                        </asp:Repeater>                         
                    </asp:Panel>
                    <asp:Panel ID="ShipMessagePanel" runat="Server" Visible='<%# !string.IsNullOrEmpty((string)Eval("ShipMessage")) %>'>
                        <br /><br />
                        <asp:Label ID="ShipMessageLabel" runat="server" Text="Delivery Instructions:" SkinID="FieldHeader"></asp:Label>
                        <asp:Label ID="ShipMessage" runat="server" Text='<%#Eval("ShipMessage")%>'></asp:Label>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td colspan="3" class="dataSheet">
                    <asp:GridView ID="ShipmentItemsGrid" runat="server" Width="100%" AutoGenerateColumns="false" DataSource='<%# OrderHelper.GetShipmentItems(Container.DataItem) %>' GridLines="none" SkinID="ItemList" OnDataBinding="ItemsGrid_DataBinding">
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
                                    <uc:OrderItemDetail ID="OrderItemDetail1" runat="server" OrderItem='<%#(OrderItem)Container.DataItem%>' ShowAssets="False" LinkProducts="False" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Tax">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" Width="40px" />
                                <ItemTemplate>
                                    <%#TaxHelper.GetTaxRate(Order, (OrderItem)Container.DataItem).ToString("0.####")%>%
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Price">
                                <HeaderStyle Width="80" HorizontalAlign="Right" />
                                <ItemStyle HorizontalAlign="Right" />
                                <ItemTemplate>
                                    <asp:Label ID="Price" runat="server" Text='<%#TaxHelper.GetInvoiceExtendedPrice(Order, (OrderItem)Container.DataItem).ToString("ulc")%>'></asp:Label>
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
                <asp:Localize ID="NonShippingItemsCaption" runat="server" Text="NON SHIPPING ITEMS"></asp:Localize>
            </th>
        </tr>
        <tr>
            <td colspan="3" class="dataSheet">
                <asp:GridView ID="NonShippingItemsGrid" runat="server" Width="100%" AutoGenerateColumns="false" GridLines="none" SkinID="PagedList" OnDataBinding="ItemsGrid_DataBinding">
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
                                <uc:OrderItemDetail ID="OrderItemDetail1" runat="server" OrderItem='<%#(OrderItem)Container.DataItem%>' ShowAssets="False" LinkProducts="False" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tax">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" Width="40px" />
                            <ItemTemplate>
                                <%#TaxHelper.GetTaxRate(Order, (OrderItem)Container.DataItem).ToString("0.####")%>%
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Price">
                            <HeaderStyle Width="80" HorizontalAlign="Right" />
                            <ItemStyle HorizontalAlign="Right" />
                            <ItemTemplate>
                                <asp:Label ID="Price" runat="server" Text='<%#TaxHelper.GetInvoiceExtendedPrice(Order, (OrderItem)Container.DataItem).ToString("ulc")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:PlaceHolder ID="DigitalGoodsPanel" runat="server">
    <table align="center" class="form" cellpadding="0" cellspacing="0" border="1">
        <tr>
            <th colspan="3">
                <asp:Localize ID="DigitalGoodsCaption" runat="server" Text="DIGITAL GOODS"></asp:Localize>
            </th>
        </tr>
        <tr>
            <td colspan="3" class="dataSheet">
                <asp:GridView ID="DigitalGoodsGrid" runat="server" AutoGenerateColumns="false" 
                    GridLines="none" OnRowDataBound="DigitalGoodsGrid_RowDataBound" Width="100%"
                    SkinID="PagedList">
                    <Columns>
                        <asp:TemplateField HeaderText="Name">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" Width="200px" />
                            <ItemTemplate>
                                <%#Eval("Name")%>
                                <asp:PlaceHolder ID="phAssets" runat="server"></asp:PlaceHolder>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Download">
                            <ItemStyle HorizontalAlign="Center" Width="80px" />
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:PlaceHolder ID="phDownloadIcon" runat="server" Visible='<%# (((DownloadStatus)Eval("DownloadStatus")) == DownloadStatus.Valid && DGFileExists(Eval("DigitalGood")))%>'>
                                    <a href="<%#GetDownloadUrl(Container.DataItem)%>"><asp:Image ID="DI" runat="server" SkinID="DownloadIcon" AlternateText="Download" /></a>
                                </asp:PlaceHolder>
                                <asp:Literal ID="DownloadStatus" runat="server" Text='<%# Eval("DownloadStatus") %>' Visible='<%# ((DownloadStatus)Eval("DownloadStatus")) != DownloadStatus.Valid %>'></asp:Literal>
                                <asp:Literal ID="MissingDownloadText" runat="server" Text="unavailable" EnableViewState="false" Visible='<%#( !DGFileExists(Eval("DigitalGood")) && ((DownloadStatus)Eval("DownloadStatus")) == DownloadStatus.Valid)%>'/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Remaining">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" Width="80px" />
                            <ItemTemplate>
                                <%#GetMaxDownloads(Container.DataItem)%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                            <ItemTemplate>
                                <asp:Label ID="SerialKeyLabel" runat="server" Text="Serial Key: " SkinID="FieldHeader" Visible='<%#!string.IsNullOrEmpty(Eval("SerialKeyData").ToString())%>'></asp:Label>
                                <asp:Literal ID="SerialKey" runat="server" Visible='<%#ShowSerialKey(Container.DataItem)%>' Text='<%#Eval("SerialKeyData")%>' />
                                <asp:LinkButton runat="server" ID="SerialKeyLink" Text="view" OnClientClick="<%#GetPopUpScript(Container.DataItem)%>" Visible='<%#ShowSerialKeyLink(Container.DataItem)%>'></asp:LinkButton>
                                <asp:Literal ID="LineBreak" runat="server" Text="<br>" Visible='<%#ShowSerialKey(Container.DataItem) || ShowSerialKeyLink(Container.DataItem)%>'></asp:Literal>
                                <asp:Label ID="MediaKeyLabel" runat="server" Text="Media Key: " SkinID="FieldHeader" Visible='<%#ShowMediakey(Container.DataItem)%>' ToolTip="This key will be required to open the download." ></asp:Label>
                                <asp:Label ID="MediaKey" runat="server" Visible='<%#ShowMediakey(Container.DataItem)%>' Text='<%#Eval("DigitalGood.MediaKey")%>' ToolTip="This key will be required to open the download."/>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>           
    </table>
</asp:PlaceHolder>
<asp:Panel ID="GiftCertificatesPanel" runat="server">
    <table align="center" class="form" cellpadding="0" cellspacing="0" border="1">
        <tr>
            <th colspan="3">
                <asp:Localize ID="Localize1" runat="server" Text="GIFT CERTIFICATES"></asp:Localize>
            </th>
        </tr>
         <tr>
            <td colspan="3" class="dataSheet">
                <asp:GridView ID="GiftCertificatesGrid" runat="server" Width="100%" AutoGenerateColumns="False" GridLines="none" SkinID="ItemList">
                    <Columns>
                        <asp:TemplateField HeaderText="Name" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:HyperLink ID="Name" runat="server" Text='<%#Eval("Name")%>' NavigateUrl='<%# Eval("GiftCertificateId", "~/Members/MyGiftCertificate.aspx?GiftCertificateId={0}")%>'></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:Label ID="Status" runat="server" Text='<%#GetGCDescription(Container.DataItem)%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                          <asp:TemplateField HeaderText="Expiration Date" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:Label ID="ExpirationDate" runat="server" Text='<%#Eval("ExpirationDate", "{0:d}")%>' visible='<%# ((DateTime)Eval("ExpirationDate") != DateTime.MinValue) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>                          
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="PaymentPanel" runat="server">
<table align="center" class="form" cellpadding="0" cellspacing="0" border="1">
    <tr>
        <th colspan="2">
            <asp:Localize ID="PaymentMethodCaption" runat="server" Text="PAYMENT INFORMATION"></asp:Localize>
        </th>
    </tr>
    <tr>
        <td class="orderSummary" valign="top">
            <uc:OrderTotalSummary ID="OrderTotalSummary1" runat="server" />
        </td>
        <td valign="top" class="expand">
            <asp:Label ID="BillingAddressCaption" runat="server" Text="Billing Address:" SkinID="FieldHeader"></asp:Label><br />
            <asp:Literal ID="BillToAddress" runat="server"></asp:Literal><br /><br />
            <asp:Repeater ID="PaymentRepeater" runat="server">
                <ItemTemplate>
                    <asp:Label ID="PaymentMethodLabel" runat="server" Text="Payment Method: " SkinID="FieldHeader"></asp:Label>
                    <asp:Label ID="PaymentMethodName" runat="server" Text='<%#Eval("PaymentMethodName")%>'></asp:Label> 
                    <asp:Label ID="ReferenceNumber" runat="server" Text='<%#Eval("ReferenceNumber")%>'></asp:Label><br />
                    <asp:Label ID="PaymentStatusLabel" runat="server" Text="Status: " SkinID="FieldHeader"></asp:Label>
                    <asp:Label ID="PaymentStatus" runat="server" Text='<%#StoreDataHelper.GetFriendlyPaymentStatus((Payment)Container.DataItem)%>'></asp:Label>
                    <asp:Panel ID="ExtendedDetailsPanel" runat="server" Visible='<%#ShowExtendedPaymentDetails(Container.DataItem) %>'>
                        <asp:Label ID="PaymentDateLabel" runat="server" Text="Date: " SkinID="FieldHeader"></asp:Label>
                        <asp:Label ID="PaymentDate" runat="server" Text='<%#Eval("PaymentDate", "{0:g}")%>'></asp:Label><br />
                        <asp:Label ID="AmountLabel" runat="server" Text="Amount: " SkinID="FieldHeader"></asp:Label>
                        <asp:Label ID="Amount" runat="server" Text='<%#Eval("Amount", "{0:ulc}")%>'></asp:Label><br />
                    </asp:Panel>
                    <asp:Panel ID="MailPayMethodMessage" runat="server" Visible='<%# ShowMailPaymentMessage(Container.DataItem) %>'>
                        <asp:Label ID="MessageLabel" runat="server" Text="Make your check payable to:" SkinID="FieldHeader"></asp:Label><br />
                        <asp:Label ID="StoreNameLabel" runat="server" Text='<%# Token.Instance.Store.Name%>'></asp:Label><br />
                        <asp:Label ID="StoreAddress" runat="server" Text='<%# Token.Instance.Store.DefaultWarehouse.FormatAddress(true)%>'></asp:Label>
                    </asp:Panel>
                    <uc:PayPalPayNowButton ID="PayPalPayNowButton" runat="server" PaymentId='<%#Eval("PaymentId")%>'></uc:PayPalPayNowButton><br />
                </ItemTemplate>
            </asp:Repeater>
        </td>
    </tr>
</table>
</asp:Panel>    
<asp:Panel ID="OrderNotesPanel" runat="server">
    <ajax:UpdatePanel ID="OrderNotesAjax" runat="server">
        <ContentTemplate>
            <table align="center" class="form" cellpadding="0" cellspacing="0" border="1">
                <tr>
                    <th>
                        <asp:Label ID="OrderNotesCaption" runat="server" Text="ORDER NOTES"></asp:Label>
                    </th>
                </tr>
                <tr>
                    <td class="dataSheet">
                        <asp:GridView ID="OrderNotesGrid" runat="server" Width="100%" AutoGenerateColumns="false" 
                            GridLines="none" SkinID="ItemList" EnableViewState="false">
                            <Columns>
                                <asp:TemplateField HeaderText="Date" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# Eval("CreatedDate") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="From">
                                    <ItemTemplate>
                                        <%# GetNoteAuthor(Container.DataItem) %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Comment">
                                    <ItemTemplate>
                                        <%# Eval("Comment") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <tr id="trAddNote" runat="server">
                    <td>
                        <table class="inputForm">
                            <tr>
                                <th class="rowHeader" valign="top">
                                    Add Note:
                                </th>
                                <td>
                                    <asp:TextBox ID="NewOrderNote" runat="server" TextMode="multiline" Width="400px" Height="80px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td><asp:LinkButton ID="NewOrderNoteButton" runat="server" Text="Submit my Note" SkinID="Button" OnClick="NewOrderNoteButton_Click"></asp:LinkButton></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Panel>
<asp:Panel ID="SubscriptionsPanel" runat="server">
    <table align="center" class="form" cellpadding="0" cellspacing="0" border="1">
        <tr>
            <th>
                <asp:Localize ID="SubscriptionsCaption" runat="server" Text="SUBSCRIPTIONS"></asp:Localize>
            </th>
        </tr>
        <tr>
            <td class="dataSheet">
                <asp:GridView ID="SubscriptionGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="SubscriptionId" DataSourceID="SubscriptionDs" 
                    SkinID="ItemList" AllowSorting="False" AllowPaging="false" CellSpacing="4" BorderWidth="0" EnableViewState="false">
                    <Columns>
                        <asp:TemplateField HeaderText="Subscription Plan">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                            <ItemTemplate>
                                <asp:Label ID="SubscriptionPlan" runat="server" text='<%#Eval("SubscriptionPlan.Name")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ship To">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                            <ItemTemplate>
                                <asp:Label ID="ShipTo" runat="server" text='<%#Eval("OrderItem.OrderShipment.ShipToFullName")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Active">
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:CheckBox ID="Active" runat="server" Checked='<%#Eval("IsActive")%>' Enabled="False" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Expiration">
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Label ID="Expiration" runat="server" text='<%#Eval("ExpirationDate", "{0:d}")%>' visible='<%# ((DateTime)Eval("ExpirationDate") != DateTime.MinValue) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <asp:Label ID="EmptyMessage" runat="server" Text="There are no subscriptions associated with this order."></asp:Label> 
                    </EmptyDataTemplate>
                </asp:GridView>
                <asp:ObjectDataSource ID="SubscriptionDs" runat="server" OldValuesParameterFormatString="original_{0}"
                    SelectMethod="LoadForOrder" TypeName="MakerShop.Orders.SubscriptionDataSource" DataObjectTypeName="MakerShop.Orders.Subscription">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="OrderId" QueryStringField="OrderId" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Panel>
