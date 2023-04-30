<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="SalesByAffiliate.aspx.cs"
    Inherits="Admin_Reports_SalesByAffiliate" Title="Sales by Affiliate" %>


<%@ Register Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar"
    TagPrefix="uc1" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls"
    TagPrefix="cb" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">


<script type="text/javascript" src="<%= Page.ResolveUrl("~")%>js/jquery.js"></script>

<script type="text/javascript">
    $(document).ready(function() {
    //hide the all of the element with class div_exp
        $(".div_exp").hide();
        //toggle the componnent with class div_exp
        $(".div_head").click(function() {
        $(this).next(".div_exp").slideToggle(600);
        });
    });
</script>

    <style type="text/css">
.div_head {
	padding: 5px 10px;
	cursor: pointer;
	position: relative;
	margin:1px;
}
.div_exp {
	padding: 5px 10px 15px;
}
</style>
    <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="pageHeader">
                <div class="caption">
                    <h1>
                        <asp:Localize ID="Caption" runat="server" Text="Sales by Affiliate"></asp:Localize>
                        <asp:Localize ID="ReportDateCaption" runat="server" Text="for {0:MMMM yyyy}" Visible="False"
                            EnableViewState="False"></asp:Localize></h1>
                </div>
            </div>
               

		

            <!--- <table cellpadding="2" cellspacing="0" class="innerLayout"> -->
            <table align="center" class="form" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <th class="sectionHeader" colspan="4">
                        <div style="text-align: left">
                            Report Period</div>
                    </th>
                </tr>
                <tr class="noPrint">
                <td style="text-align: left">
                          <asp:Label ID="Label1" runat="server" Text="From:  " SkinID="FieldHeader"></asp:Label>
                    </td>
                    <td style="text-align: left">
                    
                        <uc1:PickerAndCalendar ID="PickerAndCalendar1" runat="server" />
                    </td>
                    <td>
                        <div style="text-align: right; vertical-align: middle">
                            <asp:Label ID="Label4" runat="server" Text="To:  " SkinID="FieldHeader"></asp:Label>
                        </div>
                    </td>
                    <td style="text-align: left">
                        <uc1:PickerAndCalendar ID="PickerAndCalendar2" runat="server" />
                    </td>
                    <td>
                        <div style="text-align: right; vertical-align: middle">
                            <asp:Button ID="btUpdateReport" runat="server" OnClick="UpdateReport_Click" Text="Update Report" />
                        </div>
                    </td>
                </tr>
            </table>
            <table align="center" class="form" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td class="dataSheet" >           
                        <asp:DataList ID="AffiliateSalesGrid" runat="server" AutoGenerateColumns="False"
                             Width="100%"  Visible="False" ShowWhenEmpty="False" onitemcommand="AffiliateSalesGrid_ItemCommand">
                             <headertemplate>
                             <table style="padding:5px 4px;font-weight:bold;margin:0px; background:url(images/section_header_bg.gif) repeat-x left top;" width="600px">
                                 <tr><td style="text-align:left; width:150px;border: 1px solid #738AD0; " colspan="2">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Affiliate&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                               
                                 <td style="text-align:left; width:60px;border: 1px solid #738AD0; ">Orders</td>
                                 <td style="text-align:left; width:60px;border: 1px solid #738AD0; ">Successful</td>
                                 <td style="text-align:left; width:60px;border: 1px solid #738AD0; ">Pending</td>
                                
                                 <td style="text-align:center; width:70px;border: 1px solid #738AD0; ">Total</td>
                                 <td style="text-align:center; width:70px;border: 1px solid #738AD0; ">Commission</td>
                                 </tr>
                            
                             </headertemplate>                              
                             <itemtemplate> 
                             <div class="div_head">                                              
                             <tr style="background-color: #FFFFFF; " >
                             <td style="text-align:left; width:5px;"><asp:ImageButton ID="imgSelect" CommandName="select" ImageUrl="~/images/plus.gif" runat="server" /></td>
                             <td style="text-align:left; width:145px;">
                             <asp:HyperLink ID="AffiliateLink" runat="server" Text='<%# Eval("AffiliateName") %>' 
                                            NavigateUrl='<%#Eval("AffiliateId", "../Marketing/Affiliates/EditAffiliate.aspx?AffiliateId={0}")%>'></asp:HyperLink>
                             </td>
                             <td style="text-align:center; width:60px;">  <asp:Label ID="OrderCount" runat="server" Text='<%# Eval("OrderCount") %>'></asp:Label></td>
                             <td style="text-align:center; width:60px;"><asp:Label ID="Successful" runat="server" Text='<%# Eval("Successful") %>'></asp:Label></td>
                             <td style="text-align:center; width:60px;"><asp:Label ID="Pending" runat="server" Text='<%# Eval("Pending") %>'></asp:Label></td>
                             <td style="text-align:center; width:70px;"><asp:Label ID="OrderTotal" runat="server" Text='<%# GetOrderTotal(Container.DataItem)  %>'></asp:Label></td>
                             <td style="text-align:center; width:70px;"><asp:Label ID="CommissionRate" runat="server" Text='<%# GetCommissionRate(Container.DataItem) %>'></asp:Label>
                                  <asp:Label ID="Commission" runat="server" Text='<%# GetCommission(Container.DataItem) %>'></asp:Label></td>
                             </tr>
                          </itemtemplate>
                      
                         <selecteditemtemplate>
                      
                         <tr  style="background-color: #FFFFFF;">
                         <td style="text-align:left; width:5px;"><asp:ImageButton ID="imgGroup" CommandName="group" ImageUrl="~/images/minus.gif" runat="server" /></td>
                           <td style="text-align:left; width:145px;">
                             <asp:HyperLink ID="AffiliateLink" runat="server" Text='<%# Eval("AffiliateName") %>' 
                                            NavigateUrl='<%#Eval("AffiliateId", "../Marketing/Affiliates/EditAffiliate.aspx?AffiliateId={0}")%>'></asp:HyperLink>
                             </td>
                             <td style="text-align:center; width:60px;">  <asp:Label ID="OrderCount" runat="server" Text='<%# Eval("OrderCount") %>'></asp:Label></td>
                             <td style="text-align:center; width:60px;"><asp:Label ID="Successful" runat="server" Text='<%# Eval("Successful") %>'></asp:Label></td>
                             <td style="text-align:center; width:60px;"><asp:Label ID="Pending" runat="server" Text='<%# Eval("Pending") %>'></asp:Label></td>
                             <td style="text-align:center; width:70px;"><asp:Label ID="OrderTotal" runat="server" Text='<%# GetOrderTotal(Container.DataItem)  %>'></asp:Label></td>
                             <td style="text-align:center; width:70px;"><asp:Label ID="CommissionRate" runat="server" Text='<%# GetCommissionRate(Container.DataItem) %>'></asp:Label>
                                  <asp:Label ID="Commission" runat="server" Text='<%# GetCommission(Container.DataItem) %>'></asp:Label></td>
                             </tr>
                             <tr><td colspan="10" >
                                  <cb:SortedGridView ID="SubAffiliateSalesGrid" runat="server" DataSource='<%# getSubData((int)DataBinder.Eval(Container.DataItem, "AffiliateId"))%>' AutoGenerateColumns="False"
                             Width="600px" SkinID="Summary" Visible="true" PageSize="7" OnSorting="SubAffiliateSalesGrid_Sorting" OnPageIndexChanging="SubAffiliateSalesGrid_PageIndexChanging" 
                            DefaultSortDirection="Ascending" DefaultSortExpression="" ShowWhenEmpty="False" ShowHeader="false">
                            <Columns>
                            <asp:TemplateField >
                            <ItemStyle HorizontalAlign="Left" Width="5px" />
                            <ItemTemplate><img src="../../images/minus.gif" /></ItemTemplate>
                            </asp:TemplateField>
                                <asp:TemplateField >
                               
                                    <ItemStyle HorizontalAlign="Left" Width="100px" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="SubAffiliateLink" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"AffiliateName") %>'
                                            NavigateUrl='<%#Eval("AffiliateId", "../Marketing/Affiliates/EditAffiliate.aspx?AffiliateId={0}")%>'></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                             
                        
                                <asp:TemplateField >
                                    <ItemStyle HorizontalAlign="Center" Width="70px" />
                                    <ItemTemplate>
                                        <asp:Label ID="SubOrderCount" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"OrderCount") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField >
                                    <ItemStyle HorizontalAlign="Center" Width="60px" />
                                    <ItemTemplate>
                                        <asp:Label ID="SubSuccessful" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"Successful") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField >
                                    <ItemStyle HorizontalAlign="Center" Width="60px" />
                                    <ItemTemplate>
                                        <asp:Label ID="SubPending" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"Pending") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                    
                                <asp:TemplateField >
                                    <ItemStyle HorizontalAlign="Center" Width="70px" />
                                    <ItemTemplate>
                                        <asp:Label ID="SubOrderTotal" runat="server" Text='<%#  GetOrderTotal(Container.DataItem)  %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField >
                                
                                    <ItemStyle HorizontalAlign="Right" Width="70px" />
                                    <ItemTemplate>
                              
                                        <asp:Label ID="SubCommissionRate" runat="server" Text='<%# GetCommissionRate(Container.DataItem) %>'></asp:Label>
                                  
                                        <asp:Label ID="SubCommission" runat="server" Text='<%# GetCommission(Container.DataItem) %>'></asp:Label>
                                 
                                       
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                           
                        </cb:SortedGridView>
                             
                             
                             </td></tr>
                             </selecteditemtemplate> 
                           
                       
                          
                        <FooterTemplate>
                        </table>
                        <table id="tblNoRecord" border="1" runat="server"  width="650px" visible="false" class="empty">
                        <tr><td colspan="9"><asp:Label ID="EmptyResultsMessage" runat="server" Text="There are no results for the selected time period."></asp:Label></td></tr>
                         </table>  
                        </FooterTemplate>
                        </asp:DataList>
                       
                        <br />
                        <i>
                            <asp:Label ID="ReportTimestamp" runat="server" Text="Report generated {0:MMM-dd-yyyy hh:mm tt}"
                                EnableViewState="false"></asp:Label></i>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Button ID="btEmailReport" runat="server" OnClick="EmailReport_Click" Text="eMail CSV file" />
                  
                </tr>
            </table>
            <asp:HiddenField ID="HiddenStartDate" runat="server" Value="" />
            <asp:HiddenField ID="HiddenEndDate" runat="server" Value="" />
        </ContentTemplate>
    </ajax:UpdatePanel>

   
</asp:Content>
