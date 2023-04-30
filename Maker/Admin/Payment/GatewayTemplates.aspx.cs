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

public partial class Admin_Payment_GatewayTemplates : MakerShop.Web.UI.MakerShopAdminPage
{

    
    protected void Page_Init(object sender, EventArgs e)
    {
        string sqlCriteria = "";
        PaymentGatewayTemplateDs.SelectParameters[0].DefaultValue = sqlCriteria;
    }


    protected string GetSupportedGateways(object dataItem)
    {
        PaymentGatewayTemplate pgt = (PaymentGatewayTemplate)dataItem;
        List<string> pg = new List<string>();
        foreach(PaymentGatewayAllocation pga in pgt.PaymentGateways)
        {
            pg.Add(pga.PriorityPercent.ToString() + "%  " + pga.PaymentMethod.Name);
        }
        if (pg.Count == 0) return string.Empty;
        return string.Join("<br/>", pg.ToArray());
    }

    

}
