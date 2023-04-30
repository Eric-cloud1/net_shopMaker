using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Catalog;
using MakerShop.Products;
using MakerShop.Utility;
using MakerShop.Common;
using MakerShop.Stores;

public partial class ConLib_CategorySearchSidebar : System.Web.UI.UserControl, ISearchSidebar
{
    private List<ProductDataSource.ManufacturerProductCount> _Manufacturers;
    private const int _MaximumManufacturers = 4;
    public event EventHandler SidebarUpdated;

	private int _CategoryId = 0;
	private int _ManufacturerId = 0;
	private string _Keyword = string.Empty;
	
	public int CategoryId
    {
        get { return _CategoryId;}
        set { _CategoryId = value;}
    }

    public int ManufacturerId
    {
        get { return _ManufacturerId; }
        set { _ManufacturerId = value; }
    }

    public string Keyword
    {
        get { return _Keyword; }
        set { _Keyword = value; }
    }

	public void SubscribeSidebarUpdated(EventHandler eventHandler)
	{
		SidebarUpdated += eventHandler;
	}

	public void UnsubscribeSidebarUpdated(EventHandler eventHandler)
	{
		SidebarUpdated -= eventHandler;
	}

    protected void Page_Init(object sender, EventArgs e)
    {
		LoadCustomViewState();

        // SET UP THE MINIMUM LENGTH VALIDATOR
        int minLength = Store.GetCachedSettings().MinimumSearchLength;
        KeywordValidator.MinimumLength = minLength;
        KeywordValidator.ErrorMessage = String.Format(KeywordValidator.ErrorMessage, minLength);

		string eventTarget = Request.Form["__EVENTTARGET"];
		if (!Page.IsPostBack)
        {
            ManufacturerId = AlwaysConvert.ToInt(Request.QueryString["m"]);
            Keyword = StringHelper.StripHtml(Request.QueryString["k"]);
            CategoryId = AlwaysConvert.ToInt(Request.QueryString["c"]);
        }
		else
		{			
			if(!string.IsNullOrEmpty(eventTarget))
			{
				//here we will decide what the event target is and take appropriate action
				if(eventTarget.StartsWith(ManufacturerList.UniqueID)) 
				{
					//this is a narrow by manufacturer event
					NarrowByManufacturer(eventTarget);
				}
				else if(eventTarget.StartsWith(NarrowByCategoryLinks.UniqueID)) 
				{
					//this is narrow by category event
					NarrowByCategory(eventTarget);
				}
				else if(eventTarget.StartsWith(ExpandManufacturerLink.UniqueID)) 
				{
					//this is remove manufacturer event
					RemoveManufacturer();
				}
				else if(eventTarget.StartsWith(ExpandCategoryLinks.UniqueID)) 
				{
					//this is remove category event
					RemoveCategory(eventTarget);
				}
				else if(eventTarget.StartsWith(ExpandKeywordLink.UniqueID)) 
				{
					//this is remove keyword event
					RemoveKeyword();
				}
				else if(eventTarget.StartsWith(ShowAllManufacturers.UniqueID)) 
				{
					//this is shwow all manufacturers event
					ShowAllManufacturersLink();
				}
				else if(eventTarget.StartsWith(KeywordButton.UniqueID)) 
				{
					//this is narrow by keyword event
                    NarrowByKeyword();
				}
			}
		}
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        Trace.Write(this.GetType().ToString(), "Begin PreRender");
		if(IsPageSearchSidebarAware()) 
		{
			//GET PRODUCT COUNTS FOR MANUFACTURERS THAT MEET SEARCH FILTER
			_Manufacturers = ProductDataSource.NarrowSearchCountByManufacturer(this.Keyword, this.CategoryId, 0, 0, string.Empty);
			//BIND THE EXPAND PANEL
			BindExpandResultPanel();
			//BIND THE NARROW BY CATEGORY PANEL
			BindNarrowByCategoryPanel();
			//BIND THE NARROW BY MANUFACTURER PANEL
			BindNarrowByManufacturerPanel();
			//BIND THE NARROW BY KEYWORD PANEL
			BindNarrowByKeywordPanel();
			SaveCustomViewState();
		}
		else
		{
			//hide because there is no search sidebar aware controls on the page
            this.Controls.Clear();
            this.Visible = false;
		}
        Trace.Write(this.GetType().ToString(), "End PreRender");
    }

	private bool IsPageSearchSidebarAware()
	{
		Control ctl = PageHelper.FindSearchSidebarAwareControl(this.Page);
		return (ctl!= null);
	}

    private void LoadCustomViewState()
    {
        if (Page.IsPostBack)
        {
            UrlEncodedDictionary customViewState = new UrlEncodedDictionary(EncryptionHelper.DecryptAES(Request.Form[VS.UniqueID]));
            CategoryId = AlwaysConvert.ToInt(customViewState.TryGetValue("C"));
			ManufacturerId = AlwaysConvert.ToInt(customViewState.TryGetValue("M"));
			Keyword = customViewState.TryGetValue("K");
        }
    }

