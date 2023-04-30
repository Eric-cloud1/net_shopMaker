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

public partial class ConLib_MySubscriptionsPage : System.Web.UI.UserControl
{
    protected void SubscriptionDs_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        e.InputParameters["userIdRange"] = Token.Instance.UserId.ToString();
    }
}
