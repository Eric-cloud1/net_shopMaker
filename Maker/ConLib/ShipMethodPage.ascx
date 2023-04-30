<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ShipMethodPage.ascx.cs" Inherits="ConLib_ShipMethodPage" %>
<%--
<conlib>
<summary>The page displays cart contents and  all the applicable shipping methods available for those with shipping rates. Customer selects a shipping method for each shipment using this page.</summary>
<param name="ShowWeight" default="true">Possible values are true or false.  Indicates whether the shipping address should be displayed with each item or not.</param>
</conlib>
--%>
<%@ Register Src="~/ConLib/CheckoutProgress.ascx" TagName="CheckoutProgress" TagPrefix="uc" %>
<%@ Register Src="~/ConLib/Utility/BasketItemDetail.ascx" TagName="BasketItemDetail" TagPrefix="uc" %>
<div class="checkoutPageHeader">
    <uc:CheckoutProgress ID="CheckoutProgress1" runat="server" />
    <h1><asp:Localize ID="Caption" runat="server" Text="Select Shipping Method"></asp:Localize></h1>
    <div class="content">
        <asp:Label ID="ShippingMethodHelpText" runat="server" Text="Choose your shipping method below and click Continue."></asp:Label>
        <br />
        <asp:Label ID="NoShippingMethodMessage" runat="server" Visible="false" Text="No suitable shipping method available to ship the products in your cart. Please contact the merchant for assistance in completing your order." SkinID="ErrorCondition" EnableViewState="false"></asp:Label>
    </div>
</div>
<asp:Repeater ID="ShipmentRepeater" runat="server" OnItemDataBound="ShipmentRepeater_ItemDataBound">
    <ItemTemplate>
        <table align="center" class="form selectShippingMethod" cellpadding="0" cellspacing="0" border="1">
            <tr>
                <th colspan="2" class="shipto">
                   <asp:Localize ID="ShipmentCaption" runat="server" Text="SHIP TO: {0}"></asp:Localize>
                </th>
            </tr>
            <tr>
                <td class="dataSheet" valign="top" width="100%">
                    <asp:PlaceHolder ID="ShipWeightPanel" runat="server" Visible='<%#ShowWeight%>'>
                        <div style="padding:4px;">
                            <asp:Label ID="ShipWeightLabel" runat="server" Text="Shipping Weight: " SkinID="FieldHeader"></asp:Label>
                            <asp:Literal ID="ShipWeight" runat="server"></asp:Literal>
                        </div>
                    </asp:PlaceHolder>
                    <asp:GridView ID="ShipmentItemsGrid" runat="server" AutoGenerateColumns="false" DataSource='<%#BasketHelper.GetShipmentItems(Container.DataItem)%>' GridLines="none" SkinID="ItemList" RowStyle-CssClass="odd" AlternatingRowStyle-CssClass="even" >
                        <Columns>
                            <asp:TemplateField HeaderText="Qty">
                                <HeaderStyle Width="50" HorizontalAlign="Center"/>
                                <ItemStyle HorizontalAlign="center" />
                                <ItemTemplate  >
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
                                <HeaderStyle Width="100%" HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <uc:BasketItemDetail ID="BasketItemDetail1" runat="server" BasketItem='<%#(BasketItem)Container.DataItem%>' ShowAssets="false" LinkProducts="False" IgnoreKitShipment="false" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Tax">
                                <HeaderStyle CssClass="columnHeader" />
                                <ItemStyle HorizontalAlign="Center" Width="40px" />
                                <HeaderTemplate>
                                    <%#GetTaxHeader()%>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <%#TaxHelper.GetTaxRate((BasketItem)Container.DataItem).ToString("0.####")%>%
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Price">
                                <HeaderStyle Width="80" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="right" />
                                <ItemTemplate>
                                    <asp:Label ID="Price" runat="server" Text='<%#TaxHelper.GetInvoiceExtendedPrice(Token.Instance.User.Basket, (BasketItem)Container.DataItem).ToString("ulc")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
                <td class="methods" valign="top" width="100%">
                    <h3><asp:Localize ID="ShippingMethodCaption" runat="server" Text="Select Shipping Method"></asp:Localize></h3>
	                <asp:GridView ID="ShipMethodGrid" runat="server" DataKeyNames="ShipMethodId" AutoGenerateColumns="false" ShowHeader="false" GridLines="None" OnRowDataBound="ShipMethodGrid_RowDataBound" Width="100%">
	                    <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:PlaceHolder ID="phRadio" runat="server"></asp:PlaceHolder>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
	                            <ItemTemplate>
	                                <asp:Label ID="ShipMethodName" runat="server" Text='<%#Eval("Name")%>' />
	                                <asp:Label ID="ShipMethodRate" runat="server" Text='<%#Eval("TotalRate", " ({0:ulc})") %>' Visible='<%#((LSDecimal)Eval("TotalRate") > 0)%>' />
	                            </ItemTemplate>
                            </asp:TemplateField>
	                    </Columns>
	                </asp:GridView><br />
                    <asp:Label ID="ShipMessageCaption" runat="server" Text="Delivery Instructions:" SkinID="FieldHeader"></asp:Label><br/>
                    <asp:Label ID="ShipMessageCount"  runat="server" Text="255" ></asp:Label>
                    <asp:Label ID="ShipMessageHelp"  runat="server" Text=" characters remaining" ></asp:Label>
	                <asp:TextBox ID="ShipMessage" MaxLength="255" runat="server" TextMode="MultiLine" Columns="20" Rows="3"></asp:TextBox>
		        </td>
            </tr>
        </table>
    </ItemTemplate>
</asp:Repeater>
<div class="addGiftOptions">
    <asp:PlaceHolder ID="GiftOptionsPanel" runat="server">
        <h3><asp:Localize ID="GiftCaption" runat="server" Text="IS THIS A GIFT?"></asp:Localize></h3>
        <div class="content">
            <asp:CheckBox ID="ShowGiftOptions" runat="server" />
            <asp:Label ID="ShowGiftOptionsLabel" runat="server" Text="Check here to include a gift message or gift wrap."></asp:Label>
        </div>
    </asp:PlaceHolder>
    <div class="buttons">
        <asp:ImageButton ID="ContinueButton" runat="server" SkinID="Continue" AlternateText="Continue" OnClick="ContinueButton_Click" /><br />
        <asp:Label ID="InvalidShipMethodMessage" runat="server" Visible="false" Text="Invalid shipping method. Either you have not selected a shipping method or the selected method is not valid." SkinID="ErrorCondition" EnableViewState="false"></asp:Label>
    </div>
</div>

