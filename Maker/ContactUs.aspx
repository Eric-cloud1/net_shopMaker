<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Contact Us" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="PageContent">
  <cb:ScriptletPart
    ID="ContactUs"
    runat="server"
    Layout="Left Sidebar"
    Header="Standard Header"
    LeftSidebar="Our Departments"
    Content="Contact Us"
    Footer="Standard Footer"
    Title="Contact Us"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
