<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AgreementContent.ascx.cs" Inherits="ConLib_AgreementContent" EnableViewState="false" %>
<%-- 
<conlib>
<summary>Display page to show license agreement.</summary>
</conlib>
--%>
<asp:Panel ID="AgreementTextPanel" runat="server" CssClass="AgreementView">
<asp:Literal ID="AgreementText" runat="server"></asp:Literal><br /><br />
<p align="center">
    <asp:HyperLink ID="OKLink" runat="server" Text="Close" SkinID="Button" />
    <asp:HyperLink ID="AcceptLink" runat="server" Text="Accept" SkinID="Button" />&nbsp;&nbsp;&nbsp;
    <asp:HyperLink ID="DeclineLink" runat="server" Text="Decline" SkinID="Button" />
</p>
</asp:Panel>
