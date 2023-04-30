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
using System.Text.RegularExpressions;
using MakerShop.Common;
using MakerShop.Catalog;
using MakerShop.Orders;
using MakerShop.Products;
using MakerShop.Stores;
using MakerShop.Users;
using MakerShop.Utility;
using MakerShop.Reporting;
using System.Collections.Generic;
using MakerShop.Search;
using System.Text;

public partial class Admin_Search : MakerShop.Web.UI.MakerShopAdminPage
{
    int _InitialResultsDisplayed = 5;
    protected List<SearchAreaResults> _SearchAreaResults;

    String CategoryUrl = "~/Admin/Catalog/EditCategory.aspx?CategoryId={0}";
    String WebpageUrl = "~/Admin/Catalog/EditWebpage.aspx?WebpageId={0}";
    String LinkUrl = "~/Admin/Catalog/EditLink.aspx?LinkId={0}";
    String DigitalGoodUrl = "~/Admin/DigitalGoods/EditDigitalGood.aspx?DigitalGoodId={0}";
    String WarehouseUrl = "~/Admin/Shipping/Warehouses/EditWarehouse.aspx?WarehouseId={0}";
    String ProductUrl = "~/Admin/Products/EditProduct.aspx?ProductId={0}";
    String UserUrl = "~/Admin/People/Users/EditUser.aspx?UserId={0}";

    private String _Keywords = String.Empty;
    protected String Keywords
    {
        get
        {
            if (String.IsNullOrEmpty(_Keywords))
            {
                if(String.IsNullOrEmpty(Request.QueryString["k"]))
                {
                    _Keywords = String.Empty;
                }
                else _Keywords = Request.QueryString["k"].Trim();
            }
            return _Keywords;
        }
        set
        {
            _Keywords = value;
        }
    }

    private List<SearchArea> _SearchAreas;
    protected List<SearchArea> SearchAreas
    {
        get
        {
            if (_SearchAreas == null)
            {
                _SearchAreas = new List<SearchArea>();
                String sAreas = Request.QueryString["s"];
                if (!String.IsNullOrEmpty(sAreas))
                {
                    String[] arrAreas = sAreas.Split(',');
                    foreach (String area in arrAreas)
                    {
                        _SearchAreas.Add((SearchArea)AlwaysConvert.ToInt(area));
                    }
                }                
            }
            return _SearchAreas;
        }
        set
        {
            _SearchAreas = value;
        }
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if(!Page.IsPostBack)
        {
            Dictionary<String, int> searchAreas = new Dictionary<string, int>();
            // get the names from the enumeration
            string[] names = Enum.GetNames(typeof(SearchArea));
            // get the values from the enumeration
            Array values = Enum.GetValues(typeof(SearchArea));
            searchAreas.Add("Search All", 0);
            for (int i = 1; i < names.Length; i++)
                searchAreas.Add(names[i], (int)values.GetValue(i));

            SearchFilter.DataSource = searchAreas;
            SearchFilter.DataTextField = "Key";
            SearchFilter.DataValueField = "Value";
            SearchFilter.DataBind();

            // UPDATE SEARCH FORM AS WELL
            SearchPhrase.Text = Keywords;
            if (SearchAreas.Count > 0)
            {
                ListItem item = SearchFilter.Items.FindByText(SearchAreas[0].ToString());
                if (item != null) item.Selected = true;
            }

            // SET DEFAULT BUTTONS
            PageHelper.SetDefaultButton(SearchPhrase, SearchButton.ClientID);

            if (String.IsNullOrEmpty(Keywords) || Keywords.Length < 3)
            {
                Page.Validate("AdminSearch");
                ResultsPanel.Visible = false;
                return;
            }

            if (!String.IsNullOrEmpty(Keywords))
            {
                _SearchAreaResults = SearchDataSource.Search(Keywords, SearchAreas, _InitialResultsDisplayed);
                if (_SearchAreaResults.Count > 0)
                {
                    SearchAreasRepeater.DataSource = _SearchAreaResults;
                    SearchAreasRepeater.DataBind();
                }
                else
                {
                    NoResultsLabel.Visible = true;
                    SearchAreasRepeater.Visible = false;
                }
            }

            
        }
        else BindSearchResults();

        if (String.IsNullOrEmpty(Keywords) || Keywords.Length < 3) ResultsPanel.Visible = false;
        else SearchCaption.Text = String.Format(SearchCaption.Text, Keywords);        
    }

