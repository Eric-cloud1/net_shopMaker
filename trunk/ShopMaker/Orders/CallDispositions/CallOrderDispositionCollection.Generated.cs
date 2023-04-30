
using System;
using MakerShop.Common;

namespace MakerShop.Orders
{
    public partial class CallOrderDispositionCollection : PersistentCollection<CallOrderDisposition>
    {
        public int IndexOf(Int32 pOrderId, Int32 pCallDispositionId, Int32 pUserId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pOrderId == this[i].OrderId) && (pCallDispositionId == this[i].CallDispositionId) && (pUserId == this[i].UserId)) return i;
            }
            return -1;
        }
    }
}
