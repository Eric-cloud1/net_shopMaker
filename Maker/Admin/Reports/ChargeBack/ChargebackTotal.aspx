<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="ChargebackTotal.aspx.cs" Inherits="Admin_Reports_ChargeBack_ChargebackTotal" 
Title="Charges back (total)" %>

<%@ Register src="../../UserControls/DatesAndTime.ascx" tagname="DatesAndTime" tagprefix="uc1" %>   



<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">

 <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">

        <ContentTemplate>
        
            <div class="pageHeader">
                <div class="caption">
                    
                    <h1><asp:Localize ID="Caption" runat="server" Text="Manage Chargebacks"></asp:Localize><asp:Localize ID="ReportCaption" runat="server" Text=" {0:d} to {1:d}" Visible="false" EnableViewState="false"></asp:Localize>
                      
                    </h1>
                </div>
                            </div>
            <table align="center" class="form" cellpadding="0" cellspacing="0" border="0" >
      
       <uc1:DatesAndTime ID="dtChargeBackTotal" runat="server" />
                
                
                <tr class="noPrint">
                <td colspan="2"><div style="text-align: Left; vertical-align:middle; "> 
                  <asp:RadioButtonList ID="paymentInstrument" Width="100px" RepeatDirection="Horizontal" runat="server">
                <asp:ListItem Selected="True" Value="0">Any</asp:ListItem>
                <asp:ListItem Value="1">Visa</asp:ListItem>
                <asp:ListItem  Value="2">MC</asp:ListItem>
                 </asp:RadioButtonList>
                        </div>
                    </td>
                    
                    <td style="width:50%" colspan="2"><div style="text-align: left; vertical-align:middle; "> 
                    <asp:Button ID="ProcessButton" runat="server" Text="GO.." OnClick="ProcessButton_Click" /></div> </td>
                </tr>
           
                <tr>
                    <td class="dataSheet" colspan="4">
                        <asp:GridView ID="CBGrid" runat="server" AutoGenerateColumns="False" 
                            DefaultSortExpression="Name" DefaultSortDirection="Ascending" AllowPaging="True" AllowSorting="true"
                            SkinID="Summary" PageSize="80" OnSorting="ChargeBackGrid_Sorting" OnPageIndexChanging="ChargeBackGrid_PageIndexChanging" Width="850px">
                            <Columns>
                                <asp:TemplateField HeaderText="MID" SortExpression="Name">
                                    <ItemStyle HorizontalAlign="Left" width="200" />
                                    <ItemTemplate>
                                        <asp:Label ID="lbName" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Authorized" SortExpression="Auth">
                                    <ItemStyle HorizontalAlign="center" width="100" />
                                    <ItemTemplate>
                                        <asp:Label ID="lbAuthorized" runat="server" Text='<%# Eval("Authorized", "{0:0,#}") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Successful" SortExpression="Successful">
                                    <ItemStyle HorizontalAlign="center" width="100" />
                                    <ItemTemplate>
                                        <asp:Label ID="lbSuccessful" runat="server" Text='<%# Eval("Successful", "{0:0,#}") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Chargeback" SortExpression="ChargeBacks">
                                    <ItemStyle HorizontalAlign="center" width="100" />
                                    <ItemTemplate>
                                        <asp:Label ID="lbChargeback" runat="server" Text='<%# Eval("Chargeback", "{0:0,#}") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                   <asp:TemplateField HeaderText="% CB/Sucess" SortExpression="CBRatio" >
                                    <ItemStyle HorizontalAlign="center" width="100" />
                                    <ItemTemplate>
                                        <asp:Label ID="lbCBRatio" runat="server" Text='<%# FormatPercent(Container.DataItem)%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                   <asp:TemplateField HeaderText="% CB/Current" SortExpression="CBRatio" >
                                    <ItemStyle HorizontalAlign="center" width="100" />
                                    <ItemTemplate>
                                        <asp:Label ID="lbCBRatioCurrent" runat="server" Text='<%#FormatTotalPercent(Container.DataItem)%>'></asp:Label>
                                    </ItemTemplate>
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

        </ContentTemplate>
    </ajax:UpdatePanel>

</asp:Content>

