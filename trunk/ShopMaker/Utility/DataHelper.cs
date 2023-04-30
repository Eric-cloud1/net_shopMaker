using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MakerShop.Data;
using System.Data.Common;
using MakerShop.Common;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Caching;
using System.Web.Configuration;
using System.IO;
using System.Web;

namespace MakerShop.Utility
{
    /// <summary>
    /// Utility class for database operations
    /// </summary>
    public static class DataHelper
    {

        /// <summary>
        /// Pivots data in table based on the parameters as described.
        /// Normally the Key Column would be a date, pivotColumn would hold the data of the column names to appear as header.
        /// pivotData is the data to be sumed
        /// </summary>
        /// <param name="dt">The data</param>
        /// <param name="keyColumn">The column that is unique for the row</param>
        /// <param name="pivotColumn">The column holding the pivot column names</param>
        /// <param name="pivotData">The data to pivot</param>
        /// <returns></returns>
        public static DataTable Pivot(DataTable dt, string keyColumn, string pivotColumn, List<string> pivotData)
        { 
            List<string> keyColumns = new List<string>();
            keyColumns.Add(keyColumn);
            return Pivot(dt, keyColumns, pivotColumn, pivotData);
        }

        /// <summary>
        /// Pivots data in table based on the parameters as described.
        /// Normally the Key Column would be a date, pivotColumn would hold the data of the column names to appear as header.
        /// pivotData is the data to be sumed
        /// </summary>
        /// <param name="dt">The data</param>
        /// <param name="keyColumns">The columns that are unique for the row</param>
        /// <param name="pivotColumn">The column holding the pivot column names</param>
        /// <param name="pivotData">The data to pivot</param>
        /// <returns></returns>
        public static DataTable Pivot(DataTable dt, List<string> keyColumns, string pivotColumn, List<string> pivotData)
        {
            DataTable display = new DataTable();
            foreach (DataColumn dc in dt.Columns)
            {
                if (dc.ColumnName.ToLower() != pivotColumn.ToLower())
                {
                    display.Columns.Add(dc.ColumnName, dc.DataType);
                    if (dc.DataType.GetType() != typeof(DateTime).GetType())
                        display.Columns[dc.ColumnName].DefaultValue = 0;
                }
            }


            foreach (DataRow dr in dt.Rows)
            {
                foreach (string key in pivotData)
                {
                    if (!display.Columns.Contains(dr[pivotColumn] + " " + key))
                    {
                        display.Columns.Add(dr[pivotColumn] + " " + key, dr[key].GetType());
                        display.Columns[dr[pivotColumn] + " " + key].DefaultValue = 0;
                    }
                }
            }
            DataRow newRow;
            bool add;
            foreach (DataRow dr in dt.Rows)
            {
                add = false;
                string select = string.Empty;
                foreach (string keyColumn in keyColumns)
                {
                    if (!string.IsNullOrEmpty(select))
                        select += " AND ";
                    select += keyColumn + "='" + dr[keyColumn] + "'";
                }
                DataRow[] x = display.Select(select);
                if (x == null)
                    continue;
                if (x.Length == 0)
                {
                    add = true;
                    newRow = display.NewRow();
                    foreach (DataColumn dc in display.Columns)
                        if (dt.Columns.Contains(dc.ColumnName))
                            newRow[dc.ColumnName] = dr[dc.ColumnName];
                }
                else
                {
                    newRow = x[0];
                    foreach (DataColumn dc in display.Columns)
                    {
                        if (!keyColumns.Contains(dc.ColumnName) )
                        {
                            if (dt.Columns.Contains(dc.ColumnName))
                            {
                                if (!dc.ColumnName.Contains("%"))
                                {
                                    if (dc.DataType.FullName == typeof(int).FullName)
                                        newRow[dc.ColumnName] = (int)dr[dc.ColumnName] + (int)newRow[dc.ColumnName];
                                    else if (dc.DataType.FullName == typeof(decimal).FullName)
                                        newRow[dc.ColumnName] = (decimal)dr[dc.ColumnName] + (decimal)newRow[dc.ColumnName];
                                }
                                else
                                {
                                    if (dc.DataType.FullName == typeof(int).FullName)
                                        newRow[dc.ColumnName] = ((int)dr[dc.ColumnName] + (int)newRow[dc.ColumnName])/2;
                                    else if (dc.DataType.FullName == typeof(decimal).FullName)
                                        newRow[dc.ColumnName] = ((decimal)dr[dc.ColumnName] + (decimal)newRow[dc.ColumnName])/2;
                                }
                            }
                        }
                    }
                }

                foreach (string key in pivotData)
                {
                    if (dr[key].GetType() == typeof(int))
                        newRow[dr[pivotColumn] + " " + key] = (int)newRow[dr[pivotColumn] + " " + key] + (int)dr[key];
                    else if (dr[key].GetType() == typeof(decimal))
                        newRow[dr[pivotColumn] + " " + key] = (decimal)newRow[dr[pivotColumn] + " " + key] + (decimal)dr[key];

                }
                if (add)
                    display.Rows.Add(newRow);
            }

            return display;
        }
    }
}