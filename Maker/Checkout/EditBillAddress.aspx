<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Checkout - Edit Billing
Address" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="PageContent">
  <cb:ScriptletPart
    ID="EditBillAddressPage"
    runat="server"
    Layout="Checkout"
    Content="Edit Bill Address Page"
    Header="Standard Header"
    Footer="Standard Footer"
    Title="Edit Bill Address Page"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
