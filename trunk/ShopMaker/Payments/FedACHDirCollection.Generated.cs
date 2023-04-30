namespace MakerShop.Payments
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of GiftCertificate objects.
    /// </summary>
    public partial class FedACHDirCollection : PersistentCollection<FedACHDir>
    {
        public int IndexOf(String pRoutingNumber)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pRoutingNumber == this[i].RoutingNumber)) return i;
            }
            return -1;
        }
    }
}

