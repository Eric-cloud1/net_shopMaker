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
using MakerShop.DigitalDelivery;
using MakerShop.Orders; 
using MakerShop.Products;
using MakerShop.Users;
using MakerShop.Stores;
using MakerShop.Utility;
using MakerShop.Taxes;
using MakerShop.Payments;

public partial class ConLib_BuyProductDialog : System.Web.UI.UserControl
{
    int _ProductId = 0;
    Product _Product = null;
    Dictionary<int, int> _SelectedOptions = null;
    List<int> _SelectedKitProducts = null;

    private bool _ShowSku = true;
    public bool ShowSku
    {
        get { return _ShowSku; }
        set { _ShowSku = value; }
    }

    private bool _ShowPrice = true;
    public bool ShowPrice
    {
        get { return _ShowPrice; }
        set { _ShowPrice = value; }
    }

    private bool _ShowSubscription = true;
    public bool ShowSubscription
    {
        get { return _ShowSubscription; }
        set { _ShowSubscription = value; }
    }

    private bool _ShowMSRP = true;
    public bool ShowMSRP
    {
        get { return _ShowMSRP; }
        set { _ShowMSRP = value; }
    }

    private bool _ShowPartNumber = false;
    /// <summary>
    ///  Indicates whether the Part/Model Number will be shown or not.
    /// </summary>
    [Personalizable, WebBrowsable]
    public bool ShowPartNumber
    {
        get { return _ShowPartNumber; }
        set { _ShowPartNumber = value; }
    }
    

    protected void Page_Init(object sender, System.EventArgs e)
    {
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _Product = ProductDataSource.Load(_ProductId);
        if (_Product != null)
        {
            //DISABLE PURCHASE CONTROLS BY DEFAULT
            AddToBasketButton.Visible = false;
            rowQuantity.Visible = false;

            //HANDLE SKU ROW
            trSku.Visible = (ShowSku && (_Product.Sku != string.Empty));
            if (trSku.Visible)
            {
                Sku.Text = _Product.Sku;
            }

            //HANDLE PART/MODEL NUMBER ROW
            trPartNumber.Visible = (ShowPartNumber && (_Product.ModelNumber != string.Empty));
            if (trPartNumber.Visible)
            {
                PartNumber.Text = _Product.ModelNumber;
            }

            //HANDLE REGPRICE ROW
            if (ShowMSRP)
            {
                LSDecimal msrpWithVAT = TaxHelper.GetShopPrice(_Product.MSRP, _Product.TaxCodeId);
                if (msrpWithVAT > 0)
                {
                    trRegPrice.Visible = true;
                    RegPrice.Text = msrpWithVAT.ToString("ulc");
                }
                else trRegPrice.Visible = false;
            }
            else trRegPrice.Visible = false;

            // HANDLE PRICES VISIBILITY
            if (ShowPrice)
            {
                if (!_Product.UseVariablePrice)
                {
                    trOurPrice.Visible = true;
                    trVariablePrice.Visible = false;
                }
                else
                {
                    trOurPrice.Visible = false;
                    trVariablePrice.Visible = true;
                    VariablePrice.Text = _Product.Price.ToString("F2");
                    string varPriceText = string.Empty;
                    Currency userCurrency = Token.Instance.User.UserCurrency;
                    LSDecimal userLocalMinimum = userCurrency.ConvertFromBase(_Product.MinimumPrice);
                    LSDecimal userLocalMaximum = userCurrency.ConvertFromBase(_Product.MaximumPrice);
                    if (userLocalMinimum > 0)
                    {
                        if (userLocalMaximum > 0)
                        {
                            varPriceText = string.Format("(between {0:ulcf} and {1:ulcf})", userLocalMinimum, userLocalMaximum);
                        }
                        else
                        {
                            varPriceText = string.Format("(at least {0:ulcf})", userLocalMinimum);
                        }
                    }
                    else if (userLocalMaximum > 0)
                    {
                        varPriceText = string.Format("({0:ulcf} maximum)", userLocalMaximum);
                    }
                    phVariablePrice.Controls.Add(new LiteralControl(varPriceText));
                }
            }

            //UPDATE QUANTITY LIMITS
            if ((_Product.MinQuantity > 0) && (_Product.MaxQuantity > 0))
            {
                string format = " (min {0}, max {1})";
                QuantityLimitsPanel.Controls.Add(new LiteralControl(string.Format(format, _Product.MinQuantity, _Product.MaxQuantity)));
                Quantity.MinValue = _Product.MinQuantity;
                Quantity.MaxValue = _Product.MaxQuantity;
            }
            else if (_Product.MinQuantity > 0)
            {
                string format = " (min {0})";
                QuantityLimitsPanel.Controls.Add(new LiteralControl(string.Format(format, _Product.MinQuantity)));
                Quantity.MinValue = _Product.MinQuantity;
            }
            else if (_Product.MaxQuantity > 0)
            {
                string format = " (max {0})";
                QuantityLimitsPanel.Controls.Add(new LiteralControl(string.Format(format, _Product.MaxQuantity)));
                Quantity.MaxValue = _Product.MaxQuantity;
            }
            if (_Product.MinQuantity > 0) Quantity.Text = _Product.MinQuantity.ToString();

            //BUILD PRODUCT ATTRIBUTES
            _SelectedOptions = ProductHelper.BuildProductOptions(_Product, phOptions);
            //BUILD PRODUCT CHOICES
            ProductHelper.BuildProductChoices(_Product, phOptions);
            //BUILD KIT OPTIONS
            _SelectedKitProducts = ProductHelper.BuildKitOptions(_Product, phKitOptions);

        }
        else
        {
            this.Controls.Clear();
        }
    }

