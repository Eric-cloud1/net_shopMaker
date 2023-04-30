<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditGatewayFailoverDialog.ascx.cs" Inherits="Admin_Payment_EditGatewayFailoverDialog" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<table cellpadding="4" cellspacing="0" class="inputForm" width="100%">
    
    <tr>
        <th class="rowHeader">
            <cb:ToolTipLabel ID="sourceLabel" runat="server" Text="Source:" AssociatedControlID="ddlSource" ToolTip="Name of the source payment gateway" />
        </th>
        <td>
           <asp:DropDownList ID="ddlSource" runat="server" ></asp:DropDownList>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <cb:ToolTipLabel ID="ToolTipLabel1" runat="server" Text="Destination:" AssociatedControlID="ddlDestination" ToolTip="Name of the destination payment gateway" />
        </th>
        <td>
           <asp:DropDownList ID="ddlDestination" runat="server" ></asp:DropDownList>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <cb:ToolTipLabel ID="ToolTipLabel2" runat="server" Text="2-Way:" AssociatedControlID="cb2way" ToolTip="If the source should be the failover for the destination" />
        </th>
        <td>
          <asp:CheckBox runat="server" ID="cb2way" />
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