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

public partial class Admin_Reports_Billing_DailyBillingReport : System.Web.UI.Page
{
    private int total = -1;
    private int authorized = -1;
    private int captured = -1;
    private int refundVoid = -1;
    private int initial = -1;
    private int trial = -1;
    private int rebilled = -1;
    private int declined = -1;

    private int initialdeclined = -1;
    private int trialdeclined = -1;
    private int rebilleddeclined = -1;


    protected DataTable SessionDataSource
    {
        get
        {
            Object ds = Session["DataSource_DailyBillingReport"];
            if (ds != null)
              return (DataTable)ds;

            return null;
        }
        set { Session["DataSource_DailyBillingReport"] = value; }
    }


    private string SortDirection
    {
        get
        {

            if (Session["SortDirection_DailyBillingReport"] == null)
                return "Asc";
            return Session["SortDirection_DailyBillingReport"].ToString();
        }
        set
        {
            Session["SortDirection_DailyBillingReport"] = value;

        }
    }
    private string SortColumn
    {
        get
        {

            if (Session["SortColumn_DailyBillingReport"] == null)
                return "Total Count";
            return Session["SortColumn_DailyBillingReport"].ToString();
        }
        set
        {
            Session["SortColumn_DailyBillingReport"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {

            BindGrid();
        }
    }
    
    protected void Page_Init(object sender, EventArgs e)
    {
      
        if (dtDailyBillingReport.FirstHit)
        {
            dtDailyBillingReport.TimePeriod = Admin_UserControls_DatesAndTime.Time_Period.Today;

        }

        dtDailyBillingReport.Update += new Admin_UserControls_DatesAndTime.UpdateEventHandler(BindGrid);
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

    public static DataTable dailybillingreport(DataTable dataValues)
    {
        List<string> pivotData = new List<string>();
        pivotData.Add("Authorized Count");
        pivotData.Add("Authorized Amount");
        pivotData.Add("Captured Count");
        pivotData.Add("Captured Amount");

        return MakerShop.Utility.DataHelper.Pivot(dataValues, "TransactionDate", "PaymentGateway", pivotData);
    }

    protected void BindGrid()
    {

        DataTable dailybillds;
        dailybillds = ReportDataSource.GetDailyBillingShortReport(dtDailyBillingReport.StartDate, dtDailyBillingReport.EndDate, cbZero.Checked, short.Parse(rblGatewayInstrument.SelectedValue));
        SessionDataSource = dailybillingreport(dailybillds);

        this.downloadbutton1.DownloadData = dailybillds;
        this.downloadbutton1.StartDate = dtDailyBillingReport.StartDate;
        this.downloadbutton1.EndDate = dtDailyBillingReport.EndDate;
        this.downloadbutton1.FileName = "DailyBilling";


        string colName = string.Empty;
        for (int i = 0; i < Convert(SessionDataSource).Columns.Count; ++i)
        {
            colName = Convert(SessionDataSource).Columns[i].ColumnName;

         

            switch (colName)
            {
                case "Total":
                    total = i;
                    break;
                case "Authorized":
                    authorized = i;
                    break;
                case "Captured":
                    captured = i;
                    break;
                case "Refund Void":
                    refundVoid = i;
                    break;
                case "Initial":
                    initial = i;
                    break;
                case "Trial":
                    trial = i;
                    break;
                case "Rebill":
                    rebilled = i;
                    break;
                case "Declined":
                    declined = i;
                    break;
                case "Initial Declined":
                    initialdeclined = i;
                    break;
                case "Trial Declined":
                    trialdeclined = i;
                    break;
                case "Rebill Declined":
                    rebilleddeclined = i;
                    break;
            }
        }


        DailyBillingReportGrid.PageIndex = 0;
        DailyBillingReportGrid.DataSource =  Convert(SessionDataSource).DefaultView;
        DailyBillingReportGrid.DataBind();
    }

  

    protected void DailyBillingReportGrid_Sorting(object sender, GridViewSortEventArgs e)
    {
        //Sorting Viewstate datasource based on the SortExpression selected
        string col = e.SortExpression.ToString();
        
        if(!SessionDataSource.Columns.Contains(col))
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

     
        //bind grid to trigger a refresh
        sortedTable = CreateTable(SessionDataSource.DefaultView);
        DailyBillingReportGrid.DataSource = Convert(sortedTable);
        DailyBillingReportGrid.DataBind();

    }



    protected void DailyBillingReportGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.DailyBillingReportGrid.PageIndex = e.NewPageIndex;
        this.DailyBillingReportGrid.DataSource =  Convert(SessionDataSource).DefaultView;
        DailyBillingReportGrid.DataBind();


    }

    protected void ProcessButton_Click(object sender, EventArgs e)
    {
        dtDailyBillingReport.setDateTime();
        BindGrid();
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

    protected void DailyBillingReportGrid_PreRender(object sender, EventArgs e)
    {
        foreach (GridViewRow gvr in DailyBillingReportGrid.Rows)
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
            if((tc.Text.Contains("Count:"))&& tc.Text.Substring(tc.Text.IndexOf("Count:")+6,1) =="0")
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


    protected void DailyBillingReportGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {


        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView dr = ((DataRowView)e.Row.DataItem);
            string ids = string.Empty;

            for (int i = 0; i <= e.Row.Cells.Count - 1; i++)
            {
                if (e.Row.Cells[i].Text.ToUpper().Contains("&LT;BR/&GT;"))
                {
                    e.Row.Cells[i].Attributes.Add("align", "right");
                    e.Row.Cells[i].Text = Server.HtmlDecode(e.Row.Cells[i].Text);

                }
                else if (((DataRowView)e.Row.DataItem)[i].GetType().FullName == typeof(int).FullName)
                {
                    e.Row.Cells[i].Attributes.Add("align", "right");

                }
                else if (((DataRowView)e.Row.DataItem)[i].GetType().FullName == typeof(decimal).FullName)
                {
                    e.Row.Cells[i].Text = string.Format("{0:C}", decimal.Parse(e.Row.Cells[i].Text));
                    e.Row.Cells[i].Attributes.Add("align", "right");
                }

                    if (i == total) //total 1-3-4
                    {
                        ids = string.Format("transactionDate={0}&reportType={1}&paymentTypes={2}", dr["transactionDate"].ToString(), "", "1-3-4");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));
                    }
                  
                    else if (i == initial)  //1 initial
                    {
                        ids = string.Format("transactionDate={0}&reportType={1}&paymentTypes={2}", dr["transactionDate"].ToString(), "12", "1");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }

                    else if (i == refundVoid)  //1 initial
                    {
                        ids = string.Format("transactionDate={0}&reportType={1}&paymentTypes={2}", dr["transactionDate"].ToString(), "11", "1-3-4");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }

                    else if (i == trial)  //3 trial
                    {
                        ids = string.Format("transactionDate={0}&reportType={1}&paymentTypes={2}", dr["transactionDate"].ToString(), "13", "1-3-4");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));
                    }

                    else if (i == rebilled)  //4 rebill
                    {
                        ids = string.Format("transactionDate={0}&reportType={1}&paymentTypes={2}", dr["transactionDate"].ToString(), "5", "4");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }

                    else if (i == authorized)  //authorized 
                    {
                        ids = string.Format("transactionDate={0}&reportType={1}&paymentTypes={2}", dr["transactionDate"].ToString(), "7", "1-3-4");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }

                    else if (i == captured)  //captured
                    {

                        ids = string.Format("transactionDate={0}&reportType={1}&paymentTypes={2}", dr["transactionDate"].ToString(), "8", "1-3-4");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }

                    else if (i == declined)  //captured
                    {

                        ids = string.Format("transactionDate={0}&reportType={1}&paymentTypes={2}", dr["transactionDate"].ToString(), "9", "1-3-4");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }

                    else if (i == initialdeclined)  //captured
                    {

                        ids = string.Format("transactionDate={0}&reportType={1}&paymentTypes={2}", dr["transactionDate"].ToString(), "9", "1");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }
                    else if (i == trialdeclined)  //captured
                    {

                        ids = string.Format("transactionDate={0}&reportType={1}&paymentTypes={2}", dr["transactionDate"].ToString(), "9", "3");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));


                    }
                    else if (i == rebilleddeclined)  //captured
                    {

                        ids = string.Format("transactionDate={0}&reportType={1}&paymentTypes={2}", dr["transactionDate"].ToString(), "9", "4");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }
                     else
                    {
                        string colName = dr.Row.Table.Columns[i].ColumnName.ToLower().Replace("count", "").Replace("amount","");

                        if (colName.Contains("captured"))
                        {
                            ids = string.Format("transactionDate={0}&reportType={1}&paymentTypes={2}&paymentGatewayPrefix={3}", dr["transactionDate"].ToString(), "8", "1-3-4", colName.Replace("authorized", "").Replace("captured", ""));

                            if (e.Row.Cells[i].Text != "0")
                                e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                        }
                        else
                        {
                            ids = string.Format("transactionDate={0}&reportType={1}&paymentTypes={2}&paymentGatewayPrefix={3}", dr["transactionDate"].ToString(), "7", "1-3-4", colName.Replace("authorized", "").Replace("captured", ""));

                            if (e.Row.Cells[i].Text != "0")
                                e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                         }
                    }

   
                }

            }
        }


    }



