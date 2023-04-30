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

public partial class Admin_Products_Kits_Default : MakerShop.Web.UI.MakerShopAdminPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        int _CategoryId = PageHelper.GetCategoryId();
        Category _Category = CategoryDataSource.Load(_CategoryId);
        int _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        Product _Product = ProductDataSource.Load(_ProductId);
        if (_Product == null) Response.Redirect(NavigationHelper.GetAdminUrl("Catalog/Browse.aspx?CategoryId=" + PageHelper.GetCategoryId().ToString()));
        KitStatus status = _Product.KitStatus;
        if (_Product.KitStatus == KitStatus.Member) Response.Redirect("ViewPart.aspx?CategoryId=" + _CategoryId.ToString() + "&ProductId=" + _ProductId.ToString());
        Response.Redirect("EditKit.aspx?CategoryId=" + _CategoryId.ToString() + "&ProductId=" + _ProductId.ToString());
    }

}
