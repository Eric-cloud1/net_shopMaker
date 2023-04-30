<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MyWishlistPage.ascx.cs" Inherits="ConLib_MyWishlistPage" EnableViewState="false" %>
<%--
<conlib>
<summary>Customer account page to display wishlist items of that customer with options like edit/remove and add to cart.</summary>
</conlib>
--%>
<asp:DataList ID="WarningMessageList" runat="server" EnableViewState="false" CssClass="warnCondition">
    <HeaderTemplate><ul></HeaderTemplate>
    <ItemTemplate>
        <li><%# Container.DataItem %></li>
    </ItemTemplate>
    <FooterTemplate></ul></FooterTemplate>
</asp:DataList>
<asp:GridView ID="WishlistGrid" runat="server" AutoGenerateColumns="False" ShowHeader="false"
    ShowFooter="False" DataKeyNames="WishlistItemId" OnRowCommand="WishlistGrid_RowCommand"
    OnDataBound="WishlistGrid_DataBound" Width="100%" CellPadding="2" CellSpacing="0"
    BorderColor="LightGray" RowStyle-CssClass="altodd" AlternatingRowStyle-CssClass="alteven"  EnableViewState="false" DataSourceId="WishlistItemsDs" BorderStyle="Solid">
    <Columns>
        <asp:TemplateField>
            <ItemStyle HorizontalAlign="center" />
            <ItemTemplate>
                <asp:Image ID="Thumbnail" runat="server" AlternateText='<%# Eval("Product.Name") %>' ImageUrl='<%# Eval("Product.ThumbnailUrl") %>' Visible='<%# !string.IsNullOrEmpty(Eval("Product.ThumbnailUrl").ToString()) %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:HyperLink ID="ProductLink" runat="Server" NavigateUrl='<%# UrlGenerator.GetBrowseUrl((int)Eval("ProductId"), CatalogNodeType.Product, Eval("Product.Name").ToString()) %>'><asp:Label ID="NameLabel" runat="server" Text='<%# Eval("Product.Name") %>'></asp:Label><asp:Label ID="OptionNames" runat="Server" Text='<%#Eval("ProductVariant.VariantName", " ({0})")%>' Visible='<%#(Eval("ProductVariant") != null)%>'></asp:Label></asp:HyperLink><br />
                <asp:Label ID="PriceLabel" runat="server" Text="Price:" SkinID="FieldHeader"></asp:Label> <asp:Label ID="Price" runat="server" Text='<%# String.Format("{0:ulc}",GetPrice((WishlistItem)Container.DataItem)) %>'></asp:Label><br />
                <asp:Panel ID="InputPanel" runat="server" Visible='<%#((int)Eval("Inputs.Count") > 0)%>'>
                    <asp:DataList ID="InputList" runat="server" DataSource='<%#Eval("Inputs") %>'>
                        <ItemTemplate>
                            <asp:Label ID="InputName" Runat="server" Text='<%#Eval("InputField.Name", "{0}:")%>' SkinID="fieldheader"></asp:Label>
                            <asp:Label ID="InputValue" Runat="server" Text='<%#Eval("InputValue")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:DataList>
                </asp:Panel>
                <asp:Panel ID="WishlistItemPanel" runat="server" Visible='<%# HasKitProducts(Container.DataItem) %>'>
                    <ul>
                    <asp:Repeater ID="KitProductRepeater" runat="server" DataSource='<%# GetKitProducts(Container.DataItem) %>'>
                        <ItemTemplate>
                            <li><asp:Label ID="KitProductLabel" runat="server" Text='<%#Eval("DisplayName")%>' /></li>
                        </ItemTemplate>
                    </asp:Repeater>
                    </ul>
                </asp:Panel>
                <br />
                <asp:LinkButton ID="AddToBasketButton" runat="Server" SkinID="Button" CommandName="Basket" CommandArgument='<%#Container.DataItemIndex%>' Text="Add to Cart" />
                <asp:LinkButton ID="DeleteItemButton" runat="server" SkinID="Button" CommandName="Delete" OnClientClick="return confirm('Are you sure you want to delete this item from your wishlist?')" Text="Delete"></asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemStyle BorderColor="#CCCCCC" BorderStyle="solid" BorderWidth="1px" />
            <ItemTemplate>
                <table cellspacing="1" cellpadding="2">
                    <tr>
                        <th>
                            <asp:Label ID="DesiredLabel" runat="Server" Text="Desired" SkinID="FieldHeader"></asp:Label>
                        </th>
                        <th>
                            <asp:Label ID="ReceivedLabel" runat="Server" Text="Received" SkinID="FieldHeader"></asp:Label>
                        </th>
                        <th>
                            <asp:Label ID="PriorityLabel" runat="Server" Text="Priority" SkinID="FieldHeader"></asp:Label>
                        </th>
                    </tr>
                    <tr>
                        <td>
                            <asp:TextBox ID="Desired" runat="server" Text='<%#Bind("Desired")%>' Columns="3"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Received" runat="server" Text='<%#Bind("Received")%>' Columns="3"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="Priority" runat="server" SelectedValue='<%#Bind("Priority")%>'>
                                <asp:ListItem Value="4">highest</asp:ListItem>
                                <asp:ListItem Value="3">high</asp:ListItem>
                                <asp:ListItem Value="2">medium</asp:ListItem>
                                <asp:ListItem Value="1">low</asp:ListItem>
                                <asp:ListItem Value="0">lowest</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" align="center">
                            <asp:Label ID="CommentLabel" runat="server" Text="Comment:&nbsp;" AssociatedControlID="Comment" SkinID="FieldHeader"></asp:Label>
                            <asp:TextBox ID="Comment" runat="server" Text='<%#Bind("Comment")%>' Width="200px"></asp:TextBox>
                        </td>
                    </tr>
                </table><br />
                <span style="font-style:italic">
                    <asp:Localize ID="LastModifiedDateLabel" runat="server" Text="Last modified on "></asp:Localize><asp:Localize ID="LastModifiedDate" runat="server" text='<%# Eval("LastModifiedDate", "{0:d}") %>'></asp:Localize><br />
                </span>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <EmptyDataRowStyle CssClass="emptyWishlistPanel" />
    <EmptyDataTemplate>
        <asp:Localize ID="EmptyWishlistMessage" runat="server" Text="Your wishlist is empty."></asp:Localize>
    </EmptyDataTemplate>
    <RowStyle CssClass="altodd" />
    <AlternatingRowStyle CssClass="alteven" />
