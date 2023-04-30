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
using MakerShop.DigitalDelivery;
using MakerShop.Utility;

public partial class ConLib_ReadmeContent : System.Web.UI.UserControl
{
    protected string ReturnUrl
    {
        get
        {
            string url = Request.QueryString["ReturnUrl"];
            if (!string.IsNullOrEmpty(url))
            {
                return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(url));
            }
            return "~/Default.aspx";
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        int readmeId = AlwaysConvert.ToInt(Request.QueryString["ReadmeId"]);
        Readme readme = ReadmeDataSource.Load(readmeId);
        if (readme == null) Response.Redirect(ReturnUrl);
        Page.Title = readme.DisplayName;
        ReadmeText.Text = readme.ReadmeText;
        OkButton.NavigateUrl = ReturnUrl;
    }
}
