using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.DataClient.Api.Schema;
using System.Collections;
using MakerShop.Stores;
//using MakerShop.Data;
using MakerShop.Utility;
using MakerShop.Users;
using MakerShop.Marketing;
using MakerShop.Shipping;
using MakerShop.Products;
using MakerShop.Taxes;
using MakerShop.Orders;
using MakerShop.Common;
using System.Xml;
using System.IO;

namespace MakerShop.DataClient.Api.ObjectHandlers
{
    public partial class ExportHandler
    {
        String requestId;        
        MakerShop.Stores.Store ac6Store ;
        Api.Schema.Store clientStore;

        public Api.Schema.Store ClientStore
        {
          get { return clientStore; }
          set { clientStore = value; }
        }

        public String RequestId
        {
          get { return requestId; }
          set { requestId = value; }
        }
      
        public MakerShop.Stores.Store Ac6Store
        {
          get { return ac6Store; }
          set { ac6Store = value; }
        }

        

        /// <summary>
        /// will export all store and if parameter is true then
        /// it will export all store data but Ids of Products, Users and Orders for 
        /// chunks        
        /// </summary>
        /// <param name="strOptions">list of nodes which will be exported</param>
        /// <param name="chunkSize">represents count of obejcts which will be exported in one go (only valid for products, users and Orders)</param>
        /// <returns></returns>
        public byte[] DoStandardExport(ExportOptions exportOptions,int chunkSize)
        {            
            string strOptions = exportOptions.Data;
            DataObject dataObject;
            DataObject nestedDataObject;
            //int i = 0;

            List<String> lstOption = new List<string>();
            foreach (String option in strOptions.Split(','))
            {
                lstOption.Add(option);
            }

            ACExportResponse acExportResponse = new ACExportResponse();

            acExportResponse.ExportResponse = new ExportResponse();

            // Add basic export data
            MakerShopExport MakerShopExport = InitBasicStoreData();

            // Store Node            
            Ac6Store = Token.Instance.Store;
            // IF COMPLETE STORE IS REQUESTED
            if (strOptions.Length < Enum.GetValues(typeof(DataObject.Names)).Length)
            {
                ClientStore = StoreHandler.ConverToClient(Ac6Store, false);
            }
            else
            {
                ClientStore = StoreHandler.ConverToClient(Ac6Store, true);
            }

            // Affiliates
            if (lstOption.Contains(DataObject.Names.Affiliates.ToString()))
            {
                ClientStore.Affiliates = (Schema.Affiliate[])AffiliateHandler.ConvertToClientArray(Ac6Store.Affiliates);
            }

            //BannedIPs
            if (lstOption.Contains(DataObject.Names.BannedIPs.ToString())) 
                ClientStore.BannedIPs = (Api.Schema.BannedIP[])DataObject.ConvertToClientArray(typeof(MakerShop.Stores.BannedIP), typeof(Api.Schema.BannedIP), Ac6Store.BannedIPs);

            //Currencies
            if (lstOption.Contains(DataObject.Names.Currencies.ToString())) 
                ClientStore.Currencies = (Api.Schema.Currency[])DataObject.ConvertToClientArray(typeof(MakerShop.Stores.Currency), typeof(Api.Schema.Currency), Ac6Store.Currencies);

            //Readmes
            if (lstOption.Contains(DataObject.Names.Readmes.ToString()))
                ClientStore.Readmes = (Schema.Readme[])DataObject.ConvertToClientArray(typeof(MakerShop.DigitalDelivery.Readme), typeof(Schema.Readme), Ac6Store.Readmes);

            //LicenseAgreements
            if (lstOption.Contains(DataObject.Names.LicenseAgreements.ToString()))
                ClientStore.LicenseAgreements = (Schema.LicenseAgreement[])DataObject.ConvertToClientArray(typeof(MakerShop.DigitalDelivery.LicenseAgreement), typeof(Schema.LicenseAgreement), Ac6Store.LicenseAgreements);

            //ErrorMessages
            if (lstOption.Contains(DataObject.Names.ErrorMessages.ToString()))
                ClientStore.ErrorMessages = (Schema.ErrorMessage[])DataObject.ConvertToClientArray(typeof(MakerShop.Utility.ErrorMessage), typeof(Schema.ErrorMessage), Ac6Store.ErrorMessages);

            //StoreSettings
            if (lstOption.Contains(DataObject.Names.StoreSettings.ToString()))                
                    ClientStore.Settings = (Api.Schema.StoreSetting[])DataObject.ConvertToClientArray(typeof(MakerShop.Stores.StoreSetting), typeof(Api.Schema.StoreSetting),Ac6Store.Settings);

            //PaymentGateways
            if (lstOption.Contains(DataObject.Names.PaymentGateways.ToString()))
                ClientStore.PaymentGateways = (Schema.PaymentGateway[])DataObject.ConvertToClientArray(typeof(MakerShop.Payments.PaymentGateway), typeof(Schema.PaymentGateway), Ac6Store.PaymentGateways);

            //ShipGateways
            if (lstOption.Contains(DataObject.Names.ShipGateways.ToString()))
                ClientStore.ShipGateways = (Schema.ShipGateway[])DataObject.ConvertToClientArray(typeof(MakerShop.Shipping.ShipGateway), typeof(Schema.ShipGateway), Ac6Store.ShipGateways);

            //TaxGateways
            if (lstOption.Contains(DataObject.Names.TaxGateways.ToString()))
                ClientStore.TaxGateways = (Schema.TaxGateway[])DataObject.ConvertToClientArray(typeof(MakerShop.Taxes.TaxGateway), typeof(Schema.TaxGateway), Ac6Store.TaxGateways);

            // Menufacturers
            if (lstOption.Contains(DataObject.Names.Manufacturers.ToString()))
            {
                dataObject = new DataObject(typeof(MakerShop.Products.Manufacturer), typeof(Schema.Manufacturer));
                ClientStore.Manufacturers = (Schema.Manufacturer[])dataObject.ConvertAC6Collection(Ac6Store.Manufacturers);
            }

            // TaxCodes
            if (lstOption.Contains(DataObject.Names.TaxCodes.ToString()))
            {
                dataObject = new DataObject(typeof(MakerShop.Taxes.TaxCode), typeof(Schema.TaxCode));
                ClientStore.TaxCodes = (Schema.TaxCode[])dataObject.ConvertAC6Collection(Ac6Store.TaxCodes);
            }

            // ROLES            
            if (lstOption.Contains(DataObject.Names.Roles.ToString()))
            {
                RoleCollection acRoles = RoleDataSource.LoadAll();
                ClientStore.Roles = (Schema.Role[])DataObject.ConvertToClientArray(typeof(MakerShop.Users.Role), typeof(Schema.Role), acRoles);
            }

            // EmailTemplates
            if (lstOption.Contains(DataObject.Names.EmailTemplates.ToString()))
            {
                dataObject = new DataObject("EmailTemplate", typeof(MakerShop.Messaging.EmailTemplate), typeof(Schema.EmailTemplate));
                Schema.EmailTemplate[] arrEmailTemplate = new Schema.EmailTemplate[Ac6Store.EmailTemplates.Count];
                Schema.EmailTemplate objClientApiEmailTemplate = null;
                for (int i = 0; i < Ac6Store.EmailTemplates.Count; i++)
                {
                    MakerShop.Messaging.EmailTemplate objEmailTemplate = Ac6Store.EmailTemplates[i];
                    objClientApiEmailTemplate = (Schema.EmailTemplate)dataObject.ConvertToClientApiObject(objEmailTemplate);

                    // Triggers
                    nestedDataObject = new DataObject("EmailTemplateTrigger", typeof(MakerShop.Messaging.EmailTemplateTrigger), typeof(Schema.EmailTemplateTrigger));
                    objClientApiEmailTemplate.Triggers = (Schema.EmailTemplateTrigger[])nestedDataObject.ConvertAC6Collection(objEmailTemplate.Triggers);

                    arrEmailTemplate[i] = objClientApiEmailTemplate;
                }
                ClientStore.EmailTemplates = arrEmailTemplate;
            }

            //PersonalizationPaths
            if (lstOption.Contains(DataObject.Names.PersonalizationPaths.ToString()))
            {
                ClientStore.PersonalizationPaths = (Schema.PersonalizationPath[])DataObject.ConvertToClientArray(typeof(MakerShop.Personalization.PersonalizationPath), typeof(Schema.PersonalizationPath), Ac6Store.PersonalizationPathes);
                //SharedPersonalization
                for (int i_pp = 0; i_pp < ClientStore.PersonalizationPaths.Length; i_pp++)
                {
                    Schema.PersonalizationPath schemaObj = ClientStore.PersonalizationPaths[i_pp];
                    MakerShop.Personalization.PersonalizationPath acObject = Ac6Store.PersonalizationPathes[i_pp];

                    schemaObj.SharedPersonalization = (Schema.SharedPersonalization)DataObject.ConvertToClientObject(typeof(MakerShop.Personalization.SharedPersonalization), typeof(Schema.SharedPersonalization), acObject.SharedPersonalization);
                }
            }

            if (lstOption.Contains(DataObject.Names.Countries.ToString()))
            {
                //Countries
                dataObject = new DataObject("Country", typeof(MakerShop.Shipping.Country), typeof(Api.Schema.Country));
                Api.Schema.Country[] arrClientApiCountry = new Api.Schema.Country[Ac6Store.Countries.Count];
                Api.Schema.Country objClientApiCountry = null;

                for (int i = 0; i < Ac6Store.Countries.Count; i++)
                {
                    MakerShop.Shipping.Country objCountry = Ac6Store.Countries[i];
                    objClientApiCountry = (Api.Schema.Country)dataObject.ConvertToClientApiObject(objCountry);

                    //Provinces
                    nestedDataObject = new DataObject("Province", typeof(MakerShop.Shipping.Province), typeof(Api.Schema.Province));
                    objClientApiCountry.Provinces = (Api.Schema.Province[])nestedDataObject.ConvertAC6Collection(objCountry.Provinces);

                    arrClientApiCountry[i] = objClientApiCountry;
                }
                ClientStore.Countries = arrClientApiCountry;
            }

            // Groups
            if (lstOption.Contains(DataObject.Names.Groups.ToString()))
            {
                dataObject = new DataObject(typeof(MakerShop.Users.Group), typeof(Schema.Group));
                ClientStore.Groups = (Schema.Group[])dataObject.ConvertAC6Collection(Ac6Store.Groups);

                // GroupRoles
                for (int i = 0; i < clientStore.Groups.Length; i++)
                {
                    Schema.Group schemaObj = clientStore.Groups[i];
                    MakerShop.Users.Group acObject = Ac6Store.Groups[i];


                    DataObject subDataObject = new DataObject(typeof(MakerShop.Users.GroupRole), typeof(Schema.GroupRole));
                    schemaObj.GroupRoles = (Schema.GroupRole[])subDataObject.ConvertAC6Collection(acObject.GroupRoles);
                }
            }

            // Warehouses
            if (lstOption.Contains(DataObject.Names.Warehouses.ToString()))
            {
                dataObject = new DataObject(typeof(MakerShop.Shipping.Warehouse), typeof(Schema.Warehouse));
                ClientStore.Warehouses = (Schema.Warehouse[])dataObject.ConvertAC6Collection(Ac6Store.Warehouses);
            }

            // TaxRules
            if (lstOption.Contains(DataObject.Names.TaxRules.ToString()))
            {
                dataObject = new DataObject(typeof(MakerShop.Taxes.TaxRule), typeof(Schema.TaxRule));
                Api.Schema.TaxRule[] arrClientApiTaxRule = new Api.Schema.TaxRule[Ac6Store.TaxRules.Count];
                Api.Schema.TaxRule objClientApiTaxRule = null;

                for (int i = 0; i < Ac6Store.TaxRules.Count; i++)
                {
                    MakerShop.Taxes.TaxRule objTaxRule = Ac6Store.TaxRules[i];
                    objClientApiTaxRule = (Api.Schema.TaxRule)dataObject.ConvertToClientApiObject(objTaxRule);

                    // NESTED ASSOCIATIONS

                    //TaxRuleTaxCode
                    nestedDataObject = new DataObject("TaxRuleTaxCode", typeof(MakerShop.Taxes.TaxRuleTaxCode), typeof(Api.Schema.TaxRuleTaxCode));
                    objClientApiTaxRule.TaxRuleTaxCodes = (Api.Schema.TaxRuleTaxCode[])nestedDataObject.ConvertAC6Collection(objTaxRule.TaxRuleTaxCodes);

                    //TaxRuleGroup
                    nestedDataObject = new DataObject("TaxRuleGroup", typeof(MakerShop.Taxes.TaxRuleGroup), typeof(Api.Schema.TaxRuleGroup));
                    objClientApiTaxRule.TaxRuleGroups = (Api.Schema.TaxRuleGroup[])nestedDataObject.ConvertAC6Collection(objTaxRule.TaxRuleGroups);

                    //TaxRuleShipZone
                    nestedDataObject = new DataObject("TaxRuleShipZone", typeof(MakerShop.Taxes.TaxRuleShipZone), typeof(Api.Schema.TaxRuleShipZone));
                    objClientApiTaxRule.TaxRuleShipZones = (Api.Schema.TaxRuleShipZone[])nestedDataObject.ConvertAC6Collection(objTaxRule.TaxRuleShipZones);


                    arrClientApiTaxRule[i] = objClientApiTaxRule;
                }
                ClientStore.TaxRules = arrClientApiTaxRule;
            }
            
            // VolumeDiscounts
            if (lstOption.Contains(DataObject.Names.VolumeDiscounts.ToString()))
            {
                dataObject = new DataObject("VolumeDiscount", typeof(MakerShop.Marketing.VolumeDiscount), typeof(Schema.VolumeDiscount));
                Schema.VolumeDiscount[] arrVolumeDiscount = new Schema.VolumeDiscount[Ac6Store.VolumeDiscounts.Count];
                Schema.VolumeDiscount objClientApiVolumeDiscount = null;
                for (int i = 0; i < Ac6Store.VolumeDiscounts.Count; i++)
                {
                    MakerShop.Marketing.VolumeDiscount objVolumeDiscount = Ac6Store.VolumeDiscounts[i];
                    objClientApiVolumeDiscount = (Schema.VolumeDiscount)dataObject.ConvertToClientApiObject(objVolumeDiscount);

                    // Levels
                    nestedDataObject = new DataObject("VolumeDiscountLevel", typeof(MakerShop.Marketing.VolumeDiscountLevel), typeof(Schema.VolumeDiscountLevel));
                    objClientApiVolumeDiscount.Levels = (Schema.VolumeDiscountLevel[])nestedDataObject.ConvertAC6Collection(objVolumeDiscount.Levels);

                    //VolumeDiscountGroup
                    objClientApiVolumeDiscount.VolumeDiscountGroups = (Schema.VolumeDiscountGroup[])DataObject.ConvertToClientArray(typeof(MakerShop.Marketing.VolumeDiscountGroup), typeof(Schema.VolumeDiscountGroup), objVolumeDiscount.VolumeDiscountGroups);

                    arrVolumeDiscount[i] = objClientApiVolumeDiscount;
                }
                ClientStore.VolumeDiscounts = arrVolumeDiscount;
            }

            // Vendors
            if (lstOption.Contains(DataObject.Names.Vendors.ToString()))
            {
                dataObject = new DataObject(typeof(MakerShop.Products.Vendor), typeof(Schema.Vendor));
                ClientStore.Vendors = (Schema.Vendor[])dataObject.ConvertAC6Collection(Ac6Store.Vendors);

                //VendorGroups
                for (int i_vendor = 0; i_vendor < ClientStore.Vendors.Length; i_vendor++)
                {
                    Schema.Vendor schemaObj = ClientStore.Vendors[i_vendor];
                    MakerShop.Products.Vendor acObject = Ac6Store.Vendors[i_vendor];

                    schemaObj.VendorGroups = (Schema.VendorGroup[])DataObject.ConvertToClientArray(typeof(MakerShop.Products.VendorGroup), typeof(Schema.VendorGroup), acObject.VendorGroups);
                }


            }

            //WrapGroups
            if (lstOption.Contains(DataObject.Names.WrapGroups.ToString()))
            {
                dataObject = new DataObject("WrapGroup", typeof(MakerShop.Products.WrapGroup), typeof(Schema.WrapGroup));
                Schema.WrapGroup[] arrWrapGroup = new Schema.WrapGroup[Ac6Store.WrapGroups.Count];
                Schema.WrapGroup objClientApiWrapGroup = null;
                for (int i = 0; i < Ac6Store.WrapGroups.Count; i++)
                {

                    MakerShop.Products.WrapGroup objWrapGroup = Ac6Store.WrapGroups[i];
                    objClientApiWrapGroup = (Schema.WrapGroup)dataObject.ConvertToClientApiObject(objWrapGroup);

                    // WrapStyle
                    nestedDataObject = new DataObject("WrapStyle", typeof(MakerShop.Products.WrapStyle), typeof(Schema.WrapStyle));
                    objClientApiWrapGroup.WrapStyles = (Schema.WrapStyle[])nestedDataObject.ConvertAC6Collection(objWrapGroup.WrapStyles);

                    arrWrapGroup[i] = objClientApiWrapGroup;
                }
                ClientStore.WrapGroups = arrWrapGroup;
            }


            // ProductTemplets
            if (lstOption.Contains(DataObject.Names.ProductTemplates.ToString()))
            {
                dataObject = new DataObject("ProductTemplate", typeof(MakerShop.Products.ProductTemplate), typeof(Schema.ProductTemplate));
                Schema.ProductTemplate[] arrProductTemplate = new Schema.ProductTemplate[Ac6Store.ProductTemplates.Count];
                Schema.ProductTemplate objClientApiProductTemplate = null;
                for (int i = 0; i < Ac6Store.ProductTemplates.Count; i++)
                {
                    MakerShop.Products.ProductTemplate objProductTemplate = Ac6Store.ProductTemplates[i];
                    objClientApiProductTemplate = (Schema.ProductTemplate)dataObject.ConvertToClientApiObject(objProductTemplate);

                    // Input Field
                    nestedDataObject = new DataObject(typeof(MakerShop.Products.InputField), typeof(Schema.InputField));
                    objClientApiProductTemplate.InputFields = (Schema.InputField[])nestedDataObject.ConvertAC6Collection(objProductTemplate.InputFields);

                    // InputChoices
                    for (int i_ic = 0; i_ic < objClientApiProductTemplate.InputFields.Length; i_ic++)
                    {
                        Schema.InputField schemaObj = objClientApiProductTemplate.InputFields[i_ic];
                        MakerShop.Products.InputField acObject = objProductTemplate.InputFields[i_ic];

                        DataObject subDataObject = new DataObject(typeof(MakerShop.Products.InputChoice), typeof(Schema.InputChoice));
                        schemaObj.InputChoices = (Schema.InputChoice[])subDataObject.ConvertAC6Collection(acObject.InputChoices);
                    }

                    arrProductTemplate[i] = objClientApiProductTemplate;
                }
                ClientStore.ProductTemplates = arrProductTemplate;
            }

            //Categories
            if (lstOption.Contains(DataObject.Names.Categories.ToString()))
            {
                ClientStore.Categories = (Schema.Category[])CategoryHandler.ConvertToClientArray(Ac6Store.Categories);
            }


            // Links
            if (lstOption.Contains(DataObject.Names.Links.ToString()))
            {
                dataObject = new DataObject("Link", typeof(MakerShop.Catalog.Link), typeof(Schema.Link));
                Schema.Link[] arrLink = new Link[Ac6Store.Links.Count];
                Schema.Link objClientApiLink = null;
                for (int i = 0; i < Ac6Store.Links.Count; i++)
                {
                    MakerShop.Catalog.Link objLink = Ac6Store.Links[i];
                    objClientApiLink = (Schema.Link)dataObject.ConvertToClientApiObject(objLink);
                    String strId = String.Empty;
                    foreach (int id in objLink.Categories)
                    {
                        strId += id + ",";
                    }
                    objClientApiLink.Categories = Utility.RemoveEndSeparator(strId, ",");

                    arrLink[i] = objClientApiLink;
                }
                ClientStore.Links = arrLink;
            }

            // Webpages
            if (lstOption.Contains(DataObject.Names.Webpages.ToString()))
            {
                dataObject = new DataObject("Webpage", typeof(MakerShop.Catalog.Webpage), typeof(Schema.Webpage));
                Schema.Webpage[] arrWebpage = new Webpage[Ac6Store.Webpages.Count];
                Schema.Webpage objClientApiWebpage = null;
                for (int i = 0; i < Ac6Store.Webpages.Count; i++)
                {
                    MakerShop.Catalog.Webpage objWebpage = Ac6Store.Webpages[i];
                    objClientApiWebpage = (Schema.Webpage)dataObject.ConvertToClientApiObject(objWebpage);
                    String strId = String.Empty;
                    foreach (int id in objWebpage.Categories)
                    {
                        strId += id + ",";
                    }
                    objClientApiWebpage.Categories = Utility.RemoveEndSeparator(strId, ",");

                    arrWebpage[i] = objClientApiWebpage;
                }
                ClientStore.Webpages = arrWebpage;
            }


            // PaymentMethods
            if (lstOption.Contains(DataObject.Names.PaymentMethods.ToString()))
            {
                dataObject = new DataObject("PaymentMethod", typeof(MakerShop.Payments.PaymentMethod), typeof(Schema.PaymentMethod));
                Schema.PaymentMethod[] arrPaymentMethod = new Schema.PaymentMethod[Ac6Store.PaymentMethods.Count];
                Schema.PaymentMethod objClientApiPaymentMethod = null;
                for (int i = 0; i < Ac6Store.PaymentMethods.Count; i++)
                {
                    MakerShop.Payments.PaymentMethod objPaymentMethod = Ac6Store.PaymentMethods[i];
                    objClientApiPaymentMethod = (Schema.PaymentMethod)dataObject.ConvertToClientApiObject(objPaymentMethod);

                    objClientApiPaymentMethod.PaymentMethodGroups = (Schema.PaymentMethodGroup[])DataObject.ConvertToClientArray(typeof(MakerShop.Payments.PaymentMethodGroup), typeof(Schema.PaymentMethodGroup), objPaymentMethod.PaymentMethodGroups);

                    nestedDataObject = new DataObject("Payment", typeof(MakerShop.Payments.Payment), typeof(Schema.Payment));
                    Schema.Payment[] arrClientApiPayments = new Schema.Payment[objPaymentMethod.Payments.Count];
                    Schema.Payment objClientApiPayment = null;
                    for (int j = 0; j < objPaymentMethod.Payments.Count; j++)
                    {
                        MakerShop.Payments.Payment objPayment = objPaymentMethod.Payments[j];
                        objClientApiPayment = (Schema.Payment)nestedDataObject.ConvertToClientApiObject(objPayment);

                        DataObject DO = new DataObject("Transaction", typeof(MakerShop.Payments.Transaction), typeof(Schema.Transaction));
                        objClientApiPayment.Transactions = (Schema.Transaction[])DO.ConvertAC6Collection(objPayment.Transactions);

                        arrClientApiPayments[j] = objClientApiPayment;
                    }

                    //PaymentMethodGroups
                    objClientApiPaymentMethod.PaymentMethodGroups = (Schema.PaymentMethodGroup[])DataObject.ConvertToClientArray(typeof(MakerShop.Payments.PaymentMethodGroup), typeof(Schema.PaymentMethodGroup), objPaymentMethod.PaymentMethodGroups);

                    arrPaymentMethod[i] = objClientApiPaymentMethod;
                }
                ClientStore.PaymentMethods = arrPaymentMethod;
            }

            //OrderStatus
            if (lstOption.Contains(DataObject.Names.OrderStatuses.ToString()))
            {
                dataObject = new DataObject("OrderStatus", typeof(MakerShop.Orders.OrderStatus), typeof(Schema.OrderStatus));
                Schema.OrderStatus[] arrOrderStatus = new Schema.OrderStatus[Ac6Store.OrderStatuses.Count];
                Schema.OrderStatus objClientApiOrderStatus = null;
                for (int i = 0; i < Ac6Store.OrderStatuses.Count; i++)
                {
                    MakerShop.Orders.OrderStatus objOrderStatus = Ac6Store.OrderStatuses[i];
                    objClientApiOrderStatus = (Schema.OrderStatus)dataObject.ConvertToClientApiObject(objOrderStatus);
                    objClientApiOrderStatus.Actions = (Schema.OrderStatusAction[])DataObject.ConvertToClientArray(typeof(MakerShop.Orders.OrderStatusAction), typeof(Schema.OrderStatusAction), objOrderStatus.Actions);
                    objClientApiOrderStatus.OrderStatusEmails = (Schema.OrderStatusEmail[])DataObject.ConvertToClientArray(typeof(MakerShop.Orders.OrderStatusEmail), typeof(Schema.OrderStatusEmail), objOrderStatus.OrderStatusEmails);
                    objClientApiOrderStatus.Triggers = (Schema.OrderStatusTrigger[])DataObject.ConvertToClientArray(typeof(MakerShop.Orders.OrderStatusTrigger), typeof(Schema.OrderStatusTrigger), objOrderStatus.Triggers);

                    arrOrderStatus[i] = objClientApiOrderStatus;
                }
                ClientStore.OrderStatuses = arrOrderStatus;
            }

            // ShipZones
            if (lstOption.Contains(DataObject.Names.ShipZones.ToString()))
            {
                dataObject = new DataObject("ShipZone", typeof(MakerShop.Shipping.ShipZone), typeof(Api.Schema.ShipZone));
                Api.Schema.ShipZone[] arrClientApiShipZone = new Api.Schema.ShipZone[Ac6Store.ShipZones.Count];
                Api.Schema.ShipZone objClientApiShipZone = null;

                for (int i = 0; i < Ac6Store.ShipZones.Count; i++)
                {
                    MakerShop.Shipping.ShipZone objShipZone = Ac6Store.ShipZones[i];
                    objClientApiShipZone = (Api.Schema.ShipZone)dataObject.ConvertToClientApiObject(objShipZone);


                    // ShipZoneCountries
                    nestedDataObject = new DataObject("ShipZoneCountry", typeof(MakerShop.Shipping.ShipZoneCountry), typeof(Api.Schema.ShipZoneCountry));
                    objClientApiShipZone.ShipZoneCountries = (Api.Schema.ShipZoneCountry[])nestedDataObject.ConvertAC6Collection(objShipZone.ShipZoneCountries);

                    // ShipZoneProviences
                    nestedDataObject = new DataObject("ShipZoneProvince", typeof(MakerShop.Shipping.ShipZoneProvince), typeof(Api.Schema.ShipZoneProvince));
                    objClientApiShipZone.ShipZoneProvinces = (Api.Schema.ShipZoneProvince[])nestedDataObject.ConvertAC6Collection(objShipZone.ShipZoneProvinces);

                    arrClientApiShipZone[i] = objClientApiShipZone;

                }
                ClientStore.ShipZones = arrClientApiShipZone;
            }

            //ShipMethods
            if (lstOption.Contains(DataObject.Names.ShipMethods.ToString()))
            {
                dataObject = new DataObject("ShipMethod", typeof(MakerShop.Shipping.ShipMethod), typeof(Api.Schema.ShipMethod));
                Api.Schema.ShipMethod[] arrClientApiShipMethod = new Api.Schema.ShipMethod[Ac6Store.ShipMethods.Count];
                Api.Schema.ShipMethod objClientApiShipMethod = null;

                for (int i = 0; i < Ac6Store.ShipMethods.Count; i++)
                {
                    MakerShop.Shipping.ShipMethod objShipMethod = Ac6Store.ShipMethods[i];
                    //arrClientApiShipZone[i] = (Api.Schema.Product)dataObject.convertToClientApiObject(objShipZone);
                    objClientApiShipMethod = (Api.Schema.ShipMethod)dataObject.ConvertToClientApiObject(objShipMethod);


                    // ShipMethodGroups
                    objClientApiShipMethod.ShipMethodGroups = (Schema.ShipMethodGroup[])DataObject.ConvertToClientArray(typeof(MakerShop.Shipping.ShipMethodGroup), typeof(Schema.ShipMethodGroup), objShipMethod.ShipMethodGroups);

                    // ShipMethodShipZone
                    nestedDataObject = new DataObject("ShipMethodShipZone", typeof(MakerShop.Shipping.ShipMethodShipZone), typeof(Api.Schema.ShipMethodShipZone));
                    objClientApiShipMethod.ShipMethodShipZones = (Api.Schema.ShipMethodShipZone[])nestedDataObject.ConvertAC6Collection(objShipMethod.ShipMethodShipZones);

                    // ShipMethodWarehouse
                    objClientApiShipMethod.ShipMethodWarehouses = (Schema.ShipMethodWarehouse[])DataObject.ConvertToClientArray(typeof(MakerShop.Shipping.ShipMethodWarehouse), typeof(Schema.ShipMethodWarehouse), objShipMethod.ShipMethodWarehouses);

                    //ShipRateMatrixs
                    objClientApiShipMethod.ShipRateMatrices = (Schema.ShipRateMatrix[])DataObject.ConvertToClientArray(typeof(MakerShop.Shipping.ShipRateMatrix), typeof(Schema.ShipRateMatrix), objShipMethod.ShipRateMatrices);                                        

                    arrClientApiShipMethod[i] = objClientApiShipMethod;

                }
                ClientStore.ShipMethods = arrClientApiShipMethod;
            }


            //DigitalGoods            
            if (lstOption.Contains(DataObject.Names.DigitalGoods.ToString()))
            {
                dataObject = new DataObject(typeof(MakerShop.DigitalDelivery.DigitalGood), typeof(Schema.DigitalGood));
                ClientStore.DigitalGoods = (Schema.DigitalGood[])dataObject.ConvertAC6Collection(Ac6Store.DigitalGoods);

                //SerialKeys
                for (int i = 0; i < clientStore.DigitalGoods.Length; i++)
                {
                    Schema.DigitalGood schemaDigitalGood = clientStore.DigitalGoods[i];
                    MakerShop.DigitalDelivery.DigitalGood ACDigitalGood = Ac6Store.DigitalGoods[i];

                    dataObject = new DataObject(typeof(MakerShop.DigitalDelivery.SerialKey), typeof(Schema.SerialKey));
                    schemaDigitalGood.SerialKeys = (Schema.SerialKey[])dataObject.ConvertAC6Collection(ACDigitalGood.SerialKeys);                    
                }
            }


            // Users
            if (lstOption.Contains(DataObject.Names.Users.ToString()))
            {
                // if chunkSize > 0, then it means that we will export "UserId" of Users 
                // in order to export data in Chunks
                List<String> userIdList = UserHandler.GetIdListForStore();
                if (chunkSize > 0 && userIdList.Count > chunkSize)
                {
                    clientStore.UserIdList = String.Join(",", userIdList.ToArray());
                    ClientStore.Users = null;
                }
                else
                {

                    ClientStore.Users = UserHandler.ConvertToClientArray(Ac6Store.Users, false);
                }
            }


            // EmailLists
            if (lstOption.Contains(DataObject.Names.EmailLists.ToString()))
            {
                dataObject = new DataObject("EmailList", typeof(MakerShop.Marketing.EmailList), typeof(Schema.EmailList));
                Schema.EmailList[] arrEmailList = new Schema.EmailList[Ac6Store.EmailLists.Count];
                Schema.EmailList objClientApiEmailList = null;
                for (int i = 0; i < Ac6Store.EmailLists.Count; i++)
                {
                    MakerShop.Marketing.EmailList objEmailList = Ac6Store.EmailLists[i];
                    objClientApiEmailList = (Schema.EmailList)dataObject.ConvertToClientApiObject(objEmailList);

                    // EmailListUser
                    nestedDataObject = new DataObject("EmailListUser", typeof(MakerShop.Marketing.EmailListUser), typeof(Schema.EmailListUser));
                    objClientApiEmailList.Users = (Schema.EmailListUser[])nestedDataObject.ConvertAC6Collection(objEmailList.Users);

                    // EmailListSignup
                    nestedDataObject = new DataObject("EmailListSignup", typeof(MakerShop.Marketing.EmailListSignup), typeof(Schema.EmailListSignup));
                    objClientApiEmailList.Signups = (Schema.EmailListSignup[])nestedDataObject.ConvertAC6Collection(objEmailList.Signups);

                    arrEmailList[i] = objClientApiEmailList;
                }
                ClientStore.EmailLists = arrEmailList;
            }

            //AuditEvents    
            if (lstOption.Contains(DataObject.Names.AuditEvents.ToString()))
            {
                ClientStore.AuditEvents = (Api.Schema.AuditEvent[])DataObject.ConvertToClientArray(typeof(MakerShop.Stores.AuditEvent), typeof(Api.Schema.AuditEvent), Ac6Store.AuditEvents);
            }
            

            //Products
            if (lstOption.Contains(DataObject.Names.Products.ToString()))
            {
                // if chunkSize > 0, then it means that we will export "Product Id" of products 
                // in order to export data in Chunks
                List<String> productIdList = ProductHandler.GetIdListForStore();
                if (chunkSize > 0 && productIdList.Count > chunkSize)
                {
                    clientStore.ProductIdList = String.Join(",", productIdList.ToArray());
                    ClientStore.Products = null;
                }
                else
                {
                    ClientStore.Products = ProductHandler.ConvertToClientArray(Ac6Store.Products, false);
                }
            }

            //KitComponents
            if (lstOption.Contains(DataObject.Names.KitComponents.ToString()))
            {
                ClientStore.KitComponents = (Api.Schema.KitComponent[])DataObject.ConvertToClientArray(typeof(MakerShop.Products.KitComponent), typeof(Api.Schema.KitComponent), Ac6Store.KitComponents);

                //KitProducts
                for (int i_kp = 0; i_kp < ClientStore.KitComponents.Length; i_kp++)
                {
                    Schema.KitComponent schemaObj = ClientStore.KitComponents[i_kp];
                    MakerShop.Products.KitComponent acObject = Ac6Store.KitComponents[i_kp];

                    DataObject subDataObject = new DataObject(typeof(MakerShop.Products.KitProduct), typeof(Schema.KitProduct));
                    schemaObj.KitProducts = (Schema.KitProduct[])subDataObject.ConvertAC6Collection(acObject.KitProducts);
                }
            }

            //ProductAssociations
            if (lstOption.Contains(DataObject.Names.Products.ToString()))
            {
                ClientStore.ProductAssociations = new ProductAssociations();
                ClientStore.ProductAssociations.UpsellProducts = ProductHandler.GetUpsellProducts();
                ClientStore.ProductAssociations.RelatedProducts = ProductHandler.GetRelatedProducts();
                if (lstOption.Contains(DataObject.Names.KitComponents.ToString())) ClientStore.ProductAssociations.ProductKitComponents = ProductHandler.GetProductKitComponents();
            }

            // Coupons
            if (lstOption.Contains(DataObject.Names.Coupons.ToString()))
            {
                dataObject = new DataObject("", typeof(MakerShop.Marketing.Coupon), typeof(Schema.Coupon));
                ClientStore.Coupons = (Schema.Coupon[])dataObject.ConvertAC6Collection(Ac6Store.Coupons);
                for (int i = 0; i < clientStore.Coupons.Length; i++)
                {
                    Schema.Coupon schemaCoupon = ClientStore.Coupons[i];
                    MakerShop.Marketing.Coupon ACCoupon = Ac6Store.Coupons[i];

                    //CouponGroups
                    dataObject = new DataObject(typeof(MakerShop.Marketing.CouponGroup), typeof(Schema.CouponGroup));
                    schemaCoupon.CouponGroups = (Schema.CouponGroup[])dataObject.ConvertAC6Collection(ACCoupon.CouponGroups);

                    //CouponProducts
                    dataObject = new DataObject(typeof(MakerShop.Marketing.CouponProduct), typeof(Schema.CouponProduct));
                    schemaCoupon.CouponProducts = (Schema.CouponProduct[])dataObject.ConvertAC6Collection(ACCoupon.CouponProducts);

                    //CouponShipMethods
                    dataObject = new DataObject(typeof(MakerShop.Marketing.CouponShipMethod), typeof(Schema.CouponShipMethod));
                    schemaCoupon.CouponShipMethods = (Schema.CouponShipMethod[])dataObject.ConvertAC6Collection(ACCoupon.CouponShipMethods);
                }

            }

            //CustomFields            
            if (lstOption.Contains(DataObject.Names.CustomFields.ToString()))
            {
                ClientStore.CustomFields = (Api.Schema.CustomField[])DataObject.ConvertToClientArray(typeof(MakerShop.Stores.CustomField), typeof(Api.Schema.CustomField), Ac6Store.CustomFields);
            }



            // PageViews
            if (lstOption.Contains(DataObject.Names.PageViews.ToString()))
                ClientStore.PageViews = (Schema.PageView[])DataObject.ConvertToClientArray(typeof(MakerShop.Reporting.PageView), typeof(Schema.PageView), Ac6Store.PageViews);


            //Orders
            if (lstOption.Contains(DataObject.Names.Orders.ToString()))
            {
                // if chunkSize > 0, then it means that we will export "OrderId" of Orders 
                // in order to export data in Chunks
                List<String> orderIdList = OrderHandler.GetIdListForStore();
                if (chunkSize > 0 && orderIdList.Count > chunkSize)
                {
                    clientStore.OrderIdList = String.Join(",", orderIdList.ToArray());
                    clientStore.Orders = null;
                }
                else
                {
                    ClientStore.Orders = OrderHandler.ConvertToClientArray(Ac6Store.Orders, exportOptions.DownloadCCData , false);
                }
            }
        
            // GiftCertificates
            if (lstOption.Contains(DataObject.Names.GiftCertificates.ToString()))
            {
                dataObject = new DataObject("GiftCertificate", typeof(MakerShop.Payments.GiftCertificate), typeof(Schema.GiftCertificate));
                Schema.GiftCertificate[] arrGiftCertificate = new Schema.GiftCertificate[Ac6Store.GiftCertificates.Count];
                Schema.GiftCertificate objClientApiGiftCertificate = null;
                for (int i = 0; i < Ac6Store.GiftCertificates.Count; i++)
                {
                    MakerShop.Payments.GiftCertificate objGiftCertificate = Ac6Store.GiftCertificates[i];
                    objClientApiGiftCertificate = (Schema.GiftCertificate)dataObject.ConvertToClientApiObject(objGiftCertificate);

                    // Transactions
                    nestedDataObject = new DataObject("Transaction", typeof(MakerShop.Payments.GiftCertificateTransaction), typeof(Schema.GiftCertificateTransaction));
                    objClientApiGiftCertificate.Transactions = (Schema.GiftCertificateTransaction[])nestedDataObject.ConvertAC6Collection(objGiftCertificate.Transactions);

                    arrGiftCertificate[i] = objClientApiGiftCertificate;
                }
                ClientStore.GiftCertificates = arrGiftCertificate;
            }

           

            acExportResponse.ExportResponse.Log = new Log();
            acExportResponse.ExportResponse.Log.Message = String.Empty;

            // will be same as request id
            acExportResponse.ExportResponse.ResponseId = requestId;

            MakerShopExport.Store = ClientStore;
            acExportResponse.ExportResponse.Data = ZipUtility.Compress(EncodeHelper.Serialize(MakerShopExport));

            byte[] response = EncodeHelper.Serialize(acExportResponse);
            return response;
        }

