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
using MakerShop.Stores;
using MakerShop.Shipping;
using MakerShop.Common;
using MakerShop.Marketing;
using MakerShop.Users;
using MakerShop.Utility;

public partial class ConLib_AffiliateRegForm : System.Web.UI.UserControl
{
    Affiliate _Affiliate;
    //Boolean InEditMode = false;
    

    protected void Page_Init(object sender, EventArgs e)
    {
        Country.DataSource = CountryDataSource.LoadForStore();
        Country.DataBind();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        _Affiliate = Token.Instance.User.Affiliate;

        if (_Affiliate == null) _Affiliate = new Affiliate();        
        
        if (!Page.IsPostBack)
        {
            InitEditForm();
        }
    }

    private void InitEditForm()
    {
        if (Token.Instance.User.Affiliate != null)
        {
            //INITIALIZE FORM
            Name.Text = _Affiliate.Name;
            WebsiteUrl.Text = _Affiliate.WebsiteUrl;
            Email.Text = _Affiliate.Email;

            //INITIALIZE ADDRESS DATA
            FirstName.Text = _Affiliate.FirstName;
            LastName.Text = _Affiliate.LastName;
            Company.Text = _Affiliate.Company;
            Address1.Text = _Affiliate.Address1;
            Address2.Text = _Affiliate.Address2;
            City.Text = _Affiliate.City;
            Province.Text = _Affiliate.Province;
            
            PostalCode.Text = _Affiliate.PostalCode;
            //INITIALIZE COUNTRY
            Country.DataSource = CountryDataSource.LoadForStore("Name");
            Country.DataBind();
            if (string.IsNullOrEmpty(_Affiliate.CountryCode)) _Affiliate.CountryCode = Token.Instance.Store.DefaultWarehouse.CountryCode;
            ListItem selectedCountry = Country.Items.FindByValue(_Affiliate.CountryCode);
            if (selectedCountry != null) Country.SelectedIndex = Country.Items.IndexOf(selectedCountry);
            Phone.Text = _Affiliate.PhoneNumber;
            Fax.Text = _Affiliate.FaxNumber;
            MobileNumber.Text = _Affiliate.MobileNumber;

            InitCountryAndProvince(_Affiliate.CountryCode, _Affiliate.Province);
        }
        else
        {
            User user = Token.Instance.User;
            FirstName.Text = user.PrimaryAddress.FirstName;
            LastName.Text = user.PrimaryAddress.LastName;
            Email.Text = user.Email;

            // ADDRESS            
            Address1.Text = user.PrimaryAddress.Address1;
            Address2.Text = user.PrimaryAddress.Address2;
            City.Text = user.PrimaryAddress.City;
            Province.Text = user.PrimaryAddress.Province;
            PostalCode.Text = user.PrimaryAddress.PostalCode;
            //INITIALIZE COUNTRY
            Country.DataSource = CountryDataSource.LoadForStore("Name");
            Country.DataBind();
            if (string.IsNullOrEmpty(user.PrimaryAddress.CountryCode)) user.PrimaryAddress.CountryCode = Token.Instance.Store.DefaultWarehouse.CountryCode;
            ListItem selectedCountry = Country.Items.FindByValue(user.PrimaryAddress.CountryCode);
            if (selectedCountry != null) Country.SelectedIndex = Country.Items.IndexOf(selectedCountry);
            Phone.Text = user.PrimaryAddress.Phone;
            Fax.Text = user.PrimaryAddress.Fax;

            InitCountryAndProvince(user.PrimaryAddress.CountryCode, user.PrimaryAddress.Province);
        }

        
    }

    private void InitCountryAndProvince(String countryCode, String province)
    {
        //MAKE SURE THE CORRECT ADDRESS IS SELECTED        
        bool foundCountry = false;
        if (!string.IsNullOrEmpty(countryCode))
        {
            ListItem selectedCountry = Country.Items.FindByValue(countryCode);
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
        if (Province.Visible) Province.Text = province;
        else
        {
            ListItem selectedProvince = Province2.Items.FindByValue(province);
            if (selectedProvince != null) Province2.SelectedIndex = Province2.Items.IndexOf(selectedProvince);
        }
    }

    private void UpdateCountry()
    {
        //SEE WHETHER POSTAL CODE IS REQUIRED
        string[] countries = Store.GetCachedSettings().PostalCodeCountries.Split(",".ToCharArray());
        PostalCodeRequired.Enabled = (Array.IndexOf(countries, Country.SelectedValue) > -1);
        //SEE WHETHER PROVINCE LIST IS DEFINED
        ProvinceCollection provinces = ProvinceDataSource.LoadForCountry(Country.SelectedValue);
        if (provinces.Count > 0)
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
            ListItem selectedProvince = Province2.Items.FindByValue(Request.Form[Province2.UniqueID]);
            if (selectedProvince != null) selectedProvince.Selected = true;
            Province2Required.Enabled = true;
        }
        else
        {
            Province.Visible = true;
            Province2.Visible = false;
            Province2.Items.Clear();
            Province2Required.Enabled = false;
        }
    }


    protected void Country_Changed(object sender, EventArgs e)
    {
        //UPDATE THE FORM FOR THE NEW COUNTRY
        UpdateCountry();
    }
    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            _Affiliate.Name = Name.Text;
            _Affiliate.WebsiteUrl = WebsiteUrl.Text;
            _Affiliate.Email = Email.Text;
            
            //ADDRESS INFORMATION
            _Affiliate.FirstName = FirstName.Text;
            _Affiliate.LastName = LastName.Text;
            _Affiliate.Company = Company.Text;
            _Affiliate.Address1 = Address1.Text;
            _Affiliate.Address2 = Address2.Text;
            _Affiliate.City = City.Text;
            _Affiliate.Province = (Province.Visible ? StringHelper.StripHtml(Province.Text) : Province2.SelectedValue);
            _Affiliate.PostalCode = PostalCode.Text;
            _Affiliate.CountryCode = Country.SelectedValue;
            _Affiliate.PhoneNumber = Phone.Text;
            _Affiliate.FaxNumber = Fax.Text;
            _Affiliate.MobileNumber = MobileNumber.Text;
            _Affiliate.Save();

            if (Token.Instance.User.Affiliate == null)
            {
                Token.Instance.User.AffiliateId = _Affiliate.AffiliateId;
                Token.Instance.User.Save();
            }            
            SavedMessage.Visible = true;
        }
    }
    
}
