using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Products;
using MakerShop.Stores;
using MakerShop.Utility;

namespace MakerShop.Catalog
{
    /// <summary>
    /// DataSource class for store catalog
    /// </summary>
    public class CatalogDataSource
    {
        /// <summary>
        /// Loads a Catalogable object with given node Id and node type
        /// </summary>
        /// <param name="nodeId">Id of the object to load</param>
        /// <param name="nodeType">type of the object to load</param>
        /// <returns>ICatalogable object loaded</returns>
        public static ICatalogable Load(int nodeId, CatalogNodeType nodeType)
        {
            if (nodeId == 0) return null;
            ICatalogable catalogObject = null;
            switch (nodeType)
            {
                case CatalogNodeType.Category:
                    catalogObject = CategoryDataSource.Load(nodeId);
                    break;
                case CatalogNodeType.Product:
                    catalogObject = ProductDataSource.Load(nodeId);
                    break;
                case CatalogNodeType.Webpage:
                    catalogObject = WebpageDataSource.Load(nodeId);
                    break;
                case CatalogNodeType.Link:
                    catalogObject = LinkDataSource.Load(nodeId);
                    break;
            }
            return catalogObject;
        }

        /// <summary>
        /// Gets the display page associated with the given node
        /// </summary>
        /// <param name="nodeId">Id of the node for which to get the display page</param>
        /// <param name="nodeType">Type of the node for which to get the display page</param>
        /// <returns>The display page</returns>
        public static string GetDisplayPage(int nodeId, CatalogNodeType nodeType)
        {
            string displayPage;
            string defaultDisplayPage;
            string selectQuery;
            switch (nodeType)
            {
                case CatalogNodeType.Category:
                    selectQuery = "SELECT DisplayPage FROM ac_Categories WHERE CategoryId = @nodeId";
                    defaultDisplayPage = Token.Instance.Store.Settings.GetValueByKey("CategoryDisplayPage");
                    if (defaultDisplayPage == string.Empty) defaultDisplayPage = "Category.aspx";
                    break;
                case CatalogNodeType.Product:
                    selectQuery = "SELECT DisplayPage FROM ac_Products WHERE ProductId = @nodeId";
                    defaultDisplayPage = Token.Instance.Store.Settings.GetValueByKey("ProductDisplayPage");
                    if (defaultDisplayPage == string.Empty) defaultDisplayPage = "Product.aspx";
                    break;
                case CatalogNodeType.Webpage:
                    selectQuery = "SELECT DisplayPage FROM ac_Webpages WHERE WebpageId = @nodeId";
                    defaultDisplayPage = Token.Instance.Store.Settings.GetValueByKey("WebpageDisplayPage");
                    if (defaultDisplayPage == string.Empty) defaultDisplayPage = "Webpage.aspx";
                    break;
                case CatalogNodeType.Link:
                    selectQuery = "SELECT DisplayPage FROM ac_Links WHERE LinkId = @nodeId";
                    defaultDisplayPage = Token.Instance.Store.Settings.GetValueByKey("LinkDisplayPage");
                    if (defaultDisplayPage == string.Empty) defaultDisplayPage = "Link.aspx";
                    break;
                default:
                    throw new ArgumentException("Unrecognized node type.", "nodeType");
            }
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@nodeId", DbType.Int32, nodeId);
            displayPage = AlwaysConvert.ToString(database.ExecuteScalar(selectCommand));
            if (string.IsNullOrEmpty(displayPage)) displayPage = defaultDisplayPage;
            return displayPage;
        }

        /// <summary>
        /// Gets the category Id for the given node
        /// </summary>
        /// <param name="nodeId">Id of the node for which to get the category id</param>
        /// <param name="nodeType">Type of the node for which to get the category id</param>
        /// <returns>The category Id</returns>
        public static int GetCategoryId(int nodeId, CatalogNodeType nodeType)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT TOP 1 CategoryId FROM ac_CatalogNodes WHERE CatalogNodeId = @nodeId AND CatalogNodeTypeId = @nodeType");
            database.AddInParameter(selectCommand, "@nodeId", DbType.Int32, nodeId);
            database.AddInParameter(selectCommand, "@nodeType", DbType.Byte, nodeType);
            int categoryId = AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
            return categoryId;
        }

