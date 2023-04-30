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

public partial class Admin_Payment_GatewayGroups_Templates : MakerShop.Web.UI.MakerShopAdminPage
{

    protected void Grid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        int id = (int)Grid.DataKeys[e.NewEditIndex].Value;
        PaymentGatewayTemplate pt = PaymentGatewayTemplateDataSource.Load(id);
        if (pt != null)
        {
            AddPanel.Visible = false;
            EditPanel.Visible = true;
            EditCaption.Text = string.Format(EditCaption.Text, pt.Name);
            ASP.admin_payment_gatewaygroups_editpaymentgatewaygrouptemplatedialog_ascx 
                editDialog = EditPanel.FindControl("EditDialog1") 
                as ASP.admin_payment_gatewaygroups_editpaymentgatewaygrouptemplatedialog_ascx;
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
        TemplatesDs.SelectParameters[0].DefaultValue = sqlCriteria;
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

    protected void BindPage()
    {
        Grid.DataSource = PaymentGatewayGroupsDataSource.LoadForCriteria("");
        Grid.DataBind();
    }

 
    protected void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Delete")
        {
           // TemplatesDs.DeleteParameters[0].DefaultValue = e.CommandArgument.ToString();
          //  TemplatesDs.Delete();
        }

        
        Grid.DataBind();
    }

    protected string GetGatewaysGroup(object dataItem)
    {
        PaymentGatewayGroups pgGroup = (PaymentGatewayGroups)dataItem;
        List<string> pgs = new List<string>();

        foreach (PaymentGateways pg in pgGroup.PaymentGateways)
        {
            if (!pgs.Contains(pg.Name))
                pgs.Add(pg.Name);
        }
        if (pgs.Count == 0) return string.Empty;
        return string.Join("<br/>", pgs.ToArray());
    }


}
