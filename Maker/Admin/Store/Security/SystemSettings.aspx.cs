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
using MakerShop.Common;
using MakerShop.Stores;
using MakerShop.Utility;

public partial class Admin_Store_Security_SystemSettings : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            StoreSettingCollection settings = Token.Instance.Store.Settings;
            FileExtAssets.Text = settings.FileExt_Assets;
            FileExtThemes.Text = settings.FileExt_Themes;
            FileExtDigitalGoods.Text = settings.FileExt_DigitalGoods;
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        StoreSettingCollection settings = Token.Instance.Store.Settings;
        settings.FileExt_Assets = FileExtAssets.Text;
        settings.FileExt_Themes = FileExtThemes.Text;
        settings.FileExt_DigitalGoods = FileExtDigitalGoods.Text;
        settings.Save();
        SavedMessage.Visible = true;
        SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
    }
}
