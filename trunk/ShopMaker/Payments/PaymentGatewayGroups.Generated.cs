using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Payments;
using MakerShop.Stores;
using MakerShop.Utility;


namespace MakerShop.Payments
{
    public partial class PaymentGatewayGroups : IPersistable
    {

        #region Constructors

        public PaymentGatewayGroups() { }

        public PaymentGatewayGroups(Int32 pPaymentGatewayGroupId)
        {
            this.PaymentGatewayGroupId = pPaymentGatewayGroupId;

        }

        public static string GetColumnNames(string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) prefix = string.Empty;
            else prefix = prefix + ".";
            List<string> columnNames = new List<string>();
            columnNames.Add(prefix + "PaymentGatewayGroupId");
            columnNames.Add(prefix + "PaymentGatewayGroup");


            return string.Join(",", columnNames.ToArray());
        }

        public static void LoadDataReader(PaymentGatewayGroups pPaymentGatewayGroups, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            pPaymentGatewayGroups.PaymentGatewayGroupId = dr.GetInt32(0);
            pPaymentGatewayGroups.PaymentGatewayGroup = dr.GetString(1);
            pPaymentGatewayGroups.IsDirty = false;
        }

        #endregion

        #region Class Data

        private bool _IsDirty;

        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }


        private Int32 _PaymentGatewayGroupId;

        [DataObjectField(true, false, false)]
        public Int32 PaymentGatewayGroupId
        {
            get { return this._PaymentGatewayGroupId; }
            set
            {
                if (this._PaymentGatewayGroupId != value)
                {
                    this._PaymentGatewayGroupId = value;
                    this.IsDirty = true;
                }
            }
        }

        private String _PaymentGatewayGroup;

        [DataObjectField(true, false, false)]
        public String PaymentGatewayGroup
        {
            get { return this._PaymentGatewayGroup; }
            set
            {
                if (this._PaymentGatewayGroup != value)
                {
                    this._PaymentGatewayGroup = value;
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
            deleteQuery.Append("DELETE FROM xm_PaymentGatewayGroups");
            deleteQuery.Append(" WHERE  PaymentGatewayGroupId = @PaymentGatewayGroupId  ");
            Database database = Token.Instance.Database;

            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {

                database.AddInParameter(deleteCommand, "@PaymentGatewayGroupId", System.Data.DbType.Int32, this.PaymentGatewayGroupId);

                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }

            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            return (recordsAffected > 0);
        }

        public virtual bool Load(Int32 pPaymentGatewayGroupId)
        {
            bool result = false;

            this.PaymentGatewayGroupId = pPaymentGatewayGroupId;


            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM xm_PaymentGatewayGroups");
            selectQuery.Append(" WHERE  PaymentGatewayGroupId = @PaymentGatewayGroupId  ");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());


            database.AddInParameter(selectCommand, "@PaymentGatewayGroupId", System.Data.DbType.Int32, this.PaymentGatewayGroupId);


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
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM xm_PaymentGatewayGroups");
                    selectQuery.Append(" WHERE  PaymentGatewayGroupId = @PaymentGatewayGroupId ");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {

                        database.AddInParameter(selectCommand, "@PaymentGatewayGroupId", System.Data.DbType.Int32, this.PaymentGatewayGroupId);


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
                    updateQuery.Append("UPDATE xm_PaymentGatewayGroups SET ");

                    updateQuery.Append(" PaymentGatewayGroup = @PaymentGatewayGroup");

                    updateQuery.Append(" WHERE  PaymentGatewayGroupId = @PaymentGatewayGroupId ");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {


                        database.AddInParameter(updateCommand, "@PaymentGatewayGroupId", System.Data.DbType.Int32, this.PaymentGatewayGroupId);
                        database.AddInParameter(updateCommand, "@PaymentGatewayGroup", System.Data.DbType.String, this.PaymentGatewayGroup);


                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO xm_PaymentGatewayGroups (PaymentGatewayGroup )");
                    insertQuery.Append(" VALUES (@PaymentGatewayGroup )");


                    insertQuery.Append("; SELECT @@IDENTITY");


                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {

                        database.AddInParameter(insertCommand, "@PaymentGatewayGroupId", System.Data.DbType.Int32, this.PaymentGatewayGroupId);

                        database.AddInParameter(insertCommand, "@PaymentGatewayGroup", System.Data.DbType.String, this.PaymentGatewayGroup);

                        //RESULT IS NUMBER OF RECORDS AFFECTED;
                        result = database.ExecuteNonQuery(insertCommand);


                        this.PaymentGatewayGroupId = result;


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

        public virtual SaveResult DeleteGroupsPaymentGateways(int paymentGatewayGroupId)
        {
                Database database = Token.Instance.Database;
                bool recordExists = false;
          
                if (!recordExists)
                {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("DELETE FROM xm_PaymentGatewayGroupsPaymentGateways");
                    selectQuery.Append(" WHERE  PaymentGatewayGroupId = @PaymentGatewayGroupId ");
                    MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
                    using (DbCommand deleteCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(deleteCommand, "@PaymentGatewayGroupId", System.Data.DbType.Int32, paymentGatewayGroupId);

                        if ((int)database.ExecuteNonQuery(deleteCommand) > 0)
                        {
                            recordExists = true;
                        }
                    }

                    MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
                }
                return SaveResult.NotDirty;
        }

        public virtual SaveResult SaveGroupsPaymentGateways(int paymentGatewayGroupId, int paymentGatewayId)
        {
            if (this.IsDirty)
            {
                Database database = Token.Instance.Database;
                MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
                bool recordExists = false;
                int result = 0;

                if (!recordExists)
                {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(1) FROM xm_PaymentGatewayGroupsPaymentGateways");
                    selectQuery.Append(" WHERE  PaymentGatewayGroupId = @PaymentGatewayGroupId AND PaymentGatewayId = @PaymentGatewayId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@PaymentGatewayGroupId", System.Data.DbType.Int32, paymentGatewayGroupId);
                        database.AddInParameter(selectCommand, "@PaymentGatewayId", System.Data.DbType.Int32, paymentGatewayId);
                        
                        if ((int)database.ExecuteScalar(selectCommand) > 0)
                        {
                            recordExists = true;
                        }
                    }
                }


                if (!recordExists)
                {                   
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO xm_PaymentGatewayGroupsPaymentGateways (PaymentGatewayGroupId, PaymentGatewayId )");
                    insertQuery.Append(" VALUES (@PaymentGatewayGroupId, @PaymentGatewayId)");

                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@PaymentGatewayGroupId", System.Data.DbType.Int32, paymentGatewayGroupId);
                        database.AddInParameter(insertCommand, "@PaymentGatewayId", System.Data.DbType.Int32, paymentGatewayId);
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
