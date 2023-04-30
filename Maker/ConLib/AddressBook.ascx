<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddressBook.ascx.cs" Inherits="ConLib_AddressBook" EnableViewState="false" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<!-- STARTABOOK -->
<ajax:UpdatePanel ID="ManageStoreAjax" runat="server">
    <ContentTemplate>
        <asp:Button ID="AddButton" runat="server" Text="Add" CausesValidation="false" OnClick="AddButton_Click" /><br /><br />
        <asp:Repeater ID="A" runat="server" OnItemCommand="A_ItemCommand">
            <ItemTemplate>
                <asp:Literal ID="AddressLiteral" runat="server" Text='<%#Container.DataItem.ToString()%>'></asp:Literal><br />
                <asp:Button ID="E" runat="server" Text="Edit" CommandName="E" CommandArgument='<%#Eval("AddressId")%>' CausesValidation="false"></asp:Button>
                <asp:Button ID="D" runat="server" Text="Delete" CommandName="D" CommandArgument='<%#Eval("AddressId")%>' Visible='<%#Container.ItemIndex != 0%>' OnClientClick="return confirm('Are you sure you want to delete this address?');"></asp:Button>
            </ItemTemplate>
            <SeparatorTemplate><br /><br /></SeparatorTemplate>
        </asp:Repeater>
        <asp:HiddenField ID="VS" runat="server" />
        <asp:Button ID="DummyOK" runat="server" style="display:none" />
        <asp:Button ID="DummyCancel" runat="server" style="display:none" />
        <ajaxToolkit:ModalPopupExtender runat="server" ID="AddPanelPopup"
            TargetControlID="DummyOK"
            PopupControlID="AddPanel"
            BackgroundCssClass="modalBackground"
            DropShadow="True"
            PopupDragHandleControlID="AddHeader"
            CancelControlID="DummyCancel">
        </ajaxToolkit:ModalPopupExtender>
        <%-- this div prevents the screen from dynamically resizing when the popup shows --%>
        <div style="overflow:hidden;width:1px;height:1px;">
        <asp:Panel ID="AddPanel" runat="server" CssClass="modalPopup" Width="500">
            <asp:Panel ID="AddInnerPanel" runat="server" CssClass="section">
                <div class="pageHeader" id="AddHeader" runat="server">
                    <div class="caption">
                        <h1>
                            <asp:Localize ID="AddCaption" runat="server" Text="Add Address" />
                            <asp:Localize ID="EditCaption" runat="server" Text="Edit Address" />
                            <asp:Localize ID="EditBillingCaption" runat="server" Text="Edit Billing Address" Visible="false" />
                        </h1>
                    </div>
                </div>
                <asp:PlaceHolder ID="FormPanel" runat="server" Visible="false">
                    <asp:ValidationSummary ID="AddValidationSummary" runat="server" ShowMessageBox="false" ShowSummary="true" ValidationGroup="AddrBook" />
                    <table align="center">
                        <tr>
                            <td>
                                <table class="inputForm" cellpadding="3">
                                    <tr>
                                        <th class="rowHeader" style="width:140px">
                                            <asp:Label ID="FirstNameLabel" runat="server" Text="First Name:" AssociatedControlID="FirstName"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:TextBox ID="FirstName" runat="server" MaxLength="30"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="FirstNameRequired" runat="server" Text="*"
                                                ErrorMessage="First name is required." Display="Static" ControlToValidate="FirstName" ValidationGroup="AddrBook"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader">
                                            <asp:Label ID="LastNameLabel" runat="server" Text="Last Name:" AssociatedControlID="LastName"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:TextBox ID="LastName" runat="server" MaxLength="50"></asp:TextBox>&nbsp;
                                            <asp:RequiredFieldValidator ID="LastNameRequired" runat="server" Text="*"
                                                ErrorMessage="Last name is required." Display="Static" ControlToValidate="LastName" ValidationGroup="AddrBook"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader" style="width:140px">
                                            <asp:Label ID="CompanyLabel" runat="server" Text="Company:" AssociatedControlID="Company"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:TextBox ID="Company" runat="server" MaxLength="50"></asp:TextBox><br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader">
                                            <asp:Label ID="Address1Label" runat="server" Text="Street Address 1:" AssociatedControlID="Address1"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:TextBox ID="Address1" runat="server" MaxLength="100"></asp:TextBox> 
                                            <asp:RequiredFieldValidator ID="Address1Required" runat="server" Text="*"
                                                ErrorMessage="Address is required." Display="Static" ControlToValidate="Address1" ValidationGroup="AddrBook"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader" style="width:140px">
                                            <asp:Label ID="Address2Label" runat="server" Text="Street Address 2:" AssociatedControlID="Address2"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:TextBox ID="Address2" runat="server" MaxLength="100"></asp:TextBox> 
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader">
                                            <asp:Label ID="CityLabel" runat="server" Text="City:" AssociatedControlID="City"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:TextBox ID="City" runat="server" MaxLength="50"></asp:TextBox> 
                                            <asp:RequiredFieldValidator ID="CityRequired" runat="server" Text="*"
                                                ErrorMessage="City is required." Display="Static" ControlToValidate="City" ValidationGroup="AddrBook"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader" style="width:140px">
                                            <asp:Label ID="ProvinceLabel" runat="server" Text="State / Province:" AssociatedControlID="Province"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:TextBox ID="Province" runat="server" MaxLength="50"></asp:TextBox> 
                                            <asp:DropDownList ID="Province2" runat="server"></asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="ProvinceRequired" runat="server" Text="*"
                                                ErrorMessage="State or province is required." Display="Dynamic" ControlToValidate="Province" ValidationGroup="AddrBook"></asp:RequiredFieldValidator>
                                            <asp:RequiredFieldValidator ID="Province2Required" runat="server" Text="*"
                                                ErrorMessage="State or province is required." Display="Static" ControlToValidate="Province2" ValidationGroup="AddrBook"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader">
                                            <asp:Label ID="PostalCodeLabel" runat="server" Text="ZIP / Postal Code:" AssociatedControlID="PostalCode"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:TextBox ID="PostalCode" runat="server" MaxLength="10"></asp:TextBox> 
                                            <asp:RequiredFieldValidator ID="PostalCodeRequired" runat="server" Text="*"
                                                ErrorMessage="ZIP or Postal Code is required." Display="Static" ControlToValidate="PostalCode" ValidationGroup="AddrBook"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="USPostalCodeRequired" runat="server" Text="*" ValidationExpression="^\d{5}(-?\d{4})?$"
                                                ErrorMessage="ZIP code should be in the format of ######(-####)." Display="Static" ControlToValidate="PostalCode" ValidationGroup="AddrBook"></asp:RegularExpressionValidator>
                                            <asp:RegularExpressionValidator ID="CAPostalCodeRequired" runat="server" Text="*" ValidationExpression="^[A-Za-z][0-9][A-Za-z] ?[0-9][A-Za-z][0-9]$"
                                                ErrorMessage="Postal code should be in the format of A#A #A#." Display="Static" ControlToValidate="PostalCode" ValidationGroup="AddrBook"></asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader" style="width:140px">
                                            <asp:Label ID="CountryLabel" runat="server" Text="Country:" AssociatedControlID="Country"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:DropDownList ID="Country" runat="server" DataTextField="Name" DataValueField="CountryCode" AutoPostBack="true" OnSelectedIndexChanged="CountryChanged"></asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader">
                                            <asp:Label ID="PhoneLabel" runat="server" Text="Phone:" AssociatedControlID="Phone"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:TextBox ID="Phone" runat="server" MaxLength="30"></asp:TextBox> 
                                            <asp:RequiredFieldValidator ID="PhoneRequired" runat="server" Text="*"
                                                ErrorMessage="Phone number is required." Display="Static" ControlToValidate="Phone" ValidationGroup="AddrBook"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader" style="width:140px">
                                            <asp:Label ID="FaxLabel" runat="server" Text="Fax:" AssociatedControlID="Fax"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:TextBox ID="Fax" runat="server" MaxLength="30"></asp:TextBox> 
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td>
                                        <td><asp:CheckBox ID="IsBusiness" runat="server" Text="Check here if this is a business address." /></td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td>
                                        <td>
                                            <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ValidationGroup="AddrBook"></asp:Button>
                                            &nbsp;<asp:Button ID="CancelButton" runat="server" Text="Cancel" CausesValidation="false" OnClick="CancelButton_Click"></asp:Button>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:PlaceHolder>
            </asp:Panel>
        </asp:Panel>
        </div>
    </ContentTemplate>
</ajax:UpdatePanel>
<!-- ENDABOOK -->