using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Common;
// AC 7.2 DATABASE IMPLEMENTATION HAS CHANGED
//using Microsoft.Practices.EnterpriseLibrary.Data;
using MakerShop.Data;

using MakerShop.Products;
using MakerShop.Utility;
using System.Data.Common;
using MakerShop.Catalog;
using MakerShop.Marketing;
using MakerShop.Users;
using MakerShop.Orders;
using MakerShop.Stores;
using System.Collections.Specialized;
using System.Data;

namespace MakerShop.DataClient.Api
{
    public class ACDataSource
    {
        public static SaveResult SaveProduct(Product product, PreserveIdOption preserveIdOption, ref String messages)
        {
            if (product.IsDirty)
            {
                Database database = Token.Instance.Database;
                bool recordExists = true;
                
                //SET EMPTY STOREID TO CURRENT CONTEXT
                if (product.StoreId == 0) product.StoreId = Token.Instance.StoreId;
                if (product.ProductId == 0) recordExists = false;

                //SET DEFAULT FOR DATE FIELD
                if (product.CreatedDate == System.DateTime.MinValue) product.CreatedDate = LocaleHelper.LocalNow;
                if (product.LastModifiedDate == System.DateTime.MinValue) product.LastModifiedDate = LocaleHelper.LocalNow;

                if (recordExists) {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_Products");
                    selectQuery.Append(" WHERE ProductId = @productId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@ProductId", System.Data.DbType.Int32, product.ProductId);
                        if ((int)database.ExecuteScalar(selectCommand) == 0)
                        {
                            recordExists = false;
                        }
                    }
                }

                if (recordExists)
                {
                     switch (preserveIdOption)
                    {
                        case PreserveIdOption.OverwriteExistingObject:
                             // DELETE THE EXISTING PRODUCT
                             Product existingProduct = ProductDataSource.Load(product.ProductId);
                             if (existingProduct != null) existingProduct.Delete();

                             // INSERT
                             InsertProductWithID(product, database);

                             if (product.IsDirty) {
                                  messages += "Error while saving/overwriting product. A product with the same ID already exists. Error while saving product.\n Product ID=" + product.ProductId + "\nProduct Name=" + product.Name;
                                  return SaveResult.Failed;
                              }
                              else
                              {
                                  messages += "Found a product with same ID, the existing product is deleted and this one successfully saved with old ID. \n Product ID=" + product.ProductId + "\nProduct Name=" + product.Name;
                                  return SaveResult.RecordInserted;
                              }                        
                             
                         case PreserveIdOption.AddAsNewObject:
                            messages += "A product with the same ID already exists, saving it with new ID.\n Old Product ID=" + product.ProductId + "\nProduct Name=" + product.Name;
                            product.ProductId = 0;
                            return product.Save();
                       
                         case PreserveIdOption.JustLogWarning:
                            messages += "Unable to save product, a product with the same ID already exists.\n Product ID=" + product.ProductId + "\nProduct Name=" + product.Name;
                            break;
                    }
                }else{
                    InsertProductWithID(product,database);
                    return SaveResult.RecordInserted;
                }
            }

            //SAVE IS SUCCESSFUL IF OBJECT IS NOT DIRTY
            return SaveResult.NotDirty;        
        }

        public static SaveResult SaveCategory(Category category, PreserveIdOption preserveIdOption, ref String messages)
        {
            if (category.IsDirty)
            {
                Database database = Token.Instance.Database;
                bool recordExists = true;

                //SET EMPTY STOREID TO CURRENT CONTEXT
                if (category.StoreId == 0) category.StoreId = Token.Instance.StoreId;
                if (category.CategoryId == 0) recordExists = false;                

                if (recordExists) {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_Categories");
                    selectQuery.Append(" WHERE CategoryId = @categoryId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@CategoryId", System.Data.DbType.Int32, category.CategoryId);
                        if ((int)database.ExecuteScalar(selectCommand) == 0)
                        {
                            recordExists = false;
                        }
                    }
                }

                if (recordExists)
                {
                    switch (preserveIdOption)
                    {
                        case PreserveIdOption.OverwriteExistingObject:
                            // DELETE THE EXISTING OBJECT
                            Category existingObject = CategoryDataSource.Load(category.CategoryId);
                            if(existingObject != null) existingObject.Delete();

                            // INSERT
                            InsertCategoryWithID(category, database);

                            if (category.IsDirty)
                            {
                                messages += "Error while saving/overwriting category. A category with the same ID already exists. Error while saving category.\n Category ID=" + category.CategoryId + "\nCategory Name=" + category.Name;
                                return SaveResult.Failed;
                            }
                            else
                            {
                                messages += "Found a Category with same ID, the existing Category is deleted and this one successfully saved with old ID. \n Category ID=" + category.CategoryId + "\nCategory Name=" + category.Name;
                                return SaveResult.RecordInserted;
                            }
                        case PreserveIdOption.AddAsNewObject:
                            messages += "A Category with the same ID already exists, saving it with new ID.\n Old Category ID=" + category.CategoryId + "\nCategory Name=" + category.Name;
                            category.CategoryId = 0;
                            return category.Save();
                        case PreserveIdOption.JustLogWarning:
                            messages += "Unable to save Category, a Category with the same ID already exists.\n Category ID=" + category.CategoryId + "\nCategory Name=" + category.Name;
                            break;
                    }
                }
                else
                {
                    InsertCategoryWithID(category, database);
                    return SaveResult.RecordInserted;
                }
            }

