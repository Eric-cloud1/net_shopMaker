using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MakerShop.Common;
using MakerShop.Orders;
using MakerShop.Products;
using MakerShop.Taxes;
using MakerShop.Utility;

/// <summary>
/// Classes that assist in retrieving frequently requested data.
/// </summary>
public static class BasketHelper
{
    public static BasketItemCollection GetNonShippingItems(Basket basket)
    {
        BasketItemCollection nonShippingItems = new BasketItemCollection();
        foreach (BasketItem item in basket.Items)
        {
            if (DisplayItemForShipment(item, 0))
                nonShippingItems.Add(item);
        }

        //SHOW TAXES IF SPECIFIED FOR LINE ITEM DISPLAY
        if (TaxHelper.GetEffectiveInvoiceDisplay(basket.User) == TaxInvoiceDisplay.LineItem)
        {
            // LOOP ALL BASKET ITEMS
            foreach (BasketItem item in basket.Items)
            {
                // ONLY EXAMINE TAX ITEMS
                if (item.OrderItemType == OrderItemType.Tax)
                {
                    // DETERMINE THE PARENT ITEM
                    BasketItem parentItem = GetTaxParentItemForShipping(item);
                    // DISPLAY TAX IF PARENT IS DISPLAYED OR IF THIS IS NOT A CHILD ITEM AND IS NOT PART OF ANY SHIPMENT
                    if (nonShippingItems.IndexOf(parentItem.BasketItemId) > -1
                        || (!item.IsChildItem && item.BasketShipmentId == 0))
                    {
                        nonShippingItems.Add(item);
                    }
                }
            }
        }
        nonShippingItems.Sort(new BasketItemComparer());
        return nonShippingItems;
    }

    public static BasketItemCollection GetShipmentItems(object dataItem)
    {
        BasketShipment shipment = dataItem as BasketShipment;
        if (shipment != null) return GetShipmentItems(shipment);
        return null;
    }

    public static BasketItemCollection GetShipmentItems(BasketShipment shipment)
    {
        Basket basket = shipment.Basket;
        BasketItemCollection shipmentProducts = new BasketItemCollection();
        foreach (BasketItem item in basket.Items)
        {
            if (DisplayItemForShipment(item, shipment.BasketShipmentId))
                shipmentProducts.Add(item);
        }

        //SHOW TAXES IF SPECIFIED FOR LINE ITEM DISPLAY
        if (TaxHelper.GetEffectiveInvoiceDisplay(basket.User) == TaxInvoiceDisplay.LineItem)
        {
            // LOOP ALL BASKET ITEMS
            foreach (BasketItem item in basket.Items)
            {
                // ONLY EXAMINE TAX ITEMS
                if (item.OrderItemType == OrderItemType.Tax)
                {
                    // DETERMINE THE PARENT ITEM
                    BasketItem parentItem = GetTaxParentItemForShipping(item);
                    // DISPLAY TAX IF PARENT IS DISPLAYED OR IF THIS IS NOT A CHILD ITEM AND IS PART OF THE SHIPMENT
                    if (shipmentProducts.IndexOf(parentItem.BasketItemId) > -1 
                        || (!item.IsChildItem && item.BasketShipmentId == shipment.BasketShipmentId))
                    {
                        shipmentProducts.Add(item);
                    }
                }
            }
        }
        shipmentProducts.Sort(new BasketItemComparer());
        return shipmentProducts;
    }

