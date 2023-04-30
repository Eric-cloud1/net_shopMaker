<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PrintMyOrderPage.ascx.cs" Inherits="ConLib_PrintMyOrderPage" %>
<%--
<conlib>
<summary>Customer account page to display a printable version of order details.</summary>
</conlib>
--%>
<%@ Register Src="~/ConLib/OrderTotalSummary.ascx" TagName="OrderTotalSummary" TagPrefix="uc" %>
<%@ Register Src="~/ConLib/Utility/OrderItemDetail.ascx" TagName="OrderItemDetail" TagPrefix="uc" %>
<div class="noPrint">    
    <div class="pageHeader">
        <h1><asp:Localize ID="Caption" runat="server" Text="Order #{0}"></asp:Localize></h1>
    </div>
    <asp:LinkButton ID="PrintButton" runat="server" Text="Print" SkinID="Button" OnClientClick="window.print();return false;" />
    <asp:LinkButton ID="BackButton" runat="server" Text="Back" SkinID="Button" OnClick="BackButton_Click" />
    <br /><br />
</div>
<table align="center" class="form" cellpadding="0" cellspacing="0" border="1">
    <tr>
        <th class="verticalText">
            S<br />
            O<br />
            L<br />
            D<br /><br />
            B<br />
            Y<br />
        </th>        
        <td class="address">
            <h1 class="storeName"><asp:Localize ID="StoreName" runat="server"></asp:Localize></h1>
            <asp:Literal ID="StoreAddress" runat="server"></asp:Literal>
        </td>
        <td align="right" valign="top" class="expand">
            <h1 class="invoice">INVOICE</h1>
            <asp:Label ID="OrderNumberLabel" runat="server" Text="Order Number:" SkinID="FieldHeader"></asp:Label>
            <asp:Label ID="OrderNumber" runat="server" Text=""></asp:Label><br />
            <asp:Label ID="OrderDateLabel" runat="server" Text="Order Date: " SkinID="FieldHeader"></asp:Label>
            <asp:Label ID="OrderDate" runat="server" Text=""></asp:Label><br />
            <asp:Label ID="OrderStatusLabel" runat="server" Text="Status: " SkinID="FieldHeader"></asp:Label>
            <asp:Label ID="OrderStatus" runat="server" Text=""></asp:Label>
        </td>
    </tr>
