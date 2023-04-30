<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Currencies" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %>
<asp:Content
  ID="ScriptletContent"
  ContentPlaceHolderID="PageContent"
  Runat="Server"
>
  <cb:ScriptletPart
    ID="Basket"
    runat="server"
    Layout="One Column"
    Header="Standard Header"
    Content="Currencies Page"
    Footer="Standard Footer"
    Title="Currencies"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
