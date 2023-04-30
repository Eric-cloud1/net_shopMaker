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
using MakerShop.Taxes;
using MakerShop.Utility;

public partial class ConLib_MiniBasket : System.Web.UI.UserControl
{
	private string _AlternateControl = "PopularProductsDialog.ascx";

	public string AlternateControl
    {
        set
        {
            _AlternateControl = value;            
        }
		get 
		{
			return _AlternateControl;
		}
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        PageHelper.RegisterBasketControl(this.Page);
        if (!string.IsNullOrEmpty(_AlternateControl))
        {
            Control altControl = LoadControl(_AlternateControl);
            AlternateControlPanel.Controls.Add(altControl);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        CheckoutButton.Visible = !Token.Instance.Store.Settings.ProductPurchasingDisabled;
        PayPalExpressCheckoutButton.Visible = !Token.Instance.Store.Settings.ProductPurchasingDisabled;
        //GoogleCheckoutButton.Visible = !Token.Instance.Store.Settings.ProductPurchasingDisabled;
    }

    //BUILD THE BASKET ON PRERENDER SO THAT WE CAN ACCOUNT
    //FOR ANY PRODUCTS ADDED DURING THE POSTBACK CYCLE
    protected void Page_PreRender(object sender, System.EventArgs e)
    {
        //PREPARE BASKET FOR DISPLAY
        Basket basket = Token.Instance.User.Basket;
        basket.Package(true);
        basket.Recalculate();
        //VALIDATE THE BASKET
        List<string> basketMessages = new List<string>();
        bool basketValid = basket.Validate(out basketMessages);        
        //DISPLAY ANY WARNING MESSAGES
        if (!basketValid)
        {
            Session["BasketMessage"] = basketMessages;
            //Redirect to basket page where these error messages will be displayed
            Response.Redirect("~/Basket.aspx");
        }
        BindBasket(basket);
    }

    private void BindBasket(Basket basket)
    {
        //GET LIST OF PRODUCTS
        BasketItemCollection _Products = new BasketItemCollection();
        LSDecimal _ProductTotal = 0;
        LSDecimal _DiscountTotal = 0;

        // MAKE SURE ITEMS ARE PROPERTY SORTED BEFORE DISPLAY
        basket.Items.Sort(new BasketItemComparer());
        foreach (BasketItem item in basket.Items)
        {
            if (item.OrderItemType == OrderItemType.Product)
            {
                if (!item.IsChildItem)
                {
                    // ROOT LEVEL ITEMS GET ADDED
                    _Products.Add(item);
                    _ProductTotal += TaxHelper.GetShopExtendedPrice(basket, item);
                }
                else
                {
                    BasketItem rootItem = item.GetParentItem(true);
                    if (rootItem.Product != null && rootItem.Product.Kit.ItemizeDisplay)
                    {
                        // CHILD PRODUCTS SHOULD HAVE THEIR TOTAL ADDED TO THE ROOT PRODUCT
                        int parentIndex = _Products.IndexOf(item.GetParentItem(true).BasketItemId);
                        if (parentIndex > -1)
                        {
                            BasketItem parentItem = _Products[parentIndex];
                            LSDecimal shopPrice = TaxHelper.GetShopExtendedPrice(basket, item) / parentItem.Quantity;
                            parentItem.Price += shopPrice;
                            _ProductTotal += shopPrice;
                        }
                    }
                }
            }
            else if (item.OrderItemType == OrderItemType.Discount)
            {
                _DiscountTotal += TaxHelper.GetShopExtendedPrice(basket, item);
            }
        }
        if (_Products.Count > 0)
        {
            //BIND BASKET ITEMS 
            BasketRepeater.DataSource = _Products;
            BasketRepeater.DataBind();
            if (_DiscountTotal != 0)
            {
                Discounts.Text = _DiscountTotal.ToString("ulc");
                DiscountsPanel.Visible = true;
            }
            else
            {
                DiscountsPanel.Visible = false;
            }
            Discounts.Text = _DiscountTotal.ToString("ulc");
            SubTotal.Text = (_ProductTotal + _DiscountTotal).ToString("ulc");
            //UPDATE CHECKOUT LINK
            //CheckoutLink.NavigateUrl = NavigationHelper.GetCheckoutUrl();
            ShowBasket(true);
        }
        else
        {
            ShowBasket(false);
        }
    }

    protected LSDecimal GetItemShopPrice(BasketItem item)
    {
        return TaxHelper.GetShopPrice(item.Basket, item);
    }

    private void ShowBasket(bool status)
    {        
       BasketTable.Visible = status;
       EmptyBasketPanel.Visible = !status;
       if (!status) ContentPanel.CssClass += " nofooter";
       if (status)
       {
           MiniBasketHolder.CssClass = "VisiblePanel";
		   AlternateControlPanel.CssClass = "HiddenPanel";
       }
       else
       {
           MiniBasketHolder.CssClass = "HiddenPanel";
		   AlternateControlPanel.CssClass = "VisiblePanel";
       }
    }

    protected string GetIconUrl(Object obj)
    {
        BasketItem bitem = obj as BasketItem;
        if (bitem != null)
        {
            if (!string.IsNullOrEmpty(bitem.Product.IconUrl))
            {
                return bitem.Product.IconUrl;
            }
            else if (!string.IsNullOrEmpty(bitem.Product.ThumbnailUrl))
            {
                return bitem.Product.ThumbnailUrl;
            }
            else
            {
                return bitem.Product.ImageUrl;
            }
        }
        return "";
    }

    protected bool HasImage(Object obj)
    {
        BasketItem bitem = obj as BasketItem;
        if (bitem != null)
        {
            if (!string.IsNullOrEmpty(bitem.Product.IconUrl))
            {
                return true;
            }
            else if (!string.IsNullOrEmpty(bitem.Product.ThumbnailUrl))
            {
                return true;
            }
            else if (!string.IsNullOrEmpty(bitem.Product.ImageUrl))
            {
                return true;
            }
        }
        return false;
    }

    protected void BasketRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        Basket basket;
        int index;
        switch (e.CommandName)
        {
            case "UpdateItem":
                basket = Token.Instance.User.Basket;
                int basketItemId = AlwaysConvert.ToInt(e.CommandArgument.ToString());
                UpdateBasketItem(BasketRepeater, basketItemId);                
                break;
            case "DeleteItem":
                basket = Token.Instance.User.Basket;
                index = basket.Items.IndexOf(AlwaysConvert.ToInt(e.CommandArgument.ToString()));
                if ((index > -1))
                {
                    basket.Items.DeleteAt(index);
                }                
                break;
        }
    }

