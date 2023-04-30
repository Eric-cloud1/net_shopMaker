<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CheckoutRegisterButtonDialog.ascx.cs" Inherits="ConLib_CheckoutRegisterButtonDialog" %>
<%--
<conlib>
<summary>Displays a register button for anonymous users while checkout process.</summary>
</conlib>
--%>
<div class="dialogSection">
    <div class="header">
        <h2><asp:Localize ID="Caption" runat="server" Text="New Customers"></asp:Localize></h2>
    </div>
    <div class="content nofooter">
        <table class="inputForm" cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <asp:Label ID="InstructionText" runat="server" EnableViewState="False" Text="If this is your first purchase from {0}, click Continue to proceed."></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <br /><asp:Button ID="RegisterButton" runat="server" Text="Continue" OnClick="RegisterButton_Click" />
                </td>
            </tr>
        </table>
    </div>
</div>