namespace MakerShop.Payments
{
    using System;
    using MakerShop.Common;
    public partial class PaymentGatewayTemplateCollection : PersistentCollection<PaymentGatewayTemplate>
    {
        public int IndexOf(Int32 pPaymentGatewayTemplateId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pPaymentGatewayTemplateId == this[i].PaymentGatewayTemplateId)) return i;
            }
            return -1;
        }
    }
}

