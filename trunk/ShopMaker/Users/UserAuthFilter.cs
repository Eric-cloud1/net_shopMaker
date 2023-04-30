using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Users
{
    /// <summary>
    /// Enum used for filtering user types
    /// </summary>
    public enum UserAuthFilter : int
    {
        /// <summary>
        /// Applicable to no users
        /// </summary>
        None,
 
        /// <summary>
        /// Applicable to anonymous users only
        /// </summary>
        Anonymous, 

        /// <summary>
        /// Applicable to registered users only
        /// </summary>
        Registered, 

        /// <summary>
        /// Applicable to all users
        /// </summary>
        All
    }
}
