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

public partial class ConLib_LogoutWidget : System.Web.UI.UserControl
{
    private bool _Redirect;
    public bool Redirect
    {
        get
        {
            return _Redirect;
        }
        set
        {
            _Redirect = value;
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        MakerShop.Users.User.Logout();
        if (this.Redirect) Response.Redirect(NavigationHelper.GetHomeUrl(), false);
    }
}
