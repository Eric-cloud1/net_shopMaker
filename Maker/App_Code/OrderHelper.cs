using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MakerShop.Orders;
using MakerShop.Common;
using MakerShop.Utility;
using MakerShop.Taxes;

/// <summary>
/// Classes that assist in retrieving frequently requested data.
/// </summary>
public static class OrderHelper
{
    public static OrderItemCollection GetNonShippingItems(Order order)
    {
        OrderItemCollection nonShippingItems = new OrderItemCollection();
        foreach (OrderItem item in order.Items)
        {
            if (DisplayItemForShipment(item, 0))
                nonShippingItems.Add(item);
        }

        //SHOW TAXES IF SPECIFIED FOR LINE ITEM DISPLAY
        TaxInvoiceDisplay displayMode = TaxHelper.InvoiceDisplay;
        if (displayMode == TaxInvoiceDisplay.LineItem || displayMode == TaxInvoiceDisplay.LineItemRegistered)
        {
            // LOOP ALL BASKET ITEMS
            foreach (OrderItem item in order.Items)
            {
                // ONLY EXAMINE TAX ITEMS
                if (item.OrderItemType == OrderItemType.Tax)
                {
                    // DETERMINE THE PARENT ITEM
                    OrderItem parentItem = GetTaxParentItemForShipping(item);
                    // DISPLAY TAX IF PARENT IS DISPLAYED OR IF THIS IS NOT A CHILD ITEM AND IS NOT PART OF ANY SHIPMENT
                    if (nonShippingItems.IndexOf(parentItem.OrderItemId) > -1
                        || (!item.IsChildItem && item.OrderShipmentId == 0))
                    {
                        nonShippingItems.Add(item);
                    }
                }
            }
        }
        nonShippingItems.Sort(new OrderItemComparer());
        return nonShippingItems;
    }

    public static OrderItemCollection GetShipmentItems(object dataItem)
    {
        OrderShipment shipment = dataItem as OrderShipment;
        if (shipment != null) return GetShipmentItems(shipment);
        return null;
    }

    public static OrderItemCollection GetShipmentItems(OrderShipment shipment)
    {
        Order order = shipment.Order;
        OrderItemCollection shipmentProducts = new OrderItemCollection();
        foreach (OrderItem item in order.Items)
        {
            if (DisplayItemForShipment(item, shipment.OrderShipmentId))
                shipmentProducts.Add(item);
        }

        //SHOW TAXES IF SPECIFIED FOR LINE ITEM DISPLAY
        TaxInvoiceDisplay displayMode = TaxHelper.InvoiceDisplay;
        if (displayMode == TaxInvoiceDisplay.LineItem || displayMode == TaxInvoiceDisplay.LineItemRegistered)
        {
            // LOOP ALL BASKET ITEMS
            foreach (OrderItem item in order.Items)
            {
                // ONLY EXAMINE TAX ITEMS
                if (item.OrderItemType == OrderItemType.Tax)
                {
                    // DETERMINE THE PARENT ITEM
                    OrderItem parentItem = GetTaxParentItemForShipping(item);
                    // DISPLAY TAX IF PARENT IS DISPLAYED OR IF THIS IS NOT A CHILD ITEM AND IS PART OF THE SHIPMENT
                    if (shipmentProducts.IndexOf(parentItem.OrderItemId) > -1
                        || (!item.IsChildItem && item.OrderShipmentId == shipment.OrderShipmentId))
                    {
                        shipmentProducts.Add(item);
                    }
                }
            }
        }
        shipmentProducts.Sort(new OrderItemComparer());
        return shipmentProducts;
    }

    private static OrderItem GetTaxParentItemForShipping(OrderItem item)
    {
        // IF THIS IS NOT A CHILD ITEM, USE SELF AS TAX PARENT
        if (!item.IsChildItem) return item;

        // IF THIS IS NOT A PRODUCT, DEFER TO THE IMMEDIATE PARENT ITEM AS TAX PARENT
        if (item.OrderItemType != OrderItemType.Product)
            return GetTaxParentItemForShipping(item.GetParentItem(false));

        // DETERMINE IF THIS ITEM IS A CHILD PRODUCT IN A KIT
        OrderItem kitMasterItem = item.GetParentItem(true);

        // IF THE MASTER ITEM SPECIFIES BUNDLED DISPLAY USE MASTER ITEM AS TAX PARENT
        if (!kitMasterItem.ItemizeChildProducts) return kitMasterItem;

        // THE PARENT ITEMIZES DISPLAY, USE SELF AS TAX PARENT
        return item;
    }

