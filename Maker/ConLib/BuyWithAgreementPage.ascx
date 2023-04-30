<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BuyWithAgreementPage.ascx.cs" Inherits="ConLib_BuyWithAgreementPage" %>
<%--
<conlib>
<summary>Page displays the license agreement details.</summary>
</conlib>
--%>
<asp:Label ID="AgreementText" runat="server" EnableViewState="false"></asp:Label><br /><br />
<div align="center">
<asp:Localize ID="InstructionText" runat="server" Text="You must accept the license agreement to continue." EnableViewState="false"></asp:Localize><br /><br />
<asp:Button ID="AcceptButton" runat="server" Text="Accept" OnClick="AcceptButton_Click" EnableViewState="false" />
&nbsp;<asp:Button ID="DeclineButton" runat="server" Text="Decline" OnClick="DeclineButton_Click" EnableViewState="false" />
</div>