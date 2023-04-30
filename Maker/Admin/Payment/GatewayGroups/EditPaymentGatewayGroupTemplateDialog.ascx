<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditPaymentGatewayGroupTemplateDialog.ascx.cs" Inherits="Admin_Payment_GatewayGroups_EditPaymentGatewayGroupTemplateDialog" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>

    
    
<table cellpadding="4" cellspacing="0" class="inputForm" width="100%">
    
    <tr>
        <th class="rowHeader">
            <cb:ToolTipLabel ID="NameLabel" runat="server" Text="Name:" AssociatedControlID="Group" ToolTip="Group Name of the payment method" />
        </th>
        <td>
            <asp:TextBox ID="Group" runat="server" MaxLength="100"></asp:TextBox>
            <asp:RequiredFieldValidator ID="NameRequired" runat="server" Text="*" Display="Static" ErrorMessage="Group Name is required." ControlToValidate="Group" ValidationGroup="AddDialog"></asp:RequiredFieldValidator>
        </td>
    </tr>
    
    <tr>
        <td colspan="2">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="AddPaymentMethod" />
            <asp:Label ID="AddedMessage" runat="server" Text="Payment method {0} added." SkinID="GoodCondition" EnableViewState="false" Visible="false"></asp:Label>
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
        <td class="dialogSubmit">            
            <asp:LinkButton ID="SaveButton" runat="server" OnClick="SaveButton_Click" SkinID="Button" Text="Save" ValidationGroup="EditPaymentMethod"></asp:LinkButton>
			<asp:LinkButton ID="CancelButton" runat="server" OnClick="CancelButton_Click" SkinID="Button" Text="Cancel" ValidationGroup="EditPaymentMethod" CausesValidation="false"></asp:LinkButton>
        </td>
    </tr>
</table>

