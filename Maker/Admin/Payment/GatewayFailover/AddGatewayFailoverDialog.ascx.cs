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
using System.Collections.Generic;
using MakerShop.Common;
using MakerShop.Utility;
using MakerShop.Users;

public partial class Admin_Payment_AddGatewayFailoverDialog : System.Web.UI.UserControl
{
    //DEFINE AN EVENT TO TRIGGER WHEN AN ITEM IS ADDED 
    public event PersistentItemEventHandler ItemAdded;
    private const bool ShowIntlPaymentMethods = true;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            PaymentGatewayCollection pgc = PaymentGatewayDataSource.LoadForCriteria("StoreId= " + Token.Instance.StoreId.ToString() + " AND IsActive = 1");
            ddlSource.DataSource = pgc;
            ddlSource.DataTextField = "Name";
            ddlSource.DataValueField = "PaymentGatewayId";
            ddlDestination.DataSource = pgc;
            ddlDestination.DataTextField = "Name";
            ddlDestination.DataValueField = "PaymentGatewayId";
            ddlSource.DataBind();
            ddlDestination.DataBind();
         
        }
    }

    protected void AddButton_Click(object sender, System.EventArgs e)
    {
        if (ddlSource.SelectedValue == ddlDestination.SelectedValue)
        {
            ErrorMessage.Text = "Source & Destination Cannot be the same.";
            ErrorMessage.Visible = true;    
            return;
        }
        PaymentGatewayFailover pgf = new PaymentGatewayFailover();
        pgf.TwoWay = cb2way.Checked;
        pgf.SourcePaymentGatewayId = int.Parse(ddlSource.SelectedValue);
        pgf.DestinationPaymentGatewayId = int.Parse(ddlDestination.SelectedValue);

        pgf.Save(); 

        //UPDATE THE ADD MESSAGE
        AddedMessage.Text = string.Format(AddedMessage.Text, pgf.Name);
        AddedMessage.Visible = true;

        //RESET THE ADD FORM
       // Name.Text = string.Empty;
        
        //TRIGER ANY EVENT ATTACHED TO THE UPDATE
        if (ItemAdded != null) ItemAdded(this, new PersistentItemEventArgs(pgf.PaymentGatewayFailoverId, pgf.Name));
    }

}
