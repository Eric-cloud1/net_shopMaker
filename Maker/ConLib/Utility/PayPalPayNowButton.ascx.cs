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
using MakerShop.Orders;
using MakerShop.Payments;
using MakerShop.Payments.Providers.PayPal;

public partial class ConLib_Utility_PayPalPayNowButton : System.Web.UI.UserControl
{
    private int _PaymentId;
    public int PaymentId
    {
        get
        {
            return _PaymentId;
        }
        set
        {
            _PaymentId = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Payment paymentItem = PaymentDataSource.Load(this.PaymentId);
        if (paymentItem != null)
        {
            if (((paymentItem.PaymentStatus == PaymentStatus.Unprocessed) || (paymentItem.PaymentStatus == PaymentStatus.AuthorizationPending)) && (paymentItem.PaymentMethod != null) && (paymentItem.PaymentMethod.PaymentInstrument == PaymentInstrument.PayPal))
            {
                PaymentGateway gateway = MakerShop.Payments.Providers.PayPal.PayPalProvider.GetPayPalPaymentGateway(true);
                if (gateway != null)
                {
                    PayPalProvider provider = (PayPalProvider)gateway.GetInstance();
                    Control payNowButton = provider.GetPayNowButton(paymentItem.Order, paymentItem.PaymentId);
                    if (payNowButton != null) phPayNow.Controls.Add(payNowButton);
                }
            }
        }
    }
}
