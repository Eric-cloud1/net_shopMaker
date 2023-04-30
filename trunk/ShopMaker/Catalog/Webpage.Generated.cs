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

namespace MakerShop.Catalog
{
    /// <summary>
    /// This class represents a Webpage object in the database.
    /// </summary>
    public partial class Webpage : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Webpage() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="webpageId">Value of WebpageId.</param>
        /// </summary>
        public Webpage(Int32 webpageId)
        {
            this.WebpageId = webpageId;
        }

        /// <summary>
        /// Returns a coma separated list of column names in this database object.
        /// </summary>
        /// <param name="prefix">Prefix to use with column names. Leave null or empty for no prefix.</param>
        /// <returns>A coman separated list of column names for this database object.</returns>
        public static string GetColumnNames(string prefix)
        {
          if (string.IsNullOrEmpty(prefix)) prefix = string.Empty;
          else prefix = prefix + ".";
          List<string> columnNames = new List<string>();
          columnNames.Add(prefix + "WebpageId");
          columnNames.Add(prefix + "StoreId");
          columnNames.Add(prefix + "Name");
          columnNames.Add(prefix + "Summary");
          columnNames.Add(prefix + "Description");
          columnNames.Add(prefix + "ThumbnailUrl");
          columnNames.Add(prefix + "ThumbnailAltText");
          columnNames.Add(prefix + "DisplayPage");
          columnNames.Add(prefix + "Theme");
          columnNames.Add(prefix + "HtmlHead");
          columnNames.Add(prefix + "VisibilityId");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given Webpage object from the given database data reader.
        /// </summary>
        /// <param name="webpage">The Webpage object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(Webpage webpage, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            webpage.WebpageId = dr.GetInt32(0);
            webpage.StoreId = dr.GetInt32(1);
            webpage.Name = NullableData.GetString(dr, 2);
            webpage.Summary = NullableData.GetString(dr, 3);
            webpage.Description = NullableData.GetString(dr, 4);
            webpage.ThumbnailUrl = NullableData.GetString(dr, 5);
            webpage.ThumbnailAltText = NullableData.GetString(dr, 6);
            webpage.DisplayPage = NullableData.GetString(dr, 7);
            webpage.Theme = NullableData.GetString(dr, 8);
            webpage.HtmlHead = NullableData.GetString(dr, 9);
            webpage.VisibilityId = dr.GetByte(10);
            webpage.IsDirty = false;
        }

#endregion

        private Int32 _WebpageId;
        private Int32 _StoreId;
        private String _Name = string.Empty;
        private String _Summary = string.Empty;
        private String _Description = string.Empty;
        private String _ThumbnailUrl = string.Empty;
        private String _ThumbnailAltText = string.Empty;
        private String _DisplayPage = string.Empty;
        private String _Theme = string.Empty;
        private String _HtmlHead = string.Empty;
        private Byte _VisibilityId;
        private bool _IsDirty;

