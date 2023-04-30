using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;

namespace MakerShop.Messaging
{
    [DataObject(true)]
    public partial class EmailTemplateTrigger_CatalogDataSource
    {

        /// <summary>
        /// Loads a collection of EmailTemplateTrigger objects for the given EmailTemplateId from the database
        /// </summary>
        /// <param name="emailTemplateId">The given EmailTemplateId</param>
        /// <returns>A collection of EmailTemplateTrigger objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static EmailTemplateTrigger_CatalogCollection LoadForEmailTemplate(Int32 emailTemplateId)
        {
            return LoadForEmailTemplate(emailTemplateId, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of EmailTemplateTrigger objects for the given EmailTemplateId from the database
        /// </summary>
        /// <param name="emailTemplateId">The given EmailTemplateId</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of EmailTemplateTrigger objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static EmailTemplateTrigger_CatalogCollection LoadForEmailTemplate(Int32 emailTemplateId, string sortExpression)
        {
            return LoadForEmailTemplate(emailTemplateId, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of EmailTemplateTrigger objects for the given EmailTemplateId from the database
        /// </summary>
        /// <param name="emailTemplateId">The given EmailTemplateId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of EmailTemplateTrigger objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static EmailTemplateTrigger_CatalogCollection LoadForEmailTemplate(Int32 emailTemplateId, int maximumRows, int startRowIndex)
        {
            return LoadForEmailTemplate(emailTemplateId, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of EmailTemplateTrigger objects for the given EmailTemplateId from the database
        /// </summary>
        /// <param name="emailTemplateId">The given EmailTemplateId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of EmailTemplateTrigger objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static EmailTemplateTrigger_CatalogCollection LoadForEmailTemplate(Int32 emailTemplateId, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + EmailTemplateTrigger_Catalog.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM xm_EmailTemplateTriggers_Catalog");
            selectQuery.Append(" WHERE EmailTemplateId = @emailTemplateId");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@emailTemplateId", System.Data.DbType.Int32, emailTemplateId);
            //EXECUTE THE COMMAND
            EmailTemplateTrigger_CatalogCollection results = new EmailTemplateTrigger_CatalogCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        EmailTemplateTrigger_Catalog emailTemplateTrigger = new EmailTemplateTrigger_Catalog();
                        EmailTemplateTrigger_Catalog.LoadDataReader(emailTemplateTrigger, dr);
                        results.Add(emailTemplateTrigger);
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
