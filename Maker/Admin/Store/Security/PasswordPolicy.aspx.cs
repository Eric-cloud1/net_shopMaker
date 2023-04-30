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

public partial class Admin_Store_Security_PasswordPolicy : MakerShop.Web.UI.MakerShopAdminPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            MerchantPasswordPolicy merchantPolicy = new MerchantPasswordPolicy();
            MerchantMinLength.Text = merchantPolicy.MinLength.ToString();
            MerchantRequireUppercase.Checked = merchantPolicy.RequireUpper;
            MerchantRequireLowercase.Checked = merchantPolicy.RequireLower;
            MerchantRequireNumbers.Checked = merchantPolicy.RequireNumber;
            MerchantRequireSymbols.Checked = merchantPolicy.RequireSymbol;
            MerchantRequireNonAlpha.Checked = merchantPolicy.RequireNonAlpha;
            if (merchantPolicy.MaxAge > 0) MerchantMaxAge.Text = merchantPolicy.MaxAge.ToString();
            if (merchantPolicy.HistoryDays > 0) MerchantPasswordHistoryDays.Text = merchantPolicy.HistoryDays.ToString();
            if (merchantPolicy.HistoryCount > 0) MerchantPasswordHistoryCount.Text = merchantPolicy.HistoryCount.ToString();
            MerchantPasswordMaxAttempts.Text = merchantPolicy.MaxAttempts.ToString();
            MerchantPasswordLockoutPeriod.Text = merchantPolicy.LockoutPeriod.ToString();
            MerchantPasswordInactivePeriod.Text = merchantPolicy.InactivePeriod.ToString();
            MerchantImageCaptcha.Checked = merchantPolicy.ImageCaptcha;
            CustomerPasswordPolicy customerPolicy = new CustomerPasswordPolicy();           
            CustomerMinLength.Text = customerPolicy.MinLength.ToString();
            CustomerRequireUppercase.Checked = customerPolicy.RequireUpper;
            CustomerRequireLowercase.Checked = customerPolicy.RequireLower;
            CustomerRequireNumbers.Checked = customerPolicy.RequireNumber;
            CustomerRequireSymbols.Checked = customerPolicy.RequireSymbol;
            CustomerRequireNonAlpha.Checked = customerPolicy.RequireNonAlpha;
            if (customerPolicy.MaxAge > 0) CustomerMaxAge.Text = customerPolicy.MaxAge.ToString();
            if (customerPolicy.HistoryDays > 0) CustomerPasswordHistoryDays.Text = customerPolicy.HistoryDays.ToString();
            if (customerPolicy.HistoryCount > 0) CustomerPasswordHistoryCount.Text = customerPolicy.HistoryCount.ToString();
            CustomerPasswordMaxAttempts.Text = customerPolicy.MaxAttempts.ToString();
            CustomerPasswordLockoutPeriod.Text = customerPolicy.LockoutPeriod.ToString();
            CustomerImageCaptcha.Checked = customerPolicy.ImageCaptcha;
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        int merchantMinLength = AlwaysConvert.ToInt(MerchantMinLength.Text);
        int customerMinLength = AlwaysConvert.ToInt(CustomerMinLength.Text);
        if (merchantMinLength < 1)
        {
            MerchantMinLengthValidator1.IsValid = false;
        }
        if (customerMinLength < 1)
        {
            CustomerMinLengthValidator1.IsValid = false;
        }
        if (Page.IsValid)
        {
            MerchantPasswordPolicy merchantPolicy = new MerchantPasswordPolicy();
            merchantPolicy.MinLength = merchantMinLength;
            merchantPolicy.RequireUpper = MerchantRequireUppercase.Checked;
            merchantPolicy.RequireLower = MerchantRequireLowercase.Checked;
            merchantPolicy.RequireNumber = MerchantRequireNumbers.Checked;
            merchantPolicy.RequireSymbol = MerchantRequireSymbols.Checked;
            merchantPolicy.RequireNonAlpha = MerchantRequireNonAlpha.Checked;
            merchantPolicy.MaxAge = AlwaysConvert.ToInt(MerchantMaxAge.Text);
            merchantPolicy.HistoryDays = AlwaysConvert.ToInt(MerchantPasswordHistoryDays.Text);
            merchantPolicy.HistoryCount = AlwaysConvert.ToInt(MerchantPasswordHistoryCount.Text);
            merchantPolicy.MaxAttempts = AlwaysConvert.ToInt(MerchantPasswordMaxAttempts.Text);
            merchantPolicy.LockoutPeriod = AlwaysConvert.ToInt(MerchantPasswordLockoutPeriod.Text);
            merchantPolicy.InactivePeriod = AlwaysConvert.ToInt(MerchantPasswordInactivePeriod.Text);
            merchantPolicy.ImageCaptcha = MerchantImageCaptcha.Checked;
            merchantPolicy.Save();
            CustomerPasswordPolicy customerPolicy = new CustomerPasswordPolicy();
            customerPolicy.MinLength = customerMinLength;
            customerPolicy.RequireUpper = CustomerRequireUppercase.Checked;
            customerPolicy.RequireLower = CustomerRequireLowercase.Checked;
            customerPolicy.RequireNumber = CustomerRequireNumbers.Checked;
            customerPolicy.RequireSymbol = CustomerRequireSymbols.Checked;
            customerPolicy.RequireNonAlpha = CustomerRequireNonAlpha.Checked;
            customerPolicy.MaxAge = AlwaysConvert.ToInt(CustomerMaxAge.Text);
            customerPolicy.HistoryDays = AlwaysConvert.ToInt(CustomerPasswordHistoryDays.Text);
            customerPolicy.HistoryCount = AlwaysConvert.ToInt(CustomerPasswordHistoryCount.Text);
            customerPolicy.MaxAttempts = AlwaysConvert.ToInt(CustomerPasswordMaxAttempts.Text);
            customerPolicy.LockoutPeriod = AlwaysConvert.ToInt(CustomerPasswordLockoutPeriod.Text);
            customerPolicy.ImageCaptcha = CustomerImageCaptcha.Checked;
            customerPolicy.Save();
            trSavedMessage.Visible = true;
        }
    }

}
