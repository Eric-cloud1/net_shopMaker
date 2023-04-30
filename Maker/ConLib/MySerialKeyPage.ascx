<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MySerialKeyPage.ascx.cs" Inherits="ConLib_MySerialKeyPage" EnableViewState="false" %>
<%--
<conlib>
<summary>Displays serial key for a digital good.</summary>
</conlib>
--%>
<div>
    <h1><asp:Localize ID="Caption" runat="server" Text="Serial Key for Digital Good {0}"></asp:Localize></h1>        
</div>
<div style="width:100%;overflow:scroll;">
    <asp:Literal ID="SerialKeyData" runat="server" Text=""></asp:Literal>
</div>