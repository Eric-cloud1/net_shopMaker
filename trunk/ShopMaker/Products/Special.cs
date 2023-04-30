using System;
using MakerShop.Users;
using MakerShop.Utility;

namespace MakerShop.Products
{
    /// <summary>
    /// Class representing a special product
    /// </summary>
    public partial class Special
    {
        /// <summary>
        /// Indicates whether this special is valid or not
        /// </summary>
        /// <param name="userId">The Id of the user for whom to check the validity</param>
        /// <returns>true if special is valid, false otherwise</returns>
        public bool IsValid(int userId)
        {
            //IF START DATE IS IN EFFECT, CHECK THAT WE ARE AFTER
            if ((this.StartDate != DateTime.MinValue) && (LocaleHelper.LocalNow < this.StartDate)) return false;
            //IF END DATE IS IN EFFECT, CHECK THAT WE ARE BEFORE
            if ((this.EndDate != DateTime.MinValue) && (LocaleHelper.LocalNow > this.EndDate)) return false;
            //CHECK WHETHER A ROLE RESTRICTION IS DEFINED
            if (this.SpecialGroups.Count > 0) return IsUserInGroup(userId);
            //ALL CHECKS PASSED
            return true;
        }

        private bool IsUserInGroup(int userId)
        {
            User user = UserDataSource.Load(userId);
            if (user != null)
            {
                //LOOP ALL ROLES ASSOCIATED WITH RULE
                foreach (SpecialGroup sr in this.SpecialGroups)
                {
                    //LOOP ALL ROLES ASSOCIATED WITH USER
                    foreach (UserGroup ur in user.UserGroups)
                    {
                        //IF WE FIND A MATCH, THE RULE IS VALID
                        if (sr.GroupId == ur.GroupId) return true;
                    }
                }
            }
            return false;
        }
    }
}
