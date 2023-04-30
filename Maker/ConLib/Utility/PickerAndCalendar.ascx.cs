using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ComponentArt.Web.UI;
using MakerShop.Utility;

public partial class ConLib_Utility_PickerAndCalendar : System.Web.UI.UserControl
{
    public DateTime SelectedDate
    {
        get
        {
            if (!Picker1Enabler.Checked) return DateTime.MinValue;
            return Picker1.SelectedDate;
        }
        set
        {
            if (value == DateTime.MinValue)
            {
                Picker1.SelectedDate = LocaleHelper.LocalNow;
                Picker1Enabler.Checked = false;
                Picker1.Enabled = false;
            }
            else
            {
                Picker1.SelectedDate = value;
                Picker1Enabler.Checked = true;
                Picker1.Enabled = true;
            }
        }
    }

    /// <summary>
    /// Returns the selected date with the time set to the end of the day.
    /// </summary>
    public DateTime SelectedEndDate
    {
        get
        {
            if (!Picker1Enabler.Checked) return DateTime.MinValue;
            DateTime retval = Picker1.SelectedDate;
            return new DateTime(retval.Year, retval.Month, retval.Day, 23, 59, 59);
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        Picker1.SelectedDate = LocaleHelper.LocalNow;
        Picker1.Enabled = false;
        Picker1Enabler.Checked = false;
        Picker1Enabler.Attributes.Add("onClick", Picker1.ClientObjectId + ".set_enabled(this.checked);");
        //CalendarButton.Attributes.Add("onmouseup", "CalendarButtonMouseUp(" + Calendar1.ClientObjectId + ")");
        CalendarButton.Attributes.Add("onclick", "document.getElementById('" + Picker1Enabler.ClientID + "').checked = true;" + Picker1.ClientObjectId + ".set_enabled(true);" + Calendar1.ClientObjectId + ".SetSelectedDate(" + Picker1.ClientObjectId + ".GetSelectedDate());" + Calendar1.ClientObjectId + ".Show(this);");
        //
        Calendar1.ClientEvents.SelectionChanged = new ClientEvent(Calendar1.ClientObjectId + "_SelectionChanged");
        Picker1.ClientEvents.SelectionChanged = new ClientEvent(Picker1.ClientObjectId + "_SelectionChanged");
        GenerateScript();
    }

    public void GenerateScript()
    {
        StringBuilder script = new StringBuilder();
        //SELECTIONS CHANGED
        script.Append("function " + Calendar1.ClientObjectId + "_SelectionChanged(sender, eventArgs) { " + Picker1.ClientObjectId + ".setSelectedDate(sender.getSelectedDate()); }\r\n");
        script.Append("function " + Picker1.ClientObjectId + "_SelectionChanged(sender, eventArgs) { " + Calendar1.ClientObjectId + ".setSelectedDate(sender.getSelectedDate()); }\r\n");
        /*
        //CALENDAR CLICK
        script.Append("function CalendarButtonClick(alignElement, calendar){\r\n");
        script.Append("if (calendar.get_popUpShowing()) { calendar.hide(); }\r\n");
        script.Append("else {\r\n");
        script.Append("  calendar.setSelectedDate(calendar.AssociatedPicker.getSelectedDate());\r\n");
        script.Append("  calendar.show(alignElement);\r\n");
        script.Append("}}\r\n");
        //CALENDAR MOUSEUP
        script.Append("function CalendarButtonMouseUp(calendar){\r\n");
        script.Append("if (calendar.get_popUpShowing()) {\r\n");
        script.Append("  event.cancelBubble=true;\r\n");
        script.Append("  event.returnValue=false;\r\n");
        script.Append("  return false;\r\n");
        script.Append("} else { return true; }\r\n");
        script.Append("}\r\n");
         */
        //TOGGLE DATE
        //script.Append("function toggleDate(sender, associatedDate) {\r\n");
        //script.Append("if (sender.checked) associatedDate.style.display = \"block\";\r\n");
        //script.Append("else associatedDate.style.display = \"none\";}");
        this.Page.ClientScript.RegisterClientScriptBlock(string.Empty.GetType(), "ComponentArt_" + this.ClientID, script.ToString(), true);
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        Picker1.Enabled = Picker1Enabler.Checked;
    }
}
