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

public partial class Admin_Orders_Print_Invoices : MakerShop.Web.UI.MakerShopAdminPage
{

    private List<string> orderNumbers;
    protected int OrderCount = 0;

    private List<int> GetSelectedOrders()
    {
        //CHECK FOR QUERYSTRING PARAMETERS
        List<int> selectedOrders = new List<int>();
        orderNumbers = new List<string>();
        string orderNumberList = Request.QueryString["orders"];
        if (!string.IsNullOrEmpty(orderNumberList))
        {
            int[] orderNumberArray = AlwaysConvert.ToIntArray(orderNumberList);
            Dictionary<int, int> orderIdLookup = OrderDataSource.LookupOrderIds(orderNumberArray);
            foreach (int orderNumber in orderIdLookup.Keys)
            {
                orderNumbers.Add(orderNumber.ToString());
                selectedOrders.Add(orderIdLookup[orderNumber]);
            }
        }
        return selectedOrders;
    }

    private string GetOrderNumbers(List<int> orders)
    {
        if (orderNumbers == null)
        {
            orderNumbers = new List<string>();
            foreach (int orderId in orders)
            {
                orderNumbers.Add(orderId.ToString());
            }
        }
        if (orderNumbers.Count == 0) return string.Empty;
        return string.Join(", ", orderNumbers.ToArray());
    }

    /// <summary>
    /// Gets a collection of orders from a list of order ids.
    /// </summary>
    /// <param name="orderIds">The orderIds to load.</param>
    /// <returns>A collection of orders from the list of ids.</returns>
    protected OrderCollection GetOrders(params int[] orderIds)
    {
        OrderCollection orders = new OrderCollection();
        foreach (int orderId in orderIds)
        {
            Order order = OrderDataSource.Load(orderId);
            if (order != null)
            {
                orders.Add(order);
            }
        }
        OrderCount = orders.Count;
        return orders;
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        List<int> selectedOrders = GetSelectedOrders();
        if ((selectedOrders == null) || (selectedOrders.Count == 0)) Response.Redirect("~/Admin/Orders/Default.aspx");
        OrderRepeater.DataSource = GetOrders(selectedOrders.ToArray());
        OrderRepeater.DataBind();
        OrderList.Text = GetOrderNumbers(selectedOrders);
    }

    private OrderItemType[] displayItemTypes = { OrderItemType.Product, OrderItemType.Discount, 
        OrderItemType.Coupon, OrderItemType.Shipping, OrderItemType.Handling, OrderItemType.GiftWrap,
        OrderItemType.Charge, OrderItemType.Credit, OrderItemType.GiftCertificate, OrderItemType.Tax};
    protected OrderItemCollection GetItems(object dataItem)
    {
        Order order = (Order)dataItem;
        OrderItemCollection items = new OrderItemCollection();
        foreach (OrderItem item in order.Items)
        {
            if (Array.IndexOf(displayItemTypes, item.OrderItemType) > -1 && (item.OrderItemType != OrderItemType.Tax)) items.Add(item);
        }
        //ADD IN TAX ITEMS IF SPECIFIED FOR DISPLAY
        TaxInvoiceDisplay displayMode = TaxHelper.InvoiceDisplay;
        if (displayMode == TaxInvoiceDisplay.LineItem || displayMode == TaxInvoiceDisplay.LineItemRegistered)
        {
            foreach (OrderItem item in order.Items)
            {
                //IS THIS A TAX ITEM?
                if (item.OrderItemType == OrderItemType.Tax)
                {
                    //IS THE TAX ITEM A PARENT ITEM OR A CHILD OF A DISPLAYED ITEM?
                    if (!item.IsChildItem || (items.IndexOf(item.ParentItemId) > -1))
                    {
                        //TAX SHOULD BE SHOWN
                        items.Add(item);
                    }
                }
            }
        }
        //SORT ITEMS TO COMPLETE INTITIALIZATION
        items.Sort(new OrderItemComparer());
        return items;
    }

    protected string GetBillToAddress(object dataItem)
    {
        return ((Order)dataItem).FormatAddress(true);
    }

    protected string GetShipToAddress(object dataItem)
    {
        Order order = (Order)dataItem;
        List<string> addressList = new List<string>();
        foreach (OrderShipment shipment in order.Shipments)
        {
            string shipTo = shipment.FormatToAddress();
            if (!addressList.Contains(shipTo)) addressList.Add(shipTo);
        }
        if (addressList.Count == 0) return "n/a";
        return string.Join("<hr />", addressList.ToArray());
    }

    protected LSDecimal GetTotal(object dataItem, params OrderItemType[] orderItems)
    {
        return ((Order)dataItem).Items.TotalPrice(orderItems);
    }

    protected bool ShowAdjustmentsRow(object dataItem)
    {
        return (GetAdjustmentsTotal(dataItem) != 0);
    }

    protected LSDecimal GetAdjustmentsTotal(object dataItem)
    {
        Order order = dataItem as Order;
        if (order == null) return 0;
        return order.Items.TotalPrice(OrderItemType.Charge, OrderItemType.Credit, OrderItemType.Coupon, OrderItemType.GiftWrap);
    }

    protected LSDecimal GetProductTotal(object dataItem)
    {
        Order order = dataItem as Order;
        if (order == null) return 0;
        return order.Items.TotalPrice(OrderItemType.Product, OrderItemType.Discount);
    }

    protected LSDecimal GetShippingTotal(object dataItem)
    {
        Order order = dataItem as Order;
        if (order == null) return 0;
        return order.Items.TotalPrice(OrderItemType.Shipping, OrderItemType.Handling);
    }

    protected void OrderItems_DataBinding(object sender, EventArgs e)
    {
        GridView grid = (GridView)sender;
        grid.Columns[2].Visible = TaxHelper.ShowTaxColumn;
        grid.Columns[2].HeaderText = TaxHelper.TaxColumnHeader;
    }
    

}
