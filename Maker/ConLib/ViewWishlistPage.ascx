<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ViewWishlistPage.ascx.cs" Inherits="ConLib_ViewWishlistPage" EnableViewState="false" %>
<%--
<conlib>
<summary>Display other customers wishlist.</summary>
<param name="Caption" default="{0}'s Wishlist">Possible value can be any string.  Title of the control.</param>
</conlib>
--%>
<ajax:UpdatePanel ID="WishlistAjax" runat="server">
    <ContentTemplate>
        <div class="pageHeader">
            <h1><asp:Literal runat="server" ID="WishlistCaption" Text="{0}'s Wishlist"></asp:Literal></h1>
        </div>
        <asp:MultiView ID="WishlistMultiView" runat="server" ActiveViewIndex="0">
            <asp:View ID="WishlistView" runat="server">
                <asp:GridView ID="WishlistGrid" runat="server" AutoGenerateColumns="False" ShowHeader="false"
                    ShowFooter="False" DataKeyNames="WishlistItemId" OnRowCommand="WishlistGrid_RowCommand"
                    Width="100%" CellPadding="2" CellSpacing="0" BorderColor="white" BorderWidth="1" 
                    RowStyle-CssClass="even" AlternatingRowStyle-CssClass="altodd" CssClass="itemList" 
                    EnableViewState="false">
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
                                <asp:Label ID="PriceLabel" runat="server" Text="Price:" SkinID="FieldHeader"></asp:Label> <asp:Label ID="Price" runat="server" Text='<%# string.Format("{0:ulc}", GetPrice(Container.DataItem)) %>'></asp:Label><br />
                                <asp:Panel ID="InputPanel" runat="server" Visible='<%#((int)Eval("Inputs.Count") > 0)%>'>
                                    <asp:DataList ID="InputList" runat="server" DataSource='<%#Eval("Inputs") %>'>
                                        <ItemTemplate>
                                            <asp:Label ID="InputName" Runat="server" Text='<%#Eval("InputField.Name", "{0}:")%>' SkinID="FieldHeader"></asp:Label>
                                            <%#Eval("InputValue")%>
                                        </ItemTemplate>
                                    </asp:DataList>
                                </asp:Panel>
                                <asp:Panel ID="WishlistItemPanel" runat="server" Visible='<%# HasKitProducts(Container.DataItem) %>'>
                                    <ul>
                                    <asp:Repeater ID="KitProductRepeater" runat="server" DataSource='<%# GetKitProducts(Container.DataItem) %>'>
                                        <ItemTemplate>
                                            <li><%#Eval("Name")%></li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    </ul>
                                </asp:Panel>
                                <br />
                                <asp:LinkButton ID="AddToBasketButton" runat="Server" SkinID="Button" CommandName="Basket" CommandArgument='<%#Eval("WishlistItemId")%>' Text="Add to Cart" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemStyle BorderColor="#CCCCCC" BorderStyle="solid" BorderWidth="1" />
                            <ItemTemplate>
                                <table cellpadding="3" width="250">
                                    <tr>
                                        <th align="center">
                                            <asp:Localize ID="DesiredLabel" runat="Server" Text="Desired"></asp:Localize>
                                        </th>
                                        <th align="center">
                                            <asp:Localize ID="ReceivedLabel" runat="Server" Text="Received"></asp:Localize>
                                        </th>
                                        <th align="center">
                                            <asp:Localize ID="PriorityLabel" runat="Server" Text="Priority"></asp:Localize>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <%# Eval("Desired") %>
                                        </td>
                                        <td align="center">
                                            <%# Eval("Received") %>
                                        </td>
                                        <td align="center">
                                            <%# GetPriorityString(Eval("Priority")) %>
                                        </td>
                                    </tr>
                                    <tr id="trComment" runat="server" visible='<%# !string.IsNullOrEmpty(Eval("Comment").ToString()) %>'>
                                        <td colspan="3" align="center">
                                            <asp:Label ID="CommentLabel" runat="server" Text="Comment:&nbsp;" SkinID="FieldHeader"></asp:Label>
                                            <%# Eval("Comment") %>
                                        </td>
                                    </tr>
                                </table><br />
                                <span style="font-style:italic;padding-left:10px;">
                                    <asp:Localize ID="LastModifiedDateLabel" runat="server" Text="Last modified on "></asp:Localize><asp:Localize ID="LastModifiedDate" runat="server" text='<%# Eval("LastModifiedDate", "{0:d}") %>'></asp:Localize><br />
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataRowStyle CssClass="emptyWishlistPanel" />
                    <EmptyDataTemplate>
                        <asp:Localize ID="EmptyWishlistMessage" runat="server" Text="Your wishlist is empty."></asp:Localize>
                    </EmptyDataTemplate>
                </asp:GridView>
            </asp:View>
            <asp:View ID="PasswordView" runat="server">
                <div align="center">
                    <br />
                    <asp:Label ID="InvalidPasswordLabel" runat="server" Text="The wishlist password you provided is incorrect.  Enter the correct password to view the list:" SkinID="errorcondition" Visible=false></asp:Label>
                    <asp:Label ID="PasswordLabel" runat="server" Text="This wishlist is password protected.  Enter the password to view the list:"></asp:Label><br /><br />
                    <asp:TextBox ID="Password" runat="server" Text=""></asp:TextBox>
                    <asp:LinkButton ID="CheckPasswordButton" runat="server" Text="View" SkinID="Button" OnClick="CheckPasswordButton_Click" />
                </div><br />
            </asp:View>
        </asp:MultiView>
    </ContentTemplate>
</ajax:UpdatePanel>
