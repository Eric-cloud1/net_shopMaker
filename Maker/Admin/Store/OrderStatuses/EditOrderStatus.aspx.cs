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

public partial class Admin_Store_OrderStatuses_EditOrderStatus : MakerShop.Web.UI.MakerShopAdminPage
{

    private int _OrderStatusId;
    private OrderStatus _OrderStatus;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        _OrderStatusId = AlwaysConvert.ToInt(Request.QueryString["OrderStatusId"]);
        _OrderStatus = OrderStatusDataSource.Load(_OrderStatusId);
        PageHelper.ConvertEnterToTab(Name);
        PageHelper.ConvertEnterToTab(DisplayName);
        Caption.Text = string.Format(Caption.Text, _OrderStatus.Name);
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
            // INITIALIZE FORM
            Name.Text = _OrderStatus.Name;
            Name.Focus();
            DisplayName.Text = _OrderStatus.DisplayName;
            Report.Checked = _OrderStatus.IsActive;
            Cancelled.Checked = !_OrderStatus.IsValid;
            InventoryAction.SelectedIndex = (int)_OrderStatus.InventoryActionId;
            foreach (OrderStatusTrigger trigger in _OrderStatus.Triggers)
            {
                ListItem item = Triggers.Items.FindByValue(trigger.StoreEventId.ToString());
                if (item != null) item.Selected = true;
            }
        }
    }

    protected void EmailTemplates_DataBound(object sender, System.EventArgs e)
    {
        if (EmailTemplates.Items.Count > 0)
        {
            ListItem item;
            foreach (OrderStatusEmail email in _OrderStatus.OrderStatusEmails)
            {
                item = EmailTemplates.Items.FindByValue(email.EmailTemplateId.ToString());
                if (item != null)
                {
                    item.Selected = true;
                }
                else
                {
                    NoEmailTemplatesDefinedLabel.Visible = true;
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
        _OrderStatus.Name = Name.Text;
        _OrderStatus.DisplayName = DisplayName.Text;
        //ACTIVE FLAG IS USED TO DETERMINE VALID SALES FOR REPORTING
        _OrderStatus.IsActive = Report.Checked;
        //VALID FLAG IS USED TO DERMINE WHICH ORDERS ARE BAD
        _OrderStatus.IsValid = (!Cancelled.Checked);
        _OrderStatus.InventoryAction = (MakerShop.Orders.InventoryAction)InventoryAction.SelectedIndex;
        _OrderStatus.Triggers.DeleteAll();
        foreach (ListItem item in Triggers.Items)
        {
            if (item.Selected)
            {
                _OrderStatus.Triggers.Add(new OrderStatusTrigger(AlwaysConvert.ToInt(item.Value)));
            }
        }
        _OrderStatus.OrderStatusEmails.DeleteAll();
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
