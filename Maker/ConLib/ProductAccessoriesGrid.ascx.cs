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

public partial class Webparts_ProductAccessoriesGrid : System.Web.UI.UserControl
{
    private int _ProductId = 0;
    private Product _Product = null;

    private string _Caption;
    private int _Size = 6;
    private int _Columns = -1;
    //HOLD THE STORE SETTING FOR DISABLE PURCHASE
    private bool _GlobalDisablePurchase = false;

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
        set { _Columns = value; }
    }

    protected void Page_Init(object sender, System.EventArgs e)
    {
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _Product = ProductDataSource.Load(_ProductId);
        if (_Product == null)
        {
			Response.Redirect("~/Default.aspx");
		}
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        _GlobalDisablePurchase = Token.Instance.Store.Settings.ProductPurchasingDisabled;
        CaptionLabel.Text = string.Format(CaptionLabel.Text,_Product.Name);
        InstructionText.Text = string.Format(InstructionText.Text, _Product.Name);
        KeepShoppingLink.NavigateUrl = NavigationHelper.GetReturnUrl(NavigationHelper.GetLastShoppingUrl());
        ProductList.RepeatColumns = this.Columns;
        ProductList.DataSource = _Product.GetUpsellProducts(true);
        ProductList.DataBind();
    }

    protected void ProductList_ItemDataBound(object sender, System.Web.UI.WebControls.DataListItemEventArgs e)
    {
        if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
        {
            UpsellProduct upProduct = (UpsellProduct)e.Item.DataItem;
            Product product = upProduct.ChildProduct;
            //thumbnail.ImageUrl = "~/images/thumbs/ProductThumbnail.gif";
            //ADD IN STOCK STATUS
            Label stockStatus = (Label)e.Item.FindControl("StockStatus");
            if (stockStatus != null) stockStatus.Text = "In Stock";

            // HANDLE DISABLE PURCHASING: HIDE ADD TO CART LINK
            if (_GlobalDisablePurchase || product.DisablePurchase)
            {
                HyperLink addToCartLink = PageHelper.RecursiveFindControl(e.Item, "BuyNowLink") as HyperLink;
                addToCartLink.Visible = false;
            }
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
    }

    protected bool HasThumbnail(object dataItem)
    {
        UpsellProduct upProduct = (UpsellProduct)dataItem;
        Product product = upProduct.ChildProduct;
        return !string.IsNullOrEmpty(product.ThumbnailUrl);
    }

    protected string GetThumbnailUrl(object dataItem)
    {
        UpsellProduct upProduct = (UpsellProduct)dataItem;
        Product product = upProduct.ChildProduct;
        string thumbnailUrl = product.ThumbnailUrl;

        if (!string.IsNullOrEmpty(thumbnailUrl)) return thumbnailUrl;
        return "~/images/thumbs/ProductThumbnail.gif";
    }

    protected string GetThumbnailAltText(object dataItem)
    {
        UpsellProduct upProduct = (UpsellProduct)dataItem;
        Product product = upProduct.ChildProduct;
        return product.ThumbnailAltText;
    }

    protected string GetPrice(object dataItem)
    {
        UpsellProduct upProduct = (UpsellProduct)dataItem;
        Product product = upProduct.ChildProduct;
        ProductCalculator pc = ProductCalculator.LoadForProduct(product.ProductId, 1, string.Empty, string.Empty, PaymentTypes.Initial);
        return string.Format("{0:ulc}", pc.Price);
    }

    protected string GetName(object dataItem)
    {
        UpsellProduct upProduct = (UpsellProduct)dataItem;
        Product product = upProduct.ChildProduct;
        return product.Name;
    }

    protected string GetNavigateUrl(object dataItem)
    {
        UpsellProduct upProduct = (UpsellProduct)dataItem;
        Product product = upProduct.ChildProduct;
        return product.NavigateUrl;
    }

    protected string GetManufacturerName(object dataItem)
    {
        UpsellProduct upProduct = (UpsellProduct)dataItem;
        Product product = upProduct.ChildProduct;
        if (product.Manufacturer != null)
        {
            return product.Manufacturer.Name;
        }
        else return String.Empty;
    }
    
    protected string GetRatingImage(object dataItem)
    {
        UpsellProduct upProduct = (UpsellProduct)dataItem;
        Product product = upProduct.ChildProduct;
        return NavigationHelper.GetRatingImage(product.Rating);
    }

    protected string GetMoreAboutText(object dataItem)
    {
        UpsellProduct upProduct = (UpsellProduct)dataItem;
        Product product = upProduct.ChildProduct;
        return string.Format("Find out more about {0}",product.Name);
    }
}
