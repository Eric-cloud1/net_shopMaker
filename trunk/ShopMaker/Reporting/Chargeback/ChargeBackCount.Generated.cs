//Generated by PersistableBaseGenerator

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Stores;
using MakerShop.Utility;

namespace MakerShop.Reporting
{
   
    public partial class ChargeBackCount : IPersistable
    {
        #region Constructors

        public ChargeBackCount() { }

        public ChargeBackCount(int pPaymentGatewayId, DateTime pChargeBackDate)
        {
            this.PaymentGatewayId = pPaymentGatewayId;
            this.ChargeBackDate = pChargeBackDate;

        }

        public static string GetColumnNames(string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) prefix = string.Empty;
            else prefix = prefix + ".";
            List<string> columnNames = new List<string>();
            columnNames.Add(prefix + "PaymentGatewayId");
            columnNames.Add(prefix + "ChargeBackDate");
            columnNames.Add(prefix + "Count");
            columnNames.Add(prefix + "CreateDate");
            columnNames.Add(prefix + "CreateUser");
            columnNames.Add(prefix + "ChangeDate");
            columnNames.Add(prefix + "ChangeUser");
            columnNames.Add(prefix + "PaymentInstrumentId");


            return string.Join(",", columnNames.ToArray());
        }

        public static void LoadDataReader(ChargeBackCount pChargeBackCounts, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA

            pChargeBackCounts.PaymentGatewayId = dr.GetInt32(0);

            pChargeBackCounts.ChargeBackDate = dr.GetDateTime(1);

            pChargeBackCounts.Count = dr.GetInt32(2);

            pChargeBackCounts.CreateDate = dr.GetDateTime(3);

            pChargeBackCounts.CreateUser = dr.GetString(4);

            pChargeBackCounts.ChangeDate = dr.GetDateTime(5);

            pChargeBackCounts.ChangeUser = dr.GetString(6);

            pChargeBackCounts.PaymentInstrumentId = dr.GetInt32(7);


            pChargeBackCounts.IsDirty = false;
        }

        #endregion

        #region Class Data

