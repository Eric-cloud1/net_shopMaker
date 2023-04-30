namespace MakerShop.Catalog
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Category objects.
    /// </summary>
    public partial class CategoryCollection : PersistentCollection<Category>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="categoryId">Value of CategoryId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 categoryId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (categoryId == this[i].CategoryId) return i;
            }
            return -1;
        }
    }
}