        /// <summary>
        /// WebpageId
        /// </summary>
        [DataObjectField(true, true, false)]
        public Int32 WebpageId
        {
            get { return this._WebpageId; }
            set
            {
                if (this._WebpageId != value)
                {
                    this._WebpageId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// StoreId
        /// </summary>
        public Int32 StoreId
        {
            get { return this._StoreId; }
            set
            {
                if (this._StoreId != value)
                {
                    this._StoreId = value;
                    this.IsDirty = true;
                    this._Store = null;
                }
            }
        }

        /// <summary>
        /// Name
        /// </summary>
        public override String Name
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

        /// <summary>
        /// Summary
        /// </summary>
        public override String Summary
        {
            get { return this._Summary; }
            set
            {
                if (this._Summary != value)
                {
                    this._Summary = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Description
        /// </summary>
        public override String Description
        {
            get { return this._Description; }
            set
            {
                if (this._Description != value)
                {
                    this._Description = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// ThumbnailUrl
        /// </summary>
        public override String ThumbnailUrl
        {
            get { return this._ThumbnailUrl; }
            set
            {
                if (this._ThumbnailUrl != value)
                {
                    this._ThumbnailUrl = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// ThumbnailAltText
        /// </summary>
        public override String ThumbnailAltText
        {
            get
            {
                if (string.IsNullOrEmpty(_ThumbnailAltText)) return this.Name;
                if (_ThumbnailAltText == ".") return string.Empty;
                return _ThumbnailAltText;
            }
            set
            {
                if (this._ThumbnailAltText != value)
                {
                    this._ThumbnailAltText = value;
                    if (_ThumbnailAltText == this.Name) _ThumbnailAltText = string.Empty;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// DisplayPage
        /// </summary>
        public override String DisplayPage
        {
            get { return this._DisplayPage; }
            set
            {
                if (this._DisplayPage != value)
                {
                    this._DisplayPage = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Theme
        /// </summary>
        public override String Theme
        {
            get { return this._Theme; }
            set
            {
                if (this._Theme != value)
                {
                    this._Theme = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// HtmlHead
        /// </summary>
        public override String HtmlHead
        {
            get { return this._HtmlHead; }
            set
            {
                if (this._HtmlHead != value)
                {
                    this._HtmlHead = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// VisibilityId
        /// </summary>
        public override Byte VisibilityId
        {
            get { return this._VisibilityId; }
            set
            {
                if (this._VisibilityId != value)
                {
                    this._VisibilityId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Indicates whether this Webpage object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }

#region Parents
        private Store _Store;

        /// <summary>
        /// The Store object that this Webpage object is associated with
        /// </summary>
        public Store Store
        {
            get
            {
                if (!this.StoreLoaded)
                {
                    this._Store = StoreDataSource.Load(this.StoreId);
                }
                return this._Store;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool StoreLoaded { get { return ((this._Store != null) && (this._Store.StoreId == this.StoreId)); } }

#endregion

        /// <summary>
        /// Deletes this Webpage object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        protected bool BaseDelete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_Webpages");
            deleteQuery.Append(" WHERE WebpageId = @webpageId");
            Database database = Token.Instance.Database;

            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null);
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@WebpageId", System.Data.DbType.Int32, this.WebpageId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }

            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();

            return (recordsAffected > 0);
        }


        /// <summary>
        /// Load this Webpage object from the database for the given primary key.
        /// </summary>
        /// <param name="webpageId">Value of WebpageId of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 webpageId)
        {
            bool result = false;
            this.WebpageId = webpageId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Webpages");
            selectQuery.Append(" WHERE WebpageId = @webpageId");
            selectQuery.Append(" AND StoreId = @storeId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@webpageId", System.Data.DbType.Int32, webpageId);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                if (dr.Read())
                {
                    result = true;
                    LoadDataReader(this, dr);;
                }
                dr.Close();
            }
            return result;
        }

        /// <summary>
        /// Saves this Webpage object to the database.
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        protected SaveResult BaseSave()
        {
            if (this.IsDirty)
            {
                Database database = Token.Instance.Database;
                MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
                bool recordExists = true;
                
                //SET EMPTY STOREID TO CURRENT CONTEXT
                if (this.StoreId == 0) this.StoreId = Token.Instance.StoreId;
                if (this.WebpageId == 0) recordExists = false;

                if (recordExists) {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) As RecordCount FROM ac_Webpages");
                    selectQuery.Append(" WHERE WebpageId = @webpageId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@WebpageId", System.Data.DbType.Int32, this.WebpageId);
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
                    updateQuery.Append("UPDATE ac_Webpages SET ");
                    updateQuery.Append("StoreId = @StoreId");
                    updateQuery.Append(", Name = @Name");
                    updateQuery.Append(", Summary = @Summary");
                    updateQuery.Append(", Description = @Description");
                    updateQuery.Append(", ThumbnailUrl = @ThumbnailUrl");
                    updateQuery.Append(", ThumbnailAltText = @ThumbnailAltText");
                    updateQuery.Append(", DisplayPage = @DisplayPage");
                    updateQuery.Append(", Theme = @Theme");
                    updateQuery.Append(", HtmlHead = @HtmlHead");
                    updateQuery.Append(", VisibilityId = @VisibilityId");
                    updateQuery.Append(" WHERE WebpageId = @WebpageId");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {
                        database.AddInParameter(updateCommand, "@WebpageId", System.Data.DbType.Int32, this.WebpageId);
                        database.AddInParameter(updateCommand, "@StoreId", System.Data.DbType.Int32, this.StoreId);
                        database.AddInParameter(updateCommand, "@Name", System.Data.DbType.String, NullableData.DbNullify(this.Name));
                        database.AddInParameter(updateCommand, "@Summary", System.Data.DbType.String, NullableData.DbNullify(this.Summary));
                        database.AddInParameter(updateCommand, "@Description", System.Data.DbType.String, NullableData.DbNullify(this.Description));
                        database.AddInParameter(updateCommand, "@ThumbnailUrl", System.Data.DbType.String, NullableData.DbNullify(this.ThumbnailUrl));
                        database.AddInParameter(updateCommand, "@ThumbnailAltText", System.Data.DbType.String, NullableData.DbNullify(_ThumbnailAltText));
                        database.AddInParameter(updateCommand, "@DisplayPage", System.Data.DbType.String, NullableData.DbNullify(this.DisplayPage));
                        database.AddInParameter(updateCommand, "@Theme", System.Data.DbType.String, NullableData.DbNullify(this.Theme));
                        database.AddInParameter(updateCommand, "@HtmlHead", System.Data.DbType.String, NullableData.DbNullify(this.HtmlHead));
                        database.AddInParameter(updateCommand, "@VisibilityId", System.Data.DbType.Byte, this.VisibilityId);
                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_Webpages (StoreId, Name, Summary, Description, ThumbnailUrl, ThumbnailAltText, DisplayPage, Theme, HtmlHead, VisibilityId)");
                    insertQuery.Append(" VALUES (@StoreId, @Name, @Summary, @Description, @ThumbnailUrl, @ThumbnailAltText, @DisplayPage, @Theme, @HtmlHead, @VisibilityId)");
                    insertQuery.Append("; SELECT Scope_Identity()");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@WebpageId", System.Data.DbType.Int32, this.WebpageId);
                        database.AddInParameter(insertCommand, "@StoreId", System.Data.DbType.Int32, this.StoreId);
                        database.AddInParameter(insertCommand, "@Name", System.Data.DbType.String, NullableData.DbNullify(this.Name));
                        database.AddInParameter(insertCommand, "@Summary", System.Data.DbType.String, NullableData.DbNullify(this.Summary));
                        database.AddInParameter(insertCommand, "@Description", System.Data.DbType.String, NullableData.DbNullify(this.Description));
                        database.AddInParameter(insertCommand, "@ThumbnailUrl", System.Data.DbType.String, NullableData.DbNullify(this.ThumbnailUrl));
                        database.AddInParameter(insertCommand, "@ThumbnailAltText", System.Data.DbType.String, NullableData.DbNullify(_ThumbnailAltText));
                        database.AddInParameter(insertCommand, "@DisplayPage", System.Data.DbType.String, NullableData.DbNullify(this.DisplayPage));
                        database.AddInParameter(insertCommand, "@Theme", System.Data.DbType.String, NullableData.DbNullify(this.Theme));
                        database.AddInParameter(insertCommand, "@HtmlHead", System.Data.DbType.String, NullableData.DbNullify(this.HtmlHead));
                        database.AddInParameter(insertCommand, "@VisibilityId", System.Data.DbType.Byte, this.VisibilityId);
                        //RESULT IS NEW IDENTITY;
                        result = AlwaysConvert.ToInt(database.ExecuteScalar(insertCommand));
                        this._WebpageId = result;
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

     }
}