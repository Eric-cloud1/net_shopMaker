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
using MakerShop.Orders;
using MakerShop.Payments;
using MakerShop.Users;
using MakerShop.Utility;
using MakerShop.Stores;

public partial class Admin_Orders_AffiliateDataViewport : System.Web.UI.UserControl
{


    private Order Order
    {
        get
        {
            return OrderHelper.GetOrderFromContext();
        }

    }
    private bool ShowData
    {
        get
        {
            if (ViewState["ShowData"] == null) return false;
            return (bool)ViewState["ShowData"];
        }
        set { ViewState["ShowData"] = value; }
    }
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (ShowData)
        {
            if (Order != null)
            {
                if (Order.AffiliateId != 0)
                {
                    try
                    {
                        Affiliate.Text = Order.Affiliate.Name;
                        AffiliateID.Text = Order.AffiliateId.ToString();
                    }
                    catch
                    {
                        Affiliate.Text = Order.AffiliateId.ToString();
                        AffiliateID.Text = Order.AffiliateId.ToString();
                    }
                }
                else Affiliate.Text = "NONE";

                if (!string.IsNullOrEmpty(Order.SubAffiliate))
                {
                    Sub.Text = Order.SubAffiliate;
                }
                else Sub.Text = "NONE";
                if (!string.IsNullOrEmpty(Order.Referrer))
                {
                    OrderReferrer.NavigateUrl = Order.Referrer;

                    int len = 75;
                    if (Order.Referrer.Length < 75)
                    {
                        len = Order.Referrer.Length;
                    }

                    OrderReferrer.Text = Order.Referrer.Substring(0, len) + @"...";
                    //_Order.Referrer.Replace("/", "/<wbr />").Replace("_", "_<wbr />");
                }
            }
        }
    }
    protected void ShowData_Click(object sender, EventArgs e)
    {
        pData.Visible = true;
        ShowData = true;
        bShowData.Visible = false;
    }

}