    private void SaveCustomViewState()
    {
        UrlEncodedDictionary customViewState = new UrlEncodedDictionary();
        customViewState["C"] = _CategoryId.ToString();
		customViewState["M"] = _ManufacturerId.ToString();
		customViewState["K"] = _Keyword;
        customViewState["SALT"] = StringHelper.RandomString(6);
        VS.Value = EncryptionHelper.EncryptAES(customViewState.ToString());
    }

    protected void BindExpandResultPanel()
    {
        Trace.Write(this.GetType().ToString(), "Begin BindExpandResultPanel");
        //START WITH THE PANEL VISIBLE
        ExpandResultPanel.Visible = true;
        //BIND THE EXPAND CATEGORY LINKS
        List<CatalogPathNode> currentPath = CatalogDataSource.GetPath(this.CategoryId, false);
        if (currentPath.Count > 0)
        {
            ExpandCategoryLinks.Visible = true;
            ExpandCategoryLinks.DataSource = currentPath;
            ExpandCategoryLinks.DataBind();
        }
        else
        {
            ExpandCategoryLinks.Visible = false;
        }
        //BIND THE EXPAND MANUFACTURER LINK
        Manufacturer m = ManufacturerDataSource.Load(this.ManufacturerId);
        if (m != null)
        {
            ExpandManufacturerLink.Text = string.Format("{0} (X)", m.Name);
            ExpandManufacturerLink.Visible = true;
        }
        else
        {
            ExpandManufacturerLink.Visible = false;
        }
        //BIND THE EXPAND KEYWORD LINK
        if (!string.IsNullOrEmpty(this.Keyword))
        {
            ExpandKeywordLink.Text = string.Format("Remove Keyword: {0}", Server.HtmlEncode(this.Keyword));
            ExpandKeywordLink.Visible = true;
        }
        else
        {
            ExpandKeywordLink.Visible = false;
        }
        //SET VISIBILITY OF EXPAND PANEL BASED ON CHILD CONTROLS VISIBILITY
        ExpandResultPanel.Visible = (ExpandCategoryLinks.Visible || ExpandManufacturerLink.Visible || ExpandKeywordLink.Visible);
        Trace.Write(this.GetType().ToString(), "End BindExpandResultPanel");
    }

    #region NarrowByCategoryPanel
    protected void BindNarrowByCategoryPanel()
    {
        Trace.Write(this.GetType().ToString(), "Begin BindNarrowByCategoryPanel");
        Trace.Write(this.GetType().ToString(), "Load All Child Categories");
        CategoryCollection allCategories = CategoryDataSource.LoadForParent(this.CategoryId, true);
        List<NarrowByCategoryData> populatedCategories = new List<NarrowByCategoryData>();
        foreach (Category category in allCategories)
        {
            Trace.Write(this.GetType().ToString(), "Count Items in " + category.Name);
            int totalProducts = ProductDataSource.NarrowSearchCount(this.Keyword, category.CategoryId, this.ManufacturerId, 0, 0);
            if (totalProducts > 0)
            {
                populatedCategories.Add(new NarrowByCategoryData(category.CategoryId, category.Name, totalProducts));
            }
        }
        Trace.Write(this.GetType().ToString(), "CheckBox Populated Categories");
        if (populatedCategories.Count > 0)
        {
            NarrowByCategoryPanel.Visible = true;
            NarrowByCategoryLinks.DataSource = populatedCategories;
            NarrowByCategoryLinks.DataBind();
        }
        else NarrowByCategoryPanel.Visible = false;
        SearchFilterAjaxPanel.Update();
        Trace.Write(this.GetType().ToString(), "End BindNarrowByCategoryPanel");
    }

    public class NarrowByCategoryData
    {
        private int _CategoryId;
        private string _Name;
        private int _ProductCount;
        public int CategoryId { get { return _CategoryId; } }
        public string Name { get { return _Name; } }
        public int ProductCount { get { return _ProductCount; } }
        public NarrowByCategoryData(int categoryId, string name, int productCount)
        {
            _CategoryId = categoryId;
            _Name = name;
            _ProductCount = productCount;
        }
    }
    #endregion

    #region NarrowByManufacturerPanel
    protected void BindNarrowByManufacturerPanel()
    {
        Trace.Write(this.GetType().ToString(), "Begin BindNarrowByManufacturerPanel");
        if (!ExpandManufacturerLink.Visible)
        {
            NarrowByManufacturerPanel.Visible = true;
            if (_Manufacturers.Count > _MaximumManufacturers)
            {
                if (ShowAllManufacturers.Visible)
                {
                    _Manufacturers.RemoveRange(_MaximumManufacturers, _Manufacturers.Count - _MaximumManufacturers);
                }
            }
            else ShowAllManufacturers.Visible = false;
            ManufacturerList.DataSource = _Manufacturers;
            ManufacturerList.DataBind();
            NarrowByManufacturerPanel.Visible = (ManufacturerList.Items.Count > 0);
        }
        else NarrowByManufacturerPanel.Visible = false;
        SearchFilterAjaxPanel.Update();
        Trace.Write(this.GetType().ToString(), "End BindNarrowByManufacturerPanel");
    }

