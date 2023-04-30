using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.DataClient.Api.Schema;
//using MakerShop.Data;
using System.Collections.Specialized;
using MakerShop.Utility;
using MakerShop.Catalog;
using MakerShop.Products;
using MakerShop.Users;
using MakerShop.Marketing;
using MakerShop.Orders;
using MakerShop.DataClient.Api.ObjectHandlers;
using MakerShop.Shipping;
using System.Windows.Forms;
using MakerShop.Stores;
using System.Xml;
using System.Web;
using System.IO;
using System.Collections;
using System.Reflection;
using MakerShop.Taxes;
using MakerShop.Messaging;
using MakerShop.Payments;
using MakerShop.Common;


namespace MakerShop.DataClient.Api.ObjectHandlers
{
    public partial class ImportHandler
    {
        // PRIVATE IMPORT HELPER METHODS FOR PRODUCTS IMPORT        

        #region Products
        
        private void ImportProducts(MakerShop.DataClient.Api.Schema.Product[] arrImportObjects, String csvFields, String matchFields)
        {
            List<String> numaricFields = MakerShop.DataClient.Api.Schema.Product.GetNumaricFields();
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.Product product = (Schema.Product)schemaObject;
                if(String.IsNullOrEmpty(matchFields)) return MakerShop.Products.ProductDataSource.Load(product.ProductId);
                else 
                {
                    String strCriteria = CalculateCriteria(schemaObject, matchFields, numaricFields);
                    ProductCollection products = ProductDataSource.LoadForCriteria(strCriteria);
                    if (products.Count == 0) return null;
                    else if (products.Count == 1) return products[0];
                    else return "-1"; // INIDCATE THAT THERE ARE MORE THEN ONE MATCHING OBJECTS, SO WE CAN NOT UPDATE
                }
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Products.Product acProduct = (Products.Product)acObject;
                Schema.Product schemaProduct = (Schema.Product)schemaObject;

                // TRANSLATE ID's AND ASSOCIATE STORE ID
                acProduct.StoreId = Token.Instance.StoreId; //VERIFY

                // MANUFACTURER
                int newManufacturerId = 0;
                if (schemaProduct.ManufacturerId > 0)
                {
                    newManufacturerId = Translate("Manufacturer", schemaProduct.ManufacturerId);
                 
                }
                if (newManufacturerId == 0)
                {
                    // TRY TO LOAD BY NAME
                    if (!String.IsNullOrEmpty(schemaProduct.Manufacturer)) newManufacturerId = GetManufacturerId(schemaProduct.Manufacturer);
                }
                if (newManufacturerId > 0)
                {
                    acProduct.ManufacturerId = newManufacturerId;
                }

                // TAXCODE
                int newTaxCodeId = 0;
                if (schemaProduct.TaxCodeId > 0) newTaxCodeId = Translate("TaxCode", schemaProduct.TaxCodeId);

                if (newTaxCodeId == 0)
                {
                    // TRY TO LOAD BY NAME
                    if (!String.IsNullOrEmpty(schemaProduct.TaxCode)) newTaxCodeId = GetTaxCodeId(schemaProduct.TaxCode);
                }
                if (newTaxCodeId > 0) acProduct.TaxCodeId = newTaxCodeId;

                // WAREHOUSE
                int newWarehouseId = 0;
                if (schemaProduct.WarehouseId > 0) newWarehouseId = Translate("Warehouse", schemaProduct.WarehouseId);

                if (newWarehouseId == 0)
                {
                    // TRY TO LOAD BY NAME
                    if (!String.IsNullOrEmpty(schemaProduct.Warehouse)) newWarehouseId = GetWarehouseId(schemaProduct.Warehouse);
                }
                if (newWarehouseId > 0) acProduct.WarehouseId = newWarehouseId;

                // VENDOR
                int newVendorId = 0;
                if (schemaProduct.VendorId > 0) newVendorId = Translate("Vendor", schemaProduct.VendorId);

                if (newVendorId == 0)
                {
                    // TRY TO LOAD BY NAME
                    if (!String.IsNullOrEmpty(schemaProduct.Vendor)) newVendorId = GetVendorId(schemaProduct.Vendor);
                }
                if (newVendorId > 0) acProduct.VendorId = newVendorId;

                // PRODUCT TEMPLATE
                // FROM 7.4 WE SUPPORT MULTIPLE PRODUCT TEMPLATES
                // MOVED TO ASSOCIATIONS IMPORT SECTION

                // Shippable
                if (!String.IsNullOrEmpty(schemaProduct.Shippable))
                {
                    acProduct.ShippableId = GetShippingId(schemaProduct.Shippable);
                }

                //WrapGroupId                
                int newWrapGroupId = 0;
                if (schemaProduct.WrapGroupId > 0) newWrapGroupId = Translate("WrapGroup", schemaProduct.WrapGroupId);

                if (newWrapGroupId == 0)
                {
                    // TRY TO LOAD BY NAME
                    if (!String.IsNullOrEmpty(schemaProduct.WrapGroup)) newWrapGroupId = GetWrapGroupId(schemaProduct.WrapGroup);
                }
                if (newWrapGroupId > 0) acProduct.WrapGroupId = newWrapGroupId;                
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                Products.Product acProduct = (Products.Product)acObject;
                Schema.Product schemaProduct = (Schema.Product)schemaObject;

                // ASSOCIATIONS AND CHILDS
                ImportProductAssets(schemaProduct.Assets, acProduct.ProductId);
                ImportProductCustomFields(schemaProduct.CustomFields, acProduct.ProductId);
                ImportProductImages(schemaProduct.Images, acProduct.ProductId);
                ImportProductOptions(schemaProduct.ProductOptions, acProduct.ProductId);
                ImportProductVariants(schemaProduct.Variants, acProduct.ProductId);
                ImportProductReviews(schemaProduct.Reviews, acProduct.ProductId);
                ImportProductTemplateFields(schemaProduct.TemplateFields, acProduct.ProductId);

                ImportSpecials(schemaProduct.Specials, acProduct.ProductId);

                //ImportSubscriptionPlan
                if (schemaProduct.SubscriptionPlan != null)
                {
                    Products.SubscriptionPlan acSubscriptionPlan = null;
                    DataObject dataObject = new DataObject(typeof(Products.SubscriptionPlan), typeof(Schema.SubscriptionPlan));
                    if (acProduct.SubscriptionPlan == null)
                    {
                        
                         acSubscriptionPlan = (Products.SubscriptionPlan)dataObject.ConvertToAC6Object(schemaProduct.SubscriptionPlan);
                    }
                    else
                    {
                        acSubscriptionPlan = (Products.SubscriptionPlan)dataObject.UpdateToAC6Object(schemaProduct.SubscriptionPlan, acProduct.SubscriptionPlan);
                    }
                    acSubscriptionPlan.ProductId = acProduct.ProductId;
                    if (CanTranslate("Group", schemaProduct.SubscriptionPlan.GroupId)) acSubscriptionPlan.GroupId = Translate("Group", schemaProduct.SubscriptionPlan.GroupId);
                    if (CanTranslate("TaxCode", schemaProduct.SubscriptionPlan.TaxCodeId)) acSubscriptionPlan.TaxCodeId = Translate("TaxCode", schemaProduct.SubscriptionPlan.TaxCodeId);
                    
                }

                                
                ImportProductCategories(schemaProduct.Categories, acProduct.Categories, acProduct);


                ImportProductDigitalGoods(schemaProduct.DigitalGoods, acProduct.DigitalGoods, acProduct.ProductId);
                
                                
                ImportDynamicAssociations(schemaProduct.ProductVolumeDiscounts,
                    acProduct.ProductVolumeDiscounts, acProduct.ProductId,
                    typeof(MakerShop.Marketing.ProductVolumeDiscount),
                    "VolumeDiscountId",
                    "VolumeDiscount");

                if (schemaProduct.Kit != null)
                {
                    Products.Kit kit = null;
                    DataObject dataObject = new DataObject(typeof(Products.Kit), typeof(Schema.Kit));
                    if (acProduct.Kit == null)kit = (Products.Kit)dataObject.ConvertToAC6Object(schemaProduct.Kit);
                    else kit = (Products.Kit)dataObject.UpdateToAC6Object(schemaProduct.Kit, acProduct.Kit);
                    kit.ProductId = acProduct.ProductId;
                    kit.Save();
                }                

                // PRODUCT TEMPLATE
                // FROM 7.4 WE SUPPORT MULTIPLE PRODUCT TEMPLATES
                if (!String.IsNullOrEmpty(schemaProduct.ProductTemplate))
                {
                    String[] templateNames = schemaProduct.ProductTemplate.Split(';');
                    ProductProductTemplateCollection supportedTemplates = new ProductProductTemplateCollection();
                    foreach (String templateName in templateNames)
                    {
                        int newProductTemplateId = GetProductTemplateId(templateName);
                        if (acProduct.ProductProductTemplates.IndexOf(acProduct.ProductId, newProductTemplateId) < 0)
                        {
                            // ADD THE ASSOCIATION
                            Products.ProductProductTemplate ppt = new MakerShop.Products.ProductProductTemplate(acProduct.ProductId, newProductTemplateId);
                            ppt.Save();
                            acProduct.ProductProductTemplates.Add(ppt);
                            supportedTemplates.Add(ppt);
                        }
                    }

                    // CLEAN UP OLD ASSOCIATIONS
                    for (int j = acProduct.ProductProductTemplates.Count - 1; j >= 0; j--)
                    {
                        Products.ProductProductTemplate ppt = acProduct.ProductProductTemplates[j];
                        if (supportedTemplates.IndexOf(acProduct.ProductId, ppt.ProductTemplateId) < 0) ppt.Delete();
                        acProduct.ProductProductTemplates.RemoveAt(j);
                    }
                }
                ImportDynamicAssociations(schemaProduct.ProductProductTemplates,
                    acProduct.ProductProductTemplates, acProduct.ProductId,
                    typeof(MakerShop.Products.ProductProductTemplate),
                    "ProductTemplateId",
                    "ProductTemplate");

            };

