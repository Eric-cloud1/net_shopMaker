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
using System.Globalization;
using ExcelLibrary;


public partial class Admin_Reports_Transaction : System.Web.UI.Page
{

    protected DataTable SessionDataSource
    {
        get
        {
            Object ds = Session["DataSource_Transaction"];
            if (ds != null)
                return (DataTable)ds; // to replace with proper object

            return null;
        }
        set { Session["DataSource_Transaction"] = value; }
    }

    private string SortDirection
    {
        get
        {

            if (Session["SortDirection_Transaction"] == null)
                return "Asc";
            return Session["SortDirection_Transaction"].ToString();
        }
        set
        {
            Session.Add("SortDirection_Transaction", value);
        }
    }

    private string SortColumn
    {
        get
        {

            if (Session["SortColumn_Transaction"] == null)
                return "AffiliateID";
            return Session["SortColumn_Transaction"].ToString();
        }
        set
        {
            Session.Add("SortColumn_Transaction", value);
        }
    }

    protected bool isPostBack
    {
        get
        {
            Object ds = Session["isPostBack_Transaction"];
            if (ds != null)
                return (bool)ds; // to replace with proper object

            return false;
        }
        set { Session["isPostBack_Transaction"] = value; }
    }

    private int reportType = -1;

    private int dateType = 1;
    private DataTable dataTable = new DataTable();
    private string paymentTypeIds = string.Empty;
    private bool load = false;
    private string country = string.Empty;


    protected void Page_Init(object sender, EventArgs e)
    {
        if (dtTransaction.FirstHit)
        {
            dtTransaction.StartDate = System.DateTime.Today;
        }

        dtTransaction.Update += new Admin_UserControls_DatesAndTime.UpdateEventHandler(BindGrid);
    }

    public struct keys
    {
        public string Name, Iso;

        public keys(string name, string iso)
        {
            Name = name;
            Iso = iso;
        }
    }

  
    private void  GetCountryList()
    {
        
            List<keys> cultureList = new List<keys>();
            ListItemCollection countries = new ListItemCollection();
            int i = 0;
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);

        
            foreach (CultureInfo culture in cultures)
            {
                if (culture.Name == string.Empty)
                    continue;

                RegionInfo region = new RegionInfo(culture.LCID);

                if (!(cultureList.Contains(new keys(region.EnglishName,region.ThreeLetterISORegionName))))
                    cultureList.Add(new keys(region.EnglishName,region.ThreeLetterISORegionName));    
            }

 
            var cultureListSorted = from cul in cultureList orderby cul.Name select cul;


            foreach (keys key in cultureListSorted)
            {
                if(key.Iso =="USA")
                    countrydl.Items.Insert(0, new ListItem(key.Name, key.Iso));

                countrydl.Items.Insert(i, new ListItem(key.Name, key.Iso));
                i++;
               
            }

