using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Utility;
using MakerShop.Data;
using MakerShop.Orders;
using MakerShop.Payments;
using MakerShop.Stores;

namespace MakerShop.Payments
{
    public partial class OrderSubscriptionStatusCodesCollection : PersistentCollection<OrderSubscriptionStatusCodes>
    {
        public int IndexOf(Byte pSubscriptionStatusCode)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pSubscriptionStatusCode == this[i].SubscriptionStatusCode)) return i;
            }
            return -1;
        }
    }
}
