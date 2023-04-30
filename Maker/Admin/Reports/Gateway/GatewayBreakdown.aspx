<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
AutoEventWireup="true" CodeFile="GatewayBreakdown.aspx.cs"
Inherits="Admin_Reports_GatewayBreakdown" Title="Gateway Breakdown" %> <%@
Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls"
TagPrefix="cb" %> <%@ Register src="../../UserControls/DatesAndTime.ascx"
tagname="DatesAndTime" tagprefix="uc2" %> <%@ Register
src="../../UserControls/Gateways.ascx" tagname="Gateways" tagprefix="uc1" %> <%@
Register src="../../UserControls/downloadbutton.ascx" tagname="downloadbutton"
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
              Text="Gateway Breakdown"
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
            <div style="text-align: left">Report Period (Transaction Date)</div>
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

        <uc2:DatesAndTime ID="dtGatewayBreakdown" runat="server" />

        <tr>
          <td>
            <asp:Label
              ID="Label1"
              runat="server"
              Text="Date Group:  "
              SkinID="FieldHeader"
            ></asp:Label>
          </td>
          <td>
            <asp:CheckBox
              runat="server"
              ID="cbDateGroup"
              OnCheckedChanged="cbDateGroup_Check"
              AutoPostBack="true"
              Checked="false"
            />
          </td>
          <td>&nbsp;</td>
          <td>&nbsp;</td>
        </tr>

        <tr valign="top">
          <td colspan="2" style="padding: 0 0 0 0; margin: 0 0 0 0">
            <uc1:Gateways
              ID="ucGateways"
              runat="server"
              ShowInActive="false"
              ShowStyle="Gateway_PaymentGateway"
            />
          </td>
          <td style="text-align: left; vertical-align: middle">&nbsp;</td>
          <td class="noPrint" style="text-align: left; padding-left: 12px">
            &nbsp;
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
              OnRowDataBound="BreakdownGrid_RowDataBound"
              AllowPaging="false"
              AllowSorting="true"
              PageSize="20"
              Width="990px"
              OnSorting="BreakdownGrid_Sorting"
              SkinID="PagedList"
              OnPageIndexChanging="BreakdownGrid_PageIndexChanging"
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
