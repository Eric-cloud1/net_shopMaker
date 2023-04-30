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

public partial class ConLib_MyAffiliateAccountPage : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (Token.Instance.User.Affiliate == null)
            {
                trNoAffiliatePanel.Visible = true;
                trAffiliateReport.Visible = false;
                NoAffiliateMessage.Text = string.Format(NoAffiliateMessage.Text, Token.Instance.User.UserName);
            }
            else
            {
                trAffiliateInfo.Visible = true;
                trAffiliateReport.Visible = true;
                InstructionText.Text = string.Format(InstructionText.Text, Token.Instance.User.AffiliateId, GetHomeUrl());
            }
        }
    }

    private string GetHomeUrl()
    {
        return string.Format("http://{0}{1}", Request.Url.Authority, this.ResolveUrl(NavigationHelper.GetHomeUrl()));
    }
    
    protected void RegisterLink_Click(object sender, EventArgs e)
    {   
        trAffiliateReport.Visible = false;
        trAffiliateInfo.Visible = false;
    }
}
