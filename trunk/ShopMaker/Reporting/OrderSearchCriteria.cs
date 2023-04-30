using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Orders;
using MakerShop.Utility;
using MakerShop.Search;

namespace MakerShop.Reporting
{
    /// <summary>
    /// Class representing search criteria for orders
    /// </summary>
    /// 
    [Serializable]
    public class OrderSearchCriteria
    {
        private DateTime _OrderDateStart;
        private DateTime _OrderDateEnd;
        private string _OrderIdRange;
        private int _OrderIdStart;
        private int _OrderIdEnd;
        private string _OrderNumberRange;
        private int _OrderNumberStart;
        private int _OrderNumberEnd;
        private IdList _OrderStatus;
        private OrderPaymentStatus _PaymentStatus;
        private OrderShipmentStatus _ShipmentStatus;
        private Collection<MatchCriteria> _Filter;
        private decimal _MiniOrderPrice;
        private decimal _MaxOrderPrice;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrderSearchCriteria()
        {
            _OrderDateStart = DateTime.MinValue;
            _OrderDateEnd = DateTime.MaxValue;
            _OrderIdRange = string.Empty;
            _OrderIdStart = 0;
            _OrderIdEnd = 0;
            _OrderNumberRange = string.Empty;
            _OrderNumberStart = 0;
            _OrderNumberEnd = 0;
            _PaymentStatus = OrderPaymentStatus.Unspecified;
            _ShipmentStatus = OrderShipmentStatus.Unspecified;
            _OrderStatus = new IdList();
            _Filter = new Collection<MatchCriteria>();
            _MiniOrderPrice = 0;
            _MaxOrderPrice = 0;

        }

        /// <summary>
        /// Mini Prices for orders
        /// </summary>
        public decimal MiniOrderPrice
        {
            get { return _MiniOrderPrice; }
            set { _MiniOrderPrice = value; }
        }


        /// <summary>
        /// Max Prices for orders
        /// </summary>
        public decimal MaxOrderPrice
        {
            get { return _MaxOrderPrice; }
            set { _MaxOrderPrice = value; }
        }



        /// <summary>
        /// Start date for orders
        /// </summary>
        public DateTime OrderDateStart
        {
            get { return _OrderDateStart; }
            set { _OrderDateStart = value; }
        }

        /// <summary>
        /// End date for orders
        /// </summary>
        public DateTime OrderDateEnd
        {
            get { return _OrderDateEnd; }
            set { _OrderDateEnd = value; }
        }
        /// <summary>
        /// Todays's date for default orders
        /// </summary>
        public DateTime OrderDateDefault
        {
            get { return System.DateTime.Today; }
        }


        /// <summary>
        /// Starting order Id
        /// </summary>
        [Obsolete("Use OrderIdRange instead.")]
        public int OrderIdStart
        {
            get { return _OrderIdStart; }
            set { _OrderIdStart = value; }
        }

        /// <summary>
        /// Ending order Id
        /// </summary>
        [Obsolete("Use OrderIdRange instead.")]
        public int OrderIdEnd
        {
            get { return _OrderIdEnd; }
            set { _OrderIdEnd = value; }
        }

        /// <summary>
        /// Order Id Range
        /// </summary>
        public string OrderIdRange
        {
            get { return _OrderIdRange; }
            set { _OrderIdRange = value; }
        }

        /// <summary>
        /// Starting order number
        /// </summary>
        [Obsolete("Use OrderNumberRange instead.")]
        public int OrderNumberStart
        {
            get { return _OrderNumberStart; }
            set { _OrderNumberStart = value; }
        }

        /// <summary>
        /// Ending order number
        /// </summary>
        [Obsolete("Use OrderNumberRange instead.")]
        public int OrderNumberEnd
        {
            get { return _OrderNumberEnd; }
            set { _OrderNumberEnd = value; }
        }

        /// <summary>
        /// Order Number Range
        /// </summary>
        public string OrderNumberRange
        {
            get { return _OrderNumberRange; }
            set { _OrderNumberRange = value; }
        }

        /// <summary>
        /// Payment status of the orders to search
        /// </summary>
        public OrderPaymentStatus PaymentStatus
        {
            get { return _PaymentStatus; }
            set { _PaymentStatus = value; }
        }

