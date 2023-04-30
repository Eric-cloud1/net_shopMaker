using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using MakerShop.Common;
using MakerShop.Orders;
using MakerShop.Payments;
using MakerShop.Utility;


public partial class Admin_Tools_QuickLookup : System.Web.UI.Page
{
    private string str_orderId = string.Empty;
    private string str_orderNumber = string.Empty;
    private string str_paymentId = string.Empty;
    private string str_transactionId = string.Empty;
    private string str_productId = string.Empty;
    private Order order = null;
    private Payment payment = null;
    private Transaction transaction = null;
    

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void orderNumber_Click(object sender, EventArgs e)
    {
        str_orderNumber = string.Format("../orders/ViewOrder.aspx?OrderNumber={0}", this.orderNumber.Text);
        Response.Redirect(str_orderNumber);

    }
    protected void orderNumberHistory_Click(object sender, EventArgs e)
    {
        str_orderNumber = string.Format("../orders/OrderHistory.aspx?OrderNumber={0}", this.orderNumberHistory.Text);
        Response.Redirect(str_orderNumber);
    }

    protected void orderId_Click(object sender, EventArgs e)
    {
        str_orderId = string.Format("../orders/ViewOrder.aspx?OrderId={0}", this.orderId.Text);
        Response.Redirect(str_orderId);
    }

    protected void paymentId_Click(object sender, EventArgs e)
    {
        payment = new Payment();

        int paymentId = 0;
        int.TryParse(this.paymentId.Text, out paymentId);
        payment.Load(paymentId);

        if (paymentId == 0)
            return;

        str_paymentId = string.Format("../orders/Payments/Default.aspx?OrderId={0}", payment.OrderId);
        Response.Redirect(str_paymentId);
    }

    protected void transactionId_Click(object sender, EventArgs e)
    {
        transaction = new Transaction();
        int transactionId = 0;
        int.TryParse(this.transactionId.Text, out transactionId);

        if (transactionId == 0)
            return;

        transaction.Load(transactionId);
        str_transactionId = string.Format("../orders/Payments/Default.aspx?OrderId={0}", transaction.Payment.OrderId);
        Response.Redirect(str_transactionId);
    }

    protected void productId_Click(object sender, EventArgs e)
    {
        str_productId = string.Format("../Products/EditProduct.aspx?ProductId={0}", this.productId.Text);
        Response.Redirect(str_productId);
    }
}
