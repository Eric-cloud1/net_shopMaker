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
    public partial class OrderSubscriptionStatusCodes : IPersistable
    {

        #region Constructors

        public OrderSubscriptionStatusCodes() { }

        public OrderSubscriptionStatusCodes(Byte pSubscriptionStatusCode)
        {
            this.SubscriptionStatusCode = pSubscriptionStatusCode;

        }

        public static string GetColumnNames(string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) prefix = string.Empty;
            else prefix = prefix + ".";
            List<string> columnNames = new List<string>();
            columnNames.Add(prefix + "SubscriptionStatusCode");
            columnNames.Add(prefix + "SubscriptionStatus");
            columnNames.Add(prefix + "CreateDate");
            columnNames.Add(prefix + "CreateUser");
            columnNames.Add(prefix + "ChangeDate");
            columnNames.Add(prefix + "ChangeUser");


            return string.Join(",", columnNames.ToArray());
        }

        public static void LoadDataReader(OrderSubscriptionStatusCodes pOrderSubscriptionStatusCodes, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA

            pOrderSubscriptionStatusCodes.SubscriptionStatusCode = dr.GetByte(0);

            pOrderSubscriptionStatusCodes.SubscriptionStatus = dr.GetString(1);

            pOrderSubscriptionStatusCodes.CreateDate = LocaleHelper.ToLocalTime(dr.GetDateTime(2));

            pOrderSubscriptionStatusCodes.CreateUser = dr.GetString(3);

            pOrderSubscriptionStatusCodes.ChangeDate = LocaleHelper.ToLocalTime(dr.GetDateTime(4));

            pOrderSubscriptionStatusCodes.ChangeUser = dr.GetString(5);


            pOrderSubscriptionStatusCodes.IsDirty = false;
        }

        #endregion

        #region Class Data

        private bool _IsDirty;

        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }


        private Byte _SubscriptionStatusCode;

        [DataObjectField(true, false, false)]
        public Byte SubscriptionStatusCode
        {
            get { return this._SubscriptionStatusCode; }
            set
            {
                if (this._SubscriptionStatusCode != value)
                {
                    this._SubscriptionStatusCode = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _SubscriptionStatus;

        [DataObjectField(true, false, false)]
        public String SubscriptionStatus
        {
            get { return this._SubscriptionStatus; }
            set
            {
                if (this._SubscriptionStatus != value)
                {
                    this._SubscriptionStatus = value;
                    this.IsDirty = true;
                }
            }
        }


        private DateTime _CreateDate;

        [DataObjectField(true, false, false)]
        public DateTime CreateDate
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


        private String _CreateUser;

        [DataObjectField(true, false, false)]
        public String CreateUser
        {
            get { return this._CreateUser; }
            set
            {
                if (this._CreateUser != value)
                {
                    this._CreateUser = value;
                    this.IsDirty = true;
                }
            }
        }


        private DateTime _ChangeDate;

        [DataObjectField(true, false, false)]
        public DateTime ChangeDate
        {
            get { return this._ChangeDate; }
            set
            {
                if (this._ChangeDate != value)
                {
                    this._ChangeDate = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _ChangeUser;

        [DataObjectField(true, false, false)]
        public String ChangeUser
        {
            get { return this._ChangeUser; }
            set
            {
                if (this._ChangeUser != value)
                {
                    this._ChangeUser = value;
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
            deleteQuery.Append("DELETE FROM en_OrderSubscriptionStatusCodes");
            deleteQuery.Append(" WHERE  SubscriptionStatusCode = @SubscriptionStatusCode  ");
            Database database = Token.Instance.Database;

            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {

                database.AddInParameter(deleteCommand, "@SubscriptionStatusCode", System.Data.DbType.Byte, this.SubscriptionStatusCode);

                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }

            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            return (recordsAffected > 0);
        }

        public virtual bool Load(Byte pSubscriptionStatusCode)
        {
            bool result = false;

            this.SubscriptionStatusCode = pSubscriptionStatusCode;


            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM en_OrderSubscriptionStatusCodes");
            selectQuery.Append(" WHERE  SubscriptionStatusCode = @SubscriptionStatusCode  ");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());


            database.AddInParameter(selectCommand, "@SubscriptionStatusCode", System.Data.DbType.Byte, this.SubscriptionStatusCode);


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
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM en_OrderSubscriptionStatusCodes");
                    selectQuery.Append(" WHERE  SubscriptionStatusCode = @SubscriptionStatusCode ");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {

                        database.AddInParameter(selectCommand, "@SubscriptionStatusCode", System.Data.DbType.Byte, this.SubscriptionStatusCode);


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
                    updateQuery.Append("UPDATE en_OrderSubscriptionStatusCodes SET ");

                    updateQuery.Append("SubscriptionStatus = @SubscriptionStatus,CreateDate = @CreateDate,CreateUser = @CreateUser,ChangeDate = @ChangeDate,ChangeUser = @ChangeUser");

                    updateQuery.Append(" WHERE  SubscriptionStatusCode = @SubscriptionStatusCode ");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {


                        database.AddInParameter(updateCommand, "@SubscriptionStatusCode", System.Data.DbType.Byte, this.SubscriptionStatusCode);

                        database.AddInParameter(updateCommand, "@SubscriptionStatus", System.Data.DbType.String, this.SubscriptionStatus);

                        database.AddInParameter(updateCommand, "@CreateDate", System.Data.DbType.DateTime, this.CreateDate);

                        database.AddInParameter(updateCommand, "@CreateUser", System.Data.DbType.String, this.CreateUser);

                        database.AddInParameter(updateCommand, "@ChangeDate", System.Data.DbType.DateTime, this.ChangeDate);

                        database.AddInParameter(updateCommand, "@ChangeUser", System.Data.DbType.String, this.ChangeUser);


                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO en_OrderSubscriptionStatusCodes (SubscriptionStatusCode,SubscriptionStatus,CreateDate,CreateUser,ChangeDate,ChangeUser )");
                    insertQuery.Append(" VALUES (@SubscriptionStatusCode,@SubscriptionStatus,@CreateDate,@CreateUser,@ChangeDate,@ChangeUser )");


                    insertQuery.Append("; SELECT @@IDENTITY");


                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {

                        database.AddInParameter(insertCommand, "@SubscriptionStatusCode", System.Data.DbType.Byte, this.SubscriptionStatusCode);

                        database.AddInParameter(insertCommand, "@SubscriptionStatus", System.Data.DbType.String, this.SubscriptionStatus);

                        database.AddInParameter(insertCommand, "@CreateDate", System.Data.DbType.DateTime, this.CreateDate);

                        database.AddInParameter(insertCommand, "@CreateUser", System.Data.DbType.String, this.CreateUser);

                        database.AddInParameter(insertCommand, "@ChangeDate", System.Data.DbType.DateTime, this.ChangeDate);

                        database.AddInParameter(insertCommand, "@ChangeUser", System.Data.DbType.String, this.ChangeUser);

                        //RESULT IS NUMBER OF RECORDS AFFECTED;
                        result = database.ExecuteNonQuery(insertCommand);


                        this.SubscriptionStatusCode = (byte)result;


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
