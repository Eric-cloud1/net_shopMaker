using System;
using System.Text;
using System.Web.UI;
using ComponentArt.Web.UI;
using MakerShop.Utility;

public partial class Admin_UserControls_PickerAndCalendar : System.Web.UI.UserControl
{
    public DateTime SelectedDate
    {
        get { return Picker1.SelectedDate; }
        set { Picker1.SelectedDate = value; }
    }

    /// <summary>
    /// Returns the selected date with the time set to the end of the day.
    /// </summary>
    public DateTime SelectedEndDate
    {
        get
        {
            DateTime retval = Picker1.SelectedDate;
            if (retval == DateTime.MinValue) return retval;
            return new DateTime(retval.Year, retval.Month, retval.Day, 23, 59, 59, DateTimeKind.Local);
        }
    }

    /// <summary>
    /// Returns the selected date with the time set to the beginning of the day.
    /// </summary>
    public DateTime SelectedStartDate
    {
        get
        {
            DateTime retval = Picker1.SelectedDate;
            if (retval == DateTime.MinValue) return retval;
            return new DateTime(retval.Year, retval.Month, retval.Day, 0, 0, 0, DateTimeKind.Local);
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        //CalendarButton.Attributes.Add("onclick", );
        StringBuilder onclick = new StringBuilder();
        onclick.Append("if(" + Picker1.ClientObjectId + ".GetSelectedDate()==null)" + Calendar1.ClientObjectId + ".ClearSelectedDate();");
        onclick.Append("else " + Calendar1.ClientObjectId + ".SetSelectedDate(" + Picker1.ClientObjectId + ".GetSelectedDate());");
        onclick.Append(Calendar1.ClientObjectId + ".Show(this);");
        CalendarButton.Attributes.Add("onclick", onclick.ToString());
        Calendar1.TodaysDate = LocaleHelper.LocalNow;
        Calendar1.ClientEvents.SelectionChanged = new ClientEvent(Calendar1.ClientObjectId + "_SelectionChanged");
        Picker1.ClientEvents.SelectionChanged = new ClientEvent(Picker1.ClientObjectId + "_SelectionChanged");
        GenerateScript();
    }

    private void GenerateScript()
    {
        StringBuilder script = new StringBuilder();
        script.Append("function " + Calendar1.ClientObjectId + "_SelectionChanged(sender, eventArgs) { " + Picker1.ClientObjectId + ".setSelectedDate(sender.getSelectedDate()); }\r\n");
        script.Append("function " + Picker1.ClientObjectId + "_SelectionChanged(sender, eventArgs) { " + Calendar1.ClientObjectId + ".setSelectedDate(sender.getSelectedDate()); }\r\n");
        ScriptManager.RegisterClientScriptBlock(Calendar1, this.GetType(), this.ClientID + "_CA", script.ToString(), true);
        // SETTING ASSOCIATED PICKER / ASSOCIATED CALENDAR DOES NOT SEEM COMPATIBLE WITH AJAX UPDATEPANEL
        // THE ASSOCIATED PICKER ALWAYS RETURNED NULL ON POSTBACK
        // THIS JAVASCRIPT HACK IS USED INSTEAD TO ALLOW THE CLEAR DATE TO WORK IN FOOTER CLIENT TEMPLATE
        ScriptManager.RegisterStartupScript(Calendar1, this.GetType(), this.ClientID + "_CA_Init", "var " + Calendar1.ClientObjectId + "_Picker='" + Picker1.ClientObjectId + "';", true);
    }

    /// <summary>
    /// This can be used by calling pages to obtain a JS reference to the object.
    /// </summary>
    /// <returns></returns>
    public string GetPickerClientId()
    {
        return Picker1.ClientObjectId;
    }

    /// <summary>
    /// This can be used by calling pages to obtain a JS reference to the object.
    /// </summary>
    /// <returns></returns>
    public string GetCalendarClientId()
    {
        return Calendar1.ClientObjectId;
    }

}