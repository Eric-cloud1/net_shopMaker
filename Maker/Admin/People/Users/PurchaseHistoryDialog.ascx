<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PurchaseHistoryDialog.ascx.cs" Inherits="Admin_People_Users_PurchaseHistoryDialog" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<table class="inputForm">
    <tr>
        <td class="rowHeader" >
            <asp:Label ID="FirstOrderLabel" runat="server" SkinID="FieldHeader" Text="First order:" EnableViewState="false"/>
            <asp:Label ID="FirstOrder" runat="server" Text="-"></asp:Label>&nbsp;&nbsp;
            <asp:Label ID="PaidOrdersLabel" runat="server" SkinID="FieldHeader" Text="Paid orders:" EnableViewState="false"></asp:Label>
            <asp:Label ID="PaidOrders" runat="server"  Text="-"></asp:Label>&nbsp;&nbsp;
            <asp:Label ID="PurchasesToDateLabel" runat="server" SkinID="FieldHeader" Text="Purchases to date:" EnableViewState="false"></asp:Label>
            <asp:Label ID="PurchasesToDate" runat="server" Text="-"></asp:Label>&nbsp;&nbsp;
            <asp:Label ID="PendingOrdersLabel" runat="server" SkinID="FieldHeader" Text="Pending orders:" EnableViewState="false"></asp:Label>
            <asp:Label ID="PendingOrders" runat="server" Text="-"></asp:Label>
        </td>
    </tr>
</table>
<div class="section" id="PaidOrdersPanel" runat="server">
    <div class="header">
        <h2 class="orderhistory"><asp:Localize ID="Caption" runat="server" Text="Paid Orders"></asp:Localize></h2>
    </div>
    <div class="content">
    <ajax:UpdatePanel ID="PaidOrdersAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cb:SortedGridView ID="PaidOrderGrid" runat="server" DataSourceID="PaidOrdersDs" SkinID="PagedList" Width="100%" AllowPaging="false"  
                AutoGenerateColumns="False" DataKeyNames="OrderId" EnableViewState="True" AllowSorting="true" OnSorting="PaidOrderGrid_Sorting" DefaultSortDirection="Descending" DefaultSortExpression="OrderNumber">
                <Columns>                
                    <asp:TemplateField HeaderText="Order #" SortExpression="OrderNumber">
                        <ItemStyle HorizontalAlign="center" />
                        <ItemTemplate>                            
                            <asp:HyperLink ID="OrderNumber" runat="server" Text='<%# Eval("OrderNumber") %>' SkinID="Link" NavigateUrl='<%#String.Format("~/Admin/Orders/ViewOrder.aspx?OrderNumber={0}&OrderId={1}", Eval("OrderNumber"), Eval("OrderId"))%>'></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Date" SortExpression="OrderDate">
                        <ItemStyle HorizontalAlign="center" />
                        <ItemTemplate>
                            <asp:Label ID="OrderDate" runat="server" Text='<%# Eval("OrderDate", "{0:g}") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Name" SortExpression="Name">
                        <HeaderStyle HorizontalAlign="left" />
                        <ItemStyle HorizontalAlign="left" />
                        <ItemTemplate>
                            <asp:Label ID="ProductName" runat="server" Text='<%# Eval("ProductName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Price" SortExpression="Price">
                        <ItemStyle HorizontalAlign="right" />
                        <ItemTemplate>
                            <asp:Label ID="PurchasePrice" runat="server" Text='<%# Eval("PurchasePrice", "{0:lc}") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Quantity" SortExpression="Quantity">
                        <ItemStyle HorizontalAlign="center" />
                        <ItemTemplate>
                            <asp:Label ID="Quantity" runat="server" Text='<%# Eval("Quantity") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Total" SortExpression="Total" >
                        <ItemStyle HorizontalAlign="right" />
                        <ItemTemplate>
                            <asp:Label ID="Total" runat="server" Text='<%# Eval("Total", "{0:lc}") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            <EmptyDataTemplate>
                <asp:Label ID="EmptyMessage" runat="server" Text="No recent orders."></asp:Label>
            </EmptyDataTemplate>
        </cb:SortedGridView>
        <asp:ObjectDataSource ID="PaidOrdersDs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetUserPurchaseHistory" 
            TypeName="MakerShop.Reporting.ReportDataSource" SortParameterName="sortExpression" EnablePaging="false">
            <SelectParameters>
                <asp:QueryStringParameter Name="userId" DefaultValue="0" QueryStringField="UserId" Type="Int32" />
                <asp:Parameter Name="forPaidOrders" Type="boolean" DefaultValue="true" />
            </SelectParameters>
        </asp:ObjectDataSource>
        </ContentTemplate>
    </ajax:UpdatePanel>
    </div>    
