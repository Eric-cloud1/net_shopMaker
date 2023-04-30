<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="My Credentials" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="PageContent" Runat="Server">
  <cb:ScriptletPart
    ID="MyCredentialsPage"
    runat="server"
    Layout="Account"
    Header="Standard Header"
    Content="My Credentials Page"
    Footer="Standard Footer"
    Title="My Credentials Page"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