        private MakerShopExport InitBasicStoreData()
        {
            //Information regarding export
            MakerShopExport objMakerShopExport = new MakerShopExport();
            objMakerShopExport.ExportBuildNumber = "1.0";
            objMakerShopExport.ExportVersion = "7.0";

            // VERSION INFO
            String filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\VersionInfo.xml");
            if (File.Exists(filePath))
            {
                try
                {
                    XmlDocument versionDocument = new XmlDocument();
                    versionDocument.Load(filePath);
                    objMakerShopExport.ExportVersion = XmlUtility.GetElementValue(versionDocument.DocumentElement, "Version", "7.0");
                    objMakerShopExport.ExportBuildNumber = XmlUtility.GetElementValue(versionDocument.DocumentElement, "BuildNumber", string.Empty);
                }
                catch
                {
                    // DO NOTHING
                }
            }

            return objMakerShopExport;
        }

        //public byte[] DoSelectedStandardExport(String lstExportOptions)
        //{
        //    return DoSelectedStandardExport(lstExportOptions, 0);
        //}

        //public byte[] DoSelectedStandardExport(String lstExportOptions, int chunkSize)
        //{

        //    DataObject dataObject ;
        //    ACExportResponse acExportResponse = new ACExportResponse();
            
        //    ExportResponse objExportResponse = new ExportResponse();
        //    acExportResponse.ExportResponse = objExportResponse;
        //    objExportResponse.Data = new Api.Schema.Data();
            
