using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.ComponentModel;
using MakerShop.Common;
using MakerShop.Stores;
using MakerShop.Utility;


public partial class Admin_UserControls_autoRefreshImage : System.Web.UI.UserControl
{

   
    [Browsable(true)]
    public string StoreSetting
    {
        get
        {
            if (Session[this.ClientID + "_StoreSetting"] == null)
                return string.Empty;

            return (string)Session[this.ClientID + "_StoreSetting"];
        }
        set
        {
            Session[this.ClientID + "_StoreSetting"] = value;
        }

    }
    private bool _AutoRefresh = true;
    [Browsable(true)]
    [DefaultValue(true)]
    public bool AutoRefresh
    {
        get
        {
            return _AutoRefresh;
        }
        set
        {
            _AutoRefresh = value;
        }

    }
    protected string getStatus(string key)
    {
        string skingId = string.Empty;

        switch (key)
        {
            case "START": skingId = "Yellow"; break;
            case "STARTING": skingId = "RedYellowGreen"; break;
            case "RUNNING": skingId = "Green"; break;
            case "STOPPING": skingId = "YellowFlashing"; break;
            case "STOP": skingId = "Red"; break;
        }

        return skingId;
    }



    protected void UpdateTimer_Tick(object sender, EventArgs e)
    {
        //storeSettings.GetValueByKey(StoreSetting)
        Store store = Token.Instance.Store;
        StoreSettingCollection settings = store.Settings;
        phImageLocation.Controls.Clear();

        Image myImage = new Image();
        string key = settings.GetValueByKey(StoreSetting);

        myImage.SkinID = getStatus(key);

        if (!string.IsNullOrEmpty(myImage.SkinID))
            this.phImageLocation.Controls.Add(myImage);


    }

    [System.Security.Permissions.PermissionSet
(System.Security.Permissions.SecurityAction.Demand,
Name = "FullTrust")]
    protected void Page_Load(object sender, EventArgs e)
    {

        Store store = Token.Instance.Store;
        StoreSettingCollection settings = store.Settings;
        
        Image myImage = new Image();
        string key = settings.GetValueByKey(StoreSetting);

        myImage.SkinID = getStatus(key);

        if (!string.IsNullOrEmpty(myImage.SkinID))
            this.phImageLocation.Controls.Add(myImage);
        UpdateTimer.Enabled = AutoRefresh;
    }

}
