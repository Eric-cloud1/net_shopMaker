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
        ImportRequest objImportRequest = null;
        private ImportOptions _importOption = ImportOptions.ImportOrUpdate;


        /// <summary>
        /// Default value is ImportOptions.UpdateImport
        /// </summary>
        public ImportOptions ImportOption
        {
            get { return _importOption; }
            set { _importOption = value; }
        }

        String requestId = String.Empty;

        public String RequestId
        {
            get { return requestId; }
            set { requestId = value; }
        }

        public ImportHandler(ImportRequest objImportRequest)
        {
            this.objImportRequest = objImportRequest;
            ImportOption = (ImportOptions)Enum.Parse(typeof(ImportOptions), objImportRequest.ImportOption);
            this.RequestId = objImportRequest.RequestId;
        }

        #region Global Variables which will be Used Throughout The Import Process
        SortedDictionary<String, int> _TranslationDic = null;
        StringBuilder sWarnings = new StringBuilder();
        StringBuilder sLog = new StringBuilder();
        SortedDictionary<String, int> dicTaxCode = null;
        SortedDictionary<String, int> dicVendor = null;
        SortedDictionary<String, int> dicOrderStatus = null;
        SortedDictionary<String, int> dicManufacturer = null;
        SortedDictionary<String, int> dicCategory = new SortedDictionary<string, int>();
        SortedDictionary<String, int> dicWarehouse = null;
        SortedDictionary<String, int> dicAffiliate = null;
        SortedDictionary<String, int> dicUser = null;
        SortedDictionary<String, int> dicProdcutTemplate = null;
        SortedDictionary<String, int> dicWrapGroup = null;

        public SortedDictionary<string, int> TranslationDic
        {
            get
            {
                if (_TranslationDic == null)
                {
                    // TRY TO LOAD FROM CACHE
                    _TranslationDic = HttpContext.Current.Cache["TRANSLATIONDIC" + this.RequestId] as SortedDictionary<string, int>;
                    if (_TranslationDic == null)
                    {
                        _TranslationDic = new SortedDictionary<string, int>();
                    }                    
                }
                return _TranslationDic;
            }
            set { _TranslationDic = value; }            
        }

        #endregion

        public byte[] ImportStandardData()
        {   

            ACImportResponse objACImportResponse = new ACImportResponse();
            objACImportResponse.ImportResponse = new ImportResponse();
            MakerShop.DataClient.Api.Schema.Store store = ((MakerShopExport)EncodeHelper.Deserialize(ZipUtility.DecompressToString(objImportRequest.Data), typeof(MakerShopExport))).Store;

            ImportAffiliates(store.Affiliates);

            //BannedIPs
            ImportBannedIPs(store.BannedIPs);

            //Currencies
            ImportCurrencies(store.Currencies);

            ImportReadmes(store.Readmes);

            ImportLicenseAgreements(store.LicenseAgreements);

            ImportErrorMessages(store.ErrorMessages);

            ImportStoreSettings(store.Settings);

            ImportPaymentGateways(store.PaymentGateways);

            ImportShipGateways(store.ShipGateways);

            ImportTaxGateways(store.TaxGateways);

            ImportManufacturers(store.Manufacturers);

            ImportTaxCodes(store.TaxCodes);

            ImportRoles(store.Roles);

            ImportEmailTemplates(store.EmailTemplates);

            ImportPersonalizationPaths(store.PersonalizationPaths);

            ImportCountries(store.Countries);

            ImportGroups(store.Groups);

            ImportWarehouses(store.Warehouses);

            ImportTaxRules(store.TaxRules);

            ImportVolumeDiscounts(store.VolumeDiscounts);

            ImportVendors(store.Vendors);

            ImportWrapGroups(store.WrapGroups);

            ImportProductTemplates(store.ProductTemplates);

            ImportCategories(store.Categories, store.CategoryCSVFields, store.CategoryMatchFields);

            ImportLinks(store.Links);

            ImportWebpages(store.Webpages);

            ImportPaymentMethods(store.PaymentMethods);

            ImportOrderStatuses(store.OrderStatuses);

            ImportShipZones(store.ShipZones);

            ImportShipMethods(store.ShipMethods);

            ImportDigitalGoods(store.DigitalGoods);

            ImportUsers(store.Users, store.UserCSVFields, store.UserMatchFields);

            ImportEmailLists(store.EmailLists);

            ImportAuditEvents(store.AuditEvents);

            ImportProducts(store.Products, store.ProductCSVFields, store.ProductMatchFields);

            ImportComponents(store.KitComponents);

            ImportProductAssociations(store.ProductAssociations);

            ImportCoupons(store.Coupons);

            ImportCustomFields(store.CustomFields);

            ImportPageViews(store.PageViews);

            ImportOrders(store.Orders, store.OrderCSVFields, store.OrderMatchFields);

            ImportGiftCertificates(store.GiftCertificates);

            // REMOVE THE LAST ITEM FROM CACHE 
            HttpContext.Current.Cache.Remove("TRANSLATIONDIC" + this.RequestId);

            // IF IT IS NOT LAST CHUNK REQUEST THEN CACHE THE TRANSLATION DIC
            if (!this.objImportRequest.IsLastChunkRequest)
            {
                HttpContext.Current.Cache.Add("TRANSLATIONDIC" + this.RequestId, this.TranslationDic, null, DateTime.UtcNow.AddHours(2).AddSeconds(-1), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.NotRemovable, null);
            }

            objACImportResponse.ImportResponse.Log = new Log();
            objACImportResponse.ImportResponse.Log.Message = "\n" + sLog.ToString();
            if (sWarnings.Length > 0) objACImportResponse.ImportResponse.Log.WarningMessages += "\n" + sWarnings.ToString();
            objACImportResponse.ImportResponse.ResponseId = RequestId;
            byte[] byteRequestXml = EncodeHelper.Serialize(objACImportResponse);
            return byteRequestXml;
        }

        
        private void ImportCustomFields(MakerShop.DataClient.Api.Schema.CustomField[] arrCustomField)
        {
            if (arrCustomField != null && arrCustomField.Length > 0)
            {
                int imported = 0;
                int updated = 0;
                log("Importing " + arrCustomField.Length + " customFields .");                
                DataObject objDataObject = new DataObject("CustomField", typeof(MakerShop.Stores.CustomField), typeof(Api.Schema.CustomField));
                MakerShop.Stores.CustomField objCustomField = null;
                Api.Schema.CustomField logObject = null;
                foreach (MakerShop.DataClient.Api.Schema.CustomField objClientApiCustomField in arrCustomField)
                {
                    try
                    {
                        logObject = objClientApiCustomField;
                        int customFieldId = objClientApiCustomField.CustomFieldId;
                        if (ImportOption == ImportOptions.Import)
                        {
                            //objCustomField = CustomFieldDataSource.Load(objClientApiCustomField.CustomFieldId);
                            //if (objCustomField != null)
                            //{
                            //    log("CustomField already exist");
                            //    continue;
                            //}
                            objCustomField = (MakerShop.Stores.CustomField)objDataObject.ConvertToAC6Object(objClientApiCustomField);
                            if (objCustomField == null)
                            {
                                log("CustomField not saved");
                                continue;
                            }
                            objCustomField.CustomFieldId = 0;
                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            objCustomField = CustomFieldDataSource.Load(objClientApiCustomField.CustomFieldId);
                            if (objCustomField != null)
                            {
                                objCustomField = (MakerShop.Stores.CustomField)objDataObject.UpdateToAC6Object(objClientApiCustomField, objCustomField);
                                updated++;
                            }
                            else
                            {
                                continue;
                            }

                        }
                        else if (ImportOption == ImportOptions.ImportOrUpdate)
                        {
                            objCustomField = CustomFieldDataSource.Load(objClientApiCustomField.CustomFieldId);
                            if (objCustomField != null)
                            {
                                objCustomField = (MakerShop.Stores.CustomField)objDataObject.UpdateToAC6Object(objClientApiCustomField, objCustomField);
                                updated++;
                            }
                            else
                            {
                                objCustomField = (MakerShop.Stores.CustomField)objDataObject.ConvertToAC6Object(objClientApiCustomField);
                                objCustomField.CustomFieldId = 0;
                                imported++;
                            }
                        }
                        objCustomField.StoreId = Token.Instance.StoreId;
                        if (!objCustomField.Save().Equals(SaveResult.Failed))
                        {
                            AddToTranslationDic("CUSTOMFIELD" + objClientApiCustomField.CustomFieldId, objCustomField.CustomFieldId);                            
                        }
                        else
                        {
                            sWarnings.Append("CustomField Not Saved\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for customField");
                        if (logObject != null)
                        {
                            strLog.AppendLine("CustomFieldId= " + logObject.CustomFieldId);
                        }
                        strLog.AppendLine("Error message: " + ex.Message);                        
                        log(strLog.ToString());
                    }
                }
                log(imported + " CustomFields imported, " + updated + " CustomFields updated.");
                log("CustomFields Import Complete...");
            }
            //else
            //{
            //    log("No CustomFields available to Import...");
            //}
        }

      

        private bool IsNonEmptyArray(System.Array tempArray)
        {
            return (tempArray != null && tempArray.Length > 0);
        }

        private void ImportWarehouses(MakerShop.DataClient.Api.Schema.Warehouse[] arrWarehouse)
        {
            if (arrWarehouse != null && arrWarehouse.Length > 0)
            {
                int imported = 0;
                int updated = 0;
                log("Importing " + arrWarehouse.Length + " Warehouses .");                
                DataObject objDataObject = new DataObject("Warehouse", typeof(MakerShop.Shipping.Warehouse), typeof(Api.Schema.Warehouse));
                MakerShop.Shipping.Warehouse objWarehouse = null;
                Api.Schema.Warehouse logObject = null;
                foreach (MakerShop.DataClient.Api.Schema.Warehouse objClientApiWarehouse in arrWarehouse)
                {
                    try
                    {
                        logObject = objClientApiWarehouse;
                        int taxCodeId = objClientApiWarehouse.WarehouseId;
                        if (ImportOption == ImportOptions.Import)
                        {
                            objWarehouse = WarehouseDataSource.Load(objClientApiWarehouse.WarehouseId);
                            if (objWarehouse != null)
                            {
                                log("Warehouse already exist");
                                continue;
                            }
                            objWarehouse = (MakerShop.Shipping.Warehouse)objDataObject.ConvertToAC6Object(objClientApiWarehouse);
                            if (objWarehouse == null)
                            {
                                log("Warehouse not saved");
                                continue;
                            }
                            objWarehouse.WarehouseId = 0;
                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            objWarehouse = WarehouseDataSource.Load(objClientApiWarehouse.WarehouseId);
                            if (objWarehouse != null)
                            {
                                objWarehouse = (MakerShop.Shipping.Warehouse)objDataObject.UpdateToAC6Object(objClientApiWarehouse, objWarehouse);
                                updated++;
                            }
                            else
                            {
                                continue;
                            }

                        }
                        else if (ImportOption == ImportOptions.ImportOrUpdate)
                        {
                            objWarehouse = WarehouseDataSource.Load(objClientApiWarehouse.WarehouseId);
                            if (objWarehouse != null)
                            {
                                objWarehouse = (MakerShop.Shipping.Warehouse)objDataObject.UpdateToAC6Object(objClientApiWarehouse, objWarehouse);
                                updated++;
                            }
                            else
                            {
                                objWarehouse = (MakerShop.Shipping.Warehouse)objDataObject.ConvertToAC6Object(objClientApiWarehouse);
                                objWarehouse.WarehouseId = 0;
                                imported++;
                            }
                        }
                        objWarehouse.StoreId = Token.Instance.StoreId;
                        if (!objWarehouse.Save().Equals(SaveResult.Failed))
                        {
                            AddToTranslationDic("WAREHOUSE" + objClientApiWarehouse.WarehouseId, objWarehouse.WarehouseId);
                        }
                        else
                        {
                            sWarnings.Append("Warehouse Not Saved\n");
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for warehouse");
                        
                        strLog.AppendLine("Error message: " + ex.Message);                        
                        log(strLog.ToString());
                    }
                }
                log(imported + " Warehouses imported, " + updated + " Warehouses updated.");
                log("Warehouses Import Complete...");
            }
            //else
            //{
            //    log("No Warehouses available to Import...");
            //}
        }

       
       
        private void ImportMenufacturers(MakerShop.DataClient.Api.Schema.Manufacturer[] arrManufacturer)
        {
            if (arrManufacturer != null && arrManufacturer.Length > 0)
            {
                int imported = 0;
                int updated = 0;
                log("Importing " + arrManufacturer.Length + " Manufacturers .");
                
                DataObject objDataObject = new DataObject("Manufacturer", typeof(MakerShop.Products.Manufacturer), typeof(Api.Schema.Manufacturer));
                MakerShop.Products.Manufacturer objManufacturer = null;
                Api.Schema.Manufacturer logObject = null;
                foreach (MakerShop.DataClient.Api.Schema.Manufacturer objClientApiManufacturer in arrManufacturer)
                {
                    try
                    {
                        logObject = objClientApiManufacturer;
                        int manufacturerId = objClientApiManufacturer.ManufacturerId;
                        if (ImportOption == ImportOptions.Import)
                        {
                            objManufacturer = ManufacturerDataSource.Load(objClientApiManufacturer.ManufacturerId);
                            if (objManufacturer != null && objManufacturer.Name.Equals(objClientApiManufacturer.Name))
                            {
                                log("Manufacturer already exist");
                                continue;
                            }
                            objManufacturer = (MakerShop.Products.Manufacturer)objDataObject.ConvertToAC6Object(objClientApiManufacturer);
                            if (objManufacturer == null)
                            {
                                log("Manufacturer not saved");
                                continue;
                            }
                            objManufacturer.ManufacturerId = 0;
                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            objManufacturer = ManufacturerDataSource.Load(objClientApiManufacturer.ManufacturerId);
                            if (objManufacturer != null)
                            {
                                objManufacturer = (MakerShop.Products.Manufacturer)objDataObject.UpdateToAC6Object(objClientApiManufacturer, objManufacturer);
                                updated++;
                            }
                            else
                            {
                                continue;
                            }

                        }
                        else if (ImportOption == ImportOptions.ImportOrUpdate)
                        {
                            objManufacturer = ManufacturerDataSource.Load(objClientApiManufacturer.ManufacturerId);
                            if (objManufacturer != null)
                            {
                                objManufacturer = (MakerShop.Products.Manufacturer)objDataObject.UpdateToAC6Object(objClientApiManufacturer, objManufacturer);
                                updated++;
                            }
                            else
                            {
                                objManufacturer = (MakerShop.Products.Manufacturer)objDataObject.ConvertToAC6Object(objClientApiManufacturer);
                                objManufacturer.ManufacturerId = 0;
                                imported++;
                            }
                        }

                        if (!objManufacturer.Save().Equals(SaveResult.Failed))
                        {
                            AddToTranslationDic("MANUFACTURER" + objClientApiManufacturer.ManufacturerId, objManufacturer.ManufacturerId);
                        }
                        else
                        {
                            sWarnings.Append("Manufacturer Not Saved\n");
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for Manufacturer");
                        
                        strLog.AppendLine("Error message: " + ex.Message);                        
                        log(strLog.ToString());
                    }
                }
                log(imported + " Manufacturer imported, " + updated + " Manufacturer updated.");
                log("Manufacturer Import Complete...");
            }
            //else
            //{
            //    log("No Manufacturer available to Import...");
            //}
        }


        private void ImportCoupons(MakerShop.DataClient.Api.Schema.Coupon[] arrCoupon)
        {
            if (arrCoupon != null && arrCoupon.Length > 0)
            {
                int imported = 0;
                int updated = 0;
                log("Importing " + arrCoupon.Length + " Coupons .");
                
                DataObject objDataObject = new DataObject("Coupon", typeof(MakerShop.Marketing.Coupon), typeof(Api.Schema.Coupon));
                MakerShop.Marketing.Coupon objCoupon = null;
                Api.Schema.Coupon logObject = null;
                foreach (MakerShop.DataClient.Api.Schema.Coupon objClientApiCoupon in arrCoupon)
                {
                    try
                    {
                        logObject = objClientApiCoupon;
                        int couponId = objClientApiCoupon.CouponId;
                        if (ImportOption == ImportOptions.Import)
                        {
                            objCoupon = CouponDataSource.Load(objClientApiCoupon.CouponId);
                            if (objCoupon != null && objCoupon.Name.Equals(objClientApiCoupon.Name))
                            {
                                log("Coupon already exist");
                                continue;
                            }
                            objCoupon = (MakerShop.Marketing.Coupon)objDataObject.ConvertToAC6Object(objClientApiCoupon);
                            if (objCoupon == null)
                            {
                                log("Coupon not saved");
                                continue;
                            }
                            objCoupon.CouponId = 0;
                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            objCoupon = CouponDataSource.Load(objClientApiCoupon.CouponId);
                            if (objCoupon != null)
                            {
                                objCoupon = (MakerShop.Marketing.Coupon)objDataObject.UpdateToAC6Object(objClientApiCoupon, objCoupon);
                                updated++;
                            }
                            else
                            {
                                continue;
                            }

                        }
                        else if (ImportOption == ImportOptions.ImportOrUpdate)
                        {
                            objCoupon = CouponDataSource.Load(objClientApiCoupon.CouponId);
                            if (objCoupon != null && objCoupon.Name.Equals(objClientApiCoupon.Name))
                            {
                                objCoupon = (MakerShop.Marketing.Coupon)objDataObject.UpdateToAC6Object(objClientApiCoupon, objCoupon);
                                updated++;
                            }
                            else
                            {
                                objCoupon = (MakerShop.Marketing.Coupon)objDataObject.ConvertToAC6Object(objClientApiCoupon);
                                objCoupon.CouponId = 0;
                                imported++;
                            }
                        }

                        if (!objCoupon.Save().Equals(SaveResult.Failed))
                        {
                            if (!TranslationDic.ContainsKey("COUPON" + objClientApiCoupon.CouponId))
                            {
                                AddToTranslationDic("COUPON" + objClientApiCoupon.CouponId, objCoupon.CouponId);
                            }
                        }
                        else
                        {
                            sWarnings.Append("Coupon Not Saved\n");
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for Coupon");
                        
                        strLog.AppendLine("Error message: " + ex.Message);                        
                        log(strLog.ToString());
                    }
                }
                log(imported + " Coupon imported, " + updated + " Coupon updated.");
                log("Coupon Import Complete...");
            }
            //else
            //{
            //    log("No Coupon available to Import...");
            //}
        }

        private void ImportEmailLists(MakerShop.DataClient.Api.Schema.EmailList[] arrEmailList)
        {
            if (arrEmailList != null && arrEmailList.Length > 0)
            {
                int imported = 0;
                int updated = 0;
                log("Importing " + arrEmailList.Length + " EmailList .");
                
                DataObject objDataObject = new DataObject("EmailList", typeof(MakerShop.Marketing.EmailList), typeof(Api.Schema.EmailList));
                MakerShop.Marketing.EmailList objEmailList = null;
                Api.Schema.EmailList logObject = null;
                foreach (MakerShop.DataClient.Api.Schema.EmailList objClientApiEmailList in arrEmailList)
                {
                    try
                    {
                        logObject = objClientApiEmailList;
                        int emailListId = objClientApiEmailList.EmailListId;
                        if (ImportOption == ImportOptions.Import)
                        {
                            objEmailList = EmailListDataSource.Load(objClientApiEmailList.EmailListId);
                            if (objEmailList != null && objEmailList.Name.Equals(objClientApiEmailList.Name))
                            {
                                log("EmailList already exist");
                                continue;
                            }
                            objEmailList = (MakerShop.Marketing.EmailList)objDataObject.ConvertToAC6Object(objClientApiEmailList);
                            if (objEmailList == null)
                            {
                                log("EmailList not saved");
                                continue;
                            }
                            objEmailList.EmailListId = 0;
                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            objEmailList = EmailListDataSource.Load(objClientApiEmailList.EmailListId);
                            if (objEmailList != null)
                            {
                                objEmailList = (MakerShop.Marketing.EmailList)objDataObject.UpdateToAC6Object(objClientApiEmailList, objEmailList);
                                updated++;
                            }
                            else
                            {
                                continue;
                            }

                        }
                        else if (ImportOption == ImportOptions.ImportOrUpdate)
                        {
                            objEmailList = EmailListDataSource.Load(objClientApiEmailList.EmailListId);
                            if (objEmailList != null)
                            {
                                objEmailList = (MakerShop.Marketing.EmailList)objDataObject.UpdateToAC6Object(objClientApiEmailList, objEmailList);
                                updated++;
                            }
                            else
                            {
                                objEmailList = (MakerShop.Marketing.EmailList)objDataObject.ConvertToAC6Object(objClientApiEmailList);
                                objEmailList.EmailListId = 0;
                                imported++;
                            }
                        }
                        objEmailList.StoreId = Token.Instance.StoreId;
                        if (!objEmailList.Save().Equals(SaveResult.Failed))
                        {
                            if (!TranslationDic.ContainsKey("EMAILLIST" + objClientApiEmailList.EmailListId))
                            {
                                AddToTranslationDic("EMAILLIST" + objClientApiEmailList.EmailListId, objEmailList.EmailListId);
                            }
                        }
                        else
                        {
                            sWarnings.Append("EmailList Not Saved\n");
                            continue;
                        }

                        if (objClientApiEmailList.Users != null && objClientApiEmailList.Users.Length > 0)
                        {
                            DataObject dataObj = new DataObject("EmailListUser", typeof(MakerShop.Marketing.EmailListUser), typeof(Api.Schema.EmailListUser));
                            EmailListUserCollection emailListUserCollection = (EmailListUserCollection)dataObj.ConvertToAC6Collection(objClientApiEmailList.Users, typeof(MakerShop.Marketing.EmailListUserCollection));
                            EmailListUserCollection objEmailListUserCollection = EmailListUserDataSource.LoadForEmailList(objEmailList.EmailListId);

                            for (int i = 0; i < emailListUserCollection.Count; i++)
                            {
                                // CHECK IF EMAIL LIST USER ALREADY EXISTS
                                // IF YES THEN WE HAVE TO SKIP IT
                                MakerShop.Marketing.EmailListUser newEmailListUser = emailListUserCollection[i];
                                newEmailListUser.EmailListId = objEmailList.EmailListId;
                                int index = objEmailListUserCollection.IndexOf(objEmailList.EmailListId, newEmailListUser.Email);
                                if (index > -1)
                                {
                                    // USER ALREADY EXIST                                    
                                    objEmailListUserCollection[index] = newEmailListUser;
                                }
                                else
                                {
                                    // USER DOES NOT EXISTS
                                    newEmailListUser.Save();
                                    objEmailListUserCollection.Add(newEmailListUser);
                                }
                            }
                                
                            objEmailListUserCollection.Save();
                            objEmailList.Users.Clear();
                            objEmailList.Users.AddRange(objEmailListUserCollection);
                            objEmailList.Users.Save();
                        }

                        if (objClientApiEmailList.Signups != null && objClientApiEmailList.Signups.Length > 0)
                        {
                            DataObject dataObj = new DataObject("EmailListSignup", typeof(MakerShop.Marketing.EmailListSignup), typeof(Api.Schema.EmailListSignup));
                            EmailListSignupCollection emailListSignupCollection = (EmailListSignupCollection)dataObj.ConvertToAC6Collection(objClientApiEmailList.Signups, typeof(MakerShop.Marketing.EmailListSignupCollection));
                            EmailListSignupCollection objEmailListSignupCollection = EmailListSignupDataSource.LoadForEmailList(objEmailList.EmailListId);
                            for (int i = 0; i < emailListSignupCollection.Count; i++)
                            {
                                // CHECK IF EMAIL LIST SIGNUP ALREADY EXISTS
                                // IF YES THEN WE HAVE TO SKIP IT     
                                MakerShop.Marketing.EmailListSignup newEmailListSignup = emailListSignupCollection[i];
                                newEmailListSignup.EmailListId= objEmailList.EmailListId;

                                MakerShop.Marketing.EmailListSignup existingEmailListSignup = EmailListSignupDataSource.Load(objEmailList.EmailListId, newEmailListSignup.Email);
                                if (existingEmailListSignup == null)
                                {
                                    // DOES NOT EXIST
                                    newEmailListSignup.Save();
                                    objEmailListSignupCollection.Add(newEmailListSignup);
                                }
                                else
                                {
                                    // ALREADY EXIST
                                    int existingEmailListSignupId = existingEmailListSignup.EmailListSignupId;
                                    DataObject dataObject = new DataObject(typeof(MakerShop.Marketing.EmailListSignup), typeof(MakerShop.DataClient.Api.Schema.EmailListSignup));
                                    dataObject.UpdateToAC6Object(newEmailListSignup, existingEmailListSignup);
                                    existingEmailListSignup.EmailListSignupId = existingEmailListSignupId; // RESTORE THE EXISTING ID                                    
                                    existingEmailListSignup.Save();
                                }                                
                            }
                            objEmailListSignupCollection.Save();
                            objEmailList.Signups.Clear();
                            objEmailList.Signups.AddRange(objEmailListSignupCollection);
                            objEmailList.Signups.Save();
                        }

                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for EmailList");
                        
                        strLog.AppendLine("Error message: " + ex.Message);                        
                        log(strLog.ToString());
                    }
                }
                log(imported + " EmailList imported, " + updated + " EmailList updated.");
                log("EmailList Import Complete...");
            }
            //else
            //{
            //    log("No EmailList available to Import...");
            //}
        }

        


        private void ImportGiftCertificates(MakerShop.DataClient.Api.Schema.GiftCertificate[] arrGiftCertificate)
        {
            if (arrGiftCertificate != null && arrGiftCertificate.Length > 0)
            {
                int imported = 0;
                int updated = 0;
                log("Importing " + arrGiftCertificate.Length + " GiftCertificate .");
                
                DataObject objDataObject = new DataObject("GiftCertificate", typeof(MakerShop.Payments.GiftCertificate), typeof(Api.Schema.GiftCertificate));
                MakerShop.Payments.GiftCertificate objGiftCertificate = null;
                Api.Schema.GiftCertificate logObject = null;
                foreach (MakerShop.DataClient.Api.Schema.GiftCertificate objClientApiGiftCertificate in arrGiftCertificate)
                {
                    try
                    {
                        logObject = objClientApiGiftCertificate;
                        int giftCertificateId = objClientApiGiftCertificate.GiftCertificateId;
                        if (ImportOption == ImportOptions.Import)
                        {
                            //objVolumeDiscount = VolumeDiscountDataSource.Load(objClientApiVolumeDiscount.VolumeDiscountId);
                            //if (objVolumeDiscount != null && objVolumeDiscount.Name.Equals(objClientApiVolumeDiscount.Name))
                            //{
                            //    log("EmailList already exist");
                            //    continue;
                            //}
                            objGiftCertificate = (MakerShop.Payments.GiftCertificate)objDataObject.ConvertToAC6Object(objClientApiGiftCertificate);
                            if (objGiftCertificate == null)
                            {
                                log("GiftCertificate not saved");
                                continue;
                            }
                            objGiftCertificate.GiftCertificateId = 0;
                            imported++;
                        }
                        else if (ImportOption == ImportOptions.Update)
                        {
                            objGiftCertificate = GiftCertificateDataSource.Load(objClientApiGiftCertificate.GiftCertificateId);
                            if (objGiftCertificate != null && objClientApiGiftCertificate.Name.Equals(objGiftCertificate.Name))
                            {
                                objGiftCertificate = (MakerShop.Payments.GiftCertificate)objDataObject.UpdateToAC6Object(objClientApiGiftCertificate, objGiftCertificate);
                                updated++;
                            }
                            else
                            {
                                continue;
                            }

                        }
                        else if (ImportOption == ImportOptions.ImportOrUpdate)
                        {
                            objGiftCertificate = GiftCertificateDataSource.Load(objClientApiGiftCertificate.GiftCertificateId);
                            if (objGiftCertificate != null && objClientApiGiftCertificate.Name.Equals(objGiftCertificate.Name))
                            {
                                objGiftCertificate = (MakerShop.Payments.GiftCertificate)objDataObject.UpdateToAC6Object(objClientApiGiftCertificate, objGiftCertificate);
                                updated++;
                            }
                            else
                            {
                                objGiftCertificate = (MakerShop.Payments.GiftCertificate)objDataObject.ConvertToAC6Object(objClientApiGiftCertificate);
                                objGiftCertificate.GiftCertificateId = 0;
                                imported++;
                            }
                        }
                        objGiftCertificate.StoreId = Token.Instance.StoreId;
                        int orderItemId = 0;
                        if (TranslationDic.ContainsKey("ORDERITEM" + objGiftCertificate.OrderItemId))
                        {
                            orderItemId = TranslationDic["ORDERITEM" + objGiftCertificate.OrderItemId];
                        }
                        objGiftCertificate.OrderItemId = orderItemId;

                        if (!objGiftCertificate.Save().Equals(SaveResult.Failed))
                        {
                            if (!TranslationDic.ContainsKey("GIFTCERTIFICATE" + objClientApiGiftCertificate.GiftCertificateId))
                            {
                                AddToTranslationDic("GIFTCERTIFICATE" + objClientApiGiftCertificate.GiftCertificateId, objGiftCertificate.GiftCertificateId);
                            }
                        }
                        else
                        {
                            sWarnings.Append("GiftCertificate Not Saved\n");
                            continue;
                        }

                        if (objClientApiGiftCertificate.Transactions != null && objClientApiGiftCertificate.Transactions.Length > 0)
                        {
                            DataObject dataObj = new DataObject("GiftCertificateTransaction", typeof(MakerShop.Payments.GiftCertificateTransaction), typeof(Api.Schema.GiftCertificateTransaction));
                            GiftCertificateTransactionCollection giftCertificateTransactionCollection = (GiftCertificateTransactionCollection)dataObj.ConvertToAC6Collection(objClientApiGiftCertificate.Transactions, typeof(MakerShop.Payments.GiftCertificateTransactionCollection));
                            GiftCertificateTransactionCollection objGiftCertificateTransactionCollection = GiftCertificateTransactionDataSource.LoadForGiftCertificate(objGiftCertificate.GiftCertificateId);
                            for (int i = 0; i < giftCertificateTransactionCollection.Count; i++)
                            {
                                MakerShop.Payments.GiftCertificateTransaction newGiftCertificateTransaction = giftCertificateTransactionCollection[i];
                                newGiftCertificateTransaction.GiftCertificateId = objGiftCertificate.GiftCertificateId;
                                int newOrderId = 0;
                                if (TranslationDic.ContainsKey("ORDER" + newGiftCertificateTransaction.OrderId))
                                {
                                    newOrderId = TranslationDic["ORDER" + newGiftCertificateTransaction.OrderId];
                                }
                                newGiftCertificateTransaction.OrderId = newOrderId;
                                int index = objGiftCertificateTransactionCollection.IndexOf(newGiftCertificateTransaction.GiftCertificateTransactionId);
                                if (index > -1)
                                {
                                    objGiftCertificateTransactionCollection[index] = newGiftCertificateTransaction;
                                }
                                else
                                {
                                    newGiftCertificateTransaction.GiftCertificateTransactionId = 0;
                                    newGiftCertificateTransaction.Save();
                                    objGiftCertificateTransactionCollection.Add(newGiftCertificateTransaction);
                                }
                            }
                            objGiftCertificateTransactionCollection.Save();
                            objGiftCertificate.Transactions.Clear();
                            objGiftCertificate.Transactions.AddRange(objGiftCertificateTransactionCollection);
                            objGiftCertificate.Transactions.Save();
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder strLog = new StringBuilder();
                        strLog.Append(ImportOption.ToString() + " unsuccessful for GiftCertificate");
                        
                        strLog.AppendLine("Error message: " + ex.Message);                        
                        log(strLog.ToString());
                    }
                }
                log(imported + " GiftCertificate imported, " + updated + " GiftCertificate updated.");
                log("GiftCertificate Import Complete...");
            }
            //else
            //{
            //    log("No GiftCertificate available to Import...");
            //}
        }


        #region Private Methods to Initialize Dictionaries and to get Values from Them

        private byte GetShippingId(string Shipping)
        {
            byte ShipId = 0;//Default            
            foreach (String ShipName in Enum.GetNames(typeof(Shippable)))
            {
                if (Shipping.Equals(ShipName))
                {
                    ShipId = (byte)((Shippable)Enum.Parse(typeof(Shippable), Shipping));
                }
            }            
            return ShipId;
        }
       
        private void InitProductTemplateDictionary()
        {
            if (dicProdcutTemplate == null)
            {
                dicProdcutTemplate = new SortedDictionary<string, int>();
                MakerShop.Products.ProductTemplateCollection objProductTemplateCollection = MakerShop.Products.ProductTemplateDataSource.LoadForStore();
                foreach (MakerShop.Products.ProductTemplate objTemplate in objProductTemplateCollection)
                {
                    if (!dicProdcutTemplate.ContainsKey(objTemplate.Name)) dicProdcutTemplate.Add(objTemplate.Name, objTemplate.ProductTemplateId);
                }
            }
        }
        
        private int GetProductTemplateId(string p)
        {
            int productTemplateId = 0;
            if (dicProdcutTemplate == null) InitProductTemplateDictionary();
            if (dicProdcutTemplate.ContainsKey(p))
            {
               productTemplateId = dicProdcutTemplate[p];
            }
            else
            {
                MakerShop.Products.ProductTemplate productTemplate = new MakerShop.Products.ProductTemplate();
                productTemplate.Name = p;
                if (!productTemplate.Save().Equals(SaveResult.Failed))
                {
                    dicProdcutTemplate.Add(p, productTemplate.ProductTemplateId);
                    productTemplateId = productTemplate.ProductTemplateId;
                }
            }
            return productTemplateId;
        }

        private void InitWrapGroupDictionary()
        {
            if (dicWrapGroup == null)
            {
                dicWrapGroup = new SortedDictionary<string, int>();
                MakerShop.Products.WrapGroupCollection objWrapGroupCollection = MakerShop.Products.WrapGroupDataSource.LoadForStore();
                foreach (MakerShop.Products.WrapGroup objWrapGroup in objWrapGroupCollection)
                {
                    if (!dicWrapGroup.ContainsKey(objWrapGroup.Name)) dicWrapGroup.Add(objWrapGroup.Name, objWrapGroup.WrapGroupId);
                }
            }
        }

        private int GetWrapGroupId(string p)
        {
            int wrapGroupId = 0;
            if (dicWrapGroup == null) InitWrapGroupDictionary();
            if (dicWrapGroup.ContainsKey(p))
            {
                wrapGroupId = dicWrapGroup[p];
            }
            else
            {
                MakerShop.Products.WrapGroup wrapGroup = new MakerShop.Products.WrapGroup();
                wrapGroup.Name = p;
                if (!wrapGroup.Save().Equals(SaveResult.Failed))
                {
                    dicWrapGroup.Add(p, wrapGroup.WrapGroupId);
                    wrapGroupId = wrapGroup.WrapGroupId;
                }
            }
            return wrapGroupId;
        }

        private bool AddToTranslationDic(String key, int value ){
            String key2 = key.ToUpperInvariant();
            if (!TranslationDic.ContainsKey(key2))
            {
                TranslationDic.Add(key2, value);
                return true;
            }
            // ALREADY CONTAINS THE SAME KEY
            return false;
        }
        
        private void InitVendorDictionary()
        {
            if (dicVendor == null)
            {
                dicVendor = new SortedDictionary<string, int>();
                MakerShop.Products.VendorCollection objVendorCollection = MakerShop.Products.VendorDataSource.LoadForStore();
                foreach (MakerShop.Products.Vendor objVendor in objVendorCollection)
                {
                    if(!dicVendor.ContainsKey(objVendor.Name))
                        dicVendor.Add(objVendor.Name, objVendor.VendorId);
                }
            }
        }
       
        private int GetVendorId(String VendorName)
        {
            int VendorId = 0;
            if (dicVendor == null) InitVendorDictionary();
            if (dicVendor.ContainsKey(VendorName))
            {
                VendorId = dicVendor[VendorName];
            }
            else
            {
                MakerShop.Products.Vendor vendor = new MakerShop.Products.Vendor();
                vendor.Name = VendorName;
                if (!vendor.Save().Equals(SaveResult.Failed))
                {
                    dicVendor.Add(VendorName, vendor.VendorId);
                    VendorId = vendor.VendorId;
                }
            }
            return VendorId;
        }

        private void InitTaxCodeDictionary()
        {
            if (dicTaxCode == null)
            {
                dicTaxCode = new SortedDictionary<string, int>();
                MakerShop.Taxes.TaxCodeCollection objTaxCodeCollection = MakerShop.Taxes.TaxCodeDataSource.LoadForStore();
                if (objTaxCodeCollection != null && objTaxCodeCollection.Count > 0)
                {
                    foreach (MakerShop.Taxes.TaxCode objTaxCode in objTaxCodeCollection)
                    {
                        if(!dicTaxCode.ContainsKey(objTaxCode.Name)) dicTaxCode.Add(objTaxCode.Name, objTaxCode.TaxCodeId);
                    }
                }
            }
        }
       
        private int GetTaxCodeId(String TaxCodeName)
        {
            int TaxCodeId = 0;
            if (dicTaxCode == null) InitTaxCodeDictionary();
            if (dicTaxCode.ContainsKey(TaxCodeName))
            {
                TaxCodeId = dicTaxCode[TaxCodeName];
            }
            else
            {
                MakerShop.Taxes.TaxCode taxCode = new MakerShop.Taxes.TaxCode();
                taxCode.Name = TaxCodeName;
                if (!taxCode.Save().Equals(SaveResult.Failed))
                {
                    dicTaxCode.Add(TaxCodeName, taxCode.TaxCodeId);
                    TaxCodeId = taxCode.TaxCodeId;
                }
            }
            return TaxCodeId;
        }

        private void InitOrderStausDictionary()
        {
            if (dicOrderStatus == null)
            {
                dicOrderStatus = new SortedDictionary<string, int>();
                MakerShop.Orders.OrderStatusCollection objOrderStatusCollection = MakerShop.Orders.OrderStatusDataSource.LoadForStore();
                if (objOrderStatusCollection != null && objOrderStatusCollection.Count > 0)
                {
                    foreach (MakerShop.Orders.OrderStatus objOrderStatus in objOrderStatusCollection)
                    {                        
                        if (!dicOrderStatus.ContainsKey(objOrderStatus.Name))
                        {
                            dicOrderStatus.Add(objOrderStatus.Name, objOrderStatus.OrderStatusId);
                        }
                    }
                }
            }
        }
       
        private int GetOrderStatusId(String OrderStatusName)
        {
            int OrderStatusId = 0;
            if (dicOrderStatus == null) InitOrderStausDictionary();
            if (dicOrderStatus.ContainsKey(OrderStatusName))
            {
                OrderStatusId = dicOrderStatus[OrderStatusName];
            }
            else
            {
                MakerShop.Orders.OrderStatus orderStatus = new MakerShop.Orders.OrderStatus();
                orderStatus.Name = OrderStatusName;
                if (!orderStatus.Save().Equals(SaveResult.Failed))
                {
                    dicOrderStatus.Add(OrderStatusName, orderStatus.OrderStatusId);
                    OrderStatusId = orderStatus.OrderStatusId;
                }
            }
            //Utility.LogDebug("Found Name: " + OrderStatusName + " , Id=" + OrderStatusId);
            return OrderStatusId;
        }

        private void InitManufacturerDictionary()
        {
            if (dicManufacturer == null)
            {
                dicManufacturer = new SortedDictionary<string, int>();
                MakerShop.Products.ManufacturerCollection objManufacturerCollection = MakerShop.Products.ManufacturerDataSource.LoadForStore();
                if (objManufacturerCollection != null && objManufacturerCollection.Count > 0)
                {
                    foreach (MakerShop.Products.Manufacturer objManufacturer in objManufacturerCollection)
                    {
                        if (!dicManufacturer.ContainsKey(objManufacturer.Name))
                        {
                            dicManufacturer.Add(objManufacturer.Name, objManufacturer.ManufacturerId);
                        }
                    }
                }
            }
        }
     
        private int GetManufacturerId(String ManufacturerName)
        {
            int ManufacturerId = 0;
            if (dicManufacturer == null) InitManufacturerDictionary();
            if (dicManufacturer.ContainsKey(ManufacturerName))
            {
                ManufacturerId = dicManufacturer[ManufacturerName];
            }
            else
            {
                MakerShop.Products.Manufacturer manufacturer = new MakerShop.Products.Manufacturer();
                manufacturer.Name = ManufacturerName;
                if (!manufacturer.Save().Equals(SaveResult.Failed))
                {
                    if (!dicManufacturer.ContainsKey(ManufacturerName))
                    {
                        dicManufacturer.Add(ManufacturerName, manufacturer.ManufacturerId);
                    }
                    ManufacturerId = manufacturer.ManufacturerId;
                }
            }
            return ManufacturerId;
        }

        private int GetCategoryId(String CategoryPath)
        {
            if (String.IsNullOrEmpty(CategoryPath))
            {
                return 0;
            }
            int categoryId = 0;
            if (dicCategory == null) dicCategory = new SortedDictionary<string, int>();
            if (dicCategory.ContainsKey(CategoryPath))
            {
                categoryId = dicCategory[CategoryPath];
            }
            else
            {
                MakerShop.Catalog.Category objCategory = CategoryDataSource.LoadForPath(CategoryPath, ":", true);
                if (objCategory != null)
                {
                    if (!dicCategory.ContainsKey(CategoryPath))
                    {
                        dicCategory.Add(CategoryPath, objCategory.CategoryId);
                    }
                    categoryId = objCategory.CategoryId;
                }
            }
            return categoryId;
        }

        private void InitWarehouseDictionary()
        {
            if (dicWarehouse == null)
            {
                dicWarehouse = new SortedDictionary<string, int>();
                MakerShop.Shipping.WarehouseCollection objWarehouseCollection = MakerShop.Shipping.WarehouseDataSource.LoadForStore();
                if (objWarehouseCollection != null && objWarehouseCollection.Count > 0)
                {
                    foreach (MakerShop.Shipping.Warehouse objWarehouse in objWarehouseCollection)
                    {
                        if (!dicWarehouse.ContainsKey(objWarehouse.Name)) 
                            dicWarehouse.Add(objWarehouse.Name, objWarehouse.WarehouseId);
                    }
                }
            }
        }
        
        private int GetWarehouseId(String WarehouseName)
        {
            int WarehouseId = 0;
            if (dicWarehouse == null) InitWarehouseDictionary();
            if (dicWarehouse.ContainsKey(WarehouseName))
            {
                WarehouseId = dicWarehouse[WarehouseName];
            }
            else
            {
                MakerShop.Shipping.Warehouse objWarehouse = new MakerShop.Shipping.Warehouse();
                objWarehouse.Name = WarehouseName;
                if (!objWarehouse.Save().Equals(SaveResult.Failed))
                {
                    dicWarehouse.Add(WarehouseName, objWarehouse.WarehouseId);
                    WarehouseId = objWarehouse.WarehouseId;
                }
            }
            return WarehouseId;
        }

        private void InitAffiliateDictionary()
        {
            if (dicAffiliate == null)
            {
                dicAffiliate = new SortedDictionary<string, int>();
                MakerShop.Marketing.AffiliateCollection objAffiliateCollection = MakerShop.Marketing.AffiliateDataSource.LoadForStore();
                if (objAffiliateCollection != null && objAffiliateCollection.Count > 0)
                {
                    foreach (MakerShop.Marketing.Affiliate objAffiliate in objAffiliateCollection)
                    {
                        if (!dicAffiliate.ContainsKey(objAffiliate.Name))
                        {
                            dicAffiliate.Add(objAffiliate.Name, objAffiliate.AffiliateId);
                        }
                    }
                }
            }
        }
       
        private int GetAffiliateId(String AffiliateName)
        {
            int AffiliateId = 0;
            if (dicAffiliate == null) InitAffiliateDictionary();
            if (dicAffiliate.ContainsKey(AffiliateName))
            {
                AffiliateId = dicAffiliate[AffiliateName];
            }
            else
            {
                MakerShop.Marketing.Affiliate affiliate = new MakerShop.Marketing.Affiliate();
                affiliate.Name = AffiliateName;
                if (!affiliate.Save().Equals(SaveResult.Failed))
                {
                    if (!dicAffiliate.ContainsKey(AffiliateName))
                    {
                        dicAffiliate.Add(AffiliateName, affiliate.AffiliateId);
                    }
                    AffiliateId = affiliate.AffiliateId;
                }
            }
            return AffiliateId;
        }

        private void InitUserDictionary()
        {
            if (dicUser == null)
            {
                dicUser = new SortedDictionary<string, int>();
                MakerShop.Users.UserCollection objUserCollection = MakerShop.Users.UserDataSource.LoadForStore();
                if (objUserCollection != null && objUserCollection.Count > 0)
                {
                    foreach (MakerShop.Users.User objUser in objUserCollection)
                    {
                        if (!dicUser.ContainsKey(objUser.UserName))
                        {
                            dicUser.Add(objUser.UserName, objUser.UserId);
                        }
                    }
                }
            }
        }
        
        private int GetUserId(String UserName)
        {
            int UserId = 0;
            if (dicUser == null) InitUserDictionary();
            if (dicUser.ContainsKey(UserName))
            {
                UserId = dicUser[UserName];
            }
            else
            {
                MakerShop.Users.User objUser = new MakerShop.Users.User();
                objUser.UserName = UserName;
                if (!objUser.Save().Equals(SaveResult.Failed))
                {
                    if (!dicUser.ContainsKey(UserName))
                    {
                        dicUser.Add(UserName, objUser.UserId);
                    }
                    UserId = objUser.UserId;
                }
            }
            return UserId;
        }

       
        #endregion

        private void logError(String message)
        {
            log("\n" + message);
            Utility.LogDebug("Error while importing:" + message);
        }

        private void log(String message)
        {
            sLog.Append("\n" + message);
        }

        internal static byte[] ImportAC5xData(AC5xImportRequest ac5xImportRequest)
        {
            if (ac5xImportRequest.AC55Data != null)
            {
                String fileXml = String.Empty;
                try
                {
                    XmlDocument importDocument = new XmlDocument();
                    
                    //if (importRequest.InChunks) { fileXml = EncodeHelper.Utf8BytesToString(importRequest.AC55Data); }
                    //else 
                        fileXml = ZipUtility.DecompressToString(ac5xImportRequest.AC55Data);

                    importDocument.LoadXml(fileXml);
                    Ac55Importer ac55Importer = new Ac55Importer();
                    ac55Importer.PreserveIdOption = (PreserveIdOption)ac5xImportRequest.PreserveIdOption;
                    ac55Importer.TimeZoneOffset = (double)ac5xImportRequest.TimeZoneOffset;
                    ac55Importer.UpdateTimeZone = ac5xImportRequest.UpdateTimeZone;
                    
                    // IF IN CHUNKS THEN TRY LOADING TRANSLATION DICTIONARY FROM CACHE
                    if (ac5xImportRequest.InChunks && ac5xImportRequest.IDMappingData != null && ac5xImportRequest.IDMappingData.Length > 0)
                    {
                        StringDictionary translationDic = new StringDictionary();
                        String mappingData = ZipUtility.DecompressToString(ac5xImportRequest.IDMappingData);
                        if (!String.IsNullOrEmpty(mappingData))
                        {
                            String[] objectsIDMappingData = mappingData.Split(';');

                            int index;
                            string objectName;
                            string objectIDMappingData2;
                            string[] objectsIDData;
                            string[] objectIDs;

                            foreach (String objectIDMappingData in objectsIDMappingData)
                            {
                                index = objectIDMappingData.IndexOf('{');
                                objectName = objectIDMappingData.Substring(0, index);
                                objectIDMappingData2 = objectIDMappingData.Substring(index + 1, objectIDMappingData.Length - objectName.Length - 2);
                                objectsIDData = objectIDMappingData2.Split(',');
                                foreach (String objectIDData in objectsIDData)
                                {
                                    objectIDs = objectIDData.Split(':');
                                    translationDic.Add(objectName + objectIDs[0], objectIDs[1]);
                                }
                            }
                        }
                        
                        if (translationDic != null)
                        {
                            ac55Importer.TranslationDictionary = translationDic;
                        }
                    }


                    ac55Importer.import(importDocument);

                    ACImportResponse objACImportResponse = new ACImportResponse();
                    objACImportResponse.ImportResponse = new ImportResponse();

                    // CACHE THE TRANSLATION DICTIONARY
                    if (ac5xImportRequest.InChunks)
                    {
                        SortedDictionary<String, List<string>> objectsMapping = new SortedDictionary<string, List<string>>();
                        char[] numbersArray = new char[]{'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
                        int index;
                        string objectName;
                        string objectOldID;
                        foreach(String key in ac55Importer.TranslationDictionary.Keys)
                        {
                            index = key.IndexOfAny(numbersArray);
                            objectName = key.Substring(0,index);
                            objectOldID = key.Substring(index);
                            if (!objectsMapping.ContainsKey(objectName)) objectsMapping.Add(objectName, new List<string>());
                            objectsMapping[objectName].Add(objectOldID + ":" + ac55Importer.TranslationDictionary[key]);
                        }

                        StringBuilder mappingDataBuilder = new StringBuilder();
                        string seperator = ",";
                        foreach (String key in objectsMapping.Keys)
                        {
                            mappingDataBuilder.Append(key).Append("{").Append(String.Join(seperator, objectsMapping[key].ToArray())).Append("}").Append(";");
                        }
                        objACImportResponse.ImportResponse.IDMappingData = ZipUtility.CompressStringData(Utility.RemoveEndSeparator(mappingDataBuilder.ToString(), ";"));
                    }

                    // SEND IMPORT LOG WITH EACH RESPONSE
                    objACImportResponse.ImportResponse.Log = new Log();

                    objACImportResponse.ImportResponse.Log.Message = ac55Importer.Status.LogMessage.ToString();
                    objACImportResponse.ImportResponse.Log.WarningMessages = ac55Importer.Status.LogWarning.ToString();
                    objACImportResponse.ImportResponse.Log.ErrorMessages = ac55Importer.Status.LogError.ToString();
                    
                    objACImportResponse.ImportResponse.ResponseId = ac5xImportRequest.RequestId;
                    byte[] byteRequestXml = EncodeHelper.Serialize(objACImportResponse);
                    return byteRequestXml;
                }
                catch (Exception ex)
                {
                    Utility.LogDebug("An error has occured while importing AC5x data:" + ex.StackTrace);
                    ACImportResponse objACImportResponse = new ACImportResponse();
                    objACImportResponse.ImportResponse = new ImportResponse();

                    objACImportResponse.ImportResponse.Log = new Log();
                    objACImportResponse.ImportResponse.Log.Message = "An error has occured while importing AC5x data.";
                    objACImportResponse.ImportResponse.Log.ErrorMessages = "An error has occured while importing AC5x data." + ex.StackTrace + "\nMessage:" + ex.Message + "\n" + fileXml.Substring(0, 255) + "...";
                    objACImportResponse.ImportResponse.ResponseId = "importRequest.RequestId";
                    byte[] byteRequestXml = EncodeHelper.Serialize(objACImportResponse);
                    return byteRequestXml;
                }
            }
            else
            {
                ACImportResponse objACImportResponse = new ACImportResponse();
                objACImportResponse.ImportResponse = new ImportResponse();

                objACImportResponse.ImportResponse.Log = new Log();
                objACImportResponse.ImportResponse.Log.Error = new Error();
                objACImportResponse.ImportResponse.Log.Error.Message = "AC5x data was empty";
                objACImportResponse.ImportResponse.ResponseId = ac5xImportRequest.RequestId;
                byte[] byteRequestXml = EncodeHelper.Serialize(objACImportResponse);
                return byteRequestXml;
            }
        }
    }
}
