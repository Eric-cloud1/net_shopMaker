using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.DataClient.Api.Schema;

namespace MakerShop.DataClient.Api.ObjectHandlers
{
    class StoreHandler
    {
        public static Store ConverToClient(MakerShop.Stores.Store acStore, bool includeNestedObjects)
        {
            DataObject dataObject = new DataObject("Store",typeof(MakerShop.Stores.Store), typeof(Schema.Store));

           Api.Schema.Store objClientApiStore = (Schema.Store)dataObject.ConvertToClientApiObject(acStore);

           if (includeNestedObjects)
           {
              
              
           }

            return objClientApiStore;  
            //return new Store();
        }

        public static Schema.Store ConverToClient(int storeId, bool includeNestedObjects)
        {
            MakerShop.Stores.Store objStore = new MakerShop.Stores.Store();
            objStore.Load(storeId);
            return (Schema.Store)ConverToClient(objStore, includeNestedObjects);
        }
    }
}
