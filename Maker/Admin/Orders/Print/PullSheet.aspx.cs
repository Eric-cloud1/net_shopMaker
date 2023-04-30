using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using MakerShop.Catalog;
using MakerShop.Data;
using MakerShop.DigitalDelivery;
using MakerShop.Marketing;
using MakerShop.Messaging;
using MakerShop.Orders;
using MakerShop.Payments;
using MakerShop.Payments.Providers;
using MakerShop.Products;
using MakerShop.Reporting;
using MakerShop.Shipping;
using MakerShop.Stores;
using MakerShop.Taxes;
using MakerShop.Taxes.Providers;
using MakerShop.Users;
using MakerShop.Utility;

public partial class Admin_Orders_Print_PullSheet : MakerShop.Web.UI.MakerShopAdminPage
{

    private List<string> orderNumbers;

    private List<int> GetSelectedOrders()
    {
        //CHECK FOR QUERYSTRING PARAMETERS
        List<int> selectedOrders = new List<int>();
        orderNumbers = new List<string>();
        string orderNumberList = Request.QueryString["orders"];
        if (!string.IsNullOrEmpty(orderNumberList))
        {
            int[] orderNumberArray = AlwaysConvert.ToIntArray(orderNumberList);
            Dictionary<int, int> orderIdLookup = OrderDataSource.LookupOrderIds(orderNumberArray);
            foreach (int orderNumber in orderIdLookup.Keys)
            {
                orderNumbers.Add(orderNumber.ToString());
                selectedOrders.Add(orderIdLookup[orderNumber]);
            }
        }
        return selectedOrders;
    }

    private string GetOrderNumbers(List<int> orders)
    {
        if (orderNumbers == null)
        {
            orderNumbers = new List<string>();
            foreach (int orderId in orders)
            {
                orderNumbers.Add(orderId.ToString());
            }
        }
        if (orderNumbers.Count == 0) return string.Empty;
        return string.Join(", ", orderNumbers.ToArray());
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        List<int> selectedOrders = GetSelectedOrders();
        if ((selectedOrders == null) || (selectedOrders.Count == 0)) Response.Redirect("~/Admin/Orders/Default.aspx");
        ItemGrid.DataSource = OrderPullItemDataSource.GeneratePullSheet(selectedOrders.ToArray());
        ItemGrid.DataBind();
        OrderList.Text = GetOrderNumbers(selectedOrders);
    }


}