    private void UpdateInventoryDetails(InventoryManagerData inv)
    {
        if ((inv.InventoryMode == InventoryMode.None) || (inv.AllowBackorder)) return;
        if (inv.InStock > 0)
        {
            string inStockformat = Token.Instance.Store.Settings.InventoryInStockMessage;
            string inStockMessage = string.Format(inStockformat, inv.InStock);
            InventoryDetailsPanel.Controls.Clear();
            InventoryDetailsPanel.Controls.Add(new LiteralControl(inStockMessage));

            // CALCULATE MAX VALUE FOR QUANTITY
            if (_Product.MaxQuantity > 0 && _Product.MaxQuantity < inv.InStock)
            {
                Quantity.MaxValue = _Product.MaxQuantity;
            }
            else Quantity.MaxValue = inv.InStock;
        }
        else
        {
            string outOfStockformat = Token.Instance.Store.Settings.InventoryOutOfStockMessage;
            string outOfStockMessage = string.Format(outOfStockformat, inv.InStock);
            InventoryDetailsPanel.Controls.Clear();
            InventoryDetailsPanel.Controls.Add(new LiteralControl(outOfStockMessage));
        }
    }

    private void HideAddToBasket()
    {
        AddToBasketButton.Visible = false;
        AddToWishlistButton.Visible = false;
        rowQuantity.Visible = false;
    }

    private void ShowAddToBasket()
    {
        AddToBasketButton.Visible = true;
        AddToWishlistButton.Visible = true;
        rowQuantity.Visible = true;
    }

    protected BasketItem GetBasketItem()
    {
        //GET THE QUANTITY
        int tempQuantity = AlwaysConvert.ToInt(Quantity.Text,1);
        if (tempQuantity < 1) return null;
		if (tempQuantity > System.Int16.MaxValue) tempQuantity = System.Int16.MaxValue;

        //RECALCULATE SELECTED KIT OPTIONS
        GetSelectedKitOptions();
        // DETERMINE THE OPTION LIST
        string optionList = ProductVariantDataSource.GetOptionList(_ProductId, _SelectedOptions, false);
        //CREATE THE BASKET ITEM WITH GIVEN OPTIONS
        Basket basket = Token.Instance.User.Basket;
        BasketItem basketItem = BasketItemDataSource.CreateForProduct(_ProductId, (short)tempQuantity, optionList, AlwaysConvert.ToList(",", _SelectedKitProducts), PaymentTypes.Initial);
        basketItem.BasketId = basket.BasketId;
        //ADD IN VARIABLE PRICE
        if (_Product.UseVariablePrice)
        {
            Currency userCurrency = Token.Instance.User.UserCurrency;
            decimal userLocalPrice = AlwaysConvert.ToDecimal(VariablePrice.Text);
            basketItem.Price = userCurrency.ConvertToBase(userLocalPrice);
        }
        ProductHelper.CollectProductTemplateInput(basketItem, this);
        return basketItem;
    }

