<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Store is closed." %> <%@
Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<script runat="server">
   protected void Page_PreInit(object sender, EventArgs e)
     {
         if (Store.GetCachedSettings().StoreFrontClosed == StoreClosureType.Open || (Store.GetCachedSettings().StoreFrontClosed == StoreClosureType.ClosedForNonAdminUsers && Token.Instance.User.IsAdmin))
         {
  		Response.Redirect("Default.aspx");
  	}
  }
</script>
<asp:Content
  ID="ScriptletContent"
  ContentPlaceHolderID="PageContent"
  Runat="Server"
>
  <cb:ScriptletPart
    ID="StoreClosed"
    runat="server"
    Layout="Content Only"
    Header=""
    Content="StoreClosed Page"
    Footer=""
    Title="Store Is Closed"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
