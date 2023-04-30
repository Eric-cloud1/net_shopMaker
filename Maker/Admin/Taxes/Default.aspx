<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin_Taxes_Default" Title="Configure Taxes" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
	<div class="caption">
		<h1><asp:Localize ID="Caption" runat="server" Text="Configure Taxes"></asp:Localize></h1>
	</div>
</div>
<table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
        <td valign="top">
            <ul class="menuList">
                <li>
	                <asp:HyperLink ID="SettingsLink" runat="server" Text="Settings" NavigateUrl="Settings.aspx"></asp:HyperLink><br />
	                <asp:Label ID="SettingsDescription" runat="server" Text="Enable taxes and adjust the display settings to your needs."></asp:Label>
                </li>
                <li>
	                <asp:HyperLink ID="TaxCodesLink" runat="server" Text="Tax Codes" NavigateUrl="TaxCodes.aspx"></asp:HyperLink><br />
	                <asp:Label ID="TaxCodesDescription" runat="server" Text="Manage the tax codes that are assigned to your products, so that you can group them for proper taxation."></asp:Label>
                </li>
                <li>
	                <asp:HyperLink ID="TaxRulesLink" runat="server" Text="Tax Rules" NavigateUrl="TaxRules.aspx"></asp:HyperLink><br />
	                <asp:Label ID="TaxRulesDescription" runat="server" Text="Create tax rules to apply a percentage of tax to particular products (tax codes) and zones."></asp:Label>
                </li>
                <li>
	                <asp:HyperLink ID="ZonesLink" runat="server" Text="Zones" NavigateUrl="../Shipping/Zones/Default.aspx"></asp:HyperLink><br />
	                <asp:Label ID="ZonesDescription" runat="server" Text="Define the geographic areas that are used to help determine tax liability."></asp:Label>
                </li>
                <li>
	                <asp:HyperLink ID="TaxProvidersLink" runat="server" Text="Third Party Providers" NavigateUrl="Providers/Default.aspx"></asp:HyperLink><br />
	                <asp:Label ID="TaxProvidersDescription" runat="server" Text="Configure an integrated third party tax provider to allow real time calculation of taxes."></asp:Label>
                </li>
            </ul>
	    </td>
	</tr>
</table>    
</asp:Content>