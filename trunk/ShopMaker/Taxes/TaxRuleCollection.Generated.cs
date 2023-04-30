namespace MakerShop.Taxes
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of TaxRule objects.
    /// </summary>
    public partial class TaxRuleCollection : PersistentCollection<TaxRule>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="taxRuleId">Value of TaxRuleId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 taxRuleId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (taxRuleId == this[i].TaxRuleId) return i;
            }
            return -1;
        }
    }
}
