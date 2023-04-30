<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="View Agreement" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %>
<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="PageContent">
  <cb:ScriptletPart
    ID="AgreementPage"
    runat="server"
    Layout="Content Only"
    Content="Agreement Page"
    Title="Agreement Page"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