        //    // Add basic export data
        //    objExportResponse.Data.MakerShopExport = InitBasicStoreData();
            
        //    // Store Node            
        //    Ac6Store = Token.Instance.Store;

        //    //ClientStore = StoreHandler.ConverToClient(Ac6Store, false);


        //    objExportResponse.Data.MakerShopExport.Store = StoreHandler.ConverToClient(Ac6Store,false);
        //    ClientStore = objExportResponse.Data.MakerShopExport.Store;


        //    // Add optional data
        //    foreach (String option in lstExportOptions.Split(','))
        //    {
        //        DataObject.Names intOption = (DataObject.Names)Enum.Parse(typeof(DataObject.Names), option, true);
        //        switch (intOption)
        //        {
        //            case DataObject.Names.Affiliates:
        //                dataObject = new DataObject(typeof(MakerShop.Marketing.Affiliate), typeof(Schema.Affiliate));
        //                ClientStore.Affiliates = (Schema.Affiliate[])dataObject.ConvertAC6Collection(Ac6Store.Affiliates);                         
        //                break;
        //            case DataObject.Names.Categories:
        //                ClientStore.Categories = CategoryHandler.ConvertToClientArray(Ac6Store.Categories);
        //                break;
        //            case DataObject.Names.Orders:
        //                // if chunkSize > 0, then it means that we will export "OrderId" of orders 
        //                // in order to export data in Chunks
        //                List<String> orderIdList = OrderHandler.GetIdListForStore();
        //                if (chunkSize > 0 && orderIdList.Count > chunkSize)
        //                {
        //                    //clientStore.OrderIdList = DataObject.GetIdsFromCollection(Ac6Store.Orders, typeof(MakerShop.Orders.Order), "OrderId");
        //                    clientStore.OrderIdList = String.Join(",",orderIdList.ToArray());
        //                    ClientStore.Orders = null;
        //                }
        //                else
        //                {
        //                    ClientStore.Orders = OrderHandler.ConvertToClientArray(Ac6Store.Orders);
        //                    Utility.LogDebug("ClientStore.Orders:" + ClientStore.Orders.Length);
        //                }
        //                break;
        //            case DataObject.Names.Products:
        //                // if chunkSize > 0, then it means that we will export "Product Id" of products 
        //                // in order to export data in Chunks
        //                List<String> productIdList = ProductHandler.GetIdListForStore();
        //                if (chunkSize > 0 && productIdList.Count > chunkSize)
        //                {
        //                    clientStore.ProductIdList = String.Join(",", productIdList.ToArray());
        //                    ClientStore.Products = null;
        //                }
        //                else
        //                {
        //                    ClientStore.Products = ProductHandler.ConvertToClientArray(Ac6Store.Products);
        //                    Utility.LogDebug("ClientStore.Products:" + ClientStore.Products.Length);
        //                }
        //                break;
        //            case DataObject.Names.Users:
        //                // if chunkSize > 0, then it means that we will export "UserId" of users 
        //                // in order to export data in Chunks
        //                List<String> userIdList = UserHandler.GetIdListForStore();
        //                if (chunkSize > 0 && userIdList.Count > chunkSize)
        //                {
        //                    clientStore.UserIdList = String.Join(",", userIdList.ToArray());
        //                    ClientStore.Users = null;
        //                }
        //                else
        //                {
        //                    ClientStore.Users = UserHandler.ConvertToClientArray(Ac6Store.Users);
        //                    Utility.LogDebug("ClientStore.Users:" + ClientStore.Users.Length);
        //                }
        //                break;
        //            case DataObject.Names.Warehouses:
        //                dataObject = new DataObject(typeof(MakerShop.Shipping.Warehouse),typeof(Schema.Warehouse));
        //                ClientStore.Warehouses = (Schema.Warehouse[])dataObject.ConvertAC6Collection(Ac6Store.Warehouses); 
        //                break;
        //        }
        //    }

