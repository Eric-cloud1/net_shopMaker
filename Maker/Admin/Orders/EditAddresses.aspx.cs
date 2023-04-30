using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using MakerShop.Catalog;
using MakerShop.Data;
using MakerShop.DigitalDelivery;
using MakerShop.Marketing;
using MakerShop.Messaging;
using MakerShop.Orders;
using MakerShop.Payments;
using MakerShop.Payments.Providers;
using MakerShop.Products;
using MakerShop.Reporting;
using MakerShop.Shipping;
using MakerShop.Stores;
using MakerShop.Taxes;
using MakerShop.Taxes.Providers;
using MakerShop.Users;
using MakerShop.Utility;
using VPlex.BizRules;
using VPlex.Common;

public partial class Admin_Orders_EditAddresses : MakerShop.Web.UI.MakerShopAdminPage
{

    private int _OrderId;
    private Order _Order;

    protected void Page_Load(object sender, EventArgs e)
    {
        _OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
        _Order = OrderDataSource.Load(_OrderId);
        if (!Page.IsPostBack)
        {
            BindBilling();
            BindShipping();
        }

        this.ErrorMessage.Visible = false;
    }

    protected void BindBilling()
    {
        //INITIALIZE VALUES
        Store store = StoreDataSource.Load();

        BillToFirstName.Text = _Order.BillToFirstName;
        BillToLastName.Text = _Order.BillToLastName;
        BillToCompany.Text = _Order.BillToCompany;
        BillToAddress1.Text = _Order.BillToAddress1;
        BillToAddress2.Text = _Order.BillToAddress2;
        BillToCity.Text = _Order.BillToCity;
        BillToProvince.Text = _Order.BillToProvince;
        BillToPostalCode.Text = _Order.BillToPostalCode;
        BillToCountryCode.DataSource = store.Countries;
        BillToCountryCode.DataBind();
        ListItem selectedCountry = BillToCountryCode.Items.FindByValue(_Order.BillToCountryCode.ToString());
        if (selectedCountry != null) selectedCountry.Selected = true;
        BillToPhone.Text = _Order.BillToPhone;
    }

