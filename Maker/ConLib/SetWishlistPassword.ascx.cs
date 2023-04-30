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
using MakerShop.Utility;

public partial class ConLib_SetWishlistPassword : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Wishlist w = Token.Instance.User.PrimaryWishlist;
            WishlistPasswordValue.Text = w.ViewPassword;
        }
    }

    protected void SavePasswordButton_Click(object sender, System.EventArgs e)
    {
        WishlistPasswordValue.Text = StringHelper.StripHtml(WishlistPasswordValue.Text);
        Wishlist w = Token.Instance.User.PrimaryWishlist;
        w.ViewPassword = WishlistPasswordValue.Text;
        w.Save();
        UpdatedPanel.Visible = true;
    }
}