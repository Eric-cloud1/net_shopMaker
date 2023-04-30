namespace MakerShop.Payments
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of PaymentGateway objects.
    /// </summary>
    public partial class PaymentGatewayCollection : PersistentCollection<PaymentGateway>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="paymentGatewayId">Value of PaymentGatewayId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 paymentGatewayId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (paymentGatewayId == this[i].PaymentGatewayId) return i;
            }
            return -1;
        }
    }
}
