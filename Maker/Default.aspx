<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Home Page" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %>
<asp:Content runat="server" ContentPlaceHolderID="PageContent">
  <cb:ScriptletPart
    ID="HomePage"
    runat="server"
    Layout="Three Column"
    Content="Home Page"
    LeftSidebar="Standard Sidebar 1"
    RightSidebar="Standard Sidebar 2"
    Header="Standard Header"
    Footer="Standard Footer"
    Title="Home Page"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
