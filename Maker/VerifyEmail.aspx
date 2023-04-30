<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Email Verification" %> <%@
Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content
  ID="ScriptletContent"
  ContentPlaceHolderID="PageContent"
  Runat="Server"
>
  <cb:ScriptletPart
    ID="VerifyEmail"
    runat="server"
    Layout="Three Column"
    Content="Verify Email Page"
    LeftSidebar="Standard Sidebar 1"
    RightSidebar="Standard Sidebar 2"
    Header="Standard Header"
    Footer="Standard Footer"
    Title="Verify Email Page"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
