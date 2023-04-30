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

public partial class ConLib_RecentlyViewed : System.Web.UI.UserControl
{
    private string _Caption = "Recently Viewed";
    private int _MaxItems = 5;
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
        set
        {
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
        int userId = Token.Instance.UserId;
        if (userId != 0)
        {
            List<Product> products = PageViewDataSource.GetRecentlyViewedProducts(userId, this.MaxItems, 0, "ActivityDate DESC");
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

    protected string GetThumbnailUrl(object thumbnailUrl)
    {
        if (!string.IsNullOrEmpty((string)thumbnailUrl)) return (string)thumbnailUrl;
        return "~/images/thumbs/ProductThumbnail.gif";
    }


}
