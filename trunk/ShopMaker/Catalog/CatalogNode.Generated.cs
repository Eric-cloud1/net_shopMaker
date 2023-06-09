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
    /// This class represents a CatalogNode object in the database.
    /// </summary>
    public partial class CatalogNode : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public CatalogNode() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="categoryId">Value of CategoryId.</param>
        /// <param name="catalogNodeId">Value of CatalogNodeId.</param>
        /// <param name="catalogNodeTypeId">Value of CatalogNodeTypeId.</param>
        /// </summary>
        public CatalogNode(Int32 categoryId, Int32 catalogNodeId, Byte catalogNodeTypeId)
        {
            this.CategoryId = categoryId;
            this.CatalogNodeId = catalogNodeId;
            this.CatalogNodeTypeId = catalogNodeTypeId;
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
          columnNames.Add(prefix + "CatalogNodeId");
          columnNames.Add(prefix + "CatalogNodeTypeId");
          columnNames.Add(prefix + "OrderBy");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given CatalogNode object from the given database data reader.
        /// </summary>
        /// <param name="catalogNode">The CatalogNode object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(CatalogNode catalogNode, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            catalogNode.CategoryId = dr.GetInt32(0);
            catalogNode.CatalogNodeId = dr.GetInt32(1);
            catalogNode.CatalogNodeTypeId = dr.GetByte(2);
            catalogNode.OrderBy = dr.GetInt16(3);
            catalogNode.IsDirty = false;
        }

#endregion

        private Int32 _CategoryId;
        private Int32 _CatalogNodeId;
        private Byte _CatalogNodeTypeId;
        private Int16 _OrderBy = -1;
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
        /// CatalogNodeId
        /// </summary>
        [DataObjectField(true, false, false)]
        public Int32 CatalogNodeId
        {
            get { return this._CatalogNodeId; }
            set
            {
                if (this._CatalogNodeId != value)
                {
                    this._CatalogNodeId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// CatalogNodeTypeId
        /// </summary>
        [DataObjectField(true, false, false)]
        public Byte CatalogNodeTypeId
        {
            get { return this._CatalogNodeTypeId; }
            set
            {
                if (this._CatalogNodeTypeId != value)
                {
                    this._CatalogNodeTypeId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// OrderBy
        /// </summary>
        public Int16 OrderBy
        {
            get { return this._OrderBy; }
            set
            {
                if (this._OrderBy != value)
                {
                    this._OrderBy = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Indicates whether this CatalogNode object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }

#region Parents
        private Category _Category;

        /// <summary>
        /// The Category object that this CatalogNode object is associated with
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
        /// Deletes this CatalogNode object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_CatalogNodes");
            deleteQuery.Append(" WHERE CategoryId = @categoryId AND CatalogNodeId = @catalogNodeId AND CatalogNodeTypeId = @catalogNodeTypeId");
            Database database = Token.Instance.Database;

            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@CategoryId", System.Data.DbType.Int32, this.CategoryId);
                database.AddInParameter(deleteCommand, "@CatalogNodeId", System.Data.DbType.Int32, this.CatalogNodeId);
                database.AddInParameter(deleteCommand, "@CatalogNodeTypeId", System.Data.DbType.Byte, this.CatalogNodeTypeId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd(); 
            return (recordsAffected > 0);
        }


        /// <summary>
        /// Load this CatalogNode object from the database for the given primary key.
        /// </summary>
        /// <param name="categoryId">Value of CategoryId of the object to load.</param>
        /// <param name="catalogNodeId">Value of CatalogNodeId of the object to load.</param>
        /// <param name="catalogNodeTypeId">Value of CatalogNodeTypeId of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 categoryId, Int32 catalogNodeId, Byte catalogNodeTypeId)
        {
            bool result = false;
            this.CategoryId = categoryId;
            this.CatalogNodeId = catalogNodeId;
            this.CatalogNodeTypeId = catalogNodeTypeId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_CatalogNodes");
            selectQuery.Append(" WHERE CategoryId = @categoryId AND CatalogNodeId = @catalogNodeId AND CatalogNodeTypeId = @catalogNodeTypeId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@categoryId", System.Data.DbType.Int32, categoryId);
            database.AddInParameter(selectCommand, "@catalogNodeId", System.Data.DbType.Int32, catalogNodeId);
            database.AddInParameter(selectCommand, "@catalogNodeTypeId", System.Data.DbType.Byte, catalogNodeTypeId);
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
        /// Saves this CatalogNode object to the database.
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save()
        {
            if (this.IsDirty)
            {
                Database database = Token.Instance.Database;
                MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
                bool recordExists = true;
                
                if (this.OrderBy < 0) this.OrderBy = CatalogNodeDataSource.GetNextOrderBy(this.CategoryId);

                if (recordExists) {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) As RecordCount FROM ac_CatalogNodes");
                    selectQuery.Append(" WHERE CategoryId = @categoryId AND CatalogNodeId = @catalogNodeId AND CatalogNodeTypeId = @catalogNodeTypeId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@CategoryId", System.Data.DbType.Int32, this.CategoryId);
                        database.AddInParameter(selectCommand, "@CatalogNodeId", System.Data.DbType.Int32, this.CatalogNodeId);
                        database.AddInParameter(selectCommand, "@CatalogNodeTypeId", System.Data.DbType.Byte, this.CatalogNodeTypeId);
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
                    updateQuery.Append("UPDATE ac_CatalogNodes SET ");
                    updateQuery.Append("OrderBy = @OrderBy");
                    updateQuery.Append(" WHERE CategoryId = @CategoryId AND CatalogNodeId = @CatalogNodeId AND CatalogNodeTypeId = @CatalogNodeTypeId");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {
                        database.AddInParameter(updateCommand, "@CategoryId", System.Data.DbType.Int32, this.CategoryId);
                        database.AddInParameter(updateCommand, "@CatalogNodeId", System.Data.DbType.Int32, this.CatalogNodeId);
                        database.AddInParameter(updateCommand, "@CatalogNodeTypeId", System.Data.DbType.Byte, this.CatalogNodeTypeId);
                        database.AddInParameter(updateCommand, "@OrderBy", System.Data.DbType.Int16, this.OrderBy);
                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_CatalogNodes (CategoryId, CatalogNodeId, CatalogNodeTypeId, OrderBy)");
                    insertQuery.Append(" VALUES (@CategoryId, @CatalogNodeId, @CatalogNodeTypeId, @OrderBy)");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@CategoryId", System.Data.DbType.Int32, this.CategoryId);
                        database.AddInParameter(insertCommand, "@CatalogNodeId", System.Data.DbType.Int32, this.CatalogNodeId);
                        database.AddInParameter(insertCommand, "@CatalogNodeTypeId", System.Data.DbType.Byte, this.CatalogNodeTypeId);
                        database.AddInParameter(insertCommand, "@OrderBy", System.Data.DbType.Int16, this.OrderBy);
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
