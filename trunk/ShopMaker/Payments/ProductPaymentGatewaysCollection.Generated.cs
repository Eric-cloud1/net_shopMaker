namespace MakerShop.Payments
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a ProductPaymentGatewaysCollection of ProductPaymentGateways objects.
    /// </summary>
    public partial class ProductPaymentGatewaysCollection : PersistentCollection<ProductPaymentGateways>
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