    private bool ValidateVariablePrice()
    {
        if (!_Product.UseVariablePrice) return true;
        Currency userCurrency = Token.Instance.User.UserCurrency;
        LSDecimal userLocalPrice = AlwaysConvert.ToDecimal(VariablePrice.Text);
        LSDecimal price = userCurrency.ConvertToBase(userLocalPrice);
        bool priceValid = ((price >= _Product.MinimumPrice) && ((_Product.MaximumPrice == 0) || (price <= _Product.MaximumPrice)));
        if (!priceValid)
        {
            CustomValidator invalidPrice = new CustomValidator();
            invalidPrice.IsValid = false;
            invalidPrice.Text = "*";
            invalidPrice.ErrorMessage = "Price does not fall within the accepted range.";
            invalidPrice.ControlToValidate = "VariablePrice";
            invalidPrice.ValidationGroup = "AddToBasket";
            phVariablePrice.Controls.Add(invalidPrice);
        }
        return priceValid;
    }

    private bool AllProductOptionsSelected()
    {
        if (_SelectedOptions.Count != _Product.ProductOptions.Count)
        {
            phAddToBasketWarningOpt.Visible = true;
            return false;
        }
        else
        {
            phAddToBasketWarningOpt.Visible = false;
            return true;
        }
    }

    private int AvailableProductOptionsCount()
    {
        int count = 0;
        for (int i = 0; i < _Product.ProductOptions.Count; i++)
        {
            Option option = _Product.ProductOptions[i].Option;
            // GET THE COLLECTION OF OPTIONS THAT ARE AVAILABLE FOR THE CURRENT SELECTIONS
            OptionChoiceCollection availableChoices = OptionChoiceDataSource.GetAvailableChoices(_Product.ProductId, option.OptionId, _SelectedOptions);
            if (availableChoices.Count > 0) count++;
        }
        return count;
    }

    private bool RequiredKitOptionsSelected()
    {
        bool requiredKitOptionsSelected = ProductHelper.RequiredKitOptionsSelected(_Product, _SelectedKitProducts);
        if (requiredKitOptionsSelected)
        {
            phAddToBasketWarningKit.Visible = false;
            return true;
        }
        else
        {
            phAddToBasketWarningKit.Visible = true;
            return false;
        }
    }

    protected bool ValidateQuantity()
    {
        string optionList = ProductVariantDataSource.GetOptionList(_ProductId, _SelectedOptions, true);
        if (!string.IsNullOrEmpty(optionList))
        {
            int currentQuanity = AlwaysConvert.ToInt(Quantity.Text);
            if (currentQuanity == 0)
            {
                QuantityValidaor.ErrorMessage = "Please enter a valid quantity.";
                QuantityValidaor.IsValid = false;
                return false;
            }
            InventoryManagerData inv = InventoryManager.CheckStock(_ProductId, optionList, _SelectedKitProducts);
            if (inv.InventoryMode != InventoryMode.None && !inv.AllowBackorder && inv.InStock > 0 && currentQuanity > inv.InStock)
            {
                //Quantity.Text = inv.InStock.ToString();
                QuantityValidaor.ErrorMessage = String.Format(QuantityValidaor.ErrorMessage, inv.InStock);
                QuantityValidaor.IsValid = false;
                return false;
            }
        }
        return true;
    }

