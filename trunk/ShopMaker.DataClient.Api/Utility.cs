using System;
using System.Collections.Generic;
using System.Text;
//using MakerShop.Data;
using System.Reflection;
using System.IO;
using System.Collections.ObjectModel;
using MakerShop.Products;
using MakerShop.Users;
using MakerShop.Shipping;
using MakerShop.Taxes;
using MakerShop.Stores;
using MakerShop.DataClient.Api;
using MakerShop.Orders;
using System.Collections.Specialized;
using MakerShop.Catalog;
using MakerShop.Payments;
using MakerShop.Marketing;
using MakerShop.Utility;
using MakerShop.Personalization;
using MakerShop.Common;



namespace MakerShop.DataClient.Api
{
    public class Utility
    {

        /*public static String EncodeForCSV(String input, String textQualifier)
        {
            if (input == null) return String.Empty;
            string sOutput = input;
            if (sOutput.IndexOfAny("\",\x0A\x0D".ToCharArray()) > -1)
            {
                sOutput = sOutput.Replace("\"", "\"\"");
            }
            sOutput = "\"" + sOutput + "\"";

            return sOutput;
        }*/

        public static String RemoveEndSeparator(String str, String Seperator)
        {
            if (String.IsNullOrEmpty(str)) return str;
            int index = str.LastIndexOf(Seperator);
            if (index > 0 && str.EndsWith(Seperator))
            {
                str = str.Substring(0, index);
            }
            return str;
        }

       
        public static void WriteSchema()
        {
            StringBuilder srtSchemaobejcts = new StringBuilder();

            List<KeyValuePair<System.Type, string>> types = new List<KeyValuePair<System.Type, string>>();

            // ********** START CHANGED TYPES FROM 7.0 TO 7.1
            types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Taxes.TaxRuleGroup), "TaxRuleGroup"));
            types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Taxes.TaxRuleShipZone), "TaxRuleShipZone"));
            types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Payments.Transaction), "Transaction"));
            types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Taxes.TaxRule), "TaxRule"));
            types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Shipping.ShipZone), "ShipZone"));
            types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Shipping.ShipZoneProvince), "ShipZoneProvince"));
            types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Orders.Order), "Order"));
            types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Orders.OrderItem), "OrderItem"));
            types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Orders.BasketItem), "BasketItem"));
            // ********** END CHANGED TYPES FROM 7.0 TO 7.1

            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Stores.AuditEvent), "AuditEvent"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Orders.BasketCoupon), "BasketCoupon"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Orders.BasketItemInput), "BasketItemInput"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Orders.BasketItemKitProduct), "BasketItemKitProduct"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Orders.BasketItem), "BasketItem"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Orders.Basket), "Basket"));

            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Orders.BasketShipment), "BasketShipment"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Catalog.CatalogNode), "CatalogNode"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Catalog.CategoryParent), "CategoryParent"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Marketing.CouponCombo), "CouponCombo"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Marketing.CouponGroup), "CouponGroup"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Marketing.CouponProduct), "CouponProduct"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Marketing.CouponShipMethod), "CouponShipMethod"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Shipping.DhlWarehouse), "DhlWarehouse"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.DigitalDelivery.Download), "Download"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Utility.ErrorMessage), "ErrorMessage"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Users.GroupRole), "GroupRole"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.InputChoice), "InputChoice"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Utility.IPLocation), "IPLocation"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.KitComponent), "KitComponent"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.KitProduct), "KitProduct"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Orders.OrderCoupon), "OrderCoupon"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Orders.OrderStatusAction), "OrderStatusAction"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Orders.OrderStatusEmail), "OrderStatusEmail"));

            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Orders.OrderStatusTrigger), "OrderStatusTrigger"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Reporting.PageView), "PageView"));

            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Payments.PaymentMethodGroup), "PaymentMethodGroup"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Personalization.PersonalizationPath), "PersonalizationPath"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.ProductImage), "ProductImage"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Personalization.Profile), "Profile"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.DigitalDelivery.Readme), "Readme"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.RelatedProduct), "RelatedProduct"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Users.Role), "Role"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Personalization.SharedPersonalization), "SharedPersonalization"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Shipping.ShipGateway), "ShipGateway"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Shipping.ShipMethodGroup), "ShipMethodGroup"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Shipping.ShipMethodWarehouse), "ShipMethodWarehouse"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Shipping.ShipRateMatrix), "ShipRateMatrix"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.SpecialGroup), "SpecialGroup"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.SubscriptionPlan), "SubscriptionPlan"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Orders.Subscription), "Subscription"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Taxes.TaxGateway), "TaxGateway"));

            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.UpsellProduct), "UpsellProduct"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Shipping.UpsOrigin), "UpsOrigin"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Users.UserGroup), "UserGroup"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Personalization.UserPersonalization), "UserPersonalization"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.VendorGroup), "VendorGroup"));            
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Marketing.VolumeDiscountGroup), "VolumeDiscountGroup"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Users.WishlistItemKitProduct), "WishlistItemKitProduct"));            

            //// EXISTING OBJECTS
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Users.Address), "Address"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Marketing.Affiliate), "Affiliate"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Stores.BannedIP), "BannedIP"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Catalog.Category), "Category"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Shipping.Country), "Country"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Marketing.Coupon), "Coupon"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Stores.Currency), "Currency"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Stores.CustomField), "CustomField"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.DigitalDelivery.DigitalGood), "DigitalGood"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Marketing.EmailList), "EmailList"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Marketing.EmailListSignup), "EmailListSignup"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Marketing.EmailListUser), "EmailListUser"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Messaging.EmailTemplate), "EmailTemplate"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Messaging.EmailTemplateTrigger), "EmailTemplateTrigger"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Payments.GiftCertificate), "GiftCertificate"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Payments.GiftCertificateTransaction), "GiftCertificateTransaction"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Users.Group), "Group"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.InputField), "InputField"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Catalog.Link), "Link"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.Manufacturer), "Manufacturer"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.Option), "Option"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.OptionChoice), "OptionChoice"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Orders.Order), "Order"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Orders.OrderItem), "OrderItem"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Orders.OrderItemInput), "OrderItemInput"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Orders.OrderNote), "OrderNote"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Orders.OrderShipment), "OrderShipment"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Orders.OrderStatus), "OrderStatus"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Payments.Payment), "Payment"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Payments.PaymentGateway), "PaymentGateway"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Payments.PaymentMethod), "PaymentMethod"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.Product), "Product"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.ProductAsset), "ProductAsset"));            
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.ProductCustomField), "ProductCustomField"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.ProductOption), "ProductOption"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.ProductReview), "ProductReview"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.ProductTemplate), "ProductTemplate"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.ProductTemplateField), "ProductTemplateField"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.ProductVariant), "ProductVariant"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Marketing.ProductVolumeDiscount), "ProductVolumeDiscount"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Shipping.Province), "Province"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.ReviewerProfile), "ReviewerProfile"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Shipping.ShipMethod), "ShipMethod"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Shipping.ShipMethodShipZone), "ShipMethodShipZone"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Shipping.ShipZone), "ShipZone"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Shipping.ShipZoneCountry), "ShipZoneCountry"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Shipping.ShipZoneProvince), "ShipZoneProvince"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.Special), "Special"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Stores.Store), "Store"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Stores.StoreSetting), "StoreSetting"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Taxes.TaxCode), "TaxCode"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Taxes.TaxRule), "TaxRule"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Taxes.TaxRuleTaxCode), "TaxRuleTaxCode"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Orders.TrackingNumber), "TrackingNumber"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Payments.Transaction), "Transaction"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Users.User), "User"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Users.UserPassword), "UserPassword"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Users.UserSetting), "UserSetting"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.Vendor), "Vendor"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Marketing.VolumeDiscount), "VolumeDiscount"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Marketing.VolumeDiscountLevel), "VolumeDiscountLevel"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Shipping.Warehouse), "Warehouse"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Catalog.Webpage), "Webpage"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Users.Wishlist), "Wishlist"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Users.WishlistItem), "WishlistItem"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Users.WishlistItemInput), "WishlistItemInput"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.WrapGroup), "WrapGroup"));
            //types.Add(new System.Collections.Generic.KeyValuePair<System.Type, string>(typeof(MakerShop.Products.WrapStyle), "WrapStyle"));

            // END EXISTING OBJECTS


            foreach (KeyValuePair<Type, String> pair in types)
            {
                srtSchemaobejcts.Append(DataObject.GetObjectSchema(pair.Key, pair.Value) + "\n");
            }
            try
            {
                TextWriter objTextWriter = new StreamWriter(@"E:\Schema.txt");
                objTextWriter.Write(srtSchemaobejcts.ToString());
                objTextWriter.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogDebug(ex.Message);
            }
            
        }

        public static void LogDebug(String message)
        {
            //if (message.Length > 255) message = message.Substring(0, 254);
            //if (AppConfig.IsDebugMode)
            //{
                Logger.Debug("DataPort Import:\n" + message);
            //}
        }

        public static void LogDebug(String message,Exception ex)
        {
            //if (AppConfig.IsDebugMode)
            //{
                Logger.Debug("DataPort Import Error:\n" + message, ex);
            //}
        }

        

        public static List<KeyValuePair<string, System.Type>> ObjectsDicInImportOrder()
        {

            List<KeyValuePair<string, System.Type>> schemaTypes = new List<KeyValuePair<string, System.Type>>();

            schemaTypes.Add(new KeyValuePair<string, System.Type>("Store", typeof(Schema.Store)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("BannedIP", typeof(Schema.BannedIP)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("Currency", typeof(Schema.Currency)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("Group", typeof(Schema.Group)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("VolumeDiscount", typeof(Schema.VolumeDiscount)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("VolumeDiscountGroup", typeof(Schema.VolumeDiscountGroup)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("VolumeDiscountLevel", typeof(Schema.VolumeDiscountLevel)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("TaxCode", typeof(Schema.TaxCode)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("User", typeof(Schema.User)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("UserPassword", typeof(Schema.UserPassword)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("UserSetting", typeof(Schema.UserSetting)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("Wishlist", typeof(Schema.Wishlist)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("Address", typeof(Schema.Address)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("AuditEvent", typeof(Schema.AuditEvent)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("Basket", typeof(Schema.Basket)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("UserPersonalization", typeof(Schema.UserPersonalization)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("SharedPersonalization", typeof(Schema.SharedPersonalization)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("Profile", typeof(Schema.Profile)));

            schemaTypes.Add(new KeyValuePair<string, System.Type>("Country", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("Province", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("TaxRule", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("TaxRuleTaxCode", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("Affiliate", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("IPLocation", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("Warehouse", typeof(Schema.Profile)));
            //schemaTypes.Add(new KeyValuePair<string, System.Type>("DhlWarehouse", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("UpsOrigin", typeof(Schema.Profile)));

            schemaTypes.Add(new KeyValuePair<string, System.Type>("Coupon", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("BasketCoupon", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("CouponCombo", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("CouponGroup", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("CustomField", typeof(Schema.Profile)));

            schemaTypes.Add(new KeyValuePair<string, System.Type>("Readme", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("DigitalGood", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("SerialKey", typeof(Schema.Profile)));

            schemaTypes.Add(new KeyValuePair<string, System.Type>("EmailList", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("EmailListSignup", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("EmailListUser", typeof(Schema.Profile)));

            schemaTypes.Add(new KeyValuePair<string, System.Type>("EmailTemplate", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("OrderStatusEmail", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("EmailTemplateTrigger", typeof(Schema.Profile)));

            schemaTypes.Add(new KeyValuePair<string, System.Type>("ErrorMessage", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("Role", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("GroupRole", typeof(Schema.Profile)));

            schemaTypes.Add(new KeyValuePair<string, System.Type>("KitComponent", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("LicenseAgreement", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("Manufacturer", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("Option", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("OptionChoice", typeof(Schema.Profile)));

            schemaTypes.Add(new KeyValuePair<string, System.Type>("PersonalizationPath", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("Vendor", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("VendorGroup", typeof(Schema.Profile)));

            schemaTypes.Add(new KeyValuePair<string, System.Type>("ProductTemplate", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("InputField", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("InputChoice", typeof(Schema.Profile)));

            schemaTypes.Add(new KeyValuePair<string, System.Type>("WrapGroup", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("WrapStyle", typeof(Schema.Profile)));

            schemaTypes.Add(new KeyValuePair<string, System.Type>("Category", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("Link", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("Product", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("ProductTemplateField", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("ProductVariant", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("WishlistItem", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("WishlistItemInput", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("ProductDigitalGood", typeof(Schema.Profile)));

            schemaTypes.Add(new KeyValuePair<string, System.Type>("ProductKitComponent", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("ProductVolumeDiscount", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("RelatedProduct", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("Special", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("SpecialGroup", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("SubscriptionPlan", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("UpsellProduct", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("BasketItem", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("BasketItemInput", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("KitProduct", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("BasketItemKitProduct", typeof(Schema.Profile)));
            //schemaTypes.Add(new KeyValuePair<string, System.Type>("WishlistItemKitProduct", typeof(Schema.Profile)));

            schemaTypes.Add(new KeyValuePair<string, System.Type>("CouponProduct", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("ProductOption", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("ProductReview", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("ProductAsset", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("ProductCustomField", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("ProductImage", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("Webpage", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("CatalogNode", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("PageView", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("CategoryParent", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("CategoryVolumeDiscount", typeof(Schema.Profile)));

            schemaTypes.Add(new KeyValuePair<string, System.Type>("OrderStatus", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("OrderStatusAction", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("Order", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("OrderShipment", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("OrderItem", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("OrderItemDigitalGood", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("Download", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("GiftCertificate", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("GiftCertificateTransaction", typeof(Schema.Profile)));

            schemaTypes.Add(new KeyValuePair<string, System.Type>("OrderItemInput", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("OrderNotse", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("OrderCoupon", typeof(Schema.Profile)));

            schemaTypes.Add(new KeyValuePair<string, System.Type>("OrderStatusTrigger", typeof(Schema.Profile)));


            schemaTypes.Add(new KeyValuePair<string, System.Type>("ReviewerProfile", typeof(Schema.Profile)));

            schemaTypes.Add(new KeyValuePair<string, System.Type>("PaymentGateway", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("PaymentMethod", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("Payment", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("Transaction", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("Subscription", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("UserGroup", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("PaymentMethodGroup", typeof(Schema.Profile)));

            schemaTypes.Add(new KeyValuePair<string, System.Type>("ShipGateway", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("ShipMethod", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("ShipMethodWarehouse", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("ShipMethodGroup", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("ShipRateMatrix", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("BasketShipment", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("CouponShipMethod", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("TrackingNumber", typeof(Schema.Profile)));

            schemaTypes.Add(new KeyValuePair<string, System.Type>("ShipZone", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("ShipZoneCountrie", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("ShipZoneProvince", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("ShipMethodShipZone", typeof(Schema.Profile)));

            schemaTypes.Add(new KeyValuePair<string, System.Type>("StoreSetting", typeof(Schema.Profile)));
            schemaTypes.Add(new KeyValuePair<string, System.Type>("TaxGateway", typeof(Schema.Profile)));

            return schemaTypes;
        }

        public static UrlEncodedDictionary ParseToUrlEncodedDictionary(string value)
        {
            UrlEncodedDictionary urlEncodedDictionary = new UrlEncodedDictionary();
            if (!string.IsNullOrEmpty(value))
            {
                string[] pairs = value.Split("&".ToCharArray());
                foreach (string thisPair in pairs)
                {
                    if (!string.IsNullOrEmpty(thisPair) && thisPair.Contains("="))
                    {
                        string[] keyValue = thisPair.Split("=".ToCharArray());
                        urlEncodedDictionary[keyValue[0]] = System.Web.HttpUtility.UrlDecode(keyValue[1]);
                    }
                }
            }
            return urlEncodedDictionary;
        }
    }
}
