

namespace MakerShop.Reporting
{
    using System;
    using MakerShop.Common;

    
    public partial class ChargeBackCountCollection : PersistentCollection<ChargeBackCount>
    {
        public int IndexOf(Int32 pPaymentGatewayId, DateTime pChargeBackDate)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pPaymentGatewayId == this[i].PaymentGatewayId) && (pChargeBackDate == this[i].ChargeBackDate)) return i;
            }
            return -1;
        }
    }
}

