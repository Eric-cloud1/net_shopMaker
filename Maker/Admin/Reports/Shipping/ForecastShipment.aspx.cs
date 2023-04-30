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

public partial class Admin_Reports_Shipping_ForecastShipment : System.Web.UI.Page
{

    int toship ;
    int canceldeclined ;
    int total;
  
    protected DataTable SessionDataSource
    {
        get
        {
            Object ds = Session["DataSource_ForecastShipment"];
            if (ds != null)
                return (DataTable)ds;

            return null;
        }
        set { Session["DataSource_ForecastShipment"] = value; }
    }



    private string SortDirection
    {
        get
        {

            if (Session["SortDirection_ForecastShipment"] == null)
                return "Asc";
            return Session["SortDirection_ForecastShipment"].ToString();
        }
        set
        {
            Session.Add("SortDirection_ForecastShipment", value);
        }
    }
    private string SortColumn
    {
        get
        {

            if (Session["SortColumn_ForecastShipment"] == null)
                return "AffiliateID";
            return Session["SortColumn_ForecastShipment"].ToString();
        }
        set
        {
            Session.Add("SortColumn_ForecastShipment", value);
        }
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        if (dtForecastShipment.FirstHit)
        {
            dtForecastShipment.TimePeriod = Admin_UserControls_DatesAndTime.Time_Period.Date_Range;

            dtForecastShipment.StartDate = System.DateTime.Today.AddDays(-15);
            dtForecastShipment.EndDate = System.DateTime.Today.AddDays(15);
        }
        dtForecastShipment.Update += new Admin_UserControls_DatesAndTime.UpdateEventHandler(BindGrid);
    }




    protected void Page_Load(object sender, EventArgs e)
    {


        if (!Page.IsPostBack)
        {
           
            BindGrid();
        }

       
    }
     protected void BindGrid()
     {
         DataTable forecastshipmentds;
         forecastshipmentds = ReportDataSource.GetForecastShipments(dtForecastShipment.StartDate, dtForecastShipment.EndDate);

         this.downloadbutton1.DownloadData = forecastshipmentds;
         this.downloadbutton1.StartDate = dtForecastShipment.StartDate;
         this.downloadbutton1.EndDate = dtForecastShipment.EndDate;
         this.downloadbutton1.FileName = "ForecastShipment";


         SessionDataSource = forecastshipmentds;

         string colName = string.Empty;
         for (int i = 0; i < SessionDataSource.Columns.Count; ++i)
         {
             colName = SessionDataSource.Columns[i].ColumnName;
             switch (colName)
             {
                 case "To Ship":
                     toship = i;
                     break;
                 case "Cancelled/Declined":
                     canceldeclined = i;
                     break;
                 case "Total":
                     total = i;
                     break;

             }
         }


         forecastshipmentReportgrid.PageIndex = 0;
         forecastshipmentReportgrid.DataSource = SessionDataSource;
         forecastshipmentReportgrid.DataBind();

     }

     
    protected void ProcessButton_Click(object sender, EventArgs e)
     {

         this.dtForecastShipment.setDateTime();

         BindGrid();
    }

    protected void forecastshipmentReportgrid_Sorting(object sender, GridViewSortEventArgs e)
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
        forecastshipmentReportgrid.DataSource = SessionDataSource.DefaultView;
        forecastshipmentReportgrid.DataBind();

    }


    protected void forecastshipmentReportgrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.forecastshipmentReportgrid.PageIndex = e.NewPageIndex;

        forecastshipmentReportgrid.DataSource = SessionDataSource.DefaultView;
        forecastshipmentReportgrid.DataBind();
    }



    protected void forecastshipmentReportgrid_PreRender(object sender, EventArgs e)
    {
        foreach (GridViewRow gvr in forecastshipmentReportgrid.Rows)
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


    protected void forecastshipmentReportgrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView dr = ((DataRowView)e.Row.DataItem);

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

                if(i == toship)
                {
                    if (e.Row.Cells[i].Text != "0")
                    {
                        e.Row.Cells[i].Attributes.Add("OnMouseOver", string.Format("ShowHoverPanel(event, \"{0}, {1}, {2}\");", dr["OrderDate"].ToString(), "173", "4"));
                        e.Row.Cells[i].Attributes.Add("OnMouseOut", string.Format("HideHoverPanel();"));
                        e.Row.Cells[i].Attributes.Add("SkinID", string.Format("Link"));
                        e.Row.Cells[i].Attributes.Add("style", "cursor:pointer;");

                    }
                }

                if(i == canceldeclined) 
                {
                    if (e.Row.Cells[i].Text != "0")
                    {
                        e.Row.Cells[i].Attributes.Add("OnMouseOver", string.Format("ShowHoverPanel(event, \"{0}, {1}, {2}\");", dr["OrderDate"].ToString(), "171", "4"));
                        e.Row.Cells[i].Attributes.Add("OnMouseOut", string.Format("HideHoverPanel();"));
                        e.Row.Cells[i].Attributes.Add("SkinID", string.Format("Link"));
                        e.Row.Cells[i].Attributes.Add("style", "cursor:pointer;");

                    }
                }
                if(i == total)
                {
                    if (e.Row.Cells[i].Text != "0")
                    {
                        e.Row.Cells[i].Attributes.Add("OnMouseOver", string.Format("ShowHoverPanel(event, \"{0}, {1}, {2}\");", dr["OrderDate"].ToString(), "172", "4"));
                        e.Row.Cells[i].Attributes.Add("OnMouseOut", string.Format("HideHoverPanel();"));
                        e.Row.Cells[i].Attributes.Add("SkinID", string.Format("Link"));
                        e.Row.Cells[i].Attributes.Add("style", "cursor:pointer;");

                    }
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
