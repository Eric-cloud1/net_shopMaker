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
using MakerShop.Catalog;
using MakerShop.Products;
using MakerShop.Utility;

public partial class ConLib_CategoryGridPage : System.Web.UI.UserControl, ISearchSidebarAware
{
    private int _Cols = 3;
    private int _Rows = 5;
    private int _PageSize;
    private int _ManufacturerId = 0;
    private string _Keywords = string.Empty;
    private Category _Category;
    private int _HiddenPageIndex;
    private int _SearchResultCount;
    private int _LastPageIndex;

    private string _DefaultCaption = "Catalog";
    private bool _DisplayBreadCrumbs = true;

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
    
	protected ISearchSidebar _SearchSidebar = null;

    public int CategoryId
    {
        get
        {
			if(_SearchSidebar != null && _SearchSidebar.CategoryId > 0) return _SearchSidebar.CategoryId;
            if (ViewState["CategoryId"] == null)
                ViewState["CategoryId"] = PageHelper.GetCategoryId();
            return (int)ViewState["CategoryId"];
        }
        set
        {
            ViewState["CategoryId"] = value;
        }
    }

    private void BindPage()
    {
        //BIND THE DISPLAY ELEMENTS
        if (IsValidCategory())
        {
            CategoryBreadCrumbs1.Visible = DisplayBreadCrumbs;
            CategoryBreadCrumbs1.CategoryId = this.CategoryId;
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
            _SearchResultCount = ProductDataSource.NarrowSearchCount(_Keywords, this.CategoryId, _ManufacturerId, 0, 0, SortResults.SelectedValue.StartsWith("IsFeatured"));
            _LastPageIndex = ((int)Math.Ceiling(((double)_SearchResultCount / (double)_PageSize))) - 1;
            _PagingVarsInitialized = true;
        }
    }

    void ProcessSidebarEvent(object sender, EventArgs e)
    {
        ISearchSidebar searchSidebar = (ISearchSidebar)sender;
		if(searchSidebar == null) return;
        this.CategoryId = searchSidebar.CategoryId;
        _Category = CategoryDataSource.Load(this.CategoryId);
        _ManufacturerId = searchSidebar.ManufacturerId;
        _Keywords = searchSidebar.Keyword;
        HiddenPageIndex.Value = "0";
        InitializePagingVars(true);
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        Trace.Write(this.GetType().ToString(), "Load Begin");
        _PageSize = (_Cols * _Rows);
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
            //LOOK FOR A SIDEBAR CONTROL TO LINK TO THIS SEARCH PAGE
            Trace.Write(this.GetType().ToString(), "Locating Sidebar");
            _SearchSidebar = (ISearchSidebar) PageHelper.FindSearchSidebarControl(this.Page);
			if(_SearchSidebar != null)
			{	
                _SearchSidebar.SubscribeSidebarUpdated(new EventHandler(ProcessSidebarEvent));
                //LOAD VALUES FROM 
                if (!Page.IsPostBack) _SearchSidebar.CategoryId = this.CategoryId;
                _Keywords = _SearchSidebar.Keyword;
                _ManufacturerId = _SearchSidebar.ManufacturerId;
            }
            SetPagerIndex();            
        }

