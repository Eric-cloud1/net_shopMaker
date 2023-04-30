<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DatesAndTime.ascx.cs"
    Inherits="Admin_UserControls_DatesAndTime" %>
<%@ Register Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar"
    TagPrefix="uc1" %>
<tr style="width: 800px;">
    <td width="75px" align="left">
        <asp:Label ID="lbTimePeriod" Text="Time Period:" runat="server" Style="display: inline;
            font-weight: bold;" />
    </td>
    <td align="left">
        <asp:DropDownList ID="SelectDate" runat="server" AutoPostBack="true" OnSelectedIndexChanged="SelectDate_SelectedIndexChanged">
            <asp:ListItem Value="0">Today</asp:ListItem>
            <asp:ListItem Value="14">Last 60 Minutes</asp:ListItem>
            <asp:ListItem Value="13">Last Hour</asp:ListItem>
            <asp:ListItem Value="1">Yesterday</asp:ListItem>
            <asp:ListItem Value="2">This Week</asp:ListItem>
            <asp:ListItem Value="3">Last Week</asp:ListItem>
            <asp:ListItem Value="4">This Month</asp:ListItem>
            <asp:ListItem Value="5">Last Month</asp:ListItem>
            <asp:ListItem Value="6">Last 15 Days</asp:ListItem>
            <asp:ListItem Value="7">Last 30 Days</asp:ListItem>
            <asp:ListItem Value="8">Last 60 Days</asp:ListItem>
            <asp:ListItem Value="9">Last 90 Days</asp:ListItem>
            <asp:ListItem Value="10">Last 120 Days</asp:ListItem>
            <asp:ListItem Value="12" Selected="True">Date Range</asp:ListItem>
        </asp:DropDownList>
    </td>
    <td colspan="2">
        &nbsp;
    </td>
