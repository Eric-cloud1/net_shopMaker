//Generated by PersistableBaseGenerator

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Catalog;
using MakerShop.Utility;

namespace MakerShop.Catalog
{
    /// <summary>
    /// This class represents a CategoryParent object in the database.
    /// </summary>
    public partial class CategoryParent : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public CategoryParent() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="categoryId">Value of CategoryId.</param>
        /// <param name="parentId">Value of ParentId.</param>
        /// </summary>
        public CategoryParent(Int32 categoryId, Int32 parentId)
        {
            this.CategoryId = categoryId;
            this.ParentId = parentId;
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
          columnNames.Add(prefix + "CategoryId");
          columnNames.Add(prefix + "ParentId");
          columnNames.Add(prefix + "ParentLevel");
          columnNames.Add(prefix + "ParentNumber");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given CategoryParent object from the given database data reader.
        /// </summary>
        /// <param name="categoryParent">The CategoryParent object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(CategoryParent categoryParent, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            categoryParent.CategoryId = dr.GetInt32(0);
            categoryParent.ParentId = dr.GetInt32(1);
            categoryParent.ParentLevel = NullableData.GetByte(dr, 2);
            categoryParent.ParentNumber = NullableData.GetByte(dr, 3);
            categoryParent.IsDirty = false;
        }

#endregion

        private Int32 _CategoryId;
        private Int32 _ParentId;
        private Byte _ParentLevel;
        private Byte _ParentNumber;
        private bool _IsDirty;

        /// <summary>
        /// CategoryId
        /// </summary>
        [DataObjectField(true, false, false)]
        public Int32 CategoryId
        {
            get { return this._CategoryId; }
            set
            {
                if (this._CategoryId != value)
                {
                    this._CategoryId = value;
                    this.IsDirty = true;
                    this._Category = null;
                }
            }
        }

        /// <summary>
        /// ParentId
        /// </summary>
        [DataObjectField(true, false, false)]
        public Int32 ParentId
        {
            get { return this._ParentId; }
            set
            {
                if (this._ParentId != value)
                {
                    this._ParentId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// ParentLevel
        /// </summary>
        public Byte ParentLevel
        {
            get { return this._ParentLevel; }
            set
            {
                if (this._ParentLevel != value)
                {
                    this._ParentLevel = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// ParentNumber
        /// </summary>
        public Byte ParentNumber
        {
            get { return this._ParentNumber; }
            set
            {
                if (this._ParentNumber != value)
                {
                    this._ParentNumber = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Indicates whether this CategoryParent object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }

#region Parents
        private Category _Category;

        /// <summary>
        /// The Category object that this CategoryParent object is associated with
        /// </summary>
        public Category Category
        {
            get
            {
                if (!this.CategoryLoaded)
                {
                    this._Category = CategoryDataSource.Load(this.CategoryId);
                }
                return this._Category;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool CategoryLoaded { get { return ((this._Category != null) && (this._Category.CategoryId == this.CategoryId)); } }

#endregion

        /// <summary>
        /// Deletes this CategoryParent object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_CategoryParents");
            deleteQuery.Append(" WHERE CategoryId = @categoryId AND ParentId = @parentId");
            Database database = Token.Instance.Database;
            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null);

            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@CategoryId", System.Data.DbType.Int32, this.CategoryId);
                database.AddInParameter(deleteCommand, "@ParentId", System.Data.DbType.Int32, this.ParentId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            return (recordsAffected > 0);
        }


        /// <summary>
        /// Load this CategoryParent object from the database for the given primary key.
        /// </summary>
        /// <param name="categoryId">Value of CategoryId of the object to load.</param>
        /// <param name="parentId">Value of ParentId of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 categoryId, Int32 parentId)
        {
            bool result = false;
            this.CategoryId = categoryId;
            this.ParentId = parentId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_CategoryParents");
            selectQuery.Append(" WHERE CategoryId = @categoryId AND ParentId = @parentId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@categoryId", System.Data.DbType.Int32, categoryId);
            database.AddInParameter(selectCommand, "@parentId", System.Data.DbType.Int32, parentId);
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
        /// Saves this CategoryParent object to the database.
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save()
        {
            if (this.IsDirty)
            {
                Database database = Token.Instance.Database;
                MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
                bool recordExists = true;
                
                if (recordExists) {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) As RecordCount FROM ac_CategoryParents");
                    selectQuery.Append(" WHERE CategoryId = @categoryId AND ParentId = @parentId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@CategoryId", System.Data.DbType.Int32, this.CategoryId);
                        database.AddInParameter(selectCommand, "@ParentId", System.Data.DbType.Int32, this.ParentId);
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
                    updateQuery.Append("UPDATE ac_CategoryParents SET ");
                    updateQuery.Append("ParentLevel = @ParentLevel");
                    updateQuery.Append(", ParentNumber = @ParentNumber");
                    updateQuery.Append(" WHERE CategoryId = @CategoryId AND ParentId = @ParentId");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {
                        database.AddInParameter(updateCommand, "@CategoryId", System.Data.DbType.Int32, this.CategoryId);
                        database.AddInParameter(updateCommand, "@ParentId", System.Data.DbType.Int32, this.ParentId);
                        database.AddInParameter(updateCommand, "@ParentLevel", System.Data.DbType.Byte, NullableData.DbNullify(this.ParentLevel));
                        database.AddInParameter(updateCommand, "@ParentNumber", System.Data.DbType.Byte, NullableData.DbNullify(this.ParentNumber));
                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_CategoryParents (CategoryId, ParentId, ParentLevel, ParentNumber)");
                    insertQuery.Append(" VALUES (@CategoryId, @ParentId, @ParentLevel, @ParentNumber)");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@CategoryId", System.Data.DbType.Int32, this.CategoryId);
                        database.AddInParameter(insertCommand, "@ParentId", System.Data.DbType.Int32, this.ParentId);
                        database.AddInParameter(insertCommand, "@ParentLevel", System.Data.DbType.Byte, NullableData.DbNullify(this.ParentLevel));
                        database.AddInParameter(insertCommand, "@ParentNumber", System.Data.DbType.Byte, NullableData.DbNullify(this.ParentNumber));
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
