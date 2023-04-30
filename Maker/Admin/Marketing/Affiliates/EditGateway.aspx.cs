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
using MakerShop.Marketing;

public partial class Admin_Products_EditGateway : MakerShop.Web.UI.MakerShopAdminPage
{
    private int _ProductId;
    private int _AffiliateId;
    private Product _Product;
    private Affiliate _Affiliate;
    private PaymentGatewayTemplate_Product _ProductPaymentGatewayTemplate;
    private PaymentGatewayTemplate_Affiliate _AffiliatePaymentGatewayTemplate;

    protected void Page_PreInit(object sender, EventArgs e)
    {
      
        _AffiliateId = AlwaysConvert.ToInt(Request["AffiliateId"]);

        if (_AffiliateId != 0)
            this.MasterPageFile = "~/Admin/Marketing/Affiliates/Affiliate.master";

    }


    protected void Page_Load(object sender, EventArgs e)
    {
       
        if (_AffiliateId != 0)
        {
            _Affiliate = AffiliateDataSource.Load(_AffiliateId);
            Caption.Text = string.Format(Caption.Text, _Affiliate.Name);
        }

  
        _AffiliatePaymentGatewayTemplate = PaymentGatewayTemplate_AffiliateDataSource.Load(_AffiliateId);

        if(_AffiliatePaymentGatewayTemplate != null && _AffiliatePaymentGatewayTemplate.PaymentGatewayTemplateId > 0) ShowEditForm();
       
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

        if(_AffiliatePaymentGatewayTemplate  != null)
            ddl.SelectedValue = _AffiliatePaymentGatewayTemplate.PaymentGatewayTemplateId.ToString();
    }


    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ShowNoForm();
    }


    protected void bSave_Click(object sender, EventArgs e)
    {   
        if (_AffiliatePaymentGatewayTemplate == null)
        {
            _AffiliatePaymentGatewayTemplate = new PaymentGatewayTemplate_Affiliate(_AffiliateId);
        }

        _AffiliatePaymentGatewayTemplate.PaymentGatewayTemplateId = int.Parse(ddl.SelectedValue);

        _AffiliatePaymentGatewayTemplate.Save();

        SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
        SavedMessage.Visible = true;
        BindControls();
    }
}
