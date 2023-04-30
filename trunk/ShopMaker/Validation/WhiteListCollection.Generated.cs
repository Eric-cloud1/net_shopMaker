namespace MakerShop.Validation
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of ErrorMessage objects.
    /// </summary>
    public partial class WhiteListsCollection : PersistentCollection<WhiteLists>
    {
        public int IndexOf(Int32 pWhiteListId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pWhiteListId == this[i].WhiteListId)) return i;
            }
            return -1;
        }
    }
}

