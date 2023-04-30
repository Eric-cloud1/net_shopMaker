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
using System.Collections.Generic;

public partial class Admin_Reports_DailySales : MakerShop.Web.UI.MakerShopAdminPage
{

    //CONTAINS THE REPORT
    List<OrderSummary> _DailySales;
    //TRACKS TOTALS FOR FOOTER
    bool _TotalsCalculated;
    LSDecimal _ProductTotal;
    LSDecimal _ShippingTotal;
    LSDecimal _TaxTotal;
    LSDecimal _DiscountTotal;
    LSDecimal _CouponTotal;
    LSDecimal _OtherTotal;
    LSDecimal _GrandTotal;
    LSDecimal _ProfitTotal;
    
    protected int GetDaysInMonth(int month, int year)
    {
        return (new DateTime(year, month, 1).AddMonths(1).AddDays(-1)).Day;
    }
    
    protected void UpdateDateControls()
    {
        DateTime reportDate = (DateTime)ViewState["ReportDate"];
        //BUILD DAY
        int daysInMonth = GetDaysInMonth(reportDate.Month, reportDate.Year);
        DayList.Items.Clear();
        for (int i = 1; i <= daysInMonth; i++)
        {
            ListItem dayItem = new ListItem(i.ToString(), i.ToString());
            if (reportDate.Day == i) dayItem.Selected = true;
            DayList.Items.Add(dayItem);
        }
        //BUILD MONTH
        MonthList.SelectedIndex = -1;
        ListItem monthItem = MonthList.Items.FindByValue(reportDate.Month.ToString());
        if (monthItem != null) monthItem.Selected = true;
        //BUILD YEAR
        YearList.Items.Clear();
        int currentYear = reportDate.Year;
        for (int i = -10; i < 11; i++)
        {
            string thisYear = ((int)(currentYear + i)).ToString();
            ListItem yearItem = new ListItem(thisYear, thisYear);
            if (i == 0) yearItem.Selected = true;
            YearList.Items.Add(yearItem);
        }
        BindReport();
    }

    protected void BindReport()
    {
        //GET THE REPORT DATE
        DateTime reportDate = (DateTime)ViewState["ReportDate"];
        //UPDATE REPORT CAPTION
        ReportCaption.Visible = true;
        ReportCaption.Text = string.Format(ReportCaption.Text, reportDate);
        //RESET THE TOTALS
        _TotalsCalculated = false;
        //GET NEW DATA
        _DailySales = ReportDataSource.GetDailySales(reportDate);
        //BIND GRID
        DailySalesGrid.DataSource = _DailySales;
        DailySalesGrid.DataBind();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            DateTime tempDate = AlwaysConvert.ToDateTime(Request.QueryString["Date"], System.DateTime.MinValue);
            if (tempDate == System.DateTime.MinValue) tempDate = LocaleHelper.LocalNow;
            ViewState["ReportDate"] = tempDate;
            UpdateDateControls();
        }
    }

    protected void NextButton_Click(object sender, EventArgs e)
    {
        DateTime newReportDate = ((DateTime)ViewState["ReportDate"]).AddDays(1);
        ViewState["ReportDate"] = newReportDate;
        UpdateDateControls();
    }

    protected void PreviousButton_Click(object sender, EventArgs e)
    {
        DateTime newReportDate = ((DateTime)ViewState["ReportDate"]).AddDays(-1);
        ViewState["ReportDate"] = newReportDate;
        UpdateDateControls();
    }

    protected void ReportDate_SelectedIndexChanged(object sender, EventArgs e)
    {
        int day = AlwaysConvert.ToInt(DayList.SelectedValue);
        int month = AlwaysConvert.ToInt(MonthList.SelectedValue);
        int year = AlwaysConvert.ToInt(YearList.SelectedValue);
        switch (month)
        {
            case 4:
            case 6:
            case 9:
            case 10:
                if (day > 30) day = 30;
                break;
            case 2:
                if ((day > 28) && ((year % 4) != 0)) day = 28;
                if (day > 29) day = 29;
                break;
        }
        DateTime reportDate;
        try
        {
            reportDate = new DateTime(year, month, day);
        }
        catch
        {
            reportDate = LocaleHelper.LocalNow;
        }
        ViewState["ReportDate"] = reportDate;
        UpdateDateControls();
    }

    protected string GetTotal(string itemType)
    {
        if (!_TotalsCalculated)
        {
            _ProductTotal = 0;
            _ShippingTotal = 0;
            _TaxTotal = 0;
            _DiscountTotal = 0;
            _CouponTotal = 0;
            _OtherTotal = 0;
            _ProfitTotal = 0;
            _GrandTotal = 0;
            foreach (OrderSummary os in _DailySales)
            {
                _ProductTotal += os.ProductTotal;
                _ShippingTotal += os.ShippingTotal;
                _TaxTotal += os.TaxTotal;
                _DiscountTotal += os.DiscountTotal;
                _CouponTotal += os.CouponTotal;
                _OtherTotal += os.OtherTotal;
                _ProfitTotal += os.ProfitTotal;
                _GrandTotal += os.GrandTotal;
            }
            _TotalsCalculated = true;
        }
        switch (itemType.ToLowerInvariant())
        {
            case "product": return string.Format("{0:lc}", _ProductTotal);
            case "shipping": return string.Format("{0:lc}", _ShippingTotal);
            case "tax": return string.Format("{0:lc}", _TaxTotal);
            case "discount": return string.Format("{0:lc}", _DiscountTotal);
            case "coupon": return string.Format("{0:lc}", _CouponTotal);
            case "other": return string.Format("{0:lc}", _OtherTotal);
            case "grand": return string.Format("{0:lc}", _GrandTotal);
            case "profit": return string.Format("{0:lc}", _ProfitTotal);                
        }
        return itemType;
    }

}
