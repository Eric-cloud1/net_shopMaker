<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PickerAndCalendar.ascx.cs" Inherits="ConLib_Utility_PickerAndCalendar" %>
<%-- 
<conlib>
<summary>A utility calendar tool to select a date.</summary>
</conlib>
--%>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<table cellspacing="1" cellpadding="0" border="0">
    <tr>
        <td nowrap>
            <asp:CheckBox id="Picker1Enabler" runat="server" />
            <ComponentArt:Calendar id="Picker1" runat="server" SkinID="Picker"></ComponentArt:Calendar>
        </td>
        <td>
            <asp:Image ID="CalendarButton" runat="server" CssClass="calendar_button" ImageUrl="~/images/Calendar.gif" />
        </td>
    </tr>
</table>
<ComponentArt:Calendar runat="server" id="Calendar1" SkinID="Calendar"></ComponentArt:Calendar>