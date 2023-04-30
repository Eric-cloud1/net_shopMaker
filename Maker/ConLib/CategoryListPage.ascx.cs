using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Common;
using MakerShop.Catalog;
using MakerShop.Products;
using MakerShop.Taxes;
using MakerShop.Utility;

public partial class ConLib_CategoryListPage : System.Web.UI.UserControl
{
    private int _PageSize = 20;
    private Category _Category;
    private CatalogNodeCollection _ContentNodes;
    private int _HiddenPageIndex;
    private int _LastPageIndex;

    private string _PagingLinksLocation = "BOTTOM";

    /// <summary>
    /// Indicates where the paging links will be displayd, possible values are "TOP", "BOTTOM" and "TOPANDBOTTOM"
    /// </summary>
    [Personalizable(), WebBrowsable()]
    public string PagingLinksLocation
    {
        get { return _PagingLinksLocation; }
        set
        {
            // possible values are "TOP", "BOTTOM" and "TOPANDBOTTOM"
            String tmpLocation = value.ToUpperInvariant();
            if (tmpLocation == "TOP" || tmpLocation == "BOTTOM" || tmpLocation == "TOPANDBOTTOM")
            {
                _PagingLinksLocation = tmpLocation;
            }
        }
    }


    private string _DefaultCaption = "Catalog";
    private bool _DisplayBreadCrumbs = true;

    /// <summary>
    /// Indicates wheather the breadcrumbs should be displayed or not, default value is true
    /// </summary>
    [Personalizable(), WebBrowsable()]
    public bool DisplayBreadCrumbs
    {
        get { return _DisplayBreadCrumbs; }
        set { _DisplayBreadCrumbs = value; }
    }

    /// <summary>
    /// Caption text that will be shown when root category will be browsed
    /// </summary>
    [Personalizable(), WebBrowsable()]
    public string DefaultCaption
    {
        get { return _DefaultCaption; }
        set { _DefaultCaption = value; }
    }

    public int CategoryId
    {
        get
        {
            if (ViewState["CategoryId"] == null)
                ViewState["CategoryId"] = PageHelper.GetCategoryId();
            return (int)ViewState["CategoryId"];
        }
        set
        {
            ViewState["CategoryId"] = value;
        }
    }

    public int PageSize
    {
        get { return _PageSize; }
        set { _PageSize = value; }
    }

    private void BindPage()
    {
        CategoryBreadCrumbs1.Visible = DisplayBreadCrumbs;
        CategoryBreadCrumbs1.CategoryId = this.CategoryId;

        //BIND THE DISPLAY ELEMENTS
        if (IsValidCategory())
        {
            if (_Category != null)
            {
                Page.Title = _Category.Name;
                Caption.Text = _Category.Name;
            }
            else
            {
                // IF IT IS ROOT CATEGORY
                Page.Title = DefaultCaption;
                Caption.Text = DefaultCaption;
            }
            BindSubCategories();
        }
        else
        {
            CategoryHeaderPanel.Visible = false;
        }
        BindSearchResultsPanel();
    }

    private bool _PagingVarsInitialized = false;
    private void InitializePagingVars(bool forceRefresh)
    {
        Trace.Write("Initialize Paging Vars");
        if (!_PagingVarsInitialized || forceRefresh)
        {
            _HiddenPageIndex = AlwaysConvert.ToInt(HiddenPageIndex.Value);
            _LastPageIndex = ((int)Math.Ceiling(((double)_ContentNodes.Count / (double)_PageSize))) - 1;
            _PagingVarsInitialized = true;
        }
    }

    protected void Page_Init(object sender, System.EventArgs e)
    {
        Trace.Write(this.GetType().ToString(), "Load Begin");
        _Category = CategoryDataSource.Load(this.CategoryId);
        //EXIT PROCESSING IF CATEGORY IS INVALID OR MARKED PRIVATE
        if (!IsValidCategory())
        {
            SearchResultsAjaxPanel.Visible = false;
        }
        else
        {
            if (!Page.IsPostBack)
            {
                //REGISTER THE PAGEVIEW
                MakerShop.Reporting.PageView.RegisterCatalogNode(this.CategoryId, CatalogNodeType.Category);
                //INITIALIZE SEARCH CRITERIA ON FIRST VISIT
                HiddenPageIndex.Value = Request.QueryString["p"];
                string tempSort = Request.QueryString["s"];
                if (!string.IsNullOrEmpty(tempSort))
                {
                    ListItem item = SortResults.Items.FindByValue(tempSort);
                    if (item != null) item.Selected = true;
                }
            }
            
            Trace.Write(this.GetType().ToString(), "Load Complete");
        }
    }

