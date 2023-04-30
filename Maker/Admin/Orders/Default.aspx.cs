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
using MakerShop.Common;
using MakerShop.Orders;
using MakerShop.Payments;
using MakerShop.Reporting;
using MakerShop.Utility;
using System.Text;

public partial class Admin_Orders_Default : MakerShop.Web.UI.MakerShopAdminPage
{

    private TextBox KeywordSearchText = null;
    private DropDownList KeywordSearchField = null;
    private ImageButton RemoveKeyword = null;

    public Table ItemTable
    {
        get
        {

            if (Session[this.ClientID + "_ItemTable"] != null)
                return (Table)Session[this.ClientID + "_ItemTable"];

            return new Table();
        }

        set
        {
            Session[this.ClientID + "_ItemTable"] = value;
        }
    }


    public bool FirstHit
    {

        get
        {
            if (Session[this.ClientID + "_FirstHit"] == null)
                return true;

            return (bool)Session[this.ClientID + "_FirstHit"];
        }
        set
        {
            Session[this.ClientID + "_FirstHit"] = value;
        }
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        if (Request.QueryString["PhoneNumber"] != null)
        {
            OrderSearchCriteria criteria = new OrderSearchCriteria();
            MatchCriteria thisFilter = new MatchCriteria();

            thisFilter.FieldName = "BillToPhone";
            thisFilter.FieldValue = Request.QueryString["PhoneNumber"].ToString();
            criteria.Filter.Add(thisFilter);
            Session["OrderSearchCriteria"] = criteria;
            OrderDs.SelectParameters.Add("criteria", DbType.Object, "GetOrderSearchCriteria()");
        }
        else
        {
            Parameter criteria = new Parameter("criteria", DbType.Object);
            OrderDs.SelectParameters.Add(criteria);
        }

        OrderDs.SelectParameters.Add("maximumRows", DbType.Int32, "200");
        OrderDs.SelectParameters.Add("startRowIndex", DbType.Int32, "0");
        OrderDs.SelectParameters.Add("sortExpression", DbType.String, string.Empty);

        OrderDs.OldValuesParameterFormatString = "original_{0}";
        OrderDs.SelectMethod = "Search";
        OrderDs.SortParameterName = "SortExpression";
        OrderDs.TypeName = "MakerShop.Orders.OrderDataSource";
        OrderDs.Selecting += new ObjectDataSourceSelectingEventHandler(OrderDs_Selecting);

        OrderDs.DataBind();


        //GET ORDER STATUSES FOR STORE
        OrderStatusCollection statuses = OrderStatusDataSource.LoadForStore();
        OrderStatusCollection validStatuses = new OrderStatusCollection();
        OrderStatusCollection invalidStatuses = new OrderStatusCollection();
        //BUILD STATUS FILTER
        String statusNamePrefix = "- ";
        foreach (OrderStatus status in statuses)
        {
            if (status.IsValid)
            {
                validStatuses.Add(status);
            }
            else
            {
                invalidStatuses.Add(status);
            }            
        }
        // ADD VALID STATUSES
        StatusFilter.Items.Add(new ListItem("All Valid", "-2"));
        foreach (OrderStatus status in validStatuses)
        {
            StatusFilter.Items.Add(new ListItem(statusNamePrefix + status.Name, status.OrderStatusId.ToString()));
        }
        // ADD INVALID STATUSES
        StatusFilter.Items.Add(new ListItem("All Invalid", "-3"));
        foreach (OrderStatus status in invalidStatuses)
        {
            StatusFilter.Items.Add(new ListItem(statusNamePrefix + status.Name, status.OrderStatusId.ToString()));
        }
        // SET THE DEFAULT AS ALL-VALID
        StatusFilter.SelectedIndex = 0;

        //APPEND ORDER STATUS ACTIONS TO BATCH LIST
        string updateText = "Update status to {0}";
        foreach (OrderStatus status in statuses)
        {
            
            BatchAction.Items.Add(new ListItem(string.Format(updateText, status.Name), "OS_" + status.OrderStatusId));
        }

        InitDateQuickPick();

        InitSearch();
    }

