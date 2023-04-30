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
using MakerShop.Marketing;
using MakerShop.Orders;
using MakerShop.Utility;
using MakerShop.Catalog;
using MakerShop.Products;

public partial class ConLib_FeaturedCategoryItems : System.Web.UI.UserControl
{
    private string _Caption = "Featured Items";
    private int _MaxItems = 3;
    private string _Orientation = "VERTICAL";
    private bool _includeOutOfStockItems = false;

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
    public int MaxItems
    {
        get
        {
            if (_MaxItems < 1) _MaxItems = 1;
            return _MaxItems;
        }
        set { _MaxItems = value; }
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

    protected void Page_Load(object sender, EventArgs e)
    {
        int _CategoryId = PageHelper.GetCategoryId();
        Category _Category = CategoryDataSource.Load(_CategoryId);

        if (_Category != null)
        {
            //GET FEATURED PRODUCTS IN CATEGORY
            ProductCollection featured = ProductDataSource.GetFeaturedProducts(_CategoryId, true, IncludeOutOfStockItems, _MaxItems, 0);            
            //MAKE SURE WE HAVE SOMETHING TO SHOW
            if (featured.Count > 0)
            {
                //SET CAPTION
                phCaption.Text = this.Caption;
                //UPDATE THE WIDTH FOR HORIZONTAL DISPLAYS
                bool isHorizontal = (_Orientation == "HORIZONTAL");
                if (isHorizontal) _Width = string.Format("{0:F0}%", (100 / featured.Count));
                else _Width = "100%";
                //BIND THE PRODUCTS
                ItemsRepeater.DataSource = featured;
                ItemsRepeater.DataBind();
            }
            else
            {
                //THERE ARE NOT ANY ITEMS TO DISPLAY
                phContent.Visible = false;
            }
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