using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Shipping
{
    /// <summary>
    /// Enum representing the shippable status of a product
    /// </summary>
    public enum Shippable
    {
        /// <summary>
        /// The product is not shippable
        /// </summary>
        No, 

        /// <summary>
        /// The product is shippable (separately or with other products)
        /// </summary>
        Yes, 
        
        /// <summary>
        /// The product is only shippable separately
        /// </summary>
        Separately
    }
}
