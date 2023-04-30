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
using MakerShop.Payments;
using MakerShop.Utility;

public partial class ConLib_MyGiftCertificatePage : System.Web.UI.UserControl
{
    private int _GiftCertificateId;
    private GiftCertificate _GiftCertificate;

    protected void Page_Init(object sender, EventArgs e)
    {
        _GiftCertificateId = AlwaysConvert.ToInt(Request.QueryString["GiftCertificateId"]);
        _GiftCertificate = GiftCertificateDataSource.Load(_GiftCertificateId);
        if (_GiftCertificate == null) Response.Redirect("MyAccount.aspx");
        if ((_GiftCertificate.OrderItem.Order.UserId != Token.Instance.UserId) && (!Token.Instance.User.IsInRole("Admin"))) Response.Redirect("MyAccount.aspx");

        if (!Page.IsPostBack)
        {
            //UPDATE CAPTION
            if (_GiftCertificate.SerialNumber == null || _GiftCertificate.SerialNumber.Length == 0)
            {
                Caption.Text = "Not Assigned. Gift Certificate is not validated yet.";
            }
            else
            {
                Caption.Text = String.Format(Caption.Text, _GiftCertificate.Name, _GiftCertificate.OrderItem.Order.OrderNumber);
            }
            BindGiftCertificate();
        }
    }

    protected void BindGiftCertificate()
    {
        Name.Text = _GiftCertificate.Name;
        Serial.Text = _GiftCertificate.SerialNumber;
        Balance.Text = String.Format("{0:ulc}", _GiftCertificate.Balance);

        // SHOW LAST DESCRIPTION
        if (_GiftCertificate.Transactions != null && _GiftCertificate.Transactions.Count > 0)
        {
            GiftCertificateTransaction gct = _GiftCertificate.Transactions[_GiftCertificate.Transactions.Count - 1];
            Description.Text = gct.Description + " (Modified at " + String.Format("{0:d}", gct.TransactionDate) + ")";

        }

        if (_GiftCertificate.IsExpired())
        {
            Expires.Text = "Already expired at " + _GiftCertificate.ExpirationDate.ToString();
        }
        else if (_GiftCertificate.ExpirationDate == null || _GiftCertificate.ExpirationDate.Equals(DateTime.MinValue))
        {
            Expires.Text = "N/A";
        }
        else
        {
            Expires.Text = _GiftCertificate.ExpirationDate.ToString();
        }
    }

    protected void BackButton_Click(object sender, EventArgs e)
    {
        Response.Redirect(NavigationHelper.GetViewOrderUrl(_GiftCertificate.OrderItem.OrderId));
    }
}
