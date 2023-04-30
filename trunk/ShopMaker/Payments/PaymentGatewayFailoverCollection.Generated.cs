namespace MakerShop.Payments
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PaymentGatewayFailoverCollection of PaymentGatewayFailover objects.
    /// </summary>
    public partial class PaymentGatewayFailoverCollection : PersistentCollection<PaymentGatewayFailover>
    {
        public int IndexOf(Int32 pSourcePaymentGatewayId, Int32 pDestinationPaymentGatewayId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pSourcePaymentGatewayId == this[i].SourcePaymentGatewayId) && (pDestinationPaymentGatewayId == this[i].DestinationPaymentGatewayId)) return i;
            }
            return -1;
        }
    }
}

