using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;

namespace MakerShop.Products
{
    /// <summary>
    /// DataSource class for ProductReview objects
    /// </summary>
    [DataObject(true)]
    public partial class ProductReviewDataSource
    {
        /// <summary>
        /// Loads all unapproved product reviews
        /// </summary>
        /// <returns>A collection of unapproved product reviews</returns>
        public static ProductReviewCollection LoadUnApprovedReviews()
        {
            return LoadReviews(0,false);
        }

        /// <summary>
        /// Loads all unapproved product reviews for a given product id
        /// </summary>
        /// <param name="ProductID">Id of the product to load reviews for</param>
        /// <returns>A collection of unapproved product reviews</returns>
        public static ProductReviewCollection LoadUnApprovedReviews(int ProductID)
        {
            return LoadReviews(ProductID, false);
        }

        /// <summary>
        /// Loads all approved product reviews
        /// </summary>
        /// <returns>A collection of approved product reviews</returns>
        public static ProductReviewCollection LoadApprovedReviews()
        {
            return LoadReviews(0,true);
        }
        
        /// <summary>
        /// Loads all approved product reviews for a given product id
        /// </summary>
        /// <param name="ProductID">Id of the product to load reviews for</param>
        /// <returns>A collection of approved product reviews</returns>
        public static ProductReviewCollection LoadApprovedReviews(int ProductID)
        {
            return LoadReviews(ProductID, true);
        }

