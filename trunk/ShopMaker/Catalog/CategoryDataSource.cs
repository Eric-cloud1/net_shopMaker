using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;

namespace MakerShop.Catalog
{
    /// <summary>
    /// DataSource class for Category objects
    /// </summary>
    [DataObject(true)]
    public partial class CategoryDataSource
    {
        /// <summary>
        /// Gets Id of the parent category of the given category
        /// </summary>
        /// <param name="categoryId">Id of the category for which to get the parent category id</param>
        /// <returns>Id of the parent category of the given category</returns>
        public static int GetParentCategoryId(int categoryId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT ParentId FROM ac_CategoryParents WHERE CategoryId = @categoryId AND ParentNumber = 1");
            database.AddInParameter(selectCommand, "@categoryId", DbType.Int32, categoryId);
            int parentCategoryId = AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
            return parentCategoryId;
        }

        /// <summary>
        /// Gets name of a category given its Id
        /// </summary>
        /// <param name="categoryId">If of the category of which to get the name</param>
        /// <returns>Name of the category or empty string if category is not found</returns>
        public static string GetCategoryName(int categoryId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT Name FROM ac_Categories WHERE CategoryId = @categoryId");
            database.AddInParameter(selectCommand, "@categoryId", DbType.Int32, categoryId);
            string name = ""+database.ExecuteScalar(selectCommand);
            if (name == null) name = "";
            return name;
        }

        /// <summary>
        /// Gets the number of categories matching the given name
        /// </summary>
        /// <param name="nameToMatch">The category name to match</param>
        /// <returns>The number of categories matching the given name</returns>
        public static int FindByNameCount(string nameToMatch)
        {
            nameToMatch = StringHelper.FixSearchPattern(nameToMatch);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM ac_Categories WHERE StoreId = @storeId AND Name LIKE @nameToMatch");
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@nameToMatch", System.Data.DbType.String, nameToMatch);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Gets a collection of categories matching the given name
        /// </summary>
        /// <param name="nameToMatch">The category name to match</param>
        /// <returns>A collection of categories matching the given name</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CategoryCollection FindByName(string nameToMatch)
        {
            return CategoryDataSource.FindByName(nameToMatch, 0, 0, string.Empty);
        }

        /// <summary>
        /// Gets a collection of categories matching the given name
        /// </summary>
        /// <param name="nameToMatch">The category name to match</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded categories</param>
        /// <returns>A collection of categories matching the given name</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CategoryCollection FindByName(string nameToMatch, string sortExpression)
        {
            return CategoryDataSource.FindByName(nameToMatch, 0, 0, sortExpression);
        }

        /// <summary>
        /// Gets a collection of categories matching the given name
        /// </summary>
        /// <param name="nameToMatch">The category name to match</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <returns>A collection of categories matching the given name</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CategoryCollection FindByName(string nameToMatch, int maximumRows, int startRowIndex)
        {
            return CategoryDataSource.FindByName(nameToMatch, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Gets a collection of categories matching the given name
        /// </summary>
        /// <param name="nameToMatch">The category name to match</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded categories</param>
        /// <returns>A collection of categories matching the given name</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CategoryCollection FindByName(string nameToMatch, int maximumRows, int startRowIndex, string sortExpression)
        {
            nameToMatch = StringHelper.FixSearchPattern(nameToMatch);
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Category.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Categories");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" AND Name LIKE @nameToMatch");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@nameToMatch", System.Data.DbType.String, nameToMatch);
            //EXECUTE THE COMMAND
            CategoryCollection results = new CategoryCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Category category = new Category();
                        Category.LoadDataReader(category, dr);
                        results.Add(category);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Get all child categories of the given parent.
        /// </summary>
        /// <param name="categoryId">The parent category ID.</param>
        /// <param name="publicOnly">If true only public (published) categories are loaded.</param>
        /// <returns>A list of Category that contains all child categories of the given parent.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CategoryCollection LoadForParent(int categoryId, bool publicOnly) {
            CategoryCollection categoryList = new CategoryCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + Category.GetColumnNames("C"));
            selectQuery.Append(" FROM ac_Categories C, ac_CatalogNodes CN");
            selectQuery.Append(" WHERE C.StoreId = @storeId");
            selectQuery.Append(" AND C.ParentId = @categoryId");
            selectQuery.Append(" AND C.CategoryId = CN.CatalogNodeId");
            selectQuery.Append(" AND CN.CatalogNodeTypeId = 0");
            if (publicOnly) selectQuery.Append(" AND C.VisibilityId = 0");
            selectQuery.Append(" ORDER BY CN.OrderBy");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@categoryId", System.Data.DbType.Int32, categoryId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    Category category = new Category();
                    Category.LoadDataReader(category, dr);
                    categoryList.Add(category);
                }
                dr.Close();
            }
            return categoryList;
        }