    #endregion

    #region NarrowByKeywordPanel
    protected void BindNarrowByKeywordPanel()
    {
        Trace.Write(this.GetType().ToString(), "Begin BindNarrowByKeywordPanel");
        if (string.IsNullOrEmpty(Keyword))
        {
            NarrowByKeywordPanel.Visible = true;
        }
        else
        {
            NarrowByKeywordPanel.Visible = false;
        }
        Trace.Write(this.GetType().ToString(), "End BindNarrowByKeywordPanel");
    }

    #endregion


    protected void ManufacturerList_ItemDataBound(object source, DataListItemEventArgs e)
    {
        ProductDataSource.ManufacturerProductCount m = (ProductDataSource.ManufacturerProductCount)e.Item.DataItem;
        LinkButton NBMLink = new LinkButton();
        NBMLink.ID = "NBM_" + m.ManufacturerId;
        NBMLink.Text = string.Format("{0} ({1})", m.Name, m.ProductCount);
		NBMLink.CssClass = "searchCriteria";
        e.Item.Controls.Add(NBMLink);
    }

    protected void NarrowByManufacturer(string manufacturerControlId)
    {
		if(string.IsNullOrEmpty(manufacturerControlId)) return;
		int index = manufacturerControlId.LastIndexOf("NBM_");
		if(index < 0) return;		
		int manufacturerId = AlwaysConvert.ToInt(manufacturerControlId.Substring(index+4));
        if (manufacturerId == 0) return;		
		Manufacturer m = ManufacturerDataSource.Load(manufacturerId);
        if (m == null) return;

		ExpandManufacturerLink.Text = string.Format("{0} (X)", m.Name);
		ExpandManufacturerLink.Visible = true;
		this.ManufacturerId = manufacturerId;
		if (SidebarUpdated != null) SidebarUpdated(this, new EventArgs());
    }

    protected void RemoveManufacturer()
    {
        ExpandManufacturerLink.Visible = false;        
        this.ManufacturerId = 0;
        ShowAllManufacturers.Visible = true;
        if (SidebarUpdated != null) SidebarUpdated(this, new EventArgs());
    }

	protected void NarrowByCategoryLinks_ItemDataBound(object source, DataListItemEventArgs e)
	{
		NarrowByCategoryData nbcd = (NarrowByCategoryData) e.Item.DataItem;
        LinkButton NBCLink = new LinkButton();
        NBCLink.ID = "NBC_" + nbcd.CategoryId;
        NBCLink.Text = string.Format("{0} ({1})", nbcd.Name, nbcd.ProductCount);
		NBCLink.CssClass = "searchCriteria";
        e.Item.Controls.Add(NBCLink);
	}

    protected void NarrowByCategory(string categoryControlId)
    {
		if(string.IsNullOrEmpty(categoryControlId)) return;
		int index = categoryControlId.LastIndexOf("NBC_");
		if(index < 0) return;		
		int categoryId = AlwaysConvert.ToInt(categoryControlId.Substring(index+4),-1);
        if (categoryId < 0) return;
		
		this.CategoryId = categoryId;
		if (SidebarUpdated != null) SidebarUpdated(this, new EventArgs());
    }

	protected void ExpandCategoryLinks_ItemDataBound(object source, RepeaterItemEventArgs e)
	{
		CatalogPathNode cpn = (CatalogPathNode) e.Item.DataItem;
        LinkButton EBCLink = new LinkButton();
        EBCLink.ID = "EBC_" + cpn.CategoryId;
        EBCLink.Text = string.Format("{0} (X)", cpn.Name);
		EBCLink.CssClass = "searchCriteria";
        e.Item.Controls.Add(EBCLink);
	}

    protected void RemoveCategory(string categoryControlId)
    {
		if(string.IsNullOrEmpty(categoryControlId)) return;
		int index = categoryControlId.LastIndexOf("EBC_");
		if(index < 0) return;		
		int categoryId = AlwaysConvert.ToInt(categoryControlId.Substring(index+4),-1);
        if (categoryId < 0) return;
		
		this.CategoryId = categoryId;
		if (SidebarUpdated != null) SidebarUpdated(this, new EventArgs());
    }

	protected void NarrowByKeyword()
	{
		string kw = StringHelper.StripHtml(Request.Form[KeywordField.UniqueID]);
		if(!string.IsNullOrEmpty(kw) && kw != _Keyword && KeywordValidator.EvaluateIsValid(kw))
		{
			Keyword = kw;
			if (SidebarUpdated != null) SidebarUpdated(this, new EventArgs());
		}
	}

	protected void RemoveKeyword()
	{
        ExpandKeywordLink.Visible = false;
        this.Keyword = string.Empty;
        if (SidebarUpdated != null) SidebarUpdated(this, new EventArgs());
	}

	protected void ShowAllManufacturersLink()
	{
        ShowAllManufacturers.Visible = false;
        if (SidebarUpdated != null) SidebarUpdated(this, new EventArgs());
	}

}

