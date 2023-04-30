using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Orders
{
    /// <summary>
    /// Exception class for basket item parent exceptions
    /// </summary>
    public class BasketItemParentException : MakerShop.Exceptions.MakerShopException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        public BasketItemParentException(string message) : base(message) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public BasketItemParentException(string message, Exception innerException) : base(message, innerException) { }
    }
}
