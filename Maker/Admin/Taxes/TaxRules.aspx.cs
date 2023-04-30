using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Common;
using MakerShop.Taxes;
using MakerShop.Taxes.Providers.MakerShop;
using MakerShop.Utility;

public partial class Admin_Taxes_TaxRules : MakerShop.Web.UI.MakerShopAdminPage
{
    private bool TaxesEnabled
    {
        get
        {
            int taxGatewayId = TaxGatewayDataSource.GetTaxGatewayIdByClassId(Misc.GetClassId(typeof(MakerShopTax)));
            if (taxGatewayId == 0) return false;
            TaxGateway taxGateway = TaxGatewayDataSource.Load(taxGatewayId);
            if (taxGateway == null) return false;
            MakerShopTax provider = taxGateway.GetProviderInstance() as MakerShopTax;
            if (provider == null) return false;
            return provider.Enabled;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!TaxesEnabled)
        {
            int taxRuleCount = TaxRuleDataSource.CountForStore();
            TaxesDisabledPanel.Visible = (taxRuleCount > 0);
        }
    }

    protected string GetTaxCodes(object dataItem)
    {
        TaxRule taxRule = (TaxRule)dataItem;
        List<string> taxCodes = new List<string>();
        foreach (TaxRuleTaxCode tc in taxRule.TaxRuleTaxCodes)
        {
            taxCodes.Add(tc.TaxCode.Name);
        }
        return (string.Join(", ", taxCodes.ToArray()));
    }

    protected string GetZoneNames(object dataItem)
    {
        TaxRule rule = (TaxRule)dataItem;
        if (rule.TaxRuleShipZones.Count == 0) return "No filter";
        List<string> zones = new List<string>();
        foreach (TaxRuleShipZone zoneAssn in rule.TaxRuleShipZones)
        {
            zones.Add(zoneAssn.ShipZone.Name);
        }
        string zoneList = string.Join(", ", zones.ToArray());
        if ((zoneList.Length > 100))
        {
            zoneList = (zoneList.Substring(0, 100) + "...");
        }
        if (rule.UseBillingAddress)
            return "Billing address in: " + zoneList;
        else return "Shipping address in: " + zoneList;
    }

    protected string GetGroupNames(object dataItem)
    {
        TaxRule rule = (TaxRule)dataItem;
        if (rule.GroupRule == FilterRule.All) return "No filter";
        List<string> groups = new List<string>();
        foreach (TaxRuleGroup grpAssn in rule.TaxRuleGroups)
        {
            groups.Add(grpAssn.Group.Name);
        }
        string grpList = string.Join(", ", groups.ToArray());
        if ((grpList.Length > 100))
        {
            grpList = (grpList.Substring(0, 100) + "...");
        }
        if (rule.GroupRule == FilterRule.ExcludeSelected)
            grpList = "Exclude: " + grpList;
        else grpList = "Include: " + grpList;
        return grpList;
    }

    protected string GetTaxCodeName(object dataItem)
    {
        TaxRule rule = (TaxRule)dataItem;
        TaxCode code = rule.TaxCode;
        if (code == null) return string.Empty;
        return code.Name;
    }
}
