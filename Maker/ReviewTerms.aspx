<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Product Review Terms and
Conditions" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="PageContent">
  <cb:ScriptletPart
    ID="ProductReviewTerms"
    runat="server"
    Layout="One Column"
    Content="Product Review Terms Page"
    Header="Standard Header"
    Footer="Standard Footer"
    Title="Product Review Terms Page"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
