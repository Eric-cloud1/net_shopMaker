using System;
using MakerShop.Common;

namespace MakerShop.Affiliates
{
    public partial class AffiliateCode_AffiliateIdCollection : PersistentCollection<AffiliateCode_AffiliateId>
    {
        public int IndexOf(String pAffiliateCode)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pAffiliateCode == this[i].AffiliateCode)) return i;
            }
            return -1;
        }
    }
}
