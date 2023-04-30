<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BasketItemDetail.ascx.cs" Inherits="ConLib_Utility_BasketItemDetail" EnableViewState="false" %>
<%--
<conlib>
<summary>Shows details of a product which is currently in basket.</summary>
<param name="LinkProducts" default="false">Possible values are true or false.  Indicates whether the basket items should have a link to the product page or not.</param>
<param name="ShowShipTo" default="false">Possible values are true or false.  Indicates whether the shipping address should be displayed with each item or not.</param>
<param name="ShowAssets" default="false">Possible values are true or false.  Indicates whether the assets like readme files and licence agrements will be shown or not.</param>
<param name="ShowSubscription" default="true">Possible values are true or false.  Indicates whether subscriptions information whill be shown or not.</param>
</conlib>
--%>
<asp:PlaceHolder ID="phProductName" runat="server"></asp:PlaceHolder>
<asp:Localize ID="KitMemberLabel" runat="Server" Text="<br />in {0}" Visible="false"></asp:Localize>
<asp:DataList ID="InputList" runat="server">
    <HeaderTemplate><br /></HeaderTemplate>
    <ItemTemplate>
        <b><asp:Literal ID="InputName" Runat="server" Text='<%#Eval("InputField.Name", "{0}:")%>'></asp:Literal></b>
        <asp:Literal ID="InputValue" Runat="server" Text='<%#Eval("InputValue")%>'></asp:Literal><br />
    </ItemTemplate>
</asp:DataList>
<asp:PlaceHolder ID="KitProductPanel" runat="server" Visible="false">
    <ul class="BasketSubItemLabel">
    <asp:Repeater ID="KitProductRepeater" runat="server">
        <ItemTemplate>
            <li ><asp:Literal ID="KitProductLabel" runat="server" Text='<%#Eval("Name")%>' /></li>
        </ItemTemplate>
    </asp:Repeater>
    </ul>
</asp:PlaceHolder>
<asp:Literal ID="WishlistLabel" runat="Server" Text="<br />from {0}&#39;s Wish List"></asp:Literal>
<asp:PlaceHolder ID="ShipsToPanel" runat="server">
    <br />
    <b><asp:Literal ID="ShipsToLiteral" Runat="server" Text="Shipping to:"></asp:Literal></b>
    <asp:Literal ID="ShipsTo" Runat="server" Text=""></asp:Literal>
</asp:PlaceHolder>
<asp:PlaceHolder ID="GiftWrapPanel" runat="server">
    <br /><b><asp:Literal ID="GiftWrapLiteral" Runat="server" Text="Gift Wrap:"></asp:Literal></b>
    <asp:Literal ID="GiftWrap" Runat="server" Text=""></asp:Literal>
</asp:PlaceHolder>
<asp:PlaceHolder ID="GiftMessagePanel" runat="server">
    <br /><b><asp:Literal ID="GiftMessageLiteral" Runat="server" Text="Gift Message:"></asp:Literal></b>
    <asp:Literal ID="GiftMessage" Runat="server" Text=""></asp:Literal>
</asp:PlaceHolder>
<asp:PlaceHolder ID="SubscriptionPanel" runat="server" Visible="false">
    <br />
    <asp:Literal ID="RecurringPaymentMessage" runat="server"></asp:Literal><br />
</asp:PlaceHolder>
<asp:PlaceHolder ID="AssetsPanel" runat="server">
    <ul class="BasketSubItemLabel">
    <asp:Repeater ID="AssetLinkList" runat="server">
        <ItemTemplate>
            <li ><a href="<%#Eval("NavigateUrl")%>"><%#Eval("Text")%></a></li>
        </ItemTemplate>
    </asp:Repeater>
    </ul>
</asp:PlaceHolder>
