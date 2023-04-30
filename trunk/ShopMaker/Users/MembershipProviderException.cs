using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Users
{
    /// <summary>
    /// Exception class related to Membership Provider errors
    /// </summary>
    public class MembershipProviderException : MakerShop.Exceptions.MakerShopException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        public MembershipProviderException(string message) : base(message) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public MembershipProviderException(string message, Exception innerException) : base(message, innerException) { }
    }
}
