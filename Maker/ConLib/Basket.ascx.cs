using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using MakerShop.Catalog;
using MakerShop.Common;
using MakerShop.DigitalDelivery;
using MakerShop.Marketing;
using MakerShop.Messaging;
using MakerShop.Orders;
using MakerShop.Payments;
using MakerShop.Payments.Providers;
using MakerShop.Products;
using MakerShop.Reporting;
using MakerShop.Shipping;
using MakerShop.Stores;
using MakerShop.Taxes;
using MakerShop.Taxes.Providers;
using MakerShop.Users;
using MakerShop.Utility;
using MakerShop.Payments.Providers.GoogleCheckout.Checkout;
using MakerShop.Payments.Providers.GoogleCheckout.AC;
using MakerShop.Payments.Providers.GoogleCheckout.Util;
using System.Xml;

public partial class ConLib_Basket : System.Web.UI.UserControl
{
    private OrderItemType[] displayItemTypes = { OrderItemType.Product, OrderItemType.Discount, OrderItemType.Coupon, OrderItemType.GiftWrap };
    DataControlField _TaxColumn = null;
    BasketItemCollection _DisplayedBasketItems;

    protected void Page_Load(object sender, EventArgs e)
    {
     //   GoogleCheckoutButton.BasketGrid = BasketGrid;
      //  GoogleCheckoutButton.WarningMessageList = WarningMessageList;
        //TOGGLE VAT COLUMN
        _TaxColumn = BasketGrid.Columns[3];
        _TaxColumn.Visible = TaxHelper.ShowTaxColumn;
        _TaxColumn.HeaderText = TaxHelper.TaxColumnHeader;
    }

    private void BindBasketGrid()
    {
        //BIND THE GRID
        BasketGrid.DataSource = GetBasketItems();
        BasketGrid.DataBind();
    }

    private BasketItemCollection GetBasketItems()
    {
        User user = Token.Instance.User;
        Basket basket = user.Basket;
        basket.Package();
        _DisplayedBasketItems = new BasketItemCollection();
        foreach (BasketItem item in basket.Items)
        {
            if (Array.IndexOf(displayItemTypes, item.OrderItemType) > -1)
            {
                if (item.OrderItemType == OrderItemType.Product && item.IsChildItem)
                {
                    // WHETHER THE CHILD ITEM DISPLAYS DEPENDS ON THE ROOT
                    BasketItem rootItem = item.GetParentItem(true);
                    if (rootItem.Product != null && rootItem.Product.Kit.ItemizeDisplay)
                    {
                        // ITEMIZED DISPLAY ENABLED, SHOW THIS CHILD ITEM
                        _DisplayedBasketItems.Add(item);
                    }
                }
                else
                {
                    // NO ADDITIONAL CHECK REQUIRED TO INCLUDE ROOT PRODUCTS OR NON-PRODUCTS
                    _DisplayedBasketItems.Add(item);
                }
            }
        }
        // ADD IN ANY CHILD ITEMS

        //ADD IN TAX ITEMS IF SPECIFIED FOR DISPLAY
        if (TaxHelper.GetEffectiveShoppingDisplay(user) == TaxShoppingDisplay.LineItem)
        {
            foreach (BasketItem item in basket.Items)
            {
                //IS THIS A TAX ITEM?
                if (item.OrderItemType == OrderItemType.Tax)
                {
                    ////IS THE TAX ITEM A PARENT ITEM OR A CHILD OF A DISPLAYED ITEM?
                    //if (!item.IsChildItem || (_DisplayedBasketItems.IndexOf(item.ParentItemId) > -1))
                    //{
                        //TAX SHOULD BE SHOWN
                        _DisplayedBasketItems.Add(item);
                    //}
                }
            }
        }
        //SORT ITEMS TO COMPLETE INTITIALIZATION
        _DisplayedBasketItems.Sort(new BasketItemComparer());
        return _DisplayedBasketItems;
    }

