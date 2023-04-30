<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Products Accessories"%> <%@
Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content runat="server" ContentPlaceHolderID="PageContent">
  <cb:ScriptletPart
    ID="UpsellPage"
    runat="server"
    Layout="Right Sidebar"
    Content="ProductAccessories Page"
    RightSidebar="Product Bar 1"
    Header="Standard Header"
    Footer="Standard Footer"
    Title="Product Accessories"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
