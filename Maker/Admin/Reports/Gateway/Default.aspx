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
                    <asp:HyperLink ID="GatewayBreakdownLabel" runat="server" Text="Gateway Breakdown" NavigateUrl="GatewayBreakdown.aspx"></asp:HyperLink><br />
                    <asp:Label ID="GatewayBreakdownDescription" runat="server" Text="Gateway Breakdown Report."></asp:Label>
                </li>
                <li>
                    <asp:HyperLink ID="GatewaysStatusLabel" runat="server" Text="Gateways Status Performance" NavigateUrl="GatewayStatusSummary.aspx"></asp:HyperLink><br />
                    <asp:Label ID="GatewaysStatusDescription" runat="server" Text="Approval Transactions per Gateway (Instrument) and <br/>Status Performance Report."></asp:Label>
                </li>                
               
               
                <li>
                    <asp:HyperLink ID="ChargeBackCountLabel" runat="server" Text="Charge Back Count" NavigateUrl="~/Admin/Reports/ChargeBack/ChargeBackCount.aspx"></asp:HyperLink><br />
                    <asp:Label ID="ChargeBackCountDescription" runat="server" Text="Charge back count<br/> per Gateway manual entry form and report."></asp:Label>
                </li>   
                
                   
                <li>
                    <asp:HyperLink ID="ChargeBackLabel" runat="server" Text="Charge Back" NavigateUrl="~/Admin/Reports/ChargeBack/ChargeBack.aspx"></asp:HyperLink><br />
                    <asp:Label ID="ChargeBackDescription" runat="server" Text="Charge Back Count per gateway<br/> over transactions Report."></asp:Label>
                </li>   
                
                
            </ul>
	    </td>
	</tr>
</table>
</asp:Content>

