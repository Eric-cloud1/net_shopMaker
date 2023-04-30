//Generated by DataSourceBaseGenerator_AssnWithColumns

using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
namespace MakerShop.Users
{

    /// <summary>
    /// DataSource class for UserGroup objects
    /// </summary>
    public partial class UserGroupDataSource
    {
        /// <summary>
        /// Deletes a UserGroup object from the database
        /// </summary>
        /// <param name="userGroup">The UserGroup object to delete</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static bool Delete(UserGroup userGroup)
        {
            return userGroup.Delete();
        }

        /// <summary>
        /// Deletes a UserGroup object with given id from the database
        /// </summary>
        /// <param name="userId">Value of UserId of the object to delete.</param>
        /// <param name="groupId">Value of GroupId of the object to delete.</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        public static bool Delete(Int32 userId, Int32 groupId)
        {
            UserGroup userGroup = new UserGroup();
            if (userGroup.Load(userId, groupId)) return userGroup.Delete();
            return false;
        }

        /// <summary>
        /// Inserts/Saves a UserGroup object to the database.
        /// </summary>
        /// <param name="userGroup">The UserGroup object to insert</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Insert(UserGroup userGroup) { return userGroup.Save(); }

        /// <summary>
        /// Load a UserGroup object for the given primary key from the database.
        /// </summary>
        /// <param name="userId">Value of UserId of the object to load.</param>
        /// <param name="groupId">Value of GroupId of the object to load.</param>
        /// <returns>The loaded UserGroup object.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UserGroup Load(Int32 userId, Int32 groupId)
        {
            UserGroup userGroup = null;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + UserGroup.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_UserGroups");
            selectQuery.Append(" WHERE UserId = @userId AND GroupId = @groupId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@userId", System.Data.DbType.Int32, userId);
            database.AddInParameter(selectCommand, "@groupId", System.Data.DbType.Int32, groupId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                if (dr.Read())
                {
                    userGroup = new UserGroup();
                    UserGroup.LoadDataReader(userGroup, dr);
                }
                dr.Close();
            }
            return userGroup;
        }

        /// <summary>
        /// Loads a collection of UserGroup objects for the given criteria for Group from the database.
        /// </summary>
        /// <param name="groupId">Value of GroupId of the object to load.</param>
        /// <returns>A collection of UserGroup objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UserGroupCollection LoadForGroup(Int32 groupId)
        {
            UserGroupCollection UserGroups = new UserGroupCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT!
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + UserGroup.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_UserGroups");
            selectQuery.Append(" WHERE GroupId = @GroupId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@groupId", System.Data.DbType.Int32, groupId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    UserGroup userGroup = new UserGroup();
                    UserGroup.LoadDataReader(userGroup, dr);
                    UserGroups.Add(userGroup);
                }
                dr.Close();
            }
            return UserGroups;
        }

        /// <summary>
        /// Loads a collection of UserGroup objects for the given criteria for Subscription from the database.
        /// </summary>
        /// <param name="subscriptionId">Value of SubscriptionId of the object to load.</param>
        /// <returns>A collection of UserGroup objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UserGroupCollection LoadForSubscription(Int32 subscriptionId)
        {
            UserGroupCollection UserGroups = new UserGroupCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT!
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + UserGroup.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_UserGroups");
            selectQuery.Append(" WHERE SubscriptionId = @SubscriptionId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@subscriptionId", System.Data.DbType.Int32, NullableData.DbNullify(subscriptionId));
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    UserGroup userGroup = new UserGroup();
                    UserGroup.LoadDataReader(userGroup, dr);
                    UserGroups.Add(userGroup);
                }
                dr.Close();
            }
            return UserGroups;
        }

        /// <summary>
        /// Loads a collection of UserGroup objects for the given criteria for User from the database.
        /// </summary>
        /// <param name="userId">Value of UserId of the object to load.</param>
        /// <returns>A collection of UserGroup objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UserGroupCollection LoadForUser(Int32 userId)
        {
            UserGroupCollection UserGroups = new UserGroupCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT!
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + UserGroup.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_UserGroups");
            selectQuery.Append(" WHERE UserId = @UserId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@userId", System.Data.DbType.Int32, userId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    UserGroup userGroup = new UserGroup();
                    UserGroup.LoadDataReader(userGroup, dr);
                    UserGroups.Add(userGroup);
                }
                dr.Close();
            }
            return UserGroups;
        }

        /// <summary>
        /// Updates/Saves the given UserGroup object to the database.
        /// </summary>
        /// <param name="userGroup">The UserGroup object to update/save to database.</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Update)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Update(UserGroup userGroup) { return userGroup.Save(); }

    }
}