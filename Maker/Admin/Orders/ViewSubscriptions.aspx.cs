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

public partial class Admin_Orders_ViewSubscriptions : MakerShop.Web.UI.MakerShopAdminPage
{

    private int _OrderId;
    private Order _Order;
    private OrderSubscriptionPlanDetailsCollection _plan;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        _OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
        _Order = OrderDataSource.Load(_OrderId);
        _plan = OrderSubscriptionPlanDetailsDataSource.LoadByOrderId(_OrderId);
        Caption.Text = string.Format(Caption.Text, _Order.OrderNumber);
        OrderSubscriptions os = OrderSubscriptionsDataSource.Load(_OrderId);
        if (os != null)
            SubscriptionStatus.Text = string.Format(SubscriptionStatus.Text, os.SubscriptionStatus);
        else
            SubscriptionStatus.Visible = false;
    }

    protected void SubscriptionGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {

    }

    protected String GetPaymentTypeName(object id)
    {
        int groupId = AlwaysConvert.ToInt(id);
        return ((PaymentTypes)groupId).ToString();
    }

    protected String GetShipRate(object id)
    {

        try
        {
            int i = AlwaysConvert.ToInt(id);
             ShipMethod sm = ShipMethodDataSource.Load(i);
                 if (sm.ShipRateMatrices.Count ==0)
                     return "NONE";
                 return sm.ShipRateMatrices[0].Rate.ToString("0.00");
            
        }
        catch
        {
            return "ERROR";
        }
    }
    protected void SubscriptionGrid_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {

        GridViewRow row = SubscriptionGrid.Rows[e.RowIndex];
        DropDownList ddl = PageHelper.RecursiveFindControl(row, "DownSell") as DropDownList;

        OrderSubscriptionPlanDetails spd = OrderSubscriptionPlanDetailsDataSource.Load((int)e.Keys["OrderId"], (short)e.Keys["PaymentTypeId"]);
        if (spd != null)
        {
            spd.PaymentAmount = AlwaysConvert.ToDecimal(ddl.SelectedValue);
            spd.Save();
        }
        SubscriptionGrid.EditIndex = -1;
        e.Cancel = true;
    }


    protected SubscriptionPlanDownsellsCollection GetDownSell(object paymentTypeId)
    {
        if (_Order == null)
            Page_Load(null, null);
        int? productId=null;

        foreach (OrderItem oi in _Order.Items)
                if (oi.ProductId > 0)
                {
                    productId = oi.ProductId;
                break;
                }
        if (productId.HasValue)
            return SubscriptionPlanDownsellsDataSource.Load(productId.Value, (PaymentTypes)((short)paymentTypeId));
        return null;
    }
}
