using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;
using MakerShop.Payments;

namespace MakerShop.Validation
{
    /// <summary>
    /// DataSource class for ErrorMessage objects
    /// </summary>
    [DataObject(true)]
    public partial class WhiteListsDataSource
    {

        /// <summary>
        /// Checks to see if the values passed are on the White list
        /// </summary>
        /// <param name="CreditCard">Checks Like pattern on right</param>
        /// <param name="Phone">Checks Like pattern on right</param>
        /// <param name="eMail"></param>
        /// <returns>True it is on the list / False it is not</returns>
        public static bool IsOnWhiteList(string CreditCard, string Phone, string eMail)
        {

            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand cmd = database.GetStoredProcCommand("wsp_CheckWhiteList"))
            {
                database.AddInParameter(cmd, "@CreditCard", System.Data.DbType.String, CreditCard);
                database.AddInParameter(cmd, "@Phone", System.Data.DbType.String, Phone);
                database.AddInParameter(cmd, "@eMail", System.Data.DbType.String, eMail);
                database.AddParameter(cmd, "@rtn", System.Data.DbType.Int32, System.Data.ParameterDirection.ReturnValue, null, System.Data.DataRowVersion.Default, -1);
                database.ExecuteNonQuery(cmd);
                int rtn = (int)cmd.Parameters["@rtn"].Value;
                return rtn != 0;
            }
        }
        /// <summary>
        /// Checks to see if the values passed are on the White list
        /// </summary>
        /// <param name="Payment"></param>
        /// <returns>True it is on the list and / False it is not</returns>
        public static bool IsOnWhiteList(Payments.Payment p)
        {
            AccountDataDictionary accountData = new AccountDataDictionary(p.AccountData);

            string an=null;
            if (accountData.ContainsKey("AccountNumber"))
                an = accountData["AccountNumber"];
            return IsOnWhiteList(an, p.Order.BillToPhone, p.Order.BillToEmail);
        }
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static WhiteListsCollection Load(WhiteListTypes whitelisttype)
        {

            return LoadForCriteria("whitelisttypeid = " + ((short)whitelisttype).ToString());

        }

    
    }
    public enum WhiteListTypes : short
    {  
        CreditCard = 1,
        Phone = 2,
        eMail = 3,
        IP = 4
    }
}