    protected void BindSubCategories()
    {
        CategoryCollection allCategories = CategoryDataSource.LoadForParent(this.CategoryId, true);
        List<SubCategoryData> populatedCategories = new List<SubCategoryData>();
        foreach (Category category in allCategories)
        {
            int totalItems = CatalogNodeDataSource.CountForCategory(category.CategoryId);
            if (totalItems > 0)
            {
                populatedCategories.Add(new SubCategoryData(category.CategoryId, category.Name, category.NavigateUrl, totalItems));
            }
        }
        if (populatedCategories.Count > 0)
        {
            SubCategoryPanel.Visible = true;
            SubCategoryRepeater.DataSource = populatedCategories;
            SubCategoryRepeater.DataBind();
        }
        else SubCategoryPanel.Visible = false;
    }

    public class SubCategoryData
    {
        private int _CategoryId;
        private string _Name;
        private int _ProductCount;
        private string _NavigateUrl;
        public int CategoryId { get { return _CategoryId; } }
        public string Name { get { return _Name; } }
        public int ProductCount { get { return _ProductCount; } }
        public string NavigateUrl { get { return _NavigateUrl; } }
        public SubCategoryData(int categoryId, string name, string navigateUrl, int productCount)
        {
            _CategoryId = categoryId;
            _Name = name;
            _NavigateUrl = navigateUrl;
            _ProductCount = productCount;
        }
    }

    private void BindSearchResultsPanel()
    {
        Trace.Write(this.GetType().ToString(), "Begin Bind Search Results");
        if (_ContentNodes.Count > 0)
        {
            //SORT THE CATEGORIES ACCORDINGLY
            string sortExpression = SortResults.SelectedValue;
            if (!string.IsNullOrEmpty(sortExpression))
            {
                string[] sortTokens = sortExpression.Split(" ".ToCharArray());
                SortDirection dir = (sortTokens[1] == "ASC" ? SortDirection.Ascending : SortDirection.Descending);
                switch (sortTokens[0])
                {
                    case "Price":
                        _ContentNodes.Sort(new PriceComparer(dir));
                        break;
                    case "Name":
                        _ContentNodes.Sort(new NameComparer(dir));
                        break;
                    case "Manufacturer":
                        _ContentNodes.Sort(new ManufacturerComparer(dir));
                        break;
                }
            }
            //INITIALIZE PAGING VARIABLES
            InitializePagingVars(false);
            //BIND THE RESULT PANE
            BindResultHeader();
            //BIND THE PAGING CONTROLS FOOTER
            BindPagingControls();
        }
        else
        {
            //HIDE THE CONTENTS
            phCategoryContents.Visible = false;
            phEmptyCategory.Visible = (_Category != null && _Category.CatalogNodes.Count == 0);
        }
        //UPDATE AJAX PANEL
        SearchResultsAjaxPanel.Update();
        Trace.Write(this.GetType().ToString(), "End Bind Search Results");
    }

    protected void BindResultHeader()
    {
        //UPDATE THE RESULT INDEX MESSAGE
        int startRowIndex = (_PageSize * _HiddenPageIndex);
        int endRowIndex = startRowIndex + _PageSize;
        if (endRowIndex > _ContentNodes.Count) endRowIndex = _ContentNodes.Count;
        if (_ContentNodes.Count == 0) startRowIndex = -1;
        CatalogNodeCollection bindNodes = new CatalogNodeCollection();
        ResultIndexMessage.Text = string.Format(ResultIndexMessage.Text, (startRowIndex + 1), endRowIndex, _ContentNodes.Count);
        for (int i = startRowIndex; i < endRowIndex; i++)
        {
            bindNodes.Add(_ContentNodes[i]);
        }
        CatalogNodeList.DataSource = bindNodes;
        CatalogNodeList.DataBind();
    }

