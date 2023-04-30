using System;
using MakerShop.Common;

namespace MakerShop.Payments
{
    public partial class PaymentGatewayGroupsCollection : PersistentCollection<PaymentGatewayGroups>
    {
        public int IndexOf(Int32 pPaymentGatewayGroupId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pPaymentGatewayGroupId == this[i].PaymentGatewayGroupId)) return i;
            }
            return -1;
        }
    }
}
