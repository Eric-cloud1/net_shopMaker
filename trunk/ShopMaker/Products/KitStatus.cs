using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Products
{
    /// <summary>
    /// Enumeration that represents the status of a product as kit
    /// </summary>
    public enum KitStatus
    {
        /// <summary>
        /// Product is not a kit
        /// </summary>
        None, 
        
        /// <summary>
        /// Product is a base product (master) for a kit
        /// </summary>
        Master, 
        
        /// <summary>
        /// Product is a component (member) of another kit 
        /// </summary>
        Member
    }
}
