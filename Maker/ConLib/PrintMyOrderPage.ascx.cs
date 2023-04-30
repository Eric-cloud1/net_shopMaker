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

public partial class ConLib_PrintMyOrderPage : System.Web.UI.UserControl
{
    private int _OrderId;
    private Order _Order;

    protected Order Order { get { return _Order; } }

    protected void Page_Init(object sender, EventArgs e)
    {
        _OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
        _Order = OrderDataSource.Load(_OrderId);
        if (_Order == null) Response.Redirect(NavigationHelper.GetStoreUrl(this.Page, "Members/MyAccount.aspx"));
        if (_Order.UserId != Token.Instance.UserId) Response.Redirect(NavigationHelper.GetStoreUrl(this.Page, "Members/MyAccount.aspx"));
        if (!Page.IsPostBack)
        {
            //UPDATE CAPTION
            Caption.Text = String.Format(Caption.Text, _Order.OrderNumber);
            BindOrder();
        }
    }

    /// <summary>
    /// Gets the order balance that should be displayed to a customer.
    /// </summary>
    /// <returns>The order balance for customer display.</returns>
    /// <remarks>Order balance displayed to customer is not the actual balance.  It include payments that may not be fully processed.</remarks>
    protected LSDecimal GetOrderBalance()
    {
        LSDecimal balance = _Order.TotalCharges;
        foreach (Payment payment in _Order.Payments)
        {
            switch (payment.PaymentStatus)
            {
                case PaymentStatus.Unprocessed:
                case PaymentStatus.AuthorizationPending:
                case PaymentStatus.Authorized:
                case PaymentStatus.AuthorizationFailed:
                case PaymentStatus.CapturePending:
                case PaymentStatus.Captured:
                case PaymentStatus.CaptureFailed:
                    balance -= payment.Amount;
                    break;
            }
        }
        return balance;
    }

    protected void BindOrder()
    {
        LSDecimal balance = GetOrderBalance();
        LSDecimal payments = _Order.TotalCharges - balance;
        StoreName.Text = Token.Instance.Store.Name;
        StoreAddress.Text = Token.Instance.Store.DefaultWarehouse.FormatAddress(true);
        OrderNumber.Text = _Order.OrderNumber.ToString();
        OrderDate.Text = string.Format("{0:d}", _Order.OrderDate);
        OrderStatus.Text = _Order.OrderStatus.DisplayName;
        BillToAddress.Text = _Order.FormatAddress(true);
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
        BindOrderNotes();
    }

    protected void ShipmentRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //CAST DATA ITEM
        OrderShipment shipment = (OrderShipment)e.Item.DataItem;
        //UPDATE CAPTION
        Localize shipmentCaption = (Localize)e.Item.FindControl("ShipmentCaption");
        if (shipmentCaption != null)
        {
            shipmentCaption.Text = string.Format(shipmentCaption.Text, (e.Item.ItemIndex + 1), _Order.Shipments.Count);
        }
    }

    protected string GetShipToAddress(object dataItem)
    {
        OrderShipment shipment = (OrderShipment)dataItem;
        return shipment.FormatToAddress(true);
    }

    /// <summary>
    /// Handles binding of rows so that certain payment methods can have adjusted displays.
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">argument</param>
    protected void PaymentsGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Payment paymentItem = e.Row.DataItem as Payment;
            if ((paymentItem.PaymentStatus == PaymentStatus.Unprocessed) && (paymentItem.PaymentMethod.PaymentInstrument == PaymentInstrument.PayPal))
            {
                PaymentGateway gateway = MakerShop.Payments.Providers.PayPal.PayPalProvider.GetPayPalPaymentGateway(true);
                if (gateway != null)
                {
                    MakerShop.Payments.Providers.PayPal.PayPalProvider provider = (MakerShop.Payments.Providers.PayPal.PayPalProvider)gateway.GetInstance();
                    Control payNowButton = provider.GetPayNowButton(_Order, paymentItem.PaymentId);
                    if (payNowButton != null)
                    {
                        TableCell statusCell = e.Row.Cells[2];
                        statusCell.Controls.Clear();
                        statusCell.Controls.Add(payNowButton);
                    }
                }
            }
        }
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

    private OrderItemDigitalGoodCollection GetDigitalGoods()
    {
        OrderItemDigitalGoodCollection oidgList = new OrderItemDigitalGoodCollection();
        foreach (OrderItem orderItem in _Order.Items)
        {
            foreach (OrderItemDigitalGood oidg in orderItem.DigitalGoods)
            {
                if (oidg.DigitalGood != null) oidgList.Add(oidg);
            }
        }
        return oidgList;
    }

    protected string GetMaxDownloads(object dataItem)
    {
        OrderItemDigitalGood oidg = (OrderItemDigitalGood)dataItem;
        if (oidg.DigitalGood == null) return string.Empty;
        if (oidg.DigitalGood.MaxDownloads == 0) return "(unlimited)";
        return string.Format("(max {0})", oidg.DigitalGood.MaxDownloads);
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

    private void BindOrderNotes()
    {
        OrderNoteCollection publicNotes = GetPublicNotes();
        if (publicNotes.Count == 0) OrderNotesPanel.Visible = false;
        OrderNotesGrid.DataSource = publicNotes;
        OrderNotesGrid.DataBind();
    }

    private OrderNoteCollection GetPublicNotes()
    {
        OrderNoteCollection publicNotes = new OrderNoteCollection();
        foreach (OrderNote note in _Order.Notes)
        {
            if (!note.IsPrivate) publicNotes.Add(note);
        }
        return publicNotes;
    }

    protected void BackButton_Click(object sender, EventArgs e)
    {
        Response.Redirect( "~/Members/MyOrder.aspx?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString());
    }

    protected void ItemsGrid_DataBinding(object sender, EventArgs e)
    {
        GridView grid = (GridView)sender;
        grid.Columns[3].Visible = TaxHelper.ShowTaxColumn;
        grid.Columns[3].HeaderText = TaxHelper.TaxColumnHeader;
    }
}
