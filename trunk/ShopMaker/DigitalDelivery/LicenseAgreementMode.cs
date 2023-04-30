using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.DigitalDelivery
{
    /// <summary>
    /// Mode of license agreement display
    /// </summary>
    [Flags]
    public enum LicenseAgreementMode
    {
        /// <summary>
        /// Never display the license agreement
        /// </summary>
        Never = 0, 

        /// <summary>
        /// Display license agreement on add to basket
        /// </summary>
        OnAddToBasket = 1, 
        
        /// <summary>
        /// Display license agreement on download
        /// </summary>
        OnDownload = 2, 

        /// <summary>
        /// Always display license agreement (Displays on both add to basket and download)
        /// </summary>
        Always  = OnAddToBasket | OnDownload
    }
}
