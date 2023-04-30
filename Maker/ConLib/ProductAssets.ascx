<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductAssets.ascx.cs" Inherits="ConLib_ProductAssets" %>
<%--
<conlib>
<summary>Display additional details of assets of a product like digital goods, read me files and license agreements.</summary>
<param name="Caption" default="Additional Details">Possible value can be any string.  Title of the control.</param>
</conlib>
--%>
<asp:PlaceHolder ID="ProductAssetsPanel" runat="server" Visible="false">
    <div class="section">
        <div class="header">
            <h2><asp:Localize ID="CaptionText" runat="server" Text="Additional Details"></asp:Localize></h2>
        </div>
        <div class="Cell">
            <asp:Repeater ID="AssetLinkList" runat="server">
                <ItemTemplate>
                    <a href="<%#Eval("NavigateUrl")%>"><%#Eval("Text")%></a><br />
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:PlaceHolder>