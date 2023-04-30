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

public partial class ConLib_WishlistSearch : System.Web.UI.UserControl
{
    protected void Page_Init(object sender, EventArgs e)
    {
        PageHelper.SetDefaultButton(Name, FindButton);
    }

    protected void FindButton_Click(object sender, System.EventArgs e)
    {
        if (!string.IsNullOrEmpty(Name.Text))
        {
            Response.Redirect(("~/FindWishlist.aspx?name=" + Server.UrlEncode(Name.Text)));
        }
    }
}
