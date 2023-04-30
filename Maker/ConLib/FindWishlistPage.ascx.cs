using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Users;

public partial class ConLib_FindWishlistPage : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            SearchName.Text = Request.QueryString["name"];
            if (!string.IsNullOrEmpty(SearchName.Text)) SearchButton_Click(sender, e);
        }
    }

    protected string GetUserName(object dataItem)
    {
        User u = (User)dataItem;
        string fullName = u.PrimaryAddress.FullName;
        if (!string.IsNullOrEmpty(fullName))
        {
            return fullName;
        }
        return u.UserName;
    }

    protected string GetLocation(object dataItem)
    {
        Address a = (Address)dataItem;
        List<string> l = new List<string>();
        if (!string.IsNullOrEmpty(a.City))
        {
            l.Add(a.City);
        }
        if (!string.IsNullOrEmpty(a.Province))
        {
            l.Add(a.Province);
        }
        return string.Join(",", l.ToArray());
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        WishlistGrid.Visible = true;
    }
}
