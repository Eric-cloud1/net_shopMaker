//Generated by DataSourceBaseGenerator

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
    /// <summary>
    /// DataSource class for ProductPaymentGatewaysDataSource objects
    /// </summary>
    public partial class PaymentGatewayTemplate_ProductDataSource
    {
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static bool Delete(PaymentGatewayTemplate_Product pProductPaymentGatewayTemplate)
        {
            return pProductPaymentGatewayTemplate.Delete();
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        public static bool Delete(Int32 pProductId)
        {
            PaymentGatewayTemplate_Product varProductPaymentGatewayTemplate = new PaymentGatewayTemplate_Product();
            if (varProductPaymentGatewayTemplate.Load(pProductId)) return varProductPaymentGatewayTemplate.Delete();
            return false;
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Insert(PaymentGatewayTemplate_Product pProductPaymentGatewayTemplate) { return pProductPaymentGatewayTemplate.Save(); }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PaymentGatewayTemplate_Product Load(Int32 pProductId)
        {
            PaymentGatewayTemplate_Product varProductPaymentGatewayTemplate = new PaymentGatewayTemplate_Product();
            if (varProductPaymentGatewayTemplate.Load(pProductId)) return varProductPaymentGatewayTemplate;
            //todo: check when there is no template return a null which fails call to child static methods (delete)
            return null;
        }

        public static int CountForCriteria(string sqlCriteria)
        {
            Database database = Token.Instance.Database;
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            DbCommand selectCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) AS TotalRecords FROM xm_PaymentGatewayTemplates_Product" + whereClause);
            return MakerShop.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PaymentGatewayTemplate_ProductCollection LoadForCriteria(string sqlCriteria)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, string.Empty);
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PaymentGatewayTemplate_ProductCollection LoadForCriteria(string sqlCriteria, string sortExpression)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, sortExpression);
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PaymentGatewayTemplate_ProductCollection LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex)
        {
            return LoadForCriteria(sqlCriteria, maximumRows, startRowIndex, string.Empty);
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PaymentGatewayTemplate_ProductCollection LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex, string sortExpression)
        {

            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + PaymentGatewayTemplate_Product.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM xm_PaymentGatewayTemplates_Product");
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            selectQuery.Append(whereClause);
            if (!(string.IsNullOrEmpty(sortExpression)) )
                selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            //EXECUTE THE COMMAND
            PaymentGatewayTemplate_ProductCollection results = new PaymentGatewayTemplate_ProductCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        PaymentGatewayTemplate_Product varProductPaymentGatewayTemplate = new PaymentGatewayTemplate_Product();
                        PaymentGatewayTemplate_Product.LoadDataReader(varProductPaymentGatewayTemplate, dr);
                        results.Add(varProductPaymentGatewayTemplate);
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
        public static SaveResult Update(PaymentGatewayTemplate_Product pProductPaymentGatewayTemplate) { return pProductPaymentGatewayTemplate.Save(); }
    }
}

