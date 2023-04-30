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
using MakerShop.Common;
using MakerShop.Orders;
using MakerShop.Utility;

public partial class ConLib_OrderTotalSummary : System.Web.UI.UserControl
{
    private int _OrderId;
    private Order _Order;
    private bool _ShowTitle = true;
    private bool _ShowTaxBreakdown = true;

    protected int OrderId
    {
        get { return _OrderId; }
    }

    protected Order Order
    {
        get { return _Order; }
    }

    /// <summary>
    /// Gets or sets a flag that indicates whether the title bar should be shown for the summary dialog
    /// </summary>
    public bool ShowTitle
    {
        get { return _ShowTitle; }
        set { _ShowTitle = value; }
    }

    /// <summary>
    /// Gets or sets a flag that indicates whether taxes should be shown as a summary or breakdown
    /// </summary>
    public bool ShowTaxBreakdown
    {
        get { return _ShowTaxBreakdown; }
        set { _ShowTaxBreakdown = value; }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        _OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
        _Order = OrderDataSource.Load(_OrderId);
        LSDecimal subtotal = 0;
        LSDecimal shipping = 0;
        Dictionary<string, LSDecimal> taxes = new Dictionary<string, LSDecimal>();
        LSDecimal totalTaxAmount = 0;
        LSDecimal coupons = 0;
        LSDecimal total = 0;
        LSDecimal giftwrap = 0;
        LSDecimal adjustments = 0;
        foreach (OrderItem item in _Order.Items)
        {
            LSDecimal extendedPrice = item.ExtendedPrice;
            switch (item.OrderItemType)
            {
                case OrderItemType.Shipping:
                case OrderItemType.Handling:
                    shipping += extendedPrice;
                    break;
                case OrderItemType.Tax:
                    if (taxes.ContainsKey(item.Name)) taxes[item.Name] += extendedPrice;
                    else taxes[item.Name] = extendedPrice;
                    totalTaxAmount += extendedPrice;
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
            total += extendedPrice;
        }
        Subtotal.Text = subtotal.ToString("ulc");
        if (giftwrap != 0)
        {
            trGiftWrap.Visible = true;
            GiftWrap.Text = giftwrap.ToString("ulc");
        }
        else trGiftWrap.Visible = false;
        Shipping.Text = shipping.ToString("ulc");

        string taxRow = "<tr><th>{0}: </th><td>{1:ulc}</td></tr>";
        if (ShowTaxBreakdown)
        {
            // TAXES ARE DISPLAYED, ITEMIZE BY TAX NAME
            foreach (string taxName in taxes.Keys)
            {
                phTaxes.Controls.Add(new LiteralControl(string.Format(taxRow, taxName, taxes[taxName])));
            }
        }
        else
        {
            phTaxes.Controls.Add(new LiteralControl(string.Format(taxRow, "Taxes", totalTaxAmount)));
        }
        
        if (coupons != 0)
        {
            trCoupon.Visible = true;
            Coupons.Text = coupons.ToString("ulc");
        }
        else trCoupon.Visible = false;

        if (adjustments != 0)
        {
            trAdjustments.Visible = true;
            Adjustments.Text = adjustments.ToString("ulc");
        }
        else trAdjustments.Visible = false;
        
        Total.Text = total.ToString("ulc");
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        TitlePanel.Visible = this.ShowTitle;
    }
}
