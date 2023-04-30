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

public partial class Admin_Products_Specials_Default : MakerShop.Web.UI.MakerShopAdminPage
{

    private int _ProductId;
    private Product _Product;
    
    protected void Page_Load(object sender, EventArgs e)
    {
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _Product = ProductDataSource.Load(_ProductId);
        if (_Product == null) Response.Redirect("../../Catalog/Browse.aspx?CategoryId=" + PageHelper.GetCategoryId());
        if (!Page.IsPostBack)
        {
            Caption.Text = string.Format(Caption.Text, _Product.Name);
            AddLink.NavigateUrl += "?ProductId=" + _ProductId.ToString();
        }
    }

    protected string GetDate(object dataItem)
    {
        DateTime date = (DateTime)dataItem;
        if (date.Equals(DateTime.MinValue)) return string.Empty;
        return string.Format("{0:d}", date);
    }

    protected string GetNames(object dataItem)
    {
        Special special = (Special)dataItem;
        List<string> groupNames = new List<string>();
        foreach (SpecialGroup link in special.SpecialGroups)
        {
            groupNames.Add(link.Group.Name);
        }
        return string.Join(", ", groupNames.ToArray());
    }
}