    protected void AddToBasketButton_Click(object sender, System.EventArgs e)
    {
        if (Page.IsValid && ValidateVariablePrice() && ValidateQuantity()
            && AllProductOptionsSelected() && RequiredKitOptionsSelected())
        {
            BasketItem basketItem = GetBasketItem();
            if (basketItem != null)
            {
                // DETERMINE IF THE LICENSE AGREEMENT MUST BE REQUESTED
                BasketItemLicenseAgreementCollection basketItemLicenseAgreements = new BasketItemLicenseAgreementCollection(basketItem, LicenseAgreementMode.OnAddToBasket);
                if ((basketItemLicenseAgreements.Count > 0))
                {
                    // THESE AGREEMENTS MUST BE ACCEPTED TO ADD TO CART
                    List<BasketItem> basketItems = new List<BasketItem>();
                    basketItems.Add(basketItem);
                    string guidKey = Guid.NewGuid().ToString("N");
                    Cache.Add(guidKey, basketItems, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 10, 0), System.Web.Caching.CacheItemPriority.NotRemovable, null);
                    string acceptUrl = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("~/Basket.aspx"));
                    string declineUrl = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Page.ResolveClientUrl(_Product.NavigateUrl)));
                    Response.Redirect("~/BuyWithAgreement.aspx?Items=" + guidKey + "&AcceptUrl=" + acceptUrl + "&DeclineUrl=" + declineUrl);
                }

                // THERE ARE NO AGREEMENTS, SO SAVE THIS ITEM INTO THE BASKET
                Basket basket = Token.Instance.User.Basket;
                basket.Items.Add(basketItem);
                basket.Save();
                
                //Determine if there are associated Upsell products
                if (basketItem.Product.GetUpsellProducts(basket).Count > 0)
                {
                    //redirect to upsell page
                    string returnUrl = NavigationHelper.GetEncodedReturnUrl();
                    Response.Redirect("~/ProductAccessories.aspx?ProductId=" + basketItem.ProductId + "&ReturnUrl=" + returnUrl);
                }

                // IF BASKET HAVE SOME VALIDATION PROBLEMS MOVE TO BASKET PAGE
                List<string> basketMessages;
                if (!basket.Validate(out basketMessages))
                {
                    Session.Add("BasketMessage", basketMessages);
                    Response.Redirect(NavigationHelper.GetBasketUrl());
                }

                //IF THERE IS NO REGISTERED BASKET CONTROL, WE MUST GO TO BASKET PAGE
                if (!PageHelper.HasBasketControl(this.Page)) Response.Redirect(NavigationHelper.GetBasketUrl());
            }
        }
    }

    protected void AddToWishlistButton_Click(object sender, System.EventArgs e)
    {
		if (Page.IsValid && ValidateVariablePrice() && ValidateQuantity()
            && AllProductOptionsSelected() && RequiredKitOptionsSelected())
        {
			BasketItem wishlistItem = GetBasketItem();
			if (wishlistItem != null)
			{
                // DETERMINE IF THE LICENSE AGREEMENT MUST BE REQUESTED
                BasketItemLicenseAgreementCollection basketItemLicenseAgreements = new BasketItemLicenseAgreementCollection(wishlistItem, LicenseAgreementMode.OnAddToBasket);
                if ((basketItemLicenseAgreements.Count > 0))
                {
                    // THESE AGREEMENTS MUST BE ACCEPTED TO ADD TO CART
                    List<BasketItem> basketItems = new List<BasketItem>();
                    basketItems.Add(wishlistItem);
                    string guidKey = Guid.NewGuid().ToString("N");
                    Cache.Add(guidKey, basketItems, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 10, 0), System.Web.Caching.CacheItemPriority.NotRemovable, null);
                    string acceptUrl = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("~/Members/MyWishlist.aspx"));
                    string declineUrl = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Page.ResolveClientUrl(_Product.NavigateUrl)));
                    Response.Redirect("~/BuyWithAgreement.aspx?Items=" + guidKey + "&AcceptUrl=" + acceptUrl + "&DeclineUrl=" + declineUrl + "&ToWishlist=True");
                }

				Wishlist wishlist = Token.Instance.User.PrimaryWishlist;
				wishlist.Items.Add(wishlistItem);
				wishlist.Save();
				Response.Redirect("~/Members/MyWishlist.aspx");
			}
		}
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        //GET THE SELECTED KIT OPTIONS
        GetSelectedKitOptions();
        string optionList = ProductVariantDataSource.GetOptionList(_ProductId, _SelectedOptions, true);
        //SET THE CURRENT CALCULATED PRICE
        OurPrice.Product = _Product;
        OurPrice.OptionList = optionList;
        OurPrice.SelectedKitProducts = _SelectedKitProducts;
        bool allProductOptionsSelected = (_SelectedOptions.Count == _Product.ProductOptions.Count);
        bool requiredKitOptionsSelected = ProductHelper.RequiredKitOptionsSelected(_Product, _SelectedKitProducts);
        InventoryManagerData inv = null;

        //UPDATE THE SKU FOR THE SELECTED PRODUCT VARIANT
        if (_Product.ProductOptions.Count > 0 && _SelectedOptions.Count > 0)
        {
            ProductVariant variant = ProductVariantDataSource.LoadForOptionList(_ProductId, optionList);
            if(variant != null) Sku.Text = variant.Sku;
        }

        //SHOW SUBSCRIPTIONS
        if (this.ShowPrice && this.ShowSubscription)
        {
            SubscriptionPlan sp = _Product.SubscriptionPlan;
            if (sp != null && sp.IsRecurring)
            {
                // GET THE RECURRING PAYMENT MESSAGE FOR THIS PRODUCT
                RecurringPaymentMessage.Text = ProductHelper.GetRecurringPaymentMessage(_Product.Price, _Product.TaxCodeId, sp);
                rowSubscription.Visible = true;
            }
        }

        MakerShop.Stores.Store store = Token.Instance.Store;

        if (store.EnableInventory && store.Settings.InventoryDisplayDetails
            && (_Product.InventoryMode != InventoryMode.None) && (!_Product.AllowBackorder))
        {
            if (allProductOptionsSelected && requiredKitOptionsSelected)
            {
                inv = InventoryManager.CheckStock(_ProductId, optionList, _SelectedKitProducts);
                UpdateInventoryDetails(inv);
            }
            else
            {
                InventoryDetailsPanel.Controls.Clear();
            }
        }

        HideAddToBasket();
        if (!_Product.DisablePurchase && !Token.Instance.Store.Settings.ProductPurchasingDisabled)
        {
            if (requiredKitOptionsSelected)
            {
                if (inv != null)
                {
                    if (inv.InventoryMode == InventoryMode.None || inv.InStock > 0 || inv.AllowBackorder)
                    {
                        ShowAddToBasket();
                    }
                }
                // IF NO VARIANT ARE AVAILABLE IN STOCK
                else if (_Product.InventoryMode == InventoryMode.Variant && AvailableProductOptionsCount() == 0)
                {
                    string outOfStockformat = Token.Instance.Store.Settings.InventoryOutOfStockMessage;
                    string outOfStockMessage = string.Format(outOfStockformat, 0);
                    InventoryDetailsPanel.Controls.Add(new LiteralControl(outOfStockMessage));
                }
                else
                {
                    ShowAddToBasket();
                }
            }
        }
    }

    protected void GetSelectedKitOptions()
    {
        _SelectedKitProducts = new List<int>();
        //COLLECT ANY KIT VALUES
        foreach (ProductKitComponent pkc in _Product.ProductKitComponents)
        {
            // FIND THE CONTROL
            KitComponent component = pkc.KitComponent;
            if (component.InputType == KitInputType.IncludedHidden)
            {
                foreach (KitProduct choice in component.KitProducts)
                {
                    _SelectedKitProducts.Add(choice.KitProductId);
                }
            }
            else
            {
                WebControl inputControl = (WebControl)PageHelper.RecursiveFindControl(this, component.UniqueId);
                if (inputControl != null)
                {
                    List<int> kitProducts = component.GetControlValue(inputControl);
                    foreach (int selectedKitProductId in kitProducts)
                    {
                        _SelectedKitProducts.Add(selectedKitProductId);
                    }
                }
            }
        }
    }
}
