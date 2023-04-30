<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MyAccountPage.ascx.cs" Inherits="ConLib_MyAccountPage" %>
<%--
<conlib>
<summary>Displays the content of customer account page, like recent orders and profile information.</summary>
</conlib>
--%>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Src="~/ConLib/CommunicationPreferencesSection.ascx" TagName="CommunicationPreferencesSection" TagPrefix="uc" %>
<div class="section">
    <div class="pageHeader">
        <h1 class="preference"><asp:Localize ID="RecentOrdersCaption" runat="server" Text="Recent Orders"></asp:Localize></h1>
    </div>
    <div class="content">
        <asp:GridView ID="OrderGrid" runat="server" AutoGenerateColumns="False" ShowHeader="false" GridLines="none" 
            Width="100%" CellPadding="4" RowStyle-CssClass="altodd" AlternatingRowStyle-CssClass="alteven">
            <Columns>
                <asp:TemplateField>
                    <ItemStyle VerticalAlign="top" Width="180px"/>
                    <ItemTemplate>
                        <asp:Label ID="OrderDateLabel" runat="server" Text="Order Date:" SkinID="FieldHeader"></asp:Label>
                        <asp:Label ID="OrderDate" runat="server" Text='<%#Eval("OrderDate", "{0:d}")%>'></asp:Label><br />
                        <asp:Label ID="OrderNumberLabel" runat="server" Text="Order Number:" SkinID="FieldHeader"></asp:Label>
                        <asp:Label ID="OrderNumber" runat="server" Text='<%#Eval("OrderNumber")%>'></asp:Label><br />
                        <asp:Label ID="ShipToLabel" runat="server" Text="Ship To:" SkinID="FieldHeader"></asp:Label>
                        <asp:Label ID="ShipTo" runat="server" Text='<%#GetShipTo(Container.DataItem)%>'></asp:Label><br />
                        <asp:Label ID="StatusLabel" runat="server" Text="Status:" SkinID="FieldHeader"></asp:Label>
                        <asp:Label ID="Status" runat="server" Text='<%#GetOrderStatus(Container.DataItem)%>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemStyle VerticalAlign="top" />
                    <ItemTemplate>
                        <asp:Label ID="ItemsCaption" runat="server" Text="Items" SkinID="FieldHeader"></asp:Label>
                        <asp:Repeater ID="ItemsRepeater" runat="server" DataSource='<%#OrderHelper.GetVisibleProducts(Container.DataItem)%>'>
                            <HeaderTemplate><ul class="orderItemsList"></HeaderTemplate>
                            <ItemTemplate><li><%#Eval("Quantity")%> of: <%#GetOrderItemName(Container.DataItem)%></li></ItemTemplate>
                            <FooterTemplate></ul></FooterTemplate>
                        </asp:Repeater>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemStyle VerticalAlign="middle" Wrap="false" />
                    <ItemTemplate>
                        <asp:HyperLink ID="ViewOrderLink" runat="server" Text="View Order" NavigateUrl='<%#String.Format("~/Members/MyOrder.aspx?OrderNumber={0}&OrderId={1}", Eval("OrderNumber"), Eval("OrderId"))%>' SkinID="Button"></asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <asp:localize ID="EmptyOrderHistory" runat="server" Text="You have not placed any orders."></asp:localize>
            </EmptyDataTemplate>
            <RowStyle CssClass="altodd" />
            <AlternatingRowStyle CssClass="alteven" />
        </asp:GridView>
        <asp:HyperLink ID="ViewAllLink" runat="server" CssClass="showAll" Text="See All &raquo;" NavigateUrl="~/Members/MyOrderHistory.aspx" Visible="false"></asp:HyperLink>
    </div>
</div>
<div class="section">
    <div class="pageHeader">
        <h1 class="preference"><asp:Localize ID="AccountSettingsCaption" runat="server" Text="Account Settings"></asp:Localize></h1>
    </div>
    <div class="content">
        <ul class="columns">
            <li><asp:HyperLink ID="EditProfileLink" runat="server" Text="Update Email Address or Password" NavigateUrl="~/Members/MyCredentials.aspx"></asp:HyperLink></li>
            <li><asp:HyperLink ID="AddressBookLink" runat="server" Text="Manage Address Book" NavigateUrl="~/Members/MyAddressBook.aspx"></asp:HyperLink></li>
            <li><asp:HyperLink ID="WishlistLink" runat="server" Text="Edit My Wishlist" NavigateUrl="~/Members/MyWishlist.aspx"></asp:HyperLink></li>
            <li id="liSubscriptions" runat="server"><asp:HyperLink ID="Subscriptions" runat="server" Text="View Subscriptions" NavigateUrl="~/Members/MySubscriptions.aspx"></asp:HyperLink></li>
            <li id="liReviews" runat="server"><asp:HyperLink ID="Reviews" runat="server" Text="My Product Reviews" NavigateUrl="~/Members/MyProductReviews.aspx"></asp:HyperLink></li>
			<li id="liDigitalGoods" runat="server"><asp:HyperLink ID="DigitalGoods" runat="server" Text="Digital Goods" NavigateUrl="~/Members/MyDigitalGoods.aspx"></asp:HyperLink></li>
        </ul><br />
        <ajax:UpdatePanel ID="UserCurrencyAjax" runat="server">
            <ContentTemplate>
                <asp:Localize ID="UserCurrencyHelpText" runat="server" Text="{0} conducts all transactions in {1}.  For your convenience, you can view prices in your choice of the currencies below.  Additional currencies are given as an estimate only and may not reflect the exact amount charged."></asp:Localize>
                <table class="inputForm" id="UserCurrencyPanel" runat="server">
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="UserCurrencyLabel" runat="server" Text="Preferred Currency:" AssociatedControlID="UserCurrency"></asp:Label>
                        </th>
                        <td>
                            <asp:DropDownList ID="UserCurrency" runat="server" DataSourceID="CurrencyDs" DataTextField="Name" DataValueField="CurrencyId" AutoPostBack="true" OnSelectedIndexChanged="UserCurrency_SelectedIndexChanged" AppendDataBoundItems="true" OnDataBound="UserCurrency_DataBound">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="CurrencyDs" runat="server" OldValuesParameterFormatString="original_{0}"
                                SelectMethod="LoadForStore" TypeName="MakerShop.Stores.CurrencyDataSource" DataObjectTypeName="MakerShop.Stores.Currency"
                                DeleteMethod="Delete" UpdateMethod="Update" SortParameterName="sortExpression" InsertMethod="Insert">
                            </asp:ObjectDataSource>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </ajax:UpdatePanel>
    </div>
</div>
<uc:CommunicationPreferencesSection ID="CommunicationPreferencesSection1" runat="server" />