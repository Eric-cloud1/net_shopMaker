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
using MakerShop.Orders;
using MakerShop.Common;
using MakerShop.Utility;

public partial class Checkout_AffiliateTracker : System.Web.UI.UserControl
{
    private int _OrderId;
    private Order _Order;

    protected void Page_Load(object sender, EventArgs e)
    {
        string url = Token.Instance.Store.Settings.AffiliateTrackerUrl;
        if (!string.IsNullOrEmpty(url))
        {
            //TRACKER URL EXISTS, ATTEMPT TO LOAD UP ORDER
            _OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
            _Order = OrderDataSource.Load(_OrderId);
            if (_Order != null)
            {
                //DO NOT DISPLAY TRACKER URL IF IT IS MORE THAN 10 MINUTES FROM DATE OF ORDER
                if (_Order.OrderDate < LocaleHelper.LocalNow.AddMinutes(10))
                {
                    //UPDATE DYNAMIC VARIABLES IN URL
                    url = url.Replace("[OrderId]", _Order.OrderId.ToString());
                    url = url.Replace("[OrderNumber]", _Order.OrderNumber.ToString());
                    url = url.Replace("[OrderTotal]", _Order.TotalCharges.ToString("F2"));
                    url = url.Replace("[OrderSubTotal]", _Order.Items.TotalPrice(OrderItemType.Product, OrderItemType.Discount, OrderItemType.Coupon).ToString("F2"));
                    //CREATE THE IMAGE CONTROL
                    HtmlImage img = new HtmlImage();
                    img.Src = url;
                    img.Border = 0;
                    img.Height = 1;
                    img.Width = 1;
                    //ADD IMAGE CONTROL TO PAGE
                    phAffiliateTracker.Controls.Add(img);
                }
            }
        }
    }
}
