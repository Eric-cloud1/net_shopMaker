using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.ComponentModel;

public partial class Admin_UserControls_DatesAndTime : System.Web.UI.UserControl
{
    public enum Time_Period
    {
        Today = 0,
        Last_Hour = 13,
        Last_60_Minutes = 14,
        Yesterday = 1,
        This_Week = 2,
        Last_Week = 3,
        This_Month = 4,
        Last_Month = 5,
        Last_15_Days = 6,
        Last_30_Days = 7,
        Last_60_Days = 8,
        Last_90_Days = 9,
        Last_120_Days = 10,
        This_Year = 11,
        Date_Range = 12
    }

    private int delta = 0;
    private int startHour = 0;
    private int endHour = 0;
    private int startMinute = 0;
    private int endMinute = 0;


    public bool FirstHit
    {

        get
        {
            if (Session[this.ClientID + "_FirstHit"] == null)
                return true;

            return (bool)Session[this.ClientID + "_FirstHit"];
        }
        set
        {
            Session[this.ClientID + "_FirstHit"] = value;
        }
    }

    public DateTime StartDate
    {
        get
        {

            if (Session[this.ClientID + "_StartDate"] != null)
                return (DateTime)Session[this.ClientID + "_StartDate"];

            return DateTime.Now.Date.AddDays((DateTime.Now.Day - 1) * -1);
        }

        set
        {
            Session[this.ClientID + "_StartDate"] = value;
        }
    }

    public DateTime EndDate
    {
        get
        {

            if (Session[this.ClientID + "_EndDate"] != null)
                return (DateTime)Session[this.ClientID + "_EndDate"];

            return DateTime.Today;
        }

        set
        {
            Session[this.ClientID + "_EndDate"] = value;
        }
    }

    public Time_Period TimePeriod
    {
        get
        {
            if (Session[this.ClientID + "_TimePeriod"] != null)
                return (Time_Period)Session[this.ClientID + "_TimePeriod"];

            return Time_Period.Date_Range;
        }

        set
        {
            Session[this.ClientID + "_TimePeriod"] = value;
            FirstHit = false;
            setDateTime();
        }
    }
    /*
    [System.Security.Permissions.PermissionSet
    (System.Security.Permissions.SecurityAction.Demand,
     Name = "FullTrust")]
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

    }


    */
    protected void Page_Load(object sender, EventArgs e)
    {
        pDate.Visible = ShowDate;

        pHours1.Visible = ShowHours;
        pHours2.Visible = ShowHours;
        ProcessButton.Visible = ShowSubmit;

        if (!IsPostBack)
        {
            SelectStartDate.SelectedDate = StartDate;
            SelectEndDate.SelectedDate = EndDate;
            SelectDate.SelectedValue = ((int)TimePeriod).ToString();




            if (SelectDate.SelectedValue == ((int)Time_Period.Date_Range).ToString())
            {
                startHour = StartDate.Hour;  // int.Parse(selectStartHour.SelectedValue);
                endHour = EndDate.Hour;  //int.Parse(selectEndHour.SelectedValue);

                startMinute = StartDate.Minute;  // int.Parse(selectStartMinute.SelectedValue);
                endMinute = EndDate.Minute;  //int.Parse(selectEndMinute.SelectedValue);

                /*
                StartDate = new DateTime(SelectStartDate.SelectedDate.Year,
           SelectStartDate.SelectedDate.Month,
           SelectStartDate.SelectedDate.Day,
           startHour,
           startMinute,
           0,
           0,
           SelectStartDate.SelectedDate.Kind);


                EndDate = new DateTime(SelectEndDate.SelectedDate.Year,
            SelectEndDate.SelectedDate.Month,
            SelectEndDate.SelectedDate.Day,
            endHour,
            endMinute,
            0,
            0,
            SelectEndDate.SelectedDate.Kind);
                */
                pDate.Visible = true;
                //   ProcessButton.Visible = ShowSubmitRange;

                selectStartHour.SelectedValue = StartDate.Hour.ToString();
                selectStartMinute.SelectedValue = StartDate.Minute.ToString();
                selectEndHour.SelectedValue = EndDate.Hour.ToString();
                selectEndMinute.SelectedValue = EndDate.Minute.ToString();

            }

        }

    }


    public void setDateTime()
    {
        if (FirstHit)
            EndDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.AddDays(1).Day, 0, 0, 0);

