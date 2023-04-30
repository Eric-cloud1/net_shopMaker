using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using MakerShop.Common;
using MakerShop.Data;

using MakerShop.Reporting.Gateway;


public partial class Admin_Dashboard_Attempts : System.Web.UI.UserControl
{

    private int delta = 0;

    protected DataTable SessionDataSource
    {
        get
        {
            Object ds = ViewState["DataSource"];
            if (ds != null)
                return (DataTable)ds; // to replace with proper object

            return null;
        }
        set { ViewState["DataSource"] = value; }
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            dtDashBoardAttempts.TimePeriod = 0;

            dtDashBoardAttempts.StartDate = System.DateTime.Today;
            dtDashBoardAttempts.EndDate = System.DateTime.Today.AddDays(1);

        }
        dtDashBoardAttempts.Update += new Admin_UserControls_DatesAndTime.UpdateEventHandler(BindGrid);
    }


    protected void Page_Load(object sender, EventArgs e)
    {



        if (!IsPostBack)
        {
            this.ReportTime.Text = "As of " + System.DateTime.Now.ToShortTimeString();

        }
        BindGrid();

    }

  

    private void BindGrid()
    {

        //  DateTime paramStartDate = DateTime.Parse(startDate.Value);
        //   ConversionReport.DataSource = ReportDataSource.GetTrackingTotalCounts(paramStartDate, DateTime.MaxValue);
        //   ConversionReport.Visible = true;
        //   ConversionReport.DataBind();


        DataTable performance = ReportDataSource.GetGatewayPerformanceByDatePromoSubAffiliate(dtDashBoardAttempts.StartDate, dtDashBoardAttempts.EndDate,
    string.Empty, string.Empty, 1,"1");

        SessionDataSource = GatewayPerformanceReport(performance);


        this.AttemptReport.DataSource = SessionDataSource;
        AttemptReport.DataBind();
    }




 
   

    protected void AttemptReport_Sorting(object sender, GridViewSortEventArgs e)
    {
        //Sorting Viewstate datasource based on the SortExpression selected
        string order = e.SortExpression.ToString();

        DataTable performance = SessionDataSource;

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
        AttemptReport.DataSource = performance;
        AttemptReport.DataBind();
    }


    public static DataTable GatewayPerformanceReport(DataTable dataValues)
    {
        List<string> pivotData = new List<string>();
        // pivotData.Add("Attempts");
        // pivotData.Add("Approvals");
        pivotData.Add("Approval%");
        DataTable dt = MakerShop.Utility.DataHelper.Pivot(dataValues, "AffiliateID", "Gateway", pivotData);

        dt.Columns.Remove("Approval%");
        
      
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


    protected void AttemptReport_RowDataBound(object sender, GridViewRowEventArgs e)
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
