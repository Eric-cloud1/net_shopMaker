using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;

namespace MakerShop.Affiliates
{
    public partial class AffiliateCode_AffiliateId : IPersistable
    {

        #region Constructors

        public AffiliateCode_AffiliateId() { }

        public AffiliateCode_AffiliateId(String pAffiliateCode)
        {
            this.AffiliateCode = pAffiliateCode;

        }

        public static string GetColumnNames(string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) prefix = string.Empty;
            else prefix = prefix + ".";
            List<string> columnNames = new List<string>();
            columnNames.Add(prefix + "AffiliateCode");
            columnNames.Add(prefix + "AffiliateId");


            return string.Join(",", columnNames.ToArray());
        }

        public static void LoadDataReader(AffiliateCode_AffiliateId pAffiliateCode_AffiliateId, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA

            pAffiliateCode_AffiliateId.AffiliateCode = dr.GetString(0);

            pAffiliateCode_AffiliateId.AffiliateId = dr.GetInt32(1);


            pAffiliateCode_AffiliateId.IsDirty = false;
        }

        #endregion

        #region Class Data

        private bool _IsDirty;

        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }


        private String _AffiliateCode;

        [DataObjectField(true, false, false)]
        public String AffiliateCode
        {
            get { return this._AffiliateCode; }
            set
            {
                if (this._AffiliateCode != value)
                {
                    this._AffiliateCode = value;
                    this.IsDirty = true;
                }
            }
        }


        private Int32 _AffiliateId;

        [DataObjectField(true, false, false)]
        public Int32 AffiliateId
        {
            get { return this._AffiliateId; }
            set
            {
                if (this._AffiliateId != value)
                {
                    this._AffiliateId = value;
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
            deleteQuery.Append("DELETE FROM xm_AffiliateCode_AffiliateId");
            deleteQuery.Append(" WHERE  AffiliateCode = @AffiliateCode  ");
            Database database = Token.Instance.Database;

            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null);
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {

                database.AddInParameter(deleteCommand, "@AffiliateCode", System.Data.DbType.String, this.AffiliateCode);

                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            return (recordsAffected > 0);
        }

        public virtual bool Load(String pAffiliateCode)
        {
            bool result = false;

            this.AffiliateCode = pAffiliateCode;


            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM xm_AffiliateCode_AffiliateId");
            selectQuery.Append(" WHERE  AffiliateCode = @AffiliateCode  ");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());


            database.AddInParameter(selectCommand, "@AffiliateCode", System.Data.DbType.String, this.AffiliateCode);


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
                Database database = Token.Instance.Database;
                MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
                bool recordExists = true;

                if (recordExists)
                {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM xm_AffiliateCode_AffiliateId");
                    selectQuery.Append(" WHERE  AffiliateCode = @AffiliateCode ");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {

                        database.AddInParameter(selectCommand, "@AffiliateCode", System.Data.DbType.String, this.AffiliateCode);


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
                    updateQuery.Append("UPDATE xm_AffiliateCode_AffiliateId SET ");

                    updateQuery.Append("AffiliateId = @AffiliateId");

                    updateQuery.Append(" WHERE  AffiliateCode = @AffiliateCode ");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {


                        database.AddInParameter(updateCommand, "@AffiliateCode", System.Data.DbType.String, this.AffiliateCode);

                        database.AddInParameter(updateCommand, "@AffiliateId", System.Data.DbType.Int32, this.AffiliateId);


                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO xm_AffiliateCode_AffiliateId (AffiliateCode,AffiliateId )");
                    insertQuery.Append(" VALUES (@AffiliateCode,@AffiliateId )");


                    insertQuery.Append("; SELECT @@IDENTITY");


                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {

                        database.AddInParameter(insertCommand, "@AffiliateCode", System.Data.DbType.String, this.AffiliateCode);

                        database.AddInParameter(insertCommand, "@AffiliateId", System.Data.DbType.Int32, this.AffiliateId);

                        //RESULT IS NUMBER OF RECORDS AFFECTED;
                        result = database.ExecuteNonQuery(insertCommand);


                        this.AffiliateCode = result.ToString();


                    }
                }
                MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
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
