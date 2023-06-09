//Generated by PersistableBaseGenerator

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Shipping;
using MakerShop.Utility;

namespace MakerShop.Shipping
{
    /// <summary>
    /// This class represents a ShipMethodWarehouse object in the database.
    /// </summary>
    public partial class ShipMethodWarehouse : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ShipMethodWarehouse() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="shipMethodId">Value of ShipMethodId.</param>
        /// <param name="warehouseId">Value of WarehouseId.</param>
        /// </summary>
        public ShipMethodWarehouse(Int32 shipMethodId, Int32 warehouseId)
        {
            this.ShipMethodId = shipMethodId;
            this.WarehouseId = warehouseId;
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
          columnNames.Add(prefix + "ShipMethodId");
          columnNames.Add(prefix + "WarehouseId");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given ShipMethodWarehouse object from the given database data reader.
        /// </summary>
        /// <param name="shipMethodWarehouse">The ShipMethodWarehouse object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(ShipMethodWarehouse shipMethodWarehouse, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            shipMethodWarehouse.ShipMethodId = dr.GetInt32(0);
            shipMethodWarehouse.WarehouseId = dr.GetInt32(1);
            shipMethodWarehouse.IsDirty = false;
        }

#endregion

        private Int32 _ShipMethodId;
        private Int32 _WarehouseId;
        private bool _IsDirty;

        /// <summary>
        /// ShipMethodId
        /// </summary>
        [DataObjectField(true, false, false)]
        public Int32 ShipMethodId
        {
            get { return this._ShipMethodId; }
            set
            {
                if (this._ShipMethodId != value)
                {
                    this._ShipMethodId = value;
                    this.IsDirty = true;
                    this._ShipMethod = null;
                }
            }
        }

        /// <summary>
        /// WarehouseId
        /// </summary>
        [DataObjectField(true, false, false)]
        public Int32 WarehouseId
        {
            get { return this._WarehouseId; }
            set
            {
                if (this._WarehouseId != value)
                {
                    this._WarehouseId = value;
                    this.IsDirty = true;
                    this._Warehouse = null;
                }
            }
        }

        /// <summary>
        /// Indicates whether this ShipMethodWarehouse object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }

#region Parents
        private ShipMethod _ShipMethod;
        private Warehouse _Warehouse;

        /// <summary>
        /// The ShipMethod object that this ShipMethodWarehouse object is associated with
        /// </summary>
        public ShipMethod ShipMethod
        {
            get
            {
                if (!this.ShipMethodLoaded)
                {
                    this._ShipMethod = ShipMethodDataSource.Load(this.ShipMethodId);
                }
                return this._ShipMethod;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool ShipMethodLoaded { get { return ((this._ShipMethod != null) && (this._ShipMethod.ShipMethodId == this.ShipMethodId)); } }

        /// <summary>
        /// The Warehouse object that this ShipMethodWarehouse object is associated with
        /// </summary>
        public Warehouse Warehouse
        {
            get
            {
                if (!this.WarehouseLoaded)
                {
                    this._Warehouse = WarehouseDataSource.Load(this.WarehouseId);
                }
                return this._Warehouse;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool WarehouseLoaded { get { return ((this._Warehouse != null) && (this._Warehouse.WarehouseId == this.WarehouseId)); } }

#endregion

        /// <summary>
        /// Deletes this ShipMethodWarehouse object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_ShipMethodWarehouses");
            deleteQuery.Append(" WHERE ShipMethodId = @shipMethodId AND WarehouseId = @warehouseId");
            Database database = Token.Instance.Database;

            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@ShipMethodId", System.Data.DbType.Int32, this.ShipMethodId);
                database.AddInParameter(deleteCommand, "@WarehouseId", System.Data.DbType.Int32, this.WarehouseId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }

            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            return (recordsAffected > 0);
        }

        /// <summary>
        /// Load this ShipMethodWarehouse object from the database for the given primary key.
        /// </summary>
        /// <param name="shipMethodId">Value of ShipMethodId of the object to load.</param>
        /// <param name="warehouseId">Value of WarehouseId of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 shipMethodId, Int32 warehouseId)
        {
            this.ShipMethodId = shipMethodId;
            this.WarehouseId = warehouseId;
            this.IsDirty = false;
            return true;
        }

        /// <summary>
        /// Saves this ShipMethodWarehouse object to the database.
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save()
        {
            if (this.IsDirty)
            {
                Database database = Token.Instance.Database;
                
                bool recordExists = true;
                
                //generate key(s) if needed
                if (recordExists) {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) As RecordCount FROM ac_ShipMethodWarehouses");
                    selectQuery.Append(" WHERE ShipMethodId = @shipMethodId AND WarehouseId = @warehouseId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@ShipMethodId", System.Data.DbType.Int32, this.ShipMethodId);
                        database.AddInParameter(selectCommand, "@WarehouseId", System.Data.DbType.Int32, this.WarehouseId);
                        if ((int)database.ExecuteScalar(selectCommand) == 0)
                        {
                            recordExists = false;
                        }
                    }
                }

                if (recordExists)
                {
                    //RECORD ALREADY EXISTS IN DATABASE
                    this.IsDirty = false;
                    return SaveResult.RecordUpdated;
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_ShipMethodWarehouses (ShipMethodId, WarehouseId)");
                    insertQuery.Append(" VALUES (@ShipMethodId, @WarehouseId)");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@ShipMethodId", System.Data.DbType.Int32, this.ShipMethodId);
                        database.AddInParameter(insertCommand, "@WarehouseId", System.Data.DbType.Int32, this.WarehouseId);
                        
                        MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
                        int recordsAffected = database.ExecuteNonQuery(insertCommand);
                        MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();

                        //OBJECT IS NOT DIRTY IF RECORD WAS INSERTED
                        this.IsDirty = (recordsAffected == 0);
                        if (this.IsDirty) { return SaveResult.Failed; }
                        return SaveResult.RecordInserted;
                    }
                }

            }

            //SAVE IS SUCCESSFUL IF OBJECT IS NOT DIRTY
            return SaveResult.NotDirty;
        }

     }
}
