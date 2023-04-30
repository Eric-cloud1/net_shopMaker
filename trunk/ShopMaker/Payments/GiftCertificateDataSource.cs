using System.ComponentModel;
using System.Text;
using System.Data;
using System.Data.Common;
using MakerShop.Common;
using MakerShop.Data;
using System;

namespace MakerShop.Payments
{
    /// <summary>
    /// DataSource class for GiftCertificate objects
    /// </summary>
    [DataObject(true)]
    public partial class GiftCertificateDataSource
    {
        /// <summary>
        /// Loads a GiftCertificate object for given serial number
        /// </summary>
        /// <param name="serialNumber">Serial Number to load the GiftCertificate object for</param>
        /// <returns>GiftCertificate object for given serial number</returns>
        public static GiftCertificate LoadForSerialNumber(string serialNumber)
        {
            GiftCertificate giftCert = null;
            serialNumber = serialNumber.ToUpperInvariant();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + GiftCertificate.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_GiftCertificates WHERE SerialNumber = @serialNumber");
            selectQuery.Append(" AND StoreId = @storeId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@serialNumber", System.Data.DbType.String, serialNumber);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);

            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                if (dr.Read())
                {
                    giftCert = new GiftCertificate();
                    GiftCertificate.LoadDataReader(giftCert, dr);
                }
                dr.Close();
            }
            return giftCert;
        }

        /// <summary>
        /// Counts the gift certificates according to status value.
        /// </summary>
        /// <param name="status">The status filter to apply to the gift certificate count - active, inactive, or all.</param>
        /// <returns>The number of gift certificates matching the status filter.</returns>
        public static int CountForStatus(GiftCertificateStatus status)
        {
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM ac_GiftCertificates WHERE StoreId = @storeId");
            if (status == GiftCertificateStatus.Active)
            {
                selectQuery.Append(" AND (ExpirationDate IS NULL OR ExpirationDate > @expirationDate) AND Balance > 0 AND (SerialNumber IS NOT NULL AND SerialNumber <> @serialNumber)");
            }
            else if (status == GiftCertificateStatus.Inactive)
            {
                selectQuery.Append(" AND (ExpirationDate < @expirationDate OR Balance = 0 OR (SerialNumber IS NULL OR SerialNumber = @serialNumber))");
            }
            int storeId = Token.Instance.StoreId;
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            if (status != GiftCertificateStatus.All) database.AddInParameter(selectCommand, "@expirationDate", System.Data.DbType.DateTime, DateTime.UtcNow);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, storeId);
            database.AddInParameter(selectCommand, "@serialNumber", System.Data.DbType.String, String.Empty);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }
        
        /// <summary>
        /// Loads the gift certificates according to status value active, inactive or all.
        /// </summary>
        /// <param name="status">The status filter to apply to the gift certificate results - active, inactive, or all</param>        
        /// <returns>A collection of gift certificates matching the status filter.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static GiftCertificateCollection LoadForStatus(GiftCertificateStatus status)
        {
            return LoadForStatus(status, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads the gift certificates according to status value active, inactive or all.
        /// </summary>
        /// <param name="status">The status filter to apply to the gift certificate results - active, inactive, or all</param>        
        /// <param name="sortExpression"></param>
        /// <returns>A collection of gift certificates matching the status filter.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static GiftCertificateCollection LoadForStatus(GiftCertificateStatus status, string sortExpression)
        {
            return LoadForStatus(status, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads the gift certificates according to status value active, inactive or all.
        /// </summary>
        /// <param name="status">The status filter to apply to the gift certificate results - active, inactive, or all</param>
        /// <param name="maximumRows"></param>
        /// <param name="startRowIndex"></param>        
        /// <returns>A collection of gift certificates matching the status filter.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static GiftCertificateCollection LoadForStatus(GiftCertificateStatus status, int maximumRows, int startRowIndex)
        {
            return LoadForStatus(status, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads the gift certificates according to status value active, inactive or all.
        /// </summary>
        /// <param name="status">The status filter to apply to the gift certificate results - active, inactive, or all</param>
        /// <param name="maximumRows"></param>
        /// <param name="startRowIndex"></param>
        /// <param name="sortExpression"></param>
        /// <returns>A collection of gift certificates matching the status filter.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static GiftCertificateCollection LoadForStatus(GiftCertificateStatus status, int maximumRows, int startRowIndex, string sortExpression)
        {
            int storeId = Token.Instance.StoreId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + GiftCertificate.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_GiftCertificates");
            selectQuery.Append(" WHERE StoreId = @storeId");
            if (status == GiftCertificateStatus.Active)
            {
                selectQuery.Append(" AND (ExpirationDate IS NULL OR ExpirationDate > @expirationDate) AND Balance > 0 AND (SerialNumber IS NOT NULL AND SerialNumber <> @serialNumber)");
            }
            else if (status == GiftCertificateStatus.Inactive)
            {
                selectQuery.Append(" AND (ExpirationDate < @expirationDate OR Balance = 0 OR (SerialNumber IS NULL OR SerialNumber = @serialNumber))");
            }
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, storeId);
            if (status != GiftCertificateStatus.All) database.AddInParameter(selectCommand, "@expirationDate", System.Data.DbType.DateTime, DateTime.UtcNow);
            database.AddInParameter(selectCommand, "@serialNumber", System.Data.DbType.String, String.Empty);

            //EXECUTE THE COMMAND
            GiftCertificateCollection results = new GiftCertificateCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        GiftCertificate giftCertificate = new GiftCertificate();
                        GiftCertificate.LoadDataReader(giftCertificate, dr);
                        results.Add(giftCertificate);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }
    }
}