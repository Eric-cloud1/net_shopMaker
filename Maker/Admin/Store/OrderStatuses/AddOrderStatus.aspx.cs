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

public partial class Admin_Store_OrderStatuses_AddOrderStatus : MakerShop.Web.UI.MakerShopAdminPage
{

    protected void Page_Load(object sender, System.EventArgs e)
    {
        PageHelper.ConvertEnterToTab(Name);
        PageHelper.ConvertEnterToTab(DisplayName);
        if (!Page.IsPostBack)
        {
            // LOAD TRIGGERS SELECT BOX
            foreach (int storeEventId in Enum.GetValues(typeof(StoreEvent)))
            {
                if ((storeEventId >= 100) && (storeEventId < 200))
                {
                    StoreEvent storeEvent = (StoreEvent)storeEventId;
                    Triggers.Items.Add(new ListItem(StringHelper.SpaceName(storeEvent.ToString()), storeEventId.ToString()));
                }
            }
        }
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        OrderStatus _OrderStatus = new OrderStatus();
        _OrderStatus.Name = Name.Text;
        _OrderStatus.DisplayName = DisplayName.Text;
        //ACTIVE FLAG IS USED TO DETERMINE VALID SALES FOR REPORTING
        _OrderStatus.IsActive = Report.Checked;
        //VALID FLAG IS USED TO DERMINE WHICH ORDERS ARE BAD
        _OrderStatus.IsValid = (!Cancelled.Checked);
        _OrderStatus.InventoryAction = (MakerShop.Orders.InventoryAction)InventoryAction.SelectedIndex;
        foreach (ListItem item in Triggers.Items)
        {
            if (item.Selected)
            {
                _OrderStatus.Triggers.Add(new OrderStatusTrigger(AlwaysConvert.ToInt(item.Value)));
            }
        }
        foreach (ListItem item in EmailTemplates.Items)
        {
            if (item.Selected)
            {
                _OrderStatus.OrderStatusEmails.Add(new OrderStatusEmail(0, AlwaysConvert.ToInt(item.Value)));
            }
        }
        _OrderStatus.Save();
        Response.Redirect("Default.aspx");
    }

}
