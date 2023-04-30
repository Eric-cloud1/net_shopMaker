using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Orders;
using MakerShop.Users;
using MakerShop.Utility;

namespace MakerShop.Orders
{
    public partial class CallDispositions : IPersistable
    {

        #region Constructors

        public CallDispositions() { }

        public CallDispositions(Int32 pCallDispositionId)
        {
            this.CallDispositionId = pCallDispositionId;

        }

        public static string GetColumnNames(string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) prefix = string.Empty;
            else prefix = prefix + ".";
            List<string> columnNames = new List<string>();
            columnNames.Add(prefix + "CallDispositionId");
            columnNames.Add(prefix + "CallDisposition");
            columnNames.Add(prefix + "IsCallBack");


            return string.Join(",", columnNames.ToArray());
        }

        public static void LoadDataReader(CallDispositions pCallDispositions, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA

            pCallDispositions.CallDispositionId = dr.GetInt32(0);


            pCallDispositions.CallDisposition = NullableData.GetString(dr, 1);

            pCallDispositions.IsCallBack = NullableData.GetInt16(dr, 2);

            pCallDispositions.IsDirty = false;
        }

        #endregion

        #region Class Data

        private bool _IsDirty;

        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }


        private Int32 _CallDispositionId;
        [DataObjectField(true, false, false)]
        public Int32 CallDispositionId
        {
            get { return this._CallDispositionId; }
            set
            {
                if (this._CallDispositionId != value)
                {
                    this._CallDispositionId = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _CallDisposition;

        [DataObjectField(true, false, false)]
        public String CallDisposition
        {
            get { return this._CallDisposition; }
            set
            {
                if (this._CallDisposition != value)
                {
                    this._CallDisposition = value;
                    this.IsDirty = true;
                }
            }
        }


        private Int16? _IsCallBack;

        [DataObjectField(true, false, false)]
        public Int16? IsCallBack
        {
            get { return this._IsCallBack; }
            set
            {
                if (this._IsCallBack != value)
                {
                    this._IsCallBack = value;
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
            deleteQuery.Append("DELETE FROM xm_CallDispositions");
            deleteQuery.Append(" WHERE  CallDispositionId = @CallDispositionId  ");
            Database database = Token.Instance.Database;
            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null);
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {

                database.AddInParameter(deleteCommand, "@CallDispositionId", System.Data.DbType.Int32, this.CallDispositionId);

                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            return (recordsAffected > 0);
        }

        public virtual bool Load(Int32 pCallDispositionId)
        {
            bool result = false;

            this.CallDispositionId = pCallDispositionId;


            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM xm_CallDispositions");
            selectQuery.Append(" WHERE  CallDispositionId = @CallDispositionId  ");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());


            database.AddInParameter(selectCommand, "@CallDispositionId", System.Data.DbType.Int32, this.CallDispositionId);


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
                MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null);
                Database database = Token.Instance.Database;
                bool recordExists = true;

                if (recordExists)
                {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM xm_CallDispositions");
                    selectQuery.Append(" WHERE  CallDispositionId = @CallDispositionId ");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {

                        database.AddInParameter(selectCommand, "@CallDispositionId", System.Data.DbType.Int32, this.CallDispositionId);


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
                    updateQuery.Append("UPDATE xm_CallDispositions SET ");

                    updateQuery.Append("CallDisposition = @CallDisposition,IsCallBack = @IsCallBack");

                    updateQuery.Append(" WHERE  CallDispositionId = @CallDispositionId ");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {


                        database.AddInParameter(updateCommand, "@CallDispositionId", System.Data.DbType.Int32, this.CallDispositionId);

                        database.AddInParameter(updateCommand, "@CallDisposition", System.Data.DbType.String, this.CallDisposition);

                        database.AddInParameter(updateCommand, "@IsCallBack", System.Data.DbType.Byte, this.IsCallBack);


                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO xm_CallDispositions (CallDisposition,IsCallBack )");
                    insertQuery.Append(" VALUES (@CallDisposition,@IsCallBack )");


                    insertQuery.Append("; SELECT @@IDENTITY");


                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {

                        database.AddInParameter(insertCommand, "@CallDispositionId", System.Data.DbType.Int32, this.CallDispositionId);

                        database.AddInParameter(insertCommand, "@CallDisposition", System.Data.DbType.String, this.CallDisposition);

                        database.AddInParameter(insertCommand, "@IsCallBack", System.Data.DbType.Byte, this.IsCallBack);

                        //RESULT IS NUMBER OF RECORDS AFFECTED;
                        result = database.ExecuteNonQuery(insertCommand);


                        this.CallDispositionId = result;


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
