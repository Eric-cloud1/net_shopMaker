<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="View Wishlist" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="PageContent">
  <cb:ScriptletPart
    ID="ViewWishlistPage"
    runat="server"
    Layout="Three Column"
    Header="Standard Header"
    LeftSidebar="Wishlist Sidebar"
    RightSidebar="Shopping Bar 1"
    Content="View Wishlist Page"
    Footer="Standard Footer"
    Title="View Wishlist Page"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
