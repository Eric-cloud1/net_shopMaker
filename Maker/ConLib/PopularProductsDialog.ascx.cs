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
using MakerShop.Products;
using MakerShop.Orders;
using MakerShop.Utility;
using MakerShop.Payments;

public partial class Webparts_PopularProductsDialog : System.Web.UI.UserControl
{
    private string _Caption = "Top Sellers";
    private int _MaxItems = 3;
    private string _Orientation = "HORIZONTAL";
    private int _Columns = 3;
    private string _ThumbnailPosition = "TOP"; // LEFT OR TOP

    /// <summary>
    /// Default is "TOP" , can be "TOP" or "LEFT"
    /// </summary>
    [Personalizable(), WebBrowsable()]
    public string ThumbnailPosition
    {
        get { return _ThumbnailPosition; }
        set { 
            _ThumbnailPosition = value;
            _ThumbnailPosition = value.ToUpperInvariant();
            if ((_ThumbnailPosition != "TOP") && (_ThumbnailPosition != "LEFT")) _ThumbnailPosition = "TOP";            

        }
    }

    /// <summary>
    /// Default is 3 columns, Only for HORIZONTAL Orientation
    /// </summary>
    [Personalizable(), WebBrowsable()]
    public int Columns
    {
        get { return _Columns; }
        set { 
            _Columns = value;
            if (Orientation == "HORIZONTAL") ProductList.RepeatColumns = Columns;
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
                
            }else{
                ProductList.RepeatColumns = 1;
                ProductList.RepeatDirection = RepeatDirection.Vertical;

                // THERE SHOULD BE DIFFERENT CSS STYLE FOR ALTERNATE ITEMS
                ProductList.ItemStyle.CssClass = "ProductItemView";
                ProductList.AlternatingItemStyle.CssClass = "ProductItemViewOdd";
            }
        }
    }

    [Personalizable(), WebBrowsable()]
    public string Caption
    {
        get { return _Caption; }
        set { _Caption = value; }
    }

    [Personalizable(), WebBrowsable()]
    public int MaxItems
    {
        get { return _MaxItems; }
        set { _MaxItems = value; }
    }

    protected void ProductList_ItemDataBound(object sender, DataListItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            Product product = (Product)e.Item.DataItem;
            Image thumbnail = PageHelper.RecursiveFindControl(e.Item, "Thumbnail") as Image;
            if (thumbnail != null)
            {
                if (!string.IsNullOrEmpty(product.ThumbnailUrl))
                {
                    thumbnail.ImageUrl = product.ThumbnailUrl;
                }
                else
                {
                    thumbnail.Visible = false;
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
    }

    protected void ProductList_ItemCommand(object source, DataListCommandEventArgs e)
    {
        if (e.CommandName == "AddToCart")
        {
            int productId = AlwaysConvert.ToInt(e.CommandArgument);
            BasketItem basketItem = BasketItemDataSource.CreateForProduct(productId, 1, PaymentTypes.Initial);
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
                Response.Redirect("~/Basket.aspx");
            }
        }
    }

    protected bool ShowAddToCart(object dataItem)
    {
        Product product = (Product)dataItem;
        return ((product.ProductOptions.Count == 0) && (product.KitStatus != KitStatus.Master));
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        int prefferedCategoryId = PageHelper.GetCategoryId();
        List<Product> products = ProductDataSource.GetPopularProducts(this.MaxItems, prefferedCategoryId);
        if (products != null && products.Count > 0)
        {
            CaptionLabel.Text = this.Caption;
            ProductList.DataSource = products;
            ProductList.DataBind();
        }
        else
        {
            this.Visible = false;
        }
    }

    protected string GetThumbnailUrl(object thumbnailUrl)
    {
        if (!string.IsNullOrEmpty((string)thumbnailUrl)) return (string)thumbnailUrl;
        return "~/images/thumbs/ProductThumbnail.gif";
    }
}
