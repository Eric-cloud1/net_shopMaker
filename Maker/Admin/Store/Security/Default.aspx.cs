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

public partial class Admin_Store_Security_Default : MakerShop.Web.UI.MakerShopAdminPage
{

    private const int MAX_PAYMENT_LIFESPAN = 60;
    private StoreSettingCollection settings;
    
    protected void Page_Load(object sender, EventArgs e)
    {
        settings = Token.Instance.Store.Settings;
        if (!Page.IsPostBack)
        {
            ShowViewSSL_Panel();
            // PAYMENT SECURITY
            for (int i = 0; i <= MAX_PAYMENT_LIFESPAN; i++)
            {
                PaymentLifespan.Items.Add(i.ToString());
            }
            ListItem item = PaymentLifespan.Items.FindByValue(settings.PaymentLifespan.ToString());
            if (item != null) item.Selected = true;
            EnableCreditCardStorage.Checked = settings.EnableCreditCardStorage;
        }        
    }

    protected void SaveButon_Click(object sender, EventArgs e)
    {
        settings.PaymentLifespan = AlwaysConvert.ToInt(PaymentLifespan.SelectedValue);
        settings.EnableCreditCardStorage = EnableCreditCardStorage.Checked;
        settings.Save();
        SavedMessage.Visible = true;
    }

    protected void ChangeSSL_NextButton_Click(object sender, EventArgs e)
    {
        ChangeSSL_SecureDomain.Enabled = false;
        ChangeSSL_ConfirmPanel.Visible = true;
        string tempDomain = ChangeSSL_SecureDomain.Text;
        if (tempDomain == string.Empty) tempDomain = Request.Url.Host;
        string testUrl = MakerShop.Configuration.SSLHelper.GetSecureUrl(tempDomain, Page.ResolveUrl("~/Default.aspx"), string.Empty);        
        ChangeSSL_ConfirmLink.Text = testUrl;
        ChangeSSL_ConfirmLink.NavigateUrl = testUrl;
        ChangeSSL_Confirmed.Checked = false;
        ChangeSSL_NextButton.Visible = false;
    }

    protected String GetSecurePostBackUrl()
    {
        string tempDomain = ChangeSSL_SecureDomain.Text;
        if (tempDomain == string.Empty) tempDomain = Request.Url.Host;
        return MakerShop.Configuration.SSLHelper.GetSecureUrl(tempDomain, Page.ResolveUrl("Default.aspx"), string.Empty);        
    }

    protected void ShowViewSSL_Panel()
    {
        ChangeSSL_Panel.Visible = false;
        ViewSSL_Panel.Visible = true;
        SSLEnabled.Visible = settings.SSLEnabled;
        SSLDisabled.Visible = !settings.SSLEnabled;
        SecureDomain.Text = settings.EncryptedUri;
        SecureDomainPanel.Visible = (SecureDomain.Text != string.Empty);
    }

    protected void ChangeSSL_Confirmed_CheckChanged(object sender, EventArgs e)
    {
        ChangeSSL_FinishButton.Visible = ChangeSSL_Confirmed.Checked;
        ChangeSSL_FinishButton.PostBackUrl = GetSecurePostBackUrl();
    }

    protected void ChangeSSLButton_Click(object sender, EventArgs e)
    {
        ViewSSL_Panel.Visible = false;
        ChangeSSL_Panel.Visible = true;
        ChangeSSL_Enable.Checked = false;
        ChangeSSL_SecureDomainPanel.Visible = false;
        ChangeSSL_ConfirmPanel.Visible = false;
        ChangeSSL_NextButton.Visible = false;
        ChangeSSL_FinishButton.Visible = true;        
    }

    protected void ChangeSSL_CancelButton_Click(object sender, EventArgs e)
    {
        ChangeSSL_Panel.Visible = false;
        ViewSSL_Panel.Visible = true;
    }

    protected void ChangeSSL_Enable_CheckedChanged(object sender, EventArgs e)
    {
        if (ChangeSSL_Enable.Checked)
        {
            ChangeSSL_SecureDomainPanel.Visible = true;
            ChangeSSL_SecureDomain.Text = string.Empty;
            ChangeSSL_SecureDomain.Enabled = true;
            ChangeSSL_NextButton.Visible = true;
            ChangeSSL_FinishButton.Visible = false;
        }
        else
        {
            ChangeSSL_SecureDomainPanel.Visible = false;
            ChangeSSL_ConfirmPanel.Visible = false;
            ChangeSSL_NextButton.Visible = false;
            ChangeSSL_FinishButton.Visible = true;            
        }
    }

    private static string GetUnencryptedUri(string storeUrl)
    {
        Match m = Regex.Match(storeUrl, "^https?://(.+?)/");
        if (m.Success)
        {
            return m.Groups[1].Value;
        }
        return string.Empty;
    }

    protected void ChangeSSL_FinishButton_Click(object sender, EventArgs e)
    {
        settings.SSLEnabled = ChangeSSL_Enable.Checked;
        settings.EncryptedUri = ChangeSSL_Enable.Checked ? ChangeSSL_SecureDomain.Text : string.Empty;
        if (!string.IsNullOrEmpty(settings.EncryptedUri))
            settings.UnencryptedUri = GetUnencryptedUri(Token.Instance.Store.StoreUrl);
        else settings.UnencryptedUri = string.Empty;
        settings.Save();
        ShowViewSSL_Panel();
    }
    

}
