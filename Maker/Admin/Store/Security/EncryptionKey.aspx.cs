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
using MakerShop.Configuration;

public partial class Admin_Store_Security_EncryptionKey : MakerShop.Web.UI.MakerShopAdminPage
{

    protected void UpdateButton_Click(object sender, EventArgs e)
    {
        //GENERATE PASSPHRASE FROM RANDOM TEXT AND TIMESTAMP
        string passPhrase;
        if (RandomText.Text != "DECRYPT") passPhrase = RandomText.Text + DateTime.Now.ToString("MM/dd/yyyy HHmmnnss");
        else passPhrase = string.Empty;
        byte[] newKey = EncryptionHelper.SetEncryptionKey(passPhrase);
        RandomText.Text = string.Empty;
        //A SMALL DELAY ENSURES THAT THE SCREEN WILL UPDATE WITH THE NEW KEY DATA
        System.Threading.Thread.Sleep(100);
        UpdateProgress();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        PageHelper.DisableValidationScrolling(this.Page);
        ChangeProgressTimer.Interval = 3000;
        if (!Page.IsPostBack)
        {
            //THIS WILL CONTINUE THE PROGRESS INDICATOR IF NEEDED
            //OTHERWISE THE CHANGE FORM WILL BE DISPLAYED
            UpdateProgress();
        }
    }

    private void BindKey()
    {
        MakerShopEncryptionSection encryptionConfig = MakerShopEncryptionSection.GetSection();
        //create date is stored in utc
        DateTime createDate = LocaleHelper.ToLocalTime(encryptionConfig.EncryptionKey.CreateDate);
        if (createDate > DateTime.MinValue) LastSet.Text = createDate.ToShortDateString() + " " + createDate.ToShortTimeString();
        else LastSet.Text = "n/a";
        ToggleBackupPanels(EncryptionHelper.IsKeyValid(encryptionConfig.EncryptionKey.GetKey()));
    }

    private void ToggleBackupPanels(bool show)
    {
        BackupPanel.Visible = show;
        NoKeyNoBackupPanel.Visible = !show;
    }

    protected void ChangeProgressTimer_Tick(object sender, EventArgs e)
    {
        UpdateProgress();
    }

    protected void UpdateProgress()
    {
        int remaining = RecryptionHelper.GetRecryptionWorkload();
        if (remaining == 0)
        {
            EstimatedWorkload.Text = RecryptionHelper.EstimateRecryptionWorkload().ToString();
            ChangePanel.Visible = true;
            ChangeProgressPanel.Visible = false;
            byte[] keyData = EncryptionHelper.GetEncryptionKey();
            if (Page.IsPostBack) KeyUpdatedMessage.Visible = true;
        }
        else
        {
            ChangePanel.Visible = false;
            ChangeProgressPanel.Visible = true;
            trRestartCancel.Visible = ((!Page.IsPostBack) || (RemainingWorkload.Text == remaining.ToString()));
            RemainingWorkload.Text = remaining.ToString();
            ProgressDate.Text = LocaleHelper.LocalNow.ToString("hh:mm:ss tt");
            //SPEED UP THE CHECK BECAUSE WE ARE ALMOST DONE!
            if (remaining < 10) ChangeProgressTimer.Interval = 1000;
        }
        BindKey();
    }

    protected void ShowBackupLinks_Click(object sender, EventArgs e)
    {
        GetBackup1.Visible = true;
        GetBackup2.Visible = true;
        ShowBackupLinks.Visible = false;
    }

    protected void RestoreButton_Click(object sender, EventArgs e)
    {
        if (Request.Files.Count == 2)
        {
            byte[] part1 = new byte[Request.Files[0].ContentLength];
            byte[] part2 = new byte[Request.Files[1].ContentLength];
            Request.Files[0].InputStream.Read(part1, 0, part1.Length);
            Request.Files[1].InputStream.Read(part2, 0, part2.Length);
            try
            {
                EncryptionHelper.RestoreBackupKey(part1, part2);
                RestoredMessage.Visible = true;
                RestoredMessage.Text = string.Format(RestoredMessage.Text, LocaleHelper.LocalNow);
            }
            catch (Exception ex)
            {
                CustomValidator restoreValidator = new CustomValidator();
                restoreValidator.IsValid = false;
                restoreValidator.Text = "*";
                restoreValidator.ErrorMessage = ex.Message;
                restoreValidator.ValidationGroup = "Restore";
                phRestoreValidators.Controls.Add(restoreValidator);
            }
        }
        BindKey();
    }

    protected void RestartChangeButton_Click(object sender, EventArgs e)
    {
        //RESTART THE RECRYPTION PROCESS
        RecryptionHelper.RecryptDatabase(Context.ApplicationInstance);
        UpdateProgress();
    }

    protected void CancelChangeButton_Click(object sender, EventArgs e)
    {
        RecryptionHelper.SetRecryptionFlag(false);
        UpdateProgress();
    }

}
