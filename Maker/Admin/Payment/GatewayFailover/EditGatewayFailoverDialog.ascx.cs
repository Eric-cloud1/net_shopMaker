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

public partial class Admin_Payment_EditGatewayFailoverDialog : System.Web.UI.UserControl
{
    //DEFINE AN EVENT TO TRIGGER WHEN AN ITEM IS ADDED 
    public event PersistentItemEventHandler ItemUpdated;
    public event EventHandler Cancelled;
    private const bool ShowIntlPaymentMethods = true;

    public int PaymentGatewayFailoverId
    {
        get { return AlwaysConvert.ToInt(ViewState["PaymentGatewayFailoverId"]); }
        set { ViewState["PaymentGatewayFailoverId"] = value; }
    }

    public void LoadDialog(int paymentGatewayFailoverId)
    {
        PaymentGatewayFailoverId = paymentGatewayFailoverId;
        PaymentGatewayFailover pgf = PaymentGatewayFailoverDataSource.Load(PaymentGatewayFailoverId);
        PaymentGatewayCollection pgc = PaymentGatewayDataSource.LoadForCriteria("StoreId= " + Token.Instance.StoreId.ToString() + " AND IsActive = 1");
        ddlSource.DataSource = pgc;
        ddlSource.DataTextField = "Name";
        ddlSource.DataValueField = "PaymentGatewayId";
        ddlDestination.DataSource = pgc;
        ddlDestination.DataTextField = "Name";
        ddlDestination.DataValueField = "PaymentGatewayId";
        ddlSource.DataBind();
        ddlDestination.DataBind();
        ddlSource.SelectedValue = pgf.SourcePaymentGatewayId.ToString();
        ddlDestination.SelectedValue = pgf.DestinationPaymentGatewayId.ToString();
        cb2way.Checked = pgf.TwoWay;
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
            if (ddlSource.SelectedValue == ddlDestination.SelectedValue)
            {
                ErrorMessage.Text = "Source & Destination Cannot be the same.";
                ErrorMessage.Visible = true;
                return;
            }

            PaymentGatewayFailover pgf = PaymentGatewayFailoverDataSource.Load(PaymentGatewayFailoverId);
            pgf.TwoWay = cb2way.Checked;
            pgf.SourcePaymentGatewayId = int.Parse(ddlSource.SelectedValue);
            pgf.DestinationPaymentGatewayId = int.Parse(ddlDestination.SelectedValue);

            pgf.Save();
            //TRIGER ANY EVENT ATTACHED TO THE UPDATE
            if (ItemUpdated != null) ItemUpdated(this, new PersistentItemEventArgs(pgf.PaymentGatewayFailoverId, pgf.Name));
        }
    }


}
