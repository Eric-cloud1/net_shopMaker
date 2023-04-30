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
    public partial class GatewayResponseAction : IPersistable
    {

        #region Constructors

        public GatewayResponseAction() { }

        public GatewayResponseAction(Int32 pGatewayResponseId)
        {
            this.GatewayResponseId = pGatewayResponseId;

        }

        public static string GetColumnNames(string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) prefix = string.Empty;
            else prefix = prefix + ".";
            List<string> columnNames = new List<string>();
            columnNames.Add(prefix + "GatewayResponseId");
            columnNames.Add(prefix + "Response");
            columnNames.Add(prefix + "Cancel");
            columnNames.Add(prefix + "Decline");
            columnNames.Add(prefix + "SubscriptionStatusCode");
            columnNames.Add(prefix + "Fraud");


            return string.Join(",", columnNames.ToArray());
        }

        public static void LoadDataReader(GatewayResponseAction pGatewayResponseAction, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA

            pGatewayResponseAction.GatewayResponseId = dr.GetInt32(0);

            pGatewayResponseAction.Response = dr.GetString(1);

            pGatewayResponseAction.Cancel = dr.GetBoolean(2);

            pGatewayResponseAction.Decline = dr.GetBoolean(3);


            pGatewayResponseAction.SubscriptionStatusCode = NullableData.GetByte(dr, 4);
            pGatewayResponseAction.Fraud = dr.GetBoolean(5);


            pGatewayResponseAction.IsDirty = false;
        }

        #endregion

        #region Class Data

        private bool _IsDirty;

        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }


        private Int32 _GatewayResponseId;

        [DataObjectField(true, false, false)]
        public Int32 GatewayResponseId
        {
            get { return this._GatewayResponseId; }
            set
            {
                if (this._GatewayResponseId != value)
                {
                    this._GatewayResponseId = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _Response;

        [DataObjectField(true, false, false)]
        public String Response
        {
            get { return this._Response; }
            set
            {
                if (this._Response != value)
                {
                    this._Response = value;
                    this.IsDirty = true;
                }
            }
        }


        private Boolean _Cancel;

        [DataObjectField(true, false, false)]
        public Boolean Cancel
        {
            get { return this._Cancel; }
            set
            {
                if (this._Cancel != value)
                {
                    this._Cancel = value;
                    this.IsDirty = true;
                }
            }
        }


        private Boolean _Decline;

        [DataObjectField(true, false, false)]
        public Boolean Decline
        {
            get { return this._Decline; }
            set
            {
                if (this._Decline != value)
                {
                    this._Decline = value;
                    this.IsDirty = true;
                }
            }
        }


        private Byte? _SubscriptionStatusCode;

        [DataObjectField(true, false, false)]
        public Byte? SubscriptionStatusCode
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
        public String SubscriptionStatus
        {
            get
            {
                if ((SubscriptionStatusCode == null)||(SubscriptionStatusCode==0))
                    return string.Empty;
                
                    
                this._SubscriptionStatus = OrderSubscriptionStatusCodesDataSource.Load(this.SubscriptionStatusCode.Value).SubscriptionStatus;
                return this._SubscriptionStatus;
            }
        }

        private Boolean _Fraud;

        [DataObjectField(true, false, false)]
        public Boolean Fraud
        {
            get { return this._Fraud; }
            set
            {
                if (this._Fraud != value)
                {
                    this._Fraud = value;
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
            deleteQuery.Append("DELETE FROM xm_GatewayResponseAction");
            deleteQuery.Append(" WHERE  GatewayResponseId = @GatewayResponseId  ");
            Database database = Token.Instance.Database;

            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {

                database.AddInParameter(deleteCommand, "@GatewayResponseId", System.Data.DbType.Int32, this.GatewayResponseId);

                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }

            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();



            return (recordsAffected > 0);
        }

        public virtual bool Load(Int32 pGatewayResponseId)
        {
            bool result = false;

            this.GatewayResponseId = pGatewayResponseId;


            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM xm_GatewayResponseAction");
            selectQuery.Append(" WHERE  GatewayResponseId = @GatewayResponseId  ");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());


            database.AddInParameter(selectCommand, "@GatewayResponseId", System.Data.DbType.Int32, this.GatewayResponseId);


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
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM xm_GatewayResponseAction");
                    selectQuery.Append(" WHERE  GatewayResponseId = @GatewayResponseId ");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {

                        database.AddInParameter(selectCommand, "@GatewayResponseId", System.Data.DbType.Int32, this.GatewayResponseId);


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
                    updateQuery.Append("UPDATE xm_GatewayResponseAction SET ");

                    updateQuery.Append("Response = @Response,Cancel = @Cancel,Decline = @Decline,SubscriptionStatusCode = @SubscriptionStatusCode,Fraud = @Fraud");

                    updateQuery.Append(" WHERE  GatewayResponseId = @GatewayResponseId ");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {


                        database.AddInParameter(updateCommand, "@GatewayResponseId", System.Data.DbType.Int32, this.GatewayResponseId);

                        database.AddInParameter(updateCommand, "@Response", System.Data.DbType.String, this.Response);

                        database.AddInParameter(updateCommand, "@Cancel", System.Data.DbType.Boolean, this.Cancel);

                        database.AddInParameter(updateCommand, "@Decline", System.Data.DbType.Boolean, this.Decline);

                        database.AddInParameter(updateCommand, "@SubscriptionStatusCode", System.Data.DbType.Byte, this.SubscriptionStatusCode);

                        database.AddInParameter(updateCommand, "@Fraud", System.Data.DbType.Boolean, this.Fraud);


                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO xm_GatewayResponseAction (Response,Cancel,Decline,SubscriptionStatusCode,Fraud )");
                    insertQuery.Append(" VALUES (@Response,@Cancel,@Decline,@SubscriptionStatusCode,@Fraud )");


                    insertQuery.Append("; SELECT @@IDENTITY");


                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {

                        database.AddInParameter(insertCommand, "@GatewayResponseId", System.Data.DbType.Int32, this.GatewayResponseId);

                        database.AddInParameter(insertCommand, "@Response", System.Data.DbType.String, this.Response);

                        database.AddInParameter(insertCommand, "@Cancel", System.Data.DbType.Boolean, this.Cancel);

                        database.AddInParameter(insertCommand, "@Decline", System.Data.DbType.Boolean, this.Decline);

                        database.AddInParameter(insertCommand, "@SubscriptionStatusCode", System.Data.DbType.Byte, this.SubscriptionStatusCode);

                        database.AddInParameter(insertCommand, "@Fraud", System.Data.DbType.Boolean, this.Fraud);

                        //RESULT IS NUMBER OF RECORDS AFFECTED;
                        result = database.ExecuteNonQuery(insertCommand);


                        this.GatewayResponseId = result;


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
