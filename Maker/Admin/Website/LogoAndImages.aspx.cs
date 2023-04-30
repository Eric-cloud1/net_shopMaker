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

public partial class Admin_Website_LogoAndImages : MakerShop.Web.UI.MakerShopAdminPage
{

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            //INITIALIZE FORM
            BindCurrentLogo();
            StoreSettingCollection settings = Token.Instance.Store.Settings;
            IconWidth.Text = settings.IconImageWidth.ToString();
            IconHeight.Text = settings.IconImageHeight.ToString();
            ThumbnailWidth.Text = settings.ThumbnailImageWidth.ToString();
            ThumbnailHeight.Text = settings.ThumbnailImageHeight.ToString();
            StandardWidth.Text = settings.StandardImageWidth.ToString();
            StandardHeight.Text = settings.StandardImageHeight.ToString();
            ImageSkuLookupEnabled.Checked = settings.ImageSkuLookupEnabled;
            OptionThumbnailHeight.Text = settings.OptionThumbnailHeight.ToString();
            OptionThumbnailWidth.Text = settings.OptionThumbnailWidth.ToString();
            OptionThumbnailColumns.Text = settings.OptionThumbnailColumns.ToString();
        }
    }

    private void SaveSettings()
    {
        //Load store and update
        StoreSettingCollection settings = Token.Instance.Store.Settings;
        settings.IconImageWidth = AlwaysConvert.ToInt(IconWidth.Text);
        settings.IconImageHeight = AlwaysConvert.ToInt(IconHeight.Text);
        settings.ThumbnailImageWidth = AlwaysConvert.ToInt(ThumbnailWidth.Text);
        settings.ThumbnailImageHeight = AlwaysConvert.ToInt(ThumbnailHeight.Text);
        settings.StandardImageWidth = AlwaysConvert.ToInt(StandardWidth.Text);
        settings.StandardImageHeight = AlwaysConvert.ToInt(StandardHeight.Text);
        settings.ImageSkuLookupEnabled = ImageSkuLookupEnabled.Checked;
        settings.OptionThumbnailHeight =AlwaysConvert.ToInt(OptionThumbnailHeight.Text);
        settings.OptionThumbnailWidth = AlwaysConvert.ToInt(OptionThumbnailWidth.Text);
        settings.OptionThumbnailColumns = AlwaysConvert.ToInt(OptionThumbnailColumns.Text);
        settings.Save();
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        SaveSettings();
        SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
        SavedMessage.Visible = true;
    }

    protected void UpdateButton_Click(object sender, EventArgs e)
    {
        if (LogoOptionUpload.Checked)
        {
            if (UploadedLogo.HasFile)
            {
                string extension = System.IO.Path.GetExtension(UploadedLogo.FileName).ToLowerInvariant();
                if (extension == ".gif" || extension == ".jpg" || extension == ".png")
                {
                    //DELETE EXISTING LOGO FILES
                    string logoPath = GetLogoPhysicalPath();
                    System.IO.File.Delete(logoPath + ".jpg");
                    System.IO.File.Delete(logoPath + ".png");
                    System.IO.File.Delete(logoPath + ".gif");
                    UploadedLogo.SaveAs(logoPath + extension);
                }
            }
        }
        else
        {
            String imagePath = Server.MapPath(LogoOptionBrowseUrl.Text);
            string extension = System.IO.Path.GetExtension(imagePath);
            if(System.IO.File.Exists(imagePath)){                
                string logoPath = GetLogoPhysicalPath();
                System.IO.File.Delete(logoPath + ".jpg");
                System.IO.File.Delete(logoPath + ".png");
                System.IO.File.Delete(logoPath + ".gif");
                System.IO.File.Copy(imagePath, logoPath + extension,true);
            }
            
        }
        BindCurrentLogo();
    }

    private string GetLogoPhysicalPath()
    {
        return Server.MapPath("~/App_Themes/" + Store.GetCachedSettings().StoreTheme + "/images/logo");
    }

    private string GetLogoVirtualPath()
    {
        return "~/App_Themes/" + Store.GetCachedSettings().StoreTheme + "/images/logo";
    }

    private void BindCurrentLogo()
    {
        CurrentLogo.Controls.Clear();
        //find the correct logo path
        string logoUrl = string.Empty;
        string logoFilePath = GetLogoPhysicalPath();
        if (System.IO.File.Exists(logoFilePath + ".gif"))
            logoUrl = GetLogoVirtualPath() + ".gif";
        else if (System.IO.File.Exists(logoFilePath + ".jpg"))
            logoUrl = GetLogoVirtualPath()+ ".jpg";
        else if (System.IO.File.Exists(logoFilePath + ".png"))
            logoUrl = GetLogoVirtualPath() + ".png";
        //UPDATE CACHE
        if (!string.IsNullOrEmpty(logoUrl))
        {
            Image logo = new Image();
            logo.ImageUrl = logoUrl + "?tag=" + StringHelper.RandomNumber(6);
            CurrentLogo.Controls.Add(logo);
        }
        else
        {
            CurrentLogo.Controls.Add(new LiteralControl("A custom logo has not been uploaded."));
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        PageHelper.SetPickImageButton(LogoOptionBrowseUrl, BrowseLogoUrl);
    }

    
}
