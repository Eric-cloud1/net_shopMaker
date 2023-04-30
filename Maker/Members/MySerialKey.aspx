<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="View Serial Key" %> <%@
Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="PageContent" Runat="Server">
  <cb:ScriptletPart
    ID="MySerialKey"
    runat="server"
    Layout="Account"
    Header="Standard Header"
    Content="My Serial Key Page"
    Footer="Standard Footer"
    Title="My Serial Key"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
