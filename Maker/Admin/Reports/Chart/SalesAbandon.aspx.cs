using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Catalog;
using MakerShop.Common;
using MakerShop.Reporting;
using MakerShop.Utility;
using WebChart;
using System.Drawing;

public partial class Admin_Reports_SalesAbandon : System.Web.UI.Page
{

    private bool _ForceRefresh = false;
    private int _Size = 5;

    [Personalizable(), WebBrowsable()]
    public int Size
    {
        get { return _Size; }
        set { _Size = value; }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (Token.Instance.User.IsInRole(MakerShop.Users.Role.ReportAdminRoles))
        {           
            if (_Size < 1) _Size = 5;
            initViewChart(_ForceRefresh);
        }
        else this.Controls.Clear();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SalesDate.SelectedDate = DateTime.Now.Date;
        }
    }

    private void initViewChart(bool forceRefresh)
    {
        string cacheKey = "9172534A-5F1A-4871-83FF-1921A106A4EC";
        CacheWrapper cacheWrapper = Cache[cacheKey] as CacheWrapper;
        Dictionary<string, object> viewsData;
        if (forceRefresh || (cacheWrapper == null))
        {

            //This should be Reports=>Charts=>Current Abandons
            //DEFAULT today. But you can select any DAY in the past.
            //Y axis would be COUNT
            //X axis would be HOUR for current day.
            //1 line
            //You should have a successful order record with no payment record. 
            //Take a look at orders with no payment records and look at the statuses. 
            //The notes would contain any errors but the status should be enough to look at.

            int alert = 0;
            DataSet salesAbandon = PageViewDataSource.GetAbandonSales(this.SalesDate.SelectedDate,SalesDate.SelectedDate.AddDays(1), out alert);

            WebChart.LineChart chart1 = new LineChart();
            chart1.Line.Color = Color.Red;
            chart1.Legend = "Order Abandoned";
            chart1.DataLabels.Visible = true;


            if (salesAbandon.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow row in salesAbandon.Tables[0].Rows)
                {
                    chart1.Data.Add(new ChartPoint(row["Hours"].ToString(), (int)row["Counts"]));
                }



                if (alert == 1)
                {
                    //BIND THE CHART
                    AbandonSalesChart.Charts.Add(chart1);
                    AbandonSalesChart.ToolTip = "Current Sales by Date";
                    AbandonSalesChart.YValuesFormat = "{0:F0}";
                    AbandonSalesChart.Visible = true;
                    AlertAbandonSalesChart.Visible = false;
                    AbandonSalesChart.RedrawChart();

                }
                else
                {
                    //BIND THE CHART
                    this.AlertAbandonSalesChart.Charts.Add(chart1);
                    AlertAbandonSalesChart.ToolTip = "Current Sales by Date";
                    AlertAbandonSalesChart.YValuesFormat = "{0:F0}";
                    AlertAbandonSalesChart.Visible = true;
                    AbandonSalesChart.Visible = false;
                    AlertAbandonSalesChart.RedrawChart();

                }


                //CACHE THE DATA
                viewsData = new Dictionary<string, object>();
                viewsData["ImageID"] = AbandonSalesChart.ImageID;
                viewsData["DataSource"] = salesAbandon;

                cacheWrapper = new CacheWrapper(viewsData);
                Cache.Remove(cacheKey);
                Cache.Add(cacheKey, cacheWrapper, null, DateTime.UtcNow.AddMinutes(5).AddSeconds(-1), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, null);
            }
            else
           {
                //NO CATEGORIES HAVE BEEN VIEWED YET OR PAGE TRACKING IS NOT AVAILABLE
               this.AbandonSalesChart.Charts.Clear();
               this.AlertAbandonSalesChart.Charts.Clear();

               WebChart.LineChart chartEmpty = new LineChart();
                chartEmpty.Line.Color = Color.Red;
                chartEmpty.Legend = "no Data Available";
                chartEmpty.Data.Add(new ChartPoint("12", 0));
                chartEmpty.DataLabels.Visible = true;


                noViewsMessage.Visible = true;
                this.AbandonSalesChart.Charts.Add(chartEmpty);
            }
        }
        else
        {
            //USE CACHED VALUES
            viewsData = (Dictionary<string, object>)cacheWrapper.CacheValue;
            if (viewsData.Count != 0)
            {
                AbandonSalesChart.ImageID = (string)viewsData["ImageID"];
                AlertAbandonSalesChart.ImageID = (string)viewsData["ImageID"];
            }
         
        }
        DateTime cacheDate = (cacheWrapper != null) ? cacheWrapper.CacheDate : LocaleHelper.LocalNow;
        CacheDate1.Text = string.Format(CacheDate1.Text, cacheDate);

    }

    protected void RefreshLink_Click(object sender, EventArgs e)
    {
        _ForceRefresh = true;
    }
  
    protected void ProcessButton_Click(object sender, EventArgs e)
    {
        _ForceRefresh = true;
    }
}
