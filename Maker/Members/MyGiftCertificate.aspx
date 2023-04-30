<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Print Gift Certificate" %> <%@
Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="PageContent" Runat="Server">
  <cb:ScriptletPart
    ID="MyGiftCertificate"
    runat="server"
    Layout="Account"
    Header="Standard Header"
    Content="My Gift Certificate Page"
    Footer="Standard Footer"
    Title="My Gift Certificate"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
