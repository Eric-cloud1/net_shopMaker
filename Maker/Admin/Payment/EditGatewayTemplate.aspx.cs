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
using System.Linq;
using System.Text.RegularExpressions;
using MakerShop.Common;
using MakerShop.Catalog;
using MakerShop.Orders;
using MakerShop.Products;
using MakerShop.Stores;
using MakerShop.Users;
using MakerShop.Utility;
using MakerShop.Payments;
using System.Collections.Generic;
using MakerShop.Payments.Providers;

public partial class Admin_Payment_EditGatewayTemplate : MakerShop.Web.UI.MakerShopAdminPage
{
    public int PaymentGatewayTemplateId
    {
        get { return AlwaysConvert.ToInt(ViewState["PaymentGatewayTemplateId"]); }
        set { ViewState["PaymentGatewayTemplateId"] = value; }
    }

 

    protected void Page_Load(object sender, EventArgs e)
    {
        SavedMessage.Visible = false;
        ErrorMessageLabel.Visible = false;

        if (!IsPostBack)
        {
            PaymentGatewayTemplateId = int.Parse(Request["PaymentGatewayTemplateId"]);

            if (PaymentGatewayTemplateId == 1)
                chNotActive.Checked = false;
             


            Caption.Text = string.Format(Caption.Text, PaymentGatewayTemplateDataSource.Load(PaymentGatewayTemplateId).Name);
            LoadData(false);
        }

    }


 

    protected void Check_Clicked(Object sender, EventArgs e)
    {
        // refresh drop down with active/not active data
        LoadData(false);

        if (chNotActive.Checked == true)
        {
            ActiveMessage.Visible = true;
            ActiveMessage.Text = "Payment method selected no longer active.";
        }
    }


    private void LoadData(bool add)
    {
        PaymentGatewayAllocationCollection pga = PaymentGatewayAllocationDataSource.Load(PaymentGatewayTemplateId, (PaymentInstrument)short.Parse(ddlInstrament.SelectedValue));
        if (add || pga.Count == 0)
        {
            PaymentGatewayAllocation x = new PaymentGatewayAllocation(PaymentGatewayTemplateId, -1);
            x.PriorityPercent = 0;
            pga.Add(x);
        }

       
        
        grid.DataSource = pga;
        grid.DataKeyNames = new string[] { "PaymentMethodId" };
        grid.DataBind();
    }
    protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.DataRow)
            return;

        DropDownList ddlPM = (DropDownList)e.Row.FindControl("ddlPM");
        TextBox tbAllocation = (TextBox)e.Row.FindControl("tbAllocation");
        //Label activeMessage = (Label)e.Row.FindControl("ActiveMessage");


        if (chNotActive.Checked == true)
            ddlPM.DataSource = PaymentMethodDataSource.LoadForCriteria("PaymentInstrumentId = " + ddlInstrament.SelectedValue + " AND isActive =0 and pm.Active = 1");
    

        else
            ddlPM.DataSource = PaymentMethodDataSource.LoadForCriteria("PaymentInstrumentId = " + ddlInstrament.SelectedValue + " AND isActive =1 and pm.Active = 1");
    

        ddlPM.DataBind();

       
       

       List<string> listCopy = new List<string>();

       foreach (ListItem item in ddlPM.Items)
                    listCopy.Add(item.Value);

        string paymentMethodId = ((PaymentGatewayAllocation)e.Row.DataItem).PaymentMethodId.ToString();


        if (listCopy.Contains(paymentMethodId))
        {
            ddlPM.SelectedValue = ((PaymentGatewayAllocation)e.Row.DataItem).PaymentMethodId.ToString();
            ActiveMessage.Visible = false;
        }
        else
        {
            ActiveMessage.Visible = true;
            ddlPM.Items.Insert(0, new ListItem("-- Select One --", "-1"));
            ActiveMessage.Text = "Payment method selected no longer active.";
        }

       
        tbAllocation.Text = ((PaymentGatewayAllocation)e.Row.DataItem).PriorityPercent.ToString();
    }

    protected void addRow_Click(object sender, ImageClickEventArgs e)
    {
        SaveButton_Click(null, null);
        LoadData(true);
    }
    protected void SaveButton_Click(object sender, ImageClickEventArgs e)
    {
        PaymentGatewayAllocationCollection c =new PaymentGatewayAllocationCollection();

        foreach (GridViewRow r in grid.Rows)
        {
            DropDownList ddlPM = (DropDownList)r.FindControl("ddlPM");
            TextBox tbAllocation = (TextBox)r.FindControl("tbAllocation");

            int paymentmethod;
            try
            {
                paymentmethod = int.Parse(ddlPM.SelectedValue);
            }
            catch
            {
                continue;
            }
            PaymentGatewayAllocation pga;

            pga = new PaymentGatewayAllocation(PaymentGatewayTemplateId, paymentmethod);
            pga.PriorityPercent = byte.Parse(tbAllocation.Text);
            c.Add(pga);
        }
        if (c.TotalAllocation != 100)
        {
            ErrorMessageLabel.Text = "Total Allocation is " + c.TotalAllocation.ToString() + ". It must be 100";
            ErrorMessageLabel.Visible = true;
            return;
        }

        c.Save(ddlInstrament.SelectedValue);
        SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
        SavedMessage.Visible = true;
    }
    protected void ddlInstrament_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadData(false);
    }
    protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Del")
        {
            int paymentMethodId = int.Parse(e.CommandArgument.ToString());
            if (paymentMethodId != -1)
                PaymentGatewayAllocationDataSource.Delete(PaymentGatewayTemplateId, paymentMethodId);

            
        }
        LoadData(false);
    }
   
}