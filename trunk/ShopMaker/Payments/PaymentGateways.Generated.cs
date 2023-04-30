using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Payments.Providers;
using MakerShop.Utility;
using System.Web;

namespace MakerShop.Payments
{
    public partial class PaymentGateways : IPersistable
    {

        #region Constructors

        public PaymentGateways() { }

        public PaymentGateways(Int32 pPaymentGatewayId)
        {
            this.PaymentGatewayId = pPaymentGatewayId;

        }

        public static string GetColumnNames(string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) prefix = string.Empty;
            else prefix = prefix + ".";
            List<string> columnNames = new List<string>();
            columnNames.Add(prefix + "PaymentGatewayId");
            columnNames.Add(prefix + "StoreId");
            columnNames.Add(prefix + "Name");
            columnNames.Add(prefix + "ClassId");
            columnNames.Add(prefix + "ConfigData");
            columnNames.Add(prefix + "ReCrypt");
            columnNames.Add(prefix + "Is3D");
            columnNames.Add(prefix + "IsActive");
            columnNames.Add(prefix + "IsPaymentPageHosted");
            columnNames.Add(prefix + "IsPaymentPageIFrame");
            columnNames.Add(prefix + "IsAsynchronous");
            columnNames.Add(prefix + "BlockEmails");


            return string.Join(",", columnNames.ToArray());
        }

        public static void LoadDataReader(PaymentGateways pPaymentGateways, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA

            pPaymentGateways.PaymentGatewayId = dr.GetInt32(0);

            pPaymentGateways.StoreId = dr.GetInt32(1);

            pPaymentGateways.Name = dr.GetString(2);

            pPaymentGateways.ClassId = dr.GetString(3);


            pPaymentGateways.ConfigData = NullableData.GetString(dr, 4);
            pPaymentGateways.ReCrypt = dr.GetBoolean(5);

            pPaymentGateways.Is3D = dr.GetBoolean(6);

            pPaymentGateways.IsActive = dr.GetBoolean(7);

            pPaymentGateways.IsPaymentPageHosted = dr.GetBoolean(8);

            pPaymentGateways.IsPaymentPageIFrame = dr.GetBoolean(9);

            pPaymentGateways.IsAsynchronous = dr.GetBoolean(10);

            pPaymentGateways.BlockEmails = dr.GetBoolean(11);


            pPaymentGateways.IsDirty = false;
        }

        #endregion

        #region Class Data

