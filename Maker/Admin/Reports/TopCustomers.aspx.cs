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

public partial class Admin_Reports_TopCustomers : MakerShop.Web.UI.MakerShopAdminPage
{


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            string tempDateFilter = Request.QueryString["DateFilter"];
            if (string.IsNullOrEmpty(tempDateFilter)) tempDateFilter = "TODAY";
            int dateFilter = AlwaysConvert.ToInt(tempDateFilter);
            ListItem item = DateFilter.Items.FindByValue(dateFilter.ToString());
            if (item != null) DateFilter.SelectedIndex = DateFilter.Items.IndexOf(item);
            //UPDATE DATE FILTER
            UpdateDateFilter();
        }
    }

    protected void UpdateDateFilter()
    {
        DateTime startDate, endDate;
        StoreDataHelper.SetDateFilter(DateFilter.SelectedValue, out startDate, out endDate);
        HiddenStartDate.Value = startDate.ToString();
        HiddenEndDate.Value = endDate.ToString();
        if (startDate > DateTime.MinValue)
        {
            ReportFromDate.Text = string.Format(ReportFromDate.Text, startDate);
            ReportFromDate.Visible = true;
        }
        if (endDate > DateTime.MinValue)
        {
            ReportToDate.Text = string.Format(ReportToDate.Text, endDate);
            ReportToDate.Visible = true;
        }
    }

    protected void DateFilter_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateDateFilter();
        TopCustomerGrid.DataBind();
    }

    protected void TopCustomerGrid_Sorting(object sender, GridViewSortEventArgs e)
    {
        if (e.SortExpression != TopCustomerGrid.SortExpression) e.SortDirection = SortDirection.Descending;
    }

}
