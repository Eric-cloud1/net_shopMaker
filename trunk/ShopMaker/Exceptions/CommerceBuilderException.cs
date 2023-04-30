using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Exceptions
{
    /// <summary>
    /// General MakerShop exception
    /// </summary>
    public class MakerShopException : System.ApplicationException
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public MakerShopException() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception message</param>
        public MakerShopException(string message) : base(message) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">The inner exception</param>
        public MakerShopException(string message, Exception innerException) : base(message, innerException) { }
    }
}
