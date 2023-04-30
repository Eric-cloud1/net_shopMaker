using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Payments
{
    /// <summary>
    /// Exception class for invalid payment status errors
    /// </summary>
    public class PaymentStatusException : MakerShop.Exceptions.MakerShopException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        public PaymentStatusException(string message) : base(message) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public PaymentStatusException(string message, Exception innerException) : base(message, innerException) { }
    }
}
