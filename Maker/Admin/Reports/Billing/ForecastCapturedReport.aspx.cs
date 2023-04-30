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
using MakerShop.Reporting;
using System.Collections.Generic;
using MakerShop.Common;
using MakerShop.Utility;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using ExcelLibrary;



public partial class Admin_Reports_ForecastCaptureReport : System.Web.UI.Page
{
    private int count = -1;
    private int retryCount = -1;
    private int rebillCount = -1;
    private int trialCount = -1;
    private int trialRetryCount = -1;
    private int rebillRetryCount = -1;

    string[] valuecols;
    protected DataTable SessionDataSource
    {
        get
        {
            Object ds = Session["DataSource_ForecastCaptureReport"];
            if (ds != null)
                return (DataTable)ds;

            return null;
        }
        set { Session["DataSource_ForecastCaptureReport"] = ColumnSort(value); }
    }

    private string SortDirection
    {
        get
        {

            if (Session["SortDirection_ForecastCaptureReport"] == null)
                return "Asc";
            return Session["SortDirection_ForecastCaptureReport"].ToString();
        }
        set
        {
            Session.Add("SortDirection_ForecastCaptureReport", value);
        }
    }
    private string SortColumn
    {
        get
        {

            if (Session["SortColumn_ForecastCaptureReport"] == null)
                return "AffiliateID";
            return Session["SortColumn_ForecastCaptureReport"].ToString();
        }
        set
        {
            Session.Add("SortColumn_ForecastCaptureReport", value);
        }
    }

    private struct st
    {
        public int id;
        public string column;
    }

    private DataTable ColumnSort(DataTable dt)
    {
        int i = 3;

        //col order:  count, $, initial, initial retry, trial, trial retry, rebill, rebill retry
        if (dt.Columns["Retry Count"] != null)
        {
            dt.Columns.Remove("Retry Count");
        }
        if (dt.Columns["Retry Amount"] != null)
        {
            dt.Columns.Remove("Retry Amount");
        }

        if (dt.Columns["Initial Count"] != null)
        {
            dt.Columns["Initial Count"].SetOrdinal(i);
            ++i;
        }
        if (dt.Columns["Initial Amount"] != null)
        {
            dt.Columns["Initial Amount"].SetOrdinal(i);
            ++i;
        }


        if (dt.Columns["Trial Count"] != null)
        {
            dt.Columns["Trial Count"].SetOrdinal(i);
            ++i;
        }
        if (dt.Columns["Trial Amount"] != null)
        {
            dt.Columns["Trial Amount"].SetOrdinal(i);
            ++i;
        }

        if (dt.Columns["Trial Retry Count"] != null)
        {
            dt.Columns["Trial Retry Count"].SetOrdinal(i);
            ++i;
        }
        if (dt.Columns["Trial Retry Amount"] != null)
        {
            dt.Columns["Trial Retry Amount"].SetOrdinal(i);
            ++i;
        }
        if (dt.Columns["Retry Trial Retry Count"] != null)
        {
            dt.Columns["Retry Trial Retry Count"].SetOrdinal(i);
            ++i;
        }
        if (dt.Columns["Retry Trial Retry Amount"] != null)
        {
            dt.Columns["Retry Trial Retry Amount"].SetOrdinal(i);
            ++i;
        }


        if (dt.Columns["Rebill Count"] != null)
        {
            dt.Columns["Rebill Count"].SetOrdinal(i);
            ++i;
        }
        if (dt.Columns["Rebill Amount"] != null)
        {
            dt.Columns["Rebill Amount"].SetOrdinal(i);
            ++i;
        }

        System.Collections.Generic.List<st> columns = new List<st>();

        for (int j = i; j < dt.Columns.Count; ++j)
        {
            if (dt.Columns[j].ColumnName.ToUpper().Contains("RETRY"))
                continue;
            if (dt.Columns[j].ColumnName.ToUpper().Contains("REBILL"))
            {
                if (dt.Columns[j].ColumnName.Contains('('))
                {
                    int a;
                    if (int.TryParse(dt.Columns[j].ColumnName.Substring(dt.Columns[j].ColumnName.IndexOf('(') + 1, dt.Columns[j].ColumnName.IndexOf(')') - dt.Columns[j].ColumnName.IndexOf('(') - 1), out a))
                    {
                        st zz = new st();
                        zz.column = dt.Columns[j].ColumnName;
                        zz.id = a;
                        if ((a == 0) || columns.Count == 0)
                            columns.Insert(0, zz);
                        else
                        {
                            for (int ST = 0; ST < columns.Count; ++ST)
                            {
                                if (columns[ST].id > a)
                                {
                                    columns.Insert(ST, zz);
                                    break;
                                }
                            }

                        }
                    }
                }
                else
                {
                    dt.Columns[j].SetOrdinal(i);
                    ++i;
                }
            }
        }
        foreach (st ST in columns)
        {
            dt.Columns[ST.column].SetOrdinal(i);
            ++i;
        }

        columns = new List<st>();

        for (int j = i; j < dt.Columns.Count; ++j)
        {
            if (!dt.Columns[j].ColumnName.ToUpper().Contains("RETRY REBILL"))
                continue;
            //    if (dt.Columns[j].ColumnName.ToUpper().Contains("REBILL"))
            {
                if (dt.Columns[j].ColumnName.Contains('('))
                {
                    int a;
                    if (int.TryParse(dt.Columns[j].ColumnName.Substring(dt.Columns[j].ColumnName.IndexOf('(') + 1, dt.Columns[j].ColumnName.IndexOf(')') - dt.Columns[j].ColumnName.IndexOf('(') - 1), out a))
                    {
                        st zz = new st();
                        zz.column = dt.Columns[j].ColumnName;
                        zz.id = a;
                        if ((a == 0) || columns.Count == 0)
                            columns.Insert(0, zz);
                        else
                        {
                            for (int ST = 0; ST < columns.Count; ++ST)
                            {
                                if (columns[ST].id > a)
                                {
                                    columns.Insert(ST, zz);
                                    break;
                                }
                            }

                        }
                    }
                }
                else
                {
                    dt.Columns[j].SetOrdinal(i);
                    ++i;
                }
            }
        }
        foreach (st ST in columns)
        {
            dt.Columns[ST.column].SetOrdinal(i);
            ++i;
        }

        return dt;
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        if (dtForecastCapture.FirstHit)
        {
            dtForecastCapture.TimePeriod = Admin_UserControls_DatesAndTime.Time_Period.Date_Range;

            dtForecastCapture.StartDate = System.DateTime.Today.AddDays(-15);
            dtForecastCapture.EndDate = System.DateTime.Today.AddDays(15);

        }

        dtForecastCapture.Update += new Admin_UserControls_DatesAndTime.UpdateEventHandler(BindGrid);
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
        {
            
            BindGrid();

        }

       
    }




