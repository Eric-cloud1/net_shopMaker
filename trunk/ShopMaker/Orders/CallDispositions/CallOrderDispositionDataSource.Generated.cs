using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;

namespace MakerShop.Orders
{
    public partial class CallOrderDispositionDataSource
    {
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static bool Delete(CallOrderDisposition pCallOrderDisposition)
        {
            return pCallOrderDisposition.Delete();
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        public static bool Delete(Int32 pOrderId, Int32 pCallDispositionId, Int32 pUserId)
        {
            CallOrderDisposition varCallOrderDisposition = new CallOrderDisposition();
            if (varCallOrderDisposition.Load(pOrderId, pCallDispositionId, pUserId)) return varCallOrderDisposition.Delete();
            return false;
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Insert(CallOrderDisposition pCallOrderDisposition) { return pCallOrderDisposition.Save(); }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CallOrderDisposition Load(Int32 pOrderId, Int32 pCallDispositionId, Int32 pUserId)
        {
            CallOrderDisposition varCallOrderDisposition = new CallOrderDisposition();
            if (varCallOrderDisposition.Load(pOrderId, pCallDispositionId, pUserId)) return varCallOrderDisposition;
            return null;
        }

        public static int CountForCriteria(string sqlCriteria)
        {
            Database database = Token.Instance.Database;
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM xm_CallOrderDisposition" + whereClause);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CallOrderDispositionCollection LoadForCriteria(string sqlCriteria)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, string.Empty);
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CallOrderDispositionCollection LoadForCriteria(string sqlCriteria, string sortExpression)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, sortExpression);
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CallOrderDispositionCollection LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex)
        {
            return LoadForCriteria(sqlCriteria, maximumRows, startRowIndex, string.Empty);
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CallOrderDispositionCollection LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex, string sortExpression)
        {
            //DEFAULT SORT EXPRESSION
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "CreatedDate";
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + CallOrderDisposition.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM xm_CallOrderDisposition");
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            selectQuery.Append(whereClause);
            selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            //EXECUTE THE COMMAND
            CallOrderDispositionCollection results = new CallOrderDispositionCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        CallOrderDisposition varCallOrderDisposition = new CallOrderDisposition();
                        CallOrderDisposition.LoadDataReader(varCallOrderDisposition, dr);
                        results.Add(varCallOrderDisposition);
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
        public static SaveResult Update(CallOrderDisposition pCallOrderDisposition) { return pCallOrderDisposition.Save(); }
    }
}
