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

public partial class Admin_Shipping_Providers_Default : MakerShop.Web.UI.MakerShopAdminPage
{

    protected List<string> GetShipMethodList(object dataItem)
    {        
        ShipGateway gateway = (ShipGateway)dataItem;
        List<string> ShipMethods = new List<string>();
        foreach (ShipMethod method in gateway.ShipMethods)
        {
            ShipMethods.Add(method.Name);
        }
        return ShipMethods;
    }
    
    protected string GetConfigReference(object dataItem)
    {
        IShippingProvider provider = ((ShipGateway)dataItem).GetProviderInstance();
        if (provider != null)
        {
            return provider.Description;
        }
        return string.Empty;
    }

    protected string GetConfigUrl(object dataItem)
    {
        ShipGateway gateway = (ShipGateway)dataItem;
        IShippingProvider provider = gateway.GetProviderInstance();
        if (provider != null)
        {
            return provider.GetConfigUrl(Page.ClientScript) + "?ShipGatewayId=" + gateway.ShipGatewayId.ToString();
        }
        return string.Empty;
    }

    protected void Enabled_CheckedChanged(object sender, EventArgs e)
    {        
        CheckBox cbox = (CheckBox)sender;
        int sgwid = AlwaysConvert.ToInt(cbox.Text);
        ShipGateway sgw = ShipGatewayDataSource.Load(sgwid);
        if (sgw == null) return;
        sgw.Enabled = cbox.Checked;
        sgw.Save();
        GatewayGrid.DataBind();        
    }


    protected void ShipMethodList_DataBinding(object sender, EventArgs e)
    {

    }
    

}
