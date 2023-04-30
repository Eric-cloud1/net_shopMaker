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

public partial class Admin_Products_EditDownsell : MakerShop.Web.UI.MakerShopAdminPage
{
    private int _ProductId;
    private Product _Product;
    private SubscriptionPlanDownsellsCollection _DownSells;

    protected void Page_Load(object sender, EventArgs e)
    {
        _ProductId = PageHelper.GetProductId();
        _Product = ProductDataSource.Load(_ProductId);
        Caption.Text = string.Format(Caption.Text, _Product.Name);
        _DownSells = SubscriptionPlanDownsellsDataSource.LoadByProductId(_ProductId);
        if (_DownSells != null && _DownSells.Count > 0) ShowEditForm();
    }


    protected void ShowAddForm_Click(object sender, EventArgs e)
    {
        NoPlanPanel.Visible = false;
        PlanPanel.Visible = true;
        BindControls();
    }

    private void ShowNoForm()
    {
        NoPlanPanel .Visible = true;
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
        ddltAvailable.Items.Clear();
        ddlrAvailable.Items.Clear();

        foreach (SubscriptionPlanDownsells spd in _DownSells)
        {
            if (spd.PaymentType == PaymentTypes.Trial)
            {
                ddltAvailable.Items.Add(spd.ChargeAmount.ToString());
            }
            else if (spd.PaymentType == PaymentTypes.Recurring)
            {
                ddlrAvailable.Items.Add(spd.ChargeAmount.ToString());
            }
        }
        ddltAvailable.DataBind();
        ddlrAvailable.DataBind();


    }
    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ShowNoForm();
    }

 
    protected void Page_Init(object sender, EventArgs e)
    {
       // DataBind();

    }

    protected void btRemove_Click(object sender, EventArgs e)
    {

        foreach (SubscriptionPlanDownsells spd in _DownSells)
        {
            if (spd.PaymentType != PaymentTypes.Trial)
                continue;
            if (spd.ChargeAmount == decimal.Parse(ddltAvailable.SelectedValue))
            {
                spd.Delete();
                SavedMessage.Text = string.Format("Removed ", LocaleHelper.LocalNow);
                SavedMessage.Visible = true;
                _DownSells = SubscriptionPlanDownsellsDataSource.LoadByProductId(_ProductId);
                BindControls();
                break;
            }
        }
    }
    protected void btAdd_Click(object sender, EventArgs e)
    {
        decimal d;
        if (!decimal.TryParse(tbtAdd.Text, out d))
        {
            ErrorMessageLabel.Text = "Must have a valid price > 0.00.";
            ErrorMessageLabel.Visible = true;
            return;
        }
        if (d <= 0)
        {
            ErrorMessageLabel.Text = "Must have a valid price > 0.00.";
            ErrorMessageLabel.Visible = true;
            return;
        }

        SubscriptionPlanDownsells spd = new SubscriptionPlanDownsells(_ProductId, (short)PaymentTypes.Trial, d);
        spd.Save();
        SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
        SavedMessage.Visible = true;
        _DownSells = SubscriptionPlanDownsellsDataSource.LoadByProductId(_ProductId);
        BindControls();
    }
    protected void brRemove_Click(object sender, EventArgs e)
    {

        foreach (SubscriptionPlanDownsells spd in _DownSells)
        {
            if (spd.PaymentType != PaymentTypes.Recurring)
                continue;
            if (spd.ChargeAmount == decimal.Parse(ddlrAvailable.SelectedValue))
            {
                spd.Delete();
                SavedMessage.Text = string.Format("Removed ", LocaleHelper.LocalNow);
                SavedMessage.Visible = true;
                _DownSells = SubscriptionPlanDownsellsDataSource.LoadByProductId(_ProductId);
                BindControls();
                break;
            }
        }
    }
    protected void brAdd_Click(object sender, EventArgs e)
    {
        decimal d;
        if (!decimal.TryParse(tbrAdd.Text, out d))
        {
            ErrorMessageLabel.Text = "Must have a valid price > 0.00.";
            ErrorMessageLabel.Visible = true;
            return;
        }
        if (d <= 0)
        {
            ErrorMessageLabel.Text = "Must have a valid price > 0.00.";
            ErrorMessageLabel.Visible = true;
            return;
        }

        SubscriptionPlanDownsells spd = new SubscriptionPlanDownsells(_ProductId, (short)PaymentTypes.Recurring, d);
        spd.Save();
        SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
        SavedMessage.Visible = true;
        _DownSells = SubscriptionPlanDownsellsDataSource.LoadByProductId(_ProductId);
        BindControls();
    }
}
