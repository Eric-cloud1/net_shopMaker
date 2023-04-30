<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Site Disclaimer" %> <%@
Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content
  ID="ScriptletContent"
  ContentPlaceHolderID="PageContent"
  Runat="Server"
>
  <cb:ScriptletPart
    ID="Disclaimer"
    runat="server"
    Layout="One Column"
    Header="Standard Header"
    Content="Disclaimer"
    Footer="Standard Footer"
    Title="Site Disclaimer"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
