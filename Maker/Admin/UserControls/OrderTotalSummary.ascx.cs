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
using MakerShop.Utility;

public partial class Admin_UserControls_OrderTotalSummary : System.Web.UI.UserControl
{

    private bool _ShowTitle = true;
    public bool ShowTitle
    {
        get { return _ShowTitle; }
        set { _ShowTitle = value; }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        Order order = OrderHelper.GetOrderFromContext();
        if (order != null)
        {
            LSDecimal subtotal = 0;
            LSDecimal shipping = 0;
            LSDecimal taxes = 0;
            LSDecimal coupons = 0;
            LSDecimal total = 0;
            LSDecimal giftwrap = 0;
            LSDecimal adjustments = 0;
            foreach (OrderItem item in order.Items)
            {
                LSDecimal extendedPrice = item.ExtendedPrice;
                switch (item.OrderItemType)
                {
                    case OrderItemType.Shipping:
                    case OrderItemType.Handling:
                        shipping += extendedPrice;
                        break;
                    case OrderItemType.Tax:
                        taxes += extendedPrice;
                        break;
                    case OrderItemType.Coupon:
                        coupons += extendedPrice;
                        break;
                    case OrderItemType.GiftWrap:
                        giftwrap += extendedPrice;
                        break;
                    case OrderItemType.Charge:
                    case OrderItemType.Credit:
                        adjustments += extendedPrice;
                        break;
                    default:
                        subtotal += extendedPrice;
                        break;
                }
                total += item.ExtendedPrice;
            }

            Subtotal.Text = subtotal.ToString("lc");
            Shipping.Text = shipping.ToString("lc");
            Taxes.Text = taxes.ToString("lc");
            Total.Text = total.ToString("lc");

            if (giftwrap != 0)
            {
                trGiftWrap.Visible = true;
                GiftWrap.Text = giftwrap.ToString("lc");
            }
            else trGiftWrap.Visible = false;

            if (coupons != 0)
            {
                trCoupon.Visible = true;
                Coupons.Text = coupons.ToString("lc");
            }
            else trCoupon.Visible = false;

            if (adjustments != 0)
            {
                trAdjustments.Visible = true;
                Adjustments.Text = adjustments.ToString("lc");
            }
            else trAdjustments.Visible = false;
            TitlePanel.Visible = this.ShowTitle;
        }
    }
}
