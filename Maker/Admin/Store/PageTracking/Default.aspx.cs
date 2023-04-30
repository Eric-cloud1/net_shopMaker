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

public partial class Admin_Store_PageTracking_Default : MakerShop.Web.UI.MakerShopAdminPage
{

    StoreSettingCollection _Settings;
    
    private void SaveData() {
        _Settings.PageViewTrackingEnabled = TrackPageViews.Checked;
        _Settings.PageViewTrackingSaveArchive = (SaveArchive.SelectedIndex == 1);
        int tempDays = AlwaysConvert.ToInt(HistoryLength.Text);
        if (tempDays < 0) tempDays = 0;
        _Settings.PageViewTrackingDays = tempDays;
        _Settings.Save();
    }
    
    protected void SaveButton_Click(object sender, System.EventArgs e) {
        SaveData();
        ResponseMessage.Visible = true;
    }

    protected void SaveButtonGA_Click(object sender, System.EventArgs e) {
        _Settings.GoogleUrchinId = GooleUrchinId.Text;
		_Settings.EnableGoogleAnalyticsPageTracking=EnablePageTracking.Checked;
		_Settings.EnableGoogleAnalyticsEcommerceTracking=EnableEcommerceTracking.Checked;		
		_Settings.Save();
        ResponseMessageGA.Visible = true;
    }

    protected void CancelButton_Click(object sender, System.EventArgs e) {
        Response.Redirect("~/Admin/Default.aspx");
    }
    
    protected void SaveAndCloseButton_Click(object sender, System.EventArgs e) {
        SaveData();
        Response.Redirect("~/Admin/Default.aspx");
    }

    protected void SaveAndCloseButtonGA_Click(object sender, System.EventArgs e) {
        _Settings.GoogleUrchinId = GooleUrchinId.Text;
		_Settings.EnableGoogleAnalyticsPageTracking=EnablePageTracking.Checked;
		_Settings.EnableGoogleAnalyticsEcommerceTracking=EnableEcommerceTracking.Checked;
		_Settings.Save();
        Response.Redirect("~/Admin/Default.aspx");
    }
	
    protected void Page_Load(object sender, System.EventArgs e) {
        _Settings = Token.Instance.Store.Settings;
        if (!Page.IsPostBack)
        {
            TrackPageViews.Checked = _Settings.PageViewTrackingEnabled;
            HistoryLength.Text = _Settings.PageViewTrackingDays.ToString();
            CurrentRecords.Text = PageViewDataSource.CountForStore().ToString();
            SaveArchive.SelectedIndex = (_Settings.PageViewTrackingSaveArchive ? 1 : 0);
            SaveArchiveWarningPanel.Visible = (SaveArchive.SelectedIndex == 1);
			GooleUrchinId.Text = _Settings.GoogleUrchinId;
			EnablePageTracking.Checked = _Settings.EnableGoogleAnalyticsPageTracking;
			EnableEcommerceTracking.Checked = _Settings.EnableGoogleAnalyticsEcommerceTracking;
        }
		ResponseMessageGA.Visible = false;
		ResponseMessage.Visible = false;
    }

    protected void ClearButton_Click(object sender, EventArgs e)
    {
        PageViewDataSource.DeleteAll();
        CurrentRecords.Text = PageViewDataSource.CountForStore().ToString();
    }

    protected void ViewButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("ViewLog.aspx");
    }

    protected void SaveArchive_SelectedIndexChanged(object sender, EventArgs e)
    {
        SaveArchiveWarningPanel.Visible = (SaveArchive.SelectedIndex == 1);
    }

}
