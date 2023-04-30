<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="My Account" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="PageContent" Runat="Server">
  <cb:ScriptletPart
    ID="MyAccountPage"
    runat="server"
    Layout="Account"
    Header="Standard Header"
    Content="My Account Page"
    Footer="Standard Footer"
    Title="My Account Page"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
