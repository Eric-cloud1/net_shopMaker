<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true"
    CodeFile="GatewaysPerformance.aspx.cs" Inherits="Admin_Reports_Continuity_GatewaysPerformance"
    Title="Gateways Performance"  EnableViewState="true" %>

<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls"
    TagPrefix="cb" %>
<%@ Register Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar"
    TagPrefix="uc1" %>
    
    <%@ Register src="../../UserControls/DatesAndTime.ascx" tagname="DatesAndTime" tagprefix="uc1" %>
    
   <%@ Register src="../../UserControls/downloadbutton.ascx" tagname="downloadbutton" tagprefix="uc2" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="pageHeader">
                <div class="caption">
                    <h1>
                        Gateways Performance</h1>
                </div>
            </div>
            <table align="center" class="form" cellpadding="0" cellspacing="0" border="0">
            <tr><td>
            <table cellpadding="1" cellspacing="1" width="100%">
               <!-- Custom control Date & Time colspan is 4 -->
             <uc1:DatesAndTime ID="dtGatewaysPerformance" TimePeriod="13" runat="server" />
                            <tr>
                                <td align="left">
                                    <asp:Label ID="lbAffiliates" Text="Affiliates:" runat="server" Style="display: inline;
                                        font-weight: bold; width: 110px;" />
                                </td>
                                <td colspan="3">
                                    <asp:TextBox ID="txtAffiliates" runat="server" Style="display: inline; font-weight: normal;
                                        width: 350px;" />
                                    <ajax:TextBoxWatermarkExtender ID="twAffiliates" runat="server" TargetControlID="txtAffiliates"
                                        WatermarkText="Commas separated affiliate codes..." />
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 60px" align="left">
                                    <asp:Label ID="lbSubaffiliates" Text="Subaffiliates:" runat="server" Style="display: inline;
                                        font-weight: bold; width: 110px;" />
                                </td>
                                <td colspan="3">
                                    <asp:TextBox ID="txtSubAffiliate" runat="server" Style="display: inline; font-weight: normal;
                                        width: 350px;" />
                                    <ajax:TextBoxWatermarkExtender ID="twSubAffiliates" runat="server" TargetControlID="txtSubAffiliate"
                                        WatermarkText="Commas separated Subaffiliate codes..." />
                                    <asp:CheckBox ID="chkSubAffiliate" runat="server" Checked="true" Text="Exact match" />
                                </td>
                            </tr>
                            
                            <tr>
                             <td style="width:10%" align="left">  
                            <asp:Label ID="lbPaymentType" Text="Payments:" runat="server" style="display:inline;font-weight:bold;width:110px;" />
                             </td>
                              <td colspan="3">             
                                <asp:CheckBoxList ID="Payments" RepeatDirection="Horizontal" runat="server">
                                <asp:ListItem Text="Initial" Selected="True" Value ="1-2" />
                                   <asp:ListItem Text="Trial" Selected="True" Value ="3" />
                                    <asp:ListItem Text="Rebill" Selected="True" Value ="4" />
                                    </asp:CheckBoxList>
                           </td>
                            </tr>
                            
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
                                <td class="dataSheet" colspan="4">
                                <div style="overflow:scroll;width:950px;">
                                    <asp:GridView ID="gridGatewaysPerformance" runat="server" AutoGenerateColumns="true"
                                        DefaultSortExpression="Name" DefaultSortDirection="Ascending" AllowPaging="True"
                                   AllowSorting="true" SkinID="Summary" PageSize="25"  OnSorting="gridGatewaysPerformance_Sorting"
                                        Width="100%"  onrowdatabound="gridGatewaysPerformance_RowDataBound" OnPageIndexChanging="gridGatewaysPerformance_PageIndexChanging">
                                      <Columns>
                                        </Columns>
                                        <EmptyDataTemplate>
                                            <div>
                                                <asp:Label ID="EmptyResultsMessage" runat="server" Text="There are no results for the selected time period."></asp:Label>
                                            </div>
                                        </EmptyDataTemplate>
                                    </asp:GridView>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>
