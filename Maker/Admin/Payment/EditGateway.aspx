<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="EditGateway.aspx.cs"
    Inherits="Admin_Payment_EditGateway" Title="Edit Gateway" %>

<asp:Content ID="Content" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1>
                <asp:Localize ID="Caption" runat="server" Text="Edit Gateway"></asp:Localize></h1>
        </div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td>
                <div class="rowHeader" style="margin-left: auto; margin-right: auto; width: 100%;
                    text-align: center;">
                    <cb:ToolTipLabel ID="tt3D" runat="server" Text="3D:" ToolTip="This notifys the system that the gateway uses 3D. This means we host the Credit Card Page.">
                    </cb:ToolTipLabel>
                    <asp:CheckBox ID="Is3D" runat="server" Checked='false' />
                    <cb:ToolTipLabel ID="ttPaymentPageHosted" runat="server" Text="Payment Page Hosted:"
                        ToolTip="This notifys the system that the Credit Card Page is hosted by the gateway.">
                    </cb:ToolTipLabel>
                    <asp:CheckBox ID="IsPaymentPageHosted" runat="server" Checked='false' />
                    <cb:ToolTipLabel ID="ttPaymentPageIFrame" runat="server" Text="Payment Page IFrame:"
                        ToolTip="This notifys the system that the Credit Card Page is an IFrame of the gatway.">
                    </cb:ToolTipLabel>
                    <asp:CheckBox ID="IsPaymentPageIFrame" runat="server" Checked='false' />
                    <cb:ToolTipLabel ID="ttAsynchronous" runat="server" Text="Asynchronous:" ToolTip="This notifys the system that the gatway funchions asynchronously. This would imply that we host the creditcard page but the response still comes back asynchronously so the client will have to poll for a response.">
                    </cb:ToolTipLabel>
                    <asp:CheckBox ID="IsAsynchronous" runat="server" Checked='false' />
                    <cb:ToolTipLabel ID="ttBlockEmail" runat="server" Text="Block Emails:" ToolTip="This notifys the system that the gatway emails so emails from the system should be blocked.">
                    </cb:ToolTipLabel>
                    <asp:CheckBox ID="BlockEmail" runat="server" Checked='false' />
                    <cb:ToolTipLabel ID="ttActive" runat="server" Text="Active:" ToolTip="This notifys the system that the gateway is Active.">
                    </cb:ToolTipLabel>
                    <asp:CheckBox ID="IsActive" runat="server" Checked='True' />
                </div>
                <asp:PlaceHolder ID="phInputForm" runat="server"></asp:PlaceHolder>
                <asp:Panel ID="PaymentMethodPanel" runat="server">
                    <table class="inputForm" width="100%">
                        <tr class="sectionHeader">
                            <td>
                                <asp:Label ID="PaymentMethodCaption" runat="server" Text="Payment Instruments" SkinID="FieldHeader"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <p style="margin: 4px;">
                                    Select the payment instruments that should use this gateway.</p>
                                <div align="center">
                                    <asp:DataList ID="PaymentMethodList" runat="server" DataKeyField="PaymentInstrumentId"
                                        RepeatDirection="Horizontal" RepeatColumns="2" CellPadding="3" Width="300px">
                                        <ItemTemplate>
                                            <table align="left">
                                                <tr>
                                                    <td>
                                                        <asp:CheckBox ID="Method" runat="server" Text='<%#Eval("PaymentInstrument")%>' Checked='<%#IsMethodAssigned(Container.DataItem)%>' />
                                                    </td>
                                                </tr>
                                            </table>
                                        </ItemTemplate>
                                    </asp:DataList>
                                </div>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <div align="center" style="margin-top: 20px">
                    <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
                    <asp:Button ID="CancelButton" runat="server" Text="Cancel" CausesValidation="false"
                        OnClick="CancelButton_Click" />
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
