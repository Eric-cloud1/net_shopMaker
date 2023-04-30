using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Products
{
    /// <summary>
    /// Enumeration that represents the inventory mode of a product
    /// </summary>
    public enum InventoryMode
    {
        /// <summary>
        /// Inventory is not enabled
        /// </summary>
        None, 
        
        /// <summary>
        /// Inventory is managed for the product itself
        /// </summary>
        Product, 
        
        /// <summary>
        /// Inventory is managed for the product variants
        /// </summary>
        Variant
    }
}
