<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="Settings.aspx.cs" Inherits="Admin_Taxes_Settings" Title="Tax Settings" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="SettingsCaption" runat="server" Text="Tax Settings"></asp:Localize></h1>
        </div>
    </div>
    <ajax:UpdatePanel id="TaxAjax" runat="server" >
        <ContentTemplate>
            <asp:PlaceHolder ID="TaxesDisabledPanel" runat="server" EnableViewState="false" Visible="false">
                <br />
                <asp:Localize ID="TaxesDisabledMessage" runat="server" EnableViewState="false">
                    WARNING: Taxes are currently disabled and configured tax rules will have no effect.
                </asp:Localize>
                <br /><br />
            </asp:PlaceHolder>
            <asp:Label ID="SavedMessage" runat="server" Text="The configuration has been saved at {0}." SkinID="GoodCondition" Visible="False" EnableViewState="false"></asp:Label>
            <table class="inputForm" cellpadding="4">
                <tr>
                    <th class="rowHeader" width="150px">
                        <asp:Label ID="EnabledLabel" runat="Server" Text="Taxes Enabled:" AssociatedControlID="Enabled" ToolTip="Indicates whether tax processing is enabled for the store"></asp:Label>
                    </th>
                    <td>
                        <asp:DropDownList ID="Enabled" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Enabled_SelectedIndexChanged">
                            <asp:ListItem Text="No"></asp:ListItem>
                            <asp:ListItem Text="Yes"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr id="trShowShopPriceWithTax" runat="server">
                    <th class="rowHeader" valign="top">
                        <asp:Label ID="ShoppingDisplayLabel" runat="Server" Text="Shopping Display:" AssociatedControlID="ShoppingDisplay" ToolTip="Indicates whether prices should be shown to customers with or without tax"></asp:Label>
                    </th>
                    <td valign="top">
                        <asp:Localize ID="ShoppingDisplayHelpText" runat="server">
                            When users are shopping for products, how do you want the taxes to display in the catalog and shopping cart? If you show taxes for all users, anonymous shoppers, without known addresses, have taxes calculated based on your store's default warehouse address.
                        </asp:Localize><br /><br />
                        <asp:DropDownList ID="ShoppingDisplay" runat="server">
                            <asp:ListItem Text="Do not show taxes while shopping"></asp:ListItem>
                            <asp:ListItem Text="Show prices with tax included for all users"></asp:ListItem>
                            <asp:ListItem Text="Show prices with tax included for users with valid addresses"></asp:ListItem>
                            <asp:ListItem Text="Show taxes as separate line items in basket for all users"></asp:ListItem>
                            <asp:ListItem Text="Show taxes as separate line items in basket for users with valid addresses"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr id="trInvoiceDisplay" runat="server">
                    <th class="rowHeader" valign="top">
                        <asp:Label ID="InvoiceDisplayLabel" runat="Server" Text="Invoice Display:" AssociatedControlID="InvoiceDisplay"></asp:Label>
                    </th>
                    <td valign="top">
                        <asp:Localize ID="InvoiceDisplayHelpText" runat="server">
                            When users are checking out or viewing order invoices, how do you want the taxes to display? If you show taxes for all users, anonymous shoppers, without known addresses, have taxes calculated based on your store's default warehouse. Once the user enters an address as part of checkout, any tax will be adjusted accordingly.
                        </asp:Localize><br /><br />
                        <asp:DropDownList ID="InvoiceDisplay" runat="server">
                            <asp:ListItem Text="Show summary tax details only"></asp:ListItem>
                            <asp:ListItem Text="Show prices with tax included for all users"></asp:ListItem>
                            <asp:ListItem Text="Show prices with tax included for users with valid addresses"></asp:ListItem>
                            <asp:ListItem Text="Show taxes as separate line items in order for all users"></asp:ListItem>
                            <asp:ListItem Text="Show taxes as separate line items in order for users with valid addresses"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr id="trShowTaxColumn" runat="server">
                    <th class="rowHeader" valign="top">
                        <asp:Label ID="ShowTaxColumnLabel" runat="Server" Text="Tax Column:" AssociatedControlID="ShowTaxColumn"></asp:Label>
                    </th>
                    <td valign="top">
                        <asp:Localize ID="ShowTaxColumnHelpText" runat="server" Text="If you check the box below, invoice pages will display a column with the tax rate applied to that item."></asp:Localize><br />
                        <asp:CheckBox ID="ShowTaxColumn" runat="server" Text="Show Tax Column" AutoPostBack="true" OnCheckedChanged="ShowTaxColumn_CheckedChanged" /><br />
                    </td>
                </tr>
                <tr id="trTaxColumnHeader" runat="server">
                    <th class="rowHeader" valign="top">
                        <asp:Label ID="TaxColumnHeaderLabel" runat="server" Text="Tax Column Header:" AssociatedControlID="TaxColumnHeader"></asp:Label>
                    </th>
                    <td valign="top">
                        <asp:TextBox ID="TaxColumnHeader" runat="server" MaxLength="15" Width="80px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>
                        <asp:LinkButton ID="SaveButton" runat="server" SkinID="Button" Text="Save" OnClick="SaveButton_Click"></asp:LinkButton>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>