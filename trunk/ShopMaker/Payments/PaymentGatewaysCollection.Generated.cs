
using System;
using MakerShop.Common;

namespace MakerShop.Payments
{
    public partial class PaymentGatewaysCollection : PersistentCollection<PaymentGateways>
    {
        public int IndexOf(Int32 pPaymentGatewayId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pPaymentGatewayId == this[i].PaymentGatewayId)) return i;
            }
            return -1;
        }
    }
}
