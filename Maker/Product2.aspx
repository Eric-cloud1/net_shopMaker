<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="View Product" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %> <%--
<DisplayPage>
  <Name>Basic Product 2</Name>
  <NodeType>Product</NodeType>
  <Description
    >A product display page that shows the basic product details.</Description
  >
</DisplayPage>
--%>
<script runat="server">
  Product _Product = null;

  protected void Page_PreInit(object sender, EventArgs e)
  {
      _Product = ProductDataSource.Load(PageHelper.GetProductId());
      if (_Product != null)
      {
          if ((_Product.Visibility == CatalogVisibility.Private) &&
              (!Token.Instance.User.IsInRole(Role.CatalogAdminRoles)))
          {
              Response.Redirect(NavigationHelper.GetHomeUrl());
          }
          PageHelper.BindCmsTheme(this, _Product);
      }
      else NavigationHelper.Trigger404(Response, "Invalid Product");
  }

  protected void Page_Load(object sender, EventArgs e)
  {
      if (_Product != null)
      {
          if (!Page.IsPostBack)
              //REGISTER THE PAGEVIEW
              MakerShop.Reporting.PageView.RegisterCatalogNode(_Product.ProductId, CatalogNodeType.Product);
          PageHelper.BindMetaTags(this, _Product);
          Page.Title = _Product.Name;
      }
  }
</script>
<asp:Content ID="MainContent" ContentPlaceHolderID="PageContent" Runat="Server">
  <cb:ScriptletPart
    ID="ShowProduct"
    runat="server"
    Layout="Three Column"
    Header="Standard Header"
    LeftSidebar="Shopping Bar 1"
    Content="Show Product 1"
    RightSidebar="Product Bar 1"
    Footer="Standard Footer"
    Title="Show Product"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
