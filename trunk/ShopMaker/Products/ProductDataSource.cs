using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using System.ComponentModel;
using MakerShop.Search;
using MakerShop.Utility;
using MakerShop.Stores;
using MakerShop.Catalog;

namespace MakerShop.Products
{
    /// <summary>
    /// DataSource class for Product objects
    /// </summary>
    [DataObject(true)]
    public partial class ProductDataSource
    {
        /// <summary>
        /// Load products for given basket id
        /// </summary>
        /// <param name="basketId">The basket id for which to load the products</param>
        /// <returns></returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static Dictionary<int, Product> LoadForBasket(int basketId)
        {
            Dictionary<int, Product> ProductLookup = new Dictionary<int, Product>();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            //GET RECORDS STARTING AT FIRST ROW
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + Product.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Products WHERE ProductId IN (SELECT DISTINCT ac_Products.ProductId FROM ac_Products, ac_BasketItems WHERE ac_Products.ProductId = ac_BasketItems.ProductId AND ac_BasketItems.BasketId = @basketId)");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@basketId", System.Data.DbType.Int32, basketId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    Product product = new Product();
                    Product.LoadDataReader(product, dr); ;
                    ProductLookup.Add(product.ProductId, product);
                }
                dr.Close();
            }
            return ProductLookup;
        }

        private static int[] GetFeaturedProductIds(int categoryId, bool publicOnly, bool includeOutOfStockItems)
        {
            List<int> productIds = new List<int>();
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT P.ProductId");
            if (categoryId > 0) selectQuery.Append(" FROM ac_Products P INNER JOIN ac_CatalogNodes PC ON P.ProductId = PC.CatalogNodeId");
            else selectQuery.Append(" FROM ac_Products P");
            selectQuery.Append(" WHERE P.StoreId = @storeId");
            if (categoryId > 0)
            {
                selectQuery.Append(" AND PC.CatalogNodeTypeId = " + (short)CatalogNodeType.Product);
                selectQuery.Append(" AND PC.CategoryId IN (SELECT CategoryId FROM ac_CategoryParents WHERE ParentId = @categoryId)");
            }
            if (publicOnly) selectQuery.Append(" AND P.VisibilityId = " + (short)CatalogVisibility.Public);
            selectQuery.Append(" AND P.IsFeatured = 1");

            if (!includeOutOfStockItems && Token.Instance.Store.EnableInventory)
            {
                selectQuery.Append(" AND ( P.InventoryModeId <> " + ((short)InventoryMode.Product).ToString());
                selectQuery.Append(" OR ( P.InventoryModeId = " + ((short)InventoryMode.Product).ToString());
                selectQuery.Append(" AND (P.InStock > 0 OR P.AllowBackorder = 1)");
                selectQuery.Append(" ))");
            }

            selectQuery.Append(" AND P.IsFeatured = 1");                        


            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "storeId", DbType.Int32, Token.Instance.StoreId);
            if (categoryId != 0) database.AddInParameter(selectCommand, "categoryId", DbType.Int32, categoryId);
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    productIds.Add(dr.GetInt32(0));
                }
                dr.Close();
            }
            return productIds.ToArray();
        }

        /// <summary>
        /// Gets a random selection of featured products(excluding the out of stock products)
        /// </summary>
        /// <param name="categoryId">The category to search for featured products; pass 0 to disable category filter</param>
        /// <param name="publicOnly">If true, only public items are included in the result</param>
        /// <param name="count">The maximum number of featured items to return</param>
        /// <returns>A collection of random featured products</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductCollection GetRandomFeaturedProducts(int categoryId, bool publicOnly, int count)
        {
            return GetRandomFeaturedProducts(categoryId, publicOnly, false, count);
        }

        /// <summary>
        /// Gets a random selection of featured products
        /// </summary>
        /// <param name="categoryId">The category to search for featured products; pass 0 to disable category filter</param>
        /// <param name="publicOnly">If true, only public items are included in the result</param>
        /// <param name="includeOutOfStockItems">If true, out of stock items will also be included in results</param>
        /// <param name="count">The maximum number of featured items to return</param>
        /// <returns>A collection of random featured products</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductCollection GetRandomFeaturedProducts(int categoryId, bool publicOnly, bool includeOutOfStockItems, int count)
        {
            int[] productIds = ProductDataSource.GetFeaturedProductIds(categoryId, publicOnly, includeOutOfStockItems);
            if ((productIds.Length == 0) || (count < 1)) return null;
            List<int> productIdList = new List<int>();
            productIdList.AddRange(productIds);
            ProductCollection featuredProducts = new ProductCollection();
            for (int i = 0; ((i < count) && (productIdList.Count > 0)); i++)
            {
                int randomIndex = new Random().Next(productIdList.Count);
                Product product = ProductDataSource.Load(productIdList[randomIndex]);
                featuredProducts.Add(product);
                productIdList.RemoveAt(randomIndex);
            }
            return featuredProducts;
        }

        /// <summary>
        /// Gets a random selection of featured products (excluding the out of stock products)
        /// </summary>
        /// <param name="categoryId">The category to search for featured products; pass 0 to disable category filter</param>
        /// <param name="publicOnly">If true, only public items are included in the result</param>
        /// <returns>A collection of random featured products</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductCollection GetFeaturedProducts(int categoryId, bool publicOnly)
        {
            return GetFeaturedProducts(categoryId, publicOnly, false, 0, 0, string.Empty);
        }

        /// <summary>
        /// Gets a random selection of featured products
        /// </summary>
        /// <param name="categoryId">The category to search for featured products; pass 0 to disable category filter</param>
        /// <param name="publicOnly">If true, only public items are included in the result</param>
        /// <param name="includeOutOfStockItems">If true, out of stock items will also be included in results</param>
        /// <returns>A collection of random featured products</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductCollection GetFeaturedProducts(int categoryId, bool publicOnly, bool includeOutOfStockItems)
        {
            return GetFeaturedProducts(categoryId, publicOnly, includeOutOfStockItems, 0, 0, string.Empty);
        }

        /// <summary>
        /// Gets a random selection of featured productsc (excluding the out of stock products)
        /// </summary>
        /// <param name="categoryId">The category to search for featured products; pass 0 to disable category filter</param>
        /// <param name="publicOnly">If true, only public items are included in the result</param>
        /// <param name="sortExpression">sort expression to use for sorting the loaded products</param>
        /// <returns>A collection of random featured products</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductCollection GetFeaturedProducts(int categoryId, bool publicOnly, string sortExpression)
        {
            return GetFeaturedProducts(categoryId, publicOnly, false, 0, 0, sortExpression);
        }


        /// <summary>
        /// Gets a random selection of featured products
        /// </summary>
        /// <param name="categoryId">The category to search for featured products; pass 0 to disable category filter</param>
        /// <param name="publicOnly">If true, only public items are included in the result</param>
        /// <param name="includeOutOfStockItems">If true, out of stock items will also be included in results</param>
        /// <param name="sortExpression">sort expression to use for sorting the loaded products</param>
        /// <returns>A collection of random featured products</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductCollection GetFeaturedProducts(int categoryId, bool publicOnly, bool includeOutOfStockItems, string sortExpression)
        {
            return GetFeaturedProducts(categoryId, publicOnly, includeOutOfStockItems, 0, 0, sortExpression);
        }

        /// <summary>
        /// Gets a random selection of featured products (excluding the out of stock products)
        /// </summary>
        /// <param name="categoryId">The category to search for featured products; pass 0 to disable category filter</param>
        /// <param name="publicOnly">If true, only public items are included in the result</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <returns>A collection of random featured products</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductCollection GetFeaturedProducts(int categoryId, bool publicOnly, int maximumRows, int startRowIndex)
        {
            return GetFeaturedProducts(categoryId, publicOnly, false, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Gets a random selection of featured products
        /// </summary>
        /// <param name="categoryId">The category to search for featured products; pass 0 to disable category filter</param>
        /// <param name="publicOnly">If true, only public items are included in the result</param>
        /// <param name="includeOutOfStockItems">If true, out of stock items will also be included in results</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <returns>A collection of random featured products</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductCollection GetFeaturedProducts(int categoryId, bool publicOnly, bool includeOutOfStockItems, int maximumRows, int startRowIndex)
        {
            return GetFeaturedProducts(categoryId, publicOnly, includeOutOfStockItems, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Gets a random selection of featured products (excluding the out of stock products)
        /// </summary>
        /// <param name="categoryId">The category to search for featured products; pass 0 to disable category filter</param>
        /// <param name="publicOnly">If true, only public items are included in the result</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">sort expression to use for sorting the loaded products</param>
        /// <returns>A collection of random featured products</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductCollection GetFeaturedProducts(int categoryId, bool publicOnly, int maximumRows, int startRowIndex, string sortExpression)
        {
            return GetFeaturedProducts(categoryId, publicOnly, false, maximumRows, startRowIndex, string.Empty); 
        }

        /// <summary>
        /// Gets a random selection of featured products
        /// </summary>
        /// <param name="categoryId">The category to search for featured products; pass 0 to disable category filter</param>
        /// <param name="publicOnly">If true, only public items are included in the result</param>
        /// <param name="includeOutOfStockItems">If true, out of stock items will also be included in results</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">sort expression to use for sorting the loaded products</param>
        /// <returns>A collection of random featured products</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductCollection GetFeaturedProducts(int categoryId, bool publicOnly, bool includeOutOfStockItems, int maximumRows, int startRowIndex, string sortExpression)
        {
            ProductCollection featuredProducts = new ProductCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Product.GetColumnNames("P") + " FROM ac_Products P");
            selectQuery.Append(" WHERE P.ProductId IN (");
            //SUBQUERY TO GET THE CORRECT PRODUCTS FOR THE RESULT SET
            selectQuery.Append(" SELECT DISTINCT ");
            selectQuery.Append(" P.ProductId FROM ac_Products P INNER JOIN ac_CatalogNodes PC ON P.ProductId = PC.CatalogNodeId");
            selectQuery.Append(" WHERE P.StoreId = @storeId");
            if (categoryId != 0)
            {
                selectQuery.Append(" AND PC.CatalogNodeTypeId = " + (short)CatalogNodeType.Product);
                selectQuery.Append(" AND PC.CategoryId IN (SELECT CategoryId FROM ac_CategoryParents WHERE ParentId = @categoryId)");
            }
            if (publicOnly) selectQuery.Append(" AND P.VisibilityId = " + (short)CatalogVisibility.Public);

            // DO NOT INCLUDE OUT OF STOCK ITEMS
            if (!includeOutOfStockItems && Token.Instance.Store.EnableInventory)
            {
                selectQuery.Append(" AND ( P.InventoryModeId <> " + ((short)InventoryMode.Product).ToString());
                selectQuery.Append(" OR ( P.InventoryModeId = " + ((short)InventoryMode.Product).ToString());
                selectQuery.Append(" AND (P.InStock > 0 OR P.AllowBackorder = 1)");
                selectQuery.Append(" ))");
            }

            selectQuery.Append(" AND P.IsFeatured = 1");
            //CLOSE THE SUBQUERY
            selectQuery.Append(")");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "storeId", DbType.Int32, Token.Instance.StoreId);
            if (categoryId != 0) database.AddInParameter(selectCommand, "categoryId", DbType.Int32, categoryId);
            //EXECUTE THE COMMAND
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Product product = new Product();
                        Product.LoadDataReader(product, dr); ;
                        featuredProducts.Add(product);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return featuredProducts;
        }

        /// <summary>
        /// Gets a list of popular products
        /// </summary>
        /// <returns>A list of popular products</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> GetPopularProducts()
        {
            return ProductDataSource.GetPopularProducts(0);
        }

        /// <summary>
        /// Gets a list of popular products
        /// </summary>
        /// <param name="limit">maximum number of products to retrieve</param>
        /// <returns>A list of popular products</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> GetPopularProducts(int limit)
        {
            return ProductDataSource.GetPopularProducts(limit, 0);
        }

        /// <summary>
        /// Gets a list of popular products
        /// </summary>
        /// <param name="limit">maximum number of products to retrieve</param>
        /// <param name="prefferedCategoryId">The preferred category from which to load the popular products.</param>
        /// <returns>A list of popular products</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> GetPopularProducts(int limit, int prefferedCategoryId)
        {
            List<Product> popularProducts;
            String existingProductIdList = String.Empty;
            if (prefferedCategoryId > 0)
            {
                // IF PREFFERED CATEGORY IS SPECIFIED THEN LOAD THE PRODUCTS FROM THAT CATEGORY FIRST
                popularProducts = GetPopularProductsByCategory(prefferedCategoryId, limit);
                if (popularProducts.Count > 0)
                {
                    limit -= popularProducts.Count;

                    if (limit == 0) return popularProducts; // IF ENOUGH ITEMS ARE SELECTED 
                    else
                    {
                        String[] ids = new String[popularProducts.Count];
                        for (int i = 0; i < popularProducts.Count; i++)
                        {
                            ids[i] = popularProducts[i].ProductId.ToString();
                        }
                        existingProductIdList = String.Join(",", ids);
                    }
                }
            }
            else
            {
                popularProducts = new List<Product>();
            }

            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + Product.GetColumnNames(String.Empty) + " ");
            selectQuery.Append("FROM ac_Products WHERE ProductId IN (");
            selectQuery.Append(" SELECT " + (limit > 0 ? "TOP " + limit : string.Empty) + " ac_OrderItems.ProductId ");
            selectQuery.Append(" FROM ac_Products, ac_OrderItems ");
            selectQuery.Append(" WHERE ac_Products.ProductId = ac_OrderItems.ProductId AND ac_Products.StoreId = @storeId ");
            selectQuery.Append(" AND ac_Products.VisibilityId = " + (short)Catalog.CatalogVisibility.Public);
            if (!String.IsNullOrEmpty(existingProductIdList)) selectQuery.Append(" AND ac_Products.ProductId NOT IN (" + existingProductIdList + ")");
            selectQuery.Append(" GROUP BY ac_OrderItems.ProductId ORDER BY SUM(ac_OrderItems.Quantity) DESC");
            selectQuery.Append(")");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    Product product = new Product();
                    Product.LoadDataReader(product, dr); ;
                    popularProducts.Add(product);
                }
                dr.Close();
            }
            return popularProducts;
        }

        /// <summary>
        /// Gets a list of popular products by category
        /// </summary>
        /// <param name="categoryId">The category in which to look for popular products.</param>
        /// <param name="limit">number of products to load</param>
        /// <returns>A list of popular products for the given category</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> GetPopularProductsByCategory(int categoryId, int limit)
        {
            List<Product> popularProducts = new List<Product>();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + Product.GetColumnNames("P") + " ");
            selectQuery.Append("FROM ac_Products P WHERE P.ProductId IN (");
            selectQuery.Append(" SELECT " + (limit > 0 ? "TOP " + limit : string.Empty) + " ac_OrderItems.ProductId ");
            if (categoryId > 0)
                selectQuery.Append("FROM (ac_Products P1 INNER JOIN ac_CatalogNodes PC ON P1.ProductId = PC.CatalogNodeId), ac_OrderItems ");
            else
                selectQuery.Append("FROM ac_Products P1, ac_OrderItems ");
            selectQuery.Append("WHERE P1.ProductId = ac_OrderItems.ProductId AND P1.StoreId = @storeId ");
            if(categoryId >0)
                selectQuery.Append("AND PC.CategoryId = @categoryId AND PC.CatalogNodeTypeId = " + (short)CatalogNodeType.Product);
            selectQuery.Append("AND P1.VisibilityId = " + (short)Catalog.CatalogVisibility.Public);
            selectQuery.Append(" GROUP BY ac_OrderItems.ProductId ORDER BY SUM(ac_OrderItems.Quantity) DESC");
            selectQuery.Append(")");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            if (categoryId > 0) database.AddInParameter(selectCommand, "@categoryId", System.Data.DbType.Int32, categoryId);

            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    Product product = new Product();
                    Product.LoadDataReader(product, dr); ;
                    popularProducts.Add(product);
                }
                dr.Close();
            }
            return popularProducts;
        }

        

