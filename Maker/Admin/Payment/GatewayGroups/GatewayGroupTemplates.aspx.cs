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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MakerShop.Common;
using MakerShop.Catalog;
using MakerShop.Orders;
using MakerShop.Products;
using MakerShop.Stores;
using MakerShop.Users;
using MakerShop.Utility;
using MakerShop.Payments;

public partial class Admin_Payment_GatewayGroups_GatewayGroupTemplates : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        string sqlCriteria = "";
        PaymentGatewaysGroupDs.SelectParameters[0].DefaultValue = sqlCriteria;
    }


    protected string GetGroupGateways(object dataItem)
    {
        PaymentGatewayGroups pgGroup = (PaymentGatewayGroups)dataItem;
        List<string> pgs = new List<string>();

        foreach (PaymentGateways pg in pgGroup.PaymentGateways)
        {
            if (!pgs.Contains(pg.Name))
                pgs.Add(pg.Name);
        }
        if (pgs.Count == 0) return string.Empty;
        return string.Join("<br/>", pgs.ToArray());
    }



}
