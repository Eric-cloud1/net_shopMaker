<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RegisterDialog.ascx.cs" Inherits="ConLib_RegisterDialog" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<%--
<conlib>
<summary>Display a form using that a new customer can register for a new account.</summary>
</conlib>
--%>
<asp:Panel ID="RegisterPanel" runat="server" DefaultButton="RegisterButton" CssClass="dialogSection">
    <div class="header">
        <h2><asp:Localize ID="Caption" runat="server" Text="New Customers"></asp:Localize></h2>
    </div>
    <div class="content nofooter">
        <table class="inputForm" cellpadding="0" cellspacing="0" align="center">
            <tr>
                <td align="center" colspan="3">
                    <asp:ValidationSummary ID="RegisterValidationSummary" runat="server" ValidationGroup="Register" />
                    <div style="text-align:center">
                        <asp:Localize ID="InstructionText" runat="server" Text="If you are a new customer, take a moment to register below." EnableViewState="False"></asp:Localize>
                    </div>
                    <asp:Label ID="FailureText" runat="server" EnableViewState="False" SkinID="ErrorCondition"></asp:Label>
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName" Text="Email:"></asp:Label>
                </th>
                <td align="left" colspan="2">
                    <asp:TextBox ID="UserName" runat="server" Columns="30" TabIndex="8"></asp:TextBox>
                    <cb:EmailAddressValidator ID="EmailAddressValidator1" runat="server" ControlToValidate="UserName" 
                        ValidationGroup="Register" Display="Dynamic" Required="true" 
                        ErrorMessage="Email address should be in the format of name@domain.tld." Text="*" EnableViewState="False"></cb:EmailAddressValidator>
                    <asp:CustomValidator ID="InvalidRegistration" runat="server" ControlToValidate="UserName"
                        ErrorMessage="Registration was not successful." ToolTip="Registration was not successful."
                        Display="Dynamic" ValidationGroup="Register" Text="*"></asp:CustomValidator>
                    <asp:CustomValidator ID="DuplicateEmailValidator" runat="server" ControlToValidate="UserName" ValidationGroup="Register" Display="Dynamic"
                        Text="*" ErrorMessage="This email address is already associated with an existing user account.  Use the 'Forgot User Name or Password' feature if you need to retrieve your login information, or use a different email address to register." />
                    <span style="font-size:smaller">(username)</span>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td colspan="2">
                    <asp:CheckBox ID="RememberUserName" runat="server" Text="Remember Me" 
                        TabIndex="9" />
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" Text="Password:"></asp:Label>
                </th>
                <td align="left" nowrap>
                    <asp:TextBox ID="Password" runat="server" TextMode="Password" Width="80" 
                        MaxLength="24" TabIndex="10"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                        ErrorMessage="You must provide a password" ToolTip="You must provide a password"
                        ValidationGroup="Register" Text="*"></asp:RequiredFieldValidator>
                    <asp:PlaceHolder ID="PasswordValidatorPanel" runat="server" EnableViewState="false"></asp:PlaceHolder>
                </td>
                <td rowspan="4" valign="top">
                    <div style="width:210px;text-align:justify">
                    <i><asp:Localize ID="PasswordPolicyLength" runat="server" Text="Your password must be at least {0} characters long."></asp:Localize>
                    <asp:Localize ID="PasswordPolicyRequired" runat="server" Text="You must include at least one {0}."></asp:Localize></i>
                    </div><br />
                    <asp:PlaceHolder ID="trCaptchaImage" runat="server" Visible="false">
                        <ajax:UpdatePanel ID="CaptchaPanel" UpdateMode="Conditional" runat="server">
                            <ContentTemplate>
                                <asp:Label ID="CaptchaImageLabel" runat="server" Text="Verification Number:" AssociatedControlID="CaptchaImage" SkinID="FieldHeader" EnableViewState="False"></asp:Label><br />
                                <cb:CaptchaImage ID="CaptchaImage" runat="server" Height="60px" Width="200px" EnableViewState="False" /><br />
                                <asp:LinkButton ID="ChangeImageLink" runat="server" Text="different image" CausesValidation="false" OnClick="ChangeImageLink_Click" EnableViewState="False" TabIndex="14"></asp:LinkButton><br /><br />
                            </ContentTemplate>
                        </ajax:UpdatePanel> 
                    </asp:PlaceHolder>
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword" Text="Re-enter:"></asp:Label>
                </th>
                <td align="left" nowrap>
                    <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="Password" Width="80" 
                        MaxLength="24" TabIndex="12"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="ConfirmPasswordRequired" runat="server" ControlToValidate="ConfirmPassword"
                        ErrorMessage="You must re-enter the password." ToolTip="You must re-enter the password."
                        ValidationGroup="Register" Text="*"></asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password"
                        ControlToValidate="ConfirmPassword" Display="Dynamic" ErrorMessage="You did not re-enter the password correctly."
                        ValidationGroup="Register" Text="*"></asp:CompareValidator>
                </td>
            </tr>
           <tr id="trCaptchaField" runat="server" visible="false">
                <th class="rowHeader">
                    <asp:Label ID="CaptchaInputLabel" runat="server" Text="Verification:" AssociatedControlID="CaptchaInput" CssClass="H2" EnableViewState="False"></asp:Label><br />
                </th>
                <td>
                    <asp:TextBox ID="CaptchaInput" runat="server" Width="80px" 
                        EnableViewState="False" ValidationGroup="Register" TabIndex="13"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="CaptchaRequired" runat="server" ControlToValidate="CaptchaInput"
                        ErrorMessage="You must enter the number in the image." ToolTip="You must enter the number in the image."
                        Display="Dynamic" ValidationGroup="Register" Text="*" EnableViewState="False"></asp:RequiredFieldValidator>
                    <asp:PlaceHolder ID="phCaptchaValidators" runat="server" EnableViewState="false"></asp:PlaceHolder>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:Button ID="RegisterButton" runat="server" Text="Register" 
                        OnClick="RegisterButton_Click" ValidationGroup="Register" TabIndex="13" />
                </td>
            </tr>
        </table>
    </div>
</div>
</asp:Panel>