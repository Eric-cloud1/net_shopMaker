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
using MakerShop.DigitalDelivery;
using MakerShop.Orders;
using MakerShop.Payments;
using MakerShop.Taxes;
using MakerShop.Utility;
using MakerShop.Shipping.Providers;

public partial class ConLib_MyOrderPage : System.Web.UI.UserControl
{
    private int _OrderId;
    private Order _Order;

    protected int OrderId
    {
        get { return _OrderId; }
    }

    protected Order Order
    {
        get { return _Order; }
    }

    private bool _AllowAddNote = true;
    public bool AllowAddNote
    {
        get { return _AllowAddNote; }
        set { _AllowAddNote = value; }
    }

    private bool _HandleFailedPayments = false;
    public bool HandleFailedPayments
    {
        get { return _HandleFailedPayments; }
        set { _HandleFailedPayments = value; }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        _OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
        _Order = OrderDataSource.Load(_OrderId);
        if (_Order == null) Response.Redirect(NavigationHelper.GetStoreUrl(this.Page, "Members/MyAccount.aspx"));
        if (_Order.UserId != Token.Instance.UserId) Response.Redirect(NavigationHelper.GetStoreUrl(this.Page, "Members/MyAccount.aspx"));
        //UPDATE CAPTION
        Caption.Text = String.Format(Caption.Text, _Order.OrderNumber);
        BindOrder();
        // HIDE SUBSCRIPTIONS PANEL IF THERE IS NO SUBSCRIPTION
        SubscriptionsPanel.Visible = SubscriptionDataSource.LoadForOrder(_OrderId).Count > 0;
        //WARN ABOUT FAILED PAYMENTS
        if (this.HandleFailedPayments) ValidatePayments();
    }

    private Transaction GetLastAuthorization(Payment p)
    {
        for (int i = p.Transactions.Count - 1; i >= 0; i--)
        {
            Transaction t = (Transaction)p.Transactions[i];
            if (t.TransactionType == TransactionType.Authorize ||
                t.TransactionType == TransactionType.AuthorizeCapture) return t;
        }
        return null;
    }

