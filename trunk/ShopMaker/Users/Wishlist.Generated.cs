//Generated by PersistableBaseGenerator

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Users;
using MakerShop.Utility;

namespace MakerShop.Users
{
    /// <summary>
    /// This class represents a Wishlist object in the database.
    /// </summary>
    public partial class Wishlist : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Wishlist() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="wishlistId">Value of WishlistId.</param>
        /// </summary>
        public Wishlist(Int32 wishlistId)
        {
            this.WishlistId = wishlistId;
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
          columnNames.Add(prefix + "WishlistId");
          columnNames.Add(prefix + "UserId");
          columnNames.Add(prefix + "Name");
          columnNames.Add(prefix + "ViewPassword");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given Wishlist object from the given database data reader.
        /// </summary>
        /// <param name="wishlist">The Wishlist object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(Wishlist wishlist, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            wishlist.WishlistId = dr.GetInt32(0);
            wishlist.UserId = dr.GetInt32(1);
            wishlist.Name = NullableData.GetString(dr, 2);
            wishlist.ViewPassword = NullableData.GetString(dr, 3);
            wishlist.IsDirty = false;
        }

#endregion

        private Int32 _WishlistId;
        private Int32 _UserId;
        private String _Name = string.Empty;
        private String _ViewPassword = string.Empty;
        private bool _IsDirty;

        /// <summary>
        /// WishlistId
        /// </summary>
        [DataObjectField(true, true, false)]
        public Int32 WishlistId
        {
            get { return this._WishlistId; }
            set
            {
                if (this._WishlistId != value)
                {
                    this._WishlistId = value;
                    this.IsDirty = true;
                    this.EnsureChildProperties();
                }
            }
        }

        /// <summary>
        /// UserId
        /// </summary>
        public Int32 UserId
        {
            get { return this._UserId; }
            set
            {
                if (this._UserId != value)
                {
                    this._UserId = value;
                    this.IsDirty = true;
                    this._User = null;
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
        /// ViewPassword
        /// </summary>
        public String ViewPassword
        {
            get { return this._ViewPassword; }
            set
            {
                if (this._ViewPassword != value)
                {
                    this._ViewPassword = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Indicates whether this Wishlist object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get
            {
                if (this._IsDirty) return true;
                if (this.ItemsLoaded && this.Items.IsDirty) return true;
                return false;
            }
            set { this._IsDirty = value; }
        }

        /// <summary>
        /// Ensures that child objects of this Wishlist are properly associated with this Wishlist object.
        /// </summary>
        public virtual void EnsureChildProperties()
        {
            if (this.ItemsLoaded) { foreach (WishlistItem wishlistItem in this.Items) { wishlistItem.WishlistId = this.WishlistId; } }
        }

#region Parents
        private User _User;

        /// <summary>
        /// The User object that this Wishlist object is associated with
        /// </summary>
        public User User
        {
            get
            {
                if (!this.UserLoaded)
                {
                    this._User = UserDataSource.Load(this.UserId);
                }
                return this._User;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool UserLoaded { get { return ((this._User != null) && (this._User.UserId == this.UserId)); } }

#endregion

#region Children
        private WishlistItemCollection _Items;

        /// <summary>
        /// A collection of WishlistItem objects associated with this Wishlist object.
        /// </summary>
        public WishlistItemCollection Items
        {
            get
            {
                if (!this.ItemsLoaded)
                {
                    this._Items = WishlistItemDataSource.LoadForWishlist(this.WishlistId);
                }
                return this._Items;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool ItemsLoaded { get { return (this._Items != null); } }

#endregion

        /// <summary>
        /// Deletes this Wishlist object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_Wishlists");
            deleteQuery.Append(" WHERE WishlistId = @wishlistId");
            Database database = Token.Instance.Database;

            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null);
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@WishlistId", System.Data.DbType.Int32, this.WishlistId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            return (recordsAffected > 0);
        }


        /// <summary>
        /// Load this Wishlist object from the database for the given primary key.
        /// </summary>
        /// <param name="wishlistId">Value of WishlistId of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 wishlistId)
        {
            bool result = false;
            this.WishlistId = wishlistId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Wishlists");
            selectQuery.Append(" WHERE WishlistId = @wishlistId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@wishlistId", System.Data.DbType.Int32, wishlistId);
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
        /// Saves this Wishlist object to the database.
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        protected SaveResult BaseSave()
        {
            if (this.IsDirty)
            {
                Database database = Token.Instance.Database;
                MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
                bool recordExists = true;
                
                if (this.WishlistId == 0) recordExists = false;

                if (recordExists) {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(1) As RecordCount FROM ac_Wishlists");
                    selectQuery.Append(" WHERE WishlistId = @wishlistId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@WishlistId", System.Data.DbType.Int32, this.WishlistId);
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
                    updateQuery.Append("UPDATE ac_Wishlists SET ");
                    updateQuery.Append("UserId = @UserId");
                    updateQuery.Append(", Name = @Name");
                    updateQuery.Append(", ViewPassword = @ViewPassword");
                    updateQuery.Append(" WHERE WishlistId = @WishlistId");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {
                        database.AddInParameter(updateCommand, "@WishlistId", System.Data.DbType.Int32, this.WishlistId);
                        database.AddInParameter(updateCommand, "@UserId", System.Data.DbType.Int32, this.UserId);
                        database.AddInParameter(updateCommand, "@Name", System.Data.DbType.String, NullableData.DbNullify(this.Name));
                        database.AddInParameter(updateCommand, "@ViewPassword", System.Data.DbType.String, NullableData.DbNullify(this.ViewPassword));
                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_Wishlists (UserId, Name, ViewPassword)");
                    insertQuery.Append(" VALUES (@UserId, @Name, @ViewPassword)");
                    insertQuery.Append("; SELECT Scope_Identity()");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@WishlistId", System.Data.DbType.Int32, this.WishlistId);
                        database.AddInParameter(insertCommand, "@UserId", System.Data.DbType.Int32, this.UserId);
                        database.AddInParameter(insertCommand, "@Name", System.Data.DbType.String, NullableData.DbNullify(this.Name));
                        database.AddInParameter(insertCommand, "@ViewPassword", System.Data.DbType.String, NullableData.DbNullify(this.ViewPassword));
                        //RESULT IS NEW IDENTITY;
                        result = AlwaysConvert.ToInt(database.ExecuteScalar(insertCommand));
                        this._WishlistId = result;
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
        /// Saves that child objects associated with this Wishlist object.
        /// </summary>
        public virtual void SaveChildren()
        {
            this.EnsureChildProperties();
            if (this.ItemsLoaded) this.Items.Save();
        }

     }
}
