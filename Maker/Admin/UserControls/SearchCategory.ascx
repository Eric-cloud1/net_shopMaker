<%@ Control Language="C#" ClassName="SearchCategory" %>
<table class="section" cellspacing="0">
    <caption>
        <asp:Label ID="SearchCategoryCaption" runat="server" Text="Search Category"></asp:Label>
    </caption>
	<tr>
		<td style="text-align:center;">
			<asp:TextBox runat="server" ID="SearchPhrase" Columns="15"></asp:TextBox>
            <asp:Button ID="Button1" runat="server" Text="Search" PostBackUrl="~/Admin/Search.aspx" />
		</td>
	</tr>
</table>
