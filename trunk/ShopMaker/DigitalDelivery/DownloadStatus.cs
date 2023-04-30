using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.DigitalDelivery
{
    /// <summary>
    /// Enumeration that represents the download status of a digital good.
    /// </summary>
    public enum DownloadStatus
    {
        /// <summary>
        /// The status is unknown
        /// </summary>
        Unknown, 
        
        /// <summary>
        /// The status is pending. Activation or fulfillment is required.
        /// </summary>
        Pending, 
        
        /// <summary>
        /// The status is valid.
        /// </summary>
        Valid, 
        
        /// <summary>
        /// The download has expired.
        /// </summary>
        Expired, 
        
        /// <summary>
        /// The download has depleted.
        /// </summary>
        Depleted
    }
}
