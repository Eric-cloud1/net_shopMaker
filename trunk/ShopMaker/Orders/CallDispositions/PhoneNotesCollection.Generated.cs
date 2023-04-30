
using System;
using MakerShop.Common;

namespace MakerShop.Orders
{
    public partial class PhoneNotesCollection : PersistentCollection<PhoneNotes>
    {
        public int IndexOf(Int32 pPhoneNoteId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pPhoneNoteId == this[i].PhoneNoteId)) return i;
            }
            return -1;
        }
    }
}
