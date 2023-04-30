using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;

namespace MakerShop.Affiliates
{
    public partial class AffiliateCounts : IPersistable
    {

        #region Constructors

        public AffiliateCounts() { }

        public AffiliateCounts(Int32 pAffiliateCountId)
        {
            this.AffiliateCountId = pAffiliateCountId;

        }

        public static string GetColumnNames(string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) prefix = string.Empty;
            else prefix = prefix + ".";
            List<string> columnNames = new List<string>();
            columnNames.Add(prefix + "AffiliateCountId");
            columnNames.Add(prefix + "AffiliateCode");
            columnNames.Add(prefix + "Date");
            columnNames.Add(prefix + "Hour");
            columnNames.Add(prefix + "CreateDate");
            columnNames.Add(prefix + "SaleCount");


            return string.Join(",", columnNames.ToArray());
        }

        public static void LoadDataReader(AffiliateCounts pAffiliateCounts, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA

            pAffiliateCounts.AffiliateCountId = dr.GetInt32(0);


            pAffiliateCounts.AffiliateCode = dr.GetString(2);

            pAffiliateCounts.Date = LocaleHelper.ToLocalTime(dr.GetDateTime(3));

            pAffiliateCounts.Hour = dr.GetByte(4);

            pAffiliateCounts.CreateDate = LocaleHelper.ToLocalTime(dr.GetDateTime(5));

            pAffiliateCounts.SaleCount = dr.GetInt32(6);


            pAffiliateCounts.IsDirty = false;
        }

        #endregion

        #region Class Data

        private bool _IsDirty;

        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }


        private Int32 _AffiliateCountId;

        [DataObjectField(true, false, false)]
        public Int32 AffiliateCountId
        {
            get { return this._AffiliateCountId; }
            set
            {
                if (this._AffiliateCountId != value)
                {
                    this._AffiliateCountId = value;
                    this.IsDirty = true;
                }
            }
        }


 


        private String _AffiliateCode;

        [DataObjectField(true, false, false)]
        public String AffiliateCode
        {
            get { return this._AffiliateCode; }
            set
            {
                if (this._AffiliateCode != value)
                {
                    this._AffiliateCode = value;
                    this.IsDirty = true;
                }
            }
        }


        private DateTime _Date;

        [DataObjectField(true, false, false)]
        public DateTime Date
        {
            get { return this._Date; }
            set
            {
                if (this._Date != value)
                {
                    this._Date = value;
                    this.IsDirty = true;
                }
            }
        }


        private Int32 _Hour;

        [DataObjectField(true, false, false)]
        public Int32 Hour
        {
            get { return this._Hour; }
            set
            {
                if (this._Hour != value)
                {
                    this._Hour = value;
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


        private Int32 _SaleCount;

        [DataObjectField(true, false, false)]
        public Int32 SaleCount
        {
            get { return this._SaleCount; }
            set
            {
                if (this._SaleCount != value)
                {
                    this._SaleCount = value;
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
            deleteQuery.Append("DELETE FROM xm_AffiliateCounts");
            deleteQuery.Append(" WHERE  AffiliateCode = @AffiliateCode AND Date = @Date AND Hour = @Hour ");
            Database database = Token.Instance.Database;

            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null);
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {

                database.AddInParameter(deleteCommand, "@AffiliateCode", System.Data.DbType.String, this.AffiliateCode);

                database.AddInParameter(deleteCommand, "@Date", System.Data.DbType.DateTime, this.Date);

                database.AddInParameter(deleteCommand, "@Hour", System.Data.DbType.Byte, this.Hour);

                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            return (recordsAffected > 0);
        }

        public virtual bool Load(Int32 pAffiliateCountId)
        {
            bool result = false;

            this.AffiliateCountId = pAffiliateCountId;


            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM xm_AffiliateCounts");
            selectQuery.Append(" WHERE  AffiliateCountId = @AffiliateCountId  ");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());


            database.AddInParameter(selectCommand, "@AffiliateCountId", System.Data.DbType.Int32, this.AffiliateCountId);


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
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM xm_AffiliateCounts");
                    selectQuery.Append(" WHERE  AffiliateCode = @AffiliateCode AND Date = @Date AND Hour = @Hour ");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {


                        database.AddInParameter(selectCommand, "@AffiliateCode", System.Data.DbType.String, this.AffiliateCode);

                        database.AddInParameter(selectCommand, "@Date", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(this.Date));

                        database.AddInParameter(selectCommand, "@Hour", System.Data.DbType.Byte, this.Hour);

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
                    updateQuery.Append("UPDATE xm_AffiliateCounts SET ");

                    updateQuery.Append(" CreateDate = @CreateDate,SaleCount = @SaleCount");

                    updateQuery.Append(" WHERE  AffiliateCode = @AffiliateCode AND Date = @Date AND Hour = @Hour ");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {

                        database.AddInParameter(updateCommand, "@AffiliateCode", System.Data.DbType.String, this.AffiliateCode);

                        database.AddInParameter(updateCommand, "@Date", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(this.Date));

                        database.AddInParameter(updateCommand, "@Hour", System.Data.DbType.Byte, this.Hour);

                        database.AddInParameter(updateCommand, "@CreateDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(this.CreateDate));

                        database.AddInParameter(updateCommand, "@SaleCount", System.Data.DbType.Int32, this.SaleCount);


                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO xm_AffiliateCounts (AffiliateCode,Date,Hour,CreateDate,SaleCount )");
                    insertQuery.Append(" VALUES (@AffiliateCode,@Date,@Hour,@CreateDate,@SaleCount )");


                    insertQuery.Append("; SELECT @@IDENTITY");


                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {

                        database.AddInParameter(insertCommand, "@AffiliateCode", System.Data.DbType.String, this.AffiliateCode);

                        database.AddInParameter(insertCommand, "@Date", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(this.Date));

                        database.AddInParameter(insertCommand, "@Hour", System.Data.DbType.Byte, this.Hour);

                        database.AddInParameter(insertCommand, "@CreateDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(this.CreateDate));

                        database.AddInParameter(insertCommand, "@SaleCount", System.Data.DbType.Int32, this.SaleCount);

                        //RESULT IS NUMBER OF RECORDS AFFECTED;
                        result = database.ExecuteNonQuery(insertCommand);


                        this.AffiliateCountId = result;


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
