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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MakerShop.Common;
using MakerShop.Catalog;
using MakerShop.Orders;
using MakerShop.Products;
using MakerShop.Stores;
using MakerShop.Users;
using MakerShop.Utility;
using MakerShop.Payments;

public partial class Admin_Payment_GatewayFailover : MakerShop.Web.UI.MakerShopAdminPage
{

    protected void Grid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        int id = (int)Grid.DataKeys[e.NewEditIndex].Value;
        PaymentGatewayFailover x = PaymentGatewayFailoverDataSource.Load(id);
        if (x != null)
        {
            AddPanel.Visible = false;
            EditPanel.Visible = true;
            EditCaption.Text = string.Format(EditCaption.Text, x.Name);
            ASP.admin_payment_gatewayfailover_editgatewayfailoverdialog_ascx editDialog = EditPanel.FindControl("EditDialog1") as ASP.admin_payment_gatewayfailover_editgatewayfailoverdialog_ascx;
            if (editDialog != null) editDialog.LoadDialog(id);
            UpdatePanel1.Update();
        }
    }

    protected void Grid_RowCancelingEdit(object sender, EventArgs e)
    {
        AddPanel.Visible = true;
        EditPanel.Visible = false;
        UpdatePanel1.Update();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        AddDialog1.ItemAdded += new PersistentItemEventHandler(AddDialog1_ItemAdded);
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        string sqlCriteria = "";
        Ds.SelectParameters[0].DefaultValue = sqlCriteria;
    }
    
    void AddDialog1_ItemAdded(object sender, PersistentItemEventArgs e)
    {
        RebindPage();
    }
    protected void EditDialog1_ItemUpdated(object sender, PersistentItemEventArgs e)
    {
        DoneEditing();
    }

    protected void EditDialog1_Cancelled(object sender, EventArgs e)
    {
        DoneEditing();
    }

    protected void DoneEditing()
    {
        AddPanel.Visible = true;
        EditPanel.Visible = false;
        Grid.EditIndex = -1;
        RebindPage();
    }

    protected void RebindPage()
    {
        Grid.DataBind();
        UpdatePanel1.Update();
    }

    protected void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        
        Grid.DataBind();
    }

   


}
