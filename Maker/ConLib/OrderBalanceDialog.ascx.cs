using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Common;
using MakerShop.Orders;
using MakerShop.Payments;
using MakerShop.Utility;

public partial class ConLib_OrderBalanceDialog : System.Web.UI.UserControl
{
    private Order _Order;
    private int _OrderId = 0;
    private string _Caption = "Balance Due";

    [Personalizable(), WebBrowsable()]
    public string Caption
    {
        get { return _Caption; }
        set { _Caption = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        _OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
        _Order = OrderDataSource.Load(_OrderId);
        if (_Order != null && _Order.OrderStatus.IsValid)
        {
            LSDecimal balance = _Order.GetCustomerBalance();
            if (balance > 0)
            {
                if (!OrderHasSubscriptionPayments())
                {
                    phCaption.Text = this.Caption;
                    phInstructionText.Text = string.Format(phInstructionText.Text, _OrderId);
                    PayLink.NavigateUrl += "?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString();
                }
                else
                {
                    phInstructionText2.Text = string.Format(phInstructionText2.Text, _OrderId);
                    PayOrderPanel.Visible = false;
                    ContactUsPanel.Visible = true;
                }
            }
            else this.Controls.Clear();
        }
        else this.Controls.Clear();
    }

    private bool OrderHasSubscriptionPayments()
    {
        foreach (Payment payment in _Order.Payments)
        {
            if (payment.SubscriptionId != 0) return true;
        }
        return false;
    }
}
