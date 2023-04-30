using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using MakerShop.Catalog;
using MakerShop.Data;
using MakerShop.DigitalDelivery;
using MakerShop.Marketing;
using MakerShop.Messaging;
using MakerShop.Orders;
using MakerShop.Payments;
using MakerShop.Payments.Providers;
using MakerShop.Products;
using MakerShop.Reporting;
using MakerShop.Shipping;
using MakerShop.Stores;
using MakerShop.Taxes;
using MakerShop.Taxes.Providers;
using MakerShop.Users;
using MakerShop.Utility;

public partial class Admin_Orders_ViewDownloads : MakerShop.Web.UI.MakerShopAdminPage
{

    private OrderItemDigitalGood _OrderItemDigitalGood;
    private int _OrderItemDigitalGoodId = 0;

    protected OrderItemDigitalGood OrderItemDigitalGood
    {
        get
        {
            if (_OrderItemDigitalGood == null)
            {
                _OrderItemDigitalGood = OrderItemDigitalGoodDataSource.Load(this.OrderItemDigitalGoodId);
            }
            return _OrderItemDigitalGood;
        }
    }

    protected int OrderItemDigitalGoodId
    {
        get
        {
            if (_OrderItemDigitalGoodId.Equals(0))
            {
                _OrderItemDigitalGoodId = AlwaysConvert.ToInt(Request.QueryString["OrderItemDigitalGoodId"]);
            }
            return _OrderItemDigitalGoodId;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (OrderItemDigitalGood == null) Response.Redirect("Default.aspx");
        string name = (OrderItemDigitalGood.DigitalGood != null) ? OrderItemDigitalGood.DigitalGood.Name : OrderItemDigitalGood.OrderItem.Name;
        Caption.Text = string.Format(Caption.Text, name);
        DownloadsGrid.DataSource = OrderItemDigitalGood.Downloads;
        DownloadsGrid.DataBind();
        Order order = OrderItemDigitalGood.OrderItem.Order;
        DigitalGoodsLink.NavigateUrl += "?OrderNumber=" + order.OrderNumber.ToString() + "&OrderId=" + order.OrderId.ToString();
    }

}
