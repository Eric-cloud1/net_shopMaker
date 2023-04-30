<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="View Wishlist" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="PageContent" Runat="Server">
  <cb:ScriptletPart
    ID="MyWishlist"
    runat="server"
    Layout="Right Sidebar"
    Header="Standard Header"
    Content="Wishlist Page"
    RightSidebar="My Wishlist Sidebar"
    Footer="Standard Footer"
    Title="My Wishlist"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
