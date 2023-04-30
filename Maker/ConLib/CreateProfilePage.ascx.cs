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
using System.Collections.Generic;
using MakerShop.Utility;

public partial class ConLib_CreateProfilePage : System.Web.UI.UserControl
{
    protected void Page_Init(object sender, EventArgs e)
    {
        LoginLink.NavigateUrl = NavigationHelper.GetCheckoutUrl();
        EmailListCollection publicLists = GetPublicEmailLists();
        if (publicLists.Count > 0)
        {
            dlEmailLists.DataSource = publicLists;
            dlEmailLists.DataBind();
        }
        else
        {
            EmailListPanel.Visible = false;
        }
        Country.DataSource = CountryDataSource.LoadForStore();
        Country.DataBind();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            InitAddressForm();
            ShowPasswordPolicy();
        }
    }

    private bool _PasswordLengthValidatorAdded = false;
    private void ShowPasswordPolicy()
    {
        //SHOW THE PASSWORD POLICY
        CustomerPasswordPolicy policy = new CustomerPasswordPolicy();
        if (policy.MinLength > 0)
        {
            PasswordPolicyLength.Text = string.Format(PasswordPolicyLength.Text, policy.MinLength);
            if (!_PasswordLengthValidatorAdded)
            {
                RegularExpressionValidator PasswordLengthValidator = new RegularExpressionValidator();
                PasswordLengthValidator.ID = "PasswordLengthValidator";
                PasswordLengthValidator.EnableViewState = false;
                PasswordLengthValidator.ControlToValidate = "Password";
                PasswordLengthValidator.Text = "*";
                PasswordLengthValidator.ErrorMessage = "Password must be at least " + policy.MinLength.ToString() + " characters.";
                PasswordLengthValidator.ValidationExpression = ".{" + policy.MinLength.ToString() + ",}";
                PasswordLengthValidator.SetFocusOnError = false;
                PasswordLengthValidator.EnableClientScript = false;                
                PasswordValidatorPanel.Controls.Add(PasswordLengthValidator);
                _PasswordLengthValidatorAdded = true;
            }
        }
        else PasswordPolicyLength.Visible = false;
        List<string> requirements = new List<string>();
        if (policy.RequireUpper) requirements.Add("uppercase letter");
        if (policy.RequireLower) requirements.Add("lowercase letter");
        if (policy.RequireNumber) requirements.Add("number");
        if (policy.RequireSymbol) requirements.Add("symbol");
        if (!policy.RequireNumber && !policy.RequireSymbol && policy.RequireNonAlpha) requirements.Add("non-letter");
        PasswordPolicyRequired.Visible = (requirements.Count > 0);
        if (PasswordPolicyRequired.Visible)
        {
            if (requirements.Count > 1) requirements[requirements.Count - 1] = "and " + requirements[requirements.Count - 1];
            PasswordPolicyRequired.Text = string.Format(PasswordPolicyRequired.Text, string.Join(", ", requirements.ToArray()));
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

    protected EmailListCollection GetPublicEmailLists()
    {
        EmailListCollection publicLists = new EmailListCollection();
        EmailListCollection allLists = EmailListDataSource.LoadForStore();
        foreach (EmailList list in allLists)
        {
            if (list.IsPublic) publicLists.Add(list);
        }
        return publicLists;
    }

    protected void ContinueButton_Click(object sender, EventArgs e)
    {
        if (!ValidateProvince())
        {
            ValidatePassword();
            Province2Invalid.IsValid = false;
            UpdateCountry();
            return;
        }

        if (Page.IsValid && ValidatePassword())
        {            
            MembershipCreateStatus status;
            User newUser = UserDataSource.CreateUser(UserName.Text, UserName.Text, Password.Text, string.Empty, string.Empty, true, Token.Instance.User.AffiliateId, out status);
            if (status == MembershipCreateStatus.Success)
            {
                if (Membership.ValidateUser(UserName.Text, Password.Text))
                {
                    if (RememberMe.Checked)
                    {
                        HttpCookie cookie = new HttpCookie("UserName", UserName.Text);
                        cookie.Expires = DateTime.MaxValue;
                        Response.Cookies.Add(cookie);
                        Trace.Write("Added UserName Cookie to Response");
                    }
                    else
                    {
                        Response.Cookies.Add(new HttpCookie("UserName", ""));
                    }
                    //MIGRATE THE USER
                    MakerShop.Users.User.Migrate(Token.Instance.User, newUser);
                    Token.Instance.InitUserContext(newUser);
                    //UPDATE THE PRIMARY ADDRESS
                    Address address = newUser.PrimaryAddress;
                    address.Email = newUser.UserName;
                    address.FirstName = FirstName.Text;
                    address.LastName = LastName.Text;
                    address.Address1 = Address1.Text;
                    address.Address2 = Address2.Text;
                    address.City = City.Text;
                    address.Province = (Province.Visible ? Province.Text : Province2.SelectedValue);
                    address.PostalCode = PostalCode.Text;
                    address.CountryCode = Country.SelectedValue;
                    address.Phone = Phone.Text;
                    address.Fax = Fax.Text;
                    address.Company = Company.Text;
                    address.Residence = (Residence.SelectedIndex == 0);
                    newUser.Save();
                    //SIGNUP LIST
                    int index = 0;
                    foreach (DataListItem item in dlEmailLists.Items)
                    {
                        CheckBox selected = (CheckBox)item.FindControl("Selected");
                        if ((selected != null) && (selected.Checked))
                        {
                            int emailListId = (int)dlEmailLists.DataKeys[index];
                            EmailList list = EmailListDataSource.Load(emailListId);
                            list.ProcessSignupRequest(newUser.Email);
                        }
                        index++;
                    }
                    //UPDATE AUTHORIZATION COOKIE
                    FormsAuthentication.SetAuthCookie(UserName.Text, false);

                    if(rbtnYes.Checked)
                    {
                        //WE DO NOT WANT TO RESET THE GIFT OPTIONS AT THIS STAGE
                        int addressId = address.AddressId;
                        MakerShop.Orders.Basket basket = Token.Instance.User.Basket;
                        basket.Package(false);
                        foreach (MakerShop.Orders.BasketShipment shipment in basket.Shipments)
                        {
                            shipment.AddressId = addressId;
                            shipment.Save();
                        }
                        Response.Redirect(NavigationHelper.GetCheckoutUrl(true,false));
                    }
                    else
                        //REDIRECT TO CHECKOUT
                        Response.Redirect(NavigationHelper.GetCheckoutUrl(true));
                }
            }
            else
            {
                InvalidRegistration.IsValid = false;
                switch (status)
                {
                    case MembershipCreateStatus.DuplicateUserName:
                    case MembershipCreateStatus.DuplicateEmail:
                        InvalidRegistration.ErrorMessage = "That email address is already registered.  Sign in to access your account.";
                        break;
                    case MembershipCreateStatus.InvalidEmail:
                    case MembershipCreateStatus.InvalidUserName:
                        InvalidRegistration.ErrorMessage = "The email address you provided is not valid.";
                        break;
                    case MembershipCreateStatus.InvalidPassword:
                        InvalidRegistration.ErrorMessage = "The password you provided is not valid.";
                        break;
                    default:
                        InvalidRegistration.ErrorMessage = "Unexpected error in registration (" + status.ToString() + ")";
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Validates the current province value
    /// </summary>
    /// <returns></returns>
    private bool ValidateProvince()
    {
        string provinceName = (Province.Visible ? StringHelper.StripHtml(Province.Text) : Province2.SelectedValue);
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

    private bool ValidatePassword()
    {
        //VALIDATE PASSWORD POLICY
        CustomerPasswordPolicy policy = new CustomerPasswordPolicy();
        if (!policy.TestPassword(null, Password.Text))
        {
            CustomValidator policyValidator = new CustomValidator();
            policyValidator.ControlToValidate = "Password";
            policyValidator.IsValid = false;
            policyValidator.Text = "*";
            policyValidator.ErrorMessage = "The password does not meet the minimum requirements.";
            policyValidator.SetFocusOnError = false;            
            PasswordValidatorPanel.Controls.Add(policyValidator);
            return false;
        }
        return true;
    }
}
