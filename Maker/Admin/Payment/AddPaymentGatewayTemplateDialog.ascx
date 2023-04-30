<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddPaymentGatewayTemplateDialog.ascx.cs"
    Inherits="Admin_Payment_AddPaymentGatewayTemplateDialog" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls"
    TagPrefix="cb" %>
<asp:Label ID="AddedMessage" runat="server" Text="Payment method {0} added." SkinID="GoodCondition"
    EnableViewState="false" Visible="false"></asp:Label>
<table cellpadding="4" cellspacing="0" class="inputForm" width="100%">

    <tr>
        <th class="rowHeader">
            <cb:ToolTipLabel ID="NameLabel" runat="server" Text="Name:" AssociatedControlID="Name"
                ToolTip="Name of the payment method" />
        </th>
        <td>
            <asp:TextBox ID="Name" runat="server" MaxLength="100"></asp:TextBox>
            <asp:RequiredFieldValidator ID="NameRequired" runat="server" Text="*" Display="Static"
                ErrorMessage="Name is required." ControlToValidate="Name" ValidationGroup="AddDialog"></asp:RequiredFieldValidator>
        </td>
    </tr>
    
    <tr>
        <td colspan="2">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="AddDialog" />
            <asp:Label ID="Label1" runat="server" Text="Tax rule {0} added." SkinID="GoodCondition"
                EnableViewState="false" Visible="false"></asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            &nbsp;
        </td>
        <td>
            <asp:Button ID="AddButton" runat="server" Text="Add" OnClick="AddButton_Click" ValidationGroup="AddDialog" />
        </td>
    </tr>
</table>
