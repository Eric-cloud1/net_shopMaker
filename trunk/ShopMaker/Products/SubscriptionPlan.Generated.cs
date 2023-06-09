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
using MakerShop.Products;
using MakerShop.Taxes;
using MakerShop.Users;
using MakerShop.Utility;

namespace MakerShop.Products
{
    /// <summary>
    /// This class represents a SubscriptionPlan object in the database.
    /// </summary>
    public partial class SubscriptionPlan : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SubscriptionPlan() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="productId">Value of ProductId.</param>
        /// </summary>
        public SubscriptionPlan(Int32 productId)
        {
            this.ProductId = productId;
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
          columnNames.Add(prefix + "ProductId");
          columnNames.Add(prefix + "Name");
          columnNames.Add(prefix + "NumberOfPayments");
          columnNames.Add(prefix + "PaymentFrequency");
          columnNames.Add(prefix + "PaymentFrequencyUnitId");
          columnNames.Add(prefix + "RecurringCharge");
          columnNames.Add(prefix + "RecurringChargeSpecified");
          columnNames.Add(prefix + "GroupId");
          columnNames.Add(prefix + "TaxCodeId");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given SubscriptionPlan object from the given database data reader.
        /// </summary>
        /// <param name="subscriptionPlan">The SubscriptionPlan object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(SubscriptionPlan subscriptionPlan, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            subscriptionPlan.ProductId = dr.GetInt32(0);
            subscriptionPlan.Name = dr.GetString(1);
            subscriptionPlan.NumberOfPayments = dr.GetInt16(2);
            subscriptionPlan.PaymentFrequency = dr.GetInt16(3);
            subscriptionPlan.PaymentFrequencyUnitId = dr.GetByte(4);
            subscriptionPlan.RecurringCharge = dr.GetDecimal(5);
            subscriptionPlan.RecurringChargeSpecified = dr.GetBoolean(6);
            subscriptionPlan.GroupId = NullableData.GetInt32(dr, 7);
            subscriptionPlan.TaxCodeId = NullableData.GetInt32(dr, 8);
            subscriptionPlan.IsDirty = false;
        }

#endregion

        private Int32 _ProductId;
        private String _Name = string.Empty;
        private Int16 _NumberOfPayments;
        private Int16 _PaymentFrequency;
        private Byte _PaymentFrequencyUnitId;
        private LSDecimal _RecurringCharge;
        private Boolean _RecurringChargeSpecified;
        private Int32 _GroupId;
        private Int32 _TaxCodeId;
        private bool _IsDirty;

