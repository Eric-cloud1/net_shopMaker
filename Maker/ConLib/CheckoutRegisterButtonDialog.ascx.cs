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

public partial class ConLib_CheckoutRegisterButtonDialog : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        InstructionText.Text = string.Format(InstructionText.Text, Token.Instance.Store.Name);
    }

    protected void RegisterButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("CreateProfile.aspx");
    }
}