    protected void BasketGrid_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        //COMBINE FOOTER CELLS FOR SUBTOTAL
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            if (e.Row.Cells.Count > 2)
            {
                int colspan = e.Row.Cells.Count - 1;
                if (_TaxColumn == null || !_TaxColumn.Visible) colspan -= 1;
                int iterations = e.Row.Cells.Count - 3;
                for (int i = 0; i <= iterations; i++)
                {
                    e.Row.Cells.RemoveAt(0);
                }
                e.Row.Cells[0].ColumnSpan = colspan;
            }
        }
    }

    protected void BasketGrid_DataBound(object sender, EventArgs e)
    {
        if (BasketGrid.Rows.Count > 0)
        {
            CheckoutButton.Visible = !Token.Instance.Store.Settings.ProductPurchasingDisabled;
            ClearBasketButton.Visible = true;
            UpdateButton.Visible = true;
         /*   if (GoogleCheckoutButton.IsConfigured)
            {
                GoogleCheckoutButton.Visible = !Token.Instance.Store.Settings.ProductPurchasingDisabled;
            }
     */       EmptyBasketPanel.Visible = false;
            ValidateOrderMinMaxAmounts();
        }
        else
        {
            CheckoutButton.Visible = false;
            ClearBasketButton.Visible = false;
            UpdateButton.Visible = false;
            //GoogleCheckoutButton.Visible = false;
            OrderAboveMaximumAmountMessage.Visible = false;
            OrderBelowMinimumAmountMessage.Visible = false;
            EmptyBasketPanel.Visible = true;
        }
    }

    private void ValidateOrderMinMaxAmounts()
    {
        // IF THE ORDER AMOUNT DOES NOT FALL IN VALID RANGE SPECIFIED BY THE MERCHENT
        OrderItemType[] args = new OrderItemType[] { OrderItemType.Charge, 
                                                    OrderItemType.Coupon, OrderItemType.Credit, OrderItemType.Discount, 
                                                    OrderItemType.GiftCertificate, OrderItemType.GiftWrap, OrderItemType.Handling, 
                                                    OrderItemType.Product, OrderItemType.Shipping, OrderItemType.Tax };
        LSDecimal orderTotal = Token.Instance.User.Basket.Items.TotalPrice(args);
        StoreSettingCollection settings = Token.Instance.Store.Settings;
        LSDecimal minOrderAmount = settings.OrderMinimumAmount;
        LSDecimal maxOrderAmount = settings.OrderMaximumAmount;

        if ((minOrderAmount > orderTotal) || (maxOrderAmount > 0 && maxOrderAmount < orderTotal))
        {
            CheckoutButton.Enabled = false;
         //   GoogleCheckoutButton.Enabled = false;
            OrderBelowMinimumAmountMessage.Visible = (minOrderAmount > orderTotal);
            if (OrderBelowMinimumAmountMessage.Visible) OrderBelowMinimumAmountMessage.Text = string.Format(OrderBelowMinimumAmountMessage.Text, minOrderAmount);
            OrderAboveMaximumAmountMessage.Visible = !OrderBelowMinimumAmountMessage.Visible;
            if (OrderAboveMaximumAmountMessage.Visible) OrderAboveMaximumAmountMessage.Text = string.Format(OrderAboveMaximumAmountMessage.Text, maxOrderAmount);
        }
        else
        {
            CheckoutButton.Enabled = true;
            //GoogleCheckoutButton.Enabled = true;
            OrderAboveMaximumAmountMessage.Visible = false;
            OrderBelowMinimumAmountMessage.Visible = false;
        }
    }

    protected void BasketGrid_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        Basket basket;
        int index;
        switch (e.CommandName)
        {
            case "SaveItem":
                basket = Token.Instance.User.Basket;
                index = basket.Items.IndexOf(AlwaysConvert.ToInt(e.CommandArgument.ToString()));
                if ((index > -1))
                {
                    basket.Items.MoveToWishlist(index, Token.Instance.User.PrimaryWishlist);
                }
                break;
            case "DeleteItem":
                basket = Token.Instance.User.Basket;
                index = basket.Items.IndexOf(AlwaysConvert.ToInt(e.CommandArgument.ToString()));
                if ((index > -1))
                {
                    basket.Items.DeleteAt(index);
                }
                break;
            case "DeleteCouponItem":				
                basket = Token.Instance.User.Basket;							
                index = basket.Items.IndexOf(AlwaysConvert.ToInt(e.CommandArgument.ToString()));				
                if ((index > -1))
                {
                    BasketItem bitem = basket.Items[index];
                    if (bitem.OrderItemType == OrderItemType.Coupon)
                    {
                        basket.Items.DeleteAt(index);
                        foreach (BasketCoupon cpn in basket.BasketCoupons)
                        {
                            if (cpn.Coupon.CouponCode == bitem.Sku)
                            {                                
                                basket.BasketCoupons.Remove(cpn);
                                cpn.Delete();
                                basket.BasketCoupons.Save();
                                break;
                            }
                        }
                    }                    
                }
                break;
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        //GET ANY MESSAGES FROM SESSION
        List<string> sessionMessages = Session["BasketMessage"] as List<string>;
        //GET THE BASKET AND RECALCULATE
        Basket basket = Token.Instance.User.Basket;
		basket.Package();
        basket.Recalculate();
        //VALIDATE THE BASKET
        List<string> basketMessages;
        bool basketValid = basket.Validate(out basketMessages);		
        //DISPLAY ANY WARNING MESSAGES
        if ((!basketValid) || (sessionMessages != null))
        {
            if (sessionMessages != null)
            {
                Session.Remove("BasketMessage");
                sessionMessages.AddRange(basketMessages);
                WarningMessageList.DataSource = sessionMessages;
            }
            else
            {
                WarningMessageList.DataSource = basketMessages;
            }
            WarningMessageList.DataBind();
        }
        BindBasketGrid();
    }

    protected void ClearBasketButton_Click(object sender, EventArgs e)
    {
        Token.Instance.User.Basket.Clear();
        BindBasketGrid();
    }

    protected void KeepShoppingButton_Click(object sender, EventArgs e)
    {
        Response.Redirect(NavigationHelper.GetLastShoppingUrl());
    }

    protected void UpdateButton_Click(object sender, EventArgs e)
    {
        BasketHelper.SaveBasket(BasketGrid);
        BindBasketGrid();
    }

    protected void CheckoutButton_Click(object sender, EventArgs e)
    {
        BasketHelper.SaveBasket(BasketGrid);
        List<string> basketMessages;
        bool basketValid = Token.Instance.User.Basket.Validate(out basketMessages);
        if (basketValid) Response.Redirect(NavigationHelper.GetCheckoutUrl());
        else Session["BasketMessage"] = basketMessages;
    }

    protected LSDecimal GetBasketSubtotal()
    {
        LSDecimal basketTotal = 0;
        foreach (BasketItem bi in _DisplayedBasketItems)
        {
            basketTotal += TaxHelper.GetShopExtendedPrice(Token.Instance.User.Basket, bi);
        }
        
        return basketTotal;
    }

    protected bool ShowProductImagePanel(object dataItem)
    {
        BasketItem item = (BasketItem)dataItem;
        return ((item.OrderItemType == OrderItemType.Product) && (!string.IsNullOrEmpty(item.Product.ThumbnailUrl)));
    }

    protected bool IsProduct(object dataItem)
    {
        BasketItem item = (BasketItem)dataItem;
        return item.OrderItemType == OrderItemType.Product;
    }

    protected bool IsParentProduct(object dataItem)
    {
        BasketItem item = (BasketItem)dataItem;
        return (item.OrderItemType == OrderItemType.Product && !item.IsChildItem);
    }
}
