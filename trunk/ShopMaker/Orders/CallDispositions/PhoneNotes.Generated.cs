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
    public partial class PhoneNotes : IPersistable
    {

        #region Constructors

        public PhoneNotes() { }

        public PhoneNotes(Int32 pPhoneNoteId)
        {
            this.PhoneNoteId = pPhoneNoteId;

        }

        public static string GetColumnNames(string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) prefix = string.Empty;
            else prefix = prefix + ".";
            List<string> columnNames = new List<string>();
            columnNames.Add(prefix + "PhoneNoteId");
            columnNames.Add(prefix + "OrderId");
            columnNames.Add(prefix + "UserId");
            columnNames.Add(prefix + "CreatedDate");
            columnNames.Add(prefix + "Comment");
            columnNames.Add(prefix + "NoteTypeId");


            return string.Join(",", columnNames.ToArray());
        }

        public static void LoadDataReader(PhoneNotes pPhoneNotes, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA

            pPhoneNotes.PhoneNoteId = dr.GetInt32(0);

            pPhoneNotes.OrderId = dr.GetInt32(1);


            pPhoneNotes.UserId = NullableData.GetInt32(dr, 2);
            pPhoneNotes.CreatedDate = dr.GetDateTime(3);

            pPhoneNotes.Comment = dr.GetString(4);

            pPhoneNotes.NoteTypeId = dr.GetByte(5);


            pPhoneNotes.IsDirty = false;
        }

        #endregion

        #region Class Data

        private bool _IsDirty;

        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }


        private Int32 _PhoneNoteId;

        [DataObjectField(true, false, false)]
        public Int32 PhoneNoteId
        {
            get { return this._PhoneNoteId; }
            set
            {
                if (this._PhoneNoteId != value)
                {
                    this._PhoneNoteId = value;
                    this.IsDirty = true;
                }
            }
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


        private DateTime _CreatedDate;

        [DataObjectField(true, false, false)]
        public DateTime CreatedDate
        {
            get { return this._CreatedDate; }
            set
            {
                if (this._CreatedDate != value)
                {
                    this._CreatedDate = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _Comment;

        [DataObjectField(true, false, false)]
        public String Comment
        {
            get { return this._Comment; }
            set
            {
                if (this._Comment != value)
                {
                    this._Comment = value;
                    this.IsDirty = true;
                }
            }
        }


        private Byte _NoteTypeId;

        [DataObjectField(true, false, false)]
        public Byte NoteTypeId
        {
            get { return this._NoteTypeId; }
            set
            {
                if (this._NoteTypeId != value)
                {
                    this._NoteTypeId = value;
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
            deleteQuery.Append("DELETE FROM ac_PhoneNotes");
            deleteQuery.Append(" WHERE  PhoneNoteId = @PhoneNoteId  ");
            Database database = Token.Instance.Database;
            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null);
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {

                database.AddInParameter(deleteCommand, "@PhoneNoteId", System.Data.DbType.Int32, this.PhoneNoteId);

                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            return (recordsAffected > 0);
        }

        public virtual bool Load(Int32 pPhoneNoteId)
        {
            bool result = false;

            this.PhoneNoteId = pPhoneNoteId;


            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_PhoneNotes");
            selectQuery.Append(" WHERE  PhoneNoteId = @PhoneNoteId  ");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());


            database.AddInParameter(selectCommand, "@PhoneNoteId", System.Data.DbType.Int32, this.PhoneNoteId);


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
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_PhoneNotes");
                    selectQuery.Append(" WHERE  PhoneNoteId = @PhoneNoteId ");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {

                        database.AddInParameter(selectCommand, "@PhoneNoteId", System.Data.DbType.Int32, this.PhoneNoteId);


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
                    updateQuery.Append("UPDATE ac_PhoneNotes SET ");

                    updateQuery.Append("OrderId = @OrderId,UserId = @UserId,CreatedDate = @CreatedDate,Comment = @Comment,NoteTypeId = @NoteTypeId");

                    updateQuery.Append(" WHERE  PhoneNoteId = @PhoneNoteId ");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {


                        database.AddInParameter(updateCommand, "@PhoneNoteId", System.Data.DbType.Int32, this.PhoneNoteId);

                        database.AddInParameter(updateCommand, "@OrderId", System.Data.DbType.Int32, this.OrderId);

                        database.AddInParameter(updateCommand, "@UserId", System.Data.DbType.Int32, this.UserId);

                        database.AddInParameter(updateCommand, "@CreatedDate", System.Data.DbType.DateTime, this.CreatedDate);

                        database.AddInParameter(updateCommand, "@Comment", System.Data.DbType.String, this.Comment);

                        database.AddInParameter(updateCommand, "@NoteTypeId", System.Data.DbType.Byte, this.NoteTypeId);


                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_PhoneNotes (OrderId,UserId,CreatedDate,Comment,NoteTypeId )");
                    insertQuery.Append(" VALUES (@OrderId,@UserId,@CreatedDate,@Comment,@NoteTypeId )");


                    insertQuery.Append("; SELECT @@IDENTITY");


                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {

                        database.AddInParameter(insertCommand, "@PhoneNoteId", System.Data.DbType.Int32, this.PhoneNoteId);

                        database.AddInParameter(insertCommand, "@OrderId", System.Data.DbType.Int32, this.OrderId);

                        database.AddInParameter(insertCommand, "@UserId", System.Data.DbType.Int32, this.UserId);

                        database.AddInParameter(insertCommand, "@CreatedDate", System.Data.DbType.DateTime, this.CreatedDate);

                        database.AddInParameter(insertCommand, "@Comment", System.Data.DbType.String, this.Comment);

                        database.AddInParameter(insertCommand, "@NoteTypeId", System.Data.DbType.Byte, this.NoteTypeId);

                        //RESULT IS NUMBER OF RECORDS AFFECTED;
                        result = database.ExecuteNonQuery(insertCommand);


                        this.PhoneNoteId = result;


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