    protected void BindShipping()
    {
        ShipmentRepeater.DataSource = _Order.Shipments;
        ShipmentRepeater.DataBind();
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {

        Customer customer = new Customer();
        customer.FirstName = BillToFirstName.Text;
        customer.LastName = BillToLastName.Text;
        customer.Address = string.Format("{0} {1}", BillToAddress1.Text, BillToAddress2.Text);
        customer.City = BillToCity.Text;
        customer.PostalCode = BillToPostalCode.Text;
        customer.Region = BillToProvince.Text;
        customer.Country = BillToCountryCode.Items[BillToCountryCode.SelectedIndex].Value;
        customer.Phone = BillToPhone.Text;
        customer.BillingName = BillToCompany.Text;
     
       
        VPlexResponse Error = new VPlexResponse();
        Validation.ValidateCustomer(customer, ref Error);

        _Order.BillToFirstName = customer.FirstName;
        _Order.BillToLastName = customer.LastName;
        _Order.BillToCompany = customer.BillingName;
        _Order.BillToAddress1 = customer.Address;
        _Order.BillToCity = customer.City;
        _Order.BillToProvince = customer.Region;
        _Order.BillToPostalCode = customer.PostalCode;
        _Order.BillToCountryCode = customer.Country;
        _Order.BillToPhone = customer.Phone;

        int index = 0;
        Customer customerShipping;

        VPlexResponse ErrorShipping = new VPlexResponse();
        foreach (OrderShipment shipment in _Order.Shipments)
        {
            customerShipping = new Customer();

            RepeaterItem item = ShipmentRepeater.Items[index];

            customerShipping.FirstName = GetControlValue(item, "ShipToFirstName");
            customerShipping.LastName = GetControlValue(item, "ShipToLastName");
            customerShipping.Address = string.Format("{0} {1}", GetControlValue(item, "ShipToAddress1"), GetControlValue(item, "ShipToAddress2"));
            customerShipping.City = GetControlValue(item, "ShipToCity");
            customerShipping.PostalCode = GetControlValue(item, "ShipToPostalCode");
            customerShipping.Region = GetControlValue(item, "ShipToProvince");
            customerShipping.Country = GetControlValue(item, "ShipToCountryCode");
            customerShipping.Phone = GetControlValue(item, "ShipToPhone");
            customerShipping.BillingName = GetControlValue(item, "ShipToCompany");


            Validation.ValidateCustomer(customerShipping, ref ErrorShipping);

            shipment.ShipToFirstName = customerShipping.FirstName;
            shipment.ShipToLastName = customerShipping.LastName;
            shipment.ShipToCompany = customerShipping.BillingName;
            shipment.ShipToAddress1 = customerShipping.Address;
            shipment.ShipToCity = customerShipping.City;
            shipment.ShipToProvince = customerShipping.Region;
            shipment.ShipToPostalCode = customerShipping.PostalCode;
            shipment.ShipToCountryCode = customerShipping.Country;
            shipment.ShipToPhone = customerShipping.Phone;
        }

       

        this.ErrorMessage.Text = string.Format("<font color=red>Billing:<br/><ul>");

        foreach (string ms in Error.Messages)
            this.ErrorMessage.Text += string.Format("<li>{0}", ms);

        this.ErrorMessage.Text += string.Format("</ul>");

        this.ErrorMessage.Text += string.Format("Shipping:<br/><ul>");

        foreach (string ms in ErrorShipping.Messages)
            this.ErrorMessage.Text += string.Format("<li>{0}", ms);

        this.ErrorMessage.Text += string.Format("</ul></font>");


        if (Error.Successful == false)
        {
            this.ErrorMessage.Visible = true;
            return;
        }


        _Order.Save();
        SavedMessage.Text = string.Format(SavedMessage.Text, DateTime.UtcNow.ToLocalTime());
        SavedMessage.Visible = true;
        EditAddressAjax.Update();
    }

    protected void SetControlValue(Control parent, string controlName, string controlValue)
    {
        Control control = PageHelper.RecursiveFindControl(parent, controlName);
        if (control != null)
        {
            TextBox tb = control as TextBox;
            if (tb != null) tb.Text = controlValue;
        }
    }

    protected string GetControlValue(Control parent, string controlName)
    {
        Control control = PageHelper.RecursiveFindControl(parent, controlName);
        if (control != null)
        {
            TextBox tb = control as TextBox;
            if (tb != null) return tb.Text;
            DropDownList ddl = control as DropDownList;
            if (ddl != null) return ddl.Items[ddl.SelectedIndex].Value;
        }
        return string.Empty;
    }

    protected void ShipmentRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        OrderShipment shipment = (OrderShipment)e.Item.DataItem;
        SetControlValue(e.Item, "ShipToFirstName", shipment.ShipToFirstName);
        SetControlValue(e.Item, "ShipToLastName", shipment.ShipToLastName);
        SetControlValue(e.Item, "ShipToCompany", shipment.ShipToCompany);
        SetControlValue(e.Item, "ShipToAddress1", shipment.ShipToAddress1);
        SetControlValue(e.Item, "ShipToAddress2", shipment.ShipToAddress2);
        SetControlValue(e.Item, "ShipToCity", shipment.ShipToCity);
        SetControlValue(e.Item, "ShipToProvince", shipment.ShipToProvince);
        SetControlValue(e.Item, "ShipToPostalCode", shipment.ShipToPostalCode);
        DropDownList shipToCountry = (DropDownList)PageHelper.RecursiveFindControl(e.Item, "ShipToCountryCode");
        shipToCountry.DataSource = StoreDataSource.Load().Countries;
        shipToCountry.DataBind();
        ListItem selectedCountry = shipToCountry.Items.FindByValue(shipment.ShipToCountryCode.ToString());
        if (selectedCountry != null) selectedCountry.Selected = true;
        SetControlValue(e.Item, "ShipToPhone", shipment.ShipToPhone);
    }

}
