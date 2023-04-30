using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Marketing
{
    /// <summary>
    /// Enum that represents the type of coupons
    /// </summary>
    public enum AffiliateType : byte
    {
        /// <summary>
        /// Nothing defined
        /// </summary>
        None = 0, 
        
        /// <summary>
        /// This is the Mater Company, 1st / Top Level
        /// </summary>
        Master_Company = 10, 
        
        /// <summary>
        /// This is the Company, 2nd Level
        /// </summary>
        Company = 20,

        /// <summary>
        /// This is the Master Agent, 3rd Level
        /// </summary>
        Master_Agent = 30,

        /// <summary>
        /// This is the Agent, 4th Level
        /// </summary>
        Agent = 40,
        /// <summary>
        /// This is the Location, 5th / final Level
        /// </summary>
        Location = 50
    }
}
