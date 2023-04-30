using System.Text;
using MakerShop.Data;
using MakerShop.Common;
using System.Data.Common;
using System.Data;
namespace MakerShop.Users
{
    public partial class UserGroup
    {
        /// <summary>
        /// Recalculate and update the expiration for UserGroup
        /// </summary>
        /// <param name="userId">UserId value</param>
        /// <param name="groupId">GroupId value</param>
        public static void RecalculateExpiration(int userId, int groupId)
        {
            if (userId == 0 || groupId == 0)
            {
                return;
            }

            UserGroup userGroup = UserGroupDataSource.Load(userId, groupId);
            if (userGroup != null && userGroup.SubscriptionId == 0)
            {
                // WE WILL NOT UPDATE A USER GROUP FOR WHICH THE SUBSCRIPTION ID IS NULL OR ZERO
                return;
            }

            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT TOP 1 SubscriptionId ");
            selectQuery.Append(" FROM ac_Subscriptions");
            selectQuery.Append(" WHERE UserId = @userId AND GroupId = @groupId");
            selectQuery.Append(" AND IsActive = 1"); // CONSIDER ONLY ACTIVE SUBSCRIPTIONS
            selectQuery.Append(" ORDER BY CASE WHEN ExpirationDate IS NULL THEN 1 ELSE 0 END, ExpirationDate DESC");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@userId", System.Data.DbType.Int32, userId);
            database.AddInParameter(selectCommand, "@groupId", System.Data.DbType.Int32, groupId);

            //EXECUTE THE COMMAND
            int newSubscriptionId = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                if (dr.Read())
                {
                    newSubscriptionId = dr.GetInt32(0);
                }
                dr.Close();
            }
            
            //UPDATE
            if (newSubscriptionId > 0)
            {
                if (userGroup == null) userGroup = new UserGroup(userId, groupId);
                userGroup.SubscriptionId = newSubscriptionId;
                userGroup.Save();
            }
            else if(userGroup != null)
            {
                // DELETE USER GROUP ASSOCIATION RECORD AS SUBSCRIPTION IS ALREADY EXPIRED
                userGroup.Delete();
            }
        }

    }
}
