<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Checkout - Select Shipping
Address" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="PageContent">
  <cb:ScriptletPart
    ID="ShipAddressPage"
    runat="server"
    Layout="Checkout"
    Content="Ship Address Page"
    Header="Standard Header"
    Footer="Standard Footer"
    Title="Ship Address Page"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
