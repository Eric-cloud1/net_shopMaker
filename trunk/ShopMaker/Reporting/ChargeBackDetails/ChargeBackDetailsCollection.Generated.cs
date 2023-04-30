

namespace MakerShop.Orders.ChargeBack
{
    using System;
    using MakerShop.Common;

    public partial class ChargeBackDetailsCollection : PersistentCollection<ChargeBackDetails>
    {
        public int IndexOf(Int32 pOrderId, Int32 pPaymentId, Int32 pTransactionId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pOrderId == this[i].OrderId) && (pPaymentId == this[i].PaymentId) && (pTransactionId == this[i].TransactionId)) return i;
            }
            return -1;
        }
    }
}

