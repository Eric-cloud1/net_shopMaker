<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Attempts.ascx.cs" Inherits="Admin_Dashboard_Attempts" %>
<%@ Register Src="~/Admin/UserControls/DatesAndTime.ascx" TagName="DatesAndTime"
    TagPrefix="uc1" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls"
    TagPrefix="cb" %>
<%@ Register Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar"
    TagPrefix="uc1" %>
<div class="contentSection">
    <table cellpadding="1" cellspacing="1" width="100%">
        <uc1:DatesAndTime ID="dtDashBoardAttempts" TimePeriod="0" runat="server" ShowDate="false"
            ShowSubmitRange="True" />
    </table>
    <div style="float: right;">
        <asp:Label ID="ReportTime" runat="server" />
    </div>
    <div style="overflow: scroll; width: 400px;">
        <asp:GridView ID="AttemptReport" runat="server" ShowHeader="True" Width="100%" AutoGenerateColumns="true"
            BorderWidth="0" GridLines="Both" AllowSorting="true" OnSorting="AttemptReport_Sorting"
            OnRowDataBound="AttemptReport_RowDataBound" EnableViewState="false" SkinID="Summary">
            <HeaderStyle CssClass="gridHeader" />
            <Columns>
            </Columns>
        </asp:GridView>
    </div>
</div>
