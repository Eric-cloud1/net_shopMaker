using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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

public partial class ConLib_AddressBook : System.Web.UI.UserControl
{
    private User _User;
    private List<Address> _SortedAddresses;
    //MODE CAN BE V = VIEW, A = ADD, E = EDIT
    private string _PageMode = "V";
    //WE ONLY WANT TO SHOW PROVINCE LIST WHEN NECESSARY
    private bool _ShowProvinceList = false;
    //WE NEED TO KEEP TRACK OF THE COUNTRY CODE LAST SELECTED BY CLIENT
    private string _SelectedCountryCode = string.Empty;
    //KEEP TRACK OF ADDRESS BEING EDITED (IF ANY)
    private int _EditAddressId = 0;

    protected void Page_Init(object sender, EventArgs e)
    {
        InitAddressBook();
        Country.DataSource = CountryDataSource.LoadForStore("Name");
        Country.DataBind();
        //LOAD VIEW STATE
        LoadCustomViewState();
    }

    private void InitAddressBook()
    {
        _User = Token.Instance.User;
        _SortedAddresses = new List<Address>();
        foreach (Address a in _User.Addresses) _SortedAddresses.Add(a);
        _SortedAddresses.Sort(new AddressComparer(_User.PrimaryAddressId));
        A.DataSource = _SortedAddresses;
        A.DataBind();
    }

    protected void InitAddressForm(Address address)
    {
        string newCountryCode;
        if (address == null)
        {
            FirstName.Text = string.Empty;
            LastName.Text = string.Empty;
            Company.Text = string.Empty;
            Address1.Text = string.Empty;
            Address2.Text = string.Empty;
            City.Text = string.Empty;
            Province.Text = string.Empty;
            PostalCode.Text = string.Empty;
            Phone.Text = string.Empty;
            Fax.Text = string.Empty;
            IsBusiness.Checked = false;
            newCountryCode = GetDefaultCountryCode();
        }
        else
        {
            FirstName.Text = address.FirstName;
            LastName.Text = address.LastName;
            Company.Text = address.Company;
            Address1.Text = address.Address1;
            Address2.Text = address.Address2;
            City.Text = address.City;
            Province.Text = address.Province;
            PostalCode.Text = address.PostalCode;
            Phone.Text = address.Phone;
            Fax.Text = address.Fax;
            IsBusiness.Checked = !address.Residence;
            if (string.IsNullOrEmpty(address.CountryCode)) 
                address.CountryCode = Token.Instance.Store.DefaultWarehouse.CountryCode;
            newCountryCode = address.CountryCode;
        }
        ListItem defaultCountry = Country.Items.FindByValue(newCountryCode);
        if (defaultCountry != null) Country.SelectedIndex = Country.Items.IndexOf(defaultCountry);
        CountryChanged(null, null);
        /*
        CountryCollection allCountries = CountryDataSource.LoadForStore("Name");
        StringBuilder sb = new StringBuilder();
        sb.Append("var allCountries = \"");
        string delimiter = string.Empty;
        foreach (Country c in allCountries)
        {
            sb.Append(delimiter + c.CountryCode + "~" + c.Name);
            delimiter = "|";
        }
        sb.Append("\";\r\n");
        sb.Append("var allProvinces = \"");
        ProvinceCollection provinces = ProvinceDataSource.LoadForCriteria("CountryCode IS NOT NULL", "CountryCode,Name");
        delimiter = string.Empty;
        foreach (Province p in provinces)
        {
            sb.Append(delimiter + p.CountryCode + "~" + p.Name);
            delimiter = "|";
        }
        sb.Append("\";\r\n");
        sb.Append("var countryList = document.getElementById('" + Country.ClientID + "');\r\n");
        sb.Append("var provinceList = document.getElementById('" + Province2.ClientID + "');\r\n");
        sb.Append("var provinceText= document.getElementById('" + Province.ClientID + "');\r\n");
        sb.Append("var postalCodeRequired = document.getElementById('" + PostalCodeRequired.ClientID + "');\r\n");
        sb.Append("var validationSummary = document.getElementById('" + AddValidationSummary.ClientID + "');\r\n");
        //sb.Append("function initCountry(d){countryList.length=0;var a=allCountries.split('|');for(var i=0;i<a.length;i++){t=a[i].split('~');countryList.options[i]=new Option(t[1],t[0]);if(d==t[0])countryList.selectedIndex=i}}\r\n");
        sb.Append(@"function initProvince(d){var c=countryList.options[countryList.selectedIndex].value;ValidatorEnable(postalCodeRequired,c==""US""||c==""CA"");provinceList.options.length=0;var a=allProvinces.split(""|"");var i=0;for(var j=0;j<a.length;j++){t=a[j].split(""~"");if(t[0]==c){provinceList.options[i]=new Option(t[1]);if(t[1]==d)provinceList.selectedIndex=i;i++;}if(provinceList.options.length>0){provinceList.style.display='';provinceText.style.display='none';}else{provinceList.style.display='none';provinceText.style.display='';}}if(validationSummary.style.display!='none'){Page_ClientValidate()}else{resetValidators()}}");
        //sb.Append("function resetValidators(){for(i=0;i<Page_Validators.length;i++){Page_Validators[i].isvalid=true;ValidatorUpdateDisplay(Page_Validators[i]);}validationSummary.style.display='none'}\r\n");
        //sb.Append("function resetAddForm(){resetValidators();initCountry('" + GetDefaultCountryCode() + "');initProvince(null)}\r\n");
        //sb.Append("//resetAddForm();\r\n");
        sb.Append("var countryChangedTimer = null;\r\n");
        sb.Append("function delayonchange(code){window.clearTimeout(countryChangedTimer);countryChangedTimer=window.setTimeout(code, 500);}\r\n");
        Page.ClientScript.RegisterStartupScript(typeof(Page), "countryData", sb.ToString(), true);
        */
    }

