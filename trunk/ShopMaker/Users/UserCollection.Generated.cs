namespace MakerShop.Users
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of User objects.
    /// </summary>
    public partial class UserCollection : PersistentCollection<User>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="userId">Value of UserId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 userId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (userId == this[i].UserId) return i;
            }
            return -1;
        }
    }
}
