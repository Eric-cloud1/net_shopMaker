using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using MakerShop.Catalog;
using MakerShop.Data;
using MakerShop.DigitalDelivery;
using MakerShop.Marketing;
using MakerShop.Messaging;
using MakerShop.Orders;
using MakerShop.Payments;
using MakerShop.Payments.Providers;
using MakerShop.Products;
using MakerShop.Reporting;
using MakerShop.Shipping;
using MakerShop.Stores;
using MakerShop.Taxes;
using MakerShop.Taxes.Providers;
using MakerShop.Users;
using MakerShop.Utility;

public partial class Admin_Catalog_EditLink : MakerShop.Web.UI.MakerShopAdminPage
{

    private int _LinkId;
    private Link _Link;

    protected void Page_Init(object sender, EventArgs e)
    {
        PageHelper.SetPickImageButton(ThumbnailUrl, BrowseThumbnailUrl);
        PageHelper.SetHtmlEditor(Description, LinkDescriptionHtml);
        PageHelper.SetPageDefaultButton(Page, SaveButton);                

        PageHelper.PreventFirefoxSubmitOnKeyPress(Summary, Summary.ClientID);
        PageHelper.PreventFirefoxSubmitOnKeyPress(Description, Description.ClientID);
        PageHelper.PreventFirefoxSubmitOnKeyPress(HtmlHead, HtmlHead.ClientID);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        _LinkId = AlwaysConvert.ToInt(Request.QueryString["LinkId"]);
        _Link = LinkDataSource.Load(_LinkId);
        if (_Link == null) Response.Redirect("Browse.aspx?CategoryId=" + PageHelper.GetCategoryId().ToString());

		PreviewButton.NavigateUrl = UrlGenerator.GetBrowseUrl(_LinkId, CatalogNodeType.Link, _Link.Name);

        if (!Page.IsPostBack)
        {
            UpdateCaption();
            //NAME
            Name.Focus();
            Name.Text = _Link.Name;
            //VISIBILITY
            Visibility.SelectedIndex = (int)_Link.Visibility;
            //NAVIGATE URL
            TargetUrl.Text = _Link.TargetUrl;
            //TARGET
            ListItem targetItem = TargetWindow.Items.FindByValue(_Link.TargetWindow);
            if (targetItem != null) TargetWindow.SelectedIndex = TargetWindow.Items.IndexOf(targetItem);
            //THUMBNAIL
            ThumbnailUrl.Text = _Link.ThumbnailUrl;
            ThumbnailAltText.Text = _Link.ThumbnailAltText;
            //SUMARY
            Summary.Text = _Link.Summary;
            SummaryCharCount.Text = ((int)(Summary.MaxLength - Summary.Text.Length)).ToString();
            Description.Text = _Link.Description;
            //META
            HtmlHead.Text = _Link.HtmlHead;
            //DISPLAY PAGE
            BindDisplayPage();
            UpdateDescription();
            //THEMES
            BindThemes();
        }
        PageHelper.ConvertEnterToTab(Name);
        PageHelper.SetMaxLengthCountDown(Summary, SummaryCharCount);
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        // RETURN TO BROWSE PARENT CATEGORY
        Response.Redirect("~/Admin/Catalog/Browse.aspx?CategoryId=" + PageHelper.GetCategoryId().ToString());
    }

    protected void FinishButton_Click(object sender, EventArgs e)
    {
        SaveButton_Click(sender, e);
        // RETURN TO BROWSE PARENT CATEGORY
        Response.Redirect("~/Admin/Catalog/Browse.aspx?CategoryId=" + PageHelper.GetCategoryId().ToString());
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        _Link.Name = Name.Text;
        _Link.Visibility = (CatalogVisibility)Visibility.SelectedIndex;
        _Link.TargetUrl = TargetUrl.Text;
        _Link.TargetWindow = TargetWindow.SelectedValue;
        _Link.DisplayPage = DisplayPage.SelectedValue;
        _Link.Theme = LocalTheme.SelectedValue;
        _Link.Summary = StringHelper.Truncate(Summary.Text, Summary.MaxLength);
        _Link.Description = Description.Text;
        _Link.ThumbnailUrl = ThumbnailUrl.Text;
        _Link.ThumbnailAltText = ThumbnailAltText.Text;
        _Link.HtmlHead = HtmlHead.Text;
        _Link.Save();
		SavedMessage.Visible = true;
		SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
    }

    protected void UpdateCaption()
    {
        string shortLinkName = _Link.Name;
        if (shortLinkName.Length > 45) shortLinkName = shortLinkName.Substring(0, 40) + "...";
        Caption.Text = string.Format(Caption.Text, shortLinkName);
    }

    protected void BindDisplayPage()
    {
        List<MakerShop.UI.Styles.DisplayPage> displayPages = MakerShop.UI.Styles.DisplayPageDataSource.Load();
        foreach (MakerShop.UI.Styles.DisplayPage displayPage in displayPages)
        {
            if (displayPage.NodeType == CatalogNodeType.Link)
            {
                string displayName = string.Format("{0}", displayPage.Name);
                DisplayPage.Items.Add(new ListItem(displayName, displayPage.DisplayPageFile));
            }
        }
        if (!Page.IsPostBack)
        {
            string currentDisplayPage = _Link.DisplayPage;
            ListItem selectedItem = DisplayPage.Items.FindByValue(currentDisplayPage);
            if (selectedItem != null) DisplayPage.SelectedIndex = DisplayPage.Items.IndexOf(selectedItem);
        }
    }

    protected void DisplayPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateDescription();
    }

    private void UpdateDescription()
    {
        DisplayPageDescription.Text = "&nbsp;";
        if (DisplayPage.SelectedValue != string.Empty)
        {
            MakerShop.UI.Styles.DisplayPage page = MakerShop.UI.Styles.DisplayPageDataSource.ParseFromFile(Server.MapPath("~/" + DisplayPage.SelectedValue), "/");
            if (page != null)
            {
                DisplayPageDescription.Text = "<strong>" + DisplayPage.SelectedItem.Text + ":</strong> " + page.Description;
            }
        }
        else
        {
            DisplayPageDescription.Text = "<strong>" + DisplayPage.Items[0].Text + ":</strong> Displays a direct link to the specified url, eliminating the need for a display page.";
        }
    }

    protected void BindThemes()
    {
        LocalTheme.DataSource = StoreDataHelper.GetStoreThemes();
        LocalTheme.DataBind();
        if (!Page.IsPostBack)
        {
            string currentTheme = _Link.Theme;
            ListItem selectedItem = LocalTheme.Items.FindByValue(currentTheme);
            if (selectedItem != null) LocalTheme.SelectedIndex = LocalTheme.Items.IndexOf(selectedItem);
        }
    }

}
