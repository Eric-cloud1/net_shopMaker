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
using MakerShop.Shipping;

public partial class Admin_Shipping_Warehouses_DeleteWarehouse : MakerShop.Web.UI.MakerShopAdminPage
{

    int _WarehouseId = 0;
    Warehouse _Warehouse;

    protected void Page_Init(object sender, System.EventArgs e)
    {
        _WarehouseId = AlwaysConvert.ToInt(Request.QueryString["WarehouseId"]);
        _Warehouse = WarehouseDataSource.Load(_WarehouseId);
        if (_Warehouse == null) Response.Redirect("Default.aspx");
		//you can not delete default warehouse
		if(_Warehouse.WarehouseId == Token.Instance.Store.DefaultWarehouseId) Response.Redirect("Default.aspx");
        Caption.Text = string.Format(Caption.Text, _Warehouse.Name);
        InstructionText.Text = string.Format(InstructionText.Text, _Warehouse.Name);
        BindWarehouses();
    }

    protected void CancelButton_Click(object sender, System.EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }

    protected void DeleteButton_Click(object sender, System.EventArgs e)
    {
        if (Page.IsValid)
        {
            _Warehouse.Delete(AlwaysConvert.ToInt(WarehouseList.SelectedValue));
            Response.Redirect("Default.aspx");
        }
    }

    private void BindWarehouses()
    {
        WarehouseCollection warehouses = WarehouseDataSource.LoadForStore("Name");
        int index = warehouses.IndexOf(_WarehouseId);
        if (index > -1) warehouses.RemoveAt(index);
        WarehouseList.DataSource = warehouses;
        WarehouseList.DataBind();
    }

}
