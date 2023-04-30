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

namespace MakerShop.Marketing
{
    /// <summary>
    /// DataSource class for VolumeDiscount objects
    /// </summary>
    public partial class VolumeDiscountDataSource
    {
        /// <summary>
        /// Deletes a VolumeDiscount object from the database
        /// </summary>
        /// <param name="volumeDiscount">The VolumeDiscount object to delete</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static bool Delete(VolumeDiscount volumeDiscount)
        {
            return volumeDiscount.Delete();
        }

        /// <summary>
        /// Deletes a VolumeDiscount object with given id from the database
        /// </summary>
        /// <param name="volumeDiscountId">Value of VolumeDiscountId of the object to delete.</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        public static bool Delete(Int32 volumeDiscountId)
        {
            VolumeDiscount volumeDiscount = new VolumeDiscount();
            if (volumeDiscount.Load(volumeDiscountId)) return volumeDiscount.Delete();
            return false;
        }

        /// <summary>
        /// Inserts/Saves a VolumeDiscount object to the database.
        /// </summary>
        /// <param name="volumeDiscount">The VolumeDiscount object to insert</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Insert(VolumeDiscount volumeDiscount) { return volumeDiscount.Save(); }

        /// <summary>
        /// Loads a VolumeDiscount object for given Id from the database.
        /// </summary>
        /// <param name="volumeDiscountId">Value of VolumeDiscountId of the object to load.</param>
        /// <returns>If the load is successful the newly loaded VolumeDiscount object is returned. If load fails null is returned.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VolumeDiscount Load(Int32 volumeDiscountId)
        {
            return VolumeDiscountDataSource.Load(volumeDiscountId, true);
        }

        /// <summary>
        /// Loads a VolumeDiscount object for given Id from the database.
        /// </summary>
        /// <param name="volumeDiscountId">Value of VolumeDiscountId of the object to load.</param>
        /// <param name="useCache">If true tries to load object from cache first.</param>
        /// <returns>If the load is successful the newly loaded VolumeDiscount object is returned. If load fails null is returned.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VolumeDiscount Load(Int32 volumeDiscountId, bool useCache)
        {
            if (volumeDiscountId == 0) return null;
            VolumeDiscount volumeDiscount = null;
            string key = "VolumeDiscount_" + volumeDiscountId.ToString();
            if (useCache)
            {
                volumeDiscount = ContextCache.GetObject(key) as VolumeDiscount;
                if (volumeDiscount != null) return volumeDiscount;
            }
            volumeDiscount = new VolumeDiscount();
            if (volumeDiscount.Load(volumeDiscountId))
            {
                if (useCache) ContextCache.SetObject(key, volumeDiscount);
                return volumeDiscount;
            }
            return null;
        }

