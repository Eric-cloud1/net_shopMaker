using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Orders;
using MakerShop.Data;
using MakerShop.Users;
using System.Data;
using System.Data.Common;
using MakerShop.Utility;
using MakerShop.Common;

namespace MakerShop.DataClient.Api.ObjectHandlers
{
    class OrderHandler
    {
        public static Api.Schema.Order[] ConvertToClientArray(OrderCollection objOrderCollection, bool downloadCCData, bool onlyCsvData)
        {
            DataObject dataObject = new DataObject("Order", typeof(MakerShop.Orders.Order), typeof(Api.Schema.Order));

            Api.Schema.Order[] arrClientApiOrder = new Api.Schema.Order[objOrderCollection.Count];
            Api.Schema.Order objClientApiOrder = null;
            String errorMessage = String.Empty;
            List<String> errors = new List<string>();
            for (int i = 0; i < objOrderCollection.Count; i++ )
            {
                MakerShop.Orders.Order objOrder = objOrderCollection[i];

                errorMessage = String.Empty;
                objClientApiOrder = (Api.Schema.Order)dataObject.ConvertToClientApiObject(objOrder, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage) && !errors.Contains(errorMessage))
                {
                    errors.Add(errorMessage);
                    // LOG THIS ERROR AS WELL
                    Logger.Error(errorMessage);
                }

                if (onlyCsvData)
                {
                    if (objOrder.User != null)
                    {
                        objClientApiOrder.UserName = objOrder.User.UserName;
                    }
                    if (objOrder.Affiliate != null)
                    {
                        objClientApiOrder.Affiliate = objOrder.Affiliate.Name;
                    }
                    if (objOrder.OrderStatus != null)
                    {
                        objClientApiOrder.OrderStatus = objOrder.OrderStatus.Name;
                    }
                }
                else
                {

                    // Payments                
                    DataObject nestedDataObject = new DataObject("Payment", typeof(MakerShop.Payments.Payment), typeof(Api.Schema.Payment));
                    //objClientApiOrder.Payments = (Api.Schema.Payment[])nestedDataObject.ConvertAC6Collection(objOrder.Payments);
                    Schema.Payment[] arrPayment = new MakerShop.DataClient.Api.Schema.Payment[objOrder.Payments.Count];
                    Schema.Payment objClientApiPayment = null;
                    for (int j = 0; j < objOrder.Payments.Count; j++)
                    {
                        MakerShop.Payments.Payment objPayment = objOrder.Payments[j];
                        objClientApiPayment = (Schema.Payment)nestedDataObject.ConvertToClientApiObject(objPayment);

                        DataObject innerDataObject = new DataObject("", typeof(MakerShop.Payments.Transaction), typeof(Schema.Transaction));
                        objClientApiPayment.Transactions = (Schema.Transaction[])innerDataObject.ConvertAC6Collection(objPayment.Transactions);

                        // HANDLE CREDIT CARD DATA EXPORT
                        if (!downloadCCData) objClientApiPayment.EncryptedAccountData = string.Empty;

                        arrPayment[j] = objClientApiPayment;
                    }
                    objClientApiOrder.Payments = arrPayment;

                    // OrderNote
                    nestedDataObject = new DataObject("OrderNote", typeof(MakerShop.Orders.OrderNote), typeof(Api.Schema.OrderNote));
                    Schema.OrderNote[] arrOrderNotes = (Api.Schema.OrderNote[])nestedDataObject.ConvertAC6Collection(objOrder.Notes);
                    //Schema.OrderNote[] arrOrderNotes2 = new MakerShop.DataClient.Api.Schema.OrderNote[arrOrderNotes.Length];
                    List<Schema.OrderNote> lstOrderNotes = new List<MakerShop.DataClient.Api.Schema.OrderNote>();
                    int k = 0;
                    for (int j = 0; j < arrOrderNotes.Length; j++)
                    {
                        if (arrOrderNotes[j].NoteTypeId == (byte)NoteType.Private || arrOrderNotes[j].NoteTypeId == (byte)NoteType.Public)
                        {
                            lstOrderNotes.Add(arrOrderNotes[j]);
                        }
                    }
                    objClientApiOrder.Notes = lstOrderNotes.ToArray();


                    // OrderShipments
                    //This Dictionary will be ued to detect Order Items that either these are associated with 
                    // Shipments or Not
                    SortedDictionary<int, bool> OrderItemDic = new SortedDictionary<int, bool>();
                    nestedDataObject = new DataObject("OrderShipment", typeof(MakerShop.Orders.OrderShipment), typeof(Api.Schema.OrderShipment));
                    Api.Schema.OrderShipment[] arrOrderShipment = new MakerShop.DataClient.Api.Schema.OrderShipment[objOrder.Shipments.Count];
                    Api.Schema.OrderShipment objClientApiOrderShipment = null;
                    for (int j = 0; j < objOrder.Shipments.Count; j++)
                    {
                        MakerShop.Orders.OrderShipment objOrderShipment = objOrder.Shipments[j];
                        objClientApiOrderShipment = (Api.Schema.OrderShipment)nestedDataObject.ConvertToClientApiObject(objOrderShipment);

                        //Tracking Numbers
                        DataObject innerDataObject = new DataObject("TrackingNumber", typeof(MakerShop.Orders.TrackingNumber), typeof(Api.Schema.TrackingNumber));
                        objClientApiOrderShipment.TrackingNumbers = (Schema.TrackingNumber[])innerDataObject.ConvertAC6Collection(objOrderShipment.TrackingNumbers);

                        arrOrderShipment[j] = objClientApiOrderShipment;
                    }
                    objClientApiOrder.Shipments = arrOrderShipment;

                    // Order Items               
                    nestedDataObject = new DataObject("OrderItem", typeof(MakerShop.Orders.OrderItem), typeof(Api.Schema.OrderItem));
                    //Api.Schema.OrderItem[] arrOrderItems = (Api.Schema.OrderItem[])nestedDataObject.ConvertAC6Collection(objOrder.Items);
                    Api.Schema.OrderItem[] arrOrderItems = new MakerShop.DataClient.Api.Schema.OrderItem[objOrder.Items.Count];
                    Api.Schema.OrderItem objClientApiOrderItem = null;
                    k = 0;
                    for (int j = 0; j < objOrder.Items.Count; j++)
                    {
                        MakerShop.Orders.OrderItem objOrderItem = objOrder.Items[j];
                        objClientApiOrderItem = (Api.Schema.OrderItem)nestedDataObject.ConvertToClientApiObject(objOrderItem);
                        //FIXME:
                        //if (OrderItemDic.ContainsKey(objClientApiOrderItem.OrderItemId))
                        //{
                        //    continue;
                        //}

                        DataObject innerDataObject = new DataObject("OrderItemInput", typeof(MakerShop.Orders.OrderItemInput), typeof(Api.Schema.OrderItemInput));
                        objClientApiOrderItem.Inputs = (Schema.OrderItemInput[])innerDataObject.ConvertAC6Collection(objOrderItem.Inputs);

                        //GiftCertificates
                        innerDataObject = new DataObject("GiftCertificate", typeof(MakerShop.Payments.GiftCertificate), typeof(Api.Schema.GiftCertificate));
                        objClientApiOrderItem.GiftCertificates = (Schema.GiftCertificate[])innerDataObject.ConvertAC6Collection(objOrderItem.GiftCertificates);

                        //GiftCertificateTransaction
                        for (int i_gc = 0; i_gc < objClientApiOrderItem.GiftCertificates.Length; i_gc++)
                        {
                            Schema.GiftCertificate schemaGC = objClientApiOrderItem.GiftCertificates[i_gc];
                            MakerShop.Payments.GiftCertificate acGC = objOrderItem.GiftCertificates[i_gc];

                            innerDataObject = new DataObject("GiftCertificateTransaction", typeof(MakerShop.Payments.GiftCertificateTransaction), typeof(Api.Schema.GiftCertificateTransaction));
                            schemaGC.Transactions = (Schema.GiftCertificateTransaction[])innerDataObject.ConvertAC6Collection(acGC.Transactions);
                        }

                        //DigitalGoods
                        innerDataObject = new DataObject("OrderItemDigitalGood", typeof(MakerShop.Orders.OrderItemDigitalGood), typeof(Api.Schema.OrderItemDigitalGood));
                        objClientApiOrderItem.OrderItemDigitalGoods = (Schema.OrderItemDigitalGood[])innerDataObject.ConvertAC6Collection(objOrderItem.DigitalGoods);

                        //Download
                        for (int i_dg = 0; i_dg < objClientApiOrderItem.OrderItemDigitalGoods.Length; i_dg++)
                        {
                            Schema.OrderItemDigitalGood schemaDC = objClientApiOrderItem.OrderItemDigitalGoods[i_dg];
                            MakerShop.Orders.OrderItemDigitalGood acDG = objOrderItem.DigitalGoods[i_dg];

                            innerDataObject = new DataObject("Download", typeof(MakerShop.DigitalDelivery.Download), typeof(Api.Schema.Download));
                            schemaDC.Downloads = (Schema.Download[])innerDataObject.ConvertAC6Collection(acDG.Downloads);
                        }

                        //Subscriptions
                        innerDataObject = new DataObject("Subscription", typeof(MakerShop.Orders.Subscription), typeof(Api.Schema.Subscription));
                        objClientApiOrderItem.Subscriptions = (Schema.Subscription[])innerDataObject.ConvertAC6Collection(objOrderItem.Subscriptions);


                        arrOrderItems[k++] = objClientApiOrderItem;
                    }
                    objClientApiOrder.OrderItems = arrOrderItems;

                    // Order Coupons                
                    objClientApiOrder.Coupons = (Schema.OrderCoupon[])DataObject.ConvertToClientArray(typeof(MakerShop.Orders.OrderCoupon), typeof(Schema.OrderCoupon), objOrder.Coupons);

                }

                arrClientApiOrder[i] = objClientApiOrder;
            }
            return arrClientApiOrder;
        }
        

