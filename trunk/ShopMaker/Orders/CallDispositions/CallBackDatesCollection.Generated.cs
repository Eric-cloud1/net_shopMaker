using System;
using MakerShop.Common;

namespace MakerShop.Orders
{
    public partial class CallBackDatesCollection : PersistentCollection<CallBackDates>
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
