<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BuyProductDialog.ascx.cs" Inherits="ConLib_BuyProductDialog" %>
<%--
<conlib>
<summary>Displays product details and buy now button to add it to cart.</summary>
<param name="ShowSku" default="true">Possible values are true or false.  Indicates whether the SKU will be shown or not.</param>
<param name="ShowPrice" default="true">Possible values are true or false.  Indicates whether the price details will be shown or not.</param>
<param name="ShowSubscription" default="true">Possible values are true or false.  Indicates whether the subscription details will be shown or not.</param>
<param name="ShowMSRP" default="true">Possible values are true or false.  Indicates whether the MSPR will be shown or not.</param>
<param name="ShowPartNumber" default="false">Possible values are true or false.  Indicates whether the Part/Model Number will be shown or not.</param>
</conlib>
--%>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Src="~/ConLib/Utility/ProductPrice.ascx" TagName="ProductPrice" TagPrefix="uc" %>
<ajax:UpdatePanel ID="BuyProductPanel" runat="server" UpdateMode="Always">
    <ContentTemplate>
        <table class="buyProductForm" cellpadding="0" cellspacing="0">
            <tr id="trSku" runat="server" enableviewstate="false">
                <th class="rowHeader">
                    <asp:Localize ID="SkuLocalize" runat="server" Text="Item #:" EnableViewState="false"></asp:Localize>
                </th>
                <td>
                    <asp:Literal ID="Sku" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr id="trPartNumber" runat="server" enableviewstate="false">
                <th class="rowHeader">
                    <asp:Localize ID="PartNumberLocalize" runat="server" Text="Part #:" EnableViewState="false"></asp:Localize>
                </th>
                <td>
                    <asp:Literal ID="PartNumber" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr id="trRegPrice" runat="server" enableviewstate="false">
                <th class="rowHeader">
                    <asp:Localize ID="RegPriceLocalize" runat="server" Text="Reg. Price:" EnableViewState="false"></asp:Localize> 
                </th>
                <td>
                    <asp:Label ID="RegPrice" runat="server" SkinID="MSRP" EnableViewState="false"></asp:Label>
                </td>
            </tr>
            <tr id="trOurPrice" runat="server" EnableViewState="false">
                <th class="rowHeader" valign="top">
                    <asp:Localize ID="OurPriceLocalize" runat="server" Text="Our Price:" EnableViewState="false"></asp:Localize>        
                </th>
                <td>
                    <uc:ProductPrice ID="OurPrice" runat="server" EnableDefaultKitProducts="false" />                    
                </td>
            </tr>
            <tr id="trVariablePrice" runat="server" enableviewstate="false">
                <th class="rowHeader">
                    <asp:Localize ID="VariablePriceLabel" runat="server" Text="Enter Price:" EnableViewState="false"></asp:Localize>
                </th>
                <td>
                    <asp:TextBox ID="VariablePrice" runat="server" MaxLength="8" Width="60px" ValidationGroup="AddToBasket"></asp:TextBox>
                    <asp:PlaceHolder ID="phVariablePrice" runat="server"></asp:PlaceHolder>
                </td>
            </tr>
            <tr id="rowSubscription" runat="server" enableviewstate="false">
                <td>&nbsp;</td>
                <td>
                    <asp:Literal ID="RecurringPaymentMessage" runat="server"></asp:Literal>
                </td>
            </tr>
			<asp:PlaceHolder runat="server" id="phOptions" EnableViewState="false"></asp:PlaceHolder>
			<asp:PlaceHolder ID="phAddToBasketWarningOpt" runat="server" EnableViewState="false" Visible="false">
			<tr>
				<td>&nbsp;</td>
                <td colspan="8">          
                    <asp:Label ID="AddToBasketWarningOpt" runat="server" EnableViewState="false"  SkinID="ErrorCondition" Text="Please make your selections above."></asp:Label>
                </td>
            </tr>
			</asp:PlaceHolder>
			<asp:PlaceHolder runat="server" id="phKitOptions" EnableViewState="false"></asp:PlaceHolder>
			<asp:PlaceHolder ID="phAddToBasketWarningKit" runat="server" EnableViewState="false" Visible="false">
			<tr>
				<td>&nbsp;</td>
                <td colspan="8">          
                    <asp:Label ID="AddToBasketWarningKit" runat="server" EnableViewState="false"  SkinID="ErrorCondition" Text="Please make your selections above."></asp:Label>
                </td>
            </tr>
			</asp:PlaceHolder>
            <tr id="rowQuantity" runat="server" enableviewstate="false">
                <th class="rowHeader">
                    <asp:Localize ID="QuantityLocalize" runat="server" Text="Quantity:"></asp:Localize>        
                </th>
                <td nowrap>
                    <cb:updowncontrol Width="30" id="Quantity" runat="server" DownImageUrl="~/images/down.gif" UpImageUrl="~/images/up.gif" Columns="2" MaxLength="5" Text='1' MaxValue="32767" onFocus="this.select()"></cb:updowncontrol><asp:CustomValidator ID="QuantityValidaor" runat="server" ValidationGroup="AddToBasket" ErrorMessage="Quantity can not exceed the available stock of {0}." ControlToValidate="Quantity">*</asp:CustomValidator>
                    <asp:PlaceHolder ID="QuantityLimitsPanel" runat="server" EnableViewState="false"></asp:PlaceHolder>
                    </td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <asp:PlaceHolder ID="InventoryDetailsPanel" runat="server" EnableViewState="false"></asp:PlaceHolder>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="AddToBasket" />					
                    <asp:LinkButton ID="AddToWishlistButton" runat="server" SkinID="Button" Visible="true" OnClick="AddToWishlistButton_Click" Text="Add to Wishlist" EnableViewState="false" ValidationGroup="AddToBasket"></asp:LinkButton>
                    <asp:LinkButton ID="AddToBasketButton" runat="server" SkinID="Button" Visible="true" OnClick="AddToBasketButton_Click" Text="Add to Cart" EnableViewState="false" ValidationGroup="AddToBasket"></asp:LinkButton>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</ajax:UpdatePanel>