        if (IsValidCategory())
        {
            //BIND PAGE
            BindPage();
        }
    }

    protected void BindSubCategories()
    {
        CategoryCollection allCategories = CategoryDataSource.LoadForParent(this.CategoryId, true);
        List<SubCategoryData> populatedCategories = new List<SubCategoryData>();
        foreach (Category category in allCategories)
        {
            // SEARCH ALL CATEGORIES
            int totalProducts = ProductDataSource.NarrowSearchCount(_Keywords, category.CategoryId, _ManufacturerId, 0, 0);
            if (totalProducts > 0)
            {
                populatedCategories.Add(new SubCategoryData(category.CategoryId, category.Name, category.NavigateUrl, totalProducts));
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
        //INITIALIZE PAGING VARIABLES
        InitializePagingVars(false);
        //BIND THE RESULT HEADER
        BindResultHeader();
        //BIND THE PRODUCT LIST
        BindProductList();
        //BIND THE PAGING CONTROLS FOOTER
        BindPagingControls();
        //UPDATE AJAX PANEL
        SearchResultsAjaxPanel.Update();
        Trace.Write(this.GetType().ToString(), "End Bind Search Results");
    }

    protected void BindResultHeader()
    {
        //UPDATE THE RESULT INDEX MESSAGE
        int startRowIndex = (_PageSize * _HiddenPageIndex);
        int endRowIndex = startRowIndex + _PageSize;
        if (endRowIndex > _SearchResultCount) endRowIndex = _SearchResultCount;
        if (_SearchResultCount == 0) startRowIndex = -1;
        ResultIndexMessage.Text = string.Format(ResultIndexMessage.Text, (startRowIndex + 1), endRowIndex, _SearchResultCount);
    }

    protected void SortResults_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindPage();
    }

    protected void BindProductList()
    {
        ProductList.DataSource = ProductDataSource.NarrowSearch(_Keywords, this.CategoryId, _ManufacturerId, 0, 0, _PageSize, (_HiddenPageIndex * _PageSize), SortResults.SelectedValue);
        ProductList.DataBind();
        NoSearchResults.Visible = (_SearchResultCount == 0);
        SearchResultsAjaxPanel.Update();
    }

    protected void ProductList_ItemDataBound(object sender, System.Web.UI.WebControls.DataListItemEventArgs e)
    {
        if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
        {
            //GENERATE TEMPLATE WITH HTML CONTROLS
            //TO OPTIMIZE OUTPUT SIZE
            PlaceHolder itemTemplate1 = (PlaceHolder)e.Item.FindControl("phItemTemplate1");
            PlaceHolder itemTemplate2 = (PlaceHolder)e.Item.FindControl("phItemTemplate2");
            if ((itemTemplate1 != null) && (itemTemplate2 != null))
            {
                Product product = (Product)e.Item.DataItem;
                string productUrl = this.Page.ResolveClientUrl(product.NavigateUrl);

                //OUTPUT LINKED THUMNAIL
                if (!string.IsNullOrEmpty(product.ThumbnailUrl))
                {
                    string thumbnail = string.Format("<a href=\"{0}\"><img src=\"{1}\" alt=\"{2}\" border=\"0\" class=\"Thumbnail\" /></a><br />", productUrl, ResolveUrl(product.ThumbnailUrl), product.ThumbnailAltText);
                    itemTemplate1.Controls.Add(new LiteralControl(thumbnail));
                }

                //OUTPUT LINKED NAME
                itemTemplate1.Controls.Add(new LiteralControl(string.Format("<a href=\"{0}\" class=\"highlight\">{1}</a><br />", productUrl, product.Name)));

                //OUTPUT MANUFACTURER
                if (product.Manufacturer != null)
                {
                    itemTemplate2.Controls.Add(new LiteralControl("<br /><a href=\"Search.aspx?m=" + product.Manufacturer.ManufacturerId + "\">" + product.Manufacturer.Name + "</a>"));                    
                }

                //OUTPUT RATING
                if (Token.Instance.Store.Settings.ProductReviewEnabled != MakerShop.Users.UserAuthFilter.None)
                {
                    itemTemplate2.Controls.Add(new LiteralControl(string.Format("<br /><img src=\"{0}\" />", NavigationHelper.GetRatingImage(product.Rating))));
                }
            }
        }
        else if (e.Item.ItemType == ListItemType.Separator)
        {
            //CHECK IF WE ARE AT THE END OF THE ROW
            int tempIndex = (e.Item.ItemIndex + 1);
            if ((tempIndex % ProductList.RepeatColumns) == 0)
            {
                //END OF ROW DETECTED, HIDE SEPARATOR
                e.Item.Controls.Clear();
                e.Item.CssClass = string.Empty;
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

            int totalPages = currentPagerIndex + 1 + _PageSize ; // ADD ONE BECAUSE IT IS A ZERO BASED INDEX
            if (totalPages > (_LastPageIndex + 1)) totalPages = (_LastPageIndex + 1);

            string baseUrl;
            if (_Category != null) baseUrl = this.Page.ResolveClientUrl(_Category.NavigateUrl) + "?";
            else if (this.CategoryId == 0) baseUrl = this.Page.ResolveClientUrl(Request.Url.AbsolutePath) + "?";
            else baseUrl = NavigationHelper.GetStoreUrl(this.Page, "Search.aspx?");
            if (!string.IsNullOrEmpty(_Keywords)) baseUrl += "k=" + Server.UrlEncode(_Keywords) + "&";
            if (_ManufacturerId != 0) baseUrl += "m=" + _ManufacturerId.ToString() + "&";
            if (!String.IsNullOrEmpty(SortResults.SelectedValue)) baseUrl += "s=" + SortResults.SelectedValue + "&";

            // INCLUDE CATEGORY ID IF PAGE HAVE SEARCH SIDE BAR
            if (_SearchSidebar != null) baseUrl += "c=" + _SearchSidebar.CategoryId + "&";

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
    protected void SetPagerIndex()
    {
        InitializePagingVars(false);
        _HiddenPageIndex = AlwaysConvert.ToInt(Request.QueryString["p"]);
        if (_HiddenPageIndex < 0) _HiddenPageIndex = 0;
        if (_HiddenPageIndex > _LastPageIndex) _HiddenPageIndex = _LastPageIndex;
        HiddenPageIndex.Value = _HiddenPageIndex.ToString();
    }
    #endregion

    protected void Page_PreRender(object sender, EventArgs e)
    {
		//Binding needs to be done in Load so that mini-basket gets updated properly
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
