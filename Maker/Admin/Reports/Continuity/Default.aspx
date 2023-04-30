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
                    <asp:HyperLink ID="ConversionPerformanceLabel" runat="server" Text="Conversion" NavigateUrl="ConversionsPerformance.aspx"></asp:HyperLink><br />
                    <asp:Label ID="ConversionPerformanceDescription" runat="server" Text="Performance Conversion Report."></asp:Label>
                </li>
                <li>
                    <asp:HyperLink ID="GatewaysPerformanceLabel" runat="server" Text="Gateways Performance" NavigateUrl="GatewaysPerformance.aspx"></asp:HyperLink><br />
                    <asp:Label ID="GatewaysPerformanceDescription" runat="server" Text="Gateway Performance Description."></asp:Label>
                </li>                
               
            </ul>
	    </td>
	</tr>
</table>
</asp:Content>

