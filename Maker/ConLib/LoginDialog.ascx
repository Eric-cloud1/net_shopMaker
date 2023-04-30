<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LoginDialog.ascx.cs" Inherits="ConLib_LoginDialog" %>
<%--
<conlib>
<summary>Displays a standard login dialog that includes support for recovering forgotten passwords.</summary>
<param name="EnableAdminRememberMe" default="true">Set to <b>true</b> or <b>false</b>.  Indicates whether the remember me option is available to admin users.</param>
</conlib>
--%>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<ajax:UpdatePanel ID="LoginAjax" runat="server">
    <ContentTemplate>
        <asp:Panel ID="LoginPanel" runat="server" DefaultButton="LoginButton" CssClass="dialogSection">
            <div class="header">
	            <h2><asp:Localize ID="Caption" runat="server" Text="Returning Customers" EnableViewState="False"></asp:Localize></h2>
            </div>
            <div class="content nofooter">
                <table class="inputForm" cellpadding="0" cellspacing="0" align="center">
                    <tr>
                        <td colspan="3">
                            <asp:Label ID="InstructionText" runat="server" Text="If you have already registered with <b>{0}</b>, please sign in now." EnableViewState="False"></asp:Label><br /><br />
                            <asp:ValidationSummary ID="LoginValidationSummary" runat="server" ValidationGroup="Login" EnableViewState="False" />
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName" Text="User Name:" EnableViewState="False"></asp:Label>
                        </th>
                        <td align="left" colspan="2">
                            <asp:TextBox ID="UserName" runat="server" Columns="30" CssClass="txt" 
                                TabIndex="1"></asp:TextBox>                            
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
                    <tr id="trRememberMe" runat="server">
                        <td>&nbsp;</td>
                        <td colspan="2">
                            <asp:CheckBox ID="RememberUserName" runat="server" Text="Remember Me" 
                                TabIndex="2" />
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" Text="Password:" EnableViewState="False"></asp:Label>
                        </th>
                        <td nowrap>
                            <asp:TextBox ID="Password" runat="server" TextMode="Password" Width="80px" 
                                EnableViewState="False" TabIndex="3"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                ErrorMessage="You must provide a password" ToolTip="You must provide a password"
                                ValidationGroup="Login" Text="*" EnableViewState="False"></asp:RequiredFieldValidator>
                        </td>
                        <td rowspan="3">
                            <asp:PlaceHolder ID="trCaptchaImage" runat="server" Visible="false">
                                <br />
                                <ajax:UpdatePanel ID="CaptchaPanel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Label ID="CaptchaImageLabel" runat="server" Text="Verification Number:" AssociatedControlID="CaptchaImage" SkinID="FieldHeader" EnableViewState="False"></asp:Label><br />
                                        <cb:CaptchaImage ID="CaptchaImage" runat="server" Height="60px" Width="200px" EnableViewState="False" /><br />
                                        <asp:LinkButton ID="ChangeImageLink" runat="server" Text="different image" CausesValidation="false" OnClick="ChangeImageLink_Click" EnableViewState="False" TabIndex="6"></asp:LinkButton><br /><br />
                                    </ContentTemplate>
                                </ajax:UpdatePanel> 
                            </asp:PlaceHolder>
                        </td>
                    </tr>
                    <tr id="trCaptchaField" runat="server" visible="false">
                        <th class="rowHeader">
                            <asp:Label ID="CaptchaInputLabel" runat="server" Text="Verification:" AssociatedControlID="CaptchaInput" EnableViewState="False"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="CaptchaInput" runat="server" Width="80px" 
                                EnableViewState="False" TabIndex="4"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="CaptchaRequired" runat="server" ControlToValidate="CaptchaInput"
                                ErrorMessage="You must enter the number in the image." ToolTip="You must enter the number in the image."
                                Display="Dynamic" ValidationGroup="Login" Text="*" EnableViewState="False"></asp:RequiredFieldValidator>
                            <asp:PlaceHolder ID="phCaptchaValidators" runat="server" EnableViewState="false"></asp:PlaceHolder>
                         </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>
                            <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Sign In" 
                                ValidationGroup="Login" OnClick="LoginButton_Click" UseSubmitBehavior="false" 
                                EnableViewState="false" TabIndex="5" />
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td colspan="2">
                            <asp:LinkButton ID="ForgotPasswordButton" runat="server" 
                                Text="Forgot User Name or Password?" CausesValidation="false" 
                                OnClick="ForgotPasswordButton_Click" EnableViewState="False" TabIndex="7"></asp:LinkButton>
                        </td>
                    </tr>
                </table>
        </asp:Panel>
        <asp:Panel ID="ForgotPasswordPanel" runat="server" DefaultButton="ForgotPasswordNextButton" CssClass="dialogSection">
            <div class="header">
                <h2><asp:Localize ID="ForgotPasswordCaption" runat="server" Text="Forgot Password" EnableViewState="false"></asp:Localize></h2>
            </div>
            <div class="content nofooter">
                <table class="inputForm" cellpadding="0" cellspacing="0" align="center">
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="ForgotPasswordHelpText" runat="server" EnableViewState="False" Text="Enter your registered email address for user name or password assistance."></asp:Label>
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="ForgotPassword" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="ForgotPasswordUserNameLabel" runat="server" AssociatedControlID="ForgotPasswordUserName" Text="Email:" EnableViewState="false"></asp:Label>
                        </th>
                        <td align="left" style="width:80%;">
                            <asp:TextBox ID="ForgotPasswordUserName" runat="server" Columns="30" EnableViewState="false"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="ForgotPasswordUserNameRequired" runat="server" ControlToValidate="ForgotPasswordUserName"
                                ErrorMessage="Email address is required" ToolTip="Email address is required" 
                                ValidationGroup="ForgotPassword" Display="Dynamic" Text="*" EnableViewState="false"></asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="ForgotPasswordUserNameValidator" runat="server" ControlToValidate="ForgotPasswordUserName"
                                ErrorMessage="The given email address is not registered." ToolTip="The given email address is not registered."
                                Display="Dynamic" ValidationGroup="ForgotPassword" Text="*" EnableViewState="false"></asp:CustomValidator>
                            <asp:CustomValidator ID="DisabledUsernameValidator" runat="server" ControlToValidate="ForgotPasswordUserName"
                                ErrorMessage="The account associated with this address has been disabled." ToolTip="The account associated with this address has been disabled."
                                Display="Dynamic" ValidationGroup="ForgotPassword" Text="*" EnableViewState="false"></asp:CustomValidator>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <asp:Button ID="ForgotPasswordCancelButton" runat="server" Text="Cancel" CausesValidation="False" OnClick="ForgotPasswordCancelButton_Click" EnableViewState="false" />
                            <asp:Button ID="ForgotPasswordNextButton" runat="server" Text="Next >" ValidationGroup="ForgotPassword" OnClick="ForgotPasswordNextButton_Click" EnableViewState="false" /> 
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <asp:Panel ID="EmailSentPanel" runat="server" CssClass="dialogSection">
            <div class="header">
                <h2><asp:Localize ID="EmailSentCaption" runat="server" Text="Check Your Email" EnableViewState="false"></asp:Localize></h2>
            </div>
            <div class="content nofooter">
                <asp:Label ID="EmailSentHelpText" runat="server" Text="We have sent an email to '{0}'. Please check your email to get the user name or follow the link it contains to create a new password." EnableViewState="false"></asp:Label>
                <br /><br />
                <asp:Button ID="KeepShoppingButton" runat="server" Text="Keep Shopping" OnClick="KeepShoppingButton_Click" EnableViewState="false" />
            </div>
        </asp:Panel>
        <asp:Panel ID="PasswordExpiredPanel" runat="server" DefaultButton="ChangePasswordButton" CssClass="dialogSection">
            <div class="header">
                <h2><asp:Localize ID="PasswordExpiredCaption" runat="server" Text="Password Expired" EnableViewState="false"></asp:Localize></h2>
            </div>
            <div class="content nofooter">
                <table class="inputForm">
                    <tr>
                        <td align="left" colspan="2" >
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
                            <asp:TextBox ID="NewPassword" runat="server" Font-Size="0.8em" TextMode="Password" EnableViewState="false"></asp:TextBox>
                            <asp:PlaceHolder ID="phNewPasswordValidators" runat="server" EnableViewState="false"></asp:PlaceHolder>
                            <asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" ControlToValidate="NewPassword"
                                ErrorMessage="New password is required." ToolTip="New Password is required."
                                Text="*" ValidationGroup="PasswordExpired" EnableViewState="false"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th align="right">
                            <asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword" Text="Retype:" EnableViewState="false"></asp:Label></td>
                        </th>
                        <td align="left">
                            <asp:TextBox ID="ConfirmNewPassword" runat="server" Font-Size="0.8em" TextMode="Password" EnableViewState="false"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="ConfirmNewPasswordRequired" runat="server" ControlToValidate="ConfirmNewPassword"
                                ErrorMessage="Confirm New Password is required." ToolTip="Confirm New Password is required."
                                Text="*" ValidationGroup="PasswordExpired" EnableViewState="false"></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="NewPasswordCompare" runat="server" ControlToCompare="NewPassword"
                                ControlToValidate="ConfirmNewPassword" Display="Dynamic" ErrorMessage="You did not retype the new password correctly."
                                Text="*" ValidationGroup="PasswordExpired" EnableViewState="false"></asp:CompareValidator>
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