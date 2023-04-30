<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
AutoEventWireup="true" CodeFile="GatewayStatusSummary.aspx.cs"
Inherits="Admin_Reports_GatewayStatusSummary" Title="Gateway Status Summary" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls"
TagPrefix="cb" %> <%@ Register Src="../../UserControls/DatesAndTime.ascx"
TagName="DatesAndTime" TagPrefix="uc2" %> <%@ Register
src="../../UserControls/downloadbutton.ascx" tagname="downloadbutton"
tagprefix="uc2" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="Server">
  <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
      <div class="pageHeader">
        <div class="caption">
          <h1>
            <asp:Localize
              ID="Caption"
              runat="server"
              Text="Gateway Summary"
            ></asp:Localize>
            <asp:Localize
              ID="ReportCaption"
              runat="server"
              Visible="False"
              EnableViewState="false"
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
          <th class="sectionHeader" colspan="4">
            <div style="text-align: left">
              Report Period&nbsp;<asp:Label ID="dateType" runat="server" />
            </div>
          </th>
        </tr>
        <tr>
          <td style="width: 100%" colspan="4">
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

        <!-- Custom control Date & Time colspan is 4 -->
        <uc2:DatesAndTime
          ID="dtGatewaySummary"
          runat="server"
          ShowSubmit="false"
        />

        <tr>
          <td colspan="4">
            <asp:RadioButtonlist
              ID="dateTypelst"
              runat="server"
              RepeatDirection="Horizontal"
            >
              <asp:ListItem Value="1" Text="Authorize date" Selected="True" />
              <asp:ListItem Value="0" Text="Capture date" />
            </asp:RadioButtonlist>
          </td>
        </tr>

        <tr>
          <td>
            <div style="visibility: hidden">
              <asp:CheckBox
                runat="server"
                ID="cbAuthorize"
                OnCheckedChanged="cbAuthorize_Check"
                AutoPostBack="true"
                Checked="true"
                Text="Authorized"
              />
            </div>
          </td>
          <td>
            <asp:CheckBox
              runat="server"
              ID="cbDateGroup"
              OnCheckedChanged="cbDateGroup_Check"
              AutoPostBack="true"
              Checked="true"
              Text="Date Group"
            />
          </td>
          <td style="width: 10%">
            <div style="font-weight: bold">
              <cb:ToolTipLabel
                ID="MethodLabel"
                runat="server"
                Text="Pivot: "
                ToolTip="(Gateway) Displays counts/amounts for each Payment Gateway used.(Instrument) Displays counts/amounts for each Payment Gateway and instrument used."
              >
              </cb:ToolTipLabel>
            </div>
          </td>
          <td>
            <div style="text-align: Left; vertical-align: middle">
              <asp:RadioButtonList
                ID="rblGatewayInstrument"
                runat="server"
                RepeatDirection="Horizontal"
              >
                <asp:ListItem Selected="True" Value="0">Gateway</asp:ListItem>
                <asp:ListItem Value="1">Instrument</asp:ListItem>
              </asp:RadioButtonList>
            </div>
          </td>
        </tr>

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
            <asp:GridView
              ID="BreakdownGrid"
              runat="server"
              AutoGenerateColumns="True"
              OnRowDataBound="gv_RowDataBound"
              OnSorting="gv_Sorting"
              AllowPaging="false"
              Width="990px"
              SkinID="PagedList"
              AllowSorting="true"
            >
              <EmptyDataTemplate>
                <div class="emptyResult">
                  <asp:Label
                    ID="EmptyResultsMessage"
                    runat="server"
                    Text="There are no gateways to report for the selected dates."
                  ></asp:Label>
                </div>
              </EmptyDataTemplate>
            </asp:GridView>
          </td>
        </tr>
        <tr>
          <td style="text-align: right" colspan="4">
            <div>
              <table>
                <tr>
                  <td>
                    <asp:Label
                      ID="LblTotalCount"
                      runat="server"
                      SkinID="FieldHeader"
                      Text="Total Transactions: "
                      Visible="False"
                    ></asp:Label>
                  </td>
                  <td>
                    <asp:Label
                      ID="TotalCount"
                      runat="server"
                      Text=""
                    ></asp:Label>
                  </td>
                  <td>
                    <asp:Label
                      ID="LblTotalAmount"
                      runat="server"
                      SkinID="FieldHeader"
                      Text="Total Capture Amount: "
                      Visible="False"
                    ></asp:Label>
                  </td>
                  <td>
                    <asp:Label
                      ID="TotalAmount"
                      runat="server"
                      Text=""
                    ></asp:Label>
                  </td>
                </tr>
              </table>
            </div>
          </td>
        </tr>
      </table>
    </ContentTemplate>
  </ajax:UpdatePanel>
</asp:Content>
