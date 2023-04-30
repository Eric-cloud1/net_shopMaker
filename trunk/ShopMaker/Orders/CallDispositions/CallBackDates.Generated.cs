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
    public partial class CallBackDates : IPersistable
    {

        #region Constructors

        public CallBackDates() { }

        public CallBackDates(Int32 pOrderId)
        {
            this.OrderId = pOrderId;

        }

        public static string GetColumnNames(string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) prefix = string.Empty;
            else prefix = prefix + ".";
            List<string> columnNames = new List<string>();
            columnNames.Add(prefix + "OrderId");
            columnNames.Add(prefix + "CallBackDate");


            return string.Join(",", columnNames.ToArray());
        }

        public static void LoadDataReader(CallBackDates pCallBackDates, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            pCallBackDates.OrderId = dr.GetInt32(0);
            pCallBackDates.CallBackDate = NullableData.GetDateTime(dr, 1);
            pCallBackDates.IsDirty = false;
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


        private DateTime? _CallBackDate;

        [DataObjectField(true, false, false)]
        public DateTime? CallBackDate
        {
            get { return this._CallBackDate; }
            set
            {
                if (this._CallBackDate != value)
                {
                    this._CallBackDate = value;
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
            deleteQuery.Append("DELETE FROM xm_CallBackDates");
            deleteQuery.Append(" WHERE  OrderId = @OrderId  ");
            Database database = Token.Instance.Database;
            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null);
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {

                database.AddInParameter(deleteCommand, "@OrderId", System.Data.DbType.Int32, this.OrderId);

                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            return (recordsAffected > 0);
        }

        public virtual bool Load(Int32 pOrderId)
        {
            bool result = false;

            this.OrderId = pOrderId;


            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM xm_CallBackDates");
            selectQuery.Append(" WHERE  OrderId = @OrderId  ");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());


            database.AddInParameter(selectCommand, "@OrderId", System.Data.DbType.Int32, this.OrderId);


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
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM xm_CallBackDates");
                    selectQuery.Append(" WHERE  OrderId = @OrderId ");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {

                        database.AddInParameter(selectCommand, "@OrderId", System.Data.DbType.Int32, this.OrderId);


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
                    updateQuery.Append("UPDATE xm_CallBackDates SET ");

                    updateQuery.Append("CallBackDate = @CallBackDate");

                    updateQuery.Append(" WHERE  OrderId = @OrderId ");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {


                        database.AddInParameter(updateCommand, "@OrderId", System.Data.DbType.Int32, this.OrderId);

                        database.AddInParameter(updateCommand, "@CallBackDate", System.Data.DbType.DateTime, this.CallBackDate);


                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO xm_CallBackDates (OrderId,CallBackDate )");
                    insertQuery.Append(" VALUES (@OrderId,@CallBackDate )");


                    insertQuery.Append("; SELECT @@IDENTITY");


                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {

                        database.AddInParameter(insertCommand, "@OrderId", System.Data.DbType.Int32, this.OrderId);

                        database.AddInParameter(insertCommand, "@CallBackDate", System.Data.DbType.DateTime, this.CallBackDate);

                        //RESULT IS NUMBER OF RECORDS AFFECTED;
                        result = database.ExecuteNonQuery(insertCommand);


                        this.OrderId = result;


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
