using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Common;
using MakerShop.Users;
using MakerShop.Marketing;
using MakerShop.Orders;
using MakerShop.Products;
using MakerShop.Payments;
using MakerShop.Stores;
using MakerShop.Shipping;
using MakerShop.Taxes;
using MakerShop.Utility;

public partial class ConLib_OnePageCheckout : System.Web.UI.UserControl
{
    Basket _Basket;
    CountryCollection _Countries;
    private bool _ShipMethodError = false;

    private CountryCollection Countries
    {
        get
        {
            if (_Countries == null) InitializeCountries();
            return _Countries;
        }
    }

    bool _EditBillingAddress = true;
    private bool EditBillingAddress
    {
        get { return _EditBillingAddress; }
        set { _EditBillingAddress = value; }
    }

    bool _EditShippingAddress = true;
    private bool EditShippingAddress
    {
        get { return (_EditShippingAddress && this.HasShippableProducts); }
        set { _EditShippingAddress = value; }
    }

    int _ShipToAddressId = -1;
    private int ShipToAddressId
    {
        get { return _ShipToAddressId; }
        set { _ShipToAddressId = value; }
    }

    private int _HasShippableProducts = -1;
    private bool HasShippableProducts
    {
        get
        {
            if (_HasShippableProducts < 0)
            {
                _HasShippableProducts = _Basket.Items.HasShippableProducts ? 1 : 0;
            }
            return (_HasShippableProducts == 1);
        }
        set { _HasShippableProducts = (value ? 1 : 0); }
    }

    int _ShipmentCount = 0;
    protected int ShipmentCount
    {
        get { return _ShipmentCount; }
        set { _ShipmentCount = value; }
    }

    bool _EnableGiftWrap = true;
    public bool EnableGiftWrap
    {
        get { return _EnableGiftWrap; }
        set { _EnableGiftWrap = value; }
    }

    bool _EnableMultiShipTo = true;
    public bool EnableMultiShipTo
    {
        get { return _EnableMultiShipTo; }
        set { _EnableMultiShipTo = value; }
    }

    private bool _EnableShipMessage = false;
    public bool EnableShipMessage
    {
        get { return _EnableShipMessage; }
        set { _EnableShipMessage = value; }
    }

    private bool _AllowAnonymousCheckout = false;
    public bool AllowAnonymousCheckout
    {
        get { return _AllowAnonymousCheckout; }
        set { _AllowAnonymousCheckout = value; }
    }

    private int _MaxCheckoutAttemptsPerMinute = 10;
    public int MaxCheckoutAttemptsPerMinute
    {
        get { return _MaxCheckoutAttemptsPerMinute; }
        set { _MaxCheckoutAttemptsPerMinute = value; }
    }

    private bool _DisableBots = true;
    public bool DisableBots
    {
        get { return _DisableBots; }
        set { _DisableBots = value; }
    }

    /*private int _CheckoutMinimumDelay = 1;
    public int CheckoutMinimumDelay
    {
        get { return _CheckoutMinimumDelay; }
        set { _CheckoutMinimumDelay = value; }
    }*/

    //holds saved basket hash
    private string _SavedBasketHash = string.Empty;
    //hold the calculated basket hash
    private string _CurrentBasketHash = string.Empty;

    //KEEP TRACK OF COUNTRY SELECTIONS TO DETERMINE WHEN TO SHOW PROVINCE DROP DOWN LIST
    //(LIST ONLY SHOWS IF COUNTRY SELECTION IS CHANGED OR THERE IS A VALIDATION ERROR)
    private string _LastBillToCountry = string.Empty;
    private bool _BillToProvinceError = false;
    private string _LastShipToCountry = string.Empty;
    private bool _ShipToProvinceError = false;
    private bool _ShowBillToProvinceList = false;
    private bool _ShowShipToProvinceList = false;

    //key is the shipment id, list of qutoes
    Dictionary<int, List<LocalShipRateQuote>> _SavedShipRates = new Dictionary<int, List<LocalShipRateQuote>>();
    private void LoadCustomViewState()
    {
        User user = Token.Instance.User;
        _CurrentBasketHash = user.Basket.GetContentHash(OrderItemType.Product);
        if (Page.IsPostBack)
        {
            UrlEncodedDictionary customViewState = new UrlEncodedDictionary(EncryptionHelper.DecryptAES(Request.Form[VS_CustomState.UniqueID]));
            _EditBillingAddress = (AlwaysConvert.ToInt(customViewState.TryGetValue("EditBillingAddress")) == 1);
            _EditShippingAddress = (AlwaysConvert.ToInt(customViewState.TryGetValue("EditShippingAddress")) == 1);
            _ShipToAddressId = AlwaysConvert.ToInt(customViewState.TryGetValue("ShipToAddressId"));
            _LastBillToCountry = customViewState.TryGetValue("LastBillToCountry");
            _BillToProvinceError = AlwaysConvert.ToBool(customViewState.TryGetValue("BillToProvinceError"), false);
            _LastShipToCountry = customViewState.TryGetValue("LastShipToCountry");
            _ShipToProvinceError = AlwaysConvert.ToBool(customViewState.TryGetValue("ShipToProvinceError"), false);
            _ShowBillToProvinceList = AlwaysConvert.ToBool(customViewState.TryGetValue("ShowBillToProvinceList"), false);
            _ShowShipToProvinceList = AlwaysConvert.ToBool(customViewState.TryGetValue("ShowShipToProvinceList"), false);
            string savedBasketHash = customViewState.TryGetValue("SavedBasketHash");
            if (savedBasketHash == _CurrentBasketHash)
            {
                string savedShipRates = customViewState.TryGetValue("SavedShipRates");
                ParseSavedShipRates(savedShipRates);
            }
            _SavedBasketHash = savedBasketHash;
        }
        if (string.IsNullOrEmpty(_LastBillToCountry)) _LastBillToCountry = user.PrimaryAddress.CountryCode;
        if (string.IsNullOrEmpty(_LastShipToCountry)) _LastShipToCountry = GetDefaultShipToCountry(user);
    }

    private string GetDefaultShipToCountry(User user)
    {
        int billingAddressId = user.PrimaryAddress.AddressId;
        foreach (Address address in user.Addresses)
        {
            if (address.AddressId != billingAddressId) return address.CountryCode;
        }
        return user.PrimaryAddress.CountryCode;
    }

    private void ParseSavedShipRates(string savedShipRates)
    {
        if (!string.IsNullOrEmpty(savedShipRates))
        {
            string[] shipRates = StringHelper.Split(savedShipRates, "__");
            for (int i = 0; i < shipRates.Length; i += 2)
            {
                int shipmentNumber = AlwaysConvert.ToInt(shipRates[i]);
                List<LocalShipRateQuote> shipRateQuoteItems = new List<LocalShipRateQuote>();
                string[] encodedQuoteValues = StringHelper.Split(shipRates[i + 1], "||");
                foreach (string encodedQuoteValue in encodedQuoteValues)
                {
                    shipRateQuoteItems.Add(new LocalShipRateQuote(encodedQuoteValue));
                }
                _SavedShipRates[shipmentNumber] = shipRateQuoteItems;
            }
        }
    }

    private void SaveCustomViewState()
    {
        UrlEncodedDictionary customViewState = new UrlEncodedDictionary();
        customViewState.Add("EditBillingAddress", (_EditBillingAddress ? "1" : "0"));
        customViewState.Add("EditShippingAddress", (_EditShippingAddress ? "1" : "0"));
        customViewState.Add("ShipToAddressId", _ShipToAddressId.ToString());
        customViewState.Add("LastBillToCountry", BillToCountry.SelectedValue);
        customViewState.Add("BillToProvinceError", _BillToProvinceError.ToString());
        customViewState.Add("LastShipToCountry", ShipToCountry.SelectedValue);
        customViewState.Add("ShipToProvinceError", _ShipToProvinceError.ToString());
        customViewState.Add("SavedBasketHash", _CurrentBasketHash);
        customViewState.Add("SavedShipRates", EncodeSavedShipRates());
        customViewState.Add("ShowBillToProvinceList", _ShowBillToProvinceList.ToString());
        customViewState.Add("ShowShipToProvinceList", _ShowShipToProvinceList.ToString());
        VS_CustomState.Value = EncryptionHelper.EncryptAES(customViewState.ToString());
    }