    protected void Repeater_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {                 
        PlaceHolder ResultsPH = (PlaceHolder)e.Item.FindControl("ResultsPH");

        SearchAreaResults searchAreaResults = (SearchAreaResults)e.Item.DataItem;
        AddLinks(ResultsPH,searchAreaResults, true);
    }

    private void AddLinks(PlaceHolder ResultsPH, SearchAreaResults searchAreaResults, bool ShowAllLink)
    {
        if (ResultsPH != null)
        {
            ResultsPH.Controls.Clear(); 
            foreach (SearchResult searchResult in searchAreaResults.SearchResults)
            {                
                ResultsPH.Controls.Add(new LiteralControl("<a href=\"" + Page.ResolveClientUrl(GetLinkUrl(searchAreaResults.SearchArea, searchResult.Id)) + "\">" + searchResult.Name + "</a><br/>"));
            }
            if (searchAreaResults.TotalMatches > _InitialResultsDisplayed && ShowAllLink)
            {
                LinkButton showAllLink = new LinkButton();
                showAllLink.Text = String.Format("See all {0} matches>>", searchAreaResults.TotalMatches);
                showAllLink.CommandName = "ShowAll";
                showAllLink.CommandArgument = searchAreaResults.SearchArea.ToString();
                ResultsPH.Controls.Add(showAllLink);
            }
        }
    }
    

    private void BindSearchResults()
    {
        Keywords = SearchPhrase.Text.Trim();
        SearchAreas.Clear();
        SearchAreas.Add((SearchArea)AlwaysConvert.ToInt(SearchFilter.SelectedValue));

        _SearchAreaResults = SearchDataSource.Search(Keywords, SearchAreas, _InitialResultsDisplayed);
        SearchAreasRepeater.DataSource = _SearchAreaResults;
        SearchAreasRepeater.DataBind();
    }

    protected String GetLinkUrl(object o, int Id)
    {
        SearchArea searchArea = (SearchArea)o;
        switch (searchArea)
        {
            case SearchArea.Categories:
                return String.Format(CategoryUrl,Id);
            case SearchArea.Webpages:
                return String.Format(WebpageUrl, Id);
            case SearchArea.Links:
                return String.Format(LinkUrl, Id);
            case SearchArea.DigitalGoods:
                return String.Format(DigitalGoodUrl, Id);
            case SearchArea.Warehouses:
                return String.Format(WarehouseUrl, Id);
            case SearchArea.Products:
                return String.Format(ProductUrl, Id);
            case SearchArea.Users:
                return String.Format(UserUrl, Id);              

            // THIS CASE WILL NEVER BE CALLED
            default: return string.Empty;
        }
    }

    protected void Repeater_OnItemCommand(object sender, RepeaterCommandEventArgs e)
    {        
        if (e.CommandName == "ShowAll")
        {
            PlaceHolder ResultsPH = (PlaceHolder)e.Item.FindControl("ResultsPH");
            SearchArea searchArea = (SearchArea)Enum.Parse(typeof(SearchArea), (string)e.CommandArgument);
            SearchAreaResults results = SearchDataSource.Search(Keywords, searchArea);
            AddLinks(ResultsPH, results,false);

            UpdatePanel ResultsAjax = (UpdatePanel)e.Item.FindControl("ResultsAjax");
            ResultsAjax.Update();           
        }
    }

    
}
