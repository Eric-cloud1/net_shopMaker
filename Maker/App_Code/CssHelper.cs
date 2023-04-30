using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Payments;

/// <summary>
/// Summary description for CssHelper
/// </summary>
public static class CssHelper
{
    public static string GetPaymentStatusCssClass(object payment)
    {
        Payment tempPayment = payment as Payment;
        if (tempPayment != null) return GetPaymentStatusCssClass(tempPayment.PaymentStatus);
        return "ErrorCondition";
    }

    public static string GetPaymentStatusCssClass(PaymentStatus paymentStatus)
    {
        switch (paymentStatus)
        {
            case PaymentStatus.Unprocessed:
            case PaymentStatus.Authorized:
            case PaymentStatus.Captured:
                return "GoodCondition";
            case PaymentStatus.AuthorizationPending:
            case PaymentStatus.CapturePending:
            case PaymentStatus.Refunded:
            case PaymentStatus.Void:
                return "WarnCondition";
            default:
                return "ErrorCondition";
        }
    }
}