</div>
<br />
<div class="section" id="UnpaidOrdersPanel" runat="server">
    <div class="header">
        <h2 class="orderhistory"><asp:Localize ID="Caption2" runat="server" Text="Unpaid Orders"></asp:Localize></h2>
    </div>
    <div class="content">
    <ajax:UpdatePanel ID="UnPaidOrdersAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cb:SortedGridView ID="UnPaidOrderGrid" runat="server" DataSourceID="UnpaidOrdersDs" SkinID="PagedList" Width="100%" AllowPaging="false"  
                AutoGenerateColumns="False" DataKeyNames="OrderId" EnableViewState="True" AllowSorting="true" OnSorting="UnPaidOrderGrid_Sorting"  DefaultSortDirection="Descending" DefaultSortExpression="OrderNumber">
                <Columns>                
                    <asp:TemplateField HeaderText="Order #" SortExpression="OrderNumber">
                        <ItemStyle HorizontalAlign="center" />
                        <ItemTemplate>                            
                            <asp:HyperLink ID="OrderNumber" runat="server" Text='<%# Eval("OrderNumber") %>' SkinID="Link" NavigateUrl='<%#String.Format("~/Admin/Orders/ViewOrder.aspx?OrderNumber={0}&OrderId={1}", Eval("OrderNumber"), Eval("OrderId"))%>'></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Date" SortExpression="OrderDate">
                        <ItemStyle HorizontalAlign="center" />
                        <ItemTemplate>
                            <asp:Label ID="OrderDate" runat="server" Text='<%# Eval("OrderDate", "{0:g}") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Name" SortExpression="Name">
                        <HeaderStyle HorizontalAlign="left" />
                        <ItemStyle HorizontalAlign="left" />
                        <ItemTemplate>
                            <asp:Label ID="ProductName" runat="server" Text='<%# Eval("ProductName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Price" SortExpression="Price">
                        <ItemStyle HorizontalAlign="right" />
                        <ItemTemplate>
                            <asp:Label ID="PurchasePrice" runat="server" Text='<%# Eval("PurchasePrice", "{0:lc}") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Quantity" SortExpression="Quantity">
                        <ItemStyle HorizontalAlign="center" />
                        <ItemTemplate>
                            <asp:Label ID="Quantity" runat="server" Text='<%# Eval("Quantity") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Total" SortExpression="Total" >
                        <ItemStyle HorizontalAlign="right" />
                        <ItemTemplate>
                            <asp:Label ID="Total" runat="server" Text='<%# Eval("Total", "{0:lc}") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            <EmptyDataTemplate>
                <asp:Label ID="EmptyMessage" runat="server" Text="No recent orders."></asp:Label>
            </EmptyDataTemplate>
        </cb:SortedGridView>
        <asp:ObjectDataSource ID="UnpaidOrdersDs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetUserPurchaseHistory" 
            TypeName="MakerShop.Reporting.ReportDataSource" SortParameterName="sortExpression" EnablePaging="false">
            <SelectParameters>
                <asp:QueryStringParameter Name="userId" DefaultValue="0" QueryStringField="UserId" Type="Int32" />
                <asp:Parameter Name="forPaidOrders" Type="boolean" DefaultValue="false" />
            </SelectParameters>
        </asp:ObjectDataSource>
        </ContentTemplate>
    </ajax:UpdatePanel>
    </div>    
</div>