    private void ValidatePayments()
    {
        // ONLY VALIDATE PAYMENT IF THERE IS ONE PAYMENT / ONE TRANSACTION
        if (_Order.Payments.Count == 1 && _Order.Payments[0].Transactions.Count == 1)
        {
            LSDecimal balance = _Order.GetBalance(true);
            if (_Order.OrderStatus.IsValid && balance > 0)
            {
                Payment lastPayment = (Payment)_Order.Payments.LastPayment;
                if (lastPayment != null)
                {
                    // GET LAST AUTHORIZATION (EXCLUDING A RECURRING AUTH)
                    Transaction authTx = GetLastAuthorization(lastPayment);
                    if (authTx != null && authTx.TransactionStatus == TransactionStatus.Failed)
                    {
                        //SEND THE CUSTOMER TO THE PAY ORDER PAGE
                        Response.Redirect(NavigationHelper.GetStoreUrl(this.Page, "Members/PayMyOrder.aspx?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString()));
                    }
                }
            }
        }
    }

    private bool OrderHasSubscriptionPayments()
    {
        foreach (Payment payment in Order.Payments)
        {
            if (payment.SubscriptionId != 0) return true;
        }
        return false;
    }

    protected void BindOrder()
    {
        LSDecimal balance = Order.GetBalance();
        LSDecimal payments = Order.Payments.TotalProcessed();
        LSDecimal unprocessedPayments = Order.Payments.TotalUnprocessed();
        LSDecimal unpaidBalance = Order.GetCustomerBalance();
        if (!_Order.OrderStatus.IsValid)
        {
            OrderInvalidPanel.Visible = true;
        }
        else if (unpaidBalance > 0 && !OrderHasSubscriptionPayments())
        {
            BalanceDuePanel.Visible = true;
            BalanceDueMessage.Text = string.Format(BalanceDueMessage.Text, unpaidBalance, this.Page.ResolveUrl("~/Members/PayMyOrder.aspx?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString()));
        }
        OrderDate.Text = string.Format("{0:g}", this.Order.OrderDate);
        OrderStatus.Text = this.Order.OrderStatus.DisplayName;
        TotalCharges.Text = string.Format("{0:ulc}", Order.TotalCharges);
        TotalPayments.Text = string.Format("{0:ulc}", payments);
        Balance.Text = string.Format("{0:ulc}", balance);
        UnprocessedPayments.Text = string.Format("{0:ulc}", unprocessedPayments);
        BillToAddress.Text = _Order.FormatAddress(true);
        PrintOrder.NavigateUrl += "?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString();
        //BIND PAYMENTS
        PaymentRepeater.DataSource = _Order.Payments;
        PaymentRepeater.DataBind();
        //BIND SHIPMENTS
        ShipmentRepeater.DataSource = _Order.Shipments;
        ShipmentRepeater.DataBind();
        //BIND NONSHIPPING ITEMS
        OrderItemCollection nonShippingItems = OrderHelper.GetNonShippingItems(_Order);
        if (nonShippingItems.Count > 0)
        {
            NonShippingItemsGrid.DataSource = nonShippingItems;
            NonShippingItemsGrid.DataBind();
        }
        else
        {
            NonShippingItemsPanel.Visible = false;
        }
        BindDigitalGoods();
        BindGiftCertificates();
        BindOrderNotes();
    }

    protected string GetShipToAddress(object dataItem)
    {
        OrderShipment shipment = (OrderShipment)dataItem;
        return shipment.FormatToAddress(true);
    }

    /// <summary>
    /// Given a payment item, determine if extended properties should be displayed
    /// </summary>
    /// <param name="dataItem">The payment data item</param>
    /// <returns>True if extended payment properties should be displayed.</returns>
    /// <remarks>We want to show date and amount if there are multiple payments or when amount is not the order total.</remarks>
    protected bool ShowExtendedPaymentDetails(object dataItem)
    {
        return ((_Order.Payments.Count > 1) || (((Payment)dataItem).Amount != _Order.TotalCharges));
    }

    private void BindDigitalGoods()
    {
        OrderItemDigitalGoodCollection digitalGoodsCollection = GetDigitalGoods();
        if (digitalGoodsCollection.Count > 0)
        {
            DigitalGoodsGrid.DataSource = GetDigitalGoods();
            DigitalGoodsGrid.DataBind();
        }
        else
        {
            DigitalGoodsPanel.Visible = false;
        }
    }

    private void BindGiftCertificates()
    {
        GiftCertificateCollection giftCertificateCollection = GetGiftCertificates();
        if (giftCertificateCollection.Count > 0)
        {
            GiftCertificatesGrid.DataSource = giftCertificateCollection;
            GiftCertificatesGrid.DataBind();
        }
        else
        {
            GiftCertificatesPanel.Visible = false;
        }
    }

    private OrderItemDigitalGoodCollection GetDigitalGoods()
    {
        OrderItemDigitalGoodCollection oidgList = new OrderItemDigitalGoodCollection();
        foreach (OrderItem orderItem in this.Order.Items)
        {
            foreach (OrderItemDigitalGood oidg in orderItem.DigitalGoods)
            {
                oidgList.Add(oidg);
            }
        }
        return oidgList;
    }

    private GiftCertificateCollection GetGiftCertificates()
    {

        GiftCertificateCollection gcList = new GiftCertificateCollection();
        foreach (OrderItem orderItem in this.Order.Items)
        {
            foreach (GiftCertificate gc in orderItem.GiftCertificates)
            {
                if (gc != null) gcList.Add(gc);
            }
        }
        return gcList;
    }

    protected String GetGCDescription(object dataItem)
    {
        GiftCertificate gc = (GiftCertificate)dataItem;
        if (gc.Transactions.Count == 0) return String.Empty;
        return gc.Transactions[gc.Transactions.Count - 1].Description;
    }

    protected string GetMaxDownloads(object dataItem)
    {
        OrderItemDigitalGood oidg = (OrderItemDigitalGood)dataItem;
        if (oidg.DigitalGood == null) return string.Empty;
        if (oidg.DigitalGood.MaxDownloads == 0) return "n/a";
        int remDownloads = oidg.DigitalGood.MaxDownloads - oidg.RelevantDownloads;
        return remDownloads.ToString();
    }

    protected string GetDownloadUrl(object dataItem)
    {
        if (!_Order.OrderStatus.IsValid) return "#";
        OrderItemDigitalGood oidg = (OrderItemDigitalGood)dataItem;
        DigitalGood dg = oidg.DigitalGood;
        if ((oidg.DownloadStatus == DownloadStatus.Valid) && (dg != null))
        {
            string downloadUrl = this.Page.ResolveUrl("~/Members/Download.ashx?id=" + oidg.OrderItemDigitalGoodId.ToString());
            if ((dg.LicenseAgreementMode & LicenseAgreementMode.OnDownload) == LicenseAgreementMode.OnDownload)
            {
                if (dg.LicenseAgreement != null)
                {
                    string acceptUrl = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(downloadUrl));
                    string declineUrl = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("javascript:window.close()"));
                    string agreeUrl = this.Page.ResolveClientUrl("~/ViewLicenseAgreement.aspx") + "?id=" + dg.LicenseAgreementId + "&AcceptUrl=" + acceptUrl + "&DeclineUrl=" + declineUrl;
                    return agreeUrl + "\" onclick=\"" + PageHelper.GetPopUpScript(agreeUrl, "license", 640, 480, "resizable=1,scrollbars=yes,toolbar=no,menubar=no,location=no,directories=no") + ";return false";
                }
            }
            return downloadUrl;
        }
        return string.Empty;
    }

    protected bool ShowSerialKey(object dataItem)
    {
        OrderItemDigitalGood oidg = (OrderItemDigitalGood)dataItem;
        if (!string.IsNullOrEmpty(oidg.SerialKeyData) && oidg.SerialKeyData.Length <= 100)
        {
            return true;
        }
        return false;
    }

    protected bool ShowSerialKeyLink(object dataItem)
    {
        OrderItemDigitalGood oidg = (OrderItemDigitalGood)dataItem;
        if (!string.IsNullOrEmpty(oidg.SerialKeyData) && oidg.SerialKeyData.Length > 100)
        {
            return true;
        }
        return false;
    }

    protected string GetPopUpScript(object dataItem)
    {
        OrderItemDigitalGood oidg = (OrderItemDigitalGood)dataItem;
        string url = string.Format("~/Members/MySerialKey.aspx?OrderItemDigitalGoodId={0}", oidg.OrderItemDigitalGoodId);
        url = this.ResolveClientUrl(url);
        //string clientScript = PageHelper.GetPopUpScript(url, "Serial Key", 20, 20);
        string clientScript = string.Format("window.open('{0}'); return false;", url);
        clientScript = "javascript:" + clientScript;
        return clientScript;
    }

    private void BindOrderNotes()
    {
        OrderNoteCollection publicNotes = GetPublicNotes();
        if (publicNotes.Count == 0) OrderNotesPanel.Visible = this.AllowAddNote;
        trAddNote.Visible = this.AllowAddNote;
        OrderNotesGrid.DataSource = publicNotes;
        OrderNotesGrid.DataBind();
    }

    private OrderNoteCollection GetPublicNotes()
    {
        OrderNoteCollection publicNotes = new OrderNoteCollection();
        foreach (OrderNote note in this.Order.Notes)
        {
            if (!note.IsPrivate) publicNotes.Add(note);
        }
        return publicNotes;
    }

    protected string GetTrackingUrl(object dataItem)
    {
        TrackingNumber trackingNumber = (TrackingNumber)dataItem;
        if (trackingNumber.ShipGateway != null)
        {
            IShippingProvider provider = trackingNumber.ShipGateway.GetProviderInstance();
            TrackingSummary summary = provider.GetTrackingSummary(trackingNumber);
            if (summary != null)
            {
                // TRACKING DETAILS FOUND
                if (summary.TrackingResultType == TrackingResultType.InlineDetails)
                {
                    //send to view tracking page
                    return string.Format("MyTrackingInfo.aspx?TrackingNumberId={0}", trackingNumber.TrackingNumberId.ToString());
                }
                else if (summary.TrackingResultType == TrackingResultType.ExternalLink)
                {
                    return summary.TrackingLink;
                }
            }
        }
        return string.Empty;
    }

    protected void DigitalGoodsGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            OrderItemDigitalGood oidg = (OrderItemDigitalGood)e.Row.DataItem;
            if (oidg != null)
            {
                DigitalGood dg = oidg.DigitalGood;
                if (dg != null)
                {
                    if ((dg.LicenseAgreement != null) || (dg.Readme != null))
                    {
                        PlaceHolder phAssets = (PlaceHolder)e.Row.FindControl("phAssets");
                        if (phAssets != null)
                        {
                            string li = "<li><a href=\"{0}\">{1}</a></li>";
                            string encodedUrl = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("javascript:window.close()"));
                            string agreeUrl = this.Page.ResolveClientUrl("~/ViewLicenseAgreement.aspx") + "?id={0}&ReturnUrl=" + encodedUrl;
                            string agreePopup = agreeUrl + "\" onclick=\"" + PageHelper.GetPopUpScript(agreeUrl, "license", 640, 480, "resizable=1,scrollbars=yes,toolbar=no,menubar=no,location=no,directories=no") + ";return false";
                            string readmeUrl = this.Page.ResolveClientUrl("~/ViewReadme.aspx") + "?ReadmeId={0}&ReturnUrl=" + encodedUrl;
                            string readmePopup = "#\" onclick=\"" + PageHelper.GetPopUpScript(readmeUrl, "readme", 640, 480, "resizable=1,scrollbars=yes,toolbar=no,menubar=no,location=no,directories=no") + ";return false";
                            phAssets.Controls.Add(new LiteralControl("<ul>"));
                            if (dg.Readme != null)
                            {
                                string link = string.Format(readmePopup, dg.ReadmeId);
                                phAssets.Controls.Add(new LiteralControl(string.Format(li, link, dg.Readme.DisplayName)));
                            }
                            if (dg.LicenseAgreement != null)
                            {
                                string link = string.Format(agreePopup, dg.LicenseAgreementId);
                                phAssets.Controls.Add(new LiteralControl(string.Format(li, link, dg.LicenseAgreement.DisplayName)));
                            }
                            phAssets.Controls.Add(new LiteralControl("</ul>"));
                        }
                    }
                }
            }
        }
    }

    protected string GetShipStatus(object dataItem)
    {
        OrderShipment shipment = (OrderShipment)dataItem;
        return shipment.IsShipped ? "Shipped" : "Waiting to Ship";
    }

    protected bool HasTrackingNumbers(object dataItem)
    {
        OrderShipment shipment = (OrderShipment)dataItem;
        return (shipment.TrackingNumbers.Count > 0);
    }

    protected void NewOrderNoteButton_Click(object sender, EventArgs e)
    {
        string safeNote = StringHelper.StripHtml(NewOrderNote.Text);
        if (!string.IsNullOrEmpty(safeNote))
        {
            _Order.Notes.Add(new OrderNote(_OrderId, Token.Instance.User.UserId, LocaleHelper.LocalNow, safeNote, NoteType.Public));
            _Order.Notes.Save();
            BindOrderNotes();
        }
        NewOrderNote.Text = string.Empty;
    }

    protected string GetNoteAuthor(object dataItem)
    {
        OrderNote note = (OrderNote)dataItem;
        return (note.UserId == Token.Instance.User.UserId) ? "you" : Token.Instance.Store.Name;
    }

    protected bool ShowMailPaymentMessage(object dataItem)
    {
        Payment payment = (Payment)dataItem;
        return (payment.PaymentMethod != null && payment.PaymentMethod.PaymentInstrument == PaymentInstrument.Mail && payment.PaymentStatus == PaymentStatus.Unprocessed);
    }

    protected void ItemsGrid_DataBinding(object sender, EventArgs e)
    {
        GridView grid = (GridView)sender;
        grid.Columns[3].Visible = TaxHelper.ShowTaxColumn;
        grid.Columns[3].HeaderText = TaxHelper.TaxColumnHeader;
    }

    protected bool DGFileExists(object dataItem)
    {
        DigitalGood dg = dataItem as DigitalGood;
        if (dg != null) return System.IO.File.Exists(dg.AbsoluteFilePath);
        else return false;
    }

    public bool ShowMediakey(Object dataItem)
    {
        OrderItemDigitalGood oidg = (OrderItemDigitalGood)dataItem;
        if (oidg.DigitalGood == null) return false;
        bool showMediaKey = (oidg.DownloadStatus == DownloadStatus.Valid || oidg.DownloadStatus == DownloadStatus.Expired);
        if (showMediaKey)
            showMediaKey = !String.IsNullOrEmpty(oidg.DigitalGood.MediaKey);
        return showMediaKey;
    }
}
