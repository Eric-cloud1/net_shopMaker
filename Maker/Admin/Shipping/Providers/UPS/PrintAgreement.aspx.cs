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
using MakerShop.Shipping;
using MakerShop.Shipping.Providers.UPS;

public partial class Admin_Shipping_UPS_PrintAgreement : MakerShop.Web.UI.MakerShopAdminPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        LicenseAgreement.Text = UPS.GetAgreement();               
    }

}
