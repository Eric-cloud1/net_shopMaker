<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Checkout - Select Gift Options"
%> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="PageContent" Runat="Server">
  <cb:ScriptletPart
    ID="GiftOptions"
    runat="server"
    Layout="One Column"
    Header="Checkout Header"
    Content="Gift Options Page"
    Footer="Standard Footer"
    Title="Gift Options"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
