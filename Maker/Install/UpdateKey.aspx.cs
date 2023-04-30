using System;
using System.IO;
using System.Net;
using System.Text;
using MakerShop.Common;
using MakerShop.Utility;
using System.Collections.Generic;

public partial class Install_UpdateKey : System.Web.UI.Page
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

        if (AlwaysConvert.ToInt(Request.QueryString["U"]) == 1)
        {
            ReturnToUpgrade.Value = "1";
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


    protected void Page_Load(object sender, EventArgs e)
    {
        Domain.Text = Request.Url.Authority;
    }

    private void ChangeKeyOption(int option)
    {
        KeyDemoPanel.Visible = (option == 0);
        LicenseKeyPanel.Visible = (option == 1);
        KeyUploadPanel.Visible = (option == 2);
    }

    protected void KeyDemo_CheckedChanged(object sender, EventArgs e)
    {
        ChangeKeyOption(0);
    }

    protected void LicenseKeyOption_CheckedChanged(object sender, EventArgs e)
    {
        ChangeKeyOption(1);
    }

    protected void KeyUpload_CheckedChanged(object sender, EventArgs e)
    {
        ChangeKeyOption(2);
    }

    protected void UpdateKeyButton_Click(object sender, EventArgs e)
    {
        if (UpdateLicense())
        {
            //SETUP THE DATABASE
            Response.Write("<html><head><title>Update MakerShop License Key</title><link href=\"../App_Themes/MakerShop/ComponentArt.css\" type=\"text/css\" rel=\"stylesheet\" /><link href=\"../App_Themes/MakerShop/print.css\" type=\"text/css\" rel=\"stylesheet\" /><link href=\"../App_Themes/MakerShop/style.css\" type=\"text/css\" rel=\"stylesheet\" /><link href=\"../App_Themes/MakerShop/webparts.css\" type=\"text/css\" rel=\"stylesheet\" /></head><body style=\"width:780px;margin:auto\">");
            Response.Write("<br /><div class=\"pageHeader\"><h1 style=\"font-size:16px\">License Key Updated</h1></div>");
            if (AlwaysConvert.ToInt(ReturnToUpgrade.Value) == 0)
            {
                Response.Write("<div class=\"submit\"><a href=\"../Default.aspx\" class=\"button\">Return to Home Page</a></div>");
            }
            else
            {
                Response.Write("<div class=\"submit\"><a href=\"Upgrade.aspx\" class=\"button\">Continue Upgrade</a></div>");
            }
            Response.Write("</body></html>");
            Response.Flush();
            Response.End();
        }
    }

    private bool UpdateLicense()
    {
        if (KeyDemo.Checked) return DownloadDemoLicense();
        else if (LicenseKeyOption.Checked) return DownloadLicense();
        else
        {
            if (Request.Files.Count > 0)
            {
                string filePath = Server.MapPath("~/App_Data/MakerShop.lic");
                Request.Files[0].SaveAs(filePath);
                string keyText = FileHelper.ReadText(filePath);
                MakerShop.Configuration.License lic = new MakerShop.Configuration.License(keyText);
                if (lic.IsValid) return true;
                File.Delete(filePath);
                HandleError("The key file you uploaded is not a valid license key.");
            }
            else HandleError("No key file was uploaded.");
            return false;
        }
    }

    private bool DownloadDemoLicense()
    {
        //CONSTRUCT FORM DATA
        StringBuilder formData = new StringBuilder();
        formData.Append(UrlEncode("Name", Name.Text));
        formData.Append("&" + UrlEncode("Company", Company.Text));
        formData.Append("&" + UrlEncode("Email", Email.Text));
        formData.Append("&" + UrlEncode("Phone", Phone.Text));
        formData.Append("&" + UrlEncode("Domain", Domain.Text));
        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.MakerShop.com/livereg7/GetDemoKey.ashx");
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
                MakerShop.Configuration.License lic = new MakerShop.Configuration.License(keyText);
                if (lic.IsValid)
                {
                    //WRITE THE KEY
                    string keyPath = Server.MapPath("~/App_Data/MakerShop.lic");
                    File.WriteAllText(keyPath, keyText);
                    return true;
                }
                else
                {
                    //SOMETHING IS WRONG WITH THIS KEY
                    if (lic.LastException != null) HandleError(lic.LastException.Message);
                    else HandleError("There is an unknown problem with the specified key.");
                    return false;
                }
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
        MessagePanel.Visible = true;
        ReponseMessage.Text = errorMessage;
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
