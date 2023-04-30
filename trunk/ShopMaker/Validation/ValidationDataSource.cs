using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;

namespace MakerShop.Validation
{
    /// <summary>
    /// DataSource class for Validation 
    /// </summary>
    [DataObject(true)]
    public partial class Validation
    {      
        /// <summary>
        /// Final Validations in the database
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderNumber"></param>
        /// <param name="response">Message to the user</param>
        /// <returns></returns>
        public static bool Validate(int? orderId, int? orderNumber, out string response)
        {

            Database database = Token.Instance.Database;
            bool b = false;
            using (System.Data.Common.DbCommand cmd = database.GetStoredProcCommand("wsp_Validate"))
            {
                if (orderId.HasValue)
                    database.AddInParameter(cmd, "@OrderId", System.Data.DbType.Int32, orderId);
                if (orderNumber.HasValue)
                    database.AddInParameter(cmd, "@OrderNumber", System.Data.DbType.Int32, orderNumber);
                database.AddOutParameter(cmd, "@Response", System.Data.DbType.String,Int32.MaxValue);
                database.AddParameter(cmd, "@rtn", System.Data.DbType.Int32, System.Data.ParameterDirection.ReturnValue, null, System.Data.DataRowVersion.Default, -1);
                database.ExecuteNonQuery(cmd);
                int rtn = (int)cmd.Parameters["@rtn"].Value;
                b = rtn == 1;
                if (!b)
                {
                    response = cmd.Parameters["@Response"].Value.ToString();
                }
                else
                    response = null;
            }
            return b;
        }
    }
    
}
