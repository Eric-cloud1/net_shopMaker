//Generated by PersistableBaseGenerator

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
    public partial class PaymentGatewayAllocation : IPersistable
    {

        #region Constructors

        public PaymentGatewayAllocation() { }

        public PaymentGatewayAllocation(Int32 pPaymentGatewayTemplateId, Int32 pPaymentMethodId)
        {
            this.PaymentGatewayTemplateId = pPaymentGatewayTemplateId;
            this.PaymentMethodId = pPaymentMethodId;

        }

        public static string GetColumnNames(string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) prefix = string.Empty;
            else prefix = prefix + ".";
            List<string> columnNames = new List<string>();
            columnNames.Add(prefix + "PaymentGatewayTemplateId");
            columnNames.Add(prefix + "PaymentMethodId");
            columnNames.Add(prefix + "PriorityPercent");


            return string.Join(",", columnNames.ToArray());
        }

        public static void LoadDataReader(PaymentGatewayAllocation pPaymentGatewayAllocations, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA

            pPaymentGatewayAllocations.PaymentGatewayTemplateId = dr.GetInt32(0);

            pPaymentGatewayAllocations.PaymentMethodId = dr.GetInt32(1);

            pPaymentGatewayAllocations.PriorityPercent = dr.GetByte(2);


            pPaymentGatewayAllocations.IsDirty = false;
        }

        #endregion

        #region Class Data

        private bool _IsDirty;

        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }


        private Int32 _PaymentGatewayTemplateId;

        [DataObjectField(true, false, false)]
        public Int32 PaymentGatewayTemplateId
        {
            get { return this._PaymentGatewayTemplateId; }
            set
            {
                if (this._PaymentGatewayTemplateId != value)
                {
                    this._PaymentGatewayTemplateId = value;
                    this.IsDirty = true;
                }
            }
        }


        private Int32 _PaymentMethodId;

        [DataObjectField(true, false, false)]
        public Int32 PaymentMethodId
        {
            get { return this._PaymentMethodId; }
            set
            {
                if (this._PaymentMethodId != value)
                {
                    this._PaymentMethodId = value;
                    this.IsDirty = true;
                }
            }
        }


       

        private Byte _PriorityPercent;

        [DataObjectField(true, false, false)]
        public Byte PriorityPercent
        {
            get { return this._PriorityPercent; }
            set
            {
                if (this._PriorityPercent != value)
                {
                    this._PriorityPercent = value;
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
            deleteQuery.Append("DELETE FROM xm_PaymentGatewayAllocations");
            deleteQuery.Append(" WHERE  PaymentGatewayTemplateId = @PaymentGatewayTemplateId AND  PaymentMethodId = @PaymentMethodId  ");
            Database database = Token.Instance.Database;
            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {

                database.AddInParameter(deleteCommand, "@PaymentGatewayTemplateId", System.Data.DbType.Int32, this.PaymentGatewayTemplateId);

                database.AddInParameter(deleteCommand, "@PaymentMethodId", System.Data.DbType.Int32, this.PaymentMethodId);

                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }

            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            return (recordsAffected > 0);
        }

        public virtual bool DeleteAll(string paymentInstrumentId)
        {
            int recordsAffected = 0;
            string deleteQuery =string.Format(@"DELETE FROM xm_PaymentGatewayAllocations
             WHERE  PaymentGatewayTemplateId = @PaymentGatewayTemplateId AND PaymentMethodId 
in (select PaymentMethodId  
from dbo.ac_PaymentMethods
where PaymentInstrumentId = {0}) ", paymentInstrumentId);

            Database database = Token.Instance.Database;
            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery))
            {
                database.AddInParameter(deleteCommand, "@PaymentGatewayTemplateId", System.Data.DbType.Int32, this.PaymentGatewayTemplateId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }

            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            return (recordsAffected > 0);
        }

        public virtual bool Load(Int32 pPaymentGatewayTemplateId, Int32 pPaymentMethodId)
        {
            bool result = false;

            this.PaymentGatewayTemplateId = pPaymentGatewayTemplateId;

            this.PaymentMethodId = pPaymentMethodId;


            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM xm_PaymentGatewayAllocations");
            selectQuery.Append(" WHERE  PaymentGatewayTemplateId = @PaymentGatewayTemplateId AND  PaymentMethodId = @PaymentMethodId  ");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());


            database.AddInParameter(selectCommand, "@PaymentGatewayTemplateId", System.Data.DbType.Int32, this.PaymentGatewayTemplateId);

            database.AddInParameter(selectCommand, "@PaymentMethodId", System.Data.DbType.Int32, this.PaymentMethodId);


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
                    selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) As RecordCount FROM xm_PaymentGatewayAllocations");
                    selectQuery.Append(" WHERE  PaymentGatewayTemplateId = @PaymentGatewayTemplateId AND  PaymentMethodId = @PaymentMethodId ");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {

                        database.AddInParameter(selectCommand, "@PaymentGatewayTemplateId", System.Data.DbType.Int32, this.PaymentGatewayTemplateId);

                        database.AddInParameter(selectCommand, "@PaymentMethodId", System.Data.DbType.Int32, this.PaymentMethodId);


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
                    updateQuery.Append("UPDATE xm_PaymentGatewayAllocations SET ");

                    updateQuery.Append("PriorityPercent = @PriorityPercent");

                    updateQuery.Append(" WHERE  PaymentGatewayTemplateId = @PaymentGatewayTemplateId AND  PaymentMethodId = @PaymentMethodId ");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {


                        database.AddInParameter(updateCommand, "@PaymentGatewayTemplateId", System.Data.DbType.Int32, this.PaymentGatewayTemplateId);

                        database.AddInParameter(updateCommand, "@PaymentMethodId", System.Data.DbType.Int32, this.PaymentMethodId);

                        database.AddInParameter(updateCommand, "@PriorityPercent", System.Data.DbType.Byte, this.PriorityPercent);


                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO xm_PaymentGatewayAllocations (PaymentGatewayTemplateId,PaymentMethodId,PriorityPercent )");
                    insertQuery.Append(" VALUES (@PaymentGatewayTemplateId,@PaymentMethodId,@PriorityPercent )");



                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {

                        database.AddInParameter(insertCommand, "@PaymentGatewayTemplateId", System.Data.DbType.Int32, this.PaymentGatewayTemplateId);

                        database.AddInParameter(insertCommand, "@PaymentMethodId", System.Data.DbType.Int32, this.PaymentMethodId);

                        database.AddInParameter(insertCommand, "@PriorityPercent", System.Data.DbType.Byte, this.PriorityPercent);

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