            //SAVE IS SUCCESSFUL IF OBJECT IS NOT DIRTY
            return SaveResult.NotDirty;
        }

        public static SaveResult SaveWebpage(Webpage webpage, PreserveIdOption preserveIdOption, ref String messages)
        {
            if (webpage.IsDirty)
            {
                Database database = Token.Instance.Database;
                bool recordExists = true;

                //SET EMPTY STOREID TO CURRENT CONTEXT
                if (webpage.StoreId == 0) webpage.StoreId = Token.Instance.StoreId;
                if (webpage.WebpageId == 0) recordExists = false;

                if (recordExists)
                {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_Webpages");
                    selectQuery.Append(" WHERE WebpageId = @webpageId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@WebpageId", System.Data.DbType.Int32, webpage.WebpageId);
                        if ((int)database.ExecuteScalar(selectCommand) == 0)
                        {
                            recordExists = false;
                        }
                    }
                }


                if (recordExists)
                {
                    switch (preserveIdOption)
                    {
                        case PreserveIdOption.OverwriteExistingObject:
                            // DELETE THE EXISTING OBJECT
                            Webpage existingObject = WebpageDataSource.Load(webpage.WebpageId);
                            if (existingObject != null) existingObject.Delete();

                            // INSERT
                            InsertWebpageWithID(webpage, database);

                            if (webpage.IsDirty)
                            {
                                messages += "Error while saving/overwriting Webpage. A Webpage with the same ID already exists. Error while saving Webpage.\n Webpage ID=" + webpage.WebpageId + "\nWebpage Name=" + webpage.Name;
                                return SaveResult.Failed;
                            }
                            else
                            {
                                messages += "Found a Webpage with same ID, the existing Webpage is deleted and this one successfully saved with old ID. \n Webpage ID=" + webpage.WebpageId + "\nWebpage Name=" + webpage.Name;
                                return SaveResult.RecordInserted;
                            }
                            
                        case PreserveIdOption.AddAsNewObject:
                            messages += "A Webpage with the same ID already exists, saving it with new ID.\n Old Webpage ID=" + webpage.WebpageId + "\nWebpage Name=" + webpage.Name;
                            webpage.WebpageId = 0;
                            return webpage.Save();
                            
                        case PreserveIdOption.JustLogWarning:
                            messages += "Unable to save Webpage, a Webpage with the same ID already exists.\n Webpage ID=" + webpage.WebpageId + "\nWebpage Name=" + webpage.Name;
                            break;
                    }
                }
                else
                {
                    InsertWebpageWithID(webpage, database);
                    return SaveResult.RecordInserted;
                }
            }

            //SAVE IS SUCCESSFUL IF OBJECT IS NOT DIRTY
            return SaveResult.NotDirty;
        }

        public static SaveResult SaveAffiliate(Affiliate affiliate, PreserveIdOption preserveIdOption, ref String messages)
        {
            if (affiliate.IsDirty)
            {
                Database database = Token.Instance.Database;
                bool recordExists = true;

                //SET EMPTY STOREID TO CURRENT CONTEXT
                if (affiliate.StoreId == 0) affiliate.StoreId = Token.Instance.StoreId;
                if (affiliate.AffiliateId == 0) recordExists = false;

                if (recordExists)
                {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_Affiliates");
                    selectQuery.Append(" WHERE AffiliateId = @affiliateId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@AffiliateId", System.Data.DbType.Int32, affiliate.AffiliateId);
                        if ((int)database.ExecuteScalar(selectCommand) == 0)
                        {
                            recordExists = false;
                        }
                    }
                }


                if (recordExists)
                {
                    switch (preserveIdOption)
                    {
                        case PreserveIdOption.OverwriteExistingObject:
                            // DELETE THE EXISTING OBJECT
                            Affiliate existingObject = AffiliateDataSource.Load(affiliate.AffiliateId);
                            if (existingObject != null) existingObject.Delete();

                            // INSERT
                            InsertAffiliateWithID(affiliate, database);

                            if (affiliate.IsDirty)
                            {
                                messages += "Error while saving/overwriting Affiliate. An Affiliate with the same ID already exists. Error while saving Affiliate.\n Affiliate ID=" + affiliate.AffiliateId + "\nAffiliate Name=" + affiliate.Name;
                                return SaveResult.Failed;
                            }
                            else
                            {
                                messages += "Found a Affiliate with same ID, the existing Affiliate is deleted and this one successfully saved with old ID. \n Affiliate ID=" + affiliate.AffiliateId + "\nAffiliate Name=" + affiliate.Name;
                                return SaveResult.RecordInserted;
                            }

                        case PreserveIdOption.AddAsNewObject:
                            messages += "A Affiliate with the same ID already exists, saving it with new ID.\n Old Affiliate ID=" + affiliate.AffiliateId + "\nAffiliate Name=" + affiliate.Name;
                            affiliate.AffiliateId = 0;
                            return affiliate.Save();

                        case PreserveIdOption.JustLogWarning:
                            messages += "Unable to save Affiliate, an Affiliate with the same ID already exists.\n Affiliate ID=" + affiliate.AffiliateId + "\nAffiliate Name=" + affiliate.Name;
                            break;
                    }
                }
                else
                {
                    InsertAffiliateWithID(affiliate, database);
                    return SaveResult.RecordInserted;
                }
            }

            //SAVE IS SUCCESSFUL IF OBJECT IS NOT DIRTY
            return SaveResult.NotDirty;
        }

        public static SaveResult SaveUser(User user, PreserveIdOption preserveIdOption, ref String messages)
        {
            if (user.IsDirty)
            {
                Database database = Token.Instance.Database;
                bool recordExists = true;

                //SET EMPTY STOREID TO CURRENT CONTEXT
                if (user.StoreId == 0) user.StoreId = Token.Instance.StoreId;
                if (user.UserId == 0) recordExists = false;

                //SET DEFAULT FOR DATE FIELD
                if (user.CreateDate == System.DateTime.MinValue) user.CreateDate = LocaleHelper.LocalNow;

                if (recordExists)
                {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_Users");
                    selectQuery.Append(" WHERE UserId = @userId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@UserId", System.Data.DbType.Int32, user.UserId);
                        if ((int)database.ExecuteScalar(selectCommand) == 0)
                        {
                            recordExists = false;
                        }
                    }
                }


                if (recordExists)
                {
                    switch (preserveIdOption)
                    {
                        case PreserveIdOption.OverwriteExistingObject:
                            // DELETE THE EXISTING OBJECT
                            User existingObject = UserDataSource.Load(user.UserId);
                            if (existingObject != null) existingObject.Delete();

                            // INSERT
                            InsertUserWithID(user, database);

                            if (user.IsDirty)
                            {
                                messages += "The user " + user.UserName + " has an ID conflict and the existing user could not be overwritten. This user will not be imported. (User ID: " + user.UserId + ")";
                                return SaveResult.Failed;
                            }
                            else
                            {
                                return SaveResult.RecordInserted;
                            }

                        case PreserveIdOption.AddAsNewObject:
                            // NO WARNING SHOULD BE NECESSARY, WE ARE HANDLING THE ISSUE ACCORDING TO THE CONFIGURED RULES
                            user.UserId = 0;
                            return user.Save();

                        case PreserveIdOption.JustLogWarning:
                            messages += "The user " + user.UserName + " has an ID conflict and will not be imported. (User ID: " + user.UserId + ")";
                            break;
                    }
                }
                else
                {
                    InsertUserWithID(user, database);
                    return SaveResult.RecordInserted;
                }
            }

            //SAVE IS SUCCESSFUL IF OBJECT IS NOT DIRTY
            return SaveResult.NotDirty;
        }

        public static SaveResult SaveOrder(Order order, PreserveIdOption preserveIdOption, ref String messages)
        {
            if (order.IsDirty) 
            {
                Database database = Token.Instance.Database;
                bool orderNumberExists = true;

                //SET EMPTY STOREID TO CURRENT CONTEXT
                if (order.StoreId == 0) order.StoreId = Token.Instance.StoreId;
                if (order.OrderNumber == 0)
                {
                    orderNumberExists = false;
                    order.OrderNumber = StoreDataSource.GetNextOrderNumber(true);
                }

                // CHECK THE EXISTENCE OF ORDER BASED UPON ORDER NUMBER FIELD
                if (orderNumberExists)
                {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_Orders");
                    selectQuery.Append(" WHERE OrderNumber = @orderNumber");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@orderNumber", System.Data.DbType.Int32, order.OrderNumber);
                        if ((int)database.ExecuteScalar(selectCommand) == 0)
                        {
                            orderNumberExists = false;
                        }
                    }
                }

                int oldOrderId = order.OrderId;
                int oldOrderNumber = order.OrderNumber;
                order.OrderId = 0;
                if (orderNumberExists)
                {
                    switch (preserveIdOption)
                    {
                        case PreserveIdOption.OverwriteExistingObject:
                            // DELETE THE EXISTING OBJECT
                            Order existingObject = OrderDataSource.LoadForCriteria("OrderNumber = " + order.OrderNumber)[0];
                            if (existingObject != null) existingObject.Delete();

                            // INSERT, BUT DO NOT RE-CALCULATE SHIPMENT OR PAYMENT STATUS
                            order.Save(false);

                            if (order.IsDirty)
                            {
                                messages += "Error while saving/overwriting Order. An Order with the same Order Number already exists. Error while saving Order.\n Order ID=" + oldOrderId + "\nOrder Number=" + order.OrderNumber;
                                return SaveResult.Failed;
                            }
                            else
                            {
                                messages += "Found An Order with same Order Number, the existing Order is deleted and this one successfully saved with old Order Number. \n Order ID=" + oldOrderId + "\nOrder Number=" + order.OrderNumber;
                                return SaveResult.RecordInserted;
                            }

                        case PreserveIdOption.AddAsNewObject:
                            order.OrderNumber = StoreDataSource.GetNextOrderNumber(true);
                            messages += "Order number already exists in database, importing order with new number.\n Old Order ID: " + oldOrderId + "\nOld Order Number: " + oldOrderNumber + "\nNew Order Number: " + order.OrderNumber;
                            // SAVE WITHOUT RE-CALCULATING ORDER PAYMENT OR SHIPMENT STATUS
                            return order.Save(false);

                        case PreserveIdOption.JustLogWarning:
                            messages += "Unable to save Order, An Order with the same Order Number already exists.\n Order ID=" + order.OrderId + "\nOrder Number=" + order.OrderNumber;
                            break;
                    }
                }
                else
                {
                    // SAVE WITHOUT RE-CALCULATING ORDER PAYMENT OR SHIPMENT STATUS
                    return order.Save(false);                    
                }
            }

            //SAVE IS SUCCESSFUL IF OBJECT IS NOT DIRTY
            return SaveResult.NotDirty;
        }

        private static void InsertProductWithID(Product product, Database database)
        {            
            //INSERT
            StringBuilder insertQuery = new StringBuilder();
            insertQuery.AppendLine("SET IDENTITY_INSERT ac_Products ON;");

            // "ProductTemplateId" field is removed from 7.4
            //insertQuery.Append("INSERT INTO ac_Products (ProductId, StoreId, Name, Price, CostOfGoods, MSRP, Weight, Length, Width, Height, ManufacturerId, Sku, ModelNumber, DisplayPage, TaxCodeId, ShippableId, WarehouseId, InventoryModeId, InStock, InStockWarningLevel, ThumbnailUrl, ThumbnailAltText, ImageUrl, ImageAltText, Summary, Description, ExtendedDescription, VendorId, CreatedDate, LastModifiedDate, ProductTemplateId, IsFeatured, IsProhibited, AllowReviews, AllowBackorder, WrapGroupId, ExcludeFromFeed, HtmlHead, DisablePurchase, MinQuantity, MaxQuantity, VisibilityId, Theme, IconUrl, IconAltText, IsGiftCertificate, UseVariablePrice, MinimumPrice, MaximumPrice, SearchKeywords, HidePrice)");
            //insertQuery.Append(" VALUES (@ProductId, @StoreId, @Name, @Price, @CostOfGoods, @MSRP, @Weight, @Length, @Width, @Height, @ManufacturerId, @Sku, @ModelNumber, @DisplayPage, @TaxCodeId, @ShippableId, @WarehouseId, @InventoryModeId, @InStock, @InStockWarningLevel, @ThumbnailUrl, @ThumbnailAltText, @ImageUrl, @ImageAltText, @Summary, @Description, @ExtendedDescription, @VendorId, @CreatedDate, @LastModifiedDate, @ProductTemplateId, @IsFeatured, @IsProhibited, @AllowReviews, @AllowBackorder, @WrapGroupId, @ExcludeFromFeed, @HtmlHead, @DisablePurchase, @MinQuantity, @MaxQuantity, @VisibilityId, @Theme, @IconUrl, @IconAltText, @IsGiftCertificate, @UseVariablePrice, @MinimumPrice, @MaximumPrice, @SearchKeywords, @HidePrice)");
            insertQuery.Append("INSERT INTO ac_Products (ProductId, StoreId, Name, Price, CostOfGoods, MSRP, Weight, Length, Width, Height, ManufacturerId, Sku, ModelNumber, DisplayPage, TaxCodeId, ShippableId, WarehouseId, InventoryModeId, InStock, InStockWarningLevel, ThumbnailUrl, ThumbnailAltText, ImageUrl, ImageAltText, Summary, Description, ExtendedDescription, VendorId, CreatedDate, LastModifiedDate, IsFeatured, IsProhibited, AllowReviews, AllowBackorder, WrapGroupId, ExcludeFromFeed, HtmlHead, DisablePurchase, MinQuantity, MaxQuantity, VisibilityId, Theme, IconUrl, IconAltText, IsGiftCertificate, UseVariablePrice, MinimumPrice, MaximumPrice, SearchKeywords, HidePrice)");
            insertQuery.Append(" VALUES (@ProductId, @StoreId, @Name, @Price, @CostOfGoods, @MSRP, @Weight, @Length, @Width, @Height, @ManufacturerId, @Sku, @ModelNumber, @DisplayPage, @TaxCodeId, @ShippableId, @WarehouseId, @InventoryModeId, @InStock, @InStockWarningLevel, @ThumbnailUrl, @ThumbnailAltText, @ImageUrl, @ImageAltText, @Summary, @Description, @ExtendedDescription, @VendorId, @CreatedDate, @LastModifiedDate, @IsFeatured, @IsProhibited, @AllowReviews, @AllowBackorder, @WrapGroupId, @ExcludeFromFeed, @HtmlHead, @DisablePurchase, @MinQuantity, @MaxQuantity, @VisibilityId, @Theme, @IconUrl, @IconAltText, @IsGiftCertificate, @UseVariablePrice, @MinimumPrice, @MaximumPrice, @SearchKeywords, @HidePrice)");

            insertQuery.Append(";");
            insertQuery.AppendLine("");
            insertQuery.AppendLine("SET IDENTITY_INSERT ac_Products OFF;");
            using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
            {
                database.AddInParameter(insertCommand, "@ProductId", System.Data.DbType.Int32, product.ProductId);
                database.AddInParameter(insertCommand, "@StoreId", System.Data.DbType.Int32, product.StoreId);
                database.AddInParameter(insertCommand, "@Name", System.Data.DbType.String, product.Name);
                database.AddInParameter(insertCommand, "@Price", System.Data.DbType.Decimal, product.Price);
                database.AddInParameter(insertCommand, "@CostOfGoods", System.Data.DbType.Decimal, product.CostOfGoods);
                database.AddInParameter(insertCommand, "@MSRP", System.Data.DbType.Decimal, product.MSRP);
                database.AddInParameter(insertCommand, "@Weight", System.Data.DbType.Decimal, product.Weight);
                database.AddInParameter(insertCommand, "@Length", System.Data.DbType.Decimal, product.Length);
                database.AddInParameter(insertCommand, "@Width", System.Data.DbType.Decimal, product.Width);
                database.AddInParameter(insertCommand, "@Height", System.Data.DbType.Decimal, product.Height);
                database.AddInParameter(insertCommand, "@ManufacturerId", System.Data.DbType.Int32, NullableData.DbNullify(product.ManufacturerId));
                database.AddInParameter(insertCommand, "@Sku", System.Data.DbType.String, NullableData.DbNullify(product.Sku));
                database.AddInParameter(insertCommand, "@ModelNumber", System.Data.DbType.String, NullableData.DbNullify(product.ModelNumber));
                database.AddInParameter(insertCommand, "@DisplayPage", System.Data.DbType.String, NullableData.DbNullify(product.DisplayPage));
                database.AddInParameter(insertCommand, "@TaxCodeId", System.Data.DbType.Int32, NullableData.DbNullify(product.TaxCodeId));
                database.AddInParameter(insertCommand, "@ShippableId", System.Data.DbType.Byte, product.ShippableId);
                database.AddInParameter(insertCommand, "@WarehouseId", System.Data.DbType.Int32, NullableData.DbNullify(product.WarehouseId));
                database.AddInParameter(insertCommand, "@InventoryModeId", System.Data.DbType.Byte, product.InventoryModeId);
                database.AddInParameter(insertCommand, "@InStock", System.Data.DbType.Int32, product.InStock);
                database.AddInParameter(insertCommand, "@InStockWarningLevel", System.Data.DbType.Int32, product.InStockWarningLevel);
                database.AddInParameter(insertCommand, "@ThumbnailUrl", System.Data.DbType.String, NullableData.DbNullify(product.ThumbnailUrl));
                database.AddInParameter(insertCommand, "@ThumbnailAltText", System.Data.DbType.String, NullableData.DbNullify(product.ThumbnailAltText));
                database.AddInParameter(insertCommand, "@ImageUrl", System.Data.DbType.String, NullableData.DbNullify(product.ImageUrl));
                database.AddInParameter(insertCommand, "@ImageAltText", System.Data.DbType.String, NullableData.DbNullify(product.ImageAltText));
                database.AddInParameter(insertCommand, "@Summary", System.Data.DbType.String, NullableData.DbNullify(product.Summary));
                database.AddInParameter(insertCommand, "@Description", System.Data.DbType.String, NullableData.DbNullify(product.Description));
                database.AddInParameter(insertCommand, "@ExtendedDescription", System.Data.DbType.String, NullableData.DbNullify(product.ExtendedDescription));
                database.AddInParameter(insertCommand, "@VendorId", System.Data.DbType.Int32, NullableData.DbNullify(product.VendorId));
                database.AddInParameter(insertCommand, "@CreatedDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(product.CreatedDate));
                database.AddInParameter(insertCommand, "@LastModifiedDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(product.LastModifiedDate));
                // "ProductTemplateId" field is removed from 7.4
                //database.AddInParameter(insertCommand, "@ProductTemplateId", System.Data.DbType.Int32, NullableData.DbNullify(product.ProductTemplateId));
                database.AddInParameter(insertCommand, "@IsFeatured", System.Data.DbType.Boolean, product.IsFeatured);
                database.AddInParameter(insertCommand, "@IsProhibited", System.Data.DbType.Boolean, product.IsProhibited);
                database.AddInParameter(insertCommand, "@AllowReviews", System.Data.DbType.Boolean, product.AllowReviews);
                database.AddInParameter(insertCommand, "@AllowBackorder", System.Data.DbType.Boolean, product.AllowBackorder);
                database.AddInParameter(insertCommand, "@WrapGroupId", System.Data.DbType.Int32, NullableData.DbNullify(product.WrapGroupId));
                database.AddInParameter(insertCommand, "@ExcludeFromFeed", System.Data.DbType.Boolean, product.ExcludeFromFeed);
                database.AddInParameter(insertCommand, "@HtmlHead", System.Data.DbType.String, NullableData.DbNullify(product.HtmlHead));
                database.AddInParameter(insertCommand, "@DisablePurchase", System.Data.DbType.Boolean, product.DisablePurchase);
                database.AddInParameter(insertCommand, "@MinQuantity", System.Data.DbType.Int16, product.MinQuantity);
                database.AddInParameter(insertCommand, "@MaxQuantity", System.Data.DbType.Int16, product.MaxQuantity);
                database.AddInParameter(insertCommand, "@VisibilityId", System.Data.DbType.Byte, product.VisibilityId);
                database.AddInParameter(insertCommand, "@Theme", System.Data.DbType.String, NullableData.DbNullify(product.Theme));
                database.AddInParameter(insertCommand, "@IconUrl", System.Data.DbType.String, NullableData.DbNullify(product.IconUrl));
                database.AddInParameter(insertCommand, "@IconAltText", System.Data.DbType.String, NullableData.DbNullify(product.IconAltText));
                database.AddInParameter(insertCommand, "@IsGiftCertificate", System.Data.DbType.Boolean, product.IsGiftCertificate);
                database.AddInParameter(insertCommand, "@UseVariablePrice", System.Data.DbType.Boolean, product.UseVariablePrice);
                database.AddInParameter(insertCommand, "@MinimumPrice", System.Data.DbType.Decimal, NullableData.DbNullify(product.MinimumPrice));
                database.AddInParameter(insertCommand, "@MaximumPrice", System.Data.DbType.Decimal, NullableData.DbNullify(product.MaximumPrice));
                database.AddInParameter(insertCommand, "@SearchKeywords", System.Data.DbType.String, NullableData.DbNullify(product.SearchKeywords));
                database.AddInParameter(insertCommand, "@HidePrice", System.Data.DbType.Boolean, product.HidePrice);
                
                database.ExecuteScalar(insertCommand);
            }

            product.Categories.ProductId = product.ProductId;
            product.Categories.Save();
            product.SaveChildren();            
            product.IsDirty = false;
        }

        private static void InsertCategoryWithID(Category category, Database database)
        {            
            //INSERT
            StringBuilder insertQuery = new StringBuilder();
            insertQuery.AppendLine("SET IDENTITY_INSERT ac_Categories ON;");
            insertQuery.Append("INSERT INTO ac_Categories (CategoryId, StoreId, ParentId, Name, Summary, Description, ThumbnailUrl, ThumbnailAltText, DisplayPage, Theme, HtmlHead, VisibilityId)");
            insertQuery.Append(" VALUES (@CategoryId, @StoreId, @ParentId, @Name, @Summary, @Description, @ThumbnailUrl, @ThumbnailAltText, @DisplayPage, @Theme, @HtmlHead, @VisibilityId)");
            insertQuery.Append(";");
            insertQuery.AppendLine("");
            insertQuery.AppendLine("SET IDENTITY_INSERT ac_Categories OFF;");
            using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
            {
                database.AddInParameter(insertCommand, "@CategoryId", System.Data.DbType.Int32, category.CategoryId);
                database.AddInParameter(insertCommand, "@StoreId", System.Data.DbType.Int32, category.StoreId);
                database.AddInParameter(insertCommand, "@ParentId", System.Data.DbType.Int32, category.ParentId);
                database.AddInParameter(insertCommand, "@Name", System.Data.DbType.String, category.Name);
                database.AddInParameter(insertCommand, "@Summary", System.Data.DbType.String, NullableData.DbNullify(category.Summary));
                database.AddInParameter(insertCommand, "@Description", System.Data.DbType.String, NullableData.DbNullify(category.Description));
                database.AddInParameter(insertCommand, "@ThumbnailUrl", System.Data.DbType.String, NullableData.DbNullify(category.ThumbnailUrl));
                database.AddInParameter(insertCommand, "@ThumbnailAltText", System.Data.DbType.String, NullableData.DbNullify(category.ThumbnailAltText));
                database.AddInParameter(insertCommand, "@DisplayPage", System.Data.DbType.String, NullableData.DbNullify(category.DisplayPage));
                database.AddInParameter(insertCommand, "@Theme", System.Data.DbType.String, NullableData.DbNullify(category.Theme));
                database.AddInParameter(insertCommand, "@HtmlHead", System.Data.DbType.String, NullableData.DbNullify(category.HtmlHead));
                database.AddInParameter(insertCommand, "@VisibilityId", System.Data.DbType.Byte, category.VisibilityId);
                
                database.ExecuteScalar(insertCommand);                        
            }                        
            
            category.SaveChildren();
            Category.UpdateParent(category.CategoryId, category.ParentId);
            category.IsDirty = false;
        }

        private static void InsertWebpageWithID(Webpage webpage, Database database)
        {
            //INSERT
            StringBuilder insertQuery = new StringBuilder();
            insertQuery.AppendLine("SET IDENTITY_INSERT ac_Webpages ON;");
            insertQuery.Append("INSERT INTO ac_Webpages (WebpageId, StoreId, Name, Summary, Description, ThumbnailUrl, ThumbnailAltText, DisplayPage, Theme, HtmlHead, VisibilityId)");
            insertQuery.Append(" VALUES (@WebpageId, @StoreId, @Name, @Summary, @Description, @ThumbnailUrl, @ThumbnailAltText, @DisplayPage, @Theme, @HtmlHead, @VisibilityId)");
            insertQuery.Append(";");
            insertQuery.AppendLine("");
            insertQuery.AppendLine("SET IDENTITY_INSERT ac_Webpages OFF;");
            using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
            {
                database.AddInParameter(insertCommand, "@WebpageId", System.Data.DbType.Int32, webpage.WebpageId);
                database.AddInParameter(insertCommand, "@StoreId", System.Data.DbType.Int32, webpage.StoreId);
                database.AddInParameter(insertCommand, "@Name", System.Data.DbType.String, NullableData.DbNullify(webpage.Name));
                database.AddInParameter(insertCommand, "@Summary", System.Data.DbType.String, NullableData.DbNullify(webpage.Summary));
                database.AddInParameter(insertCommand, "@Description", System.Data.DbType.String, NullableData.DbNullify(webpage.Description));
                database.AddInParameter(insertCommand, "@ThumbnailUrl", System.Data.DbType.String, NullableData.DbNullify(webpage.ThumbnailUrl));
                database.AddInParameter(insertCommand, "@ThumbnailAltText", System.Data.DbType.String, NullableData.DbNullify(webpage.ThumbnailAltText));
                database.AddInParameter(insertCommand, "@DisplayPage", System.Data.DbType.String, NullableData.DbNullify(webpage.DisplayPage));
                database.AddInParameter(insertCommand, "@Theme", System.Data.DbType.String, NullableData.DbNullify(webpage.Theme));
                database.AddInParameter(insertCommand, "@HtmlHead", System.Data.DbType.String, NullableData.DbNullify(webpage.HtmlHead));
                database.AddInParameter(insertCommand, "@VisibilityId", System.Data.DbType.Byte, webpage.VisibilityId);
                
                database.ExecuteScalar(insertCommand);
            }
            webpage.Categories.WebpageId = webpage.WebpageId;
            webpage.Categories.Save();
            webpage.IsDirty = false;
        }

        private static void InsertAffiliateWithID(Affiliate affiliate, Database database)
        {
            //INSERT
            StringBuilder insertQuery = new StringBuilder();
            insertQuery.AppendLine("SET IDENTITY_INSERT ac_Affiliates ON;");

            insertQuery.Append("INSERT INTO ac_Affiliates (AffiliateId, StoreId, Name, PayeeName, FirstName, LastName, Company, Address1, Address2, City, Province, PostalCode, CountryCode, PhoneNumber, FaxNumber, MobileNumber, WebsiteUrl, Email, CommissionRate, CommissionIsPercent, CommissionOnTotal, ReferralDays)");
            insertQuery.Append(" VALUES (@AffiliateId, @StoreId, @Name, @PayeeName, @FirstName, @LastName, @Company, @Address1, @Address2, @City, @Province, @PostalCode, @CountryCode, @PhoneNumber, @FaxNumber, @MobileNumber, @WebsiteUrl, @Email, @CommissionRate, @CommissionIsPercent, @CommissionOnTotal, @ReferralDays)");
            insertQuery.Append(";");

            insertQuery.AppendLine("SET IDENTITY_INSERT ac_Webpages OFF;");
            using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
            {
                database.AddInParameter(insertCommand, "@AffiliateId", System.Data.DbType.Int32, affiliate.AffiliateId);
                database.AddInParameter(insertCommand, "@StoreId", System.Data.DbType.Int32, affiliate.StoreId);
                database.AddInParameter(insertCommand, "@Name", System.Data.DbType.String, affiliate.Name);
                database.AddInParameter(insertCommand, "@PayeeName", System.Data.DbType.String, NullableData.DbNullify(affiliate.PayeeName));
                database.AddInParameter(insertCommand, "@FirstName", System.Data.DbType.String, NullableData.DbNullify(affiliate.FirstName));
                database.AddInParameter(insertCommand, "@LastName", System.Data.DbType.String, NullableData.DbNullify(affiliate.LastName));
                database.AddInParameter(insertCommand, "@Company", System.Data.DbType.String, NullableData.DbNullify(affiliate.Company));
                database.AddInParameter(insertCommand, "@Address1", System.Data.DbType.String, NullableData.DbNullify(affiliate.Address1));
                database.AddInParameter(insertCommand, "@Address2", System.Data.DbType.String, NullableData.DbNullify(affiliate.Address2));
                database.AddInParameter(insertCommand, "@City", System.Data.DbType.String, NullableData.DbNullify(affiliate.City));
                database.AddInParameter(insertCommand, "@Province", System.Data.DbType.String, NullableData.DbNullify(affiliate.Province));
                database.AddInParameter(insertCommand, "@PostalCode", System.Data.DbType.String, NullableData.DbNullify(affiliate.PostalCode));
                database.AddInParameter(insertCommand, "@CountryCode", System.Data.DbType.String, NullableData.DbNullify(affiliate.CountryCode));
                database.AddInParameter(insertCommand, "@PhoneNumber", System.Data.DbType.String, NullableData.DbNullify(affiliate.PhoneNumber));
                database.AddInParameter(insertCommand, "@FaxNumber", System.Data.DbType.String, NullableData.DbNullify(affiliate.FaxNumber));
                database.AddInParameter(insertCommand, "@MobileNumber", System.Data.DbType.String, NullableData.DbNullify(affiliate.MobileNumber));
                database.AddInParameter(insertCommand, "@WebsiteUrl", System.Data.DbType.String, NullableData.DbNullify(affiliate.WebsiteUrl));
                database.AddInParameter(insertCommand, "@Email", System.Data.DbType.String, NullableData.DbNullify(affiliate.Email));
                database.AddInParameter(insertCommand, "@CommissionRate", System.Data.DbType.Decimal, affiliate.CommissionRate);
                database.AddInParameter(insertCommand, "@CommissionIsPercent", System.Data.DbType.Boolean, affiliate.CommissionIsPercent);
                database.AddInParameter(insertCommand, "@CommissionOnTotal", System.Data.DbType.Boolean, affiliate.CommissionOnTotal);
                database.AddInParameter(insertCommand, "@ReferralDays", System.Data.DbType.Int16, affiliate.ReferralDays);

                database.ExecuteScalar(insertCommand);                
            }
            
            affiliate.IsDirty = false;
        }


        private static void InsertUserWithID(User user, Database database)
        {
            //INSERT
            StringBuilder insertQuery = new StringBuilder();
            insertQuery.AppendLine("SET IDENTITY_INSERT ac_Users ON;");
            insertQuery.Append("INSERT INTO ac_Users (UserId, StoreId, UserName, LoweredUserName, Email, LoweredEmail, ReferringAffiliateId, AffiliateId, AffiliateReferralDate, PrimaryAddressId, PrimaryWishlistId, PayPalId, PasswordQuestion, PasswordAnswer, IsApproved, IsAnonymous, IsLockedOut, CreateDate, LastActivityDate, LastLoginDate, LastPasswordChangedDate, LastLockoutDate, FailedPasswordAttemptCount, FailedPasswordAttemptWindowStart, FailedPasswordAnswerAttemptCount, FailedPasswordAnswerAttemptWindowStart, Comment)");
            insertQuery.Append(" VALUES (@UserId, @StoreId, @UserName, @LoweredUserName, @Email, @LoweredEmail, @ReferringAffiliateId, @AffiliateId, @AffiliateReferralDate, @PrimaryAddressId, @PrimaryWishlistId, @PayPalId, @PasswordQuestion, @PasswordAnswer, @IsApproved, @IsAnonymous, @IsLockedOut, @CreateDate, @LastActivityDate, @LastLoginDate, @LastPasswordChangedDate, @LastLockoutDate, @FailedPasswordAttemptCount, @FailedPasswordAttemptWindowStart, @FailedPasswordAnswerAttemptCount, @FailedPasswordAnswerAttemptWindowStart, @Comment)");
            insertQuery.Append(";");
            insertQuery.AppendLine("");
            insertQuery.AppendLine("SET IDENTITY_INSERT ac_Users OFF;");
            using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
            {
                database.AddInParameter(insertCommand, "@UserId", System.Data.DbType.Int32, user.UserId);
                database.AddInParameter(insertCommand, "@StoreId", System.Data.DbType.Int32, user.StoreId);
                database.AddInParameter(insertCommand, "@UserName", System.Data.DbType.String, user.UserName);
                database.AddInParameter(insertCommand, "@LoweredUserName", System.Data.DbType.String, user.LoweredUserName);
                database.AddInParameter(insertCommand, "@Email", System.Data.DbType.String, NullableData.DbNullify(user.Email));
                database.AddInParameter(insertCommand, "@LoweredEmail", System.Data.DbType.String, NullableData.DbNullify(user.LoweredEmail));
                database.AddInParameter(insertCommand, "@ReferringAffiliateId", System.Data.DbType.Int32, NullableData.DbNullify(user.ReferringAffiliateId));
                database.AddInParameter(insertCommand, "@AffiliateId", System.Data.DbType.Int32, NullableData.DbNullify(user.AffiliateId));
                database.AddInParameter(insertCommand, "@AffiliateReferralDate", System.Data.DbType.DateTime, NullableData.DbNullify(LocaleHelper.FromLocalTime(user.AffiliateReferralDate)));
                database.AddInParameter(insertCommand, "@PrimaryAddressId", System.Data.DbType.Int32, NullableData.DbNullify(user.PrimaryAddressId));
                database.AddInParameter(insertCommand, "@PrimaryWishlistId", System.Data.DbType.Int32, NullableData.DbNullify(user.PrimaryWishlistId));
                database.AddInParameter(insertCommand, "@PayPalId", System.Data.DbType.String, NullableData.DbNullify(user.PayPalId));
                database.AddInParameter(insertCommand, "@PasswordQuestion", System.Data.DbType.String, NullableData.DbNullify(user.PasswordQuestion));
                database.AddInParameter(insertCommand, "@PasswordAnswer", System.Data.DbType.String, NullableData.DbNullify(user.PasswordAnswer));
                database.AddInParameter(insertCommand, "@IsApproved", System.Data.DbType.Boolean, user.IsApproved);
                database.AddInParameter(insertCommand, "@IsAnonymous", System.Data.DbType.Boolean, user.IsAnonymous);
                database.AddInParameter(insertCommand, "@IsLockedOut", System.Data.DbType.Boolean, user.IsLockedOut);
                database.AddInParameter(insertCommand, "@CreateDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(user.CreateDate));
                database.AddInParameter(insertCommand, "@LastActivityDate", System.Data.DbType.DateTime, NullableData.DbNullify(LocaleHelper.FromLocalTime(user.LastActivityDate)));
                database.AddInParameter(insertCommand, "@LastLoginDate", System.Data.DbType.DateTime, NullableData.DbNullify(LocaleHelper.FromLocalTime(user.LastLoginDate)));
                database.AddInParameter(insertCommand, "@LastPasswordChangedDate", System.Data.DbType.DateTime, NullableData.DbNullify(LocaleHelper.FromLocalTime(user.LastPasswordChangedDate)));
                database.AddInParameter(insertCommand, "@LastLockoutDate", System.Data.DbType.DateTime, NullableData.DbNullify(LocaleHelper.FromLocalTime(user.LastLockoutDate)));
                database.AddInParameter(insertCommand, "@FailedPasswordAttemptCount", System.Data.DbType.Int32, user.FailedPasswordAttemptCount);
                database.AddInParameter(insertCommand, "@FailedPasswordAttemptWindowStart", System.Data.DbType.DateTime, NullableData.DbNullify(LocaleHelper.FromLocalTime(user.FailedPasswordAttemptWindowStart)));
                database.AddInParameter(insertCommand, "@FailedPasswordAnswerAttemptCount", System.Data.DbType.Int32, user.FailedPasswordAnswerAttemptCount);
                database.AddInParameter(insertCommand, "@FailedPasswordAnswerAttemptWindowStart", System.Data.DbType.DateTime, NullableData.DbNullify(LocaleHelper.FromLocalTime(user.FailedPasswordAnswerAttemptWindowStart)));
                database.AddInParameter(insertCommand, "@Comment", System.Data.DbType.String, NullableData.DbNullify(user.Comment));

                database.ExecuteScalar(insertCommand);
            }
            user.SaveChildren();                        
            user.IsDirty = false;
        }

        /// <summary>
        /// Will check and extend database field lengths
        /// </summary>
        /// <param name="update">if true the field lengths will be updated as well</param>
        /// <returns>Detailed log message</returns>
        public static String CheckFieldLengths(bool update)
        {
            SortedDictionary<string,String[]> fieldsData = new SortedDictionary<string,String[]>();

            fieldsData.Add("ac_Products", new String[] { "Name:255:NOT NULL", "ThumbnailAltText:255:NULL", "ImageAltText:255:NULL", "IconAltText:255:NULL" });
            fieldsData.Add("ac_KitProducts", new String[] { "Name:255:NOT NULL" });
            fieldsData.Add("ac_OrderItems", new String[] { "Name:255:NULL" });

            fieldsData.Add("ac_Transactions", new String[] { "RemoteIP:39:NULL" });
            fieldsData.Add("ac_PageViews", new String[] { "RemoteIP:39:NULL" });
            fieldsData.Add("ac_Orders", new String[] { "RemoteIP:39:NULL" });
            fieldsData.Add("ac_EmailListUsers", new String[] { "SignupIP:39:NULL" });
            fieldsData.Add("ac_Downloads", new String[] { "RemoteAddr:39:NULL" });
            fieldsData.Add("ac_AuditEvents", new String[] { "RemoteIP:39:NULL" });

            fieldsData.Add("ac_DigitalGoods", new String[] { "Name:255:NOT NULL", "FileName:255:NOT NULL", "MediaKey:255:NULL" });
            fieldsData.Add("ac_OrderItemDigitalGoods", new String[] { "Name:255:NOT NULL" });

            fieldsData.Add("ac_GiftCertificates", new String[] { "Name:255:NOT NULL", "SerialNumber:50:NOT NULL" });
            
            fieldsData.Add("ac_OptionChoices", new String[] { "Name:255:NOT NULL", "SkuModifier:255:NULL" });

            Database database = Token.Instance.Database;
            StringBuilder logBuilder = new StringBuilder();
            foreach (String tableName in fieldsData.Keys)
            {
                String[] fields = fieldsData[tableName];
                foreach (String fieldData in fields)
                {
                    String[] arrFieldData = fieldData.Split(':');
                    String fieldName = arrFieldData[0];
                    int expectedLength = AlwaysConvert.ToInt(arrFieldData[1]);
                    String nullIndicator = arrFieldData[2];
                    try
                    {
                        int length = GetColumnMaxLength(database, tableName, fieldName);
                        if (length < expectedLength)
                        {
                            if (update)
                            {
                                database.ExecuteNonQuery(CommandType.Text, String.Format("ALTER TABLE {0} ALTER COLUMN [{1}] nvarchar({2}) {3}", tableName, fieldName, expectedLength, nullIndicator));
                                logBuilder.AppendLine(String.Format("Updated length for {0} field in {1} database table.", fieldName, tableName));
                            }
                            else
                            {
                                logBuilder.AppendLine(String.Format("Field length should be extended from '{0}' to '{1}' for field '{2}' for database table '{3}'.", length, expectedLength, fieldName, tableName));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logBuilder.AppendLine(String.Format("An error has occured while {0} database field length, table name:{1}, field name:{2}. Exception details:{3}", (update? "extending":"checking"), tableName, fieldName, ex.Message));
                    }
                }
            }
            return logBuilder.ToString();
        }

        private static int GetColumnMaxLength(Database database, String tableName, String fieldName)
        {
            String sql = String.Format("SELECT CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = '{0}' AND COLUMN_NAME = '{1}'", tableName, fieldName);
            int length = (int)database.ExecuteScalar(CommandType.Text, sql);
            return length;
        }
    }

    

    public enum PreserveIdOption
    {
        /// <summary>
        /// Will delete the existing object with sameId, and will save the new object with that Id
        /// </summary>
        OverwriteExistingObject,
        /// <summary>
        /// If an existing object with same id is found then it will save the object with a new id
        /// </summary>
        AddAsNewObject,
        /// <summary>
        /// If an existing object with the same id is found then it will not save the object and will just log the warning.
        /// </summary>
        JustLogWarning
    };
}
