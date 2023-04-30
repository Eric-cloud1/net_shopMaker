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
using MakerShop.Utility;
using MakerShop.Stores;
using MakerShop.Search;
using MakerShop.Common;
using System.Collections.Generic;
using System.Text;
using ComponentArt.Web;

public partial class Admin_UserControls_AdminHeader : System.Web.UI.UserControl
{

    protected void Menu_DataBound(object sender, EventArgs e)
    {
        SetMenuNewWindow(this.AdminMenu.Items);
    }
    public static void SetMenuNewWindow(ComponentArt.Web.UI.MenuItemCollection mic)
    {
        foreach (ComponentArt.Web.UI.MenuItem mi in mic)
            SetMenuNewWindow(mi);
    }

    public static void SetMenuNewWindow(ComponentArt.Web.UI.MenuItem mi)
    {

        foreach (ComponentArt.Web.UI.MenuItem MI in mi.Items)
        {
            if (MI.NavigateUrl.ToUpper().Contains("HTTP"))
                MI.Target = "_new";
        }
        
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        //componentart does not seem to rebind when viewstate disabled
        //force it to rebind on postback

        if (!IsPostBack)
        {

            if (AdminMenu.Items.Count >= 8)
            {
                if (AdminMenu.Items[7].Items.Count >= 2)
                {
                    AdminMenu.Items[7].Items[1].Target = "_blank";
                }
            }            
        }

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

        if (Page.IsPostBack)
        {
            AdminMenu.DataBind();
        }

        //JAVA SCRIPT TO SAVE ONE FORM POSTBACK
        StringBuilder script = new StringBuilder();
        script.Append("if (" + SearchPhrase.ClientID + ".value == '') return false;");
        script.Append(@"window.location='").Append(Page.ResolveClientUrl("~/Admin/Search.aspx"))
            .Append("?k='+").Append(SearchPhrase.ClientID).Append(".value")
            .Append("+")
            .Append("'&s='+").Append(SearchFilter.ClientID).Append(".value")
            .Append(";return false;");
        SearchButton.Attributes.Add("onclick", script.ToString());
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Token.Instance.User == null || Token.Instance.User.IsAdmin == false)
            {
                SearchPanel.Visible = false;
            }
        }
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        string safeSearchPhrase = StringHelper.StripHtml(SearchPhrase.Text);
        if (!string.IsNullOrEmpty(safeSearchPhrase))
            Response.Redirect("~/Admin/Search.aspx?k=" + Server.UrlEncode(safeSearchPhrase) + "&s=" + SearchFilter.SelectedValue);
        Response.Redirect("~/Admin/Search.aspx");
    }
}
