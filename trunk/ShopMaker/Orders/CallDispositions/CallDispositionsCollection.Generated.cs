
using System;
using MakerShop.Common;
	

namespace MakerShop.Orders
{
    public partial class CallDispositionsCollection : PersistentCollection<CallDispositions>
    {
        public int IndexOf(Int32 pCallDispositionId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pCallDispositionId == this[i].CallDispositionId)) return i;
            }
            return -1;
        }
    }
}
