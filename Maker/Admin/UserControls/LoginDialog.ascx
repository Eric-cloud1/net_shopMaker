<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LoginDialog.ascx.cs" Inherits="Admin_UserControls_LoginDialog" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>

<ajax:UpdatePanel ID="LoginAjax" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="LoginPanel" runat="server" CssClass="section" DefaultButton="LoginButton">
            <div class="header">
                <h2 class="loginpage"><asp:Localize ID="LoginCaption" runat="server" Text="Merchant Login"></asp:Localize></h2>
            </div>
            <div class="content nofooter">
                <table class="inputForm" cellpadding="4" cellspacing="0">
                    <tr>
                        <td colspan="2">
                            <asp:ValidationSummary ID="LoginValidationSummary" runat="server" ValidationGroup="Login" />
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName" Text="User name:"></asp:Label>
                        </th>
                        <td align="left">
                            <asp:TextBox ID="UserName" runat="server" Columns="30" CssClass="txt"></asp:TextBox>                            
                            <asp:CustomValidator ID="InvalidLogin" runat="server" ControlToValidate="UserName"
                                ErrorMessage="The sign in information you provided was incorrect." ToolTip="The sign in information you provided was incorrect."
                                Display="Dynamic" ValidationGroup="Login" Text="*" EnableViewState="false"></asp:CustomValidator>
                            <asp:CustomValidator ID="AccountDisabled" runat="server" ControlToValidate="UserName"
                                ErrorMessage="This account has been disabled." ToolTip="This account has been disabled."
                                Display="Dynamic" ValidationGroup="Login" Text="*" EnableViewState="false"></asp:CustomValidator>
                            <asp:CustomValidator ID="AccountLocked" runat="server" ControlToValidate="UserName"
                                ErrorMessage="This account has been locked for {0} minutes." ToolTip="This account has been locked."
                                Display="Dynamic" ValidationGroup="Login" Text="*" EnableViewState="false"></asp:CustomValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td align="left">
                            <asp:CheckBox ID="RememberUserName" runat="server" Text="Remember Me" />
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" Text="Password:"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="Password" runat="server" TextMode="Password" Width="80px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                ErrorMessage="You must provide a password" ToolTip="You must provide a password"
                                ValidationGroup="Login" Text="*"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr id="trCaptchaField" runat="server">
                        <td align="right">                         
                            <asp:Label ID="CaptchaInputLabel" runat="server" Text="Verification Number:" SkinID="FieldHeader"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="CaptchaInput" runat="server" Width="80px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="CaptchaInput"
                                Display="Dynamic" ErrorMessage="You must enter the number in the image." Text="*"></asp:RequiredFieldValidator>
                            <asp:PlaceHolder ID="phCaptchaValidators" runat="server"></asp:PlaceHolder>
                         </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>
                            <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Sign In" ValidationGroup="Login" OnClick="LoginButton_Click" UseSubmitBehavior="false" />&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:LinkButton ID="ForgotPasswordButton" runat="server" Text="Forgot Password?" CausesValidation="false" OnClick="ForgotPasswordButton_Click"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr id="trCaptchaImage" runat="server">
                        <th class="rowHeader" valign="top">
                            <asp:Label ID="CaptchaLabel" runat="server" AssociatedControlID="CaptchaImage" Text="Verification:"></asp:Label>
                        </th>
                        <td style="min-height:100px">
                            <ajax:UpdatePanel ID="CaptchaPanel" runat="server">
                                <ContentTemplate>
                                    <cb:CaptchaImage ID="CaptchaImage" runat="server" Height="80px" Width="300px" /><br />
                                    <asp:LinkButton ID="ChangeImageLink" runat="server" Text="different image" CausesValidation="false" OnClick="ChangeImageLink_Click"></asp:LinkButton><br /><br />
                                </ContentTemplate>
                            </ajax:UpdatePanel>                            
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <asp:Panel ID="ForgotPasswordPanel" runat="server" CssClass="section" DefaultButton="ForgotPasswordNextButton">
            <div class="header">
                <h2><asp:Localize ID="ForgotPasswordCaption" runat="server" Text="Forgot Password"></asp:Localize></h2>
            </div>
            <div class="content nofooter">
                <table class="inputForm" cellpadding="0" cellspacing="0">
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="ForgotPasswordHelpText" runat="server" EnableViewState="False" Text="Enter your registered email address for password assistance."></asp:Label>
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="ForgotPassword" />
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="ForgotPasswordUserNameLabel" runat="server" AssociatedControlID="ForgotPasswordUserName" Text="Email:"></asp:Label>
                        </th>
                        <td align="left">
                            <asp:TextBox ID="ForgotPasswordUserName" runat="server" Columns="30"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="ForgotPasswordUserNameRequired" runat="server" ControlToValidate="ForgotPasswordUserName"
                                ErrorMessage="Email address is required" ToolTip="Email address is required" 
                                ValidationGroup="ForgotPassword" Display="Dynamic" Text="*"></asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="ForgotPasswordUserNameValidator" runat="server" ControlToValidate="ForgotPasswordUserName"
                                ErrorMessage="The given email address is not registered." ToolTip="The given email address is not registered."
                                Display="Dynamic" ValidationGroup="ForgotPassword" Text="*"></asp:CustomValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>
                            <asp:Button ID="ForgotPasswordCancelButton" runat="server" Text="Cancel" CausesValidation="False" OnClick="ForgotPasswordCancelButton_Click" />
                            <asp:Button ID="ForgotPasswordNextButton" runat="server" Text="Next >" ValidationGroup="ForgotPassword" OnClick="ForgotPasswordNextButton_Click" /> 
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <asp:Panel ID="EmailSentPanel" runat="server" CssClass="section">
            <div class="header">
                <h2><asp:Localize ID="EmailSentCaption" runat="server" Text="Check Your Email"></asp:Localize></h2>
            </div>
            <div class="content nofooter">
                <asp:Label ID="EmailSentHelpText" runat="server" Text="We have sent an email to '{0}'. Please check your email to get the user name or follow the link it contains to create a new password."></asp:Label>
                <br /><br />
                <asp:Button ID="ForgotPasswordContinueButton" runat="server" Text="Continue" OnClick="ForgotPasswordContinueButton_Click" />
            </div>
        </asp:Panel>
        <asp:Panel ID="PasswordExpiredPanel" runat="server" CssClass="section">
            <div class="header">
                <h2><asp:Localize ID="PasswordExpiredCaption" runat="server" Text="Password Expired"></asp:Localize></h2>
            </div>
            <div class="content nofooter">
                <table class="inputForm">
                    <tr>
                        <td align="left" colspan="2">
                            <asp:Localize ID="PasswordExpiredInstructionText" runat="server" Text="You must provide a new password to continue and the new password must meet the following minimum requirements:" EnableViewState="false"></asp:Localize>
                            <ul>
                                <li runat="server" id="ppLength">
                                <asp:Localize ID="PasswordPolicyLength" runat="server" Text="  The new password must be at least {0} characters long." EnableViewState="false"></asp:Localize>
                                </li>
                                <li runat="server" id="ppHistoryCount">
                                <asp:Localize ID="PasswordPolicyHistoryCount" runat="server" Text="  You may not use any of your previous {0} passwords." EnableViewState="false"></asp:Localize>
                                </li>
                                <li runat="server" id="ppHistoryDays">
                                <asp:Localize ID="PasswordPolicyHistoryDays" runat="server" Text="  You may not reuse any passwords used within the last {0} days." EnableViewState="false"></asp:Localize>
                                </li>
                                <li runat="server" id="ppPolicyRequired">
                                <asp:Localize ID="PasswordPolicyRequired" runat="server" Text="  The password must include at least one {0}." EnableViewState="false"></asp:Localize>
                                </li>
                            </ul>
							<br/>
                            <asp:ValidationSummary ID="PasswordExpiredValidationSummary" runat="server" ValidationGroup="PasswordExpired" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr>
                        <th align="right">
                            <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword" Text="New Password" EnableViewState="false"></asp:Label>
                        </th>
                        <td align="left">
                            <asp:TextBox ID="NewPassword" runat="server" Font-Size="0.8em" TextMode="Password"></asp:TextBox>
                            <asp:PlaceHolder ID="phNewPasswordValidators" runat="server"></asp:PlaceHolder>
                            <asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" ControlToValidate="NewPassword"
                                ErrorMessage="New password is required." ToolTip="New Password is required."
                                Text="*" ValidationGroup="PasswordExpired"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th align="right">
                            <asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword" Text="Retype:" EnableViewState="false"></asp:Label></td>
                        </th>
                        <td align="left">
                            <asp:TextBox ID="ConfirmNewPassword" runat="server" Font-Size="0.8em" TextMode="Password"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="ConfirmNewPasswordRequired" runat="server" ControlToValidate="ConfirmNewPassword"
                                ErrorMessage="Confirm New Password is required." ToolTip="Confirm New Password is required."
                                Text="*" ValidationGroup="PasswordExpired"></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="NewPasswordCompare" runat="server" ControlToCompare="NewPassword"
                                ControlToValidate="ConfirmNewPassword" Display="Dynamic" ErrorMessage="You did not retype the new password correctly."
                                Text="*" ValidationGroup="PasswordExpired"></asp:CompareValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>
                            <asp:Button ID="ChangePasswordButton" runat="server" Text="Continue >" EnableViewState="false" ValidationGroup="PasswordExpired" OnClick="ChangePasswordButton_Click" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <asp:HiddenField ID="VS" runat="server" EnableViewState="false" />
    </ContentTemplate>
</ajax:UpdatePanel>