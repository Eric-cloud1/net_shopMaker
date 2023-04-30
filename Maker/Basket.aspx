<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="My Cart" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %>
<asp:Content
  ID="ScriptletContent"
  ContentPlaceHolderID="PageContent"
  Runat="Server"
>
  <cb:ScriptletPart
    ID="Basket"
    runat="server"
    Layout="Right Sidebar"
    Header="Standard Header"
    Content="Basket"
    RightSidebar="Basket Bar 1"
    Footer="Standard Footer"
    Title="My Cart"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
