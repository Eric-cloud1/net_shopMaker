<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Checkout" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %>
<asp:Content
  ID="ScriptletContent"
  ContentPlaceHolderID="PageContent"
  Runat="Server"
>
  <cb:ScriptletPart
    ID="CheckoutPage"
    runat="server"
    Layout="One Column"
    Header="Standard Header"
    Content="Checkout Page"
    Footer="Standard Footer"
    Title="Checkout Page"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
