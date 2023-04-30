<%@Page Language="C#" MasterPageFile="~/Admin/Admin.master"
CodeFile="PasswordPolicy.aspx.cs" Inherits="Admin_Store_Security_PasswordPolicy"
Title="Password Policy" EnableViewState="false" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb"
%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Password Policies"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <ajax:UpdatePanel ID="PageAjax" runat="server">
    <ContentTemplate>
      <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr
          id="trSavedMessage"
          runat="server"
          visible="false"
          enableviewstate="false"
        >
          <td colspan="2">
            <asp:Label
              ID="SavedMessage"
              runat="server"
              Text="Password policies have been saved."
              EnableViewState="false"
              SkinID="GoodCondition"
            ></asp:Label>
          </td>
        </tr>
        <tr>
          <td width="50%" valign="top">
            <div class="section">
              <div class="header">
                <h2 class="administrator">
                  <asp:Localize
                    ID="MerchantPolicyHeader"
                    runat="server"
                    Text="Merchant Policy"
                  ></asp:Localize>
                </h2>
              </div>
              <table class="inputForm" width="100%">
                <tr>
                  <td colspan="2">
                    <asp:Localize
                      ID="MerchantInstructionText"
                      runat="server"
                      Text="Requirements for non-consumer accounts."
                    ></asp:Localize>
                  </td>
                </tr>
                <tr>
                  <th class="rowHeader">
                    <cb:ToolTipLabel
                      ID="MerchantMinLengthLabel"
                      runat="server"
                      Text="Minimum Password Length:"
                      ToolTip="The minimum length required for a password."
                    ></cb:ToolTipLabel>
                  </th>
                  <td>
                    <asp:TextBox
                      ID="MerchantMinLength"
                      runat="server"
                      Width="40px"
                      MaxLength="2"
                    ></asp:TextBox>
                    <asp:Label
                      ID="MerchantMinLengthLabel2"
                      runat="server"
                      Text=" chars"
                    ></asp:Label>
                    <asp:RequiredFieldValidator
                      ID="MerchantMinLengthValidator1"
                      runat="server"
                      ControlToValidate="MerchantMinLength"
                      Text="*"
                      ErrorMessage="Merchant minimum length must be at least 1."
                    ></asp:RequiredFieldValidator>
                  </td>
                </tr>
                <tr>
                  <th class="rowHeader" valign="top">
                    <cb:ToolTipLabel
                      ID="MerchantRequiredElementsLabel"
                      runat="server"
                      Text="Required Password Elements:"
                      ToolTip="Indicate the elements that must be included for new passwords.  At least one character in a password must belong to any of the groups indicated."
                    ></cb:ToolTipLabel>
                  </th>
                  <td>
                    <asp:CheckBox
                      ID="MerchantRequireUppercase"
                      runat="server"
                      Text="Uppercase (A - Z)"
                    /><br />
                    <asp:CheckBox
                      ID="MerchantRequireLowercase"
                      runat="server"
                      Text="Lowercase (a - z)"
                    /><br />
                    <asp:CheckBox
                      ID="MerchantRequireNumbers"
                      runat="server"
                      Text="Numbers (0 - 9)"
                    /><br />
                    <asp:CheckBox
                      ID="MerchantRequireSymbols"
                      runat="server"
                      Text="Symbols (punctuation, underscore)"
                    /><br />
                    <asp:CheckBox
                      ID="MerchantRequireNonAlpha"
                      runat="server"
                      Text="Non-letter (Number or Symbol)"
                    />
                  </td>
                </tr>
                <tr>
                  <th class="rowHeader">
                    <cb:ToolTipLabel
                      ID="MerchantMaxAgeLabel"
                      runat="server"
                      Text="Maximum Password Age:"
                      ToolTip="The maximum age of a password (in days) before a new password is mandatory. Enter zero for unlimited."
                    ></cb:ToolTipLabel>
                  </th>
                  <td>
                    <asp:TextBox
                      ID="MerchantMaxAge"
                      runat="server"
                      Width="40px"
                      MaxLength="2"
                    ></asp:TextBox>
                    <asp:Label
                      ID="MerchantMaxAgeLabel2"
                      runat="server"
                      Text=" days"
                    ></asp:Label>
                  </td>
                </tr>
                <tr>
                  <th class="rowHeader">
                    <cb:ToolTipLabel
                      ID="MerchantPasswordHistoryLabel"
                      runat="server"
                      Text="Password History:"
                      ToolTip="Defines the length of password history, given a minimum number of days and passwords.  Passwords cannot be reused while they remain in the password history."
                    ></cb:ToolTipLabel>
                  </th>
                  <td>
                    <asp:TextBox
                      ID="MerchantPasswordHistoryDays"
                      runat="server"
                      Width="40px"
                      MaxLength="2"
                    ></asp:TextBox>
                    <asp:Label
                      ID="MerchantPasswordHistoryDaysLabel"
                      runat="server"
                      Text=" days"
                    ></asp:Label>
                    <asp:RangeValidator
                      ID="MerchantPasswordHistoryDaysValidator1"
                      runat="server"
                      ControlToValidate="MerchantPasswordHistoryDays"
                      Text="*"
                      ErrorMessage="Number of days must be an integer between 0 and 90"
                      type="integer"
                      MinimumValue="0"
                      MaximumValue="90"
                    ></asp:RangeValidator
                    >&nbsp;&nbsp;
                    <asp:TextBox
                      ID="MerchantPasswordHistoryCount"
                      runat="server"
                      Width="40px"
                      MaxLength="2"
                    ></asp:TextBox>
                    <asp:Label
                      ID="MerchantPasswordHistoryCountLabel"
                      runat="server"
                      Text=" passwords"
                    ></asp:Label>
                    <asp:RangeValidator
                      ID="MerchantPasswordHistoryCountValidator1"
                      runat="server"
                      ControlToValidate="MerchantPasswordHistoryCount"
                      Text="*"
                      ErrorMessage="Number of passwords must be an integer between 0 and 20"
                      type="integer"
                      MinimumValue="0"
                      MaximumValue="20"
                    ></asp:RangeValidator
                    ><br />
                  </td>
                </tr>
                <tr>
                  <th class="rowHeader">
                    <cb:ToolTipLabel
                      ID="MerchantPasswordMaxAttemptsLabel"
                      runat="server"
                      Text="Maximum Login Failures:"
                      ToolTip="The number of times in a row that a login attempt can fail before the account is locked."
                    ></cb:ToolTipLabel>
                  </th>
                  <td>
                    <asp:TextBox
                      ID="MerchantPasswordMaxAttempts"
                      runat="server"
                      Width="40px"
                      MaxLength="2"
                    ></asp:TextBox>
                    <asp:RequiredFieldValidator
                      ID="MerchantPasswordMaxAttemptsValidator1"
                      runat="server"
                      ControlToValidate="MerchantPasswordMaxAttempts"
                      Text="*"
                      ErrorMessage="You must specify the maximum number of login failures."
                    ></asp:RequiredFieldValidator>
                    <asp:RangeValidator
                      ID="MerchantPasswordMaxAttemptsValidator2"
                      runat="server"
                      ControlToValidate="MerchantPasswordMaxAttempts"
                      Text="*"
                      ErrorMessage="Maximum number of login failures must be an integer between 1 and 20"
                      type="integer"
                      MinimumValue="1"
                      MaximumValue="20"
                    ></asp:RangeValidator>
                  </td>
                </tr>
                <tr>
                  <th class="rowHeader">
                    <cb:ToolTipLabel
                      ID="MerchantPasswordLockoutPeriodLabel"
                      runat="server"
                      Text="Lockout Period:"
                      ToolTip="The number of minutes an account will be locked after the maximum number of failed login attempts is reached."
                    ></cb:ToolTipLabel>
                  </th>
                  <td>
                    <asp:TextBox
                      ID="MerchantPasswordLockoutPeriod"
                      runat="server"
                      Width="40px"
                      MaxLength="3"
                    ></asp:TextBox>
                    <asp:Label
                      ID="MerchantPasswordLockoutPeriodLabel2"
                      runat="server"
                      Text=" minutes"
                    ></asp:Label>
                    <asp:RequiredFieldValidator
                      ID="MerchantPasswordLockoutPeriodValidator1"
                      runat="server"
                      ControlToValidate="MerchantPasswordLockoutPeriod"
                      Text="*"
                      ErrorMessage="You must specify the account lockout period."
                    ></asp:RequiredFieldValidator>
                    <asp:RangeValidator
                      ID="MerchantPasswordLockoutPeriodValidator2"
                      runat="server"
                      ControlToValidate="MerchantPasswordLockoutPeriod"
                      Text="*"
                      ErrorMessage="Account lockout period must be an integer between 1 and 999"
                      type="integer"
                      MinimumValue="1"
                      MaximumValue="999"
                    ></asp:RangeValidator>
                  </td>
                </tr>
                <tr>
                  <th class="rowHeader">
                    <cb:ToolTipLabel
                      ID="MerchantPasswordInactivePeriodLabel"
                      runat="server"
                      Text="Inactivity Period:"
                      ToolTip="The number of months a merchant or administration account can go unused before it will be deactivated."
                    ></cb:ToolTipLabel>
                  </th>
                  <td>
                    <asp:TextBox
                      ID="MerchantPasswordInactivePeriod"
                      runat="server"
                      Width="40px"
                      MaxLength="2"
                    ></asp:TextBox>
                    <asp:Label
                      ID="MerchantPasswordInactivePeriodLabel2"
                      runat="server"
                      Text=" months"
                    ></asp:Label>
                    <asp:RequiredFieldValidator
                      ID="MerchantPasswordInactivePeriodValidator1"
                      runat="server"
                      ControlToValidate="MerchantPasswordInactivePeriod"
                      Text="*"
                      ErrorMessage="You must specify the account inactivity period."
                    ></asp:RequiredFieldValidator>
                    <asp:RangeValidator
                      ID="MerchantPasswordInactivePeriodValidator2"
                      runat="server"
                      ControlToValidate="MerchantPasswordInactivePeriod"
                      Text="*"
                      ErrorMessage="The inactivity period must be an integer between 1 and 24"
                      type="integer"
                      MinimumValue="1"
                      MaximumValue="24"
                    ></asp:RangeValidator>
                  </td>
                </tr>
                <tr>
                  <th class="rowHeader">
                    <cb:ToolTipLabel
                      ID="MerchantImageCaptchaLabel"
                      runat="server"
                      Text="Use Captcha:"
                      ToolTip="If checked, an image captcha is required for login."
                    ></cb:ToolTipLabel>
                  </th>
                  <td>
                    <asp:CheckBox ID="MerchantImageCaptcha" runat="server" />
                  </td>
                </tr>
              </table>
            </div>
          </td>
          <td width="50%" valign="top">
            <div class="section">
              <div class="header">
                <h2 class="customers">
                  <asp:Localize
                    ID="CustomerPolicyHeader"
                    runat="server"
                    Text="Customer Policy"
                  ></asp:Localize>
                </h2>
              </div>
              <table class="inputForm" width="100%">
                <tr>
                  <td colspan="2">
                    <asp:Localize
                      ID="CustomerInstructionText"
                      runat="server"
                      Text="Requirements for customer accounts."
                    ></asp:Localize>
                  </td>
                </tr>
                <tr>
                  <th class="rowHeader">
                    <cb:ToolTipLabel
                      ID="CustomerMinLengthLabel"
                      runat="server"
                      Text="Minimum Length:"
                      ToolTip="The minimum length required for a password."
                    ></cb:ToolTipLabel>
                  </th>
                  <td>
                    <asp:TextBox
                      ID="CustomerMinLength"
                      runat="server"
                      Width="40px"
                      MaxLength="2"
                    ></asp:TextBox>
                    <asp:Label
                      ID="CustomerMinLengthLabel2"
                      runat="server"
                      Text=" chars"
                    ></asp:Label>
                    <asp:RequiredFieldValidator
                      ID="CustomerMinLengthValidator1"
                      runat="server"
                      ControlToValidate="CustomerMinLength"
                      Text="*"
                      ErrorMessage="Customer minimum length must be at least 1."
                    ></asp:RequiredFieldValidator>
                  </td>
                </tr>
                <tr>
                  <th class="rowHeader" valign="top">
                    <cb:ToolTipLabel
                      ID="CustomerRequiredElementsLabel"
                      runat="server"
                      Text="Required Elements:"
                      ToolTip="Indicate the elements that must be included for new passwords.  At least one character in a password must belong to any of the groups indicated."
                    ></cb:ToolTipLabel>
                  </th>
                  <td>
                    <asp:CheckBox
                      ID="CustomerRequireUppercase"
                      runat="server"
                      Text="Uppercase (A - Z)"
                    /><br />
                    <asp:CheckBox
                      ID="CustomerRequireLowercase"
                      runat="server"
                      Text="Lowercase (a - z)"
                    /><br />
                    <asp:CheckBox
                      ID="CustomerRequireNumbers"
                      runat="server"
                      Text="Numbers (0 - 9)"
                    /><br />
                    <asp:CheckBox
                      ID="CustomerRequireSymbols"
                      runat="server"
                      Text="Symbols (punctuation, underscore)"
                    /><br />
                    <asp:CheckBox
                      ID="CustomerRequireNonAlpha"
                      runat="server"
                      Text="Non-letter (Number or Symbol)"
                    />
                  </td>
                </tr>
                <tr>
                  <th class="rowHeader">
                    <cb:ToolTipLabel
                      ID="CustomerMaxAgeLabel"
                      runat="server"
                      Text="Maximum Age:"
                      ToolTip="The maximum age of a password (in days) before a new password is mandatory. Enter zero for unlimited."
                    ></cb:ToolTipLabel>
                  </th>
                  <td>
                    <asp:TextBox
                      ID="CustomerMaxAge"
                      runat="server"
                      Width="40px"
                      MaxLength="3"
                    ></asp:TextBox>
                    <asp:Label
                      ID="CustomerMaxAgeLabel2"
                      runat="server"
                      Text=" days"
                    ></asp:Label>
                  </td>
                </tr>
                <tr>
                  <th class="rowHeader">
                    <cb:ToolTipLabel
                      ID="CustomerPasswordHistoryLabel"
                      runat="server"
                      Text="Password History:"
                      ToolTip="Defines the length of password history, given a minimum number of days and passwords.  Passwords cannot be reused while they remain in the password history."
                    ></cb:ToolTipLabel>
                  </th>
                  <td>
                    <asp:TextBox
                      ID="CustomerPasswordHistoryDays"
                      runat="server"
                      Width="40px"
                      MaxLength="2"
                    ></asp:TextBox>
                    <asp:Label
                      ID="CustomerPasswordHistoryDaysLabel"
                      runat="server"
                      Text=" days"
                    ></asp:Label>
                    <asp:RangeValidator
                      ID="CustomerPasswordHistoryDaysValidator1"
                      runat="server"
                      ControlToValidate="CustomerPasswordHistoryDays"
                      Text="*"
                      ErrorMessage="Number of days must be an integer between 0 and 90"
                      type="integer"
                      MinimumValue="0"
                      MaximumValue="90"
                    ></asp:RangeValidator
                    >&nbsp;&nbsp;
                    <asp:TextBox
                      ID="CustomerPasswordHistoryCount"
                      runat="server"
                      Width="40px"
                      MaxLength="2"
                    ></asp:TextBox>
                    <asp:Label
                      ID="CustomerPasswordHistoryCountLabel"
                      runat="server"
                      Text=" passwords"
                    ></asp:Label>
                    <asp:RangeValidator
                      ID="CustomerPasswordHistoryCountValidator1"
                      runat="server"
                      ControlToValidate="CustomerPasswordHistoryCount"
                      Text="*"
                      ErrorMessage="Number of passwords must be an integer between 0 and 20"
                      type="integer"
                      MinimumValue="0"
                      MaximumValue="20"
                    ></asp:RangeValidator
                    ><br />
                  </td>
                </tr>
                <tr>
                  <th class="rowHeader">
                    <cb:ToolTipLabel
                      ID="CustomerPasswordMaxAttemptsLabel"
                      runat="server"
                      Text="Maximum Login Failures:"
                      ToolTip="The number of times in a row that a login attempt can fail before the account is locked."
                    ></cb:ToolTipLabel>
                  </th>
                  <td>
                    <asp:TextBox
                      ID="CustomerPasswordMaxAttempts"
                      runat="server"
                      Width="40px"
                      MaxLength="2"
                    ></asp:TextBox>
                    <asp:RequiredFieldValidator
                      ID="CustomerPasswordMaxAttemptsValidator1"
                      runat="server"
                      ControlToValidate="CustomerPasswordMaxAttempts"
                      Text="*"
                      ErrorMessage="You must specify the maximum number of login failures."
                    ></asp:RequiredFieldValidator>
                    <asp:RangeValidator
                      ID="CustomerPasswordMaxAttemptsValidator2"
                      runat="server"
                      ControlToValidate="CustomerPasswordMaxAttempts"
                      Text="*"
                      ErrorMessage="Maximum number of login failures must be an integer between 1 and 20"
                      type="integer"
                      MinimumValue="1"
                      MaximumValue="20"
                    ></asp:RangeValidator>
                  </td>
                </tr>
                <tr>
                  <th class="rowHeader">
                    <cb:ToolTipLabel
                      ID="CustomerPasswordLockoutPeriodLabel"
                      runat="server"
                      Text="Lockout Period:"
                      ToolTip="The number of minutes an account will be locked after the maximum number of failed login attempts is reached."
                    ></cb:ToolTipLabel>
                  </th>
                  <td>
                    <asp:TextBox
                      ID="CustomerPasswordLockoutPeriod"
                      runat="server"
                      Width="40px"
                      MaxLength="3"
                    ></asp:TextBox>
                    <asp:Label
                      ID="CustomerPasswordLockoutPeriodLabel2"
                      runat="server"
                      Text=" minutes"
                    ></asp:Label>
                    <asp:RequiredFieldValidator
                      ID="CustomerPasswordLockoutPeriodValidator1"
                      runat="server"
                      ControlToValidate="CustomerPasswordLockoutPeriod"
                      Text="*"
                      ErrorMessage="You must specify the account lockout period."
                    ></asp:RequiredFieldValidator>
                    <asp:RangeValidator
                      ID="CustomerPasswordLockoutPeriodValidator2"
                      runat="server"
                      ControlToValidate="CustomerPasswordLockoutPeriod"
                      Text="*"
                      ErrorMessage="Account lockout period must be an integer between 1 and 999"
                      type="integer"
                      MinimumValue="1"
                      MaximumValue="999"
                    ></asp:RangeValidator>
                  </td>
                </tr>
                <tr>
                  <th class="rowHeader">
                    <cb:ToolTipLabel
                      ID="CustomerImageCaptchaLabel"
                      runat="server"
                      Text="Use Captcha:"
                      ToolTip="If checked, an image captcha is required for login."
                    ></cb:ToolTipLabel>
                  </th>
                  <td>
                    <asp:CheckBox ID="CustomerImageCaptcha" runat="server" />
                  </td>
                </tr>
              </table>
            </div>
          </td>
        </tr>
        <tr>
          <td colspan="2" align="center">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
            <asp:Button
              ID="SaveButton"
              runat="server"
              Text="Save Changes"
              OnClick="SaveButton_Click"
            />
          </td>
        </tr>
      </table>
    </ContentTemplate>
  </ajax:UpdatePanel>
</asp:Content>
