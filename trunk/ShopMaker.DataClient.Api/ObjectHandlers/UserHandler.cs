using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Users;
using MakerShop.Orders;
using MakerShop.Data;
using System.Data;
using System.Data.Common;
using MakerShop.Utility;
using System.Collections;
using MakerShop.Common;
namespace MakerShop.DataClient.Api.ObjectHandlers
{
    class UserHandler
    {
        
        public static Api.Schema.User[] ConvertToClientArray(UserCollection objUserCollection, bool onlyCsvData)
        {
            DataObject dataObject = new DataObject("User", typeof(MakerShop.Users.User), typeof(Api.Schema.User));

            int i = 0;
            Api.Schema.User[] arrClientApiOrder = new Api.Schema.User[objUserCollection.Count];
            Api.Schema.User objClientApiUser = null;
            String errorMessage = String.Empty;
            List<String> errors = new List<string>();
            foreach (MakerShop.Users.User objUser in objUserCollection)
            {
                if (objUser.IsAnonymous)
                {
                    // Continue, Dont export anonymous user information
                    continue;
                }
                
                errorMessage = String.Empty;

                objClientApiUser = (Api.Schema.User)dataObject.ConvertToClientApiObject(objUser, out errorMessage);
                
                if (!string.IsNullOrEmpty(errorMessage) && !errors.Contains(errorMessage))
                {
                    errors.Add(errorMessage);
                    // LOG THIS ERROR AS WELL
                    Logger.Error(errorMessage);
                }
                if (!onlyCsvData)
                {
                    // Addresses               
                    DataObject nestedDataObject = new DataObject("UserAddress", typeof(MakerShop.Users.Address), typeof(Api.Schema.Address));
                    objClientApiUser.Addresses = (Api.Schema.Address[])nestedDataObject.ConvertAC6Collection(objUser.Addresses);

                    // ReviewerProfile                  
                    nestedDataObject = new DataObject("ReviewerProfile", typeof(MakerShop.Products.ReviewerProfile), typeof(Api.Schema.ReviewerProfile));
                    objClientApiUser.ReviewerProfile = (Api.Schema.ReviewerProfile)nestedDataObject.ConvertToClientApiObject(objUser.ReviewerProfile);

                    // Passwords
                    nestedDataObject = new DataObject("Passwords", typeof(MakerShop.Users.UserPassword), typeof(Api.Schema.UserPassword));
                    objClientApiUser.Passwords = (Api.Schema.UserPassword[])nestedDataObject.ConvertAC6Collection(objUser.Passwords);

                    // UserSettings
                    nestedDataObject = new DataObject("UserSettings", typeof(MakerShop.Users.UserSetting), typeof(Api.Schema.UserSetting));
                    objClientApiUser.Settings = (Api.Schema.UserSetting[])nestedDataObject.ConvertAC6Collection(objUser.Settings);                    

                    //USER GROUPS
                    objClientApiUser.UserGroups = (Schema.UserGroup[])DataObject.ConvertToClientArray(typeof(MakerShop.Users.UserGroup), typeof(Schema.UserGroup), objUser.UserGroups);
                    

                    // WishList
                    objClientApiUser.Wishlists = getWishLists(objUser.Wishlists);


                    if (objUser.Affiliate != null)
                        objClientApiUser.Affiliate = objUser.Affiliate.Name;
                    
                    // BASKET
                    UpdateUserBaskets(objUser.Baskets, objClientApiUser);

                    // Profile                    
                    objClientApiUser.Profile = (Schema.Profile)DataObject.ConvertToClientObject(typeof(MakerShop.Personalization.Profile), typeof(Schema.Profile), objUser.Profile);

                    //UserPersonalizations
                    objClientApiUser.UserPersonalizations = (Schema.UserPersonalization[])DataObject.ConvertToClientArray(typeof(MakerShop.Personalization.UserPersonalization), typeof(Schema.UserPersonalization), objUser.UserPersonalizations);
                    
                }
                else
                {
                    objClientApiUser.FirstName = objUser.PrimaryAddress.FirstName;
                    objClientApiUser.LastName = objUser.PrimaryAddress.LastName;
                    objClientApiUser.Nickname = objUser.PrimaryAddress.Nickname;
                    objClientApiUser.Company = objUser.PrimaryAddress.Company;
                    objClientApiUser.Address1 = objUser.PrimaryAddress.Address1;
                    objClientApiUser.Address2 = objUser.PrimaryAddress.Address2;
                    objClientApiUser.City = objUser.PrimaryAddress.City;
                    objClientApiUser.Province = objUser.PrimaryAddress.Province;
                    objClientApiUser.PostalCode = objUser.PrimaryAddress.PostalCode;
                    objClientApiUser.CountryCode = objUser.PrimaryAddress.CountryCode;
                    objClientApiUser.Phone = objUser.PrimaryAddress.Phone;
                    objClientApiUser.Fax = objUser.PrimaryAddress.Fax;
                    objClientApiUser.AddressIsResidence = objUser.PrimaryAddress.Residence;

                    // SET THE PASSWORD 
                    UserPassword userPassword = UserPasswordDataSource.Load(objUser.UserId, 1);
                    if (userPassword != null) objClientApiUser.PasswordEncrypted = userPassword.Password;
                }

                arrClientApiOrder[i++] = objClientApiUser;
            }
            return arrClientApiOrder;
        }

