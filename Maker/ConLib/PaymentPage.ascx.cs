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
using MakerShop.Payments;
using MakerShop.Orders;
using MakerShop.Stores;
using MakerShop.Users;
using MakerShop.Marketing;
using MakerShop.Taxes;
using MakerShop.Utility;
using MakerShop.Payments.Providers.PayPal;

public partial class ConLib_PaymentPage : System.Web.UI.UserControl
{
    protected void Page_Init(object sender, EventArgs e)
    {
        PageHelper.DisableValidationScrolling(this.Page);
        //SET THE UPDATE PROGRESS DELAY
        Control progress = PageHelper.RecursiveFindControl(this.Page, "UpdateProgress1");
        if (progress != null)
        {
            ((UpdateProgress)progress).DisplayAfter = 100;
        }
        User user = Token.Instance.User;
        //VERIFY A VALID BILLING ADDRESS IS AVAILABLE
        if (!user.PrimaryAddress.IsValid) Response.Redirect("EditBillAddress.aspx");
        BillingAddress.Text = user.PrimaryAddress.ToString(true);
        if (!Page.IsPostBack)
        {
            user.Basket.Recalculate();
            LSDecimal orderTotal = GetBasketTotal();            
            if (orderTotal <= 0)
			{
                CheckoutRequest checkoutRequest = new CheckoutRequest(null);
                CheckoutResponse checkoutResponse = null;
                checkoutResponse = Token.Instance.User.Basket.Checkout(checkoutRequest);
                Response.Redirect( "Receipt.aspx?OrderNumber=" + checkoutResponse.OrderNumber.ToString() + "&OrderId=" + checkoutResponse.OrderId.ToString());
            }
            StoreSettingCollection settings = Token.Instance.Store.Settings;
            LSDecimal minOrderAmount = settings.OrderMinimumAmount;
            LSDecimal maxOrderAmount = settings.OrderMaximumAmount;
            TermsAndConditions.Text = settings.CheckoutTermsAndConditions;

            // IF THE ORDER AMOUNT DOES NOT FALL IN VALID RANGE SPECIFIED BY THE MERCHENT
            if ((minOrderAmount > orderTotal) || (maxOrderAmount > 0 && maxOrderAmount < orderTotal))
            {
                InvalidOrderAmountPanel.Visible = true;
                ConfirmAndPayPanel.Visible = false;
                OrderBelowMinimumAmountMessage.Visible = (minOrderAmount > orderTotal);
                if (OrderBelowMinimumAmountMessage.Visible) OrderBelowMinimumAmountMessage.Text = string.Format(OrderBelowMinimumAmountMessage.Text, minOrderAmount);
                OrderAboveMaximumAmountMessage.Visible = !OrderBelowMinimumAmountMessage.Visible;
                if (OrderAboveMaximumAmountMessage.Visible) OrderAboveMaximumAmountMessage.Text = string.Format(OrderAboveMaximumAmountMessage.Text, maxOrderAmount);
            }
            else if (TermsAndConditions.Text.Length > 0)
            {
                //SHOW TERMS AND CONDITIONS IF PRESENT
                TermsAndConditionsSection.Visible = true;
                ConfirmAndPayPanel.Visible = false;
            }

            CouponCode.Attributes.Add("autocomplete", "off");
        }

        //SEE WHETHER WE HAVE MULTIPLE SHIPMENTS
        if (HasMultipleShipments()) _EditShipToLink = "~/Checkout/ShipAddresses.aspx";
        else _EditShipToLink = "~/Checkout/ShipAddress.aspx"; 

        BindPage();
    }

    private bool IsPayPalExpress()
    {
        ExpressCheckoutSession paypalSession = ExpressCheckoutSession.Current;
        if (paypalSession != null)
        {
            ASP.PayPalExpressPaymentForm paypalForm = new ASP.PayPalExpressPaymentForm();
            phPaymentForms.Controls.Add(paypalForm);
            return true;
        }
        return false;
    }

