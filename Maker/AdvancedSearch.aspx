<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Advanced Search" %> <%@
Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content
  ID="ScriptletContent"
  runat="server"
  ContentPlaceHolderID="PageContent"
>
  <cb:ScriptletPart
    ID="AdvancedSearchPage"
    runat="server"
    Layout="One Column"
    Header="Standard Header"
    Content="Advanced Search Page"
    Footer="Standard Footer"
    Title="Advanced Search"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
