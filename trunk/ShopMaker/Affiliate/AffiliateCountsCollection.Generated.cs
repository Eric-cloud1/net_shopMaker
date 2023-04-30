using System;
using MakerShop.Common;

namespace MakerShop.Affiliates
{
    public partial class AffiliateCountsCollection : PersistentCollection<AffiliateCounts>
    {
        public int IndexOf(Int32 pAffiliateCountId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pAffiliateCountId == this[i].AffiliateCountId)) return i;
            }
            return -1;
        }
    }
}
