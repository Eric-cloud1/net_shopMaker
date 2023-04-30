<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CheckoutProgress.ascx.cs" Inherits="ConLib_CheckoutProgress" EnableViewState="false" %>
<%--
<conlib>
<summary>Displays the 4 checkout progress steps dynamically.</summary>
</conlib>
--%>
<div class="checkoutProgress">
    <span ID="Step1" runat="server" class="off"><span><asp:Localize ID="Step1Text" runat="server" Text="Checkout"></asp:Localize></span></span>
    <span ID="Step2" runat="server" class="off"><span><asp:Localize ID="Step2Text" runat="server" Text="Ship"></asp:Localize></span></span>
    <span ID="Step3" runat="server" class="off"><span><asp:Localize ID="Step3Text" runat="server" Text="Gift"></asp:Localize></span></span>
    <span ID="Step4" runat="server" class="off"><span><asp:Localize ID="Step4Text" runat="server" Text="Pay"></asp:Localize></span></span>
</div>