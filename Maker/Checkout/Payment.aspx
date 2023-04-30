<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Checkout - Confirm &amp; Pay"
%> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="PageContent">
  <cb:ScriptletPart
    ID="PaymentPage"
    runat="server"
    Layout="Checkout"
    Content="Payment Page"
    Header="Standard Header"
    Footer="Standard Footer"
    Title="Payment Page"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
