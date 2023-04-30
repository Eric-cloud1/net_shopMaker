<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BasketTotalSummary.ascx.cs" Inherits="ConLib_BasketTotalSummary" EnableViewState="false" %>
<%--
<conlib>
<summary>Display totals and summary of contents of basket.</summary>
<param name="ShowTaxes" default="true">Indicates whether taxes should be shown in summary.</param>
<param name="ShowTaxBreakdown" default="true">Indicates whether taxes should be broken down by tax name, or shown as a lump sum.</param>
</conlib>
--%>
<div class="section">
    <div class="header">
        <h2><asp:Localize ID="Caption" runat="server" Text="Order Summary"></asp:Localize></h2>
    </div>
    <div class="onePageCheckoutCell">
        <table cellpadding="0" cellspacing="0" class="orderSummary">
            <tr>
                <th><asp:Label ID="SubtotalLabel" runat="server" Text="Subtotal: "></asp:Label></th>
                <td><asp:Label ID="Subtotal" runat="server"></asp:Label></td>
            </tr>
            <tr id="trGiftWrap" runat="server">
                <th><asp:Label ID="GiftWrapLabel" runat="server" Text="Gift Wrap: "></asp:Label></th>
                <td><asp:Label ID="GiftWrap" runat="server"></asp:Label></td>
            </tr>
            <tr id="trShipping" runat="server">
                <th><asp:Label ID="ShippingLabel" runat="server" Text="Shipping: "></asp:Label></th>
                <td><asp:Label ID="Shipping" runat="server"></asp:Label></td>
            </tr>
            <asp:PlaceHolder ID="phTaxes" runat="server"></asp:PlaceHolder>
            <tr id="trCoupon" runat="server">
                <th><asp:Label ID="CouponsLabel" runat="server" Text="Coupons: "></asp:Label></th>
                <td><asp:Label ID="Coupons" runat="server"></asp:Label></td>
            </tr>
            <tr class="totalDivider"><td colspan="2"><hr /></td></tr>
            <tr>
                <th style="font-weight:bold;"><asp:Label ID="TotalLabel" runat="server" Text="Total: "></asp:Label></th>
                <td style="font-weight:bold;"><asp:Label ID="Total" runat="server"></asp:Label></td>
            </tr>
            <tr class="totalDivider"><td colspan="2"><hr /></td></tr>
            <tr><td colspan="2" style="text-align:center; width:100%;"><asp:HyperLink ID="EditOrderButton" runat="server" SkinID="button" Text="Edit Order" NavigateUrl="~/Basket.aspx" ></asp:HyperLink></td></tr>
        </table>
    </div>
</div>