        SelectDate.SelectedValue = ((int)TimePeriod).ToString();
        switch (TimePeriod)
        {
            case Time_Period.Today:
                StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0);
                EndDate = new DateTime(DateTime.Today.AddDays(1).Year, DateTime.Today.AddDays(1).Month, DateTime.Today.AddDays(1).Day, 0, 0, 0);
                selectStartHour.SelectedIndex = 0;
                selectEndHour.SelectedIndex = 0;
                selectStartMinute.SelectedIndex = 0;
                selectEndMinute.SelectedIndex = 0;
                pDate.Visible = false;
                break;  // today
            case Time_Period.Yesterday:
                StartDate = new DateTime(DateTime.Today.AddDays(-1).Year, DateTime.Today.AddDays(-1).Month, DateTime.Today.AddDays(-1).Day, 0, 0, 0);
                EndDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0);
                selectStartHour.SelectedIndex = 0;
                selectEndHour.SelectedIndex = 0;
                selectStartMinute.SelectedIndex = 0;
                selectEndMinute.SelectedIndex = 0;
                pDate.Visible = false;
                break; // Yesterday
            case Time_Period.This_Week:
                delta = (int)DateTime.Today.DayOfWeek - 1;
                StartDate = new DateTime(DateTime.Today.AddDays(-delta).Year, DateTime.Today.AddDays(-delta).Month, DateTime.Today.AddDays(-delta).Day, 0, 0, 0);
                EndDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.AddDays(1).Day, 0, 0, 0);
                selectStartHour.SelectedIndex = 0;
                selectEndHour.SelectedIndex = 0;
                selectStartMinute.SelectedIndex = 0;
                selectEndMinute.SelectedIndex = 0;
                pDate.Visible = false;
                break; // This week

            case Time_Period.Last_Week:
                delta = ((int)DateTime.Today.DayOfWeek) + 7 - 1;
                StartDate = new DateTime(DateTime.Today.AddDays(-delta).Year, DateTime.Today.AddDays(-delta).Month, DateTime.Today.AddDays(-delta).Day, 0, 0, 0);
                delta = (int)DateTime.Today.DayOfWeek - 1;
                EndDate = new DateTime(DateTime.Today.AddDays(-delta).Year, DateTime.Today.AddDays(-delta).Month, DateTime.Today.AddDays(-delta).Day, 0, 0, 0);
                selectStartHour.SelectedIndex = 0;
                selectEndHour.SelectedIndex = 0;
                selectStartMinute.SelectedIndex = 0;
                selectEndMinute.SelectedIndex = 0;
                pDate.Visible = false;
                break; // Last week
            case Time_Period.This_Month:
                delta = DateTime.Today.Day - 1;
                StartDate = new DateTime(DateTime.Today.AddDays(-delta).Year, DateTime.Today.AddDays(-delta).Month, DateTime.Today.AddDays(-delta).Day, 0, 0, 0);
                EndDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.AddDays(1).Day, 0, 0, 0);
                selectStartHour.SelectedIndex = 0;
                selectEndHour.SelectedIndex = 0;
                selectStartMinute.SelectedIndex = 0;
                selectEndMinute.SelectedIndex = 0;
                pDate.Visible = false;
                break; // This Month
            case Time_Period.Last_Month:
                int daysInMonths = System.DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.AddMonths(-1).Month);
                delta = DateTime.Today.Day + daysInMonths - 1;
                StartDate = new DateTime(DateTime.Today.AddDays(-delta).Year, DateTime.Today.AddDays(-delta).Month, DateTime.Today.AddDays(-delta).Day, 0, 0, 0);
                delta = DateTime.Today.Day - 1;
                EndDate = new DateTime(DateTime.Today.AddDays(-delta).Year, DateTime.Today.AddDays(-delta).Month, DateTime.Today.AddDays(-delta).Day, 0, 0, 0);
                selectStartHour.SelectedIndex = 0;
                selectEndHour.SelectedIndex = 0;
                selectStartMinute.SelectedIndex = 0;
                selectEndMinute.SelectedIndex = 0;
                pDate.Visible = false;
                break; // Last Month
            case Time_Period.Last_15_Days:
                StartDate = new DateTime(DateTime.Today.AddDays(-15).Year, DateTime.Today.AddDays(-15).Month, DateTime.Today.AddDays(-15).Day, 0, 0, 0);
                EndDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.AddDays(1).Day, 0, 0, 0);
                selectStartHour.SelectedIndex = 0;
                selectEndHour.SelectedIndex = 0;
                selectStartMinute.SelectedIndex = 0;
                selectEndMinute.SelectedIndex = 0;
                pDate.Visible = false;
                break; // Last 15 Days
            case Time_Period.Last_30_Days:
                StartDate = new DateTime(DateTime.Today.AddDays(-30).Year, DateTime.Today.AddDays(-30).Month, DateTime.Today.AddDays(-30).Day, 0, 0, 0);
                EndDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.AddDays(1).Day, 0, 0, 0);
                selectStartHour.SelectedIndex = 0;
                selectEndHour.SelectedIndex = 0;
                selectStartMinute.SelectedIndex = 0;
                selectEndMinute.SelectedIndex = 0;
                pDate.Visible = false;
                break; // Last 30 Days
            case Time_Period.Last_60_Days:
                StartDate = new DateTime(DateTime.Today.AddDays(-60).Year, DateTime.Today.AddDays(-60).Month, DateTime.Today.AddDays(-60).Day, 0, 0, 0);
                EndDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.AddDays(1).Day, 0, 0, 0);
                selectStartHour.SelectedIndex = 0;
                selectEndHour.SelectedIndex = 0;
                selectStartMinute.SelectedIndex = 0;
                selectEndMinute.SelectedIndex = 0;
                pDate.Visible = false;
                break; // Last 60 Days
            case Time_Period.Last_90_Days:
                StartDate = new DateTime(DateTime.Today.AddDays(-90).Year, DateTime.Today.AddDays(-90).Month, DateTime.Today.AddDays(-90).Day, 0, 0, 0);
                EndDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.AddDays(1).Day, 0, 0, 0);
                selectStartHour.SelectedIndex = 0;
                selectEndHour.SelectedIndex = 0;
                selectStartMinute.SelectedIndex = 0;
                selectEndMinute.SelectedIndex = 0;
                pDate.Visible = false;
                break; // Last 90 Days
            case Time_Period.Last_120_Days:
                StartDate = new DateTime(DateTime.Today.AddDays(-120).Year, DateTime.Today.AddDays(-120).Month, DateTime.Today.AddDays(-120).Day, 0, 0, 0);
                EndDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.AddDays(1).Day, 0, 0, 0);
                selectStartHour.SelectedIndex = 0;
                selectEndHour.SelectedIndex = 0;
                selectStartMinute.SelectedIndex = 0;
                selectEndMinute.SelectedIndex = 0;
                pDate.Visible = false;
                break; // Last 120 Days
            case Time_Period.This_Year:
                StartDate = new DateTime(DateTime.Today.Year, 1, 1, 0, 0, 0);
                EndDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.AddDays(1).Day, 0, 0, 0);
                selectStartHour.SelectedIndex = 0;
                selectEndHour.SelectedIndex = 0;
                selectStartMinute.SelectedIndex = 0;
                selectEndMinute.SelectedIndex = 0;
                pDate.Visible = false;
                break; // This Year
            case Time_Period.Last_Hour:
                StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.AddHours(-1).Hour, 0, 0, 0, SelectStartDate.SelectedDate.Kind);
                EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0, 0, SelectStartDate.SelectedDate.Kind);
                selectStartHour.SelectedValue = StartDate.Hour.ToString();
                selectEndHour.SelectedValue = EndDate.Hour.ToString();
                selectStartMinute.SelectedValue = StartDate.Minute.ToString();
                selectEndMinute.SelectedValue = EndDate.Minute.ToString();
                pDate.Visible = false;
                break; // Last 60 min
            case Time_Period.Last_60_Minutes:
                StartDate = RoundMinute(DateTime.Now.AddMinutes(-60), true);
             
                EndDate = RoundMinute(DateTime.Now, false);
              
                selectStartHour.SelectedValue = StartDate.Hour.ToString();
                selectEndHour.SelectedValue = EndDate.Hour.ToString();
                selectStartMinute.SelectedValue = StartDate.Minute.ToString();
                selectEndMinute.SelectedValue = EndDate.Minute.ToString();
                pDate.Visible = false;
                break; // Last Hour
            default: //date range
                pDate.Visible = true;

                StartDate = new DateTime(SelectStartDate.SelectedDate.Year,
                SelectStartDate.SelectedDate.Month,
                SelectStartDate.SelectedDate.Day,
                int.Parse(selectStartHour.SelectedValue),
                int.Parse(selectStartMinute.SelectedValue),
                0,
                0,
                SelectStartDate.SelectedDate.Kind);

                EndDate = new DateTime(SelectEndDate.SelectedDate.Year,
                               SelectEndDate.SelectedDate.Month,
                               SelectEndDate.SelectedDate.Day,
                               int.Parse(selectEndHour.SelectedValue),
                               int.Parse(selectEndMinute.SelectedValue),
                               0,
                               0,
                               SelectEndDate.SelectedDate.Kind);

                /*
                delta = (int)DateTime.Today.DayOfWeek - 1;
                if (FirstHit)                    StartDate = System.DateTime.Today.AddDays(-delta);

                startHour = int.Parse(selectStartHour.SelectedValue);
                endHour = int.Parse(selectEndHour.SelectedValue);
                startMinute = int.Parse(selectStartMinute.SelectedValue);
                endMinute = int.Parse(selectEndMinute.SelectedValue);

                //if (FirstHit)
                {
                    StartDate = new DateTime(System.DateTime.Today.AddDays(-delta).Year,
                System.DateTime.Today.AddDays(-delta).Month,
                System.DateTime.Today.AddDays(-delta).Day,
                startHour,
                startMinute,
                0,
                0,
                SelectStartDate.SelectedDate.Kind);


                    EndDate = new DateTime(DateTime.Today.AddDays(1).Year,
                DateTime.Today.AddDays(1).Month,
                DateTime.Today.AddDays(1).Day,
                endHour,
                endMinute,
                0,
                0,
                SelectEndDate.SelectedDate.Kind);
                }
                selectStartHour.SelectedValue = StartDate.Hour.ToString();
                selectStartMinute.SelectedValue = StartDate.Minute.ToString();
                selectEndHour.SelectedValue = EndDate.Hour.ToString();
                selectEndMinute.SelectedValue = EndDate.Minute.ToString();
                */
                break; 
        }


        SelectStartDate.SelectedDate = StartDate;
        SelectEndDate.SelectedDate = EndDate;


    }

    private DateTime RoundMinute(DateTime dt, bool down)
    {
        decimal mins = dt.Minute / 5;
        int min = (int)Math.Truncate(mins) * 5;

        if ((min == 60)||(min+5==60))
        {
            dt.AddHours(1);
            min = 0;

        }

        if (down)
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, min, dt.Second, 0, SelectStartDate.SelectedDate.Kind);
           

        else
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, min + 5, dt.Second, 0, SelectStartDate.SelectedDate.Kind);
         

    }


    protected void setStartDate(object sender, EventArgs e)
    {



    }

    private bool _showHours = true;
    /// <summary>
    /// Show the hours portion of the control
    /// </summary>
    [Browsable(true), DefaultValue(true)]
    public bool ShowHours
    {
        get { return _showHours; }
        set { _showHours = value; }
    }

    private bool _showDate = false;
    /// <summary>
    /// Show the date & hour portion of the control
    /// </summary>
    [Browsable(true), DefaultValue(false)]
    public bool ShowDate
    {
        get { return _showDate; }
        set { _showDate = value; }
    }

    private bool _showSubmit = false;

    /// <summary>
    /// Show the submit button always
    /// </summary>
    [Browsable(true), DefaultValue(false)]
    public bool ShowSubmit
    {
        get { return _showSubmit; }
        set { _showSubmit = value; }
    }

    private bool _showSubmitRange = false;
    /// <summary>
    /// Show the submit button only for date range
    /// </summary>
    [Browsable(true), DefaultValue(false)]
    public bool ShowSubmitRange
    {
        get { return _showSubmitRange; }
        set { _showSubmitRange = value; }
    }

    /// <summary>
    /// Notifies that the date has been changed
    /// </summary>
    public delegate void UpdateEventHandler();
    public event UpdateEventHandler Update;
    protected void SelectDate_SelectedIndexChanged(object sender, EventArgs e)
    {

        TimePeriod = (Time_Period)Enum.Parse(typeof(Time_Period), SelectDate.SelectedValue);

        if ((SelectDate.SelectedValue != ((int)Time_Period.Date_Range).ToString()) || (sender is Button))
            if (Update != null)
                Update();

    }

    protected void ProcessButton_Click(object sender, EventArgs e)
    {
        SelectDate_SelectedIndexChanged(sender, e);
    }
}
