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
using MakerShop.Web;

public partial class Admin_Website_ThemesAndLayouts : MakerShop.Web.UI.MakerShopAdminPage
{

    StoreSettingCollection _Settings;
    bool _RequireRedirect = false;
    
    private void SaveData() {
        _RequireRedirect = (_Settings.AdminTheme != AdminTheme.SelectedValue);
        string oldStoreTheme = _Settings.StoreTheme;
        _Settings.StoreTheme = StoreTheme.SelectedValue;
        _Settings.AdminTheme = AdminTheme.SelectedValue;
        _Settings.CategoryDisplayPage = CategoryDisplayPage.SelectedValue;
        _Settings.ProductDisplayPage = ProductDisplayPage.SelectedValue;
        _Settings.WebpageDisplayPage = WebpageDisplayPage.SelectedValue;
        _Settings.Save();
        if (string.Compare(oldStoreTheme, _Settings.StoreTheme, true) != 0)
        {
            WebflowManager.ClearCache();
        }
    }
    
    protected void SaveButton_Click(object sender, System.EventArgs e) {
        SaveData();
        if (_RequireRedirect)
        {
            //THIS REDIRECT IS REQUIRED IN ORDER TO SHOW THE UPDATED STYLING (IF NEEDED)
            Response.Redirect("ThemesAndLayouts.aspx?Updated=1");
        }
        else ResponseMessage.Visible = true;
    }
    
    protected void Page_Load(object sender, System.EventArgs e) {
        _Settings = Token.Instance.Store.Settings;
        ResponseMessage.Visible = !string.IsNullOrEmpty(Request.QueryString["Updated"]);
        if (!Page.IsPostBack)
        {
            BindThemes();
            BindDisplayPages();
        }
    }

    protected void BindThemes()
    {
        List<MakerShop.UI.Styles.Theme> themes = MakerShop.UI.Styles.ThemeDataSource.Load();
        foreach (MakerShop.UI.Styles.Theme theme in themes)
        {
            if (!theme.IsAdminTheme)
            {
                //THIS IS A STORE THEME
                StoreTheme.Items.Add(new ListItem(theme.DisplayName, theme.Name));
            } else {
                //THIS IS AN ADMIN THEME
                AdminTheme.Items.Add(new ListItem(theme.DisplayName, theme.Name));
            }
        }
        string currentStoreTheme = _Settings.StoreTheme;
        ListItem selectedItem = StoreTheme.Items.FindByValue(currentStoreTheme);
        if (selectedItem != null) selectedItem.Selected = true;
        string currentAdminTheme = _Settings.AdminTheme;
        selectedItem = AdminTheme.Items.FindByValue(currentAdminTheme);
        if (selectedItem != null) selectedItem.Selected = true;
    }

    protected void BindDisplayPages()
    {
        List<MakerShop.UI.Styles.DisplayPage> displayPages = MakerShop.UI.Styles.DisplayPageDataSource.Load();
        foreach (MakerShop.UI.Styles.DisplayPage displayPage in displayPages)
        {
            string displayName = string.Format("{0} ({1})", displayPage.Name, displayPage.DisplayPageFile);
            switch(displayPage.NodeType) {
                case CatalogNodeType.Category:
                    CategoryDisplayPage.Items.Add(new ListItem(displayName, displayPage.DisplayPageFile));
                    break;
                case CatalogNodeType.Product:
                    ProductDisplayPage.Items.Add(new ListItem(displayName, displayPage.DisplayPageFile));
                    break;
                case CatalogNodeType.Webpage:
                    WebpageDisplayPage.Items.Add(new ListItem(displayName, displayPage.DisplayPageFile));
                    break;
                //THIS IS A STORE THEME
            }
        }
        string currentCategoryDisplayPage = _Settings.CategoryDisplayPage;
        ListItem selectedItem = CategoryDisplayPage.Items.FindByValue(currentCategoryDisplayPage);
        if (selectedItem != null)
        {
            selectedItem.Selected = true;
            if (selectedItem.Value != string.Empty) UpdateDescription(CatalogNodeType.Category);
        }
        string currentProductDisplayPage = _Settings.ProductDisplayPage;
        selectedItem = ProductDisplayPage.Items.FindByValue(currentProductDisplayPage);
        if (selectedItem != null)
        {
            selectedItem.Selected = true;
            if (selectedItem.Value != string.Empty) UpdateDescription(CatalogNodeType.Product);
        }
        string currentWebpageDisplayPage = _Settings.WebpageDisplayPage;
        selectedItem = WebpageDisplayPage.Items.FindByValue(currentWebpageDisplayPage);
        if (selectedItem != null)
        {
            selectedItem.Selected = true;
            if (selectedItem.Value != string.Empty) UpdateDescription(CatalogNodeType.Webpage);
        }
    }

    protected void CategoryDisplayPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateDescription(CatalogNodeType.Category);
    }

    protected void ProductDisplayPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateDescription(CatalogNodeType.Product);
    }

    protected void WebpageDisplayPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateDescription(CatalogNodeType.Webpage);
    }

    private void UpdateDescription(CatalogNodeType nodeType)
    {
        switch (nodeType)
        {
            case CatalogNodeType.Category:
                if (CategoryDisplayPage.SelectedValue == string.Empty) CategoryDisplayPageDescription.Text = "Select a display page to see a description.";
                else
                {
                    CategoryDisplayPageDescription.Text = "&nbsp;";
                    MakerShop.UI.Styles.DisplayPage page = MakerShop.UI.Styles.DisplayPageDataSource.ParseFromFile(Server.MapPath("~/" + CategoryDisplayPage.SelectedValue), "/");
                    if (page != null) CategoryDisplayPageDescription.Text = "<strong>" + CategoryDisplayPage.SelectedItem.Text + ":</strong> " + page.Description;
                }
                break;
            case CatalogNodeType.Product:
                if (ProductDisplayPage.SelectedValue == string.Empty) ProductDisplayPageDescription.Text = "Select a display page to see a description.";
                else
                {
                    ProductDisplayPageDescription.Text = "&nbsp;";
                    MakerShop.UI.Styles.DisplayPage page = MakerShop.UI.Styles.DisplayPageDataSource.ParseFromFile(Server.MapPath("~/" + ProductDisplayPage.SelectedValue), "/");
                    if (page != null) ProductDisplayPageDescription.Text = "<strong>" + ProductDisplayPage.SelectedItem.Text + ":</strong> " + page.Description;
                }
                break;
            case CatalogNodeType.Webpage:
                if (WebpageDisplayPage.SelectedValue == string.Empty) WebpageDisplayPageDescription.Text = "Select a display page to see a description.";
                else
                {
                    WebpageDisplayPageDescription.Text = "&nbsp;";
                    MakerShop.UI.Styles.DisplayPage page = MakerShop.UI.Styles.DisplayPageDataSource.ParseFromFile(Server.MapPath("~/" + WebpageDisplayPage.SelectedValue), "/");
                    if (page != null) WebpageDisplayPageDescription.Text = "<strong>" + WebpageDisplayPage.SelectedItem.Text + ":</strong> " + page.Description;
                }
                break;
            case CatalogNodeType.Link:
                break;
        }
    }

}
