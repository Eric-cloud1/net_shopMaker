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

public partial class Admin_Orders_CancelOrder : MakerShop.Web.UI.MakerShopAdminPage
{

    private int _OrderId = 0;
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

    protected int OrderId
    {
        get
        {
            if (_OrderId.Equals(0))
            {
                _OrderId = PageHelper.GetOrderId();
            }
            return _OrderId;
        }
    }
    
    protected void CancelButton_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Comment.Text))
        {
            Order.Notes.Add(new OrderNote(OrderId, Token.Instance.UserId, DateTime.UtcNow, Comment.Text, IsPrivate.Checked ? NoteType.Private : NoteType.Public));
            Order.Notes.Save();
        }
        Order.Cancel(false);
        Response.Redirect( "ViewOrder.aspx?OrderNumber=" + Order.OrderNumber.ToString() + "&OrderId=" + OrderId.ToString());
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack) BackButton.NavigateUrl = "ViewOrder.aspx?OrderNumber=" + Order.OrderNumber.ToString() + "&OrderId=" + OrderId.ToString();
    }

}