</tr>
<asp:Panel runat="server" ID="pDate" Style="display: inline;" Enabled="true">
    <tr id="DateRange" runat="server">
        <td width="75px" align="left">
            <asp:Label ID="lbStartDate" Text="Start Date:" runat="server" Style="display: inline;
                font-weight: bold;" />
        </td>
        <td align="left" width="275px">
            <uc1:PickerAndCalendar ID="SelectStartDate" runat="server" />
            <asp:Panel runat="server" ID="pHours1" Style="display: inline;">
                <b>H:</b><asp:DropDownList ID="selectStartHour" runat="server">
                    <asp:ListItem Text="00" Selected="True" Value="0"></asp:ListItem>
                    <asp:ListItem Text="01" Value="1"></asp:ListItem>
                    <asp:ListItem Text="02" Value="2"></asp:ListItem>
                    <asp:ListItem Text="03" Value="3"></asp:ListItem>
                    <asp:ListItem Text="04" Value="4"></asp:ListItem>
                    <asp:ListItem Text="05" Value="5"></asp:ListItem>
                    <asp:ListItem Text="06" Value="6"></asp:ListItem>
                    <asp:ListItem Text="07" Value="7"></asp:ListItem>
                    <asp:ListItem Text="08" Value="8"></asp:ListItem>
                    <asp:ListItem Text="09" Value="9"></asp:ListItem>
                    <asp:ListItem Text="10" Value="10"></asp:ListItem>
                    <asp:ListItem Text="11" Value="11"></asp:ListItem>
                    <asp:ListItem Text="12" Value="12"></asp:ListItem>
                    <asp:ListItem Text="13" Value="13"></asp:ListItem>
                    <asp:ListItem Text="14" Value="14"></asp:ListItem>
                    <asp:ListItem Text="15" Value="15"></asp:ListItem>
                    <asp:ListItem Text="16" Value="16"></asp:ListItem>
                    <asp:ListItem Text="17" Value="17"></asp:ListItem>
                    <asp:ListItem Text="18" Value="18"></asp:ListItem>
                    <asp:ListItem Text="19" Value="19"></asp:ListItem>
                    <asp:ListItem Text="20" Value="20"></asp:ListItem>
                    <asp:ListItem Text="21" Value="21"></asp:ListItem>
                    <asp:ListItem Text="22" Value="22"></asp:ListItem>
                    <asp:ListItem Text="23" Value="23"></asp:ListItem>
                </asp:DropDownList>
                <b>M:</b><asp:DropDownList ID="selectStartMinute" runat="server">
                    <asp:ListItem Text="00" Selected="True" Value="0"></asp:ListItem>
                    <asp:ListItem Text="05" Value="5"></asp:ListItem>
                    <asp:ListItem Text="10" Value="10"></asp:ListItem>
                    <asp:ListItem Text="15" Value="15"></asp:ListItem>
                    <asp:ListItem Text="20" Value="20"></asp:ListItem>
                    <asp:ListItem Text="25" Value="25"></asp:ListItem>
                    <asp:ListItem Text="30" Value="30"></asp:ListItem>
                    <asp:ListItem Text="35" Value="35"></asp:ListItem>
                    <asp:ListItem Text="40" Value="40"></asp:ListItem>
                    <asp:ListItem Text="45" Value="45"></asp:ListItem>
                    <asp:ListItem Text="50" Value="50"></asp:ListItem>
                    <asp:ListItem Text="55" Value="55"></asp:ListItem>
                </asp:DropDownList>
                <b>S:</b>00</asp:Panel>
        </td>
        <td align="left" width="75px">
            <asp:Label ID="lbEndDate" Text="End Date:" runat="server" Style="display: inline;
                font-weight: bold;" />
        </td>
        <td width="350px">
            <uc1:PickerAndCalendar ID="SelectEndDate" runat="server" />
            <asp:Panel runat="server" ID="pHours2" Style="display: inline;">
                <b>H:</b><asp:DropDownList ID="selectEndHour" runat="server">
                    <asp:ListItem Text="00" Selected="True" Value="0"></asp:ListItem>
                    <asp:ListItem Text="01" Value="1"></asp:ListItem>
                    <asp:ListItem Text="02" Value="2"></asp:ListItem>
                    <asp:ListItem Text="03" Value="3"></asp:ListItem>
                    <asp:ListItem Text="04" Value="4"></asp:ListItem>
                    <asp:ListItem Text="05" Value="5"></asp:ListItem>
                    <asp:ListItem Text="06" Value="6"></asp:ListItem>
                    <asp:ListItem Text="07" Value="7"></asp:ListItem>
                    <asp:ListItem Text="08" Value="8"></asp:ListItem>
                    <asp:ListItem Text="09" Value="9"></asp:ListItem>
                    <asp:ListItem Text="10" Value="10"></asp:ListItem>
                    <asp:ListItem Text="11" Value="11"></asp:ListItem>
                    <asp:ListItem Text="12" Value="12"></asp:ListItem>
                    <asp:ListItem Text="13" Value="13"></asp:ListItem>
                    <asp:ListItem Text="14" Value="14"></asp:ListItem>
                    <asp:ListItem Text="15" Value="15"></asp:ListItem>
                    <asp:ListItem Text="16" Value="16"></asp:ListItem>
                    <asp:ListItem Text="17" Value="17"></asp:ListItem>
                    <asp:ListItem Text="18" Value="18"></asp:ListItem>
                    <asp:ListItem Text="19" Value="19"></asp:ListItem>
                    <asp:ListItem Text="20" Value="20"></asp:ListItem>
                    <asp:ListItem Text="21" Value="21"></asp:ListItem>
                    <asp:ListItem Text="22" Value="22"></asp:ListItem>
                    <asp:ListItem Text="23" Value="23"></asp:ListItem>
                </asp:DropDownList>
                <b>M:</b><asp:DropDownList ID="selectEndMinute" runat="server">
                    <asp:ListItem Text="00" Selected="True" Value="0"></asp:ListItem>
                    <asp:ListItem Text="05" Value="5"></asp:ListItem>
                    <asp:ListItem Text="10" Value="10"></asp:ListItem>
                    <asp:ListItem Text="15" Value="15"></asp:ListItem>
                    <asp:ListItem Text="20" Value="20"></asp:ListItem>
                    <asp:ListItem Text="25" Value="25"></asp:ListItem>
                    <asp:ListItem Text="30" Value="30"></asp:ListItem>
                    <asp:ListItem Text="35" Value="35"></asp:ListItem>
                    <asp:ListItem Text="40" Value="40"></asp:ListItem>
                    <asp:ListItem Text="45" Value="45"></asp:ListItem>
                    <asp:ListItem Text="50" Value="50"></asp:ListItem>
                    <asp:ListItem Text="55" Value="55"></asp:ListItem>
                </asp:DropDownList>
                <b>S:</b>00</asp:Panel>
        </td>
    </tr>
</asp:Panel>
<tr>
    <td colspan="2">
        <asp:Button ID="ProcessButton" runat="server" Text="GO&hellip;" Style="height: 25px"
            OnClick="ProcessButton_Click" />
    </td>
</tr>
