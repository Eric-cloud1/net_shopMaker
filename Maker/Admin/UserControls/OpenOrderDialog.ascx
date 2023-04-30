<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OpenOrderDialog.ascx.cs" Inherits="Admin_UserControls_OpenOrderDialog" %>
<table class="contentPanel" cellspacing="0">
    <caption>
        <asp:Localize ID="OpenOrderCaption" runat="server" Text="Open Order:"></asp:Localize>
    </caption>
	<tr>
		<td style="text-align:center;">
			<asp:TextBox runat="server" ID="OrderId" Columns="3"></asp:TextBox>
            <asp:ImageButton ID="OpenOrderButton" runat="server" SkinID="GoButton" OnClick="OpenOrderButton_Click" />
		</td>
	</tr>
</table>
