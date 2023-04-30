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

public partial class Admin_Reports_Charts_ForecastChart : System.Web.UI.Page
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
            forecastDate.SelectedDate = DateTime.Now.Date;
            _ForceRefresh = true;
        }


    }
    private void initViewChart(bool forceRefresh)
    {
        string cacheKey = "5BD19491-4C6D-4869-9911-E1347831F861";
        CacheWrapper cacheWrapper = Cache[cacheKey] as CacheWrapper;
        Dictionary<string, object> viewsData;
        if (forceRefresh || (cacheWrapper == null))

        {
            //DataSet forecastchart = ForecastChartDataSource.getforecast(this.forecastDate .SelectedDate .ToString ("mm-dd-yyyy"));
            int alert = 0;

            DataSet forecastdataset = PageViewDataSource.GetForecast(this.forecastDate.SelectedDate, forecastDate.SelectedDate.AddDays(1), out alert);



            WebChart.LineChart chart1 = new LineChart();
            chart1.Line.Color = Color.Green;
            chart1.Legend = "Trial";
            chart1.DataLabels.Visible = true;

            WebChart.LineChart chart2 = new LineChart();
            chart2.Line.Color = Color.Blue;
            chart2.Legend = "Rebill";
            chart2.DataLabels.Visible = true;

            if (forecastdataset.Tables[0].Rows .Count  > 0)
            {
                foreach (DataRow row in forecastdataset.Tables[0].Rows)
                {
                    chart1.Data.Add(new ChartPoint(row["Hours"].ToString(), (int)row["Trials"]));
                    chart2.Data.Add(new ChartPoint(row["Hours"].ToString(), (int)row["Rebills"]));

                }

                if (alert == 1)
                {
                    //bind chart
                    ForecastChart.Charts.Add(chart1);
                    ForecastChart.Charts.Add(chart2);
                    ForecastChart.ToolTip = "Forecast by Date";
                    ForecastChart.YValuesFormat = "{0:F0}";
                    ForecastChart.Visible = true;
                    AlertForecastChart.Visible = false;
                    ForecastChart.RedrawChart();
                }

                else
                {
                    AlertForecastChart.Charts.Add(chart1);
                    AlertForecastChart.Charts.Add(chart2);
                    AlertForecastChart.ToolTip = "Forecast by Date";
                    AlertForecastChart.YValuesFormat = "{0:F0}";
                    AlertForecastChart.Visible = true;
                    ForecastChart.Visible = false;
                    AlertForecastChart.RedrawChart();
                }

                viewsData = new Dictionary<string, object>();
                //viewsData.Add("ImageID", ForecastChart.ImageID);
                //viewsData.Add("DataSource", forecastdataset);

                if (viewsData.ContainsKey("ImageID"))
                {
                    //viewsData["ImageID"] = ForecastChart.ImageID;
                    //viewsData["DataSource"] = forecastdataset;
                    viewsData.Add("ImageID", ForecastChart.ImageID);
                    viewsData.Add("DataSource", forecastdataset);
                }
                else
                {
                    _ForceRefresh = true;
                    ForecastChart.RedrawChart();
                }

            
                

                cacheWrapper = new CacheWrapper(viewsData);
                Cache.Remove(cacheKey);
                Cache.Add(cacheKey, cacheWrapper, null, DateTime.UtcNow.AddMinutes(5).AddSeconds(-1), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, null);

            }
            else 
            {
                this.ForecastChart.Charts.Clear();
                this.AlertForecastChart.Charts.Clear();
                WebChart.LineChart chartempty = new LineChart();
                chartempty.Line.Color = Color.Red;
                chartempty.Legend = "No Data Availabale";
                chartempty.Data.Add(new ChartPoint("12", 0));
                chartempty.DataLabels.Visible = true;

                noViewsMessage.Visible = true;
                this.ForecastChart.Charts.Add(chartempty);


           
            }
        }
        else
        {
            viewsData = (Dictionary<string, object>)cacheWrapper.CacheValue;

            if (viewsData.Count != 0)
            {
              
                //viewsData.Add("ImageID", ForecastChart.ImageID);
                //viewsData.Add("ImageID", AlertForecastChart.ImageID);

                ForecastChart.ImageID = (string)viewsData["ImagedID"];
                AlertForecastChart.ImageID = (string)viewsData["ImagedID"];
                    

             

            
            }
     

        }
        DateTime cachedate = (cacheWrapper != null) ? cacheWrapper.CacheDate : LocaleHelper.LocalNow;
        CacheDate1.Text = string.Format(CacheDate1.Text, cachedate);
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


