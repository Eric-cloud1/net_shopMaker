namespace MakerShop.Validation
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of ErrorMessage objects.
    /// </summary>
    public partial class BlackListsCollection : PersistentCollection<BlackLists>
    {
        public int IndexOf(Int32 pBlackListId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pBlackListId == this[i].BlackListId)) return i;
            }
            return -1;
        }
    }
}

