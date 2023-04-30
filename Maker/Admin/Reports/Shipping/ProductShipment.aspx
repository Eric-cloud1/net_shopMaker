<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
AutoEventWireup="true" CodeFile="ProductShipment.aspx.cs"
Inherits="Admin_Reports_Shipping_ProductShipment" Title="Product Shipments
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
    function ShowHoverPanel(event, Id1) {
      myArr = Id1.split(",");
      OrderHoverLookupPanel.startCallback(
        event,
        "OrderDate=" +
          myArr[0] +
          "&reportType=" +
          myArr[1] +
          "&paymentTypes=" +
          myArr[2] +
          "&country=" +
          myArr[3],
        null,
        OnError
      );
    }
    function ShowHoverPanelSku(event, Id1) {
      myArr = Id1.split(",");
      OrderHoverLookupPanel.startCallback(
        event,
        "OrderDate=" +
          myArr[0] +
          "&reportType=" +
          myArr[1] +
          "&paymentTypes=" +
          myArr[2] +
          "&paymentGatewayPrefix=" +
          myArr[3] +
          "&country=" +
          myArr[4],
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
              Text="Product Shipments Report"
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

      <table
        align="center"
        class="form"
        cellpadding="0"
        cellspacing="0"
        border="0"
        width="100%"
      >
        <tr>
          <td style="width: 100%">
            <asp:Label
              ID="SavedMessage"
              runat="server"
              Text="Template saved at {0}."
              EnableViewState="false"
              Visible="false"
              SkinID="GoodCondition"
            ></asp:Label>
            <asp:Label
              ID="ErrorMessageLabel"
              runat="server"
              Text=""
              EnableViewState="false"
              Visible="false"
              SkinID="ErrorCondition"
            ></asp:Label>
          </td>
        </tr>
      </table>

      <table
        align="center"
        class="form"
        cellpadding="0"
        cellspacing="0"
        border="0"
        width="100%"
      >
        <uc1:DatesAndTime ID="dtProductShipment" runat="server" />

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
          <td class="dataSheet" colspan="3">
            <div style="overflow-x: scroll; width: 970px">
              <asp:GridView
                ID="productshipmentReportgrid"
                runat="server"
                AutoGenerateColumns="true"
                onrowdatabound="productshipmentReportgrid_RowDataBound"
                OnSorting="productshipmentReportgrid_Sorting"
                DefaultSortExpression="Name"
                DefaultSortDirection="Ascending"
                OnPreRender="productshipmentReportgrid_PreRender"
                AllowPaging="true"
                AllowSorting="true"
                SkinID="Summary"
                OnPageIndexChanging="productshipmentReportgrid_PageIndexChanging"
                PageSize="25"
                Width="950px"
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
