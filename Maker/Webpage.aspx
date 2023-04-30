<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="View Webpage" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %> <%--
<DisplayPage>
  <Name>Basic Webpage</Name>
  <NodeType>Webpage</NodeType>
  <Description
    >A basic display handler for webpages. It shows the webpage content with
    standard headers and footers.</Description
  >
</DisplayPage>
--%>
<script runat="server">
  Webpage _Webpage = null;

  protected void Page_PreInit(object sender, EventArgs e)
  {
      _Webpage = WebpageDataSource.Load(PageHelper.GetWebpageId());
      if (_Webpage != null)
      {
          if ((_Webpage.Visibility == CatalogVisibility.Private) &&
              (!Token.Instance.User.IsInRole(Role.CatalogAdminRoles)))
          {
              Response.Redirect(NavigationHelper.GetHomeUrl());
          }
          PageHelper.BindCmsTheme(this, _Webpage);
      }
      else NavigationHelper.Trigger404(Response, "Invalid Webpage");
  }

  protected void Page_Load(object sender, EventArgs e)
  {
      if (_Webpage != null)
      {
          if (!Page.IsPostBack)
              //REGISTER THE PAGEVIEW
              MakerShop.Reporting.PageView.RegisterCatalogNode(_Webpage.WebpageId, CatalogNodeType.Webpage);
          PageHelper.BindMetaTags(this, _Webpage);
          Page.Title = _Webpage.Name;
      }
  }
</script>
<asp:Content ID="MainContent" ContentPlaceHolderID="PageContent" Runat="Server">
  <cb:ScriptletPart
    ID="WebpagePage"
    runat="server"
    Layout="Left Sidebar"
    Header="Standard Header"
    Content="Webpage Page"
    LeftSidebar="Standard Sidebar 1"
    Footer="Standard Footer"
    Title="View Webpage"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
