<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PickerAndCalendar.ascx.cs" Inherits="Admin_UserControls_PickerAndCalendar" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>

<ComponentArt:Calendar id="Picker1" runat="server" SkinID="Picker"></ComponentArt:Calendar>&nbsp;
<asp:Image ID="CalendarButton" runat="server" CssClass="calendar_button" ImageUrl="~/images/Calendar.gif" ImageAlign="AbsMiddle" />

<ComponentArt:Calendar runat="server" id="Calendar1" SkinID="Calendar" ShowTitle="false">
    <HeaderClientTemplate>
        <div class="calendar_header">
            <span style="float:left;">
                <span class="button" onclick="##Parent.CalendarId##.GoPrevYear()">&darr;</span>
                <span class="button" onclick="##Parent.CalendarId##.GoPrevMonth()">&larr;</span>
                <span class="button" onclick="##Parent.CalendarId##.GoToday()">&middot;</span>
                <span class="button" onclick="##Parent.CalendarId##.GoNextMonth()">&rarr;</span>
                <span class="button" onclick="##Parent.CalendarId##.GoNextYear()">&uarr;</span>
            </span>
            <span style="float:right;">##Parent.FormatDate(Parent.VisibleDate,"MMMM yyyy")##</span>
        </div>
    </HeaderClientTemplate>
    <FooterClientTemplate>
        <div class="calendar_footer">
            <span style="float:right;">
                <input type="button" value="Clear Selected Date" onclick="eval('window.' + ##Parent.CalendarId##_Picker).ClearSelectedDate();##Parent.CalendarId##.Hide();" class="button" />
            </span>
        </div>
    </FooterClientTemplate>
</ComponentArt:Calendar>