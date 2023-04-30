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

public partial class Admin_Orders_Payments_CancelRecurringPayment : MakerShop.Web.UI.MakerShopAdminPage
{

    private int _OrderId = 0;
    private Order _Order;
    private int _PaymentId = 0;
    private Payment _Payment;

    protected void InitVars()
    {
        _PaymentId = AlwaysConvert.ToInt(Request.QueryString["PaymentId"]);
        _Payment = PaymentDataSource.Load(_PaymentId);
        if (_Payment == null) Response.Redirect("../Default.aspx");
        _OrderId = _Payment.OrderId;
        _Order = _Payment.Order;
    }

    protected bool SupportsVoid()
    {
        Transaction lastAuth = (Transaction)_Payment.Transactions.LastAuthorization;
        if ((lastAuth != null) && (lastAuth.PaymentGateway != null))
        {
            IPaymentProvider instance = lastAuth.PaymentGateway.GetInstance();
            if (instance != null)
                return ((instance.SupportedTransactions & SupportedTransactions.Void) == SupportedTransactions.Void);
        }
        return false;
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        InitVars();
        if (!Page.IsPostBack)
        {
            PaymentReference.Text = _Payment.PaymentMethodName + " - " + _Payment.ReferenceNumber;
            LSDecimal authorizations = _Payment.Transactions.GetTotalAuthorized();
            LSDecimal captures = _Payment.Transactions.GetTotalCaptured();
            AuthorizationAmount.Text = string.Format("{0:lc}", authorizations);
            CaptureAmount.Text = string.Format("{0:lc}", captures);
            VoidAmount.Text = string.Format("{0:lc}", (authorizations - captures));
            trProcessVoid.Visible = SupportsVoid();
            trForceVoid.Visible = !trProcessVoid.Visible;
        }
    }

    protected void SubmitVoidButton_Click(object sender, EventArgs e)
    {
        _Payment.Void();
        if (!string.IsNullOrEmpty(CustomerNote.Text))
        {
            OrderNote note = new OrderNote(_Order.OrderId, Token.Instance.UserId, DateTime.UtcNow, CustomerNote.Text, NoteType.Public);
            note.Save();
        }
        Response.Redirect( "Default.aspx?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString());
    }

    protected void CancelVoidButton_Click(object sender, EventArgs e)
    {
        Response.Redirect( "Default.aspx?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString());
    }

}
