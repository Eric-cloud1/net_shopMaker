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
using System.Collections.Generic;
using MakerShop.Payments;

public partial class Admin_Products_EditSubscription : MakerShop.Web.UI.MakerShopAdminPage
{
    private int _ProductId;
    private Product _Product;
    private SubscriptionPlanDetailsCollection _subscriptionPlanDetails;
    private SubscriptionPlanDetails Initial
    {
        get
        {
            SubscriptionPlanDetails sbd = _subscriptionPlanDetails.Initial;
            if (sbd == null)
            {
                sbd = SubscriptionPlanDetails.CreateBlankPlan_Initial(_ProductId);
                _subscriptionPlanDetails.Add(sbd);
            }
            return sbd;
        }
    }
    private SubscriptionPlanDetails Trial
    {
        get
        {
            SubscriptionPlanDetails sbd = _subscriptionPlanDetails.Trial;
            if (sbd == null)
            {
                sbd = SubscriptionPlanDetails.CreateBlankPlan_Trial(_ProductId);
                _subscriptionPlanDetails.Add(sbd);
            }
            return sbd;
        }
    }
    private SubscriptionPlanDetails Recurring
    {
        get
        {
            SubscriptionPlanDetails sbd = _subscriptionPlanDetails.Recurring;
            if (sbd == null)
            {
                sbd = SubscriptionPlanDetails.CreateBlankPlan_Recurring(_ProductId);
                _subscriptionPlanDetails.Add(sbd);
            }
            return sbd;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        _ProductId = PageHelper.GetProductId();
        _Product = ProductDataSource.Load(_ProductId);
        Caption.Text = string.Format(Caption.Text, _Product.Name);
        _subscriptionPlanDetails = SubscriptionPlanDetailsDataSource.LoadByProductId(_ProductId);
        if (_subscriptionPlanDetails != null && _subscriptionPlanDetails.Count > 0) ShowEditForm();
    }


    protected void ShowAddForm_Click(object sender, EventArgs e)
    {
        NoSubscriptionPlanPanel.Visible = false;
        SubscriptionPlanForm.Visible = true;
        AddSubscriptionPlanButtons.Visible = true;
        EditSubscriptionPlanButtons.Visible = false;
        _subscriptionPlanDetails = SubscriptionPlanDetails.CreateBlankCollection(_ProductId,
        _Product.Price.ToDecimal(System.Threading.Thread.CurrentThread.CurrentUICulture));
        BindControls();
    }

    private void ShowNoForm()
    {
        NoSubscriptionPlanPanel.Visible = true;
        SubscriptionPlanForm.Visible = false;
        AddSubscriptionPlanButtons.Visible = false;
        EditSubscriptionPlanButtons.Visible = false;
    }

    private void ShowEditForm()
    {
        NoSubscriptionPlanPanel.Visible = false;
        SubscriptionPlanForm.Visible = true;
        AddSubscriptionPlanButtons.Visible = false;
        EditSubscriptionPlanButtons.Visible = true;
        if (!Page.IsPostBack)
        {
            //INITIALIZE FORM
            BindControls();
        }
    }

    private void BindControls()
    {
        tbiAmount.Text = string.Format(tbiAmount.Text, Initial.PaymentAmount);
        if (Initial.ShipMethodId != 0)
            ddliShipMethods.SelectedValue = Initial.ShipMethodId.ToString();
        cbiSplit.Checked = Initial.SplitShip;
        tbiPaymentDays.Text = Initial.PaymentDays.ToString();
        tbiNOP.Text = Initial.NumberOfPayments.ToString();
        tbiDaysToCapture.Text = Initial.DaysToCapture.ToString();

        tbtAmount.Text = string.Format(tbtAmount.Text, Trial.PaymentAmount);
        if (Trial.ShipMethodId != 0)
            ddltShipMethods.SelectedValue = Trial.ShipMethodId.ToString();
        cbtSplit.Checked = Trial.SplitShip;
        tbtPaymentDays.Text = Trial.PaymentDays.ToString();
        tbtNOP.Text = Trial.NumberOfPayments.ToString();
        tbtDaysToCapture.Text = Trial.DaysToCapture.ToString();

        tbrAmount.Text = string.Format(tbrAmount.Text, Recurring.PaymentAmount);
        if (Recurring.ShipMethodId != 0)
            ddlrShipMethods.SelectedValue = Recurring.ShipMethodId.ToString();
        cbrSplit.Checked = Recurring.SplitShip;
        tbrPaymentDays.Text = Recurring.PaymentDays.ToString();
        tbrNOP.Text = Recurring.NumberOfPayments.ToString();
        tbrDaysToCapture.Text = Recurring.DaysToCapture.ToString();
    }
    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ShowNoForm();
    }

