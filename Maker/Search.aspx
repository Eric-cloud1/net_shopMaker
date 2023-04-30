<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Search" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="PageContent">
  <cb:ScriptletPart
    ID="SearchPage"
    runat="server"
    Layout="Left Sidebar"
    Header="Standard Header"
    LeftSidebar="Category Search"
    Content="Search Page"
    Footer="Standard Footer"
    Title="Search Page"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
