namespace MakerShop.Orders
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of OrderStatus objects.
    /// </summary>
    public partial class OrderStatusCollection : PersistentCollection<OrderStatus>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="orderStatusId">Value of OrderStatusId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 orderStatusId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (orderStatusId == this[i].OrderStatusId) return i;
            }
            return -1;
        }
    }
}
