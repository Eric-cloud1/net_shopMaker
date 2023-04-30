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
using MakerShop.Reporting;
using System.Collections.Generic;
using MakerShop.Shipping;
using MakerShop.Shipping.Providers.FedEx;

public partial class Admin_Shipping_FedEx_Register2 : MakerShop.Web.UI.MakerShopAdminPage
{
   
    

    protected void Page_Load(object sender, EventArgs e)
    {  
        if (!Page.IsPostBack)
        {
            FedEx provider = new FedEx();
            FedExAccountNumber.Text = provider.AccountNumber;
            FedExMeterNumber.Text = provider.MeterNumber;

            //CHECK WHETHER ANY PROVIDERS HAVE BEEN CONFIGURED
            ShipGatewayCollection configuredProviders = ShipGatewayDataSource.LoadForClassId(ClassId);
            if (configuredProviders.Count > 0)
            {
                trInstanceName.Visible = true;
                InstanceName.Text = new FedEx().Name + " #" + ((int)(configuredProviders.Count + 1)).ToString();
            }
        }
    }
    
    protected string ClassId
    {
        get
        {
            return Misc.GetClassId(typeof(FedEx));
        }
    }

    protected void RedirectToManageProvider()
    {
        FedEx provider = new FedEx();
        Response.Redirect("../Default.aspx?ProviderClassID=" + HttpUtility.UrlEncode(Misc.GetClassId(provider.GetType())));
    }

    protected void FinishButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            ShipGateway gateway = new ShipGateway();
            gateway.Name = InstanceName.Text;
            FedEx provider = new FedEx();
            gateway.ClassId = Misc.GetClassId(provider.GetType());
            provider.AccountNumber = FedExAccountNumber.Text;
            provider.MeterNumber = FedExMeterNumber.Text;
            gateway.UpdateConfigData(provider.GetConfigData());
            gateway.Enabled = true;
            gateway.Save();
            Response.Redirect("Configure.aspx?ShipGatewayId=" + gateway.ShipGatewayId.ToString());
        }
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        RedirectToManageProvider();
    }

}