        /// <summary>
        /// Shipment status of the orders to search
        /// </summary>
        public OrderShipmentStatus ShipmentStatus
        {
            get { return _ShipmentStatus; }
            set { _ShipmentStatus = value; }
        }

        /// <summary>
        /// Order status of the orders to search
        /// </summary>
        public IdList OrderStatus
        {
            get { return this._OrderStatus; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Collection<MatchCriteria> Filter
        {
            get { return this._Filter; }
            set { this._Filter = value; }
        }

        /// <summary>
        /// Builds a select command using this object and the given sort expression.
        /// </summary>
        /// <param name="sortExpression">The sort expression to use in the select command</param>
        /// <returns>The DbCommand object representing the required select command</returns>
        public DbCommand BuildSelectCommand(string sortExpression)
        {
            return this.BuildSelectCommand(sortExpression, false);
        }

        /// <summary>
        /// Builds a select command using this object, the given sort expression and the count value.
        /// </summary>
        /// <param name="sortExpression">The sort expression to use in the select command</param>
        /// <param name="count">The number of rows to retrieve</param>
        /// <returns>The DbCommand object representing the required select command</returns>
        public DbCommand BuildSelectCommand(string sortExpression, bool count)
        {
            //INITIALIZE VARIABLES
            sortExpression = sortExpression.Replace("ac_Orders.", "b.");

            bool addPhoneNumber = false;

            // ADD THE ORDER TABLE PREFIX TO AVOID AMBIGIOUS COLUMN SQL ERROR
            if (string.IsNullOrEmpty(sortExpression))
                sortExpression = "b.OrderDate DESC";
            else
                if (!sortExpression.StartsWith("b.")) 
                {
                    if ((sortExpression.Contains("BillToLastName"))||(sortExpression.Contains("FullName")))
                        sortExpression = string.Format("BillToLastName {0}, b.BillToFirstName {0}", sortExpression.Replace("BillToLastName", "").Replace("FullName", ""));


                        sortExpression = "b." + sortExpression;

                }

            Database database = Token.Instance.Database;

            //CREATE A BLANK SQL COMMAND TO BE POPULATED
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT");

            //BUILD A LIST OF TABLES TO SELECT FROM
            List<string> tables = new List<string>();
            tables.Add("ac_Orders");

            //BUILD A LIST OF ANY JOIN CRITERIA
            List<string> tableJoins = new List<string>();

            //BUILD THE WHERE CRITERIA
            List<string> whereCriteria = new List<string>();

            //BUILD THE WHERE CRITERIA
            List<string> whereORCriteria = new List<string>();

            //ALL FILTERS MUST INCLUDE STOREID
            whereCriteria.Add("ac_Orders.StoreId = @storeId");
            database.AddInParameter(selectCommand, "storeId", DbType.Int32, Token.Instance.StoreId);

            //ADD AMOUNT FILTER
            if (this.MaxOrderPrice > 0)
            {
                whereCriteria.Add("(ac_Orders.TotalCharges <= @maxOrderPrice or ac_Orders.TotalPayments >= @maxOrderPrice)");
                database.AddInParameter(selectCommand, "maxOrderPrice", DbType.Decimal, this.MaxOrderPrice);
            }

            if (this.MiniOrderPrice > 0)
            {
                whereCriteria.Add("(ac_Orders.TotalCharges >= @miniOrderPrice or ac_Orders.TotalPayments >= @miniOrderPrice )");
                database.AddInParameter(selectCommand, "miniOrderPrice", DbType.Decimal, this.MiniOrderPrice);
            }


            //ADD DATE FILTER
            if (OrderDateStart > DateTime.MinValue)
            {
                whereCriteria.Add("ac_Orders.OrderDate >= @orderDateStart");
                database.AddInParameter(selectCommand, "orderDateStart", DbType.DateTime, LocaleHelper.FromLocalTime(this.OrderDateStart));
            }


            if ((OrderDateEnd != DateTime.MaxValue) && (OrderDateEnd != DateTime.MinValue))
            {
                whereCriteria.Add("ac_Orders.OrderDate <= @orderDateEnd");
                database.AddInParameter(selectCommand, "orderDateEnd", DbType.DateTime, LocaleHelper.FromLocalTime(this.OrderDateEnd));
            }




            //ADD ORDERID FILTER
            if (_OrderIdStart > 0)
            {
                whereCriteria.Add("ac_Orders.OrderId >= @orderIdStart");
                database.AddInParameter(selectCommand, "orderIdStart", DbType.Int32, _OrderIdStart);
            }
            if (_OrderIdEnd > 0)
            {
                whereCriteria.Add("ac_Orders.OrderId <= @orderIdEnd");
                database.AddInParameter(selectCommand, "orderIdEnd", DbType.Int32, _OrderIdEnd);
            }

            // ORDER ID RANGE FILTER
            IdRangeParser orderIdRangeParser = new IdRangeParser("ac_Orders.OrderId", _OrderIdRange, "oi");
            if (orderIdRangeParser.RangeCount > 0)
            {
                whereCriteria.Add(orderIdRangeParser.GetSqlString(string.Empty));
                orderIdRangeParser.AddParameters(database, selectCommand);
            }

            //ADD ORDERNUMBER FILTER
            if (_OrderNumberStart > 0)
            {
                whereCriteria.Add("b.OrderNumber >= @orderNumberStart");
                database.AddInParameter(selectCommand, "orderNumberStart", DbType.Int32, _OrderNumberStart);
            }
            if (_OrderNumberEnd > 0)
            {
                whereCriteria.Add("b.OrderNumber <= @orderNumberEnd");
                database.AddInParameter(selectCommand, "orderNumberEnd", DbType.Int32, _OrderNumberEnd);
            }

            // ORDER NUMBER RANGE FILTER
            IdRangeParser orderNumberRangeFilter = new IdRangeParser("b.OrderNumber", _OrderNumberRange, "on");
            if (orderNumberRangeFilter.RangeCount > 0)
            {
                whereCriteria.Add(orderNumberRangeFilter.GetSqlString(string.Empty));
                orderNumberRangeFilter.AddParameters(database, selectCommand);
            }

            //ADD ORDER STATUS FILTER
            if (this.OrderStatus.Count > 0)
            {
                if (OrderStatus.Count > 1)
                {
                    whereCriteria.Add("ac_Orders.OrderStatusId IN ('" + this.OrderStatus.ToList("','") + "')");
                }
                else
                {
                    whereCriteria.Add("ac_Orders.OrderStatusId = @orderStatusId");
                    database.AddInParameter(selectCommand, "orderStatusId", DbType.Int32, this.OrderStatus[0]);
                }
            }

            

            //ADD PAYMENT STATUS FILTER
            if (this.PaymentStatus != OrderPaymentStatus.Unspecified)
            {
                whereCriteria.Add("ac_Orders.PaymentStatusId = @paymentStatus");
                database.AddInParameter(selectCommand, "paymentStatus", System.Data.DbType.Byte, this.PaymentStatus);
            }

            //ADD SHIPMENT STATUS FILTER
            if (this.ShipmentStatus != OrderShipmentStatus.Unspecified)
            {
                whereCriteria.Add("ac_Orders.ShipmentStatusId = @shipmentStatus");
                database.AddInParameter(selectCommand, "shipmentStatus", System.Data.DbType.Byte, this.ShipmentStatus);
            }

            if ((MakerShop.Common.Token.Instance.User.IsInGroup(MakerShop.Users.GroupDataSource.LoadForName("Gateway Service admin").GroupId))
                ||(MakerShop.Common.Token.Instance.User.IsInGroup(MakerShop.Users.GroupDataSource.LoadForName("Gateway Service").GroupId)))
            {
                MatchCriteria mc = new MatchCriteria();
                mc.FieldName = "PaymentMethodName";
                foreach (Users.UserSetting us in MakerShop.Common.Token.Instance.User.Settings)
                {
                    if (us.FieldName.ToUpper() == "Gateway".ToUpper())
                    {
                        mc.FieldValue = "%" + us.FieldValue + "%";
                        break;
                    }
                }
                this.Filter.Add(mc);
                whereCriteria.Add("ac_Payments.PaymentStatusid != 0");

                foreach (Users.UserSetting us in MakerShop.Common.Token.Instance.User.Settings)
                {
                    if (us.FieldName.ToUpper() == "GatewayId".ToUpper())
                    {
                        whereCriteria.Add("ac_Payments.PaymentGatewayId in (" + us.FieldValue + ") ");
                    }
                }
            }

            foreach (MatchCriteria filter in this.Filter)
            {
                string tableName = string.Empty;
                string columnName = string.Empty;
                ParseFieldName(filter.FieldName, out tableName, out columnName);
                if (!string.IsNullOrEmpty(tableName))
                {
                    string fieldValue = StringHelper.FixSearchPattern(filter.FieldValue);
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        bool addField = false;
                       
                        string sqlOperator = " = ";
                        if (fieldValue.Contains("%") || fieldValue.Contains("_")) sqlOperator = " LIKE ";
                        switch (tableName)
                        {
                            case "ac_Orders":
                                addField = true;
                                if (columnName == "FullName")
                                {
                                    if (fieldValue.Split(',').Length > 1)
                                    {
                                        whereCriteria.Add(tableName + ".BillToFirstName" + sqlOperator + "@BillToFirstName");
                                        database.AddInParameter(selectCommand, "BillToFirstName", DbType.String, "%" + fieldValue.Split(',')[1].Trim());

                                        filter.FieldName = "BillToLastName";
                                        columnName = "BillToLastName";
                                        fieldValue = "%" + fieldValue.Split(',')[0].Trim() + "%";
                                    }
                                    else
                                    {
                                        filter.FieldName = "BillToLastName";
                                        columnName = "BillToLastName";
                                        fieldValue = "%" + fieldValue.Trim() + "%";

                                    }
                                }

                                break;

                            case "ac_Orders_ex":
                                addPhoneNumber = true;

                                whereORCriteria.Add("ac_Orders.BillToPhone" + sqlOperator + "@BillToPhone");
                                database.AddInParameter(selectCommand, "@BillToPhone", DbType.String, "%" + fieldValue.Trim() + "%");


                                whereORCriteria.Add(tableName + ".CellPhone" + sqlOperator + "@CellPhone");
                                database.AddInParameter(selectCommand, "CellPhone", DbType.String, "%" + fieldValue.Trim() + "%");

                                whereORCriteria.Add(tableName + ".HomePhone" + sqlOperator + "@HomePhone");
                                database.AddInParameter(selectCommand, "HomePhone", DbType.String, "%" + fieldValue.Trim() + "%");

                                break;

                            case "ac_OrderShipments":
                                addField = true;

                                if (!tables.Contains("ac_OrderShipments"))
                                {
                                    tables.Add("ac_OrderShipments");
                                    tableJoins.Add("ac_Orders.OrderId = ac_OrderShipments.OrderId");
                                }
                                break;
                            case "ac_TrackingNumbers":
                                addField = true;

                                if (!tables.Contains("ac_OrderShipments"))
                                {
                                    tables.Add("ac_OrderShipments");
                                    tableJoins.Add("ac_Orders.OrderId = ac_OrderShipments.OrderId");
                                }
                                if (!tables.Contains("ac_TrackingNumbers"))
                                {
                                    tables.Add("ac_TrackingNumbers");
                                    tableJoins.Add("ac_TrackingNumbers.OrderShipmentId = ac_OrderShipments.OrderShipmentId");
                                }
                                break;
                            case "ac_OrderNotes":
                                addField = true;

                                if (!tables.Contains("ac_OrderNotes"))
                                {
                                    tables.Add("ac_OrderNotes");
                                    tableJoins.Add("ac_Orders.OrderId = ac_OrderNotes.OrderId");
                                }
                                break;
                            case "ac_Payments":
                                addField = true;

                                if (!tables.Contains("ac_Payments"))
                                {
                                    tables.Add("ac_Payments");
                                    tableJoins.Add("ac_Orders.OrderId = ac_Payments.OrderId");
                                }
                                if (filter.FieldName.ToLower() == "referencenumber")
                                    if (!fieldValue.StartsWith("x"))
                                        fieldValue = "x" + fieldValue;
                                break;
                            case "ac_Transactions":
                                addField = true;

                                if (!tables.Contains("ac_Payments"))
                                {
                                    tables.Add("ac_Payments");
                                   // tableJoins.Add("ac_Orders.OrderId = ac_Payments.OrderId"); // order search
                                    tableJoins.Add("b.OrderId = ac_Payments.OrderId");
                                }
                                if (!tables.Contains("ac_Transactions"))
                                {
                                    tables.Add("ac_Transactions");
                                    tableJoins.Add("ac_Transactions.PaymentId = ac_Payments.PaymentId");
                                }
                                break;
                        }
                        if (addField)
                        {
                            whereCriteria.Add(tableName + "." + columnName + sqlOperator + "@" + filter.FieldName);
                            database.AddInParameter(selectCommand, filter.FieldName, DbType.String, fieldValue);
                        }
                    }
                }
            }



            StringBuilder sqlBuilder = new StringBuilder();
            if (!count)
            {

                sqlBuilder.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT DISTINCT TOP 200 b.OrderId, ");
                sqlBuilder.Append("b.OrderNumber, b.OrderDate,b.StoreId, b.UserId, b.AffiliateId,");
                sqlBuilder.Append("b.SubAffiliate, b.BillToFirstName, b.BillToLastName, b.BillToCompany,");
                sqlBuilder.Append("b.BillToAddress1, b.BillToAddress2,b.BillToCity, b.BillToProvince, ");
                sqlBuilder.Append("b.BillToPostalCode, b.BillToCountryCode,b.BillToPhone, b.BillToFax, b.BillToEmail,");
                sqlBuilder.Append("b.ProductSubtotal, b.TotalCharges,b.TotalPayments, b.OrderStatusId, b.Exported,");
                sqlBuilder.Append("b.RemoteIP, b.Referrer, b.GoogleOrderNumber, b.PaymentStatusId, b.ShipmentStatusId");
            }

            else
            {
                sqlBuilder.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) As OrderCount ");
            }
            //sqlBuilder.Append(" FROM " + string.Join(",", tables.ToArray()) + " ");
            // sqlBuilder.Append("WHERE " + string.Join(" AND ", whereCriteria.ToArray()) + " ");


            sqlBuilder.Append(" FROM xm_OrderDetail od, " + string.Join(",", tables.ToArray()) + ",  ac_Orders b ");

            if (addPhoneNumber == true)
                sqlBuilder.Append(" LEFT JOIN  ac_Orders_ex ON b.OrderId = ac_Orders_ex.OrderId ");


            sqlBuilder.Append("WHERE " + string.Join(" AND ", whereCriteria.ToArray()) + " ");

            if(whereORCriteria.Count >0)
                sqlBuilder.Append(" AND 1 = CASE WHEN " + string.Join(" OR ", whereORCriteria.ToArray()) + " THEN 1 ELSE 0 END ");


            sqlBuilder.Append("AND ac_Orders.OrderId = od.ParentOrderId  ");
            sqlBuilder.Append("AND b.OrderId = ISNULL(od.OrderId , ac_Orders.OrderId)");



            if (tableJoins.Count > 0) sqlBuilder.Append(" AND " + string.Join(" AND ", tableJoins.ToArray()) + " ");
            //add in sort expression


            if (!count) sqlBuilder.Append("ORDER BY " + sortExpression);

            //SET SQL STRING AND RETURN COMMAND
            selectCommand.CommandText = sqlBuilder.ToString();
            return selectCommand;
        }

