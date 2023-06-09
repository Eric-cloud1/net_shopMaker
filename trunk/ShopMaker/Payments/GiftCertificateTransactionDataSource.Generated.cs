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

namespace MakerShop.Payments
{
    /// <summary>
    /// DataSource class for GiftCertificateTransaction objects
    /// </summary>
    public partial class GiftCertificateTransactionDataSource
    {
        /// <summary>
        /// Deletes a GiftCertificateTransaction object from the database
        /// </summary>
        /// <param name="giftCertificateTransaction">The GiftCertificateTransaction object to delete</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static bool Delete(GiftCertificateTransaction giftCertificateTransaction)
        {
            return giftCertificateTransaction.Delete();
        }

        /// <summary>
        /// Deletes a GiftCertificateTransaction object with given id from the database
        /// </summary>
        /// <param name="giftCertificateTransactionId">Value of GiftCertificateTransactionId of the object to delete.</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        public static bool Delete(Int32 giftCertificateTransactionId)
        {
            GiftCertificateTransaction giftCertificateTransaction = new GiftCertificateTransaction();
            if (giftCertificateTransaction.Load(giftCertificateTransactionId)) return giftCertificateTransaction.Delete();
            return false;
        }

        /// <summary>
        /// Inserts/Saves a GiftCertificateTransaction object to the database.
        /// </summary>
        /// <param name="giftCertificateTransaction">The GiftCertificateTransaction object to insert</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Insert(GiftCertificateTransaction giftCertificateTransaction) { return giftCertificateTransaction.Save(); }

        /// <summary>
        /// Loads a GiftCertificateTransaction object for given Id from the database.
        /// </summary>
        /// <param name="giftCertificateTransactionId">Value of GiftCertificateTransactionId of the object to load.</param>
        /// <returns>If the load is successful the newly loaded GiftCertificateTransaction object is returned. If load fails null is returned.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static GiftCertificateTransaction Load(Int32 giftCertificateTransactionId)
        {
            return GiftCertificateTransactionDataSource.Load(giftCertificateTransactionId, true);
        }

        /// <summary>
        /// Loads a GiftCertificateTransaction object for given Id from the database.
        /// </summary>
        /// <param name="giftCertificateTransactionId">Value of GiftCertificateTransactionId of the object to load.</param>
        /// <param name="useCache">If true tries to load object from cache first.</param>
        /// <returns>If the load is successful the newly loaded GiftCertificateTransaction object is returned. If load fails null is returned.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static GiftCertificateTransaction Load(Int32 giftCertificateTransactionId, bool useCache)
        {
            if (giftCertificateTransactionId == 0) return null;
            GiftCertificateTransaction giftCertificateTransaction = null;
            string key = "GiftCertificateTransaction_" + giftCertificateTransactionId.ToString();
            if (useCache)
            {
                giftCertificateTransaction = ContextCache.GetObject(key) as GiftCertificateTransaction;
                if (giftCertificateTransaction != null) return giftCertificateTransaction;
            }
            giftCertificateTransaction = new GiftCertificateTransaction();
            if (giftCertificateTransaction.Load(giftCertificateTransactionId))
            {
                if (useCache) ContextCache.SetObject(key, giftCertificateTransaction);
                return giftCertificateTransaction;
            }
            return null;
        }

