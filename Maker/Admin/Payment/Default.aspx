<%@ Page Language="C#" MasterPageFile="../Admin.master" CodeFile="Default.aspx.cs" Inherits="Admin_Payment__Default" Title="Payment Setup"  %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
	<div class="caption">
		<h1><asp:Localize ID="Caption" runat="server" Text="Payment Setup"></asp:Localize></h1>
	</div>
</div>
<table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
        <td valign="top">
            <ul class="menuList">
                <li>
                    <asp:HyperLink ID="MethodsLabel" runat="server" Text="Methods" NavigateUrl="Methods.aspx"></asp:HyperLink><br />
                    <asp:Label ID="MethodsDescription" runat="server" Text="Use the Payment Method screen to indicate what methods of payment you accept for your store."></asp:Label>
                </li>
                <li>
                    <asp:HyperLink ID="GatewaysLabel" runat="server" Text="Gateways" NavigateUrl="Gateways.aspx"></asp:HyperLink><br />
                    <asp:Label ID="GatewaysDescription" runat="server" Text="Configure a Gateway to allow real time processing of credit cards."></asp:Label>
                </li>                
                <li>
                    <asp:HyperLink ID="GatewayFailoverLabel" runat="server" Text="Gateway Failover" NavigateUrl="~/Admin/Payment/GatewayFailover/GatewayFailover.aspx"></asp:HyperLink><br />
                    <asp:Label ID="GatewayFailoverDescription" runat="server" Text="Configure a Gateway failover."></asp:Label>
                </li>
                <li>
                    <asp:HyperLink ID="GatewayResponseLabel" runat="server" Text="Gateways Response" NavigateUrl="~/Admin/Payment/GatewayResponse/GatewayResponse.aspx"></asp:HyperLink><br />
                    <asp:Label ID="GatewayResponseDescription" runat="server" Text="Configure a Gateway response."></asp:Label>
                </li>
                <li>
                    <asp:HyperLink ID="ViewGatewayTemplatesLabel" runat="server" Text="View Gateways Templates" NavigateUrl="~/Admin/Payment/GatewayTemplates.aspx"></asp:HyperLink><br />
                    <asp:Label ID="ViewGatewayTemplatesDescription" runat="server" Text="View Gateway Templates"></asp:Label>
                </li>
                <li>
                    <asp:HyperLink ID="GatewayTemplatesLabel" runat="server" Text="Gateways Templates" NavigateUrl="Templates.aspx"></asp:HyperLink><br />
                    <asp:Label ID="GatewayTemplatesDescription" runat="server" Text="Configure a Gateway Templates."></asp:Label>
                </li>
                <li>
                    <asp:HyperLink ID="SettingsLabel" runat="server" Text="Gateways" NavigateUrl="Settings.aspx"></asp:HyperLink><br />
                    <asp:Label ID="SettingsDescription" runat="server" Text="Configure Settings"></asp:Label>
                </li>
             
            </ul>
	    </td>
	</tr>
</table>
</asp:Content>

