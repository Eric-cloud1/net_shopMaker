using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using MakerShop.Orders;
using MakerShop.Utility;

public partial class Admin_People_Users_RelatedOrderDialog : System.Web.UI.UserControl
{
    private string SortDirection
    {
        get
        {

            if (Session["SortDirection_RelatedOrders"] == null)
                return "Asc";
            return Session["SortDirection_RelatedOrders"].ToString();
        }
        set
        {
            Session.Add("SortDirection_RelatedOrders", value);
        }
    }
    private string SortColumn
    {
        get
        {

            if (Session["SortColumn_RelatedOrders"] == null)
                return "OrderNumber";
            return Session["SortColumn_RelatedOrders"].ToString();
        }
        set
        {
            Session.Add("SortColumn_RelatedOrders", value);
        }
    }


    protected DataTable SessionDataSource
    {
        get
        {
            Object ds = Session["DataSource_RelatedOrders"];
            if (ds != null)
                return (DataTable)ds;

            return null;
        }
        set { Session["DataSource_RelatedOrders"] = value; }
    }


    private int _OrderId;
    protected void Page_Load(object sender, EventArgs e)
    {
        _OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);

        Caption.Text = String.Format(Caption.Text, _OrderId);


        if (!Page.IsPostBack)
        {
            if(_OrderId !=0)
                 SessionDataSource = OrderItemDataSource.LoadRelatedOrders(_OrderId);

            if (SessionDataSource!= null)
            {
                RelatedOrdersGrid.DataSource = SessionDataSource;
                RelatedOrdersGrid.DataBind();
            }
        }
    }




    protected void RelatedOrders_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.RelatedOrdersGrid.PageIndex = e.NewPageIndex;
        this.RelatedOrdersGrid.DataSource = SessionDataSource.DefaultView;
        RelatedOrdersGrid.DataBind();

    }


    protected void RelatedOrders_DataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView dr = ((DataRowView)e.Row.DataItem);
        }
    }

    protected string FormatDate(object dataItem)
    {

        DataRowView date = (DataRowView)dataItem;

        return string.Format("{0:MM/dd/yyyy}", date.Row.ItemArray[2]);

    }


    protected void RelatedOrders_Sorting(object sender, GridViewSortEventArgs e)
    {
        string col = e.SortExpression.ToString();


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

        RelatedOrdersGrid.DataSource = SessionDataSource.DefaultView;
        RelatedOrdersGrid.DataBind();


    }

}
