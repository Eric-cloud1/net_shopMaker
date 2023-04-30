using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Payments
{
    /// <summary>
    /// Enumeration that represents the status of a gift certificate.
    /// </summary>
    public enum GiftCertificateStatus
    {
        /// <summary>
        /// Gift certificate is active
        /// </summary>
        Active = 0, 
        
        /// <summary>
        /// Gift certificate is inactive
        /// </summary>
        Inactive, 
        
        //TODO ??? what is this
        /// <summary>
        /// Gift certificate status is all
        /// </summary>
        All
    }
}
