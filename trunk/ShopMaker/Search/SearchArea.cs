using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Search
{
    /// <summary>
    /// Enum used to specify the search area for admin searching
    /// </summary>
    public enum SearchArea
    {
        /// <summary>
        /// Search all areas
        /// </summary>
        All,
        /// <summary>
        /// Search categories
        /// </summary>
        Categories,
        /// <summary>
        /// Search products
        /// </summary>
        Products,
        /// <summary>
        /// Search webpages
        /// </summary>
        Webpages,
        /// <summary>
        /// Search links
        /// </summary>
        Links,
        /// <summary>
        /// Search digital goods
        /// </summary>
        DigitalGoods,
        /// <summary>
        /// Search warehouses
        /// </summary>
        Warehouses,
        /// <summary>
        /// Search users
        /// </summary>
        Users
    }
}
