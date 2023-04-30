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
using System.Linq;
using MakerShop.Reporting.Continuity;
using System.IO;

using System.Text;
using System.Xml;
using System.Reflection;

public partial class Admin_Reports_Continuity_ConversionsPerformance : System.Web.UI.Page
{


    protected DataView SessionDataSource
    {
        get
        {
            DataView dv;
            if (Session["DataSource_Conversion"] != null)
            {
                dv = Session["DataSource_Conversion"] as DataView;
                dv.Sort = SortColumn + " " + SortDirection;
                return dv;
            }
            else
                return null;
        }
        set { Session["DataSource_Conversion"] = value; }
    }

    private string SortDirection
    {
        get
        {

            if (Session["SortDirection_Conversion"] == null)
                return "Asc";
            return Session["SortDirection_Conversion"].ToString();
        }
        set
        {
            Session.Add("SortDirection_Conversion", value);
        }
    }
    private string SortColumn
    {
        get
        {

            if (Session["SortColumn_Conversion"] == null)
                return "AffiliateID";
            return Session["SortColumn_Conversion"].ToString();
        }
        set
        {
            Session.Add("SortColumn_Conversion", value);
        }
    }


    protected void Page_Init(object sender, EventArgs e)
    {
      if (dtConversionsPerformance.FirstHit)
        {
            dtConversionsPerformance.TimePeriod = Admin_UserControls_DatesAndTime.Time_Period.Today;

      }
        dtConversionsPerformance.Update += new Admin_UserControls_DatesAndTime.UpdateEventHandler(BindGrid);

      }

  

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {

          

            dtConversionsPerformance.TimePeriod = 0;

            if (this.txtAffiliates.Text == this.twAffiliates.WatermarkText)
                this.txtAffiliates.Text = string.Empty;

            if (this.txtSubAffiliate.Text == twSubAffiliates.WatermarkText)
                this.txtSubAffiliate.Text = string.Empty;

            BindGrid();
        }
    }



    protected void BindGrid()
    {

        int SubAffiliate = 0;

        if (chkSubAffiliate.Checked)
            SubAffiliate = 1;

        if (this.txtAffiliates.Text == this.twAffiliates.WatermarkText)
            this.txtAffiliates.Text = string.Empty;

        if (this.txtSubAffiliate.Text == twSubAffiliates.WatermarkText)
            this.txtSubAffiliate.Text = string.Empty;
        DataTable dt = ReportDataSource.GetConversionsPerformanceByDatePromoSubAffiliate(dtConversionsPerformance.StartDate, dtConversionsPerformance.EndDate,
        this.txtAffiliates.Text, this.txtSubAffiliate.Text, SubAffiliate);
        if (dt != null)
            SessionDataSource = dt.DefaultView;
        else
            SessionDataSource = null;

        gridConversionsPerformance.PageIndex = 0;
        gridConversionsPerformance.DataSource = SessionDataSource;
        gridConversionsPerformance.DataBind();
    }


    protected void gridConversionsPerformance_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.gridConversionsPerformance.PageIndex = e.NewPageIndex;
        this.gridConversionsPerformance.DataSource = SessionDataSource;
        gridConversionsPerformance.DataBind();
    }



    protected void gridConversionsPerformance_Sorting(object sender, GridViewSortEventArgs e)
    {
        //Sorting Viewstate datasource based on the SortExpression selected
        string col = e.SortExpression.ToString();
        SortColumn = col;

       
        if ((SortDirection == "Asc") && (col == SortColumn))
        {
            SortDirection = "Desc";
            SessionDataSource.Sort = string.Concat(col, " desc");
        }

        else if ((SortDirection == "Desc") && (col == SortColumn))
        {
            SortDirection = "Asc";
            SessionDataSource.Sort = string.Concat(col, " asc");
        }

       

        //bind grid to trigger a refresh
        gridConversionsPerformance.DataSource = SessionDataSource;
        gridConversionsPerformance.DataBind();

    }


    protected void ProcessButton_Click(object sender, EventArgs e)
    {
        this.gridConversionsPerformance.PageIndex = 1;
        this.dtConversionsPerformance.setDateTime();

        BindGrid();
    }

    protected string formatOrderDate(object dataItem)
    {
        DataRowView row = (DataRowView)dataItem;
        DateTime LastOrderDate = (DateTime)row["LastOrderDate"];

        TimeSpan span = new TimeSpan();
        if (LastOrderDate != DateTime.MinValue)
        {
            span = DateTime.UtcNow.Subtract(LastOrderDate);
            return string.Format(@"{0}d:{1}h:{2}m:{3}s", span.Days, span.Hours, span.Minutes, span.Seconds);
        }

        return string.Empty;

    }


    protected void ExcelReport_Click(object sender, ImageClickEventArgs e)
    {

       //TODO: export to excel
    }




    protected void CSVReport_Click(object sender, ImageClickEventArgs e)
    {

        DataView summaries = SessionDataSource;
        MemoryStream mm = new MemoryStream();
        StreamWriter sw = new StreamWriter(mm);
        sw.AutoFlush = true;

        //TODO: Pivot table routine
        sw.Write(sw.NewLine);


        foreach (DataRow _row in summaries.Table.Rows)
        {

            sw.Write(_row[0].ToString());
            sw.Write(",");


            sw.Write(_row["AffiliateID"].ToString());
            sw.Write(",");

            sw.Write(_row["Name"].ToString());
            sw.Write(",");

            sw.Write(_row["SubAffiliateId"].ToString());
            sw.Write(",");
            sw.Write(_row["Clicks"].ToString());
            sw.Write(",");
            sw.Write(_row["Leads"].ToString());
            sw.Write(",");

            sw.Write(_row["Orders"].ToString());
            sw.Write(",");

            sw.Write(_row["[ClicksToOrderPercent]"].ToString());
            sw.Write(",");

            sw.Write(_row["[ClicksToLeadPercent]"].ToString());
            sw.Write(",");

            sw.Write(_row["Attempts"].ToString());
            sw.Write(",");

            sw.Write(_row["Approvals"].ToString());
            sw.Write(",");


            sw.Write(_row["ApprovalPercent"].ToString());
            sw.Write(",");


            sw.Write(_row["[LastOrderDate]"].ToString());
            sw.Write(",");


            sw.Write(sw.NewLine);
        }
        sw.Flush();

        mm.Seek(0, 0);


        System.Net.Mail.Attachment attachement = new System.Net.Mail.Attachment(mm, "ConversionsPerformance_" + dtConversionsPerformance.StartDate.ToShortDateString() + '-' +
            dtConversionsPerformance.EndDate.ToShortDateString() + ".csv");
        if (ViewState["SentFile"] != null)
        {
            // if (ViewState["SentFile"].ToString() == attachement.Name)
            //     return;
            // else
            ViewState.Remove("SentFile");
        }
        ViewState.Add("SentFile", attachement.Name);
        System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
        mail.From = new System.Net.Mail.MailAddress("viroplex@viralplex.com");
        mail.Subject = "ConversionsPerformance_" + dtConversionsPerformance.StartDate.ToShortDateString() + '-' + dtConversionsPerformance.EndDate.ToShortDateString();
        mail.To.Add(new System.Net.Mail.MailAddress(MakerShop.Common.Token.Instance.User.Email));
        mail.Attachments.Add(attachement);
        MakerShop.Messaging.EmailClient.Send(mail);
    }



    string AffiliateID = null;
    protected void gridConversionsPerformance_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.DataRow)
            return;
        if (SortColumn == "AffiliateID")
        {
            if (AffiliateID == null)
                AffiliateID = ((DataRowView)e.Row.DataItem)["AffiliateID"].ToString();
            else if (((DataRowView)e.Row.DataItem)["AffiliateID"].ToString() == AffiliateID)
                return;
            else
            {
                AffiliateID = ((DataRowView)e.Row.DataItem)["AffiliateID"].ToString();
                
                foreach (TableCell cell in e.Row.Cells)
                {
                    cell.Style.Add("border-top-color", "BLUE");
                }

            }
        }
        else if (SortColumn == "Affiliate")
        {
            if (AffiliateID == null)
                AffiliateID = ((DataRowView)e.Row.DataItem)["Affiliate"].ToString();
            else if (((DataRowView)e.Row.DataItem)["Affiliate"].ToString() == AffiliateID)
                return;
            else
            {
                AffiliateID = ((DataRowView)e.Row.DataItem)["Affiliate"].ToString();
                
                foreach (TableCell cell in e.Row.Cells)
                {
                    cell.Style.Add("border-top-color", "BLUE");
                }

            }
        }
    }
}
