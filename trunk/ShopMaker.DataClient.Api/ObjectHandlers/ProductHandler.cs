using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Products;
using MakerShop.DataClient.Api;
using MakerShop.Utility;
using System.Data;
using System.Data.Common;
using MakerShop.Data;
using MakerShop.Marketing;
using MakerShop.Catalog;
using MakerShop.DataClient.Api.Schema;
using System.Collections;
using MakerShop.Common;


namespace MakerShop.DataClient.Api.ObjectHandlers
{
    class ProductHandler
    {
        public static List<String> GetIdListForProductCriteria(Api.Schema.ProductCriteria criteria)
        {
            return GetIdListForProductCriteria(criteria, false);
        }

        public static List<String> GetIdListForProductCriteria(Api.Schema.ProductCriteria criteria, bool checkOptions)
        {
            List<String> idList = new List<string>();
            using (IDataReader dr = GetDataReader(criteria, true, checkOptions))
            {
                while (dr.Read())
                {                   
                   idList.Add(dr["ProductId"].ToString());                   
                }
                dr.Close();
            }
            return idList;
        }

        public static ProductCollection GetCustomizedCollection(Api.Schema.ProductCriteria criteria)
        {
            return GetCustomizedCollection(criteria, false);
        }

        public static ProductCollection GetCustomizedCollection(Api.Schema.ProductCriteria criteria, bool checkOptions)
        {
            ProductCollection objProductCollection = new ProductCollection();
            using (IDataReader dr = GetDataReader( criteria, false, checkOptions))
            {
                while (dr.Read())
                {
                    MakerShop.Products.Product product = new MakerShop.Products.Product();
                    MakerShop.Products.Product.LoadDataReader(product, dr);
                    objProductCollection.Add(product);
                }
                dr.Close();
            }
            return objProductCollection;
        }

        /// <summary>
        /// DataReader to get products or Id's
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="onlyIds"></param>
        /// <returns></returns>
        private static IDataReader GetDataReader(ProductCriteria criteria, bool onlyIds)
        {
            return GetDataReader(criteria, onlyIds, false);
        }

