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


public partial class Admin_Taxes_EditTaxRule : MakerShop.Web.UI.MakerShopAdminPage
{
    private int _TaxRuleId;
    private TaxRule _TaxRule;

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
        RoundingRule.Items.Add(new ListItem("Round to Even", ((int)MakerShop.Taxes.RoundingRule.RoundToEven).ToString()));
        RoundingRule.Items.Add(new ListItem("Always Round Up", ((int)MakerShop.Taxes.RoundingRule.AlwaysRoundUp).ToString()));
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        trZoneList.Visible = (ZoneRule.SelectedIndex > 0);
        trAddressNexus.Visible = trZoneList.Visible;
        trGroupList.Visible = (GroupRule.SelectedIndex > 0);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        _TaxRuleId = AlwaysConvert.ToInt(Request.QueryString["TaxRuleId"]);
        _TaxRule = TaxRuleDataSource.Load(_TaxRuleId);
        if (_TaxRule == null) Response.Redirect("TaxRules.aspx");
        Caption.Text = string.Format(Caption.Text, _TaxRule.Name);
        if (!Page.IsPostBack)
        {
            Name.Text = _TaxRule.Name;
            foreach (ListItem taxCodeItem in this.TaxCodes.Items)
            {
                taxCodeItem.Selected = (_TaxRule.TaxRuleTaxCodes.IndexOf(_TaxRuleId, AlwaysConvert.ToInt(taxCodeItem.Value)) > -1);
            }
            TaxRate.Text = string.Format("{0:0.00##}", _TaxRule.TaxRate);
            ZoneRule.SelectedIndex = (_TaxRule.TaxRuleShipZones.Count == 0) ? 0 : 1;
            foreach (TaxRuleShipZone item in _TaxRule.TaxRuleShipZones)
            {
                ListItem listItem = ZoneList.Items.FindByValue(item.ShipZoneId.ToString());
                if (listItem != null) listItem.Selected = true;
            }
            UseBillingAddress.SelectedIndex = _TaxRule.UseBillingAddress ? 1 : 0;
            GroupRule.SelectedIndex = _TaxRule.GroupRuleId;
            foreach (TaxRuleGroup item in _TaxRule.TaxRuleGroups)
            {
                ListItem listItem = GroupList.Items.FindByValue(item.GroupId.ToString());
                if (listItem != null) listItem.Selected = true;
            }
            ListItem selectedCode = TaxCode.Items.FindByValue(_TaxRule.TaxCodeId.ToString());
            if (selectedCode != null) selectedCode.Selected = true;
            Priority.Text = _TaxRule.Priority.ToString();

            
            ListItem roundingRuleItem = RoundingRule.Items.FindByValue(_TaxRule.RoundingRuleId.ToString());
            if (roundingRuleItem != null)
            {
                RoundingRule.SelectedItem.Selected = false;
                roundingRuleItem.Selected = true;
            }

            PerUnitCalculation.Checked = _TaxRule.UsePerItemTax;
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            //CHECK IF NAME WAS CHANGED
            if (_TaxRule.Name != Name.Text)
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
            }
            //SAVE TAX RULE
            _TaxRule.Name = Name.Text;
            _TaxRule.TaxRate = AlwaysConvert.ToDecimal(TaxRate.Text);
            _TaxRule.UseBillingAddress = AlwaysConvert.ToBool(UseBillingAddress.SelectedValue.Equals("1"), false);
            _TaxRule.TaxCodeId = AlwaysConvert.ToInt(TaxCode.SelectedValue);
            _TaxRule.Priority = AlwaysConvert.ToInt16(Priority.Text);
            _TaxRule.UsePerItemTax = PerUnitCalculation.Checked;
            _TaxRule.Save();
            //UPDATE TAX CODES
            _TaxRule.TaxRuleTaxCodes.DeleteAll();
            foreach (ListItem listItem in TaxCodes.Items)
            {
                if (listItem.Selected)
                {
                    _TaxRule.TaxRuleTaxCodes.Add(new TaxRuleTaxCode(_TaxRule.TaxRuleId, AlwaysConvert.ToInt(listItem.Value)));
                    listItem.Selected = false;
                }
            }
            //UPDATE ZONES
            _TaxRule.TaxRuleShipZones.DeleteAll();
            if (ZoneRule.SelectedIndex > 0)
            {
                foreach (ListItem item in ZoneList.Items)
                {
                    if (item.Selected) _TaxRule.TaxRuleShipZones.Add(new TaxRuleShipZone(_TaxRule.TaxRuleId, AlwaysConvert.ToInt(item.Value)));
                }
            }
            //UPDATE GROUP FILTER
            _TaxRule.TaxRuleGroups.DeleteAll();
            _TaxRule.GroupRule = (FilterRule)GroupRule.SelectedIndex;
            if (_TaxRule.GroupRule != FilterRule.All)
            {
                foreach (ListItem item in GroupList.Items)
                {
                    if (item.Selected) _TaxRule.TaxRuleGroups.Add(new TaxRuleGroup(_TaxRule.TaxRuleId, AlwaysConvert.ToInt(item.Value)));
                }
            }
            //IF NO GROUPS ARE SELECTED, APPLY TO ALL GROUPS
            if (_TaxRule.TaxRuleGroups.Count == 0) _TaxRule.GroupRule = FilterRule.All;

            // UPDATE ROUNDING RULE
            _TaxRule.RoundingRuleId = AlwaysConvert.ToByte(RoundingRule.SelectedValue);

            _TaxRule.Save();
            //UPDATE THE ADD MESSAGE
            Response.Redirect("TaxRules.aspx");
        }
    }
}
