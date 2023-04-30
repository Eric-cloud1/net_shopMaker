using System;
using System.Collections.Generic;

using MakerShop.Common;
using MakerShop.DigitalDelivery;
using MakerShop.Orders;
using MakerShop.Products;
using MakerShop.Utility;
using MakerShop.Payments;

public partial class ConLib_AddToCartLink : System.Web.UI.UserControl
{
    private int _ProductId;
    public int ProductId
    {
        get { return _ProductId; }
        set { _ProductId = value; }
    }

    public Product Product
    {        
        set {   
            if( value != null)
            _ProductId = value.ProductId; 
        }
    }

    public String ProductName
    {
        get { 
            Product product = ProductDataSource.Load(_ProductId);
            if (product != null)
            {                
                return product.Name;
            }
            else return String.Empty;
        }
    }

    public bool ShowImage
    {
        get { return LI.Visible; }
        set { LI.Visible = value; }
    }

    public string LinkText
    {
        get { return LT.Text; }
        set { LT.Text = value; }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        LoadCustomViewState();
        //MAKE SURE THIS CODE ONLY FIRES ONCE IN THIS CONTEXT
        string key = "AddToCartLink_" + AC.UniqueID;
        if (Request.Form["__EVENTTARGET"] == AC.UniqueID)
        {
            string value = (string)Context.Items[key];
            if (string.IsNullOrEmpty(value))
            {
                AddToCart();
                Context.Items[key] = "ADDED";
            }
        }
    }

    private void LoadCustomViewState()
    {
        if (Page.IsPostBack)
        {
            UrlEncodedDictionary customViewState = new UrlEncodedDictionary(EncryptionHelper.DecryptAES(Request.Form[VS.UniqueID]));
            _ProductId = AlwaysConvert.ToInt(customViewState.TryGetValue("P"));
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        Product product = ProductDataSource.Load(_ProductId);
        if ((!Token.Instance.Store.Settings.ProductPurchasingDisabled) && (product != null) && (!product.DisablePurchase) && !product.HasChoices && !product.UseVariablePrice)
        {
            LT.Text = string.Format(LT.Text, product.Name);
            LI.AlternateText = string.Format(LI.AlternateText, product.Name);
        }
        else
        {
            AC.Visible = false;
        }

        MoreDetailsLink.Visible = !AC.Visible;
        if (MoreDetailsLink.Visible) MoreDetailsLink.NavigateUrl = product.NavigateUrl;
        SaveCustomViewState();
    }

    private void SaveCustomViewState()
    {
        UrlEncodedDictionary customViewState = new UrlEncodedDictionary();
        customViewState["P"] = _ProductId.ToString();
        customViewState["SALT"] = StringHelper.RandomString(6);
        VS.Value = EncryptionHelper.EncryptAES(customViewState.ToString());
    }

    protected void AddToCart()
    {
        //GET THE PRODUCT ID FROM THE URL
        Product product = ProductDataSource.Load(_ProductId);
        if (product != null)
        {
            string lastShoppingUrl = NavigationHelper.GetLastShoppingUrl();
            if (product.HasChoices || product.UseVariablePrice)
            {
                //CANT ADD DIRECTLY TO BASKET, SEND TO MORE INFO
                Response.Redirect(product.NavigateUrl);
            }
            BasketItem basketItem = BasketItemDataSource.CreateForProduct(_ProductId, 1, PaymentTypes.Initial);
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
                    string declineUrl = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Page.ResolveClientUrl(product.NavigateUrl)));
                    Response.Redirect("~/BuyWithAgreement.aspx?Items=" + guidKey + "&AcceptUrl=" + acceptUrl + "&DeclineUrl=" + declineUrl);
                }

                //ADD ITEM TO BASKET
                Basket basket = Token.Instance.User.Basket;
                basket.Items.Add(basketItem);
                basket.Package(true);
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
}