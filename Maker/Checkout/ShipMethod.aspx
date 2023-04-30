<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Checkout - Select Ship Method"
%> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="PageContent">
  <cb:ScriptletPart
    ID="ShipMethodPage"
    runat="server"
    Layout="Checkout"
    Content="Ship Method Page"
    Header="Standard Header"
    Footer="Standard Footer"
    Title="Ship Method Page"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
