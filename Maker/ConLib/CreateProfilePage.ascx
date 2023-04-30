<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CreateProfilePage.ascx.cs" Inherits="ConLib_CreateProfilePage" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<%--
<conlib>
<summary>Displays a form where customer can enter personal and billing details and can create an account.</summary>
</conlib>
--%>
<div class="checkoutPageHeader">
    <h1><asp:Localize ID="Caption" runat="server" Text="Customer Profile" EnableViewState="false"></asp:Localize></h1>
    <div class="content">
        <asp:Label ID="CustomerProfileHelpText" runat="server" Text="Please take a moment to fill in your information below, then click Continue to proceed with checkout." Font-Bold="true" EnableViewState="false"></asp:Label>
    </div>
</div>
<div class="section">
    <div class="header">
        <h2><asp:Localize ID="CreateAccountCaption" runat="server" Text="Create Account" EnableViewState="false"></asp:Localize></h2>
    </div>
    <div class="content">
        <table class="inputForm">
            <tr>
                <th class="rowHeader" valign="top" rowspan="2" width="120">
                    <asp:Label ID="UserNameLabel" runat="server" Text="Email:" AssociatedControlID="UserName" EnableViewState="false"></asp:Label>
                    <div style="font-size:smaller">(This will be your username.)</div>
                </th>
                <td nowrap>
                    <asp:TextBox ID="UserName" runat="server" Columns="40" EnableViewState="false"></asp:TextBox>
                    <asp:CustomValidator ID="InvalidRegistration" runat="server" ControlToValidate="UserName"
                        ErrorMessage="Registration is invalid." Display="Dynamic" Text="*" EnableViewState="false"></asp:CustomValidator>                    
                    <cb:EmailAddressValidator ID="EmailAddressValidator1" runat="server" ControlToValidate="UserName" Required="true" ErrorMessage="Email address should be in the format of name@domain.tld." Text="*" Display="Dynamic"></cb:EmailAddressValidator>
                </td>
                <td rowspan="2" valign="top">
                    <asp:Localize ID="CreateAccountHelpText" runat="server" Text="Creating an account provides you access to view your order status and gives you great features like wishlists and product reviews.  It's easy and free!" EnableViewState="false"></asp:Localize><br /><br />
                    <asp:Localize ID="LoginHelpText" runat="server" Text="Already have an account?" EnableViewState="false"></asp:Localize>
                    <asp:HyperLink ID="LoginLink" runat="server" Text="Click here to log in." EnableViewState="false"></asp:HyperLink>
                </td>
            </tr>
            <tr>
                <td><asp:CheckBox ID="RememberMe" runat="server" Text="Remember my email address" Checked="true" EnableViewState="false" /></td>
            </tr>
            <tr>
                <th class="rowHeader" width="120">
                    <asp:Label ID="PasswordLabel" runat="server" Text="Password:" AssociatedControlID="Password" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="Password" runat="server" TextMode="password" EnableViewState="false"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="PasswordValidator1" runat="server" ControlToValidate="Password"
                        Display="Static" ErrorMessage="Password is required." Text="*" EnableViewState="false"></asp:RequiredFieldValidator>
                    <asp:PlaceHolder ID="PasswordValidatorPanel" runat="server" EnableViewState="false"></asp:PlaceHolder>
                </td>
                <td rowspan="2" valign="top">
                    <div style="width:210px;text-align:justify">
                    <i><asp:Localize ID="PasswordPolicyLength" runat="server" Text="Your password must be at least {0} characters long."></asp:Localize>
                    <asp:Localize ID="PasswordPolicyRequired" runat="server" Text="You must include at least one {0}."></asp:Localize></i>
                    </div>
                </td>
            </tr>
            <tr>
                <th class="rowHeader" width="120" nowrap>
                    <asp:Label ID="ConfirmPasswordLabel" runat="server" Text="Confirm Password:" AssociatedControlID="ConfirmPassword" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="password" EnableViewState="false"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="ConfirmPasswordValidator1" runat="server" ControlToValidate="ConfirmPassword"
                        Display="Static" ErrorMessage="You must retype your password." Text="*" EnableViewState="false"></asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="ConfirmPasswordValidator2" runat="server" ControlToCompare="Password" ControlToValidate="ConfirmPassword" 
                        Display="Static" ErrorMessage="You did not retype your new password correctly." Text="*" EnableViewState="false"></asp:CompareValidator>
                </td>
            </tr>
        </table>
    </div>
    <div class="header">
        <h2><asp:Localize ID="BillingAddressCaption" runat="server" Text="Billing Address" EnableViewState="false"></asp:Localize></h2>
    </div>
    <ajax:UpdatePanel ID="AddressAjax" runat="server">
        <ContentTemplate>
            <div class="content">                
                <table class="inputForm" cellpadding="3">
                    <tr>
						<th class="rowHeader" width="120">
                            <asp:Label ID="IsShippingLbl" runat="server" Text="Is this is also the shipping address?" AssociatedControlID="rbtnYes" EnableViewState="false"></asp:Label>
                        </th>
                        <td>
                            <asp:RadioButton ID="rbtnYes" runat="server" Checked="True" GroupName="AddOption" Text="Yes" />&nbsp;
							<asp:RadioButton ID="rbtnNo" runat="server" GroupName="AddOption" Text="No" /></td>
                        </td>                        
                    </tr>
                    <tr>                        
                        <th class="rowHeader" width="120">
                            <asp:Label ID="FirstNameLabel" runat="server" Text="First Name:" AssociatedControlID="FirstName" EnableViewState="false"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="FirstName" runat="server" EnableViewState="false"></asp:TextBox> 
                            <asp:RequiredFieldValidator ID="FirstNameRequired" runat="server" Text="*"
                                ErrorMessage="First name is required." Display="Static" ControlToValidate="FirstName"></asp:RequiredFieldValidator>
                        </td>
                        <th class="rowHeader">
                            <asp:Label ID="LastNameLabel" runat="server" Text="Last Name:" AssociatedControlID="LastName" EnableViewState="false"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="LastName" runat="server" EnableViewState="false"></asp:TextBox> 
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
                            <asp:TextBox ID="Company" runat="server" EnableViewState="false"></asp:TextBox> 
                        </td>
                        <th class="rowHeader">
                            <asp:Label ID="PhoneLabel" runat="server" Text="Phone:" AssociatedControlID="Phone" EnableViewState="false"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="Phone" runat="server" EnableViewState="false"></asp:TextBox> 
                        </td>
                        
                        
                    </tr>
                    <tr>
                    <th class="rowHeader">
                            <asp:Label ID="Address1Label" runat="server" Text="Street Address 1:" AssociatedControlID="Address1" EnableViewState="false"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="Address1" runat="server" EnableViewState="false"></asp:TextBox> 
                            <asp:RequiredFieldValidator ID="Address1Required" runat="server" Text="*"
                                ErrorMessage="Address is required." Display="Static" ControlToValidate="Address1"
                                EnableViewState="false"></asp:RequiredFieldValidator>
                        </td>
                        <th class="rowHeader" width="120">
                            <asp:Label ID="Address2Label" runat="server" Text="Street Address 2:" AssociatedControlID="Address2" EnableViewState="false"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="Address2" runat="server" EnableViewState="false"></asp:TextBox> 
                        </td>
                        
                    </tr>
                    <tr>
                    <th class="rowHeader">
                            <asp:Label ID="CityLabel" runat="server" Text="City:" AssociatedControlID="City" EnableViewState="false"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="City" runat="server" EnableViewState="false"></asp:TextBox> 
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
                            <asp:TextBox ID="PostalCode" runat="server" EnableViewState="false"></asp:TextBox> 
                            <asp:RequiredFieldValidator ID="PostalCodeRequired" runat="server" Text="*"
                                ErrorMessage="ZIP or Postal Code is required." Display="Static" ControlToValidate="PostalCode"></asp:RequiredFieldValidator>
                        </td>
                        <th class="rowHeader" width="120">
                            <asp:Label ID="CountryLabel" runat="server" Text="Country:" AssociatedControlID="Country" EnableViewState="false"></asp:Label>
                        </th>
                        <td>
                            <asp:DropDownList ID="Country" runat="server" DataTextField="Name" DataValueField="CountryCode" 
                                OnSelectedIndexChanged="Country_Changed" AutoPostBack="true" EnableViewState="false"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" valign="top" width="120">
                            <asp:Label ID="FaxLabel" runat="server" Text="Fax:" AssociatedControlID="Fax" EnableViewState="false"></asp:Label>
                        </th>
                        <td valign="top">
                            <asp:TextBox ID="Fax" runat="server" EnableViewState="false"></asp:TextBox> 
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
                        <td colspan="4">
                            <br />
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td colspan="3"> 
                            <asp:LinkButton ID="ContinueButton" runat="server" Text="Continue" OnClick="ContinueButton_Click" SkinID="Button" EnableViewState="false" />
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
    </ajax:UpdatePanel>
    <asp:PlaceHolder ID="EmailListPanel" runat="server" EnableViewState="false">
        <div class="header">
            <h2><asp:Localize ID="EmailListCaption" runat="server" Text="Communication Preferences" EnableViewState="false"></asp:Localize></h2>
        </div>
        <div class="content" style="padding: 8px 20px;">
            <asp:Label ID="EmailListHelpText" runat="server" Text="Select the email lists you wish to sign up for:" AssociatedControlID="dlEmailLists" EnableViewState="false"></asp:Label>
            <asp:DataList ID="dlEmailLists" runat="server" DataKeyField="EmailListId">
                <ItemTemplate>
                    <asp:CheckBox ID="Selected" runat="server" Checked="true" />
                    <%#Eval("Name")%><br />
                    <%#Eval("Description")%>
                </ItemTemplate>
            </asp:DataList>
        </div>
    </asp:PlaceHolder>
</div>
