using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using MakerShop.Common;
using MakerShop.Data;
using System.Data;
using System.Data.Common;

namespace MakerShop.Users
{
    [DataObject(true)]
    public partial class WishlistDataSource
    {
        /// <summary>
        /// Search wishlists matching the given name
        /// </summary>
        /// <param name="name">The name to match</param>
        /// <param name="location">Not used</param>
        /// <returns>Array of WishList objects matching the given name</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public Wishlist[] Search(string name, string location)
        {
            if (string.IsNullOrEmpty(name)) return null;
            List<Wishlist> WishlistList= new List<Wishlist>();
            StringBuilder selectQuery = new StringBuilder();
            Database database = Token.Instance.Database;
            DbCommand selectCommand;
            if (Regex.IsMatch(name, "^([0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$"))
            {
                //GET RECORDS STARTING AT FIRST ROW
                selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT W.WishlistId, W.UserId, W.Name, W.ViewPassword");
                selectQuery.Append(" FROM ac_Wishlists W, ac_Users U");
                selectQuery.Append(" WHERE W.UserId = U.UserId");
                selectQuery.Append(" AND U.Username = @email");
                selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
                database.AddInParameter(selectCommand, "@email", System.Data.DbType.String, name);
            } else {
                string firstName = string.Empty;
                string lastName = string.Empty;
                selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT W.WishlistId, W.UserId, W.Name, W.ViewPassword");
                selectQuery.Append(" FROM ac_Wishlists W, ac_Addresses A");
                selectQuery.Append(" WHERE W.UserId = A.UserId");
                selectQuery.Append(" AND A.LastName = @lastName");
                if (name.Contains(" "))
                {
                    int spaceIndex = name.IndexOf(" ");
                    firstName = name.Substring(0, spaceIndex);
                    lastName = name.Substring(spaceIndex + 1);
                    selectQuery.Append(" AND A.FirstName = @firstName");
                }
                else
                {
                    lastName = name;
                }
                selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
                database.AddInParameter(selectCommand, "@lastName", System.Data.DbType.String, lastName);
                if (!string.IsNullOrEmpty(firstName)) database.AddInParameter(selectCommand, "@firstName", System.Data.DbType.String, firstName);
            }
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    Wishlist wishlist = new Wishlist();
                    //SET FIELDS FROM ROW DATA
                    wishlist.WishlistId = dr.GetInt32(0);
                    wishlist.UserId = dr.GetInt32(1);
                    wishlist.Name = NullableData.GetString(dr, 2);
                    wishlist.ViewPassword = NullableData.GetString(dr, 3);
                    wishlist.IsDirty = false;
                    WishlistList.Add(wishlist);
                }
                dr.Close();
            }
            return WishlistList.ToArray();
        }
    }
}
