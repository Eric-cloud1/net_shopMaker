<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OrderTotalSummary.ascx.cs" Inherits="ConLib_OrderTotalSummary" %>
<%--
<conlib>
<summary>Display summary of order totals.</summary>
<param name="ShowTitle" default="true">Indicates whether the title bar should be shown for the dialog.</param>
<param name="ShowTaxBreakdown" default="true">Indicates whether taxes should be broken down by tax name, or shown as a lump sum.</param>
</conlib>
--%>
<div class="summarySection">
    <asp:Panel ID="TitlePanel" runat="server" CssClass="summarySectionHeader">
        <h3><asp:Localize ID="Caption" runat="server" Text="Order Summary"></asp:Localize></h3>
    </asp:Panel>
    <div class="summarySectionContent">
        <table cellpadding="0" cellspacing="0" class="orderSummary">
            <tr>
                <th><asp:Label ID="SubtotalLabel" runat="server" Text="Item Subtotal:"></asp:Label></th>
                <td><asp:Label ID="Subtotal" runat="server"></asp:Label></td>
            </tr>
            <tr id="trGiftWrap" runat="server">
                <th><asp:Label ID="GiftWrapLabel" runat="server" Text="Gift Wrap:"></asp:Label></th>
                <td><asp:Label ID="GiftWrap" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <th><asp:Label ID="ShippingLabel" runat="server" Text="Shipping:"></asp:Label></th>
                <td><asp:Label ID="Shipping" runat="server"></asp:Label></td>
            </tr>
            <asp:PlaceHolder ID="phTaxes" runat="server"></asp:PlaceHolder>
            <tr id="trCoupon" runat="server">
                <th><asp:Label ID="CouponsLabel" runat="server" Text="Coupons:"></asp:Label></th>
                <td><asp:Label ID="Coupons" runat="server"></asp:Label></td>
            </tr>
            <tr id="trAdjustments" runat="server">
                <th><asp:Label ID="AdjustmentsLabel" runat="server" Text="Adjustments:"></asp:Label></th>
                <td><asp:Label ID="Adjustments" runat="server"></asp:Label></td>
            </tr>
            <tr class="totalDivider"><td colspan="2"><hr /></td></tr>
            <tr>
                <th style="font-weight: bold;"><asp:Label ID="TotalLabel" runat="server" Text="Total: "></asp:Label></th>
                <td style="font-weight:bold;"><asp:Label ID="Total" runat="server"></asp:Label></td>
            </tr>
        </table>
    </div>
</div>