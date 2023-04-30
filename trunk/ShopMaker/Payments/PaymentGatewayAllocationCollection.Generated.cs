namespace MakerShop.Payments
{
    using System;
    using MakerShop.Common;
    public partial class PaymentGatewayAllocationCollection : PersistentCollection<PaymentGatewayAllocation>
    {
        public int IndexOf(Int32 pPaymentGatewayTemplateId, Int32 pPaymentMethodId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pPaymentGatewayTemplateId == this[i].PaymentGatewayTemplateId) && (pPaymentMethodId == this[i].PaymentMethodId)) return i;
            }
            return -1;
        }
    }
}