    protected void CatalogNodeList_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {         
        if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
        {            
            //GENERATE TEMPLATE WITH HTML CONTROLS
            //TO OPTIMIZE OUTPUT SIZE
            PlaceHolder itemTemplate1 = (PlaceHolder)e.Item.FindControl("phItemTemplate1");
            PlaceHolder itemTemplate2 = (PlaceHolder)e.Item.FindControl("phItemTemplate2");            

            if ((itemTemplate1 != null) && (itemTemplate2 != null))
            {
                // CSS CLASS FOR THIS ROW
                String cssClass = "oddRow";
                if (e.Item.ItemIndex % 2 == 0) cssClass = "evenRow";

                itemTemplate1.Controls.Add(new LiteralControl("<tr class=\"" + cssClass + "\">"));

                CatalogNode catalogNode = (CatalogNode)e.Item.DataItem;
                Product product = null;
                if (catalogNode.CatalogNodeType == CatalogNodeType.Product && catalogNode.ChildObject != null)
                {
                    product = ((Product)catalogNode.ChildObject);
                }
                string catalogNodeUrl = this.Page.ResolveClientUrl(catalogNode.NavigateUrl);

                /*
                //OUTPUT LINKED THUMNAIL
                if (!string.IsNullOrEmpty(catalogNode.ThumbnailUrl))
                {
                    string thumbnail = string.Format("<a href=\"{0}\"><img src=\"{1}\" alt=\"{2}\" border=\"0\" class=\"Thumbnail\" /></a><br />", catalogNodeUrl, ResolveUrl(catalogNode.ThumbnailUrl), catalogNode.ThumbnailAltText);
                    itemTemplate.Controls.Add(new LiteralControl(thumbnail));
                }
                */

                //OUTPUT SKU
                String sku = "&nbsp;";
                if (product != null) sku = product.Sku;
                itemTemplate1.Controls.Add(new LiteralControl("<td>"));
                itemTemplate1.Controls.Add(new LiteralControl(sku));
                itemTemplate1.Controls.Add(new LiteralControl("</td>"));

                //OUTPUT LINKED NAME
                itemTemplate1.Controls.Add(new LiteralControl("<td>"));
                itemTemplate1.Controls.Add(new LiteralControl(string.Format("<a href=\"{0}\" class=\"highlight\">{1}</a><br />", catalogNodeUrl, catalogNode.Name)));
                itemTemplate1.Controls.Add(new LiteralControl("</td>"));

                //OUTPUT Manufacturer                
                if (product != null && product.Manufacturer != null)
                {
                    itemTemplate1.Controls.Add(new LiteralControl("<td><a href=\"Search.aspx?m=" + product.Manufacturer.ManufacturerId + "\">" + product.Manufacturer.Name + "</a></td>"));
                }
                else itemTemplate1.Controls.Add(new LiteralControl("<td>&nbsp;</td>"));
                

                //OUTPUT RETAIL PRICE IF AVAILABLE
                if (product !=null)
                {                    
                    itemTemplate1.Controls.Add(new LiteralControl("<td align='center'>"));
                    if (!product.UseVariablePrice && product.MSRP > 0)
                    {
                        string msrp = string.Format("<span class=\"msrp\">{0:ulc}</span> ", TaxHelper.GetShopPrice(product.MSRP, product.TaxCodeId));
                        itemTemplate1.Controls.Add(new LiteralControl(msrp));
                    }
                    else itemTemplate1.Controls.Add(new LiteralControl("&nbsp;"));
                    itemTemplate1.Controls.Add(new LiteralControl("</td>"));
                }
                else itemTemplate1.Controls.Add(new LiteralControl("<td>&nbsp;</td>"));
                itemTemplate2.Controls.Add(new LiteralControl("</tr>"));
            }
        }
    }

    #region PagingControls
    protected void BindPagingControls()
    {
        if (_LastPageIndex > 0)
        {
            PagerPanel.Visible = (PagingLinksLocation == "BOTTOM" || PagingLinksLocation == "TOPANDBOTTOM");
            PagerPanelTop.Visible = (PagingLinksLocation == "TOP" || PagingLinksLocation == "TOPANDBOTTOM");

            float tempIndex = ((float)_HiddenPageIndex / 10) * 10;
            int currentPagerIndex = (int)tempIndex;

            int totalPages = currentPagerIndex + 1 + _PageSize; // ADD ONE BECAUSE IT IS A ZERO BASED INDEX
            if (totalPages > (_LastPageIndex + 1)) totalPages = (_LastPageIndex + 1);

            string baseUrl;
            if (_Category != null) baseUrl = this.Page.ResolveClientUrl(_Category.NavigateUrl) + "?";
            else if (this.CategoryId == 0) baseUrl = this.Page.ResolveClientUrl(Request.Url.AbsolutePath) + "?";
            else baseUrl = NavigationHelper.GetStoreUrl(this.Page, "Search.aspx?");
            //if (!string.IsNullOrEmpty(_Keywords)) baseUrl += "k=" + Server.UrlEncode(_Keywords) + "&";
            //if (_ManufacturerId != 0) baseUrl += "m=" + _ManufacturerId.ToString() + "&";
            if (!String.IsNullOrEmpty(SortResults.SelectedValue)) baseUrl += "s=" + SortResults.SelectedValue + "&";

            if (PagerPanel.Visible)
            {
                PagerControls.DataSource = NavigationHelper.GetPaginationLinks(currentPagerIndex, totalPages, baseUrl);
                PagerControls.DataBind();
            }

            if (PagerPanelTop.Visible)
            {
                PagerControlsTop.DataSource = NavigationHelper.GetPaginationLinks(currentPagerIndex, totalPages, baseUrl);
                PagerControlsTop.DataBind();
            }
        }
        else
        {
            PagerPanel.Visible = false;
            PagerPanelTop.Visible = false;
        }
    }

   