        /// <summary>
        /// DataReader to get products or Id's
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="onlyIds"></param>
        /// <param name="checkOptions">if true will return only those products haveing some options defined</param>
        /// <returns></returns>
        private static IDataReader GetDataReader(ProductCriteria criteria, bool onlyIds, bool checkOptions)
        {
            SortedDictionary<string, ArrayList> parameters = new SortedDictionary<string, ArrayList>();
            int CategoryId = criteria.Category.CategoryId;
            
            //the follwoing case will never occure because in int case by default we will have 0 or categoryId
            if (CategoryId < 0)
            {
                //set it the root category
                CategoryId = 0;
            }
            bool Recursive=criteria.Category.IncludeSubCategory;

            StringBuilder strQuery = new StringBuilder();

            String strKeyword = String.Empty;
            if (criteria.Name != null && !String.IsNullOrEmpty(criteria.Name.Value))
            {
                strQuery.Append(translateStrComparisonOperator("P.Name", criteria.Name.Option, criteria.Name.Value,parameters,"name"));
            }

            if (criteria.Description != null && !String.IsNullOrEmpty(criteria.Description.Value))
            {
                strQuery.Append(translateStrComparisonOperator("P.Description", criteria.Description.Option, criteria.Description.Value,parameters,"description"));
            }
            if (criteria.Price != null && !String.IsNullOrEmpty(criteria.Price.Value))
            {
                decimal price = AlwaysConvert.ToDecimal(criteria.Price.Value,0);
                strQuery.Append("AND (");
                strQuery.Append("P.Price " + translateOperator(criteria.Price.Option) + " @price");
                KeyValuePair<string, decimal> parameter = new KeyValuePair<string, decimal>("@price", price);
                ArrayList tempList = new ArrayList();
                tempList.Add(parameter);
                parameters.Add("Price",tempList);
                strQuery.Append(")");
            }
            
            if (criteria.SKU != null && !String.IsNullOrEmpty(criteria.SKU.Value))
            {
                strQuery.Append(translateStrComparisonOperator("P.Sku", criteria.SKU.Option, criteria.SKU.Value,parameters,"sku"));
            }

            int VendorId = criteria.Vendor.VendorId;
            if (VendorId > 0)
            {
                strQuery.Append(" AND (P.VendorId =@VendorId)");
            }

            int ManufacturerId = criteria.Manufacturer.ManufacturerId;
            if (ManufacturerId > 0)
            {
                strQuery.Append(" AND (P.ManufacturerId =@ManufacturerId)");
            }

            int ShippableId = criteria.ShippableStatus.Value;
            if (ShippableId > -1)
            {
                strQuery.Append(" AND (P.ShippableId =@ShippableId)");
            }

            int WarehouseId = criteria.Warehouse.WarehouseId;

            if (WarehouseId > 0)
            {
                strQuery.Append(" AND (P.WarehouseId =@WarehouseId)");
            }

            int TaxCodeId = criteria.TaxCode.TaxCodeId;

            if (TaxCodeId > 0)
            {
                strQuery.Append(" AND (P.TaxCodeId =@TaxCodeId)");
            }

            // INCLUDE: VisibilityId, Public private, and hidden

            if (criteria.Include.Hidden || criteria.Include.Public || criteria.Include.Private)
            {
                strQuery.Append(" AND (");

                bool appendOR = false;
                if (criteria.Include.Public){
                    if (appendOR) strQuery.Append(" OR");
                    appendOR = true;
                    strQuery.Append(" P.VisibilityId = 0");
                }

                if (criteria.Include.Hidden)
                {
                    if (appendOR) strQuery.Append(" OR");
                    appendOR = true;
                    strQuery.Append(" P.VisibilityId = 1");
                }

                if (criteria.Include.Private)
                {
                    if (appendOR) strQuery.Append(" OR");
                    appendOR = true;
                    strQuery.Append(" P.VisibilityId = 2");
                }

                strQuery.Append(" )");
            }
            


            if (criteria.SortBy.Option.Equals("By Name"))
            {
                //Name
                if (criteria.SortBy.Value.Equals("Ascending"))
                {
                    strQuery.Append(" ORDER BY P.Name ASC ");
                }
                else if (criteria.SortBy.Value.Equals("Descending"))
                {
                    strQuery.Append(" ORDER BY P.Name DESC ");
                }
                else
                {
                    strQuery.Append( " ORDER BY P.Name ");
                }
            }
            else if (criteria.SortBy.Option.Equals("By Price"))
            {
                //Price
                if (criteria.SortBy.Value.Equals("Ascending"))
                {
                    strQuery.Append(" ORDER BY P.Price ASC ");
                }
                else if (criteria.SortBy.Value.Equals("Descending"))
                {
                    strQuery.Append(" ORDER BY P.Price DESC ");
                }
                else
                {
                    strQuery.Append(" ORDER BY P.Price ");
                }
            }
            else if (criteria.SortBy.Option.Equals("By SKU"))
            {
                //SKU
                if (criteria.SortBy.Value.Equals("Ascending"))
                {
                    strQuery.Append(" ORDER BY P.Sku ASC ");
                }
                else if (criteria.SortBy.Value.Equals("Descending"))
                {
                    strQuery.Append( " ORDER BY P.Sku DESC ");
                }
                else
                {
                    strQuery.Append(" ORDER BY P.Sku ");
                }
            }
            //int ResultLimit=criteria.ResultLimit.Value;
            String CustomizedQuery = strQuery.ToString();
            //Query Build now Execute 


            //Include sub-categories or not
            string categoryFilterSql;
            if (Recursive)
            {
                //this sql gets any category where the specified category is in the parent tree
                categoryFilterSql = "node.CategoryId IN (SELECT CategoryId FROM ac_CategoryParents WHERE ParentId = @categoryId)";
            }
            else
            {
                categoryFilterSql = "node.CategoryId = @categoryId";
            }            

            //GENERATE QUERY
            StringBuilder selectQuery = new StringBuilder();

            selectQuery.Append("SELECT DISTINCT");
            if (onlyIds)
            {
                selectQuery.Append(" ProductId ");
            }
            else
            {
                selectQuery.Append(" " + MakerShop.Products.Product.GetColumnNames(string.Empty) + ", node.CatalogNodeId ");
            }
            selectQuery.Append(" FROM ac_CatalogNodes node, ac_Products P ");
            selectQuery.Append(" WHERE StoreId = @storeId ");
            selectQuery.Append(" AND node.CatalogNodeId = P.ProductId AND " + categoryFilterSql);
            if (checkOptions) selectQuery.Append(" AND P.ProductId IN (SELECT ProductId FROM ac_ProductOptions)");
            
            if (!string.IsNullOrEmpty(CustomizedQuery))
            {
                selectQuery.Append(CustomizedQuery);
            }

            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@categoryId", System.Data.DbType.Int32, CategoryId);
            if (VendorId > 0) database.AddInParameter(selectCommand, "@VendorId", System.Data.DbType.Int32, VendorId);
            if (ManufacturerId > 0) database.AddInParameter(selectCommand, "@ManufacturerId", System.Data.DbType.Int32, ManufacturerId);
            if (WarehouseId > 0) database.AddInParameter(selectCommand, "@WarehouseId", System.Data.DbType.Int32, WarehouseId);
            if (TaxCodeId > 0) database.AddInParameter(selectCommand, "@TaxCodeId", System.Data.DbType.Int32, TaxCodeId);
            if (ShippableId > -1) database.AddInParameter(selectCommand, "@ShippableId", System.Data.DbType.Int32, ShippableId);
            database.AddInParameter(selectCommand, "@true", System.Data.DbType.Boolean, true);
            database.AddInParameter(selectCommand, "@false", System.Data.DbType.Boolean, false);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);

