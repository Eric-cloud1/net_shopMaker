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
    public partial class CallOrderDisposition : IPersistable
    {

        #region Constructors

        public CallOrderDisposition() { }

        public CallOrderDisposition(Int32 pOrderId, Int32 pCallDispositionId, Int32 pUserId)
        {
            this.OrderId = pOrderId;
            this.CallDispositionId = pCallDispositionId;
            this.UserId = pUserId;

        }

        public static string GetColumnNames(string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) prefix = string.Empty;
            else prefix = prefix + ".";
            List<string> columnNames = new List<string>();
            columnNames.Add(prefix + "OrderId");
            columnNames.Add(prefix + "CallDispositionId");
            columnNames.Add(prefix + "UserId");
            columnNames.Add(prefix + "CreateDate");


            return string.Join(",", columnNames.ToArray());
        }

        public static void LoadDataReader(CallOrderDisposition pCallOrderDisposition, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA

            pCallOrderDisposition.OrderId = dr.GetInt32(0);

            pCallOrderDisposition.CallDispositionId = dr.GetInt32(1);

            pCallOrderDisposition.UserId = dr.GetInt32(2);


            pCallOrderDisposition.CreateDate = NullableData.GetDateTime(dr, 3);

            pCallOrderDisposition.IsDirty = false;
        }

        #endregion

        #region Class Data

        private bool _IsDirty;

        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }


        private Int32 _OrderId;

        [DataObjectField(true, false, false)]
        public Int32 OrderId
        {
            get { return this._OrderId; }
            set
            {
                if (this._OrderId != value)
                {
                    this._OrderId = value;
                    this.IsDirty = true;
                }
            }
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


        private Int32 _UserId;

        [DataObjectField(true, false, false)]
        public Int32 UserId
        {
            get { return this._UserId; }
            set
            {
                if (this._UserId != value)
                {
                    this._UserId = value;
                    this.IsDirty = true;
                }
            }
        }


        private DateTime? _CreateDate;

        [DataObjectField(true, false, false)]
        public DateTime? CreateDate
        {
            get { return this._CreateDate; }
            set
            {
                if (this._CreateDate != value)
                {
                    this._CreateDate = value;
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
            deleteQuery.Append("DELETE FROM xm_CallOrderDisposition");
            deleteQuery.Append(" WHERE  OrderId = @OrderId AND  CallDispositionId = @CallDispositionId AND  UserId = @UserId  ");
            Database database = Token.Instance.Database;
            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null);
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {

                database.AddInParameter(deleteCommand, "@OrderId", System.Data.DbType.Int32, this.OrderId);

                database.AddInParameter(deleteCommand, "@CallDispositionId", System.Data.DbType.Int32, this.CallDispositionId);

                database.AddInParameter(deleteCommand, "@UserId", System.Data.DbType.Int32, this.UserId);

                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            return (recordsAffected > 0);
        }

        public virtual bool Load(Int32 pOrderId, Int32 pCallDispositionId, Int32 pUserId)
        {
            bool result = false;

            this.OrderId = pOrderId;

            this.CallDispositionId = pCallDispositionId;

            this.UserId = pUserId;


            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM xm_CallOrderDisposition");
            selectQuery.Append(" WHERE  OrderId = @OrderId AND  CallDispositionId = @CallDispositionId AND  UserId = @UserId  ");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());


            database.AddInParameter(selectCommand, "@OrderId", System.Data.DbType.Int32, this.OrderId);

            database.AddInParameter(selectCommand, "@CallDispositionId", System.Data.DbType.Int32, this.CallDispositionId);

            database.AddInParameter(selectCommand, "@UserId", System.Data.DbType.Int32, this.UserId);


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
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM xm_CallOrderDisposition");
                    selectQuery.Append(" WHERE  OrderId = @OrderId AND  CallDispositionId = @CallDispositionId AND  UserId = @UserId ");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {

                        database.AddInParameter(selectCommand, "@OrderId", System.Data.DbType.Int32, this.OrderId);

                        database.AddInParameter(selectCommand, "@CallDispositionId", System.Data.DbType.Int32, this.CallDispositionId);

                        database.AddInParameter(selectCommand, "@UserId", System.Data.DbType.Int32, this.UserId);


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
                    updateQuery.Append("UPDATE xm_CallOrderDisposition SET ");

                    updateQuery.Append("CreateDate = @CreateDate");

                    updateQuery.Append(" WHERE  OrderId = @OrderId AND  CallDispositionId = @CallDispositionId AND  UserId = @UserId ");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {


                        database.AddInParameter(updateCommand, "@OrderId", System.Data.DbType.Int32, this.OrderId);

                        database.AddInParameter(updateCommand, "@CallDispositionId", System.Data.DbType.Int32, this.CallDispositionId);

                        database.AddInParameter(updateCommand, "@UserId", System.Data.DbType.Int32, this.UserId);

                        database.AddInParameter(updateCommand, "@CreateDate", System.Data.DbType.DateTime, this.CreateDate);


                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO xm_CallOrderDisposition (OrderId,CallDispositionId,UserId,CreateDate )");
                    insertQuery.Append(" VALUES (@OrderId,@CallDispositionId,@UserId,@CreateDate )");



                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {

                        database.AddInParameter(insertCommand, "@OrderId", System.Data.DbType.Int32, this.OrderId);

                        database.AddInParameter(insertCommand, "@CallDispositionId", System.Data.DbType.Int32, this.CallDispositionId);

                        database.AddInParameter(insertCommand, "@UserId", System.Data.DbType.Int32, this.UserId);

                        database.AddInParameter(insertCommand, "@CreateDate", System.Data.DbType.DateTime, this.CreateDate);

                        //RESULT IS NUMBER OF RECORDS AFFECTED;
                        result = database.ExecuteNonQuery(insertCommand);



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
