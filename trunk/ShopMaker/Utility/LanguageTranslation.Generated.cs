using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;
using MakerShop.Stores;

namespace MakerShop.Utility
{
    public partial class LanguageTranslation : IPersistable
    {

        #region Constructors

        public LanguageTranslation() { }

        public LanguageTranslation(String pCulture, String pFieldName)
        {
            this.Culture = pCulture;
            this.FieldName = pFieldName;

        }

        public static string GetColumnNames(string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) prefix = string.Empty;
            else prefix = prefix + ".";
            List<string> columnNames = new List<string>();
            columnNames.Add(prefix + "LanguageId");
            columnNames.Add(prefix + "Culture");
            columnNames.Add(prefix + "FieldName");
            columnNames.Add(prefix + "FieldValue");
            columnNames.Add(prefix + "Comment");


            return string.Join(",", columnNames.ToArray());
        }

        public static void LoadDataReader(LanguageTranslation pLanguageTranslations, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA

            pLanguageTranslations.LanguageId = dr.GetInt32(0);

            pLanguageTranslations.Culture = dr.GetString(1);

            pLanguageTranslations.FieldName = dr.GetString(2);

            pLanguageTranslations.FieldValue = dr.GetString(3);


            pLanguageTranslations.Comment = NullableData.GetString(dr, 4);

            pLanguageTranslations.IsDirty = false;
        }

        #endregion

        #region Class Data

        private bool _IsDirty;

        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }


        private Int32 _LanguageId;

        [DataObjectField(true, false, false)]
        public Int32 LanguageId
        {
            get { return this._LanguageId; }
            set
            {
                if (this._LanguageId != value)
                {
                    this._LanguageId = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _Culture;

        [DataObjectField(true, false, false)]
        public String Culture
        {
            get { return this._Culture; }
            set
            {
                if (this._Culture != value)
                {
                    this._Culture = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _FieldName;

        [DataObjectField(true, false, false)]
        public String FieldName
        {
            get { return this._FieldName; }
            set
            {
                if (this._FieldName != value)
                {
                    this._FieldName = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _FieldValue;

        [DataObjectField(true, false, false)]
        public String FieldValue
        {
            get { return this._FieldValue; }
            set
            {
                if (this._FieldValue != value)
                {
                    this._FieldValue = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _Comment;

        [DataObjectField(true, false, false)]
        public String Comment
        {
            get { return this._Comment; }
            set
            {
                if (this._Comment != value)
                {
                    this._Comment = value;
                    this.IsDirty = true;
                }
            }
        }



        #endregion



        #region CURD

        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM xm_LanguageTranslations");
            deleteQuery.Append(" WHERE  LanguageId = @LanguageId ");
            //deleteQuery.Append(" WHERE  Culture = @Culture AND  FieldName = @FieldName  ");
           Database database = Token.Instance.Database;
            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null);
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {

                database.AddInParameter(deleteCommand, "@LanguageId", System.Data.DbType.Int32, this.LanguageId);
              //  database.AddInParameter(deleteCommand, "@Culture", System.Data.DbType.String, this.Culture);
              //  database.AddInParameter(deleteCommand, "@FieldName", System.Data.DbType.String, this.FieldName);

                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            return (recordsAffected > 0);
        }

        public virtual bool Load(String pCulture, String pFieldName)
        {
            bool result = false;

            this.Culture = pCulture;

            this.FieldName = pFieldName;


            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM xm_LanguageTranslations");
            selectQuery.Append(" WHERE  Culture = @Culture AND  FieldName = @FieldName  ");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());


            database.AddInParameter(selectCommand, "@Culture", System.Data.DbType.String, this.Culture);

            database.AddInParameter(selectCommand, "@FieldName", System.Data.DbType.String, this.FieldName);


            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                if (dr.Read())
                {
                    result = true;
                    LoadDataReader(this, dr); ;
                }
                dr.Close();
            }
            return result;
        }

        public virtual SaveResult Save()
        {
            if (this.IsDirty)
            {
                AuditEventDataSource.AuditInfoBegin(null);
                Database database = Token.Instance.Database;
                bool recordExists = true;

                if (recordExists)
                {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM xm_LanguageTranslations");
                    selectQuery.Append(" WHERE  LanguageId = @LanguageId ");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@LanguageId", System.Data.DbType.Int32, this.LanguageId);

                        //database.AddInParameter(selectCommand, "@Culture", System.Data.DbType.String, this.Culture);

                        //database.AddInParameter(selectCommand, "@FieldName", System.Data.DbType.String, this.FieldName);


                        if ((int)database.ExecuteScalar(selectCommand) == 0)
                        {
                            recordExists = false;
                        }
                    }
                }

                int result = 0;
                if (recordExists)
                {
                    //UPDATE
                    StringBuilder updateQuery = new StringBuilder();
                    updateQuery.Append("UPDATE xm_LanguageTranslations SET ");

                   // updateQuery.Append("LanguageId = @LanguageId, ");
                    updateQuery.Append(" FieldValue = @FieldValue, Comment = @Comment, FieldName = @FieldName");
                    updateQuery.Append(" WHERE  LanguageId = @LanguageId ");

                   // updateQuery.Append(" WHERE  Culture = @Culture AND  FieldName = @FieldName ");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {


                        database.AddInParameter(updateCommand, "@LanguageId",System.Data.DbType.Int32, this.LanguageId);

                       // database.AddInParameter(updateCommand, "@Culture", System.Data.DbType.String, this.Culture);

                        database.AddInParameter(updateCommand, "@FieldName", System.Data.DbType.String, this.FieldName);

                        database.AddInParameter(updateCommand, "@FieldValue", System.Data.DbType.String, this.FieldValue);

                        database.AddInParameter(updateCommand, "@Comment", System.Data.DbType.String, this.Comment);


                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO xm_LanguageTranslations (Culture,FieldName,FieldValue,Comment )");
                    insertQuery.Append(" VALUES (@Culture,@FieldName,@FieldValue,@Comment )");



                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {

                       // database.AddInParameter(insertCommand, "@LanguageId", System.Data.DbType.Int32, this.LanguageId);

                        database.AddInParameter(insertCommand, "@Culture", System.Data.DbType.String, this.Culture);

                        database.AddInParameter(insertCommand, "@FieldName", System.Data.DbType.String, this.FieldName);

                        database.AddInParameter(insertCommand, "@FieldValue", System.Data.DbType.String, this.FieldValue);

                        database.AddInParameter(insertCommand, "@Comment", System.Data.DbType.String, this.Comment);

                        //RESULT IS NUMBER OF RECORDS AFFECTED;
                        result = database.ExecuteNonQuery(insertCommand);



                    }
                }

                AuditEventDataSource.AuditInfoEnd();
                //OBJECT IS DIRTY IF NO RECORDS WERE UPDATED OR INSERTED
                this.IsDirty = (result == 0);
                if (this.IsDirty) { return SaveResult.Failed; }
                else { return (recordExists ? SaveResult.RecordUpdated : SaveResult.RecordInserted); }
            }

            //SAVE IS SUCCESSFUL IF OBJECT IS NOT DIRTY
            return SaveResult.NotDirty;
        }

        #endregion
    }
}
