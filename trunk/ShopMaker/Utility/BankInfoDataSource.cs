using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;

namespace MakerShop.Utility
{
    /// <summary>
    /// DataSource class for ErrorMessage objects
    /// </summary>
    [DataObject(true)]
    public partial class BankInfoDataSource
    {
        /// <summary>
        /// Deletes all error messages in the current store
        /// </summary>
        public static bool GetBankInfo(string routingNumber, out BankInfo bi)
        {
            bool good = false;
            bi = new BankInfo();
            try
            {
                int storeId = Token.Instance.StoreId;
                Database database = Token.Instance.Database;
                System.Data.SqlClient.SqlParameter[] parameters = new System.Data.SqlClient.SqlParameter[1];
                using (IDataReader dr = database.ExecuteReader("@RoutingNumber", routingNumber))
                {
                    if (dr.Read())
                    {

                        bi.RoutingNumber = dr.GetString(0);
                        bi.Name = dr.GetString(1);
                        bi.Address = dr.GetString(2);
                        bi.City = dr.GetString(3);
                        bi.State = dr.GetString(4);
                        bi.ZipCode = dr.GetString(5);
                        bi.Zip4 = dr.GetString(6);
                        good = true;
                    }
                    dr.Close();
                }
            }
            catch
            {
            }
            return good;
        }

        public struct BankInfo
        {
            public string RoutingNumber;
            public string Name;
            public string Address;
            public string City;
            public string State;
            public string ZipCode;
            public string Zip4;
        }
    }
}