        /// <summary>
        /// Counts the number of VolumeDiscount objects in result if retrieved using the given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the VolumeDiscount objects that should be loaded.</param>
        /// <returns>The number of VolumeDiscount objects matching the criteria</returns>
        public static int CountForCriteria(string sqlCriteria)
        {
            Database database = Token.Instance.Database;
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM ac_VolumeDiscounts" + whereClause);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of VolumeDiscount objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <returns>A collection of VolumeDiscount objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VolumeDiscountCollection LoadForCriteria(string sqlCriteria)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of VolumeDiscount objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of VolumeDiscount objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VolumeDiscountCollection LoadForCriteria(string sqlCriteria, string sortExpression)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of VolumeDiscount objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of VolumeDiscount objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VolumeDiscountCollection LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex)
        {
            return LoadForCriteria(sqlCriteria, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of VolumeDiscount objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of VolumeDiscount objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VolumeDiscountCollection  LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + VolumeDiscount.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_VolumeDiscounts");
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            selectQuery.Append(whereClause);
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            //EXECUTE THE COMMAND
            VolumeDiscountCollection results = new VolumeDiscountCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        VolumeDiscount volumeDiscount = new VolumeDiscount();
                        VolumeDiscount.LoadDataReader(volumeDiscount, dr);
                        results.Add(volumeDiscount);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of VolumeDiscount objects associated with the given CategoryId
        /// </summary>
        /// <param name="categoryId">The given CategoryId</param>
        /// <returns>The number of VolumeDiscount objects associated with with the given CategoryId</returns>
        public static int CountForCategory(Int32 categoryId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM ac_CategoryVolumeDiscounts WHERE CategoryId = @categoryId");
            database.AddInParameter(selectCommand, "@categoryId", System.Data.DbType.Int32, categoryId);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }
        /// <summary>
        /// Loads the VolumeDiscount objects associated with the given CategoryId
        /// </summary>
        /// <param name="categoryId">The given CategoryId</param>
        /// <returns>A collection of VolumeDiscount objects associated with with the given CategoryId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VolumeDiscountCollection LoadForCategory(Int32 categoryId)
        {
            return VolumeDiscountDataSource.LoadForCategory(categoryId, 0, 0, string.Empty);
        }
        /// <summary>
        /// Loads the VolumeDiscount objects associated with the given CategoryId
        /// </summary>
        /// <param name="categoryId">The given CategoryId</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of VolumeDiscount objects associated with with the given CategoryId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VolumeDiscountCollection LoadForCategory(Int32 categoryId, string sortExpression)
        {
            return VolumeDiscountDataSource.LoadForCategory(categoryId, 0, 0, sortExpression);
        }
        /// <summary>
        /// Loads the VolumeDiscount objects associated with the given CategoryId
        /// </summary>
        /// <param name="categoryId">The given CategoryId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of VolumeDiscount objects associated with with the given CategoryId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VolumeDiscountCollection LoadForCategory(Int32 categoryId, int maximumRows, int startRowIndex)
        {
            return VolumeDiscountDataSource.LoadForCategory(categoryId, maximumRows, startRowIndex, string.Empty);
        }
        /// <summary>
        /// Loads the VolumeDiscount objects associated with the given CategoryId
        /// </summary>
        /// <param name="categoryId">The given CategoryId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of VolumeDiscount objects associated with with the given CategoryId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VolumeDiscountCollection LoadForCategory(Int32 categoryId, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + VolumeDiscount.GetColumnNames("ac_VolumeDiscounts"));
            selectQuery.Append(" FROM ac_VolumeDiscounts, ac_CategoryVolumeDiscounts");
            selectQuery.Append(" WHERE ac_VolumeDiscounts.VolumeDiscountId = ac_CategoryVolumeDiscounts.VolumeDiscountId");
            selectQuery.Append(" AND ac_CategoryVolumeDiscounts.CategoryId = @categoryId");
            selectQuery.Append(" AND StoreId = @storeId");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@categoryId", System.Data.DbType.Int32, categoryId);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //EXECUTE THE COMMAND
            VolumeDiscountCollection results = new VolumeDiscountCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        VolumeDiscount volumeDiscount = new VolumeDiscount();
                        VolumeDiscount.LoadDataReader(volumeDiscount, dr);
                        results.Add(volumeDiscount);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of VolumeDiscount objects associated with the given ProductId
        /// </summary>
        /// <param name="productId">The given ProductId</param>
        /// <returns>The number of VolumeDiscount objects associated with with the given ProductId</returns>
        public static int CountForProduct(Int32 productId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM ac_ProductVolumeDiscounts WHERE ProductId = @productId");
            database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }
        /// <summary>
        /// Loads the VolumeDiscount objects associated with the given ProductId
        /// </summary>
        /// <param name="productId">The given ProductId</param>
        /// <returns>A collection of VolumeDiscount objects associated with with the given ProductId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VolumeDiscountCollection LoadForProduct(Int32 productId)
        {
            return VolumeDiscountDataSource.LoadForProduct(productId, 0, 0, string.Empty);
        }
        /// <summary>
        /// Loads the VolumeDiscount objects associated with the given ProductId
        /// </summary>
        /// <param name="productId">The given ProductId</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of VolumeDiscount objects associated with with the given ProductId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VolumeDiscountCollection LoadForProduct(Int32 productId, string sortExpression)
        {
            return VolumeDiscountDataSource.LoadForProduct(productId, 0, 0, sortExpression);
        }
        /// <summary>
        /// Loads the VolumeDiscount objects associated with the given ProductId
        /// </summary>
        /// <param name="productId">The given ProductId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of VolumeDiscount objects associated with with the given ProductId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VolumeDiscountCollection LoadForProduct(Int32 productId, int maximumRows, int startRowIndex)
        {
            return VolumeDiscountDataSource.LoadForProduct(productId, maximumRows, startRowIndex, string.Empty);
        }
        /// <summary>
        /// Loads the VolumeDiscount objects associated with the given ProductId
        /// </summary>
        /// <param name="productId">The given ProductId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of VolumeDiscount objects associated with with the given ProductId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VolumeDiscountCollection LoadForProduct(Int32 productId, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + VolumeDiscount.GetColumnNames("ac_VolumeDiscounts"));
            selectQuery.Append(" FROM ac_VolumeDiscounts, ac_ProductVolumeDiscounts");
            selectQuery.Append(" WHERE ac_VolumeDiscounts.VolumeDiscountId = ac_ProductVolumeDiscounts.VolumeDiscountId");
            selectQuery.Append(" AND ac_ProductVolumeDiscounts.ProductId = @productId");
            selectQuery.Append(" AND StoreId = @storeId");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //EXECUTE THE COMMAND
            VolumeDiscountCollection results = new VolumeDiscountCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        VolumeDiscount volumeDiscount = new VolumeDiscount();
                        VolumeDiscount.LoadDataReader(volumeDiscount, dr);
                        results.Add(volumeDiscount);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of VolumeDiscount objects associated with the given GroupId
        /// </summary>
        /// <param name="groupId">The given GroupId</param>
        /// <returns>The number of VolumeDiscount objects associated with with the given GroupId</returns>
        public static int CountForGroup(Int32 groupId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM ac_VolumeDiscountGroups WHERE GroupId = @groupId");
            database.AddInParameter(selectCommand, "@groupId", System.Data.DbType.Int32, groupId);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }
        /// <summary>
        /// Loads the VolumeDiscount objects associated with the given GroupId
        /// </summary>
        /// <param name="groupId">The given GroupId</param>
        /// <returns>A collection of VolumeDiscount objects associated with with the given GroupId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VolumeDiscountCollection LoadForGroup(Int32 groupId)
        {
            return VolumeDiscountDataSource.LoadForGroup(groupId, 0, 0, string.Empty);
        }
        /// <summary>
        /// Loads the VolumeDiscount objects associated with the given GroupId
        /// </summary>
        /// <param name="groupId">The given GroupId</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of VolumeDiscount objects associated with with the given GroupId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VolumeDiscountCollection LoadForGroup(Int32 groupId, string sortExpression)
        {
            return VolumeDiscountDataSource.LoadForGroup(groupId, 0, 0, sortExpression);
        }
        /// <summary>
        /// Loads the VolumeDiscount objects associated with the given GroupId
        /// </summary>
        /// <param name="groupId">The given GroupId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of VolumeDiscount objects associated with with the given GroupId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VolumeDiscountCollection LoadForGroup(Int32 groupId, int maximumRows, int startRowIndex)
        {
            return VolumeDiscountDataSource.LoadForGroup(groupId, maximumRows, startRowIndex, string.Empty);
        }
        /// <summary>
        /// Loads the VolumeDiscount objects associated with the given GroupId
        /// </summary>
        /// <param name="groupId">The given GroupId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of VolumeDiscount objects associated with with the given GroupId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VolumeDiscountCollection LoadForGroup(Int32 groupId, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + VolumeDiscount.GetColumnNames("ac_VolumeDiscounts"));
            selectQuery.Append(" FROM ac_VolumeDiscounts, ac_VolumeDiscountGroups");
            selectQuery.Append(" WHERE ac_VolumeDiscounts.VolumeDiscountId = ac_VolumeDiscountGroups.VolumeDiscountId");
            selectQuery.Append(" AND ac_VolumeDiscountGroups.GroupId = @groupId");
            selectQuery.Append(" AND StoreId = @storeId");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@groupId", System.Data.DbType.Int32, groupId);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //EXECUTE THE COMMAND
            VolumeDiscountCollection results = new VolumeDiscountCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        VolumeDiscount volumeDiscount = new VolumeDiscount();
                        VolumeDiscount.LoadDataReader(volumeDiscount, dr);
                        results.Add(volumeDiscount);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of VolumeDiscount objects for the current store.
        /// </summary>
        /// <returns>The Number of VolumeDiscount objects in the current store.</returns>
        public static int CountForStore()
        {
            int storeId = Token.Instance.StoreId;
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM ac_VolumeDiscounts WHERE StoreId = @storeId");
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, storeId);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of VolumeDiscount objects for the current store from the database
        /// </summary>
        /// <returns>A collection of VolumeDiscount objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VolumeDiscountCollection LoadForStore()
        {
            return LoadForStore(0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of VolumeDiscount objects for the current store from the database. Sorts using the given sort exrpression.
        /// </summary>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of VolumeDiscount objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VolumeDiscountCollection LoadForStore(string sortExpression)
        {
            return LoadForStore(0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of VolumeDiscount objects for the current store from the database.
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of VolumeDiscount objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VolumeDiscountCollection LoadForStore(int maximumRows, int startRowIndex)
        {
            return LoadForStore(maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of VolumeDiscount objects for the current store from the database. Sorts using the given sort exrpression.
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of VolumeDiscount objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VolumeDiscountCollection LoadForStore(int maximumRows, int startRowIndex, string sortExpression)
        {
            int storeId = Token.Instance.StoreId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + VolumeDiscount.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_VolumeDiscounts");
            selectQuery.Append(" WHERE StoreId = @storeId");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, storeId);
            //EXECUTE THE COMMAND
            VolumeDiscountCollection results = new VolumeDiscountCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        VolumeDiscount volumeDiscount = new VolumeDiscount();
                        VolumeDiscount.LoadDataReader(volumeDiscount, dr);
                        results.Add(volumeDiscount);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Updates/Saves the given VolumeDiscount object to the database.
        /// </summary>
        /// <param name="volumeDiscount">The VolumeDiscount object to update/save to database.</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Update)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Update(VolumeDiscount volumeDiscount) { return volumeDiscount.Save(); }

    }
}