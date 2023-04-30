//-----------------------------------------------------------------------
// <copyright file="KeywordSearchHelper.cs" company="Able Solutions Corporation">
//     Copyright (c) 2009 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MakerShop.Search
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Text;
    using System.Web.Caching;
    using MakerShop.Common;
    using MakerShop.Data;
    using MakerShop.Stores;
    using MakerShop.Utility;

    /// <summary>
    /// Class for maintaining with Full Text Search catalogs and indexes
    /// </summary>
    public static class KeywordSearchHelper
    {
        /// <summary>
        /// The name of the FTS catalog used by MakerShop
        /// </summary>
        private static string FTSCatalogName = "ac_SearchCatalog";

        /// <summary>
        /// Contains a list of table names that have fulltext indexes managed by MakerShop
        /// </summary>
        private static List<string> FTSIndexedTables = new List<string>();

        /// <summary>
        /// Contains a list of columns (indexed by tablename) that are part of fulltext indexes managed
        /// by MakerShop
        /// </summary>
        private static Dictionary<string, List<string>> FTSIndexedColumns = new Dictionary<string, List<string>>();

        /// <summary>
        /// Initializes static members of the KeywordSearchHelper class.
        /// </summary>
        static KeywordSearchHelper()
        {
            // TEMP VARIABLES FOR CONSTRUCTING ARRAYS
            string columns;
            List<string> columnList;

            // ADD AC_PRODUCTS INTO THE LIST OF INDEXED TABLES
            FTSIndexedTables.Add("ac_Products");
            columns = "Name,SKU,ModelNumber,Summary,Description,ExtendedDescription,SearchKeywords";
            columnList = new List<string>(columns.Split(",".ToCharArray()));
            FTSIndexedColumns.Add("ac_Products", columnList);

            // ADD AC_ADDRESSES TO LIST OF INDEXED TABLES
            FTSIndexedTables.Add("ac_Addresses");
            columns = "FirstName,LastName,Company,Address1";
            columnList = new List<string>(columns.Split(",".ToCharArray()));
            FTSIndexedColumns.Add("ac_Addresses", columnList);
        }

        /// <summary>
        /// Determines if the fulltext catalog exists
        /// </summary>
        /// <returns>True if the catalog exists, false otherwise.</returns>
        public static bool CatalogExists()
        {
            try
            {
                Database database = Token.Instance.Database;
                string sql ="SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  SELECT COUNT(1) AS NumRecords FROM sys.fulltext_catalogs WHERE [name] = '" + FTSCatalogName + "'";
                DbCommand command = database.GetSqlStringCommand(sql);
                return AlwaysConvert.ToInt(database.ExecuteScalar(command)) == 1;
            }
            catch
            {
                // WE DO NOT WANT TO THROW ERRORS FOR SQL2000 SERVER
            }
            return false;
        }

        /// <summary>
        /// Determines if the fulltext index(es) exist
        /// </summary>
        /// <returns>True if fulltext indexes are found in the catalog</returns>
        public static bool IndexesExist()
        {
            List<string> indexedTables = GetIndexedTables();
            foreach (string expectedIndex in FTSIndexedTables)
            {
                if (!indexedTables.Contains(expectedIndex)) return false;
            }
            return true;
        }

        /// <summary>
        /// Gets a list of table names that have fulltext indexes in the search catalog
        /// </summary>
        /// <returns>A list of table names that have fulltext indexes in the search catalog</returns>
        private static List<string> GetIndexedTables()
        {
            List<string> indexedTables = new List<string>();
            try
            {
                string sql ="SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  SELECT object_name(object_id) AS IndexTable";
                sql += " FROM sys.fulltext_indexes FTI, sys.fulltext_catalogs FTC";
                sql += " WHERE FTI.fulltext_catalog_id = FTC.fulltext_catalog_id AND FTC.[name] = '" + FTSCatalogName + "'";
                Database database = Token.Instance.Database;
                DbCommand command = database.GetSqlStringCommand(sql);
                using (IDataReader dr = database.ExecuteReader(command))
                {
                    while (dr.Read())
                    {
                        string tableName = AlwaysConvert.ToString(dr.GetString(0));
                        if (!string.IsNullOrEmpty(tableName))
                            indexedTables.Add(tableName);
                    }
                    dr.Close();
                }
            }
            catch
            {
                // WE DO NOT WANT TO THROW ERRORS FOR SQL2000 SERVER
            }
            return indexedTables;
        }

        /// <summary>
        /// Creates the FTS catalog
        /// </summary>
        private static void CreateCatalog()
        {
            Database database = Token.Instance.Database;
            try
            {
                string sql = "CREATE FULLTEXT CATALOG " + FTSCatalogName;
                DbCommand command = database.GetSqlStringCommand(sql);
                database.ExecuteNonQuery(command);
            }
            catch (SqlException se)
            {
                Logger.Warn("Could not create fulltext catalog " + FTSCatalogName, se);
            }
        }

        /// <summary>
        /// Create an index in the FTS catalog
        /// </summary>
        /// <param name="tableName">Name of the table to create the index for</param>
        private static void CreateIndex(string tableName)
        {
            // OBTAIN THE CORRECT SQL DEPENDING ON WHICH INDEX TO CREATE
            Database database = Token.Instance.Database;
            string sql;
            switch (tableName)
            {
                case "ac_Products":
                case "ac_Addresses":
                    string columns = string.Join(",", FTSIndexedColumns[tableName].ToArray());
                    sql = "CREATE FULLTEXT INDEX ON " + tableName + "(" + columns + ") KEY INDEX " + tableName + "_PK ON " + FTSCatalogName;
                    break;
                default:
                    throw new ArgumentException("The specified tableName is not valid.  Valid values are: ac_Products", "tableName");
            }

            // ATTEMPT TO CREATE THE FULLTEXT INDEX
            try
            {
                DbCommand command = database.GetSqlStringCommand(sql);
                database.ExecuteNonQuery(command);
            }
            catch (SqlException se)
            {
                Logger.Warn("Could not create fulltext index on " + tableName, se);
            }
        }

        /// <summary>
        /// Ensures that the expected fulltext indexes are present in the catalog
        /// </summary>
        /// <returns>True if the indexes are available; false otherwise.</returns>
        public static bool EnsureIndexes()
        {
            List<string> indexedTables = GetIndexedTables();
            foreach (string expectedIndex in FTSIndexedTables)
            {
                if (!indexedTables.Contains(expectedIndex))
                    CreateIndex(expectedIndex);
            }
            return KeywordSearchHelper.IndexesExist();
        }

        /// <summary>
        /// Ensures that full text searching is enabled and the catalog exists
        /// </summary>
        /// <returns>True if the catalog exists, false otherwise.</returns>
        public static bool EnsureCatalog()
        {
            if (!KeywordSearchHelper.IsFullTextSearchEnabled())
            {
                KeywordSearchHelper.EnableFullTextSearch();
            }
            if (!KeywordSearchHelper.CatalogExists())
            {
                KeywordSearchHelper.CreateCatalog();
            }
            return KeywordSearchHelper.CatalogExists();
        }

        /// <summary>
        /// Remove indexes from the search catalog
        /// </summary>
        private static void RemoveIndexes()
        {
            List<string> indexedTables = GetIndexedTables();
            Database database = Token.Instance.Database;
            foreach (string tableName in indexedTables)
            {
                try
                {
                    DbCommand command = database.GetSqlStringCommand("DROP FULLTEXT INDEX ON " + tableName);
                    database.ExecuteNonQuery(command);
                }
                catch (SqlException se)
                {
                    Logger.Warn("Could not remove fulltext index on " + tableName, se);
                }
            }
        }

        /// <summary>
        /// Removes the catalog
        /// </summary>
        public static void RemoveCatalog()
        {
            if (KeywordSearchHelper.CatalogExists())
            {
                // ATTEMPT TO REMOVE INDEXES IN THE CATALOG
                RemoveIndexes();

                // ATTEMPT TO REMOVE THE CATALOG, FAILURE IS NOT FATAL
                try
                {
                    Database database = Token.Instance.Database;
                    DbCommand command = database.GetSqlStringCommand("DROP FULLTEXT CATALOG " + FTSCatalogName);
                    database.ExecuteNonQuery(command);
                }
                catch (SqlException se)
                {
                    Logger.Warn("Could not remove fulltext catalog " + FTSCatalogName, se);
                }
            }
        }

        /// <summary>
        /// Determines whether fulltext search is installed on the server
        /// </summary>
        /// <returns>True if fulltext search is installed; false otherwise.</returns>
        public static bool IsFullTextSearchInstalled()
        {
            return IsFullTextSearchInstalled(true);
        }

        /// <summary>
        /// Determines whether fulltext search is installed on the server
        /// </summary>
        /// <param name="logError">If true, exceptions thrown by the database are logged to the MakerShop error log.</param>
        /// <returns>True if fulltext search is installed; false otherwise.</returns>
        public static bool IsFullTextSearchInstalled(bool logError)
        {
            // QUERY DATABASE TO SEE WHETHER FULLTEXT IS INSTALLED
            Database database = Token.Instance.Database;
            string sql ="SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  SELECT ServerProperty('IsFullTextInstalled')";
            try
            {
                DbCommand command = database.GetSqlStringCommand(sql);
                return AlwaysConvert.ToInt(database.ExecuteScalar(command)) == 1;
            }
            catch (SqlException se)
            {
                if (logError) Logger.Warn("Could not detect if FTS is installed.", se);
                return false;
            }
        }

        /// <summary>
        /// Determines whether full text search is enabled on the current database
        /// </summary>
        /// <returns>True if fulltext search is enabled on the current database; false otherwise.</returns>
        public static bool IsFullTextSearchEnabled()
        {
            return IsFullTextSearchEnabled(true);
        }

        /// <summary>
        /// Determines whether full text search is enabled on the current database
        /// </summary>
        /// <param name="logError">If true, exceptions thrown by the database are logged to the MakerShop error log.</param>
        /// <returns>True if fulltext search is enabled on the current database; false otherwise.</returns>
        public static bool IsFullTextSearchEnabled(bool logError)
        {
            try
            {
                // QUERY DATABASE TO SEE WHETHER FULLTEXT IS ENABLED
                Database database = Token.Instance.Database;
                string databaseName = DatabaseHelper.GetCurrentDatabaseName();
                string sql ="SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  SELECT DatabaseProperty('" + databaseName + "', 'IsFullTextEnabled')";
                DbCommand command = database.GetSqlStringCommand(sql);
                return AlwaysConvert.ToInt(database.ExecuteScalar(command)) == 1;
            }
            catch (SqlException se)
            {
                if (logError) Logger.Warn("Could not detect if FTS is enabled." , se);
                return false;
            }
        }

        /// <summary>
        /// Attempts to enable full text search on the current database
        /// </summary>
        public static void EnableFullTextSearch()
        {
            EnableFullTextSearch(true);
        }

        /// <summary>
        /// Attempts to enable full text search on the current database
        /// </summary>
        /// <param name="logError">If true, exceptions thrown by the database are logged to the MakerShop error log.</param>
        public static void EnableFullTextSearch(bool logError)
        {
            try
            {
                Database database = Token.Instance.Database;
                string sql = "sp_fulltext_database 'enable'";
                DbCommand command = database.GetSqlStringCommand(sql);
                database.ExecuteNonQuery(command);
            }
            catch (SqlException se)
            {
                if (logError) Logger.Warn("Could not enable FTS.", se);
            }
        }

        /// <summary>
        /// Determines if a keyword search pattern is valid for use with fulltext search
        /// </summary>
        /// <param name="keyword">The search keyword</param>
        /// <returns>True if the keyword search pattern is valid for FTS, false otherwise</returns>
        /// <remarks>FTS cannot handle searches with wildcards at the beginning of the search expression
        /// like "*book".  It can only handle whole words "book" or prefix matches like "book*".</remarks>
        internal static bool IsSearchPatternSupportedForFts(string keyword)
        {
            // REJECT EMPTY STRINGS (SHOULD NOT BE FILTERING ON KEYWORD ANYWAY)
            if (string.IsNullOrEmpty(keyword)) return false;

            // IF THE KEYWORD USES THE ? WILDCARD WE CANNOT USE FTS
            if (keyword.Contains("?")) return false;

            // IF THERE IS NO * WILDCARD, THEN THIS KEYWORD IS OK FOR USE WITH FTS
            if (!keyword.Contains("*")) return true;

            // IF THERE IS A * WILDCARD, IT CAN ONLY APPEAR AT THE END OF A WORD
            string[] words = keyword.Split(" ".ToCharArray());
            foreach (string word in words)
            {
                int index = word.IndexOf("*");

                // FOUND *, IF NOT AT THE END OF WORD THIS KEYWORD(S) CANNOT BE USED WITH FTS
                if (index < word.Length - 1) return false;
            }

            // THE * WILDCARD MUST HAVE ONLY APPEARED AT THE END OF KEYWORD(S) SO WE CAN USE FTS
            return true;
        }

        /// <summary>
        /// Gets a SQL filter for the given table, columns, and keyword
        /// </summary>
        /// <param name="tableName">Name of the table being searched</param>
        /// <param name="columnPrefix">Table name or alias that must be applied to the column(s)</param>
        /// <param name="columnList">The column(s) being searched, multiple columns should be comma delimited with no spaces.</param>
        /// <param name="parameterName">The name to use for the SQL parameter.</param>
        /// <param name="searchPattern">Pattern to search for - this value may be normalized by the procedure and must be passed by reference</param>
        /// <returns>A clause suitable for use in a SQL where statement</returns>
        /// <remarks>The returned SQL string will use a parameter - specified by parameterName.  This must be populated when executing the query.</remarks>
        [Obsolete("If substring matching should be enforced, it should be done from the front end.")]
        internal static string PrepareSqlFilterWithForcedSubstring(string tableName, string columnPrefix, string columnList, string parameterName, ref string searchPattern)
        {
            return PrepareSqlFilter(tableName, columnPrefix, columnList, parameterName, true, ref searchPattern);
        }

        /// <summary>
        /// Gets a SQL filter for the given table, columns, and keyword
        /// </summary>
        /// <param name="tableName">Name of the table being searched</param>
        /// <param name="columnPrefix">Table name or alias that must be applied to the column(s)</param>
        /// <param name="columnList">The column(s) being searched, multiple columns should be comma delimited with no spaces.</param>
        /// <param name="parameterName">The name to use for the SQL parameter.</param>
        /// <param name="searchPattern">Pattern to search for - this value may be normalized by the procedure and must be passed by reference</param>
        /// <returns>A clause suitable for use in a SQL where statement</returns>
        /// <remarks>The returned SQL string will use a parameter - specified by parameterName.  This must be populated when executing the query.</remarks>
        internal static string PrepareSqlFilter(string tableName, string columnPrefix, string columnList, string parameterName, ref string searchPattern)
        {
            return PrepareSqlFilter(tableName, columnPrefix, columnList, parameterName, false, ref searchPattern);
        }

        /// <summary>
        /// Gets a SQL filter for the given table, columns, and keyword
        /// </summary>
        /// <param name="tableName">Name of the table being searched</param>
        /// <param name="columnPrefix">Table name or alias that must be applied to the column(s)</param>
        /// <param name="columnList">The column(s) being searched, multiple columns should be comma delimited with no spaces.</param>
        /// <param name="parameterName">The name to use for the SQL parameter.</param>
        /// <param name="defaultToSubstring">If true, the search pattern will be converted to a substring match unless otherwise specified.  When false, the search pattern is used as is.  This applies only to non-FTS searches.</param>
        /// <param name="searchPattern">Pattern to search for - this value may be normalized by the procedure and must be passed by reference</param>
        /// <returns>A clause suitable for use in a SQL where statement</returns>
        /// <remarks>The returned SQL string will use a parameter - specified by parameterName.  This must be populated when executing the query.</remarks>
        private static string PrepareSqlFilter(string tableName, string columnPrefix, string columnList, string parameterName, bool defaultToSubstring, ref string searchPattern)
        {
            // VALIDATE INPUTS
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName", "The table name must be specified.");
            }
            if (string.IsNullOrEmpty(columnList))
            {
                throw new ArgumentException("At least one column must be specified.", "columns");
            }
            if (string.IsNullOrEmpty(parameterName))
            {
                throw new ArgumentNullException("parameterName", "The parameter name must be specified.");
            }
            if (string.IsNullOrEmpty(searchPattern))
            {
                throw new ArgumentNullException("searchPattern", "The search pattern must be specified.");
            }

            // MAKE SURE TABLE PREFIX HAS A TERMINAL .
            if (!string.IsNullOrEmpty(columnPrefix) && !columnPrefix.EndsWith(".")) columnPrefix += ".";

            // MAKE SURE PARAMETER NAME HAS APPROPRIATE PREFIX
            if (!parameterName.StartsWith("@")) parameterName = "@" + parameterName;

            // PARSE THE COLUMN LIST
            List<string> columns = new List<string>(columnList.Split(",".ToCharArray()));

            // DETERMINE THE SEARCH TYPE
            if (Store.GetCachedSettings().FullTextSearch
                && KeywordSearchHelper.IsSearchPatternSupportedForFts(searchPattern)
                && ColumnsIndexed(tableName, columns))
            {
                // CREATE A SEARCH FILTER USING FTS
                return PrepareFtsSqlFilter(tableName, columnPrefix, columns, parameterName, ref searchPattern);
            }
            else
            {
                // CREATE A SEARCH FILTER WITHOUT USING FTS
                return PrepareNonFtsSqlFilter(columnPrefix, columns, parameterName, defaultToSubstring, ref searchPattern);
            }
        }

        /// <summary>
        /// Determines if the given table and columns are present in the FTS index
        /// </summary>
        /// <param name="tableName">Name of the table to check</param>
        /// <param name="columns">Columns to verify for presence in the FTS index</param>
        /// <returns>True if table and all specified columns are in the FTS index, 
        /// false if the table is not indexed or one or more columns are not available.</returns>
        private static bool ColumnsIndexed(string tableName, List<string> columns)
        {
            if (!FTSIndexedTables.Contains(tableName)) return false;
            List<string> allColumns = FTSIndexedColumns[tableName];
            foreach (string columnName in columns)
            {
                if (!allColumns.Contains(columnName)) return false;
            }
            return true;
        }

        /// <summary>
        /// Creates a clause for use in a fulltext search
        /// </summary>
        /// <param name="tableName">Name of the table being searched</param>
        /// <param name="columnPrefix">Table name or alias that must be applied to the column(s)</param>
        /// <param name="columns">List of columns being searched</param>
        /// <param name="searchPattern">Search keyword to look for</param>
        /// <returns>A clause suitable for use in a SQL where statement</returns>
        private static string PrepareFtsSqlFilter(string tableName, string columnPrefix, List<string> columns, string parameterName, ref string searchPattern)
        {
            string columnList = PrepareFtsColumnList(tableName, columnPrefix, columns);
            if (searchPattern.Length > 2 && searchPattern.StartsWith("\"") && searchPattern.EndsWith("\""))
            {
                searchPattern = "\"" + searchPattern.Substring(1, searchPattern.Length - 2).Replace("\"", "\"\"") + "\"";
                return " AND CONTAINS(" + columnList + ", " + parameterName + ")";
            }
            else
            {
                return " AND FREETEXT(" + columnList + ", " + parameterName + ")";
            }
        }

        /// <summary>
        /// Creates a list of columns for an FTS clause
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <param name="columnPrefix">Table name or alias that must be applied to the column(s)</param>
        /// <param name="columns">List of columns to be searched</param>
        /// <returns>A list of columns suitable for use in the FTS clause</returns>
        private static string PrepareFtsColumnList(string tableName, string columnPrefix, List<string> columns)
        {
            if (AllIndexedColumnsSpecified(tableName, columns)) return columnPrefix + "*";
            if (columns.Count == 1) return columnPrefix + columns[0];
            return "(" + columnPrefix + string.Join("," + columnPrefix, columns.ToArray()) + ")";
        }

        /// <summary>
        /// Indicates whether all columns in the FTS index have been specified
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <param name="columns">Columns that are being searched</param>
        /// <returns>True if all columns in the index are being searched; false otherwise.</returns>
        private static bool AllIndexedColumnsSpecified(string tableName, List<string> columns)
        {
            List<string> allColumns = FTSIndexedColumns[tableName];
            foreach (string columnName in allColumns)
            {
                if (!columns.Contains(columnName)) return false;
            }
            return true;
        }

        /// <summary>
        /// Creates a SQL filter without using full text search
        /// </summary>
        /// <param name="columnPrefix">Table name or alias that must be applied to the column(s)</param>
        /// <param name="columns">Columns to include in the search</param>
        /// <param name="parameterName">Name of the parameter to use for the search</param>
        /// <param name="defaultToSubstring">If true, the search pattern will be converted to a substring match unless otherwise specified.  When false, the search pattern is used as is.  This applies only to non-FTS searches.</param>
        /// <param name="searchPattern">Pattern to search for - this value may be normalized by the procedure and must be passed by reference</param>
        /// <returns>A filter for use in a SQL WHERE clause without using full text search</returns>
        private static string PrepareNonFtsSqlFilter(string columnPrefix, List<string> columns, string parameterName, bool defaultToSubstring, ref string searchPattern)
        {
            // LEGACY CODE RELIES ON A FORCED SUBSTRING MATCH PATTERN
            if (defaultToSubstring && searchPattern.IndexOfAny("*?".ToCharArray()) < 0)
            {
                searchPattern = "*" + searchPattern + "*";
            }

            // DETERMINE WHETHER TO USE LIKE OR = OPERATOR
            string sqlOperator;
            string escapeClause = string.Empty;
            if (searchPattern.IndexOfAny("*?".ToCharArray()) > -1)
            {
                // WILDCARDS IN PATTERN, PERFORM A LIKE MATCH
                sqlOperator = " LIKE ";
                
                // WE SHOULD RESPECT A NORMALIZED WILDCARD SYNTAX, * AND ? ONLY
                if (searchPattern.IndexOfAny("%_".ToCharArray()) > -1)
                {
                    // ESCAPE MSSQL STYLE PATTERNS
                    escapeClause = " ESCAPE '\\'";
                    searchPattern = searchPattern.Replace("\\", "\\\\");
                    searchPattern = searchPattern.Replace("%", "\\%");
                    searchPattern = searchPattern.Replace("_", "\\_");
                }

                // CONVERT WILDCARDS TO MSSQL INDICATORS
                searchPattern = searchPattern.Replace("*", "%");
                searchPattern = searchPattern.Replace("?", "_");
            }
            else
            {
                // THERE ARE NO WILDCARDS IN PATTERN, PERFORM AN EQUALS MATCH
                sqlOperator = " = ";
            }

            // BUILD THE FILTER
            StringBuilder sqlFilter = new StringBuilder();
            for (int i = 0; i < columns.Count; i++)
            {
                string columnName = columns[i];
                if (i > 0) sqlFilter.Append(" OR ");
                sqlFilter.Append(columnPrefix + columnName + sqlOperator + parameterName + escapeClause);
            }

            // USE PARENTHESIS FOR MORE THAN ONE COLUMN
            if (columns.Count > 1)
            {
                sqlFilter.Insert(0, "(");
                sqlFilter.Append(")");
            }

            // RETURN THE SQL FILTER
            return " AND " + sqlFilter.ToString();
        }
    }
}