using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.DataClient.Api.Schema
{
    public partial class Product
    {
        public static string[] GetCSVEnabledFields(string EmptyString)
        {
            List<string> columnNames = new List<string>();
            columnNames.Add("ProductId");
            columnNames.Add("StoreId");
            columnNames.Add("Name");
            columnNames.Add("Price");
            columnNames.Add("CostOfGoods");
            columnNames.Add("MSRP");
            columnNames.Add("Weight");
            columnNames.Add("Length");
            columnNames.Add("Width");
            columnNames.Add("Height");
            //columnNames.Add("ManufacturerId");
            columnNames.Add("Manufacturer");
            columnNames.Add("Sku");
            columnNames.Add("ModelNumber");
            columnNames.Add("DisplayPage");
            //columnNames.Add("TaxCodeId");
            columnNames.Add("TaxCode");
            //columnNames.Add("ShippableId");
            columnNames.Add("Shippable");
            columnNames.Add("WarehouseId");
            columnNames.Add("Warehouse");
            columnNames.Add("InventoryModeId");
            columnNames.Add("InStock");
            columnNames.Add("InStockWarningLevel");
            columnNames.Add("ThumbnailUrl");
            columnNames.Add("ThumbnailAltText");
            columnNames.Add("ImageUrl");
            columnNames.Add("ImageAltText");
            columnNames.Add("Summary");
            columnNames.Add("Description");
            columnNames.Add("ExtendedDescription");
            //columnNames.Add("VendorId");
            columnNames.Add("Vendor");
            columnNames.Add("CreatedDate");
            columnNames.Add("LastModifiedDate");
            //columnNames.Add("ProductTemplateId");
            columnNames.Add("ProductTemplate");
            columnNames.Add("IsFeatured");
            columnNames.Add("IsProhibited");
            columnNames.Add("AllowReviews");
            columnNames.Add("AllowBackorder");
            //columnNames.Add("WrapGroupId");
            columnNames.Add("WrapGroup");
            columnNames.Add("ExcludeFromFeed");
            columnNames.Add("HtmlHead");
            columnNames.Add("DisablePurchase");
            columnNames.Add("MinQuantity");
            columnNames.Add("MaxQuantity");
            columnNames.Add("VisibilityId");
            columnNames.Add("Theme");
            columnNames.Add("IconUrl");
            columnNames.Add("IconAltText");
            columnNames.Add("IsGiftCertificate");
            columnNames.Add("Categories");
            columnNames.Add("UseVariablePrice");
            columnNames.Add("MinimumPrice");
            columnNames.Add("MaximumPrice");
            columnNames.Add("SearchKeywords");
            columnNames.Add("HidePrice");


            return columnNames.ToArray();
        }

        /// <summary>
        /// Returns a list of numaric column/field names
        /// </summary>
        /// <returns>Returns a list of numaric column/field names</returns>
        public static List<String> GetNumaricFields()
        {
            List<string> columnNames = new List<string>();
            columnNames.Add("ProductId");
            columnNames.Add("StoreId");
            columnNames.Add("Price");
            columnNames.Add("CostOfGoods");
            columnNames.Add("MSRP");
            columnNames.Add("Weight");
            columnNames.Add("Length");
            columnNames.Add("Width");
            columnNames.Add("Height");
            columnNames.Add("ManufacturerId");
            columnNames.Add("TaxCodeId");
            columnNames.Add("ShippableId");
            columnNames.Add("WarehouseId");
            columnNames.Add("InventoryModeId");
            columnNames.Add("InStock");
            columnNames.Add("InStockWarningLevel");
            columnNames.Add("VendorId");
            columnNames.Add("ProductTemplateId");
            columnNames.Add("WrapGroupId");
            columnNames.Add("MinQuantity");
            columnNames.Add("MaxQuantity");
            columnNames.Add("VisibilityId");
            columnNames.Add("MinimumPrice");
            columnNames.Add("MaximumPrice");
            return columnNames;
        }

        /// <summary>
        /// Fields that are required while importing csv
        /// </summary>
        /// <returns></returns>
        public static string[] GetCSVImportRequiredFields()
        {
            List<string> columnNames = new List<string>();            
            columnNames.Add("Name");
            return columnNames.ToArray();
        }

        /// <summary>
        /// Fields that are used as key field (to identify an object) while performing a csv update
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDefaultKeyFields()
        {
            List<string> columnNames = new List<string>();
            columnNames.Add("ProductId");
            return columnNames;
        }
    }
}
