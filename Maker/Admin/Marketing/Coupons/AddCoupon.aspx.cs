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
using MakerShop.Web.UI;

public partial class Admin_Marketing_Coupons_AddCoupon : MakerShopAdminPage
{
    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }

    protected void NextButton_Click(object sender, EventArgs e)
    {
        if (OrderCoupon.Checked)
        {
            Response.Redirect("AddCoupon2.aspx?CouponType=0");
        }
        else if (ProductCoupon.Checked)
        {
            Response.Redirect("AddCoupon2.aspx?CouponType=1");
        }
        Response.Redirect("AddCoupon2.aspx?CouponType=2");
    }
}