            foreach (string key in parameters.Keys)
            {
                switch (key)
                {
                    case "name":
                        foreach (KeyValuePair<string, string> parameter in parameters[key])
                        {
                            database.AddInParameter(selectCommand, parameter.Key, System.Data.DbType.String, parameter.Value);
                        }
                        break;

                    case "description":
                        foreach (KeyValuePair<string, string> parameter in parameters[key])
                        {
                            database.AddInParameter(selectCommand, parameter.Key, System.Data.DbType.String, parameter.Value);
                        }
                        break;

                    case "Price":
                        foreach (KeyValuePair<string, decimal> parameter in parameters[key])
                        {
                            database.AddInParameter(selectCommand, parameter.Key, System.Data.DbType.String, parameter.Value);
                        }
                        break;

                    case "sku":
                        foreach (KeyValuePair<string, string> parameter in parameters[key])
                        {
                            database.AddInParameter(selectCommand, parameter.Key, System.Data.DbType.String, parameter.Value);
                        }
                        break;

                }
            }
            
            //EXECUTE THE COMMAND
            IDataReader dr = database.ExecuteReader(selectCommand);
            return dr;
        }

        /// <summary>
        /// Convert AC6 ProductCollection to Api.Schema.Product[], and populates nested details
        /// </summary>
        /// <param name="objProductCollection"></param>
        /// <returns>Api.Schema.Product[]</returns>
        public static Api.Schema.Product[] ConvertToClientArray(ProductCollection objProductCollection, bool onlyCsvData)
        {
            string allcats = string.Empty;
            DataObject dataObject = new DataObject("Product", typeof(MakerShop.Products.Product), typeof(Api.Schema.Product));

            Api.Schema.Product[] arrClientApiProduct = new Api.Schema.Product[objProductCollection.Count];
            Api.Schema.Product objClientApiProduct = null;
            String errorMessage = String.Empty;
            List<String> errors = new List<string>();
            for (int i = 0; i < objProductCollection.Count;i++ )
            {
                MakerShop.Products.Product objProduct = objProductCollection[i];
                
                errorMessage = String.Empty;                
                objClientApiProduct = (Api.Schema.Product)dataObject.ConvertToClientApiObject(objProduct, out errorMessage);                
                if (!string.IsNullOrEmpty(errorMessage) && !errors.Contains(errorMessage))
                {
                    errors.Add(errorMessage);
                    // LOG THIS ERROR AS WELL
                    Logger.Error(errorMessage);
                }

                //PPRODUCT CATEGPROES XML
                /*
                 *<categories>
                    <category path=�Books/Fiction/Mystery� orderBy=�2�>
                  </categories> 
                 */

                //ProductCategoryCollection objProductCategoryCollection = objProduct.Categories;
                //string strCategory = string.Empty;
                //foreach (int id in objProductCategoryCollection)
                //{
                //    strCategory += id.ToString() + ",";
                //}
                //objClientApiProduct.Categories = Utills.RemoveEndSeparator(strCategory);
                ProductCategory[] arrProductCategory = new ProductCategory[objProduct.Categories.Count];
                ProductCategory objProductCategory = null;
                String ProdCategories = string.Empty;
                for (int j = 0; j < objProduct.Categories.Count; j++)
                {
                    int Id = objProduct.Categories[j];
                    objProductCategory = new ProductCategory();
                    List<CatalogPathNode> path = CatalogDataSource.GetPath(Id, false);
                    int orderBy = 0;
                    ProdCategories = string.Empty;
                    foreach (CatalogPathNode objNode in path)
                    {
                        ProdCategories += objNode.Name + ":";
                        orderBy = objNode.OrderBy;
                    }
                    ProdCategories = Utility.RemoveEndSeparator(ProdCategories, ":");
                    objProductCategory.CategoryId = Id;
                    objProductCategory.OrderBy = orderBy;
                    objProductCategory.Path = ProdCategories;

                    arrProductCategory[j] = objProductCategory;
                }
                objClientApiProduct.Categories = arrProductCategory;

                // if CSV only data need to embeded
                if (onlyCsvData)
                {
                    if (objProduct.Manufacturer != null)
                    {
                        objClientApiProduct.Manufacturer = objProduct.Manufacturer.Name;
                    }

                    objClientApiProduct.Shippable = objProduct.Shippable.ToString();

                    if (objProduct.TaxCode != null)
                    {
                        objClientApiProduct.TaxCode = objProduct.TaxCode.Name;
                    }
                    if (objProduct.Vendor != null)
                    {
                        objClientApiProduct.Vendor = objProduct.Vendor.Name;
                    }
                    // FROM 7.4 WE SUPPORT MULTIPLE PRODUCT TEMPLATES, "ProductTemplate" field will not contain semi colon delimited list of all templates
                    //if (objProduct.ProductTemplate != null)
                    //{
                    //    objClientApiProduct.ProductTemplate = objProduct.ProductTemplate.Name;
                    //}
                    if (objProduct.ProductProductTemplates.Count > 0)
                    {
                        ProductTemplateCollection templates = ProductTemplateDataSource.LoadForProduct(objProduct.ProductId);
                        String[] templateNames = new string[templates.Count];
                        for (int j = 0; j < templates.Count; j++)
                        {
                            templateNames[j] = templates[j].Name;
                        }
                        objClientApiProduct.ProductTemplate = String.Join(";", templateNames);
                    }

                    if (objProduct.WrapGroup != null)
                    {
                        objClientApiProduct.WrapGroup = objProduct.WrapGroup.Name;
                    }
                    if (objProduct.Warehouse != null)
                    {
                        objClientApiProduct.Warehouse = objProduct.Warehouse.Name;
                    }
                }
                else
                {

                    DataObject nestedDataObject = null;
                    // Attributes      
                    /* SC:
                    DataObject nestedDataObject = new DataObject("ProductAttribute", typeof(MakerShop.Products.ProductAttribute), typeof(Api.Schema.ProductAttribute));
                    //objClientApiProduct.Attributes = (Api.Schema.Attribute[])nestedDataObject.ConvertAC6Collection(objProduct.Attributes); ;
                    Api.Schema.ProductAttribute[] arrProductAttribute = new MakerShop.DataClient.Api.Schema.ProductAttribute[objProduct.Attributes.Count];
                    Api.Schema.ProductAttribute objClientApiProductAttribute = null;
                    for (int j = 0; j < objProduct.Attributes.Count; j++)
                    {
                        MakerShop.Products.ProductAttribute objProductAttribute = objProduct.Attributes[j];
                        objClientApiProductAttribute = (Api.Schema.ProductAttribute)nestedDataObject.ConvertToClientApiObject(objProductAttribute);

                        DataObject innerDataObject = new DataObject("AttributeOption", typeof(MakerShop.Products.AttributeOption), typeof(Api.Schema.AttributeOption));
                        objClientApiProductAttribute.AttributeOptions = (Api.Schema.AttributeOption[])innerDataObject.ConvertAC6Collection(objProductAttribute.AttributeOptions);

                        arrProductAttribute[j] = objClientApiProductAttribute;
                    }
                    objClientApiProduct.Attributes = arrProductAttribute;
                    */

                    nestedDataObject = new DataObject("ProductOption", typeof(MakerShop.Products.ProductOption), typeof(Api.Schema.ProductOption));
                    Api.Schema.ProductOption[] arrProductOptions = new MakerShop.DataClient.Api.Schema.ProductOption[objProduct.ProductOptions.Count];
                    Api.Schema.ProductOption objClientApiProductOption = null;
                    for (int j = 0; j < objProduct.ProductOptions.Count; j++)
                    {
                        MakerShop.Products.ProductOption objProductOption = objProduct.ProductOptions[j];
                        objClientApiProductOption = (Api.Schema.ProductOption)nestedDataObject.ConvertToClientApiObject(objProductOption);
                        arrProductOptions[j] = objClientApiProductOption;
                        if (objProductOption != null)
                        {
                            DataObject optionDataObject = new DataObject("Option", typeof(MakerShop.Products.Option), typeof(Api.Schema.Option));
                            arrProductOptions[j].Option = new MakerShop.DataClient.Api.Schema.Option();
                            arrProductOptions[j].Option = (Api.Schema.Option)optionDataObject.ConvertToClientApiObject(objProductOption.Option);
                            if (objProductOption.Option.Choices != null)
                            {
                                DataObject choicesDataObject = new DataObject("OptionChoice", typeof(MakerShop.Products.OptionChoice), typeof(Api.Schema.OptionChoice));
                                arrProductOptions[j].Option.Choices = new MakerShop.DataClient.Api.Schema.OptionChoice[objProductOption.Option.Choices.Count];
                                OptionChoiceCollection optionChoices = new OptionChoiceCollection();
                                for (int k = 0; k < objProductOption.Option.Choices.Count; k++)
                                {
                                    arrProductOptions[j].Option.Choices[k] = (Api.Schema.OptionChoice)choicesDataObject.ConvertToClientApiObject(objProductOption.Option.Choices[k]);
                                }
                            }
                        }

                    }
                    objClientApiProduct.ProductOptions = arrProductOptions;
                    // Reviews                
                    nestedDataObject = new DataObject("ProductReview", typeof(MakerShop.Products.ProductReview), typeof(Api.Schema.ProductReview));
                    objClientApiProduct.Reviews = (Api.Schema.ProductReview[])nestedDataObject.ConvertAC6Collection(objProduct.Reviews);

                    // Specials
                    nestedDataObject = new DataObject("Special", typeof(MakerShop.Products.Special), typeof(Api.Schema.Special));
                    SpecialCollection objSpecialCollection = objProduct.Specials;
                    Api.Schema.Special[] arrSpecial = new MakerShop.DataClient.Api.Schema.Special[objSpecialCollection.Count];
                    Api.Schema.Special objClientApiSpecial = null;
                    for (int j = 0; j < objSpecialCollection.Count; j++)
                    {
                        MakerShop.Products.Special objSpecial = objSpecialCollection[j];
                        objClientApiSpecial = (Api.Schema.Special)nestedDataObject.ConvertToClientApiObject(objSpecial);

                        //SpecialGroups
                        objClientApiSpecial.SpecialGroups = (Schema.SpecialGroup[])DataObject.ConvertToClientArray(typeof(MakerShop.Products.SpecialGroup), typeof(Schema.SpecialGroup), objSpecial.SpecialGroups);

                        arrSpecial[j] = objClientApiSpecial;
                    }
                    objClientApiProduct.Specials = arrSpecial;


                    //ProductTemplateFields
                    nestedDataObject = new DataObject("ProductTemplateField", typeof(MakerShop.Products.ProductTemplateField), typeof(Api.Schema.ProductTemplateField));
                    objClientApiProduct.TemplateFields = (Api.Schema.ProductTemplateField[])nestedDataObject.ConvertAC6Collection(objProduct.TemplateFields);

                    // CustomField
                    nestedDataObject = new DataObject("ProductCustomField", typeof(MakerShop.Products.ProductCustomField), typeof(Api.Schema.ProductCustomField));
                    objClientApiProduct.CustomFields = (Api.Schema.ProductCustomField[])nestedDataObject.ConvertAC6Collection(objProduct.CustomFields);

                    // Product Varients
                    nestedDataObject = new DataObject("ProductCustomField", typeof(MakerShop.Products.ProductVariant), typeof(Api.Schema.ProductVariant));
                    objClientApiProduct.Variants = (Api.Schema.ProductVariant[])nestedDataObject.ConvertAC6Collection(objProduct.Variants);

                    // ProductVolumeDiscounts
                    String strVolumDiscount = String.Empty;
                    foreach (MakerShop.Marketing.ProductVolumeDiscount pvd in objProduct.ProductVolumeDiscounts)
                    {
                        strVolumDiscount += pvd.VolumeDiscount.Name + ",";
                    }
                    strVolumDiscount = Utility.RemoveEndSeparator(strVolumDiscount, ",");
                    objClientApiProduct.ProductVolumeDiscounts = (Schema.ProductVolumeDiscount[])DataObject.ConvertToClientArray(typeof(MakerShop.Marketing.ProductVolumeDiscount), typeof(Schema.ProductVolumeDiscount), objProduct.ProductVolumeDiscounts);

                    // CouponProduct
                    objClientApiProduct.CouponProduct = DataObject.GetIdsFromCollection(objProduct.CouponProducts, typeof(MakerShop.Marketing.CouponProduct), "CouponId");

                    // Product Assets
                    nestedDataObject = new DataObject("ProductAsset", typeof(MakerShop.Products.ProductAsset), typeof(Api.Schema.ProductAsset));
                    objClientApiProduct.Assets = (Api.Schema.ProductAsset[])nestedDataObject.ConvertAC6Collection(objProduct.Assets);


                    //ProductImages
                    objClientApiProduct.Images = (Schema.ProductImage[])DataObject.ConvertToClientArray(typeof(MakerShop.Products.ProductImage), typeof(Schema.ProductImage), objProduct.Images);

                    //SubscriptionPlan
                    objClientApiProduct.SubscriptionPlan = (Schema.SubscriptionPlan)DataObject.ConvertToClientObject(typeof(MakerShop.Products.SubscriptionPlan), typeof(Schema.SubscriptionPlan), objProduct.SubscriptionPlan);

                    //DigitalGoods
                    objClientApiProduct.DigitalGoods = (Schema.ProductDigitalGood[])DataObject.ConvertToClientArray(typeof(MakerShop.DigitalDelivery.ProductDigitalGood), typeof(Schema.ProductDigitalGood), objProduct.DigitalGoods);

                    //Kit
                    objClientApiProduct.Kit = (Schema.Kit)DataObject.ConvertToClientObject(typeof(MakerShop.Products.Kit), typeof(Schema.Kit), objProduct.Kit);

                    //ProductProductTemplates
                    objClientApiProduct.ProductProductTemplates = (Schema.ProductProductTemplate[])DataObject.ConvertToClientArray(typeof(MakerShop.Products.ProductProductTemplate), typeof(Schema.ProductProductTemplate), objProduct.ProductProductTemplates);

                }
                arrClientApiProduct[i] = objClientApiProduct;
            }
            return arrClientApiProduct;
        }

