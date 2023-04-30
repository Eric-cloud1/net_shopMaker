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
using MakerShop.Marketing;
using MakerShop.Orders;
using MakerShop.Utility;

public partial class ConLib_Disclaimer : System.Web.UI.UserControl
{
    private String _DisclaimerMessage;
    private String _NoCookiesMessage = "Your browser does not support cookies or cookies are disabled. This site uses cookies to store information, please enable cookies otherwise you will not be able to use our site.";

    [Personalizable(), WebBrowsable()]
    public String NoCookiesMessage
    {
        get { return _NoCookiesMessage; }
        set { _NoCookiesMessage = value; }
    }

    protected String DisclaimerMessage
    {
        get
        {
            if (_DisclaimerMessage == null)
            {
                _DisclaimerMessage = Token.Instance.Store.Settings.SiteDisclaimerMessage;
            }
            return _DisclaimerMessage;
        }
    }

    protected string ReturnUrl
    {
        get
        {
            string url = Request.QueryString["ReturnUrl"];
            if (!string.IsNullOrEmpty(url))
            {
                return Server.UrlDecode(url);
            }
            return "~/Default.aspx";
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (DisclaimerMessage == null) Response.Redirect(ReturnUrl);
            DisclaimerText.Text = _DisclaimerMessage;
            NoCookiesMessageLabel.Text = NoCookiesMessage;
        }

        trNoCookies.Visible = !Request.Browser.Cookies;
        trDisclaimerMessage.Visible = Request.Browser.Cookies;

    }

    protected void OkButton_Click(object sender, EventArgs e)
    {
        Response.Redirect(ReturnUrl);
    }
}