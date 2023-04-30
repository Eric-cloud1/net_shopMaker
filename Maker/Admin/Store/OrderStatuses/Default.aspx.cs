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

public partial class Admin_Store_OrderStatuses_Default : MakerShop.Web.UI.MakerShopAdminPage
{

    protected string GetX(object boolValue)
    {
        return (bool)boolValue ? "X" : string.Empty;
    }

    protected string GetTriggers(object dataItem)
    {
        OrderStatus orderStatus = (OrderStatus)dataItem;
        List<string> triggers = new List<string>();
        foreach (OrderStatusTrigger trigger in orderStatus.Triggers)
        {
            triggers.Add(StringHelper.SpaceName(trigger.StoreEvent.ToString()));
        }
        return string.Join("<br />", triggers.ToArray());
    }
    protected bool HasOrders(object dataItem)
    {
        OrderStatus os= (OrderStatus)dataItem;
        return (OrderDataSource.CountForOrderStatus(os.OrderStatusId) > 0);
    }   

}
