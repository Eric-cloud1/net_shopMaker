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
using MakerShop.Products;
using MakerShop.Utility;

public partial class ConLib_VerifyEmail : System.Web.UI.UserControl
{
    private string _SuccessMessage = "Your email address has been verified!";
    private string _FailureMessage = "Your email address could not be verified.  Check that you have opened the link exactly as it appeared in the verification notice.";

    public string SuccessMessage
    {
        get { return _SuccessMessage; }
        set { _SuccessMessage = value; }
    }
 
    public string FailureMessage
    {
        get { return _FailureMessage; }
        set { _FailureMessage = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        bool error = false;
        int reviewerProfileId = AlwaysConvert.ToInt(Request.QueryString["Reviewer"]);
        ReviewerProfile profile = ReviewerProfileDataSource.Load(reviewerProfileId);
        if (profile != null)
        {
            if (!profile.EmailVerified)
            {
                Guid code = AlwaysConvert.ToGuid(Request.QueryString["Code"]);
                if (profile.EmailVerificationCode == code)
                {
                    //VERIFY THIS USER
                    profile.VerifyEmail();
                }
                else error = true;
            }
        }
        else error = true;
        SuccessText.Text = SuccessMessage;
        SuccessText.Visible = !error;
        FailureText.Text = FailureMessage;
        FailureText.Visible = error;
    }
}
