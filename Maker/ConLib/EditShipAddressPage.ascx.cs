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
using MakerShop.Users;
using MakerShop.Marketing;
using MakerShop.Stores;
using MakerShop.Utility;

public partial class ConLib_EditShipAddressPage : System.Web.UI.UserControl
{
    protected int ShipAddressId
    {
        get
        {
            if (ViewState["ShipAddressId"] != null) return (int)ViewState["ShipAddressId"];
            return 0;
        }
        set
        {
            ViewState["ShipAddressId"] = value;
        }
    }

    private Address _ShipAddress = null;
    protected Address ShipAddress
    {
        get
        {
            if (_ShipAddress == null)
            {
                User user = Token.Instance.User;
                if (this.ShipAddressId != 0)
                {
                    int index = user.Addresses.IndexOf(this.ShipAddressId);
                    if (index > -1) _ShipAddress = user.Addresses[index];
                }
                if (_ShipAddress == null)
                {
                    _ShipAddress = new Address();
                    _ShipAddress.UserId = user.UserId;
                    _ShipAddress.Residence = true;
                    user.Addresses.Add(_ShipAddress);
                }
            }
            return _ShipAddress;
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        this.TrackViewState();
        ShipToCountry.DataSource = CountryDataSource.LoadForStore();
        ShipToCountry.DataBind();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            this.ShipAddressId = AlwaysConvert.ToInt(Request.QueryString["AddressId"]);
            InitAddressForm();
        }
    }

    private void InitAddressForm()
    {
        Address address = this.ShipAddress;
        AddAddressCaption.Visible = (address.AddressId == 0);
        EditAddressCaption.Visible = !AddAddressCaption.Visible;
        //INITIALIZE FORM
        ShipToFirstName.Text = address.FirstName;
        ShipToLastName.Text = address.LastName;
        ShipToAddress1.Text = address.Address1;
        ShipToAddress2.Text = address.Address2;
        ShipToCity.Text = address.City;
        ShipToPostalCode.Text = address.PostalCode;
        InitCountryAndProvince();
        ShipToPhone.Text = address.Phone;
        ShipToCompany.Text = address.Company;
        ShipToFax.Text = address.Fax;
        ShipToResidence.SelectedIndex = (address.Residence ? 0 : 1);
    }

    private void InitCountryAndProvince()
    {
        //MAKE SURE THE CORRECT ADDRESS IS SELECTED
        Address address = this.ShipAddress;
        bool foundCountry = false;
        if (!string.IsNullOrEmpty(address.CountryCode))
        {
            ListItem selectedCountry = ShipToCountry.Items.FindByValue(address.CountryCode);
            if (selectedCountry != null)
            {
                ShipToCountry.SelectedIndex = ShipToCountry.Items.IndexOf(selectedCountry);
                foundCountry = true;
            }
        }
        if (!foundCountry)
        {
            Warehouse defaultWarehouse = Token.Instance.Store.DefaultWarehouse;
            ListItem selectedCountry = ShipToCountry.Items.FindByValue(defaultWarehouse.CountryCode);
            if (selectedCountry != null) ShipToCountry.SelectedIndex = ShipToCountry.Items.IndexOf(selectedCountry);
        }
        //MAKE SURE THE PROVINCE LIST IS CORRECT FOR THE COUNTRY
        UpdateCountry();
        //NOW LOOK FOR THE PROVINCE TO SET
        if (ShipToProvince.Visible) ShipToProvince.Text = address.Province;
        else
        {
            ListItem selectedProvince = ShipToProvince2.Items.FindByValue(address.Province);
            if (selectedProvince != null) ShipToProvince2.SelectedIndex = ShipToProvince2.Items.IndexOf(selectedProvince);
        }
    }

    protected void EditShipToSaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            Address address = this.ShipAddress;
            address.FirstName = StringHelper.StripHtml(ShipToFirstName.Text);
            address.LastName = StringHelper.StripHtml(ShipToLastName.Text);
            address.Address1 = StringHelper.StripHtml(ShipToAddress1.Text);
            address.Address2 = StringHelper.StripHtml(ShipToAddress2.Text);
            address.Company = StringHelper.StripHtml(ShipToCompany.Text);
            address.City = StringHelper.StripHtml(ShipToCity.Text);
            address.Province = (ShipToProvince.Visible ? StringHelper.StripHtml(ShipToProvince.Text) : ShipToProvince2.SelectedValue);
            address.PostalCode = StringHelper.StripHtml(ShipToPostalCode.Text);
            address.CountryCode = ShipToCountry.SelectedValue;
            address.Phone = StringHelper.StripHtml(ShipToPhone.Text);
            address.Fax = StringHelper.StripHtml(ShipToFax.Text);
            address.Residence = (ShipToResidence.SelectedIndex == 0);
            address.Save();
            ShowAddressBook();
        }
    }

    protected void EditShipToCancelButton_Click(object sender, EventArgs e)
    {
        ShowAddressBook();
    }

    protected void ShowAddressBook()
    {
        Response.Redirect("ShipAddress.aspx");
    }

    private void UpdateCountry()
    {
        //SEE WHETHER POSTAL CODE IS REQUIRED
        string[] countries = Store.GetCachedSettings().PostalCodeCountries.Split(",".ToCharArray());
        ShipToPostalCodeRequired.Enabled = (Array.IndexOf(countries, ShipToCountry.SelectedValue) > -1);
        //SEE WHETHER PROVINCE LIST IS DEFINED
        ProvinceCollection provinces = ProvinceDataSource.LoadForCountry(ShipToCountry.SelectedValue);
        if (provinces.Count > 0)
        {
            ShipToProvince.Visible = false;
            ShipToProvince2.Visible = true;
            ShipToProvince2.Items.Clear();
            ShipToProvince2.Items.Add(string.Empty);
            foreach (Province province in provinces)
            {
                string provinceValue = (!string.IsNullOrEmpty(province.ProvinceCode) ? province.ProvinceCode : province.Name);
                ShipToProvince2.Items.Add(new ListItem(province.Name, provinceValue));
            }
            ListItem selectedProvince = ShipToProvince2.Items.FindByValue(Request.Form[ShipToProvince2.UniqueID]);
            if (selectedProvince != null) selectedProvince.Selected = true;
            ShipToProvince2Required.Enabled = true;
        }
        else
        {
            ShipToProvince.Visible = true;
            ShipToProvince2.Visible = false;
            ShipToProvince2.Items.Clear();
            ShipToProvince2Required.Enabled = false;
        }
    }

    protected void ShipToCountry_Changed(object sender, EventArgs e)
    {
        //UPDATE THE FORM FOR THE NEW COUNTRY
        UpdateCountry();
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        this.SaveViewState();
    }

}