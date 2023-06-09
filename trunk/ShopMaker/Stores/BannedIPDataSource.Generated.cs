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

namespace MakerShop.Stores
{
    /// <summary>
    /// DataSource class for BannedIP objects
    /// </summary>
    public partial class BannedIPDataSource
    {
        /// <summary>
        /// Deletes a BannedIP object from the database
        /// </summary>
        /// <param name="bannedIP">The BannedIP object to delete</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static bool Delete(BannedIP bannedIP)
        {
            return bannedIP.Delete();
        }

        /// <summary>
        /// Deletes a BannedIP object with given id from the database
        /// </summary>
        /// <param name="bannedIPId">Value of BannedIPId of the object to delete.</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        public static bool Delete(Int32 bannedIPId)
        {
            BannedIP bannedIP = new BannedIP();
            if (bannedIP.Load(bannedIPId)) return bannedIP.Delete();
            return false;
        }

        /// <summary>
        /// Inserts/Saves a BannedIP object to the database.
        /// </summary>
        /// <param name="bannedIP">The BannedIP object to insert</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Insert(BannedIP bannedIP) { return bannedIP.Save(); }

        /// <summary>
        /// Loads a BannedIP object for given Id from the database.
        /// </summary>
        /// <param name="bannedIPId">Value of BannedIPId of the object to load.</param>
        /// <returns>If the load is successful the newly loaded BannedIP object is returned. If load fails null is returned.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static BannedIP Load(Int32 bannedIPId)
        {
            return BannedIPDataSource.Load(bannedIPId, true);
        }

        /// <summary>
        /// Loads a BannedIP object for given Id from the database.
        /// </summary>
        /// <param name="bannedIPId">Value of BannedIPId of the object to load.</param>
        /// <param name="useCache">If true tries to load object from cache first.</param>
        /// <returns>If the load is successful the newly loaded BannedIP object is returned. If load fails null is returned.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static BannedIP Load(Int32 bannedIPId, bool useCache)
        {
            if (bannedIPId == 0) return null;
            BannedIP bannedIP = null;
            string key = "BannedIP_" + bannedIPId.ToString();
            if (useCache)
            {
                bannedIP = ContextCache.GetObject(key) as BannedIP;
                if (bannedIP != null) return bannedIP;
            }
            bannedIP = new BannedIP();
            if (bannedIP.Load(bannedIPId))
            {
                if (useCache) ContextCache.SetObject(key, bannedIP);
                return bannedIP;
            }
            return null;
        }

        /// <summary>
        /// Counts the number of BannedIP objects in result if retrieved using the given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the BannedIP objects that should be loaded.</param>
        /// <returns>The number of BannedIP objects matching the criteria</returns>
        public static int CountForCriteria(string sqlCriteria)
        {
            Database database = Token.Instance.Database;
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM ac_BannedIPs" + whereClause);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of BannedIP objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <returns>A collection of BannedIP objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static BannedIPCollection LoadForCriteria(string sqlCriteria)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of BannedIP objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of BannedIP objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static BannedIPCollection LoadForCriteria(string sqlCriteria, string sortExpression)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of BannedIP objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of BannedIP objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static BannedIPCollection LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex)
        {
            return LoadForCriteria(sqlCriteria, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of BannedIP objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of BannedIP objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static BannedIPCollection  LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + BannedIP.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_BannedIPs");
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            selectQuery.Append(whereClause);
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            //EXECUTE THE COMMAND
            BannedIPCollection results = new BannedIPCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        BannedIP bannedIP = new BannedIP();
                        BannedIP.LoadDataReader(bannedIP, dr);
                        results.Add(bannedIP);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of BannedIP objects for the current store.
        /// </summary>
        /// <returns>The Number of BannedIP objects in the current store.</returns>
        public static int CountForStore()
        {
            int storeId = Token.Instance.StoreId;
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM ac_BannedIPs WHERE StoreId = @storeId");
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, storeId);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of BannedIP objects for the current store from the database
        /// </summary>
        /// <returns>A collection of BannedIP objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static BannedIPCollection LoadForStore()
        {
            return LoadForStore(0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of BannedIP objects for the current store from the database. Sorts using the given sort exrpression.
        /// </summary>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of BannedIP objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static BannedIPCollection LoadForStore(string sortExpression)
        {
            return LoadForStore(0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of BannedIP objects for the current store from the database.
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of BannedIP objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static BannedIPCollection LoadForStore(int maximumRows, int startRowIndex)
        {
            return LoadForStore(maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of BannedIP objects for the current store from the database. Sorts using the given sort exrpression.
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of BannedIP objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static BannedIPCollection LoadForStore(int maximumRows, int startRowIndex, string sortExpression)
        {
            int storeId = Token.Instance.StoreId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + BannedIP.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_BannedIPs");
            selectQuery.Append(" WHERE StoreId = @storeId");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, storeId);
            //EXECUTE THE COMMAND
            BannedIPCollection results = new BannedIPCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        BannedIP bannedIP = new BannedIP();
                        BannedIP.LoadDataReader(bannedIP, dr);
                        results.Add(bannedIP);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Updates/Saves the given BannedIP object to the database.
        /// </summary>
        /// <param name="bannedIP">The BannedIP object to update/save to database.</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Update)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Update(BannedIP bannedIP) { return bannedIP.Save(); }

    }
}
