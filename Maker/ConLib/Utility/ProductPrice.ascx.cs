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
using MakerShop.Products;
using MakerShop.Payments;
using MakerShop.Utility;
using MakerShop.Orders;
using System.Collections.Generic;
using MakerShop.Catalog;
using MakerShop.DigitalDelivery;
using MakerShop.Taxes;

public partial class ConLib_Utility_ProductPrice : System.Web.UI.UserControl
{
    private int _ProductId = 0;
    private Product _Product = null;
    private string _OptionList = string.Empty;
    private List<int> _SelectedKitProducts = new List<int>();
    private bool _EnableDefaultKitProducts = true;
    private bool _AddedItem = false;
    // FORMAT SPECIFIERS FOR FIXED (VISIBLE) PRICE
    private bool _ShowRetailPrice = false;
    private string _RetailPriceFormat = "<span class=\"msrp\">{0:ulc}</span> ";
    private string _PriceFormat = "{0:ulc}";
    private string _BasePriceFormat = "<span class=\"msrp\">{0:ulc}</span> ";
    private string _SpecialPriceFormat = "{0:ulc} ends {1:d}";
    // FORMAT SPECIFIES FOR HIDDEN (POPUP) PRICE
    private bool _ShowPopupRetailPrice = true;
    private string _PopupRetailPriceLabel = "<b>List Price:</b>";
    private string _PopupRetailPriceFormat = "{0:ulc}";
    private string _PopupPriceLabel = "<b>Our Price:</b>";
    private string _PopupPriceFormat = "{0:ulc}";
    private string _PopupBasePriceLabel = "<b>Regular Price:</b>";
    private string _PopupBasePriceFormat = "{0:ulc}";
    private string _PopupSpecialPriceLabel = "<b>Sale Price:</b>";
    private string _PopupSpecialPriceFormat = "{0:ulc} ends {1:d}";
    private string _PopupAmountSavedLabel = "<b>You Save:</b>";
    private string _PopupAmountSavedFormat = "{0:ulc} ({1:F0}%)";

    /* properties that specify what product to show price for */
    public int ProductId
    {
        set
        {
            _ProductId = AlwaysConvert.ToInt(value);
            _Product = ProductDataSource.Load(_ProductId);
        }
    }

    public object Product
    {
        set
        {
            _Product = value as Product;
            if (_Product == null)
            {
                CatalogNode node = value as CatalogNode;
                if (node != null) _Product = node.ChildObject as Product;
            }
            if (_Product != null) _ProductId = _Product.ProductId;
            else _ProductId = 0;
        }
    }

    public string OptionList
    {
        set { _OptionList = value; }
    }
    
    public List<int> SelectedKitProducts
    {
        set { _SelectedKitProducts = value; }
    }

    public bool EnableDefaultKitProducts
    {
        set { _EnableDefaultKitProducts = value; }
        get { return _EnableDefaultKitProducts; }
    }

    /* display properties */
    public string ShowPriceLinkText
    {
        get { return ShowPriceLink.Text; }
        set { ShowPriceLink.Text = value; }
    }

    public bool ShowRetailPrice
    {
        get { return _ShowRetailPrice; }
        set { _ShowRetailPrice = value; }
    }

    public string RetailPriceFormat
    {
        get { return _RetailPriceFormat; }
        set { _RetailPriceFormat = value; }
    }

    public string PriceFormat
    {
        get { return _PriceFormat; }
        set { _PriceFormat = value; }
    }

    public string BasePriceFormat
    {
        get { return _BasePriceFormat; }
        set { _BasePriceFormat = value; }
    }

    public string SpecialPriceFormat
    {
        get { return _SpecialPriceFormat; }
        set { _SpecialPriceFormat = value; }
    }

    // PROPERTIES FOR HIDDEN (POPUP) PRICE
    public bool ShowPopupRetailPrice
    {
        get { return _ShowPopupRetailPrice; }
        set { _ShowPopupRetailPrice = value; }
    }

    public string PopupRetailPriceLabel
    {
        get { return _PopupRetailPriceLabel; }
        set { _PopupRetailPriceLabel = value; }
    }

    public string PopupRetailPriceFormat
    {
        get { return _PopupRetailPriceFormat; }
        set { _PopupRetailPriceFormat = value; }
    }

    public string PopupPriceLabel
    {
        get { return _PopupPriceLabel; }
        set { _PopupPriceLabel = value; }
    }

    public string PopupPriceFormat
    {
        get { return _PopupPriceFormat; }
        set { _PopupPriceFormat = value; }
    }

    public string PopupBasePriceLabel
    {
        get { return _PopupBasePriceLabel; }
        set { _PopupBasePriceLabel = value; }
    }

    public string PopupBasePriceFormat
    {
        get { return _PopupBasePriceFormat; }
        set { _PopupBasePriceFormat = value; }
    }

    public string PopupSpecialPriceLabel
    {
        get { return _PopupSpecialPriceLabel; }
        set { _PopupSpecialPriceLabel = value; }
    }

