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
using MakerShop.Stores;

public partial class ConLib_ProductReviewTerms : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        StoreSettingCollection settings = Token.Instance.Store.Settings;
        TermsAndConditions.Text = settings.ProductReviewTermsAndConditions;
    }
}
