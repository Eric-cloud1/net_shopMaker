using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using MakerShop.Common;
using MakerShop.Orders;
using MakerShop.Payments;
using MakerShop.Shipping;
using MakerShop.Stores;
using MakerShop.Utility;
using MakerShop.Taxes;
using MakerShop.Users;

public partial class Admin_Orders_ViewOrder : MakerShop.Web.UI.MakerShopAdminPage
{
    private const string MULTIPLE_SHIP_TO_INDICATOR = "multiple";
    private const string MULTIPLE_SHIP_METHOD_INDICATOR = "multiple";
    private const string NO_SHIPMENT_INDICATOR = "n/a";

    private int _OrderId;
    private Order _Order;

    protected int LastPaymentId
    {
        get
        {
            if (ViewState["LastPaymentId"] != null) return (int)ViewState["LastPaymentId"];
            return 0;
        }
        set
        {
            ViewState["LastPaymentId"] = value;
        }
    }

    protected int PendingShipmentId
    {
        get
        {
            if (ViewState["PendingShipmentId"] != null) return (int)ViewState["PendingShipmentId"];
            return 0;
        }
        set
        {
            ViewState["PendingShipmentId"] = value;
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        _Order = OrderHelper.GetOrderFromContext();

        if (_Order == null) Response.Redirect("Default.aspx");
        _OrderId = _Order.OrderId;
        BindOrderActions();
        AllPaymentsLink.NavigateUrl = string.Format(AllPaymentsLink.NavigateUrl, _Order.OrderNumber, _OrderId);
        if (_Order.Payments.Count > 1)
        {
            AllPaymentsLink.Visible = true;
        }
        if (Token.Instance.User.IsGateway)
        
        {
            pTasks.Visible = false;
            pAffiliateInfo.Visible = false;
            pPaymentBalance.Visible = false;
            CurrentShipmentStatusPanel.Visible = false;
            pOrderSummary.Visible = false;
            BillToPhone.Visible = false;
            OrderEmailAjax.Visible = false;
            pOptin.Visible = false;
        }

        if (!IsPostBack && !Token.Instance.User.IsAdmin)
        {
            HttpCookie hc = Request.Cookies["LogView"];
            if (hc != null)
            {
                if (hc.Value == Request.Url.AbsoluteUri)
                    return;
            }
            hc = new HttpCookie("LogView",Request.Url.AbsoluteUri);
            hc.Expires = DateTime.Now.AddHours(1);
            Response.Cookies.Add(hc);

            _Order.Notes.Add(new OrderNote(_Order.OrderId, Token.Instance.UserId, DateTime.UtcNow, "Lookup", NoteType.SystemPrivate));
            _Order.Notes.Save();
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        BindOrder();
        BindOrderDetails(_OrderId);
        UpdateOrderEmail();
    }

    protected void BindOrder()
    {
        _Order.RecalculatePaymentStatus();
        _Order.RecalculateShipmentStatus();

        Caption.Text = String.Format(Caption.Text, _Order.OrderNumber, _Order.OrderStatus.Name);
       

        OrderTotal.Text = string.Format("{0:lc}", _Order.TotalCharges);
        OrderDate.Text = string.Format("{0:g}", _Order.OrderDate);
        if (!string.IsNullOrEmpty(_Order.RemoteIP))
        {
            CustomerIP.Text = _Order.RemoteIP;
            CustomerIPBlocked.Visible = BannedIPDataSource.IsBanned(CustomerIP.Text);
            BlockCustomerIP.Visible = (!CustomerIPBlocked.Visible && (_Order.RemoteIP != Request.UserHostAddress));
            BlockCustomerIP.OnClientClick = string.Format(BlockCustomerIP.OnClientClick, _Order.RemoteIP);
        }
        else
        {
            CustomerIP.Visible = false;
            CustomerIPBlocked.Visible = false;
            BlockCustomerIP.Visible = false;
        }
      
        BindBillToAddress();
        BindLastPayment();
        BindShipmentStatus();
        BindOrderItemGrid();
        BindOptIn(Token.Instance.UserId);

        CurrentPaymentStatus.Text = _Order.PaymentStatus.ToString();
        CurrentShipmentStatus.Text = _Order.ShipmentStatus.ToString();

        LSDecimal orderTotal, paymentTotal, balance;
        orderTotal = _Order.TotalCharges;
        paymentTotal = _Order.TotalPayments;
        balance = _Order.GetBalance();

        OrderTotal1.Text = string.Format(OrderTotal1.Text, orderTotal);
        TotalPayment.Text = string.Format(TotalPayment.Text, paymentTotal);
        RemainingBalance.Text = string.Format(RemainingBalance.Text, _Order.GetBalance());

        LSDecimal processedPayments, unprocessedPayments;
        processedPayments = _Order.Payments.TotalProcessed();
        unprocessedPayments = _Order.Payments.TotalUnprocessed();

        ProcessedPayments.Text = string.Format(ProcessedPayments.Text, processedPayments);
        UnprocessedPayments.Text = string.Format(UnprocessedPayments.Text, unprocessedPayments);
    }

    protected void BindOptIn(int userId)
    {
        
        OptInCollection optins = OptInDataSource.LoadForUser(userId);

        if (optins.Count == 0)
            return;

        this.pOptin.Visible = true;
        this.OptIn.Text = optins[0].OptInDate.ToShortDateString();
        this.OptOut.Text = optins[0].OptOutDate.Value.ToShortDateString();
        this.OptInMessageLabel.ToolTip = optins[0].Message;


    }
    protected void BindOrderDetails(int OrderId)
    {

        Order_Ex ex = new Order_Ex();
        bool isDetails = ex.Load(OrderId);
        if (isDetails == false)
            return;


        pOrdersEx.Visible = true;

       Regex regexObj = new Regex(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$");
        
        Label cellphone = new Label();
        cellphone.Text = string.Format("<b>Cell Phone:</b> {0}</br>", regexObj.Replace(ex.CellPhone, "($1) $2-$3"));

        Label HomePhone = new Label();
        HomePhone.Text = string.Format("<b>Home Phone:</b> {0}</br>", regexObj.Replace(ex.HomePhone, "($1) $2-$3"));

        Label DOB = new Label();
        DOB.Text = string.Format("<b>DOB:</b> {0}</br>", ex.DOB.Value.ToShortDateString());

        Label Last4SSN = new Label();
        Last4SSN.Text = string.Format("<b>SSN:</b> ***-**-{0}</br>", ex.Last4SSN);

    


        if (ex.CellPhone.Length > 0)
            CustomerInformation.Controls.Add(cellphone);

        if (ex.HomePhone.Length > 0)
            CustomerInformation.Controls.Add(HomePhone);

        if (ex.DOB != null)
            CustomerInformation.Controls.Add(DOB);

        if (ex.Last4SSN.Length > 0)
            CustomerInformation.Controls.Add(Last4SSN);

        if((ex.CellPhone.Length > 0)||(ex.HomePhone.Length > 0)||(ex.DOB != null))
            InformationCaption.Visible = true;

       Label AnnualIncome = new Label();
       AnnualIncome.Text = string.Format("<b>Annual Income:</b> {0:lc}</br>", (LSDecimal)ex.AnnualIncome);

       Label MonthlyIncome = new Label();
       MonthlyIncome.Text = string.Format("<b>Monthly Income:</b>  {0:lc}</br>", (LSDecimal)ex.MonthlyIncome);

       Label Children = new Label();
       Children.Text = string.Format("<b>Children:</b> {0}</br>", ex.Children);

       Label GovernmentProgram = new Label();
       GovernmentProgram.Text = string.Format("<b>Government Program:</b><br/>{0}</br>", ex.GovernmentProgram.ToString().Replace("_"," "));

        if(ex.AnnualIncome >0)
            DemographicInformation.Controls.Add(AnnualIncome);

        if (ex.MonthlyIncome > 0)
            DemographicInformation.Controls.Add(MonthlyIncome);

        if (ex.GovernmentProgramId > 0)
            DemographicInformation.Controls.Add(GovernmentProgram);

        if (ex.Children > 0)
            DemographicInformation.Controls.Add(Children);

        if((ex.AnnualIncome >0)||(ex.MonthlyIncome > 0)||(ex.GovernmentProgramId > 0)||(ex.Children > 0))
            DemographicCaption.Visible = true;
   
       Label PeopleEmployed = new Label();
       PeopleEmployed.Text = string.Format("<b>People Employed:</b> {0}</br>", ex.PeopleEmployed);

       Label PeopleInHousehold = new Label();
       PeopleInHousehold.Text = string.Format("<b>People in Household:</b> {0}</br>", ex.PeopleInHousehold);
        
       Label HouseholdAnnualIncome = new Label();
       HouseholdAnnualIncome.Text = string.Format("<b>Household Annual Income:</b> {0:lc}</br>", (LSDecimal)ex.HouseholdAnnualIncome);

       Label HouseholdMonthlyIncome = new Label();
       HouseholdMonthlyIncome.Text = string.Format("<b>Household Monthly Income:</b> {0:lc}</br>", (LSDecimal)ex.HouseholdMonthlyIncome);

        if(ex.PeopleEmployed > 0)
            HouseholdInformation.Controls.Add(PeopleEmployed);

        if (ex.PeopleInHousehold > 0)
            HouseholdInformation.Controls.Add(PeopleInHousehold);

        if (ex.HouseholdAnnualIncome > 0)
            HouseholdInformation.Controls.Add(HouseholdAnnualIncome);

        if (ex.HouseholdMonthlyIncome > 0)
            HouseholdInformation.Controls.Add(HouseholdMonthlyIncome);

        if((ex.PeopleEmployed > 0)||(ex.HouseholdAnnualIncome > 0)||(ex.HouseholdMonthlyIncome > 0))
            HouseholdCaption.Visible = true;

    }


    protected void BindOrderActions()
    {
        OrderAction.Items.Clear();
        //ADD BLANK ITEM TO START
        OrderAction.Items.Add(new ListItem());
        if (_Order.OrderStatus.IsValid)
        {
            //ADD SHIP ORDER ITEM
            if (IsShippable()) OrderAction.Items.Add(new ListItem("Ship Items", "SHIP"));
            //ADD CANCEL ORDER ITEM
            if (_Order.OrderStatus.IsValid) OrderAction.Items.Add(new ListItem("Cancel Order", "CANCEL"));
            //ADD SEPARATOR IF ITEMS ADDED ABOVE
            if (OrderAction.Items.Count > 1) OrderAction.Items.Add("---");
        }
        //ADD PRINTING ACTIONS
        OrderAction.Items.Add(new ListItem("Print Invoice", "INVOICE"));
        OrderAction.Items.Add(new ListItem("Print Packing Slip", "PACKSLIP"));
        OrderAction.Items.Add(new ListItem("Print Pull Sheet", "PULLSHEET"));
        //ADD SEPARATOR
        OrderAction.Items.Add("---");
        //ADD ORDER STATUS UPDATE ACTIONS
        OrderStatusCollection statuses = OrderStatusDataSource.LoadForStore();
        int index = statuses.IndexOf(_Order.OrderStatusId);
        if (index > -1) statuses.RemoveAt(index);
        foreach (OrderStatus status in statuses)
        {
            if (status.OrderStatusId != _Order.OrderStatusId) OrderAction.Items.Add(new ListItem("Change status to " + status.Name, "OS_" + status.OrderStatusId.ToString()));
        }
    }

    /// <summary>
    /// Indicates whether the order has any items left to ship.
    /// </summary>
    /// <returns>True if shippable items remain in the order.</returns>
    private bool IsShippable()
    {
        if (_Order.Shipments.Count == 0) return false;
        //LOOK FOR UNSHIPPED SHIPMENTS
        foreach (OrderShipment shipment in _Order.Shipments)
        {
            if (shipment.ShipDate == DateTime.MinValue) return true;
        }
        //LOOK FOR ITEMS THAT ARE SHIPPABLE WITHOUT AN ASSIGNED SHIPMENT
        foreach (OrderItem item in _Order.Items)
        {
            if ((item.OrderShipmentId == 0) && (item.Shippable != Shippable.No)) return true;
        }
        return false;
    }

    protected void BindBillToAddress()
    {
        string pattern = "[Company]\r\n[Name]\r\n[Address1]\r\n[Address2]\r\n[City], [Province] [PostalCode]\r\n[Country_U]\r\n[Phone]\r\n";
        Country country = CountryDataSource.Load(_Order.BillToCountryCode);
        if (country != null)
        {
            if (!string.IsNullOrEmpty(country.AddressFormat)) pattern = country.AddressFormat + "\r\n[Phone]\r\n[Email]";
        }
        BillToAddress.Text = _Order.FormatAddress(pattern, true);
        BillToEmail.Text = _Order.BillToEmail;
        String returnUrl = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("~/Admin/Orders/ViewOrder.aspx?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString()));
        BillToEmail.NavigateUrl += "?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString() + "&ReturnUrl=" + returnUrl;
        if (!string.IsNullOrEmpty(_Order.BillToPhone)) BillToPhone.Text = string.Format(BillToPhone.Text, _Order.BillToPhone);
        else BillToPhone.Visible = false;
        if (!string.IsNullOrEmpty(_Order.BillToFax)) BillToFax.Text = string.Format(BillToFax.Text, _Order.BillToFax);
        else BillToFax.Visible = false;

        
    }

    protected string GetShipToAddress()
    {
        string firstAddress = string.Empty;
        for (int i = 0; i < _Order.Shipments.Count; i++)
        {
            OrderShipment shipment = _Order.Shipments[i];
            string pattern = "[Company]\r\n[Name]\r\n[Address1]\r\n[Address2]\r\n[City], [Province] [PostalCode]\r\n[Country_U]\r\n[Phone]\r\n[Email]";
            Country country = CountryDataSource.Load(shipment.ShipToCountryCode);
            if (country != null)
            {
                if (!string.IsNullOrEmpty(country.AddressFormat)) pattern = country.AddressFormat + "\r\n[Phone]\r\n[Email]";
            }
            if (i == 0)
            {
                firstAddress = shipment.FormatToAddress(pattern, true);
            }
            else
            {
                string tempAddress = shipment.FormatToAddress(pattern, true);
                if (tempAddress != firstAddress) return MULTIPLE_SHIP_TO_INDICATOR;
            }
        }
        return firstAddress;
    }

    protected string GetShipMethod()
    {
        string firstMethod = string.Empty;
        for (int i = 0; i < _Order.Shipments.Count; i++)
        {
            OrderShipment shipment = _Order.Shipments[i];
            if (i == 0)
            {
                firstMethod = shipment.ShipMethodName;
            }
            else
            {
                string tempMethod = shipment.ShipMethodName;
                if (tempMethod != firstMethod) return MULTIPLE_SHIP_METHOD_INDICATOR;
            }
        }
        return firstMethod;
    }

    protected int CountPendingShipments()
    {
        int pendingShipments = 0;
        foreach (OrderShipment shipment in _Order.Shipments)
        {
            if (shipment.ShipDate == DateTime.MinValue) pendingShipments++;
        }
        return pendingShipments;
    }

    protected void BindShipmentStatus()
    {
        ShipToAddress.Text = GetShipToAddress();
        if (_Order.Shipments.Count > 0)
        {
            //SET THE SHIP METHOD
            ShipMethod.Text = GetShipMethod();
            //SET THE SHIPMENT STATUS
            int pendingShipments = CountPendingShipments();
            if (pendingShipments > 0)
            {
                if (pendingShipments != _Order.Shipments.Count)
                {
                    CurrentShipmentStatus.Text = "Shipped Partial";
                }
                else
                {
                    CurrentShipmentStatus.Text = (_Order.OrderStatus.IsValid ? "Pending" : "Cancelled");
                }
            }
            else
            {
                //THERE ARE NO PENDING SHIPMENTS
                CurrentShipmentStatus.Text = "Shipped";
            }
        }
    }

    protected void BindOrderItemGrid()
    {
        OrderItemCollection itemList = new OrderItemCollection();
        foreach (OrderItem item in _Order.Items)
        {
            switch (item.OrderItemType)
            {
                case OrderItemType.Tax:
                case OrderItemType.GiftCertificatePayment:
                    break;
                default:
                    itemList.Add(item);
                    break;
            }
        }

        //SHOW TAXES IF SPECIFIED
        TaxInvoiceDisplay displayMode = TaxHelper.InvoiceDisplay;
        if (displayMode == TaxInvoiceDisplay.LineItem || displayMode == TaxInvoiceDisplay.LineItemRegistered)
        {
            foreach (OrderItem item in _Order.Items)
            {
                if ((item.OrderItemType == OrderItemType.Tax)
                    && (itemList.IndexOf(item.ParentItemId) > -1))
                    itemList.Add(item);
            }
        }

        itemList.Sort(new OrderItemComparer());
        OrderItemGrid.DataSource = itemList;
        OrderItemGrid.DataBind();
    }

    protected Payment GetLastPayment()
    {
        //IF THERE IS ANY INCOMPLETE PAYMENT, RETURN THE FIRST ONE FOUND
        LastPaymentId = 0;
        foreach (Payment payment in _Order.AllPayments)
        {
            if ((payment.PaymentStatus == PaymentStatus.Unprocessed) || (payment.PaymentStatus == PaymentStatus.AuthorizationPending) ||
                (payment.PaymentStatus == PaymentStatus.Authorized) || (payment.PaymentStatus == PaymentStatus.CapturePending))
            {
                LastPaymentId = payment.PaymentId;
                return payment;
            }
        }
        if (_Order.Payments.Count > 0) return _Order.Payments[_Order.Payments.Count - 1];
        return null;
    }

    protected void BindLastPayment()
    {
        LastPaymentAVS.Visible = true;
        LastPaymentCVV.Visible = true;
        LastPaymentAVSLabel.Visible = true;
        LastPaymentCVVLabel.Visible = true;

        Payment payment = GetLastPayment();
        if (payment != null)
        {
            LastPaymentPanel.Visible = true;
            LastPaymentAmount.Text = string.Format("{0:lc}", payment.Amount);
            LastPaymentStatus.Text = StringHelper.SpaceName(payment.PaymentStatus.ToString());
            LastPaymentReference.Text = string.Format("{0} - {1}", payment.PaymentMethodName, payment.ReferenceNumber);
            Transaction lastAuthorization = (Transaction)payment.Transactions.LastAuthorization;
            if (lastAuthorization == null)
                lastAuthorization = (Transaction)payment.Transactions.LastRecurringAuthorization;
            if (lastAuthorization != null)
            {

                if (string.IsNullOrEmpty(lastAuthorization.CVVResultCode))
                {
                    LastPaymentCVV.Visible = false;
                    LastPaymentCVVLabel.Visible = false;
                }
                else
                {
                    LastPaymentCVV.Text = StoreDataHelper.TranslateCVVCode(lastAuthorization.CVVResultCode) + " (" + lastAuthorization.CVVResultCode + ")";
                }

                if (string.IsNullOrEmpty(lastAuthorization.AVSResultCode))
                {
                    LastPaymentAVS.Visible = false;
                    LastPaymentAVSLabel.Visible = false;
                }
                else
                {
                    LastPaymentAVS.Text = StoreDataHelper.TranslateAVSCode(lastAuthorization.AVSResultCode) + " (" + lastAuthorization.AVSResultCode + ")";
                }
            }
            else
            {
                  LastPaymentAVS.Visible = false;
                  LastPaymentAVSLabel.Visible = false;
                LastPaymentCVV.Visible = false;
                LastPaymentCVVLabel.Visible = false; 
                


                //LastPaymentCVV.Text =  "n/a";
                //LastPaymentAVS.Text =  "n/a";
            }
      

                 

            ReceivedButton.Visible = (payment.PaymentStatus == PaymentStatus.Unprocessed);
            VoidLink.Visible = ((payment.PaymentStatus == PaymentStatus.Unprocessed) || (payment.PaymentStatus == PaymentStatus.Authorized) || (payment.PaymentStatus == PaymentStatus.AuthorizationFailed) || (payment.PaymentStatus == PaymentStatus.CaptureFailed));
            VoidLink.NavigateUrl = "Payments/VoidPayment.aspx?PaymentId=" + payment.PaymentId.ToString() + "&OrderId=" + _OrderId;
            CaptureLink.Visible = (payment.PaymentStatus == PaymentStatus.Authorized);
            CaptureLink.NavigateUrl = "Payments/CapturePayment.aspx?PaymentId=" + payment.PaymentId.ToString();
        }
        else
        {
            LastPaymentPanel.Visible = false;
            ReceivedButton.Visible = false;
            VoidLink.Visible = false;
            CaptureLink.Visible = false;
        }
    }

    protected void ReceivedButton_Click(object sender, EventArgs e)
    {
        if (LastPaymentId != 0)
        {
            int index = _Order.Payments.IndexOf(LastPaymentId);
            if (index > -1)
            {
                Payment payment = _Order.Payments[index];
                payment.Authorize();
            }
        }
    }

    protected string GetProductName(object dataItem)
    {
        OrderItem orderItem = (OrderItem)dataItem;
        if (string.IsNullOrEmpty(orderItem.VariantName)) return orderItem.Name;
        return string.Format("{0} ({1})", orderItem.Name, orderItem.VariantName);
    }

    protected bool IsGiftCert(object dataItem)
    {
        OrderItem orderItem = dataItem as OrderItem;
        if (orderItem == null) return false;
        return (orderItem.GiftCertificates.Count > 0);
    }

    protected bool IsDigitalGood(object dataItem)
    {
        OrderItem orderItem = dataItem as OrderItem;
        if (orderItem == null) return false;
        return (orderItem.DigitalGoods.Count > 0);
    }

    protected void OrderActionButton_Click(object sender, ImageClickEventArgs e)
    {
        string action = OrderAction.SelectedValue;
        if (!string.IsNullOrEmpty(action))
        {
            switch (action)
            {
                case "SHIP":
                    foreach (OrderShipment shipment in _Order.Shipments)
                    {
                        if (shipment.ShipDate == DateTime.MinValue)
                        {
                            Response.Redirect("Shipments/ShipOrder.aspx?OrderShipmentId=" + shipment.OrderShipmentId.ToString());
                        }
                    }
                    break;
                case "CANCEL":
                    Response.Redirect( "CancelOrder.aspx?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString());
                    break;
                case "INVOICE":
                    Response.Redirect( "Print/Invoices.aspx?orders=" + _Order.OrderNumber.ToString());
                    break;
                case "PACKSLIP":
                    Response.Redirect( "Print/PackSlips.aspx?orders=" + _Order.OrderNumber.ToString());
                    break;
                case "PULLSHEET":
                    Response.Redirect( "Print/PullSheet.aspx?orders=" + _Order.OrderNumber.ToString());
                    break;
            }
            if (action.StartsWith("OS_"))
            {
                //UPDATE ORDER STATUS REQUESTED
                int orderStatusId = AlwaysConvert.ToInt(action.Substring(3));
                //VALIDATE STATUS
                OrderStatus status = OrderStatusDataSource.Load(orderStatusId);
                if (status != null)
                {
                    _Order.OrderStatusId = status.OrderStatusId;
                    _Order.Save();
                    if (!status.IsValid)
                    { // cancel subscription
                        _Order.Cancel();
                    }
                }
                BindOrderActions();
            }
        }
    }

    protected void BlockCustomerIP_Click(object sender, ImageClickEventArgs e)
    {
        BannedIP block = new BannedIP();
        block.IPRangeStart = BannedIP.ConvertToNumber(_Order.RemoteIP);
        block.IPRangeEnd = block.IPRangeStart;
        block.Comment = "Order " + _Order.OrderId.ToString();
        block.Save();
    }

    protected void ChangeOrderEmailButton_Click(object sender, EventArgs e)
    {
        BillToEmail.Visible = false;
        phEditBillToEmail.Visible = false;
        EditOrderEmailPanel.Visible = true;
        UpdateOrderEmail();
    }
    
    protected void SaveOrderEmailButton_Click(object sender, EventArgs e)
    {
        if (EmailAddressValidator1.IsValid)
        {
            _Order.BillToEmail = OrderEmail.Text;
            _Order.Save();
            UpdateOrderEmail();
            BillToEmail.Visible = true;
            phEditBillToEmail.Visible = true;
            EditOrderEmailPanel.Visible = false;
        }
        else Page.Validate("OrderEmail");
    }

    private void UpdateOrderEmail()
    {
        BillToEmail.Text = String.IsNullOrEmpty(_Order.BillToEmail) ? "Email not specified." : _Order.BillToEmail;
        OrderEmail.Text = _Order.BillToEmail;        
        PageHelper.SetDefaultButton(OrderEmail, SaveOrderEmailButton);
    }

    protected void OrderItemGrid_DataBinding(object sender, EventArgs e)
    {
        GridView grid = (GridView)sender;
        grid.Columns[2].Visible = TaxHelper.ShowTaxColumn;
        grid.Columns[2].HeaderText = TaxHelper.TaxColumnHeader;
    }

 

  
}
