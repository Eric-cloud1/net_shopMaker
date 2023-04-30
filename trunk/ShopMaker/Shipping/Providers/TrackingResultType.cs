using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Shipping.Providers
{
    /// <summary>
    /// Enum that defines the type of result returned by shipping tracking method of a shipping provider
    /// </summary>
    public enum TrackingResultType
    {
        /// <summary>
        /// The tracking details are included in the response
        /// </summary>
        InlineDetails,

        /// <summary>
        /// The response contains an external link where tracking details can be found
        /// </summary>
        ExternalLink
    }
}
