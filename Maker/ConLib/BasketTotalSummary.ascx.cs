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

public partial class ConLib_BasketTotalSummary : System.Web.UI.UserControl
{
    private bool _ShowTaxes = true;
    private bool _ShowTaxBreakdown = true;

    /// <summary>
    /// Gets or sets a flag that indicates whether taxes should be shown 
    /// </summary>
    public bool ShowTaxes
    {
        get { return _ShowTaxes; }
        set { _ShowTaxes = value; }
    }

    /// <summary>
    /// Gets or sets a flag that indicates whether taxes should be shown as a summary or breakdown
    /// </summary>
    public bool ShowTaxBreakdown
    {
        get { return _ShowTaxBreakdown; }
        set { _ShowTaxBreakdown = value; }
    }

    //USE PRERENDER TO ALLOW FOR CALCULATIONS TO BASKET CONTENTS
    protected void Page_PreRender(object sender, EventArgs e)
    {
        LSDecimal subtotal = 0;
        LSDecimal shipping = 0;
        Dictionary<string, LSDecimal> taxes = new Dictionary<string, LSDecimal>();
        LSDecimal totalTaxAmount = 0;
        LSDecimal coupons = 0;
        LSDecimal total = 0;
        LSDecimal giftwrap = 0;
        Basket basket = Token.Instance.User.Basket;
        foreach (BasketItem item in basket.Items)
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
        if (this.ShowTaxes)
        {
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
        }
        else
        {
            // TAXES ARE NOT DISPLAYED, REMOVE ANY TAX FROM THE TOTAL
            total -= totalTaxAmount;
        }

        // SHIPPING SHOULD NOT BE VISIBLE WHEN USER BILLING ADDRESS IS NOT SELECTED
        if (!basket.User.PrimaryAddress.IsValid)
        {
            trShipping.Visible = false;
        }

        if (coupons != 0)
        {
            trCoupon.Visible = true;
            Coupons.Text = coupons.ToString("ulc");
        }
        else trCoupon.Visible = false;
        Total.Text = total.ToString("ulc");
    }

}
