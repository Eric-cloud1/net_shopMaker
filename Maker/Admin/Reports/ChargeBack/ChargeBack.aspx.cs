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
using System.Linq;

public partial class Admin_Reports_ChargeBack_Chargeback : System.Web.UI.Page
{

    protected List<ChargeBack> SessionDataSource
    {
        get
        {
            Object ds = ViewState["DataSource"];
            if (ds != null)
                return (List<ChargeBack>)ds;

            return null;
        }
        set { ViewState["DataSource"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
        {
            ViewState["SortDirection"] = "Asc";

            SessionDataSource = ReportDataSource.GetVPlexChargeBackByDate(dtChargeBack.StartDate, dtChargeBack.EndDate, int.Parse(paymentInstrument.SelectedValue));
            CBGrid.DataSource = SessionDataSource;
            CBGrid.DataBind();
        }

    }

    protected void BindGrid()
    {
        SessionDataSource = ReportDataSource.GetVPlexChargeBackByDate(dtChargeBack.StartDate, dtChargeBack.EndDate, int.Parse(paymentInstrument.SelectedValue));
        CBGrid.DataSource = SessionDataSource;
        CBGrid.DataBind();
    }

    protected void ProcessButton_Click(object sender, EventArgs e)
    {
        this.dtChargeBack.setDateTime();
        BindGrid();
    }

    protected void ChargeBackGrid_Sorting(object sender, GridViewSortEventArgs e)
    {
        //Sorting Viewstate datasource based on the SortExpression selected
        string order = e.SortExpression.ToString();

        if (ViewState["SortDirection"] == "Asc")
        {
            e.SortDirection = SortDirection.Descending;
        }

        if (ViewState["SortDirection"] == "Desc")
        {
            e.SortDirection = SortDirection.Descending;
            ViewState["SortDirection"] = "Asc";
        }




        List<ChargeBack> charges = SessionDataSource;
        var sortedChargeBack = from cb in charges orderby cb.Name select cb;
        switch(order)
        {
            case "CBRatio": 
                sortedChargeBack = from cb in charges orderby cb.CBRatio descending select cb;
                break;

            case "ChargeBacks":
                sortedChargeBack = from cb in charges orderby cb.Chargeback descending select cb;
                break;

            case "Successful":
                sortedChargeBack = from cb in charges orderby cb.Successful descending select cb;
                break;
            case "Auth":
                sortedChargeBack = from cb in charges orderby cb.Authorized descending select cb;
                break;

            
        }

        //Casting the Ienumerable collection to the datasource datetype
        List<ChargeBack> Sortedcharges = new List<ChargeBack>(sortedChargeBack);

        //bind grid to trigger a refresh
        CBGrid.DataSource = Sortedcharges;
        CBGrid.DataBind();

    }

    protected string FormatPercent(object dataItem)
    {
        ChargeBack a = (ChargeBack)dataItem;

        if (a.CBRatio == 0)
            return "0%";

        if (a.CBRatio == 100) 
            return "100%";
     
        string cbRatio = string.Format("{0:0,#.00%}", a.CBRatio);

    
        return cbRatio.Substring(1, cbRatio.Length - 1);

    }

    protected string FormatTotalPercent(object dataItem)
    {
        ChargeBack a = (ChargeBack)dataItem;

        if (a.CBRatioTotal == 0)
            return "0%";

        if (a.CBRatioTotal == 100)
            return "100%";

        string cbRatio = string.Format("{0:0,#.00%}", a.CBRatioTotal);


        return cbRatio.Substring(1, cbRatio.Length - 1);

    }

   

    protected void ChargeBackGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        CBGrid.PageIndex = e.NewPageIndex;
        CBGrid.DataSource = SessionDataSource;
        CBGrid.DataBind(); ;
    }

}