    public DataTable forecastreport(DataTable forecastreport)
    {

        List<string> pivotData = new List<string>();
        for (int i = 0; i < valuecols.Length; i++)
            pivotData.Add(valuecols[i].ToString());

        return MakerShop.Utility.DataHelper.Pivot(forecastreport, "Paymentdate", "Payment Type", pivotData);
    }


    public DataTable forecastreportx(DataTable forecastreport)
    {
        List<string> pivotData = new List<string>();
        for (int i = 0; i < valuecols.Length; i++)
            pivotData.Add(valuecols[i].ToString());

        return MakerShop.Utility.DataHelper.Pivot(forecastreport, "Paymentdate", "RecordType", pivotData);
    }

    protected DataTable getData()
    {
        int SubAffiliate = 0;

        if (chkSubAffiliate.Checked)
        {
            SubAffiliate = 1;

        }

        if (this.txtAffiliates.Text == this.twAffiliates.WatermarkText)
            this.txtAffiliates.Text = string.Empty;

        if (this.txtSubAffiliate.Text == twSubAffiliates.WatermarkText)
            this.txtSubAffiliate.Text = string.Empty;


        DataTable forecastds = ReportDataSource.GetForecastShortCapturedReport(dtForecastCapture.StartDate, dtForecastCapture.EndDate, this.txtAffiliates.Text.Trim(), this.txtSubAffiliate.Text.Trim(), SubAffiliate, short.Parse(rblGatewayInstrument.SelectedValue));


      
        if (this.rdGroupBy.SelectedValue == "Date")
        {
            forecastds.Columns.Remove("Affiliate");
            forecastds.Columns.Remove("Subaffiliate");
        }
        else if (this.rdGroupBy.SelectedValue == "affiliateId")
        {
            forecastds.Columns.Remove("Subaffiliate");
        }


        if (this.RdDisplay.SelectedValue == "Both")
        {
            valuecols = new string[] { "Captured Count", "Captured Amount", "Retry Count", "Retry Amount" };


        }

        else if (this.RdDisplay.SelectedValue == "First")
        {
            valuecols = new string[] { "Captured Count", "Captured Amount" };
            forecastds.Columns.Remove("Retry Count");
            forecastds.Columns.Remove("Retry Amount");
            forecastds.Columns.Remove("Rebill Retry Count");
            forecastds.Columns.Remove("Rebill Retry Amount");
        }

        else if (this.RdDisplay.SelectedValue == "Retry")
        {
            valuecols = new string[] { "Retry Count", "Retry Amount" };
            forecastds.Columns.Remove("Captured Count");
            forecastds.Columns.Remove("Captured Amount");
            forecastds.Columns.Remove("Rebill Count");
            forecastds.Columns.Remove("Rebill Amount");
        }

        DataTable forecastdtx;
        forecastdtx = forecastreportx(forecastds);
        forecastdtx.PrimaryKey = new DataColumn[] { forecastdtx.Columns[0] };
        forecastdtx.Columns.Remove("Paymentgateway");

        DataTable forecastdty;
        forecastdty = forecastreporty(forecastds);
        forecastdty.PrimaryKey = new DataColumn[] { forecastdty.Columns[0] };
        forecastdty.Columns.Remove("RecordType");
        forecastdtx.Merge(forecastdty);



        this.downloadbutton1.DownloadData = forecastdtx;
        this.downloadbutton1.StartDate = dtForecastCapture.StartDate;
        this.downloadbutton1.EndDate = dtForecastCapture.EndDate;
        this.downloadbutton1.FileName = "ForecastCapture";

        return forecastdtx;

    }

