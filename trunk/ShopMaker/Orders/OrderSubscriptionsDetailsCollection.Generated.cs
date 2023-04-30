namespace MakerShop.Orders
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of OrderStatusEmail objects.
    /// </summary>

    public partial class OrderSubscriptionsDetailsCollection : PersistentCollection<OrderSubscriptionsDetails>
    {
        public int IndexOf(Int32 pOrderId, Int16 pPaymentTypeId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pOrderId == this[i].OrderId) && (pPaymentTypeId == this[i].PaymentTypeId)) return i;
            }
            return -1;
        }
    }

}
