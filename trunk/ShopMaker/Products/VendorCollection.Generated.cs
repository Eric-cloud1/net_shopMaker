namespace MakerShop.Products
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Vendor objects.
    /// </summary>
    public partial class VendorCollection : PersistentCollection<Vendor>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="vendorId">Value of VendorId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 vendorId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (vendorId == this[i].VendorId) return i;
            }
            return -1;
        }
    }
}
