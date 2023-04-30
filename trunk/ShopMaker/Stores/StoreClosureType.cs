using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Stores
{
    /// <summary>
    /// Enum to represent different store closure types
    /// </summary>
    public enum StoreClosureType
    {
        /// <summary>
        /// Store is open for all users
        /// </summary>
        Open = 0,

        /// <summary>
        /// Store is temporarily closed for all users
        /// </summary>
        ClosedForEveryone,

        /// <summary>
        /// Store is temporarily closed for non-admin users
        /// </summary>
        ClosedForNonAdminUsers
    }
}
