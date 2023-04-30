<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Password Assistance" %> <%@
Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content
  ID="ScriptletContent"
  ContentPlaceHolderID="PageContent"
  Runat="Server"
>
  <cb:ScriptletPart
    ID="PasswordHelp"
    runat="server"
    Layout="One Column"
    Header="Standard Header"
    Content="Password Help Page"
    Footer="Standard Footer"
    Title="Password Help"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
