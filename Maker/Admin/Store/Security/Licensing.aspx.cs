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
using System.IO;
using MakerShop.Configuration;
using System.Text;
using System.Net;

public partial class Admin_Store_Security_Licensing : MakerShop.Web.UI.MakerShopAdminPage
{


    protected void Page_Init(object sender, EventArgs e)
    {
        List<string> messages = new List<string>();

        bool permissionsValid = this.TestPermissions(messages);
        if (!permissionsValid || !string.IsNullOrEmpty(Request.QueryString["PERMTEST"]))
        {
            // SOMETHING IS WRONG WITH PERMISSIONS, WE SHOULD DISPLAY THE TEST RESULTS
            phUpdateKey.Visible = false;
            phPermissions.Visible = true;
            ProcessIdentity.Text = this.GetProcessIdentity();
            PermissionsTestResult.Text = string.Join(string.Empty, messages.ToArray());
        }
    }

    private bool TestPermissions(List<string> messages)
    {
        // FLAG TO TRACK RESULT OF TESTS
        bool permissionsValid = true;
        Exception testException;

        // DETERMINE THE BASE APPLICATION DIRECTORY
        string baseDirectory = Server.MapPath("~");
        string testFile = Server.MapPath("~/App_Data/MakerShop.lic");
        
        if (File.Exists(testFile))
        {
            testException = FileHelper.CanWriteExistingFile(testFile);
            if (testException != null)
            {
                //ACCESS EXCEPTION OCCURRED, SHOW ANY DIRECTORY PATH RELATIVE TO INSTALL
                string cleanFileName = testFile.Replace(baseDirectory, "~").Replace("\\", "/");
                string cleanMessage = testException.Message.Replace(baseDirectory, "~").Replace("\\", "/");
                messages.Add("Testing write access to " + cleanFileName + " : <font color=red>FAILED</font><blockquote>" + cleanMessage + "</blockquote>");
                permissionsValid = false;
            }
        }
        else
        {
            testException = FileHelper.CanCreateFile(testFile, "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<MakerShopLicense version=\"7.0.0.0\">\r\n</MakerShopLicense>");
            if (testException != null)
            {
                // WRITE ACCESS EXCEPTION OCCURRED, SHOW ANY DIRECTORY PATH RELATIVE TO INSTALL
                string cleanFileName = testFile.Replace(baseDirectory, "~").Replace("\\", "/");                
                string cleanMessage = testException.Message.Replace(baseDirectory, "~").Replace("\\", "/");
                messages.Add("Testing file create access in App_Data : <font color=red>FAILED</font><blockquote>" + cleanMessage + "</blockquote>");
                permissionsValid = false;
            }
        }

        return permissionsValid;
    }

    protected void Page_PreRender(object sender, System.EventArgs e)
    {
        MakerShop.Configuration.License lic = Token.Instance.License;
        LicenseType.Text = lic.KeyType;
        DomainList.DataSource = lic.Domains.ToArray();
        DomainList.DataBind();
        if (lic.SubscriptionDate > DateTime.MinValue)
        {
            if (lic.SubscriptionDate < LocaleHelper.LocalNow)
            {
                SubscriptionDate.Text = "Ended on " + lic.SubscriptionDate.ToString("dd-MMM-yyyy");
            }
            else
            {
                SubscriptionDate.Text = "Ends on " + lic.SubscriptionDate.ToString("dd-MMM-yyyy");
            }
        }
        else SubscriptionDate.Text = "None";
        if (lic.Expiration > DateTime.MinValue)
        {
            trExpiration.Visible = true;
            Expiration.Text = lic.Expiration.ToString("yyyy-MMM-dd");
        }
        //FOR LIVE LICENSES, SHOW DEMO MODE PANEL
        if (!lic.IsDemo)
        {
            DemoModePanel.Visible = true;
            MakerShopApplicationSection appConfig = MakerShopApplicationSection.GetSection();
            if (appConfig.DemoMode)
            {
                DemoModeButton.Text = "Disable Demo Mode";
                DemoModeEnabledText.Visible = true;
                DemoModeDisabledText.Visible = false;
            }
            else
            {
                DemoModeButton.Text = "Enable Demo Mode";
                DemoModeEnabledText.Visible = false;
                DemoModeDisabledText.Visible = true;
            }
        }
        else
        {
            //FOR DEMO LICENSES, HIDE DEMO MODE PANEL
            DemoModePanel.Visible = false;
        }
        if (Session["StoreLicenseUpdated"] != null)
        {
            SavedMessage.Visible = true;
            SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
            Session.Remove("StoreLicenseUpdated");
        }
    }

    protected void DemoModeButton_Click(object sender, EventArgs e)
    {
        System.Configuration.Configuration config;
        MakerShopApplicationSection appConfig = MakerShopApplicationSection.GetUpdatableSection(out config);
        if (appConfig != null)
        {
            if (config != null)
            {
                appConfig.DemoMode = !appConfig.DemoMode;
                MakerShopApplicationSection.UpdateConfig(config, appConfig);
            }
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (DownloadLicense())
        {
            //WE HAVE TO REFRESH THIS PAGE TO SHOW THE UPDATED LICENSE
            //THE TOKEN MUST BE RE-INITIALIZED TO DISPLAY THE NEW LICENSE DATA
            Session["StoreLicenseUpdated"] = true;
            Response.Redirect(Request.Url.ToString());
        }
    }

    private bool DownloadLicense()
    {
        //CONSTRUCT FORM DATA
        StringBuilder formData = new StringBuilder();
        formData.Append(UrlEncode("Key", LicenseKey.Text));
        formData.Append("&" + UrlEncode("Path", Server.MapPath("~").ToLowerInvariant() + "\\"));
        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.MakerShop.com/livereg7/GetKey.ashx");
        httpWebRequest.Method = "POST";
        httpWebRequest.ContentType = "application/x-www-form-urlencoded";
        byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(formData.ToString());
        httpWebRequest.ContentLength = requestBytes.Length;
        using (Stream requestStream = httpWebRequest.GetRequestStream())
        {
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();
        }
        string response;
        using (StreamReader responseStream = new StreamReader(httpWebRequest.GetResponse().GetResponseStream(), System.Text.Encoding.UTF8))
        {
            response = responseStream.ReadToEnd();
            responseStream.Close();
        }
        //SPLIT INTO NAME/VALUE DICTIONARY
        UrlEncodedDictionary dictionary = new UrlEncodedDictionary(response);
        if (dictionary.ContainsKey("RESULT"))
        {
            if (dictionary["RESULT"] == "1")
            {
                string keyText = dictionary["KEY"];
                //WRITE THE KEY
                string keyPath = Server.MapPath("~/App_Data/MakerShop.lic");
                File.WriteAllText(keyPath, keyText);
                return true;
            }
            else
            {
                HandleError(dictionary["MESSAGE"]);
            }
        }
        else
        {
            HandleError("Unknown error occurred contacting the MakerShop registration service!");
        }
        return false;
    }

    private void HandleError(string errorMessage)
    {
        FailedMessage.Text = string.Format(FailedMessage.Text, errorMessage);
        FailedMessage.Visible = true;
    }

    private string UrlEncode(string key, string value)
    {
        return key + "=" + Server.UrlEncode(value);
    }

    private string GetProcessIdentity()
    {
        string processIdentity;
        try
        {
            processIdentity = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }
        catch
        {
            processIdentity = "Unable to determine.  Generally NETWORK SERVICE (Windows 2003/XP) or ASPNET (Windows 2000)";
        }
        return processIdentity;
    }


}
