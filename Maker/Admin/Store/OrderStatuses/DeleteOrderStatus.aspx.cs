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

public partial class Admin_Store_OrderStatuses_DeleteOrderStatus : MakerShop.Web.UI.MakerShopAdminPage
{

    int _OrderStatusId = 0;
    OrderStatus _OrderStatus;

    protected void Page_Init(object sender, System.EventArgs e)
    {
        _OrderStatusId = AlwaysConvert.ToInt(Request.QueryString["OrderStatusId"]);
        _OrderStatus = OrderStatusDataSource.Load(_OrderStatusId);
        if (_OrderStatus == null) Response.Redirect("Default.aspx");
        Caption.Text = string.Format(Caption.Text, _OrderStatus.Name);
        InstructionText.Text = string.Format(InstructionText.Text, _OrderStatus.Name);
        BindOrderStatuses();
    }

    protected void CancelButton_Click(object sender, System.EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }

    protected void DeleteButton_Click(object sender, System.EventArgs e)
    {
        if (Page.IsValid)
        {
            _OrderStatus.Delete(AlwaysConvert.ToInt(OrderStatusList.SelectedValue));
            Response.Redirect("Default.aspx");
        }
    }

    private void BindOrderStatuses()
    {
        OrderStatusCollection orderStatuses = OrderStatusDataSource.LoadForStore("Name");
        int index = orderStatuses.IndexOf(_OrderStatusId);
        if (index > -1) orderStatuses.RemoveAt(index);
        OrderStatusList.DataSource = orderStatuses;
        OrderStatusList.DataBind();        
    }

}
