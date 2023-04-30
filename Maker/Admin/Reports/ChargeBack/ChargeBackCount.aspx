<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="ChargeBackCount.aspx.cs" Inherits="Admin_Reports_ChargeBack_ChargeBackCount"
 Title="Charges back count" %>

<%@ Register src="../../UserControls/PickerAndCalendar.ascx" tagname="PickerAndCalendar" tagprefix="uc1" %>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">

        <ContentTemplate>
        
            <div class="pageHeader">
                <div class="caption">
                    
                    <h1><asp:Localize ID="Caption" runat="server" Text="Chargebacks Count"></asp:Localize>
                        <asp:Localize ID="ReportCaption" runat="server" Text="{0:d} to {1:d}" 
                            Visible="False" EnableViewState="False"></asp:Localize>
                      
                    </h1>
                </div>
                            </div>
                             <table align="center" class="form" cellpadding="0" cellspacing="0" border="0">
            <tr><td valign="top">
     
            <table align="center" style="width:550px" cellpadding="0" cellspacing="0" border="0">
                <tr class="noPrint">
                    <td style="width:40%;">
                        <asp:Label ID="lbStartDate" runat="server" Text="Charge Back Date: " SkinID="FieldHeader"></asp:Label>                 
                          <uc1:PickerAndCalendar ID="ChargeBackDate"  runat="server" />                   
                            <br />
                    </td>
                     <td>
                        <div style="text-align: right; vertical-align:middle; width:20%">
                            <asp:Button ID="ProcessButton" runat="server" Text="GO.." OnClick="ProcessButton_Click" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="dataSheet" colspan="2">
                        <asp:GridView ID="ChargeBackGrid" runat="server" AutoGenerateColumns="False" 
                            DefaultSortExpression="Name" DefaultSortDirection="Ascending" AllowPaging="True" AllowSorting="true"
                            SkinID="Summary" PageSize="80" 
                            OnPageIndexChanging="ChargeBackGrid_PageIndexChanging" 
                            OnRowEditing="ChargeBackGrid_EditCommand" 
                            OnRowCancelingEdit="ChargeBackGrid_CancelCommand" 
                            OnRowUpdating ="ChargeBackGrid_UpdateCommand"  
                            Width="100%">
                            <Columns>
                                <asp:TemplateField HeaderText="Gateway" SortExpression="PaymentGatewayId">
                                    <ItemStyle HorizontalAlign="Left" width="200" />
                                    <ItemTemplate>
                                        <asp:Label ID="lbPaymentGatewayId" runat="server" Text='<%# FormatName(Container.DataItem) %>'></asp:Label>
                                    </ItemTemplate>
                                     <EditItemTemplate> 
                                      <asp:Label ID="lbPaymentGatewayId" runat="server" Text='<%# FormatName(Container.DataItem) %>'></asp:Label>
                                        <asp:Label ID="lbIdPaymentInstrumentId" runat="server"  Text='<%# Eval("PaymentInstrumentId") %>' style="visibility:hidden;"  />
                                     </EditItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Count" SortExpression="Count">
                                    <ItemStyle HorizontalAlign="left"  width="120" />
                                    <ItemTemplate>
                                          <asp:ImageButton ID="EditButton" runat="server" CausesValidation="False" CommandName="Edit" SkinID="EditIcon" AlternateText="Edit" />
                                    &nbsp;&nbsp;
                                        <asp:Label ID="lbCount" runat="server" Text='<%# Eval("Count", "{0:0,#}") %>'></asp:Label>
                                    </ItemTemplate>
                                      <EditItemTemplate> 
                                      <asp:ImageButton ID="UpdateButton" runat="server" CausesValidation="False" CommandName="Update" SkinID="EditIcon" AlternateText="Edit" />
                                      <asp:ImageButton ID="CancelButton" runat="server" CausesValidation="False" CommandName="Cancel" SkinID="CancelIcon" AlternateText="Cancel" />                         
                                      <asp:TextBox ID="txtCount" Width="50px" runat="server" Text='<%# Eval("Count")  %>' />
                                      <asp:Label ID="lbIdChargeBackDate" runat="server"  Text='<%# Eval("ChargeBackDate") %>' style="visibility:hidden;"  />
                                     </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="ChargeBackDate" SortExpression="ChargeBackDate">
                                    <ItemStyle HorizontalAlign="center" width="75" />
                                    <ItemTemplate>
                                        <asp:Label ID="lbChargeBackDate" runat="server" Text='<%# FormatDate(Container.DataItem)  %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Instrument" SortExpression="CreateUser">
                                    <ItemStyle HorizontalAlign="center" width="75" />
                                    <ItemTemplate>
                                        <asp:Label ID="lbInstrument" runat="server" Text='<%# FormatInstrument(Container.DataItem)%>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                    <asp:DropDownList ID="dlPaymentInstrumentId"  runat="server">
                                     <asp:ListItem  Value="0">Void</asp:ListItem>
                                    <asp:ListItem Selected="True" Value="1">Visa</asp:ListItem>
                                    <asp:ListItem Value="2">MC</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:Label ID="lbIdGateway" runat="server"  Text='<%# Eval("PaymentGatewayId") %>' style="visibility:hidden;"  />                                      
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                  
                            </Columns>
                            <EmptyDataTemplate>
                                <div >
                                    <asp:Label ID="EmptyResultsMessage" runat="server" Text="There are no results for the selected time period."></asp:Label>
                                </div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </td>
                </tr>
            </table>
            
              </td>
            <td valign="top"  >
                <div class="section" style="width: 400px">
                        <div class="header">
                            <h2 class="addgroup">Add Charge Back</h2>
                        </div>
                        <div class="content">
                        <table >
                        <tr><td style="width:100">Gateway:</td><td><asp:DropDownList ID="dlPaymentGatewaysId" runat="Server" /></td></tr>
                        <tr><td>Count:</td><td><asp:TextBox ID="txtChargeBackCount" Width="50px" runat="Server" /></td></tr>
                        <tr><td colspan="2">   <asp:RadioButtonList ID="paymentInstrument" Width="100px" RepeatDirection="Horizontal" runat="server">
                        <asp:ListItem Selected="True" Value="1">Visa</asp:ListItem>
                        <asp:ListItem  Value="2">MC</asp:ListItem>
                         </asp:RadioButtonList></td></tr>
                        <tr><td colspan="2"><asp:Button ID="BtAddnew" runat="server" Text="Add New" OnClick="AddNewButton_Click"  class="button" /></td></tr>
                        </table>
                    
                        </div>
                    
</div>
                </div>
            </td>
        </tr>
    </table>

        </ContentTemplate>
    </ajax:UpdatePanel>

</asp:Content>

