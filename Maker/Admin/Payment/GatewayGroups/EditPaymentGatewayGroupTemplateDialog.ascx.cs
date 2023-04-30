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

public partial class Admin_Payment_GatewayGroups_EditPaymentGatewayGroupTemplateDialog : System.Web.UI.UserControl
{

    //DEFINE AN EVENT TO TRIGGER WHEN AN ITEM IS ADDED 
    public event PersistentItemEventHandler ItemUpdated;
    public event EventHandler Cancelled;
    private const bool ShowIntlPaymentMethods = true;

    public int PaymentGatewayGroupId
    {
        get { return AlwaysConvert.ToInt(ViewState["PaymentGatewayGroupId"]); }
        set { ViewState["PaymentGatewayGroupId"] = value; }
    }

    public void LoadDialog(int paymentGatewayGroupId)
    {
        PaymentGatewayGroupId = paymentGatewayGroupId;
        PaymentGatewayGroups pgs = PaymentGatewayGroupsDataSource.Load(PaymentGatewayGroupId);
        Group.Text = pgs.PaymentGatewayGroup;
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
            PaymentGatewayGroups pgs = PaymentGatewayGroupsDataSource.Load(PaymentGatewayGroupId);
            pgs.PaymentGatewayGroup = Group.Text;

            pgs.Save();
            //TRIGER ANY EVENT ATTACHED TO THE UPDATE
            if (ItemUpdated != null) ItemUpdated(this, new PersistentItemEventArgs(pgs.PaymentGatewayGroupId, pgs.PaymentGatewayGroup));
        }
    }

}
