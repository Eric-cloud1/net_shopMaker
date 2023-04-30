<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Order Receipt" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %>
<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="PageContent">
  <cb:ScriptletPart
    ID="ReceiptPage"
    runat="server"
    Layout="View Order"
    Header="Standard Header"
    Content="Receipt Page"
    RightSidebar="View Order Sidebar"
    Footer="Standard Footer"
    Title="Receipt Page"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