        //    objExportResponse.Data.MakerShopExport.Store = ClientStore;

        //    objExportResponse.Log = new Log();
        //    objExportResponse.Log.Message = String.Empty;

        //    // will be same as request id
        //    objExportResponse.ResponseId = requestId;            

        //    byte[] response = EncodeHelper.Serialize(acExportResponse);
        //    return response;
        //}

        public byte[] DoCustomizedProductsExport(ACCustomizedProductRequest customizedProductRequest)
        {
            ACExportResponse acExportResponse = new ACExportResponse();

            ExportResponse objExportResponse = new ExportResponse();
            acExportResponse.ExportResponse = objExportResponse;
            

            // Add basic export data
            MakerShopExport MakerShopExport = InitBasicStoreData();

            // Store Node
            Ac6Store = Token.Instance.Store;
            MakerShopExport.Store = StoreHandler.ConverToClient(Ac6Store, false);
            ClientStore = MakerShopExport.Store;          


            //Code for chunks
            //if this is a request for customized products
            if (String.IsNullOrEmpty(customizedProductRequest.IdList))
            {
                // get customized products data
                //ProductCollection productCollection = ProductHandler.GetCustomizedCollection(customizedProductRequest.ProductCriteria);
                // if chunksize is > 0 and productCollection.count > chunksize, so export Ids
                List<String> productIdList;
                if (customizedProductRequest.ProductCriteria != null)
                    productIdList = ProductHandler.GetIdListForProductCriteria(customizedProductRequest.ProductCriteria);
                else
                    // AS CRITERIA IS NULL, SO RETURN ALL OBJECTS FOR THE STORE
                    productIdList = ProductHandler.GetIdListForStore();

                if (customizedProductRequest.ChunkSize > 0 && productIdList.Count > customizedProductRequest.ChunkSize)
                {
                    clientStore.ProductIdList = String.Join(",", productIdList.ToArray());
                    clientStore.Products = null;
                }
                // Export products
                else if (customizedProductRequest.ChunkSize == 0 || productIdList.Count <= customizedProductRequest.ChunkSize)
                {
                    ProductCollection productCollection = ProductHandler.GetProductsForIds(String.Join(",", productIdList.ToArray()));
                    //Get products on the basis of criteria and then assign
                    ClientStore.Products = ProductHandler.ConvertToClientArray(productCollection, customizedProductRequest.IsCSVRequst);
                }
            }
            // if request is to export product for the given Ids so here Criteria will be null nad IdList will contain product Ids to be exported
            else if (customizedProductRequest.ProductCriteria == null && !String.IsNullOrEmpty(customizedProductRequest.IdList))
            {
                ProductCollection productCollection = ProductHandler.GetProductsForIds(customizedProductRequest.IdList);
                ClientStore.Products = ProductHandler.ConvertToClientArray(productCollection, customizedProductRequest.IsCSVRequst);
            }

            //End of new chunk code

            objExportResponse.Log = new Log();
            objExportResponse.Log.Message = "Customized Product Export Completed.";

            // will be same as request id
            objExportResponse.ResponseId = requestId;

            objExportResponse.Data = ZipUtility.Compress(EncodeHelper.Serialize(MakerShopExport));

            byte[] response = EncodeHelper.Serialize(acExportResponse);
            return response;
        }

