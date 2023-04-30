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

public partial class Admin_Reports_GatewayBreakdown : MakerShop.Web.UI.MakerShopAdminPage
{

    protected DataTable SessionDataSource
    {
        get
        {
            Object ds = Session["DataSource_GatewayBreakdown"];
            if (ds != null)
                return (DataTable)ds;

            return null;
        }
        set { Session["DataSource_GatewayBreakdown"] = value; }
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


    private string SortDirection
    {
        get
        {

            if (Session["SortDirection_GatewayStatusSummary"] == null)
                return "Asc";
            return Session["SortDirection_GatewayStatusSummary"].ToString();
        }
        set
        {
            Session.Add("SortDirection_GatewayStatusSummary", value);
        }
    }

    private string SortColumn
    {
        get
        {

            if (Session["SortColumn_GatewayStatusSummary"] == null)
                return "Name";
            return Session["SortColumn_GatewayStatusSummary"].ToString();
        }
        set
        {
            Session.Add("SortColumn_GatewayStatusSummary", value);
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            
            cbDateGroup.Checked = DateGroup;

            BindGrid();
        }

    }

    protected void cbDateGroup_Check(object sender, EventArgs e)
    {
        DateGroup = cbDateGroup.Checked;
        BindGrid();
    }


    protected void ProcessButton_Click(object sender, EventArgs e)
    {
        dtGatewayBreakdown.setDateTime();
        BindGrid();
    }

    protected void BreakdownGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        BreakdownGrid.PageIndex = e.NewPageIndex;
        BreakdownGrid.DataSource = SessionDataSource;
        BreakdownGrid.DataBind();
    }

    protected  void BindGrid()
    {


        SessionDataSource = ReportDataSource.GetTableGatewayBreakdownSummary(dtGatewayBreakdown.StartDate, dtGatewayBreakdown.EndDate, ucGateways.PaymentGatewayId, "name", Authorize, DateGroup);

        this.downloadbutton1.DownloadData = SessionDataSource;
        this.downloadbutton1.StartDate = dtGatewayBreakdown.StartDate;
        this.downloadbutton1.EndDate = dtGatewayBreakdown.EndDate;


        List<GatewayBreakdownSummary> gatewayBreakdowns = new List<GatewayBreakdownSummary>();
        GatewayBreakdownSummary breakdownSummary = null;

        foreach(DataRow row in SessionDataSource.Rows)
        {
            breakdownSummary = new GatewayBreakdownSummary();
            breakdownSummary.PaymentGatewayId = 0;
            breakdownSummary.Name = row["Name"].ToString();
            breakdownSummary.TransactionStatus = row["Transaction Status"].ToString();
            breakdownSummary.Amount = AlwaysConvert.ToDecimal(row["Amount"].ToString());
            breakdownSummary.TransactionType = row["Transaction Type"].ToString();
            breakdownSummary.TransactionCount = row["Transaction Count"].ToString();
            gatewayBreakdowns.Add(breakdownSummary);

        }
        
        int totalCount = 0;
        LSDecimal totalAmount = 0;
        foreach (GatewayBreakdownSummary pbs in gatewayBreakdowns)
        {
            totalCount += 1;
            if (pbs.TransactionType.ToUpper().Contains("CAPTURE"))
                totalAmount += pbs.Amount;
        }
        if (gatewayBreakdowns.Count > 0)
        {
            TotalCount.Visible = true;
            LblTotalCount.Visible = true;
            TotalCount.Text = totalCount.ToString();
            TotalAmount.Visible = true;
            LblTotalAmount.Visible = true;
            TotalAmount.Text = string.Format("{0:lc}", totalAmount);
        }
        else
        {
            TotalCount.Visible = false;
            LblTotalCount.Visible = false;
            TotalAmount.Visible = false;
            LblTotalAmount.Visible = false;
        }

        SessionDataSource.DefaultView.Sort = string.Concat(SortColumn + " ", SortDirection);

        BreakdownGrid.DataSource = SessionDataSource.DefaultView;
        BreakdownGrid.DataBind();
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

   
    protected void BreakdownGrid_Sorting(object sender, GridViewSortEventArgs e)
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
        BreakdownGrid.DataSource = SessionDataSource.DefaultView;
        BreakdownGrid.DataBind();

    }


    protected void BreakdownGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            
            DataRowView dr = ((DataRowView)e.Row.DataItem);

            for (int i = 0; i <= e.Row.Cells.Count - 1; i++)
            {
                string colName = dr.Row.Table.Columns[i].ColumnName.Replace("Count", "");

                if (dr.Row[i].GetType().FullName == typeof(int).FullName)
                {
                    e.Row.Cells[i].Attributes.Add("align", "right");

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


}
