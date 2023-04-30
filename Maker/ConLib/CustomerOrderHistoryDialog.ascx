<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CustomerOrderHistoryDialog.ascx.cs" Inherits="ConLib_CustomerOrderHistoryDialog" EnableViewState="false" %>
<%--
<conlib>
<summary>Displays customer recent orders list.</summary>
<param name="Caption" default="My Recent Orders">Possible value can be any string.  Title of the control.</param>
</conlib>
--%>
<div class="section">
    <div class="header">
        <h2><asp:Localize ID="CaptionText" runat="server" Text="My Recent Orders"></asp:Localize></h2>
    </div>
    <div class="myRecentOrdersCell">
        <asp:DataList ID="OrderList" runat="server" DataKeyField="OrderId">
            <ItemTemplate>        
                <asp:HyperLink ID="OrderLink" runat="server" Text='<%#GetOrderLabel(Container.DataItem)%>' NavigateUrl='<%#String.Format("~/Members/MyOrder.aspx?OrderNumber={0}&OrderId={1}", Eval("OrderNumber"), Eval("OrderId"))%>' CssClass="bullet"></asp:HyperLink>
            </ItemTemplate>
        </asp:DataList>
     </div>
</div>