        /// <summary>
        /// Loads reviews for a given product id
        /// </summary>
        /// <param name="ProductID">Id of the product to load reviews for</param>
        /// <param name="isApproved">If true only approved reviewes are loaded. If false only unapproved reviewes are loaded.</param>
        /// <returns>A collection of ProductReview objects</returns>
        private static ProductReviewCollection LoadReviews(int ProductID, bool isApproved)
        {
            ProductReviewCollection ProductReviews = new ProductReviewCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            selectQuery.Append(" " + ProductReview.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_ProductReviews");
            selectQuery.Append(" WHERE IsApproved = @IsApproved");
            if(!int.Equals(0,ProductID)) selectQuery.Append(" AND ProductId = @productId");
            selectQuery.Append(" ORDER BY ReviewDate DESC");

            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@IsApproved", System.Data.DbType.Boolean, isApproved);
            if (!int.Equals(0, ProductID)) database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, ProductID);
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    ProductReview productReview = new ProductReview();
                    ProductReview.LoadDataReader(productReview, dr);
                    ProductReviews.Add(productReview);
                }
                dr.Close();
            }
            return ProductReviews;
        }
        
        /// <summary>
        /// Loads all product reviews
        /// </summary>
        /// <returns>A collection of all product reviews</returns>
        public static ProductReviewCollection LoadAllReviews()
        {
            ProductReviewCollection ProductReviews = new ProductReviewCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            selectQuery.Append(" " + ProductReview.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_ProductReviews");
            selectQuery.Append(" ORDER BY ReviewDate DESC ");

            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    ProductReview productReview = new ProductReview();
                    ProductReview.LoadDataReader(productReview, dr);
                    ProductReviews.Add(productReview);
                }
                dr.Close();
            }
            return ProductReviews;
        }
    
        /// <summary>
        /// Loads a product review for a given product from a particular reviewer
        /// </summary>
        /// <param name="productId">Id of the product to load review for</param>
        /// <param name="reviewerProfileId">Profile Id fo the reviewer</param>
        /// <returns>A product review for a given product from a particular reviewer</returns>
        public static ProductReview LoadForProductAndReviewerProfile(int productId, int reviewerProfileId)
        {
            if (productId == 0 || reviewerProfileId == 0)
            {
                return null;
            }

            ProductReview productReview = null;
            StringBuilder selectQuery = new StringBuilder(); 
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");            
            selectQuery.Append(" " + ProductReview.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_ProductReviews");
            selectQuery.Append(" WHERE ReviewerProfileId = @reviewerProfileId");
            selectQuery.Append(" AND ProductId = @productId");

            Database database = Token.Instance.Database;

            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());

            database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
            database.AddInParameter(selectCommand, "@reviewerProfileId", System.Data.DbType.Int32, reviewerProfileId);

            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    productReview = new ProductReview();
                    ProductReview.LoadDataReader(productReview, dr);
                }
                dr.Close();
            }
            return productReview;
        }

        /// <summary>
        /// Loads reviews for the given product
        /// </summary>
        /// <param name="ProductID">Id of the product to load review for</param>
        /// <param name="isApproved">If true only approved reviewes are loaded. If false only unapproved reviewes are loaded.</param>
        /// <param name="isEmailVerified">If true only reviews with verified emails are loaded. If false only reviews with unverified emails are loaded.</param>
        /// <returns>Collection of ProductReview objects</returns>
        public static ProductReviewCollection LoadForProduct(int ProductID, bool isApproved, bool isEmailVerified)
        {
            ProductReviewCollection ProductReviews = new ProductReviewCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            selectQuery.Append(" " + ProductReview.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_ProductReviews");
            selectQuery.Append(" WHERE ac_ProductReviews.ProductReviewId IN (");
            selectQuery.Append(" SELECT ac_ProductReviews.ProductReviewId");
            selectQuery.Append(" FROM ac_ProductReviews LEFT JOIN ac_ReviewerProfiles ON ac_ProductReviews.ReviewerProfileId = ac_ReviewerProfiles.ReviewerProfileId");
            selectQuery.Append(" WHERE ac_ProductReviews.IsApproved = @IsApproved");
            selectQuery.Append(" AND ac_ReviewerProfiles.EmailVerified = @IsEmailVerified )");
            if (!int.Equals(0, ProductID)) selectQuery.Append(" AND ProductId = @productId");
            selectQuery.Append(" ORDER BY ac_ProductReviews.ReviewDate DESC");

            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@IsApproved", System.Data.DbType.Boolean, isApproved);
            database.AddInParameter(selectCommand, "@IsEmailVerified", System.Data.DbType.Boolean, isEmailVerified);
            if (!int.Equals(0, ProductID)) database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, ProductID);
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    ProductReview productReview = new ProductReview();
                    ProductReview.LoadDataReader(productReview, dr);
                    ProductReviews.Add(productReview);
                }
                dr.Close();
            }
            return ProductReviews;
        }

        /// <summary>
        /// Counts the number of product reviews matching the given criteria
        /// </summary>
        /// <param name="productId">Id of the product to count reviews for</param>
        /// <param name="approved">Approval state of the reviews to include in the count</param>
        /// <returns>The number of product reviews matching the given criteria</returns>
        public static int SearchCount(Int32 productId, BitFieldState approved)
        {
            Database database = Token.Instance.Database;
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords");
            selectQuery.Append(" FROM ac_ProductReviews R INNER JOIN ac_Products P ON R.ProductId = P.ProductId");
            selectQuery.Append(" WHERE P.StoreId = @storeId");
            if (productId > 0) selectQuery.Append(" AND R.ProductId = @productId");
            if (approved != BitFieldState.Any)
                selectQuery.Append(" AND R.IsApproved = " + (approved == BitFieldState.True ? "1" : "0"));
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            if (productId > 0) database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Search product reviews matching the given criteria
        /// </summary>
        /// <param name="productId">Id of the product to search reviews for</param>
        /// <param name="approved">Approval state of the reviews to include in the search</param>
        /// <returns>Collection of ProductReview objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductReviewCollection Search(Int32 productId, BitFieldState approved)
        {
            return Search(productId, approved, 0, 0, string.Empty);
        }

        /// <summary>
        /// Search product reviews matching the given criteria
        /// </summary>
        /// <param name="productId">Id of the product to search reviews for</param>
        /// <param name="approved">Approval state of the reviews to include in the search</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>Collection of ProductReview objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductReviewCollection Search(Int32 productId, BitFieldState approved, string sortExpression)
        {
            return Search(productId, approved, 0, 0, sortExpression);
        }

        /// <summary>
        /// Search product reviews matching the given criteria
        /// </summary>
        /// <param name="productId">Id of the product to search reviews for</param>
        /// <param name="approved">Approval state of the reviews to include in the search</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <returns>Collection of ProductReview objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductReviewCollection Search(Int32 productId, BitFieldState approved, int maximumRows, int startRowIndex)
        {
            return Search(productId, approved, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Search product reviews matching the given criteria
        /// </summary>
        /// <param name="productId">Id of the product to search reviews for</param>
        /// <param name="approved">Approval state of the reviews to include in the search</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>Collection of ProductReview objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductReviewCollection Search(Int32 productId, BitFieldState approved, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + ProductReview.GetColumnNames("R"));
            selectQuery.Append(" FROM ((ac_ProductReviews R INNER JOIN ac_Products P ON R.ProductId = P.ProductId)");
            selectQuery.Append(" INNER JOIN ac_ReviewerProfiles RP ON R.ReviewerProfileId = RP.ReviewerProfileId)");
            selectQuery.Append(" WHERE P.StoreId = @storeId");
            if (productId > 0) selectQuery.Append(" AND R.ProductId = @productId");
            if (approved != BitFieldState.Any)
                selectQuery.Append(" AND R.IsApproved = " + (approved == BitFieldState.True ? "1" : "0"));
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            if (productId > 0) database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
            //EXECUTE THE COMMAND
            ProductReviewCollection results = new ProductReviewCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        ProductReview productReview = new ProductReview();
                        ProductReview.LoadDataReader(productReview, dr);
                        results.Add(productReview);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Calculates the average rating for a product.
        /// </summary>
        /// <param name="productId">ID of product to calculate rating for.</param>
        /// <returns>The average rating for a product.</returns>
        public static LSDecimal GetProductRating(int productId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT AVG(Rating) AS CurrentRating FROM ac_ProductReviews WHERE ProductId = @productID AND IsApproved = 1;");
            database.AddInParameter(selectCommand, "productId", DbType.Int32, productId);
            return AlwaysConvert.ToDecimal(database.ExecuteScalar(selectCommand));
        }        

    }
}
