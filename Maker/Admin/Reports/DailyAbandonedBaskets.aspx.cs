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

public partial class Admin_Reports_DailyAbandonedBaskets : MakerShop.Web.UI.MakerShopAdminPage
{

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
        //UPDATE REPORT CAPTION
        ReportCaption.Visible = true;
        ReportCaption.Text = string.Format(ReportCaption.Text, reportDate);
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

    protected int GetUserId(int basketId)
    {
        Basket basket = BasketDataSource.Load(basketId);
        if (basket != null)
        {
            return basket.UserId;
        }
        return basketId;
    }


    
}
