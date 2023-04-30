using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using MakerShop.Common;
using MakerShop.Catalog;
using MakerShop.Orders;
using MakerShop.Products;
using MakerShop.Stores;
using MakerShop.Users;
using MakerShop.Utility;
using MakerShop.Reporting;
using System.Collections.Generic;

public partial class Admin_Login : MakerShop.Web.UI.MakerShopAdminPage
{
    protected void ApproveAllUsers()
    {
        int userCount = UserDataSource.CountForStore();
        int maxRecords = 1000; // RECORDS THAT WILL BE PROCESSED EACH TIME
        int startIndex = 0;
        int loopCount = (int)((userCount / maxRecords) + 1);
        for (int i = 0; i < loopCount; i++)
        {
            // FOR LAST STEP OF LOOP
            if (i == (loopCount - 1)) maxRecords = userCount - startIndex;

            UserCollection users = UserDataSource.LoadForStore(maxRecords, startIndex);
            foreach (User user in users)
            {
                if (!user.IsApproved)
                {
                    user.IsApproved = true;
                    user.IsLockedOut = false;
                    user.Save();
                }
            }

            // INCREMENT START INDEX
            startIndex += maxRecords;
        }
    }
}
