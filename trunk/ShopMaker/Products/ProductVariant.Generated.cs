//CUSTOMIZED

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Products;
using MakerShop.Utility;

namespace MakerShop.Products
{
    /// <summary>
    /// This class represents a ProductVariant object in the database.
    /// </summary>
    public partial class ProductVariant : IPersistable
    {

        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ProductVariant() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="productVariantId">Value of ProductVariantId.</param>
        /// </summary>
        public ProductVariant(Int32 productVariantId)
        {
            this.ProductVariantId = productVariantId;
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
            columnNames.Add(prefix + "ProductVariantId");
            columnNames.Add(prefix + "ProductId");
            columnNames.Add(prefix + "Option1");
            columnNames.Add(prefix + "Option2");
            columnNames.Add(prefix + "Option3");
            columnNames.Add(prefix + "Option4");
            columnNames.Add(prefix + "Option5");
            columnNames.Add(prefix + "Option6");
            columnNames.Add(prefix + "Option7");
            columnNames.Add(prefix + "Option8");
            columnNames.Add(prefix + "VariantName");
            columnNames.Add(prefix + "Sku");
            columnNames.Add(prefix + "Price");
            columnNames.Add(prefix + "PriceModeId");
            columnNames.Add(prefix + "Weight");
            columnNames.Add(prefix + "WeightModeId");
            columnNames.Add(prefix + "CostOfGoods");
            columnNames.Add(prefix + "InStock");
            columnNames.Add(prefix + "InStockWarningLevel");
            columnNames.Add(prefix + "Available");
            return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given ProductVariant object from the given database data reader.
        /// </summary>
        /// <param name="productVariant">The ProductVariant object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(ProductVariant productVariant, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            productVariant.ProductVariantId = dr.GetInt32(0);
            productVariant.ProductId = dr.GetInt32(1);
            productVariant.Option1 = dr.GetInt32(2);
            productVariant.Option2 = dr.GetInt32(3);
            productVariant.Option3 = dr.GetInt32(4);
            productVariant.Option4 = dr.GetInt32(5);
            productVariant.Option5 = dr.GetInt32(6);
            productVariant.Option6 = dr.GetInt32(7);
            productVariant.Option7 = dr.GetInt32(8);
            productVariant.Option8 = dr.GetInt32(9);
            productVariant.VariantName = NullableData.GetString(dr, 10);
            productVariant.Sku = NullableData.GetString(dr, 11);
            productVariant.Price = NullableData.GetDecimal(dr, 12);
            productVariant.PriceModeId = dr.GetByte(13);
            productVariant.Weight = NullableData.GetDecimal(dr, 14);
            productVariant.WeightModeId = dr.GetByte(15);
            productVariant.CostOfGoods = NullableData.GetDecimal(dr, 16);
            productVariant.InStock = dr.GetInt32(17);
            productVariant.InStockWarningLevel = dr.GetInt32(18);
            productVariant.Available = dr.GetBoolean(19);
            productVariant.IsDirty = false;
        }

        #endregion

        private Int32 _ProductVariantId;
        private Int32 _ProductId;
        private Int32 _Option1;
        private Int32 _Option2;
        private Int32 _Option3;
        private Int32 _Option4;
        private Int32 _Option5;
        private Int32 _Option6;
        private Int32 _Option7;
        private Int32 _Option8;
        private String _VariantName = string.Empty;
        private String _Sku = string.Empty;
        private LSDecimal _Price;
        private Byte _PriceModeId;
        private LSDecimal _Weight;
        private Byte _WeightModeId;
        private LSDecimal _CostOfGoods;
        private Int32 _InStock;
        private Int32 _InStockWarningLevel;
        private Boolean _Available;
        private bool _IsDirty;

