<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditGatewayResponseDialog.ascx.cs" Inherits="Admin_Payment_EditGatewayResponseDialog" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<table cellpadding="4" cellspacing="0" class="inputForm" width="100%">
    
       <tr>
        <th class="rowHeader">
            <cb:ToolTipLabel ID="responseLabel" runat="server" Text="Response:" AssociatedControlID="response"
                ToolTip="Response gateway" />
        </th>
        <td><asp:TextBox ID="response" runat="server" />
          
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <cb:ToolTipLabel ID="subscriptionLabel" runat="server" Text="Subscription Status:" AssociatedControlID="ddlSubscriptionStatus"
                ToolTip="Subscription Status Code" />
        </th>
        <td>
            <asp:DropDownList ID="ddlSubscriptionStatus" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <cb:ToolTipLabel ID="typeLabel" runat="server" Text="Type:" AssociatedControlID="cbType"
                ToolTip="Gateway response type." />
        </th>
        <td>
        <asp:CheckBoxList runat="server" ID="cbType" RepeatDirection="Vertical">
        <asp:ListItem Value="1">Cancel</asp:ListItem>
        <asp:ListItem Value="1">Decline</asp:ListItem>
        <asp:ListItem Value="1">Fraud</asp:ListItem>
        </asp:CheckBoxList>      
        </td>
    </tr>
    
    <tr>
        <td colspan="2">
            <asp:Label ID="ErrorMessage" runat="server" SkinID="ErrorCondition" EnableViewState="false"
                Visible="false"></asp:Label>
            <asp:Label ID="AddedMessage" runat="server" Text="Gateway Failover {0} added." SkinID="GoodCondition"
                EnableViewState="false" Visible="false"></asp:Label>        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
        <td class="dialogSubmit">            
            <asp:LinkButton ID="SaveButton" runat="server" OnClick="SaveButton_Click" SkinID="Button" Text="Save" ValidationGroup="EditPaymentMethod"></asp:LinkButton>
			<asp:LinkButton ID="CancelButton" runat="server" OnClick="CancelButton_Click" SkinID="Button" Text="Cancel" ValidationGroup="EditPaymentMethod" CausesValidation="false"></asp:LinkButton>
        </td>
    </tr>
</table>