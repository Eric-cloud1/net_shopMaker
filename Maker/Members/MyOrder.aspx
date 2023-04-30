<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="My Order" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="PageContent" Runat="Server">
  <cb:ScriptletPart
    ID="MyOrder"
    runat="server"
    Layout="View Order"
    Header="Standard Header"
    Content="My Order Page"
    RightSidebar="View Order Sidebar"
    Footer="Standard Footer"
    Title="My Order"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