    public DataTable forecastreporty(DataTable dataValues)
    {

        List<string> pivotData = new List<string>();
        for (int i = 0; i < valuecols.Length; i++)
            pivotData.Add(valuecols[i].ToString());

        return MakerShop.Utility.DataHelper.Pivot(dataValues, "Paymentdate", "Paymentgateway", pivotData);
    }


    protected void BindGrid()
    {

        SessionDataSource = getData();
        

        string colName = string.Empty;
        for (int i = 0; i < Convert(SessionDataSource).Columns.Count; ++i)
        {
            colName = Convert(SessionDataSource).Columns[i].ColumnName;

            switch (colName)
            {
                case "Captured":
                    count = i;
                    break;
                case "Retry":
                    retryCount = i;
                    break;
                case "Trial":
                    trialCount = i;
                    break;
                case "Rebill":
                    rebillCount = i;
                    break;
                case "Trial Retry":
                    trialRetryCount = i;
                    break;
                case "Rebill Retry":
                    rebillRetryCount = i;
                    break;
                    
            }
        }

        forecastReportgrid.PageIndex = 0;
        forecastReportgrid.DataSource = Convert(SessionDataSource);
        forecastReportgrid.DataBind();
    }

    protected void ProcessButton_Click(object sender, EventArgs e)
    {
        DateTime dtNow = DateTime.Now;
        TimeSpan tsMinute = new TimeSpan(0, 0, 1, 0);

        HttpCookie subaffiliate = new HttpCookie("subaffiliate");
        subaffiliate.Value = this.txtSubAffiliate.Text;
        subaffiliate.Expires = dtNow + tsMinute;
        Response.Cookies.Add(subaffiliate);

        HttpCookie affiliates = new HttpCookie("affiliates");
        affiliates.Value = this.txtAffiliates.Text;
        affiliates.Expires = dtNow + tsMinute;
        Response.Cookies.Add(affiliates);
        dtForecastCapture.setDateTime();
        BindGrid();
    }

    private bool ColumnExists(DataTable Table, string ColumnName)
    {
        bool bRet = false;
        foreach (DataColumn col in Table.Columns)
        {
            if (col.ColumnName == ColumnName)
            {
                bRet = true;
                break;
            }
        }

        return bRet;
    }

    protected string formatSubAffiliate(object dataItem)
    {
        DataRowView row = (DataRowView)dataItem;

        string subAffiliate = row["Subaffiliate"].ToString();

        if (subAffiliate.IndexOf("-") > 0)
            subAffiliate = subAffiliate.Substring(0, subAffiliate.IndexOf("-"));

        return subAffiliate;



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


    protected void forecastReportgrid_Sorting(object sender, GridViewSortEventArgs e)
    {
        string col = e.SortExpression.ToString();

        if (!SessionDataSource.Columns.Contains(col))
            col = e.SortExpression.ToString() + " Amount";

        SortColumn = col;

        DataTable sortedTable = new DataTable();

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

        sortedTable = CreateTable(SessionDataSource.DefaultView);
      
        //bind grid to trigger a refresh
        forecastReportgrid.DataSource = Convert(sortedTable);
        forecastReportgrid.DataBind();

    }


    protected DataTable Convert(DataTable dt)
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
                        DR[dc.ColumnName] = DR[dc.ColumnName] + "<br/>Count:" + dr[dc.ColumnName + " Count"].ToString();
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

    protected void forecastReportgrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.forecastReportgrid.PageIndex = e.NewPageIndex;
        this.forecastReportgrid.DataSource = Convert(SessionDataSource).DefaultView;
        forecastReportgrid.DataBind();
    }


