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
using MakerShop.Common;

public partial class Admin_Orders_Payments_AddPayment : MakerShop.Web.UI.MakerShopAdminPage
{

    private int _OrderId = 0;
    protected int OrderId
    {
        get
        {
            if (_OrderId.Equals(0))
            {
                _OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
            }
            return _OrderId;
        }
    }

    private Order _Order;
    protected Order Order
    {
        get
        {
            if (_Order == null)
            {
                _Order = OrderDataSource.Load(this.OrderId);
            }
            return _Order;
        }
    }

    protected void RecordPayment_CheckedChanged(object sender, EventArgs e)
    {
        ProcessPaymentPanel.Visible = ProcessPayment.Checked;
        RecordPaymentPanel.Visible = !ProcessPayment.Checked;
    }

    protected void ProcessPayment_CheckedChanged(object sender, EventArgs e)
    {
        ProcessPaymentPanel.Visible = ProcessPayment.Checked;
        RecordPaymentPanel.Visible = !ProcessPayment.Checked;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            LSDecimal actualBalance = Order.GetBalance(false);
            LSDecimal pendingBalance = Order.GetBalance(true);
            Balance.Text = string.Format("{0:lc}", pendingBalance);
            PendingMessage.Visible = (actualBalance != pendingBalance);
        }
    }

}