        public byte[] DoCustomizedOrdersExport(ACCustomizedOrderRequest customizedOrderRequest)
        {
            ACExportResponse acExportResponse = initializeResponse();
            
            // Store Node
            Ac6Store = Token.Instance.Store;
            MakerShopExport MakerShopExport = InitBasicStoreData();
            MakerShopExport.Store = StoreHandler.ConverToClient(Ac6Store, false);
            ClientStore = MakerShopExport.Store;                         

            //Chunk code starts here
            OrderCollection orderCollection = null;

            //if this is a request for customized proucts
            if (String.IsNullOrEmpty(customizedOrderRequest.IdList))
            {
                // get customized users data
                //OrderCollection orderCollection = OrderHandler.GetCustomizedCollection(customizedOrderRequest.OrderCriteria);
                // if chunksize is > 0 and orderCollection.count > chunksize, so export Ids
                List<String> orderIdList;
                if (customizedOrderRequest.OrderCriteria != null)
                    orderIdList = OrderHandler.GetIdListForOrderCriteria(customizedOrderRequest.OrderCriteria, customizedOrderRequest.IsUPSWSRequest);
                else
                    // AS CRITERIA IS NULL, SO RETURN ALL OBJECTS FOR THE STORE
                    orderIdList = OrderHandler.GetIdListForStore();

                if (customizedOrderRequest.ChunkSize > 0 && orderIdList.Count > customizedOrderRequest.ChunkSize)
                {
                    clientStore.OrderIdList = String.Join(",", orderIdList.ToArray());
                    
                }
                // Export orders
                else if (customizedOrderRequest.ChunkSize == 0 || orderIdList.Count <= customizedOrderRequest.ChunkSize)
                {
                    orderCollection = OrderHandler.GetCustomizedCollection(customizedOrderRequest.OrderCriteria,customizedOrderRequest.IsUPSWSRequest);                    
                }
            }
            // if request is to export orders for the given Ids so here Criteria will be null nad IdList will contain orders Ids to be exported
            else if (customizedOrderRequest.OrderCriteria == null && !String.IsNullOrEmpty(customizedOrderRequest.IdList))
            {
                orderCollection = OrderHandler.GetOrdersForIds(customizedOrderRequest.IdList);
            }
            

            if (orderCollection != null)
            {
                ClientStore.Orders = OrderHandler.ConvertToClientArray(orderCollection,customizedOrderRequest.DownloadCCData, customizedOrderRequest.IsCSVRequst);
                if (customizedOrderRequest.IsUPSWSRequest)
                {
                    // append records for warehouses
                    DataObject dataObject = new DataObject(typeof(MakerShop.Shipping.Warehouse),typeof(MakerShop.DataClient.Api.Schema.Warehouse));
                    ClientStore.Warehouses = (MakerShop.DataClient.Api.Schema.Warehouse[])dataObject.ConvertAC6Collection(GetUPSWsWareHouses(orderCollection));
                }

            }
            else
            {
                ClientStore.Orders = null;
            }

            //Chunk code ends here
            acExportResponse.ExportResponse.Data = ZipUtility.Compress(EncodeHelper.Serialize(MakerShopExport));

            acExportResponse.ExportResponse.Log = new Log();
            acExportResponse.ExportResponse.Log.Message = "Customized Product Export Completed.";

            // will be same as request id
            acExportResponse.ExportResponse.ResponseId = requestId;


            byte[] response = EncodeHelper.Serialize(acExportResponse);
            return response;
        }

