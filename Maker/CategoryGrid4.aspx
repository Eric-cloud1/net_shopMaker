<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="View Category" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %> <%--
<DisplayPage>
  <Name>Category Grid (Shallow Item Display) With Category Data</Name>
  <NodeType>Category</NodeType>
  <Description
    >Displays categories, products, webpages, and links for only the selected
    category. Items are displayed in a grid format with breadcrumb links,
    sorting, and page navigation.</Description
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
    ID="CategoryGrid4"
    runat="server"
    Layout="Right Sidebar"
    Header="Standard Header"
    Content="Category Grid Page With Category Data"
    RightSidebar="Shopping Bar 1"
    Footer="Standard Footer"
    Title="Category Grid"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
