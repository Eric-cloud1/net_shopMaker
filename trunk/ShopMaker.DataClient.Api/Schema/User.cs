using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.DataClient.Api.Schema
{
    public partial class User
    {
        public static string[] GetCSVEnabledFields(string EmptyString)
        {
            List<string> columnNames = new List<string>();
            columnNames.Add("UserId");
            columnNames.Add("StoreId");
            columnNames.Add("UserName");
            columnNames.Add("FirstName");
            columnNames.Add("LastName");
            columnNames.Add("Email");
            columnNames.Add("ReferringAffiliateId");
            //columnNames.Add("AffiliateId");
            columnNames.Add("Affiliate");
            columnNames.Add("AffiliateReferralDate");
            columnNames.Add("PrimaryAddressId");
            columnNames.Add("PrimaryWishlistId");
            columnNames.Add("PayPalId");
            columnNames.Add("PasswordQuestion");
            columnNames.Add("PasswordAnswer");
            columnNames.Add("IsApproved");
            columnNames.Add("IsAnonymous");
            columnNames.Add("IsLockedOut");
            columnNames.Add("CreateDate");
            columnNames.Add("LastActivityDate");
            columnNames.Add("LastLoginDate");
            columnNames.Add("LastPasswordChangedDate");
            columnNames.Add("LastLockoutDate");
            columnNames.Add("FailedPasswordAttemptCount");
            columnNames.Add("FailedPasswordAttemptWindowStart");
            columnNames.Add("FailedPasswordAnswerAttemptCount");
            columnNames.Add("FailedPasswordAnswerAttemptWindowStart");
            columnNames.Add("Comment");            
            columnNames.Add("PasswordPlainText");
            columnNames.Add("PasswordEncrypted");
            columnNames.Add("Nickname");
            columnNames.Add("Company");
            columnNames.Add("Address1");
            columnNames.Add("Address2");
            columnNames.Add("City");
            columnNames.Add("Province");
            columnNames.Add("PostalCode");
            columnNames.Add("CountryCode");
            columnNames.Add("Phone");
            columnNames.Add("Fax");
            columnNames.Add("AddressIsResidence");
 
            return columnNames.ToArray();
        }


        /// <summary>
        /// Fields that are required while importing csv
        /// </summary>
        /// <returns></returns>
        public static string[] GetCSVImportRequiredFields()
        {
            List<string> columnNames = new List<string>();            
            columnNames.Add("UserName");
            columnNames.Add("Email");
            columnNames.Add("IsApproved");            
            columnNames.Add("IsLockedOut");            
            return columnNames.ToArray();
        }

        /// <summary>
        /// Fields that are used as key field (to identify an object) while performing a csv update
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDefaultKeyFields()
        {
            List<string> columnNames = new List<string>();
            columnNames.Add("UserName");
            return columnNames;
        }


        /// <summary>
        /// Returns a list of numaric column/field names
        /// </summary>
        /// <returns>Returns a list of numaric column/field names</returns>
        public static List<String> GetNumaricFields()
        {
            List<string> columnNames = new List<string>();
            columnNames.Add("UserId");
            columnNames.Add("StoreId");
            columnNames.Add("ReferringAffiliateId");
            columnNames.Add("PrimaryAddressId");
            columnNames.Add("PrimaryWishlistId");
            columnNames.Add("PayPalId");
            columnNames.Add("FailedPasswordAttemptCount");
            columnNames.Add("FailedPasswordAnswerAttemptCount");
            columnNames.Add("FailedPasswordAnswerAttemptWindowStart");
            return columnNames;
        }
    }
}