#region "FindProducts"

        /// <summary>
        /// Finds a list of products for a given name
        /// </summary>
        /// <param name="name">The product name to search for</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded products</param>
        /// <returns></returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> FindProductsByName(string name, int maximumRows, int startRowIndex, string sortExpression)
        {
            return ProductDataSource.FindProducts(name, string.Empty, 0, 0, 0, BitFieldState.Any, maximumRows, startRowIndex, sortExpression);
        }

        private static DbCommand GetFindProductsCommand(string fields, string name, string sku, int categoryId, int manufacturerId, int vendorId, BitFieldState featured, int taxCodeId, int maximumRows, int startRowIndex, string sortExpression)
        {
            //BUILD THE COMMAND
            StringBuilder selectQuery = new StringBuilder();
            if (!fields.StartsWith("COUNT(1)"))
            {
                selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
                if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
                selectQuery.Append(" " + fields + " FROM ac_Products");
            }
            else
            {
                selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + fields + " FROM ac_Products");
            }
            selectQuery.Append(" WHERE StoreId = @storeId");
            if (!string.IsNullOrEmpty(name))
            {
                name = StringHelper.FixSearchPattern(name);
                selectQuery.Append(" AND Name LIKE @name");
            }
            if (!string.IsNullOrEmpty(sku))
            {
                sku = StringHelper.FixSearchPattern(sku);
                selectQuery.Append(" AND Sku LIKE @sku");
            }
            if (categoryId != 0)
                selectQuery.Append(" AND ProductId IN (SELECT CatalogNodeId FROM ac_CatalogNodes WHERE CategoryId = @categoryId AND CatalogNodeTypeId = " + (short)CatalogNodeType.Product + ")");
            if (manufacturerId > 0)
                selectQuery.Append(" AND ManufacturerId = @manufacturerId");
            else if (manufacturerId < 0)  // TO SEARCH PRODUCTS WITH NO MANUFACTURER VALUE WILL BE IN MINUS
                selectQuery.Append(" AND ManufacturerId IS NULL OR ManufacturerId = 0");

            if (taxCodeId > 0)
                selectQuery.Append(" AND TaxCodeId = @taxCodeId");
            else if (taxCodeId < 0)  // TO SEARCH PRODUCTS WITH NO TAX CODE VALUE WILL BE IN MINUS
                selectQuery.Append(" AND (TaxCodeId IS NULL OR TaxCodeId = 0)");

            if (vendorId > 0)
                selectQuery.Append(" AND VendorId = @vendorId");
            else if (vendorId < 0)  // TO SEARCH PRODUCTS WITH NO VENDORS VALUE WILL BE IN MINUS
                selectQuery.Append(" AND (VendorId IS NULL OR VendorId = 0)");

            if (featured != BitFieldState.Any)
                selectQuery.Append(" AND IsFeatured = " + (featured == BitFieldState.True ? "1" : "0"));
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            //ADD PARAMETERS
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", DbType.Int32, Token.Instance.StoreId);
            if (!string.IsNullOrEmpty(name)) database.AddInParameter(selectCommand, "@name", DbType.String, name);
            if (!string.IsNullOrEmpty(sku)) database.AddInParameter(selectCommand, "@sku", DbType.String, sku);
            if (categoryId != 0) database.AddInParameter(selectCommand, "@categoryId", DbType.Int32, categoryId);
            if (manufacturerId > 0) database.AddInParameter(selectCommand, "@manufacturerId", DbType.Int32, manufacturerId);            
            if (vendorId != 0) database.AddInParameter(selectCommand, "@vendorId", DbType.Int32, vendorId);
            return selectCommand;
        }

        /// <summary>
        /// Finds the number of products for given parameters
        /// </summary>
        /// <param name="name">The name of the product</param>
        /// <param name="sku">The sku of the product</param>
        /// <param name="categoryId">The category Id of the product</param>
        /// <param name="manufacturerId">The manufacturer Id of the poduct</param>
        /// <param name="vendorId">The vendor Id of the product</param>
        /// <param name="featured">Whether the product is featured or not</param>
        /// <returns>The number of products mathcing the given parameters</returns>
        public static int FindProductsCount(string name, string sku, int categoryId, int manufacturerId, int vendorId, BitFieldState featured)
        {
            return FindProductsCount(name, sku, categoryId, manufacturerId, vendorId, featured, 0);
        }

        /// <summary>
        /// Finds the number of products for given parameters
        /// </summary>
        /// <param name="name">The name of the product</param>
        /// <param name="sku">The sku of the product</param>
        /// <param name="categoryId">The category Id of the product</param>
        /// <param name="manufacturerId">The manufacturer Id of the poduct</param>
        /// <param name="vendorId">The vendor Id of the product</param>
        /// <param name="featured">Whether the product is featured or not</param>
        /// <param name="taxCodeId">The tax code Id of the product</param>
        /// <returns>The number of products mathcing the given parameters</returns>
        public static int FindProductsCount(string name, string sku, int categoryId, int manufacturerId, int vendorId, BitFieldState featured, int taxCodeId)
        {
            //BUILD THE COMMAND
            DbCommand selectCommand = GetFindProductsCommand("COUNT(1) AS ProductCount", name, sku, categoryId, manufacturerId, vendorId, featured, taxCodeId, 0, 0, string.Empty);
            //EXECUTE COMMAND
            Database database = Token.Instance.Database;
            return ((int)database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Finds the products for given parameters
        /// </summary>
        /// <param name="name">The name of the product</param>
        /// <param name="sku">The sku of the product</param>
        /// <param name="categoryId">The category Id of the product</param>
        /// <param name="manufacturerId">The manufacturer Id of the poduct. Use specail value (-1) to search products without any manufacturers assigned.</param>
        /// <param name="vendorId">The vendor Id of the product. Use specail value (-1) to search products without any vendors assigned.</param>
        /// <returns>List of products mathcing the given parameters</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> FindProducts(string name, string sku, int categoryId, int manufacturerId, int vendorId)
        {
            return ProductDataSource.FindProducts(name, sku, categoryId, manufacturerId, vendorId, BitFieldState.Any, 0, 0, string.Empty);
        }

        /// <summary>
        /// Finds the products for given parameters
        /// </summary>
        /// <param name="name">The name of the product</param>
        /// <param name="sku">The sku of the product</param>
        /// <param name="categoryId">The category Id of the product</param>
        /// <param name="manufacturerId">The manufacturer Id of the poduct. Use specail value (-1) to search products without any manufacturers assigned.</param>
        /// <param name="vendorId">The vendor Id of the product. Use specail value (-1) to search products without any vendors assigned.</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded products</param>
        /// <returns>List of products mathcing the given parameters</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> FindProducts(string name, string sku, int categoryId, int manufacturerId, int vendorId, string sortExpression)
        {
            return ProductDataSource.FindProducts(name, sku, categoryId, manufacturerId, vendorId, BitFieldState.Any, 0, 0, sortExpression);
        }

        /// <summary>
        /// Finds the products for given parameters
        /// </summary>
        /// <param name="name">The name of the product</param>
        /// <param name="sku">The sku of the product</param>
        /// <param name="categoryId">The category Id of the product</param>
        /// <param name="manufacturerId">The manufacturer Id of the poduct. Use specail value (-1) to search products without any manufacturers assigned.</param>
        /// <param name="vendorId">The vendor Id of the product. Use specail value (-1) to search products without any vendors assigned.</param>
        /// <param name="featured">Whether the produt is featured or not</param>
        /// <returns>List of products mathcing the given parameters</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> FindProducts(string name, string sku, int categoryId, int manufacturerId, int vendorId, BitFieldState featured)
        {
            return ProductDataSource.FindProducts(name, sku, categoryId, manufacturerId, vendorId, featured, 0, 0, string.Empty);
        }

        /// <summary>
        /// Finds the products for given parameters
        /// </summary>
        /// <param name="name">The name of the product</param>
        /// <param name="sku">The sku of the product</param>
        /// <param name="categoryId">The category Id of the product</param>
        /// <param name="manufacturerId">The manufacturer Id of the poduct. Use specail value (-1) to search products without any manufacturers assigned.</param>
        /// <param name="vendorId">The vendor Id of the product. Use specail value (-1) to search products without any vendors assigned.</param>
        /// <param name="featured">Whether the produt is featured or not</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded products</param>
        /// <returns>List of products mathcing the given parameters</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> FindProducts(string name, string sku, int categoryId, int manufacturerId, int vendorId, BitFieldState featured, string sortExpression)
        {
            return ProductDataSource.FindProducts(name, sku, categoryId, manufacturerId, vendorId, featured, 0, 0, sortExpression);
        }

        /// <summary>
        /// Finds the products for given parameters
        /// </summary>
        /// <param name="name">The name of the product</param>
        /// <param name="sku">The sku of the product</param>
        /// <param name="categoryId">The category Id of the product</param>
        /// <param name="manufacturerId">The manufacturer Id of the poduct. Use specail value (-1) to search products without any manufacturers assigned.</param>
        /// <param name="vendorId">The vendor Id of the product. Use specail value (-1) to search products without any vendors assigned.</param>
        /// <param name="featured">Whether the produt is featured or not</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded products</param>
        /// <returns>List of products mathcing the given parameters</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> FindProducts(string name, string sku, int categoryId, int manufacturerId, int vendorId, BitFieldState featured, int maximumRows, int startRowIndex, string sortExpression)
        {
            return ProductDataSource.FindProducts(name, sku, categoryId, manufacturerId, vendorId, featured, 0, maximumRows, startRowIndex, sortExpression);
        }

        /// <summary>
        /// Finds the products for given parameters
        /// </summary>
        /// <param name="name">The name of the product</param>
        /// <param name="sku">The sku of the product</param>
        /// <param name="categoryId">The category Id of the product</param>
        /// <param name="manufacturerId">The manufacturer Id of the poduct. Use specail value (-1) to search products without any manufacturers assigned.</param>
        /// <param name="vendorId">The vendor Id of the product. Use specail value (-1) to search products without any vendors assigned.</param>
        /// <param name="featured">Whether the produt is featured or not</param>
        /// <param name="taxCodeId">The tax code Id of the product. Use specail value (-1) to search products without any vendors assigned.</param>
        /// <returns>List of products mathcing the given parameters</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> FindProducts(string name, string sku, int categoryId, int manufacturerId, int vendorId, BitFieldState featured, int taxCodeId)
        {
            return ProductDataSource.FindProducts(name, sku, categoryId, manufacturerId, vendorId, featured, taxCodeId, 0, 0, string.Empty);
        }

        /// <summary>
        /// Finds the products for given parameters
        /// </summary>
        /// <param name="name">The name of the product</param>
        /// <param name="sku">The sku of the product</param>
        /// <param name="categoryId">The category Id of the product</param>
        /// <param name="manufacturerId">The manufacturer Id of the poduct. Use specail value (-1) to search products without any manufacturers assigned.</param>
        /// <param name="vendorId">The vendor Id of the product. Use specail value (-1) to search products without any vendors assigned.</param>
        /// <param name="featured">Whether the produt is featured or not</param>
        /// <param name="taxCodeId">The tax code Id of the product. Use specail value (-1) to search products without any vendors assigned.</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded products</param>
        /// <returns>List of products mathcing the given parameters</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> FindProducts(string name, string sku, int categoryId, int manufacturerId, int vendorId, BitFieldState featured, int taxCodeId, string sortExpression)
        {
            return ProductDataSource.FindProducts(name, sku, categoryId, manufacturerId, vendorId, featured, taxCodeId, 0, 0, sortExpression);
        }

        /// <summary>
        /// Finds the products for given parameters
        /// </summary>
        /// <param name="name">The name of the product</param>
        /// <param name="sku">The sku of the product</param>
        /// <param name="categoryId">The category Id of the product</param>
        /// <param name="manufacturerId">The manufacturer Id of the poduct. Use specail value (-1) to search products without any manufacturers assigned.</param>
        /// <param name="vendorId">The vendor Id of the product. Use specail value (-1) to search products without any vendors assigned.</param>
        /// <param name="featured">Whether the produt is featured or not</param>
        /// <param name="taxCodeId">The tax code Id of the product. Use specail value (-1) to search products without any vendors assigned.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded products</param>
        /// <returns>List of products mathcing the given parameters</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> FindProducts(string name, string sku, int categoryId, int manufacturerId, int vendorId, BitFieldState featured, int taxCodeId, int maximumRows, int startRowIndex, string sortExpression)
        {
            //GET THE IDS THAT MATCH CRITERIA
            List<Product> results = new List<Product>();
            DbCommand selectCommand = GetFindProductsCommand(Product.GetColumnNames(string.Empty), name, sku, categoryId, manufacturerId, vendorId, featured, taxCodeId, maximumRows, startRowIndex, sortExpression);
            Database database = Token.Instance.Database;
            //EXECUTE THE COMMAND
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Product product = new Product();
                        Product.LoadDataReader(product, dr);
                        results.Add(product);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }
#endregion

        /// <summary>
        /// Finds Kits by name
        /// </summary>
        /// <param name="nameToMatch">The kit name to match</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded products</param>
        /// <returns>List of Kit Products matching the given name</returns>
        public static List<Product> FindKitsByName(string nameToMatch, int maximumRows, int startRowIndex, string sortExpression)
        {
            if (string.IsNullOrEmpty(nameToMatch)) return null; //this.LoadForStore(maximumRows, startRowIndex, sortExpression)
            List<Product> productList = new List<Product>();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            if (startRowIndex < 1)
            {
                //GET RECORDS STARTING AT FIRST ROW
                selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + Product.GetColumnNames(string.Empty));
                selectQuery.Append(" FROM ac_Products WHERE ProductId IN (SELECT");
                if (maximumRows > 0) selectQuery.Append(" TOP " + maximumRows);
                selectQuery.Append(" DISTINCT ac_Products.ProductId");
                selectQuery.Append(" FROM ac_Products, ac_ProductComponents");
                selectQuery.Append(" WHERE ac_Products.ProductId = ac_ProductComponents.ProductId");
                selectQuery.Append(" AND ac_Products.Name LIKE @nameToMatch");
                if (maximumRows > 0 && !string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
                selectQuery.Append(")");
                if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            }
            else
            {
                //GET RECORDS STARTING AT GIVEN ROW
                selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + Product.GetColumnNames(string.Empty));
                if (maximumRows > 0) selectQuery.Append(" FROM ac_Products WHERE ProductId IN (SELECT TOP " + (startRowIndex + maximumRows) + " ProductId");
                selectQuery.Append(" FROM ac_Products WHERE ProductId NOT IN (SELECT TOP " + startRowIndex + " DISTINCT ac_Products.ProductId");
                selectQuery.Append(" FROM ac_Products, ac_ProductComponents WHERE ac_Products.ProductId = ac_ProductComponents.ProductId AND ac_Products.Name LIKE @nameToMatch");
                if (maximumRows > 0)
                {
                    if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
                    selectQuery.Append(")");
                }
                if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
                selectQuery.Append(")");
            }
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@nameToMatch", System.Data.DbType.String, nameToMatch);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    Product product = new Product();
                    Product.LoadDataReader(product, dr);
                    productList.Add(product);
                }
                dr.Close();
            }
            return productList;
        }

        /// <summary>
        /// Count the number of results for a narrowing search.
        /// </summary>
        /// <param name="keyword">The keyword to match</param>
        /// <param name="categoryId">The category ID to search for products (search is recursive to all descendants)</param>
        /// <param name="manufacturerId">The manufacturer ID to use for filtering serach.</param>
        /// <param name="lowPrice">The lowest price to include in result.</param>
        /// <param name="highPrice">The highest price to include in result.</param>
        /// <returns>A count of the products that match the search criteria.</returns>
        public static int NarrowSearchCount(string keyword, int categoryId, int manufacturerId, LSDecimal lowPrice, LSDecimal highPrice)
        {
            // SEARCH ALL PRODUCTS BY DEFAULT
            return NarrowSearchCount(keyword, categoryId, manufacturerId, lowPrice, highPrice, false);
        }

        /// <summary>
        /// Count the number of results for a narrowing search.
        /// </summary>
        /// <param name="keyword">The keyword to match</param>
        /// <param name="categoryId">The category ID to search for products (search is recursive to all descendants)</param>
        /// <param name="manufacturerId">The manufacturer ID to use for filtering serach.</param>
        /// <param name="lowPrice">The lowest price to include in result.</param>
        /// <param name="highPrice">The highest price to include in result.</param>
        /// <param name="onlyFeatured">Indicate to search only featured products or all products</param>
        /// <returns>A count of the products that match the search criteria.</returns>
        public static int NarrowSearchCount(string keyword, int categoryId, int manufacturerId, LSDecimal lowPrice, LSDecimal highPrice, bool onlyFeatured)
        {
            StringBuilder selectQuery = new StringBuilder();
            //SELECT THE PRODUCT COLUMNS
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(DISTINCT P.ProductId) As TotalProducts FROM ");
            selectQuery.Append(" (ac_Products P INNER JOIN ac_CatalogNodes PC ON P.ProductId = PC.CatalogNodeId)");
            selectQuery.Append(" WHERE PC.CatalogNodeTypeId = " + (short)CatalogNodeType.Product);
            selectQuery.Append(" AND P.ProductId IN (");
            //SUBQUERY TO GET THE CORRECT PRODUCTS FOR THE RESULT SET
            selectQuery.Append(" SELECT DISTINCT P.ProductId FROM ");
            selectQuery.Append(GetNarrowSearchTables(categoryId, false));
            selectQuery.Append(" WHERE P.StoreId = @storeId");
            if (!string.IsNullOrEmpty(keyword))
            {
                string columnList = Store.GetCachedSettings().FullTextSearch ? "Name,SKU,ModelNumber,Summary,Description,ExtendedDescription,SearchKeywords" : "Name,SearchKeywords";
                string keywordFilter = KeywordSearchHelper.PrepareSqlFilterWithForcedSubstring("ac_Products", "P.", columnList, "@keyword", ref keyword);
                selectQuery.Append(keywordFilter);
            }
            if (categoryId != 0)
            {
                selectQuery.Append(" AND PC.CatalogNodeTypeId = " + (short)CatalogNodeType.Product);
                selectQuery.Append(" AND PC.CategoryId IN (SELECT CategoryId FROM ac_CategoryParents WHERE ParentId = @categoryId)");
            }
            if (manufacturerId != 0) selectQuery.Append(" AND P.ManufacturerId = @manufacturerId");
            if (lowPrice > 0) selectQuery.Append(" AND P.Price >= @lowPrice");
            if (highPrice > 0) selectQuery.Append(" AND P.Price <= @highPrice");
            selectQuery.Append(" AND P.VisibilityId = " + (short)CatalogVisibility.Public);
            if (onlyFeatured) selectQuery.Append(" AND P.IsFeatured = @isFeatured");
            //CLOSE THE SUBQUERY
            selectQuery.Append(")");
            
            //BUILD THE DBCOMMAND
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "storeId", DbType.Int32, Token.Instance.StoreId);
            if (!string.IsNullOrEmpty(keyword)) database.AddInParameter(selectCommand, "keyword", DbType.String, keyword);
            if (categoryId != 0) database.AddInParameter(selectCommand, "categoryId", DbType.Int32, categoryId);
            if (manufacturerId != 0) database.AddInParameter(selectCommand, "manufacturerId", DbType.Int32, manufacturerId);
            if (lowPrice > 0) database.AddInParameter(selectCommand, "lowPrice", DbType.Decimal, lowPrice);
            if (highPrice > 0) database.AddInParameter(selectCommand, "highPrice", DbType.Decimal, highPrice);
            if (onlyFeatured) database.AddInParameter(selectCommand, "isFeatured", DbType.Boolean, true);
            //EXECUTE THE COMMAND
            return AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Count the number of results for a narrowing search by manufacturer.
        /// </summary>
        /// <param name="keyword">The keyword to match</param>
        /// <param name="categoryId">The category ID to search for products (search is recursive to all descendants)</param>
        /// <param name="lowPrice">The lowest price to include in result.</param>
        /// <param name="highPrice">The highest price to include in result.</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A List of ManufacturerProductCount objects representing the counts for each manufacturer</returns>
        public static List<ManufacturerProductCount> NarrowSearchCountByManufacturer(string keyword, int categoryId, LSDecimal lowPrice, LSDecimal highPrice, string sortExpression)
        {
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT M.ManufacturerId, M.Name, COUNT(DISTINCT P.ProductId) As ProductCount FROM ");
            selectQuery.Append(GetNarrowSearchTables(categoryId, true));
            selectQuery.Append(" WHERE P.StoreId = @storeId");
            if (!string.IsNullOrEmpty(keyword))
            {
                string columnList = Store.GetCachedSettings().FullTextSearch ? "Name,SKU,ModelNumber,Summary,Description,ExtendedDescription,SearchKeywords" : "Name,SearchKeywords";
                string keywordFilter = KeywordSearchHelper.PrepareSqlFilterWithForcedSubstring("ac_Products", "P.", columnList, "@keyword", ref keyword);
                selectQuery.Append(keywordFilter);
            }
            if (categoryId != 0) selectQuery.Append(" AND PC.CategoryId IN (SELECT CategoryId FROM ac_CategoryParents WHERE ParentId = @categoryId) AND PC.CatalogNodeTypeId=1");
            if (lowPrice > 0) selectQuery.Append(" AND P.Price >= @lowPrice");
            if (highPrice > 0) selectQuery.Append(" AND P.Price <= @highPrice");
            selectQuery.Append(" AND P.VisibilityId = " + (short)CatalogVisibility.Public);
            selectQuery.Append(" GROUP BY M.ManufacturerId, M.Name");
            if (!string.IsNullOrEmpty(sortExpression))
            {
                if (sortExpression.StartsWith("Name")) sortExpression = "M." + sortExpression;
                selectQuery.Append(" ORDER BY " + sortExpression);
            }
            else
            {
                selectQuery.Append(" ORDER BY ProductCount DESC");
            }
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "storeId", DbType.Int32, Token.Instance.StoreId);
            if (!string.IsNullOrEmpty(keyword)) database.AddInParameter(selectCommand, "keyword", DbType.String, keyword);
            if (categoryId != 0) database.AddInParameter(selectCommand, "categoryId", DbType.Int32, categoryId);
            if (lowPrice > 0) database.AddInParameter(selectCommand, "lowPrice", DbType.Decimal, lowPrice);
            if (highPrice > 0) database.AddInParameter(selectCommand, "highPrice", DbType.Decimal, highPrice);
            List<ManufacturerProductCount> results = new List<ManufacturerProductCount>();
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    int manufacturerId = NullableData.GetInt32(dr, 0);
                    if (manufacturerId != 0)
                    {
                        ManufacturerProductCount mpc = new ManufacturerProductCount(manufacturerId, dr.GetString(1), dr.GetInt32(2));
                        results.Add(mpc);
                    }
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Gets List of products matching a narrowing search.
        /// </summary>
        /// <param name="keyword">keyword to match</param>
        /// <param name="categoryId">The category ID to search for products (search is recursive to all descendants)</param>
        /// <param name="manufacturerId">The manufacturer ID to use for filtering serach.</param>
        /// <param name="lowPrice">The lowest price to include in result.</param>
        /// <param name="highPrice">The highest price to include in result.</param>
        /// <returns>List of products matching a narrowing search</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> NarrowSearch(string keyword, int categoryId, int manufacturerId, LSDecimal lowPrice, LSDecimal highPrice)
        {
            return NarrowSearch(keyword, categoryId, manufacturerId, lowPrice, highPrice, 0, 0, string.Empty);
        }

        /// <summary>
        /// Gets List of products matching a narrowing search.
        /// </summary>
        /// <param name="keyword">keyword to match</param>
        /// <param name="categoryId">The category ID to search for products (search is recursive to all descendants)</param>
        /// <param name="manufacturerId">The manufacturer ID to use for filtering serach.</param>
        /// <param name="lowPrice">The lowest price to include in result.</param>
        /// <param name="highPrice">The highest price to include in result.</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded products</param>
        /// <returns>List of products matching a narrowing search</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> NarrowSearch(string keyword, int categoryId, int manufacturerId, LSDecimal lowPrice, LSDecimal highPrice, string sortExpression)
        {
            return NarrowSearch(keyword, categoryId, manufacturerId, lowPrice, highPrice, 0, 0, sortExpression);
        }

        private static string GetNarrowSearchTables(int categoryId, bool includeManufacturer)
        {
            bool filterCategory = (categoryId != 0);
            if (filterCategory)
            {
                if (!includeManufacturer)
                {
                    return "(ac_Products P INNER JOIN ac_CatalogNodes PC ON P.ProductId = PC.CatalogNodeId)";
                }
                else
                {
                    return "((ac_Products P INNER JOIN ac_CatalogNodes PC ON P.ProductId = PC.CatalogNodeId) LEFT JOIN ac_Manufacturers M ON P.ManufacturerId = M.ManufacturerId)";
                }
            }
            else
            {
                if (!includeManufacturer)
                {
                    return "ac_Products P";
                }
                else
                {
                    return "(ac_Products P LEFT JOIN ac_Manufacturers M ON P.ManufacturerId = M.ManufacturerId)";
                }
            }
        }

        /// <summary>
        /// Gets List of products matching a narrowing search.
        /// </summary>
        /// <param name="keyword">keyword to match</param>
        /// <param name="categoryId">The category ID to search for products (search is recursive to all descendants)</param>
        /// <param name="manufacturerId">The manufacturer ID to use for filtering serach.</param>
        /// <param name="lowPrice">The lowest price to include in result.</param>
        /// <param name="highPrice">The highest price to include in result.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded products</param>
        /// <returns>List of products matching a narrowing search</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> NarrowSearch(string keyword, int categoryId, int manufacturerId, LSDecimal lowPrice, LSDecimal highPrice, int maximumRows, int startRowIndex, string sortExpression)
        {
            // SEE IF WE CAN RETURN THIS RESULT FROM THE THREAD CACHE
            string criteriaHash = "{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}";
            criteriaHash = string.Format(criteriaHash, keyword, categoryId, manufacturerId, lowPrice, highPrice, maximumRows, startRowIndex, sortExpression);
            SearchCacheWrapper cachedSearch = ContextCache.GetObject("NarrowSearch") as SearchCacheWrapper;
            if (cachedSearch != null && cachedSearch.CriteriaHash == criteriaHash)
            {
                return cachedSearch.Result as List<Product>;
            }

            //SEE IF WE NEED TO INCLUDE MANFUACTURER NAME IN SORT
            bool sortByManufacturer = sortExpression.StartsWith("Manufacturer");

            // SEE IF WE NEED TO DISPLAY ONLY FEATURED PRODUCTS
            bool sortByFeatured = sortExpression.StartsWith("IsFeatured");

            string selectColumns, fromString, groupByColumns;
            StringBuilder selectQuery = new StringBuilder();
            //SELECT THE PRODUCT COLUMNS
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT ");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());

            //selectColumns = " " + Product.GetColumnNames("P");
            selectColumns = " P.ProductId, P.Name, P.Sku, P.Price, P.MSRP, P.CostOfGoods, P.Weight, P.Length, P.Width, P.Height, P.IsFeatured ";
            groupByColumns = selectColumns;
            //Default sort order should be 'OrderBy'. Include this field
            selectColumns += ", MIN(PC.OrderBy) AS OrderBy";
            //ALSO NEED TO INCLUDE MANUFACTURER COLUMN IF IT IS THE SORT KEY
            if (sortByManufacturer)
            {
                selectColumns += ", M.Name";
                groupByColumns += ", M.Name";
                fromString = " FROM ((ac_Products P INNER JOIN ac_CatalogNodes PC ON P.ProductId = PC.CatalogNodeId) LEFT JOIN ac_Manufacturers M ON P.ManufacturerId = M.ManufacturerId)";
            }
            else
            {
                fromString = " FROM (ac_Products P INNER JOIN ac_CatalogNodes PC ON P.ProductId = PC.CatalogNodeId)";
            }

            selectQuery.Append(selectColumns);
            selectQuery.Append(fromString);

            selectQuery.Append(" WHERE PC.CatalogNodeTypeId = " + (short)CatalogNodeType.Product);
            selectQuery.Append(" AND P.ProductId IN (");
            //SUBQUERY TO GET THE CORRECT PRODUCTS FOR THE RESULT SET
            selectQuery.Append(" SELECT DISTINCT P.ProductId FROM ");
            selectQuery.Append(GetNarrowSearchTables(categoryId, false));
            selectQuery.Append(" WHERE P.StoreId = @storeId");
            if (!string.IsNullOrEmpty(keyword))
            {
                string columnList = Store.GetCachedSettings().FullTextSearch ? "Name,SKU,ModelNumber,Summary,Description,ExtendedDescription,SearchKeywords" : "Name,SearchKeywords";
                string keywordFilter = KeywordSearchHelper.PrepareSqlFilterWithForcedSubstring("ac_Products", "P.", columnList, "@keyword", ref keyword);
                selectQuery.Append(keywordFilter);
            }
            if (categoryId != 0)
            {
                selectQuery.Append(" AND PC.CatalogNodeTypeId = " + (short)CatalogNodeType.Product);
                selectQuery.Append(" AND PC.CategoryId IN (SELECT CategoryId FROM ac_CategoryParents WHERE ParentId = @categoryId)");
            }
            if (manufacturerId != 0) selectQuery.Append(" AND P.ManufacturerId = @manufacturerId");
            if (lowPrice > 0) selectQuery.Append(" AND P.Price >= @lowPrice");
            if (highPrice > 0) selectQuery.Append(" AND P.Price <= @highPrice");
            selectQuery.Append(" AND P.VisibilityId = " + (short)CatalogVisibility.Public);
            if (sortByFeatured) selectQuery.Append(" AND P.IsFeatured = @isFeatured");
            //CLOSE THE SUBQUERY
            selectQuery.Append(")");
            selectQuery.Append(" GROUP BY " + groupByColumns + " ");
            if (sortByManufacturer) sortExpression = sortExpression.Replace("Manufacturer", "M.Name");
            if (!string.IsNullOrEmpty(sortExpression))
            {
                selectQuery.Append(" ORDER BY " + sortExpression);
            }
            else
            {
                selectQuery.Append(" ORDER BY OrderBy");
            }
            //BUILD THE DBCOMMAND
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "storeId", DbType.Int32, Token.Instance.StoreId);
            if (!string.IsNullOrEmpty(keyword)) database.AddInParameter(selectCommand, "keyword", DbType.String, keyword);
            if (categoryId != 0) database.AddInParameter(selectCommand, "categoryId", DbType.Int32, categoryId);
            if (manufacturerId != 0) database.AddInParameter(selectCommand, "manufacturerId", DbType.Int32, manufacturerId);
            if (lowPrice > 0) database.AddInParameter(selectCommand, "lowPrice", DbType.Decimal, lowPrice);
            if (highPrice > 0) database.AddInParameter(selectCommand, "highPrice", DbType.Decimal, highPrice);
            if (sortByFeatured) database.AddInParameter(selectCommand, "isFeatured", DbType.Boolean, true);
            //EXECUTE THE COMMAND
            List<int> productIds = new List<int>();
            int thisIndex = 0;
            int rowCount = 0;
            int prodId;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        prodId = dr.GetInt32(0);
                        productIds.Add(prodId);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            List<Product> products = LoadProductsInOrder(productIds);
            ContextCache.SetObject("NarrowSearch", new SearchCacheWrapper(criteriaHash, products));
            return products;
        }

        /// <summary>
        /// Gets a list of products matching the advanced search criteria
        /// </summary>
        /// <param name="keyword">keyword to match</param>
        /// <param name="categoryId">The category ID to search for products (search is recursive to all descendants)</param>
        /// <param name="manufacturerId">The manufacturer ID to use for filtering serach.</param>
        /// <param name="searchName">If true product name field is searched</param>
        /// <param name="searchDescription">If true product description field is searched</param>
        /// <param name="searchSKU">If true product SKU field is searched</param>
        /// <param name="lowPrice">The lowest price to include in result.</param>
        /// <param name="highPrice">The highest price to include in result.</param>
        /// <returns>A List of products matching the advanced search criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> AdvancedSearch(string keyword, int categoryId, int manufacturerId, bool searchName, bool searchDescription, bool searchSKU, decimal lowPrice, decimal highPrice)
        {
            return AdvancedSearch(keyword, categoryId, manufacturerId, searchName, searchDescription, searchSKU, lowPrice, highPrice, 0, 0, String.Empty);
        }

        /// <summary>
        /// Gets a list of products matching the advanced search criteria
        /// </summary>
        /// <param name="keyword">keyword to match</param>
        /// <param name="categoryId">The category ID to search for products (search is recursive to all descendants)</param>
        /// <param name="manufacturerId">The manufacturer ID to use for filtering serach.</param>
        /// <param name="searchName">If true product name field is searched</param>
        /// <param name="searchDescription">If true product description field is searched</param>
        /// <param name="searchSKU">If true product SKU field is searched</param>
        /// <param name="lowPrice">The lowest price to include in result.</param>
        /// <param name="highPrice">The highest price to include in result.</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded products</param>
        /// <returns>A List of products matching the advanced search criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> AdvancedSearch(string keyword, int categoryId, int manufacturerId, bool searchName, bool searchDescription, bool searchSKU, decimal lowPrice, decimal highPrice, string sortExpression)
        {
            return AdvancedSearch(keyword, categoryId, manufacturerId, searchName, searchDescription, searchSKU, lowPrice, highPrice, 0, 0, sortExpression);
        }

        /// <summary>
        /// Gets a list of products matching the advanced search criteria
        /// </summary>
        /// <param name="keyword">keyword to match</param>
        /// <param name="categoryId">The category ID to search for products (search is recursive to all descendants)</param>
        /// <param name="manufacturerId">The manufacturer ID to use for filtering serach.</param>
        /// <param name="searchName">If true product name field is searched</param>
        /// <param name="searchDescription">If true product description field is searched</param>
        /// <param name="searchSKU">If true product SKU field is searched</param>
        /// <param name="lowPrice">The lowest price to include in result.</param>
        /// <param name="highPrice">The highest price to include in result.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded products</param>
        /// <returns>A List of products matching the advanced search criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> AdvancedSearch(string keyword, int categoryId, int manufacturerId, bool searchName, bool searchDescription, bool searchSKU, decimal lowPrice, decimal highPrice, int maximumRows, int startRowIndex, string sortExpression)
        {
            //SEE IF WE NEED TO INCLUDE MANFUACTURER NAME IN SORT
            bool sortByManufacturer = sortExpression.StartsWith("Manufacturer");

            string selectColumns, fromString, groupByColumns;
            StringBuilder selectQuery = new StringBuilder();            
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT ");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());

            //selectColumns = " " + Product.GetColumnNames("P");
            selectColumns = " P.ProductId, P.Name, P.Sku, P.Price, P.MSRP, P.CostOfGoods, P.Weight, P.Length, P.Width, P.Height, P.IsFeatured ";
            groupByColumns = selectColumns;
            //Default sort order should be 'OrderBy'. Include this field
            selectColumns += ", MIN(PC.OrderBy) AS OrderBy"; 

            //ALSO NEED TO INCLUDE MANUFACTURER COLUMN IF IT IS THE SORT KEY
            if (sortByManufacturer)
            {
                selectColumns += ", M.Name";
                groupByColumns += ", M.Name";
                fromString = " FROM ((ac_Products P INNER JOIN ac_CatalogNodes PC ON P.ProductId = PC.CatalogNodeId) LEFT JOIN ac_Manufacturers M ON P.ManufacturerId = M.ManufacturerId)";
            }
            else
            {
                fromString = " FROM (ac_Products P INNER JOIN ac_CatalogNodes PC ON P.ProductId = PC.CatalogNodeId)";
            }

            selectQuery.Append(selectColumns);
            selectQuery.Append(fromString);

            selectQuery.Append(" WHERE PC.CatalogNodeTypeId = " + (short)CatalogNodeType.Product);
            selectQuery.Append(" AND P.ProductId IN (");
            //SUBQUERY TO GET THE CORRECT PRODUCTS FOR THE RESULT SET
            selectQuery.Append(" SELECT DISTINCT P.ProductId FROM ");
            selectQuery.Append(GetNarrowSearchTables(categoryId, false));
            selectQuery.Append(" WHERE P.StoreId = @storeId");
            if (!string.IsNullOrEmpty(keyword))
            {
                // BUILD A LIST OF COLUMNS TO SEARCH
                StringBuilder columnList = new StringBuilder();
                columnList.Append("SearchKeywords");
                if (searchName) columnList.Append(",Name");
                if (searchDescription) columnList.Append(",Summary,Description,ExtendedDescription");
                if (searchSKU) columnList.Append(",SKU,ModelNumber");

                // PREPARE THE SQL FILTER
                string keywordFilter = KeywordSearchHelper.PrepareSqlFilterWithForcedSubstring("ac_Products", "P.", columnList.ToString(), "@keyword", ref keyword);
                selectQuery.Append(keywordFilter);
            }
            // CHECK FOR CATEGORY FILTER
            if (categoryId != 0)
            {
                selectQuery.Append(" AND PC.CatalogNodeTypeId = " + (short)CatalogNodeType.Product);
                selectQuery.Append(" AND PC.CategoryId IN (SELECT CategoryId FROM ac_CategoryParents WHERE ParentId = @categoryId)");
            }
            if (manufacturerId != 0) selectQuery.Append(" AND P.ManufacturerId = @manufacturerId");
            if (lowPrice > 0) selectQuery.Append(" AND P.Price >= @lowPrice");
            if (highPrice > 0) selectQuery.Append(" AND P.Price <= @highPrice");
            selectQuery.Append(" AND P.VisibilityId = " + (short)CatalogVisibility.Public);
            //CLOSE THE SUBQUERY
            selectQuery.Append(")");            
            selectQuery.Append(" GROUP BY " + groupByColumns + " ");
            if (sortByManufacturer) sortExpression = sortExpression.Replace("Manufacturer", "M.Name");
            if (string.IsNullOrEmpty(sortExpression))
            {
                selectQuery.Append(" ORDER BY OrderBy");
            }
            else
            {
                selectQuery.Append(" ORDER BY " + sortExpression);
            }

            //BUILD THE DBCOMMAND
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "storeId", DbType.Int32, Token.Instance.StoreId);
            if (!string.IsNullOrEmpty(keyword)) database.AddInParameter(selectCommand, "keyword", DbType.String, keyword);
            if (categoryId != 0) database.AddInParameter(selectCommand, "categoryId", DbType.Int32, categoryId);
            if (manufacturerId != 0) database.AddInParameter(selectCommand, "manufacturerId", DbType.Int32, manufacturerId);
            if (lowPrice > 0) database.AddInParameter(selectCommand, "lowPrice", DbType.Decimal, lowPrice);
            if (highPrice > 0) database.AddInParameter(selectCommand, "highPrice", DbType.Decimal, highPrice);
            //EXECUTE THE COMMAND
            List<int> productIds = new List<int>();
            int thisIndex = 0;
            int rowCount = 0;
            int prodId = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        prodId = dr.GetInt32(0);
                        productIds.Add(prodId);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }

            return LoadProductsInOrder(productIds);            
        }

        /// <summary>
        /// Count the number of results for a advanced search.
        /// </summary>
        /// <param name="keyword">keyword to match</param>
        /// <param name="categoryId">The category ID to search for products (search is recursive to all descendants)</param>
        /// <param name="manufacturerId">The manufacturer ID to use for filtering serach.</param>
        /// <param name="searchName">If true product name field is searched</param>
        /// <param name="searchDescription">If true product description field is searched</param>
        /// <param name="searchSKU">If true product SKU field is searched</param>
        /// <param name="lowPrice">The lowest price to include in result.</param>
        /// <param name="highPrice">The highest price to include in result.</param>
        /// <returns>A count of the products that match the search criteria.</returns>
        public static int AdvancedSearchCount(string keyword, int categoryId, int manufacturerId, bool searchName, bool searchDescription, bool searchSKU, decimal lowPrice, decimal highPrice)
        {
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(DISTINCT P.ProductId) As TotalProducts ");
            selectQuery.Append(" FROM (ac_Products P INNER JOIN ac_CatalogNodes PC ON P.ProductId = PC.CatalogNodeId)");            
            selectQuery.Append(" WHERE PC.CatalogNodeTypeId = " + (short)CatalogNodeType.Product);
            selectQuery.Append(" AND P.ProductId IN (");
            //SUBQUERY TO GET THE CORRECT PRODUCTS FOR THE RESULT SET
            selectQuery.Append(" SELECT DISTINCT P.ProductId FROM ");
            selectQuery.Append(GetNarrowSearchTables(categoryId, false));
            selectQuery.Append(" WHERE P.StoreId = @storeId");
            if (!string.IsNullOrEmpty(keyword))
            {
                // BUILD A LIST OF COLUMNS TO SEARCH
                StringBuilder columnList = new StringBuilder();
                columnList.Append("SearchKeywords");
                if (searchName) columnList.Append(",Name");
                if (searchDescription) columnList.Append(",Summary,Description,ExtendedDescription");
                if (searchSKU) columnList.Append(",SKU,ModelNumber");

                // PREPARE THE SQL FILTER
                string keywordFilter = KeywordSearchHelper.PrepareSqlFilterWithForcedSubstring("ac_Products", "P.", columnList.ToString(), "@keyword", ref keyword);
                selectQuery.Append(keywordFilter);
            }
            // CHECK FOR CATEGORY FILTER
            if (categoryId != 0)
            {
                selectQuery.Append(" AND PC.CatalogNodeTypeId = " + (short)CatalogNodeType.Product);
                selectQuery.Append(" AND PC.CategoryId IN (SELECT CategoryId FROM ac_CategoryParents WHERE ParentId = @categoryId)");
            }
            if (manufacturerId != 0) selectQuery.Append(" AND P.ManufacturerId = @manufacturerId");
            if (lowPrice > 0) selectQuery.Append(" AND P.Price >= @lowPrice");
            if (highPrice > 0) selectQuery.Append(" AND P.Price <= @highPrice");
            selectQuery.Append(" AND P.VisibilityId = " + (short)CatalogVisibility.Public);
            //CLOSE THE SUBQUERY
            selectQuery.Append(")");            
            //BUILD THE DBCOMMAND
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "storeId", DbType.Int32, Token.Instance.StoreId);
            if (!string.IsNullOrEmpty(keyword)) database.AddInParameter(selectCommand, "keyword", DbType.String, keyword);
            if (categoryId != 0) database.AddInParameter(selectCommand, "categoryId", DbType.Int32, categoryId);
            if (manufacturerId != 0) database.AddInParameter(selectCommand, "manufacturerId", DbType.Int32, manufacturerId);
            if (lowPrice > 0) database.AddInParameter(selectCommand, "lowPrice", DbType.Decimal, lowPrice);
            if (highPrice > 0) database.AddInParameter(selectCommand, "highPrice", DbType.Decimal, highPrice);
            //EXECUTE THE COMMAND
            return AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Gets a collection of products that should be included in the product feeds
        /// </summary>
        /// <returns>A collection of products that should be included in the product feeds</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductCollection LoadForFeed()
        {
            return LoadForFeed(0, 0, string.Empty);
        }

        /// <summary>
        /// Gets a collection of products that should be included in the product feeds
        /// </summary>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded products</param>
        /// <returns>A collection of products that should be included in the product feeds</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductCollection LoadForFeed(string sortExpression)
        {
            return LoadForFeed(0, 0, sortExpression);
        }

        /// <summary>
        /// Gets a collection of products that should be included in the product feeds
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded products</param>
        /// <returns>A collection of products that should be included in the product feeds</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductCollection LoadForFeed(int maximumRows, int startRowIndex, string sortExpression)
        {
            int storeId = Token.Instance.StoreId;
            ProductCollection Products = new ProductCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            if (startRowIndex < 1)
            {
                //GET RECORDS STARTING AT FIRST ROW
                selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
                if (maximumRows > 0) selectQuery.Append(" TOP " + maximumRows);
                selectQuery.Append(" " + Product.GetColumnNames(string.Empty));
                selectQuery.Append(" FROM ac_Products");
                selectQuery.Append(" WHERE StoreId = @storeId");
                selectQuery.Append(" AND ExcludeFromFeed = 0");
            }
            else
            {
                //GET RECORDS STARTING AT GIVEN ROW
                selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + Product.GetColumnNames(string.Empty));
                if (maximumRows > 0) selectQuery.Append(" FROM ac_Products WHERE ProductId IN (SELECT TOP " + (startRowIndex + maximumRows) + " ProductId");
                selectQuery.Append(" FROM ac_Products WHERE ProductId NOT IN (SELECT TOP " + startRowIndex + " ProductId");
                selectQuery.Append(" FROM ac_Products WHERE StoreId = @storeId");
                selectQuery.Append(" AND ExcludeFromFeed = 0");
                if (maximumRows > 0)
                {
                    if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
                    selectQuery.Append(")");
                }
                if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
                selectQuery.Append(")");
            }
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, storeId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    Product product = new Product();
                    Product.LoadDataReader(product, dr);
                    Products.Add(product);
                }
                dr.Close();
            }
            return Products;
        }

        /// <summary>
        /// Updates Inventory Mode of the given product
        /// </summary>
        /// <param name="productId">Id of the product for which to update the inventory mode</param>
        /// <param name="inventoryMode">The new inventory mode to set</param>
        public static void UpdateInventoryMode(int productId, InventoryMode inventoryMode)
        {
            Database database = Token.Instance.Database;
            StringBuilder updateQuery = new StringBuilder();
            updateQuery.Append("UPDATE ac_Products SET ");
            updateQuery.Append(" InventoryModeId = @InventoryModeId");
            updateQuery.Append(" WHERE ProductId = @ProductId");
            updateQuery.Append(" AND StoreId = @StoreId");

            using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
            {
                database.AddInParameter(updateCommand, "@InventoryModeId", System.Data.DbType.Byte, (byte)inventoryMode);
                database.AddInParameter(updateCommand, "@ProductId", System.Data.DbType.Int32, productId);
                database.AddInParameter(updateCommand, "@StoreId", System.Data.DbType.Int32, Token.Instance.StoreId);
                int result = database.ExecuteNonQuery(updateCommand);
            }
        }

        /// <summary>
        /// Class representing product counts for each manufacturer
        /// </summary>
        public class ManufacturerProductCount
        {
            private int _ManufacturerId;
            private string _Name;
            private int _ProductCount;
            
            /// <summary>
            /// The Manufacturer Id
            /// </summary>
            public int ManufacturerId { get { return _ManufacturerId; } }

            /// <summary>
            /// The Manufacturer name
            /// </summary>
            public string Name { get { return _Name; } }

            /// <summary>
            /// Number of products
            /// </summary>
            public int ProductCount { get { return _ProductCount; } }
            
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="manufacturerId">Manufacturer Id</param>
            /// <param name="name">Manufacturer Name</param>
            /// <param name="productCount">Number of products</param>
            public ManufacturerProductCount(int manufacturerId, string name, int productCount)
            {
                _ManufacturerId = manufacturerId;
                _Name = name;
                _ProductCount = productCount;
            }
        }

        /// <summary>
        /// Gets a List of orphaned products
        /// </summary>
        /// <returns>A List of orphaned products</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> LoadOrphaned()
        {
            return LoadOrphaned(0, 0, String.Empty);
        }

        /// <summary>
        /// Gets a List of orphaned products
        /// </summary>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded products</param>
        /// <returns>A List of orphaned products</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> LoadOrphaned(string sortExpression)
        {
            return LoadOrphaned(0, 0, sortExpression);
        }

        /// <summary>
        /// Gets a List of orphaned products
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded products</param>
        /// <returns>A List of orphaned products</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Product> LoadOrphaned(int maximumRows, int startRowIndex, string sortExpression)
        {
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Product.GetColumnNames("") + " FROM ");
            selectQuery.Append(" ac_Products");            
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" AND (ProductId NOT IN");
            selectQuery.Append(" (SELECT DISTINCT CatalogNodeId FROM ac_CatalogNodes WHERE (CatalogNodeTypeId = @catalogNodeTypeId))");
            selectQuery.Append(" )");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);


            //BUILD THE DBCOMMAND
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "storeId", DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "catalogNodeTypeId", DbType.Int16, CatalogNodeType.Product);            
            
            //EXECUTE THE COMMAND
            List<Product> results = new List<Product>();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Product product = new Product();
                        Product.LoadDataReader(product, dr);
                        results.Add(product);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Deletes all child data for a product.
        /// </summary>
        /// <param name="product">The product to delete child data for</param>
        internal static void DeleteChildData(Product product)
        {
            //DELETE OPTIONS THAT ARE ONLY ATTACHED TO THIS PRODUCT
            foreach (ProductOption po in product.ProductOptions)
            {
                ProductOptionCollection c = ProductOptionDataSource.LoadForOption(po.OptionId);
                if (c.Count == 1)
                {
                    //THIS IS THE ONLY PRODUCT ATTACHED TO THIS OPTION
                    po.Option.Delete();
                }
            }


            if(product.PaymentGatewayTemplate_Product!= null)
                    product.PaymentGatewayTemplate_Product.Delete();


           foreach (SubscriptionPlanDownsells downSells in product.SubscriptionPlanDownsellsCollection)
           {

               downSells.Delete();
           }

           foreach (SubscriptionPlanDetails planDetails in product.SubscriptionPlanDetails)
           {
               planDetails.Delete();
           }
           

            if (product.KitStatus == KitStatus.Master)
            {
                foreach (ProductKitComponent pkc in product.ProductKitComponents)
                {
                    if (pkc.KitComponent == null) continue;
                    //kit components can be shared. 
                    //Is this kit the only product using this kitcomponent?
                    if (pkc.KitComponent.ProductKitComponents.Count == 1)
                    {
                        foreach (KitProduct kp in pkc.KitComponent.KitProducts)
                        {
                            kp.Delete();
                        }
                    }
                }
            }

            //PROCESS DELETES FOR CHILD TABLES
            //FKTABLES ALL HAVE PRODUCTID FIELD AND SHOULD BE DELETED
            string[] fkTables = { "ac_BasketItems", "ac_CouponProducts", "ac_KitProducts", "ac_ProductAssets", "ac_ProductCustomFields", "ac_ProductDigitalGoods", "ac_ProductImages", "ac_ProductKitComponents", "ac_ProductOptions", "ac_ProductReviews", "ac_ProductTemplateFields", "ac_ProductVariants", "ac_ProductVolumeDiscounts", "ac_RelatedProducts", "ac_Specials", "ac_SubscriptionPlans", "ac_UpsellProducts", "ac_WishlistItems" };
            //FKTABLES2 ALL HAVE CHILDPRODUCTID FIELD AND SHOULD BE DELETED
            string[] fkTables2 = { "ac_RelatedProducts", "ac_UpsellProducts" };
            //FKTABLES3 ALL HAVE PRODUCTID FIELD BUT SHOULD BE NULLIFIED RATHER THAN DELETED
            string[] fkTables3 = { "ac_OrderItems", "ac_Subscriptions" };

            Database database = Token.Instance.Database;

            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null);
            DbCommand deleteCommand;
            int productId = product.ProductId;

            foreach (string tableName in fkTables)
            {
                deleteCommand = database.GetSqlStringCommand("DELETE FROM " + tableName + " WHERE ProductId = " + productId);
                database.ExecuteNonQuery(deleteCommand);
            }

            foreach (string tableName in fkTables2)
            {
                deleteCommand = database.GetSqlStringCommand("DELETE FROM " + tableName + " WHERE ChildProductId = " + productId);
                database.ExecuteNonQuery(deleteCommand);
            }

            foreach (string tableName in fkTables3)
            {
                deleteCommand = database.GetSqlStringCommand("UPDATE " + tableName + " SET ProductId = NULL WHERE ProductId = " + productId);
                database.ExecuteNonQuery(deleteCommand);
            }

            //DELETE ALL CATALOG NODES LINKED FROM THIS PRODUCT            
            deleteCommand = database.GetSqlStringCommand("DELETE FROM ac_CatalogNodes WHERE CatalogNodeTypeId = " + (short)CatalogNodeType.Product + " AND CatalogNodeId = " + productId);
            database.ExecuteNonQuery(deleteCommand);

            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
        }

        private static List<Product> LoadProductsInOrder(List<int> productIds)
        {
            List<Product> results = new List<Product>();
            if (productIds == null || productIds.Count == 0) return results;

            foreach (int productId in productIds)
            {
                Product prod = ProductDataSource.Load(productId);
                results.Add(prod);
            }
            return results;
        }

        private class SearchCacheWrapper
        {
            private string _CriteriaHash;
            private object _Result;
            public string CriteriaHash
            {
                get { return _CriteriaHash; }
                set { _CriteriaHash = value; }
            }
            public object Result
            {
                get { return _Result; }
                set { _Result = value; }
            }
            public SearchCacheWrapper(string criteriaHash, object result)
            {
                _CriteriaHash = criteriaHash;
                _Result = result;
            }
        }
    }
}
