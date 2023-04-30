using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;

namespace MakerShop.Stores
{
    [DataObject(true)]
    public partial class BannedIPDataSource
    {
        /// <summary>
        /// Determine if an IP address is banned from accessing the store.
        /// </summary>
        /// <param name="ip">The IP address in standdard dotted notation (x.x.x.x)</param>
        public static bool IsBanned(string ip)
        {
            //CONVERT IP TO NUMBER
            long ipNumber = BannedIP.ConvertToNumber(ip);
            //BUILD QUERY
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS RangeCount");
            selectQuery.Append(" FROM ac_BannedIPs");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" AND IPRangeStart <= @ipNumber");
            selectQuery.Append(" AND IPRangeEnd >= @ipNumber");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@ipNumber", System.Data.DbType.Int64, ipNumber);
            //IP IS BANNED IF ONE OR MORE RANGES MATCHES THE NUMBER
            return (MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand)) > 0);
        }
    }
}
