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
    public partial class BlackListsDataSource
    {

        /// <summary>
        /// Checks to see if the values passed are on the Black list
        /// </summary>
        /// <param name="CreditCard">Checks Like pattern on right</param>
        /// <param name="Phone">Checks Like pattern on right</param>
        /// <param name="eMail"></param>
        /// <param name="ip"></param>
        /// <param name="PostCode">Checks Like pattern on right</param>
        /// <param name="State"></param>
        /// <param name="IsCart">Is Shopping card to not check number of times ordered</param>
        /// <param name="blackListType">What type of block is it</param>
        /// <returns>True it is on the list and / False it is not</returns>
        public static bool IsOnBlackList(string CreditCard, string Phone, string eMail, string ip, string PostCode, string State, bool IsCart, out BlackListTypes blackListType)
        {

            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand cmd = database.GetStoredProcCommand("wsp_CheckBlackList"))
            {
                database.AddInParameter(cmd, "@CreditCard", System.Data.DbType.String, CreditCard);
                database.AddInParameter(cmd, "@Phone", System.Data.DbType.String, Phone);
                database.AddInParameter(cmd, "@eMail", System.Data.DbType.String, eMail);
                database.AddInParameter(cmd, "@IP", System.Data.DbType.String, ip);
                database.AddInParameter(cmd, "@postCode", System.Data.DbType.String, PostCode);
                database.AddInParameter(cmd, "@State", System.Data.DbType.String, State);
                database.AddParameter(cmd, "@rtn", System.Data.DbType.Int32, System.Data.ParameterDirection.ReturnValue, null, System.Data.DataRowVersion.Default, -1);
                database.ExecuteNonQuery(cmd);
                int rtn = (int)cmd.Parameters["@rtn"].Value;
                blackListType = (BlackListTypes)Enum.ToObject(typeof(BlackListTypes), rtn);
                return rtn != 0;
            }
        }
        /// <summary>
        /// Checks to see if the values passed are on the Black list
        /// </summary>
        /// <param name="Payment"></param>
        /// <returns>True it is on the list and / False it is not</returns>
        public static bool IsOnBlackList(Payments.Payment p, bool IsCart, out BlackListTypes blackListType)
        {
            AccountDataDictionary accountData = new AccountDataDictionary(p.AccountData);
            string account = string.Empty;
            if (accountData.ContainsKey("AccountNumber"))
                account = accountData["AccountNumber"];
            return IsOnBlackList(account, p.Order.BillToPhone, p.Order.BillToEmail, p.Order.RemoteIP, p.Order.BillToPostalCode, p.Order.BillToProvince, IsCart, out blackListType);
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static BlackListsCollection Load(BlackListTypes Blacklisttype)
        {

            return LoadForCriteria("Blacklisttypeid = " + ((short)Blacklisttype).ToString());

        }

    
    }
    public enum BlackListTypes : int
    {  
        CreditCard = 1,
        Phone = 2,
        eMail = 3,
        IP = 4,
        postCode = 5,
        State = 6,
        IP2 = 7,
        CreditCard2 = 8,
        eMail2 = 9,
        Phone2 = 10,
        SubAffiliate = 11
    }

    public class BlackListException : MakerShop.Exceptions.MakerShopException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        public BlackListException(string message) : base(message) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public BlackListException(string message, Exception innerException) : base(message, innerException) { }
    }
}
