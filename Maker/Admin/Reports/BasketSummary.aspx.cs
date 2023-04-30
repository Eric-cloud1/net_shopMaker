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

public partial class Admin_Reports_BasketSummary : MakerShop.Web.UI.MakerShopAdminPage
{

    private int _BasketId;
    protected void Page_Load(object sender, EventArgs e)
    {
        _BasketId = AlwaysConvert.ToInt(Request.QueryString["BasketId"]);
        if (!Page.IsPostBack)
        { 
            BasketItemsGrid.DataSource = BasketItemDataSource.LoadForBasket(_BasketId);
            BasketItemsGrid.DataBind();
        }
    }

}
