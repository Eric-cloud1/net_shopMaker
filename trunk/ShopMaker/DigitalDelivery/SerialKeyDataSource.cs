using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;

namespace MakerShop.DigitalDelivery
{
    /// <summary>
    /// DataSource class for SerialKey objects
    /// </summary>
    [DataObject(true)]
    public partial class SerialKeyDataSource
    {
        public static DataTable GetPendingRefunds()
        {
            Database database = Token.Instance.Database;
            DataTable dt = new DataTable();
            using (System.Data.Common.DbCommand cmd = database.GetStoredProcCommand("wsp_DigitalGoods_PendingCancel"))
            {

                DataSet ds = database.ExecuteDataSet(cmd);
                if (ds.Tables.Count > 0)
                    dt = ds.Tables[0];

            }
            return dt;
        }

        public static DataTable GetPendingActivations()
        {
            Database database = Token.Instance.Database;
            DataTable dt = new DataTable();
            using (System.Data.Common.DbCommand cmd = database.GetStoredProcCommand("wsp_DigitalGoods_PendingActivate"))
            {

                DataSet ds = database.ExecuteDataSet(cmd);
                if (ds.Tables.Count > 0)
                    dt = ds.Tables[0];

            }
            return dt;
        }
    }
}
