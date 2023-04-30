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
using MakerShop.Utility;
using MakerShop.Payments;

public partial class Webparts_FeaturedProductsGrid : System.Web.UI.UserControl
{
    private string _Caption;
    private int _Size = 6;
    private int _Columns = -1;
    //HOLD THE STORE SETTING FOR DISABLE PURCHASE
    private bool _GlobalDisablePurchase = false;
    private bool _includeOutOfStockItems = false;

    private string _Orientation = "HORIZONTAL";
    private string _ThumbnailPosition = "LEFT"; // LEFT OR TOP

    /// <summary>
    /// Default is "TOP" , can be "TOP" or "LEFT"
    /// </summary>
    [Personalizable(), WebBrowsable()]
    public string ThumbnailPosition
    {
        get { return _ThumbnailPosition; }

        set
        {
            _ThumbnailPosition = value;
            _ThumbnailPosition = value.ToUpperInvariant();
            if ((_ThumbnailPosition != "TOP") && (_ThumbnailPosition != "LEFT")) _ThumbnailPosition = "TOP";

        }
    }

    [Personalizable(), WebBrowsable()]
    public string Orientation
    {
        get
        {
            return _Orientation;
        }

        set
        {
            _Orientation = value.ToUpperInvariant();
            if ((_Orientation != "HORIZONTAL") && (_Orientation != "VERTICAL")) _Orientation = "HORIZONTAL";
            if (_Orientation == "HORIZONTAL")
            {
                ProductList.RepeatColumns = Columns;
                ProductList.RepeatDirection = RepeatDirection.Horizontal;
                ProductList.ItemStyle.CssClass = "rowSeparator";
                ProductList.AlternatingItemStyle.CssClass = "";

            }
            else
            {
                ProductList.RepeatColumns = 1;
                ProductList.RepeatDirection = RepeatDirection.Vertical;

                // THERE SHOULD BE DIFFERENT CSS STYLE FOR ALTERNATE ITEMS
                ProductList.ItemStyle.CssClass = "ProductItemView";
                ProductList.AlternatingItemStyle.CssClass = "ProductItemViewOdd";
            }
        }
    }

    [Personalizable(), WebBrowsable()]
    public bool IncludeOutOfStockItems
    {
        get { return _includeOutOfStockItems; }
        set { _includeOutOfStockItems = value; }
    }

    [Personalizable(), WebBrowsable()]
    public string Caption
    {
        get { return _Caption; }
        set { _Caption = value; }
    }

    [Personalizable(), WebBrowsable()]
    public int Size
    {
        get { return _Size; }
        set { _Size = value; }
    }

    [Personalizable(), WebBrowsable()]
    public int Columns
    {
        get {
            if (_Columns < 0) return ProductList.RepeatColumns;
            return _Columns;
        }
        set { 
            _Columns = value;
            if (Orientation == "HORIZONTAL") ProductList.RepeatColumns = Columns;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        _GlobalDisablePurchase = Token.Instance.Store.Settings.ProductPurchasingDisabled;
        if (!string.IsNullOrEmpty(this.Caption)) CaptionLabel.Text = this.Caption;
        ProductList.RepeatColumns = this.Columns;
        ProductList.DataSource = ProductDataSource.GetRandomFeaturedProducts(0, true,IncludeOutOfStockItems, this.Size);
        ProductList.DataBind();
    }

    protected void ProductList_ItemDataBound(object sender, System.Web.UI.WebControls.DataListItemEventArgs e)
    {
        if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
        {
            Product product = (Product)e.Item.DataItem;
            //thumbnail.ImageUrl = "~/images/thumbs/ProductThumbnail.gif";
            //ADD IN STOCK STATUS
            Label stockStatus = (Label)e.Item.FindControl("StockStatus");
            if (stockStatus != null) stockStatus.Text = "In Stock";
            
        }
        else if (e.Item.ItemType == ListItemType.Separator)
        {
            //CHECK IF WE ARE AT THE END OF THE ROW
            int tempIndex = (e.Item.ItemIndex + 1);
            if ((tempIndex % ProductList.RepeatColumns) == 0)
            {
                //END OF ROW DETECTED, HIDE SEPARATOR
                Trace.Write("Separator Index: " + e.Item.ItemIndex.ToString());
                e.Item.Controls.Clear();
                e.Item.CssClass = string.Empty;
                //e.Item.Parent.Controls.Remove(e.Item);
                //e.Item.Controls.Clear();
            }
        }

        if (ThumbnailPosition == "LEFT")
        {
            Literal SingleRowLiteral = PageHelper.RecursiveFindControl(e.Item, "SingleRowLiteral") as Literal;
            Literal TwoRowsLiteral = PageHelper.RecursiveFindControl(e.Item, "TwoRowsLiteral") as Literal;

            if (SingleRowLiteral != null && TwoRowsLiteral != null)
            {
                SingleRowLiteral.Visible = true;
                TwoRowsLiteral.Visible = false;
            }
        }
    }

    protected string GetThumbnailUrl(object thumbnailUrl)
    {
        if (!string.IsNullOrEmpty((string)thumbnailUrl)) return (string)thumbnailUrl;
        return "~/images/thumbs/ProductThumbnail.gif";
    }

    protected string GetPrice(object dataItem)
    {
        Product product = (Product)dataItem;
        ProductCalculator pc = ProductCalculator.LoadForProduct(product.ProductId, 1, string.Empty, string.Empty, PaymentTypes.Initial);
        return string.Format("{0:ulc}", pc.Price);
    }

    protected string GetRatingImage(object dataItem)
    {
        return NavigationHelper.GetRatingImage(((Product)dataItem).Rating);
    }
   
    protected void BuyNowButton_Click(object sender, EventArgs e)
    {
        ImageButton button = (ImageButton)sender;
        int productId = AlwaysConvert.ToInt(button.CommandArgument);
        Product product = ProductDataSource.Load(productId);
        if (product != null)
        {
            if (product.HasChoices)
            {
                //CANT ADD DIRECTLY TO BASKET, SEND TO MORE INFO
                Response.Redirect(product.NavigateUrl);
            }
            BasketItem basketItem = GetBasketItem(productId);
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
                    string declineUrl = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Request.Url.ToString()));
                    Response.Redirect("~/BuyWithAgreement.aspx?Items=" + guidKey + "&AcceptUrl=" + acceptUrl + "&DeclineUrl=" + declineUrl);
                }
                Basket basket = Token.Instance.User.Basket;
                basket.Items.Add(basketItem);
                basket.Save();
                basket.Package();
                basket.Combine();
            }
            Response.Redirect("~/Basket.aspx");
        }
    }

    private BasketItem GetBasketItem(int productId)
    {
        BasketItem basketItem = BasketItemDataSource.CreateForProduct(productId, 1, PaymentTypes.Initial);
        return basketItem;
    }

}
