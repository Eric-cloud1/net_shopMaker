//Generated by PersistableBaseGenerator

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Marketing;
using MakerShop.Orders;
using MakerShop.Stores;
using MakerShop.Utility;

namespace MakerShop.Marketing
{
    /// <summary>
    /// This class represents a Coupon object in the database.
    /// </summary>
    public partial class Coupon : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Coupon() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="couponId">Value of CouponId.</param>
        /// </summary>
        public Coupon(Int32 couponId)
        {
            this.CouponId = couponId;
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
          columnNames.Add(prefix + "CouponId");
          columnNames.Add(prefix + "StoreId");
          columnNames.Add(prefix + "CouponTypeId");
          columnNames.Add(prefix + "Name");
          columnNames.Add(prefix + "CouponCode");
          columnNames.Add(prefix + "DiscountAmount");
          columnNames.Add(prefix + "IsPercent");
          columnNames.Add(prefix + "MaxValue");
          columnNames.Add(prefix + "MinPurchase");
          columnNames.Add(prefix + "MinQuantity");
          columnNames.Add(prefix + "MaxQuantity");
          columnNames.Add(prefix + "QuantityInterval");
          columnNames.Add(prefix + "MaxUses");
          columnNames.Add(prefix + "MaxUsesPerCustomer");
          columnNames.Add(prefix + "StartDate");
          columnNames.Add(prefix + "EndDate");
          columnNames.Add(prefix + "ProductRuleId");
          columnNames.Add(prefix + "AllowCombine");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given Coupon object from the given database data reader.
        /// </summary>
        /// <param name="coupon">The Coupon object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(Coupon coupon, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            coupon.CouponId = dr.GetInt32(0);
            coupon.StoreId = dr.GetInt32(1);
            coupon.CouponTypeId = NullableData.GetByte(dr, 2);
            coupon.Name = dr.GetString(3);
            coupon.CouponCode = dr.GetString(4);
            coupon.DiscountAmount = dr.GetDecimal(5);
            coupon.IsPercent = dr.GetBoolean(6);
            coupon.MaxValue = dr.GetDecimal(7);
            coupon.MinPurchase = dr.GetDecimal(8);
            coupon.MinQuantity = dr.GetInt16(9);
            coupon.MaxQuantity = dr.GetInt16(10);
            coupon.QuantityInterval = dr.GetInt16(11);
            coupon.MaxUses = dr.GetInt16(12);
            coupon.MaxUsesPerCustomer = dr.GetInt16(13);
            coupon.StartDate = LocaleHelper.ToLocalTime(NullableData.GetDateTime(dr, 14));
            coupon.EndDate = LocaleHelper.ToLocalTime(NullableData.GetDateTime(dr, 15));
            coupon.ProductRuleId = dr.GetByte(16);
            coupon.AllowCombine = dr.GetBoolean(17);
            coupon.IsDirty = false;
        }

#endregion

        private Int32 _CouponId;
        private Int32 _StoreId;
        private Byte _CouponTypeId;
        private String _Name = string.Empty;
        private String _CouponCode = string.Empty;
        private LSDecimal _DiscountAmount;
        private Boolean _IsPercent;
        private LSDecimal _MaxValue;
        private LSDecimal _MinPurchase;
        private Int16 _MinQuantity;
        private Int16 _MaxQuantity;
        private Int16 _QuantityInterval;
        private Int16 _MaxUses;
        private Int16 _MaxUsesPerCustomer;
        private DateTime _StartDate;
        private DateTime _EndDate;
        private Byte _ProductRuleId;
        private Boolean _AllowCombine;
        private bool _IsDirty;

