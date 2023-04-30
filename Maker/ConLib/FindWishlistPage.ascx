<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FindWishlistPage.ascx.cs" Inherits="ConLib_FindWishlistPage" %>
<%--
<conlib>
<summary>Displays a search form to find wishlists. Also displays the search results.</summary>
</conlib>
--%>
<ajax:UpdatePanel ID="Searchajax" runat="server">
    <ContentTemplate>
        <asp:Panel ID="SearchPanel" runat="server" EnableViewState="false" DefaultButton="SearchButton">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" EnableViewState="false" />
            <table class="inputForm">
	            <tr>
		            <th class="rowHeader">
                        <asp:Label ID="SearchNameLabel" runat="server" Text="Name or E-mail:" AssociatedControlID="SearchName" EnableViewState="false"></asp:Label>
                    </th>
                    <td>
			            <asp:Textbox id="SearchName" runat="server" onFocus="this.select()" Width="200px" EnableViewState="false"></asp:Textbox>
			            <asp:RequiredFieldValidator ID="SearchNameValdiator" runat="server" ControlToValidate="SearchName"
			                 Text="*" ErrorMessage="Name or email address is required." EnableViewState="false"></asp:RequiredFieldValidator>
		            </td>
	            </tr>
	            <tr>
		            <th class="rowHeader">
                        <asp:Label ID="SearchLocationLabel" runat="server" Text="City or State (optional):" EnableViewState="false"></asp:Label>
                    </th>
		            <td>
			            <asp:TextBox id="SearchLocation" runat="server" onFocus="this.select()" Width="140px" EnableViewState="false"></asp:TextBox>
			            <asp:LinkButton ID="SearchButton" runat="server" SkinID="Button" Text="Search" OnClick="SearchButton_Click" EnableViewState="false" />
		            </td>
	            </tr>
	            <tr>
	                <td>&nbsp;</td>
	                <td>
	                    <br />
                        <asp:GridView ID="WishlistGrid" runat="server" AllowPaging="True" 
                            AutoGenerateColumns="False" Width="100%" ShowHeader="False" 
                            DataKeyNames="WishlistId" SkinID="PagedList" DataSourceID="SearchDs"
                            Visible="false" EnableViewState="false">
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:Label ID="ResultNumber" runat="server" Text='<%#String.Format("{0}.", Container.DataItemIndex + 1)%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:HyperLink ID="WishlistLink" runat="server" Text='<%#GetUserName(Eval("User"))%>' NavigateUrl='<%# Eval("WishlistId", "~/ViewWishlist.aspx?WishlistId={0}")%>'></asp:HyperLink><asp:Label ID="WishlistName" runat="server" Text="'s Wish List"></asp:Label><br />
                                        <asp:Label ID="Location" runat="server" Text='<%#GetLocation(Eval("User.PrimaryAddress"))%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <asp:Localize ID="EmptySearchResult" runat="server" Text="There were no wishlists matching your search criteria."></asp:Localize>
                            </EmptyDataTemplate>
                        </asp:GridView>
                        <asp:ObjectDataSource ID="SearchDs" runat="server" OldValuesParameterFormatString="original_{0}"
                            SelectMethod="Search" TypeName="MakerShop.Users.WishlistDataSource" EnableViewState="false">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="SearchName" Name="name" PropertyName="Text" Type="String" />
                                <asp:ControlParameter ControlID="SearchLocation" Name="location" PropertyName="Text" Type="String" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
	                </td>
	            </tr>
            </table>
        </asp:Panel>
    </ContentTemplate>
</ajax:UpdatePanel>