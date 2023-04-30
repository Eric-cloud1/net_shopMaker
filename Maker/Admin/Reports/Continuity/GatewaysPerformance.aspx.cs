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
using MakerShop.Reporting.Gateway;
using System.IO;

using System.Text;
using System.Xml;
using System.Reflection;

public partial class Admin_Reports_Continuity_GatewaysPerformance : System.Web.UI.Page
{

    private int delta = 0;
    private int startHour = 0;
    private int endHour = 0;
    private int startMinute = 0;
    private int endMinute = 0;

    protected DataTable SessionDataSource
    {
        get
        {
            Object ds = ViewState["DataSource_GatewaysPerformance"];
            if (ds != null)
                return (DataTable)ds; // to replace with proper object

            return null;
        }
        set { ViewState["DataSource_GatewaysPerformance"] = value; }
    }


    private string SortDirection
    {
        get
        {

            if (Session["SortDirection_GatewaysPerformance"] == null)
                return "Asc";
            return Session["SortDirection_GatewaysPerformance"].ToString();
        }
        set
        {
            Session.Add("SortDirection_GatewaysPerformance", value);
        }
    }
    private string SortColumn
    {
        get
        {

            if (Session["SortColumn_GatewaysPerformance"] == null)
                return "AffiliateID";
            return Session["SortColumn_GatewaysPerformance"].ToString();
        }
        set
        {
            Session.Add("SortColumn_GatewaysPerformance", value);
        }
    }
  

    //TODO: pivot table
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
        {
           
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

        String paymentTypeIds = string.Empty;
        foreach (ListItem ids in Payments.Items)
        {
            if (ids.Selected == true)
                paymentTypeIds += string.Concat(ids.Value, "-");
        }

        paymentTypeIds = paymentTypeIds.Substring(0, paymentTypeIds.Length - 1);


        DataTable performance = ReportDataSource.GetGatewayPerformanceByDatePromoSubAffiliate(dtGatewaysPerformance.StartDate, dtGatewaysPerformance.EndDate,
        this.txtAffiliates.Text, this.txtSubAffiliate.Text, SubAffiliate, paymentTypeIds);

        SessionDataSource = GatewayPerformanceReport(performance);

        gridGatewaysPerformance.PageIndex = 0;
        gridGatewaysPerformance.DataSource = SessionDataSource.DefaultView;
        gridGatewaysPerformance.DataBind();

    }

    protected void gridGatewaysPerformance_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.gridGatewaysPerformance.PageIndex = e.NewPageIndex;
        this.gridGatewaysPerformance.DataSource = SessionDataSource.DefaultView;
        gridGatewaysPerformance.DataBind();
    }

    protected void gridGatewaysPerformance_Sorting(object sender, GridViewSortEventArgs e)
    {
    
        //Sorting Viewstate datasource based on the SortExpression selected
        string col = e.SortExpression.ToString();
        SortColumn = col;

       
        if ((SortDirection == "Asc") && (col == SortColumn))
        {
            SortDirection = "Desc";
            SessionDataSource.DefaultView.Sort = string.Concat(col, " desc");
        }

        else if ((SortDirection == "Desc") && (col == SortColumn))
        {
            SortDirection = "Asc";
            SessionDataSource.DefaultView.Sort = string.Concat(col, " asc");
        }

       

     
        //bind grid to trigger a refresh
        gridGatewaysPerformance.DataSource = SessionDataSource.DefaultView;
        gridGatewaysPerformance.DataBind();
    }

    protected void ProcessButton_Click(object sender, EventArgs e)
    {
        this.gridGatewaysPerformance.PageIndex = 1;
        this.dtGatewaysPerformance.setDateTime();
 
        BindGrid();
    }


    public static DataTable GatewayPerformanceReport(DataTable dataValues)
    {
        List<string> pivotData = new List<string>();
      //  pivotData.Add("Attempts");
       // pivotData.Add("Approvals");
        pivotData.Add("Approval%");
        DataTable dt = MakerShop.Utility.DataHelper.Pivot(dataValues, "AffiliateID", "Gateway", pivotData);
        dt.Columns.Remove("Approval%" );
        
      
        foreach (DataColumn dc in dt.Columns)
            dc.ColumnName = dc.ColumnName.Replace("Approval%", "%");


        DataColumn approvalPercent = new DataColumn();
        approvalPercent.ColumnName = "Approvals %";
        approvalPercent.DataType = typeof(decimal);
        dt.Columns.Add(approvalPercent);
   
        decimal numerator = 0;
        decimal denominator = 0;
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            denominator = (int)dt.Rows[i][1];
            numerator = (int)dt.Rows[i][2];

            dt.Rows[i].SetField<decimal>(approvalPercent, numerator * 100 / denominator);
        }

        dt.Columns["Approvals %"].SetOrdinal(3);

        return dt;
    }



    protected void CSVReport_Click(object sender, ImageClickEventArgs e)
    {

        DataTable summaries = SessionDataSource;
        MemoryStream mm = new MemoryStream();
        StreamWriter sw = new StreamWriter(mm);
        sw.AutoFlush = true;


        sw.Write(sw.NewLine);

        foreach (DataRow _row in summaries.Rows)
        {
            for (int i = 0; i < summaries.Columns.Count; i++)
            {
                sw.Write(_row[i].ToString());
                sw.Write(",");
            }

            sw.Write(sw.NewLine);
        }


        mm.Seek(0, 0);

     
        System.Net.Mail.Attachment attachement = new System.Net.Mail.Attachment(mm, "GatewayPerformance_" + dtGatewaysPerformance.StartDate.ToShortDateString() + '-' +
            dtGatewaysPerformance.EndDate.ToShortDateString() + ".csv");
        if (ViewState["SentFile"] != null)
        {
             if (ViewState["SentFile"].ToString() == attachement.Name)
                 return;
             else
            ViewState.Remove("SentFile");
        }
        ViewState.Add("SentFile", attachement.Name);
        System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
        mail.From = new System.Net.Mail.MailAddress("viroplex@viralplex.com");
        mail.Subject = "GatewayPerformance_" + dtGatewaysPerformance.StartDate.ToShortDateString() + '-' + dtGatewaysPerformance.EndDate.ToShortDateString();
        mail.To.Add(new System.Net.Mail.MailAddress(MakerShop.Common.Token.Instance.User.Email));
        mail.Attachments.Add(attachement);
        MakerShop.Messaging.EmailClient.Send(mail);
    }



    protected void ExcelReport_Click(object sender, ImageClickEventArgs e)
    {

        //TODO: export to excel
    }

   

    protected void gridGatewaysPerformance_RowDataBound(object sender, GridViewRowEventArgs e)
    {


        if (e.Row.RowType == DataControlRowType.DataRow)

            for (int i = 0; i <= e.Row.Cells.Count - 1; i++)
            {

                if (((DataRowView)e.Row.DataItem)[i].GetType().FullName == typeof(int).FullName)
                {
                    e.Row.Cells[i].Attributes.Add("align", "right");

                }
                else if (((DataRowView)e.Row.DataItem)[i].GetType().FullName == typeof(decimal).FullName)
                {
                    e.Row.Cells[i].Text = string.Format("{0:0.00%}", decimal.Parse(e.Row.Cells[i].Text) / 100);
                    e.Row.Cells[i].Attributes.Add("align", "right");
                }

            }
    }
}