    public string PopupSpecialPriceFormat
    {
        get { return _PopupSpecialPriceFormat; }
        set { _PopupSpecialPriceFormat = value; }
    }

    public string PopupAmountSavedLabel
    {
        get { return _PopupAmountSavedLabel; }
        set { _PopupAmountSavedLabel = value; }
    }

    public string PopupAmountSavedFormat
    {
        get { return _PopupAmountSavedFormat; }
        set { _PopupAmountSavedFormat = value; }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        LoadCustomViewState();
        //if (Request["__EVENTTARGET"] == AddToCartButton.UniqueID) AddToCart();

        string scrollScript = @"var _lastWin;
function initPricePopup(divid)
{
    _lastWin = divid;
    reposPricePopup();
    window.onscroll = reposPricePopup;
}
function reposPricePopup()
{
    var div = document.getElementById(_lastWin);
    var st = document.body.scrollTop;
    if (st == 0) {
        if (window.pageYOffset) st = window.pageYOffset;
        else st = (document.body.parentElement) ? document.body.parentElement.scrollTop : 0;
    }
    div.style.top = 150 + st + ""px"";
}";

        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "PricePopup", scrollScript, true);
        AddToCartButton.OnClientClick = String.Format("__doPostBack('{0}','{1}')", AddToCartButton.UniqueID, "");
        ShowPriceLink.OnClientClick = "initPricePopup('" + PricePopup.ClientID + "');document.getElementById('" + PricePopup.ClientID + "').style.display='block';return false;";
        ClosePopUpLink.OnClientClick = "document.getElementById('" + PricePopup.ClientID + "').style.display='none';return false;";
    }

    //save these from viewstate to process add request
    private int _AddProductId;
    private string _AddOptionList;
    private List<int> _AddKitProducts = new List<int>();
    private void LoadCustomViewState()
    {
        if (Page.IsPostBack)
        {
            string vsContent = Request.Form[VS.UniqueID];
            string decodedContent = EncryptionHelper.DecryptAES(vsContent);
            UrlEncodedDictionary customViewState = new UrlEncodedDictionary(decodedContent);
            _AddProductId = AlwaysConvert.ToInt(customViewState.TryGetValue("PID"));
            _AddOptionList = customViewState.TryGetValue("OL");
            int[] skp = AlwaysConvert.ToIntArray(customViewState.TryGetValue("SKP"));
            if (skp != null && skp.Length > 0)
            {
                _AddKitProducts.AddRange(skp);
            }
        }
    }

    protected void AddToCart_Click(object sender, EventArgs e)
    {
        //SOMETHING IS CAUSING THE ADD TO FIRE TWICE
        if (!_AddedItem)
        {
            //USE VALUES FROM VS, CURRENT PROPERTIES MAY NOT BE CORRECT
            BasketItem basketItem = BasketItemDataSource.CreateForProduct(_AddProductId, 1, _AddOptionList, AlwaysConvert.ToList(",", _AddKitProducts), PaymentTypes.Initial);
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
                    Response.Redirect("~/Basket.aspx");
                }

                //LOOK FOR A MINIBASKET CONTROL ON THIS PAGE
                if (!PageHelper.HasBasketControl(this.Page)) Response.Redirect(NavigationHelper.GetBasketUrl());
            }
            _AddedItem = true;
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (_Product != null)
        {
            if (!_Product.UseVariablePrice)
            {
                // UPDATE THE INCLUDED KITPRODUCTS (Included-Hidden and Included-Shown)
                if (_SelectedKitProducts.Count == 0 && this.EnableDefaultKitProducts) UpdateIncludedKitOptions();
                ProductCalculator pcalc = ProductCalculator.LoadForProduct(_Product.ProductId, 1, _OptionList, AlwaysConvert.ToList(",", _SelectedKitProducts), Token.Instance.UserId, true, PaymentTypes.Initial);

                //IF REQUIRED MAKE ADJUSTMENTS TO DISPLAYED PRICE TO INCLUDE VAT
                LSDecimal basePriceWithVAT = TaxHelper.GetShopPrice(_Product.Price, _Product.TaxCodeId);
                LSDecimal priceWithVAT = pcalc.PriceWithTax;
                LSDecimal msrpWithVAT = TaxHelper.GetShopPrice(_Product.MSRP, _Product.TaxCodeId);
                if (!_Product.HidePrice)
                {
                    //PRICE IS VISIBLE, NO POPUP
                    phPricePopup.Visible = false;
                    //SHOW RETAIL PRICE IF INDICATED
                    if (_ShowRetailPrice && msrpWithVAT > 0 && !_Product.UseVariablePrice)
                    {
                        RetailPrice1.Text = string.Format(_RetailPriceFormat, msrpWithVAT);
                    }
                    else RetailPrice1.Text = string.Empty;
                    //SHOW THE PRICE
                    if (pcalc.AppliedSpecial != null && pcalc.AppliedSpecial.EndDate != DateTime.MinValue)
                    {
                        // SHOW THE BASE PRICE AND SPECIAL PRICE
                        RetailPrice1.Text = string.Format(_BasePriceFormat, basePriceWithVAT);
                        Price.Text = string.Format(_SpecialPriceFormat, priceWithVAT, pcalc.AppliedSpecial.EndDate);
                    }
                    else
                    {
                        Price.Text = string.Format(_PriceFormat, priceWithVAT);
                    }
                }
                else
                {
                    // HIDDEN PRICE, USE POPUP
                    Price.Visible = false;
                    ProductName.Text = _Product.Name;
                    // THESE VARIABLES WILL TRACK SAVINGS OFF LISTED OR REGULAR PRICE
                    LSDecimal amountSaved = 0;
                    decimal percentSaved = 0;
                    // DETERMINE IF A SALE OR SPECIAL PRICE IS APPLIED
                    bool salePriceEffective = (pcalc.AppliedSpecial != null && pcalc.AppliedSpecial.EndDate != DateTime.MinValue);
                    // DETERMINE IF MSRP IS AVAILABLE FOR DISPLAY
                    if (_ShowRetailPrice && msrpWithVAT > 0 && !salePriceEffective)
                    {
                        trRetailPrice.Visible = true;
                        HiddenRetailPriceLabel.Text = PopupRetailPriceLabel;
                        HiddenRetailPrice.Text = string.Format(PopupRetailPriceFormat, msrpWithVAT);
                        // CALCULATE AMOUNT SAVED OVER MSRP
                        amountSaved = (msrpWithVAT - priceWithVAT);
                        percentSaved = ((decimal)(amountSaved / msrpWithVAT)) * 100;
                    }
                    else trRetailPrice.Visible = false;
                    // CHECK IF A SALE PRICE IS APPLIED
                    if (salePriceEffective)
                    {
                        // SPECIAL APPLIED, SHOW THE PRODUCT BASE PRICE AND THE SPECIAL PRICE
                        trSpecialPrice.Visible = true;
                        HiddenPriceLabel.Text = PopupBasePriceLabel;
                        HiddenPrice.Text = string.Format(PopupBasePriceFormat, basePriceWithVAT);
                        SpecialPriceLabel.Text = PopupSpecialPriceLabel;
                        SpecialPrice.Text = string.Format(PopupSpecialPriceFormat, priceWithVAT, pcalc.AppliedSpecial.EndDate);
                        // CALCULATE AMOUNT SAVED WITH SALE PRICE
                        amountSaved = (basePriceWithVAT - priceWithVAT);
                        percentSaved = ((decimal)(amountSaved / basePriceWithVAT)) * 100;
                    }
                    else
                    {
                        // NO SPECIAL, SO JUST SHOW THE CALCULATED PRICE
                        HiddenPriceLabel.Text = PopupPriceLabel;
                        HiddenPrice.Text = string.Format(PopupPriceFormat, priceWithVAT);
                    }
                    // SEE IF WE HAVE AN AMOUNT SAVED VALUE
                    if (amountSaved > 0)
                    {
                        trAmountSaved.Visible = true;
                        AmountSavedLabel.Text = PopupAmountSavedLabel;
                        AmountSaved.Text = string.Format(PopupAmountSavedFormat, amountSaved, percentSaved);
                    }
                    else trAmountSaved.Visible = false;
                    //DO NOT GIVE ADD BUTTON FOR KITTED PRODUCTS
                    AddToCartButton.Visible = (_Product.KitStatus != KitStatus.Master);
                }
            }
            else
            {
                //DO NOT DISPLAY THIS CONTROL IF THE PRODUCT HAS VARIABLE PRICE
                this.Controls.Clear();
            }
        }
        else
        {
            //DO NOT DISPLAY THIS CONTROL IF THE PRODUCT IS UNAVAILABLE
            this.Controls.Clear();
        }
        SaveCustomViewState();
    }

    private void SaveCustomViewState()
    {
        UrlEncodedDictionary customViewState = new UrlEncodedDictionary();
        customViewState["PID"] = _ProductId.ToString();
        customViewState["OL"] = string.Empty + _OptionList;
        customViewState["SKP"] = GetCommaDelimitedKitProducts();
        VS.Value = EncryptionHelper.EncryptAES(customViewState.ToString());
    }

    private string GetCommaDelimitedKitProducts()
    {
        if (_SelectedKitProducts.Count == 0) return string.Empty;
        List<string> stringArray = new List<string>();
        foreach (int item in _SelectedKitProducts) stringArray.Add(item.ToString());
        return string.Join(",", stringArray.ToArray());
    }

    /// <summary>
    /// Update the SelectedKitProducts by including the hidden and shown included parts.
    /// These are not included on pages other then category pages.
    /// </summary>
    protected void UpdateIncludedKitOptions()
    {
        //COLLECT ANY KIT VALUES
        Kit kit = _Product.Kit;
        List<KitProduct> defaultProducts = kit.GetDefaultKitProducts();
        foreach (KitProduct p in defaultProducts)
        {
            _SelectedKitProducts.Add(p.KitProductId);
        }
    }
}