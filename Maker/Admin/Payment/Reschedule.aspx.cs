using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
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
using MakerShop.Payments;
using System.Collections.Generic;
using MakerShop.Common;
using MakerShop.Utility;
using MakerShop.Catalog;
using MakerShop.Orders;
using MakerShop.Products;
using MakerShop.Stores;
using MakerShop.Users;


using MakerShop.Reporting.Trial;


public partial class Admin_Payment_Reschedule : MakerShop.Web.UI.MakerShopAdminPage
{
    protected DataTable DetailsDataSource
    {
        get
        {
            Object ds = ViewState["DetailsDS_Reschedule"];
            if (ds != null)
                return (DataTable)ds; // to replace with proper object

            return null;
        }
        set { ViewState["DetailsDS_Reschedule"] = value; }
    }
    protected DataTable SummaryDataSource
    {
        get
        {
            Object ds = ViewState["SummaryDS_Reschedule"];
            if (ds != null)
                return (DataTable)ds; // to replace with proper object

            return null;
        }
        set { ViewState["SummaryDS_Reschedule"] = value; }
    }

    public DateTime StartDate
    {
        get
        {
            if (Session["Reschedule_StartDate"] != null)
                return (DateTime)Session["Reschedule_StartDate"];

            return DateTime.Now.Date.AddDays((DateTime.Now.Day - 1) * -1);
        }

        set
        {
            Session["Reschedule_StartDate"] = value;
        }
    }

    public DateTime EndDate
    {
        get
        {
            if (Session["Reschedule_EndDate"] != null)
                return (DateTime)Session["Reschedule_EndDate"];

            return DateTime.Today;
        }

        set
        {
            Session["Reschedule_EndDate"] = value;
        }
    }

    private byte reschedule = 0;
    private int reportType = -1;
    private string paymentTypes = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            queueDropdown.DataSource = QueueDataSource.List();
            queueDropdown.DataBind();
            ddlToQueue.DataSource = QueueDataSource.List();
            ddlToQueue.DataBind();

            if (string.IsNullOrEmpty(Request.QueryString["type"]))
            {
                cbDetails.Checked = false;
                queueDropdown.Enabled = true;
                ddlToQueue.Enabled = true;
                btnYes.Enabled = true;
                DetailsDataSource = null;
                PullReport();

            }
            else
            {
                queueDropdown.SelectedValue = Request.QueryString["type"];
                ddlToQueue.SelectedValue = Request.QueryString["type"];
                cbDetails.Visible = false;
                foreach (DataControlField dcf in gridReschedule.Columns)
                {
                    if (dcf.SortExpression == "Processed")
                    {
                        gridReschedule.Columns.Remove(dcf);
                        break;
                    }

                }
                ViewState["SortDirection"] = "Asc";
                queueDropdown.Enabled = false;
                ddlToQueue.Enabled = false;
                DateTime startdate = DateTime.MinValue, enddate = DateTime.MaxValue;
                string gatewayprefix = string.Empty, sku = string.Empty, Affiliate = string.Empty, subAffiliate = string.Empty;
                short dateType = 1;
                int reportType = -1;
                if (!String.IsNullOrEmpty(Request.QueryString["Gateway"]))
                    gatewayprefix = Request.QueryString["Gateway"].Trim();
                if (!String.IsNullOrEmpty(Request.QueryString["SKU"]))
                    sku = Request.QueryString["SKU"].Trim();


                if (!String.IsNullOrEmpty(Request.QueryString["OrderDate"]))
                {
                    startdate = DateTime.Parse(Request.QueryString["OrderDate"]);
                    dateType = 1;
                }

                else if (!String.IsNullOrEmpty(Request.QueryString["PaymentDate"]))
                {
                    startdate = DateTime.Parse(Request.QueryString["PaymentDate"]);
                    dateType = 3;
                }
                else if (!String.IsNullOrEmpty(Request.QueryString["TransactionDate"]))
                {
                    startdate = DateTime.Parse(Request.QueryString["TransactionDate"]);
                    dateType = 2;
                }


                if (!String.IsNullOrEmpty(Request.QueryString["reportType"]))
                {
                    int.TryParse(Request.QueryString["reportType"].Trim(), out reportType);
                }

                if (!String.IsNullOrEmpty(Request.QueryString["paymentTypes"]))
                {
                    paymentTypes = Request.QueryString["paymentTypes"].Replace(",", "-");
                }



                HttpCookie subaffiliate = Request.Cookies["subaffiliate"];
                if ((subaffiliate != null) && (subaffiliate.Value != string.Empty))
                    subAffiliate = subaffiliate.Value;


                HttpCookie affiliates = Request.Cookies["affiliates"];
                if ((affiliates != null) && (affiliates.Value != string.Empty))
                    Affiliate = affiliates.Value;



                if (!string.IsNullOrEmpty(Request.QueryString["reschedule"]))
                    reschedule = 1;


                if ((Request.QueryString["EndDate"] != null) && (Request.QueryString["EndDate"] != string.Empty))
                    enddate = DateTime.Parse(Request.QueryString["EndDate"]);
                else
                    enddate = startdate.AddDays(1);
                SummaryDataSource = null;
                DetailsDataSource = ReportDataSource.GetTransactionsByDatePromoSubAffiliate(dateType, startdate,
                     enddate, Affiliate.Trim(), 0, subAffiliate.Trim(), 0, reportType, paymentTypes, gatewayprefix, sku, null);


                TitleCaption.Text = string.Format("Payment Reschedule {0}", Request.QueryString["type"]);


            }