    private string GetDefaultCountryCode()
    {
        string defaultCountry = (_SortedAddresses.Count > 0) ? _SortedAddresses[0].CountryCode : string.Empty;
        if (!string.IsNullOrEmpty(defaultCountry)) return defaultCountry;
        return Token.Instance.Store.DefaultWarehouse.CountryCode;
    }

    protected void A_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "E")
        {
            int addressId = AlwaysConvert.ToInt(e.CommandArgument);
            int index = _User.Addresses.IndexOf(addressId);
            if (index > -1)
            {
                _EditAddressId = addressId;
                InitAddressForm(_User.Addresses[index]);
                _PageMode = "E";
            }
        }
        else if (e.CommandName == "D")
        {
            int addressId = AlwaysConvert.ToInt(e.CommandArgument);
            if (addressId != _User.PrimaryAddressId)
            {
                int index = _User.Addresses.IndexOf(addressId);
                if (index > 0) _User.Addresses.DeleteAt(index);
                //REBUILD ADDRESS BOOK
                InitAddressBook();
            }
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        Address primaryAddress = Token.Instance.User.PrimaryAddress;
        if (!IsAddressValid(primaryAddress))
        {
            _EditAddressId = primaryAddress.AddressId;
            _PageMode = "E";
            InitAddressForm(primaryAddress);
            CancelButton.Visible = false;
            EditBillingCaption.Visible = true;
            EditCaption.Visible = false;
            AddCaption.Visible = false;
        }
        else CancelButton.Visible = true;
        //REBIND THE ADDRESS BOOK
        if (_PageMode != "V")
        {
            if (!EditBillingCaption.Visible)
            {
                AddCaption.Visible = (_PageMode == "A");
                EditCaption.Visible = !AddCaption.Visible;
            }
            if (Request.Form["__EVENTTARGET"] != Country.UniqueID)
            {
                ListItem defaultCountry = Country.Items.FindByValue(_SelectedCountryCode);
                if (defaultCountry != null) Country.SelectedIndex = Country.Items.IndexOf(defaultCountry);
                CountryChanged(null, null);
            }
            FormPanel.Visible = true;
            AddPanelPopup.Show();
        }
        else
        {
            FormPanel.Visible = false;
            AddPanelPopup.Hide();
            _ShowProvinceList = false;
        }
        SaveCustomViewState();
    }

    private void LoadCustomViewState()
    {
        if (Page.IsPostBack)
        {
            UrlEncodedDictionary customViewState = new UrlEncodedDictionary(EncryptionHelper.DecryptAES(Request.Form[VS.UniqueID]));
            _PageMode = customViewState.TryGetValue("M");
            //VIEW IS DEFAULT MODE
            if (_PageMode != "V" && _PageMode != "A" && _PageMode != "E") _PageMode = "V";
            _ShowProvinceList = !string.IsNullOrEmpty(customViewState.TryGetValue("P"));
            _SelectedCountryCode = customViewState.TryGetValue("C");
            if (string.IsNullOrEmpty(_SelectedCountryCode)) _SelectedCountryCode = GetDefaultCountryCode();
            _EditAddressId = AlwaysConvert.ToInt(customViewState.TryGetValue("A"));
        }
    }

    private void SaveCustomViewState()
    {
        UrlEncodedDictionary customViewState = new UrlEncodedDictionary();
        customViewState.Add("M", _PageMode);
        customViewState.Add("P", _ShowProvinceList ? "1" : string.Empty);
        customViewState.Add("C", _SelectedCountryCode);
        customViewState.Add("A", _EditAddressId.ToString());
        VS.Value = EncryptionHelper.EncryptAES(customViewState.ToString());
    }

    protected void AddButton_Click(object sender, EventArgs e)
    {
        _EditAddressId = 0;
        _PageMode = "A";
        InitAddressForm(null);
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        _EditAddressId = 0;
        _PageMode = "V";
    }

    protected void CountryChanged(object sender, EventArgs e)
    {
        //SEE WHETHER POSTAL CODE IS REQUIRED
        string[] countries = Store.GetCachedSettings().PostalCodeCountries.Split(",".ToCharArray());
        PostalCodeRequired.Enabled = (Array.IndexOf(countries, Country.SelectedValue) > -1);
        USPostalCodeRequired.Enabled = (Country.SelectedValue == "US");
        CAPostalCodeRequired.Enabled = (Country.SelectedValue == "CA");
        //LOAD PROVINCES FOR SELECTED COUNTRY
        ProvinceCollection provinces = ProvinceDataSource.LoadForCountry(Country.SelectedValue);
        //WE WANT TO SHOW THE DROP DOWN IF THE COUNTRY HAS BEEN CHANGED BY THE CLIENT
        //AND ALSO IF PROVINCES ARE AVAILABLE FOR THIS COUNTRY
        if (_ShowProvinceList && provinces.Count > 0)
        {
            Province.Visible = false;
            ProvinceRequired.Enabled = false;
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
            //WE ONLY WANT A TEXTBOX TO SHOW
            //REQUIRE THE TEXTBOX IF THERE ARE PROVINCES
            Province.Visible = true;
            ProvinceRequired.Enabled = (provinces.Count > 0);
            Province.Visible = true;
            Province2.Visible = false;
            Province2.Items.Clear();
            Province2Required.Enabled = false;
            //NOW THAT A TEXTBOX IS SHOWN, RESET THE FLAG TO DISPLAY DROPDOWN
            _ShowProvinceList = false;
        }
        //UPDATE THE LAST SELECTED COUNTRY
        _SelectedCountryCode = Country.SelectedValue;
    }

    private Address GetAddressToEdit()
    {
        if (_EditAddressId > 0)
        {
            foreach (Address a in _SortedAddresses)
            {
                if (a.AddressId == _EditAddressId) return a;
            }
        }
        Address addr = new Address();
        addr.UserId = _User.UserId;
        _User.Addresses.Add(addr);
        return addr;
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            string formattedProvinceName;
            if (ValidateProvince(Country, Province, Province2, out formattedProvinceName))
            {
                Address address = GetAddressToEdit();
                address.FirstName = StringHelper.StripHtml(FirstName.Text);
                address.LastName = StringHelper.StripHtml(LastName.Text);
                address.Address1 = StringHelper.StripHtml(Address1.Text);
                address.Address2 = StringHelper.StripHtml(Address2.Text);
                address.Company = StringHelper.StripHtml(Company.Text);
                address.City = StringHelper.StripHtml(City.Text);
                address.Province = formattedProvinceName;
                address.PostalCode = StringHelper.StripHtml(PostalCode.Text);
                address.CountryCode = Country.SelectedValue;
                address.Phone = StringHelper.StripHtml(Phone.Text);
                address.Fax = StringHelper.StripHtml(Fax.Text);
                address.Residence = !IsBusiness.Checked;
                address.Save();
                //REBUILD ADDRESS BOOK
                InitAddressBook();
                //SWITCH BACK TO VIEW MODE
                _PageMode = "V";
            }
            else
            {
                Province2Required.IsValid = false;
                _ShowProvinceList = true;
                CountryChanged(null, null);
            }
        }
    }

    private bool IsAddressValid(Address address)
    {
        if (string.IsNullOrEmpty(address.FirstName)) return false;
        if (string.IsNullOrEmpty(address.LastName)) return false;
        if (string.IsNullOrEmpty(address.Address1)) return false;
        if (string.IsNullOrEmpty(address.City)) return false;
        if (string.IsNullOrEmpty(address.CountryCode)) return false;
        //SEE WHETHER POSTAL CODE IS REQUIRED
        string[] countries = Store.GetCachedSettings().PostalCodeCountries.Split(",".ToCharArray());
        bool requirePostalCode = (Array.IndexOf(countries, address.CountryCode) > -1);
        if (requirePostalCode)
        {
            if (string.IsNullOrEmpty(address.PostalCode)) return false;
            if ((address.CountryCode == "US") && (!Regex.IsMatch(address.PostalCode, "^\\d{5}(-\\d{4})?$"))) return false;
            if ((address.CountryCode == "CA") && (!Regex.IsMatch(address.PostalCode, "^[A-Za-z][0-9][A-Za-z] ?[0-9][A-Za-z][0-9]$"))) return false;
        }
        //SEE WHETHER PROVINCE IS VALID
        if (ProvinceDataSource.CountForCountry(address.CountryCode) > 0)
        {
            int provinceId = ProvinceDataSource.GetProvinceIdByName(address.CountryCode, address.Province);
            if (provinceId == 0) return false;
        }
        return true;
    }

    /// <summary>
    /// Validates the current province value
    /// </summary>
    /// <returns></returns>
    private bool ValidateProvince(DropDownList CountryList, TextBox ProvinceText, DropDownList ProvinceList, out string provinceName)
    {
        provinceName = StringHelper.StripHtml(Request.Form[ProvinceText.UniqueID]);
        if (string.IsNullOrEmpty(provinceName)) provinceName = Request.Form[ProvinceList.UniqueID];
        string countryCode = CountryList.SelectedValue;
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

    private class AddressComparer : IComparer, IComparer<Address>
    {
        private int _PrimaryAddressId;
        public AddressComparer(int primaryAddressId)
        {
            _PrimaryAddressId = primaryAddressId;
        }
        public int Compare(object x, object y)
        {
            return this.Compare(x as Address, y as Address);
        }
        public int Compare(Address x, Address y)
        {
            if (x.AddressId == y.AddressId) return 0;
            if (x.AddressId == _PrimaryAddressId) return -1;
            if (y.AddressId == _PrimaryAddressId) return 1;
            string nX = (string.IsNullOrEmpty(x.Company) ? string.Empty : x.Company + " ") + x.FullName;
            string nY = (string.IsNullOrEmpty(y.Company) ? string.Empty : y.Company + " ") + y.FullName;
            return string.Compare(nX, nY);
        }
    }
}