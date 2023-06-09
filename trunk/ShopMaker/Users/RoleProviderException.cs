using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Users
{
    /// <summary>
    /// Role Provider Exception
    /// </summary>
    public class RoleProviderException : MakerShop.Exceptions.MakerShopException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception message</param>
        public RoleProviderException(string message) : base(message) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public RoleProviderException(string message, Exception innerException) : base(message, innerException) { }
   }
}