        private static void UpdateUserBaskets(BasketCollection userACBaskets, MakerShop.DataClient.Api.Schema.User objClientApiUser)
        {            
            DataObject nestedDataObject = new DataObject("Basket", typeof(MakerShop.Orders.Basket), typeof(Api.Schema.Basket));
            objClientApiUser.Baskets = (Api.Schema.Basket[])nestedDataObject.ConvertAC6Collection(userACBaskets);
            for (int i = 0; i < userACBaskets.Count; i++)
            {
                MakerShop.Orders.Basket userACBasket = userACBaskets[i];
                Schema.Basket schemaBasket =  objClientApiUser.Baskets[i];

                // BASKET ITEMS
                nestedDataObject = new DataObject("BasketItem", typeof(MakerShop.Orders.BasketItem), typeof(Api.Schema.BasketItem));
                schemaBasket.Items = (Api.Schema.BasketItem[])nestedDataObject.ConvertAC6Collection(userACBasket.Items);
                for (int j = 0; j < schemaBasket.Items.Length; j++)
                {
                    MakerShop.Orders.BasketItem userACBasketItem = userACBasket.Items[j];
                    Schema.BasketItem schemaBasketItem = schemaBasket.Items[j];
                    //BasketItemInput
                    nestedDataObject = new DataObject("BasketItemInput", typeof(MakerShop.Orders.BasketItemInput), typeof(Api.Schema.BasketItemInput));
                    schemaBasketItem.Inputs = (Api.Schema.BasketItemInput[])nestedDataObject.ConvertAC6Collection(userACBasketItem.Inputs);                    
                }
                // BASKET COUPONS
                nestedDataObject = new DataObject("BasketCoupons", typeof(MakerShop.Orders.BasketCoupon), typeof(Api.Schema.BasketCoupon));
                schemaBasket.BasketCoupons = (Api.Schema.BasketCoupon[])nestedDataObject.ConvertAC6Collection(userACBasket.BasketCoupons);

                // BASKET SHIPMENTS
                nestedDataObject = new DataObject("BasketShipments", typeof(MakerShop.Orders.BasketShipment), typeof(Api.Schema.BasketShipment));
                schemaBasket.Shipments = (Api.Schema.BasketShipment[])nestedDataObject.ConvertAC6Collection(userACBasket.Shipments);
            }
        }

