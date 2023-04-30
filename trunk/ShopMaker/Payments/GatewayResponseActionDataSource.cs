using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;

namespace MakerShop.Payments
{
    public partial class GatewayResponseActionDataSource
    {

 
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DataTable getSubscriptionStatus()
        {
            // Marketing.AffiliateCollection
            // return Marketing.AffiliateDataSource.LoadForStore();
            DataSet subscriptionStatus = new DataSet();
            Database database = Token.Instance.Database;
            string sql = string.Format(@"SET TRANSACTION ISOLATION
LEVEL READ UNCOMMITTED; select SubscriptionStatusCode, SubscriptionStatus from en_OrderSubscriptionStatusCodes ORDER BY SubscriptionStatus desc");

            using (System.Data.Common.DbCommand dataSetCommand = database.GetSqlStringCommand(sql))
            {
                subscriptionStatus = database.ExecuteDataSet(dataSetCommand);
            }

            return subscriptionStatus.Tables[0];
        }

    }
}
