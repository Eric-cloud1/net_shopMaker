<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="License Agreement" %> <%@
Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content
  ID="ScriptletContent"
  ContentPlaceHolderID="PageContent"
  Runat="Server"
>
  <cb:ScriptletPart
    ID="Basket"
    runat="server"
    Layout="Checkout"
    Header="Standard Header"
    Content="Buy With Agreement Page"
    Footer="Standard Footer"
    Title="License Agreement"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