        /// <summary>
        /// ProductVariantId
        /// </summary>
        [DataObjectField(true, true, false)]
        public Int32 ProductVariantId
        {
            get { return this._ProductVariantId; }
            set
            {
                if (this._ProductVariantId != value)
                {
                    this._ProductVariantId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// ProductId
        /// </summary>
        public Int32 ProductId
        {
            get { return this._ProductId; }
            set
            {
                if (this._ProductId != value)
                {
                    this._ProductId = value;
                    this.IsDirty = true;
                    this._Product = null;
                }
            }
        }

        /// <summary>
        /// Option1
        /// </summary>
        public Int32 Option1
        {
            get { return this._Option1; }
            set
            {
                if (this._Option1 != value)
                {
                    this._Option1 = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Option2
        /// </summary>
        public Int32 Option2
        {
            get { return this._Option2; }
            set
            {
                if (this._Option2 != value)
                {
                    this._Option2 = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Option3
        /// </summary>
        public Int32 Option3
        {
            get { return this._Option3; }
            set
            {
                if (this._Option3 != value)
                {
                    this._Option3 = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Option4
        /// </summary>
        public Int32 Option4
        {
            get { return this._Option4; }
            set
            {
                if (this._Option4 != value)
                {
                    this._Option4 = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Option5
        /// </summary>
        public Int32 Option5
        {
            get { return this._Option5; }
            set
            {
                if (this._Option5 != value)
                {
                    this._Option5 = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Option6
        /// </summary>
        public Int32 Option6
        {
            get { return this._Option6; }
            set
            {
                if (this._Option6 != value)
                {
                    this._Option6 = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Option7
        /// </summary>
        public Int32 Option7
        {
            get { return this._Option7; }
            set
            {
                if (this._Option7 != value)
                {
                    this._Option7 = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Option8
        /// </summary>
        public Int32 Option8
        {
            get { return this._Option8; }
            set
            {
                if (this._Option8 != value)
                {
                    this._Option8 = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// VariantName
        /// </summary>
        public String VariantName
        {
            get { return this._VariantName; }
            set
            {
                if (this._VariantName != value)
                {
                    this._VariantName = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Sku
        /// </summary>
        public String Sku
        {
            get { return this._Sku; }
            set
            {
                if (this._Sku != value)
                {
                    this._Sku = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Price
        /// </summary>
        public LSDecimal Price
        {
            get { return this._Price; }
            set
            {
                if (this._Price != value)
                {
                    this._Price = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// PriceModeId
        /// </summary>
        public Byte PriceModeId
        {
            get { return this._PriceModeId; }
            set
            {
                if (this._PriceModeId != value)
                {
                    this._PriceModeId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Weight
        /// </summary>
        public LSDecimal Weight
        {
            get { return this._Weight; }
            set
            {
                if (this._Weight != value)
                {
                    this._Weight = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// WeightModeId
        /// </summary>
        public Byte WeightModeId
        {
            get { return this._WeightModeId; }
            set
            {
                if (this._WeightModeId != value)
                {
                    this._WeightModeId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// CostOfGoods
        /// </summary>
        public LSDecimal CostOfGoods
        {
            get { return this._CostOfGoods; }
            set
            {
                if (this._CostOfGoods != value)
                {
                    this._CostOfGoods = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// InStock
        /// </summary>
        public Int32 InStock
        {
            get { return this._InStock; }
            set
            {
                if (this._InStock != value)
                {
                    this._InStock = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// InStockWarningLevel
        /// </summary>
        public Int32 InStockWarningLevel
        {
            get { return this._InStockWarningLevel; }
            set
            {
                if (this._InStockWarningLevel != value)
                {
                    this._InStockWarningLevel = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Available
        /// </summary>
        public Boolean Available
        {
            get { return this._Available; }
            set
            {
                if (this._Available != value)
                {
                    this._Available = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Indicates whether this ProductVariant object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }

        #region Parents
        private Product _Product;

        /// <summary>
        /// The Product object that this ProductVariant object is associated with
        /// </summary>
        public Product Product
        {
            get
            {
                if (!this.ProductLoaded)
                {
                    this._Product = ProductDataSource.Load(this.ProductId);
                }
                return this._Product;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool ProductLoaded { get { return ((this._Product != null) && (this._Product.ProductId == this.ProductId)); } }

        #endregion

        /// <summary>
        /// Deletes this ProductVariant object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_ProductVariants");
            deleteQuery.Append(" WHERE ProductVariantId = @productVariantId");
            Database database = Token.Instance.Database;

            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@ProductVariantId", System.Data.DbType.Int32, this.ProductVariantId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }

            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();

            return (recordsAffected > 0);
        }


        /// <summary>
        /// Load this ProductVariant object from the database for the given primary key.
        /// </summary>
        /// <param name="productVariantId">Value of ProductVariantId of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        protected bool BaseLoad(Int32 productVariantId)
        {
            bool result = false;
            this.ProductVariantId = productVariantId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_ProductVariants");
            selectQuery.Append(" WHERE ProductVariantId = @productVariantId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@productVariantId", System.Data.DbType.Int32, productVariantId);
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

        /// <summary>
        /// Saves this ProductVariant object to the database.
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        protected SaveResult BaseSave()
        {
            if (this.IsDirty)
            {
                Database database = Token.Instance.Database;
                MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
                bool recordExists = true;

                if (this.ProductVariantId == 0) recordExists = false;

                if (recordExists)
                {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) As RecordCount FROM ac_ProductVariants");
                    selectQuery.Append(" WHERE ProductVariantId = @productVariantId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@ProductVariantId", System.Data.DbType.Int32, this.ProductVariantId);
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
                    updateQuery.Append("UPDATE ac_ProductVariants SET ");
                    updateQuery.Append("ProductId = @ProductId");
                    updateQuery.Append(", Option1 = @Option1");
                    updateQuery.Append(", Option2 = @Option2");
                    updateQuery.Append(", Option3 = @Option3");
                    updateQuery.Append(", Option4 = @Option4");
                    updateQuery.Append(", Option5 = @Option5");
                    updateQuery.Append(", Option6 = @Option6");
                    updateQuery.Append(", Option7 = @Option7");
                    updateQuery.Append(", Option8 = @Option8");
                    updateQuery.Append(", VariantName = @VariantName");
                    updateQuery.Append(", Sku = @Sku");
                    updateQuery.Append(", Price = @Price");
                    updateQuery.Append(", PriceModeId = @PriceModeId");
                    updateQuery.Append(", Weight = @Weight");
                    updateQuery.Append(", WeightModeId = @WeightModeId");
                    updateQuery.Append(", CostOfGoods = @CostOfGoods");
                    updateQuery.Append(", InStock = @InStock");
                    updateQuery.Append(", InStockWarningLevel = @InStockWarningLevel");
                    updateQuery.Append(", Available = @Available");
                    updateQuery.Append(" WHERE ProductVariantId = @ProductVariantId");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {
                        database.AddInParameter(updateCommand, "@ProductVariantId", System.Data.DbType.Int32, this.ProductVariantId);
                        database.AddInParameter(updateCommand, "@ProductId", System.Data.DbType.Int32, this.ProductId);
                        database.AddInParameter(updateCommand, "@Option1", System.Data.DbType.Int32, this.Option1);
                        database.AddInParameter(updateCommand, "@Option2", System.Data.DbType.Int32, this.Option2);
                        database.AddInParameter(updateCommand, "@Option3", System.Data.DbType.Int32, this.Option3);
                        database.AddInParameter(updateCommand, "@Option4", System.Data.DbType.Int32, this.Option4);
                        database.AddInParameter(updateCommand, "@Option5", System.Data.DbType.Int32, this.Option5);
                        database.AddInParameter(updateCommand, "@Option6", System.Data.DbType.Int32, this.Option6);
                        database.AddInParameter(updateCommand, "@Option7", System.Data.DbType.Int32, this.Option7);
                        database.AddInParameter(updateCommand, "@Option8", System.Data.DbType.Int32, this.Option8);
                        database.AddInParameter(updateCommand, "@VariantName", System.Data.DbType.String, NullableData.DbNullify(this.VariantName));
                        database.AddInParameter(updateCommand, "@Sku", System.Data.DbType.String, NullableData.DbNullify(this.Sku));
                        database.AddInParameter(updateCommand, "@Price", System.Data.DbType.Decimal, NullableData.DbNullify(this.Price));
                        database.AddInParameter(updateCommand, "@PriceModeId", System.Data.DbType.Byte, this.PriceModeId);
                        database.AddInParameter(updateCommand, "@Weight", System.Data.DbType.Decimal, NullableData.DbNullify(this.Weight));
                        database.AddInParameter(updateCommand, "@WeightModeId", System.Data.DbType.Byte, this.WeightModeId);
                        database.AddInParameter(updateCommand, "@CostOfGoods", System.Data.DbType.Decimal, NullableData.DbNullify(this.CostOfGoods));
                        database.AddInParameter(updateCommand, "@InStock", System.Data.DbType.Int32, this.InStock);
                        database.AddInParameter(updateCommand, "@InStockWarningLevel", System.Data.DbType.Int32, this.InStockWarningLevel);
                        database.AddInParameter(updateCommand, "@Available", System.Data.DbType.Boolean, this.Available);
                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_ProductVariants (ProductId, Option1, Option2, Option3, Option4, Option5, Option6, Option7, Option8, VariantName, Sku, Price, PriceModeId, Weight, WeightModeId, CostOfGoods, InStock, InStockWarningLevel, Available)");
                    insertQuery.Append(" VALUES (@ProductId, @Option1, @Option2, @Option3, @Option4, @Option5, @Option6, @Option7, @Option8, @VariantName, @Sku, @Price, @PriceModeId, @Weight, @WeightModeId, @CostOfGoods, @InStock, @InStockWarningLevel, @Available)");
                    insertQuery.Append("; SELECT Scope_Identity()");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@ProductVariantId", System.Data.DbType.Int32, this.ProductVariantId);
                        database.AddInParameter(insertCommand, "@ProductId", System.Data.DbType.Int32, this.ProductId);
                        database.AddInParameter(insertCommand, "@Option1", System.Data.DbType.Int32, this.Option1);
                        database.AddInParameter(insertCommand, "@Option2", System.Data.DbType.Int32, this.Option2);
                        database.AddInParameter(insertCommand, "@Option3", System.Data.DbType.Int32, this.Option3);
                        database.AddInParameter(insertCommand, "@Option4", System.Data.DbType.Int32, this.Option4);
                        database.AddInParameter(insertCommand, "@Option5", System.Data.DbType.Int32, this.Option5);
                        database.AddInParameter(insertCommand, "@Option6", System.Data.DbType.Int32, this.Option6);
                        database.AddInParameter(insertCommand, "@Option7", System.Data.DbType.Int32, this.Option7);
                        database.AddInParameter(insertCommand, "@Option8", System.Data.DbType.Int32, this.Option8);
                        database.AddInParameter(insertCommand, "@VariantName", System.Data.DbType.String, NullableData.DbNullify(this.VariantName));
                        database.AddInParameter(insertCommand, "@Sku", System.Data.DbType.String, NullableData.DbNullify(this.Sku));
                        database.AddInParameter(insertCommand, "@Price", System.Data.DbType.Decimal, NullableData.DbNullify(this.Price));
                        database.AddInParameter(insertCommand, "@PriceModeId", System.Data.DbType.Byte, this.PriceModeId);
                        database.AddInParameter(insertCommand, "@Weight", System.Data.DbType.Decimal, NullableData.DbNullify(this.Weight));
                        database.AddInParameter(insertCommand, "@WeightModeId", System.Data.DbType.Byte, this.WeightModeId);
                        database.AddInParameter(insertCommand, "@CostOfGoods", System.Data.DbType.Decimal, NullableData.DbNullify(this.CostOfGoods));
                        database.AddInParameter(insertCommand, "@InStock", System.Data.DbType.Int32, this.InStock);
                        database.AddInParameter(insertCommand, "@InStockWarningLevel", System.Data.DbType.Int32, this.InStockWarningLevel);
                        database.AddInParameter(insertCommand, "@Available", System.Data.DbType.Boolean, this.Available);
                        //RESULT IS NEW IDENTITY;
                        result = AlwaysConvert.ToInt(database.ExecuteScalar(insertCommand));
                        this._ProductVariantId = result;
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