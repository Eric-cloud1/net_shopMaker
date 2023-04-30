<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReadmeContent.ascx.cs" Inherits="ConLib_ReadmeContent" EnableViewState="false" %>
<%--
<conlib>
<summary>Display contents of a read me file.</summary>
</conlib>
--%>
<asp:Panel ID="ReadmeTextPanel" runat="server" CssClass="AgreementView">
<asp:Literal ID="ReadmeText" runat="server"></asp:Literal><br /><br />
<p align="center">
    <asp:HyperLink ID="OkButton" runat="server" Text="Close" SkinID="Button" />
</p>
</asp:Panel>
