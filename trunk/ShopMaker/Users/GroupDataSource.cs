using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;

namespace MakerShop.Users
{
    [DataObject(true)]
    public partial class GroupDataSource
    {
        /// <summary>
        /// Makes sure that the default groups expected for a store are available
        /// in the database
        /// </summary>
        internal static void EnsureDefaultGroups()
        {
            Dictionary<string, string> groupRoles = new Dictionary<string, string>();
            groupRoles.Add("Super Users", "System");
            groupRoles.Add("Admins", "Admin");
            groupRoles.Add("Jr. Admins", "Jr. Admin");
            groupRoles.Add("Catalog Admins", "Manage Catalog");
            groupRoles.Add("Order Admins", "Manage Orders");
            groupRoles.Add("Website Admins", "Manage Website");
            groupRoles.Add("Report Admins", "View Reports");
            foreach (string groupName in groupRoles.Keys)
            {
                //MAKE SURE GROUP EXISTS
                MakerShop.Users.Group group = GroupDataSource.LoadForName(groupName);
                if (group == null)
                {
                    group = new MakerShop.Users.Group();
                    group.Name = groupName;
                    group.IsReadOnly = (group.Name == "Super Users");
                    group.Save();
                }
                //MAKE SURE GROUP HAS APPROPRIATE ROLE ASSIGNED
                string roleName = groupRoles[groupName];
                int roleId = RoleDataSource.GetIdByName(roleName);
                if (roleId > 0)
                {
                    if (group.GroupRoles.IndexOf(group.GroupId, roleId) < 0)
                    {
                        group.GroupRoles.Add(new GroupRole(group.GroupId, roleId));
                        group.Save();
                    }
                }
            }
        }

        /// <summary>
        /// Loads a group instance given the name.
        /// </summary>
        /// <param name="name">The case-insensitive group name.</param>
        /// <returns>The group instance, or null if the group is not found.</returns>
        public static Group LoadForName(string name)
        {
            int groupId = GroupDataSource.GetIdByName(name);
            return GroupDataSource.Load(groupId);
        }

        /// <summary>
        /// Gets the ID of a group given the name.
        /// </summary>
        /// <param name="name">The case-insensitive group name.</param>
        /// <returns>The int ID that represents the group, or 0 if the group is not found. </returns>
        public static int GetIdByName(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            Database database = Token.Instance.Database;
            DbCommand loadCommand = database.GetSqlStringCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT GroupId FROM ac_Groups WHERE LOWER(Name) = @name");
            database.AddInParameter(loadCommand, "@name", DbType.String, name.ToLowerInvariant());
            return AlwaysConvert.ToInt(database.ExecuteScalar(loadCommand));
        }

        /// <summary>
        /// Deletes a group from the database
        /// </summary>
        /// <param name="groupId">The group ID to delete</param>
        /// <param name="newUserGroupId">The new group ID to assign to existing members
        /// of the group to be deleted</param>
        public static void Delete(int groupId, int newUserGroupId)
        {
            // VALIDATE THE GROUP TO DELETE EXISTS IN THE DATABASE
            Group groupToDelete = GroupDataSource.Load(groupId);
            if (groupToDelete == null) throw new ArgumentException("The group to delete is invalid.", "groupId");

            // DEFINE SHARED VARIABLES
            Database database = Token.Instance.Database;
            string sql;

            // CHECK WHETHER WE NEED TO MOVE USERS
            if (newUserGroupId > 0)
            {
                // USER MOVE SPECIFIED, VALIDATE THE TARGET GROUP EXISTS
                if (GroupDataSource.Load(newUserGroupId) == null) throw new ArgumentException("The target group for existing members is invalid.", "newUserGroupId");

                // TARGET GROUP IS VALID, MOVE THE USERS WHILE MAINTAINING ANY SUBSCRIPTION ASSOCIATIONS
                using (DbCommand updateCommand = database.GetStoredProcCommand("acsp_MergeUserGroupMembers"))
                {
                    database.AddInParameter(updateCommand, "@sourceGroupId", DbType.Int32, groupToDelete.GroupId);
                    database.AddInParameter(updateCommand, "@targetGroupId", DbType.Int32, newUserGroupId);
                    database.ExecuteNonQuery(updateCommand);
                }

                // UPDATE EXISTING SUBSCRIPTIONS TO POINT TO THE NEW GROUP
                sql = "UPDATE ac_Subscriptions SET GroupId = @targetGroupId WHERE GroupId = @sourceGroupId";
                using (DbCommand updateCommand = database.GetSqlStringCommand(sql))
                {
                    database.AddInParameter(updateCommand, "@sourceGroupId", DbType.Int32, groupToDelete.GroupId);
                    database.AddInParameter(updateCommand, "@targetGroupId", DbType.Int32, newUserGroupId);
                    database.ExecuteNonQuery(updateCommand);
                }
            }

            // ANY EXISTING SUBSCRIPTIONS POINTING TO DELETING GROUP NEED TO BE NULLIFIED
            sql = "UPDATE ac_Subscriptions SET GroupId = NULL WHERE GroupId = @sourceGroupId";
            using (DbCommand updateCommand = database.GetSqlStringCommand(sql))
            {
                database.AddInParameter(updateCommand, "@sourceGroupId", DbType.Int32, groupToDelete.GroupId);
                database.ExecuteNonQuery(updateCommand);
            }

            // DELETE THE GROUP
            groupToDelete.Delete();
        }
    }
}
