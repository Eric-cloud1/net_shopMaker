<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SetWishlistPassword.ascx.cs" Inherits="ConLib_SetWishlistPassword" %>
<%--
<conlib>
<summary>Displays a form using that a customer can set password for his wishlist.</summary>
</conlib>
--%>
<ajax:UpdatePanel ID="SetPasswordAjax" runat="server">
    <ContentTemplate>
        <asp:TextBox ID="WishlistPasswordValue" runat="server" Columns="5" Width="90px" onFocus="this.select()" MaxLength="30"></asp:TextBox>
        <asp:LinkButton ID="SavePasswordButton" runat="server" Text="Update" SkinID="Button" OnClick="SavePasswordButton_Click"></asp:LinkButton>
        <asp:PlaceHolder ID="UpdatedPanel" runat="server" Visible="False">
            <asp:Label ID="UpdatedMessage" runat="server" Text="<br />Password updated." SkinID="GoodCondition"></asp:Label>
        </asp:PlaceHolder>
    </ContentTemplate>
</ajax:UpdatePanel>