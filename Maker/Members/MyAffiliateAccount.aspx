<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="My Affiliate Account" %> <%@
Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="PageContent" Runat="Server">
  <cb:ScriptletPart
    ID="MyAffiliateReportPage"
    runat="server"
    Layout="One Column"
    Header="Standard Header"
    Content="My Affiliate Account Page"
    Footer="Standard Footer"
    Title="My Affiliate Account"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
