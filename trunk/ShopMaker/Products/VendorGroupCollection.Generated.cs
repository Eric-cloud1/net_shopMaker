namespace MakerShop.Products
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of VendorGroup objects.
    /// </summary>
    public partial class VendorGroupCollection : PersistentCollection<VendorGroup>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="vendorId">Value of VendorId of the required object.</param>
        /// <param name="groupId">Value of GroupId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 vendorId, Int32 groupId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((vendorId == this[i].VendorId) && (groupId == this[i].GroupId)) return i;
            }
            return -1;
        }
    }
}
