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
using MakerShop.Taxes;
using MakerShop.Shipping;
using MakerShop.Payments;

public partial class Admin_Products_EditGateway : MakerShop.Web.UI.MakerShopAdminPage
{
    private int _ProductId;
    private Product _Product;
    private PaymentGatewayTemplate_Product _ProductPaymentGatewayTemplate;

    protected void Page_Load(object sender, EventArgs e)
    {
        _ProductId = PageHelper.GetProductId();
        _Product = ProductDataSource.Load(_ProductId);
        Caption.Text = string.Format(Caption.Text, _Product.Name);
        _ProductPaymentGatewayTemplate = PaymentGatewayTemplate_ProductDataSource.Load(_ProductId);
        if (_ProductPaymentGatewayTemplate != null && _ProductPaymentGatewayTemplate.PaymentGatewayTemplateId > 0) ShowEditForm();
    }


    protected void ShowAddForm_Click(object sender, EventArgs e)
    {
        NoPlanPanel.Visible = false;
        PlanPanel.Visible = true;
        BindControls();
    }

    private void ShowNoForm()
    {
        NoPlanPanel.Visible = true;
        PlanPanel.Visible = false;
    }

    private void ShowEditForm()
    {
        NoPlanPanel.Visible = false;
        PlanPanel.Visible = true;

        if (!Page.IsPostBack)
        {

            //INITIALIZE FORM
            BindControls();

        }
    }
    private void BindControls()
    {
        ddl.DataSource = PaymentGatewayTemplateDataSource.Load();
        ddl.DataTextField = "Name";
        ddl.DataValueField = "PaymentGatewayTemplateId";
        ddl.DataBind();
        if (_ProductPaymentGatewayTemplate != null)
            ddl.SelectedValue = _ProductPaymentGatewayTemplate.PaymentGatewayTemplateId.ToString();
    }
    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ShowNoForm();
    }

 

    protected void bSave_Click(object sender, EventArgs e)
    {
        if (_ProductPaymentGatewayTemplate == null)
        {
            _ProductPaymentGatewayTemplate = new PaymentGatewayTemplate_Product(_ProductId);
        }
        _ProductPaymentGatewayTemplate.PaymentGatewayTemplateId = int.Parse(ddl.SelectedValue);


        _ProductPaymentGatewayTemplate.Save();
        SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
        SavedMessage.Visible = true;
        BindControls();
    }
}