            ImportObjectsArray(arrImportObjects,typeof(MakerShop.Products.Product),"Product","ProductId","Name",
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations, csvFields);

        }


        #endregion

        private void ImportProductDigitalGoods(ProductDigitalGood[] arrImportObjects, MakerShop.DigitalDelivery.ProductDigitalGoodCollection productDigitalGoodCollection, int parentId)
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.ProductDigitalGood schemaProductDigitalGood = (Schema.ProductDigitalGood)schemaObject;
                int acDigitalGoodId = Translate("DigitalGood",schemaProductDigitalGood.DigitalGoodId);

                MakerShop.DigitalDelivery.ProductDigitalGoodCollection tempCollection = DigitalDelivery.ProductDigitalGoodDataSource.LoadForCriteria(" ProductId = " + parentId + " AND DigitalGoodId = " + acDigitalGoodId);
                return (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;            
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Schema.ProductDigitalGood schemaProductDigitalGood = (Schema.ProductDigitalGood)schemaObject;
                DigitalDelivery.ProductDigitalGood acProductDigitalGood = (DigitalDelivery.ProductDigitalGood)acObject;
                
                acProductDigitalGood.ProductId = (int)parentId;
                acProductDigitalGood.DigitalGoodId = Translate("DigitalGood", schemaProductDigitalGood.DigitalGoodId);

                //acProductDigitalGood.ProductVariantId = Translate("ProductVariant", schemaProductDigitalGood.ProductVariantId);
                //ProductVariant
                if (!String.IsNullOrEmpty(schemaProductDigitalGood.OptionList))
                {
                    int[] schemaOptionChoiceIds = AlwaysConvert.ToIntArray(schemaProductDigitalGood.OptionList);
                    string[] acOptionChoiceIds = new string[schemaOptionChoiceIds.Length];
                    for (int i = 0; i < schemaOptionChoiceIds.Length; i++)
                    {
                        acOptionChoiceIds[i] = Translate("OptionChoice", schemaOptionChoiceIds[i]).ToString();
                    }

                    acProductDigitalGood.OptionList = String.Join(",", acOptionChoiceIds);
                }
                

            };
            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {                
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.DigitalDelivery.ProductDigitalGood), "ProductDigitalGood", "ProductDigitalGoodId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }


        #region ImportProductAssets

        private void ImportProductAssets(MakerShop.DataClient.Api.Schema.ProductAsset[] arrImportObjects, int parentId)
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.ProductAsset ProductAsset = (Schema.ProductAsset)schemaObject;

                MakerShop.Products.ProductAssetCollection tempCollection = MakerShop.Products.ProductAssetDataSource.LoadForCriteria("AssetUrl = '" + SqlEscape(ProductAsset.AssetUrl) + "'");
                return (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
            };
            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Products.ProductAsset acProductAsset = (Products.ProductAsset)acObject;

                acProductAsset.ProductId = (int)parentId;
            };
            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate (ref Object acObject, Object schemaObject){};

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Products.ProductAsset), "ProductAsset", "ProductAssetId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        } 

        #endregion

        #region ImportProductCustomFields    
        
        private void ImportProductCustomFields(MakerShop.DataClient.Api.Schema.ProductCustomField[] arrImportObjects, int parentId)
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.ProductCustomField productCustomField = (Schema.ProductCustomField)schemaObject;
                return MakerShop.Products.ProductCustomFieldDataSource.Load(productCustomField.ProductFieldId);            
            };
            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Products.ProductCustomField acProductCustomField = (Products.ProductCustomField)acObject;
                acProductCustomField.ProductId = (int)parentId;
            };
            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject) { };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Products.ProductCustomField), "ProductCustomField", "ProductFieldId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion        
    
        #region ImportProductImages        

        private void ImportProductImages(MakerShop.DataClient.Api.Schema.ProductImage[] arrImportObjects, int parentId)
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.ProductImage productImage = (Schema.ProductImage)schemaObject;
                String strCriteria = "ImageUrl = '" + SqlEscape(productImage.ImageUrl) + "'";
                if (productImage.ProductImageId > 0) strCriteria += " AND ProductImageId = " + productImage.ProductImageId;
                MakerShop.Products.ProductImageCollection tempCollection = MakerShop.Products.ProductImageDataSource.LoadForCriteria(strCriteria);
                return (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;            
            };

            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Products.ProductImage acProductImage = (Products.ProductImage)acObject;
                acProductImage.ProductId = (int)parentId;
            };
            
            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject) {};

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Products.ProductImage), "ProductImage", "ProductImageId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion     
                
        #region ImportProductOptions

       private void ImportProductOptions(MakerShop.DataClient.Api.Schema.ProductOption[] arrImportObjects, int parentId)
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.ProductOption schemaProductOption = (Schema.ProductOption)schemaObject;

                int acProductId = Translate("Product", schemaProductOption.ProductId);
                int acOptionId = Translate("Option", schemaProductOption.OptionId);
                if (acOptionId != 0)
                {
                    // AS THE OPTIONID IS SUCCESSFULLY TRANSLATED, THIS OPTION IS ALREADY IMPORTED
                    Products.Product acProduct = ProductDataSource.Load(acProductId);
                    if (acProduct != null && acProduct.ProductOptions.IndexOf(acProductId, acOptionId) > -1)
                    {
                        return acProduct.ProductOptions[acProduct.ProductOptions.IndexOf(acProductId, acOptionId)];
                    }
                }

                return null;
            };
            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Products.ProductOption acProductOption = (Products.ProductOption)acObject;
                Schema.ProductOption schemaProductOption = (Schema.ProductOption)schemaObject;
                acProductOption.ProductId = (int)parentId;

                // IMPORT THE CHILD OPTION HERE AS WELL BECAUSE WE HAVE TO UPDATE ITS ASSOCIAION ID LATER ON
                DataObject dataObject = new DataObject(typeof(Products.Option), typeof(Schema.Option));
                Products.Option acOption;
                if (acProductOption.Option != null && acProductOption.Option.Name.Equals(schemaProductOption.Option.Name))
                {
                    // JUST UPDATE                                
                    acOption = (Products.Option)dataObject.UpdateToAC6Object(schemaProductOption.Option, acProductOption.Option);
                    acOption.Save();
                }
                else
                {
                    // NEED TO IMPORT
                    acOption = (Products.Option)dataObject.ConvertToAC6Object(schemaProductOption.Option);
                    acOption.OptionId = 0;
                    acOption.Save();
                }
                acProductOption.OptionId = acOption.OptionId;                
            };
            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                // CHILDS OR ASSOCIATION
                Schema.ProductOption schemaProductOption = (Schema.ProductOption)schemaObject;
                Products.ProductOption acProductOption = (Products.ProductOption)acObject;

                ImportOptionChoices(schemaProductOption.Option.Choices, acProductOption.OptionId);
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Products.ProductOption), "Option", "OptionId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion  
          
        #region ImportOptionChoices        

        private void ImportOptionChoices(MakerShop.DataClient.Api.Schema.OptionChoice[] arrImportObjects, int parentId)
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.OptionChoice schemaOptionChoice = (Schema.OptionChoice)schemaObject;                
                MakerShop.Products.OptionChoiceCollection tempCollection = OptionChoiceDataSource.LoadForCriteria("Name = '" + SqlEscape(schemaOptionChoice.Name) + "' AND OptionId = " + parentId);
                return (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;            
            };
            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Products.OptionChoice acOptionChoice = (Products.OptionChoice)acObject;
                acOptionChoice.OptionId = (int)parentId;
            };
            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {           

            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Products.OptionChoice), "OptionChoice", "OptionChoiceId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion  
    
        #region ImportProductVariants

        private void ImportProductVariants(MakerShop.DataClient.Api.Schema.ProductVariant[] arrImportObjects, int parentId)
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.ProductVariant schemaProductVariant = (Schema.ProductVariant)schemaObject;
                // BUILD THE CRITERIA TO LOAD THE VARIENT
                StringBuilder criteria = new StringBuilder();
                criteria .Append(" ProductId = " + parentId.ToString());
                if (schemaProductVariant.Option1 > 0) { criteria.Append(" AND Option1 = " + Translate("OptionChoice", schemaProductVariant.Option1).ToString()); }
                if (schemaProductVariant.Option2 > 0) { criteria.Append(" AND Option2 = " + Translate("OptionChoice", schemaProductVariant.Option2)); }
                if (schemaProductVariant.Option3 > 0) { criteria.Append(" AND Option3 = " + Translate("OptionChoice", schemaProductVariant.Option3)); }
                if (schemaProductVariant.Option4 > 0) { criteria.Append(" AND Option4 = " + Translate("OptionChoice", schemaProductVariant.Option4)); }
                if (schemaProductVariant.Option5 > 0) { criteria.Append(" AND Option5 = " + Translate("OptionChoice", schemaProductVariant.Option5)); }
                if (schemaProductVariant.Option6 > 0) { criteria.Append(" AND Option6 = " + Translate("OptionChoice", schemaProductVariant.Option6)); }
                if (schemaProductVariant.Option7 > 0) { criteria.Append(" AND Option7 = " + Translate("OptionChoice", schemaProductVariant.Option7)); }
                if (schemaProductVariant.Option8 > 0) { criteria.Append(" AND Option8 = " + Translate("OptionChoice", schemaProductVariant.Option8)); }
                
                
                ProductVariantCollection tempCollection = ProductVariantDataSource.LoadForCriteria(criteria.ToString());
                return (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
            };
            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Products.ProductVariant acProductVariant = (Products.ProductVariant)acObject;
                Schema.ProductVariant schemaProductVariant = (Schema.ProductVariant)schemaObject;
                acProductVariant.ProductId = (int)parentId;

                acProductVariant.Option1 = Translate("OptionChoice", schemaProductVariant.Option1);
                acProductVariant.Option2 = Translate("OptionChoice", schemaProductVariant.Option2);
                acProductVariant.Option3 = Translate("OptionChoice", schemaProductVariant.Option3);
                acProductVariant.Option4 = Translate("OptionChoice", schemaProductVariant.Option4);
                acProductVariant.Option5 = Translate("OptionChoice", schemaProductVariant.Option5);
                acProductVariant.Option6 = Translate("OptionChoice", schemaProductVariant.Option6);
                acProductVariant.Option7 = Translate("OptionChoice", schemaProductVariant.Option7);
                acProductVariant.Option8 = Translate("OptionChoice", schemaProductVariant.Option8);                  
            };
            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                // CHILDS OR ASSOCIATION
                Schema.ProductVariant schemaProductVariant = (Schema.ProductVariant)schemaObject;
                Products.ProductVariant acProductVariant = (Products.ProductVariant)acObject;

            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Products.ProductVariant), "ProductVariant", "ProductVariantId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion

        #region ImportProductReviews       

        private void ImportProductReviews(MakerShop.DataClient.Api.Schema.ProductReview[] arrImportObjects, int parentId)
        {
            TryLoadExistingObject tryLoadExistingObject = delegate(Object schemaObject)
            {
                Schema.ProductReview schemaProductReview = (Schema.ProductReview)schemaObject;

                int acProductId = parentId;

                MakerShop.Products.ProductReviewCollection tempCollection = MakerShop.Products.ProductReviewDataSource.LoadForCriteria("ReviewTitle = '" + SqlEscape(schemaProductReview.ReviewTitle) + "' AND ProductId = " + acProductId.ToString());
                return (tempCollection != null && tempCollection.Count > 0) ? tempCollection[0] : null;
            };
                TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Products.ProductReview acProductReview = (Products.ProductReview)acObject;

                acProductReview.ProductId = (int)parentId;
            };
                ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject)
            {
                // CHILDS OR ASSOCIATION
            };

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Products.ProductReview), "ProductReview", "ProductReviewId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion

        #region ImportProductTemplateFields
        

        private void ImportProductTemplateFields(MakerShop.DataClient.Api.Schema.ProductTemplateField[] arrImportObjects, int parentId)
        {            
            TryLoadExistingObject tryLoadExistingObject =  delegate(Object schemaObject){
            
                Schema.ProductTemplateField schemaProductTemplateField = (Schema.ProductTemplateField)schemaObject;
                return ProductTemplateFieldDataSource.Load(schemaProductTemplateField.ProductTemplateFieldId);
            };
            
            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Products.ProductTemplateField acProductTemplateField = (Products.ProductTemplateField)acObject;
                Schema.ProductTemplateField schemaProductTemplateField = (Schema.ProductTemplateField)schemaObject;

                acProductTemplateField.ProductId = (int)parentId;
                if (CanTranslate("InputField", schemaProductTemplateField.InputFieldId)) acProductTemplateField.InputFieldId = Translate("InputField", schemaProductTemplateField.InputFieldId);
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject) { };            

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Products.ProductTemplateField), "ProductTemplateField", "ProductTemplateFieldId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion
        
        #region ImportSpecials

        private void ImportSpecials(MakerShop.DataClient.Api.Schema.Special[] arrImportObjects, int parentId)
        {            
            TryLoadExistingObject tryLoadExistingObject =  delegate(Object schemaObject){            
                Schema.Special schemaSpecial = (Schema.Special)schemaObject;
                return SpecialDataSource.Load(schemaSpecial.SpecialId);
            };
            
            TranslateObjectAssociatedIds translateObjectAssociatedIds = delegate(ref Object acObject, Object schemaObject)
            {
                Products.Special acSpecial = (Products.Special)acObject;

                acSpecial.ProductId = (int)parentId;                
            };

            ImportObjectChildsAndAssociations importObjectChildsAndAssociations = delegate(ref Object acObject, Object schemaObject) { 
                
                Schema.Special schemaSpecial = (Schema.Special)schemaObject;
                Products.Special acSpecial = (Products.Special)acObject;

                //SpecialGroup
                ImportDynamicAssociations(schemaSpecial.SpecialGroups,
                    acSpecial.SpecialGroups,
                    acSpecial.SpecialId,
                    typeof(MakerShop.Products.SpecialGroup),
                    "GroupId",
                    "Group");

            };            

            ImportChildObjectsArray(arrImportObjects, typeof(MakerShop.Products.Special), "Special", "SpecialId", String.Empty, parentId,
                tryLoadExistingObject, translateObjectAssociatedIds, importObjectChildsAndAssociations);
        }

        #endregion


        #region ImportProductCategories

        private void ImportProductCategories(ProductCategory[] arrImportObjects, ProductCategoryCollection productCategoryCollection, Products.Product objProduct)
        {       

            if (arrImportObjects != null && arrImportObjects.Length > 0)
            {
                // CLEARE ALL EXISTING CATEGORY ASSOCIATIONS
                objProduct.Categories.Clear();
                foreach (ProductCategory prodCat in arrImportObjects)
                {
                    int oldCategoryId = prodCat.CategoryId;
                    int newCategoryId = 0;
                    if (oldCategoryId > 0 && TranslationDic.ContainsKey("CATEGORY" + oldCategoryId))
                    {
                        newCategoryId = TranslationDic["CATEGORY" + oldCategoryId];
                    }

                    if (newCategoryId == 0 && !String.IsNullOrEmpty(prodCat.Path))
                    {
                        newCategoryId = GetCategoryId(prodCat.Path);
                    }

                    if (newCategoryId > 0)
                    {
                        objProduct.Categories.Add(newCategoryId);
                        AddToTranslationDic("CATEGORY" + oldCategoryId, newCategoryId);
                    }
                }

                objProduct.Categories.Save();
            }


        }
        #endregion


        private void ImportProductAssociations(ProductAssociations productAssociations)
        {
            if (productAssociations == null) return;

            log("Importing ProductAssociations");

            if (productAssociations.UpsellProducts != null) ImportUpsellProducts(productAssociations.UpsellProducts);

            if (productAssociations.RelatedProducts != null) ImportProductRelatedProducts(productAssociations.RelatedProducts);

            if (productAssociations.ProductKitComponents != null) ImportProductKitComponents(productAssociations.ProductKitComponents);

            log("ProductAssociations Import Complete");
        }
        


        #region ImportUpsellProducts

        private void ImportUpsellProducts(MakerShop.DataClient.Api.Schema.UpsellProduct[] arrImportObjects)
        {
            
            log("Importing " + arrImportObjects.Length + " UpsellProducts Associations");
            foreach (MakerShop.DataClient.Api.Schema.UpsellProduct schemaUpsellProduct in arrImportObjects)
            {
                try
                {
                    int acProductId = Translate("Product", schemaUpsellProduct.ProductId, 0);
                    int acChildProductId = Translate("Product", schemaUpsellProduct.ChildProductId, 0);
                    if (acProductId != 0 && acChildProductId != 0)
        {
                        // TRY LOAD EXISTING OBJ
                        MakerShop.Products.UpsellProduct acUpsellProduct = UpsellProductDataSource.Load(acProductId, acChildProductId);
                        if (acUpsellProduct == null)
                {
                            acUpsellProduct = new MakerShop.Products.UpsellProduct(acProductId, acChildProductId);
                        }

                        // Update OrderBy
                        acUpsellProduct.OrderBy = schemaUpsellProduct.OrderBy;
                        acUpsellProduct.Save();
                    }
                }
                catch (Exception exception)
                {
                    String strLog = "Association import unsuccessfull for:" + "UpsellProduct" +
                            "\nProductId:" + schemaUpsellProduct.ProductId +
                            "\nChildProductId:" + schemaUpsellProduct.ChildProductId +
                            "\nExcption: " + exception.Message +
                            "\n" + exception.StackTrace;
                    if (exception.GetBaseException() != null)
                        strLog += "\nBaseExcption:" + exception.GetBaseException().Message + "\n" + exception.GetBaseException().StackTrace;

                    logError(strLog);
                }
        }
            log("UpsellProducts Associations Import complete.");            
        }

        #endregion
        
        #region ImportProductKitComponents

        private void ImportProductKitComponents(MakerShop.DataClient.Api.Schema.ProductKitComponent[] arrImportObjects)
        {       
            log("Importing " + arrImportObjects.Length + " ProductKitComponent Associations");
            foreach (MakerShop.DataClient.Api.Schema.ProductKitComponent schemaProductKitComponent in arrImportObjects)
            {
                try
                {
                    int acProductId = Translate("Product", schemaProductKitComponent.ProductId, 0);
                    int acKitComponentId = Translate("KitComponent", schemaProductKitComponent.KitComponentId, 0);
                    if (acProductId != 0 && acKitComponentId != 0)
                    {
                        // TRY LOAD EXISTING OBJ
                        MakerShop.Products.ProductKitComponent acProductKitComponent = ProductKitComponentDataSource.Load(acProductId, acKitComponentId);
                        if (acProductKitComponent == null)
                        {
                            acProductKitComponent = new MakerShop.Products.ProductKitComponent(acProductId, acKitComponentId);
                        }

                        // Update OrderBy
                        acProductKitComponent.OrderBy = (short)schemaProductKitComponent.OrderBy;
                        acProductKitComponent.Save();
                    }
                        }
                catch (Exception exception)
                        {
                    String strLog = "Association import unsuccessfull for:" + "ProductKitComponent" +
                            "\nProductId:" + schemaProductKitComponent.ProductId +
                            "\nKitComponentId:" + schemaProductKitComponent.KitComponentId +
                            "\nExcption: " + exception.Message +
                            "\n" + exception.StackTrace;
                    if (exception.GetBaseException() != null)
                        strLog += "\nBaseExcption:" + exception.GetBaseException().Message + "\n" + exception.GetBaseException().StackTrace;

                    logError(strLog);
                            }

                        }
            log("ProductKitComponent Associations Import complete.");            
                    }

        #endregion

        #region ImportProductRelatedProducts

        private void ImportProductRelatedProducts(MakerShop.DataClient.Api.Schema.RelatedProduct[] arrImportObjects)
        {
            log("Importing " + arrImportObjects.Length + " RelatedProduct Associations");
            foreach (MakerShop.DataClient.Api.Schema.RelatedProduct schemaRelatedProduct in arrImportObjects)
            {
                try
                {
                    int acProductId = Translate("Product", schemaRelatedProduct.ProductId, 0);
                    int acChildProductId = Translate("Product", schemaRelatedProduct.ChildProductId, 0);
                    if (acProductId != 0 && acChildProductId != 0)
                    {
                        // TRY LOAD EXISTING OBJ
                        MakerShop.Products.RelatedProduct acRelatedProduct = RelatedProductDataSource.Load(acProductId, acChildProductId);
                        if (acRelatedProduct == null)
                    {
                            acRelatedProduct = new MakerShop.Products.RelatedProduct(acProductId, acChildProductId);
                        }

                        // Update OrderBy
                        acRelatedProduct.OrderBy = schemaRelatedProduct.OrderBy;
                        acRelatedProduct.Save();
                    }
                }
                catch (Exception exception)
                {
                    String strLog = "Association import unsuccessfull for:" + "RelatedProduct" +
                            "\nProductId:" + schemaRelatedProduct.ProductId +
                            "\nChildProductId:" + schemaRelatedProduct.ChildProductId +
                            "\nExcption: " + exception.Message +
                            "\n" + exception.StackTrace;
                    if (exception.GetBaseException() != null)
                        strLog += "\nBaseExcption:" + exception.GetBaseException().Message + "\n" + exception.GetBaseException().StackTrace;

                    logError(strLog);
            }
            
            }
            log("RelatedProduct Associations Import complete.");            
            
        }
        #endregion        
    }
}
