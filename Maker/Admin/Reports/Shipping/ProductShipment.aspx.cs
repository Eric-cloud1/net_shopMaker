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

public partial class Admin_Reports_Shipping_ProductShipment : System.Web.UI.Page
{

    private int notshipped = -1;
    private int shipped = -1;



    protected DataTable SessionDataSource
    {
        get
        {
            Object ds = Session["DataSource_ProductShipment"];
            if (ds != null)
                return (DataTable)ds;

            return null;
        }
        set { Session["DataSource_ProductShipment"] = value; }
    }

    private string SortDirection
    {
        get
        {

            if (Session["SortDirection_ProductShipment"] == null)
                return "Asc";
            return Session["SortDirection_ProductShipment"].ToString();
        }
        set
        {
            Session.Add("SortDirection_ProductShipment", value);
        }
    }
    private string SortColumn
    {
        get
        {

            if (Session["SortColumn_ProductShipment"] == null)
                return "AffiliateID";
            return Session["SortColumn_ProductShipment"].ToString();
        }
        set
        {
            Session.Add("SortColumn_ProductShipment", value);
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
        if (dtProductShipment.FirstHit)
        {
            dtProductShipment.TimePeriod = Admin_UserControls_DatesAndTime.Time_Period.Last_Hour;

        }
        dtProductShipment.Update += new Admin_UserControls_DatesAndTime.UpdateEventHandler(BindGrid);
    }


    public static DataTable productshippingreport(DataTable dataValues)
    {
        List<string> pivotData = new List<string>();
        pivotData.Add("Shipped");
        pivotData.Add("NotShipped");

        List<string> keys = new List<string>();
        keys.Add("Orderdate");
        keys.Add("Country");


        return MakerShop.Utility.DataHelper.Pivot(dataValues, keys,  "Sku", pivotData);
    }


    protected void BindGrid()
    {
        DataTable productshipmentds = ReportDataSource.GetProductShipments(dtProductShipment.StartDate, dtProductShipment.EndDate);
        SessionDataSource = productshippingreport(productshipmentds);

        this.downloadbutton1.DownloadData = productshipmentds;
        this.downloadbutton1.StartDate = dtProductShipment.StartDate;
        this.downloadbutton1.EndDate = dtProductShipment.EndDate;
        this.downloadbutton1.FileName = "ProductShipment";




        string colName = string.Empty;
        for (int i = 0; i < SessionDataSource.Columns.Count; ++i)
        {
            colName = SessionDataSource.Columns[i].ColumnName;
            switch (colName)
            {
                case "NotShipped":
                    notshipped = i;
                    break;
                case "Shipped":
                    shipped = i;
                    break;
      
            }
        }

        productshipmentReportgrid.PageIndex = 0;
        productshipmentReportgrid.DataSource = SessionDataSource.DefaultView;
        productshipmentReportgrid.DataBind();

    }


    protected void productshipmentReportgrid_PreRender(object sender, EventArgs e)
    {
        foreach (GridViewRow gvr in productshipmentReportgrid.Rows)
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

    protected void productshipmentReportgrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.productshipmentReportgrid.PageIndex = e.NewPageIndex;
        this.productshipmentReportgrid.DataSource = SessionDataSource.DefaultView;
        productshipmentReportgrid.DataBind();
    }


    protected void productshipmentReportgrid_Sorting(object sender, GridViewSortEventArgs e)
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
        productshipmentReportgrid.DataSource = SessionDataSource.DefaultView;
        productshipmentReportgrid.DataBind();

    }

    protected void ProcessButton_Click(object sender, EventArgs e)
    {

        this.dtProductShipment.setDateTime();

        BindGrid();
    }
    protected void productshipmentReportgrid_RowDataBound(object sender, GridViewRowEventArgs e)
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

                else if (dr[i].GetType().FullName == typeof(DateTime).FullName)
                {
                    e.Row.Cells[i].Text = string.Format("{0:MM/dd/yyyy}", DateTime.Parse(e.Row.Cells[i].Text));
                    e.Row.Cells[i].Attributes.Add("align", "right");
                }


                string colName = dr.Row.Table.Columns[i].ColumnName;
                string paymentId = "1-3-4";


                if (i == notshipped)
                {
                    if (e.Row.Cells[i].Text != "0")
                        e.Row.Cells[i].Attributes.Add("OnMouseOver", string.Format("ShowHoverPanel(event, \"{0}, {1}, {2}, {3}\");", dr["OrderDate"].ToString(), "159", paymentId, dr["Country"].ToString()));
                    
                }

                if (i == shipped)
                {
                    if (e.Row.Cells[i].Text != "0")
                        e.Row.Cells[i].Attributes.Add("OnMouseOver", string.Format("ShowHoverPanel(event, \"{0}, {1}, {2}, {3}\");", dr["OrderDate"].ToString(), "179", paymentId, dr["Country"].ToString()));

                }
                else if(colName.ToLower().Contains("notshipped"))
                {
                    colName = dr.Row.Table.Columns[i].ColumnName.ToLower().Replace("notshipped", "");

                    if (e.Row.Cells[i].Text != "0")
                        e.Row.Cells[i].Attributes.Add("OnMouseOver", string.Format("ShowHoverPanelSku(event, \"{0}, {1}, {2}, {3}, {4}\");", dr["OrderDate"].ToString(), "159", paymentId, colName, dr["Country"].ToString()));
                }
                else if (colName.ToLower().Contains("shipped"))
                {
                    colName = dr.Row.Table.Columns[i].ColumnName.ToLower().Replace("shipped", "");

                    if (e.Row.Cells[i].Text != "0")
                        e.Row.Cells[i].Attributes.Add("OnMouseOver", string.Format("ShowHoverPanelSku(event, \"{0}, {1}, {2}, {3}, {4}\");", dr["OrderDate"].ToString(), "179", paymentId, colName, dr["Country"].ToString()));
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
