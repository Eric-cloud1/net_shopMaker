<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GiftWrapPage.ascx.cs" Inherits="ConLib_GiftWrapPage" %>
<%--
<conlib>
<summary>Displays giftwrap options and details when checking out.</summary>
</conlib>
--%>
<%@ Register Src="~/ConLib/Utility/BasketItemDetail.ascx" TagName="BasketItemDetail" TagPrefix="uc" %>
<%@ Register Src="~/ConLib/Utility/GiftWrapOptions.ascx" TagName="GiftWrapOptions" TagPrefix="uc" %>
<asp:Repeater ID="ShipmentRepeater" runat="server" EnableViewState="false" OnItemDataBound="ShipmentRepeater_ItemDataBound">
    <ItemTemplate>
        <table align="center" class="form selectShippingMethod" cellpadding="0" cellspacing="0" border="1">
            <tr>
                <th colspan="2" class="shipto">
                    <asp:Literal ID="ShipmentCaption" runat="server" Text="SHIP TO: " EnableViewState="false"></asp:Literal>
                    <asp:Literal ID="ShipToAddress" runat="server" Text="<%# ((BasketShipment)Container.DataItem).Address.ToString(false) %>" EnableViewState="false"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td class="dataSheet">
                    <asp:GridView ID="ShipmentItemsGrid" runat="server" Width="100%" AutoGenerateColumns="false" 
                        DataSource='<%#GetShipmentItems(Container.DataItem)%>' SkinID="ItemList" 
                        DataKeyNames="BasketItemId" OnRowDataBound="ShipmentItemsGrid_RowDataBound" EnableViewState="false" 
                        RowStyle-CssClass="altodd" AlternatingRowStyle-CssClass="alteven" BorderColor="DarkGray" BorderStyle="Solid">
                        <Columns>
                            <asp:TemplateField HeaderText="Qty">
                                <HeaderStyle Width="50" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="center" VerticalAlign="top" />
                                <ItemTemplate>
                                    <asp:Literal ID="Quantity" runat="server" Text='<%#Eval("Quantity")%>' EnableViewState="false"></asp:Literal>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="SKU">
                                <HeaderStyle Width="80" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="center" VerticalAlign="top" />
                                <ItemTemplate>
                                    <asp:Literal ID="SKU" runat="server" Text='<%#ProductHelper.GetSKU(Container.DataItem)%>' EnableViewState="false"></asp:Literal>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Item">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle VerticalAlign="top" HorizontalAlign="Left" />
                                <ItemTemplate>
                                    <uc:BasketItemDetail ID="BasketItemDetail1" runat="server" BasketItem='<%#(BasketItem)Container.DataItem%>' ShowAssets="false" LinkProducts="False" ForceKitDisplay="true" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" Width="50px" VerticalAlign="Top" />
                                <HeaderTemplate>
                                    <%#GetTaxHeader()%>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <%#TaxHelper.GetTaxRate((BasketItem)Container.DataItem).ToString("0.####")%>%
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Price">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Right" Width="50px" VerticalAlign="Top"  />
                                <ItemTemplate>
                                    <%#TaxHelper.GetInvoicePrice(Token.Instance.User.Basket, (BasketItem)Container.DataItem, true).ToString("ulc")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Gift Options">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle VerticalAlign="Top" Wrap="False" CssClass="giftOptions" />
                                <ItemTemplate>
                                    <uc:GiftWrapOptions ID="GiftWrapOptions1" runat="server" BasketItemId='<%# Eval("BasketItemId") %>' WrapGroupId='<%#Eval("Product.WrapGroupId")%>' WrapStyleId='<%#Eval("WrapStyleId")%>' GiftMessage='<%#Eval("GiftMessage")%>' /><br />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </ItemTemplate>
</asp:Repeater>
<div align="right">
    <asp:ImageButton ID="ContinueButton" runat="server" AlternateText="Continue" SkinID="Continue" OnClick="ContinueButton_Click" EnableViewState="false" />
</div>