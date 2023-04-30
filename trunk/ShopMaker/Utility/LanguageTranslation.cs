using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Orders;
using MakerShop.Utility;
using MakerShop.Marketing;
using MakerShop.Users;
using MakerShop.Products;
using System.Web;
using System.Data.SqlClient;

namespace MakerShop.Utility
{
    public partial class LanguageTranslation
    {



        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static string Translate(string culture, string key)
        {
            Database database = Token.Instance.Database;
            string s = null;
            using (System.Data.Common.DbCommand getCommand = database.GetStoredProcCommand("wsp_LanguageTranslationsLookup"))
            {
                database.AddInParameter(getCommand, "@Culture", System.Data.DbType.String, culture);
                database.AddInParameter(getCommand, "@FieldName", System.Data.DbType.String, key);

                database.AddOutParameter(getCommand, "@FieldValue", System.Data.DbType.String, -1);


                database.ExecuteNonQuery(getCommand);
                if (getCommand.Parameters["@FieldValue"].Value != DBNull.Value)
                    s = (string)getCommand.Parameters["@FieldValue"].Value;
            }
            if (string.IsNullOrEmpty(s))
                return "(" + key + ")";
            return s;

        }
    }
}
