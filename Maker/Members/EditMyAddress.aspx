<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Address Book - Edit Address" %>
<%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="PageContent" Runat="Server">
  <cb:ScriptletPart
    ID="EditMyAddressPage"
    runat="server"
    Layout="Account"
    Header="Standard Header"
    Content="Edit My Address Page"
    Footer="Standard Footer"
    Title="Edit My Address"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
