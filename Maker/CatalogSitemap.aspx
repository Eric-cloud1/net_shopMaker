<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Catalog Sitemap" %> <%@
Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content
  ID="ScriptletContent"
  ContentPlaceHolderID="PageContent"
  Runat="Server"
>
  <cb:ScriptletPart
    ID="CatalogSitemap"
    runat="server"
    Layout="One Column"
    Header="Standard Header"
    Content="Sitemap Page"
    Footer="Standard Footer"
    Title="Catalog Sitemap"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
