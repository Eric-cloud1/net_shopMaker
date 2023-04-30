namespace MakerShop.Products
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of KitComponent objects.
    /// </summary>
    public partial class KitComponentCollection : PersistentCollection<KitComponent>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="kitComponentId">Value of KitComponentId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 kitComponentId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (kitComponentId == this[i].KitComponentId) return i;
            }
            return -1;
        }
    }
}