    private void Save()
    {
        
        for (int i = 0; i < _subscriptionPlanDetails.Count; ++i)
        {
            SubscriptionPlanDetails spd = _subscriptionPlanDetails[i];

            if ((spd.PaymentAmount == 0) && (spd.ShipMethodId == 0))
            {
                spd.Delete();
                _subscriptionPlanDetails.Remove(spd);
                --i;
            }
            else if (spd.ShipMethodId == 0)
            {
                ErrorMessageLabel.Text = "Must have a Ship Method if price is > 0.00.";
                ErrorMessageLabel.Visible = true;
                return;
            }
        }
        if ((_subscriptionPlanDetails.Trial != null) && (_subscriptionPlanDetails.Recurring!= null))
            if (_subscriptionPlanDetails.Trial.PaymentDays >= _subscriptionPlanDetails.Recurring.PaymentDays)
            {
                ErrorMessageLabel.Text = "Recurring Days must be after Trial days.";
                ErrorMessageLabel.Visible = true;

            }
        _subscriptionPlanDetails.Save();
        SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
        SavedMessage.Visible = true;
    }

    protected void AddButton_Click(object sender, EventArgs e)
    {
        _subscriptionPlanDetails = SubscriptionPlanDetails.CreateBlankCollection(_ProductId,
        _Product.Price.ToDecimal(System.Threading.Thread.CurrentThread.CurrentUICulture));
        BindData();
        this.Save();
        ShowEditForm();
    }

    protected void UpdateButton_Click(object sender, EventArgs e)
    {
        _subscriptionPlanDetails = SubscriptionPlanDetailsDataSource.LoadByProductId(_ProductId);
        BindData();
        if (_subscriptionPlanDetails != null)
            this.Save();
        else ShowNoForm();
    }


    private void BindData()
    {
        Initial.PaymentAmount = AlwaysConvert.ToDecimal(tbiAmount.Text);
        Initial.ShipMethodId = AlwaysConvert.ToInt(ddliShipMethods.SelectedValue);
        Initial.SplitShip = cbiSplit.Checked;
        Initial.PaymentDays = AlwaysConvert.ToInt16(tbiPaymentDays.Text);
        Initial.NumberOfPayments = AlwaysConvert.ToInt16(tbiNOP.Text);
        Initial.DaysToCapture = AlwaysConvert.ToInt16(tbiDaysToCapture.Text);

        Trial.PaymentAmount = AlwaysConvert.ToDecimal(tbtAmount.Text);
        Trial.ShipMethodId = AlwaysConvert.ToInt(ddltShipMethods.SelectedValue);
        Trial.SplitShip = cbtSplit.Checked;
        Trial.PaymentDays = AlwaysConvert.ToInt16(tbtPaymentDays.Text);
        Trial.NumberOfPayments = AlwaysConvert.ToInt16(tbtNOP.Text);
        Trial.DaysToCapture = AlwaysConvert.ToInt16(tbtDaysToCapture.Text);

        Recurring.PaymentAmount = AlwaysConvert.ToDecimal(tbrAmount.Text);
        Recurring.ShipMethodId = AlwaysConvert.ToInt(ddlrShipMethods.SelectedValue);
        Recurring.SplitShip = cbrSplit.Checked;
        Recurring.PaymentDays = AlwaysConvert.ToInt16(tbrPaymentDays.Text);
        Recurring.NumberOfPayments = AlwaysConvert.ToInt16(tbrNOP.Text);
        Recurring.DaysToCapture = AlwaysConvert.ToInt16(tbrDaysToCapture.Text);
    }
    protected void DeleteButton_Click(object sender, EventArgs e)
    {
        if (_subscriptionPlanDetails != null)
        {
            if (_subscriptionPlanDetails.Count > 0)
            {
                foreach (SubscriptionPlanDetails spd in _subscriptionPlanDetails)
                {
                    spd.Delete();
                }
            }
            _subscriptionPlanDetails = null;
            ShowNoForm();
            //     ErrorMessageLabel.Text = "Subscription plan can not be deleted because it is currently active.";
            //     ErrorMessageLabel.Visible = true;
        }
    }



    protected void Page_Init(object sender, EventArgs e)
    {
        //WE DO THIS EACH TIME SO THAT LIST ITEMS DO NOT HAVE TO RESIDE IN VIEWSTATE
        ShipMethodCollection smc = ShipMethodDataSource.LoadForStore();

        ddliShipMethods.DataSource = smc;
        ddliShipMethods.DataBind();
        ddltShipMethods.DataSource = smc;
        ddltShipMethods.DataBind();
        ddlrShipMethods.DataSource = smc;
        ddlrShipMethods.DataBind();
       // DataBind();
       


    }



}