    private static BasketItem GetTaxParentItemForShipping(BasketItem item)
    {
        // IF THIS IS NOT A CHILD ITEM, USE SELF AS TAX PARENT
        if (!item.IsChildItem) return item;

        // IF THIS IS NOT A PRODUCT, DEFER TO THE IMMEDIATE PARENT ITEM AS TAX PARENT
        if (item.OrderItemType != OrderItemType.Product)
            return GetTaxParentItemForShipping(item.GetParentItem(false));

        // DETERMINE IF THIS ITEM IS A CHILD PRODUCT IN A KIT
        BasketItem kitMasterItem = item.GetParentItem(true);
        Product kitMasterProduct = kitMasterItem.Product;
        if (kitMasterProduct != null && kitMasterProduct.KitStatus == KitStatus.Master)
        {
            // ITEM IS A CHILD PRODUCT IN A KIT, IF THE KIT IS BUNDLED USE MASTER ITEM AS TAX PARENT
            if (!kitMasterProduct.Kit.ItemizeDisplay) return kitMasterItem;
        }

        // ITEM IS NOT PART OF A KIT, OR IS IN AN ITEMIZED KIT, USE SELF AS TAX PARENT
        return item;
    }

    private static bool DisplayItemForShipment(BasketItem item, int shipmentId)
    {
        // DO NOT INCLUDE ITEMS THAT ARE NOT IN THIS SHIPMENT, GIFT CERTIFICATE PAYMENTS, OR TAXES
        if (item.BasketShipmentId != shipmentId
            || item.OrderItemType == OrderItemType.GiftCertificatePayment
            || item.OrderItemType == OrderItemType.Tax) return false;

        // ALWAYS SHOW ROOT ITEMS AND DISCOUNTS
        if (!item.IsChildItem || item.OrderItemType == OrderItemType.Discount) return true;

        // ONLY NON DISCOUNT CHILD ITEMS REACH HERE.  DO NOT SHOW NON-PRODUCT CHILD ITEMS
        if (item.OrderItemType != OrderItemType.Product) return false;

        // ONLY PRODUCT CHILD ITEMS REACH HERE
        BasketItem parentItem = item.GetParentItem(true);

        // IF THE PARENT ITEM IS ITEMIZED, CHILD PRODUCTS ARE VISIBLE
        if (parentItem.Product != null && parentItem.Product.Kit.ItemizeDisplay) return true;

        // THE PARENT IS NOT ITEMIZED.  IN THIS CASE, WE SHOULD STILL SHOW THE PRODUCT IF THE 
        // PARENT IS IN A DIFFERENT SHIPMENT (OR POSSIBLY NON-SHIPPING) THIS AVOIDS THE 
        // POSSIBILITY THAT A SHIPMENT WILL APPEAR TO NOT CONTAIN ANY PRODUCTS
        return (parentItem.BasketShipmentId != shipmentId);
    }

    public static void SaveBasket(GridView BasketGrid)
    {
        Basket basket = Token.Instance.User.Basket;
        int rowIndex = 0;
        foreach (GridViewRow saverow in BasketGrid.Rows)
        {
            int basketItemId = (int)BasketGrid.DataKeys[rowIndex].Value;
            int itemIndex = basket.Items.IndexOf(basketItemId);
            if ((itemIndex > -1))
            {
                BasketItem item = basket.Items[itemIndex];
                if (item.OrderItemType == OrderItemType.Product && !item.IsChildItem)
                {
                    TextBox quantity = (TextBox)saverow.FindControl("Quantity");
                    if (quantity != null)
                    {
						int qty = AlwaysConvert.ToInt(quantity.Text,item.Quantity);
						if(qty > System.Int16.MaxValue)
						{
							item.Quantity = System.Int16.MaxValue;
						}else{
							item.Quantity = (System.Int16)qty;
						}

                        // Update for Minimum Maximum quantity of product
                        if (item.Quantity < item.Product.MinQuantity)
                        {
                            item.Quantity = item.Product.MinQuantity;
                            quantity.Text = item.Quantity.ToString();
                        }
                        else if ((item.Product.MaxQuantity > 0) && (item.Quantity > item.Product.MaxQuantity))
                        {
                            item.Quantity = item.Product.MaxQuantity;
                            quantity.Text = item.Quantity.ToString();
                        }

                        item.Save();
                    }
                }
                rowIndex++;
            }
        }
        basket.Recalculate();
    }
}