            countrydl.Items.Insert(0, new ListItem("-All Coutries-", ""));


           
            if (!String.IsNullOrEmpty(Request.QueryString["country"]))
            {
              country = Request.QueryString["country"];
              ListItem queryItem = dlReportType.Items.FindByValue(Request.QueryString["country"].Trim());
              dlReportType.SelectedIndex = dlReportType.Items.IndexOf(queryItem);
          }

    }



    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            ViewState["SortDirection"] = "Asc";
            dateType = int.Parse(this.rdDateType.SelectedValue);
            load = false;

            GetCountryList();
            country = countrydl.SelectedValue;

            if (!String.IsNullOrEmpty(Request.QueryString["SKU"]))
                tbSKU.Text = Request.QueryString["SKU"].Trim();

            if (!String.IsNullOrEmpty(Request.QueryString["Gateway"]))
                tbGatewayPrefix.Text = Request.QueryString["Gateway"].Trim();

            if(!String.IsNullOrEmpty(Request.QueryString["paymentGatewayPrefix"]))
                tbGatewayPrefix.Text = Request.QueryString["paymentGatewayPrefix"].Trim();
            
            

            if ((!String.IsNullOrEmpty(Request.QueryString["OrderDate"]))
                || (!String.IsNullOrEmpty(Request.QueryString["PaymentDate"]))
                || (!String.IsNullOrEmpty(Request.QueryString["TransactionDate"])))
            {

                dtTransaction.TimePeriod = 0;
                dtTransaction.StartDate = DateTime.Today;
                dtTransaction.EndDate = DateTime.Today.AddDays(1);
          
            }


            if (!String.IsNullOrEmpty(Request.QueryString["OrderDate"]))
            {
                dtTransaction.TimePeriod = Admin_UserControls_DatesAndTime.Time_Period.Date_Range;
                dtTransaction.StartDate = DateTime.Parse(Request.QueryString["OrderDate"]);
                dtTransaction.EndDate = dtTransaction.StartDate.AddDays(1);

             

                this.rdDateType.SelectedIndex = 0;
                dateType = 1;

                if ((Request.QueryString["EndDate"] != null) && (Request.QueryString["EndDate"] != string.Empty))
                    dtTransaction.EndDate = DateTime.Parse(Request.QueryString["EndDate"]);

            }

            if (!String.IsNullOrEmpty(Request.QueryString["PaymentDate"]))
            {
                dtTransaction.TimePeriod = Admin_UserControls_DatesAndTime.Time_Period.Date_Range;
                dtTransaction.StartDate = DateTime.Parse(Request.QueryString["PaymentDate"]);
                dtTransaction.EndDate = dtTransaction.StartDate.AddDays(1);


                this.rdDateType.SelectedIndex = 1;
                dateType = 3;
            }
            if (!String.IsNullOrEmpty(Request.QueryString["TransactionDate"]))
            {
                dtTransaction.TimePeriod = Admin_UserControls_DatesAndTime.Time_Period.Date_Range;
                dtTransaction.StartDate = DateTime.Parse(Request.QueryString["TransactionDate"]);
                dtTransaction.EndDate = dtTransaction.StartDate.AddDays(1);

               

                this.rdDateType.SelectedIndex = 2;
                dateType = 2;
            }

            if (!String.IsNullOrEmpty(Request.QueryString["reportType"]))
            {
                ListItem queryItem = dlReportType.Items.FindByValue(Request.QueryString["reportType"].Trim());
                dlReportType.SelectedIndex = dlReportType.Items.IndexOf(queryItem);
                load = true;
            }

            if (!String.IsNullOrEmpty(Request.QueryString["paymentTypes"]))
            {
                Payments.Items[0].Selected = false;
                Payments.Items[1].Selected = false;
                Payments.Items[2].Selected = false;

                char dash = '-';
                foreach (string paymentsId in Request.QueryString["paymentTypes"].Split(dash))
                {
                    Payments.Items.FindByValue(paymentsId.Trim()).Selected = true;
                }
            }

            int.TryParse(this.dlReportType.SelectedValue, out reportType);

            if ((!String.IsNullOrEmpty(Request.QueryString["subaffiliates"])&&(Request.QueryString["subaffiliates"].Contains("True"))))
                chkSubAffiliate.Checked = true;


            if (!String.IsNullOrEmpty(Request.QueryString["subaffiliates"]))
                txtSubAffiliate.Text = Request.QueryString["subaffiliates"].Replace("-", ",").Replace("False","").Replace("True","");
            
            HttpCookie subaffiliate = Request.Cookies["subaffiliate"];
            if ((subaffiliate != null) && (subaffiliate.Value != string.Empty))
                this.txtSubAffiliate.Text = subaffiliate.Value;

            if (!String.IsNullOrEmpty(Request.QueryString["affiliates"]))
                txtAffiliates.Text = Request.QueryString["affiliates"].Replace("-", ",");

           


            HttpCookie affiliates = Request.Cookies["affiliates"];
            if ((affiliates != null) && (affiliates.Value != string.Empty))
                this.txtAffiliates.Text = affiliates.Value;

            //currentPage.Value = "1";

            paymentTypeIds = string.Empty;
            foreach (ListItem ids in Payments.Items)
            {
                if (ids.Selected == true)
                    paymentTypeIds += string.Concat(ids.Value, "-");
            }

            paymentTypeIds = paymentTypeIds.Substring(0, paymentTypeIds.Length - 1);


            if ((load == true) || (isPostBack == true))
            {
                SessionDataSource = ReportDataSource.GetTransactionsByDatePromoSubAffiliate(dateType, dtTransaction.StartDate,
                    dtTransaction.EndDate, txtAffiliates.Text.Trim(), 0, this.txtSubAffiliate.Text.Trim(), 0, reportType, paymentTypeIds, tbGatewayPrefix.Text, tbSKU.Text, country);

                bind();
                
            }

            isPostBack = true;
        }


    }

    protected void ProcessButton_Click(object sender, EventArgs e)
    {
        SessionDataSource = null;

        this.gridTransactions.PageIndex = 0;
        dtTransaction.setDateTime();
        BindGrid();
    }

    private void bind()
    {
        gridTransactions.PageIndex = 0;
        gridTransactions.DataSource = SessionDataSource.DefaultView;
        gridTransactions.DataBind();

        lRows.Text = SessionDataSource.DefaultView.Table.Rows.Count.ToString();
        DataView dv = SessionDataSource.DefaultView;
        lOrders.Text = dv.ToTable(true, "OrderNumber").Rows.Count.ToString();
        //lAmount.Text = "$ " + SessionDataSource.Compute("Sum(CAST(TransactionAmount as money))",null).ToString();
        double p = 0;
        int count = 0;
        foreach (DataRow dr in SessionDataSource.Rows)
        {
            double d;
            if (double.TryParse(dr["TransactionAmount"].ToString(), out d))
                p += d;
            if (dr["LastTransactionDate"] == dr["LastSuccessTransactionDate"])
                ++count;
        }
        lAmount.Text = p.ToString("C");
        lSuccessful.Text = count.ToString();
    }

    protected void gridTransactions_Sorting(object sender, GridViewSortEventArgs e)
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
        BindGrid();

    }

   


    protected void BindGrid()
    {
         SessionDataSource = getData();
        int _count = SessionDataSource.Rows.Count;

        bind();
    }

    protected DataTable getData()
    {
        dateType = int.Parse(this.rdDateType.SelectedValue);

        int.TryParse(this.dlReportType.SelectedValue, out reportType);


        int SubAffiliate = 0;
        int AffiliateId = 1;


        if (chkSubAffiliate.Checked)
            SubAffiliate = 1;



        paymentTypeIds = string.Empty;
        foreach (ListItem ids in Payments.Items)
        {
            if (ids.Selected == true)
                paymentTypeIds += string.Concat(ids.Value, "-");
        }

        paymentTypeIds = paymentTypeIds.Substring(0, paymentTypeIds.Length - 1);


        if (this.txtAffiliates.Text == this.twAffiliates.WatermarkText)
            this.txtAffiliates.Text = string.Empty;

        if (this.txtSubAffiliate.Text == twSubAffiliates.WatermarkText)
            this.txtSubAffiliate.Text = string.Empty;

        DataTable data = ReportDataSource.GetTransactionsByDatePromoSubAffiliate(dateType, dtTransaction.StartDate,
          dtTransaction.EndDate, this.txtAffiliates.Text.Trim(), AffiliateId, this.txtSubAffiliate.Text.Trim(), SubAffiliate, reportType, paymentTypeIds, tbGatewayPrefix.Text, tbSKU.Text, country);
        // SessionDataSource = data;

        this.downloadbutton1.DownloadData = data;
        this.downloadbutton1.StartDate = dtTransaction.StartDate;
        this.downloadbutton1.EndDate = dtTransaction.EndDate;
        this.downloadbutton1.FileName = "Transaction";

        return data;



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


    protected string formatTransactionAmount(object dataItem)
    {
        DataRowView row = (DataRowView)dataItem;

        
        return string.Format("{0:0,#.00}", row["TransactionAmount"].ToString());
    }

    protected string formatResponseMessage(object dataItem)
    {
        DataRowView row = (DataRowView)dataItem;

        string message = row["ResponseMessage"].ToString();

        if(message.Length > 8)
            return message.Substring(0, 8);

        return message;
    }

    

    protected string formatOrderDate(object dataItem)
    {
        DataRowView row = (DataRowView)dataItem;

        return ((DateTime)row["OrderDate"]).ToString("MM/dd/yy");
    }

    protected string formatLastTransactionDate(object dataItem)
    {
        DataRowView row = (DataRowView)dataItem;

     
        if ((row["LastTransactionDate"] == null) || (row["LastTransactionDate"].ToString() == string.Empty))
           return string.Empty;


        return ((DateTime)row["LastTransactionDate"]).ToString("MM/dd/yy");

      
    }

    protected string formatLastSuccessTransactionDate(object dataItem)
    {
        DataRowView row = (DataRowView)dataItem;

        if ((row["LastSuccessTransactionDate"] == null) || (row["LastSuccessTransactionDate"].ToString() == string.Empty))
            return string.Empty;


        return ((DateTime)row["LastSuccessTransactionDate"]).ToString("MM/dd/yy");
    }

    protected void gridTransactions_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.gridTransactions.PageIndex = e.NewPageIndex;
       // currentPage.Value = (gridTransactions.PageIndex + 1).ToString();

        BindGrid();
    }

  
}
