namespace MakerShop.Products
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of KitProduct objects.
    /// </summary>
    public partial class KitProductCollection : PersistentCollection<KitProduct>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="kitProductId">Value of KitProductId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 kitProductId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (kitProductId == this[i].KitProductId) return i;
            }
            return -1;
        }
    }
}
