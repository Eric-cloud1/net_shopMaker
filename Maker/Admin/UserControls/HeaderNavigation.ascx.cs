using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Common;
using MakerShop.Users;

public partial class Admin_UserControls_HeaderNavigation : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (Token.Instance.User == null || Token.Instance.User.IsAdmin == false)
        {
            DashboardLink.Visible = false;
            OrdersLink.Visible = false;
            CatalogLink.Visible = false;
            StoreLink.Visible = false;
        }
        else
        {
            OrdersLink.Visible = (Token.Instance.User.IsInRole(Role.OrderAdminRoles));
            CatalogLink.Visible = (Token.Instance.User.IsInRole(Role.CatalogAdminRoles));
        }

        LogoutLink.Visible = true;

        if (Request.Url.AbsoluteUri.Contains("Login.aspx"))
            LogoutLink.Visible = false;

    }
}
