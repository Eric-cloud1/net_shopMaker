<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Checkout - Multiple Shipping
Addresses" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="PageContent">
  <cb:ScriptletPart
    ID="ShipMultiPage"
    runat="server"
    Layout="Right Sidebar"
    Content="Ship Multi Page"
    RightSidebar="Address Book Sidebar"
    Header="Standard Header"
    Footer="Standard Footer"
    Title="Ship Multi Page"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
