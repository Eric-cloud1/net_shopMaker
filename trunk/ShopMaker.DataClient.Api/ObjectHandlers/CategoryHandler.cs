using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Catalog;
using MakerShop.Marketing;
using System.Collections.Specialized;
using MakerShop.Utility;
using MakerShop.Common;
//using MakerShop.Data;

namespace MakerShop.DataClient.Api.ObjectHandlers
{
    class CategoryHandler
    {
        public static Api.Schema.Category[] ConvertToClientArray(CategoryCollection objCategoryCollection)
        {
            DataObject dataObject = new DataObject(typeof(MakerShop.Catalog.Category), typeof(Schema.Category));
            Schema.Category[] arrCats = new Schema.Category[objCategoryCollection.Count];
            Schema.Category objClientApiCategory = null;
            String errorMessage = String.Empty;
            List<String> errors = new List<string>();
            for (int i = 0; i < objCategoryCollection.Count; i++ )
            {
                MakerShop.Catalog.Category objCategory = objCategoryCollection[i];

                errorMessage = String.Empty;
                objClientApiCategory = (Schema.Category)dataObject.ConvertToClientApiObject(objCategory, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage) && !errors.Contains(errorMessage))
                {
                    errors.Add(errorMessage);
                    // LOG THIS ERROR AS WELL
                    Logger.Error(errorMessage);
                }

                //CategoryVolumeDiscounts
                DataObject nestedDataObject = new DataObject(typeof(MakerShop.Marketing.CategoryVolumeDiscount), typeof(Schema.CategoryVolumeDiscount));
                objClientApiCategory.CategoryVolumeDiscounts = (Schema.CategoryVolumeDiscount[])nestedDataObject.ConvertAC6Collection(objCategory.CategoryVolumeDiscounts);

                //CategoryParents
                nestedDataObject = new DataObject(typeof(MakerShop.Catalog.CategoryParent), typeof(Schema.CategoryParent));
                objClientApiCategory.Parents = (Schema.CategoryParent[])nestedDataObject.ConvertAC6Collection(objCategory.Parents);

                //CatalogNodes
                nestedDataObject = new DataObject(typeof(MakerShop.Catalog.CatalogNode), typeof(Schema.CatalogNode));
                objClientApiCategory.CatalogNodes = (Schema.CatalogNode[])nestedDataObject.ConvertAC6Collection(objCategory.CatalogNodes);
                
                arrCats[i] = objClientApiCategory;
            }
            return arrCats;
        }
    }
}
