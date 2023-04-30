<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Create Profile" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %>
<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="PageContent">
  <cb:ScriptletPart
    ID="CreateProfilePage"
    runat="server"
    Layout="Checkout"
    Content="Create Profile Page"
    Header="Standard Header"
    Footer="Standard Footer"
    Title="Create Profile Page"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
