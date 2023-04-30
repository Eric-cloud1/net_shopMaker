using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using MakerShop.Common;
using MakerShop.Data;

namespace MakerShop.Reporting
{
    public class ShippingSummary
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="US">Is null all records are returned. If False US excluded. If True only US.</param>
        /// <returns>null if no records found</returns>
        public static DataTable PackagesToShip(bool? US)
        {

            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand cmd = database.GetStoredProcCommand("rpt_Shipping"))
            {
                if (US.HasValue)
                    database.AddInParameter(cmd, "@US", DbType.Boolean, US);
                DataSet ds=  database.ExecuteDataSet(cmd);
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];
            }
            return null;
        }

        public static DataTable PackagesShipped(string gatewayPrefix, DateTime? startDate)
        {

            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand cmd = database.GetStoredProcCommand("rpt_ShippingTrackingNumbersPerGateway"))
            {
                database.AddInParameter(cmd, "@GatewayPrefix", System.Data.DbType.String, gatewayPrefix);
                if (startDate.HasValue)
                    database.AddInParameter(cmd, "@StartDate", System.Data.DbType.DateTime, startDate);
                DataSet ds  = database.ExecuteDataSet(cmd);
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];

            }
            return null;
        }
    }
}
