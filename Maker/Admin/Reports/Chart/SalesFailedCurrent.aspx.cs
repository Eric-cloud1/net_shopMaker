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

public partial class Admin_Reports_SalesFailedCurrent : System.Web.UI.Page
{

    private bool _ForceRefresh = false;
    private int _Size = 5;
    private int alert = 0;
    private DateTime cacheDate;


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
            this.SalesDate.SelectedDate = DateTime.Now.Date;

    }



    private void initViewChart(bool forceRefresh)
    {
        string cacheKey = "F9DA5C03-4415-4096-A5F4-2A6761CC4A3B";
        CacheWrapper cacheWrapper = Cache[cacheKey] as CacheWrapper;
        Dictionary<string, object> viewsData;
        if (forceRefresh || (cacheWrapper == null))
        {

            DataSet salesCurrent = PageViewDataSource.GetCurrentFailedSales(this.SalesDate.SelectedDate, SalesDate.SelectedDate.AddDays(1), out alert);
    
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
                    CurrentFailedSalesChart.Charts.Add(chart1);
                    CurrentFailedSalesChart.Charts.Add(chart2);
                    CurrentFailedSalesChart.Charts.Add(chart3);

                    CurrentFailedSalesChart.ToolTip = "Current Sales by Date";
                    CurrentFailedSalesChart.YValuesFormat = "{0:F0}";

                    CurrentFailedSalesChart.Visible = true;
                    AlertCurrentFailedSalesChart.Visible = false;

                    CurrentFailedSalesChart.RedrawChart();

                }
                else
                {
                    //BIND THE CHART
                    AlertCurrentFailedSalesChart.Charts.Add(chart1);
                    AlertCurrentFailedSalesChart.Charts.Add(chart2);
                    AlertCurrentFailedSalesChart.Charts.Add(chart3);

                    AlertCurrentFailedSalesChart.ToolTip = "Current Sales by Date";
                    AlertCurrentFailedSalesChart.YValuesFormat = "{0:F0}";

                    AlertCurrentFailedSalesChart.Visible = true;
                    CurrentFailedSalesChart.Visible = false;
                    AlertCurrentFailedSalesChart.RedrawChart();

                }
                //CACHE THE DATA
                viewsData = new Dictionary<string, object>();
                viewsData["ImageID"] = CurrentFailedSalesChart.ImageID;
                viewsData["DataSource"] = salesCurrent;

                cacheWrapper = new CacheWrapper(viewsData);
                Cache.Remove(cacheKey);
                Cache.Add(cacheKey, cacheWrapper, null, DateTime.UtcNow.AddMinutes(5).AddSeconds(-1), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, null);
            }
            else
            {
                //NO CATEGORIES HAVE BEEN VIEWED YET OR PAGE TRACKING IS NOT AVAILABLE
                this.CurrentFailedSalesChart.Charts.Clear();
                AlertCurrentFailedSalesChart.Charts.Clear();

                WebChart.LineChart chartEmpty = new LineChart();
                chartEmpty.Line.Color = Color.Red;
                chartEmpty.Legend = "no Data Available";
                chartEmpty.Data.Add(new ChartPoint("12", 0));
                chartEmpty.DataLabels.Visible = true;


                noViewsMessage.Visible = true;
                this.CurrentFailedSalesChart.Charts.Add(chartEmpty);
            }
        }
        else
        {
            //USE CACHED VALUES
            viewsData = (Dictionary<string, object>)cacheWrapper.CacheValue;
            if (viewsData.Count != 0)
            {
                CurrentFailedSalesChart.ImageID = (string)viewsData["ImageID"];
                AlertCurrentFailedSalesChart.ImageID = (string)viewsData["ImageID"];
            }
        }
        cacheDate = (cacheWrapper != null) ? cacheWrapper.CacheDate : LocaleHelper.LocalNow;
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
