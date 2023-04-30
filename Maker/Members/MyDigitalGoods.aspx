<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="My Digital Goods" %> <%@
Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="PageContent" Runat="Server">
  <cb:ScriptletPart
    ID="MyDigitalGoods"
    runat="server"
    Layout="Account"
    Header="Standard Header"
    Content="My Digital Goods Page"
    Footer="Standard Footer"
    Title="My Digital Goods"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
