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
using MakerShop.Marketing;
using MakerShop.Orders;
using MakerShop.Utility;

public partial class ConLib_CouponDialog : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CouponCode.Attributes.Add("autocomplete", "off");
    }

    protected void ApplyCouponButton_Click(object sender, EventArgs e)
    {
        ValidCouponMessage.Visible = false;
        CouponCode.Text = StringHelper.StripHtml(CouponCode.Text);
        Coupon coupon = CouponDataSource.LoadForCouponCode(CouponCode.Text);
        String couponValidityMessage = String.Empty;
        if (coupon != null)
        {
            if (!CouponCalculator.IsCouponAlreadyUsed(Token.Instance.User.Basket, coupon))
            {
                if (CouponCalculator.IsCouponValid(Token.Instance.User.Basket, coupon, out couponValidityMessage))
                {
                    Basket basket = Token.Instance.User.Basket;
                    BasketCoupon recentCoupon = new BasketCoupon(Token.Instance.UserId, coupon.CouponId);
                    basket.BasketCoupons.Add(recentCoupon);
                    // APPLY COUPON COMBINE RULE
                    //THE RULE: 
                    //If most recently applied coupon is marked "do not combine", then all previously
                    //entered coupons must be removed from basket.

                    //If most recently applied coupon is marked "combine", then remove any applied
                    //coupon that is marked "do not combine".  (Logically, in this instance there
                    //could be at most one other coupon of this type...)
                    string previousCouponsRemoved = "";

                    if (recentCoupon.Coupon.AllowCombine)
                    {
                        //IF ALLOW COMBINE, REMOVE ALL PREVIOUS NOCOMBINE COUPONS
                        for (int i = (basket.BasketCoupons.Count - 1); i >= 0; i--)
                        {
                            if (!basket.BasketCoupons[i].Coupon.AllowCombine)
                            {
                                if (previousCouponsRemoved.Length > 0)
                                {
                                    previousCouponsRemoved += "," + basket.BasketCoupons[i].Coupon.Name;
                                }
                                else
                                {
                                    previousCouponsRemoved = basket.BasketCoupons[i].Coupon.Name;
                                }

                                basket.BasketCoupons.DeleteAt(i);
                            }
                        }
                    }
                    else
                    {
                        //IF NOT ALLOW COMBINE, REMOVE ALL EXCEPT THIS COUPON
                        for (int i = (basket.BasketCoupons.Count - 1); i >= 0; i--)
                        {
                            if (basket.BasketCoupons[i] != recentCoupon)
                            {
                                if (previousCouponsRemoved.Length > 0)
                                {
                                    previousCouponsRemoved += "," + basket.BasketCoupons[i].Coupon.Name;
                                }
                                else
                                {
                                    previousCouponsRemoved = basket.BasketCoupons[i].Coupon.Name;
                                }
                                basket.BasketCoupons.DeleteAt(i);
                            }
                        }
                    }

                    basket.Save();
                    basket.Recalculate();
                    if (previousCouponsRemoved.Length > 0)
                    {
                        if (recentCoupon.Coupon.AllowCombine)
                        {
                            CombineCouponRemoveMessage.Text = String.Format(CombineCouponRemoveMessage.Text, recentCoupon.Coupon.Name, previousCouponsRemoved, previousCouponsRemoved);
                            CombineCouponRemoveMessage.Visible = true;
                        }
                        else
                        {
                            NotCombineCouponRemoveMessage.Text = String.Format(NotCombineCouponRemoveMessage.Text, recentCoupon.Coupon.Name, previousCouponsRemoved);
                            NotCombineCouponRemoveMessage.Visible = true;
                        }
                    }
                    ValidCouponMessage.Visible = true;
                }
                else
                {
                    InvalidCouponMessage.Text = couponValidityMessage + "<br /><br />";
                }
            }
            else
            {
                InvalidCouponMessage.Text = "The coupon code you've entered is already in use.<br /><br />";
            }
        }
        else
        {
            InvalidCouponMessage.Text = "The coupon code you've entered is invalid.<br /><br />";
        }
        CouponCode.Text = string.Empty;
        InvalidCouponMessage.Visible = !ValidCouponMessage.Visible;
    }
}
