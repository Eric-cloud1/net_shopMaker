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
using MakerShop.Reporting;
using System.Collections.Generic;
using MakerShop.Common;
using MakerShop.Utility;
using System.IO;
using System.Text;
using System.Xml;
using System.Reflection;

public partial class Admin_Reports_Shipping_PendingShipment : System.Web.UI.Page
{

    private int initialnotshipped = -1;
    private int rebillnotshipped = -1;
    private int forecastnotshipped = -1;

    private int shipped = -1;
    private int notshipped = -1;
    private int cancel = -1;
    private int total = -1;



    protected DataTable SessionDataSource
    {
        get
        {
            Object ds = Session["DataSource_PendingShipment"];
            if (ds != null)
                return (DataTable)ds;

            return null;
        }
        set { Session["DataSource_PendingShipment"] = ColumnSort(value); }
    }


    private struct st
    {
        public int id;
        public string column;
    }


    private DataTable ColumnSort(DataTable dt)
    {
        int i = 6;
        int a = 0;
        System.Collections.Generic.List<st> columns = new List<st>();

        for (int j = i; j < dt.Columns.Count; ++j)
        {
            if (dt.Columns[j].ColumnName.ToUpper().Contains("TRIAL")) 
            {
                  st zz = new st();
                  zz.column = dt.Columns[j].ColumnName;
                  zz.id = a;
                        
                  columns.Insert(a, zz);
                  a++;
            }
            else if(dt.Columns[j].ColumnName.ToUpper().Contains("INITIAL"))
            { 
                    dt.Columns[j].SetOrdinal(i);
                    ++i;
            }
           
        }

        for (int j = i; j < dt.Columns.Count; ++j)
        {
            if  (dt.Columns[j].ColumnName.ToUpper().Contains("REBILL"))
            {
                st zz = new st();
                zz.column = dt.Columns[j].ColumnName;
                zz.id = a;

                columns.Insert(a, zz);
                a++;
            }
        }


        foreach (st ST in columns)
        {
            dt.Columns[ST.column].SetOrdinal(i);
            ++i;
        }

        return dt;
    }


    private string SortDirection
    {
        get
        {

            if (Session["SortDirection_PendingShipment"] == null)
                return "Asc";
            return Session["SortDirection_PendingShipment"].ToString();
        }
        set
        {
            Session.Add("SortDirection_PendingShipment", value);
        }
    }
    private string SortColumn
    {
        get
        {

            if (Session["SortColumn_PendingShipment"] == null)
                return "AffiliateID";
            return Session["SortColumn_PendingShipment"].ToString();
        }
        set
        {
            Session.Add("SortColumn_PendingShipment", value);
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
        if (dtPendingShipment.FirstHit)
        {
            dtPendingShipment.TimePeriod = Admin_UserControls_DatesAndTime.Time_Period.Last_15_Days;

        }
        dtPendingShipment.Update += new Admin_UserControls_DatesAndTime.UpdateEventHandler(BindGrid);
    }


    public static DataTable pshippingreport(DataTable dataValues)
    {
        List<string> pivotData = new List<string>();
        pivotData.Add("Shipped");
        pivotData.Add("NotShipped");
        pivotData.Add("Cancelled/Declined");

        return MakerShop.Utility.DataHelper.Pivot(dataValues, "Orderdate", "OrderType", pivotData);
    }


    protected void BindGrid()
    {
        DataTable pendingshipmentds;
        pendingshipmentds = ReportDataSource.GetPendingShipments(dtPendingShipment.StartDate, dtPendingShipment.EndDate);
        SessionDataSource = pshippingreport(pendingshipmentds);

        this.downloadbutton1.DownloadData = pendingshipmentds;
        this.downloadbutton1.StartDate = dtPendingShipment.StartDate;
        this.downloadbutton1.EndDate = dtPendingShipment.EndDate;
        this.downloadbutton1.FileName = "PendingShipment";



        pendingshipmentReportgrid.PageIndex = 0;
        pendingshipmentReportgrid.DataSource = SessionDataSource;
        pendingshipmentReportgrid.DataBind();

    }

    protected void pendingshipmentReportgrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.pendingshipmentReportgrid.PageIndex = e.NewPageIndex;
        this.pendingshipmentReportgrid.DataSource = SessionDataSource.DefaultView;
        pendingshipmentReportgrid.DataBind();
    }


