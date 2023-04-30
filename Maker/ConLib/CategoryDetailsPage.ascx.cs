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

public partial class ConLib_CategoryDetailsPage : System.Web.UI.UserControl
{
    private Category _Category;
    private CatalogNodeCollection _CatalogNodes;
    private string _DefaultCaption = "Catalog";
    private string _DefaultCategorySummary = "Welcome to our store.";
    

    /// <summary>
    /// Name that will be shown as caption when root category will be browsed
    /// </summary>
    [Personalizable(), WebBrowsable()]
    public string DefaultCaption
    {
        get { return _DefaultCaption; }
        set { _DefaultCaption = value; }
    }

    /// <summary>
    /// Summary that will be shown when root category will be browsed
    /// </summary>
    [Personalizable(), WebBrowsable()]
    public string DefaultCategorySummary
    {
        get { return _DefaultCategorySummary; }
        set { _DefaultCategorySummary = value; }
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
        //BIND THE DISPLAY ELEMENTS
        if (IsValidCategory())
        {
            if (_Category != null)
            {
                Page.Title = _Category.Name;
                CategoryBreadCrumbs1.CategoryId = this.CategoryId;
                Caption.Text = _Category.Name;
                CategoryDescription.Text = _Category.Summary;
            }
            else
            {
                // IT IS ROOT CATEGORY
                Page.Title = DefaultCaption;
                CategoryBreadCrumbs1.CategoryId = this.CategoryId;
                Caption.Text = DefaultCaption;
                CategoryDescription.Text = DefaultCategorySummary;
            }

            //BindSubCategories();
        }
        else
        {
            CategoryHeaderPanel.Visible = false;
        }
        BindResultHeader();
    }

    protected void Page_Init(object sender, System.EventArgs e)
    {
        Trace.Write(this.GetType().ToString(), "Load Begin");        
        //EXIT PROCESSING IF CATEGORY IS INVALID OR MARKED PRIVATE
        if (!IsValidCategory())
        {
            // SHOW ONLY THE MESSAGE THAT REQUIRED PARAMETER IS MISSING.
            ResultsAjaxPanel.Visible = false;
            RequiredParameterMissingPanel.Visible = true;
            RequiredParameterMessage.Text = String.Format(RequiredParameterMessage.Text, "CategoryDetailsPage", "CategoryId");
            return;
        }
        else
        {
	        if (!Page.IsPostBack)
	        {
	            //REGISTER THE PAGEVIEW
                MakerShop.Reporting.PageView.RegisterCatalogNode(this.CategoryId, CatalogNodeType.Category);
	        }
    	}
        
        Trace.Write(this.GetType().ToString(), "Load Complete");
    }

    //protected void BindSubCategories()
    //{
    //    CategoryCollection allCategories = CategoryDataSource.LoadForParent(this.CategoryId, true);
    //    List<SubCategoryData> populatedCategories = new List<SubCategoryData>();
    //    foreach (Category category in allCategories)
    //    {
    //        int totalItems = CatalogNodeDataSource.CountForCategory(category.CategoryId);
    //        if (totalItems > 0)
    //        {
    //            populatedCategories.Add(new SubCategoryData(category.CategoryId, category.Name, category.NavigateUrl, totalItems));
    //        }
    //    }
    //    if (populatedCategories.Count > 0)
    //    {
    //        SubCategoryPanel.Visible = true;
    //        SubCategoryRepeater.DataSource = populatedCategories;
    //        SubCategoryRepeater.DataBind();
    //    }
    //    else SubCategoryPanel.Visible = false;
    //}

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

