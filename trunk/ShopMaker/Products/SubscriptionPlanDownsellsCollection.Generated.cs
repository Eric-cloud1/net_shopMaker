namespace MakerShop.Products
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of SubscriptionPlanDownsells objects.
    /// </summary>
    public partial class SubscriptionPlanDownsellsCollection : PersistentCollection<SubscriptionPlanDownsells>
    {
        public int IndexOf(Int32 pProductId, Int16 pPaymentTypeId, Decimal pChargeAmount)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pProductId == this[i].ProductId) && (pPaymentTypeId == this[i].PaymentTypeId) && (pChargeAmount == this[i].ChargeAmount)) return i;
            }
            return -1;
        }
    }
}