</asp:GridView>
<div style="text-align:center; margin-top:10px; margin-bottom:10px;">
    <asp:LinkButton ID="KeepShoppingButton" runat="server" SkinID="Button" Text="Keep Shopping" OnClick="KeepShoppingButton_Click"></asp:LinkButton>
    <asp:LinkButton ID="ClearWishlistButton" runat="server" SkinID="Button" Text="Clear Wishlist" OnClientClick="return confirm('Are you sure you want to clear your wishlist?')" OnClick="ClearWishlistButton_Click"></asp:LinkButton>
    <asp:LinkButton ID="UpdateButton" runat="server" SkinID="Button" Text="Update Wishlist" OnClick="UpdateButton_Click"></asp:LinkButton>
    <asp:LinkButton ID="EmailWishlistButton" runat="server" SkinID="Button" Text="Email Wishlist" OnClick="EmailWishlistButton_Click"></asp:LinkButton>
</div>
<asp:ObjectDataSource ID="WishlistItemsDs" runat="server" DataObjectTypeName="MakerShop.Users.WishlistItem"
    DeleteMethod="Delete" OldValuesParameterFormatString="original_{0}" SelectMethod="LoadForWishlist"
    TypeName="MakerShop.Users.WishlistItemDataSource" UpdateMethod="Update" OnSelecting="WishlistItemsDs_Selecting">
    <SelectParameters>
        <asp:Parameter Name="wishlistId" Type="Int32" DefaultValue="0" />
    </SelectParameters>
</asp:ObjectDataSource>
