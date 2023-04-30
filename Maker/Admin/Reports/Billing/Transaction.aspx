<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true"
    CodeFile="Transaction.aspx.cs" Inherits="Admin_Reports_Transaction" Title="Transactions" %>

<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls"
    TagPrefix="cb" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<%@ Register Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar"
    TagPrefix="uc1" %>
<%@ Register Src="../../UserControls/DatesAndTime.ascx" TagName="DatesAndTime" TagPrefix="uc1" %>

  <%@ Register src="../../UserControls/downloadbutton.ascx" tagname="downloadbutton" tagprefix="uc2" %>
  
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
  
        <ContentTemplate>
            <div class="pageHeader">
                <div class="caption">
                    <h1>
                        Transactions Report</h1>
                </div>
            </div>
        
            <table align="center" class="form" cellpadding="0" cellspacing="0" border="0" width="100%">
             <tr>
                    <td style="width: 100%" colspan="4">
                        <asp:Label ID="SavedMessage" runat="server" Text="Template saved at {0}." EnableViewState="false"
                            Visible="false" SkinID="GoodCondition"></asp:Label>
                        <asp:Label ID="ErrorMessageLabel" runat="server" Text="" EnableViewState="false"
                            Visible="false" SkinID="ErrorCondition"></asp:Label>
                    </td>
                </tr>
                <uc1:DatesAndTime ID="dtTransaction" runat="server" />
                <tr>
                    <td style="width: 10%" align="left">
                        <asp:Label ID="lbOrderType" Text="Report Type:" runat="server" Style="display: inline;
                            font-weight: bold; width: 110px;" />
                    </td>
                    <td >
                        <asp:DropDownList ID="dlReportType" runat="server">
                            <asp:ListItem Value="-1" Selected="True">All</asp:ListItem>
                            <asp:ListItem Value="1">Gross</asp:ListItem>
                            <asp:ListItem Value="2">Hardx</asp:ListItem>
                            <asp:ListItem Value="3">Hardx (pre trial)</asp:ListItem>
                            <asp:ListItem Value="4">Hardx (post trial)</asp:ListItem>
                            <asp:ListItem Value="5">Rebilling</asp:ListItem>
                            <asp:ListItem Value="6">Forecast</asp:ListItem>
                            <asp:ListItem Value="7">Authorized</asp:ListItem>
                            <asp:ListItem Value="8">Captured</asp:ListItem>
                            <asp:ListItem Value="81">Daily captured</asp:ListItem>
                            <asp:ListItem Value="9">Declined</asp:ListItem>

                            <asp:ListItem Value="10">Attempted</asp:ListItem>
                            <asp:ListItem Value="11">Refund</asp:ListItem>
                            
                            <asp:ListItem Value="111">Gross Fraud</asp:ListItem>
                            <asp:ListItem Value="112">not attempted</asp:ListItem>
                            <asp:ListItem Value="113">Decline reduced attempted</asp:ListItem>
                            <asp:ListItem Value="114">Decline reduced approved</asp:ListItem>
                            <asp:ListItem Value="115">Approved</asp:ListItem>
                            
                            <asp:ListItem Value="12">Initial</asp:ListItem>
                            <asp:ListItem Value="14">Trial</asp:ListItem>
                            <asp:ListItem Value="15">Pending Notshipped</asp:ListItem>
                            <asp:ListItem Value="17">Pending Shipped</asp:ListItem>
                            <asp:ListItem Value="16">Pending Cancelled</asp:ListItem>
                            <asp:ListItem Value="151">Pending Total</asp:ListItem>
                            
                            <asp:ListItem Value="159">Product Notshipped</asp:ListItem>
                            <asp:ListItem Value="179">Product shipped</asp:ListItem>
                            
                            <asp:ListItem Value="173">Forecast Shipment</asp:ListItem>
                            <asp:ListItem Value="171">Forecast Shipment Cancel</asp:ListItem>
                            <asp:ListItem Value="172">Forecast Shipment Total</asp:ListItem>
                            
                            <asp:ListItem Value="19">Auth Rebills Attempted</asp:ListItem>
                            <asp:ListItem Value="34">Auth Rebills Authorized</asp:ListItem>
                            <asp:ListItem Value="35">Auth Rebills Declined</asp:ListItem>
                            <asp:ListItem Value="36">Auth Rebills Captured</asp:ListItem>
                            <asp:ListItem Value="18">Caps Rebills Attempted</asp:ListItem>
                            <asp:ListItem Value="20">Caps Rebills Captured</asp:ListItem>
                            <asp:ListItem Value="21">Caps Rebills Declined</asp:ListItem>
                            <asp:ListItem Value="22">Forecast Auth Count</asp:ListItem>
                            <asp:ListItem Value="23">Forecast Auth Retry Count</asp:ListItem>
                            <asp:ListItem Value="231">Forecast Auth Retry Retry Count</asp:ListItem>
                            
                            <asp:ListItem Value="28">Forecast Cap Count</asp:ListItem>
                            <asp:ListItem Value="29">Forecast Cap Retry Count</asp:ListItem>
                            <asp:ListItem Value="301">Forecast Cap Retry Retry Count</asp:ListItem>
                         
                        </asp:DropDownList>
                    </td>                
                    <td style="width: 10%" align="left">
                      <asp:Label ID="countrylbl" Text="Country:" runat="server" Style="display: inline; font-weight: bold; width: 110px;" />
                     </td>
                    <td ><asp:DropDownList ID="countrydl" runat="server"> 
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="width: 10%" align="left">
                        <asp:Label ID="lbDateType" Text="Filter by:" runat="server" Style="display: inline;
                            font-weight: bold; width: 110px;" />
                    </td>
                    <td>
                        <asp:RadioButtonList ID="rdDateType" RepeatDirection="Horizontal" runat="server">
                            <asp:ListItem Text="OrderDate" Value="1" Selected="True" />
                            <asp:ListItem Text="PaymentDate" Value="3" />
                            <asp:ListItem Text="TransactionDate" Value="2" />
                        </asp:RadioButtonList>
                    </td>
                    <td style="width: 10%" align="left">
                        <asp:Label ID="lbPaymentType" Text="Payments:" runat="server" Style="display: inline;
                            font-weight: bold; width: 110px;" />
                    </td>
                    <td>
                        <asp:CheckBoxList ID="Payments" RepeatDirection="Horizontal" runat="server">
                            <asp:ListItem Text="Initial" Selected="True" Value="1" />
                            <asp:ListItem Text="Trial" Selected="True" Value="3" />
                            <asp:ListItem Text="Rebill" Selected="True" Value="4" />
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr>
                    <td style="width: 10%" align="left">
                        <asp:Label ID="lbAffiliates" Text="Affiliates:" runat="server" Style="display: inline;
                            font-weight: bold; width: 110px;" />
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="txtAffiliates" runat="server" Style="display: inline; font-weight: normal;
                            width: 250px;" />
                        <ajax:TextBoxWatermarkExtender ID="twAffiliates" runat="server" TargetControlID="txtAffiliates"
                            WatermarkText="Commas separated affiliate codes..." />
                    </td>
                </tr>
                <tr>
                    <td style="width: 10%" align="left">
                        <asp:Label ID="lbSubaffiliates" Text="Subaffiliates:" runat="server" Style="display: inline;
                            font-weight: bold; width: 110px;" />
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="txtSubAffiliate" runat="server" Style="display: inline; font-weight: normal;
                            width: 250px;" />
                        <ajax:TextBoxWatermarkExtender ID="twSubAffiliates" runat="server" TargetControlID="txtSubAffiliate"
                            WatermarkText="Commas separated subaffiliate codes..." />
                        &nbsp; &nbsp;<asp:CheckBox ID="chkSubAffiliate" runat="server" Text="Exact match" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 10%" align="left">
                        <asp:Label ID="gatewayPrefixlbl" Text="Gateway Prefix:" runat="server" Style="display: inline;
                            font-weight: bold; width: 110px;" />
                    </td>
                    <td>
                        <asp:TextBox runat="server" id="tbGatewayPrefix" />
                    </td>
                    <td style="width: 10%" align="left">
                        <asp:Label ID="Label2" Text="SKU:" runat="server" Style="display: inline;
                            font-weight: bold; width: 110px;" />
                    </td>
                    <td>
                        <asp:TextBox runat="server" id="tbSKU" />
                    </td>
                </tr>
                        
                 <tr>
                <td style="width: 10%">
                    <asp:Button ID="ProcessButton0" runat="server" OnClick="ProcessButton_Click" Text="GO.."
                        Style="vertical-align: top; height: 25px" />
                </td>
                 <td  colspan="2">
                      <uc2:downloadbutton ID="downloadbutton1" FileName="ForecastAuthorize" runat="server" />
            </td>
                <td>&nbsp; </td>
            </tr>
                   <tr>
                    <td colspan="4">
                        <table>
                            <tr>
                                <td style="font-weight: bold;">
                                    Rows:
                                </td>
                                <td>
                                    <asp:Label ID="lRows" runat="server" />
                                </td>
                           <td>&nbsp;</td>
                                <td style="font-weight: bold;">
                                    Unique Orders:
                                </td>
                                <td>
                                    <asp:Label ID="lOrders" runat="server" />
                                </td>
                          <td>&nbsp;</td>
                                <td style="font-weight: bold;">
                                    Amount:
                                </td>
                                <td>
                                    <asp:Label ID="lAmount" runat="server" />
                                </td>
                                <td>&nbsp;</td>
                                <td style="font-weight: bold;">
                                    Last Transaction Successful:
                                </td>
                                <td>
                                    <asp:Label ID="lSuccessful" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="dataSheet" colspan="4">
                        <div style="overflow-x: scroll; width: 950px;">
                            <asp:GridView ID="gridTransactions" runat="server" AutoGenerateColumns="False" DefaultSortExpression="OrderNumber"
                                DefaultSortDirection="Ascending" AllowPaging="true" AllowSorting="true" SkinID="Summary"
                                OnSorting="gridTransactions_Sorting" PageSize="100" OnPageIndexChanging="gridTransactions_PageIndexChanging"
                                Width="100%">
                                <Columns>
                                    <asp:TemplateField HeaderText="OrderId" SortExpression="OrderId" Visible="false">
                                        <ItemStyle HorizontalAlign="right" Width="60" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbOrderId" runat="server" Text='<%# Eval("OrderId", "{0}") %>' Width="75px"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="OrderNumber" SortExpression="OrderNumber">
                                        <ItemStyle HorizontalAlign="right" Width="60" />
                                        <ItemTemplate>
                                            <asp:HyperLink ID="lkOrderNumber" runat="server" Text='<%# Eval("OrderNumber", "{0}") %>'
                                                NavigateUrl='<%# "~/Admin/Orders/ViewOrder.aspx?OrderNumber=" + DataBinder.Eval(Container.DataItem,"OrderNumber").ToString() + "&OrderId="+ DataBinder.Eval(Container.DataItem,"OrderId").ToString() %>'
                                                Target="_blank" Width="75px"></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Order Date" SortExpression="OrderDate">
                                        <ItemStyle HorizontalAlign="left" Width="60" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbOrderDate" runat="server" Text='<%# formatOrderDate(Container.DataItem) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Last Transaction Date" SortExpression="LastTransactionDate">
                                        <ItemStyle HorizontalAlign="left" Width="40" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbLastTransactionDate" runat="server" Text='<%# formatLastTransactionDate(Container.DataItem) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Last Success Transaction Date" SortExpression="LastSuccessTransactionDate">
                                        <ItemStyle HorizontalAlign="left" Width="40" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbLastSuccessTransactionDate" runat="server" Text='<%# formatLastSuccessTransactionDate(Container.DataItem) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Attempts" SortExpression="Attempts">
                                        <ItemStyle HorizontalAlign="right" Width="40" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbAttempts" runat="server" Text='<%# Eval("Attempts", "{0:#,##0;}") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Bill To First Name" SortExpression="BillToFirstName">
                                        <ItemStyle HorizontalAlign="left" Width="75" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbBillToFirstName" runat="server" Text='<%# Eval("BillToFirstName") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Bill To Last Name" SortExpression="BillToLastName">
                                        <ItemStyle HorizontalAlign="left" Width="75" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbBillToLastName" runat="server" Text='<%# Eval("BillToLastName") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Bill To Address" SortExpression="BillToAddress1">
                                        <ItemStyle HorizontalAlign="left" Width="75px" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbBillToAddress1" runat="server" Text='<%# Eval("BillToAddress1", "{0:#,##0;}") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Bill To City" SortExpression="BillToCity">
                                        <ItemStyle HorizontalAlign="left" Width="75" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbBillToCity" runat="server" Text='<%# Eval("BillToCity", "{0:#,##0;}") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Bill To Province" SortExpression="BillToProvince">
                                        <ItemStyle HorizontalAlign="left" Width="75" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbBillToProvince" runat="server" Text='<%# Eval("BillToProvince", "{0:#,##0;}") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Bill To Postal Code" SortExpression="BillToPostalCode">
                                        <ItemStyle HorizontalAlign="left" Width="50" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbBillToPostalCode" runat="server" Text='<%# Eval("BillToPostalCode", "{0:#,##0;}") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Bill To Country Code" SortExpression="BillToCountryCode">
                                        <ItemStyle HorizontalAlign="left" Width="25" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbBillToCountryCode" runat="server" Text='<%# Eval("BillToCountryCode", "{0:#,##0;}") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Bill To Phone" SortExpression="BillToPhone">
                                        <ItemStyle HorizontalAlign="left" Width="50" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbBillToPhone" runat="server" Text='<%# Eval("BillToPhone", "{0:#,##0;}") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Transaction Amount" SortExpression="TransactionAmount">
                                        <ItemStyle HorizontalAlign="right" Width="25" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbTransactionAmount" runat="server" Text='<%# formatTransactionAmount(Container.DataItem) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="CC" SortExpression="CC">
                                        <ItemStyle HorizontalAlign="right" Width="25" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbCC" runat="server" Text='<%# Eval("CC") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Affiliate ID" SortExpression="AffiliateId">
                                        <ItemStyle HorizontalAlign="right" Width="150" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbAffiliateId" runat="server" Text='<%# Eval("AffiliateId") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Sub Affiliate" SortExpression="SubAffiliate">
                                        <ItemStyle HorizontalAlign="left" Width="50" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbSubAffiliate" runat="server" Text='<%# Eval("SubAffiliate") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Response Message" SortExpression="ResponseMessage">
                                        <ItemStyle HorizontalAlign="left" Width="75" />
                                        <ItemTemplate>
                                            <cb:ToolTipLabel ID="ResponseMessageToolTip" runat="server" Text='<%# formatResponseMessage(Container.DataItem)  %>'
                                                ToolTip='<%# Eval("ResponseMessage") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Remote IP" SortExpression="RemoteIP">
                                        <ItemStyle HorizontalAlign="right" Width="50" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbRemoteIP" runat="server" Text='<%# Eval("RemoteIP") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Processor" SortExpression="Processor">
                                        <ItemStyle HorizontalAlign="left" Width="100" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbProcessor" runat="server" Text='<%# Eval("Processor") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Card Type" SortExpression="CardType">
                                        <ItemStyle HorizontalAlign="left" Width="50" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbCardType" runat="server" Text='<%# Eval("CardType") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                           <asp:TemplateField HeaderText="SKU" SortExpression="SKU">
                                        <ItemStyle HorizontalAlign="left" Width="50" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbSKU" runat="server" Text='<%# Eval("SKU") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <asp:Label ID="EmptyResultsMessage" runat="server" Text="There are no results for the selected time period."></asp:Label>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>