    private string EncodeSavedShipRates()
    {
        StringBuilder encodedRates = new StringBuilder();
        string separator = string.Empty;
        foreach (int shipmentNumber in _SavedShipRates.Keys)
        {
            if (_SavedShipRates[shipmentNumber].Count > 0)
            {
                encodedRates.Append(separator);
                encodedRates.Append(shipmentNumber.ToString() + "__");
                string separator2 = string.Empty;
                StringBuilder encodedQuotes = new StringBuilder();
                foreach (LocalShipRateQuote quoteItem in _SavedShipRates[shipmentNumber])
                {
                    encodedQuotes.Append(separator2);
                    encodedQuotes.Append(quoteItem.Encode());
                    separator2 = "||";
                }
                encodedRates.Append(encodedQuotes.ToString());
                separator = "__";
            }
        }
        return encodedRates.ToString();
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        //MAKE SURE THERE ARE ITEMS IN THE BASKET
        _Basket = Token.Instance.User.Basket;
        bool hasItems = false;
        foreach (BasketItem item in _Basket.Items)
        {
            if (item.OrderItemType == OrderItemType.Product)
            {
                hasItems = true;
                break;
            }
        }
        if (!hasItems)
        {
            //THERE ARE NO PRODUCTS, SEND THEN TO SEE EMPTY BASKET
            Response.Redirect(NavigationHelper.GetBasketUrl());
        }

        // INITILIZE SHIP TO ADDRESS FROM QUERY STRING
        ShipToAddressId = AlwaysConvert.ToInt(Request.QueryString["ShipAddressId"], -1);

        //VALIDATE ORDER IS WITHIN LIMITS
        ValidateOrderMinMaxAmounts();

        //WE CANNOT RELY ON TRADITIONAL VIEWSTATE IN THE INIT METHOD
        //SO WE WILL USE HIDDEN FORM VARIABLES INSTEAD
        LoadCustomViewState();

        if (CheckoutPanel.Visible)
        {
            //DO NOT SCROLL TO VALIDATION SUMMARY
            PageHelper.DisableValidationScrolling(this.Page);

            //ADD SCRIPTS FOR TERMS AND CONDITIONS
            string TCScript = "function toggleTC(c) { document.getElementById(\"" + AcceptTC.ClientID + "\").checked = c; }\r\n" +
                "function validateTC(source, args) { args.IsValid = document.getElementById(\"" + AcceptTC.ClientID + "\").checked; }";
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "TCScript", TCScript, true);

            //HANDLE THE EDIT ADDRESS CLICK
            if (Request.Form["__EVENTTARGET"] == EditAddressesButton.UniqueID)
            {
                this.EditBillingAddress = true;
                trContinue.Visible = true;
                InitializeBillingAddress();
                InitializeShippingAddress();
                //WE SHOULD RECALCULATE SHIP RATES AFTER ADDRESS EDITS
                _SavedShipRates = new Dictionary<int, List<LocalShipRateQuote>>();
                InitializeBasket();
            }

            if (Request.Form["__EVENTTARGET"] == EditAddressesButton2.UniqueID)
            {
                //this.EditBillingAddress = true;
                this.EditShippingAddress = true;
                InitializeBillingAddress();

                UseBillingAddress.Checked = false;
                UseShippingAddress.Checked = true;
                ShipAddressPanel.Visible = true;
                //PREPARE ADDRESS BOOK                
                InitializeAddressBook();

                //WE SHOULD RECALCULATE SHIP RATES AFTER ADDRESS EDITS
                _SavedShipRates = new Dictionary<int, List<LocalShipRateQuote>>();
                InitializeBasket();
            }
            if (Request.Form["__EVENTTARGET"] == AddAddressesButton.UniqueID)
            {
                this.EditBillingAddress = true;
                this.EditShippingAddress = true;
                InitializeBillingAddress();

                UseBillingAddress.Checked = false;
                UseShippingAddress.Checked = true;
                ShipAddressPanel.Visible = true;
                ShipToAddressId = 0;
                //PREPARE ADDRESS BOOK                
                InitializeAddressBook();

                //WE SHOULD RECALCULATE SHIP RATES AFTER ADDRESS EDITS
                _SavedShipRates = new Dictionary<int, List<LocalShipRateQuote>>();
                InitializeBasket();

            }

            //UPDATE VALUES BASED ON SHIPMENTS
            _ShipmentCount = _Basket.Shipments.Count;
            MultipleShipmentsMessage.Visible = (_ShipmentCount > 1);
            BasketGrid.Columns[0].Visible = (_ShipmentCount > 1);

            //MAKE SURE THE CORRECT ADDRESS PANELS ARE VISIBLE BASED ON VIEWSTATE SETTING
            TogglePanels();

            //ALLOW ANONYMOUS CHECKOUT IF ENABLED BY PARAMETER, AND THERE ARE NO SUBSCRIPTION ITEMS
            AnonymousCheckoutPanel.Visible = (AllowAnonymousCheckout && !HasSubscriptionItems());
        }