    protected void forecastReportgrid_PreRender(object sender, EventArgs e)
    {
        foreach (GridViewRow gvr in forecastReportgrid.Rows)
        {
            foreach (TableCell tc in gvr.Cells)
                fixPop(tc);
        }
    }

    private void fixPop(TableCell tc)
    {
        DateTime d;

        if (!DateTime.TryParse(tc.Text, out d))
        {
            if ((tc.Text.Contains("Count:")) && tc.Text.Substring(tc.Text.IndexOf("Count:") + 6, 1) == "0")
                return;

            if (tc.Attributes["id"] != null)
            {

                // tc.Attributes.Add("OnMouseOut", string.Format("HideHoverPanel();"));
                tc.Attributes.Add("parent", "GridTable");
                tc.Attributes.Add("SkinID", string.Format("Link"));
                tc.Attributes.Add("style", "cursor:pointer;text-decoration:underline");
            }
        }
    }

    protected void forecastReportgrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        string tmp = string.Empty;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView dr = ((DataRowView)e.Row.DataItem);
            string ids = string.Empty;


            for (int i = 0; i <= e.Row.Cells.Count - 1; i++)
            {
                string colName = dr.Row.Table.Columns[i].ColumnName.Replace("Count", "");

                if (e.Row.Cells[i].Text.ToUpper().Contains("&LT;BR/&GT;"))
                {
                    e.Row.Cells[i].Attributes.Add("align", "right");
                    e.Row.Cells[i].Text = Server.HtmlDecode(e.Row.Cells[i].Text);

                }
                else if (dr[i].GetType().FullName == typeof(int).FullName)
                {
                    e.Row.Cells[i].Attributes.Add("align", "right");

                }
                else if (dr[i].GetType().FullName == typeof(decimal).FullName)
                {
                    e.Row.Cells[i].Text = string.Format("{0:C}", decimal.Parse(e.Row.Cells[i].Text));
                    e.Row.Cells[i].Attributes.Add("align", "right");
                }

                if (i == count) //Count
                {
                    ids = string.Format("paymentDate={0}&reportType={1}&paymentTypes={2}", dr["PaymentDate"].ToString(), "28", "3-4");

                    if (e.Row.Cells[i].Text != "0")
                        e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                }
                else if (i == retryCount) //Retry Count
                {
                    ids = string.Format("paymentDate={0}&reportType={1}&paymentTypes={2}", dr["PaymentDate"].ToString(), "29", "3-4");

                    if (e.Row.Cells[i].Text != "0")
                        e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                     
                }
                else if (i == trialCount) //trial Count
                {
                    ids = string.Format("paymentDate={0}&reportType={1}&paymentTypes={2}", dr["PaymentDate"].ToString(), "28", "3");

                    if (e.Row.Cells[i].Text != "0")
                        e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                      
                }

                else if (i == trialRetryCount) //trial retry Count
                {
                    ids = string.Format("paymentDate={0}&reportType={1}&paymentTypes={2}", dr["PaymentDate"].ToString(), "29", "3");

                    if (e.Row.Cells[i].Text != "0")
                        e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));


                }

                else if (i == rebillCount) //Rebill Count
                {
                    ids = string.Format("paymentDate={0}&reportType={1}&paymentTypes={2}", dr["PaymentDate"].ToString(), "28", "4");

                    if (e.Row.Cells[i].Text != "0")
                        e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

         
                }

                else if (i == rebillRetryCount) //Rebill Retry Count
                {
                    ids = string.Format("paymentDate={0}&reportType={1}&paymentTypes={2}", dr["PaymentDate"].ToString(), "29", "4");

                    if (e.Row.Cells[i].Text != "0")
                        e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    
                }
                else if (
                    (colName.Contains("(1)"))
                    ||(colName.Contains("(2)"))
                    ||(colName.Contains("(3)"))
                    ||(colName.Contains("(4)")))
                {
                }

               
                    //Retry Retry
                else
                {
                    colName = dr.Row.Table.Columns[i].ColumnName.Replace("Count", "");

                    ids = string.Format("paymentDate={0}&reportType={1}&paymentTypes={2}&paymentGatewayPrefix={3}", dr["PaymentDate"].ToString(), "28", "3-4", colName.Replace("Captured", "").Replace("Retry", "").Replace("Rebill", ""));

                    if (e.Row.Cells[i].Text != "0")
                        e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));


                }

            }
        }
    }
    private Dictionary<string, Control> newControls = new Dictionary<string, Control>();
    void cme_ResolveControlID(object sender, AjaxControlToolkit.ResolveControlEventArgs e)
    {
        e.Control = Page.FindControl(e.ControlID);
    }


}