        public static List<String> GetIdListForOrderCriteria(MakerShop.DataClient.Api.Schema.OrderCriteria criteria, bool isUPSWSRequest)
        {
            List<String> idList = new List<string>();
            using (IDataReader dr = GetDataReader(criteria, true, isUPSWSRequest))
            {
                while (dr.Read())
                {
                    idList.Add(dr["OrderId"].ToString());
                }
                dr.Close();
            }
            return idList;
        }

        public static OrderCollection GetCustomizedCollection(MakerShop.DataClient.Api.Schema.OrderCriteria criteria, bool isUPSWSRequest)
        {
            OrderCollection results = new OrderCollection();
            using (IDataReader dr = GetDataReader(criteria, false,isUPSWSRequest)) 
            {
                while (dr.Read())
                {
                    Order order = new Order();
                    Order.LoadDataReader(order, dr);
                    results.Add(order);
                }
                dr.Close();
            }
            return results;
        }

        

        private static IDataReader GetDataReader(MakerShop.DataClient.Api.Schema.OrderCriteria criteria, bool onlyIds, bool isUPSWSRequest)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT DISTINCT ");
            if (onlyIds)
            {
                selectQuery.Append(" O.OrderId  ");
            }
            else
            {
                selectQuery.Append(Order.GetColumnNames("O"));
            }
            