        /// <summary>
        /// Gets the traversal path(s) for a given category
        /// </summary>
        /// <param name="categoryId">Category for which to get the path</param>
        /// <param name="includeRoot">if true root is included in path</param>
        /// <returns>A List of paths to the given category</returns>
        public static List<CatalogPathNode> GetPath(int categoryId, bool includeRoot)
        {
            List<CatalogPathNode> path = new List<CatalogPathNode>();
            if (includeRoot)
            {
                path.Add(new CatalogPathNode(0, 0, CatalogNodeType.Category, CatalogVisibility.Public, "Catalog", string.Empty, string.Empty, string.Empty, string.Empty));
            }
            Database database = Token.Instance.Database;
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT parents.ParentId, category.VisibilityId, category.Name, category.Summary, category.ThumbnailUrl, category.ThumbnailAltText, category.Theme, parents.ParentLevel");
            selectQuery.Append(" FROM ac_CategoryParents parents, ac_Categories category");
            selectQuery.Append(" WHERE parents.ParentId = category.CategoryId AND parents.CategoryId = @categoryId");
            selectQuery.Append(" ORDER BY parents.ParentLevel");
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@categoryId", DbType.Int32, categoryId);
            //EXECUTE THE COMMAND
            int lastParentId = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    int catalogNodeId = dr.GetInt32(0);
                    CatalogVisibility visibility = (CatalogVisibility)dr.GetByte(1);
                    string name = dr.GetString(2);
                    string summary = NullableData.GetString(dr, 3);
                    string thumbnailUrl = NullableData.GetString(dr, 4);
                    string thumbnailAltText = NullableData.GetString(dr, 5);
                    string theme = NullableData.GetString(dr, 6);
                    path.Add(new CatalogPathNode(lastParentId, catalogNodeId, CatalogNodeType.Category, visibility, name, summary, thumbnailUrl, thumbnailAltText, theme));
                    lastParentId = catalogNodeId;
                }
                dr.Close();
            }
            return path;
        }

        /// <summary>
        /// Gets the traversal path(s) for a given category
        /// </summary>
        /// <param name="categoryId">Category for which to get the path</param>
        /// <param name="nodeId">Id of the node for which to get the path</param>
        /// <param name="nodeType">Type of the node for which to get the path</param>
        /// <param name="includeRoot">if true root is included in path</param>
        /// <returns>A List of paths to the given category</returns>
        public static List<CatalogPathNode> GetPath(int categoryId, int nodeId, CatalogNodeType nodeType, bool includeRoot)
        {
            List<CatalogPathNode> path = CatalogDataSource.GetPath(categoryId, includeRoot);
            switch (nodeType)
            {
                case CatalogNodeType.Product:
                    Product product = ProductDataSource.Load(nodeId);
                    if (product != null)
                    {
                        path.Add(new CatalogPathNode(categoryId, nodeId, CatalogNodeType.Product, product.Visibility, product.Name, product.Summary, product.ThumbnailUrl, product.ThumbnailAltText, product.Theme));
                    }
                    break;
                case CatalogNodeType.Webpage:
                    Webpage webpage = WebpageDataSource.Load(nodeId);
                    if (webpage != null)
                    {
                        path.Add(new CatalogPathNode(categoryId, nodeId, CatalogNodeType.Webpage, webpage.Visibility, webpage.Name, webpage.Summary, webpage.ThumbnailUrl, webpage.ThumbnailAltText, webpage.Theme));
                    }
                    break;
                default:
                    Link link = LinkDataSource.Load(nodeId);
                    if (link != null)
                    {
                        path.Add(new CatalogPathNode(categoryId, nodeId, CatalogNodeType.Link, link.Visibility, link.Name, link.Summary, link.ThumbnailUrl, link.ThumbnailAltText, link.Theme));
                    }
                    break;
            }
            return path;
        }

        /// <summary>
        /// Search catalog nodes for given search criteria
        /// </summary>
        /// <param name="categoryId">The category in which to perform the search</param>
        /// <param name="searchPhrase">The search phrase to search for</param>
        /// <param name="titlesOnly">If true only node titles are searched</param>
        /// <param name="publicOnly">If true only public (published) nodes are searched</param>
        /// <param name="recursive">If true search is performend recursively on the sub-categories</param>
        /// <param name="catalogNodeTypes">The type of catalog nodes to include in the search</param>
        /// <returns>Collection of catalog nodes matching the search criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CatalogNodeCollection Search(int categoryId, string searchPhrase, bool titlesOnly, bool publicOnly, bool recursive, CatalogNodeTypeFlags catalogNodeTypes)
        {
            return Search(categoryId, searchPhrase, titlesOnly, publicOnly, recursive, catalogNodeTypes, 0, 0, string.Empty);
        }

        /// <summary>
        /// Search catalog nodes for given search criteria
        /// </summary>
        /// <param name="categoryId">The category in which to perform the search</param>
        /// <param name="searchPhrase">The search phrase to search for</param>
        /// <param name="titlesOnly">If true only node titles are searched</param>
        /// <param name="publicOnly">If true only public (published) nodes are searched</param>
        /// <param name="recursive">If true search is performend recursively on the sub-categories</param>
        /// <param name="catalogNodeTypes">The type of catalog nodes to include in the search</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <returns>Collection of catalog nodes matching the search criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CatalogNodeCollection Search(int categoryId, string searchPhrase, bool titlesOnly, bool publicOnly, bool recursive, CatalogNodeTypeFlags catalogNodeTypes, int maximumRows, int startRowIndex)
        {
            return Search(categoryId, searchPhrase, titlesOnly, publicOnly, recursive, catalogNodeTypes, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Search catalog nodes for given search criteria
        /// </summary>
        /// <param name="categoryId">The category in which to perform the search</param>
        /// <param name="searchPhrase">The search phrase to search for</param>
        /// <param name="titlesOnly">If true only node titles are searched</param>
        /// <param name="publicOnly">If true only public (published) nodes are searched</param>
        /// <param name="recursive">If true search is performend recursively on the sub-categories</param>
        /// <param name="catalogNodeTypes">The type of catalog nodes to include in the search</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded products</param>
        /// <returns>Collection of catalog nodes matching the search criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CatalogNodeCollection Search(int categoryId, string searchPhrase, bool titlesOnly, bool publicOnly, bool recursive, CatalogNodeTypeFlags catalogNodeTypes, string sortExpression)
        {
            return Search(categoryId, searchPhrase, titlesOnly, publicOnly, recursive, catalogNodeTypes, 0, 0, sortExpression);
        }

        /// <summary>
        /// Search catalog nodes for given search criteria
        /// </summary>
        /// <param name="categoryId">The category in which to perform the search</param>
        /// <param name="searchPhrase">The search phrase to search for</param>
        /// <param name="titlesOnly">If true only node titles are searched</param>
        /// <param name="publicOnly">If true only public (published) nodes are searched</param>
        /// <param name="recursive">If true search is performend recursively on the sub-categories</param>
        /// <param name="catalogNodeTypes">The type of catalog nodes to include in the search</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded products</param>
        /// <returns>Collection of catalog nodes matching the search criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CatalogNodeCollection Search(int categoryId, string searchPhrase, bool titlesOnly, bool publicOnly, bool recursive, CatalogNodeTypeFlags catalogNodeTypes, int maximumRows, int startRowIndex, string sortExpression)
        {
            //MUST SPECIFY AT LEAST ONE CATALOG NODE TYPE
            if (catalogNodeTypes == CatalogNodeTypeFlags.None) throw new ArgumentException("You must specify at least one catalog node type.", "catalogNodeTypes");

            //DETERMINE THE CATEGORY FILTER
            string categoryFilterSql;
            if (recursive)
            {
                //this sql gets any category where the specified category is in the parent tree
                if (categoryId == 0)
                    categoryFilterSql = "((node.CategoryId = 0) OR (node.CategoryId IN (SELECT CategoryId FROM ac_CategoryParents WHERE ParentId = @categoryId)))";
                else
                    categoryFilterSql = "node.CategoryId IN (SELECT CategoryId FROM ac_CategoryParents WHERE ParentId = @categoryId)";
            }
            else
            {
                categoryFilterSql = "node.CategoryId = @categoryId";
            }

            //FIX THE SEARCH PHRASE FOR WILD CARD SUPPORT
            searchPhrase = StringHelper.FixSearchPattern(searchPhrase);

            //GENERATE QUERY
            StringBuilder selectQuery = new StringBuilder();
            if ((catalogNodeTypes & CatalogNodeTypeFlags.Category) == CatalogNodeTypeFlags.Category)
            {
                //CATEGORIES INCLUDED IN RESULTS
                selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT node.CategoryId, node.CatalogNodeId, node.CatalogNodeTypeId, node.OrderBy, child.VisibilityId, child.Name, child.Summary, child.ThumbnailUrl, child.ThumbnailAltText, 0 AS Price");
                selectQuery.Append(" FROM ac_CatalogNodes node, ac_Categories child");
                selectQuery.Append(" WHERE node.CatalogNodeTypeId = 0 AND node.CatalogNodeId = child.CategoryId AND " + categoryFilterSql);
                if (!string.IsNullOrEmpty(searchPhrase))
                {
                    if (titlesOnly)
                    {
                        selectQuery.Append(" AND child.Name LIKE @searchPhrase");
                    }
                    else
                    {
                        selectQuery.Append(" AND (child.Name LIKE @searchPhrase OR child.Summary LIKE @searchPhrase)");
                    }
                }
                if (publicOnly) selectQuery.Append(" AND child.VisibilityId = 0");
            }
            if ((catalogNodeTypes & CatalogNodeTypeFlags.Product) == CatalogNodeTypeFlags.Product)
            {
                //PRODCUTS INCLUDED IN RESULTS
                if (selectQuery.Length > 0) selectQuery.Append(" UNION ALL ");
                selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT node.CategoryId, node.CatalogNodeId, node.CatalogNodeTypeId, node.OrderBy, child.VisibilityId, child.Name, child.Summary, child.ThumbnailUrl, child.ThumbnailAltText, child.Price");
                selectQuery.Append(" FROM ac_CatalogNodes node, ac_Products child");
                selectQuery.Append(" WHERE node.CatalogNodeTypeId = 1 AND node.CatalogNodeId = child.ProductId AND " + categoryFilterSql);
                if (!string.IsNullOrEmpty(searchPhrase))
                {
                    if (titlesOnly)
                    {
                        selectQuery.Append(" AND child.Name LIKE @searchPhrase");
                    }
                    else
                    {
                        selectQuery.Append(" AND (child.Name LIKE @searchPhrase OR child.Summary LIKE @searchPhrase)");
                    }
                }
                if (publicOnly) selectQuery.Append(" AND child.VisibilityId = 0");
            }
            if ((catalogNodeTypes & CatalogNodeTypeFlags.Webpage) == CatalogNodeTypeFlags.Webpage)
            {
                //WEBPAGES INCLUDED IN RESULTS
                if (selectQuery.Length > 0) selectQuery.Append(" UNION ALL ");
                selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT node.CategoryId, node.CatalogNodeId, node.CatalogNodeTypeId, node.OrderBy, child.VisibilityId, child.Name, child.Summary, child.ThumbnailUrl, child.ThumbnailAltText, 0 AS Price");
                selectQuery.Append(" FROM ac_CatalogNodes node, ac_Webpages child");
                selectQuery.Append(" WHERE node.CatalogNodeTypeId = 2 AND node.CatalogNodeId = child.WebpageId AND " + categoryFilterSql);
                if (!string.IsNullOrEmpty(searchPhrase))
                {
                    if (titlesOnly)
                    {
                        selectQuery.Append(" AND child.Name LIKE @searchPhrase");
                    }
                    else
                    {
                        selectQuery.Append(" AND (child.Name LIKE @searchPhrase OR child.Summary LIKE @searchPhrase)");
                    }
                }
                if (publicOnly) selectQuery.Append(" AND child.VisibilityId = 0");
            }
            if ((catalogNodeTypes & CatalogNodeTypeFlags.Link) == CatalogNodeTypeFlags.Link)
            {
                //LINKS INCLUDED IN RESULTS
                if (selectQuery.Length > 0) selectQuery.Append(" UNION ALL ");
                selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT node.CategoryId, node.CatalogNodeId, node.CatalogNodeTypeId, node.OrderBy, child.VisibilityId, child.Name, child.Summary, child.ThumbnailUrl, child.ThumbnailAltText, 0 AS Price");
                selectQuery.Append(" FROM ac_CatalogNodes node, ac_Links child");
                selectQuery.Append(" WHERE node.CatalogNodeTypeId = 3 AND node.CatalogNodeId = child.LinkId AND " + categoryFilterSql);
                if (!string.IsNullOrEmpty(searchPhrase))
                {
                    if (titlesOnly)
                    {
                        selectQuery.Append(" AND child.Name LIKE @searchPhrase");
                    }
                    else
                    {
                        selectQuery.Append(" AND (child.Name LIKE @searchPhrase OR child.Summary LIKE @searchPhrase)");
                    }
                }
                if (publicOnly) selectQuery.Append(" AND child.VisibilityId = 0");
            }
            //ADD IN SORT EXPRESSION
            if (string.IsNullOrEmpty(sortExpression))
            {
                selectQuery.Append(" ORDER BY CatalogNodeTypeId,Name");
            }
            else
            {
                selectQuery.Append(" ORDER BY " + sortExpression);
            }
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@categoryId", System.Data.DbType.Int32, categoryId);
            if (!string.IsNullOrEmpty(searchPhrase)) database.AddInParameter(selectCommand, "@searchPhrase", System.Data.DbType.String, searchPhrase);

            //EXECUTE THE COMMAND
            CatalogNodeCollection CatalogNodeList = new CatalogNodeCollection();
            int thisIndex = 0;
            int rowCount = 0;
            List<String> keys = new List<string>();
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {                        
                        int parentCategoryId = dr.GetInt32(0);
                        int catalogNodeId = dr.GetInt32(1);
                        CatalogNodeType catalogNodeType = (CatalogNodeType)dr.GetByte(2); ;

                        // KEY TO CHECK AND REMOVE SAME CATALOG ITEM BEING ADDED MULTIPLE TIMES
                        String key = "CatalogNodeType:" + catalogNodeType.ToString() + "CatalogNodeId:" + catalogNodeId;
                        if (!keys.Contains(key))
                        {
                            keys.Add(key);
                        }
                        else
                        {
                            // ANOTHER RECORED ALREADY EXISTS, SKIP IT
                            continue;
                        }

                        short orderBy = dr.GetInt16(3);
                        CatalogVisibility visibility = (CatalogVisibility)dr.GetByte(4);
                        string name = dr.GetString(5);
                        string summary = NullableData.GetString(dr, 6);
                        string thumbnailUrl = NullableData.GetString(dr, 7);
                        string thumbnailAltText = NullableData.GetString(dr, 8);
                        if (catalogNodeType == CatalogNodeType.Product)
                        {
                            LSDecimal unitPrice = dr.GetDecimal(9);
                            CatalogNodeList.Add(new CatalogProductNode(parentCategoryId, catalogNodeId, catalogNodeType, orderBy, false, visibility, name, summary, thumbnailUrl, thumbnailAltText, unitPrice));
                        }
                        else
                        {
                            CatalogNodeList.Add(new CatalogNode(parentCategoryId, catalogNodeId, catalogNodeType, orderBy, false, visibility, name, summary, thumbnailUrl, thumbnailAltText));
                        }
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return CatalogNodeList;
        }

        /// <summary>
        /// Gets the number of catalog nodes mathcing the search criteria
        /// </summary>
        /// <param name="categoryId">The category in which to perform the search</param>
        /// <param name="searchPhrase">The search phrase to search for</param>
        /// <param name="titlesOnly">If true only node titles are searched</param>
        /// <param name="publicOnly">If true only public (published) nodes are searched</param>
        /// <param name="recursive">If true search is performend recursively on the sub-categories</param>
        /// <param name="catalogNodeTypes">The type of catalog nodes to include in the search</param>
        /// <returns>The number of catalog nodes matching the search criteria</returns>
        public static int SearchCount(int categoryId, string searchPhrase, bool titlesOnly, bool publicOnly, bool recursive, CatalogNodeTypeFlags catalogNodeTypes)
        {
            //MUST SPECIFY AT LEAST ONE CATALOG NODE TYPE
            if (catalogNodeTypes == CatalogNodeTypeFlags.None) throw new ArgumentException("You must specify at least one catalog node type.", "catalogNodeTypes");

            //DETERMINE THE CATEGORY FILTER
            string categoryFilterSql;
            if (recursive)
            {
                //this sql gets any category where the specified category is in the parent tree
                if (categoryId == 0)
                    categoryFilterSql = "((node.CategoryId = 0) OR (node.CategoryId IN (SELECT CategoryId FROM ac_CategoryParents WHERE ParentId = @categoryId)))";
                else
                    categoryFilterSql = "node.CategoryId IN (SELECT CategoryId FROM ac_CategoryParents WHERE ParentId = @categoryId)";
            }
            else
            {
                categoryFilterSql = "node.CategoryId = @categoryId";
            }

            //FIX THE SEARCH PHRASE FOR WILD CARD SUPPORT
            searchPhrase = StringHelper.FixSearchPattern(searchPhrase);

            //GENERATE QUERY
            StringBuilder selectQuery = new StringBuilder();
            if ((catalogNodeTypes & CatalogNodeTypeFlags.Category) == CatalogNodeTypeFlags.Category)
            {
                //CATEGORIES INCLUDED IN RESULTS
                selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) As TotalRecords");
                selectQuery.Append(" FROM ac_CatalogNodes node, ac_Categories child");
                selectQuery.Append(" WHERE node.CatalogNodeTypeId = 0 AND node.CatalogNodeId = child.CategoryId AND " + categoryFilterSql);
                if (!string.IsNullOrEmpty(searchPhrase))
                {
                    if (titlesOnly)
                    {
                        selectQuery.Append(" AND child.Name LIKE @searchPhrase");
                    }
                    else
                    {
                        selectQuery.Append(" AND (child.Name LIKE @searchPhrase OR child.Summary LIKE @searchPhrase)");
                    }
                }
                if (publicOnly) selectQuery.Append(" AND child.VisibilityId = 0");
            }
            if ((catalogNodeTypes & CatalogNodeTypeFlags.Product) == CatalogNodeTypeFlags.Product)
            {
                //PRODCUTS INCLUDED IN RESULTS
                if (selectQuery.Length > 0) selectQuery.Append(" UNION ALL ");
                selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) As TotalRecords");
                selectQuery.Append(" FROM ac_CatalogNodes node, ac_Products child");
                selectQuery.Append(" WHERE node.CatalogNodeTypeId = 1 AND node.CatalogNodeId = child.ProductId AND " + categoryFilterSql);
                if (!string.IsNullOrEmpty(searchPhrase))
                {
                    if (titlesOnly)
                    {
                        selectQuery.Append(" AND child.Name LIKE @searchPhrase");
                    }
                    else
                    {
                        selectQuery.Append(" AND (child.Name LIKE @searchPhrase OR child.Summary LIKE @searchPhrase)");
                    }
                }
                if (publicOnly) selectQuery.Append(" AND child.VisibilityId = 0");
            }
            if ((catalogNodeTypes & CatalogNodeTypeFlags.Webpage) == CatalogNodeTypeFlags.Webpage)
            {
                //WEBPAGES INCLUDED IN RESULTS
                if (selectQuery.Length > 0) selectQuery.Append(" UNION ALL ");
                selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) As TotalRecords");
                selectQuery.Append(" FROM ac_CatalogNodes node, ac_Webpages child");
                selectQuery.Append(" WHERE node.CatalogNodeTypeId = 2 AND node.CatalogNodeId = child.WebpageId AND " + categoryFilterSql);
                if (!string.IsNullOrEmpty(searchPhrase))
                {
                    if (titlesOnly)
                    {
                        selectQuery.Append(" AND child.Name LIKE @searchPhrase");
                    }
                    else
                    {
                        selectQuery.Append(" AND (child.Name LIKE @searchPhrase OR child.Summary LIKE @searchPhrase)");
                    }
                }
                if (publicOnly) selectQuery.Append(" AND child.Visibilityid = 0");
            }
            if ((catalogNodeTypes & CatalogNodeTypeFlags.Link) == CatalogNodeTypeFlags.Link)
            {
                //LINKS INCLUDED IN RESULTS
                if (selectQuery.Length > 0) selectQuery.Append(" UNION ALL ");
                selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) As TotalRecords");
                selectQuery.Append(" FROM ac_CatalogNodes node, ac_Links child");
                selectQuery.Append(" WHERE node.CatalogNodeTypeId = 3 AND node.CatalogNodeId = child.LinkId AND " + categoryFilterSql);
                if (!string.IsNullOrEmpty(searchPhrase))
                {
                    if (titlesOnly)
                    {
                        selectQuery.Append(" AND child.Name LIKE @searchPhrase");
                    }
                    else
                    {
                        selectQuery.Append(" AND (child.Name LIKE @searchPhrase OR child.Summary LIKE @searchPhrase)");
                    }
                }
                if (publicOnly) selectQuery.Append(" AND child.VisibilityId = 0");
            }
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@categoryId", System.Data.DbType.Int32, categoryId);
            if (!string.IsNullOrEmpty(searchPhrase)) database.AddInParameter(selectCommand, "@searchPhrase", System.Data.DbType.String, searchPhrase);

            //EXECUTE THE COMMAND
            int totalCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    totalCount += dr.GetInt32(0);
                }
                dr.Close();
            }
            return totalCount;
        }

        /// <summary>
        /// Loads the collection of catalog node items for given category id
        /// </summary>
        /// <param name="categoryId">The category Id for which to load the catalog nodes</param>
        /// <param name="publicOnly">If true on public (published) nodes are included</param>
        /// <returns>The collection of catalog node items for the given category</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CatalogNodeCollection LoadForCategory(int categoryId, bool publicOnly)
        {
            return LoadForCategory(categoryId, publicOnly, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads the collection of catalog node items for given category id
        /// </summary>
        /// <param name="categoryId">The category Id for which to load the catalog nodes</param>
        /// <param name="publicOnly">If true on public (published) nodes are included</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded products</param>
        /// <returns>The collection of catalog node items for the given category</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CatalogNodeCollection LoadForCategory(int categoryId, bool publicOnly, string sortExpression)
        {
            return LoadForCategory(categoryId, publicOnly, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads the collection of catalog node items for given category id
        /// </summary>
        /// <param name="categoryId">The category Id for which to load the catalog nodes</param>
        /// <param name="publicOnly">If true on public (published) nodes are included</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <returns>The collection of catalog node items for the given category</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CatalogNodeCollection LoadForCategory(int categoryId, bool publicOnly, int maximumRows, int startRowIndex)
        {
            return LoadForCategory(categoryId, publicOnly, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads the collection of catalog node items for given category id
        /// </summary>
        /// <param name="categoryId">The category Id for which to load the catalog nodes</param>
        /// <param name="publicOnly">If true on public (published) nodes are included</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded products</param>
        /// <returns>The collection of catalog node items for given category id</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CatalogNodeCollection LoadForCategory(int categoryId, bool publicOnly, int maximumRows, int startRowIndex, string sortExpression)
        {
            CatalogNodeCollection catalogNodes = new CatalogNodeCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT node.CatalogNodeId, node.CatalogNodeTypeId, node.OrderBy, child.VisibilityId, child.Name, child.Summary, child.ThumbnailUrl, child.ThumbnailAltText, 0 AS Price");
            selectQuery.Append(" FROM ac_CatalogNodes node, ac_Categories child");
            selectQuery.Append(" WHERE node.CatalogNodeId = child.CategoryId AND node.CatalogNodeTypeId = 0 AND node.CategoryId = @categoryId");
            if (publicOnly) selectQuery.Append(" AND child.VisibilityId = " + ((int)CatalogVisibility.Public).ToString());
            selectQuery.Append(" UNION ALL");
            selectQuery.Append(" SELECT node.CatalogNodeId, node.CatalogNodeTypeId, node.OrderBy, child.VisibilityId, child.Name, child.Summary, child.ThumbnailUrl, child.ThumbnailAltText, child.Price");
            selectQuery.Append(" FROM ac_CatalogNodes node, ac_Products child");
            selectQuery.Append(" WHERE node.CatalogNodeId = child.ProductId AND node.CatalogNodeTypeId = 1 AND node.CategoryId = @categoryId");
            if (publicOnly) selectQuery.Append(" AND child.VisibilityId = " + ((int)CatalogVisibility.Public).ToString());
            selectQuery.Append(" UNION ALL");
            selectQuery.Append(" SELECT node.CatalogNodeId, node.CatalogNodeTypeId, node.OrderBy, child.VisibilityId, child.Name, child.Summary, child.ThumbnailUrl, child.ThumbnailAltText, 0 AS Price");
            selectQuery.Append(" FROM ac_CatalogNodes node, ac_Webpages child");
            selectQuery.Append(" WHERE node.CatalogNodeId = child.WebpageId AND node.CatalogNodeTypeId = 2 AND node.CategoryId = @categoryId");
            if (publicOnly) selectQuery.Append(" AND child.VisibilityId = " + ((int)CatalogVisibility.Public).ToString());
            selectQuery.Append(" UNION ALL");
            selectQuery.Append(" SELECT node.CatalogNodeId, node.CatalogNodeTypeId, node.OrderBy, child.VisibilityId, child.Name, child.Summary, child.ThumbnailUrl, child.ThumbnailAltText, 0 AS Price");
            selectQuery.Append(" FROM ac_CatalogNodes node, ac_Links child");
            selectQuery.Append(" WHERE node.CatalogNodeId = child.LinkId AND node.CatalogNodeTypeId = 3 AND node.CategoryId = @categoryId");
            if (publicOnly) selectQuery.Append(" AND child.VisibilityId = " + ((int)CatalogVisibility.Public).ToString());
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "OrderBy";
            selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@categoryId", System.Data.DbType.Int32, categoryId);
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
                        int catalogNodeId = dr.GetInt32(0);
                        CatalogNodeType catalogNodeType = (CatalogNodeType)dr.GetByte(1); ;
                        short orderBy = dr.GetInt16(2);
                        CatalogVisibility visibility = (CatalogVisibility)dr.GetByte(3);
                        string name = dr.GetString(4);
                        string summary = NullableData.GetString(dr, 5);
                        string thumbnailUrl = NullableData.GetString(dr, 6);
                        string thumbnailAltText = NullableData.GetString(dr, 7);
                        if (catalogNodeType == CatalogNodeType.Product)
                        {
                            LSDecimal unitPrice = dr.GetDecimal(8);
                            catalogNodes.Add(new CatalogProductNode(categoryId, catalogNodeId, catalogNodeType, orderBy, false, visibility, name, summary, thumbnailUrl, thumbnailAltText, unitPrice));
                        }
                        else
                        {
                            catalogNodes.Add(new CatalogNode(categoryId, catalogNodeId, catalogNodeType, orderBy, false, visibility, name, summary, thumbnailUrl, thumbnailAltText));
                        }
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return catalogNodes;
        }

        /// <summary>
        /// Counts the number of catalog nodes in a category
        /// </summary>
        /// <param name="categoryId">Id of the category to count for</param>
        /// <param name="publicOnly">If true only public (published) items are included in the count</param>
        /// <returns>The number of catalog nodes in a category</returns>
        public static int CountForCategory(int categoryId, bool publicOnly)
        {
            //GENERATE THE SQL
            StringBuilder selectQuery = new StringBuilder();
            if (publicOnly)
            {
                selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords");
                selectQuery.Append(" FROM ac_CatalogNodes node, ac_Categories child");
                selectQuery.Append(" WHERE node.CatalogNodeId = child.CategoryId AND node.CatalogNodeTypeId = 0 AND node.CategoryId = @categoryId");
                selectQuery.Append(" AND child.VisibilityId = " + ((int)CatalogVisibility.Public).ToString());
                selectQuery.Append(" UNION ALL");
                selectQuery.Append(" SELECT COUNT(1) AS TotalRecords");
                selectQuery.Append(" FROM ac_CatalogNodes node, ac_Products child");
                selectQuery.Append(" WHERE node.CatalogNodeId = child.ProductId AND node.CatalogNodeTypeId = 1 AND node.CategoryId = @categoryId");
                selectQuery.Append(" AND child.VisibilityId = " + ((int)CatalogVisibility.Public).ToString());
                selectQuery.Append(" UNION ALL");
                selectQuery.Append(" SELECT COUNT(1) AS TotalRecords");
                selectQuery.Append(" FROM ac_CatalogNodes node, ac_Webpages child");
                selectQuery.Append(" WHERE node.CatalogNodeId = child.WebpageId AND node.CatalogNodeTypeId = 2 AND node.CategoryId = @categoryId");
                selectQuery.Append(" AND child.VisibilityId = " + ((int)CatalogVisibility.Public).ToString());
                selectQuery.Append(" UNION ALL");
                selectQuery.Append(" SELECT COUNT(1) AS TotalRecords");
                selectQuery.Append(" FROM ac_CatalogNodes node, ac_Links child");
                selectQuery.Append(" WHERE node.CatalogNodeId = child.LinkId AND node.CatalogNodeTypeId = 3 AND node.CategoryId = @categoryId");
                selectQuery.Append(" AND child.VisibilityId = " + ((int)CatalogVisibility.Public).ToString());
            }
            else
            {
                //SIMPLE COUNT, NO NEED TO JOIN TABLES
                selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM ac_CatalogNodes WHERE CategoryId = @categoryId");
            }

            //CREATE THE COMMAND
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@categoryId", System.Data.DbType.Int32, categoryId);

            //EXECUTE THE COMMAND
            int totalCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    totalCount += dr.GetInt32(0);
                }
                dr.Close();
            }
            return totalCount;
        }

        /// <summary>
        /// Copies a catalog node.
        /// </summary>
        /// <param name="nodeId">The id of the catalog node to copy.</param>
        /// <param name="nodeType">The type of the catalog node to copy.</param>
        /// <param name="categoryId">The category where the copy should be saved.</param>
        /// <returns>The id of the copied node.</returns>
        public static int Copy(int nodeId, CatalogNodeType nodeType, int categoryId)
        {
            switch (nodeType)
            {
                case CatalogNodeType.Product:
                    Product product = ProductDataSource.Load(nodeId);
                    if (product != null)
                    {
                        // THE PRODUCT NAME SHOULD NOT EXCEED THE MAX 255 CHARS
                        String newName = "Copy of " + product.Name;
                        if (newName.Length > 255)
                        {
                            newName = newName.Substring(0, 252) + "...";
                        }                        
                        product.SaveCopy(categoryId, newName);
                        return product.ProductId;
                    }
                    break;
                case CatalogNodeType.Webpage:
                    Webpage webpage = Webpage.Copy(nodeId);
                    if (webpage != null)
                    {
                        // THE NAME SHOULD NOT EXCEED THE MAX 100 CHARS
                        String newName = "Copy of " + webpage.Name;
                        if (newName.Length > 100)
                        {
                            newName = newName.Substring(0, 97) + "...";
                        }
                        webpage.Name = newName;
                        webpage.Categories.Add(categoryId);
                        webpage.Save();
                        return webpage.WebpageId;
                    }
                    break;
                case CatalogNodeType.Link:
                    Link link = Link.Copy(nodeId);
                    if (link != null)
                    {
                        // THE NAME SHOULD NOT EXCEED THE MAX 100 CHARS
                        String newName = "Copy of " + link.Name;
                        if (newName.Length > 100)
                        {
                            newName = newName.Substring(0, 97) + "...";
                        }
                        link.Name = newName;
                        link.Categories.Add(categoryId);
                        link.Save();
                        return link.LinkId;
                    }
                    break;
            }
            return 0;
        }
    }
}
