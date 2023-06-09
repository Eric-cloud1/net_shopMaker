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
    /// DataSource class for Affiliate objects
    /// </summary>
    public partial class AffiliateDataSource
    {
        /// <summary>
        /// Deletes a Affiliate object from the database
        /// </summary>
        /// <param name="affiliate">The Affiliate object to delete</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static bool Delete(Affiliate affiliate)
        {
            return affiliate.Delete();
        }

        /// <summary>
        /// Deletes a Affiliate object with given id from the database
        /// </summary>
        /// <param name="affiliateId">Value of AffiliateId of the object to delete.</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        public static bool Delete(Int32 affiliateId)
        {
            Affiliate affiliate = new Affiliate();
            if (affiliate.Load(affiliateId)) return affiliate.Delete();
            return false;
        }

        /// <summary>
        /// Inserts/Saves a Affiliate object to the database.
        /// </summary>
        /// <param name="affiliate">The Affiliate object to insert</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Insert(Affiliate affiliate) { return affiliate.Save(); }

        /// <summary>
        /// Loads a Affiliate object for given Id from the database.
        /// </summary>
        /// <param name="affiliateId">Value of AffiliateId of the object to load.</param>
        /// <returns>If the load is successful the newly loaded Affiliate object is returned. If load fails null is returned.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static Affiliate Load(Int32 affiliateId)
        {
            return AffiliateDataSource.Load(affiliateId, true);
        }

        /// <summary>
        /// Loads a Affiliate object for given Id from the database.
        /// </summary>
        /// <param name="affiliateId">Value of AffiliateId of the object to load.</param>
        /// <param name="useCache">If true tries to load object from cache first.</param>
        /// <returns>If the load is successful the newly loaded Affiliate object is returned. If load fails null is returned.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static Affiliate Load(Int32 affiliateId, bool useCache)
        {
            if (affiliateId == 0) return null;
            Affiliate affiliate = null;
            string key = "Affiliate_" + affiliateId.ToString();
            if (useCache)
            {
                affiliate = ContextCache.GetObject(key) as Affiliate;
                if (affiliate != null) return affiliate;
            }
            affiliate = new Affiliate();
            if (affiliate.Load(affiliateId))
            {
                if (useCache) ContextCache.SetObject(key, affiliate);
                return affiliate;
            }
            return null;
        }

        /// <summary>
        /// Counts the number of Affiliate objects in result if retrieved using the given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the Affiliate objects that should be loaded.</param>
        /// <returns>The number of Affiliate objects matching the criteria</returns>
        public static int CountForCriteria(string sqlCriteria)
        {
            Database database = Token.Instance.Database;
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM ac_Affiliates" + whereClause);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of Affiliate objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <returns>A collection of Affiliate objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static AffiliateCollection LoadForCriteria(string sqlCriteria)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of Affiliate objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Affiliate objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static AffiliateCollection LoadForCriteria(string sqlCriteria, string sortExpression)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of Affiliate objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of Affiliate objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static AffiliateCollection LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex)
        {
            return LoadForCriteria(sqlCriteria, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of Affiliate objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Affiliate objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static AffiliateCollection  LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Affiliate.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Affiliates");
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            selectQuery.Append(whereClause);
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            //EXECUTE THE COMMAND
            AffiliateCollection results = new AffiliateCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Affiliate affiliate = new Affiliate();
                        Affiliate.LoadDataReader(affiliate, dr);
                        results.Add(affiliate);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of Affiliate objects for the current store.
        /// </summary>
        /// <returns>The Number of Affiliate objects in the current store.</returns>
        public static int CountForStore()
        {
            int storeId = Token.Instance.StoreId;
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM ac_Affiliates WHERE StoreId = @storeId");
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, storeId);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of Affiliate objects for the current store from the database
        /// </summary>
        /// <returns>A collection of Affiliate objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static AffiliateCollection LoadForStore()
        {
            return LoadForStore(0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of Affiliate objects for the current store from the database. Sorts using the given sort exrpression.
        /// </summary>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Affiliate objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static AffiliateCollection LoadForStore(string sortExpression)
        {
            return LoadForStore(0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of Affiliate objects for the current store from the database.
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of Affiliate objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static AffiliateCollection LoadForStore(int maximumRows, int startRowIndex)
        {
            return LoadForStore(maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of Affiliate objects for the current store from the database. Sorts using the given sort exrpression.
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Affiliate objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static AffiliateCollection LoadForStore(int maximumRows, int startRowIndex, string sortExpression)
        {
            int storeId = Token.Instance.StoreId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Affiliate.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Affiliates");
            selectQuery.Append(" WHERE StoreId = @storeId");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, storeId);
            //EXECUTE THE COMMAND
            AffiliateCollection results = new AffiliateCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Affiliate affiliate = new Affiliate();
                        Affiliate.LoadDataReader(affiliate, dr);
                        results.Add(affiliate);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Updates/Saves the given Affiliate object to the database.
        /// </summary>
        /// <param name="affiliate">The Affiliate object to update/save to database.</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Update)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Update(Affiliate affiliate) { return affiliate.Save(); }

    }
}
