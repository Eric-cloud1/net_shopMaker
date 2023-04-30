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

public partial class Admin_Reports_Charts_ConversionLead : System.Web.UI.Page
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
        string cacheKey = "0A04CFBD-DD05-48C5-84B9-C7FA5E12E224";
        CacheWrapper cacheWrapper = Cache[cacheKey] as CacheWrapper;
        Dictionary<string, object> viewsData;
        if (forceRefresh || (cacheWrapper == null))

        {
            int alert = 0;
            Random r;
            int red;
            int green;
            int blue;
            int i = 0;

            decimal ClicksToLeadPercent = 0;
            WebChart.LineChart chart = new LineChart();
            List<string> _lstIDs = new List<string>();
            DataSet conversionLeaddataset = PageViewDataSource.GetConversion(this.forecastDate.SelectedDate, forecastDate.SelectedDate.AddDays(1), out alert);
            if (conversionLeaddataset.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in conversionLeaddataset.Tables[0].Rows)
                {

                    string filter = string.Format(@"ids ='{0}'",row["ids"]);
                    if (!_lstIDs.Contains(filter))                      
                    {
                        int color;
                        if (!int.TryParse(row["ids"].ToString().Replace("-", ""), out color))
                            continue;
                        r = new Random(color);
                        red = r.Next(255);
                        green = r.Next(255);
                        blue = r.Next(255);


                        _lstIDs.Add(filter);
                        chart = new LineChart();
                        chart.Line.Color = Color.FromArgb(red, green, blue);
                        chart.Legend = row["ids"].ToString();
                        chart.DataLabels.Visible = true;

                        foreach (DataRow rowids in conversionLeaddataset.Tables[0].Select(_lstIDs[i]))
                        {

                            decimal.TryParse(rowids["ClicksToLeadPercent"].ToString(), out ClicksToLeadPercent);
                            chart.Data.Add(new ChartPoint(rowids["Hours"].ToString(), (float)Math.Round(ClicksToLeadPercent, 2)));

                        }
                        i++;

                         if (alert == 1)
                         {
                            //bind chart
                            ConversionLeadChart.Charts.Add(chart);
                            ConversionLeadChart.ToolTip = "Conversion Lead by Date";
                            ConversionLeadChart.YValuesFormat = "{0:F0}";
                            ConversionLeadChart.Visible = true;
                            AlertConversionLeadChart.Visible = false;
                            ConversionLeadChart.RedrawChart();
                          }
                         else
                         {
                             AlertConversionLeadChart.Charts.Add(chart);
                             AlertConversionLeadChart.ToolTip = "Conversion Lead by Date";
                             AlertConversionLeadChart.YValuesFormat = "{0:F0}";
                             AlertConversionLeadChart.Visible = true;
                             ConversionLeadChart.Visible = false;
                             AlertConversionLeadChart.RedrawChart();
                         }

                    }

                }
                viewsData = new Dictionary<string, object>();


                if (viewsData.ContainsKey("ImageID"))
                {

                    viewsData.Add("ImageID", ConversionLeadChart.ImageID);
                    viewsData.Add("DataSource", conversionLeaddataset);
                }
                else
                {
                    _ForceRefresh = true;
                    ConversionLeadChart.RedrawChart();
                }

                cacheWrapper = new CacheWrapper(viewsData);
                Cache.Remove(cacheKey);
                Cache.Add(cacheKey, cacheWrapper, null, DateTime.UtcNow.AddMinutes(5).AddSeconds(-1), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, null);

            }
            else 
            {
                this.ConversionLeadChart.Charts.Clear();
                this.AlertConversionLeadChart.Charts.Clear();
                WebChart.LineChart chartempty = new LineChart();
                chartempty.Line.Color = Color.Red;
                chartempty.Legend = "No Data Availabale";
                chartempty.Data.Add(new ChartPoint("12", 0));
                chartempty.DataLabels.Visible = true;

                noViewsMessage.Visible = true;
                this.ConversionLeadChart.Charts.Add(chartempty);
           
            }
        }
        else
        {
            viewsData = (Dictionary<string, object>)cacheWrapper.CacheValue;

            if (viewsData.Count != 0)
            {
              
                //viewsData.Add("ImageID", ForecastChart.ImageID);
                //viewsData.Add("ImageID", AlertForecastChart.ImageID);

                ConversionLeadChart.ImageID = (string)viewsData["ImagedID"];
                AlertConversionLeadChart.ImageID = (string)viewsData["ImagedID"];

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


