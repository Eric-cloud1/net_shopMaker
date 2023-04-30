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
using MakerShop.Shipping.Providers;

public partial class Admin_Shipping_Providers_AddGateway : MakerShop.Web.UI.MakerShopAdminPage
{

    
    protected bool IsConfigured(ShipGatewayCollection gateways, string classId)
    {
        foreach (ShipGateway gateway in gateways)
        {
            if (gateway.ClassId.Equals(classId)) return true;
        }
        return false;
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            ProviderGrid.DataSource = ShippingProviderDataSource.GetShippingProviders();
            ProviderGrid.DataBind();
        }
    }

    protected string GetClassId(object dataItem) {
        return Misc.GetClassId(dataItem.GetType());
    }
    
    protected string GetLogoUrl(object dataItem)
    {
        IShippingProvider provider = (IShippingProvider)dataItem;
        return provider.GetLogoUrl(Page.ClientScript);
    }

    protected string GetConfigUrl(object dataItem)
    {
        IShippingProvider provider = (IShippingProvider)dataItem;
        return provider.GetConfigUrl(Page.ClientScript);
    }

}
