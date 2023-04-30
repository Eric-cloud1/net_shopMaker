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

public partial class Admin_Shipping_Warehouses_ViewProducts : MakerShop.Web.UI.MakerShopAdminPage
{


    private int _WarehouseId;
    private Warehouse _Warehouse;
    protected void Page_Load(object sender, EventArgs e)
    {
        _WarehouseId = AlwaysConvert.ToInt(Request.QueryString["WarehouseId"]);
        _Warehouse  = WarehouseDataSource.Load(_WarehouseId);
        if (_Warehouse == null) Response.Redirect("Default.aspx");
        Caption.Text = string.Format(Caption.Text, _Warehouse.Name);
        if (!Page.IsPostBack)
        {
            List<Warehouse> targets = new List<Warehouse>();
            WarehouseCollection allWarehouses = WarehouseDataSource.LoadForStore();
            foreach (Warehouse w in allWarehouses)
            {
                if (w.WarehouseId != AlwaysConvert.ToInt(_WarehouseId)) targets.Add(w);
            }
            if (targets.Count == 0)
            {
                NewWarehousePanel.Visible = false;
                ProductGrid.Columns[1].Visible = false;
            }
            else
            {
                ProductGrid.Columns[1].Visible = true;
                NewWarehouse.DataSource = targets;
                NewWarehouse.DataBind();
            }
        }
    }

    protected void BackButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }

    protected void NewWarehouseUpdateButton_Click(object sender, EventArgs e)
    {
        int newWarehouseId = AlwaysConvert.ToInt(NewWarehouse.SelectedValue);
        Warehouse newWarehouse = WarehouseDataSource.Load(newWarehouseId);
        if (newWarehouse != null)
        {
            foreach (GridViewRow row in ProductGrid.Rows)
            {
                CheckBox selected = (CheckBox)row.FindControl("Selected");
                if ((selected != null) && (selected.Checked))
                {
			        int dataItemIndex = row.DataItemIndex;					
					dataItemIndex = (dataItemIndex - (ProductGrid.PageSize * ProductGrid.PageIndex));
					int productId = (int)ProductGrid.DataKeys[dataItemIndex].Value;
                    //int productId = AlwaysConvert.ToInt(ProductGrid.DataKeys[row.DataItemIndex].Value);
                    Product product = ProductDataSource.Load(productId);
                    if (product != null)
                    {
                        product.WarehouseId = newWarehouseId;
                        product.Save();
                    }
                }
            }
            ProductGrid.DataBind();
        }
    }

    protected void ProductGrid_DataBound(object sender, EventArgs e)
    {
        foreach (GridViewRow gvr in ProductGrid.Rows)
        {
            CheckBox cb = (CheckBox)gvr.FindControl("Selected");
            ScriptManager.RegisterArrayDeclaration(ProductGrid, "CheckBoxIDs", String.Concat("'", cb.ClientID, "'"));
        }
    }
    


}
