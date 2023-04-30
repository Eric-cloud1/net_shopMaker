using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Marketing;
using MakerShop.Payments;
using MakerShop.Products;
using MakerShop.Shipping;
using MakerShop.Stores;
using MakerShop.Taxes;
using MakerShop.Users;
using MakerShop.Utility;

namespace MakerShop.Users
{
    public partial class OptIn : IPersistable
    {

        #region Constructors

        public OptIn() { }

        public OptIn(Int32 pUserId)
        {
            this.UserId = pUserId;

        }

        public static string GetColumnNames(string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) prefix = string.Empty;
            else prefix = prefix + ".";
            List<string> columnNames = new List<string>();
            columnNames.Add(prefix + "UserId");
            columnNames.Add(prefix + "OptInDate");
            columnNames.Add(prefix + "OptOutDate");
            columnNames.Add(prefix + "Message");


            return string.Join(",", columnNames.ToArray());
        }

        public static void LoadDataReader(OptIn pOptIn, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            pOptIn.UserId = dr.GetInt32(0);
            pOptIn.OptInDate = LocaleHelper.ToLocalTime(dr.GetDateTime(1));
            pOptIn.OptOutDate = (NullableData.GetDateTime(dr, 2) != null) ? LocaleHelper.ToLocalTime(NullableData.GetDateTime(dr, 2)) : NullableData.GetDateTime(dr, 2);
            pOptIn.Message = NullableData.GetString(dr, 3);
            pOptIn.IsDirty = false;
        }

        #endregion

        #region Class Data

        private bool _IsDirty;

        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
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


        private DateTime _OptInDate;

        [DataObjectField(true, false, false)]
        public DateTime OptInDate
        {
            get { return this._OptInDate; }
            set
            {
                if (this._OptInDate != value)
                {
                    this._OptInDate = value;
                    this.IsDirty = true;
                }
            }
        }


        private DateTime? _OptOutDate;

        [DataObjectField(true, false, false)]
        public DateTime? OptOutDate
        {
            get { return this._OptOutDate; }
            set
            {
                if (this._OptOutDate != value)
                {
                    this._OptOutDate = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _Message;
        [DataObjectField(true, false, false)]
        public String Message
        {
            get { return this._Message; }
            set
            {
                if (this._Message != value)
                {
                    this._Message = value;
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
            deleteQuery.Append("DELETE FROM xm_OptIn");
            deleteQuery.Append(" WHERE  UserId = @UserId  ");
            Database database = Token.Instance.Database;
            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null);
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {

                database.AddInParameter(deleteCommand, "@UserId", System.Data.DbType.Int32, this.UserId);

                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            return (recordsAffected > 0);
        }

        public virtual bool Load(Int32 pUserId)
        {
            bool result = false;

            this.UserId = pUserId;


            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM xm_OptIn");
            selectQuery.Append(" WHERE  UserId = @UserId  ");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());


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
                AuditEventDataSource.AuditInfoBegin(null);
                Database database = Token.Instance.Database;
                bool recordExists = true;

                if (recordExists)
                {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM xm_OptIn");
                    selectQuery.Append(" WHERE  UserId = @UserId ");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {

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
                    updateQuery.Append("UPDATE xm_OptIn SET ");

                    updateQuery.Append("OptInDate = @OptInDate,OptOutDate = @OptOutDate,Message = @Message");

                    updateQuery.Append(" WHERE  UserId = @UserId ");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {


                        database.AddInParameter(updateCommand, "@UserId", System.Data.DbType.Int32, this.UserId);

                        database.AddInParameter(updateCommand, "@OptInDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(this.OptInDate));

                        database.AddInParameter(updateCommand, "@OptOutDate", System.Data.DbType.DateTime, this.OptOutDate != null ? LocaleHelper.FromLocalTime(this.OptOutDate.Value):OptOutDate );

                        database.AddInParameter(updateCommand, "@Message", System.Data.DbType.String, this.Message);


                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO xm_OptIn (UserId,OptInDate,OptOutDate,Message )");
                    insertQuery.Append(" VALUES (@UserId,@OptInDate,@OptOutDate,@Message )");


                    insertQuery.Append("; SELECT @@IDENTITY");


                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {

                        database.AddInParameter(insertCommand, "@UserId", System.Data.DbType.Int32, this.UserId);

                        database.AddInParameter(insertCommand, "@OptInDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(this.OptInDate));

                        database.AddInParameter(insertCommand, "@OptOutDate", System.Data.DbType.DateTime, this.OptOutDate != null ? LocaleHelper.FromLocalTime(this.OptOutDate.Value) : OptOutDate);

                        database.AddInParameter(insertCommand, "@Message", System.Data.DbType.String, this.Message);

                        //RESULT IS NUMBER OF RECORDS AFFECTED;
                        result = database.ExecuteNonQuery(insertCommand);


                        this.UserId = result;


                    }
                }

                AuditEventDataSource.AuditInfoEnd();
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