        private bool _IsDirty;

        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }


        private Int32 _PaymentGatewayId;

        [DataObjectField(true, false, false)]
        public Int32 PaymentGatewayId
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


        private Int32 _StoreId;

        [DataObjectField(true, false, false)]
        public Int32 StoreId
        {
            get { return this._StoreId; }
            set
            {
                if (this._StoreId != value)
                {
                    this._StoreId = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _Name;

        [DataObjectField(true, false, false)]
        public String Name
        {
            get { return this._Name; }
            set
            {
                if (this._Name != value)
                {
                    this._Name = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _ClassId;

        [DataObjectField(true, false, false)]
        public String ClassId
        {
            get { return this._ClassId; }
            set
            {
                if (this._ClassId != value)
                {
                    this._ClassId = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _ConfigData;

        [DataObjectField(true, false, false)]
        public String ConfigData
        {
            get { return this._ConfigData; }
            set
            {
                if (this._ConfigData != value)
                {
                    this._ConfigData = value;
                    this.IsDirty = true;
                }
            }
        }


        private Boolean _ReCrypt;

        [DataObjectField(true, false, false)]
        public Boolean ReCrypt
        {
            get { return this._ReCrypt; }
            set
            {
                if (this._ReCrypt != value)
                {
                    this._ReCrypt = value;
                    this.IsDirty = true;
                }
            }
        }


        private Boolean _Is3D;

        [DataObjectField(true, false, false)]
        public Boolean Is3D
        {
            get { return this._Is3D; }
            set
            {
                if (this._Is3D != value)
                {
                    this._Is3D = value;
                    this.IsDirty = true;
                }
            }
        }


        private Boolean _IsActive;

        [DataObjectField(true, false, false)]
        public Boolean IsActive
        {
            get { return this._IsActive; }
            set
            {
                if (this._IsActive != value)
                {
                    this._IsActive = value;
                    this.IsDirty = true;
                }
            }
        }


        private Boolean _IsPaymentPageHosted;

        [DataObjectField(true, false, false)]
        public Boolean IsPaymentPageHosted
        {
            get { return this._IsPaymentPageHosted; }
            set
            {
                if (this._IsPaymentPageHosted != value)
                {
                    this._IsPaymentPageHosted = value;
                    this.IsDirty = true;
                }
            }
        }


        private Boolean _IsPaymentPageIFrame;

        [DataObjectField(true, false, false)]
        public Boolean IsPaymentPageIFrame
        {
            get { return this._IsPaymentPageIFrame; }
            set
            {
                if (this._IsPaymentPageIFrame != value)
                {
                    this._IsPaymentPageIFrame = value;
                    this.IsDirty = true;
                }
            }
        }


        private Boolean _IsAsynchronous;

        [DataObjectField(true, false, false)]
        public Boolean IsAsynchronous
        {
            get { return this._IsAsynchronous; }
            set
            {
                if (this._IsAsynchronous != value)
                {
                    this._IsAsynchronous = value;
                    this.IsDirty = true;
                }
            }
        }


        private Boolean _BlockEmails;

        [DataObjectField(true, false, false)]
        public Boolean BlockEmails
        {
            get { return this._BlockEmails; }
            set
            {
                if (this._BlockEmails != value)
                {
                    this._BlockEmails = value;
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
            deleteQuery.Append("DELETE FROM ac_PaymentGateways");
            deleteQuery.Append(" WHERE  PaymentGatewayId = @PaymentGatewayId  ");
            Database database = Token.Instance.Database;
            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {

                database.AddInParameter(deleteCommand, "@PaymentGatewayId", System.Data.DbType.Int32, this.PaymentGatewayId);

                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            return (recordsAffected > 0);
        }

        public virtual bool Load(Int32 pPaymentGatewayId)
        {
            bool result = false;

            this.PaymentGatewayId = pPaymentGatewayId;


            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_PaymentGateways");
            selectQuery.Append(" WHERE  PaymentGatewayId = @PaymentGatewayId  ");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());


            database.AddInParameter(selectCommand, "@PaymentGatewayId", System.Data.DbType.Int32, this.PaymentGatewayId);


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
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_PaymentGateways");
                    selectQuery.Append(" WHERE  PaymentGatewayId = @PaymentGatewayId ");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {

                        database.AddInParameter(selectCommand, "@PaymentGatewayId", System.Data.DbType.Int32, this.PaymentGatewayId);


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
                    updateQuery.Append("UPDATE ac_PaymentGateways SET ");

                    updateQuery.Append("StoreId = @StoreId,Name = @Name,ClassId = @ClassId,ConfigData = @ConfigData,ReCrypt = @ReCrypt,Is3D = @Is3D,IsActive = @IsActive,IsPaymentPageHosted = @IsPaymentPageHosted,IsPaymentPageIFrame = @IsPaymentPageIFrame,IsAsynchronous = @IsAsynchronous,BlockEmails = @BlockEmails");

                    updateQuery.Append(" WHERE  PaymentGatewayId = @PaymentGatewayId ");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {


                        database.AddInParameter(updateCommand, "@PaymentGatewayId", System.Data.DbType.Int32, this.PaymentGatewayId);

                        database.AddInParameter(updateCommand, "@StoreId", System.Data.DbType.Int32, this.StoreId);

                        database.AddInParameter(updateCommand, "@Name", System.Data.DbType.String, this.Name);

                        database.AddInParameter(updateCommand, "@ClassId", System.Data.DbType.String, this.ClassId);

                        database.AddInParameter(updateCommand, "@ConfigData", System.Data.DbType.String, this.ConfigData);

                        database.AddInParameter(updateCommand, "@ReCrypt", System.Data.DbType.Boolean, this.ReCrypt);

                        database.AddInParameter(updateCommand, "@Is3D", System.Data.DbType.Boolean, this.Is3D);

                        database.AddInParameter(updateCommand, "@IsActive", System.Data.DbType.Boolean, this.IsActive);

                        database.AddInParameter(updateCommand, "@IsPaymentPageHosted", System.Data.DbType.Boolean, this.IsPaymentPageHosted);

                        database.AddInParameter(updateCommand, "@IsPaymentPageIFrame", System.Data.DbType.Boolean, this.IsPaymentPageIFrame);

                        database.AddInParameter(updateCommand, "@IsAsynchronous", System.Data.DbType.Boolean, this.IsAsynchronous);

                        database.AddInParameter(updateCommand, "@BlockEmails", System.Data.DbType.Boolean, this.BlockEmails);


                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_PaymentGateways (StoreId,Name,ClassId,ConfigData,ReCrypt,Is3D,IsActive,IsPaymentPageHosted,IsPaymentPageIFrame,IsAsynchronous,BlockEmails )");
                    insertQuery.Append(" VALUES (@StoreId,@Name,@ClassId,@ConfigData,@ReCrypt,@Is3D,@IsActive,@IsPaymentPageHosted,@IsPaymentPageIFrame,@IsAsynchronous,@BlockEmails )");


                    insertQuery.Append("; SELECT @@IDENTITY");


                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {

                        database.AddInParameter(insertCommand, "@PaymentGatewayId", System.Data.DbType.Int32, this.PaymentGatewayId);

                        database.AddInParameter(insertCommand, "@StoreId", System.Data.DbType.Int32, this.StoreId);

                        database.AddInParameter(insertCommand, "@Name", System.Data.DbType.String, this.Name);

                        database.AddInParameter(insertCommand, "@ClassId", System.Data.DbType.String, this.ClassId);

                        database.AddInParameter(insertCommand, "@ConfigData", System.Data.DbType.String, this.ConfigData);

                        database.AddInParameter(insertCommand, "@ReCrypt", System.Data.DbType.Boolean, this.ReCrypt);

                        database.AddInParameter(insertCommand, "@Is3D", System.Data.DbType.Boolean, this.Is3D);

                        database.AddInParameter(insertCommand, "@IsActive", System.Data.DbType.Boolean, this.IsActive);

                        database.AddInParameter(insertCommand, "@IsPaymentPageHosted", System.Data.DbType.Boolean, this.IsPaymentPageHosted);

                        database.AddInParameter(insertCommand, "@IsPaymentPageIFrame", System.Data.DbType.Boolean, this.IsPaymentPageIFrame);

                        database.AddInParameter(insertCommand, "@IsAsynchronous", System.Data.DbType.Boolean, this.IsAsynchronous);

                        database.AddInParameter(insertCommand, "@BlockEmails", System.Data.DbType.Boolean, this.BlockEmails);

                        //RESULT IS NUMBER OF RECORDS AFFECTED;
                        result = database.ExecuteNonQuery(insertCommand);


                        this.PaymentGatewayId = result;


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
