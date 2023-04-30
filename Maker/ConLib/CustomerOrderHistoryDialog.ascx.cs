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
using MakerShop.Common;
using MakerShop.Orders;

public partial class ConLib_CustomerOrderHistoryDialog : System.Web.UI.UserControl
{
    private string _Caption = "My Recent Orders";

    [Personalizable(), WebBrowsable()]
    public string Caption
    {
        get { return _Caption; }
        set { _Caption = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        CaptionText.Text = this.Caption;
        OrderList.DataSource = OrderDataSource.LoadForUser(Token.Instance.UserId, 5, 0, "OrderDate DESC");
        OrderList.DataBind();
    }

    protected string GetOrderLabel(object dataItem)
    {
        Order order = (Order)dataItem;
        return string.Format("Order #{0} - {1:ulc}", order.OrderNumber, order.TotalCharges);
    }
}
