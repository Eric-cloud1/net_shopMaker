using System;
using System.Web.UI.WebControls;
using MakerShop.Orders;
using MakerShop.Orders.ChargeBack;
using MakerShop.Utility;
using MakerShop.Common;
using MakerShop.Payments;


public partial class Admin_Reports_ChargeBack_PaymentChargeBack : System.Web.UI.Page
{
    ChargeBackDetails chargeBackdetails = null;
    int _paymentid = 0;     
    int _orderId = 0;
    private Order _Order = null;
        

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!int.TryParse(Request.QueryString["PaymentId"], out _paymentid) || !int.TryParse(Request.QueryString["OrderId"], out _orderId))
            Response.Redirect("Default.aspx");

        if (!IsPostBack)
        {
            
            ChargeBackDate.SelectedDate = DateTime.Now.Date.AddDays(-2);

            dlChargebackStatus.Items.Clear();
            foreach (ChargeBackStatus cs in Enum.GetValues(typeof(ChargeBackStatus)))
            {
               dlChargebackStatus.Items.Add(new ListItem(cs.ToString(),((short)cs).ToString()));
            }


          //  dlChargebackStatus.DataSource = Enum.GetNames(typeof(ChargeBackStatus));
          //  dlChargebackStatus.DataBind();
            
           // int paymentid = 0;
           // int.TryParse(Request.QueryString["PaymentId"], out paymentid);

            TransactionCollection tc = MakerShop.Payments.TransactionDataSource.LoadForCriteria("PaymentID = " + _paymentid.ToString() + " AND TransactionStatusId =1 AND TransactionTypeId IN (1,4)");

            if (tc.Count > 0)
            {
                this.dlProviderTransactionId.DataSource = tc;
                this.dlProviderTransactionId.DataTextField = "ProviderTransactionId";
                this.dlProviderTransactionId.DataValueField = "TransactionId";
                this.dlProviderTransactionId.DataBind();

                bindDetails();
            }
            else { dlProviderTransactionId.Items.Add(new ListItem("- Transaction FAILED -", "0")); }

        }
    }

    protected void bindDetails()
    {
        int paymentid = 0;
        int.TryParse(Request.QueryString["PaymentId"], out paymentid);
        int orderId = 0;
        int.TryParse(Request.QueryString["OrderId"], out orderId);

        ChargeBackDetailsCollection cbdc = ChargeBackDetailsDataSource.LoadForCriteria("OrderId = " + orderId.ToString() + " AND PaymentID = " + paymentid.ToString() + " AND TransactionID =" + dlProviderTransactionId.SelectedValue);
        if (cbdc.Count > 0)
        {
            chargeBackdetails = cbdc[0];

            this.lCreatedDate.Text = string.Format(@"Charge Back Created on: {0:g}", LocaleHelper.ToLocalTime(chargeBackdetails.CreateDate));
            this.lCreatedDate.Visible = true;
            chargeBackCaseNumber.Text = chargeBackdetails.CaseNumber;
            chargeBackReasonCode.Text = chargeBackdetails.ReasonCode;
            chargeBackDescription.Text = chargeBackdetails.ReasonDescription;
            chargeBackReference.Text = chargeBackdetails.ReferenceNumber;

            chargeBackComment.Text = chargeBackdetails.Comment;
            ChargeBackDate.SelectedDate = chargeBackdetails.InitiateDate;
            lCreatedDate.Text = string.Format(lCreatedDate.Text, chargeBackdetails.CreateDate);

            dlChargebackStatus.SelectedValue = chargeBackdetails.ChargeBackStatus.ToString();
        }  
    }


    protected void Select_Provider(object sender, EventArgs e)
    {
        bindDetails();
    }

    protected void saveChargeBack_Click(object sender, EventArgs e)
    {
   
        chargeBackdetails = new ChargeBackDetails();
        int transactionId = int.Parse(dlProviderTransactionId.SelectedValue);

        if(transactionId == 0) return;

     
        chargeBackdetails.OrderId = _orderId;


        chargeBackdetails.TransactionId = transactionId;
        chargeBackdetails.Comment= chargeBackComment.Text;
        chargeBackdetails.ChargeBackStatus = short.Parse(dlChargebackStatus.SelectedValue);
        chargeBackdetails.PaymentId = _paymentid;
     

        chargeBackdetails.InitiateDate = ChargeBackDate.SelectedDate;
        chargeBackdetails.ReasonCode = chargeBackReasonCode.Text;
        chargeBackdetails.ReasonDescription = chargeBackDescription.Text;
        chargeBackdetails.ReferenceNumber = chargeBackReference.Text;
        chargeBackdetails.CaseNumber = chargeBackCaseNumber.Text;


        SaveResult result = chargeBackdetails.Save();
        OrderNote note = new OrderNote();

        if (result == SaveResult.Failed)
            return;

        if (result == SaveResult.RecordUpdated)
            note.Comment = "Charge back updated";

        else if (result == SaveResult.RecordInserted)
        {
            note.Comment = "Charge Back created";
            _Order = OrderHelper.GetOrderFromContext();
            _Order.Cancel(true);

        }

        note.CreatedDate = DateTime.UtcNow;
        note.NoteType= NoteType.SystemPublic;
        note.OrderId = _orderId;
        note.UserId = Token.Instance.UserId;
        note.Save();

        if(note.Save() != SaveResult.Failed)
         Response.Redirect("~/admin/orders/Payments/default.aspx?OrderId="+_orderId.ToString());      
    }
   

    protected void cancelChargeBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/admin/orders/Payments/default.aspx?OrderId=" + _orderId.ToString());
    }
   
}
