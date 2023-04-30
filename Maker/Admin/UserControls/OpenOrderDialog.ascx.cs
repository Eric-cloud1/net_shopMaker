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
using MakerShop.Utility;
using MakerShop.Orders;

public partial class Admin_UserControls_OpenOrderDialog : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        PageHelper.SetDefaultButton(OrderId, OpenOrderButton.ClientID);
    }

    protected void OpenOrderButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        int orderId = AlwaysConvert.ToInt(OrderId.Text);
        Order order = OrderDataSource.Load(orderId);
        if (order != null)
        {
            Response.Redirect("ViewOrder.aspx?OrderId=" + orderId.ToString());
        }
    }
}
