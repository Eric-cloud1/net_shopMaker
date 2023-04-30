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
using MakerShop.Payments.Providers;
using MakerShop.Payments;
using System.Collections.Generic;

public partial class Admin_Payment_Gateways : MakerShop.Web.UI.MakerShopAdminPage
{

    /*
    protected void Page_Init(object sender, EventArgs e)
    {
        string sqlCriteria = " Name<>'Gift Certificate Payment Provider' AND ClassId<>'" + StringHelper.SafeSqlString(GetGiftCertPayGatewayId()) + "'";
        if (!cbShowActive.Checked)
            sqlCriteria += " AND IsActive=1 ";
        PaymentGatewayDs.SelectParameters[0].DefaultValue = sqlCriteria;
    }*/
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GatewayNames.DataSource = MakerShop.Payments.PaymentGateway.GetGateways();
        
            GatewayNames.DataTextField = "Name";
            GatewayNames.DataValueField = "Name";

            GatewayNames.DataBind();

            GatewayNames.Items.Insert(0, new ListItem("-- All --", "-1"));
            GatewayNames.SelectedIndex = 0;
        }
            BindGrid();
        
    }

    private void BindGrid()
    {
        string sqlCriteria = string.Empty;
       
        sqlCriteria = " Name<>'Gift Certificate Payment Provider' AND ClassId<>'" + StringHelper.SafeSqlString(GetGiftCertPayGatewayId()) + "'";
        if (!cbShowActive.Checked) sqlCriteria += " AND IsActive=1 ";
        
        if(GatewayNames.SelectedIndex != 0)
        {
            sqlCriteria = " Name<>'Gift Certificate Payment Provider' AND name like  '%" + GatewayNames.SelectedValue + "%'";
            if (!cbShowActive.Checked) sqlCriteria += " AND IsActive=1 ";
        }
        sqlCriteria += " ORDER BY Name";
        PaymentGatewayDs.SelectParameters[0].DefaultValue = sqlCriteria;
        GatewayGrid.DataBind();

    }

   

    private string GetGiftCertPayGatewayId()
    {
        return Misc.GetClassId(typeof(MakerShop.Payments.Providers.GiftCertificatePaymentProvider));
    }

    protected void GatewayGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Delete")
        {
            // get paymentGatewayId  of the clicked row
            int paymentGatewayId = Convert.ToInt32(e.CommandArgument);
            PaymentGateway gateway = new PaymentGateway();
            bool HasValue = gateway.LoadPayments(paymentGatewayId);

            if (HasValue)
            {
                gateway.Load(paymentGatewayId);
                gateway.IsActive = false;
                gateway.Save();
            }
            else
            {
                gateway.Delete();

                
                BindGrid();
            }

        }
    }

    protected string GetSupportedTransactions(object dataItem)
    {
        IPaymentProvider provider = ((PaymentGateway)dataItem).GetInstance();
        List<string> supportedFeatures = new List<string>();
        if (provider != null)
        {
            if ((provider.SupportedTransactions & SupportedTransactions.Authorize) != SupportedTransactions.None) supportedFeatures.Add(SupportedTransactions.Authorize.ToString());
            if ((provider.SupportedTransactions & SupportedTransactions.AuthorizeCapture) != SupportedTransactions.None) supportedFeatures.Add(StringHelper.SpaceName(SupportedTransactions.AuthorizeCapture.ToString()));
            if ((provider.SupportedTransactions & SupportedTransactions.Capture) != SupportedTransactions.None) supportedFeatures.Add(SupportedTransactions.Capture.ToString());
            if ((provider.SupportedTransactions & SupportedTransactions.PartialCapture) != SupportedTransactions.None) supportedFeatures.Add(StringHelper.SpaceName(SupportedTransactions.PartialCapture.ToString()));
            if ((provider.SupportedTransactions & SupportedTransactions.Void) != SupportedTransactions.None) supportedFeatures.Add(SupportedTransactions.Void.ToString());
            if ((provider.SupportedTransactions & SupportedTransactions.Refund) != SupportedTransactions.None) supportedFeatures.Add(SupportedTransactions.Refund.ToString());
            if ((provider.SupportedTransactions & SupportedTransactions.PartialRefund) != SupportedTransactions.None) supportedFeatures.Add(StringHelper.SpaceName(SupportedTransactions.PartialRefund.ToString()));
        }
        if (supportedFeatures.Count == 0) return string.Empty;
        return string.Join(", ", supportedFeatures.ToArray());
    }

    protected string GetPaymentMethods(object dataItem)
    {
        PaymentGateway gateway = (PaymentGateway)dataItem;
        List<string> paymentMethods = new List<string>();
        foreach (PaymentMethod method in gateway.PaymentMethods)
        {
            if(method.Active)
                paymentMethods.Add(method.Name);
        }
        if (paymentMethods.Count == 0) return string.Empty;
        return string.Join(", ", paymentMethods.ToArray());
    }

    protected string GetConfigReference(object dataItem)
    {
        IPaymentProvider provider = ((PaymentGateway)dataItem).GetInstance();
        if (provider != null)
        {
            return provider.ConfigReference;
        }
        return string.Empty;
    }

}
