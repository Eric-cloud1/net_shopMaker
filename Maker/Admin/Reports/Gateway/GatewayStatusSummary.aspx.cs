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
using MakerShop.Reporting.Gateway;
using System.Collections.Generic;
using MakerShop.Common;
using MakerShop.Utility;
using System.IO;


public partial class Admin_Reports_GatewayStatusSummary : MakerShop.Web.UI.MakerShopAdminPage
{

    protected DataTable SessionDataSource
    {
        get
        {
            Object ds = Session["DataSource_GatewayStatusSummary"];
            if (ds != null)
                return (DataTable)ds;

            return null;
        }
        set { Session["DataSource_GatewayStatusSummary"] = value; }
    }

    protected bool Authorize
    {
        get
        {
            Object ds = Session["Authorize_GatewayStatusSummary"];
            if (ds != null)
                return (bool)ds;

            return true;
        }
        set { Session["Authorize_GatewayStatusSummary"] = value; }
    }
    protected bool DateGroup
    {
        get
        {
            Object ds = Session["DateGroup_GatewayStatusSummary"];
            if (ds != null)
                return (bool)ds;

            return true;
        }
        set { Session["DateGroup_GatewayStatusSummary"] = value; }
    }
    protected void Page_Init(object sender, EventArgs e)
    {
        if (dtGatewaySummary.FirstHit)
        {
            dtGatewaySummary.TimePeriod = Admin_UserControls_DatesAndTime.Time_Period.Today;
        }
        dtGatewaySummary.Update += new Admin_UserControls_DatesAndTime.UpdateEventHandler(BindGrid);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        dateType.Text = string.Format("(Authorize Date)");

        if(dateTypelst.SelectedValue == "0")
            dateType.Text = string.Format("(Capture Date)");


        if (!Page.IsPostBack)
        {
            cbDateGroup.Checked = DateGroup;
            cbAuthorize.Checked = Authorize;
            BindGrid();
        }
    }
    protected void cbAuthorize_Check(object sender, EventArgs e)
    {
        Authorize = cbAuthorize.Checked;
        BindGrid();

    }
    protected void cbDateGroup_Check(object sender, EventArgs e)
    {
        DateGroup = cbDateGroup.Checked;
        BindGrid();
    }
    protected void ProcessButton_Click(object sender, EventArgs e)
    {
        dtGatewaySummary.setDateTime();
        BindGrid();
    }
    private string SortDirection
    {
        get
        {

            if (Session["SortDirection_GatewaySummary"] == null)
                return "Asc";
            return Session["SortDirection_GatewaySummary"].ToString();
        }
        set
        {
            Session.Add("SortDirection_GatewaySummary", value);
        }
    }
    private string SortColumn
    {
        get
        {

            if (Session["SortColumn_GatewaySummary"] == null)
            {
                if (DateGroup)
                    return "Authorized Date";
                else
                    return "Payment Gateway";
            }
            return Session["SortColumn_GatewaySummary"].ToString();
        }
        set
        {
            Session.Add("SortColumn_GatewaySummary", value);
        }
    }
    protected void gv_Sorting(object sender, GridViewSortEventArgs e)
    {
        //Sorting Viewstate datasource based on the SortExpression selected
        string col = e.SortExpression.ToString();

        SortColumn = col;


        if ((SortDirection == "Asc") && (col == SortColumn))
        {
            if ((col != "Authorized Date") && (col != "Payment Gateway")&&(!col.Contains("%")))
                col = string.Concat(col, " Count");

            SortDirection = "Desc";
            SessionDataSource.DefaultView.Sort = string.Concat(col, " desc");
        }

        else if ((SortDirection == "Desc") && (col == SortColumn))
        {
            if ((col != "Authorized Date") && (col != "Payment Gateway") && (!col.Contains("%")))
                col = string.Concat(col, " Count");

            SortDirection = "Asc";
            SessionDataSource.DefaultView.Sort = string.Concat(col, " asc");
        }


        //bind grid to trigger a refresh
        BreakdownGrid.DataSource = convert(CreateTable(SessionDataSource.DefaultView));
        BreakdownGrid.DataBind();

    }

    public static DataTable CreateTable(DataView obDataView)
    {
        if (null == obDataView)
        {
            throw new ArgumentNullException
            ("DataView", "Invalid DataView object specified");
        }

        DataTable obNewDt = obDataView.Table.Clone();
        int idx = 0;
        string[] strColNames = new string[obNewDt.Columns.Count];
        foreach (DataColumn col in obNewDt.Columns)
        {
            strColNames[idx++] = col.ColumnName;
        }

        IEnumerator viewEnumerator = obDataView.GetEnumerator();
        while (viewEnumerator.MoveNext())
        {
            DataRowView drv = (DataRowView)viewEnumerator.Current;
            DataRow dr = obNewDt.NewRow();
            try
            {
                foreach (string strName in strColNames)
                {
                    dr[strName] = drv[strName];
                }
            }
            catch
            {
            }
            obNewDt.Rows.Add(dr);
        }

        return obNewDt;
    }

    protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (DateGroup)
            {
                e.Row.Cells[1].Text = e.Row.Cells[1].Text.Replace("_", " ");
                e.Row.Cells[1].Font.Size = 8;
            }
            else
            {
                e.Row.Cells[0].Text = e.Row.Cells[0].Text.Replace("_", " ");
                e.Row.Cells[0].Font.Size = 8;
            }
            DataRowView dr = ((DataRowView)e.Row.DataItem);

            for (int i = 0; i <= e.Row.Cells.Count - 1; i++)
            {
                string colName = dr.Row.Table.Columns[i].ColumnName.Replace("Count", "");

                if (e.Row.Cells[i].Text.ToUpper().Contains("&LT;BR/&GT;"))
                {
                    e.Row.Cells[i].Attributes.Add("align", "right");
                    e.Row.Cells[i].Text = Server.HtmlDecode(e.Row.Cells[i].Text);

                }
                else if (dr[i].GetType().FullName == typeof(decimal).FullName)
                {
                    if (colName.Contains("%"))
                        e.Row.Cells[i].Text = string.Format("{0:P}", decimal.Parse(e.Row.Cells[i].Text));
                    else
                        e.Row.Cells[i].Text = string.Format("{0:C}", decimal.Parse(e.Row.Cells[i].Text));
                    e.Row.Cells[i].Attributes.Add("align", "right");
                }
            }
        }
    }


    protected void BindGrid()
    {

        DataTable dt = ReportDataSource.GetGatewayStatusByDate(dtGatewaySummary.StartDate, dtGatewaySummary.EndDate, Authorize, DateGroup, short.Parse(rblGatewayInstrument.SelectedValue), short.Parse(dateTypelst.SelectedValue));

        this.downloadbutton1.DownloadData = dt;
        this.downloadbutton1.StartDate = dtGatewaySummary.StartDate;
        this.downloadbutton1.EndDate = dtGatewaySummary.EndDate;
        this.downloadbutton1.FileName = "Gatewaysummary";

        if (dt.Rows.Count == 0)
        {
            BreakdownGrid.DataSource = null;
            BreakdownGrid.DataBind();

        }


        SessionDataSource = dt;
        BreakdownGrid.DataSource = sortView(SessionDataSource);
        BreakdownGrid.DataBind();
    }

    private DataTable sortView(DataTable dt)
    {
        if ((SortColumn != "Authorized Date") && (!SortColumn.Contains("Count")) && (SortColumn != "Payment Gateway") && (!SortColumn.Contains("%")))
            SortColumn = string.Concat(SortColumn, " Count ");


         dt.DefaultView.Sort = string.Concat(SortColumn +" ", SortDirection);
      
        return convert(CreateTable(dt.DefaultView));
    }


    private DataTable convert(DataTable dt)
    {
        DataTable convert = new DataTable();
        foreach (DataColumn dc in dt.Columns)
        {
            if (dc.ColumnName.ToUpper().Contains("COUNT"))
                continue;
            else if (dc.ColumnName.ToUpper().Contains("AMOUNT"))
                convert.Columns.Add(dc.ColumnName.Replace("Amount", "").Trim(), typeof(string));
            else
                convert.Columns.Add(dc.ColumnName, dc.DataType);
        }
        foreach (DataRow dr in dt.Rows)
        {

            DataRow DR = convert.NewRow();
            foreach (DataColumn dc in convert.Columns)
            {


                if (dt.Columns[dc.ColumnName] == null)
                {
                    if (dr[dc.ColumnName + " Amount"] != null)
                    {
                        DR[dc.ColumnName] = string.Format("{0:C}", decimal.Parse(dr[dc.ColumnName + " Amount"].ToString())) + DR[dc.ColumnName];
                    }
                    if (dr[dc.ColumnName + " Count"] != null)
                    {
                        DR[dc.ColumnName] = DR[dc.ColumnName] + "<br/>Count-" + dr[dc.ColumnName + " Count"].ToString();
                    }
                }
                else
                {
                    DR[dc.ColumnName] = dr[dc.ColumnName];
                }

            }
            convert.Rows.Add(DR);
        }
        return convert;

    }
 
    private void UpdateMessagePanel(bool success, System.Collections.Generic.List<String> messages)
    {
        if (success)
        {
            SavedMessage.Visible = true;
            SavedMessage.Text = string.Format(string.Join("<br/>", messages.ToArray()), LocaleHelper.LocalNow);
        }
        else
        {
            ErrorMessageLabel.Visible = true;
            ErrorMessageLabel.Text = string.Join("<br/>", messages.ToArray());
        }

    }



}