        private MakerShop.Shipping.WarehouseCollection GetUPSWsWareHouses(OrderCollection orderCollection)
        {
            WarehouseCollection warehouses = new WarehouseCollection();
             foreach( MakerShop.Orders.Order order in orderCollection){
                foreach(MakerShop.Orders.OrderShipment shipment in order.Shipments){
                    if(!warehouses.Contains(shipment.Warehouse)){
                        warehouses.Add(shipment.Warehouse);
                    }
                }
            }
            return warehouses;
        }

        private ACExportResponse initializeResponse()
        {
            ACExportResponse acExportResponse = new ACExportResponse();

            ExportResponse objExportResponse = new ExportResponse();
            acExportResponse.ExportResponse = objExportResponse;

            return acExportResponse;

        }

        public byte[] DoCustomizedUsersExport(ACCustomizedUserRequest customizedUserRequest)
        {

            ACExportResponse acExportResponse = initializeResponse();

            // Store Node
            Ac6Store = Token.Instance.Store;
            MakerShopExport MakerShopExport = InitBasicStoreData();
            MakerShopExport.Store = StoreHandler.ConverToClient(Ac6Store, false);
            ClientStore = MakerShopExport.Store;           
            

            //Chunk code starts here

            //if this is a request for customized proucts

            if (String.IsNullOrEmpty(customizedUserRequest.IdList))
            {
                //get customized users data
                //UserCollection userCollection = UserHandler.GetCustomizedCollection(customizedUserRequest.UserCriteria);
                // if chunksize is > 0 and userCollection.count > chunksize, so export Ids
                List<String> userIdList;
                if (customizedUserRequest.UserCriteria != null)
                    userIdList = UserHandler.GetIdListForUserCriteria(customizedUserRequest.UserCriteria);
                else
                    userIdList = UserHandler.GetIdListForStore();

                if (customizedUserRequest.ChunkSize > 0 && userIdList.Count > customizedUserRequest.ChunkSize)
                {
                    clientStore.UserIdList = String.Join(",", userIdList.ToArray());
                    ClientStore.Users = null;
                }
                // Export users
                else if (customizedUserRequest.ChunkSize == 0 || userIdList.Count <= customizedUserRequest.ChunkSize)
                {

                    UserCollection userCollection = UserHandler.GetUsersForIds(String.Join(",", userIdList.ToArray()));
                    ClientStore.Users = UserHandler.ConvertToClientArray(userCollection, customizedUserRequest.IsCSVRequst);
                }
            }
            // if request is to export user for the given Ids so here Criteria will be null nad IdList will contain user Ids to be exported
            else 
            if (customizedUserRequest.UserCriteria == null && !String.IsNullOrEmpty(customizedUserRequest.IdList))
            {
                
                UserCollection userCollection = UserHandler.GetUsersForIds(customizedUserRequest.IdList);
                ClientStore.Users = UserHandler.ConvertToClientArray(userCollection, customizedUserRequest.IsCSVRequst);
            }

            //Chunk code ends here
            acExportResponse.ExportResponse.Data = ZipUtility.Compress(EncodeHelper.Serialize(MakerShopExport));

            acExportResponse.ExportResponse.Log = new Log();
            acExportResponse.ExportResponse.Log.Message = "Customized User Export Completed.";
            // will be same as request id
            acExportResponse.ExportResponse.ResponseId = requestId;
            byte[] response = EncodeHelper.Serialize(acExportResponse);
            return response;
        }        
    }
}
