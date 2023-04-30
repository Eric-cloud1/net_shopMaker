<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Edit My Review" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="PageContent" Runat="Server">
  <cb:ScriptletPart
    ID="EditMyReviewPage"
    runat="server"
    Layout="Account"
    Header="Standard Header"
    Content="Edit My Review Page"
    Footer="Standard Footer"
    Title="Edit My Review Page"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