    private static bool DisplayItemForShipment(OrderItem item, int shipmentId)
    {
        // DO NOT INCLUDE ITEMS THAT ARE NOT IN THIS SHIPMENT, GIFT CERTIFICATE PAYMENTS, TAXES OR HIDDEN ITEMS
        if (item.OrderShipmentId != shipmentId
            || item.OrderItemType == OrderItemType.GiftCertificatePayment
            || item.OrderItemType == OrderItemType.Tax
            || item.IsHidden) return false;

        // ALWAYS SHOW ROOT ITEMS AND DISCOUNTS
        if (!item.IsChildItem || item.OrderItemType == OrderItemType.Discount) return true;

        // ONLY NON DISCOUNT CHILD ITEMS REACH HERE.  DO NOT SHOW NON-PRODUCT CHILD ITEMS
        if (item.OrderItemType != OrderItemType.Product) return false;

        // ONLY PRODUCT CHILD ITEMS REACH HERE
        OrderItem parentItem = item.GetParentItem(true);

        // IF THE PARENT ITEM IS ITEMIZED, AND THIS CHILD IS NOT HIDDEN, IT IS VISIBLE
        if (parentItem.ItemizeChildProducts && !item.IsHidden) return true;

        // EITHER THE PARENT IS NOT ITEMIZED OR THE ITEM IS HIDDEN.  IN THIS CASE, WE SHOULD 
        // STILL SHOW THE PRODUCT IF THE PARENT IS IN A DIFFERENT SHIPMENT (OR POSSIBLY NON-SHIPPING)
        // THIS AVOIDS THE POSSIBILITY THAT A SHIPMENT WILL APPEAR TO NOT CONTAIN ANY PRODUCTS
        return (parentItem.OrderShipmentId != shipmentId);
    }

    /// <summary>
    /// Gets the products in the order that are visible to the customer
    /// </summary>
    /// <param name="dataItem">The order object</param>
    /// <returns>A collection of products that are visible to the customer</returns>
    public static OrderItemCollection GetVisibleProducts(object dataItem)
    {
        OrderItemCollection products = new OrderItemCollection();
        Order order = dataItem as Order;
        if (order != null)
        {
            foreach (OrderItem item in order.Items)
            {
                if (DisplayProductForOrder(item)) products.Add(item);
            }
        }
        return products;
    }

    private static bool DisplayProductForOrder(OrderItem item)
    {
        // DO NOT INCLUDE GIFT CERTIFICATE PAYMENT ITEMS
        if (item.OrderItemType != OrderItemType.Product) return false;

        // ALWAYS SHOW ROOT ITEMS
        if (!item.IsChildItem) return true;

        // ONLY PRODUCT CHILD ITEMS REACH HERE
        OrderItem parentItem = item.GetParentItem(true);

        // IF THE PARENT ITEM IS ITEMIZED AND THIS CHILD IS NOT HIDDEN, PRODUCT IS VISIBLE
        return (parentItem.ItemizeChildProducts && !item.IsHidden);
    }

    /// <summary>
    /// Attempts to get the order from the current context
    /// </summary>
    /// <returns></returns>
    public static Order GetOrderFromContext()
    {
        Order order = null;
        HttpRequest request = HttpContextHelper.SafeGetRequest();
        if (request != null)
        {
            int orderId = AlwaysConvert.ToInt(request.QueryString["OrderId"]);
            order = OrderDataSource.Load(orderId);
            if (order == null)
            {
                int orderNumber = AlwaysConvert.ToInt(request.QueryString["OrderNumber"]);
                if (orderNumber > 0)
                {
                    order = OrderDataSource.Load(OrderDataSource.LookupOrderId(orderNumber));
                }
            }
        }
       
        return order;
    }
}