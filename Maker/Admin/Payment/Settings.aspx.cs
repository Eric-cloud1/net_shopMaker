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
using MakerShop.Payments;
using System.Collections.Generic;
using MakerShop.Common;
using MakerShop.Stores;
using MakerShop.Utility;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;

public partial class Admin_Payment_Settings : System.Web.UI.Page
{

  

  
    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (!Page.IsPostBack)
        {
            binddata();
            bindImage();

        }
    }

    

    protected void bindImage()
    {
        Q1_RebillAuthImage.StoreSetting = "Processing_Enabled_Authorize_Q1_Rebill_STATUS";
        Q2_RebillAuthImage.StoreSetting = "Processing_Enabled_Authorize_Q2_Rebill_STATUS";
        Q3_RebillAuthImage.StoreSetting = "Processing_Enabled_Authorize_Q3_Rebill_STATUS";
        Q4_RebillAuthImage.StoreSetting = "Processing_Enabled_Authorize_Q4_Rebill_STATUS";
        Q5_RebillAuthImage.StoreSetting = "Processing_Enabled_Authorize_Q5_Rebill_STATUS";
        RescheduleAuthImage.StoreSetting = "Processing_Enabled_Authorize_Authorize_STATUS";
        RescheduleCaptureImage.StoreSetting = "Processing_Enabled_Capture_Capture_STATUS";
        AuthImage.StoreSetting = "Processing_Enabled_Authorize_STATUS";
        CaptureImage.StoreSetting = "Processing_Enabled_Capture_STATUS";
        RefundImage.StoreSetting = "Processing_Enabled_Refund_STATUS";
        VoidImage.StoreSetting = "Processing_Enabled_Void_STATUS";

    }
    protected void binddata()
    {
        Store store = Token.Instance.Store;
        StoreSettingCollection settings = store.Settings;


        cbAuthorize_Q1.Checked = AlwaysConvert.ToBool(settings.Processing_Enabled_Authorize("Q1_Rebill"),false);

        cbAuthorize_Q2.Checked = AlwaysConvert.ToBool(settings.Processing_Enabled_Authorize("Q2_Rebill"), false);

        cbAuthorize_Q3.Checked = AlwaysConvert.ToBool(settings.Processing_Enabled_Authorize("Q3_Rebill"), false);
        
        cbAuthorize_Q4.Checked = AlwaysConvert.ToBool(settings.Processing_Enabled_Authorize("Q4_Rebill"),false);

        cbAuthorize_Q5.Checked = AlwaysConvert.ToBool(settings.Processing_Enabled_Authorize("Q5_Rebill"), false);

        cbEnableAuthOverrided.Checked = AlwaysConvert.ToBool(settings.Processing_Enabled_Authorize("Authorize"), false);

        cbEnableCapOverride.Checked = AlwaysConvert.ToBool(settings.Processing_Enabled_Capture("Capture"),false);
      
        cbenableAuth.Checked = AlwaysConvert.ToBool(settings.Processing_Enabled_Authorize(null), false);

        cbenablecap.Checked = AlwaysConvert.ToBool(settings.Processing_Enabled_Capture(null), false);

        cbenableref.Checked = AlwaysConvert.ToBool(settings.Processing_Enabled_Refund(null), false);
        
        cbenablevoid.Checked = AlwaysConvert.ToBool(settings.Processing_Enabled_Void(null), false);
        
        tbAuthorize.Text = settings.Processing_Authorize_Threads(null).ToString();
        tbThreadAuthOverride.Text = settings.Processing_Authorize_Threads("Authorize").ToString();

        tbThreads_Q1.Text = settings.Processing_Authorize_Threads("Q1_Rebill").ToString();
        tbThreads_Q2.Text = settings.Processing_Authorize_Threads("Q2_Rebill").ToString();
        tbThreads_Q3.Text = settings.Processing_Authorize_Threads("Q3_Rebill").ToString();
        tbThreads_Q4.Text = settings.Processing_Authorize_Threads("Q4_Rebill").ToString();
        tbThreads_Q5.Text = settings.Processing_Authorize_Threads("Q5_Rebill").ToString();

        tbPause_Q1.Text = settings.Processing_Authorize_Threads("Q1_Rebill_PAUSE").ToString();
        tbPause_Q2.Text = settings.Processing_Authorize_Threads("Q2_Rebill_PAUSE").ToString();
        tbPause_Q3.Text = settings.Processing_Authorize_Threads("Q3_Rebill_PAUSE").ToString();
        tbPause_Q4.Text = settings.Processing_Authorize_Threads("Q4_Rebill_PAUSE").ToString();
        tbPause_Q5.Text = settings.Processing_Authorize_Threads("Q5_Rebill_PAUSE").ToString();


        tbCapture.Text = settings.Processing_Capture_Threads(null).ToString();
        tbThreadCapOverride.Text = settings.Processing_Capture_Threads("Capture").ToString();
    }


    protected void SaveSettings()
    {

        Store store = Token.Instance.Store;
        StoreSettingCollection settings = store.Settings;


        settings.Processing_Enabled_Authorize(null, cbenableAuth.Checked);
        settings.Processing_Enabled_Authorize("Authorize", cbEnableAuthOverrided.Checked);
        settings.Processing_Enabled_Authorize("Q1_Rebill", cbAuthorize_Q1.Checked);
        settings.Processing_Enabled_Authorize("Q2_Rebill", cbAuthorize_Q2.Checked);
        settings.Processing_Enabled_Authorize("Q3_Rebill", cbAuthorize_Q3.Checked);
        settings.Processing_Enabled_Authorize("Q4_Rebill", cbAuthorize_Q4.Checked);
        settings.Processing_Enabled_Authorize("Q5_Rebill", cbAuthorize_Q5.Checked);

        settings.Processing_Enabled_Capture(null, cbenablecap.Checked);
        settings.Processing_Enabled_Capture("Capture", cbEnableCapOverride.Checked);

        settings.Processing_Enabled_Refund(null, cbenableref.Checked);

        settings.Processing_Enabled_Void(null, cbenablevoid.Checked);

        settings.Processing_Authorize_Threads(null, AlwaysConvert.ToInt(tbAuthorize.Text));
        settings.Processing_Authorize_Threads("Authorize", AlwaysConvert.ToInt(tbThreadAuthOverride.Text));

        settings.Processing_Authorize_Threads("Q1_Rebill_PAUSE", AlwaysConvert.ToInt(tbPause_Q1.Text));
        settings.Processing_Authorize_Threads("Q2_Rebill_PAUSE", AlwaysConvert.ToInt(tbPause_Q2.Text));
        settings.Processing_Authorize_Threads("Q3_Rebill_PAUSE", AlwaysConvert.ToInt(tbPause_Q3.Text));
        settings.Processing_Authorize_Threads("Q4_Rebill_PAUSE", AlwaysConvert.ToInt(tbPause_Q4.Text));
        settings.Processing_Authorize_Threads("Q5_Rebill_PAUSE", AlwaysConvert.ToInt(tbPause_Q5.Text));

        if (AlwaysConvert.ToInt(tbPause_Q1.Text) != 0 )
            settings.Processing_Authorize_Threads("Q1_Rebill", 1);
        else
            settings.Processing_Authorize_Threads("Q1_Rebill", AlwaysConvert.ToInt(tbThreads_Q1.Text));

        if (AlwaysConvert.ToInt(tbPause_Q2.Text) != 0)
            settings.Processing_Authorize_Threads("Q2_Rebill", 1);
        else
            settings.Processing_Authorize_Threads("Q2_Rebill", AlwaysConvert.ToInt(tbThreads_Q2.Text));
        
        if (AlwaysConvert.ToInt(tbPause_Q3.Text) != 0)
            settings.Processing_Authorize_Threads("Q3_Rebill", 1);
        else
            settings.Processing_Authorize_Threads("Q3_Rebill", AlwaysConvert.ToInt(tbThreads_Q3.Text));
        
        if (AlwaysConvert.ToInt(tbPause_Q4.Text) != 0)
            settings.Processing_Authorize_Threads("Q4_Rebill", 1);
        else
            settings.Processing_Authorize_Threads("Q4_Rebill", AlwaysConvert.ToInt(tbThreads_Q4.Text));
        if (AlwaysConvert.ToInt(tbPause_Q5.Text) != 0)
            settings.Processing_Authorize_Threads("Q5_Rebill", 1);
        else
            settings.Processing_Authorize_Threads("Q5_Rebill", AlwaysConvert.ToInt(tbThreads_Q5.Text));
        
        settings.Processing_Capture_Threads(null, AlwaysConvert.ToInt(tbCapture.Text));
        settings.Processing_Capture_Threads("Capture", AlwaysConvert.ToInt(tbThreadCapOverride.Text));
        

        settings.Save();


        binddata();


    }
    protected void SaveButton_Click(object sender, EventArgs e)
    {
        SaveSettings();
        SavedMessage.Visible = true;
    }
    protected void SaveAndCloseButton_Click(object sender, EventArgs e)
    {
        SaveSettings();
        Response.Redirect("../Default.aspx");
    }
    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("../Default.aspx");
    }
}
