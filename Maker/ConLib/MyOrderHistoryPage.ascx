<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MyOrderHistoryPage.ascx.cs" Inherits="ConLib_MyOrderHistoryPage" %>
<%--
<conlib>
<summary>Displays order history of a customer.</summary>
</conlib>
--%>
<div class="pageHeader">
    <h1><asp:Localize ID="Caption" runat="server" Text="Your Account"></asp:Localize></h1>
</div>
<div class="section">
    <div class="header">
        <h2><asp:Localize ID="OrderHistoryCaption" runat="server" Text="{0}'s Order History"></asp:Localize></h2>
    </div>
    <div class="content">
        <asp:GridView ID="OrderGrid" runat="server" AutoGenerateColumns="False" ShowHeader="false" GridLines="none" 
            Width="100%" CellPadding="4" RowStyle-CssClass="altodd" AlternatingRowStyle-CssClass="alteven">
            <Columns>
                <asp:TemplateField>
                    <ItemStyle VerticalAlign="top" Width="180"/>
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
                            <ItemTemplate><li><%#Eval("Quantity")%> of: <%#Eval("Name") %></li></ItemTemplate>
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
        </asp:GridView>
    </div>
</div>