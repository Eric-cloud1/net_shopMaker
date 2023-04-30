namespace MakerShop.Products
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of WrapStyle objects.
    /// </summary>
    public partial class WrapStyleCollection : PersistentCollection<WrapStyle>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="wrapStyleId">Value of WrapStyleId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 wrapStyleId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (wrapStyleId == this[i].WrapStyleId) return i;
            }
            return -1;
        }
    }
}
