namespace MakerShop.Messaging
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of EmailTemplate objects.
    /// </summary>
    public partial class EmailTemplateCollection : PersistentCollection<EmailTemplate>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="emailTemplateId">Value of EmailTemplateId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 emailTemplateId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (emailTemplateId == this[i].EmailTemplateId) return i;
            }
            return -1;
        }
    }
}
