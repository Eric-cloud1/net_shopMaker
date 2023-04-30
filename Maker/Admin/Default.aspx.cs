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
using System.Text.RegularExpressions;
using MakerShop.Common;
using MakerShop.Catalog;
using MakerShop.Orders;
using MakerShop.Products;
using MakerShop.Stores;
using MakerShop.Users;
using MakerShop.Utility;
using MakerShop.Reporting;
using System.Collections.Generic;

public partial class Admin_Default : MakerShop.Web.UI.MakerShopAdminPage
{

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!(User.IsInRole("Admin") || User.IsInRole("System") || User.IsInRole("Customer Service Admin") || User.IsInRole("Gateway Service Admin")))
            Response.Redirect("~/Admin/Orders/Default.aspx");
        WebPartManager _manager = WebPartManager.GetCurrentWebPartManager(Page);
        CustomizeLink.Visible = (_manager.DisplayMode.Name.Equals("Browse"));
        AdminWebpartsPanel1.Visible = (!CustomizeLink.Visible);

    }

    protected void CustomizeLink_Click(object sender, EventArgs e)
    {
        WebPartManager _manager = WebPartManager.GetCurrentWebPartManager(Page);
        WebPartDisplayMode mode = _manager.SupportedDisplayModes["Catalog"];
        if (mode != null) _manager.DisplayMode = mode;
    }


}