            BindGrid();
        }
        pBulk.Visible = !cbCapture.Checked;

    }
    private bool ShowDetails
    {
        get
        {
            return (cbDetails.Visible && cbDetails.Checked);
        }
    }
    private void BindGrid()
    {
        gridReschedule.DataSource = DetailsDataSource;
        gridReschedule.DataBind();
        pGrid.Visible = ShowDetails;
        if (ShowDetails)
        {
            tbCount.Enabled = false;
            tbCount.Text = "";
            cbCapture.Enabled = false;
            cbCapture.Checked = false;
            ProcessButton.Enabled = (gridReschedule.Rows.Count != 0);
        }
        else
        {
            tbCount.Enabled = true;
            cbCapture.Enabled = true;
            
            gridReschedule.DataSource = null;
            ProcessButton.Enabled = true;
        }


        if (SummaryDataSource == null)
        {
            lRows.Text = DetailsDataSource.Rows.Count.ToString();
            DataView dv = DetailsDataSource.DefaultView;
            lOrders.Text = dv.ToTable(true, "OrderNumber").Rows.Count.ToString();
            //lAmount.Text = "$ " + SessionDataSource.Compute("Sum(CAST(TransactionAmount as money))",null).ToString();
            double availableAuthA = 0;
            double attemptAuthA = 0, successAuthA = 0;
            double attemptCapA = 0, successCapA = 0;

            int availableAuthC = 0;
            int attemptAuthC = 0, successAuthC = 0;
            int attemptCapC = 0, successCapC = 0;
            DateTime a, b;
            if (DetailsDataSource.Columns.Contains("Processed"))
            {
                foreach (DataRow dr in DetailsDataSource.Rows)
                {
                     double d=0;
                     double.TryParse(dr["TransactionAmount"].ToString(), out d);
                     bool successful = false;
                     if ((DateTime.TryParse(dr["LastTransactionDate"].ToString(), out a) && (DateTime.TryParse(dr["LastSuccessTransactionDate"].ToString(), out b))))
                         successful = (a <= b);
                         
                    switch (dr["Processed"].ToString())
                    {
                        case "0":
                            ++availableAuthC;
                            availableAuthA += d;
                            break;
                        case "1":
                            ++attemptAuthC;
                            attemptAuthA += d;
                            if (successful)
                            {
                                successAuthA += d;
                                ++successAuthC;
                            }
                            break;
                        case "2": 
                            ++attemptCapC;
                            attemptCapA += d;
                            if (successful)
                            {
                                successCapA += d;
                                ++successCapC;
                            } 
                            break;
                    }


                }
            }
            lAvailableAuth.Text = availableAuthC.ToString();
            lAvailableAmountAuth.Text = availableAuthA.ToString("C");
            lAvailableCap.Text = successAuthC.ToString();
            lAvailableAmountCap.Text = successAuthA.ToString("C");
            lAttemptAmountAuth.Text = attemptAuthA.ToString("C");
            lAttemptAmountCap.Text = attemptCapA.ToString("C");
            lAttemptedCountAuth.Text = attemptAuthC.ToString();
            lAttemptedCountCap.Text =attemptCapC .ToString();


            lSuccessfulAmountAuth.Text = successAuthA.ToString("C");
            lSuccessfulAmountCap.Text = successCapA.ToString("C");
            lSuccessfulCountAuth.Text = successAuthC.ToString();
            lSuccessfulCountCap.Text = successCapC.ToString();
        }
        else
        {
            lRows.Text = SummaryDataSource.Rows[0]["Rows"].ToString();
            lOrders.Text = SummaryDataSource.Rows[0]["Orders"].ToString();

            lAvailableAuth.Text = SummaryDataSource.Rows[0]["ToAuthorize"].ToString();
            lAvailableAmountAuth.Text = ((decimal)SummaryDataSource.Rows[0]["ToAuthorizeAmount"]).ToString("C");
            lAvailableCap.Text = SummaryDataSource.Rows[0]["ToCapture"].ToString();
            lAvailableAmountCap.Text = ((decimal)SummaryDataSource.Rows[0]["ToCaptureAmount"]).ToString("C");

            lAttemptAmountAuth.Text = ((decimal)SummaryDataSource.Rows[0]["AuthorizeAttemptedAmount"]).ToString("C");
            lAttemptAmountCap.Text = ((decimal)SummaryDataSource.Rows[0]["CaptureAttemptedAmount"]).ToString("C");
            lAttemptedCountAuth.Text = SummaryDataSource.Rows[0]["AuthorizeAttemptedCount"].ToString();
            lAttemptedCountCap.Text = SummaryDataSource.Rows[0]["CaptureAttemptedCount"].ToString();


            lSuccessfulAmountAuth.Text = ((decimal)SummaryDataSource.Rows[0]["AuthorizeSuccessfulAmount"]).ToString("C");
            lSuccessfulAmountCap.Text = ((decimal)SummaryDataSource.Rows[0]["CaptureSuccessfulAmount"]).ToString("C");
            lSuccessfulCountAuth.Text = SummaryDataSource.Rows[0]["AuthorizeSuccessfulCount"].ToString();
            lSuccessfulCountCap.Text = SummaryDataSource.Rows[0]["CaptureSuccessfulCount"].ToString();

        }
    }
    public void reschedulePayment()
    {
        StartDate = RescheduleDate.SelectedDate;

        if (cbDetails.Checked)
        {
            string paymentIDs = string.Empty;
            bool flagCheck = false;
            List<string> x = new List<string>();

            foreach (GridViewRow i in gridReschedule.Rows)
            {
                CheckBox chkDelete = (CheckBox)i.FindControl("selectThis");
                Label lbPaymentId = (Label)i.FindControl("paymentId");
                if ((chkDelete.Checked) && (lbPaymentId.Text.Length > 0))
                {
                    flagCheck = true;
                    x.Add(lbPaymentId.Text);
                }
            }

            paymentIDs = string.Join(",", x.ToArray());

            if (flagCheck == false)
                return;

            
            EndDate = StartDate.AddDays(1);

            QueueDataSource.Update(ddlToQueue.SelectedItem.Text, paymentIDs, StartDate);
        }
        else
        {
            int i;
            
            if (!int.TryParse(tbCount.Text, out i) && pBulk.Visible)
            {
                Label message = new Label();
                message.SkinID = "ErrorCondition";
                message.Text = string.Format("Must have number in the count to move.");
                ErrorPanel.Controls.Add(message);
            }
            else
            {
                DateTime dt;
                if (RescheduleDate.SelectedDate != DateTime.MinValue)
                    dt = RescheduleDate.SelectedDate;
                else
                {
                    if (cbCapture.Checked)
                    {
                        Label message = new Label();
                        message.SkinID = "ErrorCondition";
                        message.Text = string.Format("Must have a Reschedule Date Selected.");
                        ErrorPanel.Controls.Add(message);
                        return;
                    }
                    else
                        dt = new DateTime(2000, 1, 1);
                }
                if (cbCapture.Checked)
                    QueueDataSource.MoveCapture(queueDropdown.SelectedItem.Text, ddlToQueue.SelectedItem.Text, dt);
                else
                    QueueDataSource.Move(queueDropdown.SelectedItem.Text, ddlToQueue.SelectedItem.Text, i, dt);
            }
        }
        if (!string.IsNullOrEmpty(Request.QueryString["reschedule"]))
            reschedule = 1;

        if (!string.IsNullOrEmpty(Request.QueryString["reportType"]))
            reportType = int.Parse(Request.QueryString["reportType"]);

        if (!string.IsNullOrEmpty(Request.QueryString["paymentTypes"]))
            paymentTypes = Request.QueryString["paymentTypes"];



    }


    protected void gridReschedule_Sorting(object sender, GridViewSortEventArgs e)
    {
        //Sorting Viewstate datasource based on the SortExpression selected
        string order = e.SortExpression.ToString();

        DataTable performance = DetailsDataSource;

        if (ViewState["SortDirection"].ToString() == "Asc")
        {
            performance.DefaultView.Sort = string.Concat(order, " asc");
            ViewState["SortDirection"] = "Desc";
        }

        else if (ViewState["SortDirection"].ToString() == "Desc")
        {
            performance.DefaultView.Sort = string.Concat(order, " desc");
            ViewState["SortDirection"] = "Asc";
        }



        //bind grid to trigger a refresh
        BindGrid();

    }

    protected void btnYes_Click(object sender, EventArgs e)
    {
        QueueDataSource.Reset(queueDropdown.SelectedItem.Text);
        reschedulePayment();
        PullReport();
        BindGrid();
        queueDropdown.Enabled = true;
    }


    protected void btnNo_Click(object sender, EventArgs e)
    {

        reschedulePayment();
        PullReport();
        BindGrid();
        queueDropdown.Enabled = true;
    }

    protected void gridReschedule_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.gridReschedule.PageIndex = e.NewPageIndex;
        this.gridReschedule.DataSource = DetailsDataSource;
        gridReschedule.DataBind();
    }

    protected string formatSubAffiliate(object dataItem)
    {
        DataRowView row = (DataRowView)dataItem;

        string subAffiliate = row["Subaffiliate"].ToString();

        if (subAffiliate.IndexOf("-") > 0)
            subAffiliate = subAffiliate.Substring(0, subAffiliate.IndexOf("-"));

        return subAffiliate;
    }

    protected string formatOrderDate(object dataItem)
    {
        DataRowView row = (DataRowView)dataItem;

        return ((DateTime)row["OrderDate"]).ToString("MM/dd/yy");
    }

    protected string formatLastTransactionDate(object dataItem)
    {
        DataRowView row = (DataRowView)dataItem;

        return ((DateTime)row["LastTransactionDate"]).ToString("MM/dd/yy");
    }

    protected string formatLastSuccessTransactionDate(object dataItem)
    {
        DataRowView row = (DataRowView)dataItem;

        if ((row["LastSuccessTransactionDate"] == null) || (row["LastSuccessTransactionDate"].ToString() == string.Empty))
            return string.Empty;


        return ((DateTime)row["LastSuccessTransactionDate"]).ToString("MM/dd/yy");
    }


    protected void queueDropdown_SelectedIndexChanged(object sender, EventArgs e)
    {
        queueReset.Enabled = !(queueDropdown.SelectedItem.Text.ToLower().Contains("rebill") || queueDropdown.SelectedItem.Text.ToLower().Contains("hold"));
        btnYes.Enabled = !(queueDropdown.SelectedItem.Text.ToLower().Contains("rebill")||queueDropdown.SelectedItem.Text.ToLower().Contains("hold"));
        ddlToQueue.SelectedIndex = queueDropdown.SelectedIndex;
        PullReport();
        BindGrid();

    }
    protected void queueReset_Click(object sender, EventArgs e)
    {
        QueueDataSource.Reset(queueDropdown.SelectedItem.Text);
        PullReport();
        BindGrid();
        queueDropdown.Enabled = true;
    }
    private void PullReport()
    {
        DateTime? dt=null;
        if (RescheduleDate.SelectedDate != DateTime.MinValue)
            dt = RescheduleDate.SelectedDate;

        DataSet ds = QueueDataSource.Report(queueDropdown.SelectedItem.Text, !ShowDetails, dt);
        if (ds == null)
        {
            SummaryDataSource = null;
            DetailsDataSource = null;
        }
        else
        {
            SummaryDataSource = ds.Tables[0];
            if ((ShowDetails) && (ds.Tables.Count > 1))
                DetailsDataSource = ds.Tables[1];
            else
                DetailsDataSource = null;
        }
    }
    protected void cbDetails_CheckedChanged(object sender, EventArgs e)
    {
        PullReport();
        BindGrid();
    }
    protected void cbCapture_CheckedChanged(object sender, EventArgs e)
    {
        if (cbCapture.Checked)
        {
            pBulk.Visible = false;
            ddlToQueue.SelectedValue = "Capture";
            ddlToQueue.Enabled = false;
            
        }
        else
        {
            pBulk.Visible = true;
            ddlToQueue.Enabled = true;
        }
    }
    protected void bRefresh_Click(object sender, EventArgs e)
    {
        PullReport();
        BindGrid();
    }
}
