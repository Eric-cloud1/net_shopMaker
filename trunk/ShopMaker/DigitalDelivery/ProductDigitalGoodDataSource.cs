using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;

namespace MakerShop.DigitalDelivery
{
    [DataObject(true)]
    public partial class ProductDigitalGoodDataSource
    {
        /// <summary>
        /// Counts the number of ProductDigitalGood objects for a product variant
        /// </summary>
        /// <param name="productId">ID of the product to count for</param>
        /// <param name="optionList">Option list that specifies the product variant</param>
        /// <returns>The number of ProductDigitalGood objects for a product variant</returns>
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
        /// Loads a collection of ProductDigitalGood objects for the variant from the database
        /// </summary>
        /// <param name="productId">ID of the product to count for</param>
        /// <param name="optionList">Option list that specifies the product variant</param>
        /// <returns>A collection of ProductDigitalGood objects</returns>
        /// <remarks>If option list is null or empty, the collection returned is for digital goods
        /// that are not associated with any variant</remarks>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductDigitalGoodCollection LoadForVariant(int productId, string optionList)
        {
            return LoadForVariant(productId, optionList, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of ProductDigitalGood objects for the variant from the database. Sorts using the given sort exrpression.
        /// </summary>
        /// <param name="productId">ID of the product to count for</param>
        /// <param name="optionList">Option list that specifies the product variant</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of ProductDigitalGood objects</returns>
        /// <remarks>If option list is null or empty, the collection returned is for digital goods
        /// that are not associated with any variant</remarks>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductDigitalGoodCollection LoadForVariant(int productId, string optionList, string sortExpression)
        {
            return LoadForVariant(productId, optionList, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of ProductDigitalGood objects for the variant from the database.
        /// </summary>
        /// <param name="productId">ID of the product to count for</param>
        /// <param name="optionList">Option list that specifies the product variant</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of ProductDigitalGood objects</returns>
        /// <remarks>If option list is null or empty, the collection returned is for digital goods
        /// that are not associated with any variant</remarks>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductDigitalGoodCollection LoadForVariant(int productId, string optionList, int maximumRows, int startRowIndex)
        {
            return LoadForVariant(productId, optionList, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of ProductDigitalGood objects for the variant from the database. Sorts using the given sort exrpression.
        /// </summary>
        /// <param name="productId">ID of the product to count for</param>
        /// <param name="optionList">Option list that specifies the product variant</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of ProductDigitalGood objects</returns>
        /// <remarks>If option list is null or empty, the collection returned is for digital goods
        /// that are not associated with any variant</remarks>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductDigitalGoodCollection LoadForVariant(int productId, string optionList, int maximumRows, int startRowIndex, string sortExpression)
        {
            bool variantSpecified = !string.IsNullOrEmpty(optionList);
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + ProductDigitalGood.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_ProductDigitalGoods");
            selectQuery.Append(" WHERE ProductId = @productId");
            if (variantSpecified) selectQuery.Append(" AND OptionList = @optionList");
            else selectQuery.Append(" AND OptionList IS NULL");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
            if (variantSpecified) database.AddInParameter(selectCommand, "@optionList", System.Data.DbType.String, optionList);
            //EXECUTE THE COMMAND
            ProductDigitalGoodCollection results = new ProductDigitalGoodCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        ProductDigitalGood digitalGood = new ProductDigitalGood();
                        ProductDigitalGood.LoadDataReader(digitalGood, dr);
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
        /// Loads a ProductDigitalGood object for given key from the database.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="digitalGoodId"></param>
        /// <returns>If the load is successful the newly loaded ProductDigitalGood object is returned. If load fails null is returned.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductDigitalGood Load(int productId, int digitalGoodId)
        {
            return ProductDigitalGoodDataSource.Load(productId, digitalGoodId, true);
        }

        /// <summary>
        /// Loads a ProductDigitalGood object for given key from the database.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="digitalGoodId"></param>
        /// <param name="useCache">If true tries to load object from cache first.</param>
        /// <returns>If the load is successful the newly loaded ProductDigitalGood object is returned. If load fails null is returned.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductDigitalGood Load(int productId, int digitalGoodId, bool useCache)
        {
            if (productId == 0) return null;
            if (digitalGoodId == 0) return null;

            ProductDigitalGood productDigitalGood = null;
            string key = "ProductDigitalGood_" + productId.ToString()+"_"+digitalGoodId.ToString();
            if (useCache)
            {
                productDigitalGood = ContextCache.GetObject(key) as ProductDigitalGood;
                if (productDigitalGood != null) return productDigitalGood;
            }
            productDigitalGood = new ProductDigitalGood();
            if (productDigitalGood.Load(productId, digitalGoodId))
            {
                if (useCache) ContextCache.SetObject(key, productDigitalGood);
                return productDigitalGood;
            }
            return null;
        }

 

    }
}