        /// <summary>
        /// This method wil return a ProductCollection against list of product Ids
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public static ProductCollection GetProductsForIds(String idList)
        {
            ProductCollection productCollection = new ProductCollection();
            MakerShop.Products.Product product = null;
            String[] arrIds = idList.Split(',');
            foreach (String id in arrIds)
            {
                product = new MakerShop.Products.Product();
                int productId = AlwaysConvert.ToInt(id);
                if (product.Load(productId))
                {
                    productCollection.Add(product);
                }
            }
            return productCollection;     
        }

        /// <summary>
        /// This method will return ProductsId of all products in the store
        /// </summary>
        /// <returns></returns>
        internal static List<string> GetIdListForStore()
        {
            return GetIdListForStore(false);
        }
        /// <summary>
        /// This method will return ProductsId of all products in the store
        /// </summary>
        /// <param name="checkOptions">if true will count only products having at least one options</param>
        /// <returns></returns>
        public static List<String> GetIdListForStore(bool checkOptions)
        {
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT ProductId From ac_Products Where StoreId = " + Token.Instance.StoreId);
            if (checkOptions) selectQuery.Append(" AND ProductId IN (SELECT ProductId FROM ac_ProductOptions)");
            List<String> idList = new List<string>();
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    idList.Add(dr[0].ToString());
                }
                dr.Close();
            }
            return idList;
        }

        private static String translateStrComparisonOperator(String columnName, String strOption, String value, SortedDictionary<string, ArrayList>
            parameters,String parameterName)
        {
            StrCompareOption option = (StrCompareOption)Enum.Parse(typeof(StrCompareOption), strOption);
            ArrayList myParameters = new ArrayList();
            KeyValuePair<string, string> parameter;
            switch (option)
            {
                case StrCompareOption.ContainsString:
                    parameter = new KeyValuePair<string, string>("@" + parameterName, "%"+value+"%");
                    myParameters.Add(parameter);
                    parameters.Add(parameterName, myParameters);
                    //return " AND ("+columnName + " LIKE '%" + "@" + parameterName + "%')";
                    return " AND (" + columnName + " LIKE "+"@" + parameterName + ")";

                case StrCompareOption.ContainsWords:
                    String retValue = String.Empty;
                    string[] words = value.Split(' ');
                    int lessCond = 0;
                    // Append Brackets
                    if (words.Length > 0)
                    {
                        retValue += "AND (";
                    }

                    for (int i = 0; i < words.Length; i++)
                    {
                        retValue += columnName + " LIKE @" + parameterName.Trim() + i.ToString();
                        parameter = new KeyValuePair<string, string>("@" + parameterName.Trim() + i.ToString(), "%"+words[i]+"%");
                        myParameters.Add(parameter);
                        lessCond = i;
                        lessCond++;
                        if (lessCond < words.Length)
                        {
                            retValue += " OR ";
                        }
                    }
                    // Close Bracket
                    if (words.Length > 0)
                    {
                        retValue += ")";
                    }
                    parameters.Add(parameterName, myParameters);
                    return retValue;

                case StrCompareOption.Matches:
                    parameter = new KeyValuePair<string, string>("@" + parameterName.Trim(), value);
                    myParameters.Add(parameter);
                    parameters.Add(parameterName, myParameters);
                    return " AND ("+columnName + " ="+"@"+ parameterName.Trim()+")";

                default:
                    return String.Empty;
                    
            }
        }

        private static String translateOperator(String strOption)
        {
            CompareOption option = (CompareOption)Enum.Parse(typeof(CompareOption), strOption);
            switch (option)
            {
                case CompareOption.Equal:
                    return " = ";
                  
                case CompareOption.NotEqual:
                    return " <> ";
                    
                case CompareOption.LessThan:
                    return " < ";
                  
                case CompareOption.LessThanEqualTo:
                    return " <= ";
                 
                case CompareOption.GreatorThan:
                    return " > ";
                  
                case CompareOption.GreatorThanEqualTo:
                    return " >= ";
                    
                case CompareOption.Between:
                    return " BETWEEN ";
                    
                default:
                    return String.Empty;
            }
        }

        internal static MakerShop.DataClient.Api.Schema.UpsellProduct[] GetUpsellProducts()
        {
            UpsellProductCollection UpsellProducts = new UpsellProductCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT!
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + MakerShop.Products.UpsellProduct.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_UpsellProducts");
            //selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" ORDER BY ProductId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    MakerShop.Products.UpsellProduct upsellProduct = new MakerShop.Products.UpsellProduct();
                    MakerShop.Products.UpsellProduct.LoadDataReader(upsellProduct, dr);
                    UpsellProducts.Add(upsellProduct);
                }
                dr.Close();
            }

            return (MakerShop.DataClient.Api.Schema.UpsellProduct[])DataObject.ConvertToClientArray(typeof(MakerShop.Products.UpsellProduct), typeof(Schema.UpsellProduct), UpsellProducts); 
        }

        internal static MakerShop.DataClient.Api.Schema.RelatedProduct[] GetRelatedProducts()
        {
            RelatedProductCollection RelatedProducts = new RelatedProductCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT!
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + MakerShop.Products.RelatedProduct.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_RelatedProducts");
            //selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" ORDER BY ProductId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    MakerShop.Products.RelatedProduct relatedProduct = new MakerShop.Products.RelatedProduct();
                    MakerShop.Products.RelatedProduct.LoadDataReader(relatedProduct, dr);
                    RelatedProducts.Add(relatedProduct);
                }
                dr.Close();
            }

            return (MakerShop.DataClient.Api.Schema.RelatedProduct[])DataObject.ConvertToClientArray(typeof(MakerShop.Products.RelatedProduct), typeof(Schema.RelatedProduct), RelatedProducts); 
        }

        internal static MakerShop.DataClient.Api.Schema.ProductKitComponent[] GetProductKitComponents()
        {

            ProductKitComponentCollection ProductKitComponents = new ProductKitComponentCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT!
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + MakerShop.Products.ProductKitComponent.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_ProductKitComponents");
            //selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" ORDER BY ProductId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    MakerShop.Products.ProductKitComponent productKitComponent = new MakerShop.Products.ProductKitComponent();
                    MakerShop.Products.ProductKitComponent.LoadDataReader(productKitComponent, dr);
                    ProductKitComponents.Add(productKitComponent);
                }
                dr.Close();
            }

            return (MakerShop.DataClient.Api.Schema.ProductKitComponent[])DataObject.ConvertToClientArray(typeof(MakerShop.Products.ProductKitComponent), typeof(Schema.ProductKitComponent), ProductKitComponents); 
        }
        
    }
}