            if (isUPSWSRequest)
            {
                selectQuery.Append(" FROM (ac_Orders O INNER JOIN  ac_OrderShipments OS ON O.OrderId = OS.OrderId");
                selectQuery.Append(" INNER JOIN  ac_ShipMethods SM ON OS.ShipMethodId = SM.ShipMethodId");
                selectQuery.Append(" INNER JOIN  ac_ShipGateways SG ON SM.ShipGatewayId = SG.ShipGatewayId)");
                selectQuery.Append(" WHERE (SG.ClassId LIKE '%MakerShop.Shipping.Providers.UPS.UPS%'");
                selectQuery.Append(" OR SG.ClassId LIKE '%MakerShop.UPS%')");
                selectQuery.Append(" AND  O.StoreId = 1");                
            }
            else
            {
                selectQuery.Append(" FROM ac_Orders O");
                selectQuery.Append(" WHERE O.StoreId = @storeId");
            }

            if (criteria != null)
            {

                // Order Number Filter
                if (criteria.OrderNumberFilter != null)
                {

                    switch ((CompareOption)Enum.Parse(typeof(CompareOption), criteria.OrderNumberFilter.Option))
                    {
                        case CompareOption.Equal:
                            if (criteria.OrderNumberFilter.Value1 > 0)
                            {
                                selectQuery.Append(" AND O.OrderNumber = @OrderNumber1");
                            }
                            break;

                        case CompareOption.NotEqual:
                            if (criteria.OrderNumberFilter.Value1 > 0)
                            {
                                selectQuery.Append(" AND O.OrderNumber != @OrderNumber1");
                            }
                            break;

                        case CompareOption.LessThan:
                            if (criteria.OrderNumberFilter.Value1 > 0)
                            {
                                selectQuery.Append(" AND O.OrderNumber <  @OrderNumber1");
                            }
                            break;

                        case CompareOption.GreatorThan:
                            if (criteria.OrderNumberFilter.Value1 > 0)
                            {
                                selectQuery.Append(" AND O.OrderNumber >  @OrderNumber1");
                            }
                            break;

                        case CompareOption.LessThanEqualTo:
                            if (criteria.OrderNumberFilter.Value1 > 0)
                            {
                                selectQuery.Append(" AND O.OrderNumber <=  @OrderNumber1");
                            }
                            break;

                        case CompareOption.GreatorThanEqualTo:
                            if (criteria.OrderNumberFilter.Value1 > 0)
                            {
                                selectQuery.Append(" AND O.OrderNumber >= @OrderNumber1");
                            }
                            break;

                        case CompareOption.Between:
                            if (criteria.OrderNumberFilter.Value1 > 0 && criteria.OrderNumberFilter.Value2 > 0)
                            {
                                selectQuery.Append(" AND O.OrderNumber BETWEEN  @OrderNumber1  AND  @OrderNumber2");
                            }
                            break;
                    }
                }

                // Order Date Filter
                if (criteria.OrderDateFilter != null)
                {
                    if (!criteria.OrderDateFilter.StartDate.Equals(DateTime.MinValue))
                    {
                        selectQuery.Append(" AND O.OrderDate >= @StartDate");
                    }

                    if (!criteria.OrderDateFilter.EndDate.Equals(DateTime.MinValue))
                    {
                        selectQuery.Append(" AND O.OrderDate <= @EndDate");
                    }
                }

                // Order Status Filter

                switch (criteria.OrderStatusFilter.ToLower())
                {
                    case "any":
                        //Nothing will be added
                        break;
                    default:
                        if (!String.IsNullOrEmpty(criteria.OrderStatusFilter))
                        {
                            selectQuery.Append(" AND O.OrderStatusId IN(" + criteria.OrderStatusFilter + ")");
                        }
                        break;
                }
            }


            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());

            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);

            if (criteria != null)
            {
                if (criteria.OrderNumberFilter != null)
                {
                    if (criteria.OrderNumberFilter.Value1 > 0)
                    {
                        database.AddInParameter(selectCommand, "@OrderNumber1", System.Data.DbType.Int32, criteria.OrderNumberFilter.Value1);
                    }
                    if (criteria.OrderNumberFilter.Value2 > 0)
                    {
                        database.AddInParameter(selectCommand, "@OrderNumber2", System.Data.DbType.Int32, criteria.OrderNumberFilter.Value2);
                    }
                }

                if (criteria.OrderDateFilter != null)
                {
                    if (!criteria.OrderDateFilter.StartDate.Equals(DateTime.MinValue))
                    {
                        database.AddInParameter(selectCommand, "@StartDate", System.Data.DbType.DateTime, criteria.OrderDateFilter.StartDate);
                    }
                    if (!criteria.OrderDateFilter.EndDate.Equals(DateTime.MinValue))
                    {
                        database.AddInParameter(selectCommand, "@EndDate", System.Data.DbType.DateTime, criteria.OrderDateFilter.EndDate);
                    }
                }
            }

            //EXECUTE THE COMMAND            
            IDataReader dr = database.ExecuteReader(selectCommand);
            return dr;
        }

        /// <summary>
        /// This method wil return a OrderCollection against list of order Ids
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public static OrderCollection GetOrdersForIds(String idList)
        {
            OrderCollection orderCollection = new OrderCollection();
            MakerShop.Orders.Order order = null;
            String[] arrIds = idList.Split(',');
            foreach (String id in arrIds)
            {
                order = new MakerShop.Orders.Order();
                int orderId = AlwaysConvert.ToInt(id);
                if (order.Load(orderId))
                {
                    orderCollection.Add(order);
                }
            }
            return orderCollection;
        }

        /// <summary>
        /// This method will return OrderId of all orders in the store
        /// </summary>
        /// <returns></returns>
        public static List<String> GetIdListForStore()
        {
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT OrderId From ac_Orders Where StoreId = " + Token.Instance.StoreId);
            List<String> idList = new List<string>();
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    idList.Add(dr[0].ToString());
                }
                dr.Close();
            }
            return idList;
        }
    }
}
