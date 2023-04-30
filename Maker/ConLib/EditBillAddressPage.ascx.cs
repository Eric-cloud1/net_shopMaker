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
using MakerShop.Stores;
using MakerShop.Users;
using MakerShop.Utility;

public partial class ConLib_EditBillAddressPage : System.Web.UI.UserControl
{
    protected void Page_Init(object sender, EventArgs e)
    {
        if (Token.Instance.User.Basket.Items.Count == 0)
            Response.Redirect(NavigationHelper.GetBasketUrl());
        Country.DataSource = CountryDataSource.LoadForStore();
        Country.DataBind();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            InitAddressForm();
        }
    }

    protected void InitAddressForm()
    {
        Address address = Token.Instance.User.PrimaryAddress;
        FirstName.Text = address.FirstName;
        LastName.Text = address.LastName;
        Address1.Text = address.Address1;
        Address2.Text = address.Address2;
        City.Text = address.City;
        PostalCode.Text = address.PostalCode;
        InitCountryAndProvince();
        Phone.Text = address.Phone;
        Company.Text = address.Company;
        Fax.Text = address.Fax;
        Residence.SelectedIndex = (address.Residence ? 0 : 1);
    }

    private void InitCountryAndProvince()
    {
        //MAKE SURE THE CORRECT ADDRESS IS SELECTED
        Address address = Token.Instance.User.PrimaryAddress;
        bool foundCountry = false;
        if (!string.IsNullOrEmpty(address.CountryCode))
        {
            ListItem selectedCountry = Country.Items.FindByValue(address.CountryCode);
            if (selectedCountry != null)
            {
                Country.SelectedIndex = Country.Items.IndexOf(selectedCountry);
                foundCountry = true;
            }
        }
        if (!foundCountry)
        {
            Warehouse defaultWarehouse = Token.Instance.Store.DefaultWarehouse;
            ListItem selectedCountry = Country.Items.FindByValue(defaultWarehouse.CountryCode);
            if (selectedCountry != null) Country.SelectedIndex = Country.Items.IndexOf(selectedCountry);
        }
        //MAKE SURE THE PROVINCE LIST IS CORRECT FOR THE COUNTRY
        UpdateCountry();
        //NOW LOOK FOR THE PROVINCE TO SET
        if (Province.Visible) Province.Text = address.Province;
        else
        {
            ListItem selectedProvince = Province2.Items.FindByValue(address.Province);
            if (selectedProvince != null) Province2.SelectedIndex = Province2.Items.IndexOf(selectedProvince);
        }
    }

    protected void ContinueButton_Click(object sender, EventArgs e)
    {
        string provinceName;
        if (ValidateProvince(out provinceName))
        {
            if (Page.IsValid)
            {
                Address address = Token.Instance.User.PrimaryAddress;
                address.FirstName = StringHelper.StripHtml(FirstName.Text);
                address.LastName = StringHelper.StripHtml(LastName.Text);
                address.Address1 = StringHelper.StripHtml(Address1.Text);
                address.Address2 = StringHelper.StripHtml(Address2.Text);
                address.Company = StringHelper.StripHtml(Company.Text);
                address.City = StringHelper.StripHtml(City.Text);
                address.Province = provinceName;
                address.PostalCode = StringHelper.StripHtml(PostalCode.Text);
                address.CountryCode = Country.SelectedValue;
                address.Phone = StringHelper.StripHtml(Phone.Text);
                address.Fax = StringHelper.StripHtml(Fax.Text);
                address.Residence = (Residence.SelectedIndex == 0);
                address.Save();
                // REDIRECT BASED ON PRESENCE OF SHIPPABLE ITEMS
                if (Token.Instance.User.Basket.Items.HasShippableProducts)
                    Response.Redirect("ShipAddress.aspx");
                else Response.Redirect("Payment.aspx");
            }
        }
        else
        {
            Province2Invalid.IsValid = false;
            UpdateCountry();
        }
    }

    /// <summary>
    /// Validates the current province value
    /// </summary>
    /// <returns></returns>
    private bool ValidateProvince(out string provinceName)
    {
        provinceName = (Province.Visible ? StringHelper.StripHtml(Province.Text) : Province2.SelectedValue);
        string countryCode = Country.SelectedValue;
        if (ProvinceDataSource.CountForCountry(countryCode) == 0) return true;
        //CHECK THE VALUE
        int provinceId = ProvinceDataSource.GetProvinceIdByName(countryCode, provinceName);
        if (provinceId > 0)
        {
            //UPDATE VALUE
            Province p = ProvinceDataSource.Load(provinceId);
            if (p.ProvinceCode.Length > 0) provinceName = p.ProvinceCode;
            else provinceName = p.Name;
        }
        return (provinceId > 0);
    }

    private void UpdateCountry()
    {
        //SEE WHETHER POSTAL CODE IS REQUIRED
        string[] countries = Store.GetCachedSettings().PostalCodeCountries.Split(",".ToCharArray());
        PostalCodeRequired.Enabled = (Array.IndexOf(countries, Country.SelectedValue) > -1);
        //SEE WHETHER PROVINCE LIST IS DEFINED
        ProvinceCollection provinces = ProvinceDataSource.LoadForCountry(Country.SelectedValue);
        if (Page.IsPostBack && provinces.Count > 0)
        {
            Province.Visible = false;
            Province2.Visible = true;
            Province2.Items.Clear();
            Province2.Items.Add(string.Empty);
            foreach (Province province in provinces)
            {
                string provinceValue = (!string.IsNullOrEmpty(province.ProvinceCode) ? province.ProvinceCode : province.Name);
                Province2.Items.Add(new ListItem(province.Name, provinceValue));
            }
            ListItem selectedProvince = FindSelectedProvince();
            if (selectedProvince != null) selectedProvince.Selected = true;
            Province2Required.Enabled = true;
            Province.Text = string.Empty;
        }
        else
        {
            string defaultValue = Province.Text;
            if (string.IsNullOrEmpty(defaultValue)) defaultValue = Request.Form[Province2.UniqueID];
            Province.Text = defaultValue;
            Province.Visible = true;
            Province2.Visible = false;
            Province2.Items.Clear();
            Province2Required.Enabled = false;
        }
    }

    /// <summary>
    /// Obtains the province that should default to selected in the drop down list
    /// </summary>
    /// <returns>The province that should default to selected in the drop down list</returns>
    private ListItem FindSelectedProvince()
    {
        string defaultValue = Province.Text;
        if (string.IsNullOrEmpty(defaultValue)) defaultValue = Request.Form[Province2.UniqueID];
        if (string.IsNullOrEmpty(defaultValue)) return null;
        defaultValue = defaultValue.ToUpperInvariant();
        foreach (ListItem item in Province2.Items)
        {
            string itemText = item.Text.ToUpperInvariant();
            string itemValue = item.Value.ToUpperInvariant();
            if (itemText == defaultValue || itemValue == defaultValue) return item;
        }
        return null;
    }

    protected void Country_Changed(object sender, EventArgs e)
    {
        //UPDATE THE FORM FOR THE NEW COUNTRY
        UpdateCountry();
    }
}