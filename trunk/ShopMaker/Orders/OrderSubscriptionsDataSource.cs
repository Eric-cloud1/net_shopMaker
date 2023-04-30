//Generated by DataSourceBaseGenerator

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;

namespace MakerShop.Orders
{
    [DataObject(true)]
    public partial class OrderSubscriptionsDataSource
    {

        /// <summary>
        /// Makes a snapshot of the order
        /// </summary>
        /// <param name="orderId">Order to cancel</param>
        /// <param name="site">where the order came from</param>
        /// <param name="user">User</param>
        public static void CreateOrderSubscription(int orderId, string user)
        {

            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand updateCommand = database.GetStoredProcCommand("wsp_OrderSubscriptions_Create"))
            {
                database.AddInParameter(updateCommand, "@OrderId", System.Data.DbType.Int32, orderId);
                database.AddInParameter(updateCommand, "@ModifiedUser", System.Data.DbType.String, user);

                database.ExecuteNonQuery(updateCommand);
            }
        }
        /// <summary>
        /// Setup Subscription
        /// </summary>
        /// <param name="orderId">Order to cancel</param>
        /// <param name="user">User</param>
        public static void CaptureOrderSubscription(int orderId, string user)
        {

            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand updateCommand = database.GetStoredProcCommand("wsp_OrderSubscriptions_Capture"))
            {
                database.AddInParameter(updateCommand, "@OrderId", System.Data.DbType.Int32, orderId);
                database.AddInParameter(updateCommand, "@ModifiedUser", System.Data.DbType.String, user);
                database.ExecuteNonQuery(updateCommand);
            }
        }

        /// <summary>
        /// Cancel Subscription
        /// </summary>
        /// <param name="orderId">Order to cancel</param>
        /// <param name="user">User</param>
        /// <returns></returns>
        public static bool CancelOrderSubscription(int orderId, string user)
        {

            return CancelOrderSubscription(orderId, user, 255);
        }


        /// <summary>
        /// Cancel Subscription
        /// </summary>
        /// <param name="orderId">Order to cancel</param>
        /// <param name="user">User</param>
        /// <param name="orderSubscriptionStatusCode">orderSubscriptionStatusCode</param>
        /// <returns></returns>
        public static bool CancelOrderSubscription(int orderId, string user, short orderSubscriptionStatusCode)
        {
            //  [wsp_OrderSubscriptions_CancelSubscription]             
            //@OrderId  int,             
            //@ModifiedDate datetime =null,              
            //@ModifiedUser varchar(50) =null,              
            //@Cancel bit = 0, 
            //@OrderSubscriptionStatusCode tinyint = 255,
            //@Debug  bit =null  
            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand updateCommand = database.GetStoredProcCommand("wsp_OrderSubscriptions_CancelSubscription"))
            {
                database.AddInParameter(updateCommand, "@OrderId", System.Data.DbType.Int32, orderId);
                database.AddInParameter(updateCommand, "@OrderSubscriptionStatusCode", System.Data.DbType.Int16, orderSubscriptionStatusCode);
                database.AddInParameter(updateCommand, "@ModifiedUser", System.Data.DbType.String, user);
                database.AddParameter(updateCommand, "@rtn", System.Data.DbType.Int32, System.Data.ParameterDirection.ReturnValue, null, System.Data.DataRowVersion.Default, -1);
                database.ExecuteNonQuery(updateCommand);
                int rtn = (int)updateCommand.Parameters["@rtn"].Value;
                return rtn > 0;

            }
        }

       
                  
              
     }
}
