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
using System.Collections.Generic;
using MakerShop.Common;
using MakerShop.Utility;
using System.Linq;
using MakerShop.Reporting.Trial;
using System.IO;
using System.Text;
using System.Xml;
using System.Reflection;
using ExcelLibrary;


public partial class Admin_Reports_Trial_TrialStatus : System.Web.UI.Page
{
    private int gross = -1;
    private int hardx = -1;
    private int hardxpre = -1;
    private int hardxpost = -1;
    private int attempted = -1;
    private int authorized = -1;
    private int captured = -1;
    private int approved = -1;
    private int decline = -1;
    private int refund = -1;


    private int grossfraud = -1;
    private int notattempted = -1;
    private int declinereducedapproved = -1;
    private int declinereducedattempted = -1;


    private DataTable dataTable = new DataTable();


    protected DataTable SessionDataSource
    {
        get
        {
            Object ds = Session["DataSource_TrialStatus"];
            if (ds != null)
                return (DataTable)ds; // to replace with proper object

            return null;
        }
        set { Session["DataSource_TrialStatus"] = value; }
    }

    private string SortDirection
    {
        get
        {

            if (Session["SortDirection_TrialStatus"] == null)
                return "Asc";
            return Session["SortDirection_TrialStatus"].ToString();
        }
        set
        {
            Session.Add("SortDirection_TrialStatus", value);
        }
    }
    private string SortColumn
    {
        get
        {

            if (Session["SortColumn_TrialStatus"] == null)
                return "AffiliateID";
            return Session["SortColumn_TrialStatus"].ToString();
        }
        set
        {
            Session.Add("SortColumn_TrialStatus", value);
        }
    }
  

    protected void Page_Init(object sender, EventArgs e)
    {
        if (dtTrialStatus.FirstHit)
        {
            dtTrialStatus.TimePeriod = Admin_UserControls_DatesAndTime.Time_Period.Last_15_Days;
        }

        dtTrialStatus.Update += new Admin_UserControls_DatesAndTime.UpdateEventHandler(BindGrid);

    }
    protected void Page_Load(object sender, EventArgs e)
    {

        this.gridCharge.Columns[0].HeaderText = "Order_Date";
        this.gridCharge.Columns[1].Visible = false;


        if (this.rdGroupBy.SelectedValue == "affiliateId")
            this.gridCharge.Columns[0].HeaderText = "Affiliate";


        if (this.rdGroupBy.SelectedValue == "Subaffiliate")
        {
            this.gridCharge.Columns[0].HeaderText = "Affiliate";
            this.gridCharge.Columns[1].HeaderText = "Subaffiliate";
            this.gridCharge.Columns[1].Visible = true;
        }

        if (!Page.IsPostBack)
        {

            gridCharge.PageIndex = 0;

            this.dlSku.DataSource = ReportDataSource.GetSku();
            this.dlSku.DataTextField = "Sku";
            this.dlSku.DataValueField = "value";
            this.dlSku.DataBind();


            dataTable = ReportDataSource.GetTrialChargesByDatePromoSubAffiliate(dtTrialStatus.StartDate, dtTrialStatus.EndDate,
             this.txtAffiliates.Text, 1, this.txtSubAffiliate.Text, 0, this.dlSku.SelectedValue, this.rdGroupBy.SelectedValue, paymentGatewayPrefixTxt.Text);

            SessionDataSource = dataTable;


            string colName = string.Empty;
            for (int i = 0; i < SessionDataSource.Columns.Count; ++i)
            {
                colName = SessionDataSource.Columns[i].ColumnName;

                switch (colName)
                {
                    case "Gross":
                        gross = i;
                        break;
                    case "Hardx":
                        hardx = i;
                        break;
                    case "HardxPreTrial":
                        hardxpre = i;
                        break;
                    case "HardXPostTrial":
                        hardxpost = i;
                        break;
                    case "Attempted":
                        attempted = i;
                        break;
                    case "Authorized":
                        authorized = i;
                        break;
                    case "Captured":
                        captured = i;
                        break;
                    case "Approved":
                        approved = i;
                        break;
                    case "Decline":
                        decline = i;
                        break;
                    case "Refund":
                        refund = i;
                        break;
                    case "NotAttempted":
                        notattempted = i;
                        break;
                    case "GrossFraud":
                        grossfraud = i;
                        break;
                    case "DeclineReducedAttempted":
                        declinereducedattempted = i;
                        break;
                    case "DeclineReducedApproved":
                        declinereducedapproved = i;
                        break;
                

                }
            }

          //  GridChargeBind(SessionDataSource);

        }
        // BindGrid();
        GridChargeBind(SessionDataSource);
    }



