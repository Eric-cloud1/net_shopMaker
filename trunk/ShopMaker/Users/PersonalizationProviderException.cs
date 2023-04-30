using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Users
{
    /// <summary>
    /// Exception class related to Personalization Provider errors
    /// </summary>
    public class PersonalizationProviderException : MakerShop.Exceptions.MakerShopException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        public PersonalizationProviderException(string message) : base(message) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public PersonalizationProviderException(string message, Exception innerException) : base(message, innerException) { }
    }
}
