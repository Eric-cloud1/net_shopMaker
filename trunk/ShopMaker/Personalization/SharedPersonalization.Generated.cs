//Generated by PersistableBaseGenerator

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Personalization;
using MakerShop.Utility;

namespace MakerShop.Personalization
{
    /// <summary>
    /// This class represents a SharedPersonalization object in the database.
    /// </summary>
    public partial class SharedPersonalization : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SharedPersonalization() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="personalizationPathId">Value of PersonalizationPathId.</param>
        /// </summary>
        public SharedPersonalization(Int32 personalizationPathId)
        {
            this.PersonalizationPathId = personalizationPathId;
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
          columnNames.Add(prefix + "PersonalizationPathId");
          columnNames.Add(prefix + "PageSettings");
          columnNames.Add(prefix + "Theme");
          columnNames.Add(prefix + "MasterPageFile");
          columnNames.Add(prefix + "LastUpdatedDate");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given SharedPersonalization object from the given database data reader.
        /// </summary>
        /// <param name="sharedPersonalization">The SharedPersonalization object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(SharedPersonalization sharedPersonalization, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            sharedPersonalization.PersonalizationPathId = dr.GetInt32(0);
            sharedPersonalization.PageSettings = NullableData.GetBytes(dr, 1);
            sharedPersonalization.Theme = NullableData.GetString(dr, 2);
            sharedPersonalization.MasterPageFile = NullableData.GetString(dr, 3);
            sharedPersonalization.LastUpdatedDate = LocaleHelper.ToLocalTime(dr.GetDateTime(4));
            sharedPersonalization.IsDirty = false;
        }

#endregion

        private Int32 _PersonalizationPathId;
        private Byte[] _PageSettings;
        private String _Theme = string.Empty;
        private String _MasterPageFile = string.Empty;
        private DateTime _LastUpdatedDate;
        private bool _IsDirty;

        /// <summary>
        /// PersonalizationPathId
        /// </summary>
        [DataObjectField(true, true, false)]
        public Int32 PersonalizationPathId
        {
            get { return this._PersonalizationPathId; }
            set
            {
                if (this._PersonalizationPathId != value)
                {
                    this._PersonalizationPathId = value;
                    this.IsDirty = true;
                    this._PersonalizationPath = null;
                }
            }
        }

        /// <summary>
        /// PageSettings
        /// </summary>
        public Byte[] PageSettings
        {
            get { return this._PageSettings; }
            set
            {
                if (this._PageSettings != value)
                {
                    this._PageSettings = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Theme
        /// </summary>
        public String Theme
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
        /// MasterPageFile
        /// </summary>
        public String MasterPageFile
        {
            get { return this._MasterPageFile; }
            set
            {
                if (this._MasterPageFile != value)
                {
                    this._MasterPageFile = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// LastUpdatedDate
        /// </summary>
        public DateTime LastUpdatedDate
        {
            get { return this._LastUpdatedDate; }
            set
            {
                if (this._LastUpdatedDate != value)
                {
                    this._LastUpdatedDate = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Indicates whether this SharedPersonalization object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }

#region Parents
        private PersonalizationPath _PersonalizationPath;

        /// <summary>
        /// The PersonalizationPath object that this SharedPersonalization object is associated with
        /// </summary>
        public PersonalizationPath PersonalizationPath
        {
            get
            {
                if (!this.PersonalizationPathLoaded)
                {
                    this._PersonalizationPath = PersonalizationPathDataSource.Load(this.PersonalizationPathId);
                }
                return this._PersonalizationPath;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool PersonalizationPathLoaded { get { return ((this._PersonalizationPath != null) && (this._PersonalizationPath.PersonalizationPathId == this.PersonalizationPathId)); } }

#endregion

        /// <summary>
        /// Deletes this SharedPersonalization object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_SharedPersonalization");
            deleteQuery.Append(" WHERE PersonalizationPathId = @personalizationPathId");
            Database database = Token.Instance.Database;

            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@PersonalizationPathId", System.Data.DbType.Int32, this.PersonalizationPathId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            return (recordsAffected > 0);
        }


        /// <summary>
        /// Load this SharedPersonalization object from the database for the given primary key.
        /// </summary>
        /// <param name="personalizationPathId">Value of PersonalizationPathId of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 personalizationPathId)
        {
            bool result = false;
            this.PersonalizationPathId = personalizationPathId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_SharedPersonalization");
            selectQuery.Append(" WHERE PersonalizationPathId = @personalizationPathId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@personalizationPathId", System.Data.DbType.Int32, personalizationPathId);
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
        /// Saves this SharedPersonalization object to the database.
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save()
        {
            if (this.IsDirty)
            {
                Database database = Token.Instance.Database;
                MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
                bool recordExists = true;
                
                //SET DEFAULT FOR DATE FIELD
                if (this.LastUpdatedDate == System.DateTime.MinValue) this.LastUpdatedDate = LocaleHelper.LocalNow;

                if (recordExists) {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) As RecordCount FROM ac_SharedPersonalization");
                    selectQuery.Append(" WHERE PersonalizationPathId = @personalizationPathId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@PersonalizationPathId", System.Data.DbType.Int32, this.PersonalizationPathId);
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
                    updateQuery.Append("UPDATE ac_SharedPersonalization SET ");
                    updateQuery.Append("PageSettings = @PageSettings");
                    updateQuery.Append(", Theme = @Theme");
                    updateQuery.Append(", MasterPageFile = @MasterPageFile");
                    updateQuery.Append(", LastUpdatedDate = @LastUpdatedDate");
                    updateQuery.Append(" WHERE PersonalizationPathId = @PersonalizationPathId");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {
                        database.AddInParameter(updateCommand, "@PersonalizationPathId", System.Data.DbType.Int32, this.PersonalizationPathId);
                        database.AddInParameter(updateCommand, "@PageSettings", System.Data.DbType.Binary, NullableData.DbNullify(this.PageSettings));
                        database.AddInParameter(updateCommand, "@Theme", System.Data.DbType.String, NullableData.DbNullify(this.Theme));
                        database.AddInParameter(updateCommand, "@MasterPageFile", System.Data.DbType.String, NullableData.DbNullify(this.MasterPageFile));
                        database.AddInParameter(updateCommand, "@LastUpdatedDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(this.LastUpdatedDate));
                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_SharedPersonalization (PersonalizationPathId, PageSettings, Theme, MasterPageFile, LastUpdatedDate)");
                    insertQuery.Append(" VALUES (@PersonalizationPathId, @PageSettings, @Theme, @MasterPageFile, @LastUpdatedDate)");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@PersonalizationPathId", System.Data.DbType.Int32, this.PersonalizationPathId);
                        database.AddInParameter(insertCommand, "@PageSettings", System.Data.DbType.Binary, NullableData.DbNullify(this.PageSettings));
                        database.AddInParameter(insertCommand, "@Theme", System.Data.DbType.String, NullableData.DbNullify(this.Theme));
                        database.AddInParameter(insertCommand, "@MasterPageFile", System.Data.DbType.String, NullableData.DbNullify(this.MasterPageFile));
                        database.AddInParameter(insertCommand, "@LastUpdatedDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(this.LastUpdatedDate));
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

     }
}
