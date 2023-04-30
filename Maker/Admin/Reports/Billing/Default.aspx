<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Default.aspx.cs" Inherits="Admin_Payment__Default" Title="Payment Setup"  %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
	<div class="caption">
		<h1><asp:Localize ID="Caption" runat="server" Text="Continuity Report"></asp:Localize></h1>
	</div>
</div>
<table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
        <td valign="top">
            <ul class="menuList">
                <li>
                    <asp:HyperLink ID="DailyBillingLabel" runat="server" Text="Daily Billing" NavigateUrl="DailyBillingReport.aspx"></asp:HyperLink><br />
                    <asp:Label ID="DailyBillingDescription" runat="server" Text="Performance Conversion Report."></asp:Label>
                </li>
                <li>
                    <asp:HyperLink ID="ForecastAuthorizedLabel" runat="server" Text="Forecast Authorized" NavigateUrl="ForecastAuthorizedReport.aspx"></asp:HyperLink><br />
                    <asp:Label ID="ForecastAuthorizedDescription" runat="server" Text="Forecast Authorized Report."></asp:Label>
                </li>                
               
                 <li>
                    <asp:HyperLink ID="ForecastCapturedLabel" runat="server" Text="Forecast Captured" NavigateUrl="ForecastCapturedReport.aspx"></asp:HyperLink><br />
                    <asp:Label ID="ForecastCapturedDescription" runat="server" Text="Forecast Captured Report."></asp:Label>
                </li>     
                 <li>
                    <asp:HyperLink ID="RebillAuthorizedLabel" runat="server" Text="Rebill Authorized" NavigateUrl="RebillAuthorizeReport.aspx"></asp:HyperLink><br />
                    <asp:Label ID="RebillAuthorizedDescription" runat="server" Text="Rebill Authorized Report."></asp:Label>
                </li>      
                
                 <li>
                    <asp:HyperLink ID="RebillCapturedLabel" runat="server" Text="Rebill Captured" NavigateUrl="RebillCaptureReport.aspx"></asp:HyperLink><br />
                    <asp:Label ID="RebillCapturedDescription" runat="server" Text="Rebill Captured Report."></asp:Label>
                </li>         
                  <li>
                    <asp:HyperLink ID="TransactionLabel" runat="server" Text="Transaction" NavigateUrl="Transaction.aspx"></asp:HyperLink><br />
                    <asp:Label ID="TransactionDescription" runat="server" Text="Transactions Report."></asp:Label>
                </li>     
                  <li>
                    <asp:HyperLink ID="InitialBillingLabel" runat="server" Text="Initial Billing" NavigateUrl="TrialStatus.aspx"></asp:HyperLink><br />
                    <asp:Label ID="InitialBillingDescription" runat="server" Text="Initial Billing Report."></asp:Label>
                </li>      
            </ul>
	    </td>
	</tr>
</table>
</asp:Content>

