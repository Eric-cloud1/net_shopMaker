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
using MakerShop.Utility;
using MakerShop.Users;

public partial class ConLib_SearchPage : System.Web.UI.UserControl, ISearchSidebarAware
{
    private int _Cols = 3;
    private int _Rows = 5;
    private int _PageSize;
    private int _ManufacturerId = 0;
    private string _Keywords = string.Empty;
    private int _HiddenPageIndex;
    private int _SearchResultCount;
    private int _LastPageIndex;
    
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

    private bool _PagingVarsInitialized = false;
    private void InitializePagingVars(bool forceRefresh)
    {
        Trace.Write(this.GetType().ToString(), "Begin InitializePagingVars");
        if (!_PagingVarsInitialized || forceRefresh)
        {
            _HiddenPageIndex = AlwaysConvert.ToInt(HiddenPageIndex.Value);
            _SearchResultCount = ProductDataSource.NarrowSearchCount(_Keywords, this.CategoryId, _ManufacturerId, 0, 0, SortResults.SelectedValue.StartsWith("IsFeatured"));
            _LastPageIndex = ((int)Math.Ceiling(((double)_SearchResultCount / (double)_PageSize))) - 1;
            _PagingVarsInitialized = true;
        }
        Trace.Write(this.GetType().ToString(), "End InitializePagingVars");
    }

    void ProcessSidebarEvent(object sender, EventArgs e)
    {
        ISearchSidebar searchSidebar = (ISearchSidebar)sender;
		if(searchSidebar == null) return;
        this.CategoryId = searchSidebar.CategoryId;        
        _ManufacturerId = searchSidebar.ManufacturerId;
        _Keywords = searchSidebar.Keyword;
        HiddenPageIndex.Value = "0";
        InitializePagingVars(true);
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        _PageSize = (_Cols * _Rows);
        if (!Page.IsPostBack)
        {
            //INITIALIZE SEARCH CRITERIA ON FIRST VISIT
            HiddenPageIndex.Value = AlwaysConvert.ToInt(Request.QueryString["p"]).ToString();
            string tempSort = Request.QueryString["s"];
            if (!string.IsNullOrEmpty(tempSort))
            {
                ListItem item = SortResults.Items.FindByValue(tempSort);
                if (item != null) item.Selected = true;
            }
        }

        RefreshCriteria();
        BindSearchResultsPanel();
    }

    private void RefreshCriteria()
    {
		//LOOK FOR A SIDEBAR CONTROL TO LINK TO THIS SEARCH PAGE
		Trace.Write(this.GetType().ToString(), "Locating Sidebar");
		_SearchSidebar  = (ISearchSidebar) PageHelper.FindSearchSidebarControl(this.Page);
		if (_SearchSidebar != null)
		{			
			_SearchSidebar.SubscribeSidebarUpdated(new EventHandler(ProcessSidebarEvent));
			//LOAD VALUES FROM 
			if (AlwaysConvert.ToInt(Request.QueryString["c"]) > 0)
			{
				CategoryId = AlwaysConvert.ToInt(Request.QueryString["c"]);
			}
			else
			{
				if (!Page.IsPostBack)
				{
					_SearchSidebar.CategoryId = this.CategoryId;
				}
				else
				{
					this.CategoryId = _SearchSidebar.CategoryId;
				}
			}			
			_Keywords = _SearchSidebar.Keyword;
			_ManufacturerId = _SearchSidebar.ManufacturerId;
		}

        SetPagerIndex();
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
        Trace.Write(this.GetType().ToString(), "Begin BindResultHeader");
        if (string.IsNullOrEmpty(_Keywords))
        {
            ResultTermMessage.Visible = false;
        }
        else
        {
            ResultTermMessage.Visible = true;
            ResultTermMessage.Text = string.Format(ResultTermMessage.Text, _Keywords);
        }
        //UPDATE THE RESULT INDEX MESSAGE
        int startRowIndex = (_PageSize * _HiddenPageIndex);
        int endRowIndex = startRowIndex + _PageSize;
        if (endRowIndex > _SearchResultCount) endRowIndex = _SearchResultCount;
        if (_SearchResultCount == 0) startRowIndex = -1;
        ResultIndexMessage.Text = string.Format(ResultIndexMessage.Text, (startRowIndex + 1), endRowIndex, _SearchResultCount);
        Trace.Write(this.GetType().ToString(), "End BindResultHeader");
    }