    protected void PagerControls_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Page")
        {
            InitializePagingVars(false);
            _HiddenPageIndex = AlwaysConvert.ToInt((string)e.CommandArgument);
            if (_HiddenPageIndex < 0) _HiddenPageIndex = 0;
            if (_HiddenPageIndex > _LastPageIndex) _HiddenPageIndex = _LastPageIndex;
            HiddenPageIndex.Value = _HiddenPageIndex.ToString();
        }
    }
    #endregion

    #region Comparers

    private class NameComparer : IComparer
    {
        SortDirection _SortDirection;
        public NameComparer(SortDirection sortDirection)
        {
            _SortDirection = sortDirection;
        }

        #region IComparer Members
        public int Compare(object x, object y)
        {
            if (_SortDirection == SortDirection.Ascending)
                return ((CatalogNode)x).Name.CompareTo(((CatalogNode)y).Name);
            return ((CatalogNode)y).Name.CompareTo(((CatalogNode)x).Name);
        }
        #endregion
    }

    private class PriceComparer : IComparer
    {
        SortDirection _SortDirection;
        public PriceComparer(SortDirection sortDirection)
        {
            _SortDirection = sortDirection;
        }

        #region IComparer Members
        public int Compare(object x, object y)
        {
            LSDecimal xPrice = 0;
            LSDecimal yPrice = 0;

            CatalogNode catalogNodeX = x as CatalogNode;
            CatalogNode catalogNodeY = y as CatalogNode;
            
            if(catalogNodeX.CatalogNodeType == CatalogNodeType.Product )
            {
                xPrice = ((MakerShop.Products.Product)catalogNodeX.ChildObject).Price;            
            }

            if (catalogNodeY.CatalogNodeType == CatalogNodeType.Product)
            {
                yPrice = ((MakerShop.Products.Product)catalogNodeY.ChildObject).Price;
            }

            if (_SortDirection == SortDirection.Ascending)
                return (xPrice.CompareTo(yPrice));
            return (yPrice.CompareTo(xPrice));
        }
        #endregion
    }

    private class ManufacturerComparer : IComparer
    {
        SortDirection _SortDirection;
        public ManufacturerComparer(SortDirection sortDirection)
        {
            _SortDirection = sortDirection;
        }

        #region IComparer Members
        public int Compare(object x, object y)
        {
            string xMan = string.Empty;
            Product pX = null;
            if (((CatalogNode)x).CatalogNodeType == CatalogNodeType.Product)
                pX = ((Product)((CatalogNode)x).ChildObject);
            if ((pX != null) && (pX.Manufacturer != null)) xMan = pX.Manufacturer.Name;
            string yMan = string.Empty;
            Product pY = null;
            if (((CatalogNode)y).CatalogNodeType == CatalogNodeType.Product)
                pY = ((Product)((CatalogNode)y).ChildObject);
            if ((pY != null) && (pY.Manufacturer != null)) yMan = pY.Manufacturer.Name;
            if (_SortDirection == SortDirection.Ascending)
                return (xMan.CompareTo(yMan));
            return (yMan.CompareTo(xMan));
        }
        #endregion
    }
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsValidCategory())
        {
            //INITIALIZE THE CONTENT NODES
            _ContentNodes = new CatalogNodeCollection();
            CatalogNodeCollection allNodes;
            if (_Category != null) allNodes = _Category.CatalogNodes;
            else allNodes = CatalogNodeDataSource.LoadForCategory(0);
            foreach (CatalogNode node in allNodes)
            {
                if ((node.CatalogNodeType == CatalogNodeType.Product)
                    && (node.Visibility == CatalogVisibility.Public))
                {
                    if (!SortResults.SelectedValue.StartsWith("IsFeatured")) _ContentNodes.Add(node);
                    else if (node.CatalogNodeType == CatalogNodeType.Product && ((Product)node.ChildObject).IsFeatured) _ContentNodes.Add(node);
                }
            }

            //BIND PAGE
            BindPage();
        }
    }

    private bool IsValidCategory()
    {
        // IF IT IS ROOT CATEGORY
        if (this.CategoryId == 0) return true;
        else
        {
            // TRY TO LOAD THE CATEGORY AGAIN
            if (_Category == null) _Category = CategoryDataSource.Load(this.CategoryId);
            if (_Category != null && _Category.Visibility != CatalogVisibility.Private) return true;
            else return false;
        }
    }
}