</table>
<asp:Repeater ID="ShipmentRepeater" runat="server" OnItemDataBound="ShipmentRepeater_ItemDataBound">
    <ItemTemplate>
        <table align="center" class="form" cellpadding="0" cellspacing="0" border="1">
            <tr>
                <th colspan="3" class="header">
                                        <asp:Localize ID="ShipmentCaption" runat="server" Text="SHIPMENT {0} of {1}" EnableViewState="false"></asp:Localize>
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
                    <asp:Label ID="ShippingMethodLabel" runat="server" Text="Shipping Method:" SkinID="FieldHeader"></asp:Label>
                    <asp:Label ID="ShippingMethod" runat="server" Text='<%#Eval("ShipMethodName") %>'></asp:Label>
                    <asp:Panel ID="ShipMessagePanel" runat="Server" Visible='<%# !string.IsNullOrEmpty((string)Eval("ShipMessage")) %>'>
                        <br /><br />
                        <asp:Label ID="ShipMessageLabel" runat="server" Text="Customer Comment:" SkinID="FieldHeader"></asp:Label>
                        <asp:Label ID="ShipMessage" runat="server" Text='<%#Eval("ShipMessage")%>'></asp:Label>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td colspan="3" class="dataSheet">
                    <asp:GridView ID="ShipmentItemsGrid" runat="server" AutoGenerateColumns="false" 
                        DataSource='<%# OrderHelper.GetShipmentItems(Container.DataItem) %>' GridLines="none" SkinID="ItemList" OnDataBinding="ItemsGrid_DataBinding">
                        <Columns>
                            <asp:TemplateField HeaderText="Qty">
                                <HeaderStyle Width="50px" HorizontalAlign="Center" />
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:Label ID="Quantity" runat="server" Text='<%#Eval("Quantity")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="SKU">
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle Width="80px" HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:Label ID="SKU" runat="server" Text='<%#ProductHelper.GetSKU(Container.DataItem)%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Item">
                                <ItemTemplate>
                                    <uc:OrderItemDetail ID="OrderItemDetail1" runat="server" OrderItem='<%#(OrderItem)Container.DataItem%>' ShowAssets="True" LinkProducts="False" />
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
                                <HeaderStyle Width="80px" HorizontalAlign="Right" />
                                <ItemStyle Width="80px" HorizontalAlign="Right" />
                                <ItemTemplate>
                                    <%#TaxHelper.GetInvoiceExtendedPrice(Order, (OrderItem)Container.DataItem).ToString("ulc")%><br />
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
            <th class="header">
                <asp:Localize ID="NonShippingItemsCaption" runat="server" Text="NON SHIPPING ITEMS"></asp:Localize>
            </th>
        </tr>
        <tr>
            <td class="dataSheet">
                <asp:GridView ID="NonShippingItemsGrid" runat="server" Width="100%" AutoGenerateColumns="false" GridLines="none" SkinID="ItemList" OnDataBinding="ItemsGrid_DataBinding">
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
                                <uc:OrderItemDetail ID="OrderItemDetail1" runat="server" OrderItem='<%#(OrderItem)Container.DataItem%>' ShowAssets="True" LinkProducts="False" />
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
                                <%#TaxHelper.GetInvoiceExtendedPrice(Order, (OrderItem)Container.DataItem).ToString("ulc")%><br />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="DigitalGoodsPanel" runat="server">
    <table align="center" class="form" cellpadding="0" cellspacing="0" border="1">
        <tr>
            <th class="header">
                <asp:Localize ID="DigitalGoodsCaption" runat="server" Text="DIGITAL GOODS"></asp:Localize>
            </th>
        </tr>
        <tr>
            <td class="dataSheet">
                <asp:GridView ID="DigitalGoodsGrid" runat="server" Width="100%" AutoGenerateColumns="false" GridLines="none" SkinID="ItemList">
                    <Columns>
                        <asp:TemplateField HeaderText="Name" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:HyperLink ID="Name" runat="server" Text='<%#Eval("DigitalGood.Name")%>' NavigateUrl='<%# GetDownloadUrl(Container.DataItem) %>'></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="# Downloads" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:Label ID="Downloads" runat="server" Text='<%#Eval("RelevantDownloads")%>' />
                                <asp:Label ID="MaxDownloads" runat="server" Text='<%#GetMaxDownloads(Container.DataItem)%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:Label ID="DownloadStatus" runat="server" Text='<%#Eval("DownloadStatus")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
</asp:Panel>
<table align="center" class="form" cellpadding="0" cellspacing="0" border="1">
    <tr>
        <th colspan="3" class="header">
            <asp:Localize ID="PaymentMethodCaption" runat="server" Text="PAYMENT INFORMATION"></asp:Localize>
        </th>
    </tr>
    <tr>
        <td class="orderSummary">
            <uc:OrderTotalSummary ID="OrderTotalSummary1" runat="server" />
        </td>
        <td valign="top" colspan="2" class="expand">
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
                        <br />
                        <asp:Label ID="PaymentDateLabel" runat="server" Text="Date: " SkinID="FieldHeader"></asp:Label>
                        <asp:Label ID="PaymentDate" runat="server" Text='<%#Eval("PaymentDate", "{0:g}")%>'></asp:Label><br />
                        <asp:Label ID="AmountLabel" runat="server" Text="Amount: " SkinID="FieldHeader"></asp:Label>
                        <asp:Label ID="Amount" runat="server" Text='<%#Eval("Amount", "{0:ulc}")%>'></asp:Label><br />
                    </asp:Panel>
                </ItemTemplate>
            </asp:Repeater>
        </td>
    </tr>
</table>    
<asp:Panel ID="OrderNotesPanel" runat="server">
    <table align="center" class="form" cellpadding="0" cellspacing="0" border="1">
        <tr>
            <th>
                <asp:Label ID="OrderNotesCaption" runat="server" Text="ORDER NOTES"></asp:Label>
            </th>
        </tr>
        <tr>
            <td class="dataSheet">
                <asp:GridView ID="OrderNotesGrid" runat="server" Width="100%" AutoGenerateColumns="false" GridLines="none" SkinID="ItemList">
                    <Columns>
                        <asp:TemplateField HeaderText="Date" ItemStyle-Wrap="false">
                            <ItemTemplate>
                                <asp:Label ID="CreatedDate" runat="server" Text='<%# Eval("CreatedDate") %>'></asp:Label><br />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Comment">
                            <ItemTemplate>
                                <asp:Label ID="Comment" runat="server" Text='<%# Eval("Comment") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
</asp:Panel>