        private static Schema.Wishlist[] getWishLists(MakerShop.Users.WishlistCollection objWishlistCollection)
        {
            DataObject dataObjectWishList = new DataObject("Wishlist", typeof(MakerShop.Users.Wishlist), typeof(Schema.Wishlist));
            Schema.Wishlist[] arrWishList = new Schema.Wishlist[objWishlistCollection.Count];
            Schema.Wishlist objClientApiWishlist = null;
            int i = 0;
            foreach (MakerShop.Users.Wishlist objWishList in objWishlistCollection)
            {
                objClientApiWishlist = (Schema.Wishlist)dataObjectWishList.ConvertToClientApiObject(objWishList);

                DataObject dataObjectWishListItem = new DataObject("WishlistItem", typeof(MakerShop.Users.WishlistItem), typeof(Schema.WishlistItem));
                Schema.WishlistItem[] arrWishListItem = new Schema.WishlistItem[objWishList.Items.Count];
                Schema.WishlistItem objClientApiWishlistItem = null;
                int j = 0;
                foreach (WishlistItem objWishlistItem in objWishList.Items)
                {
                    objClientApiWishlistItem = (Schema.WishlistItem)dataObjectWishListItem.ConvertToClientApiObject(objWishlistItem);

                    //Add Inputs
                    DataObject nestedDataObject = new DataObject("WishlistItemInput", typeof(MakerShop.Users.WishlistItemInput), typeof(Schema.WishlistItemInput));
                    objClientApiWishlistItem.Inputs = (Schema.WishlistItemInput[])nestedDataObject.ConvertAC6Collection(objWishlistItem.Inputs);                    

                    arrWishListItem[j++] = objClientApiWishlistItem;
                }
                objClientApiWishlist.Items = arrWishListItem;

                arrWishList[i++] = objClientApiWishlist;
            }
            return arrWishList;
        }

        

        public static List<String> GetIdListForUserCriteria(MakerShop.DataClient.Api.Schema.UserCriteria criteria)
        {
            List<String> idList = new List<string>();
            using (IDataReader dr = GetDataReader(criteria, true))
            {
                while (dr.Read())
                {
                    idList.Add(dr["UserId"].ToString());
                }
                dr.Close();
            }
            return idList;
        }

        public static UserCollection GetCustomizedCollection(MakerShop.DataClient.Api.Schema.UserCriteria criteria)
        {
            UserCollection results = new UserCollection();
            using (IDataReader dr = GetDataReader(criteria,false))
            {
                while (dr.Read())
                {
                    User user = new User();
                    User.LoadDataReader(user, dr);
                    results.Add(user);
                }
                dr.Close();
            }
            return results;
        }