        /// <summary>
        /// Loads a category record given the named path.
        /// </summary>
        /// <param name="path">A textual path of category names.</param>
        /// <param name="delimiter">The character string that delimits category names in the path.</param>
        /// <param name="create">If true, the named category path will be created if it does not already exist.  If false, null is returned if the path is not present.</param>
        /// <returns>The last category named in the path.</returns>
        public static Category LoadForPath(string path, string delimiter, bool create)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            if (string.IsNullOrEmpty(delimiter)) throw new ArgumentNullException("delimiter");
            if (delimiter.Length != 1) throw new ArgumentException("The delimiter value must be a single character.  Multi-character delimiters are not supported.", "delimiter");
            string[] categories = path.Split(delimiter.ToCharArray());
            int lastCategoryId = 0;
            Category lastCategory = null;
            foreach (string categoryName in categories)
            {
                lastCategory = LoadForName(categoryName.Trim(), lastCategoryId);
                if (lastCategory == null)
                {
                    if (!create) return null;
                    lastCategory = new Category();
                    lastCategory.ParentId = lastCategoryId;
                    lastCategory.Name = categoryName.Trim();
                    lastCategory.Visibility = CatalogVisibility.Public;
                    lastCategory.Save();
                }
                lastCategoryId = lastCategory.CategoryId;
            }
            return lastCategory;
        }

        /// <summary>
        /// Loads a category given the name.
        /// </summary>
        /// <param name="name">The name of the category to locate.</param>
        /// <param name="parentCategoryId">Optionally specifies the parent category to search within.  Pass -1 to search the entire catalog.</param>
        /// <returns>A category instance if the named category is found.  Returns null if the category does not exist.</returns>
        /// <remarks>Case sensitivity of the search is determined by the database.  If more than one matching category is found, only the first match is returned.</remarks>
        public static Category LoadForName(string name, int parentCategoryId)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(name);
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + Category.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Categories");
            selectQuery.Append(" WHERE StoreId = @storeId");
            if (parentCategoryId >= 0) selectQuery.Append(" AND ParentId = @parentCategoryId");
            selectQuery.Append(" AND Name = @name");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            if (parentCategoryId >= 0) database.AddInParameter(selectCommand, "@parentCategoryId", System.Data.DbType.Int32, parentCategoryId);
            database.AddInParameter(selectCommand, "@name", System.Data.DbType.String, name);
            //EXECUTE THE COMMAND
            Category category = null;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                if (dr.Read())
                {
                    category = new Category();
                    Category.LoadDataReader(category, dr);
                }
                dr.Close();
            }
            return category;
        }

        /// <summary>
        /// Counts the number of child categories of the given parent category
        /// </summary>
        /// <param name="categoryId">The parent category ID</param>
        /// <returns>The number of child categories of the given parent category</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static int CountForParent(int categoryId)
        {
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS RecordCount");
            selectQuery.Append(" FROM ac_Categories");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" AND ParentId = @categoryId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@categoryId", System.Data.DbType.Int32, categoryId);
            return AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }


        /// <summary>
        /// Gets a List of orphaned categories
        /// </summary>
        /// <returns>A List of orphaned categories</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Category> LoadOrphaned()
        {
            return LoadOrphaned(0, 0, String.Empty);
        }

        /// <summary>
        /// Gets a List of orphaned categories
        /// </summary>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded categories</param>
        /// <returns>A List of orphaned categories</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Category> LoadOrphaned(string sortExpression)
        {
            return LoadOrphaned(0, 0, sortExpression);
        }

        /// <summary>
        /// Gets a List of orphaned categories
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded categories</param>
        /// <returns>A List of orphaned categories</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Category> LoadOrphaned(int maximumRows, int startRowIndex, string sortExpression)
        {
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Category.GetColumnNames("") + " FROM ");
            selectQuery.Append(" ac_Categories");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" AND (CategoryId NOT IN");
            selectQuery.Append(" (SELECT DISTINCT CatalogNodeId FROM ac_CatalogNodes WHERE (CatalogNodeTypeId = @catalogNodeTypeId))");
            selectQuery.Append(" )");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);


            //BUILD THE DBCOMMAND
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "storeId", DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "catalogNodeTypeId", DbType.Int16, CatalogNodeType.Category);

            //EXECUTE THE COMMAND
            List<Category> results = new List<Category>();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Category category = new Category();
                        Category.LoadDataReader(category, dr);
                        results.Add(category);
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
