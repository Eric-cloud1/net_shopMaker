<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="My Address Book" %> <%@
Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="PageContent" Runat="Server">
  <cb:ScriptletPart
    ID="MyAddressBookPage"
    runat="server"
    Layout="Account"
    Header="Standard Header"
    Content="My Address Book Page"
    Footer="Standard Footer"
    Title="My Address Book"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
