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

public partial class Admin_Reports_TaxDetail : MakerShop.Web.UI.MakerShopAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // VALIDATE THE SPECIFIED TAX NAME
        string taxName = Request.QueryString["T"];
        if (!TaxReportDataSource.IsTaxNameValid(taxName)) Response.Redirect("Taxes.aspx");
        HiddenTaxName.Value = taxName;
        TaxNameLink.Text = taxName;

        if (!Page.IsPostBack)
        {
            // SET THE DEFAULT DATE FILTER
            int dateFilter = AlwaysConvert.ToInt(Request.QueryString["D"]);
            ListItem item = DateFilter.Items.FindByValue(dateFilter.ToString());
            if (item != null) DateFilter.SelectedIndex = DateFilter.Items.IndexOf(item);
        }

        //UPDATE DATE FILTER
        UpdateDateFilter();
    }

    protected void DateFilter_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateDateFilter();
        TaxesGrid.DataBind();
    }

    protected void UpdateDateFilter()
    {
        int dateFilter = AlwaysConvert.ToInt(DateFilter.SelectedValue);
        DateTime fromDate;
        DateTime localNow = LocaleHelper.LocalNow;
        switch (dateFilter)
        {
            case 0:
                //today
                HiddenStartDate.Value = new DateTime(localNow.Year, localNow.Month, localNow.Day).ToString();
                HiddenEndDate.Value = string.Empty;
                break;
            case 1:
                //this week
                DateTime firstDayOfWeek = localNow.AddDays(-1 * (int)localNow.DayOfWeek);
                HiddenStartDate.Value = new DateTime(firstDayOfWeek.Year, firstDayOfWeek.Month, firstDayOfWeek.Day).ToString();
                HiddenEndDate.Value = string.Empty;
                break;
            case 2:
                //last week
                DateTime firstDayOfLastWeek = localNow.AddDays((-1 * (int)localNow.DayOfWeek) - 7);
                DateTime lastDayOfLastWeek = firstDayOfLastWeek.AddDays(6);
                HiddenStartDate.Value = new DateTime(firstDayOfLastWeek.Year, firstDayOfLastWeek.Month, firstDayOfLastWeek.Day).ToString();
                HiddenEndDate.Value = new DateTime(lastDayOfLastWeek.Year, lastDayOfLastWeek.Month, lastDayOfLastWeek.Day, 23, 59, 59).ToString();
                break;
            case 3:
                //this month
                HiddenStartDate.Value = new DateTime(localNow.Year, localNow.Month, 1).ToString();
                HiddenEndDate.Value = string.Empty;
                break;
            case 4:
                //last month
                DateTime lastMonth = DateTime.Now.AddMonths(-1);
                HiddenStartDate.Value = new DateTime(lastMonth.Year, lastMonth.Month, 1).ToString();
                DateTime lastDayOfLastMonth = new DateTime(lastMonth.Year, DateTime.Now.Month, 1).AddDays(-1);
                HiddenEndDate.Value = lastDayOfLastMonth.ToString();
                break;
            case 5:
                //LAST 15 DAYS
                fromDate = localNow.AddDays(-15);
                HiddenStartDate.Value = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day).ToString();
                HiddenEndDate.Value = string.Empty;
                break;
            case 6:
                //LAST 30 DAYS
                fromDate = localNow.AddDays(-30);
                HiddenStartDate.Value = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day).ToString();
                HiddenEndDate.Value = string.Empty;
                break;
            case 7:
                //LAST 60 DAYS
                fromDate = localNow.AddDays(-60);
                HiddenStartDate.Value = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day).ToString();
                HiddenEndDate.Value = string.Empty;
                break;
            case 8:
                //LAST 90 DAYS
                fromDate = localNow.AddDays(-90);
                HiddenStartDate.Value = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day).ToString();
                HiddenEndDate.Value = string.Empty;
                break;
            case 9:
                //LAST 120 DAYS
                fromDate = localNow.AddDays(-120);
                HiddenStartDate.Value = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day).ToString();
                HiddenEndDate.Value = string.Empty;
                break;
            case 10:
                //This Year
                HiddenStartDate.Value = new DateTime(localNow.Year, 1, 1).ToString();
                HiddenEndDate.Value = string.Empty;
                break;
            default:
                //DEFAULT TO ALL DATES
                HiddenStartDate.Value = string.Empty;
                HiddenEndDate.Value = string.Empty;
                break;
        }
        ReportCaption.Text = string.Format(ReportCaption.Text, Server.HtmlEncode(HiddenTaxName.Value));
        if (!string.IsNullOrEmpty(HiddenStartDate.Value))
        {
            ReportCaptionDateRange.Visible = true;
            ReportCaptionDateRange.Text = string.Format(ReportCaptionDateRange.Text, AlwaysConvert.ToDateTime(HiddenStartDate.Value, localNow), AlwaysConvert.ToDateTime(HiddenEndDate.Value, localNow));
        }
    }

    protected string GetOrderLink(object dataItem)
    {
        TaxReportDetailItem detail = (TaxReportDetailItem)dataItem;
        return string.Format("~/Admin/Orders/ViewOrder.aspx?OrderId={0}&OrderNumber={1}", detail.OrderId, detail.OrderNumber);
    }
}