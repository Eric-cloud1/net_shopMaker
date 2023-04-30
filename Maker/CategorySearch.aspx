<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="Category Search" %> <%@
Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %> <%--
<DisplayPage>
  <Name>Category Search</Name>
  <NodeType>Category</NodeType>
  <Description
    >Displays a search panel to narrow and expand results based on category,
    manufacturer, and keyword. This can only be used with certain content
    scriptlets (Search Page, Category Grid, and Category Grid Page with
    Basket).</Description
  >
</DisplayPage>
--%>
<script runat="server">
  Category _Category = null;

  protected void Page_PreInit(object sender, EventArgs e)
  {
      _Category = CategoryDataSource.Load(PageHelper.GetCategoryId());
      if (_Category != null)
      {
          if ((_Category.Visibility == CatalogVisibility.Private) &&
              (!Token.Instance.User.IsInRole(Role.CatalogAdminRoles)))
          {
              Response.Redirect(NavigationHelper.GetHomeUrl());
          }
          PageHelper.BindCmsTheme(this, _Category);
      }
      else NavigationHelper.Trigger404(Response, "Invalid Category");
  }

  protected void Page_Load(object sender, EventArgs e)
  {
      if (_Category != null) PageHelper.BindMetaTags(this, _Category);
  }
</script>
<asp:Content ID="MainContent" ContentPlaceHolderID="PageContent" Runat="Server">
  <cb:ScriptletPart
    ID="CategorySearch"
    runat="server"
    Layout="Left Sidebar"
    Header="Standard Header"
    LeftSidebar="Category Search"
    Content="Category Grid Page"
    Footer="Standard Footer"
    Title="Category Search"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
