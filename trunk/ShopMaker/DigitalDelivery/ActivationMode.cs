using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.DigitalDelivery
{
    /// <summary>
    /// Enumeration that represents the activation mode of a digital good.
    /// </summary>
    public enum ActivationMode
    {
        /// <summary>
        /// Manual activation
        /// </summary>
        Manual = 0, 
        
        /// <summary>
        /// Activation on placement of order
        /// </summary>
        OnOrder, 
        
        /// <summary>
        /// Activation on payment of order
        /// </summary>
        OnPaidOrder
    }
}
