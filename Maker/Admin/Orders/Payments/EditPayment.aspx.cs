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

public partial class Admin_Orders_Payments_EditPayment : MakerShop.Web.UI.MakerShopAdminPage
{

    private int _PaymentId;
    private Payment _Payment;

    protected void Page_Load(object sender, EventArgs e)
    {
        _PaymentId = AlwaysConvert.ToInt(Request.QueryString["PaymentId"]);
        _Payment = PaymentDataSource.Load(_PaymentId);
        if (_Payment == null) Response.Redirect("../Default.aspx");
        if (!Page.IsPostBack)
        {
            PaymentMethod paymentMethod = _Payment.PaymentMethod;
            if ((paymentMethod != null) && (paymentMethod.IsCreditOrDebitCard() || paymentMethod.PaymentInstrument == PaymentInstrument.PayPal))
            {
                foreach (PaymentStatus s in Enum.GetValues(typeof(PaymentStatus)))
                {
                    ListItem item = new ListItem(StringHelper.SpaceName(s.ToString()), ((int)s).ToString());
                    if (s == _Payment.PaymentStatus) item.Selected = true;
                    CurrentPaymentStatus.Items.Add(item);
                }
            }
            else
            {
                ListItem item = new ListItem(StringHelper.SpaceName(PaymentStatus.Unprocessed.ToString()), ((int)PaymentStatus.Unprocessed).ToString());
                if (PaymentStatus.Unprocessed == _Payment.PaymentStatus) item.Selected = true;
                CurrentPaymentStatus.Items.Add(item);

                item = new ListItem(StringHelper.SpaceName(PaymentStatus.Completed.ToString()), ((int)PaymentStatus.Completed).ToString());
                if (PaymentStatus.Completed == _Payment.PaymentStatus) item.Selected = true;
                CurrentPaymentStatus.Items.Add(item);

                item = new ListItem(StringHelper.SpaceName(PaymentStatus.Void.ToString()), ((int)PaymentStatus.Void).ToString());
                if (PaymentStatus.Void == _Payment.PaymentStatus) item.Selected = true;
                CurrentPaymentStatus.Items.Add(item);
            }
            PaymentDate.Text = string.Format("{0:g}", _Payment.PaymentDate);
            Amount.Text = string.Format("{0:F2}", _Payment.Amount);
            PaymentMethod.Text = _Payment.PaymentMethodName;
            Caption.Text = string.Format(Caption.Text, _Payment.PaymentMethodName, _Payment.ReferenceNumber);
        }
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        int orderNumber = OrderDataSource.LookupOrderNumber(_Payment.OrderId);
        Response.Redirect( "Default.aspx?OrderNumber=" + orderNumber.ToString() + "&OrderId=" + _Payment.OrderId.ToString());
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        _Payment.PaymentStatus = (PaymentStatus)(AlwaysConvert.ToInt(CurrentPaymentStatus.SelectedValue));
        _Payment.Save();
        int orderNumber = OrderDataSource.LookupOrderNumber(_Payment.OrderId);
        Response.Redirect( "Default.aspx?OrderNumber=" + orderNumber.ToString() + "&OrderId=" + _Payment.OrderId.ToString());
    }

}
