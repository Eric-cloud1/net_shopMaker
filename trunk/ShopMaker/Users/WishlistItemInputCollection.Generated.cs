namespace MakerShop.Users
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of WishlistItemInput objects.
    /// </summary>
    public partial class WishlistItemInputCollection : PersistentCollection<WishlistItemInput>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="wishlistItemInputId">Value of WishlistItemInputId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 wishlistItemInputId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (wishlistItemInputId == this[i].WishlistItemInputId) return i;
            }
            return -1;
        }
    }
}
