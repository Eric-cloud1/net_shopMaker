using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.DataClient.Api.Schema
{
    public partial class Category
    {
        public static string[] GetCSVEnabledFields(string EmptyString)
        {
            List<string> columnNames = new List<string>();
            columnNames.Add("CategoryId");
            columnNames.Add("StoreId");
            columnNames.Add("ParentId");
            columnNames.Add("Name");
            columnNames.Add("Summary");
            columnNames.Add("Description");
            columnNames.Add("ThumbnailUrl");
            columnNames.Add("ThumbnailAltText");
            columnNames.Add("DisplayPage");
            columnNames.Add("Theme");
            columnNames.Add("HtmlHead");
            columnNames.Add("VisibilityId");
            return columnNames.ToArray();
        }

        /// <summary>
        /// Fields that are required while importing csv
        /// </summary>
        /// <returns></returns>
        public static string[] GetCSVImportRequiredFields()
        {
            List<string> columnNames = new List<string>();
            columnNames.Add("Name");
            columnNames.Add("ParentId");
            return columnNames.ToArray();

        }

        /// <summary>
        /// Fields that are used as key field (to identify an object) while performing a csv update
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDefaultKeyFields()
        {
            List<string> columnNames = new List<string>();            
            columnNames.Add("Name");
            columnNames.Add("ParentId");
            return columnNames;
        }


        /// <summary>
        /// Returns a list of numaric column/field names
        /// </summary>
        /// <returns>Returns a list of numaric column/field names</returns>
        public static List<String> GetNumaricFields()
        {
            List<string> columnNames = new List<string>();
            columnNames.Add("CategoryId");
            columnNames.Add("StoreId");
            columnNames.Add("ParentId");
            columnNames.Add("VisibilityId");
            return columnNames;
        }
    }
}
