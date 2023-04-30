<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MySubscriptionsPage.ascx.cs" Inherits="ConLib_MySubscriptionsPage" %>
<%--
<conlib>
<summary>Customer account page to display all associated subscriptions and related details.</summary>
</conlib>
--%>
<div class="pageHeader">
    <h1><asp:Localize ID="Caption" runat="server" Text="My Subscriptions"></asp:Localize></h1>
</div>
<div align="center">
    <ajax:UpdatePanel ID="SubAjax" runat="server">
        <ContentTemplate>
            <cb:SortedGridView ID="SubscriptionGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="SubscriptionId" DataSourceID="SubscriptionDs" 
                AllowSorting="True" AllowPaging="false" EnableViewState="False" Width="700" SkinID="PagedList" BorderWidth="0" CellPadding="4" CellSpacing="1"
                DefaultSortDirection="Ascending" DefaultSortExpression="S.ExpirationDate">
                <Columns>
                    <asp:TemplateField HeaderText="Order #" SortExpression="O.OrderNumber">
                        <ItemStyle VerticalAlign="middle" Wrap="false" />
                        <ItemTemplate>
                            <asp:HyperLink ID="ViewOrderLink" runat="server" Text='<%#Eval("OrderItem.Order.OrderNumber")%>' NavigateUrl='<%#String.Format("~/Members/MyOrder.aspx?OrderNumber={0}&OrderId={1}", Eval("OrderItem.Order.OrderNumber"), Eval("OrderItem.OrderId"))%>' SkinID="Link"></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Subscription Plan" SortExpression="SP.Name">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Label ID="SubscriptionPlan" runat="server" text='<%#Eval("SubscriptionPlan.Name")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Active" SortExpression="S.IsActive">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:CheckBox ID="Active" runat="server" Checked='<%#Eval("IsActive")%>' Enabled="False" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Expiration" SortExpression="S.ExpirationDate">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:Label ID="Expiration" runat="server" text='<%#Eval("ExpirationDate", "{0:d}")%>' visible='<%# ((DateTime)Eval("ExpirationDate") != DateTime.MinValue) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <asp:localize ID="NoSubscriptions" runat="server" Text="You do not have any active subscriptions."></asp:localize>
                </EmptyDataTemplate>
            </cb:SortedGridView>
        </ContentTemplate>
    </ajax:UpdatePanel>
</div>
<asp:ObjectDataSource ID="SubscriptionDs" runat="server" SelectMethod="Search" SelectCountMethod="SearchCount"
    TypeName="MakerShop.Orders.SubscriptionDataSource" DataObjectTypeName="MakerShop.Orders.Subscription"
    SortParameterName="sortExpression" EnablePaging="true" OnSelecting="SubscriptionDs_Selecting">
    <SelectParameters>
        <asp:Parameter Name="subscriptionPlanId" DefaultValue="" />
        <asp:Parameter Name="orderRange" DefaultValue="" />
        <asp:Parameter Name="userIdRange" DefaultValue="" />
        <asp:Parameter Name="firstName" DefaultValue="" />
        <asp:Parameter Name="lastName" DefaultValue="" />
        <asp:Parameter Name="email" DefaultValue="" />
        <asp:Parameter Name="expirationStart" DefaultValue="" />
        <asp:Parameter Name="expirationEnd" DefaultValue="" />
        <asp:Parameter Name="active" DefaultValue="" />
    </SelectParameters>
</asp:ObjectDataSource>