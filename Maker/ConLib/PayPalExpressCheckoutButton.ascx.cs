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
using MakerShop.Catalog;
using MakerShop.Products;
using MakerShop.Payments.Providers.PayPal;
using MakerShop.Stores;

public partial class ConLib_PayPalExpressCheckoutButton : System.Web.UI.UserControl
{
    private bool BasketHasProducts()
    {
        Basket basket = Token.Instance.User.Basket;
        foreach (BasketItem item in basket.Items)
        {
            if (item.OrderItemType == OrderItemType.Product) return true;
        }
        return false;
    }

    protected void Page_PreRender(object sender, System.EventArgs e)
    {
        // EXPRESS CHECKOUT BUTTON IS NOT VISIBLE BY DEFAULT
        // DETERMINE WHETHER THE PAYPAL GATEWAY IS DEFINED, AND WHETHER IT HAS API SERVICES ENABLED
        // BUTTON ONLY SHOWS IF PRODUCTS ARE IN THE BASKET AND MIN/MAX ORER LIMITS ARE MET
        if (BasketHasProducts())
        {
            // FIND THE PAYPAL GATEWAY
            PayPalProvider provider = StoreDataHelper.GetPayPalProvider();
            if (provider != null && provider.ApiEnabled && ValidateOrderMinMaxAmounts())
            {
                //SHOW PANEL IF API ENABLED
                ExpressCheckoutPanel.Visible = !Token.Instance.Store.Settings.ProductPurchasingDisabled;
                if (ShowHeader) phHeader.Visible = true;
                else phHeader.Visible = false;
                if (ShowDescription) phDescription.Visible = true;
                else phDescription.Visible = false;
                ExpressCheckoutPanel.CssClass = PanelCSSClass;
            }
        }
    }

    private bool _ShowHeader = true;
    public bool ShowHeader
    {
        get { return this._ShowHeader; }
        set { this._ShowHeader = value; }
    }

    private bool _ShowDescription = true;
    public bool ShowDescription
    {
        get { return this._ShowDescription; }
        set { this._ShowDescription = value; }
    }

    private string _PanelCSSClass = "section";
    public string PanelCSSClass
    {
        get { return _PanelCSSClass; }
        set { _PanelCSSClass = value; }
    }

    protected bool ValidateOrderMinMaxAmounts()
    {
        // IF THE ORDER AMOUNT DOES NOT FALL IN VALID RANGE SPECIFIED BY THE MERCHENT
        OrderItemType[] args = new OrderItemType[] { OrderItemType.Charge, 
                                                    OrderItemType.Coupon, OrderItemType.Credit, OrderItemType.Discount, 
                                                    OrderItemType.GiftCertificate, OrderItemType.GiftWrap, OrderItemType.Handling, 
                                                    OrderItemType.Product, OrderItemType.Shipping, OrderItemType.Tax };
        LSDecimal orderTotal = Token.Instance.User.Basket.Items.TotalPrice(args);
        StoreSettingCollection settings = Token.Instance.Store.Settings;
        decimal minOrderAmount = settings.OrderMinimumAmount;
        decimal maxOrderAmount = settings.OrderMaximumAmount;

        if ((minOrderAmount > orderTotal) || (maxOrderAmount > 0 && maxOrderAmount < orderTotal))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    protected void ExpressCheckoutLink_Click(object sender, EventArgs e)
    {
        GridView basketGrid = PageHelper.RecursiveFindControl(this.Page, "BasketGrid") as GridView;
        if (basketGrid != null)
        {
            BasketHelper.SaveBasket(basketGrid);
        }
        if (ValidateOrderMinMaxAmounts())
        {
            Response.Redirect("~/PayPalExpressCheckout.aspx?Action=SET");
        }
        else
        {
            Response.Redirect(NavigationHelper.GetBasketUrl());
        }
    }
}