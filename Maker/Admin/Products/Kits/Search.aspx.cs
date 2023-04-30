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

public partial class Admin_Products_Kits_Search : MakerShop.Web.UI.MakerShopAdminPage
{


    protected void SearchButton_Click(object sender, EventArgs e)
    {
        SearchResultsGrid.Visible = true;
        SearchResultsGrid.Columns[0].Visible = ShowImages.Checked;
        SearchResultsGrid.DataBind();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        SearchName.Focus();
    }


}
