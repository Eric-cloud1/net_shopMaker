namespace MakerShop.Products
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of SubscriptionPlanDetails objects.
    /// </summary>
    public partial class SubscriptionPlanDetailsCollection : PersistentCollection<SubscriptionPlanDetails>
    {
        public int IndexOf(Int32 pProductId, Int16 pPaymentTypeId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pProductId == this[i].ProductId) && (pPaymentTypeId == this[i].PaymentTypeId)) return i;
            }
            return -1;
        }
       
    }
}