        private void ParseFieldName(string fieldName, out string tableName, out string columnName)
        {
            switch (fieldName.ToLowerInvariant())
            {
                case "billtofirstname":
                case "billtocompany":
                case "billtophone":
                    tableName = "ac_Orders_ex";
                    columnName = fieldName;
                    break;
                case "billtoaddress1":
                case "billtoaddress2":
                case "billtocity":
                case "billtoprovince":
                case "billtocountrycode":
                case "billtoemail":
                    tableName = "ac_Orders";
                    columnName = fieldName;
                    break;
                case "fullname":
                    tableName = "ac_Orders";
                    columnName = "FullName";
                    break;
                case "shiptofirstname":
                case "shiptolastname":
                case "shiptocompany":
                case "shiptophone":
                case "shiptoaddress1":
                case "shiptoaddress2":
                case "shiptocity":
                case "shiptoprovince":
                case "shiptocountrycode":
                    tableName = "ac_OrderShipments";
                    columnName = fieldName;
                    break;
                case "ordernotes":
                    tableName = "ac_OrderNotes";
                    columnName = "Comment";
                    break;
                case "referencenumber":
                case "paymentmethodname":
                    tableName = "ac_Payments";
                    columnName = fieldName;
                    break;
                case "providertransactionid":
                    tableName = "ac_Transactions";
                    columnName = fieldName;
                    break;
                case "trackingnumberdata":
                    tableName = "ac_TrackingNumbers";
                    columnName = fieldName;
                    break;
                default:
                    tableName = string.Empty;
                    columnName = string.Empty;
                    break;
            }
        }
    }
}
