using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using MakerShop.Common;
using MakerShop.Orders;
using MakerShop.Payments;
using MakerShop.Shipping;
using MakerShop.Stores;
using MakerShop.Utility;
using MakerShop.Taxes;
using MakerShop.Users;

public partial class Admin_Orders_Call_CallDisposition : System.Web.UI.Page
{

    private int _OrderId;
    private Order _Order;

    protected void Page_Init(object sender, EventArgs e)
    {
        _Order = OrderHelper.GetOrderFromContext();

        if (_Order == null) Response.Redirect("../Default.aspx");
  

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Caption.Text = String.Format(Caption.Text, _Order.OrderNumber, _Order.OrderStatus.Name);

    }
}
