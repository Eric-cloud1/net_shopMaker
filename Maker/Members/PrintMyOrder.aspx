<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Print My Order" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="PageContent" Runat="Server">
  <cb:ScriptletPart
    ID="PrintMyOrder"
    runat="server"
    Layout="Account"
    Header="Standard Header"
    Content="Print My Order Page"
    Footer="Standard Footer"
    Title="Print My Order"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
