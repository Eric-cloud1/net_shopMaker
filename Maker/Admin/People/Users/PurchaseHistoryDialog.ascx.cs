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
using MakerShop.Reporting;
using MakerShop.Utility;
using System.Collections.Generic;
using MakerShop.Common;

public partial class Admin_People_Users_PurchaseHistoryDialog : System.Web.UI.UserControl
{
    private int _UserId;
    protected void Page_Load(object sender, EventArgs e)
    {
        _UserId = AlwaysConvert.ToInt(Request.QueryString["UserId"]);
        List<PurchaseSummary> paidItems = (List<PurchaseSummary>)PaidOrdersDs.Select();
        List<PurchaseSummary> unpaidItems = (List<PurchaseSummary>)UnpaidOrdersDs.Select();

        // CALCULATE FIRST ORDER DATE AND TOTAL PAID
        LSDecimal totalPaid = 0;
        DateTime firstOrderDate = DateTime.MinValue;
        List<int> paidOrderIds = new List<int>();
        List<int> unpaidOrderIds = new List<int>();
        foreach (PurchaseSummary summary in paidItems)
        {
            if (firstOrderDate == DateTime.MinValue) firstOrderDate = summary.OrderDate;
            else if(firstOrderDate.CompareTo(summary.OrderDate) < 0) firstOrderDate = summary.OrderDate;
            totalPaid += summary.Total;

            if (!paidOrderIds.Contains(summary.OrderId)) paidOrderIds.Add(summary.OrderId);
        }
        foreach (PurchaseSummary summary in unpaidItems)
        {
            if (firstOrderDate == DateTime.MinValue) firstOrderDate = summary.OrderDate;
            else if (firstOrderDate.CompareTo(summary.OrderDate) < 0) firstOrderDate = summary.OrderDate;

            if (!unpaidOrderIds.Contains(summary.OrderId)) unpaidOrderIds.Add(summary.OrderId);
        }
        if (firstOrderDate != DateTime.MinValue) FirstOrder.Text = String.Format("{0:d}", firstOrderDate);
        PurchasesToDate.Text = String.Format("{0:lc}",totalPaid);

        PaidOrders.Text = paidOrderIds.Count.ToString();
        PendingOrders.Text = unpaidOrderIds.Count.ToString();

        if (paidItems.Count == 0) PaidOrdersPanel.Visible = false;

        if (unpaidItems.Count == 0) UnpaidOrdersPanel.Visible = false;

        
    }

    protected void PaidOrderGrid_Sorting(object sender, GridViewSortEventArgs e)
    {
        if (e.SortExpression != PaidOrderGrid.SortExpression) e.SortDirection = SortDirection.Descending;
    }

    protected void UnPaidOrderGrid_Sorting(object sender, GridViewSortEventArgs e)
    {
        if (e.SortExpression != UnPaidOrderGrid.SortExpression) e.SortDirection = SortDirection.Descending;
    }
}
