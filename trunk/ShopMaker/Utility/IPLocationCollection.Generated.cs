namespace MakerShop.Utility
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of IPLocation objects.
    /// </summary>
    public partial class IPLocationCollection : PersistentCollection<IPLocation>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="iPRangeStart">Value of IPRangeStart of the required object.</param>
        /// <param name="iPRangeEnd">Value of IPRangeEnd of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int64 iPRangeStart, Int64 iPRangeEnd)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((iPRangeStart == this[i].IPRangeStart) && (iPRangeEnd == this[i].IPRangeEnd)) return i;
            }
            return -1;
        }
    }
}
