<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="View Readme" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %>
<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="PageContent">
  <cb:ScriptletPart
    ID="ReadmePage"
    runat="server"
    Layout="Content Only"
    Content="Readme Page"
    Title="Readme Page"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
