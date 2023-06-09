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
using MakerShop.Payments;
using System.Collections.Generic;
using MakerShop.Payments.Providers;

public partial class Admin_Payment_EditGateway : MakerShop.Web.UI.MakerShopAdminPage
{

    private int _PaymentGatewayId = 0;
    private PaymentGateway _PaymentGateway;
    private IPaymentProvider _ProviderInstance;
    private List<PaymentMethod> _PaymentMethods;

    protected Dictionary<string, string> GetConfigData()
    {
        Dictionary<string, string> configData = new Dictionary<String, String>();
        string configPrefix = phInputForm.Parent.UniqueID + "$Config_";
        foreach (string key in Request.Form)
        {
            if (key.StartsWith(configPrefix))
            {
                configData.Add(key.Remove(0, configPrefix.Length), Request.Form[key]);
            }
        }
        return configData;
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Caption.Text = string.Format(Caption.Text, _PaymentGateway.Name);
            Is3D.Checked = _PaymentGateway.Is3D;
            IsPaymentPageHosted.Checked = _PaymentGateway.IsPaymentPageHosted;
            IsPaymentPageIFrame.Checked = _PaymentGateway.IsPaymentPageIFrame;
            IsAsynchronous.Checked = _PaymentGateway.IsAsynchronous;
            BlockEmail.Checked = _PaymentGateway.BlockEmails;
            IsActive.Checked = _PaymentGateway.IsActive;

            BindPaymentMethods();
        }
    }

    protected PaymentMethod GetPaymentMethod(int paymentInstrumentId)
    {
        foreach (PaymentMethod method in _PaymentMethods)
        {
            if (method.PaymentInstrumentId == paymentInstrumentId) return method;
        }
        return null;
    }
    
    private bool IsMethodVisible(PaymentMethod method)
    {
        PaymentInstrument[] hiddenMethods = { PaymentInstrument.GiftCertificate, PaymentInstrument.GoogleCheckout, PaymentInstrument.Mail, PaymentInstrument.PhoneCall, PaymentInstrument.PurchaseOrder };
        //DO NOT SHOW HIDDEN PAYMENT INTRUMENTS
        if (Array.IndexOf(hiddenMethods, method.PaymentInstrument) > -1) return false;
        //ONLY SHOW PAYPAL FOR THAT GATEWAY
        bool isPayPalGateway = (_ProviderInstance is MakerShop.Payments.Providers.PayPal.PayPalProvider);
        if (method.PaymentInstrument == PaymentInstrument.PayPal) return isPayPalGateway;
        //MUST BE CREDIT CARD, CHECK, OR UNKNOWN
        return true;
    }

    protected void LoadPaymentMethods()
    {
        _PaymentMethods = new List<PaymentMethod>();
        PaymentMethodCollection allPaymentMethods = PaymentMethodDataSource.LoadForCriteria("pm.PaymentGatewayId=" + _PaymentGatewayId.ToString() + "  OR ( " +
            "pm.PaymentGatewayId IS NULL AND pm.Name like '%" + _PaymentGateway.Name + "%'"
            + " )" );
        foreach (PaymentMethod method in allPaymentMethods)
        {
            if (IsMethodVisible(method))
            {
                _PaymentMethods.Add(method);
            }
        }
    }
    
    protected void BindPaymentMethods()
    {
        PaymentMethodList.Visible = (_PaymentMethods.Count > 0);
        PaymentMethodList.DataSource = _PaymentMethods;
        PaymentMethodList.DataBind();
    }

    protected bool IsMethodAssigned(object dataItem)
    {
        PaymentMethod method = (PaymentMethod)dataItem;
        foreach (PaymentMethod assignedMethod in _PaymentGateway.PaymentMethods)
        {
            if ((assignedMethod.PaymentMethodId == method.PaymentMethodId)&&(assignedMethod.Active == true)) return true;
        }
        return false;
    }
    
    protected void CancelButton_Click(object sender, System.EventArgs e)
    {
        Response.Redirect("Gateways.aspx");
    }

    protected void SaveButton_Click(object sender, System.EventArgs e)
    {
        _PaymentGateway.UpdateConfigData(this.GetConfigData());
        _PaymentGateway.Is3D = Is3D.Checked;
        _PaymentGateway.IsPaymentPageHosted = IsPaymentPageHosted.Checked;
        _PaymentGateway.IsPaymentPageIFrame = IsPaymentPageIFrame.Checked;
        _PaymentGateway.IsAsynchronous = IsAsynchronous.Checked;
        _PaymentGateway.BlockEmails = BlockEmail.Checked;
        _PaymentGateway.IsActive = IsActive.Checked;
        _PaymentGateway.Save();
        if (ShowPaymentMethods(_PaymentGateway))
        {
            //UPDATE PAYMENT METHODS
            int index = 0;
            foreach (DataListItem item in PaymentMethodList.Items)
            {
                int paymentMethodId = AlwaysConvert.ToInt(PaymentMethodList.DataKeys[index]);
                PaymentMethod method = GetPaymentMethod(paymentMethodId);
                if (method != null)
                {
                    CheckBox cbMethod = (CheckBox)PageHelper.RecursiveFindControl(item, "Method");
                    if (cbMethod.Checked)
                    {
                        method.PaymentGatewayId = _PaymentGatewayId;
                        method.Active = true;

                    }
                    else if (method.PaymentGatewayId == _PaymentGatewayId)
                    {
                        method.PaymentGatewayId = _PaymentGatewayId; 
                        method.Active = false;
                    }
                    method.Save();
                }
                index++;
            }
          
        }
        Response.Redirect("Gateways.aspx");
    }

    protected void RedirectMe()
    {
        Response.Redirect("Gateways.aspx");
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        _PaymentGatewayId = AlwaysConvert.ToInt(Request.QueryString["PaymentGatewayId"]);
        _PaymentGateway = PaymentGatewayDataSource.Load(_PaymentGatewayId);
        if (_PaymentGateway == null) RedirectMe();
        _ProviderInstance = _PaymentGateway.GetInstance();
        if (_ProviderInstance == null) RedirectMe();
        //DYNAMICALLY BUILD FORM
        phInputForm.Controls.Clear();
        _ProviderInstance.BuildConfigForm(phInputForm);
        //LOAD PAYMENT METHODS
        LoadPaymentMethods();
        //TOGGLE THE PAYMENT METHODS SECTION
        if (!ShowPaymentMethods(_PaymentGateway))
            PaymentMethodPanel.Visible = false;
    }

    private bool ShowPaymentMethods(PaymentGateway gateway)
    {
        string gcerClassId = Misc.GetClassId(typeof(MakerShop.Payments.Providers.GiftCertificatePaymentProvider));
        string gchkClassId = Misc.GetClassId(typeof(MakerShop.Payments.Providers.GoogleCheckout.GoogleCheckout));
        bool unassignable = gcerClassId.Equals(gateway.ClassId) || gchkClassId.Equals(gateway.ClassId);
        return !unassignable;
    }

}
