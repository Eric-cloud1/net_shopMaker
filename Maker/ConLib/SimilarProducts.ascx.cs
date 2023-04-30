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

public partial class ConLib_SimilarProducts : System.Web.UI.UserControl
{
    private string _Caption = "Similar Products";    
    private string _Orientation = "HORIZONTAL";
    private int _Columns = 3;

    /// <summary>
    /// Default is 3 columns, Only for HORIZONTAL Orientation
    /// </summary>
    [Personalizable(), WebBrowsable()]
    public int Columns
    {
        get { return _Columns; }
        set { _Columns = value; }
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
        }
    }

    [Personalizable(), WebBrowsable()]
    public string Caption
    {
        get { return _Caption; }
        set { _Caption = value; }
    }

    protected void ProductList_ItemDataBound(object sender, DataListItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            Product product = ((RelatedProduct)e.Item.DataItem).ChildProduct;
            Image thumbnail = PageHelper.RecursiveFindControl(e.Item, "Thumbnail") as Image;
            if (thumbnail != null)
            {
                if (!string.IsNullOrEmpty(product.ThumbnailUrl))
                {
                    thumbnail.ImageUrl = product.ThumbnailUrl;
                    thumbnail.Attributes.Add("hspace", "2");
                    thumbnail.Attributes.Add("vspace", "2");
                }
                else
                {
                    thumbnail.Visible = false;
                }
            }
            //TODO: SHOW/HIDE ADD TO CART
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
                    Session["BuyWithAgreement"] = basketItems;
                    string acceptUrl = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("~/Basket.aspx"));
                    string declineUrl = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Request.Url.ToString()));
                    Response.Redirect(("~/BuyWithAgreement.aspx?AcceptUrl=" + (acceptUrl + ("&DeclineUrl=" + Server.UrlEncode(declineUrl)))));
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
        Product product = ProductDataSource.Load(PageHelper.GetProductId());
        if (product != null && product.RelatedProducts.Count > 0)
        {
            CaptionLabel.Text = this.Caption;
            if (this.Columns > product.RelatedProducts.Count) this.Columns = product.RelatedProducts.Count;
            //UPDATE THE WIDTH FOR HORIZONTAL DISPLAYS
            bool isHorizontal = (_Orientation == "HORIZONTAL");
            if (isHorizontal) _Width = string.Format("{0:F0}%", (100 / this.Columns));
            else _Width = "100%";
            List<Product> products = new List<Product>();
            foreach (RelatedProduct rp in product.RelatedProducts)
                products.Add(rp.ChildProduct);
            ItemsRepeater.DataSource = products;
            ItemsRepeater.DataBind();
        }
        else
        {
            this.Visible = false;
        }
    }

    public string GetThumbnail(object dataItem)
    {
        Product p = (Product)dataItem;
        if (!string.IsNullOrEmpty(p.ThumbnailUrl))
        {
            string altText = String.IsNullOrEmpty(p.ThumbnailAltText) ? p.Name : p.ThumbnailAltText;
            altText = altText.Replace("\"", "&quot;");
            return string.Format("<a href=\"{0}\"><img src=\"{1}\" border=\"0\" alt=\"{2}\" /></a><br />", Page.ResolveClientUrl(p.NavigateUrl), Page.ResolveClientUrl(p.ThumbnailUrl), altText);
        }
        return string.Empty;
    }

    public string GetRow(bool open, bool horizontal)
    {
        bool isHorizontalMode = (_Orientation == "HORIZONTAL");
        if (!(isHorizontalMode ^ horizontal))
        {
            if (open) return "<tr>";
            return "</tr>";
        }
        return string.Empty;
    }

    private string _Width = string.Empty;
    protected string Width
    {
        get { return _Width; }
    }
}