    //private void BindSearchResultsPanel()
    //{
    //    Trace.Write(this.GetType().ToString(), "Begin Bind Search Results");
    //    if (_CatalogNodes.Count > 0)
    //    {
    //        //SORT THE CATEGORIES ACCORDINGLY
    //        string sortExpression = SortResults.SelectedValue;
    //        if (!string.IsNullOrEmpty(sortExpression))
    //        {
    //            string[] sortTokens = sortExpression.Split(" ".ToCharArray());
    //            SortDirection dir = (sortTokens[1] == "ASC" ? SortDirection.Ascending : SortDirection.Descending);
    //            switch (sortTokens[0])
    //            {
    //                case "Price":
    //                    _CatalogNodes.Sort(new PriceComparer(dir));
    //                    break;
    //                case "Name":
    //                    _CatalogNodes.Sort(new NameComparer(dir));
    //                    break;
    //                case "Manufacturer":
    //                    _CatalogNodes.Sort(new ManufacturerComparer(dir));
    //                    break;
    //            }
    //        }
    //        //INITIALIZE PAGING VARIABLES
    //        InitializePagingVars(false);
    //        //BIND THE RESULT PANE
    //        BindResultHeader();
    //        //BIND THE PAGING CONTROLS FOOTER
    //        BindPagingControls();
    //    }
    //    else
    //    {
    //        //HIDE THE CONTENTS
    //        phCategoryContents.Visible = false;
    //        phEmptyCategory.Visible = (_Category.CatalogNodes.Count == 0);
    //    }
    //    //UPDATE AJAX PANEL
    //    SearchResultsAjaxPanel.Update();
    //    Trace.Write(this.GetType().ToString(), "End Bind Search Results");
    //}

    protected void BindResultHeader()
    {
        //UPDATE THE RESULT INDEX MESSAGE        
        CatalogNodeList.DataSource = _CatalogNodes;
        CatalogNodeList.DataBind();
    }

    protected void CatalogNodeList_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
        {
            //GENERATE TEMPLATE WITH HTML CONTROLS
            //TO OPTIMIZE OUTPUT SIZE
            PlaceHolder itemTemplate1 = (PlaceHolder)e.Item.FindControl("phItemTemplate1");

            if ((itemTemplate1 != null))
            {
                CatalogNode catalogNode = (CatalogNode)e.Item.DataItem;
                //if (catalogNode.CatalogNodeType == CatalogNodeType.Category) return;
                string catalogNodeUrl = this.Page.ResolveClientUrl(catalogNode.NavigateUrl);

                string target = "_self";
                if (catalogNode.CatalogNodeType == CatalogNodeType.Link)
                    target = ((Link)catalogNode.ChildObject).TargetWindow;

                //OUTPUT LINKED THUMNAIL
                itemTemplate1.Controls.Add(new LiteralControl("<tr>"));
                itemTemplate1.Controls.Add(new LiteralControl("<td rowspan=\"2\" valign=\"top\">"));
                string thumbnail = "&nbsp;";
                if (!string.IsNullOrEmpty(catalogNode.ThumbnailUrl))
                {
                    thumbnail = string.Format("<a href=\"{0}\" target=\"{3}\"><img src=\"{1}\" alt=\"{2}\" border=\"0\" class=\"Thumbnail\" /></a>", catalogNodeUrl, ResolveClientUrl(catalogNode.ThumbnailUrl), catalogNode.ThumbnailAltText, target);
                }
                itemTemplate1.Controls.Add(new LiteralControl(thumbnail));
                itemTemplate1.Controls.Add(new LiteralControl("</td>"));

                //OUTPUT LINKED NAME
                itemTemplate1.Controls.Add(new LiteralControl("<td style=\"vertical-align:top;height:100%;\">"));
                itemTemplate1.Controls.Add(new LiteralControl(string.Format("<a href=\"{0}\" class=\"highlight\" target=\"{2}\">{1}</a>", catalogNodeUrl, catalogNode.Name,target)));
                itemTemplate1.Controls.Add(new LiteralControl("</td></tr>"));

                //OUTPUT DESCRIPTION                
                itemTemplate1.Controls.Add(new LiteralControl("<tr><td style=\"vertical-align:top;\">"));
                itemTemplate1.Controls.Add(new LiteralControl(catalogNode.Summary));
                itemTemplate1.Controls.Add(new LiteralControl("</td></tr>"));
            }
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (IsValidCategory())
        {
			//INITIALIZE THE CONTENT NODES
	        _CatalogNodes = new CatalogNodeCollection();
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
                        _CatalogNodes.Add(node);
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