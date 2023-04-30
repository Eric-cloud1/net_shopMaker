using System;
using System.Data;
using System.Data.Common;
using System.ComponentModel;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Catalog;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using MakerShop.Products;

namespace MakerShop.Reporting
{
    /// <summary>
    /// DataSource class for PageView objects
    /// </summary>
    [DataObject(true)]
    public partial class PageViewDataSource
    {

        /// <summary>
        /// Get Current Sales Data by date
        /// </summary>
        public static DataSet GetProductStatusoverall(DateTime startDate, DateTime endDate, int productId)
        {
            MakerShop.Data.Database database = Token.Instance.Database;


            DataSet productStatusoverall = new DataSet();

            using (System.Data.Common.DbCommand getCommand = database.GetStoredProcCommand("rpt_ProductStatusoverall"))
            {
                database.AddInParameter(getCommand, "@startDate", System.Data.DbType.DateTime, startDate);
                database.AddInParameter(getCommand, "@endDate", System.Data.DbType.DateTime, endDate);
                database.AddInParameter(getCommand, "@productId", System.Data.DbType.Int32);
          
                productStatusoverall = database.ExecuteDataSet(getCommand);
             
            }

            return productStatusoverall;
        }



        /// <summary>
        /// Get Current Sales Data by date
        /// </summary>
        public static DataSet GetAbandonSales(DateTime currentDate, DateTime endDate, out int alert)
        {
            MakerShop.Data.Database database = Token.Instance.Database;


            DataSet abandonnedSales = new DataSet();

            using (System.Data.Common.DbCommand getCommand = database.GetStoredProcCommand("rpt_AbandonedSales"))
            {
                database.AddInParameter(getCommand, "@Currentdate", System.Data.DbType.DateTime, currentDate);
                database.AddInParameter(getCommand, "@EndDate", System.Data.DbType.DateTime, endDate);
                database.AddInParameter(getCommand, "@Alert", System.Data.DbType.Int32);
                getCommand.Parameters["@Alert"].Direction = ParameterDirection.Output;

                abandonnedSales = database.ExecuteDataSet(getCommand);
                alert = (int)getCommand.Parameters["@Alert"].Value;
            }

            return abandonnedSales;
        }


        /// <summary>
        /// Get Current Sales Data by date
        /// </summary>
        public static DataSet GetCurrentFailedSales(DateTime currentDate, DateTime endDate, out int alert)
        {
            MakerShop.Data.Database database = Token.Instance.Database;
            DataSet failedSales = new DataSet();

            using (System.Data.Common.DbCommand getCommand = database.GetStoredProcCommand("rpt_CurrentFailedSales"))
            {
                database.AddInParameter(getCommand, "@Currentdate", System.Data.DbType.DateTime, currentDate);
                database.AddInParameter(getCommand, "@EndDate", System.Data.DbType.DateTime, endDate);
                database.AddInParameter(getCommand, "@Alert", System.Data.DbType.Int32);
                getCommand.Parameters["@Alert"].Direction = ParameterDirection.Output;

                failedSales = database.ExecuteDataSet(getCommand);
                alert = (int)getCommand.Parameters["@Alert"].Value;
            }

            return failedSales;
        }


        /// <summary>
        /// Get Current Sales Data by date
        /// </summary>
        public static DataSet GetCurrentSales(DateTime currentDate, DateTime endDate, out int alert)
        {
            MakerShop.Data.Database database = Token.Instance.Database;

            DataSet salesCurrent = new DataSet();

            using (System.Data.Common.DbCommand getCommand = database.GetStoredProcCommand("rpt_CurrentSales"))
            {
                database.AddInParameter(getCommand, "@Currentdate", System.Data.DbType.DateTime, currentDate);
                database.AddInParameter(getCommand, "@EndDate", System.Data.DbType.DateTime, endDate);
                database.AddInParameter(getCommand, "@Alert", System.Data.DbType.Int32);
                getCommand.Parameters["@Alert"].Direction = ParameterDirection.Output;

                salesCurrent = database.ExecuteDataSet(getCommand);
                alert = (int)getCommand.Parameters["@Alert"].Value;
            }

            return salesCurrent;
        }

