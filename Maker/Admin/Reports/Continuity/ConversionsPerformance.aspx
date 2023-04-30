<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
AutoEventWireup="true" CodeFile="ConversionsPerformance.aspx.cs"
Inherits="Admin_Reports_Continuity_ConversionsPerformance" Title="Conversion
Performance" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %> <%@ Register
Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar"
TagPrefix="uc1" %> <%@ Register src="../../UserControls/DatesAndTime.ascx"
tagname="DatesAndTime" tagprefix="uc1" %> <%@ Register
src="../../UserControls/downloadbutton.ascx" tagname="downloadbutton"
tagprefix="uc2" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
  <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
      <div class="pageHeader">
        <div class="caption">
          <h1>Conversions Performance</h1>
        </div>
      </div>
      <table
        align="center"
        class="form"
        cellpadding="0"
        cellspacing="0"
        border="0"
      >
        <tr>
          <td>
            <table cellpadding="1" cellspacing="1" width="100%">
              <!-- Custom control Date & Time colspan is 4 -->
              <uc1:DatesAndTime
                ID="dtConversionsPerformance"
                TimePeriod="0"
                runat="server"
              />

              <tr>
                <td width="120px" align="left">
                  <asp:Label
                    ID="lbAffiliates"
                    Text="Affiliates:"
                    runat="server"
                    Style="display: inline;
                                            font-weight: bold;"
                  />
                </td>
                <td colspan="3">
                  <asp:TextBox
                    ID="txtAffiliates"
                    runat="server"
                    Style="display: inline; font-weight: normal;
                                            width: 350px;"
                  />
                  <ajax:TextBoxWatermarkExtender
                    ID="twAffiliates"
                    runat="server"
                    TargetControlID="txtAffiliates"
                    WatermarkText="Commas separated affiliate codes..."
                  />
                </td>
              </tr>
              <tr>
                <td align="left">
                  <asp:Label
                    ID="lbSubaffiliates"
                    Text="Subaffiliates:"
                    runat="server"
                    Style="display: inline;
                                            font-weight: bold;"
                  />
                </td>
                <td colspan="3">
                  <asp:TextBox
                    ID="txtSubAffiliate"
                    runat="server"
                    Style="display: inline; font-weight: normal;
                                            width: 350px;"
                  />
                  <ajax:TextBoxWatermarkExtender
                    ID="twSubAffiliates"
                    runat="server"
                    TargetControlID="txtSubAffiliate"
                    WatermarkText="Commas separated Subaffiliate codes..."
                  />
                  <asp:CheckBox
                    ID="chkSubAffiliate"
                    runat="server"
                    Checked="true"
                    Text="Exact match"
                  />
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
                    ID="gridConversionsPerformance"
                    runat="server"
                    AutoGenerateColumns="False"
                    DefaultSortExpression="Name"
                    DefaultSortDirection="Ascending"
                    AllowPaging="True"
                    AllowSorting="true"
                    SkinID="Summary"
                    PageSize="50"
                    EnableSortingAndPagingCallbacks="True"
                    OnPageIndexChanging="gridConversionsPerformance_PageIndexChanging"
                    OnSorting="gridConversionsPerformance_Sorting"
                    Width="920px"
                    onrowdatabound="gridConversionsPerformance_RowDataBound"
                  >
                    <Columns>
                      <asp:TemplateField
                        HeaderText="Affiliate<br/>ID"
                        SortExpression="AffiliateID"
                        Visible="True"
                      >
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                          <asp:Label
                            ID="lbAffiliateID"
                            runat="server"
                            Text='<%# Eval("AffiliateID") %>'
                            >></asp:Label
                          >
                        </ItemTemplate>
                      </asp:TemplateField>
                      <asp:TemplateField
                        HeaderText="Affiliate"
                        SortExpression="Name"
                      >
                        <ItemStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                          <asp:Label
                            ID="lbName"
                            style="font-size: 8px"
                            runat="server"
                            Text='<%# Eval("Name") %>'
                          ></asp:Label>
                        </ItemTemplate>
                      </asp:TemplateField>
                      <asp:TemplateField
                        HeaderText="Sub<br/>Affiliate"
                        SortExpression="SubAffiliateId"
                      >
                        <ItemStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                          <asp:Label
                            ID="lbSubAffiliateId"
                            runat="server"
                            Text='<%# Eval("SubAffiliateId") %>'
                          ></asp:Label>
                        </ItemTemplate>
                      </asp:TemplateField>
                      <asp:TemplateField
                        HeaderText="Clicks"
                        SortExpression="Clicks"
                      >
                        <ItemStyle HorizontalAlign="Right" />
                        <ItemTemplate>
                          <asp:Label
                            ID="lbClicks"
                            runat="server"
                            Text='<%# Eval("Clicks", "{0:#,##0;}") %>'
                          ></asp:Label>
                        </ItemTemplate>
                      </asp:TemplateField>

                      <asp:TemplateField
                        HeaderText="Leads"
                        SortExpression="Leads"
                      >
                        <ItemStyle HorizontalAlign="Right" />
                        <ItemTemplate>
                          <asp:Label
                            ID="lbLeads"
                            runat="server"
                            Text='<%# Eval("Leads", "{0:#,##0;}") %>'
                          ></asp:Label>
                        </ItemTemplate>
                      </asp:TemplateField>

                      <asp:TemplateField
                        HeaderText="Orders"
                        SortExpression="Orders"
                      >
                        <ItemStyle HorizontalAlign="Right" />
                        <ItemTemplate>
                          <asp:Label
                            ID="lbOrders"
                            runat="server"
                            Text='<%# Eval("Orders", "{0:#,##0;}") %>'
                          ></asp:Label>
                        </ItemTemplate>
                      </asp:TemplateField>

                      <asp:TemplateField
                        HeaderText="Conversions"
                        SortExpression="ClicksToOrderPercent"
                      >
                        <ItemStyle HorizontalAlign="Right" />
                        <ItemTemplate>
                          <asp:Label
                            ID="lbClicksToOrderPercent"
                            runat="server"
                            Text='<%# Eval("ClicksToOrderPercent", "{0:#,##0.00%}") %>'
                          ></asp:Label>
                        </ItemTemplate>
                      </asp:TemplateField>
                      <asp:TemplateField
                        HeaderText="Clicks To Lead"
                        SortExpression="ClicksToLeadPercent"
                      >
                        <ItemStyle HorizontalAlign="Right" />
                        <ItemTemplate>
                          <asp:Label
                            ID="lbClicksToLeadPercent"
                            runat="server"
                            Text='<%# Eval("ClicksToLeadPercent", "{0:#,##0.00%}") %>'
                          ></asp:Label>
                        </ItemTemplate>
                      </asp:TemplateField>
                      <asp:TemplateField
                        HeaderText="Leads To Order"
                        SortExpression="LeadsToOrderPercent"
                      >
                        <ItemStyle HorizontalAlign="Right" />
                        <ItemTemplate>
                          <asp:Label
                            ID="lbLeadsToOrderPercent"
                            runat="server"
                            Text='<%# Eval("LeadsToOrderPercent", "{0:#,##0.00%}") %>'
                          ></asp:Label>
                        </ItemTemplate>
                      </asp:TemplateField>
                      <asp:TemplateField
                        HeaderText="Attempts"
                        SortExpression="Attempts"
                      >
                        <ItemStyle HorizontalAlign="Right" />
                        <ItemTemplate>
                          <asp:Label
                            ID="lbAttempts"
                            runat="server"
                            Text='<%# Eval("Attempts", "{0:#,##0;}") %>'
                          ></asp:Label>
                        </ItemTemplate>
                      </asp:TemplateField>
                      <asp:TemplateField
                        HeaderText="Approvals"
                        SortExpression="Approvals"
                      >
                        <ItemStyle HorizontalAlign="Right" />
                        <ItemTemplate>
                          <asp:Label
                            ID="lbApprovals"
                            runat="server"
                            Text='<%# Eval("Approvals", "{0:#,##0;}") %>'
                          ></asp:Label>
                        </ItemTemplate>
                      </asp:TemplateField>
                      <asp:TemplateField
                        HeaderText="Approval"
                        SortExpression="ApprovalPercent"
                      >
                        <ItemStyle HorizontalAlign="Right" />
                        <ItemTemplate>
                          <asp:Label
                            ID="lbApprovalPercent"
                            runat="server"
                            Text='<%# Eval("ApprovalPercent", "{0:#,##0.00%}") %>'
                          ></asp:Label>
                        </ItemTemplate>
                      </asp:TemplateField>
                      <asp:TemplateField
                        HeaderText="Last<br/>Order"
                        SortExpression="LastOrderDate"
                      >
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                          <asp:Label
                            ID="lbLastOrder"
                            runat="server"
                            style="font-size: 9px"
                            Text="<%# formatOrderDate(Container.DataItem) %>"
                          ></asp:Label>
                        </ItemTemplate>
                      </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                      <div>
                        <asp:Label
                          ID="EmptyResultsMessage"
                          runat="server"
                          Text="There are no results for the selected time period."
                        ></asp:Label>
                      </div>
                    </EmptyDataTemplate>
                  </asp:GridView>
                </td>
              </tr>
            </table>
          </td>
        </tr>
      </table>
    </ContentTemplate>
  </ajax:UpdatePanel>
</asp:Content>
