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
using MakerShop.Taxes;
using MakerShop.Taxes.Providers.MakerShop;
using MakerShop.Utility;

public partial class Admin_Taxes_Settings : MakerShop.Web.UI.MakerShopAdminPage
{
    private int _TaxGatewayId;
    private TaxGateway _TaxGateway;
    private MakerShopTax _TaxProvider;

    protected void Page_Load(object sender, EventArgs e)
    {
        _TaxGatewayId = TaxGatewayDataSource.GetTaxGatewayIdByClassId(Misc.GetClassId(typeof(MakerShopTax)));
        if (_TaxGatewayId > 0) _TaxGateway = TaxGatewayDataSource.Load(_TaxGatewayId);
        if (_TaxGateway != null) _TaxProvider = _TaxGateway.GetProviderInstance() as MakerShopTax;
        if (_TaxGateway == null) _TaxGateway = new TaxGateway();
        if (_TaxProvider == null) _TaxProvider = new MakerShopTax();
        SavedMessage.Visible = false;

        if (!Page.IsPostBack)
        {
            Enabled.SelectedIndex = _TaxProvider.Activated ? 1 : 0;
            switch (_TaxProvider.ShoppingDisplay)
            {
                case TaxShoppingDisplay.Hide: ShoppingDisplay.SelectedIndex = 0; break;
                case TaxShoppingDisplay.Included: ShoppingDisplay.SelectedIndex = 1; break;
                case TaxShoppingDisplay.IncludedRegistered: ShoppingDisplay.SelectedIndex = 2; break;
                case TaxShoppingDisplay.LineItem: ShoppingDisplay.SelectedIndex = 3; break;
                case TaxShoppingDisplay.LineItemRegistered: ShoppingDisplay.SelectedIndex = 4; break;
            }
            switch (_TaxProvider.InvoiceDisplay)
            {
                case TaxInvoiceDisplay.Summary: InvoiceDisplay.SelectedIndex = 0; break;
                case TaxInvoiceDisplay.Included: InvoiceDisplay.SelectedIndex = 1; break;
                case TaxInvoiceDisplay.IncludedRegistered: InvoiceDisplay.SelectedIndex = 2; break;
                case TaxInvoiceDisplay.LineItem: InvoiceDisplay.SelectedIndex = 3; break;
                case TaxInvoiceDisplay.LineItemRegistered: InvoiceDisplay.SelectedIndex = 4; break;
            }
            ShowTaxColumn.Checked = _TaxProvider.ShowTaxColumn;
            TaxColumnHeader.Text = _TaxProvider.TaxColumnHeader;
        }
        ToggleConfig();
    }

    protected void Enabled_SelectedIndexChanged(object sender, EventArgs e)
    {
        ToggleConfig();
    }

    protected void ShowTaxColumn_CheckedChanged(object sender, EventArgs e)
    {
        ToggleConfig();
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        _TaxGateway.Name = _TaxProvider.Name;
        _TaxGateway.ClassId = Misc.GetClassId(typeof(MakerShopTax));
        _TaxProvider.Enabled = (Enabled.SelectedIndex == 1);
        _TaxProvider.PriceIncludesTax = false;
        switch (ShoppingDisplay.SelectedIndex)
        {
            case 0: _TaxProvider.ShoppingDisplay = TaxShoppingDisplay.Hide; break;
            case 1: _TaxProvider.ShoppingDisplay = TaxShoppingDisplay.Included; break;
            case 2: _TaxProvider.ShoppingDisplay = TaxShoppingDisplay.IncludedRegistered; break;
            case 3: _TaxProvider.ShoppingDisplay = TaxShoppingDisplay.LineItem; break;
            case 4: _TaxProvider.ShoppingDisplay = TaxShoppingDisplay.LineItemRegistered; break;
        }
        switch (InvoiceDisplay.SelectedIndex)
        {
            case 0: _TaxProvider.InvoiceDisplay = TaxInvoiceDisplay.Summary; break;
            case 1: _TaxProvider.InvoiceDisplay = TaxInvoiceDisplay.Included; break;
            case 2: _TaxProvider.InvoiceDisplay = TaxInvoiceDisplay.IncludedRegistered; break;
            case 3: _TaxProvider.InvoiceDisplay = TaxInvoiceDisplay.LineItem; break;
            case 4: _TaxProvider.InvoiceDisplay = TaxInvoiceDisplay.LineItemRegistered; break;
        }
        _TaxProvider.ShowTaxColumn = ShowTaxColumn.Checked;
        _TaxProvider.TaxColumnHeader = TaxColumnHeader.Text;
        _TaxGateway.UpdateConfigData(_TaxProvider.GetConfigData());
        _TaxGateway.Save();
        _TaxGatewayId = _TaxGateway.TaxGatewayId;
        SavedMessage.Visible = true;
        SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
    }

    private void ToggleConfig()
    {
        bool enableConfig = (Enabled.SelectedIndex == 1);
        trShowShopPriceWithTax.Visible = enableConfig;
        trInvoiceDisplay.Visible = enableConfig;
        trShowTaxColumn.Visible = enableConfig;
        trTaxColumnHeader.Visible = enableConfig && ShowTaxColumn.Checked;
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!_TaxProvider.Enabled)
        {
            int taxRuleCount = TaxRuleDataSource.CountForStore();
            TaxesDisabledPanel.Visible = (taxRuleCount > 0);
        }
    }
}
