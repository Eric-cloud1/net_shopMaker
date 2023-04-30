<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="My Order History" %> <%@
Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="PageContent" Runat="Server">
  <cb:ScriptletPart
    ID="MyOrderHistory"
    runat="server"
    Layout="Account"
    Header="Standard Header"
    Content="My Order History Page"
    Footer="Standard Footer"
    Title="My Order History"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
