using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Products
{
    /// <summary>
    /// Invalid options exception
    /// </summary>
    public class InvalidOptionsException : MakerShop.Exceptions.MakerShopException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public InvalidOptionsException() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        public InvalidOptionsException(string message) : base(message) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public InvalidOptionsException(string message, Exception innerException) : base(message, innerException) { }
    }
}