using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Exceptions
{
    /// <summary>
    /// Class that represents transaction exceptions
    /// </summary>
    public class TransactionException : MakerShop.Exceptions.MakerShopException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        public TransactionException(string message) : base(message) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public TransactionException(string message, Exception innerException) : base(message, innerException) { }
    }
}
