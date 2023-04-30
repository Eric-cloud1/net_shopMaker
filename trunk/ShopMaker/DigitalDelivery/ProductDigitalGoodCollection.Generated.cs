namespace MakerShop.DigitalDelivery
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of ProductDigitalGood objects.
    /// </summary>
    public partial class ProductDigitalGoodCollection : PersistentCollection<ProductDigitalGood>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="productDigitalGoodId">Value of ProductDigitalGoodId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 productDigitalGoodId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (productDigitalGoodId == this[i].ProductDigitalGoodId) return i;
            }
            return -1;
        }
    }
}