        /// <summary>
        /// CouponId
        /// </summary>
        [DataObjectField(true, true, false)]
        public Int32 CouponId
        {
            get { return this._CouponId; }
            set
            {
                if (this._CouponId != value)
                {
                    this._CouponId = value;
                    this.IsDirty = true;
                    this.EnsureChildProperties();
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
        /// CouponTypeId
        /// </summary>
        public Byte CouponTypeId
        {
            get { return this._CouponTypeId; }
            set
            {
                if (this._CouponTypeId != value)
                {
                    this._CouponTypeId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Name
        /// </summary>
        public String Name
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
        /// CouponCode
        /// </summary>
        public String CouponCode
        {
            get { return this._CouponCode; }
            set
            {
                if (this._CouponCode != value)
                {
                    this._CouponCode = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// DiscountAmount
        /// </summary>
        public LSDecimal DiscountAmount
        {
            get { return this._DiscountAmount; }
            set
            {
                if (this._DiscountAmount != value)
                {
                    this._DiscountAmount = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// IsPercent
        /// </summary>
        public Boolean IsPercent
        {
            get { return this._IsPercent; }
            set
            {
                if (this._IsPercent != value)
                {
                    this._IsPercent = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// MaxValue
        /// </summary>
        public LSDecimal MaxValue
        {
            get { return this._MaxValue; }
            set
            {
                if (this._MaxValue != value)
                {
                    this._MaxValue = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// MinPurchase
        /// </summary>
        public LSDecimal MinPurchase
        {
            get { return this._MinPurchase; }
            set
            {
                if (this._MinPurchase != value)
                {
                    this._MinPurchase = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// MinQuantity
        /// </summary>
        public Int16 MinQuantity
        {
            get { return this._MinQuantity; }
            set
            {
                if (this._MinQuantity != value)
                {
                    this._MinQuantity = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// MaxQuantity
        /// </summary>
        public Int16 MaxQuantity
        {
            get { return this._MaxQuantity; }
            set
            {
                if (this._MaxQuantity != value)
                {
                    this._MaxQuantity = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// QuantityInterval
        /// </summary>
        public Int16 QuantityInterval
        {
            get { return this._QuantityInterval; }
            set
            {
                if (this._QuantityInterval != value)
                {
                    this._QuantityInterval = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// MaxUses
        /// </summary>
        public Int16 MaxUses
        {
            get { return this._MaxUses; }
            set
            {
                if (this._MaxUses != value)
                {
                    this._MaxUses = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// MaxUsesPerCustomer
        /// </summary>
        public Int16 MaxUsesPerCustomer
        {
            get { return this._MaxUsesPerCustomer; }
            set
            {
                if (this._MaxUsesPerCustomer != value)
                {
                    this._MaxUsesPerCustomer = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// StartDate
        /// </summary>
        public DateTime StartDate
        {
            get { return this._StartDate; }
            set
            {
                if (this._StartDate != value)
                {
                    this._StartDate = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// EndDate
        /// </summary>
        public DateTime EndDate
        {
            get { return this._EndDate; }
            set
            {
                if (this._EndDate != value)
                {
                    this._EndDate = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// ProductRuleId
        /// </summary>
        public Byte ProductRuleId
        {
            get { return this._ProductRuleId; }
            set
            {
                if (this._ProductRuleId != value)
                {
                    this._ProductRuleId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// AllowCombine
        /// </summary>
        public Boolean AllowCombine
        {
            get { return this._AllowCombine; }
            set
            {
                if (this._AllowCombine != value)
                {
                    this._AllowCombine = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Indicates whether this Coupon object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get
            {
                if (this._IsDirty) return true;
                if (this.BasketCouponsLoaded && this.BasketCoupons.IsDirty) return true;
                if (this.CouponGroupsLoaded && this.CouponGroups.IsDirty) return true;
                if (this.CouponProductsLoaded && this.CouponProducts.IsDirty) return true;
                if (this.CouponShipMethodsLoaded && this.CouponShipMethods.IsDirty) return true;
                return false;
            }
            set { this._IsDirty = value; }
        }

        /// <summary>
        /// Ensures that child objects of this Coupon are properly associated with this Coupon object.
        /// </summary>
        public virtual void EnsureChildProperties()
        {
            if (this.BasketCouponsLoaded) { foreach (BasketCoupon basketCoupon in this.BasketCoupons) { basketCoupon.CouponId = this.CouponId; } }
            if (this.CouponGroupsLoaded) { foreach (CouponGroup couponGroup in this.CouponGroups) { couponGroup.CouponId = this.CouponId; } }
            if (this.CouponProductsLoaded) { foreach (CouponProduct couponProduct in this.CouponProducts) { couponProduct.CouponId = this.CouponId; } }
            if (this.CouponShipMethodsLoaded) { foreach (CouponShipMethod couponShipMethod in this.CouponShipMethods) { couponShipMethod.CouponId = this.CouponId; } }
        }

#region Parents
        private Store _Store;

        /// <summary>
        /// The Store object that this Coupon object is associated with
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

#region Associations
        private BasketCouponCollection _BasketCoupons;
        private CouponGroupCollection _CouponGroups;
        private CouponProductCollection _CouponProducts;
        private CouponShipMethodCollection _CouponShipMethods;

        /// <summary>
        /// A collection of BasketCoupon objects associated with this Coupon object.
        /// </summary>
        public BasketCouponCollection BasketCoupons
        {
            get
            {
                if (!this.BasketCouponsLoaded)
                {
                    this._BasketCoupons = BasketCouponDataSource.LoadForCoupon(this.CouponId);
                }
                return this._BasketCoupons;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool BasketCouponsLoaded { get { return (this._BasketCoupons != null); } }
        /// <summary>
        /// A collection of CouponGroup objects associated with this Coupon object.
        /// </summary>
        public CouponGroupCollection CouponGroups
        {
            get
            {
                if (!this.CouponGroupsLoaded)
                {
                    this._CouponGroups = CouponGroupDataSource.LoadForCoupon(this.CouponId);
                }
                return this._CouponGroups;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool CouponGroupsLoaded { get { return (this._CouponGroups != null); } }
        /// <summary>
        /// A collection of CouponProduct objects associated with this Coupon object.
        /// </summary>
        public CouponProductCollection CouponProducts
        {
            get
            {
                if (!this.CouponProductsLoaded)
                {
                    this._CouponProducts = CouponProductDataSource.LoadForCoupon(this.CouponId);
                }
                return this._CouponProducts;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool CouponProductsLoaded { get { return (this._CouponProducts != null); } }
        /// <summary>
        /// A collection of CouponShipMethod objects associated with this Coupon object.
        /// </summary>
        public CouponShipMethodCollection CouponShipMethods
        {
            get
            {
                if (!this.CouponShipMethodsLoaded)
                {
                    this._CouponShipMethods = CouponShipMethodDataSource.LoadForCoupon(this.CouponId);
                }
                return this._CouponShipMethods;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool CouponShipMethodsLoaded { get { return (this._CouponShipMethods != null); } }
#endregion

        /// <summary>
        /// Deletes this Coupon object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_Coupons");
            deleteQuery.Append(" WHERE CouponId = @couponId");
            Database database = Token.Instance.Database;

            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null);
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@CouponId", System.Data.DbType.Int32, this.CouponId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();

            return (recordsAffected > 0);
        }


        /// <summary>
        /// Load this Coupon object from the database for the given primary key.
        /// </summary>
        /// <param name="couponId">Value of CouponId of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 couponId)
        {
            bool result = false;
            this.CouponId = couponId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Coupons");
            selectQuery.Append(" WHERE CouponId = @couponId");
            selectQuery.Append(" AND StoreId = @storeId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@couponId", System.Data.DbType.Int32, couponId);
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
        /// Saves this Coupon object to the database.
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save()
        {
            if (this.IsDirty)
            {
                Database database = Token.Instance.Database;
                MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
                bool recordExists = true;
                
                //SET EMPTY STOREID TO CURRENT CONTEXT
                if (this.StoreId == 0) this.StoreId = Token.Instance.StoreId;
                if (this.CouponId == 0) recordExists = false;

                if (recordExists) {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) As RecordCount FROM ac_Coupons");
                    selectQuery.Append(" WHERE CouponId = @couponId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@CouponId", System.Data.DbType.Int32, this.CouponId);
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
                    updateQuery.Append("UPDATE ac_Coupons SET ");
                    updateQuery.Append("StoreId = @StoreId");
                    updateQuery.Append(", CouponTypeId = @CouponTypeId");
                    updateQuery.Append(", Name = @Name");
                    updateQuery.Append(", CouponCode = @CouponCode");
                    updateQuery.Append(", DiscountAmount = @DiscountAmount");
                    updateQuery.Append(", IsPercent = @IsPercent");
                    updateQuery.Append(", MaxValue = @MaxValue");
                    updateQuery.Append(", MinPurchase = @MinPurchase");
                    updateQuery.Append(", MinQuantity = @MinQuantity");
                    updateQuery.Append(", MaxQuantity = @MaxQuantity");
                    updateQuery.Append(", QuantityInterval = @QuantityInterval");
                    updateQuery.Append(", MaxUses = @MaxUses");
                    updateQuery.Append(", MaxUsesPerCustomer = @MaxUsesPerCustomer");
                    updateQuery.Append(", StartDate = @StartDate");
                    updateQuery.Append(", EndDate = @EndDate");
                    updateQuery.Append(", ProductRuleId = @ProductRuleId");
                    updateQuery.Append(", AllowCombine = @AllowCombine");
                    updateQuery.Append(" WHERE CouponId = @CouponId");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {
                        database.AddInParameter(updateCommand, "@CouponId", System.Data.DbType.Int32, this.CouponId);
                        database.AddInParameter(updateCommand, "@StoreId", System.Data.DbType.Int32, this.StoreId);
                        database.AddInParameter(updateCommand, "@CouponTypeId", System.Data.DbType.Byte, NullableData.DbNullify(this.CouponTypeId));
                        database.AddInParameter(updateCommand, "@Name", System.Data.DbType.String, this.Name);
                        database.AddInParameter(updateCommand, "@CouponCode", System.Data.DbType.String, this.CouponCode);
                        database.AddInParameter(updateCommand, "@DiscountAmount", System.Data.DbType.Decimal, this.DiscountAmount);
                        database.AddInParameter(updateCommand, "@IsPercent", System.Data.DbType.Boolean, this.IsPercent);
                        database.AddInParameter(updateCommand, "@MaxValue", System.Data.DbType.Decimal, this.MaxValue);
                        database.AddInParameter(updateCommand, "@MinPurchase", System.Data.DbType.Decimal, this.MinPurchase);
                        database.AddInParameter(updateCommand, "@MinQuantity", System.Data.DbType.Int16, this.MinQuantity);
                        database.AddInParameter(updateCommand, "@MaxQuantity", System.Data.DbType.Int16, this.MaxQuantity);
                        database.AddInParameter(updateCommand, "@QuantityInterval", System.Data.DbType.Int16, this.QuantityInterval);
                        database.AddInParameter(updateCommand, "@MaxUses", System.Data.DbType.Int16, this.MaxUses);
                        database.AddInParameter(updateCommand, "@MaxUsesPerCustomer", System.Data.DbType.Int16, this.MaxUsesPerCustomer);
                        database.AddInParameter(updateCommand, "@StartDate", System.Data.DbType.DateTime, NullableData.DbNullify(LocaleHelper.FromLocalTime(this.StartDate)));
                        database.AddInParameter(updateCommand, "@EndDate", System.Data.DbType.DateTime, NullableData.DbNullify(LocaleHelper.FromLocalTime(this.EndDate)));
                        database.AddInParameter(updateCommand, "@ProductRuleId", System.Data.DbType.Byte, this.ProductRuleId);
                        database.AddInParameter(updateCommand, "@AllowCombine", System.Data.DbType.Boolean, this.AllowCombine);
                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_Coupons (StoreId, CouponTypeId, Name, CouponCode, DiscountAmount, IsPercent, MaxValue, MinPurchase, MinQuantity, MaxQuantity, QuantityInterval, MaxUses, MaxUsesPerCustomer, StartDate, EndDate, ProductRuleId, AllowCombine)");
                    insertQuery.Append(" VALUES (@StoreId, @CouponTypeId, @Name, @CouponCode, @DiscountAmount, @IsPercent, @MaxValue, @MinPurchase, @MinQuantity, @MaxQuantity, @QuantityInterval, @MaxUses, @MaxUsesPerCustomer, @StartDate, @EndDate, @ProductRuleId, @AllowCombine)");
                    insertQuery.Append("; SELECT Scope_Identity()");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@CouponId", System.Data.DbType.Int32, this.CouponId);
                        database.AddInParameter(insertCommand, "@StoreId", System.Data.DbType.Int32, this.StoreId);
                        database.AddInParameter(insertCommand, "@CouponTypeId", System.Data.DbType.Byte, NullableData.DbNullify(this.CouponTypeId));
                        database.AddInParameter(insertCommand, "@Name", System.Data.DbType.String, this.Name);
                        database.AddInParameter(insertCommand, "@CouponCode", System.Data.DbType.String, this.CouponCode);
                        database.AddInParameter(insertCommand, "@DiscountAmount", System.Data.DbType.Decimal, this.DiscountAmount);
                        database.AddInParameter(insertCommand, "@IsPercent", System.Data.DbType.Boolean, this.IsPercent);
                        database.AddInParameter(insertCommand, "@MaxValue", System.Data.DbType.Decimal, this.MaxValue);
                        database.AddInParameter(insertCommand, "@MinPurchase", System.Data.DbType.Decimal, this.MinPurchase);
                        database.AddInParameter(insertCommand, "@MinQuantity", System.Data.DbType.Int16, this.MinQuantity);
                        database.AddInParameter(insertCommand, "@MaxQuantity", System.Data.DbType.Int16, this.MaxQuantity);
                        database.AddInParameter(insertCommand, "@QuantityInterval", System.Data.DbType.Int16, this.QuantityInterval);
                        database.AddInParameter(insertCommand, "@MaxUses", System.Data.DbType.Int16, this.MaxUses);
                        database.AddInParameter(insertCommand, "@MaxUsesPerCustomer", System.Data.DbType.Int16, this.MaxUsesPerCustomer);
                        database.AddInParameter(insertCommand, "@StartDate", System.Data.DbType.DateTime, NullableData.DbNullify(LocaleHelper.FromLocalTime(this.StartDate)));
                        database.AddInParameter(insertCommand, "@EndDate", System.Data.DbType.DateTime, NullableData.DbNullify(LocaleHelper.FromLocalTime(this.EndDate)));
                        database.AddInParameter(insertCommand, "@ProductRuleId", System.Data.DbType.Byte, this.ProductRuleId);
                        database.AddInParameter(insertCommand, "@AllowCombine", System.Data.DbType.Boolean, this.AllowCombine);
                        //RESULT IS NEW IDENTITY;
                        result = AlwaysConvert.ToInt(database.ExecuteScalar(insertCommand));
                        this._CouponId = result;
                    }
                }
                this.SaveChildren();
                MakerShop.Stores.AuditEventDataSource.AuditInfoEnd(); 
                //OBJECT IS DIRTY IF NO RECORDS WERE UPDATED OR INSERTED
                this.IsDirty = (result == 0);
                if (this.IsDirty) { return SaveResult.Failed; }
                else { return (recordExists ? SaveResult.RecordUpdated : SaveResult.RecordInserted); }
            }

            //SAVE IS SUCCESSFUL IF OBJECT IS NOT DIRTY
            return SaveResult.NotDirty;
        }

        /// <summary>
        /// Saves that child objects associated with this Coupon object.
        /// </summary>
        public virtual void SaveChildren()
        {
            this.EnsureChildProperties();
            if (this.BasketCouponsLoaded) this.BasketCoupons.Save();
            if (this.CouponGroupsLoaded) this.CouponGroups.Save();
            if (this.CouponProductsLoaded) this.CouponProducts.Save();
            if (this.CouponShipMethodsLoaded) this.CouponShipMethods.Save();
        }

     }
}
