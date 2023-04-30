using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Common;
using MakerShop.Orders;
using MakerShop.Payments;
using MakerShop.Taxes;
using MakerShop.Utility;
using MakerShop.Users;
using MakerShop.Shipping;

public partial class ConLib_PayMyOrderPage : System.Web.UI.UserControl
{
    private int _OrderId;
    private Order _Order;
    private Order _NoSaveOrder;
    private LSDecimal _Balance;
    private LSDecimal _TotalPayments;
    private const int FailedPaymentLimit = 3;
	CountryCollection _Countries;

    protected Order Order { get { return _Order; } }
    
    private CountryCollection Countries
    {
        get
        {
            if (_Countries == null) InitializeCountries();
            return _Countries;
        }
    }

    private bool OrderHasSubscriptionPayments()
    {
        foreach (Payment payment in _Order.Payments)
        {
            if (payment.SubscriptionId != 0) return true;
        }
        return false;
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        //LOAD THE ORDER
        _OrderId = PageHelper.GetOrderId();
        _Order = OrderDataSource.Load(_OrderId);
        if (_Order == null) Response.Redirect(NavigationHelper.GetStoreUrl(this.Page, "Members/MyAccount.aspx"));
        if (_Order.UserId != Token.Instance.UserId) Response.Redirect(NavigationHelper.GetStoreUrl(this.Page, "Members/MyAccount.aspx"));
        if (!_Order.OrderStatus.IsValid || OrderHasSubscriptionPayments()) Response.Redirect(NavigationHelper.GetStoreUrl(this.Page, "Members/MyOrder.aspx?OrderId=" + _OrderId.ToString()));

        //WE NEED TO ALTER ITEMS FOR DISPLAY REASONS, BUT WE DO 
        //NOT WANT TO SAVE THE CHANGES
        _NoSaveOrder = OrderDataSource.Load(_OrderId, false);

        //CANCEL FAILED AUTHORIZATIONS
        PaymentCollection payments = _Order.Payments;
        for (int i = 0; i < payments.Count; i++)
        {
            Payment p = payments[i];
            if (p.PaymentStatus == PaymentStatus.AuthorizationFailed)
            {
                p.Void();
            }
        }

        //CACULATE BALANCE OWED AND REDIRECT IF NONE DUE
        _Balance = _Order.GetCustomerBalance();
        if (_Balance <= 0) Response.Redirect( NavigationHelper.GetStoreUrl(this.Page, "Members/MyOrder.aspx?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString()));
        _TotalPayments = _Order.TotalCharges - _Balance;

        //DO NOT SCROLL TO VALIDATION SUMMARY
        PageHelper.DisableValidationScrolling(this.Page);

        //INITIALIZE BILLING ADDRESS
        InitializeBillingAddress();

        //BIND ORDER DETAILS IN THE SIDEBAR
        BindOrder();

        if (HasTooManyFailedPayments())
        {
            //DISABLE THE PAYMENT PANEL FOR TOO MANY FAILED ATTEMPTS
            TooManyTriesPanel.Visible = true;
            PaymentPanel.Visible = false;
        }
        else
        {
            //SHOW THE PAYMENT FORMS
            ShowFailedPaymentPanel();
            BindPaymentMethodForms();
        }

        //SHOW THE PAYMENT HISTORY
        PaymentGrid.DataSource = payments;
        PaymentGrid.DataBind();

        //SHOW THE ORDER ITEMS
        BasketGrid.Columns[2].Visible = TaxHelper.ShowTaxColumn;
        BasketGrid.Columns[2].HeaderText = TaxHelper.TaxColumnHeader;
        BasketGrid.DataSource = GetOrderProducts();
        BasketGrid.DataBind();
    }

    protected OrderItemCollection GetOrderProducts()
    {
        OrderItemType[] displayTypes = { OrderItemType.Product, OrderItemType.Discount, OrderItemType.Coupon, OrderItemType.Shipping, OrderItemType.Handling, OrderItemType.Tax, OrderItemType.GiftWrap };
        OrderItemCollection orderProducts = new OrderItemCollection();
        foreach (OrderItem item in _NoSaveOrder.Items)
        {
            if (Array.IndexOf(displayTypes, item.OrderItemType) > -1)
            {
                if (item.ParentItemId == 0)
                {
                    orderProducts.Add(item);
                }
                else if ((item.ParentItemId > 0) && (item.OrderItemType != OrderItemType.Tax) && (item.Price != 0))
                {
                    //WE NEED TO ADD THE PRICE OF THE ITEM INTO THE PARENT
                    int index = orderProducts.IndexOf(item.ParentItemId);
                    if (index > -1) orderProducts[index].Price += (item.ExtendedPrice / orderProducts[index].Quantity);
                }
            }
        }
        //SHOW TAXES IF SPECIFIED
        TaxInvoiceDisplay displayMode = TaxHelper.InvoiceDisplay;
        if (displayMode == TaxInvoiceDisplay.LineItem || displayMode == TaxInvoiceDisplay.LineItemRegistered)
        {
            foreach (OrderItem item in _NoSaveOrder.Items)
            {
                if ((item.OrderItemType == OrderItemType.Tax)
                    && (orderProducts.IndexOf(item.ParentItemId) > -1))
                    orderProducts.Add(item);
            }
        }
        orderProducts.Sort(new OrderItemComparer());
        return orderProducts;
    }

    /// <summary>
    /// Determines whether the last X payments in a row have "failed"
    /// </summary>
    /// <returns>True if the last X payments in a row have "failed"</returns>
    private bool HasTooManyFailedPayments()
    {
        int paymentCount = _Order.Payments.Count;
        if (paymentCount < FailedPaymentLimit) return false;
        int failedPaymentCount = 0;
        for (int i = paymentCount - 1; i >= 0; i--)
        {
            Payment p = _Order.Payments[i];
            if (IsFailedPayment(p))
            {
                failedPaymentCount++;
                if (failedPaymentCount == FailedPaymentLimit) return true;
            }
            else return false;
        }
        return false;
    }

    /// <summary>
    /// Determines whether this payment has "failed"
    /// </summary>
    /// <param name="p">Payment to check</param>
    /// <returns>True if the payment authorization failed</returns>
    private bool IsFailedPayment(Payment p)
    {
        Transaction authTx = GetLastAuthorization(p);
        return (authTx != null && authTx.TransactionStatus == TransactionStatus.Failed);
    }

    private void ShowFailedPaymentPanel()
    {
        Payment lastPayment = (Payment)_Order.Payments.LastPayment;
        if ((lastPayment != null) && (lastPayment.PaymentStatus == PaymentStatus.Void))
        {
            Transaction authTx = GetLastAuthorization(lastPayment);
            if (authTx != null && authTx.TransactionStatus == TransactionStatus.Failed)
            {
                PaymentFailedPanel.Visible = true;
                string failedReason = authTx.ResponseMessage;
                if (string.IsNullOrEmpty(failedReason)) failedReason = "Authorization failed";
                PaymentFailedReason.Text = failedReason;
            }
        }
        else PaymentFailedPanel.Visible = false;
    }

    private void BindOrder()
    {
        Caption.Text = string.Format(Caption.Text, _Order.OrderNumber);
        OrderNumber.Text = _Order.OrderNumber.ToString();
        OrderDate.Text = string.Format("{0:g}", _Order.OrderDate);
        OrderStatus.Text = _Order.OrderStatus.DisplayName;
        TotalCharges.Text = string.Format("{0:lc}", _Order.TotalCharges);
        TotalPayments.Text = string.Format("{0:lc}", _TotalPayments);
        Balance.Text = string.Format("{0:lc}", _Balance);
    }

    protected string GetPaymentName(object dataItem)
    {
        Payment p = (Payment)dataItem;
        if (!string.IsNullOrEmpty(p.ReferenceNumber)) return p.PaymentMethodName + " " + p.ReferenceNumber;
        return p.PaymentMethodName;
    }

    private Transaction GetLastAuthorization(Payment p)
    {
        for (int i = p.Transactions.Count - 1; i >= 0; i--)
        {
            Transaction t = (Transaction)p.Transactions[i];
            if (t.TransactionType == TransactionType.Authorize ||
                t.TransactionType == TransactionType.AuthorizeCapture ||
                t.TransactionType == TransactionType.AuthorizeRecurring) return t;
        }
        return null;
    }

    protected string GetPaymentStatus(object dataItem)
    {
        Payment p = (Payment)dataItem;
        if (p.PaymentStatus == PaymentStatus.Void) return "VOID";
        return StringHelper.SpaceName(p.PaymentStatus.ToString());
    }

    protected string GetPaymentStatusReason(object dataItem)
    {
        Payment p = (Payment)dataItem;
        string transactionStatus = string.Empty;
        if (p.PaymentStatus == PaymentStatus.Void)
        {
            Transaction authTx = GetLastAuthorization(p);
            if (authTx != null && authTx.TransactionStatus == TransactionStatus.Failed)
            {
                return authTx.ResponseMessage;
            }
        }
        return string.Empty;
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
        string[] countries = MakerShop.Stores.Store.GetCachedSettings().PostalCodeCountries.Split(",".ToCharArray());
        PostalCodeValidator.Enabled = (Array.IndexOf(countries, countryCode) > -1);
        //SEE WHETHER PROVINCE LIST IS DEFINED
        ProvinceCollection provinces = ProvinceDataSource.LoadForCountry(countryCode);
        if (provinces.Count > 0)
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
        }
        else
        {
            ProvinceText.Visible = true;
            ProvinceList.Visible = false;
            ProvinceList.Items.Clear();            
        }
        BillToProvinceRequired.Enabled = BillToProvinceList.Visible;
    }

    private void SelectProvince(TextBox ProvinceText, DropDownList ProvinceList, string defaultProvince, bool forceProvince)
    {
        if (ProvinceText.Visible)
        {
            ProvinceText.Text = defaultProvince;
        }
        else
        {
            string provinceValue = Request.Form[ProvinceList.UniqueID];
            if (forceProvince || string.IsNullOrEmpty(provinceValue)) provinceValue = defaultProvince;
            ListItem selectedProvince = ProvinceList.Items.FindByValue(provinceValue);
            if (selectedProvince != null) ProvinceList.SelectedIndex = ProvinceList.Items.IndexOf(selectedProvince);
        }
    }

    private void InitializeBillingAddress()
    {
        User user = Token.Instance.User;
        BillToFirstName.Text = _Order.BillToFirstName;
        BillToLastName.Text = _Order.BillToLastName;
        BillToCompany.Text = _Order.BillToCompany;
        BillToAddress1.Text = _Order.BillToAddress1;
        BillToAddress2.Text = _Order.BillToAddress2;
        BillToCity.Text = _Order.BillToCity;
        BillToPostalCode.Text = _Order.BillToPostalCode;
        //BillToAddressType.SelectedIndex = (_Order.BillToIsResidence ? 0 : 1);
        BillToPhone.Text = _Order.BillToPhone;
        //INITIALIZE BILLING COUNTRY AND PROVINCE
        BillToCountry.DataSource = this.Countries;
        BillToCountry.DataBind();
        if (_Order.BillToCountryCode.Length == 0) _Order.BillToCountryCode = Token.Instance.Store.DefaultWarehouse.CountryCode;
        SelectCountryAndProvince(BillToCountry, _Order.BillToCountryCode, false, BillToProvince, BillToProvinceList, BillToPostalCodeRequired, _Order.BillToProvince, false);
    }

    private void SaveBillingAddress()
    {
        // LOOK FOR SHIPMENTS THAT SHIP TO BILLING ADDRESSES
        List<OrderShipment> shipments = GetShipToBilling(_Order);

        // UPDATE THE BILLING ADDRESS
        _Order.BillToFirstName = StringHelper.StripHtml(BillToFirstName.Text);
        _Order.BillToLastName = StringHelper.StripHtml(BillToLastName.Text);
        _Order.BillToCountryCode = BillToCountry.SelectedValue;
        _Order.BillToCompany = StringHelper.StripHtml(BillToCompany.Text);
        _Order.BillToAddress1 = StringHelper.StripHtml(BillToAddress1.Text);
        _Order.BillToAddress2 = StringHelper.StripHtml(BillToAddress2.Text);
        _Order.BillToCity = StringHelper.StripHtml(BillToCity.Text);
        _Order.BillToProvince = (BillToProvince.Visible ? StringHelper.StripHtml(BillToProvince.Text) : BillToProvinceList.SelectedValue);
        _Order.BillToPostalCode = StringHelper.StripHtml(BillToPostalCode.Text);
        _Order.BillToPhone = StringHelper.StripHtml(BillToPhone.Text);

        // UPDATE SHIPPING ADDRESSES
        foreach (OrderShipment shipment in shipments)
        {
            shipment.ShipToFirstName = _Order.BillToFirstName;
            shipment.ShipToLastName = _Order.BillToLastName;
            shipment.ShipToCountryCode = _Order.BillToCountryCode;
            shipment.ShipToCompany = _Order.BillToCompany;
            shipment.ShipToAddress1 = _Order.BillToAddress1;
            shipment.ShipToAddress2 = _Order.BillToAddress2;
            shipment.ShipToCity = _Order.BillToCity;
            shipment.ShipToProvince = _Order.BillToProvince;
            shipment.ShipToPostalCode = _Order.BillToPostalCode;
            shipment.ShipToPhone = _Order.BillToPhone;
        }

        // SAVE ORDER
        _Order.Save();

        // UPDATE USER BILLING ADDRESS
        Address billingAddress = Token.Instance.User.PrimaryAddress;
        billingAddress.FirstName = _Order.BillToFirstName;
        billingAddress.LastName = _Order.BillToLastName;
        billingAddress.CountryCode = _Order.BillToCountryCode;
        billingAddress.Company = _Order.BillToCompany;
        billingAddress.Address1 = _Order.BillToAddress1;
        billingAddress.Address2 = _Order.BillToAddress2;
        billingAddress.City = _Order.BillToCity;
        billingAddress.Province = _Order.BillToProvince;
        billingAddress.PostalCode = _Order.BillToPostalCode;
        billingAddress.Phone = _Order.BillToPhone;
        billingAddress.Save();
    }

    private static List<OrderShipment> GetShipToBilling(Order order)
    {
        List<OrderShipment> shipments = new List<OrderShipment>();
        foreach (OrderShipment shipment in order.Shipments)
        {
            if (order.BillToFirstName != shipment.ShipToFirstName) continue;
            if (order.BillToLastName != shipment.ShipToLastName) continue;
            if (order.BillToCountryCode != shipment.ShipToCountryCode) continue;
            if (order.BillToCompany != shipment.ShipToCompany) continue;
            if (order.BillToAddress1 != shipment.ShipToAddress1) continue;
            if (order.BillToAddress2 != shipment.ShipToAddress2) continue;
            if (order.BillToCity != shipment.ShipToCity) continue;
            if (order.BillToProvince != shipment.ShipToProvince) continue;
            if (order.BillToPostalCode != shipment.ShipToPostalCode) continue;
            if (order.BillToPhone != shipment.ShipToPhone) continue;
            shipments.Add(shipment);
        }
        return shipments;
    }

    private void BindPaymentMethodForms()
    {
        //CHECK ORDER TOTAL
        Basket basket = Token.Instance.User.Basket;
        LSDecimal orderTotal = _Balance;

        //EXIT THIS METHOD IF WE HAVE ALREADY ADDED PAYMENT CONTROLS
        if (phPaymentForms.Controls.Count > 0) return;

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
                        paymentMethods.Insert(0, new DictionaryEntry(0, "Credit Card"));
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
                cardPaymentForm.ValidationGroup = "OPC";
                cardPaymentForm.ValidationSummaryVisible = false;
                cardPaymentForm.PaymentAmount = _Balance;
                cardPaymentForm.AllowAmountEntry = true;
                phPaymentForms.Controls.Add(cardPaymentForm);
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
                        checkForm.ValidationGroup = "OPC";
                        checkForm.ValidationSummaryVisible = false;
                        checkForm.PaymentAmount = _Balance;
                        checkForm.AllowAmountEntry = true;
                        phPaymentForms.Controls.Add(checkForm);
                        break;
                    case PaymentInstrument.PurchaseOrder:
                        ASP.PurchaseOrderPaymentForm poForm = new ASP.PurchaseOrderPaymentForm();
                        poForm.PaymentMethodId = selectedMethod.PaymentMethodId;
                        poForm.CheckingOut += new CheckingOutEventHandler(CheckingOut);
                        poForm.ValidationGroup = "OPC";
                        poForm.ValidationSummaryVisible = false;
                        poForm.PaymentAmount = _Balance;
                        phPaymentForms.Controls.Add(poForm);
                        break;
                    case PaymentInstrument.PayPal:
                        ASP.PayPalPaymentForm paypalForm = new ASP.PayPalPaymentForm();
                        paypalForm.PaymentMethodId = selectedMethod.PaymentMethodId;
                        paypalForm.CheckingOut += new CheckingOutEventHandler(CheckingOut);
                        paypalForm.ValidationGroup = "OPC";
                        paypalForm.ValidationSummaryVisible = false;
                        paypalForm.PaymentAmount = _Balance;
                        phPaymentForms.Controls.Add(paypalForm);
                        break;
                    case PaymentInstrument.Mail:
                        ASP.MailPaymentForm mailForm = new ASP.MailPaymentForm();
                        mailForm.PaymentMethodId = selectedMethod.PaymentMethodId;
                        mailForm.CheckingOut += new CheckingOutEventHandler(CheckingOut);
                        mailForm.ValidationGroup = "OPC";
                        mailForm.ValidationSummaryVisible = false;
                        mailForm.PaymentAmount = _Balance;
                        phPaymentForms.Controls.Add(mailForm);
                        break;
                    case PaymentInstrument.PhoneCall:
                        ASP.PhoneCallPaymentForm phoneCallForm = new ASP.PhoneCallPaymentForm();
                        phoneCallForm.PaymentMethodId = selectedMethod.PaymentMethodId;
                        phoneCallForm.CheckingOut += new CheckingOutEventHandler(CheckingOut);
                        phoneCallForm.ValidationGroup = "OPC";
                        phoneCallForm.ValidationSummaryVisible = false;
                        phoneCallForm.PaymentAmount = _Balance;
                        phPaymentForms.Controls.Add(phoneCallForm);
                        break;
                    default:
                        //types not supported
                        break;
                }
            }
        }

        //WE DO NOT NEED THE PAYMENT SELECTION LIST IF THERE IS NOT MORE THAN ONE
        //AVAILABLE TYPE OF PAYMENT
        tdPaymentMethodList.Visible = (PaymentMethodList.Items.Count > 1);
    }

    void CheckingOut(object sender, CheckingOutEventArgs e)
    {		
        //MAKE SURE WE HAVE VALIDATED THIS FORM
        Page.Validate("OPC");
        //IF ANYTHING WAS INVALID CANCEL CHECKOUT
        if (!Page.IsValid) e.Cancel = true;

		//Update the billing address
		SaveBillingAddress();

        //CANCEL
        if (e.Payment != null)
        {
            if ((e.Payment.Amount > 0) && (e.Payment.Amount <= _Balance))
            {
                bool result = PayOrder(e.Payment);
                if (result) Response.Redirect( NavigationHelper.GetStoreUrl(this.Page, "Members/MyOrder.aspx?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString()));
                else Response.Redirect( NavigationHelper.GetStoreUrl(this.Page, "Members/PayMyOrder.aspx?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString()));
            }
            else
            {
                PaymentErrorPanel.Visible = true;
                PaymentErrorMessage.Text = "Invalid amount.  Payment must be greater than zero and no more than " + _Balance.ToString("lc") + ".";
            }
        }
        else
        {
            PaymentErrorPanel.Visible = true;
            PaymentErrorMessage.Text = "The selected payment method is not available.";
        }
		//Always cancel. We make the payment using PayOrder method
        e.Cancel = true;
    }

    private bool PayOrder(Payment payment)
    {
        bool hasGateway = (payment.PaymentMethod.PaymentGateway != null);
        if (hasGateway)
        {
            //PRESERVE ACCOUNT DATA FOR PROCESSING
            string accountData = payment.AccountData;
            //payment.AccountData = string.Empty;
            //SAVE PAYMENT
            _Order.Payments.Add(payment);
            _Order.Save();
            //PROCESS PAYMENT WITH SAVED ACCOUNT DATA
            payment.AccountData = accountData;
            payment.Authorize(false);
            return (payment.PaymentStatus != PaymentStatus.AuthorizationFailed) ? true : false;
        }
        else
        {
            //SAVE PAYMENT
            _Order.Payments.Add(payment);
            _Order.Save();
            return true;
        }
    }
}