    protected void gridCharge_Sorting(object sender, GridViewSortEventArgs e)
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
        GridChargeBind(SessionDataSource);

    }


    protected void gridCharge_PreRender(object sender, EventArgs e)
    {
        foreach (GridViewRow gvr in gridCharge.Rows)
        {
            foreach (TableCell tc in gvr.Cells)
                fixPop(tc);
        }
    }

    private void fixPop(TableCell tc)
    {
  
        foreach (Control c in tc.Controls)
            if (c is Label)
            {
                Label l = (Label)c;
                decimal d;

                if (decimal.TryParse(l.Text, out d))
                {
                    if ((d > 0) && (tc.Attributes["id"] != null))
                    {
                        tc.Attributes.Add("parent", "GridTable");
                        tc.Attributes.Add("SkinID", string.Format("Link"));
                        tc.Attributes.Add("style", "cursor:pointer;text-decoration:underline");
                    }
       
                }    
            }
    }


    protected void gridCharge_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        bool subExactMatch = (chkSubAffiliate.Checked && true);
        string subaffiliates = string.Empty;


        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView dr = ((DataRowView)e.Row.DataItem);
            string ids = string.Empty;

            for (int i = 0; i <= e.Row.Cells.Count - 1; i++)
            {

                if (rdGroupBy.SelectedValue == "Subaffiliate")
                {
                    subaffiliates = dr["Subaffiliate"].ToString();
                    #region Subaffiliate
                    if (i == gross)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "1", "1", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, dr["Subaffiliate"].ToString(), string.Format("{0} {1}", subaffiliates, subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));
        
                    }
                    else if (i == hardx)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "2", "1", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, dr["Subaffiliate"].ToString(), string.Format("{0} {1}", subaffiliates, subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }
                    else if (i == hardxpre)
                    {

                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "3", "1", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, dr["Subaffiliate"].ToString(), string.Format("{0} {1}", subaffiliates, subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }
                    else if (i == hardxpost)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "4", "1", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, dr["Subaffiliate"].ToString(), string.Format("{0} {1}", subaffiliates, subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));
                   }
                    else if (i == authorized)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "7", "3", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, dr["Subaffiliate"].ToString(), string.Format("{0} {1}", subaffiliates, subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }

                    else if (i == captured)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "8", "3", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, dr["Subaffiliate"].ToString(), string.Format("{0} {1}", subaffiliates, subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));


                    }

                    else if (i == decline)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "9", "3", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, dr["Subaffiliate"].ToString(), string.Format("{0} {1}", subaffiliates, subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));


                    }
                    else if (i == attempted)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "10", "3", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, dr["Subaffiliate"].ToString(), string.Format("{0} {1}", subaffiliates, subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }
                    else if (i == refund)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "11", "3", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, dr["Subaffiliate"].ToString(), string.Format("{0} {1}", subaffiliates, subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));


                    }

                    else if (i == grossfraud)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "111", "1-3-4", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, dr["Subaffiliate"].ToString(), string.Format("{0} {1}", subaffiliates, subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }

                    else if (i == notattempted)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "112", "1-3-4", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, dr["Subaffiliate"].ToString(), string.Format("{0} {1}", subaffiliates, subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }

                    else if (i == declinereducedattempted)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "114", "1-3-4", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, dr["Subaffiliate"].ToString(), string.Format("{0} {1}", subaffiliates, subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));


                    }

                    else if (i == declinereducedapproved)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "113", "1-3-4", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, dr["Subaffiliate"].ToString(), string.Format("{0} {1}", subaffiliates, subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));


                    }
                   
                    else if (i == approved)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "115", "1-3-4", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, dr["Subaffiliate"].ToString(), string.Format("{0} {1}", subaffiliates, subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }
                  
                  
                  
                    #endregion

                }

                else if (this.rdGroupBy.SelectedValue == "affiliateId") 
                {
                    #region AffiliateId
                    if (i == gross)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "1", "1", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, "");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }
                    else if (i == hardx)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "2", "1", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, "");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }
                    else if (i == hardxpre)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "3", "1", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, "");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                   }
                    else if (i == hardxpost)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "4", "1", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, "");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

       
                    }
                    else if (i == authorized)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "7", "3", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, "");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                   }

                    else if (i == captured)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "8", "3", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, "");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }

                    else if (i == decline)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "9", "3", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, "");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));


                     }
                    else if (i == attempted)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "10", "3", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, "");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }
                    else if (i == refund)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "11", "1-3", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, "");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

               
                    }
                    else if (i == grossfraud)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "111", "1-3-4", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, "");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

     
                    }

                    else if (i == notattempted)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "112", "1-3-4", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, "");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));


                 
                    }
                    else if (i == declinereducedattempted)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "114", "1-3-4", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, "");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }

                    else if (i == declinereducedapproved)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "113", "1-3-4", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, "");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));


                    }
                 


                   else if (i == approved)
                    {
                        ids = string.Format("OrderDate={0}&EndDate={1}&reportType={2}&paymentTypes={3}&affiliates={4}&paymentGatewayPrefix={5}&subaffiliates={6}", dtTrialStatus.StartDate, dtTrialStatus.EndDate, "115", "1-3-4", dr["GroupBy"].ToString(), paymentGatewayPrefixTxt.Text, "");

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                
                    }

                   

                    #endregion

                }

                else
                {
                    #region groupbyDate
                    //       OrderHoverLookupPanel.startCallback(event, "OrderDate=" + myArr[0] + "&reportType=" + myArr[1] + "&paymentTypes=" + myArr[2] + "&paymentGatewayPrefix=" + myArr[3] + "&affiliates=" + myArr[4] + "&subaffiliates=" + myArr[5], null, OnError);
     
                    if (i == gross)
                    {
                        ids = string.Format("OrderDate={0}&reportType={1}&paymentTypes={2}&paymentGatewayPrefix={3}&affiliates={4}&subaffiliates={5}", dr["GroupBy"].ToString(), "1", "1", paymentGatewayPrefixTxt.Text, this.txtAffiliates.Text.Replace(",", "-"), string.Format("{0} {1}",this.txtSubAffiliate.Text.Replace(",", "-"),subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));


                    }
                    else if (i == hardx)
                    {
                        ids = string.Format("OrderDate={0}&reportType={1}&paymentTypes={2}&paymentGatewayPrefix={3}&affiliates={4}&subaffiliates={5}", dr["GroupBy"].ToString(), "2", "1-3", paymentGatewayPrefixTxt.Text, this.txtAffiliates.Text.Replace(",", "-"), string.Format("{0} {1}", this.txtSubAffiliate.Text.Replace(",", "-"), subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                   }
                    else if (i == hardxpre)
                    {
                        ids = string.Format("OrderDate={0}&reportType={1}&paymentTypes={2}&paymentGatewayPrefix={3}&affiliates={4}&subaffiliates={5}", dr["GroupBy"].ToString(), "3", "1", paymentGatewayPrefixTxt.Text, this.txtAffiliates.Text.Replace(",", "-"), string.Format("{0} {1}", this.txtSubAffiliate.Text.Replace(",", "-"), subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }
                    else if (i == hardxpost)
                    {
                        ids = string.Format("OrderDate={0}&reportType={1}&paymentTypes={2}&paymentGatewayPrefix={3}&affiliates={4}&subaffiliates={5}", dr["GroupBy"].ToString(), "4", "3", paymentGatewayPrefixTxt.Text, this.txtAffiliates.Text.Replace(",", "-"), string.Format("{0} {1}", this.txtSubAffiliate.Text.Replace(",", "-"), subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));


                    }
                    else if (i == authorized)
                    {
                        ids = string.Format("OrderDate={0}&reportType={1}&paymentTypes={2}&paymentGatewayPrefix={3}&affiliates={4}&subaffiliates={5}", dr["GroupBy"].ToString(), "7", "3", paymentGatewayPrefixTxt.Text, this.txtAffiliates.Text.Replace(",", "-"), string.Format("{0} {1}", this.txtSubAffiliate.Text.Replace(",", "-"), subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }
                    else if (i == captured)
                    {
                        ids = string.Format("OrderDate={0}&reportType={1}&paymentTypes={2}&paymentGatewayPrefix={3}&affiliates={4}&subaffiliates={5}", dr["GroupBy"].ToString(), "8", "3", paymentGatewayPrefixTxt.Text, this.txtAffiliates.Text.Replace(",", "-"), string.Format("{0} {1}", this.txtSubAffiliate.Text.Replace(",", "-"), subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));


                    }

                    else if (i == decline)
                    {
                        ids = string.Format("OrderDate={0}&reportType={1}&paymentTypes={2}&paymentGatewayPrefix={3}&affiliates={4}&subaffiliates={5}", dr["GroupBy"].ToString(), "9", "3", paymentGatewayPrefixTxt.Text, this.txtAffiliates.Text.Replace(",", "-"), string.Format("{0} {1}", this.txtSubAffiliate.Text.Replace(",", "-"), subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }
                    else if (i == attempted)
                    {
                        ids = string.Format("OrderDate={0}&reportType={1}&paymentTypes={2}&paymentGatewayPrefix={3}&affiliates={4}&subaffiliates={5}", dr["GroupBy"].ToString(), "10", "3", paymentGatewayPrefixTxt.Text, this.txtAffiliates.Text.Replace(",", "-"), string.Format("{0} {1}", this.txtSubAffiliate.Text.Replace(",", "-"), subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                   }
                    else if (i == refund)
                    {
                        ids = string.Format("OrderDate={0}&reportType={1}&paymentTypes={2}&paymentGatewayPrefix={3}&affiliates={4}&subaffiliates={5}", dr["GroupBy"].ToString(), "11", "1-3", paymentGatewayPrefixTxt.Text, this.txtAffiliates.Text.Replace(",", "-"), string.Format("{0} {1}", this.txtSubAffiliate.Text.Replace(",", "-"), subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));


                    }


                    else if (i == grossfraud)
                    {
                        ids = string.Format("OrderDate={0}&reportType={1}&paymentTypes={2}&paymentGatewayPrefix={3}&affiliates={4}&subaffiliates={5}", dr["GroupBy"].ToString(), "111", "3", paymentGatewayPrefixTxt.Text, this.txtAffiliates.Text.Replace(",", "-"), string.Format("{0} {1}", this.txtSubAffiliate.Text.Replace(",", "-"), subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));


                     }

                    else if (i == notattempted)
                    {

                        ids = string.Format("OrderDate={0}&reportType={1}&paymentTypes={2}&paymentGatewayPrefix={3}&affiliates={4}&subaffiliates={5}", dr["GroupBy"].ToString(), "112", "3", paymentGatewayPrefixTxt.Text, this.txtAffiliates.Text.Replace(",", "-"), string.Format("{0} {1}", this.txtSubAffiliate.Text.Replace(",", "-"), subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }

                    else if (i == declinereducedattempted)
                    {
                        ids = string.Format("OrderDate={0}&reportType={1}&paymentTypes={2}&paymentGatewayPrefix={3}&affiliates={4}&subaffiliates={5}", dr["GroupBy"].ToString(), "114", "3", paymentGatewayPrefixTxt.Text, this.txtAffiliates.Text.Replace(",", "-"), string.Format("{0} {1}", this.txtSubAffiliate.Text.Replace(",", "-"), subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }

                    else if (i == declinereducedapproved)
                    {
                        ids = string.Format("OrderDate={0}&reportType={1}&paymentTypes={2}&paymentGatewayPrefix={3}&affiliates={4}&subaffiliates={5}", dr["GroupBy"].ToString(), "113", "3", paymentGatewayPrefixTxt.Text, this.txtAffiliates.Text.Replace(",", "-"), string.Format("{0} {1}", this.txtSubAffiliate.Text.Replace(",", "-"), subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                     }
                     
                    else if (i == approved)
                    {
                        ids = string.Format("OrderDate={0}&reportType={1}&paymentTypes={2}&paymentGatewayPrefix={3}&affiliates={4}&subaffiliates={5}", dr["GroupBy"].ToString(), "115", "3", paymentGatewayPrefixTxt.Text, this.txtAffiliates.Text.Replace(",", "-"), string.Format("{0} {1}", this.txtSubAffiliate.Text.Replace(",", "-"), subExactMatch));

                        if (e.Row.Cells[i].Text != "0")
                            e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

                    }

                    #endregion
                }
            }
        }
    }

    protected void BindGrid()
    {

        int SubAffiliate = 0;
        int PromoCodes = 1;


        if (chkSubAffiliate.Checked)
            SubAffiliate = 1;

        if (this.txtAffiliates.Text == this.twAffiliates.WatermarkText)
            this.txtAffiliates.Text = string.Empty;

        if (this.txtSubAffiliate.Text == twSubAffiliates.WatermarkText)
            this.txtSubAffiliate.Text = string.Empty;

        SessionDataSource = ReportDataSource.GetTrialChargesByDatePromoSubAffiliate(dtTrialStatus.StartDate, dtTrialStatus.EndDate,
        this.txtAffiliates.Text.Trim(), PromoCodes, this.txtSubAffiliate.Text.Trim(), SubAffiliate, this.dlSku.SelectedValue, this.rdGroupBy.SelectedValue, paymentGatewayPrefixTxt.Text);

        this.downloadbutton1.DownloadData = SessionDataSource;
        this.downloadbutton1.StartDate = dtTrialStatus.StartDate;
        this.downloadbutton1.EndDate = dtTrialStatus.EndDate;
        this.downloadbutton1.FileName = "TrialStatus";


        string colName = string.Empty;
        for (int i = 0; i < SessionDataSource.Columns.Count; ++i)
        {
            colName = SessionDataSource.Columns[i].ColumnName;

            switch (colName)
            {
                case "Gross":
                    gross = i;
                    break;
                case "Hardx":
                    hardx = i;
                    break;
                case "HardxPreTrial":
                    hardxpre = i;
                    break;
                case "HardXPostTrial":
                    hardxpost = i;
                    break;
                case "Attempted":
                    attempted = i;
                    break;
                case "Authorized":
                    authorized = i;
                    break;
                case "Captured":
                    captured = i;
                    break;
                case "Approved":
                    approved = i;
                    break;
                case "Decline":
                    decline = i;
                    break;
                case "Refund":
                    refund = i;
                    break;
                case "GrossFraud":
                    grossfraud = i;
                    break;
                case "DeclineReducedAttempted":
                    declinereducedattempted = i;
                    break;
                case "DeclineReducedApproved":
                    declinereducedapproved = i;
                    break;
                case "NotAttempted":
                    notattempted = i;
                    break;

            }
        }

        if ((SessionDataSource == null) || (SessionDataSource.Rows.Count == 0))
        {
            gridCharge.Visible = false;
            EmptyResultsMessage.Visible = true;
            return;
        }
          


        GridChargeBind(SessionDataSource);

    }

    private void GridChargeBind(DataTable dt)
    {
        gridCharge.Visible = true;
        EmptyResultsMessage.Visible = false;
        gridCharge.PageIndex = 0;
        gridCharge.DataSource = dt.DefaultView;
        gridCharge.DataBind();

        if (gridCharge.HeaderRow != null)
            gridCharge.HeaderRow.TableSection = TableRowSection.TableHeader;

      
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
   
    protected string formatSubAffiliate(object dataItem)
    {
        DataRowView row = (DataRowView)dataItem;

        string subAffiliate = row["Subaffiliate"].ToString();

        if (subAffiliate.IndexOf("-") > 0)
            subAffiliate = subAffiliate.Substring(0, subAffiliate.IndexOf("-"));

        return subAffiliate;

    }

    protected string formatHeader()
    {

        if ((this.rdGroupBy.SelectedValue == "PromoCode") || (this.rdGroupBy.SelectedValue == "affiliateId"))
            return "Affiliate";

        return "Date";
    }

    protected string formatDeclineReducedApproved(object dataItem)
    {
        DataRowView row = (DataRowView)dataItem;


        return string.Format("{0:#,##0;<span style='color:red'>(#,##0)</span>}", (int)row["DeclineReducedApproved"]);

    }

    protected string formatDeclineReducedAttempted(object dataItem)
    {
        DataRowView row = (DataRowView)dataItem;

        return string.Format("{0:#,##0;<span style='color:red'>(#,##0)</span>}", (int)row["DeclineReducedAttempted"]);


    }

    protected string formatApprovalRate(object dataItem)
    {
        try
        {
            DataRowView row = (DataRowView)dataItem;

            decimal approvalRate = (decimal)row["ApprovalRate"];

            return string.Format("{0:#,##0.00%;<span style='color:red'>(#,##0.00%)</span>}", approvalRate / 10000);
        }
        catch { return string.Empty; }

    }

    protected string formatDeclinedApprovalRate(object dataItem)
    {
        try
        {
            DataRowView row = (DataRowView)dataItem;

            decimal declinedApprovalRate = (decimal)row["DeclinedApprovalRate"] / 10000;

            return string.Format("{0:#,##0.00%;<span style='color:red'>(#,##0.00%)</span>}", declinedApprovalRate);
        }
        catch { return string.Empty; }


    }

    protected string formatNetINITIALChargeSuccess(object dataItem)
    {
        try
        {
            DataRowView row = (DataRowView)dataItem;

            decimal netChargeSucess = (decimal)row["NetInitialChargeSuccess"] / 10000;

            return string.Format("{0:#,##0.00%;<span style='color:red'>(#,##0.00%)</span>}", netChargeSucess);
        }
        catch { return string.Empty; }


    }

    protected string formatNetFULLChargeSuccess(object dataItem)
    {
        try
        {
            DataRowView row = (DataRowView)dataItem;

            decimal netChargeSucess = (decimal)row["NetFULLChargeSuccess"] / 10000;

            return string.Format("{0:#,##0.00%;<span style='color:red'>(#,##0.00%)</span>}", netChargeSucess);
        }
        catch { return string.Empty; }
    }

    protected string formatFraudRate(object dataItem)
    {
        try
        {
            DataRowView row = (DataRowView)dataItem;

            decimal fraudRate = (decimal)row["FraudRate"] / 10000;

            return string.Format("{0:#,##0.00%;<span style='color:red'>(#,##0.00%)</span>}", fraudRate);
        }
        catch { return string.Empty; }

    }

    protected void gridCharge_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.gridCharge.PageIndex = e.NewPageIndex;

        GridChargeBind(SessionDataSource);
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
        
        dtTrialStatus.setDateTime();
        BindGrid();
    }

}