        public static DataSet GetForecast(DateTime forecastDate, DateTime endDate, out int alert)
        {
            MakerShop.Data.Database database = Token.Instance.Database;


            DataSet forecastdataset = new DataSet();

            using (System.Data.Common.DbCommand getCommand = database.GetStoredProcCommand("rpt_ForecastChart"))
            {
                database.AddInParameter(getCommand, "@startdate", System.Data.DbType.DateTime, forecastDate);
                database.AddInParameter(getCommand, "@EndDate", System.Data.DbType.DateTime, endDate);
                database.AddInParameter(getCommand, "@Alert", System.Data.DbType.Int32);
                getCommand.Parameters["@Alert"].Direction = ParameterDirection.Output;

                forecastdataset = database.ExecuteDataSet(getCommand);
                alert = (int)getCommand.Parameters["@Alert"].Value;
            }

            return forecastdataset;
        }


        public static DataSet GetConversion(DateTime forecastDate, DateTime endDate, out int alert)
        {
            MakerShop.Data.Database database = Token.Instance.Database;


            DataSet conversiondataset = new DataSet();

            using (System.Data.Common.DbCommand getCommand = database.GetStoredProcCommand("rpt_ConversionPerformanceChart"))
            {
                database.AddInParameter(getCommand, "@startdate", System.Data.DbType.DateTime, forecastDate);
                database.AddInParameter(getCommand, "@EndDate", System.Data.DbType.DateTime, endDate);
                database.AddInParameter(getCommand, "@Alert", System.Data.DbType.Int32);
                getCommand.Parameters["@Alert"].Direction = ParameterDirection.Output;

                conversiondataset = database.ExecuteDataSet(getCommand);
                alert = (int)getCommand.Parameters["@Alert"].Value;
            }

            return conversiondataset;
        }

        /// <summary>
        /// Deletes all PageView records from the database
        /// </summary>
        public static void DeleteAll()
        {
            int storeId = Token.Instance.StoreId;
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("DELETE FROM ac_PageViews WHERE StoreId = @storeId");
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, storeId);
            database.ExecuteScalar(selectCommand);
        }

