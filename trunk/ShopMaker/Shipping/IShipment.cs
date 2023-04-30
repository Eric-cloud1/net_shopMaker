using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Orders;
using MakerShop.Users;

namespace MakerShop.Shipping
{
    /// <summary>
    /// Interface representing a shipment
    /// </summary>
    public interface IShipment
    {
        /// <summary>
        /// Gets the collection of basket items in the shipment
        /// </summary>
        /// <returns>Collection of BasketItem objects</returns>
        BasketItemCollection GetItems();

        /// <summary>
        /// Gets or sets the warehouse ID for this shipment
        /// </summary>
        int WarehouseId { get; set; }

        /// <summary>
        /// Gets the warehouse from where to ship this shipment
        /// </summary>
        Warehouse Warehouse { get;}

        /// <summary>
        /// Gets the address to which this shipment is destined
        /// </summary>
        Address Address { get;}

        /// <summary>
        /// Gets a collection of zones that contain the shipping destination
        /// </summary>
        ShipZoneCollection ShipZones { get;}
    }
}