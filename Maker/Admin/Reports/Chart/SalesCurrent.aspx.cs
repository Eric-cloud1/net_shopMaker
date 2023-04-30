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

public partial class Admin_Reports_SalesCurrent : System.Web.UI.Page
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
        string cacheKey = "540C7B90-8F5F-4D98-BD1B-F477C357EA00";
        CacheWrapper cacheWrapper = Cache[cacheKey] as CacheWrapper;
        Dictionary<string, object> viewsData;
        if (forceRefresh || (cacheWrapper == null))
        {
            int alert = 0;
            DataSet salesCurrent = PageViewDataSource.GetCurrentSales(this.SalesDate.SelectedDate, SalesDate.SelectedDate.AddDays(1), out alert);

            WebChart.LineChart chart1 = new LineChart();
            chart1.Line.Color = Color.Red;
            chart1.Legend = "1 Signup";
            chart1.DataLabels.Visible = true;

            WebChart.LineChart chart2 = new LineChart();
            chart2.Line.Color = Color.Blue;
            chart2.Legend = "3 Trial";
            chart2.DataLabels.Visible = true;

            WebChart.LineChart chart3 = new LineChart();
            chart3.Line.Color = Color.Green;
            chart3.Legend = "4 Rebill";
            chart3.DataLabels.Visible = true;

            if (salesCurrent.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow row in salesCurrent.Tables[0].Rows)
                {
                    chart1.Data.Add(new ChartPoint(row["Hours"].ToString(), (int)row["Counts1"])); 
                    chart2.Data.Add(new ChartPoint(row["Hours"].ToString(), (int)row["Counts2"]));
                    chart3.Data.Add(new ChartPoint(row["Hours"].ToString(), (int)row["Counts3"]));
                }

           


                if (alert == 1)
                {
                    //BIND THE CHART
                    this.CurrentSalesChart.Charts.Add(chart1);
                    this.CurrentSalesChart.Charts.Add(chart2);
                    this.CurrentSalesChart.Charts.Add(chart3);
                    CurrentSalesChart.ToolTip = "Current Sales by Date";
                    CurrentSalesChart.YValuesFormat = "{0:F0}";
                    CurrentSalesChart.Visible = true;
                    AlertCurrentSalesChart.Visible = false;
                    CurrentSalesChart.RedrawChart();
                }
                else
                {
                    //BIND THE CHART
                    AlertCurrentSalesChart.Charts.Add(chart1);
                    AlertCurrentSalesChart.Charts.Add(chart2);
                    AlertCurrentSalesChart.Charts.Add(chart3);
                    AlertCurrentSalesChart.ToolTip = "Current Sales by Date";
                    AlertCurrentSalesChart.YValuesFormat = "{0:F0}";
                    AlertCurrentSalesChart.Visible = true;
                    CurrentSalesChart.Visible = false;
                    AlertCurrentSalesChart.RedrawChart();
                }
           
                //CACHE THE DATA
                viewsData = new Dictionary<string, object>();
                viewsData["ImageID"] = CurrentSalesChart.ImageID;
                viewsData["DataSource"] = salesCurrent;

                cacheWrapper = new CacheWrapper(viewsData);
                Cache.Remove(cacheKey);
                Cache.Add(cacheKey, cacheWrapper, null, DateTime.UtcNow.AddMinutes(5).AddSeconds(-1), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, null);
            }
            else
            {
                //NO CATEGORIES HAVE BEEN VIEWED YET OR PAGE TRACKING IS NOT AVAILABLE
                this.CurrentSalesChart.Charts.Clear();
                this.AlertCurrentSalesChart.Charts.Clear();
                WebChart.LineChart chartEmpty = new LineChart();
                chartEmpty.Line.Color = Color.Red;
                chartEmpty.Legend = "no Data Available";
                chartEmpty.Data.Add(new ChartPoint("12", 0));
                chartEmpty.DataLabels.Visible = true;


                noViewsMessage.Visible = true;
                this.CurrentSalesChart.Charts.Add(chartEmpty);
            }
        }
        else
        {
            //USE CACHED VALUES
            viewsData = (Dictionary<string, object>)cacheWrapper.CacheValue;
            if (viewsData.Count != 0)
            {
                CurrentSalesChart.ImageID = (string)viewsData["ImageID"];
                AlertCurrentSalesChart.ImageID = (string)viewsData["ImageID"];
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