        private static IDataReader GetDataReader(MakerShop.DataClient.Api.Schema.UserCriteria criteria, bool onlyIds)
        {
            SortedDictionary<string, ArrayList> parameters = new SortedDictionary<string, ArrayList>();

            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT DISTINCT ");
            if (onlyIds)
            {
                selectQuery.Append(" UserId ");
            }
            else
            {
                selectQuery.Append(User.GetColumnNames(string.Empty));
            }
            selectQuery.Append(" FROM ac_Users");
            selectQuery.Append(" WHERE StoreId = @storeId ");            

            // Profile Filter
            if (criteria.ProfileFilter != null)
            {
                //Outer Query Clause
                if(criteria.ProfileFilter.Email != null && !String.IsNullOrEmpty(criteria.ProfileFilter.Email.Option))
                {
                    selectQuery.Append(" AND " + translateStrComparisonOperator("Email" , criteria.ProfileFilter.Email.Option,criteria.ProfileFilter.Email.value,parameters));
                }

                //Inner Query
                if ( criteria.ProfileFilter.Name != null  
                    || criteria.ProfileFilter.Address != null
                    || !String.IsNullOrEmpty(criteria.ProfileFilter.Country) 
                    || !String.IsNullOrEmpty(criteria.ProfileFilter.Province) 
                    || criteria.ProfileFilter.ZipCode > 0)
                {
                    selectQuery.Append(" AND UserId IN (SELECT UserId FROM ac_Addresses WHERE StoreId = @storeId ");
                    if (criteria.ProfileFilter.Name != null && !String.IsNullOrEmpty(criteria.ProfileFilter.Name.Option))
                    {
                        selectQuery.Append("AND (");
                        selectQuery.Append(translateStrComparisonOperator("FirstName", criteria.ProfileFilter.Name.Option, criteria.ProfileFilter.Name.value, parameters));
                        selectQuery.Append(" OR " + translateStrComparisonOperator("LastName", criteria.ProfileFilter.Name.Option, criteria.ProfileFilter.Name.value, parameters));
                        selectQuery.Append(")");
                    }

                    if (criteria.ProfileFilter.Address != null && !String.IsNullOrEmpty(criteria.ProfileFilter.Address.Option))
                    {
                        selectQuery.Append("AND (");
                        selectQuery.Append(translateStrComparisonOperator("Address1", criteria.ProfileFilter.Address.Option, criteria.ProfileFilter.Address.value,parameters));
                        selectQuery.Append(" OR " + translateStrComparisonOperator("Address2", criteria.ProfileFilter.Address.Option, criteria.ProfileFilter.Address.value,parameters));
                        selectQuery.Append(")");
                    }

                    if (!String.IsNullOrEmpty(criteria.ProfileFilter.Country))
                    {
                        selectQuery.Append(" AND CountryCode  = @countrycode");
                        ArrayList tempList = new ArrayList();
                        KeyValuePair<string, string> parameter = new KeyValuePair<string, string>("@countrycode", criteria.ProfileFilter.Country);
                        tempList.Add(parameter);
                        parameters.Add("CountryCode", tempList);
                    }
                    if (!String.IsNullOrEmpty(criteria.ProfileFilter.Province))
                    {
                        ArrayList tempList = new ArrayList();
                        KeyValuePair<string, string> parameter = new KeyValuePair<string, string>("@province", criteria.ProfileFilter.Province);
                        tempList.Add(parameter);
                        selectQuery.Append(" AND Province  = @province");
                        parameters.Add("Province", tempList);
                    }
                    if (criteria.ProfileFilter.ZipCode > 0)
                    {
                        ArrayList tempList = new ArrayList();
                        KeyValuePair<string, string> parameter = new KeyValuePair<string, string>("@postalcode", criteria.ProfileFilter.ZipCode.ToString());
                        tempList.Add(parameter);
                        selectQuery.Append(" AND PostalCode  = @postalcode");
                        parameters.Add("PostalCode", tempList);
                    }
                    selectQuery.Append(")");
                }                
            }

            // Groups Filter
            if (!String.IsNullOrEmpty(criteria.GroupFilter))
            {
                criteria.GroupFilter.Trim();                
                //selectQuery.Append(" AND UserId IN (SELECT UserId FROM ac_UserRoles WHERE  StoreId = @storeId AND RoleId IN (" + criteria.GroupFilter + "))");
                selectQuery.Append(" AND UserId IN (SELECT UserId FROM ac_UserGroups WHERE  GroupId IN ( SELECT GroupId FROM ac_GroupRoles WHERE RoleId IN(" + criteria.GroupFilter + ")))");
            }
            // Activity Filter            
            if (criteria.ActivityFilter != null)
            {
                if (!String.IsNullOrEmpty(criteria.ActivityFilter.LastDateOption) && !(criteria.ActivityFilter.LastDate == DateTime.MinValue))
                {
                    CompareOption option = (CompareOption)Enum.Parse(typeof(CompareOption), criteria.ActivityFilter.LastDateOption);
                    if(CompareOption.Equal == option)
                    {
                        selectQuery.Append(" AND ( LastActivityDate >= @LastDateMin AND LastActivityDate <= @LastDateMax) ");
                        ArrayList tempList = new ArrayList();
                        KeyValuePair<string, string> parameter = new KeyValuePair<string, string>("@LastDateMin", criteria.ActivityFilter.LastDate.ToString());
                        tempList.Add(parameter);
                        parameter = new KeyValuePair<string, string>("@LastDateMax", criteria.ActivityFilter.LastDate.ToString());
                        tempList.Add(parameter);
                        parameters.Add("LastActivityDate", tempList);        
                    }
                    else
                    if(CompareOption.NotEqual == option)
                    {
                        selectQuery.Append(" AND ( LastActivityDate < @LastDateMin OR LastActivityDate > @LastDateMax) ");
                        ArrayList tempList = new ArrayList();
                        KeyValuePair<string, string> parameter = new KeyValuePair<string, string>("@LastDateMin", criteria.ActivityFilter.LastDate.ToString());
                        tempList.Add(parameter);
                        parameter = new KeyValuePair<string, string>("@LastDateMax", criteria.ActivityFilter.LastDate.ToString());
                        tempList.Add(parameter);
                        parameters.Add("LastActivityDate", tempList);        
                    }
                    else
                    {
                        //selectQuery.Append(" AND LastActivityDate " + criteria.ActivityFilter.LastDateOption+  criteria.ActivityFilter.LastDate);
                        selectQuery.Append(" AND LastActivityDate " + translateOperator(criteria.ActivityFilter.LastDateOption) + "@LastDate");
                        ArrayList tempList = new ArrayList();
                        KeyValuePair<string, string> parameter = new KeyValuePair<string, string>("@lastdate", criteria.ActivityFilter.LastDate.ToString());
                        tempList.Add(parameter);
                        parameters.Add("LastActivityDate", tempList);
                    }
                }
                if (!String.IsNullOrEmpty(criteria.ActivityFilter.SalesOption))
                {
                    selectQuery.Append(" AND UserId IN (SELECT UserId FROM ac_Orders GROUP BY UserId HAVING SUM(TotalCharges)" + translateOperator(criteria.ActivityFilter.SalesOption) + criteria.ActivityFilter.Sales.ToString());
                    selectQuery.Append(")");
                    ArrayList tempList = new ArrayList();
                    KeyValuePair<string, string> parameter = new KeyValuePair<string, string>("@sales", criteria.ActivityFilter.Sales.ToString());
                    tempList.Add(parameter);
                    parameters.Add("Sales", tempList);
                }
            }

            if(criteria.SortFilter != null)
            {
                switch (criteria.SortFilter.OrderBy.ToUpper().Trim())
                {

                    case "USER ID":
                        selectQuery.Append("  ORDER BY UserId ");
                        break;
                    case "NAME":
                        selectQuery.Append("  ORDER BY UserName ");
                        break;
                    case "EMAIL":
                        selectQuery.Append("  ORDER BY EMail ");
                        break;
                }
                
                if (criteria.SortFilter.Asc)
                    selectQuery.Append(" ASC");
                else
                    selectQuery.Append(" DESC");
            }
            

            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());

            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //if (criteria.ActivityFilter != null && !String.IsNullOrEmpty( criteria.ActivityFilter.LastDateOption) && criteria.ActivityFilter.LastDate != DateTime.MinValue)
            //{
            //    database.AddInParameter(selectCommand, "@LastDate", System.Data.DbType.DateTime, criteria.ActivityFilter.LastDate.Date);
            //}
            //if (criteria.ProfileFilter != null && !String.IsNullOrEmpty(criteria.ProfileFilter.EMail.Option) )
            //{
            //    database.AddInParameter(selectCommand, "@Email", System.Data.DbType.String, criteria.ProfileFilter.EMail.value);
            //}
            
