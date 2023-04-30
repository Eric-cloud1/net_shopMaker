namespace MakerShop.Orders
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Subscription objects.
    /// </summary>
    public partial class OrderSubscriptionsCollection : PersistentCollection<OrderSubscriptions>
    {
        public int IndexOf(Int32 pOrderId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pOrderId == this[i].OrderId)) return i;
            }
            return -1;
        }
    }
}

