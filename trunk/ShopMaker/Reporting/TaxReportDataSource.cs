//-----------------------------------------------------------------------
// <copyright file="TaxReportDataSource.cs" company="Able Solutions Corporation">
//     Copyright (c) 2009 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MakerShop.Reporting
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Text;
    using MakerShop.Common;
    using MakerShop.Data;
    using MakerShop.Orders;
    using MakerShop.Taxes;
    using MakerShop.Utility;

    /// <summary>
    /// Datasource class for generating tax reports
    /// </summary>
    public class TaxReportDataSource
    {
        /// <summary>
        /// Determines whether a tax name is valid for use in detail queries
        /// </summary>
        /// <param name="taxName">Name of the tax</param>
        /// <returns>True if the tax name is present in the order records, false otherwise.</returns>
        public static bool IsTaxNameValid(string taxName)
        {
            if (string.IsNullOrEmpty(taxName)) return false;
            Database database = Token.Instance.Database;
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TaxNameCount");
            selectQuery.Append(" FROM ac_Orders O INNER JOIN ac_OrderItems OI ON O.OrderId = OI.OrderId");
            selectQuery.Append(" WHERE O.StoreId = @storeId");
            selectQuery.Append(" AND OI.OrderItemTypeId = " + ((int)OrderItemType.Tax).ToString());
            selectQuery.Append(" AND OI.Name = @taxName");
            List<OrderStatus> reportStatuses = OrderStatusDataSource.GetReportStatuses();
            selectQuery.Append(" AND " + ReportDataSource.GetStatusFilter(reportStatuses));
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@taxName", System.Data.DbType.String, taxName);
            ReportDataSource.SetStatusFilterParams(reportStatuses, database, selectCommand);
            return (MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand)) > 0);
        }

        /// <summary>
        /// Counts the number of TaxReportSummaryItem objects in result if retrieved using the given date range.
        /// </summary>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <returns>The number of TaxReportSummaryItem objects present wtihin the date range.</returns>
        public static int CountSummaries(DateTime startDate, DateTime endDate)
        {
            Database database = Token.Instance.Database;
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS SummaryCount FROM (SELECT DISTINCT OI.Name");
            selectQuery.Append(" FROM ac_Orders O INNER JOIN ac_OrderItems OI ON O.OrderId = OI.OrderId");
            selectQuery.Append(" WHERE O.StoreId = @storeId");
            selectQuery.Append(" AND OI.OrderItemTypeId = " + ((int)OrderItemType.Tax).ToString());
            if (startDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                startDate = LocaleHelper.FromLocalTime(startDate);
                selectQuery.Append(" AND O.OrderDate >= @startDate");
            }
            if (endDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                endDate = LocaleHelper.FromLocalTime(endDate);
                selectQuery.Append(" AND O.OrderDate <= @endDate");
            }
            List<OrderStatus> reportStatuses = OrderStatusDataSource.GetReportStatuses();
            selectQuery.Append(" AND " + ReportDataSource.GetStatusFilter(reportStatuses) + ") AS DistinctTaxes");
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            ReportDataSource.SetStatusFilterParams(reportStatuses, database, selectCommand);
            if (startDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, startDate);
            if (endDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, endDate);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of TaxReportSummaryItem for the given date range.
        /// </summary>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <returns>A collection of TaxReportSummaryItem objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<TaxReportSummaryItem> LoadSummaries(DateTime startDate, DateTime endDate)
        {
            return LoadSummaries(startDate, endDate, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of TaxReportSummaryItem for the given date range.
        /// </summary>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of TaxReportSummaryItem objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<TaxReportSummaryItem> LoadSummaries(DateTime startDate, DateTime endDate, string sortExpression)
        {
            return LoadSummaries(startDate, endDate, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of TaxReportSummaryItem for the given date range.
        /// </summary>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of TaxReportSummaryItem objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<TaxReportSummaryItem> LoadSummaries(DateTime startDate, DateTime endDate, int maximumRows, int startRowIndex)
        {
            return LoadSummaries(startDate, endDate, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of TaxReportSummaryItem for the given date range.
        /// </summary>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of TaxReportSummaryItem objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<TaxReportSummaryItem> LoadSummaries(DateTime startDate, DateTime endDate, int maximumRows, int startRowIndex, string sortExpression)
        {
            // LOAD ALL TAX CODES FOR THE STORE
            Dictionary<string, int> taxRuleLookup = LoadTaxRuleLookup();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" OI.Name AS TaxName,SUM(OI.Price * OI.Quantity) AS TaxAmount");
            selectQuery.Append(" FROM ac_Orders O INNER JOIN ac_OrderItems OI ON O.OrderId = OI.OrderId");
            selectQuery.Append(" WHERE O.StoreId = @storeId");
            selectQuery.Append(" AND OI.OrderItemTypeId = " + ((int)OrderItemType.Tax).ToString());
            if (startDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                startDate = LocaleHelper.FromLocalTime(startDate);
                selectQuery.Append(" AND O.OrderDate >= @startDate");
            }
            if (endDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                endDate = LocaleHelper.FromLocalTime(endDate);
                selectQuery.Append(" AND O.OrderDate <= @endDate");
            }
            //GET INVALID ORDER STATUSES
            List<OrderStatus> reportStatuses = OrderStatusDataSource.GetReportStatuses();
            selectQuery.Append(" AND " + ReportDataSource.GetStatusFilter(reportStatuses));
            selectQuery.Append(" GROUP BY OI.Name");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            ReportDataSource.SetStatusFilterParams(reportStatuses, database, selectCommand);
            if (startDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, startDate);
            if (endDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, endDate);
            //EXECUTE THE COMMAND
            List<TaxReportSummaryItem> results = new List<TaxReportSummaryItem>();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        TaxReportSummaryItem summary = new TaxReportSummaryItem();
                        summary.TaxName = dr.GetString(0);
                        summary.TaxAmount = NullableData.GetDecimal(dr, 1);
                        summary.TaxRuleId = GetTaxRuleId(taxRuleLookup, summary.TaxName);
                        results.Add(summary);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of TaxReportDetailItem objects in result if retrieved using the given date range.
        /// </summary>
        /// <param name="taxName">Name of tax to load detail for</param>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <returns>The number of TaxReportDetailItem objects present wtihin the date range.</returns>
        public static int CountDetail(string taxName, DateTime startDate, DateTime endDate)
        {
            Database database = Token.Instance.Database;
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS DetailCount FROM (SELECT DISTINCT O.OrderId");
            selectQuery.Append(" FROM ac_Orders O INNER JOIN ac_OrderItems OI ON O.OrderId = OI.OrderId");
            selectQuery.Append(" WHERE O.StoreId = @storeId");
            selectQuery.Append(" AND OI.OrderItemTypeId = " + ((int)OrderItemType.Tax).ToString());
            selectQuery.Append(" AND OI.Name = @taxName");
            if (startDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                startDate = LocaleHelper.FromLocalTime(startDate);
                selectQuery.Append(" AND O.OrderDate >= @startDate");
            }
            if (endDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                endDate = LocaleHelper.FromLocalTime(endDate);
                selectQuery.Append(" AND O.OrderDate <= @endDate");
            }
            List<OrderStatus> reportStatuses = OrderStatusDataSource.GetReportStatuses();
            selectQuery.Append(" AND " + ReportDataSource.GetStatusFilter(reportStatuses) + ") AS DistinctTaxes");
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@taxName", System.Data.DbType.String, taxName);
            ReportDataSource.SetStatusFilterParams(reportStatuses, database, selectCommand);
            if (startDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, startDate);
            if (endDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, endDate);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of TaxReportDetailItem for the given date range.
        /// </summary>
        /// <param name="taxName">Name of tax to load detail for</param>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <returns>A collection of TaxReportDetailItem objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<TaxReportDetailItem> LoadDetail(string taxName, DateTime startDate, DateTime endDate)
        {
            return LoadDetail(taxName, startDate, endDate, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of TaxReportDetailItem for the given date range.
        /// </summary>
        /// <param name="taxName">Name of tax to load detail for</param>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of TaxReportDetailItem objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<TaxReportDetailItem> LoadDetail(string taxName, DateTime startDate, DateTime endDate, string sortExpression)
        {
            return LoadDetail(taxName, startDate, endDate, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of TaxReportDetailItem for the given date range.
        /// </summary>
        /// <param name="taxName">Name of tax to load detail for</param>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of TaxReportDetailItem objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<TaxReportDetailItem> LoadDetail(string taxName, DateTime startDate, DateTime endDate, int maximumRows, int startRowIndex)
        {
            return LoadDetail(taxName, startDate, endDate, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of TaxReportDetailItem for the given date range.
        /// </summary>
        /// <param name="taxName">Name of tax to load detail for</param>
        /// <param name="startDate">Inclusive start date to consider</param>
        /// <param name="endDate">Inclusive end date to consider</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of TaxReportDetailItem objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<TaxReportDetailItem> LoadDetail(string taxName, DateTime startDate, DateTime endDate, int maximumRows, int startRowIndex, string sortExpression)
        {
            // LOAD ALL TAX CODES FOR THE STORE
            Dictionary<string, int> taxRuleLookup = LoadTaxRuleLookup();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" O.OrderId,O.OrderNumber,O.OrderDate,SUM(OI.Price * OI.Quantity) AS TaxAmount");
            selectQuery.Append(" FROM ac_Orders O INNER JOIN ac_OrderItems OI ON O.OrderId = OI.OrderId");
            selectQuery.Append(" WHERE O.StoreId = @storeId");
            selectQuery.Append(" AND OI.OrderItemTypeId = " + ((int)OrderItemType.Tax).ToString());
            selectQuery.Append(" AND OI.Name = @taxName");
            if (startDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                startDate = LocaleHelper.FromLocalTime(startDate);
                selectQuery.Append(" AND O.OrderDate >= @startDate");
            }
            if (endDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                endDate = LocaleHelper.FromLocalTime(endDate);
                selectQuery.Append(" AND O.OrderDate <= @endDate");
            }
            //GET INVALID ORDER STATUSES
            List<OrderStatus> reportStatuses = OrderStatusDataSource.GetReportStatuses();
            selectQuery.Append(" AND " + ReportDataSource.GetStatusFilter(reportStatuses));
            selectQuery.Append(" GROUP BY O.OrderId,O.OrderNumber,O.OrderDate");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@taxName", System.Data.DbType.String, taxName);
            ReportDataSource.SetStatusFilterParams(reportStatuses, database, selectCommand);
            if (startDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, startDate);
            if (endDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, endDate);
            //EXECUTE THE COMMAND
            List<TaxReportDetailItem> results = new List<TaxReportDetailItem>();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        TaxReportDetailItem summary = new TaxReportDetailItem();
                        summary.OrderId = dr.GetInt32(0);
                        summary.OrderNumber = dr.GetInt32(1);
                        summary.OrderDate = dr.GetDateTime(2);
                        summary.TaxAmount = NullableData.GetDecimal(dr, 3);
                        summary.TaxName = taxName;
                        summary.TaxRuleId = GetTaxRuleId(taxRuleLookup, taxName);
                        results.Add(summary);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        private static Dictionary<string, int> LoadTaxRuleLookup()
        {
            Dictionary<string, int> taxRuleLookup = new Dictionary<string, int>();
            TaxRuleCollection taxRules = TaxRuleDataSource.LoadForStore();
            foreach (TaxRule taxRule in taxRules)
            {
                string name = taxRule.Name.ToLowerInvariant();
                taxRuleLookup[name] = taxRule.TaxRuleId;
            }
            return taxRuleLookup;
        }

        private static int GetTaxRuleId(Dictionary<string, int> taxRuleLookup, string taxName)
        {
            if (taxRuleLookup == null || taxRuleLookup.Count == 0) return 0;
            if (string.IsNullOrEmpty(taxName)) return 0;
            string cleanName = taxName.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(cleanName)) return 0;
            if (taxRuleLookup.ContainsKey(cleanName)) return taxRuleLookup[cleanName];
            return 0;
        }
    }
}
