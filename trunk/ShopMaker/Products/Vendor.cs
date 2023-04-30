using System;
using System.Data;
using System.Data.Common;
using MakerShop.Common;
using MakerShop.Data;

namespace MakerShop.Products
{
    public partial class Vendor
    {
        /// <summary>
        /// Deletes this Vendor object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public bool Delete()
        {
            //RESET ANY PRODUCTS THAT HAVE THIS VENDOR ASSOCIATED
            Database database = Token.Instance.Database;
            DbCommand deleteCommand = database.GetSqlStringCommand("UPDATE ac_Products SET VendorId = NULL WHERE VendorId = @vendorId");
            database.AddInParameter(deleteCommand, "@vendorId", DbType.Int32, this.VendorId);
            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
            database.ExecuteNonQuery(deleteCommand);
            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            //DELETE THE VENDOR
            return this.BaseDelete();
        }
    }
}
