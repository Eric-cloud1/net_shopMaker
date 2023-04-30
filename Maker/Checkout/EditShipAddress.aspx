<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Checkout - Edit Shipping
Address" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="PageContent">
  <cb:ScriptletPart
    ID="EditShipAddressPage"
    runat="server"
    Layout="Checkout"
    Content="Edit Ship Address Page"
    Header="Standard Header"
    Footer="Standard Footer"
    Title="Edit Ship Address Page"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
