<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="EditGatewayTemplate.aspx.cs"
    Inherits="Admin_Payment_EditGatewayTemplate" Title="Edit Gateway" %>

<asp:Content ID="Content" ContentPlaceHolderID="MainContent" runat="Server">
    <ajax:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="pageHeader">
                <div class="caption">
                    <h1>
                        <asp:Localize ID="Caption" runat="server" Text="Gateway Template Payment Methods for {0}"></asp:Localize></h1>
                </div>
            </div>
            <table cellpadding="2" cellspacing="0" class="innerLayout">
                <tr>
                    <td>
                        <asp:Label ID="SavedMessage" runat="server" Text="Template saved at {0}." EnableViewState="false"
                            Visible="false" SkinID="GoodCondition"></asp:Label>
                        <asp:Label ID="ErrorMessageLabel" runat="server" Text="" EnableViewState="false"
                            Visible="false" SkinID="ErrorCondition"></asp:Label>
                         <asp:Label ID="ActiveMessage" runat="server" Text="" EnableViewState="false"
                            Visible="false" SkinID="ErrorCondition"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td valign="top" class="itemList">
                        <asp:DropDownList ID="ddlInstrament" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlInstrament_SelectedIndexChanged">
                            <asp:ListItem Value="1" Text="Visa" Selected="True"  />
                            <asp:ListItem Value="2" Text="MasterCard" />
                            <asp:ListItem Value="8" Text="Purchase Order" />
                            <asp:ListItem Value="0" Text="Unknown" />
                        </asp:DropDownList>&nbsp;<asp:CheckBox ID="chNotActive" runat="server" selected="true" Text="Show not Active" OnCheckedChanged="Check_Clicked" AutoPostBack="true" /> 
                  
                        <div style="padding-top: 5px;">
                            <asp:GridView ID="grid" Style="padding-left: auto; padding-right: auto;" runat="server"
                                OnRowDataBound="grid_RowDataBound" AutoGenerateColumns="false" 
                                SkinID="PagedList" onrowcommand="grid_RowCommand" >
                                <Columns>
                                    <asp:TemplateField HeaderText="Payment Method">
                                        <ItemTemplate>
                                            <asp:DropDownList runat="server" ID="ddlPM" DataTextField="Name" DataValueField="PaymentMethodId">
                                            </asp:DropDownList>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Allocation" ControlStyle-Width="30px">
                                        <ItemTemplate>
                                            <center>
                                                <asp:TextBox runat="server" ID="tbAllocation" MaxLength="3" Style="text-align: right;"></asp:TextBox>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="DeleteButton" runat="server" CausesValidation="False" CommandName="Del" CommandArgument='<%#Eval("PaymentMethodId")%>' 
                                                SkinID="DeleteIcon" 
                                                AlternateText="Delete" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                        <asp:ImageButton ID="addRow" runat="server" CausesValidation="False" SkinID="AddIcon"
                            AlternateText="Add Row" OnClick="addRow_Click" />
                    </td>
                </tr>
                <tr>
                    <td align="center" style="width: 100%">
                        <asp:ImageButton ID="SaveButton" runat="server" CausesValidation="False" SkinID="SaveIcon"
                            AlternateText="Save" OnClick="SaveButton_Click" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>
