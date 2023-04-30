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

public partial class ConLib_AgreementContent : System.Web.UI.UserControl
{
    private bool HasReturnUrl
    {
        get { return (!string.IsNullOrEmpty(Request.QueryString["ReturnUrl"])); }
    }

    private bool HasAcceptUrl
    {
        get { return (!string.IsNullOrEmpty(Request.QueryString["AcceptUrl"])); }
    }

    private bool HasDeclineUrl
    {
        get { return (!string.IsNullOrEmpty(Request.QueryString["DeclineUrl"])); }
    }

    protected string AcceptUrl
    {
        get
        {
            if (HasAcceptUrl)
                return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Request.QueryString["AcceptUrl"]));
            return "~/Default.aspx";
        }
    }

    protected string DeclineUrl
    {
        get
        {
            if (HasDeclineUrl)
                return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Request.QueryString["DeclineUrl"]));
            return "~/Default.aspx";
        }
    }

    protected string ReturnUrl
    {
        get
        {
            if (HasReturnUrl)
                return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Request.QueryString["ReturnUrl"]));
            return "~/Default.aspx";
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        int licenseAgreementId = AlwaysConvert.ToInt(Request.QueryString["id"]);
        LicenseAgreement licenseAgreement = LicenseAgreementDataSource.Load(licenseAgreementId);
        if (licenseAgreement == null) Response.Redirect(AcceptUrl);
        Page.Title = licenseAgreement.DisplayName;
        AgreementText.Text = licenseAgreement.AgreementText;
        if (HasReturnUrl)
        {
            OKLink.NavigateUrl = ReturnUrl;
            AcceptLink.Visible = false;
            DeclineLink.Visible = false;
        }
        else
        {
            OKLink.Visible = false;
            AcceptLink.NavigateUrl = AcceptUrl;
            DeclineLink.NavigateUrl = DeclineUrl;
        }
    }
}