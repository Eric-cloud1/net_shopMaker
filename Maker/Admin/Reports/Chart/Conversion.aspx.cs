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

public partial class Admin_Reports_Charts_Conversion : System.Web.UI.Page
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
        string cacheKey = "646A35E2-640C-424A-A520-5056C05C7742";
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

            decimal ClicksToOrderPercent =0;
            WebChart.LineChart chart = new LineChart();
            List<string> _lstIDs = new List<string>();
            DataSet conversiondataset = PageViewDataSource.GetConversion(this.forecastDate.SelectedDate, forecastDate.SelectedDate.AddDays(1), out alert);
            if (conversiondataset.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in conversiondataset.Tables[0].Rows)
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

                        foreach (DataRow rowids in conversiondataset.Tables[0].Select(_lstIDs[i]))
                        {
                            decimal.TryParse(rowids["ClicksToOrderPercent"].ToString(), out ClicksToOrderPercent);
                            chart.Data.Add(new ChartPoint(rowids["Hours"].ToString(), (float)Math.Round(ClicksToOrderPercent,2)));

                        }
                        i++;

                         if (alert == 1)
                         {
                            //bind chart
                            ConversionChart.Charts.Add(chart);
                            ConversionChart.ToolTip = "Conversion Click by Date";
                            ConversionChart.YValuesFormat = "{0:F0}";
                            ConversionChart.Visible = true;
                            AlertConversionChart.Visible = false;
                            ConversionChart.RedrawChart();
                          }
                         else
                         {
                              AlertConversionChart.Charts.Add(chart);
                               AlertConversionChart.ToolTip = "Conversion Click by Date";
                               AlertConversionChart.YValuesFormat = "{0:F0}";
                               AlertConversionChart.Visible = true;
                               ConversionChart.Visible = false;
                               AlertConversionChart.RedrawChart();
                         }

                    }

                }
                viewsData = new Dictionary<string, object>();


                if (viewsData.ContainsKey("ImageID"))
                {

                    viewsData.Add("ImageID", ConversionChart.ImageID);
                    viewsData.Add("DataSource", conversiondataset);
                }
                else
                {
                    _ForceRefresh = true;
                    ConversionChart.RedrawChart();
                }

                cacheWrapper = new CacheWrapper(viewsData);
                Cache.Remove(cacheKey);
                Cache.Add(cacheKey, cacheWrapper, null, DateTime.UtcNow.AddMinutes(5).AddSeconds(-1), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, null);

            }
            else 
            {
                this.ConversionChart.Charts.Clear();
                this.AlertConversionChart.Charts.Clear();
                WebChart.LineChart chartempty = new LineChart();
                chartempty.Line.Color = Color.Red;
                chartempty.Legend = "No Data Availabale";
                chartempty.Data.Add(new ChartPoint("12", 0));
                chartempty.DataLabels.Visible = true;

                noViewsMessage.Visible = true;
                this.ConversionChart.Charts.Add(chartempty);
           
            }
        }
        else
        {
            viewsData = (Dictionary<string, object>)cacheWrapper.CacheValue;

            if (viewsData.Count != 0)
            {
                ConversionChart.ImageID = (string)viewsData["ImagedID"];
                AlertConversionChart.ImageID = (string)viewsData["ImagedID"];

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