        private bool _IsDirty;

        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }


        private int _PaymentGatewayId;

        [DataObjectField(true, false, false)]
        public int PaymentGatewayId
        {
            get { return this._PaymentGatewayId; }
            set
            {
                if (this._PaymentGatewayId != value)
                {
                    this._PaymentGatewayId = value;
                    this.IsDirty = true;
                }
            }
        }


        private DateTime _ChargeBackDate;

        [DataObjectField(true, false, false)]
        public DateTime ChargeBackDate
        {
            get { return this._ChargeBackDate; }
            set
            {
                if (this._ChargeBackDate != value)
                {
                    this._ChargeBackDate = value;
                    this.IsDirty = true;
                }
            }
        }


        private int _Count;

        [DataObjectField(true, false, false)]
        public int Count
        {
            get { return this._Count; }
            set
            {
                if (this._Count != value)
                {
                    this._Count = value;
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


        private string _CreateUser;

        [DataObjectField(true, false, false)]
        public string CreateUser
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

        private Int32 _paymentInstrumentId;

        [DataObjectField(true, false, false)]
        public Int32 PaymentInstrumentId
        {
            get { return this._paymentInstrumentId; }
            set
            {
                if (this._paymentInstrumentId != value)
                {
                    this._paymentInstrumentId = value;
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


        private string _ChangeUser;

        [DataObjectField(true, false, false)]
        public string ChangeUser
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
            deleteQuery.Append("DELETE FROM rpt_ChargeBackCounts");
            deleteQuery.Append(" WHERE  PaymentGatewayId = @PaymentGatewayId AND  ChargeBackDate = @ChargeBackDate AND PaymentInstrumentId = @PaymentInstrumentId  ");

            Database database = Token.Instance.Database;

            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null);
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {

                database.AddInParameter(deleteCommand, "@PaymentGatewayId", System.Data.DbType.Int32, this.PaymentGatewayId);

                database.AddInParameter(deleteCommand, "@ChargeBackDate", System.Data.DbType.Date, this.ChargeBackDate);

                database.AddInParameter(deleteCommand, "@PaymentInstrumentId", System.Data.DbType.Int32, this.PaymentInstrumentId);

                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }

            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();

            return (recordsAffected > 0);
        }

        public virtual bool Load(int pPaymentGatewayId, DateTime pChargeBackDate)
        {
            bool result = false;

            this.PaymentGatewayId = pPaymentGatewayId;

            this.ChargeBackDate = pChargeBackDate;


            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM rpt_ChargeBackCounts");
            selectQuery.Append(" WHERE  PaymentGatewayId = @PaymentGatewayId AND  ChargeBackDate = @ChargeBackDate  ");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());


            database.AddInParameter(selectCommand, "@PaymentGatewayId", System.Data.DbType.Int32, this.PaymentGatewayId);

            database.AddInParameter(selectCommand, "@ChargeBackDate", System.Data.DbType.Date, this.ChargeBackDate);


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

                this.CreateDate = DateTime.UtcNow;
                this.CreateUser = Token.Instance.User.UserName;
                this.ChangeDate = DateTime.UtcNow;
                this.ChangeUser = Token.Instance.User.UserName;

                Database database = Token.Instance.Database;
                MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
                bool recordExists = true;

                if (recordExists)
                {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) As RecordCount FROM rpt_ChargeBackCounts");
                    selectQuery.Append(" WHERE  PaymentGatewayId = @PaymentGatewayId AND  ");
                    selectQuery.Append(" ChargeBackDate = @ChargeBackDate AND PaymentInstrumentId = @PaymentInstrumentId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {

                        database.AddInParameter(selectCommand, "@PaymentGatewayId", System.Data.DbType.Int32, this.PaymentGatewayId);

                        database.AddInParameter(selectCommand, "@PaymentInstrumentId", System.Data.DbType.Int32, this.PaymentInstrumentId);

                        database.AddInParameter(selectCommand, "@ChargeBackDate", System.Data.DbType.Date, this.ChargeBackDate);


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
                    updateQuery.Append("UPDATE rpt_ChargeBackCounts SET ");

                    updateQuery.Append("Count = @Count,ChangeDate = @ChangeDate,ChangeUser = @ChangeUser, PaymentInstrumentId = @PaymentInstrumentId");

                    updateQuery.Append(" WHERE  PaymentGatewayId = @PaymentGatewayId AND  ChargeBackDate = @ChargeBackDate AND  PaymentInstrumentId = @PaymentInstrumentId  ");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {


                        database.AddInParameter(updateCommand, "@PaymentGatewayId", System.Data.DbType.Int32, this.PaymentGatewayId);

                        database.AddInParameter(updateCommand, "@ChargeBackDate", System.Data.DbType.Date, this.ChargeBackDate);

                        database.AddInParameter(updateCommand, "@Count", System.Data.DbType.Int32, this.Count);

                        database.AddInParameter(updateCommand, "@ChangeDate", System.Data.DbType.DateTime, this.ChangeDate);

                        database.AddInParameter(updateCommand, "@ChangeUser", System.Data.DbType.String, this.ChangeUser);

                        database.AddInParameter(updateCommand, "@PaymentInstrumentId", System.Data.DbType.Int32, this.PaymentInstrumentId);


                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO rpt_ChargeBackCounts (PaymentGatewayId,ChargeBackDate,Count,CreateDate,CreateUser,ChangeDate,ChangeUser, PaymentInstrumentId )");
                    insertQuery.Append(" VALUES (@PaymentGatewayId,@ChargeBackDate,@Count,@CreateDate,@CreateUser,@ChangeDate,@ChangeUser, @PaymentInstrumentId )");



                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {

                        database.AddInParameter(insertCommand, "@PaymentGatewayId", System.Data.DbType.Int32, this.PaymentGatewayId);

                        database.AddInParameter(insertCommand, "@ChargeBackDate", System.Data.DbType.Date, this.ChargeBackDate);

                        database.AddInParameter(insertCommand, "@Count", System.Data.DbType.Int32, this.Count);

                        database.AddInParameter(insertCommand, "@CreateDate", System.Data.DbType.DateTime, this.CreateDate);

                        database.AddInParameter(insertCommand, "@CreateUser", System.Data.DbType.String, this.CreateUser);

                        database.AddInParameter(insertCommand, "@ChangeDate", System.Data.DbType.DateTime, this.ChangeDate);

                        database.AddInParameter(insertCommand, "@ChangeUser", System.Data.DbType.String, this.ChangeUser);

                        database.AddInParameter(insertCommand, "@PaymentInstrumentId", System.Data.DbType.Int32, this.PaymentInstrumentId);

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
