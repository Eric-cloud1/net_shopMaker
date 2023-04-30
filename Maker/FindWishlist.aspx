<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Find Wishlist" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %>
<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="PageContent">
  <cb:ScriptletPart
    ID="FindWishlistPage"
    runat="server"
    Layout="One Column"
    Content="Find Wishlist Page"
    Header="Standard Header"
    Footer="Standard Footer"
    Title="Find Wishlist Page"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
