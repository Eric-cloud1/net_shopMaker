<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MyCredentialsPage.ascx.cs" Inherits="ConLib_MyCredentialsPage" %>
<%--
<conlib>
<summary>Displays a form where customer can update email address or password.</summary>
</conlib>
--%>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<div class="pageHeader">
    <h1><asp:Localize ID="Caption" runat="server" Text="Update Email Address or Password"></asp:Localize></h1>
</div>
<ajax:UpdatePanel ID="AjaxPanel" runat="server">
    <ContentTemplate>
        <asp:Panel ID="EditPanel" runat="server">
            <p align="Justify">
                <asp:Localize ID="UpdateAccountText" runat="server" Text="You can update the username, email address or password associated with your account using the form below.  You must provide your current password in order to save the changes.  If you do not wish to change your password, leave the new and confirm password fields empty."></asp:Localize>
                <asp:Localize ID="NewAccountText" runat="server" Text="Enter your email address and password below so that you may access your account in the future."></asp:Localize>
            </p>
            <table class="inputForm" cellpadding="0" cellspacing="0">
                <tr>
                    <th class="rowHeader">
                        <asp:Label ID="UserNameLabel" runat="server" Text="User Name:"></asp:Label>
                    </th>
                    <td>
                        <asp:TextBox ID="UserName" runat="server" width="200px" MaxLength="200"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                            Display="Dynamic" ErrorMessage="You must provide a username." Text="*"></asp:RequiredFieldValidator>
                        <asp:PlaceHolder ID="phUserNameUnavailable" runat="server" Visible="false" EnableViewState="false">
						    <div class="errorCondition">The user name you have provided is already in use. Please choose a different user name.</div>
						</asp:PlaceHolder>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader">
                        <asp:Label ID="EmailLabel" runat="server" Text="Email:"></asp:Label>
                    </th>
                    <td>
                        <asp:TextBox ID="Email" runat="server" width="200px" MaxLength="200"></asp:TextBox>
                        <asp:CustomValidator ID="DuplicateEmailValidator" runat="server" ControlToValidate="Email"
                            Display="Dynamic" ErrorMessage="That email address is registered to another account." Text="*"></asp:CustomValidator>
                        <cb:EmailAddressValidator ID="EmailAddressValidator1" runat="server" 
                            ErrorMessage="Email address should be in the format of name@domain.tld." ControlToValidate="Email" Display="Static" Text="*" Required="true"></cb:EmailAddressValidator>
                    </td>
                </tr>                
                <tr id="trCurrentPassword" runat="server">
                    <th class="rowHeader">
                        <asp:Label ID="CurrentPasswordLabel" runat="server" Text="Current Password:"></asp:Label>
                    </th>
                    <td>
                        <asp:TextBox ID="CurrentPassword" runat="server" TextMode="password" Width="150px" MaxLength="30"></asp:TextBox>
                        <asp:CustomValidator ID="InvalidPassword" runat="server" ControlToValidate="CurrentPassword"
                            Display="Dynamic" ErrorMessage="The password you provided is incorrect." Text="*"></asp:CustomValidator>
                        <asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server" ControlToValidate="CurrentPassword"
                            Display="Static" ErrorMessage="Current password is required to save your changes." Text="*"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                     <td></td>
                     <td>
                        <ul style="margin:8px 0px;padding:0px;">
                            <asp:Localize ID="PasswordPolicyLength" runat="server" Text="<li>The new password must be at least {0} characters long.</li>"></asp:Localize>
                            <asp:Localize ID="PasswordPolicyHistoryCount" runat="server" Text="<li>You may not use any of your previous {0} passwords.</li>"></asp:Localize>
                            <asp:Localize ID="PasswordPolicyHistoryDays" runat="server" Text="<li>You may not reuse any passwords used within the last {0} days.</li>"></asp:Localize>
                            <asp:Localize ID="PasswordPolicyRequired" runat="server" Text="<li>The password must include at least one {0}.</li>"></asp:Localize>
                        </ul>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader" valign="top">
                        <asp:Label ID="PasswordLabel" runat="server" Text="New Password:"></asp:Label>
                    </th>
                    <td valign="top">
                        <asp:TextBox ID="Password" runat="server" TextMode="password" Width="150px" MaxLength="30"></asp:TextBox>
                        <asp:CustomValidator ID="PasswordRequiredValidator" runat="server" ControlToValidate="Password"
                            Display="Dynamic" ErrorMessage="You must provide a new password to continue." Text="*"></asp:CustomValidator>
                        <asp:CustomValidator ID="PasswordPolicyValidator" runat="server" ControlToValidate="Password"
                            Display="Dynamic" ErrorMessage="The new password you provided does not meet the minimum requirements." Text="*" EnableViewState="false"></asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader" nowrap>
                        <asp:Label ID="ConfirmPasswordLabel" runat="server" Text="Confirm Password:"></asp:Label>
                    </th>
                    <td>
                        <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="password" Width="150px" MaxLength="30"></asp:TextBox>
                        <asp:CompareValidator ID="ConfirmPasswordValidator2" runat="server" ControlToCompare="Password" ControlToValidate="ConfirmPassword" 
                            Display="Dynamic" ErrorMessage="You did not retype your new password correctly." Text="*"></asp:CompareValidator>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                        <asp:LinkButton ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" SkinID="Button" />
                        <asp:HyperLink ID="CancelButton" runat="server" Text="Cancel" NavigateUrl="~/Members/Myaccount.aspx" SkinID="Button" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="ConfirmPanel" runat="server">
            <asp:Label ID="ConfirmText" runat="server" Text="Your information has been updated." SkinID="GoodCondition"></asp:Label><br /><br />
            <asp:HyperLink ID="ContinueLink" runat="server" Text="Continue" NavigateUrl="~/Members/MyAccount.aspx" SkinID="Button"></asp:HyperLink>
        </asp:Panel>
    </ContentTemplate>
</ajax:UpdatePanel>
