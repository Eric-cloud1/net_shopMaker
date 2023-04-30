<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductPrice.ascx.cs" Inherits="Admin_UserControls_ProductPrice" EnableViewState="false" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:PlaceHolder ID="phFixedPrice" runat="server">
<asp:Literal ID="RetailPrice1" runat="server"></asp:Literal>
<asp:Literal ID="Price" runat="server"></asp:Literal>
</asp:PlaceHolder>
<asp:PlaceHolder ID="phPricePopup" runat="server">
<asp:LinkButton ID="ShowPriceLink" runat="server" Text="Click to see price"></asp:LinkButton>
<asp:Panel ID="PricePopup" runat="server" Style="display:none" CssClass="pricePopup">
    <asp:Panel ID="PricePopupHeader" runat="server" CssClass="pricePopupHeader">
        <b><asp:Literal ID="ProductName" runat="server"></asp:Literal></b>
    </asp:Panel>
    <div style="padding: 8px 4px;">
        <asp:PlaceHolder ID="trRetailPrice" runat="server">
            <span class="fieldHeader"><asp:Literal ID="HiddenRetailPriceLabel" runat="server" Text=""></asp:Literal></span>
            <span class="fieldValue"><asp:Literal ID="HiddenRetailPrice" runat="server"></asp:Literal></span><br />
        </asp:PlaceHolder>
        <span class="fieldHeader"><asp:Literal ID="HiddenPriceLabel" runat="server" Text=""></asp:Literal></span>
        <span class="fieldValue"><asp:Literal ID="HiddenPrice" runat="server"></asp:Literal></span>
        <asp:PlaceHolder ID="trSpecialPrice" runat="server" Visible="false">
            <br /><span class="fieldHeader"><asp:Literal ID="SpecialPriceLabel" runat="server" Text=""></asp:Literal></span>
            <span class="fieldValue"><asp:Literal ID="SpecialPrice" runat="server"></asp:Literal></span>
        </asp:PlaceHolder>        
        <asp:PlaceHolder ID="trAmountSaved" runat="server">
            <br /><span class="fieldHeader"><asp:Literal ID="AmountSavedLabel" runat="server" Text=""></asp:Literal></span>
            <span class="fieldValue"><asp:Literal ID="AmountSaved" runat="server"></asp:Literal></span>
        </asp:PlaceHolder><br /><br />
        <div style="text-align:center">            
            <asp:LinkButton ID="ClosePopUpLink" runat="server" Text="Close" SkinID="Button"></asp:LinkButton>
        </div>
    </div>
</asp:Panel>
</asp:PlaceHolder>
<asp:HiddenField ID="VS" runat="server" EnableViewState="false" />