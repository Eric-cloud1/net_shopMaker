<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="My Subscriptions" %> <%@
Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="PageContent" Runat="Server">
  <cb:ScriptletPart
    ID="MySubscriptions"
    runat="server"
    Layout="Account"
    Header="Standard Header"
    Content="My Subscriptions Page"
    Footer="Standard Footer"
    Title="My Subscriptions"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
