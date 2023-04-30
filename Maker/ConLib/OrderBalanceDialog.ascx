<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OrderBalanceDialog.ascx.cs" Inherits="ConLib_OrderBalanceDialog" EnableViewState="false" %>
<%--
<conlib>
<summary>Displays a link to make a payment when a balance is due for an order.</summary>
<param name="Caption" default="Balance Due">Possible value can be any string.  Title of the control.</param>
</conlib>
--%>
<div class="section">
    <div class="header">
        <h2><asp:Localize ID="phCaption" runat="server" Text="Balance Due"></asp:Localize></h2>
    </div>
    <div class="Cell">
        <asp:PlaceHolder ID="PayOrderPanel" runat="server">
            <p align="justify"><asp:Label ID="phInstructionText" runat="server" Text="Order #{0} has a balance due.  Click the button below to make a payment."></asp:Label></p><br />
            <p align="center"><asp:HyperLink ID="PayLink" runat="server" Text="Pay Now" NavigateUrl="~/Members/PayMyOrder.aspx" SkinID="Button" /></p>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="ContactUsPanel" runat="server" Visible="false">
            <p align="justify"><asp:Label ID="phInstructionText2" runat="server" Text="Order #{0} has a balance due but it includes one or more subscription payments.  Please contact us in order to complete your order."></asp:Label></p>
        </asp:PlaceHolder>
        <br />
    </div>
</div>