    public static void UpdateBasketItem(Repeater BasketRepeater, int itemId)
    {
        if (itemId <= 0) return;
        Basket basket = Token.Instance.User.Basket;
        foreach (RepeaterItem saveItem in BasketRepeater.Items)
        {
            int basketItemId = 0;
            HiddenField basketItemIdField = (HiddenField)saveItem.FindControl("BasketItemId");
            if (basketItemIdField != null)
            {
                basketItemId = AlwaysConvert.ToInt(basketItemIdField.Value);
            }

            if (basketItemId > 0 && basketItemId == itemId)
            {
                int itemIndex = basket.Items.IndexOf(basketItemId);
                if ((itemIndex > -1))
                {
                    BasketItem item = basket.Items[itemIndex];
                    TextBox quantity = (TextBox)saveItem.FindControl("Quantity");
                    if (quantity != null)
                    {
						int qty = AlwaysConvert.ToInt(quantity.Text,item.Quantity);
						if(qty > System.Int16.MaxValue)
						{
							item.Quantity = System.Int16.MaxValue;
						}else{
							item.Quantity = (System.Int16)qty;
						}
                        
                        // Update for Minimum Maximum quantity of product
                        if (item.Quantity < item.Product.MinQuantity)
                        {
                            item.Quantity = item.Product.MinQuantity;
                            quantity.Text = item.Quantity.ToString();
                        }
                        else if ((item.Product.MaxQuantity > 0) && (item.Quantity > item.Product.MaxQuantity))
                        {
                            item.Quantity = item.Product.MaxQuantity;
                            quantity.Text = item.Quantity.ToString();
                        }
                        item.Save();
                    }
                }
            }
        }
    }

    protected void CheckoutButton_Click(object sender, EventArgs e)
    {
        Basket basket = Token.Instance.User.Basket;
        foreach (RepeaterItem saveItem in BasketRepeater.Items)
        {
            int basketItemId = 0;
            HiddenField basketItemIdField = (HiddenField)saveItem.FindControl("BasketItemId");
            if (basketItemIdField != null)
            {
                basketItemId = AlwaysConvert.ToInt(basketItemIdField.Value);
            }

            if (basketItemId > 0 )
            {
                int itemIndex = basket.Items.IndexOf(basketItemId);
                if ((itemIndex > -1))
                {
                    BasketItem item = basket.Items[itemIndex];
                    TextBox quantity = (TextBox)saveItem.FindControl("Quantity");
                    if (quantity != null)
                    {
                        item.Quantity = AlwaysConvert.ToInt16(quantity.Text);
                        // Update for Minimum Maximum quantity of product
                        if (item.Quantity < item.Product.MinQuantity)
                        {
                            item.Quantity = item.Product.MinQuantity;
                            quantity.Text = item.Quantity.ToString();
                        }
                        else if ((item.Product.MaxQuantity > 0) && (item.Quantity > item.Product.MaxQuantity))
                        {
                            item.Quantity = item.Product.MaxQuantity;
                            quantity.Text = item.Quantity.ToString();
                        }
                        item.Save();
                    }
                }
            }
        }

        Response.Redirect(NavigationHelper.GetCheckoutUrl());

    }
}
