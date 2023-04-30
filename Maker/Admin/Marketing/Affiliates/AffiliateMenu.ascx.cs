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
using MakerShop.Orders;
using MakerShop.Utility;
using MakerShop.Payments;
using MakerShop.Marketing;

public partial class Admin_Marketing_Affiliates_AffiliateMenu : System.Web.UI.UserControl
{
    private int _AffiliateId;
    private Affiliate _Affiliate = null;
    private void InitAffiliate()
    {
        //Check for query string
        _AffiliateId = AlwaysConvert.ToInt(Request.QueryString["AffiliateId"]);
        _Affiliate = AffiliateDataSource.Load(_AffiliateId);

        if (_Affiliate != null)
        {
        }
    }



    protected void Page_Load(object sender, EventArgs e)
    {
        InitAffiliate();
        if (_Affiliate != null)
        {
            string suffix = "?AffiliateId=" + _Affiliate.AffiliateId.ToString() + "&ParentAffiliateId=" + _Affiliate.ParentAffiliateID.ToString();
            Payments.NavigateUrl += suffix;
            EditAffiliate.NavigateUrl += suffix;
            AddAffiliate.NavigateUrl += suffix;
            Gateway.NavigateUrl += suffix;
        }

    }
}