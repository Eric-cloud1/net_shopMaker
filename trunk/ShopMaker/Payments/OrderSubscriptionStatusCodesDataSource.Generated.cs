using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Utility;
using MakerShop.Data;
using MakerShop.Orders;
using MakerShop.Payments;
using MakerShop.Stores;

namespace MakerShop.Payments
{
       public partial class OrderSubscriptionStatusCodesDataSource
        {
            [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
            public static bool Delete(OrderSubscriptionStatusCodes pOrderSubscriptionStatusCodes)
            {
                return pOrderSubscriptionStatusCodes.Delete();
            }

            [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
            public static bool Delete(Byte pSubscriptionStatusCode)
            {
                OrderSubscriptionStatusCodes varOrderSubscriptionStatusCodes = new OrderSubscriptionStatusCodes();
                if (varOrderSubscriptionStatusCodes.Load(pSubscriptionStatusCode)) return varOrderSubscriptionStatusCodes.Delete();
                return false;
            }

            [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert)]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
            public static SaveResult Insert(OrderSubscriptionStatusCodes pOrderSubscriptionStatusCodes) { return pOrderSubscriptionStatusCodes.Save(); }

            [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
            public static OrderSubscriptionStatusCodes Load(Byte pSubscriptionStatusCode)
            {
                OrderSubscriptionStatusCodes varOrderSubscriptionStatusCodes = new OrderSubscriptionStatusCodes();
                if (varOrderSubscriptionStatusCodes.Load(pSubscriptionStatusCode)) return varOrderSubscriptionStatusCodes;
                return null;
            }

            public static int CountForCriteria(string sqlCriteria)
            {
                Database database = Token.Instance.Database;
                string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
                DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM en_OrderSubscriptionStatusCodes" + whereClause);
                return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
            }

            [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
            public static OrderSubscriptionStatusCodesCollection LoadForCriteria(string sqlCriteria)
            {
                return LoadForCriteria(sqlCriteria, 0, 0, string.Empty);
            }

            [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
            public static OrderSubscriptionStatusCodesCollection LoadForCriteria(string sqlCriteria, string sortExpression)
            {
                return LoadForCriteria(sqlCriteria, 0, 0, sortExpression);
            }

            [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
            public static OrderSubscriptionStatusCodesCollection LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex)
            {
                return LoadForCriteria(sqlCriteria, maximumRows, startRowIndex, string.Empty);
            }

            [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
            public static OrderSubscriptionStatusCodesCollection LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex, string sortExpression)
            {
                //DEFAULT SORT EXPRESSION
                if (string.IsNullOrEmpty(sortExpression)) sortExpression = "OrderBy";
                //CREATE THE DYNAMIC SQL TO LOAD OBJECT
                StringBuilder selectQuery = new StringBuilder();
                selectQuery.Append("SELECT");
                if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
                selectQuery.Append(" " + OrderSubscriptionStatusCodes.GetColumnNames(string.Empty));
                selectQuery.Append(" FROM en_OrderSubscriptionStatusCodes");
                string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
                selectQuery.Append(whereClause);
                selectQuery.Append(" ORDER BY " + sortExpression);
                Database database = Token.Instance.Database;
                DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
                //EXECUTE THE COMMAND
                OrderSubscriptionStatusCodesCollection results = new OrderSubscriptionStatusCodesCollection();
                int thisIndex = 0;
                int rowCount = 0;
                using (IDataReader dr = database.ExecuteReader(selectCommand))
                {
                    while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                    {
                        if (thisIndex >= startRowIndex)
                        {
                            OrderSubscriptionStatusCodes varOrderSubscriptionStatusCodes = new OrderSubscriptionStatusCodes();
                            OrderSubscriptionStatusCodes.LoadDataReader(varOrderSubscriptionStatusCodes, dr);
                            results.Add(varOrderSubscriptionStatusCodes);
                            rowCount++;
                        }
                        thisIndex++;
                    }
                    dr.Close();
                }
                return results;
            }

            [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Update)]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
            public static SaveResult Update(OrderSubscriptionStatusCodes pOrderSubscriptionStatusCodes) { return pOrderSubscriptionStatusCodes.Save(); }
        }
    }

