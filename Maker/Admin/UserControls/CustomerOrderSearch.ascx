<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CustomerOrderSearch.ascx.cs" Inherits="Admin_UserControls_CustomerOrderSearch" %>
<table class="contentPanel" cellspacing="0">
    <caption>
        <asp:Label ID="OpenOrderCaption" runat="server" Text="Customer Search"></asp:Label>
    </caption>
	<tr>
		<td style="text-align:center;">
            <asp:Label ID="InstructionText" runat="server" Text="Enter a customer name or address to search for.  You can use * and ? as wildcards.  All orders matching the criteria will be displayed."></asp:Label>
			<asp:TextBox runat="server" ID="CustomerOrderSearchPhrase" Columns="20"></asp:TextBox>
            <asp:ImageButton ID="SearchButton" runat="server" />
		</td>
	</tr>
</table>