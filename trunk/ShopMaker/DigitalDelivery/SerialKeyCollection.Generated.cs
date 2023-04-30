namespace MakerShop.DigitalDelivery
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of SerialKey objects.
    /// </summary>
    public partial class SerialKeyCollection : PersistentCollection<SerialKey>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="serialKeyId">Value of SerialKeyId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 serialKeyId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (serialKeyId == this[i].SerialKeyId) return i;
            }
            return -1;
        }
    }
}
