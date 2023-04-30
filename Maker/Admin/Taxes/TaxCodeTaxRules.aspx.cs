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
using MakerShop.Taxes;

public partial class Admin_Taxes_TaxCodeTaxRules : MakerShop.Web.UI.MakerShopAdminPage
{


    private int _TaxCodeId;
    private TaxCode _TaxCode;
    protected void Page_Load(object sender, EventArgs e)
    {
        _TaxCodeId = AlwaysConvert.ToInt(Request.QueryString["TaxCodeId"]);
        _TaxCode  = TaxCodeDataSource.Load(_TaxCodeId);
        if (_TaxCode == null) Response.Redirect("TaxCodes.aspx");
        if (!Page.IsPostBack)
        {
            Caption.Text = string.Format(Caption.Text, _TaxCode.Name);
        }
    }

    protected bool IsLinked(object dataItem)
    {
        TaxRule rule = (TaxRule)dataItem;
        return (rule.TaxRuleTaxCodes.IndexOf(rule.TaxRuleId, _TaxCodeId) > -1);
    }

    protected void Linked_CheckChanged(object sender, EventArgs e)
    {
        CheckBox linked = (CheckBox)sender;
        int taxRuleId = AlwaysConvert.ToInt(linked.Text);
        TaxRule taxRule = TaxRuleDataSource.Load(taxRuleId);
        if (linked.Checked)
        {
            //ADD IF NOT FOUND
            if (taxRule.TaxRuleTaxCodes.IndexOf(taxRuleId, _TaxCodeId) < 0)
            {
                taxRule.TaxRuleTaxCodes.Add(new TaxRuleTaxCode(taxRuleId, _TaxCodeId));
                taxRule.Save();
            }
        }
        else
        {
            //DELETE IF FOUND
            int index = taxRule.TaxRuleTaxCodes.IndexOf(taxRuleId, _TaxCodeId);
            if (index > -1)
            {
                taxRule.TaxRuleTaxCodes.DeleteAt(index);
            }
        }
        TaxRuleGrid.DataBind();
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        _TaxCode.TaxRuleTaxCodes.DeleteAll();
        foreach (GridViewRow gvr in TaxRuleGrid.Rows)
        {
            CheckBox Linked = gvr.FindControl("Linked") as CheckBox;
            if (Linked.Checked)
            {
                int taxRuleId = (int)TaxRuleGrid.DataKeys[gvr.DataItemIndex].Value;
                _TaxCode.TaxRuleTaxCodes.Add(new TaxRuleTaxCode(taxRuleId, _TaxCodeId));
            }
        }
        _TaxCode.Save();
        SavedMessage.Visible = true;
        SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
    }

}
