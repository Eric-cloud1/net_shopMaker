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
using MakerShop.Common;
using MakerShop.Orders;
using MakerShop.Shipping;
using MakerShop.Stores;
using MakerShop.Users;
using MakerShop.Utility;
using MakerShop.Reporting;


public partial class Admin_Tools_ReportSchedule : System.Web.UI.Page
{

  
    protected DataTable SessionDataSource
    {
        get
        {
            Object ds = Session["DataSource_ReportSchedule"];
            if (ds != null)
                return (DataTable)ds;

            return null;
        }
        set { Session["DataSource_ReportSchedule"] = value; }
    }

    private string SortDirection
    {
        get
        {

            if (Session["SortDirection_ReportSchedule"] == null)
                return "Asc";
            return Session["SortDirection_ReportSchedule"].ToString();
        }
        set
        {
            Session.Add("SortDirection_ReportSchedule", value);
        }
    }
    private string SortColumn
    {
        get
        {

            if (Session["SortColumn_ReportSchedule"] == null)
                return "Procedure";
            return Session["SortColumn_ReportSchedule"].ToString();
        }
        set
        {
            Session.Add("SortColumn_ReportSchedule", value);
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {

            BindGrid();

        }
    }

 

    protected void ScheduleReportGrid_Sorting(object sender, GridViewSortEventArgs e)
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
        BindGrid();

    }



    protected void BindGrid()
    {

        SessionDataSource = getData();
        ScheduleReportGrid.DataSource = SessionDataSource;
        ScheduleReportGrid.DataBind();

    }

    protected DataTable getData()
    {
        return ReportDataSource.GetReportSchedule(Token.Instance.UserId);
    }

    protected bool IsChecked(object dataItem)
    {
        DataRowView item = (DataRowView)dataItem;

        if (AlwaysConvert.ToInt(item[4]) == 1)
            return true;

        return false;
       

    }

    protected void ScheduleReportGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DropDownList list = (DropDownList)e.Row.FindControl("Schedules");
        if (list != null)
        {
            foreach (string s in Enum.GetNames(typeof(ReportScheduleTypes)))
            {
                string value = ((int)Enum.Parse(typeof(ReportScheduleTypes), s)).ToString();
                ListItem i = new ListItem(s, value);
                list.Items.Add(i);
            }
        }
    }


     protected  void Check_Clicked(Object sender, EventArgs e) 
     {
         CheckBox selectReport = (CheckBox)sender;
         GridViewRow items = (GridViewRow)selectReport.NamingContainer;

         Label reportId = (Label)items.FindControl("reportIdlabel");
         DropDownList schedule = (DropDownList)items.FindControl("Schedules");

         Save(AlwaysConvert.ToInt(reportId.Text), AlwaysConvert.ToInt16(schedule.SelectedValue), selectReport.Checked);
    }


    protected void Schedules_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList schedule = (DropDownList)sender;
        GridViewRow items = (GridViewRow)schedule.NamingContainer;
        CheckBox selectReport = (CheckBox)items.FindControl("selectReport");

        Label reportId = (Label)items.FindControl("reportIdlabel");

        Save(AlwaysConvert.ToInt(reportId.Text), AlwaysConvert.ToInt16(schedule.SelectedValue), selectReport.Checked);
    }


    protected void Save(int reportId, short scheduleId,  bool add)
    {
        ReportDataSource.SaveSchedule(Token.Instance.UserId, reportId, scheduleId, add);

        
        UserAddedMessage.Text = string.Format(@"Report Scheduled at:{0}h:{1}m", DateTime.Now.Hour, DateTime.Now.Minute);
    }

}
