using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Orders;
using System.Data;
using System.Data.Common;
using System.Web.Security;

namespace MakerShop.Users
{
    /// <summary>
    /// This class represents an order
    /// </summary>
    public partial class OptIn
    {
       
        /// <summary>
        /// OptIn User based on Order
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <param name="message">Anything, usually the opt in text</param>
        public static void OptInUserbyOrderNumber(int orderNumber, string message)
        {

            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand cmd = database.GetStoredProcCommand("wsp_OptIn"))
            {
                database.AddInParameter(cmd, "@OrderNumber", System.Data.DbType.Int32, orderNumber);
                database.AddInParameter(cmd, "@Message", System.Data.DbType.Int32, message);
                database.ExecuteNonQuery(cmd);
            }
        }

    }
}