            foreach(string key in parameters.Keys)
            {
                

                switch(key)
                {
                    case "Email":
                        foreach(KeyValuePair<string,string> parameter in parameters[key])
                        {
                            database.AddInParameter(selectCommand,parameter.Key,System.Data.DbType.String,parameter.Value);   
                        } 
                        break;

                    case "FirstName":
                        foreach (KeyValuePair<string, string> parameter in parameters[key])
                        {
                            database.AddInParameter(selectCommand, parameter.Key, System.Data.DbType.String, parameter.Value);
                        }
                        break;

                    case "LastName":
                        foreach (KeyValuePair<string, string> parameter in parameters[key])
                        {
                            database.AddInParameter(selectCommand, parameter.Key, System.Data.DbType.String, parameter.Value);
                        }
                        break;

                    case "Address1":
                        foreach (KeyValuePair<string, string> parameter in parameters[key])
                        {
                            database.AddInParameter(selectCommand, parameter.Key, System.Data.DbType.String, parameter.Value);
                        }
                        break;

                    case "Address2":
                        foreach (KeyValuePair<string, string> parameter in parameters[key])
                        {
                            database.AddInParameter(selectCommand, parameter.Key, System.Data.DbType.String, parameter.Value);
                        }
                        break;
                    case "CountryCode":
                        foreach (KeyValuePair<string, string> parameter in parameters[key])
                        {
                            database.AddInParameter(selectCommand, parameter.Key, System.Data.DbType.String, parameter.Value);
                        }
                        break;
                    case "Province":
                        foreach (KeyValuePair<string, string> parameter in parameters[key])
                        {
                            database.AddInParameter(selectCommand, parameter.Key, System.Data.DbType.String, parameter.Value);
                        }
                        break;
                    case "PostalCode":
                        foreach (KeyValuePair<string, string> parameter in parameters[key])
                        {
                            database.AddInParameter(selectCommand, parameter.Key, System.Data.DbType.String, parameter.Value);
                        }
                        break;
                    case "LastActivityDate":
                        CompareOption option = (CompareOption)Enum.Parse(typeof(CompareOption), criteria.ActivityFilter.LastDateOption);
                        if (CompareOption.Equal == option || CompareOption.NotEqual == option)
                        {
                            foreach (KeyValuePair<string, string> parameter in parameters[key])
                            {
                                if (parameter.Key.CompareTo("@LastDateMin") == 0)
                                {
                                    DateTime tempDate = Convert.ToDateTime(parameter.Value);
                                    DateTime lastDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 0, 0, 0, 0);
                                    database.AddInParameter(selectCommand, parameter.Key, System.Data.DbType.DateTime, lastDate);
                                }
                                else
                                {
                                    DateTime tempDate = Convert.ToDateTime(parameter.Value);
                                    DateTime lastDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 23, 59, 59, 999);
                                    database.AddInParameter(selectCommand, parameter.Key, System.Data.DbType.DateTime, lastDate);
                                }
                            }
                        }
                        else
                        {
                            if (CompareOption.LessThanEqualTo == option)
                            {
                                foreach (KeyValuePair<string, string> parameter in parameters[key])
                                {
                                    DateTime tempDate = Convert.ToDateTime(parameter.Value);
                                    DateTime lastDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 23, 59, 59, 999);
                                    database.AddInParameter(selectCommand, parameter.Key, System.Data.DbType.DateTime, lastDate);
                                }
                            }
                            else
                            if (CompareOption.LessThan == option)
                            {
                                foreach (KeyValuePair<string, string> parameter in parameters[key])
                                {
                                    DateTime tempDate = Convert.ToDateTime(parameter.Value);
                                    DateTime lastDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 0, 0, 0, 0);
                                    database.AddInParameter(selectCommand, parameter.Key, System.Data.DbType.DateTime, lastDate);
                                }
                            }
                            else
                            if (CompareOption.GreatorThan == option)
                            {
                                foreach (KeyValuePair<string, string> parameter in parameters[key])
                                {
                                    DateTime tempDate = Convert.ToDateTime(parameter.Value);
                                    DateTime lastDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 23, 59, 59, 999);
                                    database.AddInParameter(selectCommand, parameter.Key, System.Data.DbType.DateTime, lastDate);
                                }
                            }
                            else
                            if (CompareOption.GreatorThanEqualTo == option)
                            {
                                foreach (KeyValuePair<string, string> parameter in parameters[key])
                                {
                                    DateTime tempDate = Convert.ToDateTime(parameter.Value);
                                    DateTime lastDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 0, 0, 0, 0);
                                    database.AddInParameter(selectCommand, parameter.Key, System.Data.DbType.DateTime, lastDate);
                                }
                            }

                        }
                        break;
                    case "Sales":
                        foreach (KeyValuePair<string, string> parameter in parameters[key])
                        {
                            database.AddInParameter(selectCommand, parameter.Key, System.Data.DbType.Double, Convert.ToDouble(parameter.Value));
                        }
                        break;
                }    
            }
            //EXECUTE THE COMMAND
            IDataReader dr = database.ExecuteReader(selectCommand);
            return dr;

        }        

        private static String translateStrComparisonOperator(String columnName, String strOption,String value,SortedDictionary<string,ArrayList>
            parameters)
        {
            StrCompareOption option = (StrCompareOption)Enum.Parse(typeof(StrCompareOption), strOption);
            ArrayList myParameters = new ArrayList();
            KeyValuePair<string, string> parameter; 
            switch (option)
            {
                case StrCompareOption.ContainsString:
                    parameter = new KeyValuePair<string, string>("@" + columnName, "%"+value+"%");
                    myParameters.Add(parameter);
                    parameters.Add(columnName,myParameters);
                    return columnName + " LIKE " + "@" + columnName;

                case StrCompareOption.ContainsWords:
                    String retValue = String.Empty;
                    string[] words = value.Split(' ');
                    int lessCond = 0;
                    // Append Brackets
                    if (words.Length > 0)
                    {
                        retValue += "(";
                    }

                    for (int i = 0; i < words.Length; i++)
                    {
                        retValue += columnName + " LIKE "+"@" + columnName.Trim() + i.ToString();
                        parameter = new KeyValuePair<string, string>("@"+ columnName.Trim()+i.ToString(), "%"+words[i]+"%");
                        myParameters.Add(parameter);
                        lessCond = i;
                        lessCond++;
                        if (lessCond < words.Length)
                        {
                            retValue += " OR ";
                        }
                    }
                    // Close Bracket
                    if (words.Length > 0)
                    {
                        retValue += ")";
                    }
                    parameters.Add(columnName, myParameters);
                    return retValue;
                   
                case StrCompareOption.Matches:
                    parameter = new KeyValuePair<string, string>("@" + columnName.Trim(), value);
                    myParameters.Add(parameter);
                    parameters.Add(columnName, myParameters);
                    return columnName + "="+"@"+columnName.Trim();

                default:
                    return String.Empty;                    
            }
        }

        private static String translateOperator(String strOption)
        {
            CompareOption option = (CompareOption)Enum.Parse(typeof(CompareOption), strOption);
            switch (option)
            {
                case CompareOption.Equal:
                    return "=";
                    
                case CompareOption.NotEqual:                    
                    return " <> ";
                    
                case CompareOption.LessThan:
                    return " < ";
                
                case CompareOption.LessThanEqualTo:
                    return " <= ";
                    
                case CompareOption.GreatorThan:
                    return " > ";
                   
                case CompareOption.GreatorThanEqualTo:
                    return " >= ";
                    
                case CompareOption.Between:
                    return " BETWEEN ";
                   
                default:
                    return String.Empty;
            }
        }

        /// <summary>
        /// This method wil return a UserCollection against list of user Ids
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public static UserCollection GetUsersForIds(String idList)
        {
            UserCollection userCollection = new UserCollection();
            MakerShop.Users.User user = null;
            String[] arrIds = idList.Split(',');
            foreach (String id in arrIds)
            {
                user = new MakerShop.Users.User();
                int userId = AlwaysConvert.ToInt(id);
                if (user.Load(userId))
                {
                    userCollection.Add(user);
                }
            }
            return userCollection;
        }

        /// <summary>
        /// This method will return UserId of all users in the store
        /// </summary>
        /// <returns></returns>
        public static List<String> GetIdListForStore()
        {
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT UserId From ac_Users Where StoreId = " + Token.Instance.StoreId + " AND IsAnonymous = @IsAnonymous");
            List<String> idList = new List<string>();
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@IsAnonymous", System.Data.DbType.Boolean, false);
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    idList.Add(dr[0].ToString());
                }
                dr.Close();
            }
            return idList;
        }
    }

}


