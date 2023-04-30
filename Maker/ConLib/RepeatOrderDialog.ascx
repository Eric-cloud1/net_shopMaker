<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RepeatOrderDialog.ascx.cs" Inherits="ConLib_RepeatOrderDialog" EnableViewState="false" %>
<%--
<conlib>
<summary>A control which have a reorder button, that can be used to reorder the contents of an existing order.</summary>
<param name="Caption" default="Repeat Order">Possible value can be any string.  Title of the control.</param>
</conlib>
--%>
<asp:PlaceHolder ID="phRepeatable" runat="server">
    <div class="section">
        <div class="header">
            <h2><asp:Localize ID="phCaption" runat="server" Text="Repeat Order"></asp:Localize></h2>
        </div>
        <div class="Cell">
            <p align="justify"><asp:Label ID="phInstructionText" runat="server" Text="Click the reorder button below to add all items from this order into your basket."></asp:Label></p><br />
            <p align="center"><asp:HyperLink ID="ReorderButton" runat="server" Text="Reorder" SkinID="Button" /></p>
        </div>
    </div>
</asp:PlaceHolder>