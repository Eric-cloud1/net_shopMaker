<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="View Tracking Info" %> <%@
Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="PageContent" Runat="Server">
  <cb:ScriptletPart
    ID="MyTrackingInfo"
    runat="server"
    Layout="Account"
    Header="Standard Header"
    Content="My Tracking Info Page"
    Footer="Standard Footer"
    Title="My Tracking Info"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
