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
using MakerShop.Catalog;
using MakerShop.Products;

public partial class ConLib_MoreCategoryItems : System.Web.UI.UserControl
{
    private string _Caption = "Other Items In This Category";
    private int _MaxItems = 4;
    private string _DisplayMode = "SEQUENTIAL";
    private string _Orientation = "HORIZONTAL";

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
    public string DisplayMode
    {
        get
        {
            return _DisplayMode;
        }
        set
        {
            _DisplayMode = value.ToUpperInvariant();
            if ((_DisplayMode != "SEQUENTIAL") && (_DisplayMode != "RANDOM")) _DisplayMode = "SEQUENTIAL";
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
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        int _CategoryId = PageHelper.GetCategoryId();
        Category _Category = CategoryDataSource.Load(_CategoryId);

        if (_Category != null)
        {
            //GET ALL PRODUCTS IN CATEGORY
            CatalogNodeCollection productNodes = GetProductNodes(_CategoryId);

            //LOOK FOR A PRODUCT TO ESTABLISH STARTING POINT
            int productId = PageHelper.GetProductId();
            int productIndex = productNodes.IndexOf(_CategoryId, productId, (byte)CatalogNodeType.Product);
            //PRODUCT WAS FOUND IN SET, DISPLAY ITEMS BEFORE AND AFTER
            //AVAILABLE SET DOES NOT INCLUDE CURRENT PRODCT
			if(productIndex >=0) 
			{
				productNodes.RemoveAt(productIndex);
			}

            //MAKE SURE WE HAVE SOMETHING TO SHOW
            if (productNodes.Count > 0)
            {
                //SET CAPTION
                phCaption.Text = this.Caption;
                //DETERMINE THE DISPLAY MODE
                bool isSequential = (_DisplayMode == "SEQUENTIAL");
                //DETERMINE HOW MANY PRODUCTS ARE AVAILABLE TO US
                int activeSize = (productNodes.Count >= _MaxItems) ? _MaxItems : productNodes.Count;
                //UPDATE THE WIDTH FOR HORIZONTAL DISPLAYS
                bool isHorizontal = (_Orientation == "HORIZONTAL");
                if (isHorizontal) _Width = string.Format("{0:F0}%", (100 / activeSize));
                else _Width = "100%";
                //BUILD THE RESULT SET TO DISPLAY
                List<Product> showProducts = new List<Product>();
                if (isSequential)
                {
                    //IF NO PRODUCT INDEX IS FOUND, USE A RANDOM STARTING POINT
                    if (productIndex < 0) productIndex = new Random().Next(0, productNodes.Count);
                    //DISPLAY ONE ITEM BEFORE IF WE ARE NOT AT THE START
                    //AND ARE SHOWING MORE THAN 1 ITEM
                    //OTHERWISE DISPLAY STARTING AFTER THE PRODUCT
                    int startIndex = productIndex;
                    if ((productIndex > 0) && (_MaxItems > 1)) startIndex--;
                    //LOOP THE PRODUCT NODES TO BUILD SET
                    int currentIndex = startIndex;
                    bool exitCondition = false;
                    while (!exitCondition)
                    {
                        showProducts.Add((Product)productNodes[currentIndex].ChildObject);
                        currentIndex++;
                        if (currentIndex == productNodes.Count) currentIndex = 0;
                        //CHECK IF WE HAVE ALL THE PRODUCTS WE NEED
                        if (showProducts.Count >= activeSize) exitCondition = true;
                        //CHECK IF WE HAVE RETURNED TO OUR STARTING POINT
                        if (currentIndex == startIndex) exitCondition = true;
                    }
                }
                else
                {
                    //DISPLAY $_MAXITEMS RANDOM PRODUCTS
                    //LOOP THE PRODUCT NODES TO BUILD SET
                    Random rng = new Random();
                    bool exitCondition = false;
                    while (!exitCondition)
                    {
                        int randomIndex = rng.Next(0, productNodes.Count);
                        showProducts.Add((Product)productNodes[randomIndex].ChildObject);
                        productNodes.RemoveAt(randomIndex);
                        //CHECK IF WE HAVE ALL THE PRODUCTS WE NEED
                        if (showProducts.Count >= activeSize) exitCondition = true;
                        //FAIL SAFE, EXIT ON EMPTY SET
                        if (productNodes.Count < 0) exitCondition = true;
                    }
                }
                //BIND THE PRODUCTS
                ItemsRepeater.DataSource = showProducts;
                ItemsRepeater.DataBind();
            }
            else
            {
                //THERE ARE NOT ANY ITEMS TO DISPLAY
                phContent.Visible = false;
            }
        }
    }

    private CatalogNodeCollection GetProductNodes(int categoryId)
    {
        //GET ALL NODES IN THE CATEGORY
        CatalogNodeCollection allNodes = CatalogDataSource.LoadForCategory(categoryId, true, "OrderBy");
        //EXTRACT THE PRODUCT NODES
        CatalogNodeCollection productNodes = new CatalogNodeCollection();
        foreach (CatalogNode node in allNodes)
        {
            //ATTEMPT TO CAST TO PRODUCT
            CatalogProductNode p = node as CatalogProductNode;
            if (p != null) productNodes.Add(p);
        }
        return productNodes;
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