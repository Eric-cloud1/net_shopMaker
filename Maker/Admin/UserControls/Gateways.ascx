<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Gateways.ascx.cs" Inherits="Admin_UserControls_Gateways" %>
<ajax:UpdatePanel runat="server" ID="upGateways">
    <ContentTemplate>
        <table cellpadding="2" cellspacing="0" class="innerLayout">
            <tr>
                <td >
                    <cb:ToolTipLabel ID="ttActive" runat="server" Text="Show InActive:"  SkinID="FieldHeader" ToolTip="Select this if you want to view inactive payment gateways.">
                    </cb:ToolTipLabel>
                    </td><td>
                    <asp:CheckBox AutoPostBack="true" ID="cbShowInActive" runat="server" Checked="False"
                        OnCheckedChanged="cbShowInActive_CheckedChanged" />
                </td>
            </tr>
            <tr runat="server" id="rowGateways">
                <td align="left" valign="top">
                    <asp:Label ID="Label8" runat="server" Text="Gateways:" SkinID="FieldHeader"></asp:Label>
                </td>
                <td align="left" valign="top">
                    <asp:DropDownList ID="GatewayNames" runat="server" AutoPostBack="true" OnSelectedIndexChanged="GatewayNames_SelectedIndexChanged"
                        Width="100%">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr runat="server" id="rowPaymentGateways">
                <td align="left" valign="top">
                    <asp:Label ID="Label1" runat="server" Text="Payment Gateways:" SkinID="FieldHeader"></asp:Label>
                </td>
                <td align="left" valign="top">
                    <asp:DropDownList ID="PaymentGateways" runat="server" Width="100%">
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</ajax:UpdatePanel>
