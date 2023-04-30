<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Login or Register" %> <%@
Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<script runat="server">
  protected void Page_Init(object sender, EventArgs e)
  {
      string returnUrl = NavigationHelper.GetReturnUrl(string.Empty);
      if (!string.IsNullOrEmpty(returnUrl))
      {
          if (returnUrl.ToLowerInvariant().Contains("/admin/"))
          {

              Response.Redirect(NavigationHelper.GetAdminUrl("Login.aspx?ReturnUrl=" + Server.UrlEncode(returnUrl)));
          }
      }
  }
</script>
<asp:Content
  ID="ScriptletContent"
  ContentPlaceHolderID="PageContent"
  Runat="Server"
>
  <cb:ScriptletPart
    ID="Login"
    runat="server"
    Layout="One Column"
    Header="Standard Header"
    Content="Login Page"
    Footer="Standard Footer"
    Title="Login or Register"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
