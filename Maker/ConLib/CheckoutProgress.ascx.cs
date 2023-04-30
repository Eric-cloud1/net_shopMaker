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

public partial class ConLib_CheckoutProgress : System.Web.UI.UserControl
{
    protected void Page_Init(object sender, EventArgs e)
    {
        //DETERMINE THE CURRENT PAGE
        string path = Request.Url.AbsolutePath; ;
        int index = path.LastIndexOf("/");
        if (index > -1) path = path.Substring(index + 1);
        path = path.ToLowerInvariant();
        //TURN ON THE APPROPRIATE STAGE
        switch (path)
        {
            case "default.aspx":
            case "createprofile.aspx":
            case "editbilladdress.aspx":
                Step1.Attributes["class"] = "on";
                break;
            case "shipaddress.aspx":
            case "shipaddresses.aspx":
            case "editshipaddress.aspx":
            case "shipmethod.aspx":
                Step2.Attributes["class"] = "on";
                break;
            case "giftoptions.aspx":
                Step3.Attributes["class"] = "on";
                break;
            case "payment.aspx":
                Step4.Attributes["class"] = "on";
                break;
        }
    }
}
