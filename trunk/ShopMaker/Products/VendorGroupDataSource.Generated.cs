//Generated by DataSourceBaseGenerator_Assn

using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
namespace MakerShop.Products
{

    /// <summary>
    /// DataSource class for VendorGroup objects
    /// </summary>
    public partial class VendorGroupDataSource
    {
        /// <summary>
        /// Deletes a VendorGroup object from the database
        /// </summary>
        /// <param name="vendorGroup">The VendorGroup object to delete</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static bool Delete(VendorGroup vendorGroup)
        {
            return vendorGroup.Delete();
        }

        /// <summary>
        /// Deletes a VendorGroup object with given id from the database
        /// </summary>
        /// <param name="vendorId">Value of VendorId of the object to delete.</param>
        /// <param name="groupId">Value of GroupId of the object to delete.</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        public static bool Delete(Int32 vendorId, Int32 groupId)
        {
            VendorGroup vendorGroup = new VendorGroup();
            if (vendorGroup.Load(vendorId, groupId)) return vendorGroup.Delete();
            return false;
        }

        /// <summary>
        /// Inserts/Saves a VendorGroup object to the database.
        /// </summary>
        /// <param name="vendorGroup">The VendorGroup object to insert</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Insert(VendorGroup vendorGroup) { return vendorGroup.Save(); }

        /// <summary>
        /// Load a VendorGroup object for the given primary key from the database.
        /// </summary>
        /// <param name="vendorId">Value of VendorId of the object to load.</param>
        /// <param name="groupId">Value of GroupId of the object to load.</param>
        /// <returns>The loaded VendorGroup object.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VendorGroup Load(Int32 vendorId, Int32 groupId)
        {
            VendorGroup vendorGroup = new VendorGroup();
            vendorGroup.VendorId = vendorId;
            vendorGroup.GroupId = groupId;
            vendorGroup.IsDirty = false;
            return vendorGroup;
        }

        /// <summary>
        /// Loads a collection of VendorGroup objects for the given criteria for Group from the database.
        /// </summary>
        /// <param name="groupId">Value of GroupId of the object to load.</param>
        /// <returns>A collection of VendorGroup objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VendorGroupCollection LoadForGroup(Int32 groupId)
        {
            VendorGroupCollection VendorGroups = new VendorGroupCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT VendorId");
            selectQuery.Append(" FROM ac_VendorGroups");
            selectQuery.Append(" WHERE GroupId = @groupId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@groupId", System.Data.DbType.Int32, groupId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    VendorGroup vendorGroup = new VendorGroup();
                    vendorGroup.GroupId = groupId;
                    vendorGroup.VendorId = dr.GetInt32(0);
                    VendorGroups.Add(vendorGroup);
                }
                dr.Close();
            }
            return VendorGroups;
        }

        /// <summary>
        /// Loads a collection of VendorGroup objects for the given criteria for Vendor from the database.
        /// </summary>
        /// <param name="vendorId">Value of VendorId of the object to load.</param>
        /// <returns>A collection of VendorGroup objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static VendorGroupCollection LoadForVendor(Int32 vendorId)
        {
            VendorGroupCollection VendorGroups = new VendorGroupCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT GroupId");
            selectQuery.Append(" FROM ac_VendorGroups");
            selectQuery.Append(" WHERE VendorId = @vendorId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@vendorId", System.Data.DbType.Int32, vendorId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    VendorGroup vendorGroup = new VendorGroup();
                    vendorGroup.VendorId = vendorId;
                    vendorGroup.GroupId = dr.GetInt32(0);
                    VendorGroups.Add(vendorGroup);
                }
                dr.Close();
            }
            return VendorGroups;
        }

        /// <summary>
        /// Updates/Saves the given VendorGroup object to the database.
        /// </summary>
        /// <param name="vendorGroup">The VendorGroup object to update/save to database.</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Update)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Update(VendorGroup vendorGroup) { return vendorGroup.Save(); }

    }
}
