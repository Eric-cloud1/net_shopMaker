namespace MakerShop.Orders
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of OrderStatusEmail objects.
    /// </summary>
    public partial class OrderStatusEmailCollection : PersistentCollection<OrderStatusEmail>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="orderStatusId">Value of OrderStatusId of the required object.</param>
        /// <param name="emailTemplateId">Value of EmailTemplateId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 orderStatusId, Int32 emailTemplateId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((orderStatusId == this[i].OrderStatusId) && (emailTemplateId == this[i].EmailTemplateId)) return i;
            }
            return -1;
        }
    }
}
