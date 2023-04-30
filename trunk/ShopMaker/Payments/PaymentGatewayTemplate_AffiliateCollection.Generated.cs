namespace MakerShop.Payments
{
    using System;
    using MakerShop.Common;


    public partial class PaymentGatewayTemplate_AffiliateCollection : PersistentCollection<PaymentGatewayTemplate_Affiliate>
    {
        public int IndexOf(Int32 pAffiliateId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pAffiliateId == this[i].AffiliateId)) return i;
            }
            return -1;
        }
    }
}

