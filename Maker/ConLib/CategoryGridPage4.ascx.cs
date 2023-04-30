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
using AjaxControlToolkit;

public partial class ConLib_CategoryGridPage4 : System.Web.UI.UserControl
{
    private int _Cols = 3;
    private int _Rows = 5;
    private int _PageSize;
    private Category _Category;
    private CatalogNodeCollection _ContentNodes;
    private int _HiddenPageIndex;
    private int _LastPageIndex;
    private string _DefaultCaption = "Catalog";
    private bool _DisplayBreadCrumbs = true;
    private string _PagingLinksLocation = "BOTTOM";

    private int _MaximumSummaryLength = 250; // LENGTH IN CHARACTERS

    /// <summary>
    /// Maximum characters to display for summary
    /// </summary>
    [Personalizable(), WebBrowsable()]
    public int MaximumSummaryLength
    {
        get { return _MaximumSummaryLength; }
        set { _MaximumSummaryLength = value; }
    }

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

                if (!string.IsNullOrEmpty(_Category.Description))
                {
                    CategoryDescriptionPanel.Visible = true;
                    CategoryDescription.Text = _Category.Description;
                }
                else CategoryDescriptionPanel.Visible = false;
            }
            else
            {
                // IF IT IS ROOT CATEGORY
                Page.Title = DefaultCaption;
                Caption.Text = DefaultCaption;
                CategoryDescriptionPanel.Visible = false;
            }
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
            Trace.Write(this.GetType().ToString(), "Load Complete");
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
            phEmptyCategory.Visible = phEmptyCategory.Visible = (_Category != null && _Category.CatalogNodes.Count == 0);
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

    protected void CatalogNodeList_ItemDataBound(object sender, System.Web.UI.WebControls.DataListItemEventArgs e)
    {
        if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
        {
            //GENERATE TEMPLATE WITH HTML CONTROLS
            //TO OPTIMIZE OUTPUT SIZE
            PlaceHolder itemTemplate1 = (PlaceHolder)e.Item.FindControl("phItemTemplate1");
            PlaceHolder itemTemplate2 = (PlaceHolder)e.Item.FindControl("phItemTemplate2");
            if ((itemTemplate1 != null) && (itemTemplate2 != null))
            {
                CatalogNode catalogNode = (CatalogNode)e.Item.DataItem;
                string catalogNodeUrl = this.Page.ResolveClientUrl(catalogNode.NavigateUrl);

                string target = "_self";
                if (catalogNode.CatalogNodeType == CatalogNodeType.Link)
                    target = ((Link)catalogNode.ChildObject).TargetWindow;

                //OUTPUT LINKED NAME
                itemTemplate1.Controls.Add(new LiteralControl(string.Format("<a href=\"{0}\" class=\"highlight\" target=\"{2}\">{1}</a><br />", catalogNodeUrl, catalogNode.Name,target)));

                //OUTPUT LINKED THUMNAIL
                if (!string.IsNullOrEmpty(catalogNode.ThumbnailUrl))
                {
                    string thumbnail = string.Format("<a href=\"{0}\" target=\"{3}\"><img src=\"{1}\" alt=\"{2}\" border=\"0\" class=\"Thumbnail\" /></a><br />", catalogNodeUrl, ResolveUrl(catalogNode.ThumbnailUrl), catalogNode.ThumbnailAltText, target);
                    itemTemplate1.Controls.Add(new LiteralControl(thumbnail));
                }
                else
                {
                    itemTemplate1.Controls.Add(new LiteralControl("<br />"));
                }

                //OUTPUT RETAIL PRICE IF AVAILABLE
                if (catalogNode.CatalogNodeType == CatalogNodeType.Product)
                {
                    Product product = (Product)catalogNode.ChildObject;
                    //OUTPUT MANUFACTURER
                    if (product.Manufacturer != null)
                    {
                        itemTemplate1.Controls.Add(new LiteralControl("<a href=\"Search.aspx?m=" + product.Manufacturer.ManufacturerId + "\">by " + product.Manufacturer.Name + "</a><br />"));
                    }

                    //OUTPUT RATING
                    if (Token.Instance.Store.Settings.ProductReviewEnabled != MakerShop.Users.UserAuthFilter.None)
                    {
                        itemTemplate1.Controls.Add(new LiteralControl(string.Format("<img src=\"{0}\" /><br />", NavigationHelper.GetRatingImage(product.Rating))));
                    }

                    if (!string.IsNullOrEmpty(catalogNode.Summary))
                    {
                        AddSummaryDetails(catalogNode, itemTemplate1);
                    }

                    itemTemplate1.Controls.Add(new LiteralControl("<br />"));

                    ////OUTPUT THE ADD TO CART LINK
                    //itemTemplate2.Controls.Add(new LiteralControl("<br />"));
                    //ASP.conlib_addtocartlink_ascx add2cart = new ASP.conlib_addtocartlink_ascx();
                    //add2cart.ProductId = catalogNode.CatalogNodeId;
                    //itemTemplate2.Controls.Add(add2cart);
                }
                else
                {
                    AddSummaryDetails(catalogNode, itemTemplate2);
                }
            }
        }
        else if (e.Item.ItemType == ListItemType.Separator)
        {
            //CHECK IF WE ARE AT THE END OF THE ROW
            int tempIndex = (e.Item.ItemIndex + 1);
            if ((tempIndex % CatalogNodeList.RepeatColumns) == 0)
            {
                //END OF ROW DETECTED, HIDE SEPARATOR
                e.Item.Controls.Clear();
                e.Item.CssClass = string.Empty;
            }
        }
    }

    private void AddSummaryDetails(CatalogNode catalogNode, PlaceHolder itemTemplate)
    {
        //OUTPUT SUMMARY (FIRST 100 CHARACTERS ONLY)
        String tmpSummary = catalogNode.Summary;
        if (tmpSummary.Length > MaximumSummaryLength)
        {
            tmpSummary = tmpSummary.Substring(0, MaximumSummaryLength);
            Panel textLabelPanel = new Panel();
            textLabelPanel.ID = "c_summary_" + catalogNode.CatalogNodeId;
            Label textLabel = new Label();
            textLabel.ID = "c_summary_label_" + catalogNode.CatalogNodeId;
            textLabelPanel.Controls.Add(textLabel);

            Panel summaryPanel = new Panel();
            summaryPanel.ID = "summary_" + catalogNode.CatalogNodeId;

            CollapsiblePanelExtender cpe = new CollapsiblePanelExtender();
            cpe.TargetControlID = summaryPanel.ID;
            cpe.ExpandControlID = textLabel.ID;
            cpe.CollapseControlID = textLabel.ID;
            cpe.TextLabelID = textLabel.ID;
            cpe.Collapsed = true;
            cpe.CollapsedText = tmpSummary + "... <span class=\"nodeSummaryLink\">Read More</span>";
            cpe.ExpandedText = catalogNode.Summary + "... <span class=\"nodeSummaryLink\">Read Less</span>";

            itemTemplate.Controls.Add(textLabelPanel);
            itemTemplate.Controls.Add(summaryPanel);
            itemTemplate.Controls.Add(cpe);
        }
        else
        {
            Panel textLabelPanel = new Panel();
            Label textLabel = new Label();
            textLabel.Text = tmpSummary;
            textLabelPanel.Controls.Add(textLabel);
            itemTemplate.Controls.Add(textLabelPanel);
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

            if (catalogNodeX.CatalogNodeType == CatalogNodeType.Product)
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
                if (node.Visibility == CatalogVisibility.Public)
                {
                    bool addNode = true;
                    if (node.CatalogNodeType == CatalogNodeType.Category)
                    {
                        addNode = (CatalogDataSource.CountForCategory(node.CatalogNodeId, true) > 0);
                    }
                    if (addNode)
                    {
                        if (!SortResults.SelectedValue.StartsWith("IsFeatured")) _ContentNodes.Add(node);
                        else if (node.CatalogNodeType == CatalogNodeType.Product && ((Product)node.ChildObject).IsFeatured) _ContentNodes.Add(node);
                    }
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
