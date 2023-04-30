<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
Inherits="MakerShop.Web.UI.MakerShopPage" Title="View Link" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts"
TagPrefix="cb" %> <%--
<DisplayPage>
  <Name>Basic Link</Name>
  <NodeType>Link</NodeType>
  <Description
    >A basic display handler for links. It shows the description with a link to
    the specified URL.</Description
  >
</DisplayPage>
--%>
<script runat="server">
  Link _Link = null;

  protected void Page_PreInit(object sender, EventArgs e)
  {
      _Link = LinkDataSource.Load(PageHelper.GetLinkId());
      if (_Link != null)
      {
          if ((_Link.Visibility == CatalogVisibility.Private) &&
              (!Token.Instance.User.IsInRole(Role.CatalogAdminRoles)))
          {
              Response.Redirect(NavigationHelper.GetHomeUrl());
          }
          PageHelper.BindCmsTheme(this, _Link);
      }
      else NavigationHelper.Trigger404(Response, "Invalid Link");
  }

  protected void Page_Load(object sender, EventArgs e)
  {
      if (_Link != null)
      {
          if (!Page.IsPostBack)
              //REGISTER THE PAGEVIEW
              MakerShop.Reporting.PageView.RegisterCatalogNode(_Link.LinkId, CatalogNodeType.Link);
          PageHelper.BindMetaTags(this, _Link);
          Page.Title = _Link.Name;
      }
  }
</script>
<asp:Content ID="MainContent" ContentPlaceHolderID="PageContent" Runat="Server">
  <cb:ScriptletPart
    ID="LinkPage"
    runat="server"
    Layout="Left Sidebar"
    Header="Standard Header"
    Content="Link Page"
    LeftSidebar="Standard Sidebar 1"
    Footer="Standard Footer"
    Title="View Link"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