        /// <summary>
        /// Loads the page views for the specified node type and user.
        /// </summary>
        /// <param name="catalogNodeType">The type of catalog node views to load</param>
        /// <param name="userId">The user to filter views by; pass 0 for all users</param>
        /// <returns>A PageViewCollection that contains all views meeting the criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PageViewCollection LoadForCatalogNodeType(CatalogNodeType catalogNodeType, int userId)
        {
            return LoadForCatalogNodeType(catalogNodeType, userId, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads the page views for the specified node type and user.
        /// </summary>
        /// <param name="catalogNodeType">The type of catalog node views to load</param>
        /// <param name="userId">The user to filter views by; pass 0 for all users</param>
        /// <param name="sortExpression">Sort expression to apply to the result set</param>
        /// <returns>A PageViewCollection that contains all views meeting the criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PageViewCollection LoadForCatalogNodeType(CatalogNodeType catalogNodeType, int userId, string sortExpression)
        {
            return LoadForCatalogNodeType(catalogNodeType, userId, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads the page views for the specified node type and user.
        /// </summary>
        /// <param name="catalogNodeType">The type of catalog node views to load</param>
        /// <param name="userId">The user to filter views by; pass 0 for all users</param>
        /// <param name="maximumRows">Maximum number of rows to return</param>
        /// <param name="startRowIndex">Starting index of first record in result set</param>
        /// <returns>A PageViewCollection that contains all views meeting the criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PageViewCollection LoadForCatalogNodeType(CatalogNodeType catalogNodeType, int userId, int maximumRows, int startRowIndex)
        {
            return LoadForCatalogNodeType(catalogNodeType, userId, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads the page views for the specified node type and user.
        /// </summary>
        /// <param name="catalogNodeType">The type of catalog node views to load</param>
        /// <param name="userId">The user to filter views by; pass 0 for all users</param>        
        /// <param name="maximumRows">Maximum number of rows to return</param>
        /// <param name="startRowIndex">Starting index of first record in result set</param>
        /// <param name="sortExpression">Sort expression to apply to the result set</param>
        /// <returns>A PageViewCollection that contains all views meeting the criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PageViewCollection LoadForCatalogNodeType(CatalogNodeType catalogNodeType, int userId, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + PageView.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_PageViews");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" AND CatalogNodeTypeId = @catalogNodeTypeId");
            if (userId != 0) selectQuery.Append(" AND UserId = @userId");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@catalogNodeTypeId", DbType.Byte, catalogNodeType);
            if (userId != 0) database.AddInParameter(selectCommand, "@userId", DbType.Int32, userId); ;
            //EXECUTE THE COMMAND
            PageViewCollection results = new PageViewCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        PageView pageView = new PageView();
                        PageView.LoadDataReader(pageView, dr);
                        results.Add(pageView);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Gets a collection of Key Value pairs; each key representing a ICatalogable (Category) object 
        /// and each value representing the number of views for this object. The result contains 
        /// views by category.
        /// </summary>
        /// <returns>A collection of Key Value pairs; each key representing a ICatalogable (Category) object 
        /// and each value representing the number of views for this object</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SortableCollection<KeyValuePair<ICatalogable, int>> GetViewsByCategory()
        {
            return GetViewsByCategory(0, 0, string.Empty);
        }

        /// <summary>
        /// Gets a collection of Key Value pairs; each key representing a ICatalogable (Category) object 
        /// and each value representing the number of views for this object. The result contains 
        /// views by category.
        /// </summary>
        /// <param name="sortExpression">Expression used for sorting the results</param>
        /// <returns>A collection of Key Value pairs; each key representing a ICatalogable (Category) object 
        /// and each value representing the number of views for this object</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SortableCollection<KeyValuePair<ICatalogable, int>> GetViewsByCategory(string sortExpression)
        {
            return GetViewsByCategory(0, 0, sortExpression);
        }

        /// <summary>
        /// Gets a collection of Key Value pairs; each key representing a ICatalogable (Category) object 
        /// and each value representing the number of views for this object. The result contains 
        /// views by category.
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Index of the row from where to start retrieving</param>
        /// <returns>A collection of Key Value pairs; each key representing a ICatalogable (Category) object 
        /// and each value representing the number of views for this object</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SortableCollection<KeyValuePair<ICatalogable, int>> GetViewsByCategory(int maximumRows, int startRowIndex)
        {
            return GetViewsByCategory(maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Gets a collection of Key Value pairs; each key representing a ICatalogable (Category) object 
        /// and each value representing the number of views for this object. The result contains 
        /// views by category.
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Index of the row from where to start retrieving</param>
        /// <param name="sortExpression">Expression used for sorting the results</param>
        /// <returns>A collection of Key Value pairs; each key representing a ICatalogable (Category) object 
        /// and each value representing the number of views for this object</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SortableCollection<KeyValuePair<ICatalogable, int>> GetViewsByCategory(int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" COUNT(1) as ViewCount, C.CategoryId, C.Name");
            selectQuery.Append(" FROM ac_PageViews V INNER JOIN ac_Categories C");
            selectQuery.Append(" ON V.CatalogNodeId = C.CategoryId");            
            selectQuery.Append(" WHERE V.CatalogNodeTypeId = @catalogNodeTypeId");            
            selectQuery.Append(" GROUP BY C.CategoryId, C.Name");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@catalogNodeTypeId", DbType.Byte, CatalogNodeType.Category);
            //EXECUTE THE COMMAND
            SortableCollection<KeyValuePair<ICatalogable, int>> results = new SortableCollection<KeyValuePair<ICatalogable, int>>();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        ICatalogable catalogNode = CatalogDataSource.Load(NullableData.GetInt32(dr, 1), CatalogNodeType.Category);
                        if (catalogNode != null)
                        {
                            results.Add(new KeyValuePair<ICatalogable, int>(catalogNode, dr.GetInt32(0)));
                        }
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Gets the number of results in GetViewsByCategory method
        /// </summary>
        /// <returns>Number of results in GetViewsByCategory method</returns>
        public static int GetViewsByCategoryCount()
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT (DISTINCT C.CategoryId) AS TotalRows");
            selectQuery.Append(" FROM ac_PageViews V INNER JOIN ac_Categories C");
            selectQuery.Append(" ON V.CatalogNodeId = C.CategoryId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }


        /// <summary>
        /// Gets a collection of Key Value pairs; each key representing a ICatalogable (Product) object 
        /// and each value representing the number of views for this object. The result contains 
        /// views by product.
        /// </summary>
        /// <returns>A collection of Key Value pairs; each key representing a ICatalogable (Product) object 
        /// and each value representing the number of views for this object</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SortableCollection<KeyValuePair<ICatalogable, int>> GetViewsByProduct()
        {
            return GetViewsByProduct(0, 0, string.Empty);
        }

        /// <summary>
        /// Gets a collection of Key Value pairs; each key representing a ICatalogable (Product) object 
        /// and each value representing the number of views for this object. The result contains 
        /// views by product.
        /// </summary>
        /// <param name="sortExpression">Expression used for sorting the results</param>
        /// <returns>A collection of Key Value pairs; each key representing a ICatalogable (Product) object 
        /// and each value representing the number of views for this object</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SortableCollection<KeyValuePair<ICatalogable, int>> GetViewsByProduct(string sortExpression)
        {
            return GetViewsByProduct(0, 0, sortExpression);
        }

        /// <summary>
        /// Gets a collection of Key Value pairs; each key representing a ICatalogable (Product) object 
        /// and each value representing the number of views for this object. The result contains 
        /// views by product.
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Index of the row from where to start retrieving</param>
        /// <returns>A collection of Key Value pairs; each key representing a ICatalogable (Product) object 
        /// and each value representing the number of views for this object</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SortableCollection<KeyValuePair<ICatalogable, int>> GetViewsByProduct(int maximumRows, int startRowIndex)
        {
            return GetViewsByProduct(maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Gets a collection of Key Value pairs; each key representing a ICatalogable (Product) object 
        /// and each value representing the number of views for this object. The result contains 
        /// views by product.
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Index of the row from where to start retrieving</param>
        /// <param name="sortExpression">Expression used for sorting the results</param>
        /// <returns>A collection of Key Value pairs; each key representing a ICatalogable (Product) object 
        /// and each value representing the number of views for this object</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SortableCollection<KeyValuePair<ICatalogable, int>> GetViewsByProduct(int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" COUNT(1) as ViewCount, P.ProductId, P.Name");
            selectQuery.Append(" FROM ac_PageViews V INNER JOIN ac_Products P");
            selectQuery.Append(" ON V.CatalogNodeId = P.ProductId");
            selectQuery.Append(" WHERE V.CatalogNodeTypeId = @catalogNodeTypeId");
            selectQuery.Append(" GROUP BY P.ProductId, P.Name");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@catalogNodeTypeId", DbType.Byte, CatalogNodeType.Product);
            //EXECUTE THE COMMAND
            SortableCollection<KeyValuePair<ICatalogable, int>> results = new SortableCollection<KeyValuePair<ICatalogable, int>>();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        ICatalogable catalogNode = CatalogDataSource.Load(NullableData.GetInt32(dr, 1), CatalogNodeType.Product);
                        if (catalogNode != null)
                        {
                            results.Add(new KeyValuePair<ICatalogable, int>(catalogNode, dr.GetInt32(0)));
                        }
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Gets the number of results in GetViewsByProduct method
        /// </summary>
        /// <returns>Number of results in GetViewsByProduct method</returns>
        public static int GetViewsByProductCount()
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(DISTINCT P.ProductId) AS TotalRows");
            selectQuery.Append(" FROM ac_PageViews V INNER JOIN ac_Products P");
            selectQuery.Append(" ON V.CatalogNodeId = P.ProductId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Gets a collection of Key Value pairs; each key representing a ICatalogable object 
        /// and each value representing the number of views for this object.
        /// </summary>
        /// <param name="catalogNodeType">The type of catalog node to get views for</param>
        /// <returns>A collection of Key Value pairs; each key representing a ICatalogable object 
        /// and each value representing the number of views for this object.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SortableCollection<KeyValuePair<ICatalogable, int>> GetViews(CatalogNodeType catalogNodeType)
        {
            return GetViews(catalogNodeType, 0, 0, string.Empty);        
        }

        /// <summary>
        /// Gets a collection of Key Value pairs; each key representing a ICatalogable object 
        /// and each value representing the number of views for this object.
        /// </summary>
        /// <param name="catalogNodeType">The type of catalog node to get views for</param>
        /// <param name="sortExpression">The expression that is used to sort the data retrieved.</param>
        /// <returns>A collection of Key Value pairs; each key representing a ICatalogable object 
        /// and each value representing the number of views for this object.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SortableCollection<KeyValuePair<ICatalogable, int>> GetViews(CatalogNodeType catalogNodeType, string sortExpression)
        {
            return GetViews(catalogNodeType, 0, 0, sortExpression);
        }

        /// <summary>
        /// Gets a collection of Key Value pairs; each key representing a ICatalogable object 
        /// and each value representing the number of views for this object.
        /// </summary>
        /// <param name="catalogNodeType">The type of catalog node to get views for</param>
        /// <param name="maximumRows">The maximum number of data rows that should be returned. </param>
        /// <param name="startRowIndex">The starting position that should be used when retrieving data rows.</param>
        /// <returns>A collection of Key Value pairs; each key representing a ICatalogable object 
        /// and each value representing the number of views for this object.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SortableCollection<KeyValuePair<ICatalogable, int>> GetViews(CatalogNodeType catalogNodeType, int maximumRows, int startRowIndex)
        {
            return GetViews(catalogNodeType, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Gets a collection of Key Value pairs; each key representing a ICatalogable object 
        /// and each value representing the number of views for this object.
        /// </summary>
        /// <param name="catalogNodeType">The type of catalog node to get views for</param>
        /// <param name="maximumRows">The maximum number of data rows that should be returned. </param>
        /// <param name="startRowIndex">The starting position that should be used when retrieving data rows.</param>
        /// <param name="sortExpression">The expression that is used to sort the data retrieved.</param>
        /// <returns>A collection of Key Value pairs; each key representing a ICatalogable object 
        /// and each value representing the number of views for this object.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SortableCollection<KeyValuePair<ICatalogable, int>> GetViews(CatalogNodeType catalogNodeType, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            //GET RECORDS STARTING AT FIRST ROW
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" COUNT(1) as ViewCount, CatalogNodeId" );
            selectQuery.Append(" FROM ac_PageViews");
            selectQuery.Append(" WHERE CatalogNodeTypeId = @catalogNodeTypeId");
            selectQuery.Append(" GROUP BY CatalogNodeId");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());            
            database.AddInParameter(selectCommand, "@catalogNodeTypeId", System.Data.DbType.Byte, catalogNodeType);
            //EXECUTE THE COMMAND
            SortableCollection<KeyValuePair<ICatalogable, int>> results = new SortableCollection<KeyValuePair<ICatalogable, int>>();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        ICatalogable catalogNode = CatalogDataSource.Load(NullableData.GetInt32(dr, 1), catalogNodeType);
                        if (catalogNode != null)
                        {
                            results.Add(new KeyValuePair<ICatalogable, int>(catalogNode, dr.GetInt32(0)));
                        }
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Loads a collection of browsers and it's number of views, as a key value pair.
        /// </summary>
        /// <returns>Collection of Key Value pairs where the key is the UserAgent (browser) and the value is the number of views for the browser.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SortableCollection<KeyValuePair<string, int>> GetViewsByBrowser()
        {
            return GetViewsByBrowser(0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of browsers and it's number of views, as a key value pair.
        /// </summary>
        /// <param name="sortExpression">The expression that is used to sort the data retrieved.</param>
        /// <returns>Collection of Key Value pairs where the key is the UserAgent (browser) and the value is the number of views for the browser.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SortableCollection<KeyValuePair<string, int>> GetViewsByBrowser(string sortExpression)
        {
            return GetViewsByBrowser(0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of browsers and it's number of views, as a key value pair.
        /// </summary>
        /// <param name="maximumRows">The maximum number of data rows that should be returned. </param>
        /// <param name="startRowIndex">The starting position that should be used when retrieving data rows.</param>
        /// <returns>Collection of Key Value pairs where the key is the UserAgent (browser) and the value is the number of views for the browser.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SortableCollection<KeyValuePair<string, int>> GetViewsByBrowser(int maximumRows, int startRowIndex)
        {
            return GetViewsByBrowser(maximumRows, startRowIndex, string.Empty);
        }
        
        /// <summary>
        /// Loads a collection of browsers and it's number of views, as a key value pair.
        /// </summary>
        /// <param name="maximumRows">The maximum number of data rows that should be returned. </param>
        /// <param name="startRowIndex">The starting position that should be used when retrieving data rows.</param>
        /// <param name="sortExpression">The expression that is used to sort the data retrieved.</param>
        /// <returns>Collection of Key Value pairs where the key is the UserAgent (browser) and the value is the number of views for the browser.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SortableCollection<KeyValuePair<string, int>> GetViewsByBrowser(int maximumRows, int startRowIndex, string sortExpression)
        {
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" Browser, COUNT(1) as ViewCount");
            selectQuery.Append(" FROM ac_PageViews");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" GROUP BY Browser");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            else selectQuery.Append(" ORDER BY ViewCount DESC");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());            
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            SortableCollection<KeyValuePair<string, int>> results = new SortableCollection<KeyValuePair<string, int>>();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        results.Add(new KeyValuePair<string, int>(dr.GetString(0), dr.GetInt32(1)));
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Gets a count of all unique browsers that have view counts available.
        /// </summary>
        /// <returns>A count of all unique browsers that have view counts available.</returns>
        public static int GetViewsByBrowserCount()
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(DISTINCT Browser) AS TotalRows FROM ac_PageViews");
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Gets a collection of Key Value pairs; each key representing an hour 
        /// and each value representing the number of views during that hour.
        /// </summary>
        /// <param name="includeZeros">If <b>true</b> hours with zero views are also included</param>
        /// <returns>A collection of Key Value pairs; each key representing an hour 
        /// and each value representing the number of views during that hour.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SortableCollection<KeyValuePair<int, int>> GetViewsByHour(bool includeZeros)
        {
            return GetViewsByHour(includeZeros, DateTime.MinValue);
        }

        /// <summary>
        /// Gets a collection of Key Value pairs; each key representing an hour 
        /// and each value representing the number of views during that hour.
        /// </summary>
        /// <param name="includeZeros">If <b>true</b> hours with zero views are also included</param>
        /// <param name="fromDate">DateTime from which to start getting the views information</param>
        /// <returns>A collection of Key Value pairs; each key representing an hour 
        /// and each value representing the number of views during that hour.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SortableCollection<KeyValuePair<int, int>> GetViewsByHour(bool includeZeros, DateTime fromDate)
        {
            //FIX THE HOURS FOR THE TIMEZONE OFFSET
            //THIS IS IMPERFECT BECAUSE WE CANNOT TAKE INTO ACCOUNT PARTIAL HOURS
            //SO CAST THE TIMEZONE OFFSET TO AN INTEGER
            int hourOffset = (int)Token.Instance.Store.TimeZoneOffset;
            //GET VIEWS BY HOUR
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT DATEPART(hh, DATEADD(hh, " + hourOffset + ", ActivityDate)) AS ViewHour, COUNT(1) as ViewCount");
            selectQuery.Append(" FROM ac_PageViews");
            selectQuery.Append(" WHERE StoreId = @storeId");
            if (fromDate > System.DateTime.MinValue) selectQuery.Append(" AND ActivityDate > @fromDate");
            selectQuery.Append(" GROUP BY DATEPART(hh, DATEADD(hh, " + hourOffset + ", ActivityDate))");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            if (fromDate > System.DateTime.MinValue) database.AddInParameter(selectCommand, "@fromDate", DbType.DateTime, fromDate);
            //EXECUTE THE COMMAND
            SortableCollection<KeyValuePair<int, int>> sparseArray = new SortableCollection<KeyValuePair<int, int>>();
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    //CONVERT HOUR TO LOCAL TIME
                    int hour = dr.GetInt32(0);
                    sparseArray.Add(new KeyValuePair<int, int>(hour, dr.GetInt32(1)));
                }
                dr.Close();
            }
            //SORT THE RESULTS BY HOUR ASCENDING
            sparseArray.Sort("Key");
            if (!includeZeros) return sparseArray;
            //MAKE A FULL ARRAY
            int sparseIndex = 0;
            SortableCollection<KeyValuePair<int, int>> fullArray = new SortableCollection<KeyValuePair<int, int>>();
            for (int hour = 0; hour < 24; hour++)
            {
                if ((sparseIndex < sparseArray.Count) && (sparseArray[sparseIndex].Key == hour))
                {
                    fullArray.Add(sparseArray[sparseIndex]);
                    sparseIndex++;
                }
                else
                {
                    fullArray.Add(new KeyValuePair<int, int>(hour, 0));
                }
            }
            return fullArray;
        }

        /// <summary>
        /// Gets a collection of Key Value pairs; each key representing a day 
        /// and each value representing the number of views during that day.
        /// </summary>
        /// <param name="includeZeros">If <b>true</b> days with zero views are also included</param>
        /// <returns>A collection of Key Value pairs; each key representing a day 
        /// and each value representing the number of views during that day.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SortableCollection<KeyValuePair<int, int>> GetViewsByDay(bool includeZeros)
        {
            //FIX THE HOURS FOR THE TIMEZONE OFFSET
            //THIS IS IMPERFECT BECAUSE WE CANNOT TAKE INTO ACCOUNT PARTIAL HOURS
            //SO CAST THE TIMEZONE OFFSET TO AN INTEGER
            int hourOffset = (int)Token.Instance.Store.TimeZoneOffset;
            //GET VIEWS BY HOUR
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT DATEPART(dd, DATEADD(hh, " + hourOffset + ", ActivityDate)) AS ViewHour, COUNT(1) as ViewCount");
            selectQuery.Append(" FROM ac_PageViews");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" GROUP BY DATEPART(dd, DATEADD(hh, " + hourOffset + ", ActivityDate))");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //EXECUTE THE COMMAND
            SortableCollection<KeyValuePair<int, int>> sparseArray = new SortableCollection<KeyValuePair<int, int>>();
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    sparseArray.Add(new KeyValuePair<int, int>(dr.GetInt32(0), dr.GetInt32(1)));
                }
                dr.Close();
            }
            //SORT THE RESULTS BY HOUR ASCENDING
            sparseArray.Sort("Key");
            if (!includeZeros) return sparseArray;
            //MAKE A FULL ARRAY
            int sparseIndex = 0;
            SortableCollection<KeyValuePair<int, int>> fullArray = new SortableCollection<KeyValuePair<int, int>>();
            for (int day = 1; day < 31; day++)
            {
                if ((sparseIndex < sparseArray.Count) && (sparseArray[sparseIndex].Key == day))
                {
                    fullArray.Add(sparseArray[sparseIndex]);
                    sparseIndex++;
                }
                else
                {
                    fullArray.Add(new KeyValuePair<int, int>(day, 0));
                }
            }
            return fullArray;
        }

        /// <summary>
        /// Gets a collection of Key Value pairs; each key representing a month 
        /// and each value representing the number of views during that month.
        /// </summary>
        /// <param name="includeZeros">If <b>true</b> months with zero views are also included</param>
        /// <returns>A collection of Key Value pairs; each key representing a month 
        /// and each value representing the number of views during that month.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SortableCollection<KeyValuePair<int, int>> GetViewsByMonth(bool includeZeros)
        {
            //FIX THE HOURS FOR THE TIMEZONE OFFSET
            //THIS IS IMPERFECT BECAUSE WE CANNOT TAKE INTO ACCOUNT PARTIAL HOURS
            //SO CAST THE TIMEZONE OFFSET TO AN INTEGER
            int hourOffset = (int)Token.Instance.Store.TimeZoneOffset;
            //GET VIEWS BY HOUR
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT DATEPART(mm, DATEADD(hh, " + hourOffset + ", ActivityDate)) AS ViewHour, COUNT(1) as ViewCount");
            selectQuery.Append(" FROM ac_PageViews");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" GROUP BY DATEPART(mm, DATEADD(hh, " + hourOffset + ", ActivityDate))");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //EXECUTE THE COMMAND
            SortableCollection<KeyValuePair<int, int>> sparseArray = new SortableCollection<KeyValuePair<int, int>>();
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    sparseArray.Add(new KeyValuePair<int, int>(dr.GetInt32(0), dr.GetInt32(1)));
                }
                dr.Close();
            }
            //SORT THE RESULTS BY HOUR ASCENDING
            sparseArray.Sort("Key");
            if (!includeZeros) return sparseArray;
            //MAKE A FULL ARRAY
            int sparseIndex = 0;
            SortableCollection<KeyValuePair<int, int>> fullArray = new SortableCollection<KeyValuePair<int, int>>();
            for (int month = 1; month < 13; month++)
            {
                if ((sparseIndex < sparseArray.Count) && (sparseArray[sparseIndex].Key == month))
                {
                    fullArray.Add(sparseArray[sparseIndex]);
                    sparseIndex++;
                }
                else
                {
                    fullArray.Add(new KeyValuePair<int, int>(month, 0));
                }
            }
            return fullArray;
        }

        /// <summary>
        /// Get the recently viewed products for the given user Id
        /// </summary>
        /// <param name="userId">The user to filter views by; pass 0 for all users</param>                
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> GetRecentlyViewedProducts(int userId)
        {
            return GetRecentlyViewedProducts(userId,0,0,String.Empty);
        }

        /// <summary>
        /// Get the recently viewed products for the given user Id
        /// </summary>
        /// <param name="userId">The user to filter views by; pass 0 for all users</param>                
        /// <param name="sortExpression">Sort expression to apply to the result set</param>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> GetRecentlyViewedProducts(int userId, string sortExpression)
        {
            return GetRecentlyViewedProducts(userId, 0, 0, sortExpression);
        }

        /// <summary>
        /// Get the recently viewed products for the given user Id
        /// </summary>
        /// <param name="userId">The user to filter views by; pass 0 for all users</param>        
        /// <param name="maximumRows">Maximum number of rows to return</param>
        /// <param name="startRowIndex">Starting index of first record in result set</param>        
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> GetRecentlyViewedProducts(int userId, int maximumRows, int startRowIndex)
        {
            return GetRecentlyViewedProducts(userId, maximumRows, startRowIndex, String.Empty);
        }

        /// <summary>
        /// Get the recently viewed products for the given user Id
        /// </summary>
        /// <param name="userId">The user to filter views by; pass 0 for all users</param>        
        /// <param name="maximumRows">Maximum number of rows to return</param>
        /// <param name="startRowIndex">Starting index of first record in result set</param>
        /// <param name="sortExpression">Sort expression to apply to the result set</param>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> GetRecentlyViewedProducts(int userId,int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append(" SELECT TOP " + ((startRowIndex + maximumRows) * 3).ToString()); /* */// TOP IS REQUIRED FOR ORDERBY CLAUSE
            selectQuery.Append(" " + PageView.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_PageViews");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" AND CatalogNodeTypeId = @catalogNodeTypeId");
            if (userId != 0) selectQuery.Append(" AND UserId = @userId");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@catalogNodeTypeId", DbType.Byte, CatalogNodeType.Product);
            if (userId != 0) database.AddInParameter(selectCommand, "@userId", DbType.Int32, userId); ;
            //EXECUTE THE COMMAND

            List<int> productIds = new List<int>();
            List<Product> results = new List<Product>();
            int thisIndex = 0;
            //int rowCount = 0;

            int nodeIdColumnIndex = -1;

            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    if (thisIndex >= startRowIndex)
                    {
                        if (nodeIdColumnIndex == -1)
                        {
                            nodeIdColumnIndex = GetCatalogNodeIdColumnIndex(dr);
                            if (nodeIdColumnIndex == -1) break;
                        }
                        int productId = dr.GetInt32(nodeIdColumnIndex);
                        if (!productIds.Contains(productId))
                        {
                            productIds.Add(productId);
                        }
                    }
                    thisIndex++;
                }
                dr.Close();
            }

            foreach (int productId in productIds)
            {
                Product product = ProductDataSource.Load(productId);
                if (product != null)
                {
                    results.Add(product);
                }
                if (results.Count >= maximumRows) break;
            }
            
            return results;
        }

        private static int GetCatalogNodeIdColumnIndex(IDataReader dr)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals("CatalogNodeId", StringComparison.InvariantCultureIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }

    }
}
