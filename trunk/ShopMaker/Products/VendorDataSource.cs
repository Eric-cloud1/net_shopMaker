using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;

namespace MakerShop.Products
{
    /// <summary>
    /// The DataSource class for Vendor Objects
    /// </summary>
    [DataObject(true)]
    public partial class VendorDataSource
    {
        /// <summary>
        /// Loads all vendors that are associated with products in an order
        /// </summary>
        /// <param name="orderId">The id of the order to check for vendors</param>
        /// <returns>All vendors associated with items in an order</returns>
        public static VendorCollection LoadForOrder(int orderId)
        {
            int storeId = Token.Instance.StoreId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT DISTINCT " + Vendor.GetColumnNames("V"));
            selectQuery.Append(" FROM ac_OrderItems OI,ac_Products P,ac_Vendors V");
            selectQuery.Append(" WHERE OI.OrderId = @orderId");
            selectQuery.Append(" AND OI.ProductId = P.ProductId");
            selectQuery.Append(" AND P.VendorId = V.VendorId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@orderId", System.Data.DbType.Int32, orderId);
            //EXECUTE THE COMMAND
            VendorCollection results = new VendorCollection();
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    Vendor vendor = new Vendor();
                    Vendor.LoadDataReader(vendor, dr);
                    results.Add(vendor);
                }
                dr.Close();
            }
            return results;
        }

    }
}
