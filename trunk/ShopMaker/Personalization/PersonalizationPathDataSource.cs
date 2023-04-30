using System;
using System.Data;
using System.Data.Common;
using System.ComponentModel;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;

namespace MakerShop.Personalization
{
    /// <summary>
    /// DataSource class for PersonalizationPath objects
    /// </summary>
    [DataObject(true)]
    public partial class PersonalizationPathDataSource
    {
        /// <summary>
        /// Loads a PersonalizationPath object for given path value
        /// </summary>
        /// <param name="path">The path for which to load the PersonalizationPath object</param>
        /// <param name="create">If <b>true</b> new PersonalizationPath object will be created if it could not be loaded successfully.</param>
        /// <returns>A PersonalizationPath object</returns>
        public static PersonalizationPath LoadForPath(string path, bool create)
        {
            PersonalizationPath personalizationPath = null;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT PersonalizationPathId, Path");
            selectQuery.Append(" FROM ac_PersonalizationPaths");
            selectQuery.Append(" WHERE Path = @path");
            selectQuery.Append(" AND StoreId = @storeId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@path", System.Data.DbType.String, path);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                if (dr.Read())
                {
                    personalizationPath = new PersonalizationPath();
                    personalizationPath.PersonalizationPathId = dr.GetInt32(0);
                    personalizationPath.StoreId = Token.Instance.StoreId;
                    personalizationPath.Path = dr.GetString(1);
                    personalizationPath.IsDirty = false;
                    
                }
                dr.Close();
            }
            if ((personalizationPath == null) && create)
            {
                personalizationPath = new PersonalizationPath();
                personalizationPath.Path = path;
                personalizationPath.Save();
            }
            return personalizationPath;
        }
    }
}
