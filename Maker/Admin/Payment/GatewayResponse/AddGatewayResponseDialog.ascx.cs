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

public partial class Admin_Payment_AddGatewayResponseDialog : System.Web.UI.UserControl
{
    //DEFINE AN EVENT TO TRIGGER WHEN AN ITEM IS ADDED 
    public event PersistentItemEventHandler ItemAdded;
    private const bool ShowIntlPaymentMethods = true;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            DataTable ds = GatewayResponseActionDataSource.getSubscriptionStatus();
            ddlSubscriptionStatus.DataSource = ds;
            ddlSubscriptionStatus.DataTextField = "SubscriptionStatus";
            ddlSubscriptionStatus.DataValueField = "SubscriptionStatusCode";
            ddlSubscriptionStatus.DataBind();

            ddlSubscriptionStatus.Items.Insert(0, new ListItem(" None ", "-1"));
            ddlSubscriptionStatus.SelectedIndex = 0;
  
         
        }
    }

    protected void AddButton_Click(object sender, System.EventArgs e)
    {
        if (ddlSubscriptionStatus.SelectedIndex == 0)
        {
            ErrorMessage.Text = "Select a subscription status.";
            ErrorMessage.Visible = true;    
            return;
        }

        if (this.response.Text.Trim() == string.Empty)
        {
            ErrorMessage.Text = "Enter a response value.";
            ErrorMessage.Visible = true;
            return;
        }

        GatewayResponseAction gra = new GatewayResponseAction();

        int item = 0;

        foreach (ListItem ids in cbType.Items)
        {

            switch (item)
            {
                case 0:
                    gra.Cancel = false;
                    break;
                case 1:
                    gra.Decline = false;
                    break;
                case 2:
                    gra.Fraud = false;
                    break;
            }



            if (ids.Selected == true)
            {
                switch (item)
                {
                    case 0:
                        gra.Cancel = true;
                        break;
                    case 1:
                        gra.Decline = true;
                        break;
                    case 2:
                        gra.Fraud = true;
                        break;
                }
            }



            item++;
        }



        gra.Response = this.response.Text;
        gra.SubscriptionStatusCode = byte.Parse(this.ddlSubscriptionStatus.SelectedValue);
        gra.Save(); 

        //UPDATE THE ADD MESSAGE
        AddedMessage.Text = string.Format(AddedMessage.Text, gra.Response);
        AddedMessage.Visible = true;

        //RESET THE ADD FORM
        this.response.Text = string.Empty;
        this.ddlSubscriptionStatus.SelectedIndex = 0;
        
        //TRIGER ANY EVENT ATTACHED TO THE UPDATE
        if (ItemAdded != null) ItemAdded(this, new PersistentItemEventArgs(gra.GatewayResponseId, gra.Response));
    }

}
