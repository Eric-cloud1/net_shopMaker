<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="My Product Reviews" %> <%@
Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="PageContent" Runat="Server">
  <cb:ScriptletPart
    ID="MyProductReviews"
    runat="server"
    Layout="Account"
    Header="Standard Header"
    Content="My Product Reviews Page"
    Footer="Standard Footer"
    Title="My Product Reviews"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
