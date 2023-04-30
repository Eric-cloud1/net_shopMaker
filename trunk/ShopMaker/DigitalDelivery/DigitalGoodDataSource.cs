using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;

namespace MakerShop.DigitalDelivery
{
    /// <summary>
    /// DataSource class for DigitalGood objects
    /// </summary>
    [DataObject(true)]
    public partial class DigitalGoodDataSource
    {
        /// <summary>
        /// Gets the number of DigitalGood objects matching the given name
        /// </summary>
        /// <param name="nameToMatch">The name to match</param>
        /// <returns>Number of DigitalGood objects matching the given name</returns>
        public static int FindByNameCount(string nameToMatch)
        {
            nameToMatch = StringHelper.FixSearchPattern(nameToMatch);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM ac_DigitalGoods WHERE StoreId = @storeId AND (Name LIKE @nameToMatch OR FileName LIKE @nameToMatch)");
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@nameToMatch", System.Data.DbType.String, nameToMatch);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Gets a collection of DigitalGood objects matching the given name
        /// </summary>
        /// <param name="nameToMatch">The name to match</param>
        /// <returns>A collection of DigitalGood objects matching the given name</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DigitalGoodCollection FindByName(string nameToMatch)
        {
            return DigitalGoodDataSource.FindByName(nameToMatch, 0, 0, string.Empty);
        }

        /// <summary>
        /// Gets a collection of DigitalGood objects matching the given name
        /// </summary>
        /// <param name="nameToMatch">The name to match</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of DigitalGood objects matching the given name</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DigitalGoodCollection FindByName(string nameToMatch, string sortExpression)
        {
            return DigitalGoodDataSource.FindByName(nameToMatch, 0, 0, sortExpression);
        }

        /// <summary>
        /// Gets a collection of DigitalGood objects matching the given name
        /// </summary>
        /// <param name="nameToMatch"></param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of DigitalGood objects matching the given name</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DigitalGoodCollection FindByName(string nameToMatch, int maximumRows, int startRowIndex)
        {
            return DigitalGoodDataSource.FindByName(nameToMatch, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Gets a collection of DigitalGood objects matching the given name
        /// </summary>
        /// <param name="nameToMatch">The name to match</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of DigitalGood objects matching the given name</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DigitalGoodCollection FindByName(string nameToMatch, int maximumRows, int startRowIndex, string sortExpression)
        {
            nameToMatch = StringHelper.FixSearchPattern(nameToMatch);
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + DigitalGood.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_DigitalGoods");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" AND (Name LIKE @nameToMatch OR FileName LIKE @nameToMatch)");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@nameToMatch", System.Data.DbType.String, nameToMatch);
            //EXECUTE THE COMMAND
            DigitalGoodCollection results = new DigitalGoodCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        DigitalGood digitalGood = new DigitalGood();
                        DigitalGood.LoadDataReader(digitalGood, dr);
                        results.Add(digitalGood);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of DigitalGood objects for a product variant
        /// </summary>
        /// <param name="productId">ID of the product to count for</param>
        /// <param name="optionList">Option list that specifies the product variant</param>
        /// <returns>The number of DigitalGood objects for a product variant</returns>
        /// <remarks>If option list is null or empty, the count returned is for digital goods
        /// that are not associated with any variant</remarks>
        public static int CountForVariant(int productId, string optionList)
        {
            bool variantSpecified = !string.IsNullOrEmpty(optionList);
            Database database = Token.Instance.Database;
            StringBuilder sql = new StringBuilder();
            sql.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM ac_ProductDigitalGoods WHERE ProductId=@productId");
            if (variantSpecified) sql.Append(" AND OptionList=@optionList");
            else sql.Append(" AND OptionList IS NULL");
            DbCommand selectCommand = database.GetSqlStringCommand(sql.ToString());
            database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
            if (variantSpecified) database.AddInParameter(selectCommand, "@optionList", System.Data.DbType.String, optionList);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of DigitalGood objects for the variant from the database
        /// </summary>
        /// <param name="productId">ID of the product to count for</param>
        /// <param name="optionList">Option list that specifies the product variant</param>
        /// <returns>A collection of DigitalGood objects</returns>
        /// <remarks>If option list is null or empty, the collection returned is for digital goods
        /// that are not associated with any variant</remarks>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DigitalGoodCollection LoadForVariant(int productId, string optionList)
        {
            return LoadForVariant(productId, optionList, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of DigitalGood objects for the variant from the database. Sorts using the given sort exrpression.
        /// </summary>
        /// <param name="productId">ID of the product to count for</param>
        /// <param name="optionList">Option list that specifies the product variant</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of DigitalGood objects</returns>
        /// <remarks>If option list is null or empty, the collection returned is for digital goods
        /// that are not associated with any variant</remarks>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DigitalGoodCollection LoadForVariant(int productId, string optionList, string sortExpression)
        {
            return LoadForVariant(productId, optionList, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of DigitalGood objects for the variant from the database.
        /// </summary>
        /// <param name="productId">ID of the product to count for</param>
        /// <param name="optionList">Option list that specifies the product variant</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of DigitalGood objects</returns>
        /// <remarks>If option list is null or empty, the collection returned is for digital goods
        /// that are not associated with any variant</remarks>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DigitalGoodCollection LoadForVariant(int productId, string optionList, int maximumRows, int startRowIndex)
        {
            return LoadForVariant(productId, optionList, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of DigitalGood objects for the variant from the database. Sorts using the given sort exrpression.
        /// </summary>
        /// <param name="productId">ID of the product to count for</param>
        /// <param name="optionList">Option list that specifies the product variant</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of DigitalGood objects</returns>
        /// <remarks>If option list is null or empty, the collection returned is for digital goods
        /// that are not associated with any variant</remarks>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DigitalGoodCollection LoadForVariant(int productId, string optionList, int maximumRows, int startRowIndex, string sortExpression)
        {
            bool variantSpecified = !string.IsNullOrEmpty(optionList);
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + DigitalGood.GetColumnNames("DG"));
            selectQuery.Append(" FROM ac_DigitalGoods DG, ac_ProductDigitalGoods PDG");
            selectQuery.Append(" WHERE DG.DigitalGoodId = PDG.DigitalGoodId");
            selectQuery.Append(" AND PDG.ProductId = @productId");
            if (variantSpecified) selectQuery.Append(" AND PDG.OptionList = @optionList");
            else selectQuery.Append(" AND PDG.OptionList IS NULL");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
            if (variantSpecified) database.AddInParameter(selectCommand, "@optionList", System.Data.DbType.String, optionList);
            //EXECUTE THE COMMAND
            DigitalGoodCollection results = new DigitalGoodCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        DigitalGood digitalGood = new DigitalGood();
                        DigitalGood.LoadDataReader(digitalGood, dr);
                        results.Add(digitalGood);
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