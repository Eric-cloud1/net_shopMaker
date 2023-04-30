namespace MakerShop.Products
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of SubscriptionPlanDetails objects.
    /// </summary>
    public partial class SubscriptionPlanDetailsCollection : PersistentCollection<SubscriptionPlanDetails>
    {
       
        public SubscriptionPlanDetails Initial
        {
            get
            {
                return this.Find(x=>x.PaymentTypeId == (short)Payments.PaymentTypes.Initial);
            }
        }
        public SubscriptionPlanDetails Recurring
        {
            get
            {
                return this.Find(x => x.PaymentTypeId == (short)Payments.PaymentTypes.Recurring);
            }
        }
        public SubscriptionPlanDetails Trial
        {
            get
            {
                return this.Find(x => x.PaymentTypeId == (short)Payments.PaymentTypes.Trial);
            }
        }
    }
}