    protected void SortResults_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindSearchResultsPanel();
    }

    protected void BindProductList()
    {
        Trace.Write(this.GetType().ToString(), "Begin BindProductList");
        ProductList.DataSource = ProductDataSource.NarrowSearch(_Keywords, this.CategoryId, _ManufacturerId, 0, 0, _PageSize, (_HiddenPageIndex * _PageSize), SortResults.SelectedValue);
        ProductList.DataBind();
        NoSearchResults.Visible = (_SearchResultCount == 0);
        SearchResultsAjaxPanel.Update();
        Trace.Write(this.GetType().ToString(), "End BindProductList");
    }

    protected void ProductList_ItemDataBound(object sender, System.Web.UI.WebControls.DataListItemEventArgs e)
    {
        if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
        {
            Trace.Write(this.GetType().ToString(), "Begin ItemDataBound");
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
                    string thumbnail = string.Format("<a href=\"{0}\"><img src=\"{1}\" alt=\"{2}\" border=\"0\" class=\"Thumbnail\" /></a><br />", productUrl, Page.ResolveClientUrl(product.ThumbnailUrl), product.ThumbnailAltText);
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
                if (Token.Instance.Store.Settings.ProductReviewEnabled != UserAuthFilter.None)
                {
                    itemTemplate2.Controls.Add(new LiteralControl(string.Format("<br /><img src=\"{0}\" />", NavigationHelper.GetRatingImage(product.Rating))));
                }

            }
            Trace.Write(this.GetType().ToString(), "End ItemDataBound");
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
        Trace.Write(this.GetType().ToString(), "Begin BindPagingControls");
        if (_LastPageIndex > 0)
        {
            PagerPanel.Visible = true;
            List<PagerLinkData> pagerLinkData = new List<PagerLinkData>();
            float tempIndex = ((float)_HiddenPageIndex / 10) * 10;
            int currentPagerIndex = (int)tempIndex;

            int totalPages = currentPagerIndex + 1 + _PageSize; // ADD ONE BECAUSE IT IS A ZERO BASED INDEX
            if (totalPages > (_LastPageIndex + 1)) totalPages = (_LastPageIndex + 1);

            string baseurl = string.Empty;
            string[] segm = this.Page.Request.Url.Segments;
            foreach (string sg in segm)
            {
                baseurl += sg;
            }

            int searchBarCatId = 0;
            if (_SearchSidebar != null)
            {
                searchBarCatId = _SearchSidebar.CategoryId;
            }

            string navigateUrl = string.Empty;
            baseurl += "?" + (String.IsNullOrEmpty(_Keywords) ? "" : "k=" + _Keywords + "&")
            + (_ManufacturerId == 0 ? "" : "m=" + _ManufacturerId.ToString() + "&")
            + (String.IsNullOrEmpty(SortResults.SelectedValue) ? "" : "s=" + SortResults.SelectedValue + "&")
            + (searchBarCatId == 0 ? "" : "c=" + searchBarCatId.ToString() + "&");


            PagerControls.DataSource = NavigationHelper.GetPaginationLinks(currentPagerIndex, totalPages, baseurl);
            PagerControls.DataBind();
        }
        else
        {
            PagerPanel.Visible = false;
        }
        Trace.Write(this.GetType().ToString(), "End BindPagingControls");
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
		//Do binding in Page_Load
    }
}
