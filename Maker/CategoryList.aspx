<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="View Category" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %> <%--
<DisplayPage>
  <Name>Category List</Name>
  <NodeType>Category</NodeType>
  <Description
    >Displays products for only the selected category. Items are displayed in a
    row format with columns for SKU, manufacturer, and pricing. Includes
    breadcrumb links, sorting, page navigation, and sub-category link
    options.</Description
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
    ID="CategoryList"
    runat="server"
    Layout="Three Column"
    Header="Standard Header"
    Content="Category List Page"
    LeftSidebar="Standard Sidebar 1"
    RightSidebar="Shopping Bar 1"
    Footer="Standard Footer"
    Title="Category List"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
