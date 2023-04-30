using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Common;
using MakerShop.Orders;
using MakerShop.Users;
using MakerShop.Shipping;
using MakerShop.Utility;

public partial class ConLib_MyOrderHistoryPage : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        OrderHistoryCaption.Text = string.Format(OrderHistoryCaption.Text, Token.Instance.User.PrimaryAddress.FullName);
        OrderGrid.DataSource = OrderDataSource.LoadForUser(Token.Instance.UserId, "OrderDate DESC");
        OrderGrid.DataBind();
    }

    protected string GetShipTo(object dataItem)
    {
        Order order = (Order)dataItem;
        List<string> recipients = new List<string>();
        foreach (OrderShipment shipment in order.Shipments)
        {
            string name = shipment.ShipToFullName;
            if (!recipients.Contains(name)) recipients.Add(name);
        }
        return string.Join(", ", recipients.ToArray());
    }

    protected string GetOrderStatus(object dataItem)
    {
        Order order = (Order)dataItem;
        OrderStatus status = order.OrderStatus;
        if (status == null) return string.Empty;
        return StringHelper.SpaceName(status.Name);
    }
}