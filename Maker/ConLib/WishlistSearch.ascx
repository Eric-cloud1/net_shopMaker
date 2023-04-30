<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WishlistSearch.ascx.cs" Inherits="ConLib_WishlistSearch" EnableViewState="false" %>
<%--
<conlib>
<summary>A search form to find wishlists of other customers.</summary>
</conlib>
--%>
<ajax:UpdatePanel ID="WishlistAjax" runat="server">
    <ContentTemplate>
        <asp:TextBox ID="Name" Width="100px" runat="server" onFocus="this.select()"></asp:TextBox>
        <asp:LinkButton ID="FindButton" runat="server" Text="Search" SkinID="Button" OnClick="FindButton_Click"></asp:LinkButton>
    </ContentTemplate>
</ajax:UpdatePanel>
