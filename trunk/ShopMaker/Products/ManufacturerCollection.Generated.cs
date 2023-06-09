namespace MakerShop.Products
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Manufacturer objects.
    /// </summary>
    public partial class ManufacturerCollection : PersistentCollection<Manufacturer>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="manufacturerId">Value of ManufacturerId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 manufacturerId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (manufacturerId == this[i].ManufacturerId) return i;
            }
            return -1;
        }
    }
}
