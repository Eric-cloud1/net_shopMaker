using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Products
{
    /// <summary>
    /// Exception class for invalid products
    /// </summary>
    public class InvalidProductException : MakerShop.Exceptions.MakerShopException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public InvalidProductException() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        public InvalidProductException(string message) : base(message) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public InvalidProductException(string message, Exception innerException) : base(message, innerException) { }
    }
}