        /// <summary>
        /// ProductId
        /// </summary>
        [DataObjectField(true, true, false)]
        public Int32 ProductId
        {
            get { return this._ProductId; }
            set
            {
                if (this._ProductId != value)
                {
                    this._ProductId = value;
                    this.IsDirty = true;
                    this.EnsureChildProperties();
                    this._Product = null;
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
        /// NumberOfPayments
        /// </summary>
        public Int16 NumberOfPayments
        {
            get { return this._NumberOfPayments; }
            set
            {
                if (this._NumberOfPayments != value)
                {
                    this._NumberOfPayments = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// PaymentFrequency
        /// </summary>
        public Int16 PaymentFrequency
        {
            get { return this._PaymentFrequency; }
            set
            {
                if (this._PaymentFrequency != value)
                {
                    this._PaymentFrequency = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// PaymentFrequencyUnitId
        /// </summary>
        public Byte PaymentFrequencyUnitId
        {
            get { return this._PaymentFrequencyUnitId; }
            set
            {
                if (this._PaymentFrequencyUnitId != value)
                {
                    this._PaymentFrequencyUnitId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// RecurringCharge
        /// </summary>
        public LSDecimal RecurringCharge
        {
            get { return this._RecurringCharge; }
            set
            {
                if (this._RecurringCharge != value)
                {
                    this._RecurringCharge = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// RecurringChargeSpecified
        /// </summary>
        public Boolean RecurringChargeSpecified
        {
            get { return this._RecurringChargeSpecified; }
            set
            {
                if (this._RecurringChargeSpecified != value)
                {
                    this._RecurringChargeSpecified = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// GroupId
        /// </summary>
        public Int32 GroupId
        {
            get { return this._GroupId; }
            set
            {
                if (this._GroupId != value)
                {
                    this._GroupId = value;
                    this.IsDirty = true;
                    this._Group = null;
                }
            }
        }

        /// <summary>
        /// TaxCodeId
        /// </summary>
        public Int32 TaxCodeId
        {
            get { return this._TaxCodeId; }
            set
            {
                if (this._TaxCodeId != value)
                {
                    this._TaxCodeId = value;
                    this.IsDirty = true;
                    this._TaxCode = null;
                }
            }
        }

        /// <summary>
        /// Indicates whether this SubscriptionPlan object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get
            {
                if (this._IsDirty) return true;
                if (this.SubscriptionsLoaded && this.Subscriptions.IsDirty) return true;
                return false;
            }
            set { this._IsDirty = value; }
        }

        /// <summary>
        /// Ensures that child objects of this SubscriptionPlan are properly associated with this SubscriptionPlan object.
        /// </summary>
        public virtual void EnsureChildProperties()
        {
            if (this.SubscriptionsLoaded) { foreach (Subscription subscription in this.Subscriptions) { subscription.ProductId = this.ProductId; } }
        }

#region Parents
        private Group _Group;
        private Product _Product;
        private TaxCode _TaxCode;

        /// <summary>
        /// The Group object that this SubscriptionPlan object is associated with
        /// </summary>
        public Group Group
        {
            get
            {
                if (!this.GroupLoaded)
                {
                    this._Group = GroupDataSource.Load(this.GroupId);
                }
                return this._Group;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool GroupLoaded { get { return ((this._Group != null) && (this._Group.GroupId == this.GroupId)); } }

        /// <summary>
        /// The Product object that this SubscriptionPlan object is associated with
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

        /// <summary>
        /// The TaxCode object that this SubscriptionPlan object is associated with
        /// </summary>
        public TaxCode TaxCode
        {
            get
            {
                if (!this.TaxCodeLoaded)
                {
                    this._TaxCode = TaxCodeDataSource.Load(this.TaxCodeId);
                }
                return this._TaxCode;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool TaxCodeLoaded { get { return ((this._TaxCode != null) && (this._TaxCode.TaxCodeId == this.TaxCodeId)); } }

#endregion

#region Children
        private SubscriptionCollection _Subscriptions;

        /// <summary>
        /// A collection of Subscription objects associated with this SubscriptionPlan object.
        /// </summary>
        public SubscriptionCollection Subscriptions
        {
            get
            {
                if (!this.SubscriptionsLoaded)
                {
                    this._Subscriptions = SubscriptionDataSource.LoadForSubscriptionPlan(this.ProductId);
                }
                return this._Subscriptions;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool SubscriptionsLoaded { get { return (this._Subscriptions != null); } }

#endregion

        /// <summary>
        /// Deletes this SubscriptionPlan object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_SubscriptionPlans");
            deleteQuery.Append(" WHERE ProductId = @productId");
            Database database = Token.Instance.Database;

            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null);
            
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@ProductId", System.Data.DbType.Int32, this.ProductId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            return (recordsAffected > 0);
        }


        /// <summary>
        /// Load this SubscriptionPlan object from the database for the given primary key.
        /// </summary>
        /// <param name="productId">Value of ProductId of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 productId)
        {
            bool result = false;
            this.ProductId = productId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_SubscriptionPlans");
            selectQuery.Append(" WHERE ProductId = @productId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
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
        /// Saves this SubscriptionPlan object to the database.
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
                    selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) As RecordCount FROM ac_SubscriptionPlans");
                    selectQuery.Append(" WHERE ProductId = @productId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@ProductId", System.Data.DbType.Int32, this.ProductId);
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
                    updateQuery.Append("UPDATE ac_SubscriptionPlans SET ");
                    updateQuery.Append("Name = @Name");
                    updateQuery.Append(", NumberOfPayments = @NumberOfPayments");
                    updateQuery.Append(", PaymentFrequency = @PaymentFrequency");
                    updateQuery.Append(", PaymentFrequencyUnitId = @PaymentFrequencyUnitId");
                    updateQuery.Append(", RecurringCharge = @RecurringCharge");
                    updateQuery.Append(", RecurringChargeSpecified = @RecurringChargeSpecified");
                    updateQuery.Append(", GroupId = @GroupId");
                    updateQuery.Append(", TaxCodeId = @TaxCodeId");
                    updateQuery.Append(" WHERE ProductId = @ProductId");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {
                        database.AddInParameter(updateCommand, "@ProductId", System.Data.DbType.Int32, this.ProductId);
                        database.AddInParameter(updateCommand, "@Name", System.Data.DbType.String, this.Name);
                        database.AddInParameter(updateCommand, "@NumberOfPayments", System.Data.DbType.Int16, this.NumberOfPayments);
                        database.AddInParameter(updateCommand, "@PaymentFrequency", System.Data.DbType.Int16, this.PaymentFrequency);
                        database.AddInParameter(updateCommand, "@PaymentFrequencyUnitId", System.Data.DbType.Byte, this.PaymentFrequencyUnitId);
                        database.AddInParameter(updateCommand, "@RecurringCharge", System.Data.DbType.Decimal, this.RecurringCharge);
                        database.AddInParameter(updateCommand, "@RecurringChargeSpecified", System.Data.DbType.Boolean, this.RecurringChargeSpecified);
                        database.AddInParameter(updateCommand, "@GroupId", System.Data.DbType.Int32, NullableData.DbNullify(this.GroupId));
                        database.AddInParameter(updateCommand, "@TaxCodeId", System.Data.DbType.Int32, NullableData.DbNullify(this.TaxCodeId));
                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_SubscriptionPlans (ProductId, Name, NumberOfPayments, PaymentFrequency, PaymentFrequencyUnitId, RecurringCharge, RecurringChargeSpecified, GroupId, TaxCodeId)");
                    insertQuery.Append(" VALUES (@ProductId, @Name, @NumberOfPayments, @PaymentFrequency, @PaymentFrequencyUnitId, @RecurringCharge, @RecurringChargeSpecified, @GroupId, @TaxCodeId)");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@ProductId", System.Data.DbType.Int32, this.ProductId);
                        database.AddInParameter(insertCommand, "@Name", System.Data.DbType.String, this.Name);
                        database.AddInParameter(insertCommand, "@NumberOfPayments", System.Data.DbType.Int16, this.NumberOfPayments);
                        database.AddInParameter(insertCommand, "@PaymentFrequency", System.Data.DbType.Int16, this.PaymentFrequency);
                        database.AddInParameter(insertCommand, "@PaymentFrequencyUnitId", System.Data.DbType.Byte, this.PaymentFrequencyUnitId);
                        database.AddInParameter(insertCommand, "@RecurringCharge", System.Data.DbType.Decimal, this.RecurringCharge);
                        database.AddInParameter(insertCommand, "@RecurringChargeSpecified", System.Data.DbType.Boolean, this.RecurringChargeSpecified);
                        database.AddInParameter(insertCommand, "@GroupId", System.Data.DbType.Int32, NullableData.DbNullify(this.GroupId));
                        database.AddInParameter(insertCommand, "@TaxCodeId", System.Data.DbType.Int32, NullableData.DbNullify(this.TaxCodeId));
                        //RESULT IS NUMBER OF RECORDS AFFECTED;
                        result = database.ExecuteNonQuery(insertCommand);
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
        /// Saves that child objects associated with this SubscriptionPlan object.
        /// </summary>
        public virtual void SaveChildren()
        {
            this.EnsureChildProperties();
            if (this.SubscriptionsLoaded) this.Subscriptions.Save();
        }

     }
}
