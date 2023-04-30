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
using MakerShop.Shipping;

public partial class Admin_Shipping_Countries_Default : MakerShop.Web.UI.MakerShopAdminPage
{

    
    protected void CountryGrid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        string countryCode = CountryGrid.DataKeys[e.NewEditIndex].Value.ToString();
        Country country = CountryDataSource.Load(countryCode);
        if (country != null)
        {
            AddCountryDialog1.Visible = false;
            EditCountryDialog1.Visible = true;
            EditCountryDialog1.LoadDialog(countryCode);
        }
    }

    protected void CountryGrid_RowCancelingEdit(object sender, EventArgs e)
    {
        AddCountryDialog1.Visible = true;
        EditCountryDialog1.Visible = false;
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        AddCountryDialog1.ItemAdded += new EventHandler(AddCountryDialog1_ItemAdded);

        AlphabetRepeater.DataSource = GetAlphabetDS();
        AlphabetRepeater.DataBind();
    }

    protected string[] GetAlphabetDS()
    {
        string[] alphabet = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "All" };
        return alphabet;
    }

    protected void AlphabetRepeater_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        String searchPattern = String.Empty;
        if ((e.CommandArgument.ToString().Length == 1))
        {
            searchPattern = (e.CommandArgument.ToString());
        }

        if (!String.IsNullOrEmpty(searchPattern)) CountryDs.SelectParameters[0].DefaultValue = "Name LIKE '" + searchPattern + "%'";
        else CountryDs.SelectParameters[0].DefaultValue = String.Empty;
        
        CountryGrid.DataBind();
    }

    void AddCountryDialog1_ItemAdded(object sender, EventArgs e)
    {
        CountryGrid.DataBind();
    }

    protected void EditCountryDialog1_ItemUpdated(object sender, EventArgs e)
    {
        DoneEditing();
    }

    protected void EditCountryDialog1_Cancelled(object sender, EventArgs e)
    {
        DoneEditing();
    }

    protected void DoneEditing()
    {
        AddCountryDialog1.Visible = true;
        EditCountryDialog1.Visible = false;
        CountryGrid.EditIndex = -1;
        CountryGrid.DataBind();
    }


}
