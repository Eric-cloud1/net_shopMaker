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
using MakerShop.Products;
using MakerShop.Reporting;

public partial class ConLib_RelatedProducts : System.Web.UI.UserControl
{
    private string _Caption = "Related Products";
    private int _MaxItems = 5;
    private string _Orientation = "HORIZONTAL";
    private int _Columns = 3;

    /// <summary>
    /// Default is 3 columns, Only for HORIZONTAL Orientation
    /// </summary>
    [Personalizable(), WebBrowsable()]
    public int Columns
    {
        get { return _Columns; }
        set
        {
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
            }
            else
            {
                ProductList.RepeatColumns = 1;
                ProductList.RepeatDirection = RepeatDirection.Vertical;
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

    protected void Page_Load(object sender, EventArgs e)
    {
        phCaption.Text = this.Caption;
        int productId = PageHelper.GetProductId();
        Product product = ProductDataSource.Load(productId);
        if (product != null)
        {
            RelatedProductCollection relatedProducts = product.RelatedProducts;
            ProductCollection products = new ProductCollection();

            // GET ALL CHILD PRODUCTS AND ADD TO THE COLLECTION
            foreach (RelatedProduct relatedProduct in relatedProducts) products.Add(relatedProduct.ChildProduct);

            if (products.Count > 0)
            {
                ProductList.DataSource = products;
                ProductList.DataBind();
            }
            else phContent.Visible = false;
        }
        else phContent.Visible = false;
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


}
