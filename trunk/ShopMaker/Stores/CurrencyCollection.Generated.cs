namespace MakerShop.Stores
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Currency objects.
    /// </summary>
    public partial class CurrencyCollection : PersistentCollection<Currency>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="currencyId">Value of CurrencyId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 currencyId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (currencyId == this[i].CurrencyId) return i;
            }
            return -1;
        }
    }
}
