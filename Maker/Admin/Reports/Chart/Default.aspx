<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Default.aspx.cs" Inherits="Admin_Payment__Default" Title="Payment Setup"  %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
	<div class="caption">
		<h1><asp:Localize ID="Caption" runat="server" Text="ChargeBack Report"></asp:Localize></h1>
	</div>
</div>
<table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
        <td valign="top">
            <ul class="menuList">
                <li>
                    <asp:HyperLink ID="ConversionLabel" runat="server" Text="Conversion" NavigateUrl="Conversion.aspx"></asp:HyperLink><br />
                    <asp:Label ID="ConversionDescription" runat="server" Text="Conversion Charts report."></asp:Label>
                </li>
                <li>
                    <asp:HyperLink ID="ConversionLeadLabel" runat="server" Text="Conversion Lead" NavigateUrl="ConversionLead.aspx"></asp:HyperLink><br />
                    <asp:Label ID="ConversionLeadDescription" runat="server" Text="Conversion Lead report."></asp:Label>
                </li>                
               
               
                <li>
                    <asp:HyperLink ID="ConversionLeadsOrderLabel" runat="server" Text="Conversion Leads Order" NavigateUrl="ConversionLeadsOrder.aspx"></asp:HyperLink><br />
                    <asp:Label ID="ConversionLeadsOrderDescription" runat="server" Text="Conversion Leads Order report."></asp:Label>
                </li>   
                
                   
                <li>
                    <asp:HyperLink ID="ForecastLabel" runat="server" Text="Forecast" NavigateUrl="ForecastChart.aspx"></asp:HyperLink><br />
                    <asp:Label ID="ForecastDescription" runat="server" Text="Forcast report."></asp:Label>
                </li>   
                
                 <li>
                    <asp:HyperLink ID="SalesAbandonLabel" runat="server" Text="Sales Abandon" NavigateUrl="SalesAbandon.aspx"></asp:HyperLink><br />
                    <asp:Label ID="SalesAbandonDescription" runat="server" Text="Sales Abandon report."></asp:Label>
                </li>  
                
                 <li>
                    <asp:HyperLink ID="SalesCurrentLabel" runat="server" Text="Sales Current" NavigateUrl="SalesCurrent.aspx"></asp:HyperLink><br />
                    <asp:Label ID="SalesCurrentDescription" runat="server" Text="Sales Current report."></asp:Label>
                </li>  
                
                 <li>
                    <asp:HyperLink ID="SalesFailedCurrent" runat="server" Text="Sales Failed Current" NavigateUrl="SalesFailedCurrent.aspx"></asp:HyperLink><br />
                    <asp:Label ID="SalesFailedCurrentDescription" runat="server" Text="Sales Failed Current report."></asp:Label>
                </li>  
                
            </ul>
	    </td>
	</tr>
</table>
</asp:Content>

