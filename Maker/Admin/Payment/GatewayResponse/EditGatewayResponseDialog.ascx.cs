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
using MakerShop.Utility;
using MakerShop.Common;
using MakerShop.Users;

public partial class Admin_Payment_EditGatewayResponseDialog : System.Web.UI.UserControl
{
    //DEFINE AN EVENT TO TRIGGER WHEN AN ITEM IS ADDED 
    public event PersistentItemEventHandler ItemUpdated;
    public event EventHandler Cancelled;
    private const bool ShowIntlPaymentMethods = true;

    public int PaymentGatewayResponseId
    {
        get { return AlwaysConvert.ToInt(ViewState["PaymentGatewayResponseId"]); }
        set { ViewState["PaymentGatewayResponseId"] = value; }
    }

    public void loadSubscriptionStatus()
    {
            DataTable ds = GatewayResponseActionDataSource.getSubscriptionStatus();
        
            ddlSubscriptionStatus.DataSource = ds;
            ddlSubscriptionStatus.DataTextField = "SubscriptionStatus";
            ddlSubscriptionStatus.DataValueField = "SubscriptionStatusCode";
            ddlSubscriptionStatus.DataBind();

            ddlSubscriptionStatus.Items.Insert(0, new ListItem(" None ", "-1"));
            ddlSubscriptionStatus.SelectedIndex = 0;

    }

    public void LoadDialog(int paymentGatewayResponseId)
    {
        PaymentGatewayResponseId = paymentGatewayResponseId;

        GatewayResponseAction gra = GatewayResponseActionDataSource.Load(paymentGatewayResponseId);

        this.loadSubscriptionStatus();

        this.response.Text = gra.Response;




        if (gra.Cancel == true)
            cbType.Items[0].Selected = true;
        if (gra.Decline == true)
            cbType.Items[1].Selected = true;
        if (gra.Fraud == true)
            cbType.Items[2].Selected = true;

   
        


        this.ddlSubscriptionStatus.SelectedIndex = 
            this.ddlSubscriptionStatus.Items.IndexOf(this.ddlSubscriptionStatus.Items.FindByValue(gra.SubscriptionStatusCode.ToString()));


    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        //TRIGER ANY EVENT ATTACHED TO THE CANCEL
        if (Cancelled != null) Cancelled(sender, e);
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {

        if (Page.IsValid)
        {
            if (ddlSubscriptionStatus.SelectedIndex == 0)
            {
                ErrorMessage.Text = "You must select a Subscription Status.";
                ErrorMessage.Visible = true;
                return;
            }

            if (this.response.Text.Trim() == string.Empty)
            {
                ErrorMessage.Text = "Enter a response value.";
                ErrorMessage.Visible = true;
                return;
            }

            GatewayResponseAction gra = GatewayResponseActionDataSource.Load(PaymentGatewayResponseId);

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

                

                item ++;
            }

          

            gra.Response = this.response.Text;
            gra.SubscriptionStatusCode = byte.Parse(this.ddlSubscriptionStatus.SelectedValue);
            //UPDATE THE ADD MESSAGE
            AddedMessage.Text = string.Format(AddedMessage.Text, gra.Response);
            AddedMessage.Visible = true;

            this.response.Text = string.Empty;
            this.ddlSubscriptionStatus.SelectedIndex = 0;

            gra.Save();
            //TRIGER ANY EVENT ATTACHED TO THE UPDATE
            if (ItemUpdated != null) ItemUpdated(this, new PersistentItemEventArgs(gra.GatewayResponseId, gra.Response));
        }
    }


}
