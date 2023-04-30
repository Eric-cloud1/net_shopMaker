using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Marketing;
using MakerShop.Common;
//using MakerShop.Data;

namespace MakerShop.DataClient.Api.ObjectHandlers
{
    class AffiliateHandler
    {
        public static Api.Schema.Affiliate[] ConvertToClientArray(AffiliateCollection objAffiliateCollection)
        {
            DataObject dataObject = new DataObject(typeof(MakerShop.Marketing.Affiliate), typeof(Schema.Affiliate));
            Schema.Affiliate[] arrAffiliates = new MakerShop.DataClient.Api.Schema.Affiliate[objAffiliateCollection.Count];
            Schema.Affiliate objClientApiAffiliate = null;
            for (int i = 0; i < objAffiliateCollection.Count; i++ )
            {
                MakerShop.Marketing.Affiliate objAffiliate = objAffiliateCollection[i];
                objClientApiAffiliate = (Schema.Affiliate)dataObject.ConvertToClientApiObject(objAffiliate);                

                // add RoleAffiliates
                //objClientApiAffiliate.RoleAffiliates = DataObject.GetIdsFromCollection(objAffiliate.RoleAffiliates, typeof(MakerShop.Marketing.RoleAffiliate), "RoleId");

                arrAffiliates[i] = objClientApiAffiliate;
            }
            return arrAffiliates;
        }        
    }
}
