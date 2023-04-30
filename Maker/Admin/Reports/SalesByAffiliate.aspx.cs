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
using System.Text.RegularExpressions;
using MakerShop.Common;
using MakerShop.Catalog;
using MakerShop.Orders;
using MakerShop.Products;
using MakerShop.Stores;
using MakerShop.Users;
using MakerShop.Utility;
using MakerShop.Reporting;
using System.Text;
using MakerShop.Marketing;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;

public partial class Admin_Reports_SalesByAffiliate : MakerShop.Web.UI.MakerShopAdminPage
{


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            DateTime localNow = LocaleHelper.LocalNow;
            PickerAndCalendar1.SelectedDate = localNow.AddDays(-(localNow.Day)+1);
            PickerAndCalendar2.SelectedDate = localNow;
            //XXX?
           // DateTime reportDate = PickerAndCalendar1.SelectedDate;
            //ViewState["ReportDate"] = reportDate;
            ViewState["GridSelectedIndex"] = -1;
            UpdateDateFilter();
        }
    }

    protected void UpdateDateFilter()
    {
        GenerateReport();
    }
    
    protected void SubAffiliateSalesGrid_Sorting(object sender, GridViewSortEventArgs e)
    {
        ((DataList)this.FindControl("AffiliateSalesGrid")).SelectedIndex = (int)ViewState["GridSelectedIndex"];
        if (e.SortExpression != ((MakerShop.Web.UI.WebControls.SortedGridView)sender).SortExpression) e.SortDirection = SortDirection.Descending;
    }

    protected void SubAffiliateSalesGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        ((DataList)this.FindControl("AffiliateSalesGrid")).SelectedIndex = (int)ViewState["GridSelectedIndex"];
        ((MakerShop.Web.UI.WebControls.SortedGridView)sender).PageIndex= e.NewPageIndex;
        ((MakerShop.Web.UI.WebControls.SortedGridView)sender).DataBind(); ;
    }

    protected void AffiliateSalesGrid_ItemCommand(object Sender, DataListCommandEventArgs e)
    {
        // change the Datalist to selected index
        string cmd = ((ImageButton)e.CommandSource).CommandName;
        DataList sender = (DataList)Sender;

        ViewState["GridSelectedIndex"] = e.Item.ItemIndex;

        if (cmd == "select")
              sender.SelectedIndex = (int)ViewState["GridSelectedIndex"];

        if (cmd == "group")
              sender.SelectedIndex = -1;


        UpdateDateFilter();

    }

    protected string GetDetailUrl(object dataItem)
    {
        AffiliateSalesSummary a = (AffiliateSalesSummary)dataItem;
        StringBuilder url = new StringBuilder();
        url.Append("SalesByAffiliateDetail.aspx?AffiliateId=" + a.AffiliateId.ToString());
        url.Append(string.Format("&ReportDate={0:MMddyyyy}", a.StartDate));
        return url.ToString();
            
    }

    private void GenerateReport()
    {
       // xxxx ?
       // DateTime reportDate = (DateTime)ViewState["ReportDate"];
        try
        {
            //GET THE REPORT DATE
            DateTime startOfMonth = Convert.ToDateTime(PickerAndCalendar1.SelectedDate.ToShortDateString());
            DateTime endOfMonth = Convert.ToDateTime(PickerAndCalendar2.SelectedDate.AddDays(1).ToShortDateString()).AddMilliseconds(-1);

            //UPDATE REPORT CAPTION
            ReportDateCaption.Visible = true;
            ReportDateCaption.Text = string.Format(ReportDateCaption.Text, startOfMonth);
            ReportTimestamp.Text = string.Format(ReportTimestamp.Text, LocaleHelper.LocalNow);
            //GET SUMMARIES

            DataTable datasourceId = new DataTable();

            datasourceId = MakerShop.Reporting.ReportDataSource.GetSalesByAffiliateSummary(startOfMonth, endOfMonth, null);

            if (datasourceId.Rows.Count == 0)
                ((System.Web.UI.WebControls.Table)this.FindControl("tblNoRecord")).Visible = true;

            AffiliateSalesGrid.DataSource = datasourceId;
            AffiliateSalesGrid.Visible = true;
            AffiliateSalesGrid.DataBind();
        }
        catch { }
    }

    protected DataTable getSubData(int AffiliateId)
    {
        DateTime startOfMonth = Convert.ToDateTime(PickerAndCalendar1.SelectedDate.ToShortDateString());
        DateTime endOfMonth = Convert.ToDateTime(PickerAndCalendar2.SelectedDate.ToShortDateString());

        DataTable ds = new DataTable();
        //ds = MakerShop.Reporting.ReportDataSource.GetSalesByAffiliate(startOfMonth, endOfMonth, 0, subAffiliate);

        ds = MakerShop.Reporting.ReportDataSource.GetSalesByAffiliateSummary(startOfMonth, endOfMonth, AffiliateId);

        return ds;

    }

    protected string GetCommissionRate(object dataItem)
    {
        DataRowView row = (DataRowView)dataItem;

        if (row["CommissionIsPercent"].ToString() == "0")
        {
            string format = "{0:0.##}% of {1:c}  ";

            if (row["CommissionOnTotal"].ToString() == "0") return string.Format(format, (decimal)row["CommissionRate"], (decimal)row["OrderTotal"]);
            return string.Format(format, (decimal)row["CommissionRate"], (decimal)row["ProductSubtotal"]);

        }
        return string.Format("{0} x {1:c} = ", (int)row["OrderCount"], (decimal)row["CommissionRate"]);
    }

    protected string GetCommission(object dataItem)
    {
        DataRowView row = (DataRowView)dataItem;

        return string.Format("{0:c}",(decimal)row["Commission"]);
    }

    protected string GetConversionRate(object dataItem)
    {
        DataRowView row = (DataRowView)dataItem;
        if (row["ReferralCount"].ToString() == "0") return "-";
        return string.Format("{0:0.##}%", (decimal)row["ConversionRate"]);
    }

    protected string GetOrderTotal(object dataItem)
    {
        DataRowView row = (DataRowView)dataItem;

  
        if (row["CommissionIsPercent"].ToString() == "0")
        {
            if (row["CommissionOnTotal"].ToString() == "0") { return string.Format("{0:c}", (decimal)row["OrderTotal"]); }
            return string.Format("{0:c}", (decimal)row["ProductSubtotal"]);
        }
      
         return string.Format("{0:c}", (decimal)row["OrderTotal"]);  
  
    }

    protected void UpdateReport_Click(object sender, EventArgs e)
    {
        UpdateDateFilter();
    }

    protected void EmailReport_Click(object sender, EventArgs e)
    {
        List<AffiliateSalesSummary> summaries = 
            MakerShop.Reporting.ReportDataSource.GetSalesByAffiliate(
            LocaleHelper.FromLocalTime(Convert.ToDateTime(PickerAndCalendar1.SelectedDate.ToShortDateString())), 
            LocaleHelper.FromLocalTime(Convert.ToDateTime(PickerAndCalendar2.SelectedDate.AddDays(1).ToShortDateString()).AddMilliseconds(-1)),
            0);
        MemoryStream mm = new MemoryStream();
        StreamWriter sw = new StreamWriter(mm);
        sw.AutoFlush = true;


        sw.Write("Affiliate", "Referrals", "Conversion", "Orders", "Successful", "Pending", "Products", "Total", "Commision");

        sw.Write(sw.NewLine);

   
        for (int k = 0; k < summaries.Count; ++k)
        {
           
            Affiliate affiliate = AffiliateDataSource.Load(summaries[k].AffiliateId);

            sw.Write(affiliate.Name);
            sw.Write(",");
            sw.Write(summaries[k].ReferralCount.ToString());
            sw.Write(",");
            sw.Write(summaries[k].ConversionRate.ToString());
            sw.Write(",");
            sw.Write(summaries[k].OrderCount.ToString());
            sw.Write(",");
            sw.Write(summaries[k].Successful.ToString());
            sw.Write(",");
            sw.Write(summaries[k].Pending.ToString());
            sw.Write(",");
            sw.Write(summaries[k].ProductSubtotal.ToString());
            sw.Write(",");
            sw.Write(summaries[k].OrderTotal.ToString());
            sw.Write(",");
            sw.Write(summaries[k].Commission.ToString());
            

            sw.Write(sw.NewLine);
        }
        sw.Flush();

        mm.Seek(0, 0);
        System.Net.Mail.Attachment attachement = new System.Net.Mail.Attachment(mm, "Sales_by_Affiliate_" + DateTime.Now.Month.ToString() + '-' +
            DateTime.Now.Day.ToString() + '-' + DateTime.Now.Year.ToString() + ".csv");
        if (ViewState["SentFile"] != null)
        {
            if (ViewState["SentFile"].ToString() == attachement.Name)
                return;
            else
                ViewState.Remove("SentFile");
        }
        ViewState.Add("SentFile", attachement.Name);
        System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
        mail.From = new System.Net.Mail.MailAddress("viroplex@viralplex.com");
        mail.Subject = "Sales_by_Affiliate" + DateTime.Now.Month.ToString() + '-' + DateTime.Now.Day.ToString() + '-' + DateTime.Now.Year.ToString();
        mail.To.Add(new System.Net.Mail.MailAddress(MakerShop.Common.Token.Instance.User.Email));
        mail.Attachments.Add(attachement);
        MakerShop.Messaging.EmailClient.Send(mail);
    }

    #region Abandon format XLSX file
    public void emailXLSXFile()
    {
        List<AffiliateSalesSummary> summaries =
        MakerShop.Reporting.ReportDataSource.GetSalesByAffiliate(
        LocaleHelper.FromLocalTime(Convert.ToDateTime(PickerAndCalendar1.SelectedDate.ToShortDateString())),
        LocaleHelper.FromLocalTime(Convert.ToDateTime(PickerAndCalendar2.SelectedDate.AddDays(1).ToShortDateString()).AddMilliseconds(-1)),
        0);


        MemoryStream mm = new MemoryStream();

        mm = CreateXLSXFile(summaries);

        mm.Seek(0, SeekOrigin.Begin);
        System.Net.Mime.ContentType contentType = new System.Net.Mime.ContentType();
        contentType.MediaType = System.Net.Mime.MediaTypeNames.Application.Octet;
        contentType.Name = "Sales_by_Affiliate_" + DateTime.Now.Month.ToString() + '-' +
            DateTime.Now.Day.ToString() + '-' + DateTime.Now.Year.ToString() + ".xls";


        System.Net.Mail.Attachment attachement = new System.Net.Mail.Attachment(mm, contentType);
        if (ViewState["SentFile"] != null)
        {
            if (ViewState["SentFile"].ToString() == attachement.Name)
                return;
            else
                ViewState.Remove("SentFile");
        }
        ViewState.Add("SentFile", attachement.Name);
        System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
        mail.From = new System.Net.Mail.MailAddress("viroplex@viralplex.com");
      //  mail.From = new System.Net.Mail.MailAddress("ericf@telecomworldus.com");
        mail.Subject = "Sales_by_Affiliate" + DateTime.Now.Month.ToString() + '-' + DateTime.Now.Day.ToString() + '-' + DateTime.Now.Year.ToString();
        mail.To.Add(new System.Net.Mail.MailAddress(MakerShop.Common.Token.Instance.User.Email));
        mail.Attachments.Add(attachement);
        MakerShop.Messaging.EmailClient.Send(mail);
    }

    public MemoryStream CreateXLSXFile(List<AffiliateSalesSummary> summaries)
    {
        MemoryStream stream = new MemoryStream();

        XmlTextWriter x = new XmlTextWriter(stream, Encoding.UTF8);

           
            x.WriteRaw("<?xml version=\"1.0\"?><?mso-application progid=\"Excel.Sheet\"?>");
            x.WriteRaw("<ss:Workbook xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\" ");
            x.WriteRaw("xmlns:o=\"urn:schemas-microsoft-com:office:office\" ");
            x.WriteRaw("xmlns:x=\"urn:schemas-microsoft-com:office:excel\">");

           // x.WriteRaw("<workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" ");
           // x.WriteRaw("xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">");

           // x.WriteRaw("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\" ?>");
            //x.WriteRaw("<workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" >");
       

            x.WriteRaw("<ss:Styles>");
            x.WriteRaw("<ss:Style ss:ID='sText'><ss:NumberFormat ss:Format='@'/></ss:Style>");

            //create a format for currency
            x.WriteRaw("<ss:Style ss:ID='sCurrency'><ss:NumberFormat ss:Format='@'/></ss:Style>");
            x.WriteRaw("<ss:Style ss:ID='sDate'><ss:NumberFormat ss:Format='[$-409]m/d/yy\\ h:mm\\ AM/PM;@'/></ss:Style>");
            x.WriteRaw("</ss:Styles>");

            x.WriteRaw("<ss:Worksheet ss:Name='Sheet 0'>");
            x.WriteRaw("<ss:Table>");
            x.WriteRaw("<ss:Column ss:StyleID='sText'/>");
            x.WriteRaw("<ss:Column ss:StyleID='sText'/>");
            x.WriteRaw("<ss:Column ss:StyleID='sText'/>");
            x.WriteRaw("<ss:Column ss:StyleID='sText'/>");
            x.WriteRaw("<ss:Column ss:StyleID='sText'/>");
            x.WriteRaw("<ss:Column ss:StyleID='sText'/>");
            x.WriteRaw("<ss:Column ss:StyleID='sCurrency'/>");
            x.WriteRaw("<ss:Column ss:StyleID='sCurrency'/>");
            x.WriteRaw("<ss:Column ss:StyleID='sCurrency'/>");

            //column headers
            x.WriteRaw("<ss:Row>");

            x.WriteRaw("<ss:Cell><ss:Data ss:Type='String'>");
            x.WriteRaw("Affiliate");
            x.WriteRaw("</ss:Data></ss:Cell>");

            x.WriteRaw("<ss:Cell><ss:Data ss:Type='String'>");
            x.WriteRaw("Referral");
            x.WriteRaw("</ss:Data></ss:Cell>");

            x.WriteRaw("<ss:Cell><ss:Data ss:Type='String'>");
            x.WriteRaw("Conversion");
            x.WriteRaw("</ss:Data></ss:Cell>");

            x.WriteRaw("<ss:Cell><ss:Data ss:Type='String'>");
            x.WriteRaw("Orders");
            x.WriteRaw("</ss:Data></ss:Cell>");

            x.WriteRaw("<ss:Cell><ss:Data ss:Type='String'>");
            x.WriteRaw("Successful");
            x.WriteRaw("</ss:Data></ss:Cell>");

            x.WriteRaw("<ss:Cell><ss:Data ss:Type='String'>");
            x.WriteRaw("Pending");
            x.WriteRaw("</ss:Data></ss:Cell>");

            x.WriteRaw("<ss:Cell><ss:Data ss:Type='String'>");
            x.WriteRaw("ProductSubtotal");
            x.WriteRaw("</ss:Data></ss:Cell>");

            x.WriteRaw("<ss:Cell><ss:Data ss:Type='String'>");
            x.WriteRaw("OrderTotal");
            x.WriteRaw("</ss:Data></ss:Cell>");

            x.WriteRaw("<ss:Cell><ss:Data ss:Type='String'>");
            x.WriteRaw("Commission");
            x.WriteRaw("</ss:Data></ss:Cell>");
          
            x.WriteRaw("</ss:Row>");
        
            for (int k = 0; k < summaries.Count; ++k)
            {
                
                Affiliate affiliate = AffiliateDataSource.Load(summaries[k].AffiliateId);

                x.WriteRaw("<ss:Row>");
                x.WriteRaw("<ss:Cell><ss:Data ss:Type='String'>");
                x.WriteString(affiliate.Name);
                x.WriteRaw("</ss:Data></ss:Cell>");

                x.WriteRaw("<ss:Cell><ss:Data ss:Type='String'>");
                x.WriteString(summaries[k].ConversionRate.ToString());
                x.WriteRaw("</ss:Data></ss:Cell>");

                x.WriteRaw("<ss:Cell><ss:Data ss:Type='String'>");
                x.WriteString(summaries[k].OrderCount.ToString());
                x.WriteRaw("</ss:Data></ss:Cell>");

                x.WriteRaw("<ss:Cell><ss:Data ss:Type='String'>");
                x.WriteString(summaries[k].Successful.ToString());
                x.WriteRaw("</ss:Data></ss:Cell>");

                x.WriteRaw("<ss:Cell><ss:Data ss:Type='String'>");
                x.WriteString(summaries[k].Pending.ToString());
                x.WriteRaw("</ss:Data></ss:Cell>");

                x.WriteRaw("<ss:Cell><ss:Data ss:Type='String'>");
                x.WriteString(summaries[k].ProductSubtotal.ToString());
                x.WriteRaw("</ss:Data></ss:Cell>");

                x.WriteRaw("<ss:Cell><ss:Data ss:Type='String'>");
                x.WriteString(summaries[k].OrderTotal.ToString());
                x.WriteRaw("</ss:Data></ss:Cell>");


                x.WriteRaw("<ss:Cell><ss:Data ss:Type='String'>");
                x.WriteString(summaries[k].Commission.ToString());
                x.WriteRaw("</ss:Data></ss:Cell>");

                x.WriteRaw("</ss:Row>");
                
            }
        
            x.WriteRaw("</ss:Table></ss:Worksheet>");

            x.WriteRaw("</ss:Workbook>");

            x.Flush();

          stream.Flush();

       return stream;

    }

    #endregion

}

