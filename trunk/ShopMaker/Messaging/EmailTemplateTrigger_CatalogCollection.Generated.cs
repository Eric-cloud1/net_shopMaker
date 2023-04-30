namespace MakerShop.Messaging
{
    using System;
    using MakerShop.Common;
    public partial class EmailTemplateTrigger_CatalogCollection : PersistentCollection<EmailTemplateTrigger_Catalog>
    {
        public int IndexOf(Int32 pEmailTemplateId, Int32 pStoreEventId, Byte pCatalogNodeTypeId, Int32 pCatalogNodeId, Int16 pPaymentTypeId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pEmailTemplateId == this[i].EmailTemplateId) && (pStoreEventId == this[i].StoreEventId) && (pCatalogNodeTypeId == this[i].CatalogNodeTypeId) && (pCatalogNodeId == this[i].CatalogNodeId) && (pPaymentTypeId == this[i].PaymentTypeId)) return i;
            }
            return -1;
        }
    }
}

