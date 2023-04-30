using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.DataClient.Api.Schema
{
    public partial class Order
    {
        public static string[] GetCSVEnabledFields(string prefix)
        {
            List<string> columnNames = new List<string>();
            columnNames.Add("OrderId");
            columnNames.Add("OrderNumber");
            columnNames.Add("OrderDate");
            columnNames.Add("StoreId");
            columnNames.Add("UserName");
            //columnNames.Add("UserId");
            //columnNames.Add("AffiliateId");
            columnNames.Add("Affiliate");
            columnNames.Add("BillToFirstName");
            columnNames.Add("BillToLastName");
            columnNames.Add("BillToCompany");
            columnNames.Add("BillToAddress1");
            columnNames.Add("BillToAddress2");
            columnNames.Add("BillToCity");
            columnNames.Add("BillToProvince");
            columnNames.Add("BillToPostalCode");
            columnNames.Add("BillToCountryCode");
            columnNames.Add("BillToPhone");
            columnNames.Add("BillToFax");
            columnNames.Add("BillToEmail");
            columnNames.Add("ProductSubtotal");
            columnNames.Add("TotalCharges");
            columnNames.Add("TotalPayments");
            //columnNames.Add("OrderStatusId");
            columnNames.Add("OrderStatus");
            //columnNames.Add("Exported");
            columnNames.Add("RemoteIP");
            columnNames.Add("Referrer");
            columnNames.Add("GoogleOrderNumber");
            columnNames.Add("PaymentStatusId");
            columnNames.Add("ShipmentStatusId");
            return  columnNames.ToArray();
        }

        /// <summary>
        /// Fields that are required while importing csv
        /// </summary>
        /// <returns></returns>
        public static string[] GetCSVImportRequiredFields()
        {
            List<string> columnNames = new List<string>();
            columnNames.Add("OrderNumber");
            columnNames.Add("OrderDate");            
            columnNames.Add("UserName");                        
            columnNames.Add("BillToFirstName");
            columnNames.Add("BillToLastName");            
            columnNames.Add("BillToAddress1");            
            columnNames.Add("BillToCity");
            columnNames.Add("BillToProvince");
            columnNames.Add("BillToPostalCode");
            columnNames.Add("BillToCountryCode");
            columnNames.Add("BillToPhone");            
            columnNames.Add("BillToEmail");            
            columnNames.Add("TotalCharges");
            columnNames.Add("TotalPayments");            
            columnNames.Add("OrderStatus");                        
            return columnNames.ToArray();

        }

        
        /// <summary>
        /// Fields that are used as key field (to identify an object) while performing a csv update
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDefaultKeyFields()
        {
            List<string> columnNames = new List<string>();
            columnNames.Add("OrderId");
            return columnNames;
        }


        /// <summary>
        /// Returns a list of numaric column/field names
        /// </summary>
        /// <returns>Returns a list of numaric column/field names</returns>
        public static List<String> GetNumaricFields()
        {
            List<string> columnNames = new List<string>();
            columnNames.Add("OrderId");
            columnNames.Add("OrderNumber");
            columnNames.Add("StoreId");
            columnNames.Add("AffiliateId");
            columnNames.Add("ProductSubtotal");
            columnNames.Add("TotalCharges");
            columnNames.Add("TotalPayments");
            columnNames.Add("OrderStatus");
            columnNames.Add("GoogleOrderNumber");
            columnNames.Add("PaymentStatusId");
            columnNames.Add("ShipmentStatusId");
            return columnNames;
        }
    }
}
