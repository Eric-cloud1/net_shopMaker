<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
AutoEventWireup="true" CodeFile="ForecastShipment.aspx.cs"
Inherits="Admin_Reports_Shipping_ForecastShipment" Title="Forecast Shipment
Report" %> <%@ Register Src="~/Admin/UserControls/PickerAndCalendar.ascx"
TagName="PickerAndCalendar" TagPrefix="uc1" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb"
%> <%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI"
TagPrefix="ComponentArt" %> <%@ Register assembly="wwhoverpanel"
Namespace="Westwind.Web.Controls" TagPrefix="wwh" %> <%@ Register
src="~/Admin/UserControls/DatesAndTime.ascx" tagname="DatesAndTime"
tagprefix="uc1" %> <%@ Register src="../../UserControls/downloadbutton.ascx"
tagname="downloadbutton" tagprefix="uc2" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <script type="text/javascript">
    function ShowHoverPanel(event, Id) {
      myArr = Id.split(",");
      OrderHoverLookupPanel.startCallback(
        event,
        "OrderDate=" +
          myArr[0] +
          "&reportType=" +
          myArr[1] +
          "&paymentTypes=" +
          myArr[2],
        null,
        OnError
      );
    }
    function HideHoverPanel() {
      OrderHoverLookupPanel.hide();
    }
    function OnError(Result) {
      alert("*** Error:\r\n\r\n" + Result.message);
    }
  </script>
  <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
      <div class="pageHeader">
        <div class="caption">
          <h1>
            <asp:Localize
              ID="Caption"
              runat="server"
              Text="Forecast Shipments Report"
            ></asp:Localize>
            <asp:Localize
              ID="ReportCaption"
              runat="server"
              Visible="False"
              EnableViewState="False"
            ></asp:Localize>
          </h1>
        </div>
      </div>
      <br />
      <br />
      <table
        align="center"
        cellpadding="0"
        cellspacing="0"
        border="0"
        width="90%"
      >
        <uc1:DatesAndTime
          ID="dtForecastShipment"
          TimePeriod="13"
          runat="server"
        />
        <tr>
          <td style="width: 10%">
            <asp:Button
              ID="ProcessButton0"
              runat="server"
              OnClick="ProcessButton_Click"
              Text="GO.."
              Style="vertical-align: top; height: 25px"
            />
          </td>
          <td colspan="2">
            <uc2:downloadbutton
              ID="downloadbutton1"
              FileName="ForecastAuthorize"
              runat="server"
            />
          </td>
          <td>&nbsp;</td>
        </tr>

        <tr>
          <td class="dataSheet" colspan="4">
            <div style="overflow-x: scroll; width: 970px">
              <asp:GridView
                ID="forecastshipmentReportgrid"
                runat="server"
                AutoGenerateColumns="true"
                OnSorting="forecastshipmentReportgrid_Sorting"
                onrowdatabound="forecastshipmentReportgrid_RowDataBound"
                OnPreRender="forecastshipmentReportgrid_PreRender"
                OnPageIndexChanging="forecastshipmentReportgrid_PageIndexChanging"
                DefaultSortExpression="Name"
                DefaultSortDirection="Ascending"
                AllowPaging="False"
                AllowSorting="true"
                SkinID="Summary"
                PageSize="80"
                Width="100%"
              >
                <Columns> </Columns>
                <EmptyDataTemplate>
                  <asp:Label
                    ID="EmptyResultsMessage"
                    runat="server"
                    Text="There are no results for the selected time period."
                  ></asp:Label>
                </EmptyDataTemplate>
              </asp:GridView>
            </div>
          </td>
        </tr>
      </table>
    </ContentTemplate>
  </ajax:UpdatePanel>

  <wwh:wwHoverPanel
    ID="OrderHoverLookupPanel"
    runat="server"
    serverurl="~/Admin/Reports/Billing/ViewDetails.ashx"
    Navigatedelay="500"
    scriptlocation="WebResource"
    style="display: none; background: white"
    panelopacity="0.89"
    shadowoffset="8"
    shadowopacity="0.18"
    PostBackMode="None"
    AdjustWindowPosition="true"
  >
  </wwh:wwHoverPanel>
</asp:Content>