    protected void pendingshipmentReportgrid_Sorting(object sender, GridViewSortEventArgs e)
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
        pendingshipmentReportgrid.DataSource = SessionDataSource.DefaultView;
        pendingshipmentReportgrid.DataBind();

    }




    protected void ProcessButton_Click(object sender, EventArgs e)
    {

        this.dtPendingShipment.setDateTime();

        BindGrid();
    }


    protected void pendingshipmentReportgrid_PreRender(object sender, EventArgs e)
    {
        foreach (GridViewRow gvr in pendingshipmentReportgrid.Rows)
        {
            foreach (TableCell tc in gvr.Cells)
                fixPop(tc);
        }
    }

    private void fixPop(TableCell tc)
    {
        double d;
        if (double.TryParse(tc.Text, out d))
        {
            if ((d > 0) && (tc.Attributes["OnMouseOver"] != null))
            {

                tc.Attributes.Add("OnMouseOut", string.Format("HideHoverPanel();"));
                tc.Attributes.Add("SkinID", string.Format("Link"));
                tc.Attributes.Add("style", "cursor:pointer;text-decoration:underline");
            }
            else
                tc.Attributes.Remove("OnMouseOver");
        }
    }

    protected void pendingshipmentReportgrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView dr = ((DataRowView)e.Row.DataItem);
            string colName = string.Empty;
            string paymentId = string.Empty;


            for (int i = 0; i <= e.Row.Cells.Count - 1; i++)
            {

                if (dr[i].GetType().FullName == typeof(int).FullName)
                {
                    e.Row.Cells[i].Attributes.Add("align", "right");

                }
                else if (dr[i].GetType().FullName == typeof(decimal).FullName)
                {
                    e.Row.Cells[i].Text = string.Format("{0:C}", decimal.Parse(e.Row.Cells[i].Text));
                    e.Row.Cells[i].Attributes.Add("align", "right");
                }

                else if (dr[i].GetType().FullName == typeof(DateTime).FullName)
                {
                    e.Row.Cells[i].Text = string.Format("{0:MM/dd/yyyy}", DateTime.Parse(e.Row.Cells[i].Text));
                    e.Row.Cells[i].Attributes.Add("align", "right");
                }

                colName = dr.Row.Table.Columns[i].ColumnName;

                paymentId = "1-3-4";

                if (colName.ToLower().Contains("initial"))
                    paymentId = "1";

                if (colName.ToLower().Contains("rebill"))
                    paymentId = "4";

                if (colName.ToLower().Contains("trial"))
                    paymentId = "3";


                if (colName.ToLower().Contains("shipped"))
                {
                    if (e.Row.Cells[i].Text != "0")
                        e.Row.Cells[i].Attributes.Add("OnMouseOver", string.Format("ShowHoverPanel(event, \"{0}, {1}, {2}\");", dr["OrderDate"].ToString(), "17", paymentId));

                }
                if (colName.ToLower().Contains("notshipped"))
                {
                    if (e.Row.Cells[i].Text != "0")
                        e.Row.Cells[i].Attributes.Add("OnMouseOver", string.Format("ShowHoverPanel(event, \"{0}, {1}, {2}\");", dr["OrderDate"].ToString(), "15", paymentId));

                }
                if (colName.ToLower().Contains("cancel"))
                {
                    if (e.Row.Cells[i].Text != "0")
                        e.Row.Cells[i].Attributes.Add("OnMouseOver", string.Format("ShowHoverPanel(event, \"{0}, {1}, {2}\");", dr["OrderDate"].ToString(), "16", paymentId));

                }

                if (colName.ToLower().Contains("total"))
                {
                    if (e.Row.Cells[i].Text != "0")
                        e.Row.Cells[i].Attributes.Add("OnMouseOver", string.Format("ShowHoverPanel(event, \"{0}, {1}, {2}\");", dr["OrderDate"].ToString(), "151", paymentId));

                }
   
         
                   
                    e.Row.Cells[i].Attributes.Add("OnMouseOut", string.Format("HideHoverPanel();"));
                    e.Row.Cells[i].Attributes.Add("SkinID", string.Format("Link"));
                    e.Row.Cells[i].Attributes.Add("style", "cursor:pointer;");

               }

            }
        
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

   
    private Dictionary<string, Control> newControls = new Dictionary<string, Control>();
    void cme_ResolveControlID(object sender, AjaxControlToolkit.ResolveControlEventArgs e)
    {
        e.Control = Page.FindControl(e.ControlID);
    }
}
