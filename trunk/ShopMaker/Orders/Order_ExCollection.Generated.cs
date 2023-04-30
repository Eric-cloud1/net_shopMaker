using System;
using MakerShop.Common;
namespace MakerShop.Orders
{
    public partial class Order_ExCollection : PersistentCollection<Order_Ex>
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
