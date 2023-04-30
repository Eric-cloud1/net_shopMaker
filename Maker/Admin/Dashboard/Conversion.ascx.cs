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
using MakerShop.Reporting;

public class ConversionData
{
    //TODO: build a class

}

public partial class Admin_Dashboard_Conversion : System.Web.UI.UserControl
{
    private int delta = 0;

    string SortField
    {
        get
        {
            object o = ViewState["SortField"];
            if (o == null)
            {
                return "Clicks";
            }
            return (string)o;
        }
        set
        {
            if (value == SortField)
            {
                SortAscending = !SortAscending;
            }
            ViewState["SortField"] = value;
        }
    }
    bool SortAscending
    {
        get
        {
            object o = ViewState["SortAscending"];

            if (o == null)
            {
                return false;
            }
            return (bool)o;
        }
        set
        {
            ViewState["SortAscending"] = value;
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        if (dtDashBoardConversion.FirstHit)
            dtDashBoardConversion.TimePeriod = Admin_UserControls_DatesAndTime.Time_Period.Today;

        dtDashBoardConversion.Update += new Admin_UserControls_DatesAndTime.UpdateEventHandler(BindList);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.ReportTime.Text = "As of " + System.DateTime.Now.ToShortTimeString();
        }
        BindList();
    }
    private void BindList()
    {
        Report.DataSource = ReportDataSource.GetTrackingCounts(dtDashBoardConversion.StartDate, dtDashBoardConversion.EndDate, SortField, SortAscending);
        Report.DataBind();
    }
    protected void SortGrid(Object sender, DataGridSortCommandEventArgs e)
    {
        // change sort field
        SortField = (string)e.SortExpression;

        // re-bind to display new sorting
        BindList();
    }
    protected string formatLastOrderDate(object dataItem)
    {
        TrackingCount row = (TrackingCount)dataItem;
        TimeSpan span = new TimeSpan();

        if (row.LastOrderDate.HasValue)
        {
            span = DateTime.UtcNow.Subtract(row.LastOrderDate.Value);
            return string.Format(@"{0}d:{1}h:{2}m:{3}s", span.Days, span.Hours, span.Minutes, span.Seconds);

        }
        return string.Empty;
    }
    protected string formatConversionPercent(object dataItem)
    {
        TrackingCount row = (TrackingCount)dataItem;

        decimal coversionPercent = 0;
        if (row.Clicks != 0)
            coversionPercent = (decimal)row.Orders / (decimal)row.Clicks;


        return string.Format("{0:#,##0.00%}", coversionPercent);
    }
    protected void Report_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {

            TrackingCount tc = ReportDataSource.GetTrackingTotalCounts(dtDashBoardConversion.StartDate, dtDashBoardConversion.EndDate)[0];

            e.Row.Cells[1].Text = "<div align=\"right\" width=\"100%\">Total:</div>" + e.Row.Cells[1].Text;
            e.Row.Cells[2].Text = "<div align=\"right\" width=\"100%\">" + tc.Clicks.ToString() + "</div>" + e.Row.Cells[2].Text;
            e.Row.Cells[3].Text = "<div align=\"right\" width=\"100%\">" + tc.Leads.ToString() + "</div>" + e.Row.Cells[3].Text;
            e.Row.Cells[4].Text = "<div align=\"right\" width=\"100%\">" + tc.Sales.ToString() + "</div>" + e.Row.Cells[4].Text;
            e.Row.Cells[5].Text = "<div align=\"right\" width=\"100%\">&nbsp;</div>" + e.Row.Cells[5].Text;
            e.Row.Cells[6].Text = "<div align=\"right\" width=\"100%\">" + tc.CX.ToString() + "</div>" + e.Row.Cells[6].Text;
            e.Row.Cells[7].Text = "<div align=\"right\" width=\"100%\">" + tc.Orders.ToString() + "</div>" + e.Row.Cells[7].Text;
            e.Row.Cells[8].Text = "<div align=\"right\" width=\"100%\">&nbsp;</div>" + e.Row.Cells[8].Text;

        }
    }
}
