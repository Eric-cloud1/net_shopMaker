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
using MakerShop.Common;
using MakerShop.Shipping;
using MakerShop.Taxes;
using MakerShop.Utility;
using MakerShop.Users;

public partial class Admin_Taxes_AddTaxRule : MakerShop.Web.UI.MakerShopAdminPage
{
    protected void Page_Init()
    {
        TaxCodeCollection taxCodes = TaxCodeDataSource.LoadForStore();
        TaxCodes.DataSource = taxCodes;
        TaxCodes.DataBind();
        TaxCode.Items.Clear();
        TaxCode.Items.Add("");
        TaxCode.DataSource = taxCodes;
        TaxCode.DataBind();
        ZoneList.DataSource = ShipZoneDataSource.LoadForStore("Name");
        ZoneList.DataBind();
        GroupList.DataSource = GroupDataSource.LoadForStore("Name");
        GroupList.DataBind();

        // ROUNDING RULE
        RoundingRule.Items.Clear();
        RoundingRule.Items.Add(new ListItem("Common Method", ((int)MakerShop.Taxes.RoundingRule.Common).ToString()));
        ListItem item = new ListItem("Round to Even", ((int)MakerShop.Taxes.RoundingRule.RoundToEven).ToString());
        item.Selected = true;
        RoundingRule.Items.Add(item);
        RoundingRule.Items.Add(new ListItem("Always Round Up", ((int)MakerShop.Taxes.RoundingRule.AlwaysRoundUp).ToString()));
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        trZoneList.Visible = (ZoneRule.SelectedIndex > 0);
        trAddressNexus.Visible = trZoneList.Visible;
        trGroupList.Visible = (GroupRule.SelectedIndex > 0);
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            // DUPLICATE TAX RULE NAMES SHOULD NOT BE ALLOWED                        
            int taxRuleId = TaxRuleDataSource.GetTaxRuleIdByName(Name.Text);
            if (taxRuleId > 0)
            {
                // TAX RULE(S) WITH SAME NAME ALREADY EXIST
                CustomValidator customNameValidator = new CustomValidator();
                customNameValidator.ControlToValidate = "Name";
                customNameValidator.Text = "*";
                customNameValidator.ErrorMessage = "A Tax Rule with the same name already exists.";
                customNameValidator.IsValid = false;
                phNameValidator.Controls.Add(customNameValidator);
                return;
            }
            //SAVE TAX RULE
            TaxRule taxRule = new TaxRule();
            taxRule.Name = Name.Text;
            taxRule.TaxRate = AlwaysConvert.ToDecimal(TaxRate.Text);
            taxRule.UseBillingAddress = AlwaysConvert.ToBool(UseBillingAddress.SelectedValue.Equals("1"), false);
            taxRule.TaxCodeId = AlwaysConvert.ToInt(TaxCode.SelectedValue);
            taxRule.Priority = AlwaysConvert.ToInt16(Priority.Text);
            taxRule.UsePerItemTax = PerUnitCalculation.Checked;
            taxRule.Save();
            //UPDATE TAX CODES
            taxRule.TaxRuleTaxCodes.DeleteAll();
            foreach (ListItem listItem in TaxCodes.Items)
            {
                if (listItem.Selected)
                {
                    taxRule.TaxRuleTaxCodes.Add(new TaxRuleTaxCode(taxRule.TaxRuleId, AlwaysConvert.ToInt(listItem.Value)));
                    listItem.Selected = false;
                }
            }
            //UPDATE ZONES
            taxRule.TaxRuleShipZones.DeleteAll();
            if (ZoneRule.SelectedIndex > 0)
            {
                foreach (ListItem item in ZoneList.Items)
                {
                    if (item.Selected) taxRule.TaxRuleShipZones.Add(new TaxRuleShipZone(taxRule.TaxRuleId, AlwaysConvert.ToInt(item.Value)));
                }
            }
            //UPDATE GROUP FILTER
            taxRule.TaxRuleGroups.DeleteAll();
            taxRule.GroupRule = (FilterRule)GroupRule.SelectedIndex;
            if (taxRule.GroupRule != FilterRule.All)
            {
                foreach (ListItem item in GroupList.Items)
                {
                    if (item.Selected) taxRule.TaxRuleGroups.Add(new TaxRuleGroup(taxRule.TaxRuleId, AlwaysConvert.ToInt(item.Value)));
                }
            }
            //IF NO GROUPS ARE SELECTED, APPLY TO ALL GROUPS
            if (taxRule.TaxRuleGroups.Count == 0) taxRule.GroupRule = FilterRule.All;

            // UPDATE ROUNDING RULE
            taxRule.RoundingRuleId = AlwaysConvert.ToByte(RoundingRule.SelectedValue);

            taxRule.Save();
            //UPDATE THE ADD MESSAGE
            Response.Redirect("TaxRules.aspx");
        }
    }
}