    private void InitDateQuickPick()
    {
        StringBuilder js = new StringBuilder();
        js.AppendLine("function dateQP(selectDom) {");
        js.AppendLine("var startPicker = " + OrderStartDate.GetPickerClientId());
        js.AppendLine("var endPicker = " + OrderEndDate.GetPickerClientId());
        js.AppendLine("switch(selectDom.selectedIndex){");
        string setStart= "startPicker.setSelectedDate(new Date('{0}'));";
        string setEnd= "endPicker.setSelectedDate(new Date('{0}'));";
        string clearStart= "startPicker.clearSelectedDate();";
        string clearEnd= "endPicker.clearSelectedDate();";
        int startIndex = 1;

        DateQuickPick.Items.Add(new ListItem("Today"));
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(string.Format(setStart, LocaleHelper.LocalNow.ToString("d")));
        js.Append(clearEnd);
        js.AppendLine("break;");

        DateQuickPick.Items.Add(new ListItem("Last 7 Days"));
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(string.Format(setStart, LocaleHelper.LocalNow.AddDays(-7).ToString("d")));
        js.Append(clearEnd);
        js.AppendLine("break;");

        DateQuickPick.Items.Add(new ListItem("Last 14 Days"));
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(string.Format(setStart, LocaleHelper.LocalNow.AddDays(-14).ToString("d")));
        js.Append(clearEnd);
        js.AppendLine("break;");

        DateQuickPick.Items.Add(new ListItem("Last 30 Days"));
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(string.Format(setStart, LocaleHelper.LocalNow.AddDays(-30).ToString("d")));
        js.Append(clearEnd);
        js.AppendLine("break;");

        DateQuickPick.Items.Add(new ListItem("Last 60 Days"));
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(string.Format(setStart, LocaleHelper.LocalNow.AddDays(-60).ToString("d")));
        js.Append(clearEnd);
        js.AppendLine("break;");

        DateQuickPick.Items.Add(new ListItem("Last 90 Days"));
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(string.Format(setStart, LocaleHelper.LocalNow.AddDays(-90).ToString("d")));
        js.Append(clearEnd);
        js.AppendLine("break;");

        DateQuickPick.Items.Add(new ListItem("Last 120 Days"));
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(string.Format(setStart, LocaleHelper.LocalNow.AddDays(-120).ToString("d")));
        js.Append(clearEnd);
        js.AppendLine("break;");

        DateQuickPick.Items.Add(new ListItem("This Week"));
        DateTime startDate = LocaleHelper.LocalNow.AddDays(-1 * (int)LocaleHelper.LocalNow.DayOfWeek);
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(string.Format(setStart, startDate.ToString("d")));
        js.Append(clearEnd);
        js.AppendLine("break;");

        DateQuickPick.Items.Add(new ListItem("Last Week"));
        startDate = LocaleHelper.LocalNow.AddDays((-1 * (int)LocaleHelper.LocalNow.DayOfWeek) - 7);
        DateTime endDate = startDate.AddDays(6);
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(string.Format(setStart, startDate.ToString("d")));
        js.Append(string.Format(setEnd, endDate.ToString("d")));
        js.AppendLine("break;");

        DateQuickPick.Items.Add(new ListItem("This Month"));
        startDate = new DateTime(LocaleHelper.LocalNow.Year, LocaleHelper.LocalNow.Month, 1);
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(string.Format(setStart, startDate.ToString("d")));
        js.Append(clearEnd);
        js.AppendLine("break;");

        DateQuickPick.Items.Add(new ListItem("Last Month"));
        DateTime lastMonth = LocaleHelper.LocalNow.AddMonths(-1);
        startDate = new DateTime(lastMonth.Year, lastMonth.Month, 1);
        endDate = startDate.AddMonths(1).AddDays(-1);
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(string.Format(setStart, startDate.ToString("d")));
        js.Append(string.Format(setEnd, endDate.ToString("d")));
        js.AppendLine("break;");

        DateQuickPick.Items.Add(new ListItem("This Year"));
        startDate = new DateTime(LocaleHelper.LocalNow.Year, 1, 1);
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(string.Format(setStart, startDate.ToString("d")));
        js.Append(clearEnd);
        js.AppendLine("break;");

        DateQuickPick.Items.Add(new ListItem("All Dates"));
        startDate = new DateTime(LocaleHelper.LocalNow.Year, 1, 1);
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(clearStart);
        js.Append(clearEnd);
        js.AppendLine("break;");

        // close switch
        js.Append("}");

        // reset quick picker
        js.AppendLine("selectDom.selectedIndex = 1;");
        js.Append("}");
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "dateQP", js.ToString(), true);
        DateQuickPick.Attributes.Add("onChange", "dateQP(this)");
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
        {

            // GET DATE FROM QUERYSTRING
            int dateFilter;
            int.TryParse(Request.QueryString["DateFilter"], out dateFilter);

            if (dateFilter == 1) OrderStartDate.SelectedDate = LocaleHelper.LocalNow;
            else if (dateFilter == 5) OrderStartDate.SelectedDate = LocaleHelper.LocalNow.AddDays(-30);
            else if (dateFilter == 7) OrderStartDate.SelectedDate = LocaleHelper.LocalNow.AddDays(-90);
            else dateFilter = 0;

            // GET ORDER STATUS FROM QUERY STRING
            int orderStatusId;
            int.TryParse(Request.QueryString["OrderStatusId"], out orderStatusId);

            if (orderStatusId > 0)
            {
                ListItem item = StatusFilter.Items.FindByValue(orderStatusId.ToString());
                if (item != null)
                {
                    StatusFilter.SelectedIndex = StatusFilter.Items.IndexOf(item);
                }
                else orderStatusId = 0;
            }

            //DO NOT LOAD LAST SEARCH IF QUERY STRING PARAMETERS WERE NOT SET
            if (dateFilter == 0 && orderStatusId == 0)
                LoadLastSearch();


                if (FirstHit)
                {
                    KeywordSearchText = new TextBox();
                    KeywordSearchField = new DropDownList();
                    RemoveKeyword = new ImageButton();

                    LoadAdvancedSearch(KeywordSearchText, KeywordSearchField, RemoveKeyword);

                    FirstHit = false;
                }
            }

