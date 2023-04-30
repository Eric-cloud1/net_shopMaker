using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Orders
{
    /// <summary>
    /// Enumeration that represents the status of an order shipment
    /// </summary>
    public enum OrderShipmentStatus
    {
        /// <summary>
        /// Shipment status is unspecified.
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// Shipment is unshipped.
        /// </summary>
        Unshipped,

        /// <summary>
        /// Shipment is shipped.
        /// </summary>
        Shipped,

        /// <summary>
        /// Shipment is not shippable.
        /// </summary>
        NonShippable
    }
}