        /// <summary>
        /// Counts the number of GiftCertificateTransaction objects in result if retrieved using the given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the GiftCertificateTransaction objects that should be loaded.</param>
        /// <returns>The number of GiftCertificateTransaction objects matching the criteria</returns>
        public static int CountForCriteria(string sqlCriteria)
        {
            Database database = Token.Instance.Database;
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM ac_GiftCertificateTransactions" + whereClause);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of GiftCertificateTransaction objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <returns>A collection of GiftCertificateTransaction objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static GiftCertificateTransactionCollection LoadForCriteria(string sqlCriteria)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of GiftCertificateTransaction objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of GiftCertificateTransaction objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static GiftCertificateTransactionCollection LoadForCriteria(string sqlCriteria, string sortExpression)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of GiftCertificateTransaction objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of GiftCertificateTransaction objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static GiftCertificateTransactionCollection LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex)
        {
            return LoadForCriteria(sqlCriteria, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of GiftCertificateTransaction objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of GiftCertificateTransaction objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static GiftCertificateTransactionCollection  LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + GiftCertificateTransaction.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_GiftCertificateTransactions");
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            selectQuery.Append(whereClause);
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            //EXECUTE THE COMMAND
            GiftCertificateTransactionCollection results = new GiftCertificateTransactionCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        GiftCertificateTransaction giftCertificateTransaction = new GiftCertificateTransaction();
                        GiftCertificateTransaction.LoadDataReader(giftCertificateTransaction, dr);
                        results.Add(giftCertificateTransaction);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of GiftCertificateTransaction objects for the given GiftCertificateId in the database.
        /// <param name="giftCertificateId">The given GiftCertificateId</param>
        /// </summary>
        /// <returns>The Number of GiftCertificateTransaction objects for the given GiftCertificateId in the database.</returns>
        public static int CountForGiftCertificate(Int32 giftCertificateId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM ac_GiftCertificateTransactions WHERE GiftCertificateId = @giftCertificateId");
            database.AddInParameter(selectCommand, "@giftCertificateId", System.Data.DbType.Int32, giftCertificateId);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of GiftCertificateTransaction objects for the given GiftCertificateId from the database
        /// </summary>
        /// <param name="giftCertificateId">The given GiftCertificateId</param>
        /// <returns>A collection of GiftCertificateTransaction objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static GiftCertificateTransactionCollection LoadForGiftCertificate(Int32 giftCertificateId)
        {
            return LoadForGiftCertificate(giftCertificateId, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of GiftCertificateTransaction objects for the given GiftCertificateId from the database
        /// </summary>
        /// <param name="giftCertificateId">The given GiftCertificateId</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of GiftCertificateTransaction objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static GiftCertificateTransactionCollection LoadForGiftCertificate(Int32 giftCertificateId, string sortExpression)
        {
            return LoadForGiftCertificate(giftCertificateId, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of GiftCertificateTransaction objects for the given GiftCertificateId from the database
        /// </summary>
        /// <param name="giftCertificateId">The given GiftCertificateId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of GiftCertificateTransaction objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static GiftCertificateTransactionCollection LoadForGiftCertificate(Int32 giftCertificateId, int maximumRows, int startRowIndex)
        {
            return LoadForGiftCertificate(giftCertificateId, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of GiftCertificateTransaction objects for the given GiftCertificateId from the database
        /// </summary>
        /// <param name="giftCertificateId">The given GiftCertificateId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of GiftCertificateTransaction objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static GiftCertificateTransactionCollection LoadForGiftCertificate(Int32 giftCertificateId, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + GiftCertificateTransaction.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_GiftCertificateTransactions");
            selectQuery.Append(" WHERE GiftCertificateId = @giftCertificateId");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@giftCertificateId", System.Data.DbType.Int32, giftCertificateId);
            //EXECUTE THE COMMAND
            GiftCertificateTransactionCollection results = new GiftCertificateTransactionCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        GiftCertificateTransaction giftCertificateTransaction = new GiftCertificateTransaction();
                        GiftCertificateTransaction.LoadDataReader(giftCertificateTransaction, dr);
                        results.Add(giftCertificateTransaction);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of GiftCertificateTransaction objects for the given OrderId in the database.
        /// <param name="orderId">The given OrderId</param>
        /// </summary>
        /// <returns>The Number of GiftCertificateTransaction objects for the given OrderId in the database.</returns>
        public static int CountForOrder(Int32 orderId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM ac_GiftCertificateTransactions WHERE OrderId = @orderId");
            database.AddInParameter(selectCommand, "@orderId", System.Data.DbType.Int32, NullableData.DbNullify(orderId));
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of GiftCertificateTransaction objects for the given OrderId from the database
        /// </summary>
        /// <param name="orderId">The given OrderId</param>
        /// <returns>A collection of GiftCertificateTransaction objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static GiftCertificateTransactionCollection LoadForOrder(Int32 orderId)
        {
            return LoadForOrder(orderId, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of GiftCertificateTransaction objects for the given OrderId from the database
        /// </summary>
        /// <param name="orderId">The given OrderId</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of GiftCertificateTransaction objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static GiftCertificateTransactionCollection LoadForOrder(Int32 orderId, string sortExpression)
        {
            return LoadForOrder(orderId, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of GiftCertificateTransaction objects for the given OrderId from the database
        /// </summary>
        /// <param name="orderId">The given OrderId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of GiftCertificateTransaction objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static GiftCertificateTransactionCollection LoadForOrder(Int32 orderId, int maximumRows, int startRowIndex)
        {
            return LoadForOrder(orderId, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of GiftCertificateTransaction objects for the given OrderId from the database
        /// </summary>
        /// <param name="orderId">The given OrderId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of GiftCertificateTransaction objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static GiftCertificateTransactionCollection LoadForOrder(Int32 orderId, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + GiftCertificateTransaction.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_GiftCertificateTransactions");
            selectQuery.Append(" WHERE OrderId = @orderId");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@orderId", System.Data.DbType.Int32, NullableData.DbNullify(orderId));
            //EXECUTE THE COMMAND
            GiftCertificateTransactionCollection results = new GiftCertificateTransactionCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        GiftCertificateTransaction giftCertificateTransaction = new GiftCertificateTransaction();
                        GiftCertificateTransaction.LoadDataReader(giftCertificateTransaction, dr);
                        results.Add(giftCertificateTransaction);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Updates/Saves the given GiftCertificateTransaction object to the database.
        /// </summary>
        /// <param name="giftCertificateTransaction">The GiftCertificateTransaction object to update/save to database.</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Update)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Update(GiftCertificateTransaction giftCertificateTransaction) { return giftCertificateTransaction.Save(); }

    }
}
