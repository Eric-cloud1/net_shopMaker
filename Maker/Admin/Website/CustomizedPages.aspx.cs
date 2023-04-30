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
using MakerShop.Personalization;

public partial class Admin_Website_CustomizedPages : MakerShop.Web.UI.MakerShopAdminPage
{
    protected string HasData(object dataItem)
    {
        SharedPersonalization p = (SharedPersonalization)dataItem;
        if ((p.PageSettings != null) && (p.PageSettings.Length > 0)) return "custom";
        return "default";
    }

    protected void PageGrid_RowDeleted(object sender, EventArgs e)
    {
        MakerShop.Web.WebflowManager.ClearCache();
    }
}
