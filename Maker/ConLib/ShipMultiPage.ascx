<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ShipMultiPage.ascx.cs" Inherits="ConLib_ShipMultiPage" EnableViewState="false" %>
<%--
<conlib>
<summary>Display page to select multiple shipping addresses for each of shipment.</summary>
</conlib>
--%>

<%@ Register Src="~/ConLib/CheckoutProgress.ascx" TagName="CheckoutProgress" TagPrefix="uc" %>
<%@ Register Src="~/ConLib/Utility/BasketItemDetail.ascx" TagName="BasketItemDetail" TagPrefix="uc" %>
<ajax:UpdatePanel ID="ShippingAddressAjax" runat="server">
    <ContentTemplate>
        <div class="checkoutPageHeader">
            <uc:CheckoutProgress ID="CheckoutProgress1" runat="server" />
            <h1><asp:Localize ID="Caption" runat="server" Text="Select Shipping Addresses"></asp:Localize></h1>
            <div class="content">
                <asp:Label ID="SingleAddressHelpText" runat="server" Text="Select a shipping address for each item below."></asp:Label>
            </div>
        </div>
        <asp:GridView ID="BasketGrid" runat="server" AutoGenerateColumns="False"
            ShowFooter="false" DataKeyNames="BasketItemId" Width="100%" CellPadding="4" CellSpacing="0"
            RowStyle-CssClass="altodd" AlternatingRowStyle-CssClass="alteven" BorderColor="DarkGray" BorderStyle="Solid">
            <Columns>
                <asp:TemplateField HeaderText="Item">
                    <ItemTemplate>
                        <uc:BasketItemDetail ID="I" runat="server" BasketItem='<%# GetBasketItem(Container.DataItem) %>' ShowAssets="false" ShowShipTo="False" ShowSubscription="False" LinkProducts="false" ForceKitDisplay="true" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Price">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" Width="70px" />
                    <ItemTemplate>
                        <%#TaxHelper.GetInvoicePrice(Token.Instance.User.Basket, GetBasketItem(Container.DataItem), true).ToString("ulc")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" Width="40px" />
                    <HeaderTemplate>
                        <%#GetTaxHeader()%>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%#TaxHelper.GetTaxRate(GetBasketItem(Container.DataItem)).ToString("0.####")%>%
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Ship To">
                    <ItemTemplate>
                        <asp:DropDownList ID="ShippingAddress" runat="server"></asp:DropDownList>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <div style="text-align:right; margin-top:10px; margin-bottom:10px; margin-right:5px;">
            <asp:ImageButton ID="ContinueButton" runat="server" AlternateText="Continue" SkinID="Continue" OnClick="ContinueButton_Click" CausesValidation="false" />
        </div>
    </ContentTemplate>
</ajax:UpdatePanel>
