using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using System.ComponentModel;
using MakerShop.Users;
using System.Web.UI.WebControls.WebParts;

namespace MakerShop.Personalization
{
    /// <summary>
    /// DataSource class for UserPersonalization objects
    /// </summary>
    [DataObject(true)]
    public partial class UserPersonalizationDataSource
    {
        /// <summary>
        /// Loads UserPersonalization object for given path and user name
        /// </summary>
        /// <param name="path">The path to load the UserPersonalization object</param>
        /// <param name="userName">The user name</param>
        /// <param name="create">If <b>ture</b> new object is created if it does not already exist</param>
        /// <returns>UserPersonalization object</returns>
        public static UserPersonalization LoadForPath(string path, string userName, bool create)
        {
            UserPersonalization userPersonalization = null;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + UserPersonalization.GetColumnNames("acUP"));
            selectQuery.Append(" FROM ac_UserPersonalization acUP, ac_PersonalizationPaths acP, ac_Users acU");
            selectQuery.Append(" WHERE acUP.PersonalizationPathId = acP.PersonalizationPathId AND acUP.UserId = acU.UserId");
            selectQuery.Append(" AND acP.Path = @path");
            selectQuery.Append(" AND acP.StoreId = @storeId");
            selectQuery.Append(" AND acU.UserName = @username");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@path", System.Data.DbType.String, path);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@username", System.Data.DbType.String, userName);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                if (dr.Read())
                {
                    userPersonalization = new UserPersonalization();
                    UserPersonalization.LoadDataReader(userPersonalization, dr);
                }
                dr.Close();
            }
            if ((userPersonalization == null) && create)
            {
                User user = UserDataSource.LoadForUserName(userName);
                if (user == null) throw new PersonalizationProviderException("Invalid userName");
                userPersonalization = new UserPersonalization(PersonalizationPathDataSource.LoadForPath(path, true), user);
            }
            return userPersonalization;
        }

        /// <summary>
        /// Determines how many users have made personalization for a given path
        /// </summary>
        /// <param name="path">the path to check</param>
        /// <param name="totalSize">total size of personalization</param>
        /// <param name="numberOfUsers">number of users with personalization data</param>
        public static void CountForPath(string path, out int totalSize, out int numberOfUsers)
        {
            //INITIALIZE COUNTS
            totalSize = 0;
            numberOfUsers = 0;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT SUM(DataLength(PageSettings)) As TotalSize, COUNT(1) As TotalCount");
            selectQuery.Append(" FROM ac_UserPersonalization U, ac_PersonalizationPaths P");
            selectQuery.Append(" WHERE U.PersonalizationPathId = P.PersonalizationPathId");
            selectQuery.Append(" AND P.StoreId = @storeId");
            selectQuery.Append(" AND P.Path = @path");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@path", System.Data.DbType.String, path);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                if (dr.Read())
                {
                    totalSize = dr.GetInt32(0);
                    numberOfUsers = dr.GetInt32(1);
                }
                dr.Close();
            }
        }

        /// <summary>
        /// Gets a collection of PersonalizationStateInfo objects for the given PersonalizationStateQuery
        /// </summary>
        /// <param name="query">PersonalizationStateQuery</param>
        /// <param name="pageIndex">Index of the page to start retrieving</param>
        /// <param name="pageSize">Size of the page to retrieve</param>
        /// <param name="totalRecords">Returns total number of records retrieved in this parameter</param>
        /// <returns></returns>
        public static PersonalizationStateInfoCollection FindState(PersonalizationStateQuery query, int pageIndex, int pageSize, out int totalRecords)
        {
            PersonalizationStateInfoCollection results = new PersonalizationStateInfoCollection();
            //USERNAMES ARE NOT ASSOCIATED WITH SHARED DATA
            //IF A USERNAME WAS SPECIFIED, RETURN AN EMPTY COLLECTION
            totalRecords = 0;
            if (!string.IsNullOrEmpty(query.UsernameToMatch)) return results;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            //CHECK WHETHER TO LOAD ALL PATHS, OR TO FILTER
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + UserPersonalization.GetColumnNames("UP") + ",P.Path,U.UserName");
            selectQuery.Append(" FROM ac_UserPersonalization UP, ac_PersonalizationPaths P, ac_Users U");
            selectQuery.Append(" WHERE U.PersonalizationPathId = P.PersonalizationPathId AND UP.UserId = U.UserId AND P.StoreId = @storeId");
            if (!string.IsNullOrEmpty(query.PathToMatch)) selectQuery.Append(" AND P.Path LIKE @pathToMatch");
            if (!string.IsNullOrEmpty(query.UsernameToMatch)) selectQuery.Append(" AND U.UserName LIKE @usernameToMatch");
            selectQuery.Append(" ORDER BY P.Path");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            if (!string.IsNullOrEmpty(query.PathToMatch)) database.AddInParameter(selectCommand, "@pathToMatch", System.Data.DbType.String, query.PathToMatch);
            if (!string.IsNullOrEmpty(query.UsernameToMatch)) database.AddInParameter(selectCommand, "@usernameToMatch", System.Data.DbType.String, query.UsernameToMatch);
            //EXECUTE THE COMMAND
            int startRowIndex = (pageIndex * pageSize);
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((rowCount < pageSize)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        UserPersonalization p = new UserPersonalization();
                        UserPersonalization.LoadDataReader(p, dr);
                        UserPersonalizationStateInfo i = new UserPersonalizationStateInfo(dr.GetString(6), p.LastUpdatedDate, p.PageSettings.Length, dr.GetString(7), System.DateTime.MinValue);
                        results.Add(i);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            totalRecords = rowCount;
            return results;
        }

    }
}