    protected void BindPage()
    {
        BindBasket();
        //WE SHOULD ONLY SHOW OUR PAYMENT METHODS IF THIS IS NOT PAYPAL EXPRESS CHECKOUT
        phPaymentForms.Controls.Clear();
        if (!IsPayPalExpress())
        {
            //ADD PAYMENT FORMS
            bool creditCardAdded = false;
            PaymentMethodCollection methods = StoreDataHelper.GetPaymentMethods(Token.Instance.UserId);
            foreach (PaymentMethod method in methods)
            {
                switch (method.PaymentInstrument)
                {
                    case PaymentInstrument.AmericanExpress:
                    case PaymentInstrument.Discover:
                    case PaymentInstrument.JCB:
                    case PaymentInstrument.MasterCard:
                    case PaymentInstrument.DinersClub:
                    case PaymentInstrument.Visa:
                    case PaymentInstrument.Maestro:
                    case PaymentInstrument.SwitchSolo:
                    case PaymentInstrument.VisaDebit:
                        if (!creditCardAdded)
                        {
                            phPaymentForms.Controls.Add(new ASP.CreditCardPaymentForm());
                            creditCardAdded = true;
                        }
                        break;
                    case PaymentInstrument.Check:
                        ASP.CheckPaymentForm checkForm = new ASP.CheckPaymentForm();
                        checkForm.PaymentMethodId = method.PaymentMethodId;
                        phPaymentForms.Controls.Add(checkForm);
                        break;
                    case PaymentInstrument.PurchaseOrder:
                        ASP.PurchaseOrderPaymentForm poForm = new ASP.PurchaseOrderPaymentForm();
                        poForm.PaymentMethodId = method.PaymentMethodId;
                        phPaymentForms.Controls.Add(poForm);
                        break;
                    case PaymentInstrument.PayPal:
                        ASP.PayPalPaymentForm paypalForm = new ASP.PayPalPaymentForm();
                        paypalForm.PaymentMethodId = method.PaymentMethodId;
                        phPaymentForms.Controls.Add(paypalForm);
                        break;
                    case PaymentInstrument.Mail:
                        ASP.MailPaymentForm mailForm = new ASP.MailPaymentForm();
                        mailForm.PaymentMethodId = method.PaymentMethodId;
                        phPaymentForms.Controls.Add(mailForm);
                        break;
                    case PaymentInstrument.PhoneCall:
                        ASP.PhoneCallPaymentForm phoneCallForm = new ASP.PhoneCallPaymentForm();
                        phoneCallForm.PaymentMethodId = method.PaymentMethodId;
                        phPaymentForms.Controls.Add(phoneCallForm);
                        break;
                    default:
                        //types not supported
                        break;
                }
            }

            if (StoreDataHelper.HasGiftCertificates())
            {
                ASP.GiftCertificatePaymentForm gcForm = new ASP.GiftCertificatePaymentForm();
                gcForm.NonShippingItemsGrid = NonShippingItemsGrid;
                phPaymentForms.Controls.Add(gcForm);
            }
        }
    }

    private void BindBasket()
    {
        //BIND SHIPMENTS
        Basket basket = Token.Instance.User.Basket;
        ShipmentRepeater.DataSource = basket.Shipments;
        ShipmentRepeater.DataBind();
        //BIND NONSHIPPING ITEMS
        BasketItemCollection nonShippingItems = BasketHelper.GetNonShippingItems(basket);
        if (nonShippingItems.Count > 0)
        {
            nonShippingItems.Sort(new BasketItemComparer());
            NonShippingItemsPanel.Visible = true;
            NonShippingItemsGrid.DataSource = nonShippingItems;
            NonShippingItemsGrid.DataBind();
        }
        else
        {
            NonShippingItemsPanel.Visible = false;
        }

        //HIDE THE COLUMNS BASED ON SETTING        
        NonShippingItemsGrid.Columns[3].Visible = TaxHelper.ShowTaxColumn;
        NonShippingItemsGrid.Columns[3].HeaderText = TaxHelper.TaxColumnHeader;
    }

    protected void ShipmentRepeater_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        GridView ShipmentItemsGrid = (GridView)e.Item.FindControl("ShipmentItemsGrid");
        if (ShipmentItemsGrid != null)
        {
            ShipmentItemsGrid.Columns[3].Visible = TaxHelper.ShowTaxColumn;
            ShipmentItemsGrid.Columns[3].HeaderText = TaxHelper.TaxColumnHeader;
        }        
    }

    protected LSDecimal GetBasketTotal()
    {
        OrderItemType[] args = new OrderItemType[] { OrderItemType.Charge, 
                                                    OrderItemType.Coupon, OrderItemType.Credit, OrderItemType.Discount, 
                                                    OrderItemType.GiftCertificate, OrderItemType.GiftWrap, OrderItemType.Handling, 
                                                    OrderItemType.Product, OrderItemType.Shipping, OrderItemType.Tax };
        return Token.Instance.User.Basket.Items.TotalPrice(args);
    }

    protected void ApplyCouponButton_Click(object sender, EventArgs e)
    {
        ValidCouponMessage.Visible = false;
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
                    BindBasket();
                }
                else
                {
                    InvalidCouponMessage.Text = couponValidityMessage;
                }
            }
            else
            {
                InvalidCouponMessage.Text = "The coupon code you've entered is already in use.";
            }
        }
		else 
		{
			InvalidCouponMessage.Text = "The coupon code you've entered is invalid.";
		}

        CouponCode.Text = string.Empty;
        InvalidCouponMessage.Visible = !ValidCouponMessage.Visible;
    }

    protected void AcceptTermsAndConditions_Click(object sender, EventArgs e)
    {
        TermsAndConditionsSection.Visible = false;
        ConfirmAndPayPanel.Visible = true;
    }

    private bool HasMultipleShipments()
    {
        Basket basket = Token.Instance.User.Basket;
        if (basket.Shipments.Count < 2) return false;
        int firstAddressId = basket.Shipments[0].AddressId;
        for (int i = 1; i < basket.Shipments.Count; i++)
        {
            if (firstAddressId != basket.Shipments[i].AddressId) return true;
        }
        return false;
    }

    private string _EditShipToLink;
    protected string GetEditShipToLink()
    {
        return _EditShipToLink;
    }

    protected string GetTaxHeader()
    {
        return TaxHelper.TaxColumnHeader;
    }
}
