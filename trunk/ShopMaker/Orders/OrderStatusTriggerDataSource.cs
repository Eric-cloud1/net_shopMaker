using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using MakerShop.Stores;
using MakerShop.Common;
using MakerShop.Data;

namespace MakerShop.Orders
{
    /// <summary>
    /// DataSource class for OrderStatusTrigger objects
    /// </summary>
    [DataObject(true)]
    public partial class OrderStatusTriggerDataSource
    {
        /// <summary>
        /// Loads an OrderStatus object for given StoreEvent
        /// </summary>
        /// <param name="storeEvent">The StoreEvent to load OrderStatus for</param>
        /// <returns>Loaded OrderStatus object</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderStatus LoadForStoreEvent(StoreEvent storeEvent)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            //GET RECORDS STARTING AT FIRST ROW
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + OrderStatus.GetColumnNames("OS"));
            selectQuery.Append(" FROM ac_OrderStatusTriggers OST, ac_OrderStatuses OS");
            selectQuery.Append(" WHERE OST.OrderStatusId = OS.OrderStatusId");
            selectQuery.Append(" AND OST.StoreEventId = @storeEventId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeEventId", System.Data.DbType.Int32, (int)storeEvent);
            //EXECUTE THE COMMAND
            OrderStatus status = null;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                if (dr.Read())
                {
                    status = new OrderStatus();
                    OrderStatus.LoadDataReader(status, dr);
                }
                dr.Close();
            }
            return status;
        }

        /// <summary>
        /// Get a list of StoreEvent objects that are overloaded
        /// </summary>
        /// <returns>A list of StoreEvent objects that are overloaded</returns>
        public static List<StoreEvent> GetOverloadedEvents()
        {
            List<StoreEvent> eventList = new List<StoreEvent>();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            //GET RECORDS STARTING AT FIRST ROW
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TriggerCount, StoreEventId");
            selectQuery.Append(" FROM ac_OrderStatusTriggers T, ac_OrderStatuses S");
            selectQuery.Append(" WHERE T.OrderStatusId = S.OrderStatusId");
            selectQuery.Append(" AND S.StoreId = @storeId");
            selectQuery.Append(" GROUP BY StoreEventId");
            selectQuery.Append(" HAVING COUNT(1) > 1");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    try
                    {
                        eventList.Add((StoreEvent)dr.GetInt32(1));
                    }
                    catch { }
                }
                dr.Close();
            }
            return eventList;
        }
    }
}
