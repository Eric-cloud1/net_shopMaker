<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BuyProductDialogOptionsList.ascx.cs" Inherits="ConLib_BuyProductDialogOptionsList" %>
<%--
<conlib>
<summary>Displays product details and buy now button to add it to cart.  This control should not be used with products having more than 8 options.</summary>
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
        <asp:Label ID="SavedMessage" runat="server" Text="Data saved at {0:g}" EnableViewState="false" Visible="false" SkinID="GoodCondition"></asp:Label>
        <cb:SortedGridView ID="VariantGrid" runat="server" AutoGenerateColumns="False" Width="100%" SkinID="PagedList" DataKeyNames="OptionList" OnRowCommand="VariantGrid_RowCommand">
            <Columns>				
            </Columns>
        </cb:SortedGridView>
</ContentTemplate>
</ajax:UpdatePanel>

