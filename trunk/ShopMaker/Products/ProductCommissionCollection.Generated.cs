namespace MakerShop.Products
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of KitProduct objects.
    /// </summary>
    public partial class ProductCommissionCollection : PersistentCollection<ProductCommission>
    {
        public int IndexOf(Int32 pProductId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pProductId == this[i].ProductId)) return i;
            }
            return -1;
        }
    }
}

