<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditShipAddressPage.ascx.cs" Inherits="ConLib_EditShipAddressPage" EnableViewState="true" %>
<%--
<conlib>
<summary>Displays a form to add/edit customer shipping address.</summary>
</conlib>
--%>
<%@ Register Src="~/ConLib/CheckoutProgress.ascx" TagName="CheckoutProgress" TagPrefix="uc" %>
<div class="checkoutPageHeader">
    <uc:CheckoutProgress ID="CheckoutProgress2" runat="server" />
    <h1>
        <asp:Localize ID="AddAddressCaption" runat="server" Text="New Shipping Address"></asp:Localize>
        <asp:Localize ID="EditAddressCaption" runat="server" Text="Edit Shipping Address"></asp:Localize>
    </h1>
</div>
<ajax:UpdatePanel ID="EditAddressAjax" runat="server">
    <ContentTemplate>        
        <table class="inputForm" cellpadding="3">
            <tr>
                <th class="rowHeader" width="120">
                    <asp:Label ID="ShipToFirstNameLabel" runat="server" Text="First Name:" AssociatedControlID="ShipToFirstName" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="ShipToFirstName" runat="server" EnableViewState="false" MaxLength="30"></asp:TextBox> 
                    <asp:RequiredFieldValidator ID="ShipToFirstNameRequired" runat="server" Text="*"
                        ErrorMessage="First name is required." Display="Static" ControlToValidate="ShipToFirstName"></asp:RequiredFieldValidator>
                </td>
                <th class="rowHeader">
                    <asp:Label ID="ShipToLastNameLabel" runat="server" Text="Last Name:" AssociatedControlID="ShipToLastName" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="ShipToLastName" runat="server" EnableViewState="false" MaxLength="50"></asp:TextBox> 
                    <asp:RequiredFieldValidator ID="ShipToLastNameRequired" runat="server" Text="*"
                        ErrorMessage="Last name is required." Display="Static" ControlToValidate="ShipToLastName"
                        EnableViewState="false"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <th class="rowHeader" width="120">
                    <asp:Label ID="ShipToCompanyLabel" runat="server" Text="Company:" AssociatedControlID="ShipToCompany" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="ShipToCompany" runat="server" EnableViewState="false" MaxLength="50"></asp:TextBox> 
                </td>
                <th class="rowHeader">
                    <asp:Label ID="ShipToPhoneLabel" runat="server" Text="Phone:" AssociatedControlID="ShipToPhone" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="ShipToPhone" runat="server" EnableViewState="false" MaxLength="30"></asp:TextBox> 
                </td>
                
            </tr>
            <tr>
            <th class="rowHeader">
                    <asp:Label ID="ShipToAddress1Label" runat="server" Text="Street Address 1:" AssociatedControlID="ShipToAddress1" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="ShipToAddress1" runat="server" EnableViewState="false" MaxLength="100"></asp:TextBox> 
                    <asp:RequiredFieldValidator ID="ShipToAddress1Required" runat="server" Text="*"
                        ErrorMessage="Address is required." Display="Static" ControlToValidate="ShipToAddress1"
                        EnableViewState="false"></asp:RequiredFieldValidator>
                </td>
                <th class="rowHeader" width="120">
                    <asp:Label ID="ShipToAddress2Label" runat="server" Text="Street Address 2:" AssociatedControlID="ShipToAddress2" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="ShipToAddress2" runat="server" EnableViewState="false" MaxLength="100"></asp:TextBox> 
                </td>
                
            </tr>
            <tr>
            <th class="rowHeader">
                    <asp:Label ID="ShipToCityLabel" runat="server" Text="City:" AssociatedControlID="ShipToCity" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="ShipToCity" runat="server" EnableViewState="false" MaxLength="50"></asp:TextBox> 
                    <asp:RequiredFieldValidator ID="ShipToCityRequired" runat="server" Text="*"
                        ErrorMessage="City is required." Display="Static" ControlToValidate="ShipToCity"
                        EnableViewState="false"></asp:RequiredFieldValidator>
                </td>
                <th class="rowHeader" width="120">
                    <asp:Label ID="ShipToProvinceLabel" runat="server" Text="State / Province:" AssociatedControlID="ShipToProvince" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="ShipToProvince" runat="server" MaxLength="50"></asp:TextBox> 
                    <asp:DropDownList ID="ShipToProvince2" runat="server"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="ShipToProvince2Required" runat="server" Text="*"
                        ErrorMessage="State or province is required." Display="Static" ControlToValidate="ShipToProvince2"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
            
                <th class="rowHeader">
                    <asp:Label ID="ShipToPostalCodeLabel" runat="server" Text="ZIP / Postal Code:" AssociatedControlID="ShipToPostalCode" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="ShipToPostalCode" runat="server" EnableViewState="false" MaxLength="10"></asp:TextBox> 
                    <asp:RequiredFieldValidator ID="ShipToPostalCodeRequired" runat="server" Text="*"
                        ErrorMessage="ZIP or Postal Code is required." Display="Static" ControlToValidate="ShipToPostalCode"></asp:RequiredFieldValidator>
                </td>
                <th class="rowHeader" width="120">
                    <asp:Label ID="ShipToCountryLabel" runat="server" Text="Country:" AssociatedControlID="ShipToCountry" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:DropDownList ID="ShipToCountry" runat="server" DataTextField="Name" DataValueField="CountryCode" 
                        OnSelectedIndexChanged="ShipToCountry_Changed" AutoPostBack="true" EnableViewState="false"></asp:DropDownList>
                </td>
                
            </tr>
            <tr>
                <th class="rowHeader" valign="top" width="120">
                    <asp:Label ID="ShipToFaxLabel" runat="server" Text="Fax:" AssociatedControlID="ShipToFax" EnableViewState="false"></asp:Label>
                </th>
                <td valign="top">
                    <asp:TextBox ID="ShipToFax" runat="server" EnableViewState="false" MaxLength="30"></asp:TextBox> 
                </td>
                <th class="rowHeader" valign="top">
                    <asp:Label ID="ShipToResidenceLabel" runat="server" Text="Type:" AssociatedControlID="ShipToResidence" EnableViewState="false"></asp:Label>
                </th>
                <td valign="top">
                    <asp:DropDownList ID="ShipToResidence" runat="server" EnableViewState="false">
                        <asp:ListItem Text="This is a residence" Value="1" Selected="true"></asp:ListItem>
                        <asp:ListItem Text="This is a business" Value="0"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td colspan="3">
                    <asp:ValidationSummary ID="EditShipToValidationSummary" runat="server" EnableViewState="false" />
                    <asp:Button ID="EditShipToCancelButton" runat="server" Text="Cancel" OnClick="EditShipToCancelButton_Click" CausesValidation="false" EnableViewState="false"></asp:Button>
                    <asp:Button ID="EditShipToSaveButton" runat="server" Text="Save" OnClick="EditShipToSaveButton_Click"></asp:Button>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</ajax:updatePanel>