        advancedSearch.Controls.Add(ItemTable);
     

        if (OrderEndDate.SelectedDate > DateTime.Today)
            OrderEndDate.SelectedDate = DateTime.Today;

    }

    private void InitSearch()
    {

        for (int i = 0; i < ItemTable.Rows.Count; i++)
        {
            if (i == 0)
            {
                ((ImageButton)ItemTable.Rows[i].Cells[0].Controls[0]).Click += new ImageClickEventHandler(Keyword_Add);
                continue;
            }


            ((ImageButton)ItemTable.Rows[i].Cells[0].Controls[0]).Click += new ImageClickEventHandler(Keyword_Remove);
        }

      

    }

    private void LoadAdvancedSearch(TextBox keywordSearch, DropDownList keywordSearchField, ImageButton removeKeyword)
    {
        ListItemCollection keywords = new ListItemCollection();
        keywords.Add(new ListItem("Last Name, First Name", "FullName"));
        keywords.Add(new ListItem("Phone Number", "BillToPhone"));
        keywords.Add(new ListItem("Email", "BillToEmail"));
        keywords.Add(new ListItem("Tracking Number", "TrackingNumberData"));
        keywords.Add(new ListItem("Credit Card Last 4", "ReferenceNumber"));
        keywords.Add(new ListItem("Processor's Id", "ProviderTransactionId"));
        keywords.Add(new ListItem("Order Notes", "OrderNotes"));
        keywords.Add(new ListItem("Address", "BillToAddress1"));
        keywords.Add(new ListItem("Region", "BillToProvince"));

        ListItemCollection selectedKeywords = new ListItemCollection();

        Random rand = new Random((int)DateTime.Now.Ticks);
        int key = rand.Next();

        keywordSearch.ID = string.Format("keywordSearch{1}_{0}", this.ClientID, key);
        keywordSearch.Width = Unit.Pixel(185);
        keywordSearch.MaxLength = 100;

        keywordSearchField.ID = string.Format("keywordSearchField{1}_{0}", this.ClientID, key);
        keywordSearchField.AutoPostBack = true;

        removeKeyword.ID = string.Format("removeKeyword{1}_{0}", this.ClientID, key);
        //removeKeyword.PostBackUrl = this.Request.Url.AbsoluteUri;

        int count = ItemTable.Rows.Count;

        if (count == 0)
        {

            removeKeyword.ImageUrl = "~/images/add.gif"; //Images/Icons/add.gif
            removeKeyword.Height = Unit.Pixel(15);
            
            foreach (ListItem i in keywords)
                keywordSearchField.Items.Add(i);
        }
        else
        {
            removeKeyword.ImageUrl = "~/images/remove.gif";
            removeKeyword.Height = Unit.Pixel(15);
       
            foreach (TableRow row in ItemTable.Rows)
            {
                keywordSearch.ID = string.Format("keywordSearch{1}_{0}", this.ClientID, rand.Next());
                selectedKeywords.Add(((DropDownList)row.Cells[1].Controls[0]).SelectedItem);
            }

            foreach (ListItem word in keywords)
            {
                keywordSearchField.ID = string.Format("keywordSearchField{1}_{0}", this.ClientID, rand.Next());

                if(!selectedKeywords.Contains(word))
                    keywordSearchField.Items.Add(word); 
            }

            if (keywordSearchField.Items.Count == 0)
                return;
        }


        Table table = ItemTable;
        TableRow row1 = new TableRow();
        TableCell cell1 = new TableCell();
        cell1.ColumnSpan = 2;
        cell1.Controls.Add(removeKeyword);
        cell1.Controls.Add(keywordSearch);

        TableCell cell2 = new TableCell();
        cell2.ColumnSpan = 1;
        cell2.Controls.Add(keywordSearchField);

        row1.Controls.Add(cell1);
        row1.Controls.Add(cell2);
        table.Rows.Add(row1);

        ItemTable = table;
    
    }

    protected void Keyword_Remove(object sender, ImageClickEventArgs e)
    {
        int index = 0;

        for (int i =0; i < ItemTable.Rows.Count; i++ )
            index = i;
       

        ItemTable.Rows.RemoveAt(index);

    }


    protected void Keyword_Add(Object sender, EventArgs e)
    {
        TextBox KeywordSearchTextA = new TextBox();
        DropDownList KeywordSearchFieldA = new DropDownList();
        ImageButton ImageButton = new ImageButton();
       
        LoadAdvancedSearch(KeywordSearchTextA, KeywordSearchFieldA, ImageButton);
    }


    private void LoadLastSearch()
    {
        // LOAD CRITERIA FROM SESSION?
        OrderSearchCriteria criteria = Session["OrderSearchCriteria"] as OrderSearchCriteria;
        int selectStatusId = AlwaysConvert.ToInt(Session["OrderSearchCriteriaSelectedStatus"]);
        if (criteria != null)
        {
            // SET THE ORDER STATUS FILTER            
            if (selectStatusId == 0) selectStatusId = -2;
            ListItem statusItem = StatusFilter.Items.FindByValue(selectStatusId.ToString());
            if (statusItem != null) StatusFilter.SelectedIndex = StatusFilter.Items.IndexOf(statusItem);
            //SET THE PAYMENT STATUS FILTER
            statusItem = PaymentStatusFilter.Items.FindByValue(((int)criteria.PaymentStatus).ToString());
            if (statusItem != null) statusItem.Selected = true;
            //SET THE SHIPMENT STATUS FILTER
            statusItem = ShipmentStatusFilter.Items.FindByValue(((int)criteria.ShipmentStatus).ToString());
            if (statusItem != null) statusItem.Selected = true;
            // SET THE ORDERNUMBER
            OrderNumberFilter.Text = criteria.OrderNumberRange;

            // SET PRICE RANGE
            if(criteria.MaxOrderPrice > 0)
            this.AmountMaxPriceFilter.Text = criteria.MaxOrderPrice.ToString();

            if(criteria.MiniOrderPrice > 0)
            this.AmountMiniPriceFilter.Text = criteria.MiniOrderPrice.ToString();

            // SET THE KEYWORD FILTER
            if (criteria.Filter.Count > 0)
            {

                for (int i = 0; i < criteria.Filter.Count; i++)
                {

                    if (!criteria.Filter[i].FieldValue.Contains("%"))
                        ((TextBox)ItemTable.Rows[i].Cells[0].Controls[1]).Text = criteria.Filter[i].FieldValue.Trim();


                    switch (criteria.Filter[i].FieldName)
                    {
                        case "OrderNotes":
                            ((DropDownList)ItemTable.Rows[i].Cells[1].Controls[0]).SelectedIndex = 6;
                            break;
                        case "BillToPhone":
                            ((DropDownList)ItemTable.Rows[i].Cells[1].Controls[0]).SelectedIndex = 1;
                            break;
                        case "BillToEmail":
                            ((DropDownList)ItemTable.Rows[i].Cells[1].Controls[0]).SelectedIndex = 2;
                            break;
                        case "FullName":
                            ((DropDownList)ItemTable.Rows[i].Cells[1].Controls[0]).SelectedIndex = 0;
                            break;
                        case "BillToAddress1":
                            ((DropDownList)ItemTable.Rows[i].Cells[1].Controls[0]).SelectedIndex = 7;
                            break;
                        case "BillToProvince":
                            ((DropDownList)ItemTable.Rows[i].Cells[1].Controls[0]).SelectedIndex = 8;
                            break;
                        case "ReferenceNumber":
                            ((DropDownList)ItemTable.Rows[i].Cells[1].Controls[0]).SelectedIndex = 4;
                            break;
                        case "ProviderTransactionId":
                            ((DropDownList)ItemTable.Rows[i].Cells[1].Controls[0]).SelectedIndex = 5;
                            break;
                        case "TrackingNumberData":
                            ((DropDownList)ItemTable.Rows[i].Cells[1].Controls[0]).SelectedIndex = 3;
                            break;
                    }


                }


            }
            //IF ORDER START DATE IS SPECIFIED, DETERMINE DEFAULT FOR DATE RANGE
            if (criteria.OrderDateStart > DateTime.MinValue && criteria.OrderDateStart < DateTime.MaxValue)
                OrderStartDate.SelectedDate = criteria.OrderDateStart;
            if (criteria.OrderDateEnd > DateTime.MinValue && criteria.OrderDateEnd < DateTime.MaxValue)
                OrderEndDate.SelectedDate = criteria.OrderDateEnd;

            if (criteria.OrderDateEnd > DateTime.Today)
                OrderEndDate.SelectedDate = DateTime.Today;

        }
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        int pageSize = AlwaysConvert.ToInt(PageSize.SelectedValue);
        if (pageSize == 0) OrderGrid.AllowPaging = false;
        else
        {
            OrderGrid.AllowPaging = true;
            OrderGrid.PageSize = pageSize;
        }
        OrderGrid.DataBind();
        SearchResultAjax.Update();
    }

    protected void CreateOrderButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("Create/CreateOrder1.aspx");
    }

    protected string GetOrderStatus(Object orderStatusId)
    {
        OrderStatus status = OrderStatusDataSource.Load((int)orderStatusId);
        if (status != null) return status.Name;
        return string.Empty;
    }

    protected string GetPaymentStatus(object dataItem)
    {
        Order order = (Order)dataItem;
        if (order.PaymentStatus == OrderPaymentStatus.Paid) return "Paid";
        if (order.Payments.Count > 0)
        {
            order.Payments.Sort("PaymentDate");
            Payment lastPayment = order.Payments[order.Payments.Count - 1];
            return StringHelper.SpaceName(lastPayment.PaymentStatus.ToString());
        }
        return order.PaymentStatus.ToString();
    }

    protected void OrderGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Capture") || e.CommandName.Equals("Ship"))
        {
            int orderId = (int)OrderGrid.DataKeys[AlwaysConvert.ToInt(e.CommandArgument)].Value;
            Order order = OrderDataSource.Load(orderId);
            if (order != null)
            {
                if (e.CommandName.Equals("Capture"))
                {
                    if ((order.Payments.Count == 1) && (order.Payments[0].PaymentStatus == PaymentStatus.Authorized))
                    {
                        Response.Redirect("Payments/CapturePayment.aspx?PaymentId=" + order.Payments[0].PaymentId.ToString());
                    }
                }
                else
                {
                    foreach (OrderShipment shipment in order.Shipments)
                    {
                        if (shipment.ShipDate == System.DateTime.MinValue)
                        {
                            Response.Redirect("Shipments/ShipOrder.aspx?OrderShipmentId=" + shipment.OrderShipmentId.ToString());
                        }
                    }
                }
            }
            OrderGrid.DataBind();
        }
    }

    private List<int> GetSelectedOrderIds()
    {
        int indexPeg = OrderGrid.PageSize * OrderGrid.PageIndex;

        List<int> selectedOrders = new List<int>();
        foreach (GridViewRow row in OrderGrid.Rows)
        {
            CheckBox selected = (CheckBox)PageHelper.RecursiveFindControl(row, "Selected");
            if ((selected != null) && selected.Checked)
            {
                selectedOrders.Add((int)OrderGrid.DataKeys[row.DataItemIndex - indexPeg].Values[0]);
            }
        }
        return selectedOrders;
    }


    protected void OrderGrid_RowCreated(object sender, GridViewRowEventArgs e)
    {
        return;
        if ((e.Row != null) && (e.Row.RowType == DataControlRowType.DataRow) && (e.Row.DataItem != null))
        {
            //FORCE RELOAD OF DATA ITEM - NEED MORE EFFICIENT SOLUTION TO REFRESH GRID FROM BATCH UPDATES
            //THIS PREVENTS DATA FROM DATABASE AND GRID BECOMING UNSYNCRHONIZED
            Order order = OrderDataSource.Load(((Order)e.Row.DataItem).OrderId, false);
            e.Row.DataItem = order;
        }
    }

    protected void OrderGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if ((e.Row != null) && (e.Row.RowType == DataControlRowType.DataRow) && (e.Row.DataItem != null))
        {
            Order order = (Order)e.Row.DataItem;
            PlaceHolder phPaymentStatus = PageHelper.RecursiveFindControl(e.Row, "phPaymentStatus") as PlaceHolder;
            if (phPaymentStatus != null)
            {
                Image paymentStatus = new Image();
                paymentStatus.SkinID = GetPaymentStatusSkinID(order);
                phPaymentStatus.Controls.Add(paymentStatus);
            }
            PlaceHolder phShipmentStatus = PageHelper.RecursiveFindControl(e.Row, "phShipmentStatus") as PlaceHolder;
            if (phShipmentStatus != null)
            {
                Image shipmentStatus = new Image();
                shipmentStatus.SkinID = GetShipmentStatusSkinID(order);
                phShipmentStatus.Controls.Add(shipmentStatus);
            }


            PlaceHolder phProcessorCaption = PageHelper.RecursiveFindControl(e.Row, "phProcessorCaption") as PlaceHolder;
            if (phProcessorCaption != null)
            {
                Image PaymentInstrument = new Image();
                PaymentInstrument.SkinID = GetPaymentInstrumentSkinID(order);
             
                    if (!string.IsNullOrEmpty(PaymentInstrument.SkinID))
                        phProcessorCaption.Controls.Add(PaymentInstrument);
            }

        }
    }

    protected void OrderGrid_Sorting(object sender, GridViewSortEventArgs e)
    {
        //Sorting Viewstate datasource based on the SortExpression selected

      //  LoadLastSearch();

        string col = e.SortExpression.ToString();
      //  SortColumn = col;

     //   if ((SortDirection == "Asc") && (col == SortColumn))
    //    {
   //         SortDirection = "Desc";
   //         SessionDataSource.DefaultView.Sort = string.Concat(col, " desc");
   //     }

   //     else if ((SortDirection == "Desc") && (col == SortColumn))
  //      {
  //          SortDirection = "Asc";
  //          SessionDataSource.DefaultView.Sort = string.Concat(col, " asc");
  //      }



        //bind grid to trigger a refresh
       // GridChargeBind(SessionDataSource);

    }

    protected string GetPaymentInstrumentSkinID(Order order)
    {
        //if (order.OrderStatus.Name.Equals("Cancelled", StringComparison.InvariantCultureIgnoreCase)) return "CodeGreen";
      //  if (!order.OrderStatus.IsValid) return "CodeRed";
        if (order.Payments.Count > 0)
        {
            switch (order.Payments.LastPayment.PaymentInstrumentId)
            {
                case (short)PaymentInstrument.AmericanExpress: return "Amex";
                case (short)PaymentInstrument.Check: return "Check";
                case (short)PaymentInstrument.DinersClub: return "Diners";
                case (short)PaymentInstrument.Discover: return "Discover";
                case (short)PaymentInstrument.MasterCard: return "MasterCard";
                case (short)PaymentInstrument.PayPal: return "Paypal";
                case (short)PaymentInstrument.Visa: return "Visa";
                case (short)PaymentInstrument.GiftCertificate: return "GiftCard";
                case (short)PaymentInstrument.JCB: return "Jcb";
                case (short)PaymentInstrument.Maestro: return "Maestro";
                case (short)PaymentInstrument.SwitchSolo: return "Solo";
                case (short)PaymentInstrument.Unknown: return "Unknown";
                case (short)PaymentInstrument.GoogleCheckout: return "Google";
                case (short)PaymentInstrument.VisaDebit: return "VisaDebit";
                case (short)PaymentInstrument.PurchaseOrder: return "PurchaseOrder";
                case (short)PaymentInstrument.PhoneCall: return "Phone";
                case (short)PaymentInstrument.Mail: return "Mail";

            }

            return "Unknown";
        }
        else return "";
    }

    protected string GetPaymentStatusSkinID(Order order)
    {
        //if (order.OrderStatus.Name.Equals("Cancelled", StringComparison.InvariantCultureIgnoreCase)) return "CodeGreen";
		if (!order.OrderStatus.IsValid) return "CodeRed";
        
        switch (order.PaymentStatus)
        {
            case OrderPaymentStatus.Unspecified:
            case OrderPaymentStatus.Unpaid:
                return "CodeYellow";
            case OrderPaymentStatus.Problem:
                return "CodeRed";
            default:
                return "CodeGreen";
        }
    }

    protected string GetShipmentStatusSkinID(Order order)
    {
        //if (order.OrderStatus.Name.Equals("Cancelled", StringComparison.InvariantCultureIgnoreCase)) return "CodeGreen";
        if (!order.OrderStatus.IsValid) return "CodeRed";
        
        switch (order.ShipmentStatus)
        {
            case OrderShipmentStatus.Unspecified:
            case OrderShipmentStatus.Unshipped:
                return "CodeYellow";
            default:
                return "CodeGreen";
        }
    }

    protected void OrderGrid_DataBound(object sender, EventArgs e)
    {
        List<int> orderIds = new List<int>();
        foreach (GridViewRow gvr in OrderGrid.Rows)
        {
            int index = gvr.DataItemIndex - (OrderGrid.PageIndex * OrderGrid.PageSize);
            orderIds.Add((int)((GridView)sender).DataKeys[index].Value);
            CheckBox cb = (CheckBox)gvr.FindControl("Selected");
            ScriptManager.RegisterArrayDeclaration(OrderGrid, "CheckBoxIDs", String.Concat("'", cb.ClientID, "'"));
        }
        Session["LastOrderList"] = orderIds;
        selectedOrdersPanel.Visible = (OrderGrid.Rows.Count > 0);
    }

    protected void BatchButton_Click(object sender, EventArgs e)
    {
        List<string> messages = new List<string>();
        List<int> orderIds = GetSelectedOrderIds();
        if (orderIds.Count > 0)
        {
            if (BatchAction.SelectedValue.StartsWith("OS_"))
            {
                //UPDATE ORDER STATUS REQUESTED
                int orderStatusId = AlwaysConvert.ToInt(BatchAction.SelectedValue.Substring(3));
                //VALIDATE STATUS
                OrderStatus status = OrderStatusDataSource.Load(orderStatusId);
                if (status != null)
                {
                    foreach (int orderId in orderIds)
                    {
                        Order order = OrderDataSource.Load(orderId);
                        if (order != null)
                        {
                            order.UpdateOrderStatus(status);
                        }
                    }
                }
            }
            else
            {
                switch (BatchAction.SelectedValue)
                {
                    case "INVOICE":
                        Response.Redirect("Print/Invoices.aspx?orders=" + GetOrderList(orderIds));
                        break;
                    case "PACKSLIP":
                        Response.Redirect("Print/PackSlips.aspx?orders=" + GetOrderList(orderIds));
                        break;
                    case "PULLSHEET":
                        Response.Redirect("Print/PullSheet.aspx?orders=" + GetOrderList(orderIds));
                        break;
                    case "CANCEL":
                        Response.Redirect("Batch/Cancel.aspx?orders=" + GetOrderList(orderIds));
                        break;
                    case "SHIPOPT":
                        Response.Redirect("Batch/Ship.aspx?orders=" + GetOrderList(orderIds));
                        break;
                    case "SHIP":
                        int shipCount = 0;
                        foreach (int orderId in orderIds)
                        {
                            Order order = OrderDataSource.Load(orderId);
                            if (order != null && order.Shipments != null)
                            {
                                bool shipped = false;
                                int shipmentCount = order.Shipments.Count;
                                for (int i = 0; i < shipmentCount; i++)
                                {
                                    OrderShipment shipment = order.Shipments[i];
                                    if (shipment != null && !shipment.IsShipped)
                                    {
                                        shipment.Ship();
                                        shipped = true;
                                    }
                                }
                                if (shipped)
                                {
                                    messages.Add("Order #" + order.OrderNumber + " shipped.");
                                    shipCount++;
                                }
                                else messages.Add("Order #" + order.OrderNumber + " did not have any unshipped items.");
                            }
                        }
                        messages.Add(shipCount + " orders shipped.");
                        break;
                    case "PAY":
                        int payCount = 0;
                        foreach (int orderId in orderIds)
                        {
                            Order order = OrderDataSource.Load(orderId);
                            if (order != null)
                            {
                                bool paid = false;
                                int paymentCount = order.Payments.Count;
                                for (int i = 0; i < paymentCount; i++)
                                {
                                    Payment payment = order.Payments[i];
                                    if (payment.PaymentStatus == PaymentStatus.Authorized)
                                    {
                                        payment.Capture(payment.Amount, true);
                                        paid = true;
                                    }
                                    else if (payment.PaymentStatus == PaymentStatus.Unprocessed)
                                    {
                                        payment.Authorize();
                                        paid = true;
                                    }
                                }
                                if (paid)
                                {
                                    payCount++;
                                    messages.Add("Order " + order.OrderNumber.ToString() + " processed.");
                                }
                                else messages.Add("Order " + order.OrderNumber.ToString() + " does not have any payments to be processed.");
                            }
                        }
                        messages.Add(payCount + " orders processed.");
                        break;
                }
            }
        }
        if (messages.Count > 0)
        {
            trBatchMessage.Visible = true;
            BatchMessage.Text = string.Join("<br />", messages.ToArray());
        }
        BatchAction.SelectedIndex = -1;
        OrderGrid.DataBind();
    }

    private string GetOrderList(List<int> orderIds)
    {
        Dictionary<int, int> orderNumberLookup = OrderDataSource.LookupOrderNumbers(orderIds.ToArray());
        List<string> orderNumberList = new List<string>();
        foreach (int id in orderNumberLookup.Keys)
            orderNumberList.Add(orderNumberLookup[id].ToString());
        return string.Join(",", orderNumberList.ToArray());
    }

    protected OrderSearchCriteria GetOrderSearchCriteria()
    {
        // CREATE CRITERIA INSTANCE
        OrderSearchCriteria criteria = new OrderSearchCriteria();
        if (OrderStartDate.SelectedStartDate > DateTime.MinValue)
            criteria.OrderDateStart = OrderStartDate.SelectedStartDate;
        if (OrderEndDate.SelectedEndDate > DateTime.MinValue && OrderEndDate.SelectedEndDate < DateTime.MaxValue)
            criteria.OrderDateEnd = OrderEndDate.SelectedEndDate.AddDays(1).AddMilliseconds(-1);

        if (OrderEndDate.SelectedEndDate > DateTime.Today.AddDays(1).AddMilliseconds(-1))
            criteria.OrderDateEnd = DateTime.Today.AddDays(1).AddMilliseconds(-1);

        if (!string.IsNullOrEmpty(OrderNumberFilter.Text))
        {
            criteria.OrderDateEnd = DateTime.MaxValue;
            criteria.OrderNumberRange = OrderNumberFilter.Text;
        }
        criteria.PaymentStatus = (OrderPaymentStatus)AlwaysConvert.ToByte(PaymentStatusFilter.SelectedValue);
        criteria.ShipmentStatus = (OrderShipmentStatus)AlwaysConvert.ToByte(ShipmentStatusFilter.SelectedValue);
        // ADD IN ORDER STATUS FILTER
        int statusId = 0;
        if (StatusFilter.SelectedValue == "-2")
        {
            OrderStatusCollection statuses = OrderStatusDataSource.LoadForStore();            
            foreach (OrderStatus status in statuses)
            {
                if (status.IsValid)
                {
                    criteria.OrderStatus.Add(status.OrderStatusId);
                }
            }
        }
        else if (StatusFilter.SelectedValue == "-3")
        {
            OrderStatusCollection statuses = OrderStatusDataSource.LoadForStore();
            foreach (OrderStatus status in statuses)
            {
                if (!status.IsValid)
                {
                    criteria.OrderStatus.Add(status.OrderStatusId);
                }
            }
        }
        else
        {
            statusId =  AlwaysConvert.ToInt(StatusFilter.SelectedValue);
            if (statusId > 0) criteria.OrderStatus.Add(statusId);
        }


        foreach (TableRow row in ItemTable.Rows)
        {
            // ADD IN KEYWORD FILTER
            if (!string.IsNullOrEmpty(((TextBox)row.Cells[0].Controls[1]).Text))
            {
                MatchCriteria keyword = new MatchCriteria();
                keyword.FieldName = ((DropDownList)row.Cells[1].Controls[0]).SelectedValue;
                keyword.FieldValue = ((TextBox)row.Cells[0].Controls[1]).Text.Trim(); 
                criteria.Filter.Add(keyword);
            }
        }

        decimal price ;
        // ADD IN PRICE AMOUNT FILTER
        if (!string.IsNullOrEmpty(AmountMaxPriceFilter.Text))
        {
            price = 0;
            decimal.TryParse(this.AmountMaxPriceFilter.Text, out price);
            criteria.MaxOrderPrice = price;


        }

        if (!string.IsNullOrEmpty(AmountMiniPriceFilter.Text))
        {
            price = 0;
            decimal.TryParse(this.AmountMiniPriceFilter.Text, out price);
            criteria.MiniOrderPrice = price;

        }
        

        // RETURN THE CRITERIA OBJECT
        Session["OrderSearchCriteria"] = criteria;
        Session["OrderSearchCriteriaSelectedStatus"] = StatusFilter.SelectedValue;
        return criteria;
    }

    protected void OrderDs_Selecting(object sender, System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs e)
    {
        // ADD IN THE SEARCH CRITERIA
        e.InputParameters["criteria"] = GetOrderSearchCriteria();
        //e.InputParameters["maximumRows"] = 50; // was 200;
        //e.InputParameters["startRowIndex"] = 0;
        
    }

    protected void JumpToOrderButton_Click(Object sender, EventArgs e)
    {
        int tempOrderNumber = AlwaysConvert.ToInt(JumpToOrderNumber.Text);
        Order order = OrderDataSource.Load(OrderDataSource.LookupOrderId(tempOrderNumber));
        if (order != null)
        {
            Response.Redirect("~/Admin/Orders/ViewOrder.aspx?OrderNumber=" + order.OrderNumber.ToString() + "&OrderId=" + order.OrderId.ToString());
        }
        else
        {
            CustomValidator invalidOrderId = new CustomValidator();
            invalidOrderId.ControlToValidate = "JumpToOrderNumber";
            invalidOrderId.Text = "not found, try search!";
            invalidOrderId.IsValid = false;
            invalidOrderId.ValidationGroup = "JumpToOrder";
            phJumpToOrder.Controls.Add(invalidOrderId);
        }
    }
  
    protected string GetChargeInfo(object dataItem)
    {
        if (Token.Instance.User.IsGateway)
            return "";
        Order order = (Order)dataItem;
        string chargeDescriptors = string.Empty;
        string customerServicePhones = string.Empty;

        bool rtn = order.GetChargeInfo(out chargeDescriptors, out customerServicePhones);

        if (rtn == true)
            return chargeDescriptors;
        if (order.Payments.LastPayment != null)
            return order.Payments.LastPayment.PaymentGateway.Name.Replace("_"," ");
        return string.Empty;

    }

    protected string GetResponseMessage(object dataItem)
    {
        Order order = (Order)dataItem;
        string rtn = string.Empty;
        try
        {
            DateTime dt  = DateTime.MinValue;
            foreach (Transaction t in order.Payments.LastPayment.Transactions)
            {
                if ((t.TransactionDate > dt) && (!string.IsNullOrEmpty(t.ResponseMessage)) && (!string.IsNullOrEmpty(t.ResponseCode)))
                {
                    dt = t.TransactionDate;
                    rtn = "(" + t.ResponseCode + ")" + t.ResponseMessage;
                }
                else
                {
                    rtn = "&nbsp;";
                }
                
            }
        }
        catch { }
        return rtn.Replace("\0","");

    }

}
