using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Orders
{
    /// <summary>
    /// Enumeration that represents an inventory action 
    /// </summary>
    public enum InventoryAction : int
    {
        /// <summary>
        /// No action.
        /// </summary>
        None = 0, 
        
        /// <summary>
        /// Destock the inventory.
        /// </summary>
        Destock, 
        
        /// <summary>
        /// Restock the inventory.
        /// </summary>
        Restock
    }
}
