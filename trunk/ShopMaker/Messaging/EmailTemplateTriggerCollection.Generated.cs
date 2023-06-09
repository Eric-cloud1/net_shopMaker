namespace MakerShop.Messaging
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of EmailTemplateTrigger objects.
    /// </summary>
    public partial class EmailTemplateTriggerCollection : PersistentCollection<EmailTemplateTrigger>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="emailTemplateId">Value of EmailTemplateId of the required object.</param>
        /// <param name="storeEventId">Value of StoreEventId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 emailTemplateId, Int32 storeEventId, Int16 pPaymentTypeId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((emailTemplateId == this[i].EmailTemplateId) && (storeEventId == this[i].StoreEventId)&& (pPaymentTypeId == this[i].PaymentTypeId)) return i;
            }
            return -1;
        }
    }
}
