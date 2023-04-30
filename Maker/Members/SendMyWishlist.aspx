<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Send Wishlist" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="PageContent" Runat="Server">
  <cb:ScriptletPart
    ID="SendMyWishlist"
    runat="server"
    Layout="Right Sidebar"
    Header="Standard Header"
    Content="Send Wishlist Page"
    RightSidebar="My Wishlist Sidebar"
    Footer="Standard Footer"
    Title="Send My Wishlist"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
