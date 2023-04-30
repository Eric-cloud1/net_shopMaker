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

public partial class Admin_Orders_Batch_Ship : MakerShop.Web.UI.MakerShopAdminPage
{

    private OrderCollection _Orders;
    private OrderShipmentCollection _Shipments;
    private string _OrderNumbers;
    private string _InvalidOrderNumbers;

    private List<int> GetSelectedOrders()
    {
        //CHECK FOR QUERYSTRING PARAMETERS
        List<int> selectedOrders = new List<int>();
        string orderNumberList = Request.QueryString["orders"];
        if (!string.IsNullOrEmpty(orderNumberList))
        {
            int[] orderNumberArray = AlwaysConvert.ToIntArray(orderNumberList);
            Dictionary<int, int> orderIdLookup = OrderDataSource.LookupOrderIds(orderNumberArray);
            foreach (int orderNumber in orderIdLookup.Keys)
            {
                selectedOrders.Add(orderIdLookup[orderNumber]);
            }
        }
        return selectedOrders;
    }

    protected void LoadOrders(params int[] orderIds)
    {
        _Orders = new OrderCollection();
        foreach (int orderId in orderIds)
        {
            Order order = OrderDataSource.Load(orderId);
            if (order != null) _Orders.Add(order);
        }
        _Orders.Sort("OrderId");
        List<string> OrderNumbers = new List<string>();
        foreach (Order order in _Orders)
            OrderNumbers.Add(order.OrderNumber.ToString());
        _OrderNumbers = string.Join(", ", OrderNumbers.ToArray());
    }
    
    protected void LoadShipments(params int[] orderIds)
    {
        //GET THE ORDER COLLECTION
        LoadOrders(orderIds);
        //BUILD THE SHIPMENT COLLECTION
        _Shipments = new OrderShipmentCollection();
        List<int> invalidOrderNumbers = new List<int>();
        foreach (Order order in _Orders)
        {
            bool foundShipment = false;
            foreach (OrderShipment shipment in order.Shipments)
            {
                if (!shipment.IsShipped)
                {
                    _Shipments.Add(shipment);
                    foundShipment = true;
                }
            }
            if (!foundShipment) invalidOrderNumbers.Add(order.OrderNumber);
        }
        //build invalid order lists
        invalidOrderNumbers.Sort();
        List<string> OrderNumbers = new List<string>();
        foreach (int number in invalidOrderNumbers)
            OrderNumbers.Add(number.ToString());
        _InvalidOrderNumbers = string.Join(", ", OrderNumbers.ToArray());
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        List<int> selectedOrders = GetSelectedOrders();
        if ((selectedOrders == null) || (selectedOrders.Count == 0)) Response.Redirect("~/Admin/Orders/Default.aspx");
        LoadShipments(selectedOrders.ToArray());
        ShipmentGrid.DataSource = _Shipments;
        ShipmentGrid.DataBind();
        OrderList.Text = _OrderNumbers;
        InvalidOrdersPanel.Visible = (!string.IsNullOrEmpty(_InvalidOrderNumbers));
        if (InvalidOrdersPanel.Visible) InvalidOrderList.Text = _InvalidOrderNumbers;
    }

    protected string GetOrderStatus(Object orderStatusId)
    {
        OrderStatus status = OrderStatusDataSource.Load((int)orderStatusId);
        if (status != null) return status.Name;
        return string.Empty;
    }

    protected string GetPaymentStatus(object dataItem)
    {
        Order order = (Order)dataItem;
        if (order.PaymentStatus == OrderPaymentStatus.Paid) return "Paid";
        if (order.Payments.Count > 0)
        {
            order.Payments.Sort("PaymentDate");
            Payment lastPayment = order.Payments[order.Payments.Count - 1];
            return StringHelper.SpaceName(lastPayment.PaymentStatus.ToString());
        }
        return string.Empty;
    }

    protected void ShipButton_Click(object sender, EventArgs e)
    {
        foreach (GridViewRow row in ShipmentGrid.Rows)
        {
            DateTime shipDate = LocaleHelper.LocalNow;
            int shipmentId = (int)ShipmentGrid.DataKeys[row.DataItemIndex].Value;
            int index = _Shipments.IndexOf(shipmentId);
            if (index > -1) {
                OrderShipment shipment = _Shipments[index];
                TextBox tb = row.FindControl("ShipDate") as TextBox;
                if (tb != null) {
                    shipDate = AlwaysConvert.ToDateTime(tb.Text, LocaleHelper.LocalNow);
                }
                DropDownList ddl = row.FindControl("ShipGateway") as DropDownList;
                tb = row.FindControl("TrackingNumber") as TextBox;
                if ((ddl != null) && (tb != null)) {
                    TrackingNumber tn = new TrackingNumber();
                    tn.OrderShipmentId = shipmentId;
                    tn.ShipGatewayId = AlwaysConvert.ToInt(ddl.SelectedValue);
                    tn.TrackingNumberData = tb.Text;
                    shipment.TrackingNumbers.Add(tn);
                    shipment.TrackingNumbers.Save();
                }
                shipment.Ship(shipDate);
            }
        }
        UpdatePanel.Visible = false;
        ConfirmPanel.Visible = true;
    }
}
