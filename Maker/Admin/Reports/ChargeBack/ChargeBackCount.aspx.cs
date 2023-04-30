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
using MakerShop.Payments;
using MakerShop.Reporting;
using System.Collections.Generic;
using MakerShop.Common;
using MakerShop.Utility;
using System.Linq;
using System.IO;
using MakerShop.Data;


public partial class Admin_Reports_ChargeBack_ChargeBackCount : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ChargeBackDate.SelectedDate = DateTime.Now.Date;
            Database database = Token.Instance.Database;
            string sql = @"SELECT PaymentGatewayId, Name + ' (' + CONVERT(nvarchar(50),PaymentGatewayId) +')' as Name FROM dbo.ac_PaymentGateways ORDER BY Name";
            using (System.Data.Common.DbCommand getCommand = database.GetSqlStringCommand(sql))
                
            {
                this.dlPaymentGatewaysId.DataSource = database.ExecuteDataSet(getCommand);
                this.dlPaymentGatewaysId.DataTextField = "Name";
                this.dlPaymentGatewaysId.DataValueField ="PaymentGatewayId";
                this.dlPaymentGatewaysId.DataBind();
                
            }

            BindGrid();
            
        }

    }

    protected void BindGrid()
    {
        ChargeBackCountCollection ds = ChargeBackCountDataSource.LoadByChargeBackDate(ChargeBackDate.SelectedDate);
      
        this.ChargeBackGrid.DataSource = ds;
        ChargeBackGrid.DataBind();

    }


    protected void ProcessButton_Click(object sender, EventArgs e)
    {
   
        BindGrid();
    }


    protected string FormatDate(object dataItem)
    {
        ChargeBackCount a = (ChargeBackCount)dataItem;
      
        return a.ChargeBackDate.ToShortDateString();

    }

    protected string FormatName(object dataItem)
    {
        ChargeBackCount a = (ChargeBackCount)dataItem;

        return string.Format(@"{0} ({1})",a.PaymentGateway.Name, a.PaymentGatewayId);

    }

    protected string FormatInstrument(object dataItem)
    {
        ChargeBackCount a = (ChargeBackCount)dataItem;

        string Instrument = string.Empty;

        switch (a.PaymentInstrumentId)
        {
            case 1: Instrument = "Visa"; break;
            case 2: Instrument = "MC"; break;
            default: Instrument = "Visa"; break;
        }
        return Instrument;

    }




    protected void AddNewButton_Click(object sender, EventArgs e)
    {
        ChargeBackCount cb = new ChargeBackCount();
        cb.CreateUser = Token.Instance.User.UserName;
        cb.ChangeUser = Token.Instance.User.UserName;
        cb.CreateDate = System.DateTime.Now.Date;
        cb.ChangeDate = System.DateTime.Now.Date;
        cb.PaymentInstrumentId = int.Parse(paymentInstrument.SelectedValue);

        cb.ChargeBackDate = ChargeBackDate.SelectedDate;

        int isPaymentGatewayId = 0;
        int.TryParse(this.dlPaymentGatewaysId.SelectedValue, out isPaymentGatewayId);
        cb.PaymentGatewayId = isPaymentGatewayId;

        if (isPaymentGatewayId == 0)
            return;

         int isCount = 0;
         int.TryParse(this.txtChargeBackCount.Text, out isCount);
         cb.Count = isCount;

        if(cb.Count !=0)
            ChargeBackCountDataSource.Insert(cb);

        this.txtChargeBackCount.Text = string.Empty;
        BindGrid();
    }

   

    protected void ChargeBackGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        ChargeBackGrid.PageIndex = e.NewPageIndex;
       

        BindGrid();
    }

    protected void ChargeBackGrid_EditCommand(object sender, GridViewEditEventArgs e)
    {
        ChargeBackGrid.EditIndex = e.NewEditIndex;


        BindGrid();
    }

    protected void ChargeBackGrid_CancelCommand(object sender, GridViewCancelEditEventArgs e)
    {
        ChargeBackGrid.EditIndex = -1;
        BindGrid();

    }

    protected void ChargeBackGrid_UpdateCommand(Object Sender, GridViewUpdateEventArgs e)
    {
        ChargeBackCount cb = new ChargeBackCount();

        int index = ChargeBackGrid.EditIndex;
        GridViewRow row = ChargeBackGrid.Rows[index];


        cb.ChangeUser = Token.Instance.User.UserName;
        cb.ChangeDate = System.DateTime.Now.Date;
        cb.PaymentGatewayId = int.Parse(((System.Web.UI.WebControls.Label)row.FindControl("lbIdGateway")).Text);
        cb.ChargeBackDate = DateTime.Parse(((System.Web.UI.WebControls.Label)row.FindControl("lbIdChargeBackDate")).Text);
      
        cb.Count = int.Parse(((System.Web.UI.WebControls.TextBox)row.FindControl("txtCount")).Text);
        cb.PaymentInstrumentId = int.Parse(((System.Web.UI.WebControls.DropDownList)row.FindControl("dlPaymentInstrumentId")).SelectedValue);


        if (cb.PaymentInstrumentId != 0)
            ChargeBackCountDataSource.Update(cb);

        if (cb.PaymentInstrumentId == 0)
        {
            cb.PaymentInstrumentId = int.Parse(((System.Web.UI.WebControls.Label)row.FindControl("lbIdPaymentInstrumentId")).Text);
            ChargeBackCountDataSource.Delete(cb);
        }
        
        this.ChargeBackGrid.EditIndex = -1;
        BindGrid();
    }


}