        //OPCNotBot.ResponseMinimumDelaySeconds = CheckoutMinimumDelay;
        OPCNotBot.ResponseMinimumDelaySeconds = 0;
        OPCNotBot.CutoffWindowSeconds = 60;
        OPCNotBot.CutoffMaximumInstances = MaxCheckoutAttemptsPerMinute;
    }

    private bool HasSubscriptionItems()
    {
        Basket basket = Token.Instance.User.Basket;
        foreach (BasketItem item in basket.Items)
        {
            if ((item.Product != null) && (item.Product.SubscriptionPlan != null)) return true;
        }
        return false;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (CheckoutPanel.Visible)
        {
            //SHIPPING ADDRESS HAS CHANGED, INITIALIZE NOW
            //TO OVERRIDE POSTED BACK VALUES
            if (_NewShippingAddress != null)
                InitializeShippingAddressForm(_NewShippingAddress, true);

            if (!Page.IsPostBack)
            {
                InitializeBasket();
            }
            else if (Request.Form["__EVENTTARGET"].EndsWith("ShipMethodList"))
            {
                //RECALCULATE ORDER USING SELECTED SHIPPING METHODS
                //ONLY DO THIS IF THE SHIPPING METHOD LIST HAS CHANGED
                //ALSO REBIND THE PAYMENT FORMS
                RecalculateBasket(true);
                ValidateShipStatus(false);
            }
            //PROCESS SELECTION AFTER VIEWSTATE LOADS
            SelectProvinceOnLoad();
        }
        if (!IsPostBack)
        {
            BotMessagePanel.Visible = false;
        }
    }

    private void InitializeBasket()
    {
        Basket basket = Token.Instance.User.Basket;
        basket.Package(false, true);
        foreach (BasketShipment shipment in basket.Shipments)
        {
            shipment.ShipMethodId = 0;
            shipment.Save();
        }
        basket.Recalculate();
        _CurrentBasketHash = basket.GetContentHash(OrderItemType.Product);
    }

    private void RecalculateBasket()
    {
        RecalculateBasket(false);
    }

    private void RecalculateBasket(bool rebindPaymentForms)
    {
        Basket basket = Token.Instance.User.Basket;
        UpdateShippingAddress(basket, GetShippingAddress());
        //UPDATE SHIPPING RATES
        if (trShowRates.Visible)
        {
            int shipmentIndex = 0;
            foreach (RepeaterItem item in ShipmentList.Items)
            {
                BasketShipment shipment = basket.Shipments[shipmentIndex];
                DropDownList ShipMethodList = (DropDownList)item.FindControl("ShipMethodList");
                if (ShipMethodList != null)
                {
                    shipment.ShipMethodId = AlwaysConvert.ToInt(ShipMethodList.SelectedValue);
                }
                else
                {
                    shipment.ShipMethodId = 0;
                }
                shipment.Save();
                shipmentIndex++;
            }
        }
        //RECALCULATE SHIPPING, TAXES, DISCOUNTS, ETC.
        basket.Recalculate();
        _CurrentBasketHash = basket.GetContentHash(OrderItemType.Product);
        if (rebindPaymentForms) BindPaymentMethodForms();
    }

    private static void UpdateShippingAddress(Basket basket, Address shipAddress)
    {
        foreach (BasketShipment shipment in basket.Shipments)
        {
            shipment.SetAddress(shipAddress);
            shipment.Save();
        }
    }

    private void TogglePanels()
    {
        //PREPARE FLAGS
        User user = Token.Instance.User;
        Basket basket = user.Basket;
        bool editBilling = this.EditBillingAddress;
        bool editShipping = this.EditShippingAddress;
        bool isAnonymous = user.IsAnonymous;
        bool needsShipping = this.HasShippableProducts;
        //DISPLAY LOGIN PANEL FOR ANONYMOUS USERS
        if (editBilling && isAnonymous)
        {
            LoginPanel.Visible = true;
            LoginLink.NavigateUrl = "~/Login.aspx?ReturnUrl=" + Server.UrlEncode(Request.Url.AbsolutePath);
        }
        else LoginPanel.Visible = false;
        //DISPLAY BILLING ADDRESS
        if (editBilling)
        {
            EnterBillingPanel.Visible = true;
            trEmail.Visible = isAnonymous;
            InitializeBillingAddress();
            ViewBillingPanel.Visible = false;
        }
        else
        {
            EnterBillingPanel.Visible = false;
            ViewBillingPanel.Visible = true;
        }
        //DISPLAY SHIPPING ADDRESS
        if (needsShipping)
        {
            EnterShippingPanel.Visible = editShipping;
            if (editShipping) InitializeShippingAddress();
            ViewShippingPanel.Visible = !editShipping;
            trContinue.Visible = (editShipping || editBilling);
            trShowRates.Visible = !(editShipping || editBilling);
        }
        else
        {
            trShippingAddress.Visible = false;
            trContinue.Visible = (editShipping || editBilling);
            trShowRates.Visible = false;
        }
        if (!editShipping)
        {
            bool hasAddressBook = (user.Addresses.Count > 1);
            bool shippingIsBilling = (this.ShipToAddressId < 0) || (this.ShipToAddressId == user.PrimaryAddress.AddressId);
            EditAddressesButton2.Visible = (hasAddressBook || !shippingIsBilling);
        }
        //DISPLAY REGISTRATION
        trAlreadyRegistered.Visible = false;
        trAccount.Visible = (!(editBilling || editShipping) && isAnonymous);
        if (trAccount.Visible)
        {
            bool anonChecked = !string.IsNullOrEmpty(Request.Form[AnonymousCheckoutIndicator.UniqueID]);
            if (anonChecked) CreateAccountPanel.Visible = false;
            else
            {
                CreateAccountPanel.Visible = true;
                int existingUserId = UserDataSource.GetUserIdByEmail(user.PrimaryAddress.Email, false);
                if(existingUserId == 0) existingUserId = UserDataSource.GetUserIdByUserName(user.PrimaryAddress.Email);

                if (existingUserId > 0)
                {
                    trAlreadyRegistered.Visible = true;
                    RegisteredUserName.Text = user.PrimaryAddress.Email;
                    trAccount.Visible = false;
                    trShowRates.Visible = false;
                }
                else
                {
                    trNewUserName1.Visible = needsShipping;
                    trNewUserName2.Visible = !needsShipping;
                    NewUserPassword.Attributes.Add("value", Request.Form[NewUserPassword.UniqueID]);
                    NewUserPassword2.Attributes.Add("value", Request.Form[NewUserPassword2.UniqueID]);
                    ShowPasswordPolicy();
                }
            }
        }
        if (!trAlreadyRegistered.Visible)
        {
            //DISPLAY EMAIL LISTS
            trEmailLists.Visible = false;
            if (!(editBilling || editShipping) && !trAlreadyRegistered.Visible)
            {
                EmailListCollection publicLists = GetPublicEmailLists();
                if (publicLists.Count > 0)
                {
                    trEmailLists.Visible = true;
                    EmailLists.DataSource = publicLists;
                    EmailLists.DataBind();
                    //HIDE THE LABEL FOR THIS SECTION IF THEY ARE A NEW USER
                    //if (isAnonymous) EmailListHeader.Visible = false;
                }
            }
            //DISPLAY PAYMENT METHODS
            if (!(editBilling || editShipping))
            {
                trPayment.Visible = true;
                if (needsShipping)
                {
                    //calculate shipping
                    UpdateShippingAddress(basket, GetShippingAddress());
                    ShipmentList.DataSource = basket.Shipments;
                    ShipmentList.DataBind();
                }
                if (_ShipMethodError)
                {
                    trAccount.Visible = false;
                    trAlreadyRegistered.Visible = false;
                    trEmailLists.Visible = false;
                    trPayment.Visible = false;
                    ShipMethodErrorMessage.Visible = true;
                }
                else
                {
                    //update account setting
                    NewUserName.Text = user.PrimaryAddress.Email;
                    //bind the payments
                    BindPaymentMethodForms();
                }
            }
            //UPDATE RIGHT COLUMN ELEMENTS
            if (!needsShipping)
            {
                LSDecimal totalCost = basket.Items.TotalPrice();
                if (totalCost == 0)
                {
                    //THIS ORDER WILL NOT HAVE A TOTAL, COUPON IS POINTLESS
                    CouponDialog1.Visible = false;
                }
                //CANNOT HAVE GIFTWRAP
                GiftWrapPanel.Visible = false;
                //CANNOT SHIP MUTLI
                ShipMultiPanel.Visible = false;
            }
            else
            {
                //UPDATE GIFT WRAP LINK
                if (ShipToAddressId > 0) EditGiftOptionsLink.NavigateUrl = "~/Checkout/GiftOptions.aspx?ReturnUrl=" + Server.UrlEncode(Request.Url.AbsolutePath + "?ShipAddressId=" + ShipToAddressId);
                else EditGiftOptionsLink.NavigateUrl = "~/Checkout/GiftOptions.aspx?ReturnUrl=" + Server.UrlEncode(Request.Url.AbsolutePath);
                ShipMultiPanel.Visible = (ShippableProductCount > 1);
                if (ShipMultiPanel.Visible) ShipMultiRegisterMessage.Visible = isAnonymous;
                //DO NOT DISPLAY COUPON UNTIL PAYMENT IS SHOWN
                CouponDialog1.Visible = trPayment.Visible;
            }
        }
    }

    private int _ShippableProductCount = -1;
    private int ShippableProductCount
    {
        get
        {
            if (_ShippableProductCount < 0)
            {
                _ShippableProductCount = _Basket.Items.ShippableProductCount;
                Trace.Write("ShippableProductCount: " + _ShippableProductCount);
            }
            return _ShippableProductCount;
        }
    }

    private void ShowPasswordPolicy()
    {
        //SHOW THE PASSWORD POLICY
        CustomerPasswordPolicy policy = new CustomerPasswordPolicy();
        if (policy.MinLength > 0)
        {
            PasswordPolicyLength.Text = string.Format(PasswordPolicyLength.Text, policy.MinLength);            
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

    EmailListCollection _PublicEmailLists = null;
    private EmailListCollection GetPublicEmailLists()
    {
        if (_PublicEmailLists == null)
        {
            _PublicEmailLists = new EmailListCollection();
            EmailListCollection allLists = EmailListDataSource.LoadForStore("Name");
            foreach (EmailList list in allLists)
            {
                if (list.IsPublic) _PublicEmailLists.Add(list);
            }
        }
        return _PublicEmailLists;
    }

    protected bool IsEmailListChecked(object dataItem)
    {
        User user = Token.Instance.User;
        if (user.IsAnonymous) return true;
        EmailList list = (EmailList)dataItem;
        return (list.IsMember(user.Email));
    }

    private void InitializeCountries()
    {
        _Countries = CountryDataSource.LoadForStore("Name");
        //FIND STORE COUNTRY AND COPY TO FIRST POSITION
        string storeCountry = Token.Instance.Store.DefaultWarehouse.CountryCode;
        if (storeCountry.Length == 0) storeCountry = "US";
        int index = _Countries.IndexOf(storeCountry);
        if (index > -1)
        {
            Country breakItem = new Country(storeCountry);
            breakItem.Name = "----------";
            _Countries.Insert(0, breakItem);
            _Countries.Insert(0, _Countries[index + 1]);
            if (storeCountry == "US")
            {
                index = _Countries.IndexOf("CA");
                if (index > -1) _Countries.Insert(1, _Countries[index]);
            }
            else if (storeCountry == "CA")
            {
                index = _Countries.IndexOf("US");
                if (index > -1) _Countries.Insert(1, _Countries[index]);
            }
        }
    }

    private void InitializeBillingAddress()
    {
        User user = Token.Instance.User;
        Address billingAddress = user.PrimaryAddress;
        BillToFirstName.Text = billingAddress.FirstName;
        BillToLastName.Text = billingAddress.LastName;
        BillToCompany.Text = billingAddress.Company;
        BillToAddress1.Text = billingAddress.Address1;
        BillToAddress2.Text = billingAddress.Address2;
        BillToCity.Text = billingAddress.City;
        BillToPostalCode.Text = billingAddress.PostalCode;
        if (!billingAddress.IsValid) billingAddress.Residence = true;
        BillToAddressType.SelectedIndex = (billingAddress.Residence ? 0 : 1);
        BillToPhone.Text = billingAddress.Phone;
        BillToEmail.Text = billingAddress.Email;
        //INITIALIZE BILLING COUNTRY AND PROVINCE
        BillToCountry.DataSource = this.Countries;
        BillToCountry.DataBind();
        if (billingAddress.CountryCode.Length == 0) billingAddress.CountryCode = Token.Instance.Store.DefaultWarehouse.CountryCode;
        SelectCountryAndProvince(BillToCountry, billingAddress.CountryCode, false, BillToProvince, BillToProvinceList, BillToPostalCodeRequired, billingAddress.Province, false);
    }

    private void InitializeShippingAddress()
    {
        //CHECK WHETHER TO SHOW SHIPPING ADDRESS FORM
        if (Request.Form[UseShippingAddress.UniqueID] == "UseBillingAddress")
        {
            UseBillingAddress.Checked = true;
            this.ShipToAddressId = -1;
        }
        else if (Request.Form[UseShippingAddress.UniqueID] == "UseShippingAddress")
        {
            UseBillingAddress.Checked = false;
        }
        else UseBillingAddress.Checked = (ShipToAddressId < 0);
        UseShippingAddress.Checked = !UseBillingAddress.Checked;
        if (UseShippingAddress.Checked)
        {
            ShipAddressPanel.Visible = true;
            //PREPARE ADDRESS BOOK
            InitializeAddressBook();
        }
    }

    //ADDRESS BOOK
    private Address _NewShippingAddress = null;
    private void InitializeAddressBook()
    {
        //ONLY SHOW THE ADDRESES THAT ARE NOT THE PRIMARY (BILLING)
        AddressCollection addresses = new AddressCollection();
        addresses.AddRange(Token.Instance.User.Addresses);
        addresses.Sort("LastName");
        int defaultIndex = addresses.IndexOf(Token.Instance.User.PrimaryAddressId);
        if (defaultIndex > -1) addresses.RemoveAt(defaultIndex);
        if (addresses.Count > 0)
        {
            List<DictionaryEntry> formattedAddresses = new List<DictionaryEntry>();
            foreach (Address address in addresses)
                formattedAddresses.Add(new DictionaryEntry(address.AddressId, address.FullName + " " + address.Address1 + " " + address.City));
            //BIND THE ADDRESSES TO THE DATALIST
            AddressBook.DataSource = formattedAddresses;
            AddressBook.DataBind();
        }
        if (addresses.Count == AddressBook.Items.Count)
        {
            // PREVENT FROM ADDING TWICE
            AddressBook.Items.Add(new ListItem("new address", "0"));
        }
        //INITIALIZE THE SHIP TO COUNTRIES
        ShipToCountry.DataSource = this.Countries;
        ShipToCountry.DataBind();
        //FIND THE LAST ADDRESS BOUND
        bool reloadAddress = false;
        if (!string.IsNullOrEmpty(Request.Form[AddressBook.UniqueID]))
        {
            int tempAddressId = AlwaysConvert.ToInt(Request.Form[AddressBook.UniqueID]);
            if (ShipToAddressId != tempAddressId) reloadAddress = true;
            ShipToAddressId = tempAddressId;
        }
        else if ((ShipToAddressId < 0) && (addresses.Count > 0))
        {
            ShipToAddressId = addresses[0].AddressId;
        }
        Address shippingAddress = null;
        if (ShipToAddressId > 0)
        {
            User user = Token.Instance.User;
            int index = user.Addresses.IndexOf(ShipToAddressId);
            if (index > -1) shippingAddress = user.Addresses[index];
        }
        if (shippingAddress == null) shippingAddress = new Address();
        if (shippingAddress.CountryCode.Length == 0)
            shippingAddress.CountryCode = Token.Instance.Store.DefaultWarehouse.CountryCode;
        //BIND THE FORM
        ListItem selected = AddressBook.Items.FindByValue(shippingAddress.AddressId.ToString());
        if (selected != null)
        {
            AddressBook.SelectedIndex = AddressBook.Items.IndexOf(selected);
            //IF WE ARE CHANGING THE ADDRESS, INITIALIZE DURING FORM LOAD
            if (reloadAddress) _NewShippingAddress = shippingAddress;
            //OTHERWISE INTIIALIZE NOW
            else InitializeShippingAddressForm(shippingAddress, false);
        }

        // HIDE THE ADDRESS BOOK IF NO ADDRESSES IN THE ADDRESSBOOK
        if (AddressBook.Items.Count == 1)
        {
            trAddressBook.Visible = false;
        }
    }

    private void InitializeShippingAddressForm(Address shippingAddress, bool reloadAddress)
    {
        //IF SHIPPING COUNTRY IS CHANGED, WE SHOULD PROBABLY LOAD FROM FORM POST
        string countryValue = Request.Form[ShipToCountry.UniqueID];
        bool shippingCountryChanged = (Request.Form["__EVENTTARGET"] == ShipToCountry.UniqueID) || ((!string.IsNullOrEmpty(countryValue) && (shippingAddress.CountryCode != countryValue)));
        bool shipToNameEmpty = (string.IsNullOrEmpty(Request.Form[ShipToFirstName.UniqueID]) || string.IsNullOrEmpty(Request.Form[ShipToLastName.UniqueID]));
        if (reloadAddress || (shipToNameEmpty && !shippingCountryChanged))
        {
            //LOAD THE VALUES FROM THE DATABASE
            ShipToFirstName.Text = shippingAddress.FirstName;
            ShipToLastName.Text = shippingAddress.LastName;
            ShipToCompany.Text = shippingAddress.Company;
            ShipToAddress1.Text = shippingAddress.Address1;
            ShipToAddress2.Text = shippingAddress.Address2;
            ShipToCity.Text = shippingAddress.City;
            ShipToPostalCode.Text = shippingAddress.PostalCode;
            //if (!shippingAddress.IsValid) shippingAddress.Residence = true;
            ShipToAddressType.SelectedIndex = (shippingAddress.Residence ? 0 : 1);
            ShipToPhone.Text = shippingAddress.Phone;
            SelectCountryAndProvince(ShipToCountry, shippingAddress.CountryCode, true, ShipToProvince, ShipToProvinceList, ShipToPostalCodeRequired, shippingAddress.Province, true);
        }
        else
        {
            //WE NEED TO LOAD THE POSTED VALUES
            ShipToFirstName.Text = Request.Form[ShipToFirstName.UniqueID];
            ShipToLastName.Text = Request.Form[ShipToLastName.UniqueID];
            ShipToCompany.Text = Request.Form[ShipToCompany.UniqueID];
            ShipToAddress1.Text = Request.Form[ShipToAddress1.UniqueID];
            ShipToAddress2.Text = Request.Form[ShipToAddress2.UniqueID];
            ShipToCity.Text = Request.Form[ShipToCity.UniqueID];
            ShipToPostalCode.Text = Request.Form[ShipToPostalCode.UniqueID];
            ShipToAddressType.SelectedIndex = AlwaysConvert.ToInt(Request.Form[ShipToAddressType.UniqueID]);
            ShipToPhone.Text = Request.Form[ShipToPhone.UniqueID];
            SelectCountryAndProvince(ShipToCountry, shippingAddress.CountryCode, false, ShipToProvince, ShipToProvinceList, ShipToPostalCodeRequired, string.Empty, false);
        }
        ShipToAddressId = shippingAddress.AddressId;
    }

    private void SelectCountryAndProvince(DropDownList CountryList, string defaultCountry, bool forceCountry, TextBox ProvinceText, DropDownList ProvinceList, RequiredFieldValidator PostalCodeValidator, string defaultProvince, bool forceProvince)
    {
        SelectCountry(CountryList, defaultCountry, ProvinceText, ProvinceList, PostalCodeValidator, forceCountry);
        SelectProvince(ProvinceText, ProvinceList, defaultProvince, forceProvince);
    }

    private void SelectCountry(DropDownList CountryList, string defaultCountry, TextBox ProvinceText, DropDownList ProvinceList, RequiredFieldValidator PostalCodeValidator, bool forceCountry)
    {
        string countryValue = Request.Form[CountryList.UniqueID];
        if (forceCountry || string.IsNullOrEmpty(countryValue)) countryValue = defaultCountry;
        int index = this.Countries.IndexOf(countryValue);
        if (index > -1)
        {
            CountryList.SelectedIndex = index;
            UpdateProvinces(countryValue, ProvinceText, ProvinceList, PostalCodeValidator);
        }
    }

    private void UpdateProvinces(string countryCode, TextBox ProvinceText, DropDownList ProvinceList, RequiredFieldValidator PostalCodeValidator)
    {
        //SEE WHETHER POSTAL CODE IS REQUIRED
        string[] countries = Store.GetCachedSettings().PostalCodeCountries.Split(",".ToCharArray());
        PostalCodeValidator.Enabled = (Array.IndexOf(countries, countryCode) > -1);
        bool countryChanged, provinceError, showProvinces;
        if (ProvinceText.ID == "BillToProvince")
        {
            countryChanged = (!string.IsNullOrEmpty(_LastBillToCountry)) && (_LastBillToCountry != countryCode);
            provinceError = _BillToProvinceError;
            showProvinces = _ShowBillToProvinceList;
        }
        else
        {
            countryChanged = (!string.IsNullOrEmpty(_LastShipToCountry)) && (_LastShipToCountry != countryCode);
            provinceError = _ShipToProvinceError;
            showProvinces = _ShowShipToProvinceList;
        }
        //SEE WHETHER PROVINCE LIST IS DEFINED
        ProvinceCollection provinces = ProvinceDataSource.LoadForCountry(countryCode);
        if (provinces.Count > 0 && (countryChanged || provinceError || showProvinces))
        {
            ProvinceText.Visible = false;
            ProvinceList.Visible = true;
            ProvinceList.Items.Clear();
            ProvinceList.Items.Add(String.Empty);
            foreach (Province province in provinces)
            {
                string provinceValue = (!string.IsNullOrEmpty(province.ProvinceCode) ? province.ProvinceCode : province.Name);
                ProvinceList.Items.Add(new ListItem(province.Name, provinceValue));
            }
            ListItem selectedProvince = FindSelectedProvince(ProvinceText, ProvinceList);
            if (selectedProvince != null)
            {
                if (ProvinceText.ID == "BillToProvince")
                {
                    _SelectedBillToProvinceList = ProvinceList;
                    _SelectedBillToProvince = selectedProvince;
                }
                else
                {
                    _SelectedShipToProvinceList = ProvinceList;
                    _SelectedShipToProvince = selectedProvince;
                }
            }
            ProvinceText.Text = string.Empty;
        }
        else
        {
            string defaultValue = Request.Form[ProvinceText.UniqueID];
            if (string.IsNullOrEmpty(defaultValue)) defaultValue = Request.Form[ProvinceList.UniqueID];
            if (!string.IsNullOrEmpty(defaultValue))
            {
                if (ProvinceText.ID == "BillToProvince") _LoadBillToProvinceText = defaultValue;
                else _LoadShipToProvinceText = defaultValue;
            }
            ProvinceText.Visible = true;
            ProvinceList.Visible = false;
            ProvinceList.Items.Clear();
        }
        BillToProvinceRequired.Enabled = BillToProvinceList.Visible;
        _ShowBillToProvinceList = _ShowBillToProvinceList || BillToProvinceList.Visible;
        ShipToProvinceRequired.Enabled = ShipToProvinceList.Visible;
        _ShowShipToProvinceList = _ShowShipToProvinceList || ShipToProvinceList.Visible;
    }

    private ListItem _SelectedBillToProvince = null;
    private DropDownList _SelectedBillToProvinceList = null;
    private ListItem _SelectedShipToProvince = null;
    private DropDownList _SelectedShipToProvinceList = null;
    private string _LoadBillToProvinceText = string.Empty;
    private string _LoadShipToProvinceText = string.Empty;
    private void SelectProvinceOnLoad()
    {
        if (_SelectedBillToProvinceList != null && _SelectedBillToProvince != null)
        {
            _SelectedBillToProvinceList.SelectedIndex = _SelectedBillToProvinceList.Items.IndexOf(_SelectedBillToProvince);
        }
        if (_SelectedShipToProvinceList != null && _SelectedShipToProvince != null)
        {
            _SelectedShipToProvinceList.SelectedIndex = _SelectedShipToProvinceList.Items.IndexOf(_SelectedShipToProvince);
        }
        if (!string.IsNullOrEmpty(_LoadBillToProvinceText)) BillToProvince.Text = _LoadBillToProvinceText;
        if (!string.IsNullOrEmpty(_LoadShipToProvinceText)) ShipToProvince.Text = _LoadShipToProvinceText;
    }

    /// <summary>
    /// Obtains the province that should default to selected in the drop down list
    /// </summary>
    /// <returns>The province that should default to selected in the drop down list</returns>
    private ListItem FindSelectedProvince(TextBox ProvinceText, DropDownList ProvinceList)
    {
        string defaultValue = Request.Form[ProvinceText.UniqueID];
        if (string.IsNullOrEmpty(defaultValue)) defaultValue = Request.Form[ProvinceList.UniqueID];
        if (string.IsNullOrEmpty(defaultValue)) return null;
        defaultValue = defaultValue.ToUpperInvariant();
        foreach (ListItem item in ProvinceList.Items)
        {
            string itemText = item.Text.ToUpperInvariant();
            string itemValue = item.Value.ToUpperInvariant();
            if (itemText == defaultValue || itemValue == defaultValue) return item;
        }
        return null;
    }

    private void SelectProvince(TextBox ProvinceText, DropDownList ProvinceList, string defaultProvince, bool forceProvince)
    {
        if (ProvinceText.Visible)
        {
            ProvinceText.Text = defaultProvince;
            if (forceProvince)
            {
                if (ProvinceText.ID == "BillToProvinceText") _LoadBillToProvinceText = defaultProvince;
                else _LoadShipToProvinceText = defaultProvince;
            }
        }
        else
        {
            string provinceValue = Request.Form[ProvinceList.UniqueID];
            if (forceProvince || string.IsNullOrEmpty(provinceValue)) provinceValue = defaultProvince;
            ListItem selectedProvince = ProvinceList.Items.FindByValue(provinceValue);
            if (selectedProvince != null)
            {
                ProvinceList.SelectedIndex = ProvinceList.Items.IndexOf(selectedProvince);
                if (forceProvince)
                {
                    if (ProvinceList.ID == "BillToProvinceList")
                    {
                        _SelectedBillToProvinceList = ProvinceList;
                        _SelectedBillToProvince = selectedProvince;
                    }
                    else
                    {
                        _SelectedShipToProvinceList = ProvinceList;
                        _SelectedShipToProvince = selectedProvince;
                    }
                }
            }
        }
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

    protected void ShipmentList_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //SHOW SHIPPING METHODS
        DropDownList ShipMethodList = (DropDownList)e.Item.FindControl("ShipMethodList");
        if (ShipMethodList != null)
        {
            BasketShipment shipment = (BasketShipment)e.Item.DataItem;
            if (shipment != null)
            {
                List<LocalShipRateQuote> localQuotes;
                _SavedShipRates.TryGetValue(shipment.BasketShipmentId, out localQuotes);
                if (localQuotes == null)
                {
                    //RECALCULATE THE RATES
                    localQuotes = new List<LocalShipRateQuote>();
                    ICollection<ShipRateQuote> rateQuotes = ShipRateQuoteDataSource.QuoteForShipment(shipment);
                    foreach (ShipRateQuote quote in rateQuotes)
                    {
                        LSDecimal totalRate = TaxHelper.GetShopPrice(quote.TotalRate, quote.ShipMethod.TaxCodeId, null, new TaxAddress(shipment.Address));
                        string formattedRate = totalRate.ToString("ulc");
                        string methodName = (totalRate > 0) ? quote.Name + ": " + formattedRate : quote.Name;
                        localQuotes.Add(new LocalShipRateQuote(quote.ShipMethodId, methodName, formattedRate));
                    }
                    _SavedShipRates[shipment.BasketShipmentId] = localQuotes;
                }
                ShipMethodList.DataSource = localQuotes;
                ShipMethodList.DataBind();
                if (localQuotes.Count == 0)
                {
                    //NO RATES AVAILABLE, WE HAVE TO DISABLE CHECKOUT
                    ShipMethodList.Visible = false;
                    _ShipMethodError = true;
                }
                else
                {
                    //MAKE SURE WE HAVE THE CORRECT ITEM SELECTED
                    int shipMethodId = AlwaysConvert.ToInt(Request.Form[ShipMethodList.UniqueID]);
                    ListItem selected = ShipMethodList.Items.FindByValue(shipMethodId.ToString());
                    if (selected != null) selected.Selected = true;
                }
            }
        }
    }

    private void BindPaymentMethodForms()
    {
        //CHECK ORDER TOTAL
        Basket basket = Token.Instance.User.Basket;
        LSDecimal orderTotal = basket.Items.TotalPrice();
        HiddenField hiddenOrderTotal;

        if (phPaymentForms.Controls.Count > 0)
        {
            hiddenOrderTotal = phPaymentForms.FindControl("OT") as HiddenField;
            if (hiddenOrderTotal != null)
            {
                //DO NOT CONTINUE TO PROCESS METHOD IF ORDER TOTAL HAS NOT CHANGED
                //BETWEEN ZERO AND NON ZERO
                LSDecimal savedOrderTotal = AlwaysConvert.ToDecimal(hiddenOrderTotal.Value);
                if ((orderTotal == 0 && savedOrderTotal == 0) || (orderTotal != 0 && savedOrderTotal != 0))
                    //PAYMENT METHODS WOULD NOT CHANGE, EXIT METHOD
                    return;
            }
            //RESET PAYMENT FORMS
            phPaymentForms.Controls.Clear();
        }

        //CHECK FOR TERMS AND CONDITIONS
        string tcText = Token.Instance.Store.Settings.CheckoutTermsAndConditions;
        if (tcText.Length > 0)
        {
            TermsAndConditionsText.Text = tcText;
            phTermsAndConditions.Visible = true;
        }

        if (orderTotal > 0)
        {
            List<DictionaryEntry> paymentMethods = new List<DictionaryEntry>();
            //ADD PAYMENT FORMS
            bool creditCardAdded = false;
            PaymentMethodCollection availablePaymentMethods = StoreDataHelper.GetPaymentMethods(Token.Instance.UserId);
            foreach (PaymentMethod method in availablePaymentMethods)
            {
                switch (method.PaymentInstrument)
                {
                    case PaymentInstrument.AmericanExpress:
                    case PaymentInstrument.Discover:
                    case PaymentInstrument.JCB:
                    case PaymentInstrument.MasterCard:
                    case PaymentInstrument.Visa:
                    case PaymentInstrument.DinersClub:
                    case PaymentInstrument.Maestro:
                    case PaymentInstrument.SwitchSolo:
                    case PaymentInstrument.VisaDebit:
                        if (!creditCardAdded)
                        {
                            paymentMethods.Insert(0, new DictionaryEntry(0, "Credit/Debit Card"));
                            creditCardAdded = true;
                        }
                        break;
                    case PaymentInstrument.Check:
                        paymentMethods.Add(new DictionaryEntry(method.PaymentMethodId, method.Name));
                        break;
                    case PaymentInstrument.PurchaseOrder:
                        paymentMethods.Add(new DictionaryEntry(method.PaymentMethodId, method.Name));
                        break;
                    case PaymentInstrument.PayPal:
                        paymentMethods.Add(new DictionaryEntry(method.PaymentMethodId, method.Name));
                        break;
                    case PaymentInstrument.Mail:
                        paymentMethods.Add(new DictionaryEntry(method.PaymentMethodId, method.Name));
                        break;
                    case PaymentInstrument.PhoneCall:
                        paymentMethods.Add(new DictionaryEntry(method.PaymentMethodId, method.Name));
                        break;
                    default:
                        //types not supported
                        break;
                }
            }

            if (StoreDataHelper.HasGiftCertificates())
            {
                paymentMethods.Add(new DictionaryEntry(-1, "Gift Certificate"));
            }

            //BIND THE RADIO LIST FOR PAYMENT METHOD SELECTION
            PaymentMethodList.DataSource = paymentMethods;
            PaymentMethodList.DataBind();

            //CONTINUE IF PAYMENT METHODS ARE AVAILABLE
            if (paymentMethods.Count > 0)
            {
                //MAKE SURE THE CORRECT PAYMENT METHOD IS SELECTED
                int paymentMethodId = AlwaysConvert.ToInt(Request.Form[PaymentMethodList.UniqueID]);
                ListItem selectedListItem = PaymentMethodList.Items.FindByValue(paymentMethodId.ToString());
                if (selectedListItem != null)
                {
                    PaymentMethodList.SelectedIndex = PaymentMethodList.Items.IndexOf(selectedListItem);
                }
                else PaymentMethodList.SelectedIndex = 0;

                //GET THE CURRENTLY SELECTED METHOD
                paymentMethodId = AlwaysConvert.ToInt(PaymentMethodList.SelectedValue);
                if (paymentMethodId == 0)
                {
                    ASP.CreditCardPaymentForm cardPaymentForm = new ASP.CreditCardPaymentForm();
                    cardPaymentForm.CheckingOut += new CheckingOutEventHandler(CheckingOut);
                    cardPaymentForm.CheckedOut += new CheckedOutEventHandler(CheckedOut);
                    cardPaymentForm.ValidationGroup = "OPC";
                    cardPaymentForm.ValidationSummaryVisible = false;
                    phPaymentForms.Controls.Add(cardPaymentForm);
                }
                else if (paymentMethodId == -1)
                {
                    ASP.GiftCertificatePaymentForm gcForm = new ASP.GiftCertificatePaymentForm();
                    gcForm.CheckingOut += new CheckingOutEventHandler(CheckingOut);
                    gcForm.CheckedOut += new CheckedOutEventHandler(CheckedOut);
                    gcForm.ValidationGroup = "OPC";
                    gcForm.ValidationSummaryVisible = false;
                    phPaymentForms.Controls.Add(gcForm);
                }
                else
                {
                    //DISPLAY FORM FOR SPECIFIC METHOD
                    PaymentMethod selectedMethod = availablePaymentMethods[availablePaymentMethods.IndexOf(paymentMethodId)];
                    switch (selectedMethod.PaymentInstrument)
                    {
                        case PaymentInstrument.Check:
                            ASP.CheckPaymentForm checkForm = new ASP.CheckPaymentForm();
                            checkForm.PaymentMethodId = selectedMethod.PaymentMethodId;
                            checkForm.CheckingOut += new CheckingOutEventHandler(CheckingOut);
                            checkForm.CheckedOut += new CheckedOutEventHandler(CheckedOut);
                            checkForm.ValidationGroup = "OPC";
                            checkForm.ValidationSummaryVisible = false;
                            phPaymentForms.Controls.Add(checkForm);
                            break;
                        case PaymentInstrument.PurchaseOrder:
                            ASP.PurchaseOrderPaymentForm poForm = new ASP.PurchaseOrderPaymentForm();
                            poForm.PaymentMethodId = selectedMethod.PaymentMethodId;
                            poForm.CheckingOut += new CheckingOutEventHandler(CheckingOut);
                            poForm.CheckedOut += new CheckedOutEventHandler(CheckedOut);
                            poForm.ValidationGroup = "OPC";
                            poForm.ValidationSummaryVisible = false;
                            phPaymentForms.Controls.Add(poForm);
                            break;
                        case PaymentInstrument.PayPal:
                            ASP.PayPalPaymentForm paypalForm = new ASP.PayPalPaymentForm();
                            paypalForm.PaymentMethodId = selectedMethod.PaymentMethodId;
                            paypalForm.CheckingOut += new CheckingOutEventHandler(CheckingOut);
                            paypalForm.CheckedOut += new CheckedOutEventHandler(CheckedOut);
                            paypalForm.ValidationGroup = "OPC";
                            paypalForm.ValidationSummaryVisible = false;
                            phPaymentForms.Controls.Add(paypalForm);
                            break;
                        case PaymentInstrument.Mail:
                            ASP.MailPaymentForm mailForm = new ASP.MailPaymentForm();
                            mailForm.PaymentMethodId = selectedMethod.PaymentMethodId;
                            mailForm.CheckingOut += new CheckingOutEventHandler(CheckingOut);
                            mailForm.CheckedOut += new CheckedOutEventHandler(CheckedOut);
                            mailForm.ValidationGroup = "OPC";
                            mailForm.ValidationSummaryVisible = false;
                            phPaymentForms.Controls.Add(mailForm);
                            break;
                        case PaymentInstrument.PhoneCall:
                            ASP.PhoneCallPaymentForm phoneCallForm = new ASP.PhoneCallPaymentForm();
                            phoneCallForm.PaymentMethodId = selectedMethod.PaymentMethodId;
                            phoneCallForm.CheckingOut += new CheckingOutEventHandler(CheckingOut);
                            phoneCallForm.CheckedOut += new CheckedOutEventHandler(CheckedOut);
                            phoneCallForm.ValidationGroup = "OPC";
                            phoneCallForm.ValidationSummaryVisible = false;
                            phPaymentForms.Controls.Add(phoneCallForm);
                            break;
                        default:
                            //types not supported
                            break;
                    }
                }
            }
        }
        else
        {
            ASP.ZeroValuePaymentForm freeForm = new ASP.ZeroValuePaymentForm();
            freeForm.CheckingOut += new CheckingOutEventHandler(CheckingOut);
            freeForm.CheckedOut += new CheckedOutEventHandler(CheckedOut);
            freeForm.ValidationGroup = "OPC";
            freeForm.ValidationSummaryVisible = false;
            phPaymentForms.Controls.Add(freeForm);
        }

        //STORE THE VALUE OF THE BASKET THAT THE PAYMENT FORMS WERE BOUND TO
        hiddenOrderTotal = new HiddenField();
        hiddenOrderTotal.ID = "OT";
        hiddenOrderTotal.Value = orderTotal.ToString();
        phPaymentForms.Controls.Add(hiddenOrderTotal);

        //WE DO NOT NEED THE PAYMENT SELECTION LIST IF THERE IS NOT MORE THAN ONE
        //AVAILABLE TYPE OF PAYMENT
        tdPaymentMethodList.Visible = (PaymentMethodList.Items.Count > 1) && (orderTotal > 0);
    }

    void CheckedOut(object sender, CheckedOutEventArgs e)
    {
        CheckoutResponse response = e.CheckoutResponse;
        if (response.Success)
        {
            bool anonCheckout = false;
            if (trAccount.Visible)
            {
                //MIGRATE THE USER
                User oldUser = Token.Instance.User;
                string newUserName = oldUser.PrimaryAddress.Email;
                string newEmail = oldUser.PrimaryAddress.Email;
                string newPassword = NewUserPassword.Text;
                if (!CreateAccountPanel.Visible)
                {
                    anonCheckout = true;
                    newUserName = "zz_anonymous_" + Guid.NewGuid().ToString("N") + "@domain.xyz";
                    newPassword = Guid.NewGuid().ToString("N");
                }
                MembershipCreateStatus createStatus;
                User newUser = UserDataSource.CreateUser(newUserName, newEmail, newPassword, string.Empty, string.Empty, true, Token.Instance.User.AffiliateId, out createStatus);
                //IF THE CREATE FAILS, IGNORE AND CONTINUE CREATING THE ORDER
                if (createStatus == MembershipCreateStatus.Success)
                {
                    if (anonCheckout)
                    {
                        // CHANGE THE NAME AND EMAIL TO SOMETHING MORE FRIENDLY THAN GUID
                        newUser.UserName = "zz_anonymous_" + newUser.UserId.ToString() + "@domain.xyz";
                        newUser.Save();
                    }
                    User.Migrate(oldUser, newUser, true);
                    Token.Instance.InitUserContext(newUser);
                    FormsAuthentication.SetAuthCookie(newUser.UserName, false);
                }
            }

            //UPDATE COMMUNICATION PREFERENCES FOR REGISTERED USERS ONLY
            User currentUser = Token.Instance.User;
            if (trEmailLists.Visible)
            {
                string email = currentUser.Email;
                int listIndex = 0;
                foreach (DataListItem item in EmailLists.Items)
                {
                    EmailList list = _PublicEmailLists[listIndex];
                    CheckBox selected = (CheckBox)item.FindControl("Selected");
                    if (selected != null)
                    {
                        if (selected.Checked) list.ProcessSignupRequest(email);
                        else list.RemoveMember(email);
                    }
                    else list.RemoveMember(email);
                    listIndex++;
                }
            }
        }
        else
        {
            CheckoutMessagePanel.Visible = true;
            CheckoutMessage.Text = string.Join("<br /><br />", response.WarningMessages.ToArray());
        }
    }

    void CheckingOut(object sender, CheckingOutEventArgs e)
    {
        if (this.DisableBots)
        {
            // BOT CHECKING IS ENABLED, PERFORM VALIDATION
            AjaxControlToolkit.NoBotState state = new AjaxControlToolkit.NoBotState();
            if (!OPCNotBot.IsValid(out state))
            {
                // BOT STATE IS NOT VALID, SHOW ERROR MESSAGE
                BotMessagePanel.Visible = true;
                CheckoutPanel.Visible = false;
                string msg = "Your activity has been detected as a <strong>Bot</strong> or <strong>Spider</strong> by our system. If you believe you are seeing this message in error you can <a href=\"{0}\">Retry</a> the checkout after <strong>one minute</strong>.";
                msg = string.Format(msg, Page.ResolveClientUrl(NavigationHelper.GetCheckoutUrl()));
                BotMessage.Text = msg;
                e.Cancel = true;
                return;
            }
        }

        // BOT VALIDATION PASSED, OR THE CHECK WAS NOT ENABLED
        BotMessagePanel.Visible = false;
        CheckoutPanel.Visible = true;

        //MAKE SURE WE HAVE VALIDATED THIS FORM
        Page.Validate("OPC");
        if(trAccount.Visible) Page.Validate("CreateAccount");
        //IF ANYTHING WAS INVALID CANCEL CHECKOUT
        if (!Page.IsValid) e.Cancel = true;
        //HANDLE CUSTOM VALIDATION
        if (trAccount.Visible && CreateAccountPanel.Visible)
        {
            if (NewUserPassword.Text.Length == 0)
            {
                e.Cancel = true;
                RequiredFieldValidator passwordRequired = new RequiredFieldValidator();
                passwordRequired.ControlToValidate = "NewUserPassword";
                passwordRequired.IsValid = false;
                passwordRequired.Text = "*";
                passwordRequired.ErrorMessage = "You must provide a password to create an account.";
                passwordRequired.SetFocusOnError = false;
                passwordRequired.ValidationGroup = "CreateAccount";
                PasswordValidatorPanel.Controls.Add(passwordRequired);
            }
            else
            {
                //VALIDATE PASSWORD POLICY
                CustomerPasswordPolicy policy = new CustomerPasswordPolicy();

                PasswordTestResult result = policy.TestPasswordWithFeedback(NewUserPassword.Text);
                if ((result & PasswordTestResult.Success) != PasswordTestResult.Success)
                {
                    e.Cancel = true;
                    //Your password did not meet the following minimum requirements
                    if ((result & PasswordTestResult.PasswordTooShort) == PasswordTestResult.PasswordTooShort) AddPasswordValidator("Password length must be at least " + policy.MinLength.ToString() + " characters.");
                    if ((result & PasswordTestResult.RequireLower) == PasswordTestResult.RequireLower) AddPasswordValidator("Password must contain at least one lowercase letter.<br>");
                    if ((result & PasswordTestResult.RequireUpper) == PasswordTestResult.RequireUpper) AddPasswordValidator("Password must contain at least one uppercase letter.<br> ");
                    if ((result & PasswordTestResult.RequireNonAlpha) == PasswordTestResult.RequireNonAlpha) AddPasswordValidator("Password must contain at least one non-letter.<br> ");
                    if ((result & PasswordTestResult.RequireNumber) == PasswordTestResult.RequireNumber) AddPasswordValidator("Password must contain at least one number.<br> ");
                    if ((result & PasswordTestResult.RequireSymbol) == PasswordTestResult.RequireSymbol) AddPasswordValidator("Password must contain at least one symbol.<br> ");
                    
                }
            }
        }

        //to enable scripted load testing, we need to allow recalculation of the basket hash
        if (!string.IsNullOrEmpty(Request.Form["RecalcHash"]))
        {
            RecalculateBasket(true);
            _CurrentBasketHash = Token.Instance.User.Basket.GetContentHash(OrderItemType.Product);
            _SavedBasketHash = _CurrentBasketHash;
        }

        //Make sure basket hasn't changed during checkout
        if (_CurrentBasketHash != _SavedBasketHash)
        {
            e.Cancel = true;
            CheckoutMessagePanel.Visible = true;
            CheckoutMessage.Text = "Your order has not been completed and payment was not processed.<br /><br />Your cart appears to have been modified during checkout.  Please verify the contents of your order and resubmit your payment.";
            RecalculateBasket(true);
            return;
        }

        //Make sure that a valid billing address is set
        User user = Token.Instance.User;
        if (user.PrimaryAddress == null || !user.PrimaryAddress.IsValid)
        {
            e.Cancel = true;
            CheckoutMessagePanel.Visible = true;
            CheckoutMessage.Text = "Your order has not been completed and payment was not processed.<br /><br />The billing address is invalid.  Please correct the address and resubmit your payment.";
            return;
        }

        if (!ValidateShipStatus(true))
        {
            e.Cancel = true;
            return;
        }

        //MAKE SURE THE SHIPPING MESSAGE IS SET
        if ((!e.Cancel) && (_EnableShipMessage))
        {
            Basket basket = Token.Instance.User.Basket;
            int shipmentIndex = 0;
            foreach (RepeaterItem item in ShipmentList.Items)
            {
                BasketShipment shipment = basket.Shipments[shipmentIndex];
                TextBox shipMessage = (TextBox)item.FindControl("ShipMessage");
                if (shipMessage != null)
                {
                    shipment.ShipMessage = StringHelper.Truncate(shipMessage.Text, 200);
                    shipment.Save();
                }
                shipmentIndex++;
            }
        }
    }

    private void AddPasswordValidator(String message)
    {
        CustomValidator validator = new CustomValidator();
        validator.ControlToValidate = "NewUserPassword";
        validator.ValidationGroup = "CreateAccount";
        validator.ErrorMessage = message;
        validator.SetFocusOnError = false;
        validator.Text = "*";
        validator.IsValid = false;
        PasswordValidatorPanel.Controls.Add(validator);        
    }

    protected void ValidateTC(object source, ServerValidateEventArgs args)
    {
        args.IsValid = AcceptTC.Checked;
    }

    private bool SaveBillingAddress()
    {
        string billToProvince;
        if (ValidateProvince(BillToCountry, BillToProvince, BillToProvinceList, out billToProvince))
        {
            User user = Token.Instance.User;
            Address billingAddress = user.PrimaryAddress;
            billingAddress.FirstName = StringHelper.StripHtml(BillToFirstName.Text);
            billingAddress.LastName = StringHelper.StripHtml(BillToLastName.Text);
            billingAddress.Company = StringHelper.StripHtml(BillToCompany.Text);
            billingAddress.Address1 = StringHelper.StripHtml(BillToAddress1.Text);
            billingAddress.Address2 = StringHelper.StripHtml(BillToAddress2.Text);
            billingAddress.City = StringHelper.StripHtml(BillToCity.Text);
            billingAddress.Province = billToProvince;
            billingAddress.PostalCode = StringHelper.StripHtml(BillToPostalCode.Text);
            billingAddress.CountryCode = BillToCountry.SelectedValue;
            billingAddress.Residence = (BillToAddressType.SelectedIndex == 0);
            billingAddress.Phone = StringHelper.StripHtml(BillToPhone.Text);
            if (trEmail.Visible) billingAddress.Email = StringHelper.StripHtml(BillToEmail.Text);
            billingAddress.Save();
            _BillToProvinceError = false;
            return true;
        }
        else
        {
            BillToProvinceInvalid.IsValid = false;
            _BillToProvinceError = true;
            UpdateProvinces(BillToCountry.SelectedValue, BillToProvince, BillToProvinceList, BillToPostalCodeRequired);
            return false;
        }
    }

    private Address GetShippingAddress()
    {
        //RETURN THE SELECTED SHIPPING ADDRESS FOR CHECKOUT
        User user = Token.Instance.User;
        if (this.ShipToAddressId > 0)
        {
            //A SHIPPING ADDRESS IS SPECIFIED, LOOK FOR IT IN USER ADDRESS COLLECTION
            int index = user.Addresses.IndexOf(ShipToAddressId);
            if (index > -1) return user.Addresses[index];
            //NOT FOUND IN USER ADDRESS COLLECTION, TRY TO LOAD FROM DB?
            Address shipAddress = AddressDataSource.Load(this.ShipToAddressId, false);
            if (shipAddress != null) return shipAddress;
        }
        //WE ARE USING THE BILLING ADDRESS AS SHIPPING ADDRESS
        this.ShipToAddressId = 0;
        return user.PrimaryAddress;
    }

    private bool SaveShippingAddress()
    {
        //can we save billing address
        User user = Token.Instance.User;
        Address shippingAddress = null;
        int index = user.Addresses.IndexOf(ShipToAddressId);
        if (index > -1) shippingAddress = user.Addresses[index];
        else
        {
            shippingAddress = new Address();
            shippingAddress.UserId = user.UserId;
            user.Addresses.Add(shippingAddress);
        }
        string shipToProvince;
        if (ValidateProvince(ShipToCountry, ShipToProvince, ShipToProvinceList, out shipToProvince))
        {
            //UPDATE THE FIELDS FROM FORM
            shippingAddress.FirstName = StringHelper.StripHtml(ShipToFirstName.Text);
            shippingAddress.LastName = StringHelper.StripHtml(ShipToLastName.Text);
            shippingAddress.Company = StringHelper.StripHtml(ShipToCompany.Text);
            shippingAddress.Address1 = StringHelper.StripHtml(ShipToAddress1.Text);
            shippingAddress.Address2 = StringHelper.StripHtml(ShipToAddress2.Text);
            shippingAddress.City = StringHelper.StripHtml(ShipToCity.Text);
            shippingAddress.Province = shipToProvince;
            shippingAddress.PostalCode = StringHelper.StripHtml(ShipToPostalCode.Text);
            shippingAddress.CountryCode = ShipToCountry.SelectedValue;
            shippingAddress.Residence = (AlwaysConvert.ToInt(ShipToAddressType.SelectedValue) == 1);
            shippingAddress.Phone = StringHelper.StripHtml(ShipToPhone.Text);
            shippingAddress.Save();
            this.ShipToAddressId = shippingAddress.AddressId;
            _ShipToProvinceError = false;
            return true;
        }
        else
        {
            ShipToProvinceInvalid.IsValid = false;
            _ShipToProvinceError = true;
            UpdateProvinces(ShipToCountry.SelectedValue, ShipToProvince, ShipToProvinceList, ShipToPostalCodeRequired);
            return false;
        }
    }

    protected void ContinueButton_Click(object sender, EventArgs e)
    {
        //Page.Validate();
        if (Page.IsValid)
        {
            if (this.EditBillingAddress)
            {
                if (!SaveBillingAddress()) return;
            }
            if (this.EditShippingAddress && this.HasShippableProducts && UseShippingAddress.Checked)
            {
                if (!SaveShippingAddress()) return;
            }
            this.EditBillingAddress = false;
            this.EditShippingAddress = false;
            TogglePanels();
            RecalculateBasket();
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (CheckoutPanel.Visible)
        {
            User user = Token.Instance.User;
            //CHECK IF VIEW BILLING ADDRESS IS VISIBLE (EITHER BY TOGGLE, OR BY PROCESSING SHIP RATE CLICK)
            if (ViewBillingPanel.Visible)
            {
                //DISPLAY SELECTED BILLING ADDRESS
                FormattedBillingAddress.Text = user.PrimaryAddress.ToString(true);
            }

            if (ViewShippingPanel.Visible)
            {
                //DISPLAY SELECTED SHIPPING ADDRESS
                int index = user.Addresses.IndexOf(ShipToAddressId);
                if (index > -1)
                {
                    FormattedShippingAddress.Text = user.Addresses[index].ToString(true);
                }
                else FormattedShippingAddress.Text = FormattedBillingAddress.Text;
            }

            //SEE IF WE NEED TO ALWAYS HIDE CERTAIN SIDEBAR ELEMENTS
            if (!this.EnableGiftWrap) GiftWrapPanel.Visible = false;
            if (!this.EnableMultiShipTo) ShipMultiPanel.Visible = false;
            //SHOW THE ORDER ITEM GRID
            BasketItemCollection basketItems = new BasketItemCollection();
            //UPDATE THE TAX DISPLAY ON BASKET SUMMARY
            TaxInvoiceDisplay taxDisplayMode = TaxHelper.GetEffectiveInvoiceDisplay(user);
            // IF WE ARE USING SUMMARY ONLY TAXES, WE SHOULD ONLY SHOW IT ONCE BILLING ADDRESS IS PROVIDED
            BasketTotalSummary1.ShowTaxes = (!this.EditBillingAddress | taxDisplayMode != TaxInvoiceDisplay.Summary);
            BasketGrid.Columns[3].Visible = TaxHelper.ShowTaxColumn && (!this.EditBillingAddress || taxDisplayMode == TaxInvoiceDisplay.Included);
            BasketGrid.Columns[3].HeaderText = TaxHelper.TaxColumnHeader;
            //REMOVE VAT ENTRIES
            bool showTaxLineItems = (taxDisplayMode == TaxInvoiceDisplay.LineItem);
            foreach (BasketItem bi in user.Basket.Items)
            {
                if (bi.OrderItemType == OrderItemType.Product && bi.IsChildItem)
                {
                    BasketItem parentItem = bi.GetParentItem(true);
                    if (parentItem.Product != null && parentItem.Product.Kit.ItemizeDisplay)
                    {
                        basketItems.Add(bi);
                    }
                }
                else if (bi.OrderItemType != OrderItemType.Tax || showTaxLineItems)
                {
                    basketItems.Add(bi);
                }
            }
            basketItems.Sort(new BasketItemComparer());
            trItemGrid.Visible = true;
            BasketGrid.DataSource = basketItems;
            BasketGrid.DataBind();
            //PERSIST VALUES TO FORM
            SaveCustomViewState();
        }
    }

    private void ValidateOrderMinMaxAmounts()
    {
        // IF THE ORDER AMOUNT DOES NOT FALL IN VALID RANGE SPECIFIED BY THE MERCHENT
        OrderItemType[] args = new OrderItemType[] { OrderItemType.Charge, 
                                                    OrderItemType.Coupon, OrderItemType.Credit, OrderItemType.Discount, 
                                                    OrderItemType.GiftCertificate, OrderItemType.GiftWrap, OrderItemType.Handling, 
                                                    OrderItemType.Product, OrderItemType.Shipping, OrderItemType.Tax };
        LSDecimal orderTotal = Token.Instance.User.Basket.Items.TotalPrice(args);

        StoreSettingCollection settings = Token.Instance.Store.Settings;
        decimal minOrderAmount = settings.OrderMinimumAmount;
        decimal maxOrderAmount = settings.OrderMaximumAmount;
        if ((minOrderAmount > orderTotal) || (maxOrderAmount > 0 && maxOrderAmount < orderTotal))
        {
            // REDIRECT TO BASKET PAGE
            Response.Redirect("~/Basket.aspx");
        }
        else
        {
            CheckoutPanel.Visible = true;
        }
    }

    private bool ValidateShipStatus(bool paymentForm)
    {
        //Make sure that
        //a) Each shipment has a valid shipping address set
        //b) Each shipment has a shipping method set
        bool isValid = true;
        Basket basket = Token.Instance.User.Basket;
        foreach (BasketShipment shipment in basket.Shipments)
        {
            bool invalidShippingAddress = (shipment.Address == null || !shipment.Address.IsValid);
            if (invalidShippingAddress || shipment.ShipMethodId < 1)
            {
                isValid = false;
                CheckoutMessagePanel.Visible = true;
                if (invalidShippingAddress)
                {
                    if (paymentForm)
                        CheckoutMessage.Text = "Your order has not been completed and payment was not processed.<br /><br />The shipping address is invalid.  Please correct the address and resubmit your payment.";
                    else
                        CheckoutMessage.Text = "The shipping address is invalid.  Please correct the address and resubmit your payment.";
                }
                else
                {
                    if (paymentForm) CheckoutMessage.Text = "Your order has not been completed and payment was not processed.<br /><br />Either a selected shipping method is no longer valid or a shipment has been modified during checkout.  Please verify the contents of your order and resubmit your payment.";
                    else
                        CheckoutMessage.Text = "Either a selected shipping method is no longer valid or a shipment has been modified during checkout.  Please verify the contents of your order and proceed again.";
                    RebindShipments();
                    RecalculateBasket(false);
                }
                break;
            }
        }
        return isValid;
    }

    private void RebindShipments()
    {
        _SavedShipRates = new Dictionary<int, List<LocalShipRateQuote>>();
        InitializeBasket();
        ShipmentList.DataSource = Token.Instance.User.Basket.Shipments;
        ShipmentList.DataBind();
    }

    public class LocalShipRateQuote
    {
        private int _ShipMethodId;
        private string _Name;
        private string _FormattedRate;
        public int ShipMethodId { get { return _ShipMethodId; } }
        public string Name { get { return _Name; } }
        public string FormattedRate { get { return _FormattedRate; } }
        public LocalShipRateQuote(int shipMethodId, string name, string formattedRate)
        {
            _ShipMethodId = shipMethodId;
            _Name = name;
            _FormattedRate = formattedRate;
        }
        public LocalShipRateQuote(string encodedData)
        {
            string[] values = StringHelper.Split(encodedData, "~~");
            _ShipMethodId = AlwaysConvert.ToInt(values[0]);
            _Name = values[1];
            _FormattedRate = values[2];
        }
        public string Encode()
        {
            return _ShipMethodId.ToString() + "~~" + _Name + "~~" + _FormattedRate;
        }
    }

    protected string GetShipmentNumber(object dataItem)
    {
        BasketItem basketItem = (BasketItem)dataItem;
        if (basketItem.BasketShipment != null)
        {
            return basketItem.BasketShipment.ShipmentNumber.ToString();
        }
        return "-";
    }

    protected void LoginLink2_Click(object sender, EventArgs e)
    {
        User user = Token.Instance.User;
        user.Addresses.DeleteAll();
        user.PrimaryAddressId = 0;
        user.Save();
        Response.Redirect("~/Login.aspx?ReturnUrl=" + Server.UrlEncode(Request.Url.AbsolutePath));
    }

    protected void RefreshRatesButton_Click(object sender, EventArgs e)
    {
        RebindShipments();
        RecalculateBasket(false);
    }
}