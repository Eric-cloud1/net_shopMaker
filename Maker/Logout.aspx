<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Logout" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %>
<asp:Content
  ID="ScriptletContent"
  ContentPlaceHolderID="PageContent"
  Runat="Server"
>
  <cb:ScriptletPart
    ID="Logout"
    runat="server"
    Layout="Three Column"
    Content="Logout Page"
    LeftSidebar="Standard Sidebar 1"
    RightSidebar="Standard Sidebar 2"
    Header="Standard Header"
    Footer="Standard Footer"
    Title="Logout Page"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
