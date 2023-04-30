<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditBillAddressPage.ascx.cs" Inherits="ConLib_EditBillAddressPage" %>
<%--
<conlib>
<summary>Displays a form to edit billing address of the customer.</summary>
</conlib>
--%>
<%@ Register Src="~/ConLib/CheckoutProgress.ascx" TagName="CheckoutProgress" TagPrefix="uc" %>
<div class="checkoutPageHeader">
    <uc:CheckoutProgress ID="CheckoutProgress1" runat="server" />
    <h1><asp:Localize ID="Caption" runat="server" Text="Billing Address"></asp:Localize></h1>
    <div class="content">
        <asp:Label ID="InstructionText" runat="server" Text="Enter your billing address below and click continue."></asp:Label>
    </div>
</div>
<ajax:UpdatePanel ID="EditAddressAjax" runat="server">
    <ContentTemplate>        
        <table class="inputForm" cellpadding="3">
            <tr>
                <th class="rowHeader" width="120">
                    <asp:Label ID="FirstNameLabel" runat="server" Text="First Name:" AssociatedControlID="FirstName" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="FirstName" runat="server" EnableViewState="false" MaxLength="30"></asp:TextBox> 
                    <asp:RequiredFieldValidator ID="FirstNameRequired" runat="server" Text="*"
                        ErrorMessage="First name is required." Display="Static" ControlToValidate="FirstName"></asp:RequiredFieldValidator>
                </td>
                <th class="rowHeader">
                    <asp:Label ID="LastNameLabel" runat="server" Text="Last Name:" AssociatedControlID="LastName" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="LastName" runat="server" EnableViewState="false" MaxLength="50"></asp:TextBox> 
                    <asp:RequiredFieldValidator ID="LastNameRequired" runat="server" Text="*"
                        ErrorMessage="Last name is required." Display="Static" ControlToValidate="LastName"
                        EnableViewState="false"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <th class="rowHeader" width="120">
                    <asp:Label ID="CompanyLabel" runat="server" Text="Company:" AssociatedControlID="Company" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="Company" runat="server" EnableViewState="false" MaxLength="50"></asp:TextBox> 
                </td>
                <th class="rowHeader">
                    <asp:Label ID="PhoneLabel" runat="server" Text="Phone:" AssociatedControlID="Phone" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="Phone" runat="server" EnableViewState="false" MaxLength="30"></asp:TextBox> 
                </td>
            </tr>
            <tr>
            <th class="rowHeader">
                    <asp:Label ID="Address1Label" runat="server" Text="Street Address 1:" AssociatedControlID="Address1" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="Address1" runat="server" EnableViewState="false" MaxLength="100"></asp:TextBox> 
                    <asp:RequiredFieldValidator ID="Address1Required" runat="server" Text="*"
                        ErrorMessage="Address is required." Display="Static" ControlToValidate="Address1"
                        EnableViewState="false"></asp:RequiredFieldValidator>
                </td>
                <th class="rowHeader" width="120">
                    <asp:Label ID="Address2Label" runat="server" Text="Street Address 2:" AssociatedControlID="Address2" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="Address2" runat="server" EnableViewState="false" MaxLength="100"></asp:TextBox> 
                </td>
                
            </tr>
            <tr>
            <th class="rowHeader">
                    <asp:Label ID="CityLabel" runat="server" Text="City:" AssociatedControlID="City" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="City" runat="server" EnableViewState="false" MaxLength="50"></asp:TextBox> 
                    <asp:RequiredFieldValidator ID="CityRequired" runat="server" Text="*"
                        ErrorMessage="City is required." Display="Static" ControlToValidate="City"
                        EnableViewState="false"></asp:RequiredFieldValidator>
                </td>
                <th class="rowHeader" width="120">
                    <asp:Label ID="ProvinceLabel" runat="server" Text="State / Province:" AssociatedControlID="Province" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="Province" runat="server" MaxLength="30"></asp:TextBox> 
                    <asp:DropDownList ID="Province2" runat="server"></asp:DropDownList>
                    <asp:CustomValidator ID="Province2Invalid" runat="server" Text="*"
                        ErrorMessage="The state or province you entered was not recognized.  Please choose from the list." Display="Dynamic" ControlToValidate="Province2"></asp:CustomValidator>
                    <asp:RequiredFieldValidator ID="Province2Required" runat="server" Text="*"
                        ErrorMessage="State or province is required." Display="Static" ControlToValidate="Province2"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
            <th class="rowHeader">
                    <asp:Label ID="PostalCodeLabel" runat="server" Text="ZIP / Postal Code:" AssociatedControlID="PostalCode" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="PostalCode" runat="server" EnableViewState="false" MaxLength="10"></asp:TextBox> 
                    <asp:RequiredFieldValidator ID="PostalCodeRequired" runat="server" Text="*"
                        ErrorMessage="ZIP or Postal Code is required." Display="Static" ControlToValidate="PostalCode"></asp:RequiredFieldValidator>
                </td>
                <th class="rowHeader" width="120">
                    <asp:Label ID="CountryLabel" runat="server" Text="Country:" AssociatedControlID="Country" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:DropDownList ID="Country" runat="server" DataTextField="Name" DataValueField="CountryCode" 
                        OnSelectedIndexChanged="Country_Changed" AutoPostBack="true"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <th class="rowHeader" valign="top" width="120">
                    <asp:Label ID="FaxLabel" runat="server" Text="Fax:" AssociatedControlID="Fax" EnableViewState="false"></asp:Label>
                </th>
                <td valign="top">
                    <asp:TextBox ID="Fax" runat="server" EnableViewState="false" MaxLength="30"></asp:TextBox> 
                </td>
                <th class="rowHeader" valign="top">
                    <asp:Label ID="ResidenceLabel" runat="server" Text="Type:" AssociatedControlID="Residence" EnableViewState="false"></asp:Label>
                </th>
                <td valign="top">
                    <asp:DropDownList ID="Residence" runat="server" EnableViewState="false">
                        <asp:ListItem Text="This is a residence" Value="1" Selected="true"></asp:ListItem>
                        <asp:ListItem Text="This is a business" Value="0"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td colspan="3">
                    <asp:ValidationSummary ID="EditValidationSummary" runat="server" EnableViewState="false" />
                    <asp:LinkButton ID="ContinueButton" runat="server" Text="Continue" OnClick="ContinueButton_Click" SkinID="Button"></asp:LinkButton>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</ajax:updatePanel>
