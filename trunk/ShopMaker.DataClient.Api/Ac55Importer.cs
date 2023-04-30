using System;
using System.Data;
using System.IO;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Text.RegularExpressions;
using System.Text;

using MakerShop.Catalog;
using MakerShop.Common;
using MakerShop.DigitalDelivery;
using MakerShop.Exceptions;
using MakerShop.Utility;

using System.Collections.Generic;
using System.Collections.Specialized;

using System.Web.Caching;
using MakerShop.Stores;
using MakerShop.Messaging;
using MakerShop.Products;
using MakerShop.Shipping;
using MakerShop.Taxes;
using MakerShop.Users;
using MakerShop.Orders;
using MakerShop.Marketing;
using MakerShop.Payments;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System.Security.Cryptography;


namespace MakerShop.DataClient.Api
{
    /// <summary>
    /// Summary description for ImportTemp
    /// </summary>
    public class Ac55Importer
    {   

        #region Defination of Veriables

        StringDictionary dicTranslate;
        SortedDictionary<int, StoreEvent> eventsDictionary = null;        
        StringDictionary dicManufacturers = null;
        StringDictionary dicCountryZone = null;
        StringDictionary dicTaxZone = null;
        StringDictionary dicTaxCodes = null;
        

        // are used for asynchronious import
        private delegate void ImportDelegate(System.Xml.XmlDocument importXml);
        private ImportDelegate importDelegate;

        // Holds the status of import progress
        ImportStatus importStatus;

        // need to calculate in phase percentage
        int currentPhaseWeightage = 0;
        int overAllPercent = 0;
        int currentPhasePercent = 0;


        int intOldID, intOldParentID;
        int newParentID = 0;

        // ROOT CATEGORY FOR ORPHANED CATALOG ITEMS
        int orphanedItemsCategoryId = 0;

        SmtpSettings _SmtpSettingsBackup = null;

        PreserveIdOption _PreserveIdOption = PreserveIdOption.AddAsNewObject;
        public PreserveIdOption PreserveIdOption
        {
            get {
                return this._PreserveIdOption;
            }
            set {
                this._PreserveIdOption = value;
            }
        }

        double _TimeZoneOffset = 0.0;
        /// <summary>
        /// The time zone offset value used while converting all date values
        /// </summary>
        public double TimeZoneOffset
        {
            get { return _TimeZoneOffset; }
            set { _TimeZoneOffset = value; }
        }

        bool _UpdateTimeZone = false;
        /// <summary>
        /// Indicates that either the timezone settings for store will be updated or not (based on TimeZoneOffset property)
        /// </summary>
        public bool UpdateTimeZone
        {
            get { return _UpdateTimeZone; }
            set { _UpdateTimeZone = value; }
        }

        

        public StringDictionary TranslationDictionary
        {
            get {
                if (dicTranslate == null)
                    dicTranslate = new StringDictionary();
                return dicTranslate; 
            }
            set { dicTranslate = value; }
        }

        public int OrphanedItemsCategoryId
        {
            get
            {
                if (orphanedItemsCategoryId == 0)
                {
                    // TRY TO LOAD 
                    Category category  = CategoryDataSource.LoadForName("Root", 0);
                    if (category == null)
                    {
                        // IF NOT FOUND THEN CREATE A NEW CATEGORY AT ROOT
                        category = new Category();
                        category.Name = "Root";
                        category.StoreId = Token.Instance.StoreId;
                        category.ParentId = 0;
                        category.Save();
                    }
                    orphanedItemsCategoryId = category.CategoryId;
                }
                return orphanedItemsCategoryId;
            }
        }

        private void HandleOrphanedItemsCategory()
        {
            if (orphanedItemsCategoryId > 0)
            {
                Category category = CategoryDataSource.Load(orphanedItemsCategoryId);
                if (category != null && category.CatalogNodes.Count == 0) category.Delete();
            }
        }

        #endregion

        public Ac55Importer()
        {
            importDelegate = new ImportDelegate(this.import);
            importStatus = new ImportStatus();
            LogStatus(overAllPercent, "Initializing import process...");
        }

        #region AC5 Password Helper

        private static Byte[] m_AESIV = { 16, 23, 73, 104, 55, 62, 17, 108, 97, 120, 211, 39, 223, 14, 45, 201 };
        private static RijndaelManaged m_AES;

        private static RijndaelManaged getAES()
        {
            if (m_AES == null)
            {
                m_AES = new RijndaelManaged();
                m_AES.IV = m_AESIV;
            }
            return m_AES;
        }

        public static string Decrypt128(string vstrRaw, string vstrKey)
        {
            if (string.IsNullOrEmpty(vstrRaw)) return string.Empty;
            byte[] bytInput;
            //TRY TO DECODE THE BASE64 INPUT STRING
            try
            {
                bytInput = Convert.FromBase64String(vstrRaw);
            }
            catch
            {
                //THE INPUT IS NOT BASE64 ENCODED, RETURN INPUT
                return vstrRaw;
            }
            string strKey = vstrKey;
            int intLength = strKey.Length;
            int intRemaining;
            if (intLength > 16)
            {
                strKey = strKey.Substring(0, 16);
            }
            else if (intLength < 16)
            {
                intRemaining = (16 - intLength);
                strKey = strKey + (new String(' ', intRemaining)).Replace(" ", "X");
            }
            byte[] bytKey = Encoding.UTF8.GetBytes(strKey.ToCharArray());
            string sDec;
            try
            {
                //TRY TO DECRYPT THE VALUE
                RijndaelManaged objAES = getAES();
                using (MemoryStream objMemoryStream = new MemoryStream(bytInput))
                {
                    using (CryptoStream objCryptoStream = new CryptoStream(objMemoryStream, objAES.CreateDecryptor(bytKey, m_AESIV), CryptoStreamMode.Read))
                    {
                        using (StreamReader objStreamReader = new StreamReader(objCryptoStream, Encoding.UTF8))
                        {
                            sDec = objStreamReader.ReadToEnd();
                            objStreamReader.Close();
                        }
                        objCryptoStream.Close();
                    }
                    objMemoryStream.Close();
                }
                objAES = null;
            }
            catch 
            {
                //DECRYPTION FAILED, RETURN INPUT VALUE
                sDec = vstrRaw;
            }
            return sDec;
        }

        #endregion

        public void import(XmlDocument xmlImportDocument)
        {
            overAllPercent = 1;
            LogStatus(overAllPercent, "Processing import data...");

            //get the store node
            XmlNode nodSourceStore;
            nodSourceStore = xmlImportDocument.DocumentElement.SelectSingleNode("Stores/Store");
            try
            {
                // DISABLE EMAILS
                DisableSmtpSettings();

                // UPDATE TIME ZONE SETTINGS
                if (this.UpdateTimeZone)
                {
                    StoreSettingCollection settings = Token.Instance.Store.Settings;
                    settings.TimeZoneOffset = this.TimeZoneOffset;
                    settings.TimeZoneCode = this.TimeZoneOffset.ToString();
                    settings.Save();
                }

                // Import Groups
                currentPhaseWeightage = 3;
                ImportGroups(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // Import Discounts
                currentPhaseWeightage = 3;
                ImportDiscounts(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // Import Categories
                currentPhaseWeightage = 3;
                importCategories(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // CURRENCY IMPORT DISABLED (bug 7100)
                //// Import Currencies
                //currentPhaseWeightage = 3;
                //ImportCurrencies(nodSourceStore);
                //overAllPercent += currentPhaseWeightage;


                // Import Email Templates
                currentPhaseWeightage = 3;
                ImportEmailTempletes(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // Import ESDs
                currentPhaseWeightage = 3;
                ImportESDs(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // Import Gateways
                currentPhaseWeightage = 3;
                ImportGateways(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // Import Links
                currentPhaseWeightage = 3;
                ImportLinks(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // Import Order Status
                currentPhaseWeightage = 3;
                ImportOrderStatus(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // Import WareHouses
                currentPhaseWeightage = 3;
                ImportWareHouses(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // Import Shipping
                currentPhaseWeightage = 3;
                ImportShippingMethods(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // Import Taxe Codes
                currentPhaseWeightage = 3;
                ImportTaxCodes(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // Import Taxe Rules
                currentPhaseWeightage = 3;
                ImportTaxRules(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // Import Vendorss
                currentPhaseWeightage = 3;
                ImportVendors(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                //Import WebPages
                currentPhaseWeightage = 3;
                ImportWebPages(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // Import Giftwraps
                currentPhaseWeightage = 3;
                ImportGiftwraps(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                /********** Pass 2 *******************/

                // Import Affiliates
                currentPhaseWeightage = 3;
                ImportAffiliates(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // Import Payment Methods
                currentPhaseWeightage = 3;
                ImportPaymentMethods(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // Import Pruducts
                currentPhaseWeightage = 3;
                ImportProducts(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // Import Associations
                currentPhaseWeightage = 2;
                ImportObjectAssociations(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // Import Store Data
                currentPhaseWeightage = 3;
                ImportStoreData(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // Import Kits
                currentPhaseWeightage = 3;
                ImportKits(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // Import Users
                currentPhaseWeightage = 3;
                ImportUsers(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                /************  Pass 3 ***************/

                // Import Coupons
                currentPhaseWeightage = 3;
                ImportCoupons(nodSourceStore);
                overAllPercent += currentPhaseWeightage;


                // Import Gateway Transactions
                currentPhaseWeightage = 3;
                ImportGatewayTransactions(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // Import Wishlists
                currentPhaseWeightage = 3;
                ImportWishlists(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // Import Orders
                currentPhaseWeightage = 3;
                ImportOrders(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // Import Product File Associations
                currentPhaseWeightage = 3;
                ImportProductFileAssociations(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // Import Rights
                currentPhaseWeightage = 3;
                ImportRights(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                /******   Pass 4 *******/

                // Import Gift Certifictes
                currentPhaseWeightage = 3;
                ImportGiftCerts(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // Import Category Discounts
                currentPhaseWeightage = 3;
                ImportCategoryDiscounts(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // Import Product Discounts
                currentPhaseWeightage = 3;
                ImportProductDiscounts(nodSourceStore);
                overAllPercent += currentPhaseWeightage;                

                //Complete User Import
                currentPhaseWeightage = 3;
                CompleteUserImport(nodSourceStore);
                overAllPercent += currentPhaseWeightage;

                // DELETE ORPHANED ITEMS CATEGORY IF NEEDED
                HandleOrphanedItemsCategory();

                overAllPercent = 100;
                LogStatus(overAllPercent, "Import Completed Successfully!");
            }
            catch (Exception ex)
            {
                LogError(overAllPercent, "Import Unsuccessfull, Error Message:" + ", " + ex.Message + "\n" + ex.StackTrace);
                LogError(overAllPercent, "Import Unsuccessfull.");
            }
            finally
            {
                // RESTORE EMAIL SETTINGS
                RestoreSmtpSettings();
            }
        }

        #region Private Import Methods

        #region Pass 1

        private void importCategories(XmlNode nodSourceStore)
        {
            currentPhasePercent = 0;
            //LogStatus(GetPercentResult(), "Checking Categories...");

            XmlNodeList nlsItems = nodSourceStore.SelectNodes("Categories/Category");
            if (nlsItems.Count > 0)
            {


                int i = 0; // counter variable            
                LogStatus(GetPercentResult(), "Importing " + nlsItems.Count + " Categories .");

                Category objCategory = null;
                foreach (XmlNode objNode in nlsItems)
                {
                    // update the import status
                    i++;
                    currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
                    LogStatus(GetPercentResult(), "Importing Category " + i + " of " + nlsItems.Count);

                    try
                    {

                        objCategory = new Category();
                        intOldID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID", "0"));
                        intOldParentID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "Parent", "0"));

                        //AC55 category description should be set in ac6 category description field. Summary is new to AC6 so it will not have data from import.
                        objCategory.Description = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Description", ""));
                        objCategory.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Name", "Category"));
                        if (intOldParentID > 0)
                        {
                            if (TranslationDictionary.ContainsKey("CATEGORY" + intOldParentID))
                            {
                                newParentID = AlwaysConvert.ToInt(TranslationDictionary["CATEGORY" + intOldParentID]);
                            }
                            else
                            {
                                newParentID = 0; // ADD IT TO ROOT OF THE CATALOG
                            }
                        }
                        else
                        {
                            newParentID = 0; // ADD IT TO ROOT OF THE CATALOG
                        }

                        //Comments:
                        /*
                        If you leave the values as an empty string, the rewriter and/or merchant 
                        configuration will apply the correct default page.
                        */
                        //objCategory.DisplayPage = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "DisplayPage", ""));

                        //Comments :
                        //You may leave out old theme data. It is not applicable in AC6.
                        //objCategory.Theme = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Theme"));




                        //Comments:
                        /*
                            Image1 ? ThumbnailUrl
                            Image1Params ? Ignore Data*
                            Image2 ? ImageUrl
                            Image2Params ? Ignore Data*
                         * 
                         * on the parameter data, this is an unsupported feature so we cannot use 
                         * this data. However, in AC6 we have new fields ThumbnailAltText and 
                         * ImageAltText. The image �params� fields held HTML attribute data 
                         * (like height=�20�). Use a regular expression to look for an alternate 
                         * text (alt=�Some text�) and if found, put it into the appropriate AltText 
                         * field.
                         */
                        //objCategory.Image1 = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Image1", ""))
                        //objCategory.Image1Params = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Image1Params", ""))

                        objCategory.ThumbnailUrl = FormatImgURL(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Image1", "")));
                        objCategory.ThumbnailAltText = GetAltText(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Image1Params", "")));
                        objCategory.Visibility = ConvertCatalogNodeVisibility(AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "Active", "1")));
                        objCategory.StoreId = Token.Instance.StoreId;
                        objCategory.ParentId = newParentID;

                        // PRESERVE ID's FOR OBJECT
                        String messages = String.Empty;
                        objCategory.CategoryId = intOldID;
                        SaveResult saveResult = ACDataSource.SaveCategory(objCategory, this.PreserveIdOption, ref messages);
                        if (messages.Length > 0) LogWarning(GetPercentResult(), messages);

                        // SKIP IMPORTING MORE DETAILS IF FAILED SAVING OBJECT
                        if (saveResult == SaveResult.Failed) continue;

                        if (saveResult != SaveResult.Failed)
                        {
                            TranslationDictionary.Add("CATEGORY" + intOldID, objCategory.CategoryId.ToString());

                            // RELOAD THE CATALOG NODES INSTEAD OF USING THE CATEGORY.CATALOGNODES
                            CatalogNodeCollection catalogNodes = CatalogNodeDataSource.LoadForCategory(objCategory.CategoryId);

                            foreach (CatalogNode node in catalogNodes)
                            {
                                node.OrderBy = AlwaysConvert.ToInt16(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "OrderBy", "")));
                            }
                        }
                    }catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "Categories Import, CategoryId:" + intOldID + " , Name: " + objCategory.Name + ", " + ex.Message + "\n" + ex.StackTrace);
                    }
                }
                LogStatus(GetPercentResult(), "Categories Import Complete...");
            }
            else
            {
                currentPhasePercent = currentPhaseWeightage;
                //LogStatus(GetPercentResult(), "No  Categories Available to Import...");
            }
        }

        // CURRENCY IMPORT DISABLED (bug 7100)
        //private void ImportCurrencies(XmlNode nodSourceStore)
        //{
        //    currentPhasePercent = 0;
        //    //LogStatus(GetPercentResult(), "Checking Currencies...");

        //    XmlNodeList nlsItems = nodSourceStore.SelectNodes("Currencies/Currency");



        //    if (nlsItems.Count > 0)
        //    {
        //        int i = 0; // counter variable            
        //        LogStatus(GetPercentResult(), "Importing " + nlsItems.Count + " Currencies .");

        //        Currency objCurrency = null;
        //        int intOldCurrencyID = 0;
        //        foreach (XmlNode objNode in nlsItems)
        //        {
        //            // update the import status
        //            i++;
        //            currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
        //            LogStatus(GetPercentResult(), "Importing Currency " + i + " of " + nlsItems.Count);
        //            try{
        //                objCurrency = new Currency();
        //                intOldCurrencyID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID", "0"));
        //                objCurrency.StoreId = Token.Instance.StoreId;
        //                objCurrency.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Name", "Currency"));
        //                objCurrency.CurrencySymbol = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Prefix", ""));                        
        //                objCurrency.DecimalSeparator = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "DecimalSep", ""));
        //                objCurrency.DecimalDigits = AlwaysConvert.ToByte(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "DecimalPlaces", "")));
        //                objCurrency.GroupSeparator = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "ThousandSep", ""));
        //                objCurrency.ISOCode = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "ISOCode", ""));
        //                objCurrency.ExchangeRate = AlwaysConvert.ToDecimal(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Rate", "")));

        //                objCurrency.Save();
        //                TranslationDictionary.Add("CURRENCY" + intOldCurrencyID, objCurrency.CurrencyId.ToString());
        //            }
        //            catch (Exception ex)
        //            {
        //                LogError(GetPercentResult(), "Currencies Import, CurrencyID:" + intOldCurrencyID + " , Name: " + objCurrency.Name + ", " + ex.Message + "\n" + ex.StackTrace);
        //            }

        //        }
        //        objCurrency = null;
        //        LogStatus(GetPercentResult(), "Currencies Import Complete...");
        //    }
        //    else
        //    {
        //        currentPhasePercent = currentPhaseWeightage;
        //        //LogStatus(GetPercentResult(), "No  Currencies Available to Import...");
        //    }
        //}

        private void ImportEmailTempletes(XmlNode nodSourceStore)
        {
            ////if InStr(lstCopyOpts, ",EMAILS,") > 0 then
            //    'groups selected for copy


            currentPhasePercent = 0;
            //LogStatus(GetPercentResult(), "Checking Emails...");

            XmlNodeList nlsItems = nodSourceStore.SelectNodes("Emails/Email");
            if (nlsItems.Count > 0)
            {

                int i = 0; // counter variable            
                LogStatus(GetPercentResult(), "Importing " + nlsItems.Count + " Email Templetes .");

                EmailTemplate objEmail = null;
                int intOldEmailID = 0;
                EmailTemplateTriggerCollection colEmailTriggers;                
                foreach (XmlNode objNode in nlsItems)
                {

                    // update the import status
                    i++;
                    currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
                    LogStatus(GetPercentResult(), "Importing Email Templete " + i + " of " + nlsItems.Count);

                    try
                    {
                        objEmail = new EmailTemplate();
                        intOldEmailID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID", ""));
                        objEmail.StoreId = Token.Instance.StoreId;
                        objEmail.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "DisplayName", "Email"));
                        objEmail.ToAddress = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "ToAddress", ""));
                        objEmail.FromAddress = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "FromAddress", ""));
                        objEmail.ReplyToAddress = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "ReplyToAddress", ""));
                        objEmail.CCList = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "CCList", ""));
                        objEmail.BCCList = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "BCCList", ""));
                        objEmail.Subject = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Subject", ""));
                        objEmail.Body = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Body", ""));
                        objEmail.IsHTML = ToBool(XmlUtility.GetElementValue(objNode, "IsHTML", "false"), false);

                        if (objEmail.Save() != SaveResult.Failed)
                        {
                            TranslationDictionary.Add("Email" + intOldEmailID, objEmail.EmailTemplateId.ToString());
                            //update email event list [ Convert events to respective new events and add trigers]
                            colEmailTriggers = objEmail.Triggers;
                            string eventsList = XmlUtility.GetElementValue(objNode, "Events", "");
                            string[] eventsArray = eventsList.Split(',');
                            if (eventsArray.Length > 0)
                            {   
                                foreach (string evnt in eventsArray)
                                {
                                    int oldEventId = AlwaysConvert.ToInt(evnt, 0);
                                    colEmailTriggers.Add(new EmailTemplateTrigger(objEmail.EmailTemplateId, (int)StoreEventDictionary[oldEventId], 1));
                                }
                                colEmailTriggers.Save();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "Emails Import, EmailId:" + intOldEmailID + " , Name: " + objEmail.Name + ", " + ex.Message + "\n" + ex.StackTrace);
                    }
                }
                objEmail = null;
                LogStatus(GetPercentResult(), "Emails Import Complete...");
            }
            else
            {
                currentPhasePercent = currentPhaseWeightage;
                //LogStatus(GetPercentResult(), "No  Emails Available to Import...");
            }
        }

        #region ESD Lines = 286

        private void ImportESDs(XmlNode nodSourceStore)
        {
            
            /*
 
	currentPhasePercent = 0;
    Status(GetPercentResult(), "Checking ESD Data...");	

    XmlNodeList nlsItems = nodSourceStore.SelectNodes("ESD/EmailContents/EmailContent");
    if( nlsItems.Count > 0){
        int i = 0; // counter variable            
        Status(GetPercentResult(), "Importing " + nlsItems.Count + " Email Contents (ESD) .");
 
				
        //Dim objContent as cbContent

        foreach(XmlNode objNode in nlsItems){
            // update the import status
            i++;
            currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
            Status(GetPercentResult(), "Importing Email Content (ESD) " + i + " of " + nlsItems.Count);

            objContent = new cbContent()
            intOldContentID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID"))
            objContent.StoreId = Token.Instance.StoreId
            objContent.DisplayName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "DisplayName", "Email Content"))
        //	'objContent.ContentGroup = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "ContentGroup"))
            objContent.ContentBody = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "ContentBody", ""))
        //	'objContent.IsHTML = ToBool(XmlUtility.GetElementValue(objNode, "IsHTML"))
            if bPreserveIDs then
            //	'delete Content
                Token.Instance.StoreGroupDB.QueryNoRS("DELETE FROM EMAIL_CONTENTS WHERE Content_ID=" & intOldContentID)
            //	'force the next id returned from maxidlookup
                Token.Instance.StoreGroupDB.QueryNoRS("UPDATE MAX_ID_LOOKUP SET Max_ID=" & (intOldContentID-1) & " WHERE TableName=" & FCast.FSQLString("EMAIL_CONTENTS", intDBType))
            end if
            objContent.Save()
            if not objTranslate.ContainsKey("EMAILCONTENT" & intOldContentID) then
                objTranslate.Add("EMAILCONTENT" & intOldContentID, objContent.Content_ID)
            end if
        //	'verify preserved ids
            if bPreserveIDs then
                If (intOldContentID <> objContent.Content_ID) Then
                //	'force of ID was not successful
                    sWarnings.Append("Warning: Email Content " & intOldContentID & " did not have the ID successfully preserved.  The new ID is " & objContent.Content_ID & ".")
                    //response.write(" !")
                End If
            End If
            //response.write(" .")
					
        } //next objNode
        objContent = null
        //response.write("")
				
    else
        Status(GetPercentResult(), "No Email Contents (ESD) Available to Import...")
				
    end if
     
   XmlNodeList nlsItems = nodSourceStore.SelectNodes("ESD/EmailEnvelopes/EmailEnvelope")
    if( nlsItems.Count > 0){
        //response.write("Importing " & nlsItems.Count & " Email Envelopes (ESD) .")
				
        Dim objEnvelope as cbEnvelope
        foreach(XmlNode objNode in nlsItems){
            objEnvelope = new cbEnvelope()
            intOldEnvelopeID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID"))
            objEnvelope.StoreId = Token.Instance.StoreId
            objEnvelope.DisplayName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "DisplayName", "Email Envelope"))
            objEnvelope.FromAddress = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "FromAddress", ""))
            objEnvelope.ReplyToAddress = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "ReplyToAddress", ""))
            objEnvelope.CCList = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "CCList", ""))
            objEnvelope.BCCList = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "BCCList", ""))
            objEnvelope.Subject = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Subject", ""))
            objEnvelope.Header = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Header", ""))
            objEnvelope.Footer = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Footer", ""))
            objEnvelope.IsHTML = ToBool(XmlUtility.GetElementValue(objNode, "IsHTML"))
            if bPreserveIDs then
            //	'delete Envelope
                Token.Instance.StoreGroupDB.QueryNoRS("DELETE FROM EMAIL_ENVELOPES WHERE Envelope_ID=" & intOldEnvelopeID)
            //	'force the next id returned from maxidlookup
                Token.Instance.StoreGroupDB.QueryNoRS("UPDATE MAX_ID_LOOKUP SET Max_ID=" & (intOldEnvelopeID-1) & " WHERE TableName=" & FCast.FSQLString("EMAIL_ENVELOPES", intDBType))
            end if
            objEnvelope.Save()
            if not objTranslate.ContainsKey("EMAILENVELOPE" & intOldEnvelopeID) then
                objTranslate.Add("EMAILENVELOPE" & intOldEnvelopeID, objEnvelope.Envelope_ID)
            end if
        //	'verify preserved ids
            if bPreserveIDs then
                If (intOldEnvelopeID <> objEnvelope.Envelope_ID) Then
                //	'force of ID was not successful
                    sWarnings.Append("Warning: Email Envelope " & intOldEnvelopeID & " did not have the ID successfully preserved.  The new ID is " & objEnvelope.Envelope_ID & ".")
                    //response.write(" !")
                End If
            End If
            //response.write(" .")
					
        } //next objNode
        objEnvelope = null
        //response.write("")
				
    else
        Status(GetPercentResult(), "No Email Envelopes (ESD) Available to Import...")
				
    end if
          * */

    XmlNodeList nlsItems = nodSourceStore.SelectNodes("ESD/Notices/Notice");
    if (nlsItems.Count > 0)
    {

        int i = 0; // counter variable            
        LogStatus(GetPercentResult(), "Importing " + nlsItems.Count + " Agreements &amp; Readmes (ESD).");

        Readme objReadme = null;
        LicenseAgreement objAgreement = null;
        int intOldNoticeID = 0;
        foreach (XmlNode objNode in nlsItems)
        {

            // update the import status
            i++;
            currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
            LogStatus(GetPercentResult(), "Importing Agreement &amp; Readme " + i + " of " + nlsItems.Count);

            try
            {
                //IN AC55, WE USED A SINGLE TABLE (NOTICES) FOR READMES AND LICENSE AGREEMENTS
                //WE USED NOTICEGROUP FIELD TO TELL THE DIFFERENCE
                //IN AC7, WE HAVE SEPARATE TABLES AND THIS FIELD IS NOT NEEDED
                int noticeGroup = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "NoticeGroup", ""));
                if (noticeGroup == 1)
                {
                    // README
                    objReadme = new Readme();
                    intOldNoticeID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID", ""));
                    objReadme.StoreId = Token.Instance.StoreId;
                    objReadme.DisplayName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "DisplayName", "ReadMe"));
                                       
                    objReadme.ReadmeText = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "NoticeBody", ""));
                    objReadme.IsHTML = ToBool(XmlUtility.GetElementValue(objNode, "IsHTML", "false"));

                    objReadme.Save();
                    if (!TranslationDictionary.ContainsKey("README" + intOldNoticeID))
                    {
                        TranslationDictionary.Add("README" + intOldNoticeID, objReadme.ReadmeId.ToString());
                    }
                }
                else if (noticeGroup == 2)
                {
                    //AGREEMENT
                    objAgreement = new LicenseAgreement();
                    intOldNoticeID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID", ""));
                    objAgreement.StoreId = Token.Instance.StoreId;
                    objAgreement.DisplayName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "DisplayName", "Licence Agreement"));
                                        
                    objAgreement.AgreementText = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "NoticeBody", ""));
                    objAgreement.IsHTML = ToBool(XmlUtility.GetElementValue(objNode, "IsHTML", "false"));

                    objAgreement.Save();
                    if (!TranslationDictionary.ContainsKey("AGREEMENT" + intOldNoticeID))
                    {
                        TranslationDictionary.Add("AGREEMENT" + intOldNoticeID, objAgreement.LicenseAgreementId.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                String name = String.Empty;
                if (objAgreement != null) name = objAgreement.DisplayName;
                else if (objReadme != null) name = objReadme.DisplayName;
                LogError(GetPercentResult(), "Agreements &amp; Readmes Import Import, Agreement/Readme ID:" + intOldNoticeID + " , Name: " + name + ", " + ex.Message + "\n" + ex.StackTrace);
            }
        } //next objNode
        
        LogStatus(GetPercentResult(), "Agreements &amp; Readmes Import Complete...");
    }
    else
    {
        currentPhasePercent = currentPhaseWeightage;
        //LogStatus(GetPercentResult(), "No  Agreements &amp; Readmes Available to Import...");
    }
    		
    nlsItems = nodSourceStore.SelectNodes("ESD/Files/File");
    if (nlsItems.Count > 0)
    {
        int i = 0; // counter variable            
        LogStatus(GetPercentResult(), "Importing " + nlsItems.Count + " Files (ESD)");

        DigitalGood objFile = null;
        
        foreach (XmlNode objNode in nlsItems)
        {
            // update the import status
            i++;
            currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
            LogStatus(GetPercentResult(), "Importing Files (ESD) " + i + " of " + nlsItems.Count);
            int intOldFileID = 0;
            String fileName = String.Empty;
            try
            {
                fileName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "FileName", ""));
                intOldFileID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID", ""));

                if (fileName == "_GIFTCERT")
                {
                    GiftCertificate objGCFile = new GiftCertificate();

                    objGCFile.StoreId = Token.Instance.StoreId;
                    objGCFile.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "DisplayName", "File"));

                                    ////TODO:
                    ////String strRelativeTimeout = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "RelativeTimeout", ""));
                    ////String strAbsoluteTimeout = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "AbsoluteTimeout", ""));
                    ////if (!String.IsNullOrEmpty(strRelativeTimeout))
                    ////{
                    ////    objGCFile.DownloadTimeout = strRelativeTimeout;
                    ////}

                    ////if (!String.IsNullOrEmpty(strAbsoluteTimeout))
                    ////{
                    ////    objFile.ActivationTimeout = strAbsoluteTimeout;
                    ////}


                    ////objGCFile.MediaKey = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "MediaKey", ""));

                    ////TODO: objFile.Licensor_ID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "Licensor_ID", ""));
                    ////int licensorId = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "Licensor_ID", ""));
                    ////if(licensorId == 1) objFile.SerialKeyProviderId = SerialKeyProviderDataSource

                    ////	transfer agreement ID
                    ////int intOldAgreementID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "Agreement_ID", ""));
                    ////if (TranslationDictionary.ContainsKey("AGREEMENT" + intOldAgreementID))
                    ////{
                    ////    objFile.LicenseAgreementId = AlwaysConvert.ToInt(TranslationDictionary["AGREEMENT" + intOldAgreementID]);
                    ////}
                    //////	transfer readme ID
                    ////int intOldReadmeID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "Readme_ID", ""));
                    ////if (TranslationDictionary.ContainsKey("README" + intOldReadmeID))
                    ////{
                    ////    objFile.ReadmeId = AlwaysConvert.ToInt(TranslationDictionary["README" + intOldReadmeID]);
                    ////}

                    ////objFile.LicenseAgreementMode = ConvertLicenseAgreementMode(AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "ShowAgreement", "")));
                    ////if bPreserveIDs then
                    //////	'delete File
                    ////    Token.Instance.StoreGroupDB.QueryNoRS("DELETE FROM FILES WHERE File_ID=" & intOldFileID)
                    //////	'force the next id returned from maxidlookup
                    ////    Token.Instance.StoreGroupDB.QueryNoRS("UPDATE MAX_ID_LOOKUP SET Max_ID=" & (intOldFileID-1) & " WHERE TableName=" & FCast.FSQLString("FILES", intDBType))
                    ////end if
                    //	'save file
                    if (objGCFile.Save() != SaveResult.Failed)
                    {
                        if (!TranslationDictionary.ContainsKey("GIFTCERT" + intOldFileID))
                        {
                            TranslationDictionary.Add("GIFTCERT" + intOldFileID, objGCFile.GiftCertificateId.ToString());
                        }
                    //    //	'verify preserved ids
                    //    //if bPreserveIDs then
                    //    //    If (intOldFileID <> objFile.File_ID) Then
                    //    //    //	'force of ID was not successful
                    //    //        sWarnings.Append("Warning: ESD File " & intOldFileID & " did not have the ID successfully preserved.  The new ID is " & objFile.File_ID & ".")
                    //    //        //response.write(" !")
                    //    //    End If
                    //    //End If
                    //    //	'import FILE_EVENT_ASSN



                    //    //dim objFileEvent as cbFileEvent
                    //    //Dim colFileEvents as cbFileEventCollection
                    //    //colFileEvents = objFile.Events(objDestToken)
                    //    //nlsEvents = objNode.SelectNodes("Events/Event")
                    //    //for each nodEvent in nlsEvents
                    //    //    objFileEvent = New cbFileEvent
                    //    //    objFileEvent.Event_ID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodEvent, "ID", ""));
                    //    //    objFileEvent.Validate = ToBool(XmlUtility.GetElementValue(nodEvent, "Validate", ""));
                    //    //    objFileEvent.Fulfill = ToBool(XmlUtility.GetElementValue(nodEvent, "Fulfill", ""));
                    //    //    colFileEvents.Add(objFileEvent)
                    //    //    objFileEvent = null
                    //    //next nodEvent
                    //    //colFileEvents.SaveAsNew(objDestToken, objFile.File_ID)
                    //    //colFileEvents = null
                    //    bool activationModeImported = false;
                    //    bool fullFillmentModeImported = false;


                    //    XmlNodeList nlsEvents = objNode.SelectNodes("Events/Event");
                    //    foreach (XmlNode eventNode in nlsEvents)
                    //    {
                    //        int oldEventId = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(eventNode, "ID", ""));
                    //        bool validate = ToBool(XmlUtility.GetElementValue(eventNode, "Validate", ""));
                    //        bool fulfill = ToBool(XmlUtility.GetElementValue(eventNode, "Fulfill", ""));
                    //        if (validate)
                    //        {
                    //            if (!activationModeImported)
                    //            {
                    //                switch (oldEventId)
                    //                {
                    //                    case 1:    // On Order 
                    //                        objFile.ActivationMode = ActivationMode.OnOrder; break;
                    //                    case 2: // On Order Paid Full
                    //                    case 3:  // On Order Full payment 
                    //                        objFile.ActivationMode = ActivationMode.OnPaidOrder; break;
                    //                    default:
                    //                        objFile.ActivationMode = ActivationMode.Manual;
                    //                        // LOG Warning message
                    //                        LogWarning(GetPercentResult(), "ESD file import, DigitalGood activation/validation mode may not be imported correctly, setting it to Manual mode. Digital Good Name: " + objFile.Name + ", Digital Good Id: " + objFile.DigitalGoodId + ", Old ESD file Id:" + intOldFileID);
                    //                        break;
                    //                }
                    //                activationModeImported = true;
                    //            }
                    //            else LogWarning(GetPercentResult(), "ESD file import, DigitalGood automatic activation/validation on multiple store events not supported in AC7, validation mode is set to: " + objFile.ActivationMode.ToString() + ". Digital Good Name: " + objFile.Name + ", Digital Good Id: " + objFile.DigitalGoodId + ", Old ESD file Id:" + intOldFileID);
                    //        }

                    //        if (fulfill)
                    //        {
                    //            if (!fullFillmentModeImported)
                    //            {
                    //                // SET IT TO AUTO FULLFILMENT BY DEFAULT
                    //                objFile.UseAutomaticFulfillment = true;
                    //                LogWarning(GetPercentResult(), "ESD file import, DigitalGood automatic fullfilment setting may not be imported correctly, fullfilment mode is set to: " + (objFile.UseAutomaticFulfillment ? "UseAutomaticFulfillment" : "UseManualFulfillment") + ". Digital Good Name: " + objFile.Name + ", Digital Good Id: " + objFile.DigitalGoodId + ", Old ESD file Id:" + intOldFileID);
                    //                fullFillmentModeImported = true;
                    //            }
                    //            else LogWarning(GetPercentResult(), "ESD file import, DigitalGood automatic fullfilment on store events not supported in AC7, fullfilment mode is set to: " + (objFile.UseAutomaticFulfillment ? "UseAutomaticFulfillment" : "UseManualFulfillment") + ". Digital Good Name: " + objFile.Name + ", Digital Good Id: " + objFile.DigitalGoodId + ", Old ESD file Id:" + intOldFileID);

                    //        }

                    //    }

                    //    //	'import LICENSEKEYS
                    //    XmlNodeList nlsLicenses = objNode.SelectNodes("LicenseKeys/LicenseKey");
                    //    foreach (XmlNode nodLicense in nlsLicenses)
                    //    {
                    //        String licenseKey = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodLicense, "LicenseKey", ""));
                    //        SerialKey key = new SerialKey();
                    //        key.SerialKeyData = licenseKey;
                    //        key.DigitalGoodId = objFile.DigitalGoodId;
                    //        key.Save();
                    //        if (!String.IsNullOrEmpty(licenseKey)) objFile.SerialKeys.Add(key);
                    //    }

                    //    //SAVE 
                    //    objFile.Save();
                    //    objFile.SerialKeys.Save();
                   

                    }
                    else
                    {
                        LogError(GetPercentResult(), "Files (Giftcertificate) Import, Error while saving, File (Giftcertificate) ID:" + intOldFileID + " , Name: " + fileName);
                    }

                }
                else
                {
                    objFile = new DigitalGood();
                    
                    objFile.StoreId = Token.Instance.StoreId;
                    objFile.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "DisplayName", "File"));
                    objFile.FileName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "SaveAsName", ""));


                    if (objFile.FileName.Length > 100)
                    {
                        // LOG THE WARNING MESSAGE AND SKIP THE FILE NAME IMPORT
                        LogWarning(GetPercentResult(), "DigitalGood(File) import, FileName: '" + objFile.FileName + "' should not be longer then 100 characters, FileName import is skipped. Leaving for you to manually adjust the file path. Old File Id:" + intOldFileID);
                        objFile.FileName = String.Empty;
                    }
                    
                    objFile.ServerFileName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "FileName", ""));

                    objFile.MaxDownloads = (byte)AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "MaxDownloads", ""));

                    //TODO:
                    String strRelativeTimeout = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "RelativeTimeout", ""));
                    String strAbsoluteTimeout = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "AbsoluteTimeout", ""));
                    if (!String.IsNullOrEmpty(strRelativeTimeout))
                    {
                        objFile.DownloadTimeout = strRelativeTimeout;
                    }

                    if (!String.IsNullOrEmpty(strAbsoluteTimeout))
                    {
                        objFile.ActivationTimeout = strAbsoluteTimeout;
                    }


                    objFile.MediaKey = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "MediaKey", ""));

                    //Licensor_id = 1 is the default provider.  Any other ID should be ignored and not mapped.
                    int licensorId = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "Licensor_ID", ""));
                    if (licensorId == 1) objFile.SerialKeyProviderId = Misc.GetClassId(typeof(DefaultSerialKeyProvider));

                    //	transfer agreement ID
                    int intOldAgreementID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "Agreement_ID", ""));
                    if (TranslationDictionary.ContainsKey("AGREEMENT" + intOldAgreementID))
                    {
                        objFile.LicenseAgreementId = AlwaysConvert.ToInt(TranslationDictionary["AGREEMENT" + intOldAgreementID]);
                    }
                    //	transfer readme ID
                    int intOldReadmeID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "Readme_ID", ""));
                    if (TranslationDictionary.ContainsKey("README" + intOldReadmeID))
                    {
                        objFile.ReadmeId = AlwaysConvert.ToInt(TranslationDictionary["README" + intOldReadmeID]);
                    }

                    objFile.LicenseAgreementMode = ConvertLicenseAgreementMode(AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "ShowAgreement", "")));
                    //if bPreserveIDs then
                    ////	'delete File
                    //    Token.Instance.StoreGroupDB.QueryNoRS("DELETE FROM FILES WHERE File_ID=" & intOldFileID)
                    ////	'force the next id returned from maxidlookup
                    //    Token.Instance.StoreGroupDB.QueryNoRS("UPDATE MAX_ID_LOOKUP SET Max_ID=" & (intOldFileID-1) & " WHERE TableName=" & FCast.FSQLString("FILES", intDBType))
                    //end if
                    //	'save file
                    if (objFile.Save() != SaveResult.Failed)
                    {
                        if (!TranslationDictionary.ContainsKey("FILE" + intOldFileID))
                        {
                            TranslationDictionary.Add("FILE" + intOldFileID, objFile.DigitalGoodId.ToString());
                        }

                        // BUILD A LIST OF EVENT NAMES FOR TRANSLATION
                        Dictionary<int, string> ac5EventNames = new Dictionary<int, string>();
                        ac5EventNames.Add(0, "Manual");
                        ac5EventNames.Add(1, "On Order");
                        ac5EventNames.Add(2, "On Order (Paid Full)");
                        ac5EventNames.Add(23, "On Order (Paid Partial)");
                        ac5EventNames.Add(3, "On Full Payment");
                        ac5EventNames.Add(4, "On Partial Payment");
                        ac5EventNames.Add(24, "On Negative Balance");
                        ac5EventNames.Add(25, "On Positive Balance");
                        ac5EventNames.Add(26, "On Zero Balance");
                        ac5EventNames.Add(11, "On Complete Order Shipment");
                        ac5EventNames.Add(12, "On Partial Order Shipment");
                        ac5EventNames.Add(15, "On File Added");
                        ac5EventNames.Add(16, "On File Validated");
                        ac5EventNames.Add(17, "On File Fulfilled");

                        List<int> validateEvents = new List<int>();
                        List<int> fulfillEvents = new List<int>();

                        XmlNodeList nlsEvents = objNode.SelectNodes("Events/Event");
                        foreach (XmlNode eventNode in nlsEvents)
                        {
                            int oldEventId = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(eventNode, "ID", ""));
                            bool validate = ToBool(XmlUtility.GetElementValue(eventNode, "Validate", "0"));
                            if (validate) validateEvents.Add(oldEventId);
                            bool fulfill = ToBool(XmlUtility.GetElementValue(eventNode, "Fulfill", "0"));
                            if (fulfill) fulfillEvents.Add(oldEventId);
                        }

                        if (validateEvents.Count == 0)
                        {
                            // AC5X VALIDATION WAS MANUAL
                            objFile.ActivationMode = ActivationMode.Manual;
                        }
                        else if (validateEvents.IndexOf(1) > -1)
                        {
                            // AC5X VALIDATION INCLUDED THE ONORDER EVENT, TREAT THIS AS ONORDER VALIDATION
                            objFile.ActivationMode = ActivationMode.OnOrder;
                        }
                        else if (validateEvents.IndexOf(2) > -1 && validateEvents.IndexOf(3) > -1)
                        {
                            // AC5X VALIDATION INCLUDED ON PAID ORDER AND ON FULL PAYMENT EVENTS, TREAT THIS AS ON PAID ORDER VALIDATION
                            objFile.ActivationMode = ActivationMode.OnPaidOrder;
                        }
                        else
                        {
                            // AC5X VALIDATION HAS NO EQUIVALENT
                            objFile.ActivationMode = ActivationMode.Manual;
                            string warningMessage = "Warning: Import of digital good \"{0}\" should be verified.  AC5 data indicates download activation on the following events: {1}.  There is no AC7 equivalent and the digital good has been set to manual activation. (Old File Id: {2})";
                            warningMessage = string.Format(warningMessage, objFile.Name, GetEventNames(validateEvents, ac5EventNames), intOldFileID);
                            LogWarning(GetPercentResult(), warningMessage);
                        }

                        if (fulfillEvents.Count == 0)
                        {
                            // AC5X VALIDATION WAS MANUAL
                            objFile.FulfillmentMode = FulfillmentMode.Manual;
                        }
                        else if (fulfillEvents.IndexOf(1) > -1)
                        {
                            // AC5X VALIDATION INCLUDED THE ONORDER EVENT, TREAT THIS AS ONORDER VALIDATION
                            objFile.FulfillmentMode = FulfillmentMode.OnOrder;
                        }
                        else if (fulfillEvents.IndexOf(2) > -1 && fulfillEvents.IndexOf(3) > -1)
                        {
                            // AC5X VALIDATION INCLUDED ON PAID ORDER AND ON FULL PAYMENT EVENTS, TREAT THIS AS ON PAID ORDER VALIDATION
                            objFile.FulfillmentMode = FulfillmentMode.OnPaidOrder;
                        }
                        else
                        {
                            // AC5X VALIDATION HAS NO EQUIVALENT
                            objFile.FulfillmentMode = FulfillmentMode.Manual;
                            string warningMessage = "Warning: Import of digital good \"{0}\" should be verified.  AC5 data indicates fulfillment on the following events: {1}.  There is no AC7 equivalent and the digital good has been set to manual fulfillment. (Old File Id: {2}";
                            warningMessage = string.Format(objFile.Name, GetEventNames(fulfillEvents, ac5EventNames), intOldFileID);
                            LogWarning(GetPercentResult(), warningMessage);
                        }

                        // IMPORT LICENSEKEYS
                        XmlNodeList nlsLicenses = objNode.SelectNodes("LicenseKeys/LicenseKey");
                        foreach (XmlNode nodLicense in nlsLicenses)
                        {
                            String licenseKey = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodLicense, "LicenseKey", ""));
                            if (!string.IsNullOrEmpty(licenseKey))
                            {
                                SerialKey key = new SerialKey();
                                key.SerialKeyData = licenseKey;
                                key.DigitalGoodId = objFile.DigitalGoodId;
                                key.Save();
                                objFile.SerialKeys.Add(key);
                            }
                        }

                        //SAVE 
                        objFile.Save();
                        objFile.SerialKeys.Save();
                    }
                    else
                    {
                        LogError(GetPercentResult(), "Files (ESD) Import, Error while saving, File (ESD) ID:" + intOldFileID + " , Name: " + objFile.Name);
                    }
                }



            }
            catch (Exception ex)
            {                
                LogError(GetPercentResult(), "Files (ESD) Import, Files (ESD) ID:" + intOldFileID + " , Name: " + fileName + ", " + ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
    else
    {
        currentPhasePercent = currentPhaseWeightage;
        //LogStatus(GetPercentResult(), "No Files (ESD) Available to Import...");
    }
    
    
/*    XmlNodeList nlsItems = nodSourceStore.SelectNodes("ESD/Files/File")
    if( nlsItems.Count > 0){
        //response.write("Importing " & nlsItems.Count & " Files (ESD) .")
				
        Dim objFile as cbFile
        foreach(XmlNode objNode in nlsItems){
            objFile = new cbFile()
            intOldFileID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID"))
            objFile.StoreId = Token.Instance.StoreId
            objFile.DisplayName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "DisplayName", "File"))
            objFile.FileName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "FileName"))
            objFile.SaveAsName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "SaveAsName"))
            objFile.MaxDownloads = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "MaxDownloads"))
            objFile.SetRelativeTimeout(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "RelativeTimeout")))
            objFile.SetAbsoluteTimeout(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "AbsoluteTimeout")))
            objFile.MediaKey = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "MediaKey"))
            objFile.Licensor_ID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "Licensor_ID"))
        //	'transfer agreement ID
            intOldNoticeID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "Agreement_ID"))
            if (TranslationDictionary.ContainsKey("NOTICE" & intOldNoticeID) then
                objFile.Agreement_ID = AlwaysConvert.ToInt(objTranslate("NOTICE" & intOldNoticeID))
            else
                objFile.Agreement_ID = 0
            end if
        //	'transfer readme ID
            intOldNoticeID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "Readme_ID"))
            if (TranslationDictionary.ContainsKey("NOTICE" & intOldNoticeID) then
                objFile.Readme_ID = AlwaysConvert.ToInt(objTranslate("NOTICE" & intOldNoticeID))
            else
                objFile.Readme_ID = 0
            end if
            objFile.ShowAgreement = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "ShowAgreement"))
            if bPreserveIDs then
            //	'delete File
                Token.Instance.StoreGroupDB.QueryNoRS("DELETE FROM FILES WHERE File_ID=" & intOldFileID)
            //	'force the next id returned from maxidlookup
                Token.Instance.StoreGroupDB.QueryNoRS("UPDATE MAX_ID_LOOKUP SET Max_ID=" & (intOldFileID-1) & " WHERE TableName=" & FCast.FSQLString("FILES", intDBType))
            end if
        //	'save file
            if objFile.Save() Then
                objTranslate.Item("FILE" & intOldFileID) = objFile.File_ID
            //	'verify preserved ids
                if bPreserveIDs then
                    If (intOldFileID <> objFile.File_ID) Then
                    //	'force of ID was not successful
                        sWarnings.Append("Warning: ESD File " & intOldFileID & " did not have the ID successfully preserved.  The new ID is " & objFile.File_ID & ".")
                        //response.write(" !")
                    End If
                End If
            //	'import FILE_EVENT_ASSN
                dim objFileEvent as cbFileEvent
                Dim colFileEvents as cbFileEventCollection
                colFileEvents = objFile.Events(objDestToken)
                nlsEvents = objNode.SelectNodes("Events/Event")
                for each nodEvent in nlsEvents
                    objFileEvent = New cbFileEvent
                    objFileEvent.Event_ID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodEvent, "ID"))
                    objFileEvent.Validate = ToBool(XmlUtility.GetElementValue(nodEvent, "Validate"))
                    objFileEvent.Fulfill = ToBool(XmlUtility.GetElementValue(nodEvent, "Fulfill"))
                    colFileEvents.Add(objFileEvent)
                    objFileEvent = null
                next nodEvent
                colFileEvents.SaveAsNew(objDestToken, objFile.File_ID)
                colFileEvents = null
            //	'import LICENSEKEYS
                Dim nlsLicenses as XmlNodeList = objNode.SelectNodes("LicenseKeys/LicenseKey")
                If nlsLicenses.Count > 0 Then
                    Dim nodLicense as XmlNode
                    Dim arrLicenseKeys() As String
                    Dim intIndex As Integer = 0
                    Redim arrLicenseKeys(nlsLicenses.Count - 1)
                    For Each nodLicense In nlsLicenses
                        arrLicenseKeys(intIndex) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodLicense, "LicenseKey"))
                        intIndex += 1
                    Next nodLicense
                //	'GET THE ESD LICENSOR AND ADD LICENSES
                    Dim objLicensor As New MakerShop5.ESD.Licensors.cbMakerShop()
                    objLicensor.AddLicenses(objDestToken, objFile.File_ID, arrLicenseKeys)
                End If
            end if
            //response.write(" .")
					
        } //next objNode
        objFile = null
        //response.write("")
				

    else
        Status(GetPercentResult(), "No Files (ESD) Available to Import...")				
    end if

    XmlNodeList nlsItems = nodSourceStore.SelectNodes("ESD/FileEmails/FileEmail")
    if( nlsItems.Count > 0){
        //response.write("Importing " & nlsItems.Count & " File Email Associations (ESD) .")
				
        Dim objFileEmail as cbFileEmail
        foreach(XmlNode objNode in nlsItems){
        //	'transfer file ID
            intOldFileID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "File_ID"))
            if (TranslationDictionary.ContainsKey("FILE" & intOldFileID) then
                intNewFileID = AlwaysConvert.ToInt(objTranslate("FILE" & intOldFileID))
            else
                intNewFileID = 0
            end if
        //	'transfer envelope ID
            intOldEnvelopeID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "Envelope_ID"))
            if (TranslationDictionary.ContainsKey("EMAILENVELOPE" & intOldEnvelopeID) then
                intNewEnvelopeID = AlwaysConvert.ToInt(objTranslate("EMAILENVELOPE" & intOldEnvelopeID))
            else
                intNewEnvelopeID = 0
            end if
        //	'transfer content ID
            intOldContentID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "Content_ID"))
            if (TranslationDictionary.ContainsKey("EMAILCONTENT" & intOldContentID) then
                intNewContentID = AlwaysConvert.ToInt(objTranslate("EMAILCONTENT" & intOldContentID))
            else
                intNewContentID = 0
            end if
            objFileEmail = new cbFileEmail()
            objFileEmail.File_ID = intNewFileID
            objFileEmail.Envelope_ID = intNewEnvelopeID
            objFileEmail.Content_ID = intNewContentID
            objFileEmail.EmailTrigger = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "EmailTrigger"))
            objFileEmail.Save()

            //response.write(" .")
					
        } //next objNode
        objFileEmail = null
        //response.write("")
				
    else
        Status(GetPercentResult(), "No File Email Associations (ESD) Available to Import...")
				
    end if
else
    //response.write("Skipping ESD Import...")
			
end if
     * */

        }

        /// <summary>
        /// Converts an ID list into comma delimited string representation
        /// </summary>
        /// <param name="eventIDs">List of IDs</param>
        /// <param name="eventNames">Equivalent Names</param>
        /// <returns></returns>
        private string GetEventNames(List<int> eventIds, Dictionary<int, string> eventNames)
        {
            if (eventIds == null || eventIds.Count == 0) return string.Empty;
            List<string> translatedNames = new List<string>();
            foreach (int eventId in eventIds)
            {
                if (eventNames.ContainsKey(eventId))
                {
                    translatedNames.Add(eventNames[eventId]);
                }
            }
            if (translatedNames.Count == 0) return string.Empty;
            return string.Join(",", translatedNames.ToArray());
        }

        #endregion

        #region Gateways

        private void ImportGateways(XmlNode nodSourceStore)
        {/*
//if InStr(lstCopyOpts, ",GATEWAYS,") > 0 then
//	'groups selected for copy
    //response.write("Checking Payment Gateways...")
			
    XmlNodeList nlsItems = nodSourceStore.SelectNodes("PayGateways/PayGateway")
    if( nlsItems.Count > 0){
        //response.write("Importing " & nlsItems.Count & " Payment Gateways .")
				
        Dim objPayGateway As cbStorePayGateway
        foreach(XmlNode objNode in nlsItems){
            intOldGatewayID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID"))
            sOldGatewayClassID = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "ClassID"))
        //	'check if old gateway ID is correct
            intNewGatewayID = AlwaysConvert.ToInt(Token.Instance.StoreGroupDB.QueryValue("SELECT PayGateway_ID FROM PAYGATEWAYS WHERE PayGateway_ID=" & intOldGatewayID & " AND ClassID=" & FCast.FSQLString(sOldGatewayClassID, Token.Instance.StoreGroupDB.DBType)))
            if intNewGatewayID = 0 then
            //	'gateway not found, check for any matches to class ID
                intNewGatewayID = AlwaysConvert.ToInt(Token.Instance.StoreGroupDB.QueryValue("SELECT PayGateway_ID FROM PAYGATEWAYS WHERE ClassID=" & FCast.FSQLString(sOldGatewayClassID, Token.Instance.StoreGroupDB.DBType)))
            end if
            if intNewGatewayID > 0 then
            //	'check if gateway already defined
                if AlwaysConvert.ToInt(Token.Instance.StoreGroupDB.QueryValue("SELECT Count(*) as NumGW FROM STORE_PAYGATEWAY_ASSN WHERE Store_ID=" & Token.Instance.StoreId & " AND PayGateway_ID=" & intNewGatewayID)) = 0 then
                //	'no settings present, import them
                    objPayGateway = new cbStorePayGateway()
                    objPayGateway.PayGateway_ID = intNewGatewayID
                    objPayGateway.GWParam(0) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam0"))
                    objPayGateway.GWParam(1) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam1"))
                    objPayGateway.GWParam(2) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam2"))
                    objPayGateway.GWParam(3) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam3"))
                    objPayGateway.GWParam(4) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam4"))
                    objPayGateway.GWParam(5) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam5"))
                    objPayGateway.GWParam(6) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam6"))
                    objPayGateway.GWParam(7) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam7"))
                    objPayGateway.GWParam(8) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam8"))
                    objPayGateway.GWParam(9) = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "GWParam9"))
                    objPayGateway.Save(objDestToken)
                    objPayGateway = null
                end if
            //	'ADD TRANSLATION
                objTranslate.Item("PAYGATEWAY" & intOldGatewayID) = intNewGatewayID
            end if
            //response.write(" .")
					
        } //next objNode
        //response.write("")
				
    else
        Status(GetPercentResult(), "No Payment Gateways Available to Import...")
				
    end if
			
    //response.write("Checking Shipping Gateways...")
			
    XmlNodeList nlsItems = nodSourceStore.SelectNodes("ShipGateways/ShipGateway")
    if( nlsItems.Count > 0){
        //response.write("Importing " & nlsItems.Count & " Shipment Gateways .")
				
        Dim objShipGateway As cbStoreShipGateway
        foreach(XmlNode objNode in nlsItems){
            intOldGatewayID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID"))
            sOldGatewayClassID = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "ClassID"))
        //	'check if old gateway ID is correct
            intNewGatewayID = AlwaysConvert.ToInt(Token.Instance.StoreGroupDB.QueryValue("SELECT ShipGateway_ID FROM SHIPGATEWAYS WHERE ShipGateway_ID=" & intOldGatewayID & " AND ClassID=" & FCast.FSQLString(sOldGatewayClassID, Token.Instance.StoreGroupDB.DBType)))
            if intNewGatewayID = 0 then
            //	'gateway not found, check for any matches to class ID
                intNewGatewayID = AlwaysConvert.ToInt(Token.Instance.StoreGroupDB.QueryValue("SELECT ShipGateway_ID FROM SHIPGATEWAYS WHERE ClassID=" & FCast.FSQLString(sOldGatewayClassID, Token.Instance.StoreGroupDB.DBType)))
            end if
            if intNewGatewayID > 0 then
            //	'check if gateway already defined
                if AlwaysConvert.ToInt(Token.Instance.StoreGroupDB.QueryValue("SELECT Count(*) as NumGW FROM STORE_SHIPGATEWAY_ASSN WHERE Store_ID=" & Token.Instance.StoreId & " AND ShipGateway_ID=" & intNewGatewayID)) = 0 then
                //	'no settings present, import them
                    objShipGateway = new cbStoreShipGateway()
                    objShipGateway.ShipGateway_ID = intNewGatewayID
                    objShipGateway.GWParam(0) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam0"))
                    objShipGateway.GWParam(1) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam1"))
                    objShipGateway.GWParam(2) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam2"))
                    objShipGateway.GWParam(3) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam3"))
                    objShipGateway.GWParam(4) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam4"))
                    objShipGateway.GWParam(5) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam5"))
                    objShipGateway.GWParam(6) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam6"))
                    objShipGateway.GWParam(7) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam7"))
                    objShipGateway.GWParam(8) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam8"))
                    objShipGateway.GWParam(9) = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "GWParam9"))
                    objShipGateway.Save(objDestToken)
                    objShipGateway = null
                end if
            //	'update ID translation
                if not objTranslate.ContainsKey("SHIPGATEWAY" & intOldGatewayID) then
                    objTranslate.Add("SHIPGATEWAY" & intOldGatewayID, intNewGatewayID)
                end if
            end if
            //response.write(" .")
					
        } //next objNode
        //response.write("")
				
    else
        Status(GetPercentResult(), "No Shipping Gateways Available to Import...")
				
    end if

    //response.write("Checking Email Gateways...")
			
    XmlNodeList nlsItems = nodSourceStore.SelectNodes("EmailGateways/EmailGateway")
    if( nlsItems.Count > 0){
        //response.write("Importing " & nlsItems.Count & " Email Gateways .")
				
        Dim objEmailGateway As cbStoreEmailGateway
        foreach(XmlNode objNode in nlsItems){
            intOldGatewayID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID"))
            sOldGatewayClassID = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "ClassID"))
        //	'check if old gateway ID is correct
            intNewGatewayID = AlwaysConvert.ToInt(Token.Instance.StoreGroupDB.QueryValue("SELECT EmailGateway_ID FROM EMAILGATEWAYS WHERE EmailGateway_ID=" & intOldGatewayID & " AND ClassID=" & FCast.FSQLString(sOldGatewayClassID, Token.Instance.StoreGroupDB.DBType)))
            if intNewGatewayID = 0 then
            //	'gateway not found, check for any matches to class ID
                intNewGatewayID = AlwaysConvert.ToInt(Token.Instance.StoreGroupDB.QueryValue("SELECT EmailGateway_ID FROM EMAILGATEWAYS WHERE ClassID=" & FCast.FSQLString(sOldGatewayClassID, Token.Instance.StoreGroupDB.DBType)))
            end if
            if intNewGatewayID > 0 then
            //	'check if gateway already defined
                if AlwaysConvert.ToInt(Token.Instance.StoreGroupDB.QueryValue("SELECT Count(*) as NumGW FROM STORE_EMAILGATEWAY_ASSN WHERE Store_ID=" & Token.Instance.StoreId & " AND EmailGateway_ID=" & intNewGatewayID)) = 0 then
                //	'no settings present, import them
                    objEmailGateway = new cbStoreEmailGateway()
                    objEmailGateway.EmailGateway_ID = intNewGatewayID
                    objEmailGateway.GWParam(0) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam0"))
                    objEmailGateway.GWParam(1) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam1"))
                    objEmailGateway.GWParam(2) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam2"))
                    objEmailGateway.GWParam(3) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam3"))
                    objEmailGateway.GWParam(4) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam4"))
                    objEmailGateway.GWParam(5) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam5"))
                    objEmailGateway.GWParam(6) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam6"))
                    objEmailGateway.GWParam(7) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam7"))
                    objEmailGateway.GWParam(8) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam8"))
                    objEmailGateway.GWParam(9) = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam9"))
                    objEmailGateway.Save(objDestToken)
                    objEmailGateway = null
                end if
            //	'update ID translation
                if not objTranslate.ContainsKey("EMAILGATEWAY" & intOldGatewayID) then
                    objTranslate.Add("EMAILGATEWAY" & intOldGatewayID, intNewGatewayID)
                end if
            end if
            //response.write(" .")
					
        } //next objNode
        //response.write("")
				
    else
        Status(GetPercentResult(), "No Email Gateways Available to Import...")
				
    end if

    //response.write("Checking Tax Gateways...")
			
    XmlNodeList nlsItems = nodSourceStore.SelectNodes("TaxGateways/TaxGateway")
    if( nlsItems.Count > 0){
        //response.write("Importing " & nlsItems.Count & " Tax Gateways .")
				
        foreach(XmlNode objNode in nlsItems){
            intOldGatewayID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID"))
            sOldGatewayClassID = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "ClassID"))
        //	'check if old gateway ID is correct
            intNewGatewayID = AlwaysConvert.ToInt(Token.Instance.StoreGroupDB.QueryValue("SELECT TaxGateway_ID FROM TAXGATEWAYS WHERE TaxGateway_ID=" & intOldGatewayID & " AND ClassID=" & FCast.FSQLString(sOldGatewayClassID, Token.Instance.StoreGroupDB.DBType)))
            if intNewGatewayID = 0 then
            //	'gateway not found, check for any matches to class ID
                intNewGatewayID = AlwaysConvert.ToInt(Token.Instance.StoreGroupDB.QueryValue("SELECT TaxGateway_ID FROM TAXGATEWAYS WHERE ClassID=" & FCast.FSQLString(sOldGatewayClassID, Token.Instance.StoreGroupDB.DBType)))
            end if
            if intNewGatewayID > 0 then
            //	'check if gateway already defined
                if AlwaysConvert.ToInt(Token.Instance.StoreGroupDB.QueryValue("SELECT Count(*) as NumGW FROM STORE_TAXGATEWAY_ASSN WHERE Store_ID=" & Token.Instance.StoreId & " AND TaxGateway_ID=" & intNewGatewayID)) = 0 then
                //	'no settings present, import them
                //	'MUST IMPORT DIRECTLY TO DB, CLASS TO HANDLE THIS RECORD MISSING
                    strSQL = New Text.StringBuilder()
                    strSQL.Append("INSERT INTO STORE_TAXGATEWAY_ASSN(Store_ID,TaxGateway_ID,GWParam0,GWParam1,GWParam2,GWParam3,GWParam4,GWParam5,GWParam6,GWParam7,GWParam8,GWParam9,Activated) VALUES(")
                    strSQL.Append(Token.Instance.StoreId & "," & intNewGatewayID & ",")
                    strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam0")), intDBType) & ",")
                    strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam1")), intDBType) & ",")
                    strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam2")), intDBType) & ",")
                    strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam3")), intDBType) & ",")
                    strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam4")), intDBType) & ",")
                    strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam5")), intDBType) & ",")
                    strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam6")), intDBType) & ",")
                    strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam7")), intDBType) & ",")
                    strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam8")), intDBType) & ",")
                    strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWParam9")), intDBType) & ",")
                    strSQL.Append(AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "Activated")).ToString() & ")")
                    intResult = Token.Instance.StoreGroupDB.QueryNoRS(strSQL.ToString())
                end if
            //	'update ID translation
                if not objTranslate.ContainsKey("TAXGATEWAY" & intOldGatewayID) then
                    objTranslate.Add("TAXGATEWAY" & intOldGatewayID, intNewGatewayID)
                end if
            end if
            //response.write(" .")
					
        } //next objNode
        //response.write("")
				
    else
        Status(GetPercentResult(), "No Tax Gateways Available to Import...")
				
    end if

else
    //response.write("Skipping Gateways Import...")
			
end if
     * */
        }


        #endregion

        private void ImportGroups(XmlNode nodSourceStore)
        {

            currentPhasePercent = 0;
            //LogStatus(GetPercentResult(), "Checking Groups...");

            XmlNodeList nlsItems = nodSourceStore.SelectNodes("Groups/Group");
            if (nlsItems.Count > 0)
            {
                int i = 0; // counter variable            
                LogStatus(GetPercentResult(), "Importing " + nlsItems.Count + " Groups .");

                MakerShop.Users.Group objGroup = null;
                int intOldGroupID = 0;
                foreach (XmlNode objNode in nlsItems)
                {

                    // update the import status
                    i++;
                    currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
                    LogStatus(GetPercentResult(), "Importing Group " + i + " of " + nlsItems.Count);

                    try
                    {
                        objGroup = new MakerShop.Users.Group();
                        intOldGroupID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID", ""), 0);

                        objGroup.StoreId = Token.Instance.StoreId;
                        objGroup.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Name", "Group"));
                        objGroup.Description = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Description", ""));
                        objGroup.Save();
                        TranslationDictionary.Add("GROUP" + intOldGroupID, objGroup.GroupId.ToString());
                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "Groups Import, GroupID:" + intOldGroupID + " , Name: " + objGroup.Name + ", " + ex.Message + "\n" + ex.StackTrace);
                    }
                    LogStatus(GetPercentResult(), "Groups Import Complete...");

                } //next objNode
                objGroup = null;

            }
            else
            {
                //LogStatus(GetPercentResult(), "No Groups Available to Import...");

            }
        }

        private void ImportLinks(XmlNode nodSourceStore)
        {
            currentPhasePercent = 0;
            //LogStatus(GetPercentResult(), "Checking Links...");

            XmlNodeList nlsItems = nodSourceStore.SelectNodes("Links/Link");
            if (nlsItems.Count > 0)
            {
                int i = 0; // counter variable
                LogStatus(GetPercentResult(), "Importing " + nlsItems.Count + " Links .");

                Link objLink = null;
                int intOldLinkID = 0;
                foreach (XmlNode objNode in nlsItems)
                {
                    // update the import status
                    i++;

                    currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
                    LogStatus(GetPercentResult(), "Importing Link " + i + " of " + nlsItems.Count);

                    try
                    {
                        objLink = new Link();
                        intOldLinkID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID", "0"), 0);
                        objLink.StoreId = Token.Instance.StoreId;
                        objLink.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Name", ""));
                        // Comments:
                        /*
                        Explanation: In AC6, we will provide the option to have a display page 
                        as an exit point for the site. So the display page might say something 
                        like �You are about to leave our site, we are not responsible for the 
                        content�� and provide the link from NavigateUrl. By default it still works 
                        like before� the URL Rewriter uses the NavigateUrl as the display page value 
                        unless something else is specified.
                        */
                        objLink.TargetUrl = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "DisplayPage", ""));
                        objLink.TargetWindow = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Target", ""));
                        objLink.Summary = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Description1", ""));
                        objLink.ThumbnailUrl = FormatImgURL(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Image1", "")));
                        objLink.ThumbnailAltText = GetAltText(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Image1Params", "")));
                        ////objLink.NavigateUrl = ToBool(XmlUtility.GetElementValue(objNode, "ExternalLink", ""));
                        objLink.Visibility = ConvertCatalogNodeVisibility(AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "Active", "1")));
                        objLink.Save();
                        if (!TranslationDictionary.ContainsKey("LINK" + intOldLinkID))
                        {
                            TranslationDictionary.Add("LINK" + intOldLinkID, objLink.LinkId.ToString());
                        }

                        // ASSOCIATE CATEGORIES
                        String lstOldCategories = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Categories", ""));
                        if (lstOldCategories.Length > 0)
                        {
                            String[] arrOldCategories = lstOldCategories.Split(',');
                            foreach (String strCatId in arrOldCategories)
                            {
                                int intOldCategoryID = AlwaysConvert.ToInt(strCatId);
                                if (TranslationDictionary.ContainsKey("CATEGORY" + intOldCategoryID))
                                {
                                    objLink.Categories.Add(AlwaysConvert.ToInt(TranslationDictionary["CATEGORY" + intOldCategoryID]));
                                }
                                else
                                {
                                    LogError(GetPercentResult(), "Link Import, Unable to associate Link to Category because Category not found. LinkID:" + intOldLinkID + ", Name:" + objLink.Name + ", CategoryID:" + intOldCategoryID);
                                }
                            }
                            objLink.Categories.LinkId = objLink.LinkId;
                            objLink.Categories.Save();

                            // MAKE SURE IT IS ASSIGNED IN AT LEAST ONE CATEOGRY
                            if (objLink.Categories.Count == 0)
                            {
                                objLink.Categories.Add(OrphanedItemsCategoryId);
                                objLink.Categories.LinkId = objLink.LinkId;
                                objLink.Categories.Save();
                            }
                        }
                        else
                        {
                            // IT IS ORPHANED Link, ADD IT TO ROOT CATEGORY
                            objLink.Categories.Add(OrphanedItemsCategoryId);
                            objLink.Categories.LinkId = objLink.LinkId;
                            objLink.Categories.Save();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "Links Import, LinkID:" + intOldLinkID + " , Name: " + objLink.Name + ", " + ex.Message + "\n" + ex.StackTrace);
                    }

                } //next objNode
                objLink = null;
                LogStatus(GetPercentResult(), "Links Import Complete...");

            }
            else
            {
                currentPhasePercent = currentPhaseWeightage;
                //LogStatus(GetPercentResult(), "No Links Available to Import...");
            }
        }

        private void ImportOrderStatus(XmlNode nodSourceStore)
        {
            currentPhasePercent = 0;
            //LogStatus(GetPercentResult(), "Checking Order Status...");

            XmlNodeList nlsItems = nodSourceStore.SelectNodes("OrderStatuses/OrderStatus");
            if (nlsItems.Count > 0)
            {

                int i = 0; // counter variable
                LogStatus(GetPercentResult(), "Importing " + nlsItems.Count + " Order Statuses .");

                OrderStatus objOrderStatus = null;
                int intOldOrderStatusID = 0;
                foreach (XmlNode objNode in nlsItems)
                {

                    // update the import status
                    i++;

                    currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
                    LogStatus(GetPercentResult(), "Importing Order Status " + i + " of " + nlsItems.Count);

                    try
                    {
                        objOrderStatus = new OrderStatus();
                        intOldOrderStatusID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID", ""), 0);
                        objOrderStatus.StoreId = Token.Instance.StoreId;
                        objOrderStatus.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Name", ""));
                        objOrderStatus.DisplayName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "DisplayName", ""));

                        
                        //objOrderStatus.StockAdj = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "StockAdj"));
                        objOrderStatus.InventoryAction = ConvertInventoryAction(AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "StockAdj","0")));
                        

                        objOrderStatus.IsActive = ToBool(XmlUtility.GetElementValue(objNode, "IsActive", "0"));
                        objOrderStatus.IsValid = ToBool(XmlUtility.GetElementValue(objNode, "IsValid", "0"));
                        objOrderStatus.OrderBy = AlwaysConvert.ToInt16(XmlUtility.GetElementValue(objNode, "OrderBy", ""));

                        if (!objOrderStatus.Save().Equals(SaveResult.Failed))
                        {
                            if (!TranslationDictionary.ContainsKey("ORDERSTATUS" + intOldOrderStatusID))
                            {
                                TranslationDictionary.Add("ORDERSTATUS" + intOldOrderStatusID, objOrderStatus.OrderStatusId.ToString());
                            }
                            else
                            {
                                //update the id
                                TranslationDictionary["ORDERSTATUS" + intOldOrderStatusID] = objOrderStatus.OrderStatusId.ToString();
                            }

                            // BUG # bug 7144
                            //Import from 5.5 - order statuses for 7 should remain unchanged
                            // So, dont import order triggers
                            //OrderStatusTriggerCollection colOrderStatusEvents;
                            //colOrderStatusEvents = objOrderStatus.Triggers;

                            //string lstEvents = XmlUtility.GetElementValue(objNode, "Events", "");
                            //if (lstEvents.Length > 0)
                            //{
                            //    string[] arrEvents = lstEvents.Split(',');
                            //    foreach (string evnt in arrEvents)
                            //    {
                            //        int oldEventId = AlwaysConvert.ToInt(evnt, 0);
                            //        OrderStatusTrigger newOrderStatusTrigger = new OrderStatusTrigger(StoreEventDictionary[oldEventId]);
                            //        newOrderStatusTrigger.OrderStatusId = objOrderStatus.OrderStatusId;
                            //        colOrderStatusEvents.Add(newOrderStatusTrigger);
                            //    }
                            //}
                            //colOrderStatusEvents.Save();

                        }//end if
                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "Order Statuses Import, OrderStatusID:" + intOldOrderStatusID + " , Name: " + objOrderStatus.Name + ", " + ex.Message + "\n" + ex.StackTrace);
                    }
                } //next objNode        
                LogStatus(GetPercentResult(), "Order Statuses Import Complete...");

            }
            else
            {
                currentPhasePercent = currentPhaseWeightage;
                //LogStatus(GetPercentResult(), "No Order Statuses Available to Import...");

            }//end if

            //Order Status Rules : DO NOT IMPORT - NO EQUIVALENT

            //    //response.write("Checking Order Status Rules...")

            //    XmlNodeList nlsItems = nodSourceStore.SelectNodes("OrderStatusRules/OrderStatusRule");
            //    if( nlsItems.Count > 0){
            //        //response.write("Importing " & nlsItems.Count & " Order Status Rules .")

            //        //using OrderStatusRule to OrderStatusAction ???
            //        //OrderStatusRule objOrderStatusRule as OrderStatusRule
            //        OrderStatusAction objOrderStatusRule;
            //        //need more details about this
            //        /*
            //        foreach(XmlNode objNode in nlsItems){
            //            int intOldOrderStatusRuleID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID",""),0);
            //        //	'translate OrderStatus_ID1
            //            int intOldID1 = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "OrderStatus_ID1",""));
            //            int newID1 = AlwaysConvert.ToInt(objTranslate("ORDERSTATUS" + intOldID1));
            //            int intOldID2 = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "OrderStatus_ID2",""));
            //            int newID2 = AlwaysConvert.ToInt(objTranslate("ORDERSTATUS" + intOldID2));
            //            if( (!newID1.Equals(0)) &&  (!newID2.Equals(0))){                
            //                objOrderStatusRule = new OrderStatusAction();
            //                objOrderStatusRule.OrderStatusId = newID1;
            //                objOrderStatusRule.OrderStatus_ID2 = newID2;
            //                objOrderStatusRule.Event_ID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "Event_ID"));
            //                ////	'force preserved id
            //                //    if bPreserveIDs then
            //                //    //	'delete OrderStatusRule
            //                //        Token.Instance.StoreGroupDB.QueryNoRS("DELETE FROM ORDERSTATUSRULES WHERE OrderStatusRule_ID=" & intOldOrderStatusRuleID)
            //                //    //	'force the next id returned from maxidlookup
            //                //        Token.Instance.StoreGroupDB.QueryNoRS("UPDATE MAX_ID_LOOKUP SET Max_ID=" & (intOldOrderStatusRuleID-1) & " WHERE TableName=" & FCast.FSQLString("ORDERSTATUSRULES", intDBType))
            //                //    end if
            //                //save
            //                objOrderStatusRule.Save();
            //                objTranslate.Item("ORDERSTATUSRULE" + intOldOrderStatusRuleID) = objOrderStatusRule.OrderStatusRule_ID;
            //            //	'verify preserved ids
            //                //if bPreserveIDs then
            //                //    If (intOldOrderStatusRuleID <> objOrderStatusRule.OrderStatusRule_ID) Then
            //                //    //	'force of ID was not successful
            //                //        sWarnings.Append("Warning: OrderStatusRule " & intOldOrderStatusRuleID & " did not have the ID successfully preserved.  The new ID is " & objOrderStatusRule.OrderStatusRule_ID & ".")
            //                //        //response.write(" !")
            //                //    End If
            //                //End If
            //            }
            //            //response.write(" .")

            //        } //next objNode

            //        objOrderStatusRule = null;
            //        //response.write("")

            //    }else{
            //        Status(GetPercentResult(), "No Order Status Rules Available to Import...");

            //    }
            ////else
            ////    //response.write("Skipping Order Status Import...")

            ////end if
        }

        
        #region Shipping Methods Lines = 100

        private void ImportShippingMethods(XmlNode nodSourceStore)
        {
            currentPhasePercent = 0;
            //LogStatus(GetPercentResult(), "Checking Shipping Methods...");

            XmlNodeList nlsItems = nodSourceStore.SelectNodes("ShipMethods/ShipMethod");
            if (nlsItems.Count > 0)
            {
                int i = 0; // counter variable
                LogStatus(GetPercentResult(), "Importing " + nlsItems.Count + " Shipping Methods.");

                ShipMethod objShipMethod = null;
                int intOldShipMethodID = 0;
                foreach (XmlNode objNode in nlsItems)
                {

                    // update the import status
                    i++;

                    currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
                    LogStatus(GetPercentResult(), "Importing Shipping Method " + i + " of " + nlsItems.Count);

                    try
                    {

                        objShipMethod = new ShipMethod();
                        intOldShipMethodID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID", "0"), 0);
                        objShipMethod.StoreId = Token.Instance.StoreId;
                        objShipMethod.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Name", "ShipMethod"));                        
                        //objShipMethod.ShipType = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "ShipType",""));
                        objShipMethod.ShipMethodType = TranslateShipMethodType(AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "ShipType", ""), 0));
                        objShipMethod.Surcharge = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(objNode, "Surcharge", ""));

                        //	save					
                        objShipMethod.Save();

                        //TODO: Confirm
                        //objShipMethod.Warehouses = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Warehouses"))
                        String Warehouses = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Warehouses", ""));
                        String[] WarehouseLst = Warehouses.Split(',');
                        int newWarehouseID = 0;
                        foreach (string oldWarehouseID in WarehouseLst)
                        {
                            // SKIP INVALID ID's
                            if (String.IsNullOrEmpty(oldWarehouseID)) continue;
                            if (TranslationDictionary.ContainsKey("WAREHOUSE" + oldWarehouseID))
                            {
                                newWarehouseID = AlwaysConvert.ToInt(TranslationDictionary["WAREHOUSE" + oldWarehouseID]);
                                objShipMethod.ShipMethodWarehouses.Add(new ShipMethodWarehouse(objShipMethod.ShipMethodId, newWarehouseID));
                            }
                            else
                            {
                                LogError(GetPercentResult(), "ShipMethods Import, Unable to associate with warehouse because warehouse not found. ShipMethodID:" + intOldShipMethodID + ", Name:" + objShipMethod.Name + ", WareHouseId:" + oldWarehouseID );
                            }

                        }
                        objShipMethod.ShipMethodWarehouses.Save();
                        //TODO: Confirm
                        //objShipMethod.Countries.ListData = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Countries"))
                        String CountryCodes = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Countries", ""));
                        int ShipZoneID = GetZoneIDForCountryList(objShipMethod.Name, CountryCodes);
                        objShipMethod.ShipMethodShipZones.Add(new ShipMethodShipZone(objShipMethod.ShipMethodId, ShipZoneID));
                        objShipMethod.ShipMethodShipZones.Save();
                        objShipMethod.Save();
                        TranslationDictionary.Add("ShipMethod" + intOldShipMethodID, objShipMethod.ShipMethodId.ToString());



                        //	now add in discount matrix
                        XmlNodeList nlsMatrix = objNode.SelectNodes("Matrix/MatrixItem");
                        if (nlsMatrix.Count > 0)
                        {
                            int intOldShipMatrixID = 0;
                            ShipRateMatrix objShipMatrixItem;
                            foreach (XmlNode objMatrixNode in nlsMatrix)
                            {
                                intOldShipMatrixID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objMatrixNode, "ID", "0"), 0);
                                objShipMatrixItem = new ShipRateMatrix();
                                objShipMatrixItem.ShipMethodId = objShipMethod.ShipMethodId;
                                
                                String sMatrixRange = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objMatrixNode, "MinVal", ""));
                                if (sMatrixRange.Length > 0)
                                {
                                    
                                    objShipMatrixItem.RangeStart = AlwaysConvert.ToDecimal(sMatrixRange);
                                }
                                
                                
                                sMatrixRange = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objMatrixNode, "MaxVal", ""));
                                if (sMatrixRange.Length > 0)
                                {                                    
                                    objShipMatrixItem.RangeEnd = AlwaysConvert.ToDecimal(sMatrixRange);
                                }
                                
                                objShipMatrixItem.Rate = AlwaysConvert.ToDecimal(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objMatrixNode, "ShipRate", "")));
                                int tempValue = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objMatrixNode, "IsPercent", "0"));
                                if (tempValue == 0) objShipMatrixItem.IsPercent = false;
                                else
                                {
                                    objShipMatrixItem.IsPercent = true;

                                    // LOG THE WARNINGS IF THERE ARE INCONSISTANCIES
                                    if (tempValue == 1 && objShipMethod.ShipMethodType != ShipMethodType.WeightBased)
                                        LogWarning(GetPercentResult(), "Shipping method import: unable to import unsupported ship matrix item type, AC5x ShipMethod ID:" + intOldShipMethodID.ToString() + ", Name:" + objShipMethod.Name + ", Old item type: % of Total Weight");
                                    else if (tempValue == 2 && objShipMethod.ShipMethodType != ShipMethodType.CostBased)
                                        LogWarning(GetPercentResult(), "Shipping method import: unable to import unsupported ship matrix item type, AC5x ShipMethod ID:" + intOldShipMethodID.ToString() + ", Name:" + objShipMethod.Name + ", Old item type: % of Total Cost");
                                }
                                    
                                
                                objShipMatrixItem.Save();
                                objShipMethod.ShipRateMatrices.Add(objShipMatrixItem);
                            }//next objMatrixNode
                            objShipMatrixItem = null;
                        }
                        nlsMatrix = null;
                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "Shipping Methods Import, ShippingMethodID:" + intOldShipMethodID + " , Name: " + objShipMethod.Name + ", " + ex.Message + "\n" + ex.StackTrace);
                    }
                } //next objNode
                objShipMethod = null;
                LogStatus(GetPercentResult(), "Shipping Methods Import Complete...");
            }
            else
            {
                //LogStatus(GetPercentResult(), "No Shipping Methods Available to Import...");
            }

        }

        #endregion

        private void ImportTaxCodes(XmlNode nodSourceStore)
        {
            //'load tax codes for the product and order imports
            //	'build a dictionary of taxcode names and IDs that exist in the current store
            //	'when translating taxes, check them against this dictionary

            currentPhasePercent = 0;
            //LogStatus(GetPercentResult(), "Checking Tax Codes...");

            XmlNodeList nlsItems = nodSourceStore.SelectNodes("TaxCodes/TaxCode");
            if (nlsItems.Count > 0)
            {
                int i = 0; // counter variable
                LogStatus(GetPercentResult(), "Importing " + nlsItems.Count + " Tax Codes .");
                String sTaxCodeName;

                TaxCode objTaxCode = null;
                int intOldTaxCodeID = 0;
                foreach (XmlNode objNode in nlsItems)
                {

                    // update the import status
                    i++;

                    currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
                    LogStatus(GetPercentResult(), "Importing Tax Code " + i + " of " + nlsItems.Count);

                    try
                    {
                        sTaxCodeName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Name", "TaxCode"));
                        if (sTaxCodeName.Length > 0)
                        {
                            intOldTaxCodeID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID", "0"), 0);
                            if (!DicTaxCode.ContainsKey(sTaxCodeName))
                            {
                                //if newTaxCodeID = 0 then
                                objTaxCode = new TaxCode();
                                objTaxCode.StoreId = Token.Instance.StoreId;
                                objTaxCode.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Name", "TaxCode"));
                                //	save
                                objTaxCode.Save();
                                //newTaxCodeID = objTaxCode.TaxCodeId;
                                if (!TranslationDictionary.ContainsKey("TAXCODE" + intOldTaxCodeID))
                                {
                                    TranslationDictionary.Add("TAXCODE" + intOldTaxCodeID, objTaxCode.TaxCodeId.ToString());
                                }
                            }
                            else
                            {
                                // MAKE SURE THE OLD TO NEW ID MAPPING IN TRANSLATION DIC
                                if (!TranslationDictionary.ContainsKey("TAXCODE" + intOldTaxCodeID))
                                {
                                    TranslationDictionary.Add("TAXCODE" + intOldTaxCodeID, DicTaxCode[sTaxCodeName]);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "Tax Codes Import, TaxCodeID:" + intOldTaxCodeID + " , Name: " + objTaxCode.Name + ", " + ex.Message + "\n" + ex.StackTrace);
                    }

                } //next objNode
                objTaxCode = null;
                LogStatus(GetPercentResult(), "Tax Codes Import Complete...");
            }
            else
            {
                currentPhasePercent = currentPhaseWeightage;
                //LogStatus(GetPercentResult(), "No  Tax Codes Available to Import...");
            }

            //Skipping 
            //else
            //response.write("Skipping Tax Import...")

        }

        private void ImportTaxRules(XmlNode nodSourceStore)
        {
            currentPhasePercent = 0;
            //LogStatus(GetPercentResult(), "Checking Tax Rules...");

            XmlNodeList nlsItems = nodSourceStore.SelectNodes("TaxRules/TaxRule");
            if (nlsItems.Count > 0)
            {
                int i = 0; // counter variable
                LogStatus(GetPercentResult(), "Importing " + nlsItems.Count + " Tax Rules .");

                TaxRule objTaxRule = null;
                int intOldTaxRuleID = 0;
                foreach (XmlNode objNode in nlsItems)
                {
                    // update the import status
                    i++;

                    currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
                    LogStatus(GetPercentResult(), "Importing Tax Rule " + i + " of " + nlsItems.Count);

                    try
                    {
                        objTaxRule = new TaxRule();
                        
                        intOldTaxRuleID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID", "0"), 0);
                        objTaxRule.StoreId = Token.Instance.StoreId;
                        objTaxRule.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Name", "TaxRule"));
                        objTaxRule.CountryCode = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "CountryCode", ""));
                        objTaxRule.ProvinceId = ProvinceDataSource.GetProvinceIdByName(objTaxRule.CountryCode, HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "ProvinceSearchName", "")).ToLowerInvariant());
                        objTaxRule.PostalCodeFilter = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "PostalCode", ""));                        

                        String baseAddress = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "BaseAddr", "1"));
                        // 0 TO USE BILL ADDRESS 1 TO USE SHIP ADDRESS
                        objTaxRule.UseBillingAddress = (baseAddress == "0");
                        objTaxRule.TaxRate = AlwaysConvert.ToDecimal(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "TaxRate", "")));
                        //	'save
                        objTaxRule.Save();

                        // 7.1 STABLE IMPROVEMENT, TAX RULE CAN BE BASED UPON SHIPPING ZONES
                        String provinceName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "ProvinceSearchName", ""));
                        int taxRuleZoneID = GetZoneIDForTaxRuleData(objTaxRule.Name, objTaxRule.CountryCode, provinceName, objTaxRule.PostalCodeFilter);
                        objTaxRule.TaxRuleShipZones.Add(new TaxRuleShipZone(objTaxRule.TaxRuleId, taxRuleZoneID));
                        objTaxRule.TaxRuleShipZones.Save();

                        // RoundingRuleId : New field added at 7.3, Set the default value
                        objTaxRule.RoundingRuleId = (byte)RoundingRule.RoundToEven;                        

                        objTaxRule.Save(); 

                        TranslationDictionary.Add("TAXRULE" + intOldTaxRuleID, objTaxRule.TaxRuleId.ToString());
                        //	'add taxcodes to new tax rule

                        String lstTaxCodes = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "TaxCodes", ""));
                        if (lstTaxCodes.Length > 0)
                        {
                            String[] arrTaxCodes = lstTaxCodes.Split(',');
                            foreach (String sOldTaxCode in arrTaxCodes)
                            {
                                int intOldTaxCodeID = 0;
                                int newTaxCodeID = 0;
                                if (sOldTaxCode.Length > 0)
                                {
                                    bool IsNum = int.TryParse(sOldTaxCode, out intOldTaxCodeID);
                                    if (IsNum)
                                    {
                                        if (intOldTaxCodeID > 0)
                                        {
                                            if (TranslationDictionary.ContainsKey("TAXCODE" + sOldTaxCode))
                                            {
                                                newTaxCodeID = AlwaysConvert.ToInt(TranslationDictionary["TAXCODE" + sOldTaxCode]);
                                            }
                                            else
                                            {
                                                LogError(GetPercentResult(), "TaxRule Import, Unable to associate TaxRule to Tax Codes because TaxCode not found. TaxRuleID:" + intOldTaxRuleID + ", Name:" + objTaxRule.Name + ", TaxCodeID:" + sOldTaxCode);
                                            }
                                        }
                                        else
                                        {
                                            //	'negative tax codes are special (-1 = shipping)
                                            //TODO: 
                                            //Comments:
                                            /*
                                            �While importing Tax Code�... Are you referring to the Product_ID value?
                                            If yes, then this does have a translation. In AC6, we have the 
                                            OrderItemTypeId (OrderItemType enum) field. So for Product_ID 
                                            values less than 0, these will have a corresponding OrderItemType 
                                            such as OrderItemType.Shipping, OrderItemType.Handling, 
                                            OrderItemType.Tax, etc.
                                            */
                                            //newTaxCodeID = intOldTaxCodeID;
                                        }
                                    }
                                    else
                                    {
                                        if (DicTaxCode.ContainsKey(sOldTaxCode))
                                        {
                                            newTaxCodeID = AlwaysConvert.ToInt(DicTaxCode[sOldTaxCode]);
                                        }
                                        else if (sOldTaxCode.Equals("SHIPPING"))
                                        {
                                            //TODO:
                                            //newTaxCodeID = -1;
                                        }
                                    }
                                }
                                if (!newTaxCodeID.Equals(0))
                                {
                                    //objTaxRule.TaxCodes.Add(newTaxCodeID)
                                    objTaxRule.TaxRuleTaxCodes.Add(new TaxRuleTaxCode(objTaxRule.TaxRuleId, newTaxCodeID));
                                }
                            }//next j
                        }

                        objTaxRule.TaxRuleTaxCodes.Save();
                        objTaxRule.Save();
                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "Tax Rules Import, TaxRuleID:" + intOldTaxRuleID + " , Name: " + objTaxRule.Name + ", " + ex.Message + "\n" + ex.StackTrace);
                    }
                } //next objNode
                objTaxRule = null;
                LogStatus(GetPercentResult(), "Tax Rules Import Complete...");
            }
            else
            {
                currentPhasePercent = currentPhaseWeightage;
                //LogStatus(GetPercentResult(), "No Tax Rules Available to Import...");
            }
        }

        private void ImportVendors(XmlNode nodSourceStore)
        {
            currentPhasePercent = 0;
            //LogStatus(GetPercentResult(), "Checking Vendors...");

            XmlNodeList nlsItems = nodSourceStore.SelectNodes("Vendors/Vendor");
            if (nlsItems.Count > 0)
            {

                int i = 0; // counter variable
                LogStatus(GetPercentResult(), "Importing " + nlsItems.Count + " Vendors .");

                Vendor objVendor = null;
                int intOldVendorID = 0;
                foreach (XmlNode objNode in nlsItems)
                {

                    // update the import status
                    i++;

                    currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
                    LogStatus(GetPercentResult(), "Importing Vendor " + i + " of " + nlsItems.Count);

                    try
                    {
                        objVendor = new Vendor();
                        intOldVendorID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID", "0"), 0);
                        objVendor.StoreId = Token.Instance.StoreId;

                        //TODO: i think this is not used, I explored the AC5x store but no use of this property is found
                        //Comments:
                        /*
                         Leave this out for now. It was not fully implemented in AC55,
                         * so doubtful this value means anything.
                         */
                        //int intOldGroupID = AlwaysConvert.ToInt(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "AdminGroup_ID","")));
                        //if (intOldGroupID > 0){
                        //    if( objTranslate.ContainsKey("GROUP" + intOldGroupID)){
                        //         
                        //        //objVendor.AdminGroup_ID = objTranslate("GROUP" + intOldGroupID)
                        //    }
                        //}

                        //objVendor.StoreId = Token.Instance.StoreId;
                        objVendor.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Name", "Vendor"));
                        objVendor.Email = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Email", ""));
                        


                        //	'save
                        objVendor.Save();
                        TranslationDictionary.Add("VENDOR" + intOldVendorID, objVendor.VendorId.ToString());
                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "Vendors Import, VendorID:" + intOldVendorID + " , Name: " + objVendor.Name + ", " + ex.Message + "\n" + ex.StackTrace);
                    }
                } //next objNode
                objVendor = null;
                LogStatus(GetPercentResult(), "Vendors Import Complete...");
            }
            else
            {
                currentPhasePercent = currentPhaseWeightage;
                //LogStatus(GetPercentResult(), "No  Vendors Available to Import...");
            }
            //}else{
            //        //response.write("Skipping Vendor Import...")
            //        
            //}
        }

        private void ImportWareHouses(XmlNode nodSourceStore)
        {

            //if InStr(lstCopyOpts, ",WAREHOUSES,") > 0 then
            //	'groups selected for copy

            currentPhasePercent = 0;
            //LogStatus(GetPercentResult(), "Checking WareHouses...");

            XmlNodeList nlsItems = nodSourceStore.SelectNodes("Warehouses/Warehouse");
            if (nlsItems.Count > 0)
            {
                int i = 0; // counter variable
                LogStatus(GetPercentResult(), "Importing " + nlsItems.Count + " WareHouses .");

                Warehouse objWarehouse = null;
                int intOldWarehouseID = 0;
                foreach (XmlNode objNode in nlsItems)
                {

                    // update the import status
                    i++;

                    currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
                    LogStatus(GetPercentResult(), "Importing WareHouse " + i + " of " + nlsItems.Count);
                    try
                    {
                        objWarehouse = new Warehouse();
                        intOldWarehouseID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID", "0"), 0);
                        objWarehouse.StoreId = Token.Instance.StoreId;
                        objWarehouse.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Name", "Warehouse"));
                        objWarehouse.Address1 = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Address1", ""));
                        objWarehouse.Address2 = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Address2", ""));
                        objWarehouse.City = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "City", ""));
                        objWarehouse.Province = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Province", ""));
                        objWarehouse.PostalCode = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "PostalCode", ""));
                        objWarehouse.CountryCode = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "CountryCode", ""));
                        objWarehouse.Phone = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Phone", ""));
                        objWarehouse.Fax = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Fax", ""));
                        objWarehouse.Email = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Email", ""));

                        //	'save
                        objWarehouse.Save();
                        TranslationDictionary.Add("WAREHOUSE" + intOldWarehouseID, objWarehouse.WarehouseId.ToString());
                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "WareHouses Import, WareHouseID:" + intOldWarehouseID + " , Name: " + objWarehouse.Name + ", " + ex.Message + "\n" + ex.StackTrace);
                    }
                } //next objNode
                objWarehouse = null;

                LogStatus(GetPercentResult(), "WareHouses Import Complete...");

            }
            else
            {
                currentPhasePercent = currentPhaseWeightage;
                //LogStatus(GetPercentResult(), "No  Warehouses Available to Import...");
            }
            //}else{
            //        //response.write("Skipping Warehouse Import...")

            //}
        }

        #region WebPages Lines = 75

        private void ImportWebPages(XmlNode nodSourceStore)
        {
            
            //if InStr(lstCopyOpts, ",WEBPAGES,") > 0 then
            //	'webpages selected for copy

            currentPhasePercent = 0;
            //LogStatus(GetPercentResult(), "Checking Webpages...");

            XmlNodeList nlsItems = nodSourceStore.SelectNodes("Webpages/Webpage");
            if (nlsItems.Count > 0)
            {

                int i = 0; // counter variable
                LogStatus(GetPercentResult(), "Importing " + nlsItems.Count + " Webpages .");

                Webpage objWebpage = null;
                int intOldWebpageID = 0;
                foreach (XmlNode objNode in nlsItems)
                {

                    // update the import status
                    i++;
                    currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
                    LogStatus(GetPercentResult(), "Importing Webpage " + i + " of " + nlsItems.Count);
                    try
                    {
                        objWebpage = new Webpage();
                        intOldWebpageID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID", "0"), 0);
                        objWebpage.StoreId = Token.Instance.StoreId;
                        objWebpage.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Name", ""));
                        objWebpage.Summary = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Description1", ""));
                        ////we dont need to import this
                        //objWebpage.DisplayPage = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "DisplayPage", ""))
                        //objWebpage.Theme = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Theme"))

                        //objWebpage.Image1 = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode,"Image1"))
                        //objWebpage.Image1Params = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode,"Image1Params"))

                        objWebpage.ThumbnailUrl = FormatImgURL(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Image1", "")));
                        objWebpage.ThumbnailAltText = GetAltText(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Image1Params", "")));

                        objWebpage.Visibility = ConvertCatalogNodeVisibility(AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "Active", "1")));
                        //objWebpage.Header = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode,"Header","1"));
                        objWebpage.HtmlHead = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Header", "1"));
                        objWebpage.Description = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Content", "1"));

                        //	'save
                        //objWebpage.Save();
                        // PRESERVE ID's FOR OBJECT
                        String messages = String.Empty;
                        objWebpage.WebpageId= intOldWebpageID;
                        SaveResult saveResult = ACDataSource.SaveWebpage(objWebpage, this.PreserveIdOption, ref messages);
                        if (messages.Length > 0) LogWarning(GetPercentResult(), messages);

                        // SKIP IMPORTING MORE DETAILS IF FAILED SAVING OBJECT
                        if (saveResult == SaveResult.Failed) continue;

                        TranslationDictionary.Add("WEBPAGE" + intOldWebpageID, objWebpage.WebpageId.ToString());

                        String lstOldCategories = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Categories", ""));
                        if (lstOldCategories.Length > 0)
                        {
                            String[] arrOldCategories = lstOldCategories.Split(',');
                            foreach (String strCatId in arrOldCategories)
                            {
                                int intOldCategoryID = AlwaysConvert.ToInt(strCatId);
                                if (TranslationDictionary.ContainsKey("CATEGORY" + intOldCategoryID))
                                {
                                    objWebpage.Categories.Add(AlwaysConvert.ToInt(TranslationDictionary["CATEGORY" + intOldCategoryID]));
                                }
                                else
                                {
                                    LogError(GetPercentResult(), "Webpage Import, Unable to associate Webpage to Category because Category not found. WebpageID:" + intOldWebpageID + ", Name:" + objWebpage.Name + ", CategoryID:" + intOldCategoryID);
                                }
                            }
                            objWebpage.Categories.WebpageId = objWebpage.WebpageId;
                            objWebpage.Categories.Save();

                            // MAKE SURE IT IS ASSIGNED IN AT LEAST ONE CATEOGRY
                            if (objWebpage.Categories.Count == 0)
                            {
                                objWebpage.Categories.Add(OrphanedItemsCategoryId);
                                objWebpage.Categories.WebpageId = objWebpage.WebpageId;
                                objWebpage.Categories.Save();
                            }
                        }
                        else
                        {
                            // SO FOR CONSIDER IT IS AS ORPHANED WEBPAGE, ADD IT TO ROOT CATEGORY
                            // IF ANY OF THE OBJECT LINK RECORD FOUND THEN IT WILL BE MOVED TO THE RESPECTIVE CATEGORY
                            objWebpage.Categories.Add(OrphanedItemsCategoryId);
                            objWebpage.Categories.WebpageId = objWebpage.WebpageId;
                            objWebpage.Categories.Save();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "Webpages Import, WebpageID:" + intOldWebpageID + " , Name: " + objWebpage.Name + ", " + ex.Message + "\n" + ex.StackTrace);
                    }
					
                }                 
                LogStatus(GetPercentResult(), "Webpages Import Complete...");
            }else{
                //LogStatus(GetPercentResult(), "No Webpages Available to Import...");				
            }
            //else
            //    //response.write("Skipping Webpage Import...")
        		
            //end if
        }
        
     
        

        #endregion

        private void ImportGiftwraps(XmlNode nodSourceStore)
        {
            currentPhasePercent = 0;
            //LogStatus(GetPercentResult(), "Checking GiftWrap Groups...");


            XmlNodeList nlsItems = nodSourceStore.SelectNodes("WrapGroups/WrapGroup");
            if (nlsItems.Count > 0)
            {
                int i = 0; // counter variable
                LogStatus(GetPercentResult(), "Importing " + nlsItems.Count + " GiftWrap Groups .");

                WrapGroup objWrapGroup =  null;
                int intOldWrapGroupID = 0;
                foreach (XmlNode objNode in nlsItems)
                {
                    // update the import status
                    i++;

                    currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
                    LogStatus(GetPercentResult(), "Importing GiftWrap " + i + " of " + nlsItems.Count);
                    try
                    {

                        objWrapGroup = new WrapGroup();
                        intOldWrapGroupID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID", "0"));
                        objWrapGroup.StoreId = Token.Instance.StoreId;
                        objWrapGroup.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Name", ""));

                        //	'save
                        objWrapGroup.Save();
                        int newWrapGroupID = objWrapGroup.WrapGroupId;
                        //	'add new id to translation
                        TranslationDictionary.Add("WRAPGROUP" + intOldWrapGroupID, newWrapGroupID.ToString());

                        //	'import WRAPSTYLES
                        WrapStyle objWrapStyle;
                        XmlNodeList nlsWrapStyles = objNode.SelectNodes("WrapStyles/WrapStyle");
                        foreach (XmlNode nodWrapStyle in nlsWrapStyles)
                        {
                            objWrapStyle = new WrapStyle();
                            int intOldWrapStyleID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodWrapStyle, "ID", ""));

                            objWrapStyle.WrapGroupId = newWrapGroupID;
                            objWrapStyle.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodWrapStyle, "Name", ""));
                            objWrapStyle.ThumbnailUrl = FormatImgURL(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodWrapStyle, "Image1", "")));
                            objWrapStyle.ImageUrl = FormatImgURL(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodWrapStyle, "Image2", "")));

                            //these do not exist
                            /*
                            objWrapStyle.Image1Params = URLDecode(XmlUtility.GetElementValue(nodWrapStyle, "Image1Params",""));
                            objWrapStyle.Image2Params = URLDecode(XmlUtility.GetElementValue(nodWrapStyle, "Image2Params"))
                            */

                            objWrapStyle.Price = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodWrapStyle, "UnitPrice", ""));
                            //	Translate TaxCode
                            int newTaxCodeID = 0;
                            String sOldTaxCode = XmlUtility.GetElementValue(nodWrapStyle, "TaxCode", "");
                            if (sOldTaxCode.Length > 0)
                            {
                                if (DicTaxCode.ContainsKey(sOldTaxCode))
                                {
                                    newTaxCodeID = AlwaysConvert.ToInt(DicTaxCode[sOldTaxCode]);
                                }
                            }
                            
                            objWrapStyle.TaxCodeId = newTaxCodeID;
                            objWrapStyle.OrderBy = AlwaysConvert.ToInt16(XmlUtility.GetElementValue(nodWrapStyle, "OrderBy", "0"));

                            //save
                            objWrapStyle.Save();
                            objWrapGroup.WrapStyles.Add(objWrapStyle);
                            //	'ADD TRANSLATION
                            TranslationDictionary.Add("WRAPSTYLE" + intOldWrapStyleID, objWrapStyle.WrapStyleId.ToString());

                            objWrapStyle = null;
                        }//next nodWrapStyle


                        objWrapGroup.Save();
                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "GiftWrap Groups Import, GiftWrapID:" + intOldWrapGroupID + " , Name: " + objWrapGroup.Name + ", " + ex.Message + "\n" + ex.StackTrace);
                    }
                } //next objNode


                LogStatus(GetPercentResult(), "GiftWrap Groups Import Complete...");
            }
            else
            {
                currentPhasePercent = currentPhaseWeightage;
                //LogStatus(GetPercentResult(), "No GiftWrap Groups Available to Import...");
            }
        }

        #endregion

        #region Pass 2

        private void ImportAffiliates(XmlNode nodSourceStore)
        {
            currentPhasePercent = 0;
            //LogStatus(GetPercentResult(), "Checking Affiliates...");

            XmlNodeList nlsItems = nodSourceStore.SelectNodes("Affiliates/Affiliate");
            if (nlsItems.Count > 0)
            {
                int i = 0; // counter variable
                LogStatus(GetPercentResult(), "Importing " + nlsItems.Count + " Affiliates .");


                Affiliate objAffiliate = null;
                int intOldAffiliateID = 0;
                foreach (XmlNode objNode in nlsItems)
                {
                    // update the import status
                    i++;

                    currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
                    LogStatus(GetPercentResult(), "Importing Affiliate " + i + " of " + nlsItems.Count);
                    try
                    {

                        objAffiliate = new Affiliate();
                        intOldAffiliateID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID", "0"), 0);
                        objAffiliate.StoreId = Token.Instance.StoreId;
                        int intOldReferrerID = AlwaysConvert.ToInt(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Referrer_ID", "0")));
                        
                        
                        //objAffiliate.Referrer_ID does not exist
                        //if (objTranslate.ContainsKey("AFFILIATE" + intOldReferrerID))
                        //{
                        //    //objAffiliate.Referrer_ID = objTranslate("AFFILIATE" & intOldReferrerID)
                        //}
                        //else
                        //{
                        //    //objAffiliate.Referrer_ID = 0
                        //}   
                        

                        objAffiliate.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Name", ""));

                        objAffiliate.Address1 = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Address1", ""));
                        objAffiliate.Address2 = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Address2", ""));
                        objAffiliate.City = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "City", ""));
                        objAffiliate.Province = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Province", ""));
                        objAffiliate.PostalCode = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "PostalCode", ""));
                        objAffiliate.CountryCode = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "CountryCode", ""));
                        objAffiliate.PhoneNumber = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Phone", ""));
                        objAffiliate.Email = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Email", ""));
                        //objAffiliate.PurchaseRate = AlwaysConvert.ToDecimal(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "PurchaseRate", "0.0")));
                        //objAffiliate.PurchasePercent = AlwaysConvert.ToDecimal(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "PurchasePercent", "0.0")));
                        objAffiliate.CommissionRate = AlwaysConvert.ToDecimal(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "PurchaseRate", "0.0")));
                        objAffiliate.CommissionIsPercent = AlwaysConvert.ToDecimal(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "PurchasePercent", "0.0"))) > 0 ? true : false;
                        if (objAffiliate.CommissionIsPercent)
                        {
                            // IF RATE IS PERCENT THEN IT SHOULD BE PERCENT RATE
                            objAffiliate.CommissionRate = AlwaysConvert.ToDecimal(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "PurchasePercent", "0.0")));
                        }

                        objAffiliate.WebsiteUrl = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "CompanyURL", ""));

                        // NOT FOUND :objAffiliate.Persistent = ToBool(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Persistent", "0")), false);

                        //	'save
                        String messages = String.Empty;
                        objAffiliate.AffiliateId = intOldAffiliateID;
                        SaveResult saveResult = ACDataSource.SaveAffiliate(objAffiliate, this.PreserveIdOption, ref messages);
                        if (messages.Length > 0) LogWarning(GetPercentResult(), messages);

                        // SKIP IMPORTING MORE DETAILS IF FAILED SAVING OBJECT
                        if (saveResult == SaveResult.Failed) continue;

                        /*
                    int intOldGroupID = AlwaysConvert.ToInt(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "AdminGroup_ID", "0")));
                    if (objTranslate.ContainsKey("GROUP" + intOldGroupID))
                    {
                        //TODO: objAffiliate.AdminGroup_ID does not exist  
                        int newGroupID = AlwaysConvert.ToInt(objTranslate["GROUP" + intOldGroupID]);
                        objAffiliate.GroupAffiliates.Add(new GroupAffiliate(newGroupID, objAffiliate.AffiliateId));
                        objAffiliate.GroupAffiliates.Save();
                    }*/
                        TranslationDictionary.Add("AFFILIATE" + intOldAffiliateID, objAffiliate.AffiliateId.ToString());
                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "Affiliates Import, AffiliateID:" + intOldAffiliateID + " , Name: " + objAffiliate.Name + ", " + ex.Message + "\n" + ex.StackTrace);
                    }

                } //next objNode
                objAffiliate = null;
                LogStatus(GetPercentResult(), "Affiliates Import Complete...");

            }
            else
            {
                //LogStatus(GetPercentResult(), "No Affiliates Available to Import...");
            }

        }

        private void ImportDiscounts(XmlNode nodSourceStore)
        {

            currentPhasePercent = 0;
            //LogStatus(GetPercentResult(), "Checking Discounts...");

            XmlNodeList nlsItems = nodSourceStore.SelectNodes("Discounts/Discount");
            if (nlsItems.Count > 0)
            {

                int i = 0; // counter variable
                LogStatus(GetPercentResult(), "Importing " + nlsItems.Count + " Discounts .");

                VolumeDiscount objDiscount = null;
                CategoryVolumeDiscountCollection colRootDiscounts = null;
                int intOldDiscountID = 0;
                foreach (XmlNode objNode in nlsItems)
                {

                    // update the import status
                    i++;

                    currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
                    LogStatus(GetPercentResult(), "Importing Discount " + i + " of " + nlsItems.Count);

                    try
                    {
                        objDiscount = new VolumeDiscount();
                        colRootDiscounts = objDiscount.CategoryVolumeDiscounts;
                        intOldDiscountID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID", "0"), 0);
                        objDiscount.StoreId = Token.Instance.StoreId;

                        //	'save
                        objDiscount.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Name", ""));

                        //	'add root discount if global
                        objDiscount.IsGlobal = ToBool(XmlUtility.GetElementValue(objNode, "IsGlobal", "0"), false);                        

                        objDiscount.Save();
                        String lstOldGroups = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Groups", ""));
                        if (lstOldGroups.Length > 0)
                        {
                            String[] arrOldGroups = lstOldGroups.Split(',');
                            foreach (String OldGroupID in arrOldGroups)
                            {
                                //intOldGroupID = arrOldGroups(j)
                                if (TranslationDictionary.ContainsKey("GROUP" + OldGroupID))
                                {
                                    int newGroupID = AlwaysConvert.ToInt(TranslationDictionary["GROUP" + OldGroupID]);
                                    objDiscount.VolumeDiscountGroups.Add(new VolumeDiscountGroup(objDiscount.VolumeDiscountId, newGroupID));
                                }
                                else
                                {
                                    LogError(GetPercentResult(), "Discount Import, Unable to associate Discount to Group because Group not found. DiscountID:" + intOldDiscountID + ", Name:" + objDiscount.Name + ", GroupID:" + OldGroupID);
                                }
                            }
                        }

                        TranslationDictionary.Add("DISCOUNT" + intOldDiscountID, objDiscount.VolumeDiscountId.ToString());

                        //	'now add in discount matrix
                        XmlNodeList nlsMatrix = objNode.SelectNodes("Matrix/MatrixItem");
                        if (nlsMatrix.Count > 0)
                        {
                            VolumeDiscountLevel objMatrixItem;
                            foreach (XmlNode objMatrixNode in nlsMatrix)
                            {
                                int intOldDiscountMatrixID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objMatrixNode, "ID", "0"), 0);
                                objMatrixItem = new VolumeDiscountLevel();
                                objMatrixItem.VolumeDiscountId = objDiscount.VolumeDiscountId;
                                objMatrixItem.DiscountAmount = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(objMatrixNode, "Amount", ""));
                                objMatrixItem.IsPercent = ToBool(XmlUtility.GetElementValue(objMatrixNode, "IsPercent", "0"), false);
                                objMatrixItem.MaxValue = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(objMatrixNode, "MaxQty", ""));
                                objMatrixItem.MinValue = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(objMatrixNode, "MinQty", ""));

                                objMatrixItem.Save();
                                objDiscount.Levels.Add(objMatrixItem);

                            }//next objMatrixNode
                            objDiscount.Levels.Save();
                            objMatrixItem = null;
                            //objMatrixNode = null;
                        }
                        objDiscount.Save();
                        nlsMatrix = null;

                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "Discounts Import, DiscountID:" + intOldDiscountID + " , Name: " + objDiscount.Name + ", " + ex.Message + "\n" + ex.StackTrace);
                    }
                } //next objNode
                objDiscount = null;

            }
            else
            {
                //LogStatus(GetPercentResult(), "No Discounts Available to Import...");
            }
        }

        private void ImportPaymentMethods(XmlNode nodSourceStore)
        {
            //if InStr(lstCopyOpts, ",PAYMETH,") > 0 then
            //	'PaymentMethods selected for copy

            currentPhasePercent = 0;
            //LogStatus(GetPercentResult(), "Checking Payment Methods...");

            XmlNodeList nlsItems = nodSourceStore.SelectNodes("PayMethods/PayMethod");
            if (nlsItems.Count > 0)
            {
                int i = 0; // counter variable
                LogStatus(GetPercentResult(), "Importing " + nlsItems.Count + " Payment Methods.");

                PaymentMethod objPayMethod = null;
                int intOldPayMethodID = 0;
                foreach (XmlNode objNode in nlsItems)
                {
                    i++;
                    currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
                    LogStatus(GetPercentResult(), "Importing Payment Method " + i + " of " + nlsItems.Count);

                    try
                    {
                        objPayMethod = new PaymentMethod();
                        intOldPayMethodID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID", "0"), 0);
                        objPayMethod.StoreId = Token.Instance.StoreId;
                        objPayMethod.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Name", "PayMethod"));
                        //Ignore this
                        //objPayMethod.Active = ToBool(XmlUtility.GetElementValue(objNode, "Active"))
                        
                        int payType = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "PayType","0"));
                        int validatorId = AlwaysConvert.ToInt16(XmlUtility.GetElementValue(objNode, "PayValidator_ID", "0"));

                        objPayMethod.PaymentInstrumentId = (short)ConvertPaymentMethodType(payType, validatorId, objPayMethod.Name);
                        //	'TRANSLATE PAYGATEWAY ID
                        int newPaygatewayID = 0;
                        int intOldGatewayID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "PayGateway_ID", ""), 0);
                        if (TranslationDictionary.ContainsKey("PAYGATEWAY" + intOldGatewayID))
                        {
                            newPaygatewayID = AlwaysConvert.ToInt(TranslationDictionary["PAYGATEWAY" + intOldGatewayID]);
                        }
                        else
                        {
                            newPaygatewayID = 0;
                        }
                        objPayMethod.PaymentGatewayId = newPaygatewayID;

                        //	'save
                        objPayMethod.Save();
                        int newGroupID = 0;
                        String lstOldGroups = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Groups", ""));
                        if (lstOldGroups.Length > 0)
                        {
                            String[] arrOldGroups = lstOldGroups.Split(',');
                            foreach (String OldGroupID in arrOldGroups)
                            {
                                if (TranslationDictionary.ContainsKey("GROUP" + OldGroupID))
                                {
                                    newGroupID = AlwaysConvert.ToInt(TranslationDictionary["GROUP" + OldGroupID]);
                                    objPayMethod.PaymentMethodGroups.Add(new PaymentMethodGroup(objPayMethod.PaymentMethodId, newGroupID));
                                }
                                else
                                {
                                    LogError(GetPercentResult(), "PayMethod Import, Unable to associate PayMethod to Group because Group not found. PayMethodID:" + intOldPayMethodID + ", Name:" + objPayMethod.Name + ", GroupID:" + OldGroupID);
                                }
                            }
                            objPayMethod.PaymentMethodGroups.Save();
                        }

                        TranslationDictionary.Add("PAYMETHOD" + intOldPayMethodID, objPayMethod.PaymentMethodId.ToString());

                        objPayMethod.Save();
                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "PayMethods Import, PayMethodID:" + intOldPayMethodID + " , Name: " + objPayMethod.Name + ", " + ex.Message + "\n" + ex.StackTrace);
                    }
                } //next objNode
                objPayMethod = null;
                LogStatus(GetPercentResult(), "PayMethods Import Complete...");

            }
            else
            {
                currentPhasePercent = currentPhaseWeightage;
                //LogStatus(GetPercentResult(), "No PayMethods Available to Import...");
            }
        }

        private void ImportProducts(XmlNode nodSourceStore)
        {
            //if InStr(lstCopyOpts, ",PRODUCTS,") > 0 then
            //	'products selected for copy
            //response.write("Checking Products...")


            currentPhasePercent = 0;
            //LogStatus(GetPercentResult(), "Checking Products...");

            XmlNodeList nlsProducts = nodSourceStore.SelectNodes("Products/Product");
            if (nlsProducts.Count > 0)
            {

                int i = 0; // counter variable
                LogStatus(GetPercentResult(), "Importing " + nlsProducts.Count + " Products .");


                int intOldProductID = 0;
                Product objProduct = null;                
                foreach (XmlNode nodProduct in nlsProducts)
                {
                    // update the import status
                    i++;

                    currentPhasePercent = CalculatePartialPercentage(i, nlsProducts.Count);
                    LogStatus(GetPercentResult(), "Importing Product " + i + " of " + nlsProducts.Count);
                    try
                    {
                        objProduct = new Product();
                        intOldProductID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodProduct, "ID", "0"));
                        objProduct.StoreId = Token.Instance.StoreId;
                        objProduct.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodProduct, "Name", ""));
                        objProduct.Price = AlwaysConvert.ToDecimal(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodProduct, "UnitPrice", "0")));
                        objProduct.MSRP = AlwaysConvert.ToDecimal(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodProduct, "UnitRetail", "0")));
                        objProduct.Weight = AlwaysConvert.ToDecimal(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodProduct, "UnitWeight", "0")));

                        int intOldCategoryID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodProduct, "Category_ID", "0"));

                        int newCategoryId = 0;
                        if (TranslationDictionary.ContainsKey("CATEGORY" + intOldCategoryID))
                        {
                            newCategoryId = AlwaysConvert.ToInt(TranslationDictionary["CATEGORY" + intOldCategoryID]);
                        }
                        else if (intOldCategoryID > 0)
                        {
                            LogError(GetPercentResult(), "Product Import, Unable to associate Product to Category because Category not found. ProductID:" + intOldProductID + ", Name:" + objProduct.Name + ", CategoryID:" + intOldCategoryID);
                        }
                        
                        if (newCategoryId > 0 && !objProduct.Categories.Contains(newCategoryId))
                        {
                            objProduct.Categories.Add(newCategoryId);
                        }
                        else if (newCategoryId == 0)
                        {
                            // IT IS A PRODUCT AT ROOT LEVEL
                            // AS AC7 DID NOT SUPPORT ADDING PRODUCTS TO ROOT LEVEL ADD IT TO ORPHANED ITEMS CATEGORY
                            objProduct.Categories.Add(OrphanedItemsCategoryId);
                        }


                        objProduct.Length = AlwaysConvert.ToDecimal(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodProduct, "Length", "0")));
                        objProduct.Width = AlwaysConvert.ToDecimal(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodProduct, "Width", "0")));
                        objProduct.Height = AlwaysConvert.ToDecimal(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodProduct, "Height", "0")));

                        //Build a manufacturers dictionary and find it inside.
                        string sOldManufacturer = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodProduct, "Manufacturer", ""));

                        Manufacturer manufacturer = null;
                        int newManufacturerId = 0;
                        if (DicManufacturers.ContainsKey(sOldManufacturer))
                        {
                            newManufacturerId = AlwaysConvert.ToInt(DicManufacturers[sOldManufacturer]);
                        }
                        else
                        {
                            manufacturer = new Manufacturer();
                            manufacturer.Name = sOldManufacturer;
                            manufacturer.Save();
                            newManufacturerId = manufacturer.ManufacturerId;
                            DicManufacturers.Add(sOldManufacturer, newManufacturerId.ToString());
                        }
                        objProduct.ManufacturerId = newManufacturerId;
                        objProduct.Sku = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodProduct, "SKU", ""));
                        objProduct.Visibility = ConvertCatalogNodeVisibility(AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodProduct, "Active", "1")));
                        int newTaxCodeID = 0;
                        string sOldTaxCode = XmlUtility.GetElementValue(nodProduct, "TaxCode", "");
                        if (sOldTaxCode.Length > 0)
                        {
                            if (DicTaxCode.ContainsKey(sOldTaxCode))
                            {
                                newTaxCodeID = AlwaysConvert.ToInt(DicTaxCode[sOldTaxCode]);
                            }
                        }

                        objProduct.TaxCodeId = newTaxCodeID;

                        objProduct.Shippable = (Shippable)AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodProduct, "Shippable", ""));

                        int intOldWarehouseID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodProduct, "Warehouse_ID", ""));

                        
                        int newWarehouseID = Token.Instance.Store.DefaultWarehouse.WarehouseId;

                        if (TranslationDictionary.ContainsKey("WAREHOUSE" + intOldWarehouseID))
                        {
                            newWarehouseID = AlwaysConvert.ToInt(TranslationDictionary["WAREHOUSE" + intOldWarehouseID]);
                        }
                        else if (objProduct.Shippable == Shippable.No)
                        {
                            //if product is not shippable, set warehouse to 0 or 0
                            newWarehouseID = 0;
                        }
                        else if(intOldWarehouseID > 0)
                        {
                            LogError(GetPercentResult(), "Product Import, Unable to associate Product to Warehouse because Warehouse not found. ProductID:" + intOldProductID + ", Name:" + objProduct.Name + ", WarehouseID:" + intOldWarehouseID);
                        }

                        objProduct.WarehouseId = newWarehouseID;

                        objProduct.InventoryModeId = (byte)AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodProduct, "InvMode", ""));
                        objProduct.InventoryMode = (InventoryMode)AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodProduct, "InvMode", ""));
                        objProduct.InStock = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodProduct, "InStock", ""), 0) - AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodProduct, "ReserveStock", ""), 0);
                        objProduct.InStockWarningLevel = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodProduct, "Reorder", ""));
                        

                        objProduct.ThumbnailUrl = FormatImgURL(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodProduct, "Image1", "")));
                        objProduct.ThumbnailAltText = GetAltText(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodProduct, "Image1Params", "")));
                        objProduct.ImageUrl = FormatImgURL(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodProduct, "Image2", "")));
                        objProduct.ImageAltText = GetAltText(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodProduct, "Image2Params", "")));

                        objProduct.Summary = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodProduct, "Description1", ""));
                        objProduct.Description = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodProduct, "Description2", ""));
                        objProduct.ExtendedDescription = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodProduct, "Description3", ""));
                        //Comments:
                        //Description1 ? Summary; Description2 + Description 3 ? Description;
                        //objProduct.Description1 = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodProduct, "Description1"));
                        //objProduct.Description2 = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodProduct, "Description2"));
                        //objProduct.Description3 = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodProduct, "Description3"));

                        //translate Vendor_ID	                
                        int intOldVendorID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodProduct, "Vendor_ID", ""));
                        int newVendorID = 0;
                        if (TranslationDictionary.ContainsKey("VENDOR" + intOldVendorID))
                        {
                            newVendorID = AlwaysConvert.ToInt(TranslationDictionary["VENDOR" + intOldVendorID]);
                        }
                        else if(intOldVendorID > 0)
                        {
                            LogError(GetPercentResult(), "Product Import, Unable to associate Product to Vendor because Vendor not found. ProductID:" + intOldProductID + ", Name:" + objProduct.Name + ", VendorID:" + intOldVendorID);
                        }
                        
                        objProduct.VendorId = newVendorID;

                        //	translate WrapGroup_ID
                        int intOldWrapGroupID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodProduct, "WrapGroup_ID", ""));
                        int newWrapGroupID = 0;
                        if (TranslationDictionary.ContainsKey("WRAPGROUP" + intOldWrapGroupID))
                        {
                            newWrapGroupID = AlwaysConvert.ToInt(TranslationDictionary["WRAPGROUP" + intOldWrapGroupID]);
                        }
                        else if (intOldWrapGroupID > 0)
                        {
                            LogError(GetPercentResult(), "Product Import, Unable to associate Product to WrapGroup because WrapGroup not found. ProductID:" + intOldProductID + ", Name:" + objProduct.Name + ", WrapGroupID:" + intOldWrapGroupID);
                        }


                        objProduct.WrapGroupId = newWrapGroupID;

                        //save product
                        //objProduct.Save();

                        // PRESERVE ID's FOR OBJECT
                        String messages = String.Empty;
                        objProduct.ProductId = intOldProductID;
                        SaveResult saveResult =  ACDataSource.SaveProduct(objProduct, this.PreserveIdOption, ref messages);
                        if(messages.Length > 0) LogWarning(GetPercentResult(), messages);

                        // SKIP IMPORTING MORE DETAILS IF FAILED SAVING OBJECT
                        if (saveResult == SaveResult.Failed) continue;

                        int newProductID = objProduct.ProductId;
                        TranslationDictionary.Add("PRODUCT" + intOldProductID, newProductID.ToString());

                        //	copy old attributes
                        //	go direct to db to avoid OSD problems
                        XmlNodeList nlsAttributes = nodProduct.SelectNodes("Attributes/Attribute");
                        if (nlsAttributes.Count > 0)
                        {
                            Option objAttribute = null;;
                            int intOldAttributeID = 0; 
                            foreach (XmlNode nodAttribute in nlsAttributes)
                            {

                                try
                                {
                                    intOldAttributeID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodAttribute, "ID", ""), 0);
                                    objAttribute = new Option();
                                    objAttribute.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodAttribute, "Name", ""));
                                    short tempOrderBy = AlwaysConvert.ToInt16(XmlUtility.GetElementValue(nodAttribute, "OrderBy", ""));
                                    objAttribute.ProductOptions.Add(new ProductOption(newProductID, 0, tempOrderBy));

                                    //	save
                                    objAttribute.Save();
                                    int newAttributeID = objAttribute.OptionId;
                                    if (intOldAttributeID > 0)
                                    {
                                        if (!TranslationDictionary.ContainsKey("ATTRIBUTE" + intOldAttributeID))
                                        {
                                            TranslationDictionary.Add("ATTRIBUTE" + intOldAttributeID, newAttributeID.ToString());
                                        }
                                    }

                                    //	copy options
                                    XmlNodeList nlsOptions = nodAttribute.SelectNodes("Options/Option");
                                    if (nlsOptions.Count > 0)
                                    {
                                        OptionChoice objOption = null;
                                        int intOldOptionID = 0;
                                        foreach (XmlNode nodOption in nlsOptions)
                                        {
                                            try
                                            {
                                                intOldOptionID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodOption, "ID", ""), 0);
                                                objOption = new OptionChoice();
                                                objOption.OptionId = newAttributeID;
                                                objOption.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOption, "Name", ""));
                                                objOption.OrderBy = (short)AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOption, "OrderBy", ""));

                                                //Comments:
                                                /*
                                                The values 1, 2, and 3 are no longer tracked at the 
                                                attribute / option level. They were intended to allow 
                                                calculation for the values in the variant matrix. 
                                                In AC6, we no longer calculate the values this way. 
                                                For 4 and 5:

                                                Inventory management should not be too difficult. The AC55 
                                                root store node has an EnableInventory option, this value must 
                                                be transferred to the AC6 store object. In addition, these are
                                                the product translations:

                                                AC55 Product.InvMode = AC6 Product.InventoryMode. The integer 
                                                values are the same between these two versions, but the string 
                                                representation in the enum has changed.

                                                AC55 Product.InStock � Product.ReserveStock = AC6 Product.InStock. 
                                                AC55 kept track of stock that was �reserved� � associated with an 
                                                order but not yet shipped. This did not seem to be helpful, and was
                                                rather confusing. AC6 no longer tracks this value.

                                                AC55 Product.Reorder = AC6 Product.InStockWarningLevel. This field 
                                                was just renamed. No feature change.
                                                */

                                                

                                                //These properties are no longer used
                                                objOption.PriceModifier = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOption, "PriceMod", ""));
                                                objOption.SkuModifier = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOption, "SKUMod", ""));
                                                                                                
                                                objOption.WeightModifier = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOption, "WeightMod", ""));
                                                
                                                //save
                                                objOption.Save();
                                                objAttribute.Choices.Add(objOption);
                                                int newOptionID = objOption.OptionChoiceId;
                                                if (intOldOptionID > 0)
                                                {
                                                    if (!TranslationDictionary.ContainsKey("OPTION" + intOldOptionID))
                                                    {
                                                        TranslationDictionary.Add("OPTION" + intOldOptionID, newOptionID.ToString());
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                LogError(GetPercentResult(), "Product Attribute Options Import, OptionID:" + intOldOptionID + " , Name: " + objOption.Name + ", " + ex.Message + "\n" + ex.StackTrace);
                                            }
                                        } //next nodOption								
                                    }
                                    objAttribute.Choices.Save();
                                }
                                catch (Exception ex)
                                {
                                    LogError(GetPercentResult(), "Product Attributes Import, AttributeID:" + intOldAttributeID + " , Name: " + objAttribute.Name + ", " + ex.Message + "\n" + ex.StackTrace);
                                }
                            }//next nodAttribute
                            //objAttribute = null
                        }

                        //import OSD data
                        //inventory managemnt

                        // UPDATE THE INVENTORY MODE
                        objProduct.InventoryModeId = (byte)AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodProduct, "InvMode", ""));
                        objProduct.InventoryMode = (InventoryMode)AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodProduct, "InvMode", ""));

                        XmlNodeList nlsOptionMatrix = nodProduct.SelectNodes("OptionMatrix/OSD");
                        if(nlsOptionMatrix.Count > 0 ){
                            ProductVariantManager objProductVariantManager = new ProductVariantManager(objProduct.ProductId);
                            if (objProductVariantManager != null && objProductVariantManager.Count > 0)
                            {
                                foreach(XmlNode nodMatrixItem in nlsOptionMatrix){
                                    int intOldOSDID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodMatrixItem, "ID", ""), 0);
                                    int[] arrOldIDs = new int[8];
                                    int[] arrNewIDs = new int[8];                                    
                                    try{
                                        for(int j = 0; j <= 7; j++){
                                            arrOldIDs[j] = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodMatrixItem, "Option_ID" + j,"0"));
                                            arrNewIDs[j] = 0;
                                            if( arrOldIDs[j] > 0){
                                                if (TranslationDictionary.ContainsKey("OPTION" + arrOldIDs[j]))
                                                {
                                                    arrNewIDs[j] = AlwaysConvert.ToInt(TranslationDictionary["OPTION" + arrOldIDs[j]]);
                                                }
                                            }
                                        }                                        
                                        ProductVariant objVariant = objProductVariantManager.GetVariantFromOptions(arrNewIDs);
                                        if(objVariant != null){
                                            objVariant.Sku = XmlUtility.GetElementValue(nodMatrixItem, "SKU","");
                                            // AC7.InStock = AC5.InStock - AC5.ReserveStock;
                                            objVariant.InStock = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodMatrixItem, "InStock","0")) - AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodMatrixItem, "ReserveStock",""),0);
                                            
                                            String sTemp = XmlUtility.GetElementValue(nodMatrixItem, "UnitPrice","");
                                            if(!String.IsNullOrEmpty(sTemp) ){
                                                objVariant.Price = AlwaysConvert.ToDecimal(sTemp);
                                                objVariant.PriceMode = ModifierMode.Modify;
                                            }
                                            sTemp = XmlUtility.GetElementValue(nodMatrixItem, "UnitWeight", "");
                                            if(!String.IsNullOrEmpty(sTemp) ){
                                                objVariant.Weight = AlwaysConvert.ToDecimal(sTemp);
                                                objVariant.WeightMode = ModifierMode.Modify;
                                            }
                                            objVariant.Available = (AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodMatrixItem, "Available","")) != 0);
                                            objVariant.Save();

                                            if (!TranslationDictionary.ContainsKey("OSD" + intOldProductID + "|" + intOldOSDID))
                                            {
                                                TranslationDictionary.Add("OSD" + intOldProductID + "|" + intOldOSDID, objVariant.ProductVariantId.ToString());
                                            }
                                        }

                                    }catch (Exception ex)
                                    {
                                        LogError(GetPercentResult(), "Product OptionMatrix(variants data) Import, OSD ID:" + intOldOSDID + ", Some unexpected error has occured while import." + ex.Message + " \n" + ex.StackTrace);
                                    }
                                } //next nodMatrixItem
                            }
                        }

                        string lstOldCategories = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodProduct, "Categories", ""));
                        if (lstOldCategories.Length > 0)
                        {
                            string[] arrOldCategories = lstOldCategories.Split(',');
                            for (int j = 0; j < arrOldCategories.Length; j++)
                            {
                                intOldCategoryID = AlwaysConvert.ToInt(arrOldCategories[j]);
                                if (TranslationDictionary.ContainsKey("CATEGORY" + intOldCategoryID))
                                {
                                    objProduct.Categories.Add(AlwaysConvert.ToInt(TranslationDictionary["CATEGORY" + intOldCategoryID]));
                                }
                                else if (intOldCategoryID > 0)
                                {
                                    LogError(GetPercentResult(), "Product Import, Unable to associate Product to Category because Category not found. ProductID:" + intOldProductID + ", Name:" + objProduct.Name + ", CategoryID:" + intOldCategoryID);
                                }
                            }//next j
                            objProduct.Categories.Save();
                            objProduct.Save();
                        }

                        //	import SPECIALS
                        XmlNodeList nlsSpecials = nodProduct.SelectNodes("Specials/Special");
                        if (nlsSpecials.Count > 0)
                        {
                            Special objSpecial = null;
                            int intOldSpecialID = 0;
                            int newSpecialID;
                            foreach (XmlNode nodSpecial in nlsSpecials)
                            {
                                try{
                                    intOldSpecialID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodSpecial, "ID", ""), 0);
                                    objSpecial = new Special();
                                    objSpecial.ProductId = newProductID;
                                    objSpecial.Price = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodSpecial, "Price", ""));
                                    
                                    string srtStartDate = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodSpecial, "StartDT", ""));
                                    string srtEndDate = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodSpecial, "EndDT", ""));
                                    DateTime tempDate = ConvertDateTime(srtStartDate, "Special start date");
                                    if (tempDate > DateTime.MinValue) objSpecial.StartDate = tempDate;

                                    tempDate = ConvertDateTime(srtEndDate, "Special end date");
                                    if (tempDate > DateTime.MinValue) objSpecial.EndDate = tempDate;
                                    

                                    //Comments:
                                    //verify: Yes, groups are renamed to �Groups� in the database. This was to fit the ASP.NET membership model, but the two are equivalent.

                                    string lstGroups = XmlUtility.GetElementValue(nodSpecial, "Groups", "");
                                    string[] arrGroups;

                                    if (lstGroups.Length > 0)
                                    {
                                        arrGroups = lstGroups.Split(',');
                                        foreach (string group in arrGroups)
                                        {
                                            int intOldGroupID = AlwaysConvert.ToInt(group);
                                            if (intOldGroupID > 0 && TranslationDictionary.ContainsKey("GROUP" + intOldGroupID))
                                            {
                                                int groupId = AlwaysConvert.ToInt(TranslationDictionary["GROUP" + intOldGroupID]);
                                                objSpecial.SpecialGroups.Add(new SpecialGroup(objSpecial.SpecialId, groupId));
                                            }
                                        }
                                    }

                                    //save
                                    objSpecial.Save();
                                    newSpecialID = objSpecial.SpecialId;
                                    if (intOldSpecialID > 0)
                                    {
                                        if (!TranslationDictionary.ContainsKey("SPECIAL" + intOldSpecialID))
                                        {
                                            TranslationDictionary.Add("SPECIAL" + intOldSpecialID, newSpecialID.ToString());
                                        }

                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogError(GetPercentResult(), "Product Specials Import, SpecialID:" + intOldSpecialID + " , ProductID: " + intOldProductID + ", " + ex.Message + "\n" + ex.StackTrace);
                                }
                            }//next nodSpecial
                        }

                        //	import CUSTOMFIELDS (Product)
                        string lstCustomFields = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodProduct, "CustomFields", ""));
                        try{
                            if (lstCustomFields.Length > 0)
                            {
                                int productTemplateId = 0;
                                string[] arrCustomFields = lstCustomFields.Split('&');

                                // build a key for product template using the Custom field names
                                string productTemplateKey = "PRODUCTTEMPLATES0";
                                foreach (string sCustomField in arrCustomFields)
                                {
                                    string[] arrTemp2 = sCustomField.Split('=');
                                    productTemplateKey += HttpUtility.UrlDecode(arrTemp2[0]);
                                }

                                if (!TranslationDictionary.ContainsKey(productTemplateKey))
                                {
                                    ProductTemplate productTemplate = new ProductTemplate();
                                    productTemplate.StoreId = Token.Instance.StoreId;
                                    productTemplate.Name = "AC55 Imported Template" + i;
                                    productTemplate.Save();
                                    InputField inputField;
                                    ProductTemplateField productCF;
                                    foreach (string sCustomField in arrCustomFields)
                                    {
                                        string[] arrTemp2 = sCustomField.Split('=');
                                        if (arrTemp2.Length < 2) continue;
                                        inputField = new InputField();

                                        //the field type should be textbox
                                        inputField.InputType = InputType.TextBox;
                                        //all choices are merchant fields
                                        inputField.IsMerchantField = true;
                                        //the field name is the same as the field name in ac55
                                        inputField.Name = HttpUtility.UrlDecode(arrTemp2[0]);
                                        // the user prompt should be the field name followed by a colon �:� character
                                        inputField.UserPrompt = HttpUtility.UrlDecode(arrTemp2[0]) + ":";
                                        inputField.ProductTemplateId = productTemplate.ProductTemplateId;
                                        inputField.Save();

                                        productTemplate.InputFields.Add(inputField);

                                        //fill in the inputfield values for the product, using the assigned template and the values from ac55 product data                                
                                        productCF = new ProductTemplateField();
                                        productCF.InputFieldId = inputField.InputFieldId;
                                        productCF.InputValue = HttpUtility.UrlDecode(arrTemp2[1]);
                                        //productCF.Save();

                                        objProduct.TemplateFields.Add(productCF);

                                    }
                                    productTemplate.Save();
                                    productTemplateId = productTemplate.ProductTemplateId;
                                    TranslationDictionary.Add(productTemplateKey, productTemplateId.ToString());

                                    // FROM 7.4 WE SUPPORT MULTIPLE PRODUCT TEMPLATES, SO IMPLEMENTATION IS CHANGED
                                    //objProduct.ProductTemplateId = productTemplateId;
                                    ProductProductTemplate ppt = ProductProductTemplateDataSource.Load(objProduct.ProductId, productTemplateId);
                                    if (ppt == null)
                                    {
                                        ppt = new ProductProductTemplate(objProduct.ProductId, productTemplateId);
                                        ppt.Save();
                                        objProduct.ProductProductTemplates.Add(ppt);
                                    }
                                }
                                else
                                {

                                    // TEMPLATE ALREADY CREATED, ASSIGN IT TO THIS PRODUCT
                                    productTemplateId = AlwaysConvert.ToInt(TranslationDictionary[productTemplateKey]);
                                    
                                    // FROM 7.4 WE SUPPORT MULTIPLE PRODUCT TEMPLATES, SO IMPLEMENTATION IS CHANGED
                                    //objProduct.ProductTemplateId = productTemplateId;
                                    ProductProductTemplate ppt = ProductProductTemplateDataSource.Load(objProduct.ProductId, productTemplateId);
                                    if (ppt == null)
                                    {
                                        ppt = new ProductProductTemplate(objProduct.ProductId, productTemplateId);
                                        ppt.Save();
                                        objProduct.ProductProductTemplates.Add(ppt);
                                    }

                                    ProductTemplate ptemplate = ProductTemplateDataSource.Load(productTemplateId);

                                    if(ptemplate != null){
                                        for (int j = 0; j < arrCustomFields.Length; j++)
                                        {
                                            String sCustomField = arrCustomFields[j];
                                            string[] arrTemp2 = sCustomField.Split('=');
                                            if (arrTemp2.Length < 2) continue;

                                            foreach(InputField inputField in ptemplate.InputFields)
                                            {                                                
                                                if(arrTemp2[0] == inputField.Name){
                                                    ProductTemplateField productCF = new ProductTemplateField();
                                                    productCF.InputFieldId = inputField.InputFieldId;
                                                    productCF.InputValue = HttpUtility.UrlDecode(arrTemp2[1]);
                                                    productCF.ProductId = objProduct.ProductId;
                                                    
                                                    productCF.Save();
                                                    objProduct.TemplateFields.Add(productCF);
                                                }
                                            }
                                        }

                                        objProduct.TemplateFields.Save();
                                    }
                                }
                                // TODO: need to add custom fields in collection
                                //objProduct.CustomFields = dicTemp;
                                
                                objProduct.Save();
                            }//end if
                        }
                        catch (Exception ex)
                        {
                            LogError(GetPercentResult(), "Product Custom Fields Import, ProductID:" + intOldProductID + ", " + ex.Message + "\n" + ex.StackTrace);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "Products Import, ProductID:" + intOldProductID + " , Name: " + objProduct.Name +  ex.Message + "\n" + ex.StackTrace);
                    }

                    // UPDATE LAST MODIFIED DATE
                    DateTime dtLastUpdate = ConvertDateTime(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodProduct, "LastUpdateDT", "")), "last modified date");
                    if (dtLastUpdate > DateTime.MinValue) objProduct.LastModifiedDate = dtLastUpdate;
                                        
                    // SAVE AGAIN
                    objProduct.Save();

                }//next nodProduct
                //	'clean up
                objProduct = null;
                LogStatus(GetPercentResult(), "Products Import Complete...");

            }
            else
            {
                currentPhasePercent = currentPhaseWeightage;
                //LogStatus(GetPercentResult(), "No Products Available to Import...");

            }//end if
            //}else{
            //        //response.write("Skipping Products Import...")

            //    end if
        }

        private void ImportStoreData(XmlNode nodSourceStore)
        {
            //if InStr(lstCopyOpts, ",STORE,") > 0 then
            //	'groups selected for copy

            currentPhasePercent = 0;
            bool haveStoreData = HaveStoreData(nodSourceStore);
            if (haveStoreData)
            {

                LogStatus(GetPercentResult(), "Importing Store Data .");

                //TODO: need to save in store settings?? 	
                //Token.Instance.Store.DateFormat = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodSourceStore,"DateFormat", Token.Instance.Store.DateFormat));
                //Token.Instance.Store.TimeFormat = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodSourceStore,"TimeFormat", Token.Instance.Store.TimeFormat));

                String sWeightUnit = XmlUtility.GetElementValue(nodSourceStore, "WeightUnit", "");
                if (!String.IsNullOrEmpty(sWeightUnit)) Token.Instance.Store.WeightUnit = ParseWeithUnit(HttpUtility.UrlDecode(sWeightUnit));
                // import measurement unit    
                //Token.Instance.Store.DimUnit = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodSourceStore,"DimUnit", Token.Instance.Store.DimUnit));
                string sDimUnit = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodSourceStore, "DimUnit", ""));
                if (!String.IsNullOrEmpty(sDimUnit))
                {
                    if (sDimUnit.Equals("in"))
                    {
                        Token.Instance.Store.MeasurementUnit = MeasurementUnit.Inches;
                    }
                    else
                    {
                        Token.Instance.Store.MeasurementUnit = MeasurementUnit.Centimeters;
                    }
                }


                //TODO: Verify enable inverntory setting is imported OK
                Token.Instance.Store.EnableInventory = ToBool(XmlUtility.GetElementValue(nodSourceStore, "EnableInventory", (Token.Instance.Store.EnableInventory ? "1" : "0")));

                // old themes are different so not importing themes
                //Token.Instance.Store.Theme = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodSourceStore, "Theme", Token.Instance.Store.Theme));

                // TODO Store.DiscountMode
                //Token.Instance.Store.DiscountMode = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodSourceStore, "DiscountMode", Token.Instance.Store.DiscountMode));

                //IMPORT THE NEXT ORDER NUMBER
                int nextOrderNumber = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodSourceStore, "OrderNumber", "0"));

                // THE ORDER Number SHOULD BE GREATER THEN THE EXISTING ORDER NUMBERS
                int maxOrderNumber = StoreDataSource.GetMaxOrderNumber();
                if (nextOrderNumber > maxOrderNumber)
                {
                    Token.Instance.Store.NextOrderId = StoreDataSource.SetNextOrderNumber(nextOrderNumber);
                }

                //TODO: do we need to import custom fields for store settings, i think these are not utilized any where

                string lstCustomFields = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodSourceStore, "CustomFields", ""));
                //CustomField CF = new CustomField();
                StoreSetting objStoreSetting = new StoreSetting();
                if (lstCustomFields.Length > 0)
                {
                    //CustomFieldCollection dicTemp = Token.Instance.Store.CustomFields;
                    String[] arrCustomFields = lstCustomFields.Split('&');
                    for (int i = 0; i < arrCustomFields.Length; i++)
                    {
                        string[] arrTemp2 = arrCustomFields[i].Split('=');
                        //dicTemp.Add(HttpUtility.UrlDecode(arrTemp2(0)), HttpUtility.UrlDecode(arrTemp2(1)));
                        //TODO: Veirfy the name given to this Field 
                        //
                        objStoreSetting.FieldName = HttpUtility.UrlDecode(arrTemp2[0]);
                        objStoreSetting.FieldValue = HttpUtility.UrlDecode(arrTemp2[1]);
                        objStoreSetting.StoreId = Token.Instance.StoreId;
                        objStoreSetting.Save();
                        Token.Instance.Store.Settings.Add(objStoreSetting);
                    }
                    Token.Instance.Store.Settings.Save();
                }



                // CURRENCY IMPORT DISABLED (bug 7100)
                //int intOldCurrencyID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodSourceStore, "DefaultCurrency_ID", ""));
                //int newCurrencyID = 0;
                //Currency baseCurrency  = null;
                //if (TranslationDictionary.ContainsKey("CURRENCY" + intOldCurrencyID))
                //{
                //    newCurrencyID = AlwaysConvert.ToInt(TranslationDictionary["CURRENCY" + intOldCurrencyID]);
                //    baseCurrency = CurrencyDataSource.Load(newCurrencyID);
                //}
                //else if (intOldCurrencyID > 0)
                //{
                //    LogError(GetPercentResult(), "Store Settings Import, Unable to set store Currency because Currency not found. CurrencyID:" + intOldCurrencyID);
                //}

                //if (baseCurrency != null )
                //{
                //    Token.Instance.Store.BaseCurrency = baseCurrency;                
                //}

                int intOldWarehouseID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodSourceStore, "DefaultWarehouse_ID", ""));
                if (intOldWarehouseID > 0)
                {
                    if (TranslationDictionary.ContainsKey("WAREHOUSE" + intOldWarehouseID))
                    {
                        Token.Instance.Store.DefaultWarehouseId = AlwaysConvert.ToInt(TranslationDictionary["WAREHOUSE" + intOldWarehouseID]);
                    }
                    else if (intOldWarehouseID > 0)
                    {
                        LogError(GetPercentResult(), "Store Settings Import, Unable to set Default Store Warehouse because Warehouse not found. WarehouseID:" + intOldWarehouseID);
                    }
                }
                Token.Instance.Store.Save();

                //CHECK FOR LOGGING
                //    //response.write(" o")
                //    Token.Instance.Store.Logging = (AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodSourceStore, "Logging")) <> 0)
                ////	'IMPORT LOG SETTINGS
                //    nodTemp = XmlUtility.GetElement(nodSourceStore, "LogSettings", false)
                //    if not (nodTemp is nothing) then
                //        Dim objLogging as cbLogging = new cbLogging()
                //        objLogging.Load(objDestToken, Token.Instance.StoreId)
                //        objLogging.Maintenance = XmlUtility.GetElementValue(nodTemp, "Maintenance", "1")
                //        objLogging.SizeLimit = XmlUtility.GetElementValue(nodTemp, "SizeLimit", "5000")
                //        objLogging.Buffer = XmlUtility.GetElementValue(nodTemp, "Buffer", "3000")
                //        objLogging.ReportDir = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodTemp, "ReportDir", ""))
                //        objLogging.ReportURL = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodTemp, "ReportURL", ""))
                //        objLogging.ArchiveDir = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodTemp, "ArchiveDir", ""))
                //        objLogging.ArchiveURL = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodTemp, "ArchiveURL", ""))
                //        objLogging.Save(objDestToken)
                //        objLogging = null
                //    end if

                LogStatus(GetPercentResult(), "Store Data Import Complete...");
            }

            //else
            //    //response.write("Skipping Store Import...")

            //end if
        }

        private bool HaveStoreData(XmlNode nodSourceStore)
        {            
            if (!String.IsNullOrEmpty(XmlUtility.GetElementValue(nodSourceStore, "WeightUnit", string.Empty))) return true;
            if (!String.IsNullOrEmpty(XmlUtility.GetElementValue(nodSourceStore, "DimUnit", string.Empty))) return true;
            if (!String.IsNullOrEmpty(XmlUtility.GetElementValue(nodSourceStore, "EnableInventory", string.Empty))) return true;
            if (!String.IsNullOrEmpty(XmlUtility.GetElementValue(nodSourceStore, "CustomFields", string.Empty))) return true;
            if (!String.IsNullOrEmpty(XmlUtility.GetElementValue(nodSourceStore, "DefaultWarehouse_ID", string.Empty))) return true;

            return false;
        }

        private void ImportUsers(XmlNode nodSourceStore)
        {
            currentPhasePercent = 0;
            //LogStatus(GetPercentResult(), "Checking Users...");

            XmlNodeList nlsUsers = nodSourceStore.SelectNodes("Users/User");
            if (nlsUsers.Count > 0)
            {
                int i = 0; // counter variable
                LogStatus(GetPercentResult(), "Importing " + nlsUsers.Count + " Users .");

                User objUser = null;
                Address objAddress = null;
                bool bLegacyPassword = false;
                int newUserID = 0;
                int intOldAddressID = 0;
                int newAddressID = 0;
                int newGroupID = 0;

                int intOldUserID = 0;
                string SessionId = string.Empty;
                string UserName = String.Empty;
                string Password = String.Empty;
                int AffiliateId = 0;
                string PayPalId = string.Empty;
                int FailedPasswordAttemptCount = 0;
                bool IsLockedOut = false;

                foreach (XmlNode nodUser in nlsUsers)
                {
                    // update the import status
                    i++;

                    currentPhasePercent = CalculatePartialPercentage(i, nlsUsers.Count);
                    LogStatus(GetPercentResult(), "Importing User " + i + " of " + nlsUsers.Count);

                    try
                    {
                        intOldUserID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodUser, "ID", "0"), 0);
                        UserName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodUser, "UserName", ""));

                        // BE CAREFULL, DONT UPDATE / OVERWRITE THE CURRENT USER, SKIP IMPORTING SUCH USER
                        if (this.PreserveIdOption == PreserveIdOption.OverwriteExistingObject && intOldUserID == Token.Instance.UserId)
                        {
                            LogWarning(GetPercentResult(), "User Import(Preserve Id Option: Delete Existing), Cannot import a user with same UserId as of currently logged in user, skipping user import. UserID:" + intOldUserID + ", Name:" + UserName);
                            continue;
                        }

                        //The Following Fields Does Not Exist
                        //objUser.BillAddr_ID = 0   Replaced with "PrimaryAddressId"
                        //objUser.ShipAddr_ID = 0   Does not Exist
                        //objUser.DefaultWishlist_ID = 0    Replaced With "PrimaryWishlistId"
                        //Session_ID does not exist
                        //objUser.Session_ID = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodUser, "Session_ID"))
                        

                        //	'try to set legacy password
                        //
                        //objUser.Password = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodUser, "Password"))
                        Password = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodUser, "Password", ""));
                        //objUser.SetPassword(Password);
                        //bLegacyPassword = (Len(objUser.Password) > 0)
                        bLegacyPassword = (Password.Length > 0);
                        //	'get new affiliate id
                        int intOldAffiliateID = AlwaysConvert.ToInt(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodUser, "Affiliate_ID", "")));
                        if (TranslationDictionary.ContainsKey("AFFILIATE" + intOldAffiliateID))
                        {
                            AffiliateId = AlwaysConvert.ToInt(TranslationDictionary["AFFILIATE" + intOldAffiliateID]);
                        }
                        else if (intOldAffiliateID > 0)
                        {
                            LogError(GetPercentResult(), "User Import, Unable to associate User to Affiliate because Affiliate not found. UserID:" + intOldUserID + ", Name:" + UserName + ", AffiliateID:" + intOldAffiliateID);
                        }
                        //	'SET AC55SR2 USER DATA
                        PayPalId = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodUser, "PayPalID", ""));
                        //objUser.LoginFailures = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "LoginFailures"))
                        FailedPasswordAttemptCount = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodUser, "LoginFailures", ""));
                        //objUser.AccountLocked = ToBool(XmlUtility.GetElementValue(objNode, "AccountLocked"))
                        IsLockedOut = ToBool(XmlUtility.GetElementValue(nodUser, "AccountLocked", "0"), false);
                        //	'save

                        //Here int result is returned to check the update status
                        //intResult = objUser.Save();
                        //if intResult = -2 then

                        objUser = UserDataSource.LoadForUserName(UserName);
                        if (objUser == null)
                        {
                            //User does not exists
                            objUser = new User();
                            objUser.UserName = UserName;
                            objUser.PrimaryAddressId = 0;
                            objUser.PrimaryWishlistId = 0;
                            
                            objUser.AffiliateId = AffiliateId;
                            objUser.PayPalId = PayPalId;
                            objUser.FailedPasswordAttemptCount = FailedPasswordAttemptCount;
                            objUser.IsLockedOut = false;

                            // IN AC5x USERS WERE ALWAYS APPROVED
                            objUser.IsApproved = !IsLockedOut;


                            //	'NEED TO UPDATE LASTAUTHDT
                            string strLastAuthDT = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodUser, "LastAuthDT", ""));
                            DateTime tempDate = ConvertDateTime(strLastAuthDT, "User LastAuthDT");
                            if (tempDate > System.DateTime.MinValue)
                            {
                                //Token.Instance.StoreGroupDB.QueryNoRS("UPDATE USERS SET LastAuthDT=" & FCast.FSQLDate(dtLastAuth, intDBType) & " WHERE User_ID=" & intNewUserID)
                                objUser.LastLoginDate = tempDate;

                                // SET THE OTHER ACTIVITY DATES TO PREVENT ACCOUNT LOCKING
                                objUser.LastActivityDate = tempDate;
                                objUser.LastPasswordChangedDate = tempDate;
                            }
                            

                            // PRESERVE ID's FOR OBJECT
                            String messages = String.Empty;
                            SaveResult saveResult;
                            objUser.UserId = intOldUserID;
                            saveResult = ACDataSource.SaveUser(objUser, this.PreserveIdOption, ref messages);
                            if (messages.Length > 0) LogWarning(GetPercentResult(), messages);
                            
                            // SKIP IMPORTING MORE DETAILS IF FAILED SAVING OBJECT
                            if (saveResult == SaveResult.Failed)
                            {
                                LogError(GetPercentResult(), "User Import, Unable to Save User . UserID:" + intOldUserID + ", Name:" + objUser.UserName);
                                // CONTINUE TO TRY TO IMPORT NEXT USER, SKIP REST OF THE IMPORT FOR THIS USER
                                continue;
                            }
                            else
                            {
                                newUserID = objUser.UserId;

                                if (intOldUserID > 0)
                                {
                                    if (!TranslationDictionary.ContainsKey("USER" + intOldUserID))
                                    {
                                        TranslationDictionary.Add("USER" + intOldUserID, newUserID.ToString());
                                    }
                                }
                            }
                            if (bLegacyPassword)
                            {
                                objUser.SetPassword(Password);
                            }
                            //	'INSERT USER_PASSWORD RECORDS  
                            else
                            {
                                XmlNodeList nlsPasswords = nodUser.SelectNodes("Password");

                                int intPasswordNumber;
                                String sPasswordHash;
                                DateTime dtPassword;
                                UserPassword userPassword;
                                foreach (XmlNode nodPassword in nlsPasswords)
                                {
                                    intPasswordNumber = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodPassword, "PasswordNumber", ""));
                                    sPasswordHash = XmlUtility.GetAttributeValue(nodPassword, "PasswordHash", "");


                                    if ((intPasswordNumber > 0) && (sPasswordHash.Length > 0))
                                    {

                                        try
                                        {
                                            //Password Decryption
                                            string ac5Password = Decrypt128(sPasswordHash, "a$8X~,sa^fk3K#!k");

                                            // GENERATE A RANDOM PASSWORD IF IT WAS BLANK OR COULD NOT BE DECRYPTED
                                            if (String.IsNullOrEmpty(ac5Password) || ac5Password == sPasswordHash)
                                            {
                                                if (sPasswordHash == "0KWT4jRoPiN6qXZWGNJvJQ==")
                                                {
                                                    LogWarning(GetPercentResult(), "User " + objUser.UserName + " had a blank password.  A new random password will be generated.");
                                                }
                                                else
                                                {
                                                    LogError(GetPercentResult(), "User " + objUser.UserName + " had a password value that could not be decrypted.  A new random password will be generated. (Password Number: " + intPasswordNumber + ", Password Hash: " + sPasswordHash + ")");
                                                }
                                                ac5Password = StringHelper.RandomString(8);
                                            }

                                            userPassword = new UserPassword();
                                            if (userPassword.Load(newUserID, AlwaysConvert.ToByte(intPasswordNumber)))
                                            {
                                                userPassword.Delete();
                                            }

                                            userPassword = new UserPassword(newUserID, AlwaysConvert.ToByte(intPasswordNumber));

                                            userPassword.Password = UserPasswordHelper.EncodePassword(ac5Password, "SHA1");
                                            userPassword.PasswordFormat = "SHA1";
                                            String strDtPassword = HttpUtility.UrlDecode(XmlUtility.GetAttributeValue(nodPassword, "PasswordDT", ""));
                                            dtPassword = ConvertDateTime(strDtPassword, "PasswordDT");
                                            if (dtPassword > DateTime.MinValue) userPassword.CreateDate = dtPassword;

                                            userPassword.Save();
                                            objUser.Passwords.Add(userPassword);
                                        }
                                        catch (Exception ex)
                                        {
                                            LogError(GetPercentResult(), "Error Occured During Importing Password. User: " + objUser.UserName + ", Password Number: " + intPasswordNumber + ", " + ex.Message + "\n" + ex.StackTrace);
                                        }

                                    }
                                }//Next nodPassword
                                objUser.Passwords.Save();
                            }

                            string lstTemp = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodUser, "CustomFields", ""));
                            //TODO:Is CustomField exist
                            //if (lstTemp.Length > 0){
                            //    dicTemp = objUser.GetCustomFields()
                            //    arrTemp = Split(lstTemp, "&")
                            //    for i = 0 to ubound(arrTemp)
                            //        arrTemp2 = Split(arrTemp(i),"=")
                            //        sTempKey = HttpUtility.UrlDecode(arrTemp2(0))
                            //        If Len(sTempKey) > 0 Then
                            //            sTempValue = HttpUtility.UrlDecode(arrTemp2(1))
                            //            dicTemp(sTempKey) = sTempValue
                            //        End If
                            //    next i
                            //    objUser.SetCustomFields(dicTemp)
                            //end if

                            XmlNodeList nlsAddresses = nodUser.SelectNodes("Addresses/Address");
                            foreach (XmlNode nodAddress in nlsAddresses)
                            {
                                try
                                {
                                    intOldAddressID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodAddress, "ID", "0"), 0);
                                    objAddress = new Address();
                                    objAddress.UserId = (int)newUserID;
                                    objAddress.Nickname = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodAddress, "NickName", ""));
                                    objAddress.Address1 = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodAddress, "Address1", ""));
                                    objAddress.Address2 = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodAddress, "Address2", ""));
                                    objAddress.City = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodAddress, "City", ""));
                                    objAddress.FirstName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodAddress, "FirstName", ""));
                                    objAddress.LastName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodAddress, "LastName", ""));
                                    objAddress.Company = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodAddress, "Company", ""));
                                    objAddress.Province = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodAddress, "Province", ""));
                                    objAddress.PostalCode = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodAddress, "PostalCode", ""));
                                    string sCountryCode = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodAddress, "CountryCode", ""));
                                    if (sCountryCode.Length > 0 && !sCountryCode.Equals("0"))
                                    {
                                        // VALIDATE THE COUNTRY CODE, THE COUNTRY MUST EXIST IN DATABASE
                                        Country tempCountry = CountryDataSource.Load(sCountryCode);
                                        if (tempCountry == null)
                                        {
                                            // ADD A NEW COUNTRY FOR THE GIVEN COUNTRY CODE
                                            tempCountry = new Country();
                                            tempCountry.CountryCode = sCountryCode;
                                            tempCountry.Name = sCountryCode;
                                            tempCountry.Save();

                                            //LOG THE WARNING MESSAGE
                                            LogWarning(GetPercentResult(), "Warning: User address import, Country not found for country code:" + sCountryCode + ", added a new country to associate the address to that country. Old UserId:" + intOldUserID + " User Name:\'" +  objUser.UserName + "\', Old Address Id:" + intOldAddressID + ".");
                                        }
                                        objAddress.CountryCode = sCountryCode;
                                    }
                                    objAddress.Phone = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodAddress, "Phone", ""));
                                    objAddress.Fax = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodAddress, "Fax", ""));
                                    objAddress.Email = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodAddress, "Email", ""));
                                    objAddress.Residence = ToBool(XmlUtility.GetElementValue(nodAddress, "Residence", "1"), false);
                                    //	'save
                                    objAddress.Save();
                                    //	'FORCE ADDRESS VALIDATION FLAG
                                    if (AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodAddress, "Validated", "0")) == 1)
                                    {
                                        //Token.Instance.StoreGroupDB.QueryNoRS("UPDATE ADDRESSES SET Validated=1 WHERE Address_ID=" & objAddress.Address_ID)
                                        objAddress.Validated = true;
                                        objAddress.Save();
                                    }
                                    if (intOldAddressID > 0)
                                    {
                                        if (!TranslationDictionary.ContainsKey("ADDRESS" + intOldAddressID))
                                        {
                                            TranslationDictionary.Add("ADDRESS" + intOldAddressID, objAddress.AddressId.ToString());
                                        }
                                    }
                                    objUser.Addresses.Add(objAddress);
                                }
                                catch (Exception ex)
                                {
                                    LogError(GetPercentResult(), "User Address Import, UserID:" + intOldUserID + " , Name: " + UserName + ", AddressID:" + intOldAddressID + ", " + ex.Message + "\n" + ex.StackTrace);
                                }
                            }//next nodAddress
                            objUser.Addresses.Save();
                            objAddress = null;
                        }
                        else
                        {
                            //User Not Imported But existing user is loaded
                            newUserID = objUser.UserId;
                            if (!TranslationDictionary.ContainsKey("USER" + intOldUserID))
                            {
                                TranslationDictionary.Add("USER" + intOldUserID, newUserID.ToString());
                            }
                            
                            LogWarning(GetPercentResult(), "Warning: User \'" + objUser.UserName + "\' already exists.  User not imported.");
                            continue;
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "User Import, UserID:" + intOldUserID + " , Name: " + UserName + ", " + ex.Message + "\n" + ex.StackTrace);
                        continue; // SKIP IMPORTING THIS USER DATA
                    }

                    //	'update billaddr_ID
                    intOldAddressID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodUser, "BillAddr_ID", "0"), 0);
                    if (TranslationDictionary.ContainsKey("ADDRESS" + intOldAddressID))
                    {
                        newAddressID = AlwaysConvert.ToInt(TranslationDictionary["ADDRESS" + intOldAddressID]);
                        objUser.PrimaryAddressId = newAddressID;
                        

                        Address primaryAddress = AddressDataSource.Load(newAddressID);
                        if (primaryAddress != null) objUser.Email = primaryAddress.Email;
                        objUser.Save();
                    }
                    

                    // IMPORT USER GROUPS
                    String lstOldGroups = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodUser, "Groups", ""));
                    if (lstOldGroups.Length > 0)
                    {
                        String[] arrOldGroups = lstOldGroups.Split(',');
                        //for j = 0 to ubound(arrOldGroups)
                        foreach (string Group in arrOldGroups)
                        {
                            //intOldGroupID = arrOldGroups(j)
                            int intOldGroupID = AlwaysConvert.ToInt(Group, 0);
                            if (TranslationDictionary.ContainsKey("GROUP" + intOldGroupID))
                            {
                                newGroupID = AlwaysConvert.ToInt(TranslationDictionary["GROUP" + intOldGroupID]);
                                objUser.UserGroups.Add(new UserGroup(objUser.UserId, newGroupID));
                            }
                            else
                            {
                                LogError(GetPercentResult(), "User Import, Unable to associate User to Group because Group not found,  UserID:" + intOldUserID + ", Name:" +  objUser.UserName + ", GroupID:" + intOldGroupID);
                            }
                        }
                        objUser.UserGroups.Save();
                    }


                }//next nodUser
                objUser = null;


                //TODO:if a  user is not imported so should we import the default settings for that user 
                //or not? and if not so should we move the follwoing code to the upper foreach block
                //as it seems that there will not be any effect by doing so.
                //LogStatus(GetPercentResult(), "Importing Address Defaults and Groups for " + nlsUsers.Count + " Users .");
                //User user;
                //foreach (XmlNode nodUser in nlsUsers)
                //{
                //    try
                //    {
                //        user = new User();
                //        intOldUserID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodUser, "ID", "0"), 0);
                //        if (TranslationDictionary.ContainsKey("USER" + intOldUserID))
                //        {
                //            newUserID = AlwaysConvert.ToInt(TranslationDictionary["USER" + intOldUserID]);
                //        }
                //        else
                //        {
                //            newUserID = 0;
                //        }
                //        if (!newUserID.Equals(0))
                //        {
                //            //	'update billaddr_ID
                //            intOldAddressID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodUser, "BillAddr_ID", "0"), 0);
                //            if (TranslationDictionary.ContainsKey("ADDRESS" + intOldAddressID))
                //            {
                //                newAddressID = AlwaysConvert.ToInt(TranslationDictionary["ADDRESS" + intOldAddressID]);
                //            }
                //            else
                //            {
                //                newAddressID = 0;
                //            }
                //            //if (!newAddressID.Equals(0))
                //            //{
                //            //   
                //            //    //intResult = Token.Instance.StoreGroupDB.QueryNoRS("UPDATE USERS SET BillAddr_ID=" & intNewAddressID & " WHERE User_ID=" & newUserID)

                //            //    if (user.Load(newUserID))
                //            //    {
                //            //        user.PrimaryAddressId = newAddressID;
                //            //        user.Save();
                //            //    }
                //            //}
                //            //else
                //            //{
                //            //    //	'no billing address id was identified, take the default
                //            //    //
                //            //    //newAddressID = AlwaysConvert.ToInt(Token.Instance.StoreGroupDB.QueryValue("SELECT Min(Address_ID) as BillAddr FROM ADDRESSES WHERE User_ID=" & intNewUserID))
                //            //    //intResult = Token.Instance.StoreGroupDB.QueryNoRS("UPDATE USERS SET BillAddr_ID=" & intNewAddressID & " WHERE User_ID=" & intNewUserID)
                //            //    user.PrimaryAddressId = newAddressID;
                //            //    user.Save();

                //            //}
                //            if (user.Load(newUserID))
                //            {
                //                user.PrimaryAddressId = newAddressID;
                //                user.Email = user.PrimaryAddress.Email;
                //            }


                //            //	'update ShipAddr_ID
                //            //intOldAddressID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodUser, "ShipAddr_ID", "0"), 0);
                //            //if (objTranslate.ContainsKey("ADDRESS" + intOldAddressID))
                //            //{
                //            //    newAddressID = AlwaysConvert.ToInt(objTranslate["ADDRESS" + intOldAddressID]);
                //            //}
                //            //else
                //            //{
                //            //    newAddressID = 0;
                //            //}
                //            //if (!newAddressID.Equals(0)){
                //            //    //
                //            //    //intResult = Token.Instance.StoreGroupDB.QueryNoRS("UPDATE USERS SET ShipAddr_ID=" & intNewAddressID & " WHERE User_ID=" & intNewUserID)
                //            //}else{
                //            ////	'no billing address id was identified, take the default
                //            //    //
                //            //    //intNewAddressID = AlwaysConvert.ToInt(Token.Instance.StoreGroupDB.QueryValue("SELECT Min(Address_ID) as ShipAddr FROM ADDRESSES WHERE User_ID=" & intNewUserID))
                //            //    //intResult = Token.Instance.StoreGroupDB.QueryNoRS("UPDATE USERS SET ShipAddr_ID=" & intNewAddressID & " WHERE User_ID=" & intNewUserID)
                //            //}

                //            //	'now get groups
                //            String lstOldGroups = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodUser, "Groups", ""));
                //            if (lstOldGroups.Length > 0)
                //            {
                //                String[] arrOldGroups = lstOldGroups.Split(',');
                //                //for j = 0 to ubound(arrOldGroups)
                //                foreach (string Group in arrOldGroups)
                //                {
                //                    //intOldGroupID = arrOldGroups(j)
                //                    int intOldGroupID = AlwaysConvert.ToInt(Group, 0);
                //                    if (TranslationDictionary.ContainsKey("GROUP" + intOldGroupID))
                //                    {
                //                        newGroupID = AlwaysConvert.ToInt(TranslationDictionary["GROUP" + intOldGroupID]);
                //                        user.UserGroups.Add(new UserGroup(user.UserId, newGroupID));
                //                    }
                //                }
                //                user.UserGroups.Save();
                //            }
                //        }
                //        user.Save();
                //    }
                //    catch (Exception ex)
                //    {
                //        LogError(GetPercentResult(), "Users  Address Defaults and Groups Import, UserID:" + intOldUserID + " , Name: " + UserName + ", " + ex.Message + "\n" + ex.StackTrace);
                //    }
                //}//next nodUser
                //nodUser = null;
                LogStatus(GetPercentResult(), "Users Import Complete...");
            }
            else
            {
                currentPhasePercent = currentPhaseWeightage;
                //LogStatus(GetPercentResult(), "No Users Available to Import...");
            }
            nlsUsers = null;
        }

        #endregion

        #region Pass 3

        #region Coupons Lines = 160

        private void ImportCoupons(XmlNode nodSourceStore)
        {
            //if InStr(lstCopyOpts, ",COUPONS,") > 0 then
            currentPhasePercent = 0;
            //LogStatus(GetPercentResult(), "Checking Basket Coupons (AC52)...");

            //TODO: Data is not exported.
            //XmlNodeList nlsItems = nodSourceStore.SelectNodes("BasketCoupons/BasketCoupon");
            //if( nlsItems.Count > 0){
            //    //response.write("Importing " & nlsItems.Count & " Basket Coupons .")

            //    Coupon objCoupon;
            //    //BasketCoupon objCoupon;

            //    foreach(XmlNode objNode in nlsItems)
            //    {
            //        //objCoupon = new BasketCoupon();
            //        objCoupon = new Coupon();

            //        int intOldCouponID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID","0"),0);
            //        //objCoupon.CouponType = 0
            //        objCoupon.CouponType=CouponType.Order;
            //        objCoupon.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Name", "Coupon"));
            //        //TODO: Confirm
            //        //objCoupon.MinValue = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(objNode, "MinPurchase"))
            //        objCoupon.MinPurchase = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(objNode, "MinPurchase",""));
            //        objCoupon.MaxValue = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(objNode, "MaxValue",""));
            //        //TODO: Field does not exist
            //        //objCoupon.StepValue = 0
            //        objCoupon.DiscountAmount = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(objNode, "DiscountAmount",""));
            //        //objCoupon.DiscountPercent = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(objNode, "DiscountPercent"))
            //        objCoupon.MaxUses = AlwaysConvert.ToInt16(XmlUtility.GetElementValue(objNode, "MaxUses",""));
            //        objCoupon.CouponCode = XmlUtility.GetElementValue(objNode, "CouponCode","");
            //        //TODO : Confirm 
            //        //objCoupon.ComboRule = cbCoupon.eComboRule.Restrict
            //        objCoupon.ComboRule = CouponRule.AllowSelected;
            //        //TOD Not found
            //        //objCoupon.Combos.ListData = ""
            //        //TODO: Confirm
            //        //objCoupon.ProductRule = cbCoupon.eProductRule.Exclude
            //        objCoupon.ProductRule = CouponRule.ExcludeSelected;
            //        objCoupon.StoreId = Token.Instance.StoreId;

            //    //	'save
            //        if (!objCoupon.Save().Equals(SaveResult.Failed)){
            //            objTranslate.Add("COUPON" + intOldCouponID, objCoupon.CouponId.ToString());
            //        }
            //        //response.write(" .")

            //    } //next objNode
            //    objCoupon = null;
            //    //response.write("")

            //}else{
            //    Status(GetPercentResult(), "No Basket Coupons Available To Import...");
            //}


            //response.write("Checking Coupons...")

            XmlNodeList nlsItems = nodSourceStore.SelectNodes("Coupons/Coupon");
            if (nlsItems.Count > 0)
            {

                int i = 0; // counter variable
                LogStatus(GetPercentResult(), "Importing " + nlsItems.Count + " Coupons .");
                Coupon objCoupon = null;
                int intOldCouponID = 0;
                foreach (XmlNode objNode in nlsItems)
                {

                    // update the import status
                    i++;

                    currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
                    LogStatus(GetPercentResult(), "Importing Coupon " + i + " of " + nlsItems.Count);

                    try
                    {
                        objCoupon = new Coupon();
                        intOldCouponID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID", "0"), 0);
                        objCoupon.StoreId = Token.Instance.StoreId;
                        objCoupon.CouponType = TranslateCouponType(AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "CouponType", "0"), 0));
                        objCoupon.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Name", "Coupon"));
                        if (objCoupon.CouponType == CouponType.Product)
                        {
                            // IF IT IS A AC55 QUANTITY(PRODUCT) COUPON
                            //objCoupon.MinValue = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(objNode, "MinValue"))
                            objCoupon.MinQuantity = (short)AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(objNode, "MinValue", ""));

                            //objCoupon.MaxValue = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(objNode, "MaxValue",""));
                            objCoupon.MaxQuantity = (short)AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(objNode, "MaxValue", ""));

                            //objCoupon.StepValue = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(objNode, "StepValue"))
                            objCoupon.QuantityInterval = (short)AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "StepValue", "0"), 0);

                            //if (ac5.min == ac5.max), then ac7.repeat is false
                            if (objCoupon.MinQuantity == objCoupon.MaxQuantity) objCoupon.QuantityInterval = 0;                            

                        }else{
                            // IT MUST BE A AC55 VALUE (ORDER) COUPON
                            objCoupon.MinPurchase = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(objNode, "MinValue", ""));

                            //objCoupon.MaxValue = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(objNode, "MaxValue",""));
                            objCoupon.MaxValue = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(objNode, "MaxValue", ""));

                        }                         

                        decimal discountAmount = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(objNode, "DiscountAmount", ""));
                        if (discountAmount == 0)
                        {
                            discountAmount = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(objNode, "DiscountPercent",""));
                            if (discountAmount > 0)
                            {
                                objCoupon.DiscountAmount = discountAmount;
                                objCoupon.IsPercent = true;
                            }
                        }
                        else
                        {
                            objCoupon.DiscountAmount = discountAmount;
                            objCoupon.IsPercent = false;
                        }
                        
                        objCoupon.MaxUsesPerCustomer = AlwaysConvert.ToInt16(XmlUtility.GetElementValue(objNode, "MaxUses", "0"), 0);
                        objCoupon.CouponCode = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "CouponCode", ""));
                        string srtStartDate = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "StartDT", ""));
                        string srtEndDate = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "EndDT", ""));
                        DateTime tempDate = ConvertDateTime(srtStartDate, "Coupon start date");
                        if (tempDate > DateTime.MinValue) objCoupon.StartDate = tempDate;

                        tempDate = ConvertDateTime(srtEndDate, "Coupon end date");
                        if (tempDate > DateTime.MinValue) objCoupon.EndDate = tempDate;
                        
                        int combine = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "ComboRule", ""), 0);
                        if (combine == 0)
                        {
                            // AC5X SETTING IS "Can Combine With Any Coupon"
                            objCoupon.AllowCombine = true;
                        }
                        else
                        {
                            // GET COUPONS ASSOCIATED TO THIS COUPON FOR USE WITH THE COMBINE RULE
                            string selectedCoupons = XmlUtility.GetElementValue(objNode, "Combos", string.Empty);
                            if (combine == 2)
                            {
                                // AC5X SETTING IS "Can Combine Only With Selected Coupons"
                                if (string.IsNullOrEmpty(selectedCoupons))
                                {
                                    // THERE ARE NO SELECTED COUPONS, THIS IS EQUIVALENT TO DO NOT COMBINE
                                    objCoupon.AllowCombine = false;
                                }
                                else
                                {
                                    LogWarning(GetPercentResult(), "Unable to import unsupported coupon combine rule settings, coupon will be set to disallow combine.  AC5x Coupon Id: " + intOldCouponID.ToString() + ", Name:" + objCoupon.Name + ", AC5x Combine Rule: Can Combine Only With Selected Coupons");
                                }
                            }
                            else
                            {
                                // AC5X SETTING IS "Can Combine With All Except Selected Coupons"
                                if (string.IsNullOrEmpty(selectedCoupons))
                                {
                                    // THERE ARE NO SELECTED COUPONS, THIS IS EQUIVALENT TO ALLOW COMBINE
                                    objCoupon.AllowCombine = true;
                                }
                                else
                                {
                                    LogWarning(GetPercentResult(), "Unable to import unsupported coupon combine rule settings, coupon will be set to disallow combine. AC5x Coupon Id: " + intOldCouponID.ToString() + ", Name:" + objCoupon.Name + ", AC5x Combine Rule: Can Combine With All Except Selected Coupons");
                                }
                            }
                        }

                        objCoupon.ProductRule = TranslateProductRule(AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "ProductRule", ""), 0));

                        //	'save
                        if (!objCoupon.Save().Equals(SaveResult.Failed))
                        {
                            //	'add translation
                            TranslationDictionary.Add("COUPON" + intOldCouponID, objCoupon.CouponId.ToString());
                        }

                        //TODO:
                        String GroupsList = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Groups", ""));
                        if (GroupsList.Length > 0)
                        {
                            String[] arrGroups = GroupsList.Split(',');
                            foreach (String oldGroupID in arrGroups)
                            {
                                int intOldGroupID = AlwaysConvert.ToInt(oldGroupID);
                                if (TranslationDictionary.ContainsKey("GROUP" + oldGroupID))
                                {
                                    int newGroupID = AlwaysConvert.ToInt(TranslationDictionary["GROUP" + oldGroupID]);
                                    objCoupon.CouponGroups.Add(new CouponGroup(objCoupon.CouponId, newGroupID));
                                }
                                else if (intOldGroupID > 0)
                                {
                                    LogError(GetPercentResult(), "Coupon Import, Unable to associate Coupon to Group because Group not found. CouponID:" + intOldCouponID + ", Name:" + objCoupon.Name + ", GroupID:" + intOldGroupID);
                                }
                            }
                            objCoupon.CouponGroups.Save();
                        }
                        //	'get product list
                        String lstProducts;
                        String[] arrProducts;
                        lstProducts = XmlUtility.GetElementValue(objNode, "Products", "");
                        if (lstProducts.Length > 0)
                        {
                            arrProducts = lstProducts.Split(',');
                            foreach (String oldProductID in arrProducts)
                            {
                                if (AlwaysConvert.ToInt(oldProductID) > 0)
                                {
                                    if (TranslationDictionary.ContainsKey("PRODUCT" + oldProductID))
                                    {
                                        int newProductID = AlwaysConvert.ToInt(TranslationDictionary["PRODUCT" + oldProductID]);
                                        objCoupon.CouponProducts.Add(new CouponProduct(objCoupon.CouponId, newProductID));
                                    }
                                    else
                                    {
                                        LogError(GetPercentResult(), "Coupons Import, product association failded because of product not found CouponID:" + intOldCouponID + " , Name: " + objCoupon.Name + ", ProductID:" + oldProductID);
                                    }
                                }
                            }
                            objCoupon.CouponProducts.Save();
                        }
                        //response.write(" .")
                        objCoupon.Save();
                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "Coupons Import, CouponID:" + intOldCouponID + " , Name: " + objCoupon.Name + ", " + ex.Message + "\n" + ex.StackTrace);
                    }
                } //next objNode
                objCoupon = null;
                //response.write("")
            }
            else
            {
                //LogStatus(GetPercentResult(), "No Coupons Available To Import...");
            }


            //response.write("Checking Coupon Associations .")

            //TODO:
            //How to Add Combination OR here Combo
            nlsItems = nodSourceStore.SelectNodes("Coupons/Coupon");
            //if( nlsItems.Count > 0){
            //    Coupon objCoupon ;
            //    String lstCombos;
            //    String[] arrCombos;
            //    foreach(XmlNode objNode in nlsItems){
            //        int intOldCouponID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID","0"),0);
            //        int newCouponID = AlwaysConvert.ToInt(objTranslate["COUPON" + intOldCouponID]);
            //        lstCombos = XmlUtility.GetElementValue(objNode, "Combos","");
            //        if (Len(lstCombos) > 0) AndAlso (intNewCouponID > 0) Then
            //            objCoupon = new cbCoupon()
            //            if objCoupon.Load(objDestToken, intNewCouponID) Then
            //                if Len(lstCombos) > 0 then
            //                    arrCombos = Split(lstCombos, ",")
            //                    for i = 0 to UBound(arrCombos)
            //                        arrCombos(i) = CStr(AlwaysConvert.ToInt(objTranslate.Item("COUPON" & AlwaysConvert.ToInt(arrCombos(i)))))
            //                    Next i
            //                    objCoupon.Combos.ListData = Join(arrCombos, ",")
            //                    objCoupon.Combos.Save(objDestToken)
            //                End If
            //            End If
            //            objCoupon = null;
            //        End If
            //        //response.write(" .")

            //    } //next objNode
            //    objCoupon = null
            //end if
            //response.write("")

            //else
            //    //response.write("Skipping Coupon Import...")

            //end if
        }

        #endregion

        #region Kits Lines = 140

        private void ImportKits(XmlNode nodSourceStore)
        {
                //if InStr(lstCopyOpts, ",KITS,") > 0 then
        //	'groups selected for copy
            //response.write("Checking Kits...")
            currentPhasePercent = 0;


            XmlNodeList nlsKits = nodSourceStore.SelectNodes("Kits/Kit");
            if (nlsKits.Count > 0)
            {
                int i = 0; // counter variable
                LogStatus(GetPercentResult(), "Importing " + nlsKits.Count + " Kits.");
				
                // objKit as
                
                KitComponent objComponent = null;                
                //String  lstKitOptions  = string.Empty;
                String[] arrKitOptions = null;

                foreach(XmlNode nodKit in nlsKits){
                    ProductKitComponent productKitComponent = null;
                    int intOldProductID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodKit, "ID","0"));
                    int intNewProductID  = 0;
                    if(intOldProductID == 0) continue; // SKIP INVALID KIT

                    try{

                        i++;
                        currentPhasePercent = CalculatePartialPercentage(i, nlsKits.Count);
                        LogStatus(GetPercentResult(), "Importing Kit " + i + " of " + nlsKits.Count);

                        if (TranslationDictionary.ContainsKey("PRODUCT" + intOldProductID)){
                            intNewProductID = AlwaysConvert.ToInt(TranslationDictionary["PRODUCT" + intOldProductID]);
                        }else{
                            intNewProductID = 0;
                        }

                        if(intNewProductID > 0 ){
                           XmlNodeList nlsKitComponents = nodKit.SelectNodes("KitComponents/KitComponent");
                            if(nlsKitComponents.Count > 0){
                                //objKit = new cbKit
                                productKitComponent = new ProductKitComponent();
                                productKitComponent.ProductId = intNewProductID;

                                int intOldComponentID = 0;
                                
                                foreach(XmlNode nodKitComponent in nlsKitComponents){

                                    try
                                    {
                                        intOldComponentID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodKitComponent, "ID", "0"));
                                        objComponent = new KitComponent();
                                        //objComponent.Kit_ID = intNewProductID

                                        objComponent.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodKitComponent, "Name", ""));

                                        // DEFAULT AS DROPDOWN
                                        objComponent.InputType = ConvertKitInputType(AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodKitComponent, "InputType", "1")));
                                        objComponent.Columns = AlwaysConvert.ToByte(XmlUtility.GetElementValue(nodKitComponent, "FieldSize", "0"));
                                        objComponent.HeaderOption = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodKitComponent, "HeaderOption", ""));
                                        //objComponent.TitleCellFormat = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodKitComponent,"TitleCellFormat"))
                                        //objComponent.DataCellFormat = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodKitComponent,"DataCellFormat"))
                                        //objComponent.OrderBy = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodKitComponent,"OrderBy"))
                                        objComponent.StoreId = Token.Instance.StoreId;


                                        //SAVE THE KIT COMPONENT
                                        SaveResult saveResult = objComponent.Save();
                                        if (saveResult == SaveResult.Failed)
                                        {
                                            // LOG ERROR AND CONTINUE TO IMPORT NEXT
                                            LogError(GetPercentResult(), "KitComponentImport: Error while saving KitComponent, ID:" + intOldComponentID + " , Name:" + objComponent.Name);
                                            continue;
                                        }

                                        TranslationDictionary.Add("KITCOMP" + intOldComponentID.ToString(), objComponent.KitComponentId.ToString());

                                        // ASSOCIATE THE KIT COMPONENT WITH THE PRODUCT
                                        productKitComponent.KitComponentId = objComponent.KitComponentId;
                                        productKitComponent.OrderBy = AlwaysConvert.ToInt16(XmlUtility.GetElementValue(nodKitComponent, "OrderBy", ""));
                                        productKitComponent.Save();

                                        //objKit.Components.Add(objComponent)


                                     

                                        XmlNodeList nlsKitProds = nodKitComponent.SelectNodes("KitProducts/KitProduct");
                                        foreach (XmlNode nodKitProd in nlsKitProds)
                                        {
                                            int intOldKitProductID = 0;
                                            KitProduct objKitProd = null;

                                            int intNewProductID2 = 0;

                                            try
                                            {
                                                int intOldProductID2 = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodKitProd, "Product_ID", "0"));

                                                if (TranslationDictionary.ContainsKey("PRODUCT" + intOldProductID2))
                                                {
                                                    intNewProductID2 = AlwaysConvert.ToInt(TranslationDictionary["PRODUCT" + intOldProductID2]);
                                                }

                                                if (intNewProductID2 > 0)
                                                {
                                                    intOldKitProductID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodKitProd, "ID", ""));
                                                    objKitProd = new KitProduct();

                                                    objKitProd.KitComponentId = objComponent.KitComponentId;
                                                    objKitProd.ProductId = intNewProductID2;
                                                    objKitProd.Quantity = AlwaysConvert.ToInt16(XmlUtility.GetElementValue(nodKitProd, "KitQty", "0"));

                                                    objKitProd.Price = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodKitProd, "KitPrice", ""));
                                                    objKitProd.PriceMode = InheritanceMode.Override;

                                                    objKitProd.Weight = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodKitProd, "KitWeight", ""));
                                                    objKitProd.WeightMode = InheritanceMode.Override;

                                                    // THE DISPLAY NAMES CAN NOT BE PARSED, BECAUSE WE USE HTML FORMATING, AND CODES TO FORMAT NAMES
                                                    // THIS FEATURE IS MISSING IN AC7
                                                    //objKitProd.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodKitProd, "DisplayName", ""));
                                                    Product product = ProductDataSource.Load(intNewProductID2);
                                                    if(product != null) objKitProd.Name = product.Name;


                                                    String lstKitOptions = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodKitProd, "KitOptions", String.Empty));

                                                    if (lstKitOptions.Length > 0)
                                                    {
                                                        arrKitOptions = lstKitOptions.Split(',');
                                                        List<string> newOptionsList = new List<string>();
                                                        for (int index = 0; index < arrKitOptions.Length; index++)
                                                        {
                                                            //	'loop each option, translate to new id
                                                            int intOldOptionID = AlwaysConvert.ToInt(arrKitOptions[index]);
                                                            if (TranslationDictionary.ContainsKey("OPTION" + intOldOptionID))
                                                            {
                                                                newOptionsList.Add(TranslationDictionary["OPTION" + intOldOptionID].ToString());
                                                            }
                                                            else
                                                            {
                                                                // OPTION CAN NOT BE TRANSLATED
                                                                LogError(GetPercentResult(), "KitProduct Options Import: Unable to Translate KitProduct Option Id because option not found, KitComponentId:" + intOldComponentID + " , KitComponentName:" + objComponent.Name + " , KitProductId:" + objKitProd.KitProductId + " , OptionId:" + intOldOptionID);
                                                            }
                                                        }
                                                        lstKitOptions = String.Join(",", newOptionsList.ToArray());
                                                    }
                                                    objKitProd.OptionList = lstKitOptions;

                                                    objKitProd.OrderBy = AlwaysConvert.ToInt16(XmlUtility.GetElementValue(nodKitProd, "OrderBy", String.Empty));

                                                   

                                                    //	SAVE                                                    
                                                    objComponent.KitProducts.Add(objKitProd);
                                                    objKitProd.Save();

                                                    if (!TranslationDictionary.ContainsKey("KITPRODUCT" + intOldKitProductID))
                                                    {
                                                        TranslationDictionary.Add("KITPRODUCT" + intOldKitProductID, objKitProd.KitProductId.ToString());
                                                    }
                                                }

                                            }
                                            catch (Exception ex)
                                            {
                                                LogError(GetPercentResult(), "Kit Product Import, KitComponentID:" + intOldComponentID + " , Name:" + objComponent.Name + " , KitProductId:" + intOldKitProductID + " , " + ex.Message + "\n" + ex.StackTrace);
                                            }
                                        }//next nodKitProd                                
                                        objComponent.KitProducts.Save();

                                    }
                                    catch (Exception ex)
                                    {
                                        LogError(GetPercentResult(), "Kit Component Import, KitComponentID:" + intOldComponentID + " , Name:" + objComponent.Name + " , " + ex.Message + "\n" + ex.StackTrace);
                                    }
                                }//next nodKitComponent

                                //objKit.Save();
                            }
                            //objKitProd = null;
                            //objComponent = null;
                            //objKit = null;
                        }// end if
                        //response.write(" .")
					
                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "Kit Import, KitID:" + intOldProductID +  ex.Message + "\n" + ex.StackTrace);
                    }

                } //next objNode
                LogStatus(GetPercentResult(), "Kits Import Complete...");

            }
            else
            {
                currentPhasePercent = currentPhaseWeightage;
                //LogStatus(GetPercentResult(), "No Wishlists Available to Import...");
            }
        }

        #endregion

        #region Gateways - Gateway Transaction

        private void ImportGatewayTransactions(XmlNode nodSourceStore)
        {/*
                //if InStr(lstCopyOpts, ",GATEWAYS,") > 0 then
        //	'payment gateways imported, so import transactions
            //response.write("Checking Gateway Transactions...")
			
			
            XmlNodeList nlsItems = nodSourceStore.SelectNodes("Transactions/Transaction")
            if( nlsItems.Count > 0){
                //response.write("Importing " & nlsItems.Count & " Gateway Transactions .")
				
                Dim objTransaction as cbTransaction
                Dim dtTransaction As DateTime
                foreach(XmlNode objNode in nlsItems){
                    intOldGatewayID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "PayGateway_ID"))
                    if (TranslationDictionary.ContainsKey("PAYGATEWAY" & intOldGatewayID) then
                    //	'only import transactions that have gateways defined for the destination store
                        intNewGatewayID = objTranslate.Item("PAYGATEWAY" & intOldGatewayID)
                        objTransaction = new cbTransaction()
                        objTransaction.PayGateway_ID = intNewGatewayID
                        intOldTransactionID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID"),0)
                        objTransaction.StoreId = Token.Instance.StoreId
                        intOldUserID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "User_ID"))
                        if (TranslationDictionary.ContainsKey("USER" & intOldUserID) then
                            objTransaction.User_ID = objTranslate("USER" & intOldUserID)
                        else
                            objTransaction.User_ID = 0
                        end if
                        objTransaction.RemoteAddr = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "RemoteAddr"))
                        objTransaction.HTTPReferer = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "HTTPReferer"))
                        objTransaction.AccountNumber = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "AccountNumber"))
                        objTransaction.Amount = AlwaysConvert.ToDecimal(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Amount")))
                        objTransaction.TransactionDT = AlwaysConvert.ToDateTime(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "TransactionDT")))
                        objTransaction.Status = AlwaysConvert.ToInt(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Status")))
                        objTransaction.ErrorCode = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "ErrorCode"))
                        objTransaction.ErrorMsg = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "ErrorMsg"))
                        objTransaction.AuthCode = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "AuthCode"))
                        objTransaction.AVSCode = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "AVSCode"))
                        objTransaction.GWTxID = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "GWTxID"))
                    //	'force preserved id
                        if bPreserveIDs then
                        //	'delete Transaction
                            Token.Instance.StoreGroupDB.QueryNoRS("DELETE FROM TRANSACTIONS WHERE Transaction_ID=" & intOldTransactionID)
                        //	'force the next id returned from maxidlookup
                            Token.Instance.StoreGroupDB.QueryNoRS("UPDATE MAX_ID_LOOKUP SET Max_ID=" & (intOldTransactionID-1) & " WHERE TableName=" & FCast.FSQLString("TRANSACTIONS", intDBType))
                        end if
                    //	'save
                        objTransaction.Save()
                    //	'FORCE TRANSACTIONDT
                        dtTransaction = AlwaysConvert.ToDateTime(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "TransactionDT")))
                        If (dtTransaction > System.DateTime.MinValue) Then
                            Token.Instance.StoreGroupDB.QueryNoRS("UPDATE TRANSACTIONS SET TransactionDT=" & FCast.FSQLDate(dtTransaction, intDBType) & " WHERE Transaction_ID=" & objTransaction.Transaction_ID)
                        End If
                        objTranslate.Add("TRANSACTION" & intOldTransactionID, objTransaction.Transaction_ID)
                    //	'verify preserved ids
                        If bPreserveIDs AndAlso (intOldTransactionID <> objTransaction.Transaction_ID) Then
                        //	'force of ID was not successful
                            sWarnings.Append("Warning: Transaction " & intOldTransactionID & " did not have the ID successfully preserved.  The new ID is " & objTransaction.Transaction_ID & ".")
                            //response.write(" !")
                        End If
                        objTransaction = null
                    end if
                    //response.write(" .")
					
                } //next objNode
                //response.write("")
				
            else
                Status(GetPercentResult(), "No Gateway Transactiouns Available to Import...")
				
            end if
        else
            //response.write("Skipping Gateway Transaction Import...")
			
        end if
      * */
        }

        #endregion


        private void ImportWishlists(XmlNode nodSourceStore)
        {
            //if InStr(lstCopyOpts, ",WISHLIST,") > 0 then
            //response.write("Checking Wishlist...")
            currentPhasePercent = 0;
            //LogStatus(GetPercentResult(), "Checking Wishlists...");

            XmlNodeList nlsItems = nodSourceStore.SelectNodes("Wishlists/Wishlist");
            if (nlsItems.Count > 0)
            {
                int i = 0; // counter variable
                LogStatus(GetPercentResult(), "Importing " + nlsItems.Count + " Wishlists.");

                Wishlist objWishlist = null;
                bool bSavedWishlist = false;
                int intOldUserID = 0;
                int intOldWishlistID = 0;
                int newUserID = 0;
                foreach (XmlNode objNode in nlsItems)
                {
                    i++;
                    currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
                    LogStatus(GetPercentResult(), "Importing for Wishlist " + i + " of " + nlsItems.Count);

                    try
                    {
                        //	'translate user id
                        intOldUserID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "User_ID", "0"), 0);
                        newUserID = AlwaysConvert.ToInt(TranslationDictionary["USER" + intOldUserID]);
                        if (!(newUserID.Equals(0)))
                        {
                            objWishlist = new Wishlist();

                            intOldWishlistID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID", "0"), 0);
                            //Comments :objWishlist.StoreId does not exist
                            //objWishlist.StoreId = Token.Instance.StoreId;
                            objWishlist.UserId = newUserID;
                            objWishlist.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Name", "Default"));
                            objWishlist.ViewPassword = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "ViewPassword", ""));
                            //	'save
                            bSavedWishlist = !(objWishlist.SaveForUser(newUserID).Equals(SaveResult.Failed));
                            //response.write(IIf(bSavedWishlist, " o", " !"))
                            if (bSavedWishlist)
                            {
                                //	record translation
                                TranslationDictionary.Add("WISHLIST" + intOldWishlistID, objWishlist.WishlistId.ToString());
                                //	add in products to wishlist
                                XmlNodeList nlsWishlistProducts;
                                //XmlNode nodWishlistProduct ;
                                //TODO : I am confused with this

                                WishlistItem objWishlistProduct;
                                String lstKitIDs;
                                nlsWishlistProducts = objNode.SelectNodes("WishlistProducts/WishlistProduct");
                                foreach (XmlNode nodWishlistProduct in nlsWishlistProducts)
                                {
                                    //	'translate Product id
                                    int intOldProductID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodWishlistProduct, "Product_ID", ""));
                                    int newProductID = AlwaysConvert.ToInt(TranslationDictionary["PRODUCT" + intOldProductID]);
                                    if (!(newProductID.Equals(0)))
                                    {
                                        int intOldWishlistProductID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodWishlistProduct, "ID", "0"), 0);                                        
                                        //objWishlistProduct = new cbWishlistProduct()
                                        objWishlistProduct = new WishlistItem();
                                        //objWishlistProduct.Product_ID = intNewProductID
                                        objWishlistProduct.ProductId = newProductID;
                                        
                                        //objWishlistProduct.DesiredQuantity = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodWishlistProduct, "DesiredQuantity"))
                                        objWishlistProduct.Desired = AlwaysConvert.ToInt16(XmlUtility.GetElementValue(nodWishlistProduct, "DesiredQuantity", ""));
                                        //12) ac55 purchasedquantity = ac6 �WishlistItem.Recieved"
                                        objWishlistProduct.Received = AlwaysConvert.ToInt16(XmlUtility.GetElementValue(nodWishlistProduct, "PurchasedQuantity", ""));
                                        objWishlistProduct.LineMessage = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodWishlistProduct, "LineMessage", ""));
                                        objWishlistProduct.WishlistId = objWishlist.WishlistId;
                                        //	add to wishlist
                                        objWishlistProduct.Save();
                                        lstKitIDs = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodWishlistProduct, "KitIDs", ""));
                                        String newKitList = ConvertKitList(lstKitIDs, String.Empty);
                                        if(!String.IsNullOrEmpty(newKitList)) objWishlistProduct.KitList = newKitList;

                                        String lstOptionIDs = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodWishlistProduct, "OptionIDs", ""));
                                        String newOptionList = ConvertOptionList(lstOptionIDs, String.Empty);
                                        if (!String.IsNullOrEmpty(newOptionList)) objWishlistProduct.OptionList = newOptionList;
                                        
                                        objWishlist.Items.Add(objWishlistProduct);
                                        objWishlist.Items.Save();
                                        objWishlistProduct = null;
                                    }
                                }//next nodWishlistProduct
                            }
                            objWishlist.SaveForUser(newUserID);
                            objWishlist = null;
                        }
                    }
                    catch
                    {
                        // AS WISHLIST IMPORT NOT SO IMPORTANT, SO LOG IT JUST AS A WARNING MESSAGE
                        LogWarning(GetPercentResult(), "Wishlist Import, WishlistID:" + intOldWishlistID + " , Name: " + objWishlist.Name + ", wishlist not importable.");
                    }

                } //next objNode
                LogStatus(GetPercentResult(), "Wishlist Import Complete...");

            }
            else
            {
                currentPhasePercent = currentPhaseWeightage;
                //LogStatus(GetPercentResult(), "No Wishlists Available to Import...");
            }

        }

        private void ImportOrders(XmlNode nodSourceStore)
        {
            //if InStr(lstCopyOpts, ",ORDERS,") > 0 then        
            currentPhasePercent = 0;
            //LogStatus(GetPercentResult(), "Checking Orders...");

            int maxOrderNumber = StoreDataSource.GetMaxOrderNumber();

            XmlNodeList nlsOrders = nodSourceStore.SelectNodes("Orders/Order");
            if (nlsOrders.Count > 0)
            {
                // TEMPORARILY DISABLE PRODUCT INVENTORY ( SO THAT WHILE SAVING ORDERS INVENTORY NOT DEDUCTED)
                bool inventoryEnabled = Token.Instance.Store.EnableInventory;
                if (inventoryEnabled) Token.Instance.Store.EnableInventory = false;


                int i = 0; // counter variable
                //LogStatus(GetPercentResult(), "Checking " + nlsOrders.Count + " Orders.");

                foreach (XmlNode nodOrder in nlsOrders)
                {
                    i++;
                    currentPhasePercent = CalculatePartialPercentage(i, nlsOrders.Count);
                    LogStatus(GetPercentResult(), "Importing Order " + i + " of " + nlsOrders.Count);
                    Order objOrder = null;
                    int intOldOrderID = 0;
                    int intOldOrderNumber = 0;
                    try
                    {

                        
                        intOldOrderID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodOrder, "ID", ""));

                        
                        DateTime dtOrderDate;
                        //ATTEMPT TO USE THE OLD ORDER NUMBER AS THE NEW ORDER ID
                        intOldOrderNumber = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrder, "OrderNumber", "0"));
                        int newOrderId = intOldOrderID;
                        int newOrderNumber = intOldOrderNumber;
                        ////MAKE SURE NEW ORDER ID IS GREATER THE MAXIMUM IN USE
                        //if (newOrderId <= maxOrderId)
                        //{
                        //    newOrderId = StoreDataSource.GetNextOrderId(true);
                        //}                        

                        ////TODO:if Order is not saved so this number will be skiped.
                        //bool bOrderExists = (!OrderDataSource.GetOrderIdByOrderNumber(intOrderNumber).Equals(0));
                        //if (bOrderExists)
                        //{
                        //    //select case intOrderHandling
                        //    //    case 0: 'import and renumber
                        //    //int intNewOrderNumber = Token.Instance.Store.OrderNumber;


                        //    sWarnings.Append("Note: Duplicate order " + intOrderNumber + " renumbered to " + intNewOrderNumber + "");
                        //    intOrderNumber = intNewOrderNumber;
                        //    //    case 2://	'import and don't renumber
                        //    //        if not objTranslate.ContainsKey("ORDERNUMBER" & intOrderNumber) then
                        //    //            objTranslate.Add("ORDERNUMBER" & intOrderNumber, CStr(intOrderNumber))
                        //    //        end if
                        //    //        sWarnings.Append("Note: Duplicate order " & intOrderNumber & " imported.")
                        //    //    case else: 'do not import
                        //    //        bImportOrder = false
                        //    //end select
                        //}

                        
                        //strSQL = new Text.StringBuilder()
                        //strSQL.Append("INSERT INTO ORDERS(Order_ID,Store_ID,OrderNumber,User_ID,Affiliate_ID,Address_ID,FirstName,LastName,Company,Address1,Address2,City,Province,PostalCode,CountryCode,Phone,Fax,Email,Residence,OrderDT,Cancelled,Archived,TotalCharges,TotalPayments,OrderStatus_ID,Exported) VALUES(")
                        //strSQL.Append(newOrderId & "," & Token.Instance.StoreId & ",")
                        objOrder = new Order();
                        //objOrder.OrderId = newOrderId;
                        objOrder.StoreId = Token.Instance.StoreId;

                        //	translate user id
                        int newUserID = 0;
                        int intOldUserID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrder, "User_ID", ""), 0);
                        if (TranslationDictionary.ContainsKey("USER" + intOldUserID))
                        {
                            newUserID = AlwaysConvert.ToInt(TranslationDictionary["USER" + intOldUserID]);
                        }
                        else if (intOldUserID > 0)
                        {
                            //TODO: should we import the orders for users which are not imported
                            // Although this case is never going to happen but for users who had already created an account in new store and already member of old account
                            newUserID = 0;
                            LogError(GetPercentResult(), "Order Import, Unable to associate Order to User because User not found. OrderID:" + intOldOrderID + ", OrderNumber:" + intOldOrderNumber + ", UserID:" + intOldUserID);                                
                        }

                        //strSQL.Append(intNewUserID & ",")
                        objOrder.UserId = newUserID;

                        //	translate affiliate id
                        int intOldAffiliateID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrder, "Affiliate_ID", ""), 0);
                        int newAffiliateID = 0;
                        if (TranslationDictionary.ContainsKey("AFFILIATE" + intOldAffiliateID))
                        {
                            newAffiliateID = AlwaysConvert.ToInt(TranslationDictionary["AFFILIATE" + intOldAffiliateID]);
                        }
                        else if (intOldAffiliateID > 0)
                        {
                            LogError(GetPercentResult(), "Order Import, Unable to associate Order to Affiliate because Affiliate not found. OrderID:" + intOldOrderID + ", OrderNumber:" + intOldOrderNumber + ", AffiliateID:" + intOldAffiliateID);
                        }

                        //strSQL.Append(newAffiliateID & ",")
                        objOrder.AffiliateId = newAffiliateID;

                        //	translate address id
                        int intOldAddressID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrder, "Address_ID", ""), 0);
                        int newAddressID1 = 0;
                        if (TranslationDictionary.ContainsKey("ADDRESS" + intOldAddressID))
                        {
                            newAddressID1 = AlwaysConvert.ToInt(TranslationDictionary["ADDRESS" + intOldAddressID]);
                        }
                        //strSQL.Append(newAddressID1 & ",")
                        //Comments: No addressid field for order object
                        //objOrder.AddressId = newAddressID1 ;


                        //	next line is first name, last name, company
                        //strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "FirstName","")), intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "LastName","")), intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "Company","")), intDBType) & ",")
                        objOrder.BillToFirstName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "FirstName", ""));
                        objOrder.BillToLastName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "LastName", ""));
                        objOrder.BillToCompany = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "Company", ""));

                        //	next line is address1, adderss2, city
                        //strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "Address1","")), intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "Address2","")), intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "City","")), intDBType) & ",")
                        objOrder.BillToAddress1 = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "Address1", ""));
                        objOrder.BillToAddress2 = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "Address2", ""));
                        objOrder.BillToCity = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "City", ""));
                        //	'next line is province, postalcode, countrycode
                        //strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "Province","")), intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "PostalCode","")), intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "CountryCode","")), intDBType) & ",")
                        objOrder.BillToProvince = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "Province", ""));
                        objOrder.BillToPostalCode = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "PostalCode", ""));
                        objOrder.BillToCountryCode = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "CountryCode", ""));

                        //	'next line is phone, fax, email
                        //strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "Phone","")), intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "Fax","")), intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "Email","")), intDBType) & ",")

                        objOrder.BillToPhone = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "Phone", ""));
                        objOrder.BillToFax = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "Fax", ""));
                        objOrder.BillToEmail = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "Email", ""));

                        //next line is residence, OrderDT
                        //strSQL.Append(AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrder, "Residence", "1")) + "," + FCast.FSQLDate(dtOrderDate, intDBType) & ",")
                        string strOrderDate = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrder, "OrderDT", ""));
                        dtOrderDate = ConvertDateTime(strOrderDate, "Order date");
                        objOrder.OrderDate = dtOrderDate;
                                                
                        //Comment:
                        /*
                         *  Ignore residence field. It was added to support UPS, who has 
                            since developed software to automatically determine this value.
                            They now ignore the field, so we have no need to collect it.
                         */
                        //objOrder.Residence = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrder, "Residence", "1");

                        //	next line is cancelled, archived, TotalCharges, TotalPayments
                        //strSQL.Append(NullZeroInt(AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrder, "Cancelled",""))) & "," & NullZeroInt(AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrder, "Archived",""))) & "," & AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOrder, "TotalCharges",""), intDBType) & "," & AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOrder, "TotalPayments",""), intDBType) & ",")
                        //TODO: 
                        //Comments:
                        /*
                         * For now we can skip importing of cancelled orders. For sake of completeness,
                           we might need to revisit this issue at a later time.
                         */
                        //objOrder.cancelled = NullZeroInt(AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrder, "Cancelled","")))
                        //Comments:
                        //No Longer used
                        //objOrder.archived = NullZeroInt(AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrder, "Archived",""))) ;
                        objOrder.TotalCharges = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOrder, "TotalCharges", ""));
                        objOrder.TotalPayments = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOrder, "TotalPayments", ""));

                        //	translate orderstatus ID
                        int intOldOrderStatusID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrder, "OrderStatus_ID", ""), 0);
                        int newOrderStatusID = 0;
                        if (TranslationDictionary.ContainsKey("ORDERSTATUS" + intOldOrderStatusID))
                        {
                            newOrderStatusID = AlwaysConvert.ToInt(TranslationDictionary["ORDERSTATUS" + intOldOrderStatusID]);
                        }
                        else
                        {
                            // ASSIGN THE DEFAULT ORDER STATUS
                            if (!TranslationDictionary.ContainsKey("ORDERSTATUS" + 0))
                            {
                                // IF DEFAULT ORDER STATUS DOES NOT EXISTS
                                // CREATE A NEW ORDER STATUS                                    
                                OrderStatus orderStatus = null;
                                OrderStatusCollection orderStatuses = OrderStatusDataSource.LoadForCriteria("DisplayName = 'AC5xUnknown'");
                                if (orderStatuses != null && orderStatuses.Count > 0) orderStatus = orderStatuses[0];
                                if (orderStatus == null)
                                {
                                    LogWarning(GetPercentResult(), "Unable to parse Order status, setting 'AC5xUnknown' for OrderID:" + intOldOrderID + ", OrderNumber:" + intOldOrderNumber + ", Old OrderStatus ID:" + intOldOrderStatusID);

                                    orderStatus = new OrderStatus();
                                    orderStatus.Name = "AC5xUnknown";
                                    orderStatus.DisplayName = "AC5xUnknown";
                                    orderStatus.IsActive = true;
                                    orderStatus.IsValid = true;                                        
                                    orderStatus.Save();
                                }
                                // ADD TO TRANSLATION DICTIONARY FOR FUTURE USE
                                TranslationDictionary.Add("ORDERSTATUS" + 0, orderStatus.OrderStatusId.ToString());
                            }
                            newOrderStatusID = AlwaysConvert.ToInt(TranslationDictionary["ORDERSTATUS" + 0]);
                        }

                        //strSQL.Append(newOrderStatusID + ",");
                        objOrder.OrderStatusId = newOrderStatusID;

                        //	next line is Exported
                        //strSQL.Append(AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrder, "Exported","")) & ")")
                        objOrder.Exported = ToBool(XmlUtility.GetElementValue(nodOrder, "Exported", ""));

                        //SaveResult saveResult = objOrder.Save(false); // DONT RECALCULATE PAYMENT AND SHIPMENT STATUSES
                        //intResult = Token.Instance.StoreGroupDB.QueryNoRS(strSQL.ToString())
                        //strSQL = null

                        // PRESERVE ID's FOR OBJECT
                        String messages = String.Empty;
                        // FOR ORDERS WE NEED TO PRESERVE ORDER ID & NUMBER 
                        objOrder.OrderId = intOldOrderID;
                        objOrder.OrderNumber = intOldOrderNumber;
                        SaveResult saveResult = ACDataSource.SaveOrder(objOrder, this.PreserveIdOption, ref messages);
                        if (messages.Length > 0) LogWarning(GetPercentResult(), messages);

                        // SKIP IMPORTING MORE DETAILS IF FAILED SAVING OBJECT
                        if (saveResult == SaveResult.Failed)
                        {                                
                            continue;
                        }
                        else
                        {

                            //	'order successfully copied, now copy associated data
                            newOrderId = objOrder.OrderId;
                            newOrderNumber = objOrder.OrderNumber;
                            TranslationDictionary.Add("ORDER" + intOldOrderID, newOrderId.ToString());
                            if (maxOrderNumber < newOrderNumber) maxOrderNumber = newOrderNumber;

                            XmlNodeList nlsShipments;
                            //XmlNode nodShipment;

                            int intDefaultOldShipmentID = 0;
                            int defaultNewShipmentID = 0;

                            nlsShipments = nodOrder.SelectNodes("Shipments/Shipment");
                            if (nlsShipments.Count > 0)
                            {

                                foreach (XmlNode nodShipment in nlsShipments)
                                {
                                    int intOldShipmentID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodShipment, "ID", ""), 0);

                                    try{
                                        if (intDefaultOldShipmentID == 0)
                                        {
                                            intDefaultOldShipmentID = intOldShipmentID;
                                        }
                                        //newShipmentID = Token.Instance.StoreGroupDB.GetMaxID("SHIPMENTS")
                                        int newShipmentID = 0;

                                        //strSQL = new Text.StringBuilder()                                    
                                        //strSQL.Append("INSERT INTO SHIPMENTS(Shipment_ID,Order_ID,ShipGateway_ID,ShipMethodCode,ShipMethodName,ShipDT,ShipMsg,Warehouse_ID,Address_ID,FirstName,LastName,Company,Address1,Address2,City,Province,PostalCode,CountryCode,Phone,Fax,Email,Residence) VALUES(")
                                        OrderShipment objOrderShipment = new OrderShipment();

                                        //strSQL.Append(newShipmentID & "," & newOrderId & "," & AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodShipment, "ShipGateway_ID","")) & ",")
                                        objOrderShipment.OrderId = newOrderId;

                                        //Comments: Ignore this data.: objOrderShipment.gatewayid = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodShipment, "ShipGateway_ID",""))


                                        //	next line is shipmethodcode, shipmethodname, shipdt, shipmsg
                                        //strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "ShipMethodCode","")), intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "ShipMethodName","")), intDBType) & "," & FCast.FSQLDate(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "ShipDT","")), intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "ShipMsg","")), intDBType) & ",")

                                        //TODO: shipmethod id need to be parsed from code
                                        //objOrderShipment.ShipMethodId = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "ShipMethodCode","");

                                        objOrderShipment.ShipMethodName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "ShipMethodName", ""));

                                        // IF SHIP DATE IS PROVIDED THEN IMPORT IT
                                        String strShipDate = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "ShipDT", ""));
                                        DateTime tempDate = ConvertDateTime(strShipDate, "Ship date");
                                        if (tempDate.Ticks >= objOrder.OrderDate.Ticks) objOrderShipment.ShipDate = tempDate;
                                       

                                        objOrderShipment.ShipMessage = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "ShipMsg", ""));

                                        //	translate warehouse id
                                        int intOldWarehouseID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodShipment, "Warehouse_ID", ""));
                                        int newWarehouseID = Token.Instance.Store.DefaultWarehouseId;
                                        if (TranslationDictionary.ContainsKey("WAREHOUSE" + intOldWarehouseID))
                                        {
                                            newWarehouseID = AlwaysConvert.ToInt(TranslationDictionary["WAREHOUSE" + intOldWarehouseID]);
                                        }
                                        else if(intOldWarehouseID > 0)
                                        {
                                            LogError(GetPercentResult(), "Order Shipment Import, Unable to associate Shipment to Warehouse because Warehouse not found, assigning Default Warehouse ID. OrderShipmentID:" + intOldShipmentID + ", OrderID:" + intOldOrderID + ", OrderNumber:" + intOldOrderNumber + ", WarehouseID:" + intOldWarehouseID); 
                                        }//end if

                                        //strSQL.Append(newWarehouseID & ",")
                                        objOrderShipment.WarehouseId = newWarehouseID;

                                        //	translate address id
                                        intOldAddressID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodShipment, "Address_ID", ""));

                                        if (TranslationDictionary.ContainsKey("ADDRESS" + intOldAddressID))
                                        {
                                            newAddressID1 = AlwaysConvert.ToInt(TranslationDictionary["ADDRESS" + intOldAddressID]);
                                        }
                                        else
                                        {
                                            newAddressID1 = 0;
                                        }

                                        //strSQL.Append(newAddressID1 & ",") 
                                        //no longer used: objOrderShipment.ShipAddressId = newAddressID1; does not exist                                        


                                        //	next line is first name, last name, company
                                        //strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "FirstName","")), intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "LastName","")), intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "Company","")), intDBType) & ",")
                                        objOrderShipment.ShipToFirstName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "FirstName", ""));
                                        objOrderShipment.ShipToLastName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "LastName", ""));
                                        objOrderShipment.ShipToCompany = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "Company", ""));

                                        //	next line is address1, adderss2, city
                                        //strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "Address1","")), intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "Address2","")), intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "City","")), intDBType) & ",")
                                        objOrderShipment.ShipToAddress1 = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "Address1", ""));
                                        objOrderShipment.ShipToAddress2 = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "Address2", ""));
                                        objOrderShipment.ShipToCity = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "City", ""));

                                        //	next line is province, postalcode, countrycode
                                        //strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "Province","")), intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "PostalCode","")), intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "CountryCode","")), intDBType) & ",")
                                        objOrderShipment.ShipToProvince = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "Province", ""));
                                        objOrderShipment.ShipToPostalCode = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "PostalCode", ""));
                                        objOrderShipment.ShipToCountryCode = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "CountryCode", ""));

                                        //	next line is phone, fax, email, residence
                                        //strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "Phone","")), intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "Fax","")), intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "Email","")), intDBType) & "," & AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodShipment, "Residence", "1")) & ")")
                                        objOrderShipment.ShipToPhone = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "Phone", ""));
                                        objOrderShipment.ShipToFax = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "Fax", ""));
                                        objOrderShipment.ShipToEmail = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodShipment, "Email", ""));
                                        objOrderShipment.ShipToResidence = ToBool(XmlUtility.GetElementValue(nodShipment, "Residence", "1"));

                                        saveResult = objOrderShipment.Save();
                                        //intResult = Token.Instance.StoreGroupDB.QueryNoRS(strSQL.ToString())
                                        if (!saveResult.Equals(SaveResult.Failed))
                                        {
                                            //	shipment copied
                                            //objTranslate.Add("SHIPMENT" + intOldShipmentID, newShipmentID.ToString());
                                            TranslationDictionary.Add("SHIPMENT" + intOldShipmentID, objOrderShipment.OrderShipmentId.ToString());
                                            if (defaultNewShipmentID.Equals(0))
                                            {
                                                defaultNewShipmentID = newShipmentID;
                                            }
                                        }
                                    }catch(Exception ex){
                                        LogError(GetPercentResult(), "Order Shipment Import, ShipmentID:" + intOldShipmentID + " , OrderID: " + intOldOrderID + " , OrderNumber: " + intOldOrderNumber + ", " + ex.Message + "\n" + ex.StackTrace);
                                    }
                                }//next nodShipment
                            }

                            XmlNodeList nlsOrderItems;
                            //XmlNode nodOrderItem;
                            Boolean blnShippable;
                            

                            nlsOrderItems = nodOrder.SelectNodes("OrderItems/OrderItem");
                            if (nlsOrderItems.Count > 0)
                            {
                                foreach (XmlNode nodOrderItem in nlsOrderItems)
                                {
                                    int intOldOrderItemID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodOrderItem, "ID", ""), 0);
                                    try{
                                       
                                        
                                        int newOrderItemID = 0;//Token.Instance.StoreGroupDB.GetMaxID("ORDERITEMS")
                                        OrderItem objOrderItem = new OrderItem();
                                        

                                        //strSQL = new Text.StringBuilder();
                                        //strSQL.Append("INSERT INTO ORDERITEMS(OrderItem_ID,Shipment_ID,Order_ID,Product_ID,Wishlist_ID,Warehouse_ID,Parent_ID,KitProduct_ID,Name,SKU,Quantity,UnitPrice,UnitWeight,UnitPriceAdj,UnitWeightAdj,Length,Width,Height,OptionIDs,OptionNames,KitIDs,KitNames,ExtPrice,ExtWeight,ExtPriceAdj,ExtWeightAdj,Shippable,TaxCode_ID,LineMessage,StockAdj,WrapStyle_ID,GiftMessage,OrderBy) VALUES(")

                                        //strSQL.Append(newOrderItemID + ",")                                    

                                        objOrderItem.OrderId = newOrderId;

                                        //translate shipment id
                                        int intOldShipmentID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrderItem, "Shipment_ID", intDefaultOldShipmentID.ToString()));
                                        int newShipmentID = 0;
                                        if (TranslationDictionary.ContainsKey("SHIPMENT" + intOldShipmentID))
                                        {
                                            newShipmentID = AlwaysConvert.ToInt(TranslationDictionary["SHIPMENT" + intOldShipmentID]);
                                        }
                                        else
                                        {
                                            newShipmentID = defaultNewShipmentID;                                                
                                        }
                                        //strSQL.Append(newShipmentID + "," & newOrderId + ",");
                                        objOrderItem.OrderShipmentId = newShipmentID;

                                        //translate Product id
                                        int intOldProductID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrderItem, "Product_ID", ""));
                                        int newProductID = 0;
                                        if (intOldProductID > 0)
                                        {
                                            if (TranslationDictionary.ContainsKey("PRODUCT" + intOldProductID))
                                            {
                                                newProductID = AlwaysConvert.ToInt(TranslationDictionary["PRODUCT" + intOldProductID]);
                                            }
                                        }

                                        //strSQL.Append(newProductID & ",")
                                        objOrderItem.ProductId = newProductID;

                                        //	translate Wishlist_ID
                                        //TODO: how to translate wishlists to wishlist items
                                        //int intOldWishlistID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrderItem, "Wishlist_ID",""));
                                        //int newWishlistID = 0;
                                        //if(objTranslate.ContainsKey("WISHLIST" + intOldWishlistID)){
                                        //    newWishlistID = objTranslate("WISHLIST" + intOldWishlistID);
                                        //}
                                        //strSQL.Append(newWishlistID & ",")
                                        //objOrderItem.WishlistItemId = newWishlistID;

                                        //	translate warehouse id
                                        blnShippable = ToBool(XmlUtility.GetElementValue(nodOrderItem, "Shippable", "1"));
                                        int intOldWarehouseID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrderItem, "Warehouse_ID", ""));
                                        int newWarehouseID = 0;
                                        if (TranslationDictionary.ContainsKey("WAREHOUSE" + intOldWarehouseID))
                                        {
                                            newWarehouseID = AlwaysConvert.ToInt(TranslationDictionary["WAREHOUSE" + intOldWarehouseID]);
                                        }
                                        else if (blnShippable)
                                        {
                                            //	if order item is shippable, set warehouse to default
                                            //	otherwise set to 0                                        
                                            newWarehouseID = Token.Instance.Store.DefaultWarehouseId;
                                            if (intOldWarehouseID > 0) LogError(GetPercentResult(), "Order Item Import, Unable to associate Order Item to Warehouse because Warehouse not found, assigning Default Warehouse ID. OrderItemID:" + intOldOrderItemID + ", OrderID:" + intOldOrderID + ", OrderNumber:" + intOldOrderNumber + ", WarehouseID:" + intOldWarehouseID); 
                                        }
                                        //strSQL.Append(newWarehouseID & ",")
                                        if (objOrderItem.OrderShipmentId > 0)
                                        {
                                            //IF ORDERITEM HAS A SHIPMENT
                                            OrderShipment objOrderShipment = OrderShipmentDataSource.Load(objOrderItem.OrderShipmentId);
                                            objOrderShipment.WarehouseId = newWarehouseID;
                                            objOrderShipment.Save();
                                        }


                                        //	translate PARENT (orderitem) id
                                        int intOldParentID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrderItem, "Parent_ID", ""));
                                        int newParentID = 0;
                                        if (TranslationDictionary.ContainsKey("ORDERITEM" + intOldParentID))
                                        {
                                            newParentID = AlwaysConvert.ToInt(TranslationDictionary["ORDERITEM" + intOldParentID], 0);
                                        }
                                        else
                                        {
                                            newParentID = 0;
                                        }
                                        //strSQL.Append(newParentID + ",");                                    
                                        objOrderItem.ParentItemId = newParentID;

                                        ////TODO:	translate KitProduct id
                                        //int intOldKitProductID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrderItem, "KitProduct_ID",""));
                                        //int newKitProductID = 0;
                                        //if (objTranslate.ContainsKey("PRODUCT" + intOldKitProductID)){
                                        //    newKitProductID = AlwaysConvert.ToInt(objTranslate("PRODUCT" + intOldKitProductID));
                                        //}
                                        ////strSQL.Append(NullZeroInt(newKitProductID) & ",")
                                        //Ignore this data.:objOrderItem.pro no kit product Id available
                                        //objOrderItem.KitProductID 


                                        //	next line is name, sku, quantity
                                        //strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrderItem, "Name","")), intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrderItem, "SKU","")), intDBType) & "," & AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrderItem, "Quantity","")) & ",")
                                        objOrderItem.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrderItem, "Name", ""));

                                        objOrderItem.Sku = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrderItem, "SKU", ""));
                                        objOrderItem.Quantity = (short)AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrderItem, "Quantity", ""));

                                        //	next line is UnitPrice,UnitWeight,UnitPriceAdj
                                        //strSQL.Append(AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOrderItem, "UnitPrice")) & "," & AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOrderItem, "UnitWeight")) & "," & NullZeroDec(AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOrderItem, "UnitPriceAdj",""))) & ",")
                                        objOrderItem.Price = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOrderItem, "UnitPrice", ""));
                                        
                                        // IF IT IS A TAX ITEM
                                        if (intOldProductID == -1)
                                        {
                                            objOrderItem.OrderItemType = OrderItemType.Tax;
                                            if(objOrderItem.Price == 0)   objOrderItem.Price = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOrderItem, "ExtPrice", ""));
                                        }

                                        // IF IT IS A SHIPPING ITEM
                                        else if (intOldProductID == -2) objOrderItem.OrderItemType = OrderItemType.Shipping;
                                        else if (intOldProductID == -3) objOrderItem.OrderItemType = OrderItemType.Coupon;
                                        else if (intOldProductID == -4) objOrderItem.OrderItemType = OrderItemType.Discount;
                                        else if (intOldProductID == -5) objOrderItem.OrderItemType = OrderItemType.GiftWrap;
                                        else if (intOldProductID == -999)
                                        {
                                            if(objOrderItem.Price >= 0)   objOrderItem.OrderItemType = OrderItemType.Charge;
                                            else objOrderItem.OrderItemType = OrderItemType.Credit;
                                        }
                                        else if (intOldProductID >= 0) objOrderItem.OrderItemType = OrderItemType.Product;
                                        
                                        objOrderItem.Weight = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOrderItem, "UnitWeight", ""));

                                        //Comments: ignore this data: not found objOrderItem.priceAdj = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOrderItem, "UnitPriceAdj",""));

                                        //	next line is UnitWeightAdj,Length,Width,Height
                                        //strSQL.Append(NullZeroDec(AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOrderItem, "UnitWeightAdj"))) & "," & NullZeroDec(AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOrderItem, "Length"))) & "," & NullZeroDec(AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOrderItem, "Width",""))) & "," & NullZeroDec(AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOrderItem, "Height",""))) & ",")
                                        //Comments: ignore this data, No longer used.: not found objOrderItem.UnitWeightAdj
                                        //Length, Width, Height: Ignore this data. It was due to sharing of records between baskets and orders. Length, Width , Height, do we need to import this data, becaue it is already contained in the product associated with this order item                                


                                        //Comments: Ignore this data.: do we need to import this, no equvalent present in code
                                        //	,OptionNames,,KitNames


                                        objOrderItem.VariantName = ConvertOptionNames(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrderItem, "OptionNames", "")));
                                        							
                                        //strSQL.Append(FCast.FSQLString(lstOptionIDs, intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrderItem, "OptionNames","")), intDBType) & "," & FCast.FSQLString(lstKitIDs, intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrderItem, "KitNames","")), intDBType) & ",")

                                        String lstKitIDs = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrderItem, "KitIDs", ""));
                                        String newKitList = ConvertKitList(lstKitIDs, String.Empty);
                                        if (!String.IsNullOrEmpty(newKitList)) objOrderItem.KitList = newKitList;

                                        String lstOptionIDs = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrderItem,"OptionIDs",""));
                                        String newOptionList = ConvertOptionList(lstOptionIDs, String.Empty);
                                        if (!String.IsNullOrEmpty(newOptionList)) objOrderItem.OptionList = newOptionList;
                                        


                                        //	next line is (21) ExtPrice,ExtWeight,ExtPriceAdj,ExtWeightAdj,Shippable
                                        //strSQL.Append(AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOrderItem, "ExtPrice","")) & "," & AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOrderItem, "ExtWeight","")) & "," & AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOrderItem, "ExtPriceAdj","")) & "," & AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOrderItem, "ExtWeightAdj","")) & "," & iif(blnShippable, 1, 0) & ",")

                                        //TODO: objOrderItem.ExtendedPrice, objOrderItem.ExtendedWeight are readonly , what to do
                                        //objOrderItem.ExtendedPrice = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOrderItem, "ExtPrice","")) ;
                                        //objOrderItem.ExtendedWeight = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOrderItem, "ExtWeight",""));
                                        //Comments:
                                        //Ignore these fields: not found objOrderItem.ExtendedWeightAdj = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOrderItem, "ExtWeightAdj","")) ;
                                        //Ignore these fields: not found objOrderItem.ExtPriceAdj = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodOrderItem, "ExtPriceAdj","")) ;
                                        //Ignore these fields: not found objOrderItem.isShipable = (blnShippable? 1, 0)


                                        //	translate TaxCode id (26)
                                        int newTaxCodeID = 0;
                                        string sOldTaxCode = XmlUtility.GetElementValue(nodOrderItem, "TaxCode", "");
                                        if (sOldTaxCode.Length > 0)
                                        {
                                            if (DicTaxCode.ContainsKey(sOldTaxCode))
                                            {
                                                newTaxCodeID = AlwaysConvert.ToInt(DicTaxCode[sOldTaxCode]);
                                            }
                                        }
                                        //strSQL.Append(newTaxCodeID & ",")
                                        objOrderItem.TaxCodeId = newTaxCodeID;

                                        //	next line is LineMessage
                                        //strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrderItem, "LineMessage","")), intDBType) & ",")
                                        objOrderItem.LineMessage = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrderItem, "LineMessage", ""));

                                        //	stockadj
                                        //strSQL.Append(AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrderItem, "StockAdj","")) & ",")
                                        //Comments:Ignore these fields : not available objOrderItem.stockadj = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrderItem, "StockAdj","")) ;

                                        //	translate WrapStyle_ID
                                        int intOldWrapStyleID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrderItem, "WrapStyle_ID", ""));
                                        int newWrapStyleID = 0;
                                        if (TranslationDictionary.ContainsKey("WRAPSTYLE" + intOldWrapStyleID))
                                        {
                                            newWrapStyleID = AlwaysConvert.ToInt(TranslationDictionary["WRAPSTYLE" + intOldWrapStyleID]);
                                        }
                                        //strSQL.Append(newWrapStyleID & ",")
                                        objOrderItem.WrapStyleId = newWrapStyleID;                                        

                                        //	wrapstyle_id, giftmessage, orderby
                                        //strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrderItem, "GiftMessage","")), intDBType) & ",")
                                        objOrderItem.GiftMessage = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrderItem, "GiftMessage", ""));

                                        //strSQL.Append(AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrderItem, "OrderBy","")) & ")")
                                        objOrderItem.OrderBy = (short)AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrderItem, "OrderBy", ""));

                                        //ADD ORDERITEM RECORD
                                        //intResult = Token.Instance.StoreGroupDB.QueryNoRS(strSQL.ToString())
                                        saveResult = objOrderItem.Save();

                                        if (!saveResult.Equals(SaveResult.Failed))
                                        {
                                            newOrderItemID = objOrderItem.OrderItemId;
                                            //	ADD TRANSLATION
                                            TranslationDictionary.Add("ORDERITEM" + intOldOrderItemID, newOrderItemID.ToString());
                                        }

                                    }catch(Exception ex){
                                        LogError(GetPercentResult(), "Order Item Import, OrerItemID:" + intOldOrderItemID + " , OrderID: " + intOldOrderID + " , OrderNumber: " + intOldOrderNumber + ", " + ex.Message + "\n" + ex.StackTrace);
                                    }
                                }
                            }

                            //	import Paymenting numbers
                            XmlNodeList nlsPayments;
                            nlsPayments = nodOrder.SelectNodes("Payments/Payment");
                            if (nlsPayments.Count > 0)
                            {
                                foreach (XmlNode nodPayment in nlsPayments)
                                {
                                    int intOldPaymentID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodPayment, "ID", ""), 0);
                                    Payment objPayment = null;
                                    try{
                                        objPayment = new Payment();

                                        //strSQL = new Text.StringBuilder()
                                        //strSQL.Append("INSERT INTO PAYMENTS(Payment_ID,Store_ID,Order_ID,Transaction_ID,PayMethod_ID,PayMethodType,PayMethodName,AccountReference,Amount,PaymentDT,Processed,PaymentStatus,PaymentStatusReason,AccountData) VALUES(")
                                        //strSQL.Append(intNewPaymentID & "," & Token.Instance.StoreId & "," & newOrderId & ",")

                                        //translate Transaction id
                                        int intOldTransactionID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodPayment, "Transaction_ID", ""));
                                        int newTransactionID = 0;
                                        if (TranslationDictionary.ContainsKey("TRANSACTION" + intOldTransactionID))
                                        {
                                            newTransactionID = AlwaysConvert.ToInt(TranslationDictionary["TRANSACTION" + intOldTransactionID]);
                                        }

                                        objPayment.OrderId = newOrderId;

                                        //strSQL.Append(newTransactionID & ",")
                                        //TODO: Comments: 9) In AC55, it was a one transaction per payment model. To import the transaction successfully, you have to make sure to get the associated details of the transaction. So the code you have given is OK, but you would need to assign additional details:
                                        //Transaction newTransaction = new Transaction(newTransactionID)
                                        //newTransaction.AuthCode = ...;
                                        //newTransaction.AVSCode = ...;
                                        //Payment.Transactions.Add(newTransaction)
                                        //I think those additional transaction details would be available in the AC55 data export.

                                        //Transaction newTransaction = new Transaction(newTransactionID);
                                        //newTransaction.AuthorizationCode = ...;
                                        //newTransaction.AVSResultCode = ...;                                
                                        //objPayment.Transactions.Add(newTransaction);

                                        //	translate PayMethod_ID
                                        int intOldPayMethodID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodPayment, "PayMethod_ID", ""));
                                        int newPayMethodID = 0;
                                        if (TranslationDictionary.ContainsKey("PAYMETHOD" + intOldPayMethodID))
                                        {
                                            newPayMethodID = AlwaysConvert.ToInt(TranslationDictionary["PAYMETHOD" + intOldPayMethodID]);
                                        }
                                        //strSQL.Append(newPayMethodID & ",")
                                        objPayment.PaymentMethodId = newPayMethodID;

                                        //	next line is paymethodtype, paymethodname, accountreference,
                                        //strSQL.Append(AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodPayment, "PayMethodType","")) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodPayment, "PayMethodName")), intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodPayment, "AccountReference")), intDBType) & ",")
                                        
                                        objPayment.PaymentMethodName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodPayment, "PayMethodName", ""));

                                        //Comments: Both are same AccountReference to ReferenceNumber
                                        objPayment.ReferenceNumber = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodPayment, "AccountReference", ""));
                                        
                                        //	make sure date is not null
                                        string sPaymentDate = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodPayment, "PaymentDT", ""));

                                        DateTime tempDate = ConvertDateTime(sPaymentDate, "Order Payment date");
                                        if (tempDate > DateTime.MinValue) objPayment.PaymentDate = tempDate;

                                        //	next line is Amount, PaymentDT
                                        //strSQL.Append(AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodPayment, "Amount","")) & "," & sPaymentDate & ",")
                                        objPayment.Amount = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodPayment, "Amount", ""));                                        

                                        //	next line is Processed, PaymentStatus
                                        //strSQL.Append(AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodPayment, "Processed","")) & "," & AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodPayment, "PaymentStatus","")) & ",")
                                        //TODO: not found objPayment.processed =  AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodPayment, "Processed",""));
                                        int oldStatus = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodPayment, "Processed", "0"));
                                        objPayment.PaymentStatus = (oldStatus == 0) ? PaymentStatus.Unprocessed : PaymentStatus.Completed;

                                        //	next line is PaymentStatusReason, AccountData
                                        //strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodPayment, "PaymentStatusReason","")), intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodPayment, "AccountData")), intDBType) & ")")
                                        objPayment.PaymentStatusReason = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodPayment, "PaymentStatusReason", ""));
                                        //TODO: need to verify objPayment.AccountData = , intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodPayment, "AccountData"));

                                        saveResult = objPayment.Save();
                                        //intResult = Token.Instance.StoreGroupDB.QueryNoRS(strSQL.ToString());
                                        if (!saveResult.Equals(SaveResult.Failed))
                                        {
                                            //	payment transferred
                                            TranslationDictionary.Add("PAYMENT" + intOldPaymentID, objPayment.PaymentId.ToString());
                                            //TODO: function need to be implemented, I THINK THIS IS NOT NEEDED,AS WE ARE ALREADY IMPORTING THE PAY METHOD AND EACH PAY METHOD HAVE ITS OWN INSTRUMENT
                                            //objPayment.PaymentMethod.PaymentInstrument = ConvertPaymentMethodType(AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodPayment, "PayMethodType", "")), HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodPayment, "PayMethodName", "")));
                                            objPayment.Save();
                                        }
                                    }catch(Exception ex){
                                        LogError(GetPercentResult(), "Order Payment Import, PaymentID:" + intOldPaymentID + " , OrderID: " + intOldOrderID + " , OrderNumber: " + intOldOrderNumber + ", " + ex.Message + "\n" + ex.StackTrace);
                                    }
                                }//next nodPayment
                            }

                            

                            XmlNodeList nlsOrderNotes;
                            //

                            nlsOrderNotes = nodOrder.SelectNodes("OrderNotes/OrderNote");
                            if (nlsOrderNotes.Count > 0)
                            {
                                foreach (XmlNode nodOrderNote in nlsOrderNotes)
                                {
                                    int intOldOrderNoteID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodOrderNote, "ID", ""), 0);

                                    try{
                                        //strSQL = new Text.StringBuilder()
                                        //strSQL.Append("INSERT INTO ORDERNOTES(OrderNote_ID,Order_ID,User_ID,NoteDT,NoteData) VALUES(")
                                        //strSQL.Append(intNewOrderNoteID & "," & newOrderId & ",")
                                        OrderNote objOrderNote = new OrderNote();
                                        objOrderNote.OrderId = newOrderId;

                                        //	translate User id
                                        intOldUserID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrderNote, "User_ID", ""));
                                        //int newUserID = 0;
                                        if (TranslationDictionary.ContainsKey("USER" + intOldUserID))
                                        {
                                            newUserID = AlwaysConvert.ToInt(TranslationDictionary["USER" + intOldUserID]);
                                        }
                                        //strSQL.Append(newUserID & ",");
                                        objOrderNote.UserId = newUserID;

                                        //	next line is NoteDT, NoteData
                                         objOrderNote.CreatedDate = ConvertDateTime(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrderNote, "NoteDT", "")), "NoteDT");
                                        

                                        objOrderNote.Comment = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrderNote, "NoteData", ""));

                                        // MARK ALL ORDER NOTES AS PRIVATE, AS FOR AC55 THERE WERE NO PUBLIC NOTES (bug 8133)
                                        objOrderNote.NoteType = NoteType.Private;

                                        saveResult = objOrderNote.Save();

                                    }catch(Exception ex){
                                        LogError(GetPercentResult(), "Order Note Import, OrerNoteID:" + intOldOrderNoteID + " , OrderID: " + intOldOrderID + " , OrderNumber: " + intOldOrderNumber + ", " + ex.Message + "\n" + ex.StackTrace);
                                    }
                                }//next nodOrderNote
                            }

                            //	import tracking numbers
                            XmlNodeList nlsTrackingItems;

                            nlsTrackingItems = nodOrder.SelectNodes("Tracking/TrackingItem");
                            if (nlsTrackingItems.Count > 0)
                            {
                                foreach (XmlNode nodTrackingItem in nlsTrackingItems)
                                {
                                    int intOldTrackingID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodTrackingItem, "ID", ""), 0);

                                    try{
                                        //strSQL = new Text.StringBuilder()
                                        //strSQL.Append("INSERT INTO TRACKING(Tracking_ID,Shipment_ID,ShipGateway_ID,CarrierName,TrackingNumber) VALUES(")
                                        //strSQL.Append(newTrackingID & ",")
                                        TrackingNumber objTrackingNumber = new TrackingNumber();


                                        // translate Shipment id
                                        int intOldShipmentID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodTrackingItem, "Shipment_ID", intDefaultOldShipmentID.ToString()));
                                        int newShipmentID = 0;
                                        if (TranslationDictionary.ContainsKey("SHIPMENT" + intOldShipmentID))
                                        {
                                            newShipmentID = AlwaysConvert.ToInt(TranslationDictionary["SHIPMENT" + intOldShipmentID]);
                                        }
                                        else
                                        {
                                            newShipmentID = defaultNewShipmentID;
                                        }
                                        //	next line is new shipment id, shipgatewayid
                                        //strSQL.Append(newShipmentID + "," + AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodTrackingItem, "ShipGateway_ID")) & ",")
                                        objTrackingNumber.OrderShipmentId = newShipmentID;
                                        //TODO: NOT importing ship gateways: objTrackingNumber.ShipGatewayId = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodTrackingItem, "ShipGateway_ID")) & ",")

                                        //	next line is carriername, trackingnumber
                                        //strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodTrackingItem, "CarrierName")), intDBType) & "," & FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodTrackingItem, "TrackingNumber")), intDBType) & ")")
                                        //objTrackingNumber.TrackingNumberData = TODO:

                                        saveResult = objTrackingNumber.Save();
                                        //	don't need to record new tracking ids
                                    }catch(Exception ex){
                                        LogError(GetPercentResult(), "Order Tracking Number Import, Old TrackingID:" + intOldTrackingID + " , OrderID: " + intOldOrderID + " , OrderNumber: " + intOldOrderNumber + ", " + ex.Message + "\n" + ex.StackTrace);
                                    }
                                }//next nodTrackingItem
                            }

                            // IMPORT AC5X ORDERFILES (AC7X ORDERITEMDIGITALGOODS)
                            XmlNodeList nlsOrderFiles;
                            nlsOrderFiles = nodOrder.SelectNodes("OrderFiles/OrderFile");
                            if (nlsOrderFiles.Count > 0)
                            {
                                foreach (XmlNode nodOrderFile in nlsOrderFiles)
                                {
                                    int intOldOrderFileID = 0;
                                    int intOldOrderItemID = 0;
                                    string digitalGoodName = string.Empty;
                                    try
                                    {
                                        // GET OLD ORDERFILEID FOR TRANSLATION DICTIONARY
                                        intOldOrderFileID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodOrderFile, "ID", ""), 0);

                                        // DETERMINE BEST DIGITAL GOOD NAME
                                        string displayName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrderFile, "DisplayName", string.Empty));
                                        string saveAsName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrderFile, "SaveAsName", string.Empty));
                                        string fileName = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrderFile, "FileName", string.Empty));
                                        digitalGoodName = displayName;
                                        if (string.IsNullOrEmpty(digitalGoodName))
                                        {
                                            digitalGoodName = saveAsName;
                                            if (string.IsNullOrEmpty(digitalGoodName))
                                            {
                                                digitalGoodName = fileName;
                                                if (string.IsNullOrEmpty(digitalGoodName))
                                                {
                                                    digitalGoodName = "AC5x Order File";
                                                }
                                            }
                                        }

                                        // DETERMINE THE ASSOCIATED ORDERITEM ID FOR THIS DIGITAL GOOD
                                        intOldOrderItemID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrderFile, "OrderItem_ID", ""));
                                        int newOrderItemID = 0;
                                        if (TranslationDictionary.ContainsKey("ORDERITEM" + intOldOrderItemID))
                                        {
                                            newOrderItemID = AlwaysConvert.ToInt(TranslationDictionary["ORDERITEM" + intOldOrderItemID]);
                                        }

                                        if (newOrderItemID == 0)
                                        {
                                            // No OrderItemID association, treat as a digital good file directly associated with order
                                            // AC7 enforces database integrity so we must create a dummy order item to enable the association
                                            OrderItem orderItem = new OrderItem();
                                            orderItem.OrderId = newOrderId;
                                            orderItem.Name = displayName;
                                            orderItem.OrderItemType = OrderItemType.Product;
                                            orderItem.Price = 0;
                                            orderItem.Shippable = Shippable.No;
                                            orderItem.Save();
                                            newOrderItemID = orderItem.OrderItemId;
                                        }

                                        //	translate file id
                                        int intOldFileID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodOrderFile, "File_ID", ""));
                                        int newFileID = 0;
                                        if (TranslationDictionary.ContainsKey("FILE" + intOldFileID))
                                        {
                                            newFileID = AlwaysConvert.ToInt(TranslationDictionary["FILE" + intOldFileID]);
                                        }

                                        String validatedt = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrderFile, "ValidateDT", ""));
                                        String fulfilldt = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodOrderFile, "FulfillDT", ""));

                                        String licenseKey = XmlUtility.GetElementValue(nodOrderFile, "LicenseKey", "");

                                        // IF IT IS A GIF CERTIFICATE
                                        if (fileName.Equals("_GIFTCERT"))
                                        {
                                            GiftCertificate giftCertificate = null;
                                            if (TranslationDictionary.ContainsKey("GIFTCERT" + intOldFileID))
                                            {
                                                giftCertificate = GiftCertificateDataSource.Load(AlwaysConvert.ToInt(TranslationDictionary["GIFTCERT" + intOldFileID]));
                                            }
                                            if (giftCertificate == null) giftCertificate = new GiftCertificate();
                                            giftCertificate.OrderItemId = newOrderItemID;
                                            giftCertificate.StoreId = Token.Instance.StoreId;
                                            giftCertificate.Name = digitalGoodName;

                                            if (!String.IsNullOrEmpty(licenseKey)) giftCertificate.SerialNumber = licenseKey;


                                            saveResult = giftCertificate.Save();

                                            if (!saveResult.Equals(SaveResult.Failed))
                                            {
                                                //	add translation
                                                TranslationDictionary.Add("ORDERFILE" + intOldOrderFileID, giftCertificate.GiftCertificateId.ToString());
                                                // TRANSACTIONS
                                                // ACTIVATEION
                                                if (!String.IsNullOrEmpty(validatedt))
                                                {
                                                    try
                                                    {
                                                        DateTime dt = ConvertDateTime(validatedt, "Gift certificate validatedt");
                                                        if (dt.Ticks >= objOrder.OrderDate.Ticks)
                                                        {
                                                            GiftCertificateTransaction trans = new GiftCertificateTransaction();
                                                            trans.Description = "Gift certificate activated.";
                                                            trans.TransactionDate = dt;
                                                            trans.GiftCertificateId = giftCertificate.GiftCertificateId;
                                                            giftCertificate.Transactions.Add(trans);
                                                            giftCertificate.Transactions.Save();
                                                        }
                                                    }
                                                    catch (Exception exception) { LogError(GetPercentResult(), "Unable to add giftcertificate validate transaction for  validate date:" + validatedt + " Error:" + exception.Message + "\n" + exception.GetBaseException().StackTrace); }
                                                }
                                                else
                                                {
                                                    // BUG # 8234, IMPORT THE GIFT CERTIFICATE BALANCE AS ZERO AND LOG AN *ERROR* IN THE SCRIPT.
                                                    String message = "Gift certificate import need to be reviewed, as gift certificate is not validated";
                                                    if (String.IsNullOrEmpty(fulfilldt)) message += " and fulfilled";
                                                    message += ", so, its balance can not be imported. , OrderId:" + objOrder.OrderId + ", Gift Certificate Name:" + displayName + ", Gift certificate Id:" + giftCertificate.GiftCertificateId + ", OrderFile Id:" + intOldOrderFileID + ", Old OrderId:" + intOldOrderID + ", Old Order Number:" + intOldOrderNumber;
                                                    LogError(GetPercentResult(), message);
                                                }
                                            }
                                            else
                                            {
                                                LogError(GetPercentResult(), "Order Files import, Giftcertificate import failed(unable to save) , OrderId:" + objOrder.OrderId + ", OrderFile Id:" + intOldOrderFileID + ", Gift Certificate Name:" + displayName + ", Old OrderId:" + intOldOrderID + ", Old Order Number:" + intOldOrderNumber);
                                            }
                                        }
                                        else
                                        {
                                            OrderItemDigitalGood objOrderItemDigitalGood = new OrderItemDigitalGood();
                                            ////strSQL = new Text.StringBuilder()
                                            ////strSQL.Append("INSERT INTO ORDERFILES(OrderFile_ID,Order_ID,OrderItem_ID,File_ID,DisplayName,FileName,SaveAsName,MaxDownloads,RelativeTimeout,AbsoluteTimeout,MediaKey,ValidateDT,FulfillDT,LicenseKey) VALUES(")
                                            ////strSQL.Append(newOrderFileID & "," & newOrderId & ",")

                                            objOrderItemDigitalGood.OrderItemId = newOrderItemID;
                                            objOrderItemDigitalGood.DigitalGoodId = newFileID;
                                            objOrderItemDigitalGood.Name = digitalGoodName;

                                            // AC5x ValidateDT changes to AC7 ActivationDate
                                            objOrderItemDigitalGood.ActivationDate = ConvertDateTime(validatedt, "OrderItem DigitalGood validate date:" + validatedt + ", OrderId:" + objOrder.OrderId + ", OrderItemId:" + objOrderItemDigitalGood.OrderItemId + ", DigitalGood Name:" + objOrderItemDigitalGood.Name);

                                            //TODO: objOrderItemDigitalGood.DownloadDate

                                            // SET LICENSE KEY DATA
                                            if (!String.IsNullOrEmpty(licenseKey))
                                            {
                                                objOrderItemDigitalGood.SetSerialKey(HttpUtility.UrlDecode(licenseKey), false);
                                            }

                                            // SAVE THE DIGITAL GOOD
                                            saveResult = objOrderItemDigitalGood.Save();
                                            if (!saveResult.Equals(SaveResult.Failed))
                                            {
                                                //	add translation
                                                TranslationDictionary.Add("ORDERFILE" + intOldOrderFileID, objOrderItemDigitalGood.OrderItemDigitalGoodId.ToString());
                                            }
                                        } // next orderfile
                                    }
                                    catch (Exception ex)
                                    {
                                        LogError(GetPercentResult(), "Order File Import, Old OrderFileID:" + intOldOrderFileID + ", Digital Good Name:" + digitalGoodName + " , OrderID: " + intOldOrderID + " , OrderNumber: " + intOldOrderNumber + ", " + ex.Message + "\n" + ex.StackTrace);
                                    }
                                }

                                //	ORDER FILES PRESENT, CHECK FOR DOWNLOADS
                                XmlNodeList nlsDownloads;
                                nlsDownloads = nodOrder.SelectNodes("Downloads/Download");
                                if (nlsDownloads.Count > 0)
                                {
                                    int intOldDownloadID = 0;
                                    int intOldOrderFileID = 0;
                                    Dictionary<int, DateTime> earliestRelevantDownloads = new Dictionary<int, DateTime>();
                                    foreach (XmlNode nodDownload in nlsDownloads)
                                    {
                                        intOldDownloadID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodDownload, "ID", ""), 0);
                                        try
                                        {
                                            //newDownloadID = Token.Instance.StoreGroupDB.GetMaxID("DOWNLOADS")

                                            //	INSERT RECORD
                                            //strSQL = new Text.StringBuilder()
                                            //strSQL.Append("INSERT INTO DOWNLOADS(Download_ID,OrderFile_ID,DownloadDT,RemoteAddr,UserAgent,Referrer,Relevant) VALUES(")
                                            //strSQL.Append(newDownloadID & ",")

                                            //	LOCATE THE NEW AC7X ORDERITEM DIGITAL GOOD
                                            intOldOrderFileID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodDownload, "OrderFile_ID", ""));
                                            int newOrderFileID = 0;
                                            if (TranslationDictionary.ContainsKey("ORDERFILE" + intOldOrderFileID))
                                            {
                                                newOrderFileID = AlwaysConvert.ToInt(TranslationDictionary["ORDERFILE" + intOldOrderFileID]);
                                            }
                                            OrderItemDigitalGood oidg = OrderItemDigitalGoodDataSource.Load(newOrderFileID);
                                            if (oidg != null)
                                            {
                                                // ORDERITEMDIGITALGOOD LOCATED, PROCESS THE DOWNLOAD DATA
                                                Download objDownload = new Download();

                                                //strSQL.Append(newOrderFileID & ",")
                                                objDownload.OrderItemDigitalGoodId = newOrderFileID;

                                                //DownloadDT
                                                //strSQL.Append(FCast.FSQLDate(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodDownload, "DownloadDT")), intDBType) & ",")
                                                objDownload.DownloadDate = ConvertDateTime(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodDownload, "DownloadDT", "")), "DownloadDT");

                                                //	RemoteAddr
                                                //strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodDownload, "RemoteAddr")), intDBType) & ",")
                                                objDownload.RemoteAddr = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodDownload, "RemoteAddr", ""));

                                                //	UserAgent
                                                //strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodDownload, "UserAgent")), intDBType) & ",")
                                                objDownload.UserAgent = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodDownload, "UserAgent", ""));

                                                //	Referrer
                                                //strSQL.Append(FCast.FSQLString(HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodDownload, "Referrer")), intDBType) & ",")
                                                objDownload.Referrer = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodDownload, "Referrer", ""));

                                                //	For Relevant flag, we need to know earliest relevant AC5x download
                                                bool isRelevant = (AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodDownload, "Relevant", "0")) == 1);
                                                if (isRelevant &&
                                                    (!earliestRelevantDownloads.ContainsKey(oidg.OrderItemDigitalGoodId)
                                                    || objDownload.DownloadDate < earliestRelevantDownloads[oidg.OrderItemDigitalGoodId]))
                                                {
                                                    earliestRelevantDownloads[oidg.OrderItemDigitalGoodId] = objDownload.DownloadDate;
                                                }

                                                // RECORD DOWNLOAD
                                                saveResult = objDownload.Save();
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            LogError(GetPercentResult(), "Order Download import, Old DownloadID:" + intOldDownloadID + ", Old OrderFileID :" + intOldOrderFileID + " , OrderID: " + intOldOrderID + " , OrderNumber: " + intOldOrderNumber + ", " + ex.Message + "\n" + ex.StackTrace);
                                        }
                                    } //Next nodDownload

                                    // UPDATE THE EARLIEST RELEVANT DOWNLOAD COUNTER FOR AC7
                                    foreach (int oidgId in earliestRelevantDownloads.Keys)
                                    {
                                        try
                                        {
                                            OrderItemDigitalGood oidg = OrderItemDigitalGoodDataSource.Load(oidgId);
                                            if (oidg != null)
                                            {
                                                oidg.DownloadDate = earliestRelevantDownloads[oidgId];
                                                oidg.Save();
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }

                            //import ORDER_COUPON_ASSN
                            XmlNodeList nlsCoupons = nodOrder.SelectNodes("Coupons/Coupon");
                            if (nlsCoupons.Count > 0)
                            {
                                foreach (XmlNode nodCoupon in nlsCoupons)
                                {
                                    //	translate Coupon_ID
                                    int intOldCouponID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodCoupon, "ID", ""), 0);
                                    int newCouponID = 0;
                                    if (TranslationDictionary.ContainsKey("COUPON" + intOldCouponID))
                                    {
                                        newCouponID = AlwaysConvert.ToInt(TranslationDictionary["COUPON" + intOldCouponID]);
                                    }
                                    //	only import record if new coupon identified... otherwise data is meaningless
                                    if (!newCouponID.Equals(0))
                                    {
                                        //TODO: need copon code here
                                        //OrderCoupon objOrderCoupon = new OrderCoupon(newOrderId,newCouponID);
                                        //saveResult =  objOrderCoupon.Save();

                                        //strSQL = New Text.StringBuilder()
                                        //strSQL.Append("INSERT INTO ORDER_COUPON_ASSN(Order_ID,Coupon_ID,OrderBy) VALUES(")
                                        //strSQL.Append(newOrderId & "," & newCouponID & "," & AlwaysConvert.ToInt(XmlUtility.GetElementValue(nodCoupon,"OrderBy")) & ")")
                                        //intResult = Token.Instance.StoreGroupDB.QueryNoRS(strSQL.ToString())
                                        //strSQL = null
                                    }
                                }//next nodCoupon
                            }

                            //	update totals based on new order items
                            //TODO:
                            //decTotalCharges = AlwaysConvert.ToDecimal(Token.Instance.StoreGroupDB.QueryValue("SELECT SUM(ExtPrice*Quantity) as TotalPrice FROM ORDERITEMS WHERE Order_ID=" & newOrderId))
                            //decTotalPayments = AlwaysConvert.ToDecimal(Token.Instance.StoreGroupDB.QueryValue("SELECT SUM(Amount) as TotalPmt FROM PAYMENTS WHERE Order_ID=" & newOrderId & " AND Processed=1"))
                            //intResult = Token.Instance.StoreGroupDB.QueryNoRS("UPDATE ORDERS SET TotalCharges=" & decTotalCharges & ",TotalPayments=" & decTotalPayments & " WHERE Order_ID=" & newOrderId)

                        } //intresult > 0

                        // UPDATE PAYMENT AND SHIPMENT STATUS
                        objOrder.RecalculatePaymentStatus();
                        objOrder.RecalculateShipmentStatus();

                        // RESET THE ORDER STATUS AS IMPORTED FROM AC5x
                        objOrder.OrderStatusId = newOrderStatusID;
                        objOrder.Save(false);                        

                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "Orders Import, OrderID:" + intOldOrderID + " , Order Number: " + intOldOrderNumber + ", " + ex.Message + "\n" + ex.StackTrace);
                    }

                }//next nodOrder                

                // RE-ENABLE THE PRODUCT INVENTORY IF NEEDED
                if (inventoryEnabled) Token.Instance.Store.EnableInventory = true;

                LogStatus(GetPercentResult(), "Orders Import Complete...");
            }
            else
            {
                currentPhasePercent = currentPhaseWeightage;
                //LogStatus(GetPercentResult(), "No Orders Available to Import...");
            }            

            //UPDATE ORDER NUMBER FOR CURRENT STORE TO NEXT AVAILABLE NUMBER
            StoreDataSource.SetNextOrderNumber();

            //    int intOrderNumber_SuggSetting = Token.Instance.Store.OrderNumber  //AlwaysConvert.ToInt(Token.Instance.StoreGroupDB.QueryValue("SELECT MAX(OrderNumber) FROM ORDERS WHERE Store_ID=" & Token.Instance.StoreId)) + 1
            //    if intNextNum = 1 then
            //    //	'update to import setting
            //        Token.Instance.Store.OrderNumber = intOrderNumber_ImportSetting
            //        sWarnings.Append("Note: Next Order Number Updated to Import Setting of " & intOrderNumber_ImportSetting & ".")
            //        if intOrderNumber_ImportSetting <> intOrderNumber_SuggSetting then
            //            sWarnings.Append("Warning: Recommended value for Next Order Number is " & intOrderNumber_SuggSetting & " to prevent duplicates and/or gaps in the number sequence.  Visit the store settings page to update the Next Order Number.")
            //        end if
            //    else
            //    //	'calculate next order number
            //        Token.Instance.Store.OrderNumber = intOrderNumber_SuggSetting
            //        sWarnings.Append("Note: Next Order Number Updated to Calculated Setting of " & intOrderNumber_SuggSetting & ".")
            //    end if
            //    Token.Instance.Store.Save(objDestToken)
            //end if

            
        }

        private bool objectTypeImpoted(String name)
        {
            name = name.ToLowerInvariant();            

            foreach (String key in TranslationDictionary.Keys)
            {
                if(key.StartsWith(name)) return true;
            }
            return false;            
        }

        private void ImportObjectAssociations(XmlNode nodSourceStore)
        {            
            
            
            XmlNodeList nlsItems = nodSourceStore.SelectNodes("ObjectLinks/Object");
            if (nlsItems.Count > 0)
            {
                int intTypeCount = 4;
                bool[] arrTypes = new bool[intTypeCount];

                bool categoriesImported = objectTypeImpoted("CATEGORY");
                bool productsImported = objectTypeImpoted("PRODUCT");
                bool webpagesImported = objectTypeImpoted("WEBPAGE");
                bool linksImported = objectTypeImpoted("LINK");


                String[] arrTranslateTypes = { "CATEGORY", "PRODUCT", "WEBPAGE", "LINK" };
                String[] arrDisplayTypes = { "Category", "Product", "Webpage", "Link" };

                int objType1, objType2;
                int intOldLinkID1, newLinkID1;
                int intOldLinkID2, newLinkID2;
                int orderBy, visible;
                CatalogVisibility newVisibility;

                LogStatus(GetPercentResult(), "Importing " + nlsItems.Count + " Associations .");
                int i = 0; // counter variable
                
                foreach (XmlNode objNode in nlsItems)
                {
                    i++;

                    currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);                    
                    try
                    {
                        objType1 = AlwaysConvert.ToInt(objNode.Attributes["ObjectType1"].Value);
                        objType2 = AlwaysConvert.ToInt(objNode.Attributes["ObjectType2"].Value);
                        intOldLinkID1 = AlwaysConvert.ToInt(objNode.Attributes["Object_ID1"].Value);
                        intOldLinkID2 = AlwaysConvert.ToInt(objNode.Attributes["Object_ID2"].Value);
                        orderBy = AlwaysConvert.ToInt(objNode.Attributes["OrderBy"].Value);
                        visible = AlwaysConvert.ToInt(objNode.Attributes["Visible"].Value);

                        LogStatus(GetPercentResult(), "Importing  Association " + i + " of " + nlsItems.Count + " (" + arrDisplayTypes[objType1] + " -> " + arrDisplayTypes[objType2] + " association)");

                        // SKIP INVALID ASSOCIATION
                        if( intOldLinkID2 == 0) continue;

                        // TRANSLATE TO NEW ID's
                        newLinkID1 = 0;
                        newLinkID2 = 0;
                        if(TranslationDictionary.ContainsKey(arrTranslateTypes[objType1].ToString() + intOldLinkID1.ToString())){
                            newLinkID1 = AlwaysConvert.ToInt(TranslationDictionary[arrTranslateTypes[objType1].ToString() + intOldLinkID1.ToString()]);
                        }

                        if(TranslationDictionary.ContainsKey(arrTranslateTypes[objType2].ToString() + intOldLinkID2.ToString())){
                            newLinkID2 = AlwaysConvert.ToInt(TranslationDictionary[arrTranslateTypes[objType2].ToString() + intOldLinkID2.ToString()]);
                        }

                        // VISIBILITY 
                        switch(visible){
                            case 0: newVisibility = CatalogVisibility.Hidden; break;
                            case 1: newVisibility = CatalogVisibility.Private; break;
                            case 3: newVisibility = CatalogVisibility.Public; break;
                            default: newVisibility = CatalogVisibility.Hidden; break;                            
                        }

                        // SKIP NON MAPPED ASSOCIATIONS
                        if( newLinkID2 == 0) continue;

                        
                        // IMPORT CATEGORY -> CATEGORY ASSOCIATIONS
                        if(categoriesImported && objType1 == 0 && objType2 == 0){
                            Category category = CategoryDataSource.Load(newLinkID2);
                            if (category != null)
                            {
                                if (category.ParentId == newLinkID1)
                                {
                                    // IF EXISTING VISIBILITY IS PRIVATE THEN IT MUST BE PRIVATE
                                    if (category.Visibility != CatalogVisibility.Private)
                                    {
                                        category.Visibility = newVisibility;
                                    }
                                    
                                    // RELOAD CATALOG NODES, DONT USE THE CATEGORY.CATALOGNODES
                                    CatalogNode catalogNode = CatalogNodeDataSource.Load(category.ParentId , category.CategoryId, (byte)CatalogNodeType.Category);
                                    if (catalogNode != null)
                                    {
                                        catalogNode.OrderBy = (Int16)orderBy;
                                        catalogNode.Save();
                                    }

                                    category.Save();
                                }
                            }
                        }
                        // IMPORT CATEGORY -> PRODUCT ASSOCIATION
                        else if(categoriesImported && productsImported &&  objType1 == 0 && objType2 == 1){
                            Product obCatalogNode = ProductDataSource.Load(newLinkID2);
                            if (obCatalogNode != null)
                            {
                                if (!obCatalogNode.Categories.Contains(newLinkID1))
                                {
                                    obCatalogNode.Categories.Add(newLinkID1);

                                    // IF THIS IS PREVIOUSLY TREATED IT AS ORPHANED THEN CORRECT IT
                                    if (obCatalogNode.Categories.Contains(OrphanedItemsCategoryId))
                                    {
                                        obCatalogNode.Categories.Remove(OrphanedItemsCategoryId);
                                    }

                                    obCatalogNode.Categories.Save();
                                }
                                // IF EXISTING VISIBILITY IS PRIVATE THEN IT MUST BE PRIVATE
                                if (obCatalogNode.Visibility != CatalogVisibility.Private)
                                {
                                    obCatalogNode.Visibility = newVisibility;
                                }
                                obCatalogNode.Save();
                            }
                        }
                        // IMPORT CATEGORY -> WEBPAGE ASSOCIATION 
                        else if(categoriesImported && webpagesImported &&  objType1 == 0 && objType2 == 2){
                            Webpage obCatalogNode = WebpageDataSource.Load(newLinkID2);
                            if (obCatalogNode != null)
                            {
                                if (!obCatalogNode.Categories.Contains(newLinkID1))
                                {
                                    obCatalogNode.Categories.Add(newLinkID1);
                                    
                                    // IF THIS IS PREVIOUSLY TREATED IT AS ORPHANED THEN CORRECT IT
                                    if (obCatalogNode.Categories.Contains(OrphanedItemsCategoryId))
                                    {
                                        obCatalogNode.Categories.Remove(OrphanedItemsCategoryId);
                                    }

                                    obCatalogNode.Categories.Save();
                                }
                                // IF EXISTING VISIBILITY IS PRIVATE THEN IT MUST BE PRIVATE
                                if (obCatalogNode.Visibility != CatalogVisibility.Private)
                                {
                                    obCatalogNode.Visibility = newVisibility;
                                }
                                obCatalogNode.Save();
                            }
                        }
                        // IMPORT CATEGORY -> LINK ASSOCIATION
                        else if(categoriesImported && linksImported &&  objType1 == 0 && objType2 == 3){
                            Link obCatalogNode = LinkDataSource.Load(newLinkID2);
                            if (obCatalogNode != null)
                            {
                                if (!obCatalogNode.Categories.Contains(newLinkID1))
                                {
                                    obCatalogNode.Categories.Add(newLinkID1);

                                    // IF THIS IS PREVIOUSLY TREATED IT AS ORPHANED THEN CORRECT IT
                                    if (obCatalogNode.Categories.Contains(OrphanedItemsCategoryId))
                                    {
                                        obCatalogNode.Categories.Remove(OrphanedItemsCategoryId);
                                    }

                                    obCatalogNode.Categories.Save();
                                }
                                // IF EXISTING VISIBILITY IS PRIVATE THEN IT MUST BE PRIVATE
                                if (obCatalogNode.Visibility != CatalogVisibility.Private)
                                {
                                    obCatalogNode.Visibility = newVisibility;
                                }
                                obCatalogNode.Save();
                            }
                        }
                        // IMPORT PRODUCT -> PRODUCT ASSOCIATION (RELATED PRODUCTS)
                        else if(productsImported &&  objType1 == 1 && objType2 == 1){
                            Product product = ProductDataSource.Load(newLinkID1);
                            if (product != null  && product.RelatedProducts.IndexOf(newLinkID1,newLinkID2) < 0)
                            {
                                RelatedProduct relatedProduct = new RelatedProduct(newLinkID1,newLinkID2);
                                relatedProduct.OrderBy = (short)orderBy;                                
                                relatedProduct.Save();
                                product.RelatedProducts.Add(relatedProduct);
                                product.RelatedProducts.Save();
                            }
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "Associations Import, Data:" + objNode .OuterXml + ", " + ex.Message + "\n" + ex.StackTrace);
                    }                    
                }//next                 
                LogStatus(GetPercentResult(), "Associations Import Complete...");

            }
            else
            {
                currentPhasePercent = currentPhaseWeightage;
                //LogStatus(GetPercentResult(), "No Associations Available to Import...");
            }
        }

        #region ESD -ProductFileAssociations Lines = 36

        private void ImportProductFileAssociations(XmlNode nodSourceStore)
        {
             StringDictionary dicProdFiles = new StringDictionary();
            //if InStr(lstCopyOpts, ",ESD,") > 0 then
                XmlNodeList nlsItems = nodSourceStore.SelectNodes("ESD/ProductFiles/ProductFile");
                if( nlsItems.Count > 0){
                    //response.write("Importing " & nlsItems.Count & " Product File Associations (ESD) .")
				    
                    LogStatus(GetPercentResult(), "Importing " + nlsItems.Count + " Product File Associations (ESD) .");
                    List<String> processedAssociationsList = new List<string>();
                    int i = 0; // counter variable
                    
                    foreach (XmlNode objNode in nlsItems)
                    {
                        i++;

                        currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);  
                        ProductDigitalGood productDigitalGood = null;
                        try
                        {

                            productDigitalGood = new ProductDigitalGood();
                            //'transfer Product ID
                            int intOldFileID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "File_ID", ""));
                            int intOldProductID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "Product_ID", ""));
                            int intOldOSDID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "OSD_ID", ""));

                            // BUG # 8217 Do not import duplicate product file associations
                            string key = intOldProductID + ":" + intOldFileID + ":" + ((intOldOSDID == 0 || intOldOSDID == -1) ? "0" : intOldOSDID.ToString());
                            if (processedAssociationsList.Contains(key))
                            {
                                // CONTINUE, SKIP IMPORTING THIS DUPLICATE ASSOCIATION
                                continue;
                            }
                            else processedAssociationsList.Add(key);

                            // VALIDATE THE PRODUCT ID
                            int intNewProductID = 0;
                            if (TranslationDictionary.ContainsKey("PRODUCT" + intOldProductID))
                            {
                                intNewProductID = AlwaysConvert.ToInt(TranslationDictionary["PRODUCT" + intOldProductID]);
                            }
                            Product product = ProductDataSource.Load(intNewProductID);

                            // VALIDATE THE FILE (DIGITAL GOOD) ID
                            int intNewFileID = 0;
                            if (TranslationDictionary.ContainsKey("FILE" + intOldFileID))
                            {
                                intNewFileID = AlwaysConvert.ToInt(TranslationDictionary["FILE" + intOldFileID]);
                            }
                            DigitalGood digitalGood = DigitalGoodDataSource.Load(intNewFileID);

                            // HANDLE INVALID DATA
                            if (product == null && digitalGood == null)
                            {
                                LogWarning(GetPercentResult(), "Cannot save product file association because Product " + intOldProductID + " and File ID " + intOldID + " were missing from the import data. Old Product Id: " + intOldProductID + ", Old File Id:" + intOldFileID + " , OSD_ID:" + intOldOSDID);
                                continue;
                            }
                            else if (product == null)
                            {
                                // PRODUCT IS MISSING SO WE CAN NOT ASSOCIATE
                                LogWarning(GetPercentResult(), "Cannot save product file association because Product " + intOldProductID + " was missing from the import data., Old Product Id: " + intOldProductID + ", Old File Id:" + intOldFileID + " , OSD_ID:" + intOldOSDID);
                                //Skip the import
                                continue;
                            }
                            else if (digitalGood == null)
                            {
                                // CHECK IF IT IS ACTUALLY A PRODUCT -> GIFTCERTI ASSOCIATION
                                if (TranslationDictionary.ContainsKey("GIFTCERT" + intOldFileID))
                                {
                                    // MARK PRODUCT AS GIFT CERTIFICATE
                                    Product gcProduct = ProductDataSource.Load(intNewProductID);
                                    gcProduct.IsGiftCertificate = true;
                                    gcProduct.Save();
                                }

                                // DIGITAL GOOD IS MISSING SO WE CAN NOT ASSOCIATE
                                else LogWarning(GetPercentResult(), "Cannot save product file association because File ID " + intOldFileID + " was missing from the import data., Old Product Id: " + intOldProductID + ", Old File Id:" + intOldFileID + " , OSD_ID:" + intOldOSDID);

                                // SKIP THE IMPORT
                                continue;
                            }

                            // LINK THE PRODUCT AND DIGITALGOOD
                            productDigitalGood.ProductId = intNewProductID;
                            productDigitalGood.DigitalGoodId = intNewFileID;

                            // BUG # 8216 ( -1 OPTION ID INDICATES THAT THE FILE IS ASSOCIATED TO PRODUCT)
                            // WE ONLY NEED TO TRANSLATE THE OSD ID IF IT IS GREATER THAN 0 BECAUSE
                            // AC5X USED A 1-BASED VARIANT INDEX.  ANY VALUE 0 OR BELOW INDICATES THE FILE
                            // IS ATTACHED TO THE PRODUCT ITSELF RATHER THAN AN OPTION
                            if (intOldOSDID > 0)
                            {
                                if (product.ProductOptions == null || product.ProductOptions.Count == 0)
                                {
                                    // IMPORT DATA SPECIFIES A VARIANT INDEX, BUT NO VARIANTS EXIST FOR PRODUCT
                                    // LOG A WARNING MESSAGE AND SKIP IMPORT
                                    LogWarning(GetPercentResult(), "Invalid product file association detected.  The file is associated to an option but no options are defined for this product.  File association is not imported.  Product: " + product.Name + ", File: " + digitalGood.Name + ", Invalid OSD_ID: " + intOldOSDID + ", Node Data:" + objNode.OuterXml);
                                    continue;
                                }
                                else
                                {
                                    // OSD_ID IS A 1-BASED VARIANT INDEX (AC7 USES A ZERO BASED VARIANT INDEX)
                                    // TRANSLATE THE OPTION LIST FOR THIS DIGITAL GOOD
                                    int intNewVariantId = 0;
                                    ProductVariant variant = null;

                                    // IF PRODUCT HAS PERSISTENT VARIANTS THEN TRANSLATE THE OLD OSD ID
                                    if (TranslationDictionary.ContainsKey("OSD" + intOldProductID + "|" + intOldOSDID) && product.Variants.Count > 0)
                                    {
                                        intNewVariantId = AlwaysConvert.ToInt(TranslationDictionary["OSD" + intOldProductID + "|" + intOldOSDID]);
                                        variant = ProductVariantDataSource.Load(intNewVariantId);
                                    }
                                    else
                                    {
                                        // MAP THE AC55 VARIANT TO RESPECTIVE PRODUCT VARIANT
                                        // THERE WILL BE NO VARIANT ID, BECAUSE THE VARIANT IS NOT PERSISTANT
                                        ProductVariantManager variantManager = new ProductVariantManager(product.ProductId);
                                        if (variantManager.CountVariantGrid() >= intOldOSDID) variant = variantManager.GetVariantByIndex(intOldOSDID - 1);
                                    }

                                    if (variant == null)
                                    {
                                        // VARIANT COULD NOT BE MAPPED
                                        // LOG A WARNING MESSAGE AND SKIP IMPORT
                                        LogWarning(GetPercentResult(), "Invalid product file association detected.  The file is associated to a variant that could not be correctly mapped.  File association is not imported.  Product: " + product.Name + ", File: " + digitalGood.Name + ", Invalid OSD_ID: " + intOldOSDID + ", Node Data:" + objNode.OuterXml);
                                        continue;
                                    }
                                    else
                                    {
                                        productDigitalGood.OptionList = variant.OptionList;
                                    }
                                }
                            }

                            // SAVE THE PRODUCT -> DIGITAL GOOD ASSOCIATION
                            if (productDigitalGood.Save() == SaveResult.Failed)
                            {
                                LogError(GetPercentResult(), "Product File Associations (ESD) Import: Error while saving, Data:" + objNode.OuterXml);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogError(GetPercentResult(), "Product File Associations (ESD) Import, Data:" + objNode.OuterXml + ", " + ex.Message + "\n" + ex.StackTrace);
                        }                    
                    }//next                 
                    LogStatus(GetPercentResult(), "Product File Associations (ESD) Import Complete...");
				    
                }else{
                    currentPhasePercent = currentPhaseWeightage;
                    //LogStatus(GetPercentResult(), "No Product File Associations (ESD) Available to Import...");
				    
                }
        
            /*
             //REMOVE THIS OLD COMMENTED CODE AFTER COPLETION OF CONVERTION
        Dim dicProdFiles As New StringDictionary
            //if InStr(lstCopyOpts, ",ESD,") > 0 then
                XmlNodeList nlsItems = nodSourceStore.SelectNodes("ESD/ProductFiles/ProductFile")
                if( nlsItems.Count > 0){
                    //response.write("Importing " & nlsItems.Count & " Product File Associations (ESD) .")
				    
                    foreach(XmlNode objNode in nlsItems){
                        'transfer Product ID
                        intOldProductID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "Product_ID"))
                        if (TranslationDictionary.ContainsKey("PRODUCT" & intOldProductID) then
                            intNewProductID = AlwaysConvert.ToInt(objTranslate("PRODUCT" & intOldProductID))
                        else
                            intNewProductID = 0
                        end if
                        'transfer OSD ID
                        intOldOSDID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "OSD_ID"))
                        if (TranslationDictionary.ContainsKey("OSD" & intOldOSDID) then
                            intNewOSDID = AlwaysConvert.ToInt(objTranslate("OSD" & intOldOSDID))
                        else
                            intNewOSDID = intOldOSDID
                        end if
                        'transfer file ID
                        intOldFileID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "File_ID"))
                        if (TranslationDictionary.ContainsKey("FILE" & intOldFileID) then
                            intNewFileID = AlwaysConvert.ToInt(objTranslate("FILE" & intOldFileID))
                        else
                            intNewFileID = 0
                        end if
                        If Not dicProdFiles.ContainsKey(intNewProductID & ":" & intNewOSDID & ":" & intNewFileID) Then
                            Token.Instance.StoreGroupDB.QueryNoRS("INSERT INTO PRODUCT_FILE_ASSN (Product_ID,OSD_ID,File_ID) VALUES (" & intNewProductID & "," & intNewOSDID & "," & intNewFileID & ")")
                            dicProdFiles(intNewProductID & ":" & intNewOSDID & ":" & intNewFileID) = ""
                        End If
                        //response.write(" .")
					    
                    } //next objNode
                    //response.write("")
				    
                else
                    Status(GetPercentResult(), "No Product File Associations (ESD) Available to Import...")
				    
                end if
            end if
      * */
        }

        #endregion


        private void ImportRights(XmlNode nodSourceStore)
        {
            //if InStr(lstCopyOpts, ",RIGHTS,") > 0 then          
            currentPhasePercent = 0;
            //LogStatus(GetPercentResult(), "Checking Rights...");

            XmlNodeList nlsRights = nodSourceStore.SelectNodes("Rights/Right");
            if (nlsRights.Count > 0)
            {
                int i = 0; // counter variable
                //LogStatus(GetPercentResult(), "Checking " + nlsRights.Count + " Rights.");

                Role objRight = null;
                
                foreach (XmlNode nodRight in nlsRights)
                {
                    i++;
                    int intOldRightID = 0;
                    currentPhasePercent = CalculatePartialPercentage(i, nlsRights.Count);
                    LogStatus(GetPercentResult(), "Importing Right " + i + " of " + nlsRights.Count);

                    try
                    {
                        objRight = new Role();
                        intOldRightID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(nodRight, "ID", "0"), 0);
                        objRight.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(nodRight, "Name", "Right"));
                        if (!objRight.Save().Equals(SaveResult.Failed))
                        {
                            TranslationDictionary.Add("RIGHT" + intOldRightID, objRight.RoleId.ToString());
                        }

                        //	'now import files
                        string[] arrTemp = null;
                        string lstTemp = XmlUtility.GetElementValue(nodRight, "Files", "");
                        if (lstTemp.Length > 0)
                        {
                            arrTemp = lstTemp.Split(',');
                            //for (int i = 0; i<arrTemp.Length;i++){
                            //    //Comments:
                            //    //There is no equivalent to user rights. I decided this was too complex without being useful. All �rights� are managed with �role� memberships in AC6. Ignore user rights and user file rights data in import.
                            //    //Token.Instance.StoreGroupDB.QueryNoRS("INSERT INTO FILE_RIGHT_ASSN (FileName,Store_ID,RightName) VALUES (" & FCast.FSQLString(arrTemp(i), Token.Instance.StoreGroupDB.DBType) & "," & Token.Instance.StoreId & "," & FCast.FSQLString(objRight.Name, Token.Instance.StoreGroupDB.DBType) & ")")
                            //}
                        }

                        //	now import groups
                        lstTemp = XmlUtility.GetElementValue(nodRight, "Groups", "");
                        if (lstTemp.Length > 0)
                        {
                            arrTemp = lstTemp.Split(',');
                            //for(int i = 0; i< arrTemp.Length;i++){
                            //    int intOldGroupID = AlwaysConvert.ToInt(arrTemp[i]);
                            //    if (objTranslate.ContainsKey("GROUP" + intOldGroupID)){
                            //        int newGroupID = AlwaysConvert.ToInt(objTranslate["GROUP" + intOldGroupID]);
                            //        //TODO:
                            //        //Comments:
                            //        //Group rights�. There is only one translation that can be made I think. Unfortunately it�s not a simple one. For now, ignore group rights data. Just import the group as a �role� and add the appropriate users.
                            //        //Token.Instance.StoreGroupDB.QueryNoRS("INSERT INTO GROUP_RIGHT_ASSN (Group_ID,Right_ID,Authorized) VALUES (" & intNewGroupID & "," & objRight.Right_ID & ",1)")
                            //    }
                            //}
                        }

                        //	'now import users
                        lstTemp = XmlUtility.GetElementValue(nodRight, "Users", "");
                        if (lstTemp.Length > 0)
                        {
                            arrTemp = lstTemp.Split(',');
                            //for(int i = 0; i< arrTemp.Length; i++){
                            //    int intOldUserID = AlwaysConvert.ToInt(arrTemp[i]);
                            //    if (objTranslate.ContainsKey("USER" + intOldUserID)){
                            //        int newUserID = AlwaysConvert.ToInt(objTranslate["USER" + intOldUserID]);
                            //        //Comments:
                            //        //There is no equivalent to user rights. I decided this was too complex without being useful. All �rights� are managed with �role� memberships in AC6. Ignore user rights and user file rights data in import.
                            //        //Token.Instance.StoreGroupDB.QueryNoRS("INSERT INTO USER_RIGHT_ASSN (User_ID,Right_ID,Authorized) VALUES (" & intNewUserID & "," & objRight.Right_ID & ",1)")
                            //    }
                            //}
                        }

                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "Rights Import, RightID:" + intOldRightID + " , Name: " + objRight.Name + ", " + ex.Message + "\n" + ex.StackTrace);
                    }                    
                }//next nodRight
                objRight = null;
                LogStatus(GetPercentResult(), "Rights Import Complete...");

            }
            else
            {
                currentPhasePercent = currentPhaseWeightage;
                //LogStatus(GetPercentResult(), "No Rights Available to Import...");
            }
        }



        #endregion

        #region pass 4

        #region GiftCerts Lines = 70

        private void ImportGiftCerts(XmlNode nodSourceStore)
        {
            // if InStr(lstCopyOpts, ",GIFTCERTS,") > 0 then
            //'relies on order information (orderitem_ID)
            //response.write("Checking Gift Certificates ...")
			
            //XmlNodeList nlsItems = nodSourceStore.SelectNodes("GiftCerts/GiftCert")
            //if( nlsItems.Count > 0){
            //    response.write("Importing " & nlsItems.Count & " Gift Certificates .")
            
            currentPhasePercent = 0;            

            XmlNodeList nlsItems = nodSourceStore.SelectNodes("GiftCerts/GiftCert");
            if (nlsItems.Count > 0)
            {
                int i = 0; // counter variable
                //LogStatus(GetPercentResult(), "Checking " + nlsItems.Count + " Categories.");

                GiftCertificate objGiftCert = null;
                List<int> mappedGiftCerts = new List<int>();
                foreach(XmlNode objNode in nlsItems)
                {
                    objGiftCert = null;
                    i++;

                    currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
                    LogStatus(GetPercentResult(), "Importing Certificate " + i + " of " + nlsItems.Count);
                    int intOldGiftCertID = 0;
                    try
                        {                                  
                            intOldGiftCertID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID",""),0);                                

                            //	Translate OrderItem_ID
                            int intOldOrderItemID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "OrderItem_ID",""));
                            int intNewOrderItemID = 0;
                            if(TranslationDictionary.ContainsKey("ORDERITEM" + intOldOrderItemID.ToString())){
                                intNewOrderItemID = AlwaysConvert.ToInt(TranslationDictionary["ORDERITEM" + intOldOrderItemID.ToString()]);
                            }else{
                                intNewOrderItemID = 0;
                            }

                            String serialNumber = XmlUtility.GetElementValue(objNode, "SerialNumber", "");
                            
                            // CHECK IF THE  GIFT CERTIFICATE DATE FOR ORDER AS ORDERFILE IS ALREADY IMPORTED
                            OrderItem gcOrderItem = OrderItemDataSource.Load(intNewOrderItemID);
                            if (gcOrderItem != null)
                            {
                                if(!String.IsNullOrEmpty(serialNumber)){
                                    foreach (GiftCertificate gc in gcOrderItem.GiftCertificates)
                                    {
                                        if (gc.SerialNumber.Equals(serialNumber))
                                        {
                                            objGiftCert = gc;
                                            if (!mappedGiftCerts.Contains(gc.GiftCertificateId)) mappedGiftCerts.Add(gc.GiftCertificateId);
                                            break;
                                        }
                                    }
                                }
                                
                                if(objGiftCert == null)
                                { 
                                    // MAP IT WITH THE NEXT UN MAPED GC FOR THIS ORDER ITEM
                                    foreach (GiftCertificate gc in gcOrderItem.GiftCertificates)
                                    { 
                                        // MAP IT TO THE FIRST AVAILABLE GC AND BREAK THE LOOP TO CONTINUE
                                        if(!mappedGiftCerts.Contains(gc.GiftCertificateId)){
                                            objGiftCert = gc;  
                                            mappedGiftCerts.Add(gc.GiftCertificateId);
                                            break;
                                        }
                                    }
                                }
                            }

                            if (objGiftCert == null)
                            {
                                objGiftCert = new GiftCertificate();
                                objGiftCert.Name = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "SerialNumber", ""));
                            }

                            objGiftCert.OrderItemId = intNewOrderItemID;
                             
                            objGiftCert.SerialNumber = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "SerialNumber",""));
                            objGiftCert.Balance = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(objNode, "Balance",""));
             
                            if(objGiftCert.Save() != SaveResult.Failed){
                                //	UPDATE PROPERTIES THAT ARE FORCED BY SAVEASNEW
                                //	Translate CreatedBy (User_ID)

                                int intOldUserID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "CreatedBy",""));
                                int intNewUserID = 0;
                                if(TranslationDictionary.ContainsKey("USER" + intOldUserID.ToString())){
                                    intNewUserID = AlwaysConvert.ToInt(TranslationDictionary["USER" + intOldUserID.ToString()]);
                                    objGiftCert.CreatedBy = intNewUserID;
                                }
                              
                                //objGiftCert.CreateDT = AlwaysConvert.ToDateTime(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "CreateDT")));
                                string srtCreateDT = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "CreateDT",""));
                                objGiftCert.CreateDate = ConvertDateTime(srtCreateDT, "CreateDate for Giftcertificate:" + srtCreateDT);
                                                                
                                string srtExpirationDate = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "ExpireDT", ""));
                                DateTime expDate = ConvertDateTime(srtExpirationDate, "ExpirationDate for Giftcertificate:" + srtExpirationDate);
                                if (expDate > objGiftCert.CreateDate) objGiftCert.ExpirationDate = expDate;
                                

                                //	SAVE UPDATES
                                objGiftCert.Save();

                                //	ADD TRANSLATION
                                TranslationDictionary.Add("GiftCert" + intOldGiftCertID.ToString(), objGiftCert.GiftCertificateId.ToString());
             

                            }
                        
                        }
                        catch (Exception ex)
                        {
                            LogError(GetPercentResult(), "Gift Certificates Import, GiftCertID:" + intOldGiftCertID + " , Name: " + objGiftCert.Name + ", " + ex.Message + "\n" + ex.StackTrace);
                        }

               }

               LogStatus(GetPercentResult(), "Gift Certificates Import Complete...");                
           }
           else
           {
                currentPhasePercent = currentPhaseWeightage;
                //Status(GetPercentResult(), "No Gift Certificates Available To Import...");
           }
        }
           

        #endregion

        private void ImportCategoryDiscounts(XmlNode nodSourceStore)
        {
            String lstDiscounts = string.Empty;
            String[] arrOldDiscounts;

            //if InStr(lstCopyOpts, ",CATEGORIES,") > 0 AND InStr(lstCopyOpts, ",DISCOUNTS,") > 0 then
            //	'import category discount associations        
            currentPhasePercent = 0;
            //LogStatus(GetPercentResult(), "Checking Category Discounts...");

            XmlNodeList nlsItems = nodSourceStore.SelectNodes("Categories/Category");
            if (nlsItems.Count > 0)
            {
                int i = 0; // counter variable
                //LogStatus(GetPercentResult(), "Checking " + nlsItems.Count + " Categories.");

                Category objCategory = null;
                foreach (XmlNode objNode in nlsItems)
                {
                    i++;

                    currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
                    LogStatus(GetPercentResult(), "Importing for Category " + i + " of " + nlsItems.Count);
                    int intOldCategoryID = 0;
                    try
                    {
                        lstDiscounts = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Discounts", ""));
                        if (lstDiscounts.Length > 0)
                        {
                            intOldCategoryID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID", "0"), 0);
                            if (TranslationDictionary.ContainsKey("CATEGORY" + intOldCategoryID))
                            {
                                int newCategoryID = AlwaysConvert.ToInt(TranslationDictionary["CATEGORY" + intOldCategoryID]);
                                objCategory = new Category();
                                if (objCategory.Load(newCategoryID))
                                {
                                    arrOldDiscounts = lstDiscounts.Split(',');
                                    foreach (String OldDiscountID in arrOldDiscounts)
                                    {
                                        if (TranslationDictionary.ContainsKey("DISCOUNT" + OldDiscountID))
                                        {
                                            //objCategory.Discounts.Add(AlwaysConvert.ToInt(objTranslate("DISCOUNT" & arrOldDiscounts(i))))
                                            int newDicountID = AlwaysConvert.ToInt(TranslationDictionary["DISCOUNT" + OldDiscountID]);
                                            objCategory.CategoryVolumeDiscounts.Add(new CategoryVolumeDiscount(newCategoryID, newDicountID));
                                        }
                                    }
                                    objCategory.CategoryVolumeDiscounts.Save();
                                    objCategory.Save();
                                    //response.write(" o")
                                }
                                objCategory = null;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "Category Discounts Import, CategoryID:" +intOldCategoryID + " , Name: " + objCategory.Name + ", " + ex.Message + "\n" + ex.StackTrace);
                    }

                } //next objNode

                LogStatus(GetPercentResult(), "Category Discounts Import Complete...");
                //Token.Instance.StoreGroupDB.QueryNoRS("DELETE FROM CATEGORY_DISCOUNT_ASSN WHERE Discount_ID=0");
            }
            else
            {
                currentPhasePercent = currentPhaseWeightage;
                //LogStatus(GetPercentResult(), "No Category Discounts Available to Import...");
            }
        }

        private void ImportProductDiscounts(XmlNode nodSourceStore)
        {
            //if InStr(lstCopyOpts, ",PRODUCTS,") > 0 AND InStr(lstCopyOpts, ",DISCOUNTS,") > 0 then
            //	'import product discount associations
            //response.write("Checking Product Discounts...")
            currentPhasePercent = 0;
            //LogStatus(GetPercentResult(), "Checking Product Discounts...");

            XmlNodeList nlsItems = nodSourceStore.SelectNodes("Products/Product");
            if (nlsItems.Count > 0)
            {
                //response.write("Checking " & nlsItems.Count & " Products .")
                int i = 0; // counter variable
                //LogStatus(GetPercentResult(), "Checking " + nlsItems.Count + " Products.");

                Product objProduct = null;
                foreach (XmlNode objNode in nlsItems)
                {
                    int intOldProductID = 0;
                    i++;

                    currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
                    LogStatus(GetPercentResult(), "Importing for Product " + i + " of " + nlsItems.Count);

                    try
                    {
                        String lstDiscounts = HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "Discounts", ""));
                        if (lstDiscounts.Length > 0)
                        {
                            intOldProductID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID", "0"), 0);
                            if (TranslationDictionary.ContainsKey("PRODUCT" + intOldProductID))
                            {
                                int newProductID = AlwaysConvert.ToInt(TranslationDictionary["PRODUCT" + intOldProductID]);
                                objProduct = new Product();
                                if (objProduct.Load(newProductID))
                                {
                                    String[] arrOldDiscounts = lstDiscounts.Split(',');
                                    foreach (String OldDiscountID in arrOldDiscounts)
                                    {
                                        if (TranslationDictionary.ContainsKey("DISCOUNT" + OldDiscountID))
                                        {
                                            int newDiscountID = AlwaysConvert.ToInt(TranslationDictionary["DISCOUNT" + OldDiscountID]);
                                            objProduct.ProductVolumeDiscounts.Add(new ProductVolumeDiscount(newProductID, newDiscountID));
                                            //objProduct.Discounts.Add(AlwaysConvert.ToInt(objTranslate("DISCOUNT" & arrOldDiscounts(i))))
                                        }
                                    }
                                    objProduct.ProductVolumeDiscounts.Save();
                                    objProduct.Save();//response.write(" o")
                                }
                                objProduct = null;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "Product Discounts Import, ProductID:" + intOldProductID + " , Name: " + objProduct.Name + ", " + ex.Message + "\n" + ex.StackTrace);
                    }
                } //next objNode
                LogStatus(GetPercentResult(), "Product Discounts Import Complete...");
                //Token.Instance.StoreGroupDB.QueryNoRS("DELETE FROM PRODUCT_DISCOUNT_ASSN WHERE Discount_ID=0")
            }
            else
            {
                currentPhasePercent = currentPhaseWeightage;
                //LogStatus(GetPercentResult(), "No Product Discounts Available to Import...");
            }
        }        

        private void CompleteUserImport(XmlNode nodSourceStore)
        {
            //if InStr(lstCopyOpts, ",USERS,") > 0 Then
            //	FINALIZE IMPORTED USERS        
            currentPhasePercent = 0;           

            XmlNodeList nlsItems = nodSourceStore.SelectNodes("Users/User");
            if (nlsItems.Count > 0)
            {
                LogStatus(GetPercentResult(), "Completing User Import (Associating wishlists)...");
                //response.write("Checking " & nlsItems.Count & " Users .")
                int i = 0; // counter variable
                //LogStatus(GetPercentResult(), "Checking " + nlsItems.Count + " Users.");

                DateTime dtLastAuth;
                int intOldWishlistID = 0;
                int newWishlistID = 0;
                User objUser = null;
                foreach (XmlNode objNode in nlsItems)
                {
                    // update the import status
                    i++;

                    currentPhasePercent = CalculatePartialPercentage(i, nlsItems.Count);
                    LogStatus(GetPercentResult(), "Importing for User " + i + " of " + nlsItems.Count);
                    int intOldUserID = 0;
                    try
                    {
                        //	'force LastAuthDT
                        objUser = new User();
                        intOldUserID = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(objNode, "ID", "0"), 0);
                        if (TranslationDictionary.ContainsKey("USER" + intOldUserID))
                        {
                            int newUserID = AlwaysConvert.ToInt(TranslationDictionary["USER" + intOldUserID]);
                            dtLastAuth = ConvertDateTime(HttpUtility.UrlDecode(XmlUtility.GetElementValue(objNode, "LastAuthDT", "")), "LastAuthDT"); 
                            //	'TRANSLATE DEFAULT WISHLIST ID
                            intOldWishlistID = AlwaysConvert.ToInt(XmlUtility.GetElementValue(objNode, "DefaultWishlist_ID", "0"));
                            if (TranslationDictionary.ContainsKey("WISHLIST" + intOldWishlistID))
                            {
                                newWishlistID = AlwaysConvert.ToInt(TranslationDictionary["WISHLIST" + intOldWishlistID]);
                            }
                            else
                            {
                                newWishlistID = 0;
                            }
                            if (objUser.Load(newUserID))
                            {
                                if (dtLastAuth > System.DateTime.MinValue)
                                {
                                    //	'UPDATE AUTH AND WISHLIST
                                    //Token.Instance.StoreGroupDB.QueryNoRS("UPDATE USERS SET LastAuthDT=" & FCast.FSQLDate(dtLastAuth, intDBType) & ",DefaultWishlist_ID=" & NullZeroInt(intNewWishlistID) & " WHERE User_ID=" & intNewUserID)
                                    objUser.LastLoginDate = dtLastAuth;
                                    objUser.PrimaryWishlistId = newWishlistID;
                                    //response.write(" o")
                                }
                                else
                                {
                                    //	'UPDATE WISHLIST ONLY
                                    //Token.Instance.StoreGroupDB.QueryNoRS("UPDATE USERS SET DefaultWishlist_ID=" & NullZeroInt(intNewWishlistID) & " WHERE User_ID=" & intNewUserID)
                                    objUser.PrimaryWishlistId = newWishlistID;
                                }
                                objUser.Save();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "Completing Users Import( Associating wishlists), UserID:" + intOldUserID + " , User Name: " + objUser.UserName +" , WishlistID: " + intOldWishlistID + ", " + ex.Message + "\n" + ex.StackTrace);
                    }
                }

                LogStatus(GetPercentResult(), "Users Import Complete(Associating wishlists)...");

            }
            else
            {
                currentPhasePercent = currentPhaseWeightage;
                //LogStatus(GetPercentResult(), "No Users Available to Update...");
            }
        }

        #endregion

        // "********** Import Complete ***********/
        

        #endregion

        #region Asynchronios import Methods

        // Begins an asynchronous call to our long running import method.
        public System.IAsyncResult BeginAsycnImport(XmlDocument xmlImportDocument, System.AsyncCallback cb, object state)
        {
            return this.importDelegate.BeginInvoke(xmlImportDocument, cb, state);
        }

        // Waits for the pending asynchronous request to complete.
        public void EndAsycnImport(System.IAsyncResult result)
        {
            this.importDelegate.EndInvoke(result);
        }


        #endregion

        #region Static Methods

        /// <summary>
        /// Saves and validates the uploaded zip file
        /// uncompresses the zip files and validates the xml file to be valid MakerShop import xml
        /// </summary>
        /// <param name="UploadedFile"></param>
        /// <returns>An error message in case of an error otherwise an empty string</returns>
        public static XmlDocument SaveAndValidateFile(FileUpload UploadedFile, string path)
        {
            // Specify the path on the server to
            // save the uploaded file to.        
            //String savePath = path;        
            String fileContent = "";
            String strMessage = string.Empty;
            XmlDocument xmlImport = new XmlDocument();


            // 1-- Save the uploaded file
            // Get the name of the file to upload.
            //String fileName =  Guid.NewGuid().ToString() + UploadedFile.FileName;

            //// Append the name of the file to upload to the path.
            //if (!File.Exists(savePath))
            //{
            //    Directory.CreateDirectory(savePath);
            //}
            //savePath += fileName;
            //UploadedFile.SaveAs(savePath);

            // 2-- uncompress file and get the uncompressed xml file
            if (UploadedFile.HasFile)
            {
                HttpPostedFile postedFile = UploadedFile.PostedFile;

                if (UploadedFile.FileName.ToLowerInvariant().EndsWith(".zip"))
                {
                    fileContent = UnzipFile(postedFile.InputStream);
                }
                else
                {
                    int contentLength = postedFile.ContentLength;
                    if (contentLength > 0)
                    {
                        StreamReader reader = new StreamReader(postedFile.InputStream);
                        fileContent = reader.ReadToEnd();
                    }
                }
            }


            // 3-- validate xml file         
            //StreamReader sr = File.OpenText(savePath);
            //fileContent = sr.ReadToEnd();
            //sr.Close();
            //sr.Dispose();


            if (fileContent != null || fileContent.Length > 0)
            {
                XmlNodeList nlsStores;
                try
                {
                    xmlImport.LoadXml(fileContent);
                }
                catch (XmlException xmle)
                {
                    throw new Exception("The uploaded file was not valid XML data.  Exception reported: " + xmle.ToString());
                }

                nlsStores = xmlImport.DocumentElement.SelectNodes("Stores/Store");
                if (nlsStores.Count == 0)
                {
                    throw new Exception("The uploaded file was either an invalid store export, or the file did not contain any store data.");
                }
            }
            else
            {
                throw new Exception("Error: Uploaded file was empty.");
            }

            return xmlImport;
        }

        #endregion

        #region Private helper methods


        private string ConvertOptionNames(string optionNames)
        {
            List<String> options = new List<string>();
            try
            {
                Regex RegexObj = new Regex("<span class=\"name\"[^>]*>(.*?)</span>");
                Match MatchResults = RegexObj.Match(optionNames);
                while (MatchResults.Success)
                {
                    for (int i = 1; i < MatchResults.Groups.Count; i++)
                    {
                        System.Text.RegularExpressions.Group GroupObj = MatchResults.Groups[i];
                        if (GroupObj.Success)
                        {
                            options.Add(GroupObj.Value);
                        }
                    }
                    MatchResults = MatchResults.NextMatch();
                }
            }
            catch
            {
                // Syntax error in the regular expression
            }

            if (options.Count > 0) return String.Join(",", options.ToArray());
            else return String.Empty;
        }
        
        private DateTime ConvertDateTime(String strDateTime, String dateDescription)
        {
            DateTime tempDate = DateTime.MinValue;
            if (!String.IsNullOrEmpty(strDateTime))
            {
                try
                {                    
                    tempDate = DateTime.Parse(strDateTime);
                    tempDate = LocaleHelper.FromLocalTime(tempDate, this.TimeZoneOffset);
                }
                catch { LogError(GetPercentResult(), "Unable to parse " + dateDescription + ":" + strDateTime); }
            }
            return tempDate;
        }

        /// <summary>
        /// Backups the Original SMTP Settings of the store and removes the store settings 
        /// so that during the import process no emails are sent.
        /// </summary>
        private void DisableSmtpSettings()
        {
            if(SmtpSettings.DefaultSettings.Server != null)
            {
                _SmtpSettingsBackup = new SmtpSettings();
                _SmtpSettingsBackup.Server = SmtpSettings.DefaultSettings.Server;
                _SmtpSettingsBackup.Port = SmtpSettings.DefaultSettings.Port;
                _SmtpSettingsBackup.RequiresAuthentication = SmtpSettings.DefaultSettings.RequiresAuthentication;
                _SmtpSettingsBackup.UserName = SmtpSettings.DefaultSettings.UserName;
                _SmtpSettingsBackup.Password = SmtpSettings.DefaultSettings.Password;


                // CHANGE THE SETTINGS SO THAT NO EMAILS ARE SENT
                SmtpSettings.DefaultSettings.Server = String.Empty;
                Token.Instance.Store.Settings.SmtpServer = String.Empty;
            }            
        }

        private void RestoreSmtpSettings()
        {
            if (_SmtpSettingsBackup != null)
            {
                // RESTORE THE SETTINGS SO THAT STORE CAN WORK PROPERLY AGAIN
                SmtpSettings.DefaultSettings.Server = _SmtpSettingsBackup.Server;
                Token.Instance.Store.Settings.SmtpServer = _SmtpSettingsBackup.Server;
                Token.Instance.Store.Settings.Save();
            }
        }

        private string ConvertOptionList(string oldKitOptions, String logMessage)
        {
            String strNewList = String.Empty;
            String[] arrKitOptions;
            if (oldKitOptions.Length > 0)
            {
                arrKitOptions = oldKitOptions.Split(',');
                List<string> newOptionsList = new List<string>();
                for (int index = 0; index < arrKitOptions.Length; index++)
                {
                    //	'loop each option, translate to new id
                    int intOldOptionID = AlwaysConvert.ToInt(arrKitOptions[index]);
                    if (TranslationDictionary.ContainsKey("OPTION" + intOldOptionID))
                    {
                        newOptionsList.Add(TranslationDictionary["OPTION" + intOldOptionID].ToString());
                    }
                    else if (!String.IsNullOrEmpty(logMessage))
                    {
                        // OPTION CAN NOT BE TRANSLATED
                        LogError(GetPercentResult(), logMessage);
                    }
                }
                strNewList = String.Join(",", newOptionsList.ToArray());
            }
            return strNewList;
        }

        private string ConvertKitList(String lstOldKitIDs, string logMessage)
        {
            String lstNewKitIDs = String.Empty; 
            String[] arrKitIDs;
            int KitID = 0;
            if (lstOldKitIDs.Length > 0)
            {
                arrKitIDs = lstOldKitIDs.Split(',');
                List<String> kitList = new List<string>();
                //loop each option, translate to new id
                foreach (String OldKitProductID in arrKitIDs)
                {
                    if (TranslationDictionary.ContainsKey("KITPRODUCT" + OldKitProductID))
                    {
                        KitID = AlwaysConvert.ToInt(TranslationDictionary["KITPRODUCT" + OldKitProductID]); ;
                        if (KitID > 0) kitList.Add(KitID.ToString());
                    }
                    else if(!String.IsNullOrEmpty(logMessage))
                    {
                        LogWarning(GetPercentResult(), logMessage);
                    }                                                
                }
                if (kitList.Count > 0) lstNewKitIDs = String.Join(",", kitList.ToArray());
            }
            return lstNewKitIDs;
        }

        private LicenseAgreementMode ConvertLicenseAgreementMode(int oldMode)
        {
            // AC55 values
            //0 = Never
            //1 = On Add to Basket
            //2 = On Download
            //3 = Both
 	        switch(oldMode){
                case 0: return LicenseAgreementMode.Never;
                case 1: return LicenseAgreementMode.OnAddToBasket;
                case 2: return LicenseAgreementMode.OnDownload;
                case 3: return LicenseAgreementMode.Always; 
                default: return LicenseAgreementMode.Never;
            }
        }

        public static string UnzipFile(Stream inputStream)
        {
            StringBuilder fileContent = new StringBuilder();
            ZipInputStream s = null;
            try
            {
                s = new ZipInputStream(inputStream);
                ZipEntry firstEntry;
                if ((firstEntry = s.GetNextEntry()) != null)
                {
                    if (s.Length > 0)
                    {
                        StreamReader sr = new StreamReader(s);
                        fileContent.Append(sr.ReadToEnd());
                    }
                }
            }
            finally
            {
                if (s != null) s.Close();
            }
            return fileContent.ToString();
        }

        
        private KitInputType ConvertKitInputType(int oldInputType)
        {
            switch(oldInputType){
                case 0: return KitInputType.IncludedHidden; // Hidden (Required)
				case 1: return KitInputType.DropDown;       // Select Box
				case 2: return KitInputType.CheckBox;       //Multiple Select Box
				case 3: return KitInputType.CheckBox;       //Check Box
				case 4: return KitInputType.RadioButton;    //Radio

                default: return KitInputType.DropDown;
            }
        }
        private PaymentInstrument ConvertPaymentMethodType(int oldTypeNumber, int validatorId, String payMethodName)
        {
            // TODO: implementation pending
            switch (oldTypeNumber)
            {
                case 1:
                    //1	Credit Card
                    switch (validatorId)
                    {
                        case 1:
                            //Visa = 1,
                            return PaymentInstrument.Visa;
                        case 2:
                            //2 = MasterCard,
                            return PaymentInstrument.MasterCard;
                        case 3:
                            //3 = American Express
                            return PaymentInstrument.AmericanExpress;
                        case 4:
                            //4 = Discover
                            return PaymentInstrument.Discover;
                        default:
                            return PaymentInstrument.Unknown;
                    }
                case 2:
                    //2	Electronic Check
                    return PaymentInstrument.Check;
                case 3:
                    //3	Phone Order
                    return PaymentInstrument.PhoneCall;
                case 4:
                    //4	Fax/Mail Order
                    return PaymentInstrument.Mail;
                case 5:
                    //5	Purchase Order
                    return PaymentInstrument.PurchaseOrder;
                case 6:
                    //6	Generic
                    // DISCUSSED IN  BUG # 6661
                    if (payMethodName.ToLowerInvariant().Equals("paypal") || payMethodName.ToLowerInvariant().Equals("pay pal")) return PaymentInstrument.PayPal;
                    return PaymentInstrument.Unknown;
                case 7:
                    //7	Gift Certificate
                    return PaymentInstrument.GiftCertificate;
                default:
                    return PaymentInstrument.Unknown;
            }
            

            //AC55 PayMethod types
            //1	Credit Card
            //2	Electronic Check
            //3	Phone Order
            //4	Fax/Mail Order
            //5	Purchase Order
            //6	Generic
            //7	Gift Certificate
            
            // VALIDATORS
            //0 = None
            //3 = American Express
            //5 = Diner's Club / CarteBlanche
            //4 = Discover
            //6 = EnRoute
            //7 = JCB
            //2 = MasterCard
            //1 = VISA


            //AC7 Payment Instruments        
            //Unknown = 0,
            //Visa = 1,
            //MasterCard = 2,
            //Discover = 3,
            //AmericanExpress = 4,
            //JCB = 5,
            //Maestro = 6,
            //PayPal = 7,
            //PurchaseOrder = 8,
            //Check = 9,
            //Mail = 10,
            //GoogleCheckout = 11,
            //GiftCertificate = 12,
            //PhoneCall = 13,
            //SwitchSolo = 14,
            //VisaDebit = 15, 
            //DinersClub = 16
        }

        private CatalogVisibility ConvertCatalogNodeVisibility(int oldVisibilityValue)
        {
            switch (oldVisibilityValue)
            {
                case 0:     return CatalogVisibility.Private;
                case 1:     return CatalogVisibility.Public;                
                default:    return CatalogVisibility.Hidden;
            }
        }

        private InventoryAction ConvertInventoryAction(int oldValue)
        {
            // AC55 VALUES
            /** No inventory action taken */
            //None = 0;

            /**
             * Reserves stock for items in the order. Stock will not be adjusted for items that have
             * already been reserved or destocked.
             */
            //Reserve = 1;

            /**
             * Destocks items in the order. Stock will not be adjusted for items that have already been
             * destocked.
             */
            //Destock = 2;

            /**
             * Releases any reserve set for items in the order Stock will not be adjusted for items
             * that have not been reserved or have already been destocked.
             */
            //ReleaseReserve = 3;

            /**
             * Restocks any items in the order Stock will not be adjusted for items that have have not
             * been reserved or destocked
             */
            //Restock = 4;

            switch (oldValue)
            {
                case 0: return InventoryAction.None;    // AC5x None = 0;
                case 1: return InventoryAction.Destock;    // AC5x Reserve = 1;
                case 2: return InventoryAction.Destock; // AC5x Destock = 2;
                case 3: return InventoryAction.None;    // AC5x ReleaseReserve = 3;
                case 4: return InventoryAction.Restock; // AC5x Restock = 4;
                default: return InventoryAction.None;   
            }
        }
        

        /// <summary>
        /// Converts the int value to bool<br/>
        /// Converts 1 to true, while any other value to false
        /// Also the "True" or "true" will be converted to true
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private bool ToBool(string val)
        {
            bool retValue = false;
            if (!string.IsNullOrEmpty(val))
            {
                if (val.Equals("True") || val.Equals("true") || val.Equals("1"))
                {
                    retValue = true;
                }
            }
            return retValue;
        }

        /// <summary>
        /// Converts the int value to bool<br/>
        /// Converts 1 to true, while any other value to false
        /// Also the "True" or "true" will be converted to true
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private bool ToBool(string val, bool defaultValue)
        {
            bool retValue = false;
            if (!string.IsNullOrEmpty(val))
            {
                if (val.Equals("True") || val.Equals("true") || val.Equals("1"))
                {
                    retValue = true;
                }
                else
                {
                    retValue = AlwaysConvert.ToBool(val, defaultValue);
                }
            }
            return retValue;
        }

        private StringDictionary DicTaxCode
        {
            get
            {
                if (dicTaxCodes == null)
                {
                    dicTaxCodes = new StringDictionary();

                    PersistentCollection<TaxCode> colTaxCode = TaxCodeDataSource.LoadForStore();
                    if (colTaxCode.Count > 0)
                    {
                        foreach (TaxCode objTax in colTaxCode)
                        {
                            if (objTax.Name.Length > 0)
                            {
                                if (!(dicTaxCodes.ContainsKey(objTax.Name)))
                                {
                                    dicTaxCodes.Add(objTax.Name, objTax.TaxCodeId.ToString());
                                }
                            }
                        }
                    }
                }
                return dicTaxCodes;
            }
        }

        private StringDictionary DicManufacturers
        {
            get
            {
                if (dicManufacturers == null)
                {
                    dicManufacturers = new StringDictionary();
                    //initialize with the already defined Manufacturers
                    ManufacturerCollection manufacturerCollection = ManufacturerDataSource.LoadForStore();
                    foreach (Manufacturer m in manufacturerCollection)
                    {
                        if (!dicManufacturers.ContainsKey(m.Name))
                        {
                            dicManufacturers.Add(m.Name, m.ManufacturerId.ToString());
                        }
                    }
                }
                return dicManufacturers;
            }
        }

        private SortedDictionary<int, StoreEvent> StoreEventDictionary
        {
            get
            {

                if (eventsDictionary == null)
                {
                    eventsDictionary = new SortedDictionary<int, StoreEvent>();
                    eventsDictionary.Add(-1, StoreEvent.None);
                    // None
                    eventsDictionary.Add(0, StoreEvent.None);
                    //ORDER_PLACED
                    eventsDictionary.Add(1, StoreEvent.OrderPlaced);
                    //ORDER_PLACED_PAID_FULL = 2;                            PaymentCaptured || OrderPlaced
                    eventsDictionary.Add(2, StoreEvent.OrderPlaced);
                    //int PAYMENT_RECEIVED_FULL = 3;                            PaymentCaptured
                    eventsDictionary.Add(3, StoreEvent.PaymentCaptured);
                    //int PAYMENT_RECEIVED_PARTIAL = 4;                    PaymentCapturedPartial
                    eventsDictionary.Add(4, StoreEvent.PaymentCapturedPartial);
                    //int ORDER_ITEM_ADDED = 5;
                    eventsDictionary.Add(5, StoreEvent.None);
                    //int ORDER_ITEM_REMOVED = 6;
                    eventsDictionary.Add(6, StoreEvent.None);
                    //int ORDER_ITEM_CHANGED = 7;
                    eventsDictionary.Add(7, StoreEvent.None);
                    //int ORDER_ITEM_MOVED = 8;
                    eventsDictionary.Add(8, StoreEvent.None);
                    //int ORDER_ITEM_RETURNED = 9;
                    eventsDictionary.Add(9, StoreEvent.None);
                    //int SHIPMENT_CREATED = 10;
                    eventsDictionary.Add(10, StoreEvent.None);
                    //int ORDER_SHIPPED_FULL = 11; OrderShipped
                    eventsDictionary.Add(11, StoreEvent.OrderShipped);
                    //int ORDER_SHIPPED_PARTIAL = 12;   OrderShippedPartial
                    eventsDictionary.Add(12, StoreEvent.OrderShippedPartial);
                    //int TRACKING_NUMBER_ADDED = 13;
                    eventsDictionary.Add(13, StoreEvent.None);
                    //int ORDER_NOTE_ADDED = 14;    OrderNoteAddedByMerchant || OrderNoteAddedByCustomer,
                    eventsDictionary.Add(14, StoreEvent.OrderNoteAddedByMerchant);
                    //int ORDER_FILE_ADDED = 15;
                    eventsDictionary.Add(15, StoreEvent.None);
                    //int ORDER_FILE_VALIDATED = 16;
                    eventsDictionary.Add(16, StoreEvent.None);
                    //int ORDER_FILE_FULFILLED = 17;
                    eventsDictionary.Add(17, StoreEvent.None);
                    //int ORDER_STATUS_UPDATED = 18;    OrderStatusUpdated
                    eventsDictionary.Add(18, StoreEvent.OrderStatusUpdated);
                    //int LOW_INVENTORY_ITEM_PURCHASED = 19;
                    eventsDictionary.Add(19, StoreEvent.None);
                    //int LOW_INVENTORY_ITEM_SHIPPED = 20;
                    eventsDictionary.Add(20, StoreEvent.None);
                    //int VENDOR_ITEM_PURCHASED = 21;
                    eventsDictionary.Add(21, StoreEvent.None);
                    //int CUSTOMER_PASSWORD_REQUEST = 22;
                    eventsDictionary.Add(22, StoreEvent.None);
                    //int ORDER_PLACED_PAID_PARTIAL = 23;   OrderPaidPartial || OrderPlaced
                    eventsDictionary.Add(23, StoreEvent.OrderPaidPartial);
                    //int ORDER_NEGATIVE_BALANCE = 24;
                    eventsDictionary.Add(24, StoreEvent.None);
                    //int ORDER_POSITIVE_BALANCE = 25;
                    eventsDictionary.Add(25, StoreEvent.None);
                    //int ORDER_ZERO_BALANCE = 26;
                    eventsDictionary.Add(26, StoreEvent.None);
                }
                return eventsDictionary;
            }
        }

        private int GetZoneIDForCountryList(String Name, String CountryList)
        {
            if (DicCountryZone.ContainsKey(CountryList))
            {
                return AlwaysConvert.ToInt(DicCountryZone[CountryList]);
            }
            else
            {
                ShipZoneCollection ZoneCollection = ShipZoneDataSource.LoadForStore();
                ShipZone shipZone = new ShipZone();
                shipZone.Name = "Imported " + Name + " Zone";                
                shipZone.Save();

                // IF THERE ARE COUNTRY CODES IN LIST
                if (!String.IsNullOrEmpty(CountryList) && !CountryList.Equals("-1"))
                {
                    String[] CountryArr = CountryList.Split(',');
                    foreach (String CC in CountryArr)
                    {
                        // SKIP INVALID COUNTRY CODES
                        if (String.IsNullOrEmpty(CC)) continue;

                        // CATCH COUNTRY CODE PROBLEM, AC7 MAY HAVE SOME MISSING COUNTRY CODES
                        try
                        {
                            Country country = CountryDataSource.Load(CC);
                            if (country == null)
                            {
                                // CREATE A NEW COUNTRY FOR THIS MISSING CC
                                country = new Country(CC);
                                country.StoreId = Token.Instance.StoreId;
                                country.Save();
                            }
                            ShipZoneCountry szc = new ShipZoneCountry(shipZone.ShipZoneId, CC);
                            szc.Save();
                            shipZone.ShipZoneCountries.Add(szc);
                        }
                        catch (Exception ex)
                        {
                            LogError(GetPercentResult(), "Error Creating ShipZoneCountry for country code \"" + CC + "\"\n" + ex.Message + "\n" + ex.StackTrace);
                        }
                    }
                    shipZone.ShipZoneCountries.Save();
                    shipZone.CountryRule = FilterRule.IncludeSelected;
                    shipZone.Save();
                }
                ZoneCollection.Add(shipZone);
                ZoneCollection.Save();
                DicCountryZone.Add(CountryList, shipZone.ShipZoneId.ToString());
                return shipZone.ShipZoneId;
            }
        }

        private int GetZoneIDForTaxRuleData(String name, String countryCode, String provinceName, String postalCode)
        {
            String taxRuleZoneKey = countryCode + ":" + provinceName + ":" + postalCode;
            if (DicTaxZone.ContainsKey(countryCode))
            {
                return AlwaysConvert.ToInt(DicTaxZone[taxRuleZoneKey]);
            }
            else
            {
                ShipZoneCollection ZoneCollection = ShipZoneDataSource.LoadForStore();
                ShipZone shipZone = new ShipZone();
                shipZone.Name = "Imported " + name + " Zone";
                shipZone.Save();

                // IF THERE ARE COUNTRY CODES IN LIST
                if (!String.IsNullOrEmpty(countryCode) && !countryCode.Equals("-1"))
                {                   

                    // CATCH COUNTRY CODE PROBLEM, AC7 MAY HAVE SOME MISSING COUNTRY CODES
                    try
                    {
                        Country country = CountryDataSource.Load(countryCode);
                        if (country == null)
                        {
                            // CREATE A NEW COUNTRY FOR THIS MISSING CC
                            country = new Country(countryCode);
                            country.StoreId = Token.Instance.StoreId;
                            country.Save();
                        }
                        ShipZoneCountry szc = new ShipZoneCountry(shipZone.ShipZoneId, countryCode);
                        szc.Save();
                        shipZone.ShipZoneCountries.Add(szc);
                    }
                    catch (Exception ex)
                    {
                        LogError(GetPercentResult(), "Error Creating ShipZoneCountry for country code \"" + countryCode + "\"\n" + ex.Message + "\n" + ex.StackTrace);
                    }
                    shipZone.ShipZoneCountries.Save();
                    shipZone.CountryRule = FilterRule.IncludeSelected;

                    if (!String.IsNullOrEmpty(provinceName))
                    {
                        // CATCH PROVICE CODE PROBLEM, AC7 MAY HAVE SOME MISSING PROVINCE CODES
                        try
                        {
                            int provinceId = ProvinceDataSource.GetProvinceIdByName(countryCode, provinceName);
                            Province province = ProvinceDataSource.Load(provinceId);
                            if (province == null)
                            {
                                // CREATE A NEW PROVINCE FOR THIS MISSING PROVINCE
                                province = new Province();
                                province.Name = provinceName;
                                province.CountryCode = countryCode;  
                                province.Save();
                            }
                            ShipZoneProvince szp = new ShipZoneProvince(shipZone.ShipZoneId, province.ProvinceId);
                            szp.Save();
                            shipZone.ShipZoneProvinces.Add(szp);
                        }
                        catch (Exception ex)
                        {
                            LogError(GetPercentResult(), "Error Creating ShipZoneProvice for province code \"" + provinceName + "\"\n" + ex.Message + "\n" + ex.StackTrace);
                        }
                        shipZone.ShipZoneProvinces.Save();
                        shipZone.ProvinceRule = FilterRule.IncludeSelected;
                    }

                    if (!String.IsNullOrEmpty(postalCode)) shipZone.PostalCodeFilter = postalCode;
                    
                    shipZone.Save();
                }
                ZoneCollection.Add(shipZone);
                ZoneCollection.Save();
                DicTaxZone.Add(taxRuleZoneKey, shipZone.ShipZoneId.ToString());
                return shipZone.ShipZoneId;
            }
        }

        private StringDictionary DicCountryZone
        {
            get
            {
                if (dicCountryZone == null)
                {
                    dicCountryZone = new StringDictionary();
                    ShipZoneCollection ZoneCollection = ShipZoneDataSource.LoadForStore();
                    foreach (ShipZone zone in ZoneCollection)
                    {
                        String CountryList = String.Empty;
                        ShipZoneCountryCollection ShipZoneCountryCol = zone.ShipZoneCountries;
                        foreach (ShipZoneCountry ShipZoneCountry in ShipZoneCountryCol)
                        {
                            CountryList += ShipZoneCountry.CountryCode + ",";
                        }
                        int index = CountryList.LastIndexOf(',');
                        if (index > 0)
                        {
                            CountryList = CountryList.Substring(0, index);
                        }
                        if (!dicCountryZone.ContainsKey(CountryList))
                        {
                            dicCountryZone.Add(CountryList, zone.ShipZoneId.ToString());
                        }
                    }
                }
                return dicCountryZone;
            }
        }

        private StringDictionary DicTaxZone
        {
            get
            {
                if (dicTaxZone == null)
                {
                    dicTaxZone = new StringDictionary();
                    ShipZoneCollection ZoneCollection = ShipZoneDataSource.LoadForStore();
                    foreach (ShipZone zone in ZoneCollection)
                    {
                        // AS IN AC55 WE ONLY SUPPORT ONE COUNTRY, ONE PROVINCE ( OR ALL PROVINCES), ONE ZIP CODE PER TAX RULE
                        // SO, ONLY INCLUDE VALID ZONES IN LIST
                        if (zone.ShipZoneCountries.Count == 1 && (zone.ProvinceRule == FilterRule.All || zone.ShipZoneProvinces.Count == 1))
                        {
                            //KEY FORMAT "COUNTRY_CODE:PROVINCE_CODE:ZIP_CODE"
                            String taxRuleZoneKey = zone.ShipZoneCountries[0].CountryCode;
                            taxRuleZoneKey += ":";

                            taxRuleZoneKey += (zone.ProvinceRule == FilterRule.All? "":zone.ShipZoneProvinces[0].Province.ProvinceCode);
                            taxRuleZoneKey += ":";

                            taxRuleZoneKey += zone.PostalCodeFilter;
                            
                            if (!dicTaxZone.ContainsKey(taxRuleZoneKey))
                            {
                                dicTaxZone.Add(taxRuleZoneKey, zone.ShipZoneId.ToString());
                            }
                        }
                    }
                }
                return dicTaxZone;
            }
        }

        private ShipMethodType TranslateShipMethodType(int MethodType)
        {
            switch (MethodType)
            {
                case 0:
                    return ShipMethodType.FlatRate;
                case 1:
                    return ShipMethodType.WeightBased;
                case 2:
                    return ShipMethodType.CostBased;
                case 3:
                    return ShipMethodType.QuantityBased;
                case 4:
                    return ShipMethodType.IntegratedProvider;
                default:
                    return ShipMethodType.FlatRate;
            }
        }

        private CouponType TranslateCouponType(int coupontype)
        {
            switch (coupontype)
            {
                case 0:
                    //AC55 0 = Order
                    return CouponType.Order;
                case 1:
                    //AC55 1 = Product
                    return CouponType.Product;
                case 2:
                    return CouponType.Shipping;
                default:
                    return CouponType.Order;
            }
        }

        private CouponRule TranslateProductRule(int ProductRule)
        {
            switch (ProductRule)
            {
                case 0:
                    return CouponRule.All;
                case 1:
                    return CouponRule.ExcludeSelected;
                case 2:
                    return CouponRule.AllowSelected;
                //TODO Confirm
                default:
                    return CouponRule.ExcludeSelected;
            }
        }

        //private bool TranslateCouponAllowCombine(int ComboRule)
        //{
        //    if (ComboRule == 0) return false;
        //    else 
        //    switch (ComboRule)
        //    {

        //        case 0:
        //            return CouponRule.All;
        //        case 1:
        //            return CouponRule.ExcludeSelected;
        //        case 2:
        //            return CouponRule.AllowSelected;
        //        default:
        //            return CouponRule.AllowSelected;
        //    }
        //}

        private WeightUnit ParseWeithUnit(string sOldWeightUnit)
        {

            if (sOldWeightUnit == null || sOldWeightUnit.Length == 0)
            {
                return Token.Instance.Store.WeightUnit;
            }

            switch (sOldWeightUnit)
            {
                case "lb":
                    return WeightUnit.Pounds;
                case "gm":
                    return WeightUnit.Grams;
                case "kg":
                    return WeightUnit.Kilograms;
                case "oz":
                    return WeightUnit.Ounces;
                default:
                    return Token.Instance.Store.WeightUnit;
            }
        }

        private string GetAltText(string str)
        {
            Match Result = Regex.Match(str, "Alt=\"(?<AltText>[^\"]+)");
            return Result.Groups["AltText"].Value.ToString();
        }

        private string FormatImgURL(string url)
        {
            if(String.IsNullOrEmpty(url)) return String.Empty;
            if (url.ToLowerInvariant().StartsWith("http:") || url.ToLowerInvariant().StartsWith("https:"))
            {
                return url;
            }
            return "~/Assets/" + url;
        }

        #endregion

        #region Import Status Related methods


        /// <summary>
        /// Will be used only inside private import messages
        /// Where we are importing only for one phase
        /// It is used to calculate the total percentage (importStatus.Percent + currentPhasePercent)
        /// </summary>
        /// <returns></returns>
        private int GetPercentResult()
        {
            return (overAllPercent + currentPhasePercent);
        }

        private int CalculatePartialPercentage(int completed, int total)
        {
            // calculate this pahse is completed for what percent
            Decimal pp = ((Decimal)completed / (Decimal)total * 100);
            // calculate with respect to overall weightage
            int overAllPercentage = (int)((Decimal)currentPhaseWeightage * pp / 100);
            return overAllPercentage;
        }

        private void LogStatus(int percent, string message)
        {
            importStatus.Percent = percent; //(statusGUID + "_Percent", "" + GetPercentResult(), null, DateTime.UtcNow.AddHours(1), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Normal, null);
            importStatus.Message = message;
            importStatus.LogMessage.AppendLine(percent + "% : " + message );
        }

        private void LogWarning(int percent, string message)
        {
            importStatus.Percent = percent;
            importStatus.Message = message;
            importStatus.LogWarning.AppendLine();
            importStatus.LogWarning.AppendLine("Warning: " + message);
        }
        

        private void LogError(int percent, string message)
        {
            Logger.Debug("An error occurred while importing AC5x data: " + message);
            importStatus.Percent = percent;
            importStatus.Message = message;

            // DONT LOG COMPLETE EXCEPTION STACK TRACE IN STATUS TO REDUCE SIZE
            String subMessage = String.Empty;

            importStatus.LogError.AppendLine();
            if (message.Length > 255)
            {
                subMessage = message.Substring(0, 255) + "(for more details check server log)...";
                importStatus.LogError.AppendLine("Error: " + subMessage);
            }
            else importStatus.LogError.AppendLine("Error: " + message);            
        }

        public ImportStatus Status
        {
            get { return importStatus; }
            set
            {
                importStatus = value;
                overAllPercent = importStatus.Percent;
            }
        }

        public class ImportStatus
        {
            StringBuilder logMessage = new StringBuilder("");
            StringBuilder logWarnings = new StringBuilder();
            StringBuilder logErrors = new StringBuilder();

            string message = "";            
            int percent = 0;

            public string Message
            {
                get { return message; }
                set { message = value; }
            }

            public int Percent
            {
                get { return percent; }
                set { percent = value; }
            }

            public StringBuilder LogMessage
            {
                get { return logMessage; }
                set { logMessage = value; }
            }

            public StringBuilder LogWarning
            {
                get { return logWarnings; }
                set { logWarnings = value; }
            }

            public StringBuilder LogError
            {
                get { return logErrors; }
                set { logErrors = value; }
            }            
        }

        #endregion

    }
}