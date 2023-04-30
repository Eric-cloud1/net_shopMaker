namespace MakerShop.Payments
{
    using System;
    using MakerShop.Common;


    public partial class PaymentGatewayTemplate_ProductCollection : PersistentCollection<PaymentGatewayTemplate_Product>
    {
        public int IndexOf(Int32 pProductId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pProductId == this[i].ProductId)) return i;
            }
            return -1;
        }
    }
}

