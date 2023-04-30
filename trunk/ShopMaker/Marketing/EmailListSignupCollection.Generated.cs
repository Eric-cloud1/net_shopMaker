namespace MakerShop.Marketing
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of EmailListSignup objects.
    /// </summary>
    public partial class EmailListSignupCollection : PersistentCollection<EmailListSignup>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="emailListSignupId">Value of EmailListSignupId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 emailListSignupId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (emailListSignupId == this[i].EmailListSignupId) return i;
            }
            return -1;
        }
    }
}
