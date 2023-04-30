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
    /// DataSource class for ProductProductTemplate objects
    /// </summary>
    [DataObject(true)]
    public partial class ProductProductTemplateDataSource
    {
        /// <summary>
        /// Delete any templates associated with the product
        /// </summary>
        public static void DeleteForProduct(int productId)
        {
            Database database = Token.Instance.Database;
            string sql = "DELETE FROM ac_ProductProductTemplates WHERE ProductId = @productId";

            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
            using (DbCommand deleteCommand = database.GetSqlStringCommand(sql))
            {
                database.AddInParameter(deleteCommand, "productId", DbType.Int32, productId);
                database.ExecuteNonQuery(deleteCommand);
            }
            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
        }
    }
}
