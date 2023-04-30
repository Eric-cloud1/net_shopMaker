using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;

namespace MakerShop.Payments
{
    public partial class PaymentGatewayGroupsDataSource
    {
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static bool Delete(PaymentGatewayGroups pPaymentGatewayGroups)
        {
            return pPaymentGatewayGroups.Delete();
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        public static bool Delete(Int32 pPaymentGatewayGroupId)
        {
            PaymentGatewayGroups varPaymentGatewayGroups = new PaymentGatewayGroups();
            if (varPaymentGatewayGroups.Load(pPaymentGatewayGroupId)) return varPaymentGatewayGroups.Delete();
            return false;
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Insert(PaymentGatewayGroups pPaymentGatewayGroups) { return pPaymentGatewayGroups.Save(); }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PaymentGatewayGroups Load(Int32 pPaymentGatewayGroupId)
        {
            PaymentGatewayGroups varPaymentGatewayGroups = new PaymentGatewayGroups();
            if (varPaymentGatewayGroups.Load(pPaymentGatewayGroupId)) return varPaymentGatewayGroups;
            return null;
        }

        public static int CountForCriteria(string sqlCriteria)
        {
            Database database = Token.Instance.Database;
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM xm_PaymentGatewayGroups" + whereClause);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PaymentGatewayGroupsCollection LoadForCriteria(string sqlCriteria)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, string.Empty);
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PaymentGatewayGroupsCollection LoadForCriteria(string sqlCriteria, string sortExpression)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, sortExpression);
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PaymentGatewayGroupsCollection LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex)
        {
            return LoadForCriteria(sqlCriteria, maximumRows, startRowIndex, string.Empty);
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PaymentGatewayGroupsCollection LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex, string sortExpression)
        {
            //DEFAULT SORT EXPRESSION
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "PaymentGatewayGroup";
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + PaymentGatewayGroups.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM xm_PaymentGatewayGroups");
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            selectQuery.Append(whereClause);
            selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            //EXECUTE THE COMMAND
            PaymentGatewayGroupsCollection results = new PaymentGatewayGroupsCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        PaymentGatewayGroups varPaymentGatewayGroups = new PaymentGatewayGroups();
                        PaymentGatewayGroups.LoadDataReader(varPaymentGatewayGroups, dr);
                        results.Add(varPaymentGatewayGroups);
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
        public static SaveResult Update(PaymentGatewayGroups pPaymentGatewayGroups) { return pPaymentGatewayGroups.Save(); }
    }
}
