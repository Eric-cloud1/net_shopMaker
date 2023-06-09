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
    public partial class PaymentGatewayTemplate_Affiliate : IPersistable
    {

        #region Constructors

        public PaymentGatewayTemplate_Affiliate() { }

        public PaymentGatewayTemplate_Affiliate(Int32 pAffiliateId)
        {
            this.AffiliateId = pAffiliateId;

        }

        public static string GetColumnNames(string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) prefix = string.Empty;
            else prefix = prefix + ".";
            List<string> columnNames = new List<string>();
            columnNames.Add(prefix + "AffiliateId");
            columnNames.Add(prefix + "PaymentGatewayTemplateId");
            columnNames.Add(prefix + "CreateDate");
            columnNames.Add(prefix + "CreateUser");
            columnNames.Add(prefix + "ChangeDate");
            columnNames.Add(prefix + "ChangeUser");


            return string.Join(",", columnNames.ToArray());
        }

        public static void LoadDataReader(PaymentGatewayTemplate_Affiliate pPaymentGatewayTemplate_Affiliate, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA

            pPaymentGatewayTemplate_Affiliate.AffiliateId = dr.GetInt32(0);

            pPaymentGatewayTemplate_Affiliate.PaymentGatewayTemplateId = dr.GetInt32(1);

            pPaymentGatewayTemplate_Affiliate.CreateDate = LocaleHelper.ToLocalTime(dr.GetDateTime(2));

            pPaymentGatewayTemplate_Affiliate.CreateUser = dr.GetString(3);

            pPaymentGatewayTemplate_Affiliate.ChangeDate = LocaleHelper.ToLocalTime(dr.GetDateTime(4));

            pPaymentGatewayTemplate_Affiliate.ChangeUser = dr.GetString(5);


            pPaymentGatewayTemplate_Affiliate.IsDirty = false;
        }

        #endregion

        #region Class Data

        private bool _IsDirty;

        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
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
            deleteQuery.Append("DELETE FROM xm_PaymentGatewayTemplates_Affiliate");
            deleteQuery.Append(" WHERE  AffiliateId = @AffiliateId  ");
            Database database = Token.Instance.Database;

            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null);
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {

                database.AddInParameter(deleteCommand, "@AffiliateId", System.Data.DbType.Int32, this.AffiliateId);

                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }

            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            return (recordsAffected > 0);
        }

        public virtual bool Load(Int32 pAffiliateId)
        {
            bool result = false;

            this.AffiliateId = pAffiliateId;


            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM xm_PaymentGatewayTemplates_Affiliate");
            selectQuery.Append(" WHERE  AffiliateId = @AffiliateId  ");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());


            database.AddInParameter(selectCommand, "@AffiliateId", System.Data.DbType.Int32, this.AffiliateId);


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
                this.CreateDate = DateTime.UtcNow;
                this.CreateUser = Token.Instance.User.UserName;
                this.ChangeDate = DateTime.UtcNow;
                this.ChangeUser = Token.Instance.User.UserName;

                Database database = Token.Instance.Database;
                bool recordExists = true;

                if (recordExists)
                {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) As RecordCount FROM xm_PaymentGatewayTemplates_Affiliate");
                    selectQuery.Append(" WHERE  AffiliateId = @AffiliateId ");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {

                        database.AddInParameter(selectCommand, "@AffiliateId", System.Data.DbType.Int32, this.AffiliateId);


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
                    updateQuery.Append("UPDATE xm_PaymentGatewayTemplates_Affiliate SET ");

                    updateQuery.Append("PaymentGatewayTemplateId = @PaymentGatewayTemplateId,ChangeDate = @ChangeDate,ChangeUser = @ChangeUser");

                    updateQuery.Append(" WHERE  AffiliateId = @AffiliateId ");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {


                        database.AddInParameter(updateCommand, "@AffiliateId", System.Data.DbType.Int32, this.AffiliateId);

                        database.AddInParameter(updateCommand, "@PaymentGatewayTemplateId", System.Data.DbType.Int32, this.PaymentGatewayTemplateId);

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
                    insertQuery.Append("INSERT INTO xm_PaymentGatewayTemplates_Affiliate (AffiliateId,PaymentGatewayTemplateId,CreateDate,CreateUser,ChangeDate,ChangeUser )");
                    insertQuery.Append(" VALUES (@AffiliateId,@PaymentGatewayTemplateId,@CreateDate,@CreateUser,@ChangeDate,@ChangeUser )");


                    insertQuery.Append("; SELECT @@IDENTITY");


                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {

                        database.AddInParameter(insertCommand, "@AffiliateId", System.Data.DbType.Int32, this.AffiliateId);

                        database.AddInParameter(insertCommand, "@PaymentGatewayTemplateId", System.Data.DbType.Int32, this.PaymentGatewayTemplateId);

                        database.AddInParameter(insertCommand, "@CreateDate", System.Data.DbType.DateTime, this.CreateDate);

                        database.AddInParameter(insertCommand, "@CreateUser", System.Data.DbType.String, this.CreateUser);

                        database.AddInParameter(insertCommand, "@ChangeDate", System.Data.DbType.DateTime, this.ChangeDate);

                        database.AddInParameter(insertCommand, "@ChangeUser", System.Data.DbType.String, this.ChangeUser);

                        //RESULT IS NUMBER OF RECORDS AFFECTED;
                        result = database.ExecuteNonQuery(insertCommand);


                        this.AffiliateId = result;


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
