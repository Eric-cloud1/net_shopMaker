//Generated by PersistableBaseGenerator

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Orders;
using MakerShop.Utility;

namespace MakerShop.Orders
{
    /// <summary>
    /// This class represents a OrderCoupon object in the database.
    /// </summary>
    public partial class OrderCoupon : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public OrderCoupon() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="orderId">Value of OrderId.</param>
        /// <param name="couponCode">Value of CouponCode.</param>
        /// </summary>
        public OrderCoupon(Int32 orderId, String couponCode)
        {
            this.OrderId = orderId;
            this.CouponCode = couponCode;
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
          columnNames.Add(prefix + "OrderId");
          columnNames.Add(prefix + "CouponCode");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given OrderCoupon object from the given database data reader.
        /// </summary>
        /// <param name="orderCoupon">The OrderCoupon object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(OrderCoupon orderCoupon, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            orderCoupon.OrderId = dr.GetInt32(0);
            orderCoupon.CouponCode = dr.GetString(1);
            orderCoupon.IsDirty = false;
        }

#endregion

        private Int32 _OrderId;
        private String _CouponCode = string.Empty;
        private bool _IsDirty;

        /// <summary>
        /// OrderId
        /// </summary>
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
                    this._Order = null;
                }
            }
        }

        /// <summary>
        /// CouponCode
        /// </summary>
        [DataObjectField(true, false, false)]
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
        /// Indicates whether this OrderCoupon object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }

#region Parents
        private Order _Order;

        /// <summary>
        /// The Order object that this OrderCoupon object is associated with
        /// </summary>
        public Order Order
        {
            get
            {
                if (!this.OrderLoaded)
                {
                    this._Order = OrderDataSource.Load(this.OrderId);
                }
                return this._Order;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool OrderLoaded { get { return ((this._Order != null) && (this._Order.OrderId == this.OrderId)); } }

#endregion

        /// <summary>
        /// Deletes this OrderCoupon object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_OrderCoupons");
            deleteQuery.Append(" WHERE OrderId = @orderId AND CouponCode = @couponCode");
            Database database = Token.Instance.Database;
            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@OrderId", System.Data.DbType.Int32, this.OrderId);
                database.AddInParameter(deleteCommand, "@CouponCode", System.Data.DbType.String, this.CouponCode);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            return (recordsAffected > 0);
        }

        /// <summary>
        /// Load this OrderCoupon object from the database for the given primary key.
        /// </summary>
        /// <param name="orderId">Value of OrderId of the object to load.</param>
        /// <param name="couponCode">Value of CouponCode of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 orderId, String couponCode)
        {
            this.OrderId = orderId;
            this.CouponCode = couponCode;
            this.IsDirty = false;
            return true;
        }

        /// <summary>
        /// Saves this OrderCoupon object to the database.
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
                    selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) As RecordCount FROM ac_OrderCoupons");
                    selectQuery.Append(" WHERE OrderId = @orderId AND CouponCode = @couponCode");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@OrderId", System.Data.DbType.Int32, this.OrderId);
                        database.AddInParameter(selectCommand, "@CouponCode", System.Data.DbType.String, this.CouponCode);
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
                    insertQuery.Append("INSERT INTO ac_OrderCoupons (OrderId, CouponCode)");
                    insertQuery.Append(" VALUES (@OrderId, @CouponCode)");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@OrderId", System.Data.DbType.Int32, this.OrderId);
                        database.AddInParameter(insertCommand, "@CouponCode", System.Data.DbType.String, this.CouponCode);
                        
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
