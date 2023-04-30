using System;
using System.ComponentModel;
using MakerShop.Common;
using MakerShop.Data;
using System.Data;
using System.Data.Common;
using MakerShop.Utility;

namespace MakerShop.Shipping
{
    /// <summary>
    /// DataSource class for Province objects
    /// </summary>
    [DataObject(true)]
    public partial class ProvinceDataSource
    {
        /// <summary>
        /// Gets Id of a province given its name and country
        /// </summary>
        /// <param name="countryCode">The country of the province</param>
        /// <param name="provinceName">Name of the province</param>
        /// <returns>Id of the province if found or '0' if province is not found</returns>
        public static int GetProvinceIdByName(string countryCode, string provinceName)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT ProvinceId FROM ac_Provinces WHERE CountryCode=@countryCode AND (Name=@provinceName OR LoweredName=@loweredProvinceName OR ProvinceCode=@provinceName)");
            database.AddInParameter(selectCommand, "@countryCode", DbType.String, countryCode);
            database.AddInParameter(selectCommand, "@provinceName", DbType.String, provinceName);
            string loweredProvinceName = (provinceName == null) ? string.Empty : provinceName.ToLowerInvariant();
            database.AddInParameter(selectCommand, "@loweredProvinceName", DbType.String, loweredProvinceName);
            return AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }
    }
}