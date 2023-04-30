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
using MakerShop.Messaging;

public partial class Admin_Store_ProductReviewSettings : MakerShop.Web.UI.MakerShopAdminPage
{

    protected void Page_Init(object sender, System.EventArgs e)
    {
        EmailTemplate.DataSource = EmailTemplateDataSource.LoadForStore("Name");
        EmailTemplate.DataBind();
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            //INITIALIZE FORM
            StoreSettingCollection settings = Token.Instance.Store.Settings;
            ListItem item;
            item = Enabled.Items.FindByValue(((int)settings.ProductReviewEnabled).ToString());
            if (item != null) item.Selected = true;
            item = Approval.Items.FindByValue(((int)settings.ProductReviewApproval).ToString());
            if (item != null) item.Selected = true;
            item = ImageVerification.Items.FindByValue(((int)settings.ProductReviewImageVerification).ToString());
            if (item != null) item.Selected = true;
            item = EmailVerification.Items.FindByValue(((int)settings.ProductReviewEmailVerification).ToString());
            if (item != null) item.Selected = true;
            item = EmailTemplate.Items.FindByValue(settings.ProductReviewEmailVerificationTemplate.ToString());
            if (item != null) item.Selected = true;
            TermsAndConditions.Text = settings.ProductReviewTermsAndConditions;
         }
    }

    private void SaveSettings()
    {
        //Load store and update
        StoreSettingCollection settings = Token.Instance.Store.Settings;
        settings.ProductReviewEnabled = (UserAuthFilter)AlwaysConvert.ToInt(Enabled.SelectedValue);
        settings.ProductReviewApproval = (UserAuthFilter)AlwaysConvert.ToInt(Approval.SelectedValue);
        settings.ProductReviewImageVerification = (UserAuthFilter)AlwaysConvert.ToInt(ImageVerification.SelectedValue);
        settings.ProductReviewEmailVerification = (UserAuthFilter)AlwaysConvert.ToInt(EmailVerification.SelectedValue);
        settings.ProductReviewEmailVerificationTemplate = AlwaysConvert.ToInt(EmailTemplate.SelectedValue);
        settings.ProductReviewTermsAndConditions = TermsAndConditions.Text;
        settings.Save();
    }
    
    protected void SaveButton_Click(object sender, EventArgs e)
    {
        SaveSettings();
        SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
        SavedMessage.Visible = true;
    }
    
    protected void SaveAndCloseButton_Click(object sender, EventArgs e)
    {
        SaveSettings();
        Response.Redirect("../Default.aspx");
    }

}
