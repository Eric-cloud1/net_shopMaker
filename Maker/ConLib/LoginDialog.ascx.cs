using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Common;
using MakerShop.Users;
using MakerShop.Utility;

public partial class ConLib_LoginDialog : System.Web.UI.UserControl
{
    private string _LastPasswordValue = string.Empty;

    private bool _EnableAdminRememberMe = true;
    public bool EnableAdminRememberMe
    {
        get { return _EnableAdminRememberMe; }
        set { _EnableAdminRememberMe = value; }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        LoadCustomViewState();
    }

    private void LoadCustomViewState()
    {
        if (Page.IsPostBack)
        {
            string vsContent = Request.Form[VS.UniqueID];
            string decodedContent = EncryptionHelper.DecryptAES(vsContent);
            UrlEncodedDictionary customViewState = new UrlEncodedDictionary(decodedContent);
            _LastPasswordValue = customViewState.TryGetValue("P");
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        InstructionText.Text = string.Format(InstructionText.Text, Token.Instance.Store.Name);
        if (!Page.IsPostBack)
        {
            HttpCookie usernameCookie = Request.Cookies["UserName"];
            if ((usernameCookie != null) && !string.IsNullOrEmpty(usernameCookie.Value))
            {
                UserName.Text = usernameCookie.Value;
                RememberUserName.Checked = true;
                Password.Focus();
            }
            else
            {
                UserName.Focus();
            }
            PasswordExpiredPanel.Visible = false;
            ForgotPasswordPanel.Visible = false;
            EmailSentPanel.Visible = false;
            CustomerPasswordPolicy policy = new CustomerPasswordPolicy();
            if (Request.Url.Host.ToString() == "localhost")
            {
                trCaptchaField.Visible = false;
                trCaptchaImage.Visible = false;
            }
            else
            {
                //trCaptchaField.Visible = policy.ImageCaptcha;
                //trCaptchaImage.Visible = trCaptchaField.Visible;
                trCaptchaField.Visible = policy.ImageCaptcha;
                trCaptchaImage.Visible = policy.ImageCaptcha;
            }
            
        }

        PageHelper.ConvertEnterToTab(UserName);
        // SET DEFALUT BUTTONS FOR INPUT FIELDS
        if (trCaptchaImage.Visible)
        {
            PageHelper.ConvertEnterToTab(Password);
            WebControl inputControl = CaptchaImage.FindControl("CaptchaInput") as WebControl;
            if (inputControl != null) PageHelper.SetDefaultButton(inputControl, LoginButton.ClientID);
        }
        else
        {
            PageHelper.SetDefaultButton(Password, LoginButton.ClientID);
        }
    }

    protected void LoginButton_Click(object sender, EventArgs e)
    {
        _LastPasswordValue = Password.Text;
        User loginUser = UserDataSource.LoadForUserName(UserName.Text);
        if (loginUser != null)
        {
            bool stillNeedsCaptcha = false;
            if ((loginUser.IsAdmin) && (!trCaptchaField.Visible))
            {
                stillNeedsCaptcha = (new MerchantPasswordPolicy()).ImageCaptcha;
            }
            if (!stillNeedsCaptcha)
            {
                //EITHER THIS IS NOT AN ADMIN USER, OR THE CAPTCHA IS ALREADY VISIBLE
                if ((!trCaptchaField.Visible) || (CaptchaImage.Authenticate(CaptchaInput.Text)))
                {
                    //CAPTCHA IS HIDDEN OR VALIDATED, PROCEED WITH LOGIN ATTEMPT
                    if (User.Login(UserName.Text, Password.Text))
                    {
                        //LOGIN SUCCEEDED, MIGRATE USER IF NEEDED
                        int newUserId = loginUser.UserId;
                        int oldUserId = Token.Instance.UserId;
                        if ((oldUserId != newUserId) && (newUserId != 0))
                        {
                            User.Migrate(Token.Instance.User, UserDataSource.Load(newUserId));
                            Token.Instance.UserId = newUserId;
                        }
                        //HANDLE LOGIN PROCESSING
                        if (trRememberMe.Visible && RememberUserName.Checked)
                        {
                            HttpCookie cookie = new HttpCookie("UserName", UserName.Text);
                            cookie.Expires = DateTime.MaxValue;
                            Response.Cookies.Add(cookie);
                        }
                        else
                        {
                            Response.Cookies.Add(new HttpCookie("UserName", ""));
                        }
                        //CHECK FOR EXPIRED PASSWORDS
                        PasswordPolicy policy;
                        if (loginUser.IsAdmin) policy = new MerchantPasswordPolicy();
                        else policy = new CustomerPasswordPolicy();
                        if (policy.IsPasswordExpired(loginUser))
                        {
                            ShowPasswordExpired(policy, loginUser);
                        }
                        else
                        {
                            //REDIRECT TO THE STANDARD PAGE
                            FormsAuthentication.RedirectFromLoginPage(UserName.Text, false);
                        }
                    }
                    else
                    {
                        if (loginUser != null)
                        {
                            if (!loginUser.IsApproved)
                            {
                                AccountDisabled.IsValid = false;
                            }
                            else
                            {
                                PasswordPolicy policy;
                                if (loginUser.IsAdmin) policy = new MerchantPasswordPolicy();
                                else policy = new CustomerPasswordPolicy();
                                int remainingTries = policy.MaxAttempts - loginUser.FailedPasswordAttemptCount;
                                if (!loginUser.IsLockedOut && remainingTries > 0)
                                {
                                    InvalidLogin.ErrorMessage += " You have {0} tries remaining.";
                                    InvalidLogin.ErrorMessage = String.Format(InvalidLogin.ErrorMessage, remainingTries);
                                    InvalidLogin.IsValid = false;
                                }
                                else
                                {
                                    AccountLocked.ErrorMessage = String.Format(AccountLocked.ErrorMessage, policy.LockoutPeriod);
                                    AccountLocked.IsValid = false;
                                }
                            }
                        }
                        else
                        {
                            InvalidLogin.IsValid = false;
                        }
                    }
                }
                else
                {
                    //CAPTCHA IS VISIBLE AND DID NOT AUTHENTICATE
                    CustomValidator invalidInput = new CustomValidator();
                    invalidInput.ValidationGroup = "Login";
                    invalidInput.Text = "*";
                    invalidInput.ErrorMessage = "You did not input the verification number correctly.";
                    invalidInput.IsValid = false;
                    phCaptchaValidators.Controls.Add(invalidInput);
					CaptchaInput.Text = "";
                    Password.Attributes.Add("value", string.Empty);
                    RefreshCaptcha();
                }
            }
            else
            {
                //THIS IS AN ADMIN USER AND CAPTCHA IS NOT DISPLAYED YET
                trCaptchaField.Visible = true;
                trCaptchaImage.Visible = true;
                trRememberMe.Visible = _EnableAdminRememberMe;
                CaptchaImage.ChallengeText = StringHelper.RandomNumber(6);
                CustomValidator needsCaptcha = new CustomValidator();
                needsCaptcha.ValidationGroup = "Login";
                needsCaptcha.Text = "*";
                needsCaptcha.ErrorMessage = "Please type the verification number to log in.";
                needsCaptcha.IsValid = false;
                phCaptchaValidators.Controls.Add(needsCaptcha);
                Password.Attributes.Add("value", Password.Text);
            }
        }
        else
        {
            //THIS IS AN INVALID USER NAME
            InvalidLogin.IsValid = false;
        }
    }

    private void ShowPasswordExpired(PasswordPolicy policy, User user)
    {
        LoginPanel.Visible = false;
        PasswordExpiredPanel.Visible = true;
        List<string> requirements = new List<string>();
        PasswordPolicyLength.Text = string.Format(PasswordPolicyLength.Text, policy.MinLength);
        ppHistoryCount.Visible = (policy.HistoryCount > 0);
        if (ppHistoryCount.Visible) PasswordPolicyHistoryCount.Text = string.Format(PasswordPolicyHistoryCount.Text, policy.HistoryCount);
        ppHistoryDays.Visible = (policy.HistoryDays > 0);
        if (ppHistoryDays.Visible) PasswordPolicyHistoryDays.Text = string.Format(PasswordPolicyHistoryDays.Text, policy.HistoryDays);
        if (policy.RequireUpper) requirements.Add("uppercase letter");
        if (policy.RequireLower) requirements.Add("lowercase letter");
        if (policy.RequireNumber) requirements.Add("number");
        if (policy.RequireSymbol) requirements.Add("symbol");
        if (!policy.RequireNumber && !policy.RequireSymbol && policy.RequireNonAlpha) requirements.Add("non-letter");
        
        ppPolicyRequired.Visible = (requirements.Count > 0);
        if (ppPolicyRequired.Visible)
        {
            if (requirements.Count > 1) requirements[requirements.Count - 1] = "and " + requirements[requirements.Count - 1];
            PasswordPolicyRequired.Text = string.Format(PasswordPolicyRequired.Text, string.Join(", ", requirements.ToArray()));
        }
    }

    protected void ForgotPasswordButton_Click(object sender, EventArgs e)
    {
        LoginPanel.Visible = false;
        ForgotPasswordPanel.Visible = true;
        ForgotPasswordUserName.Text = UserName.Text;
    }

    protected void ForgotPasswordCancelButton_Click(object sender, EventArgs e)
    {
        LoginPanel.Visible = true;
        ForgotPasswordPanel.Visible = false;
    }

    protected void ForgotPasswordNextButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            User user = null;
            UserCollection users = UserDataSource.LoadForEmail(ForgotPasswordUserName.Text);
            if (users.Count != 0) user = users[0];
            if (user != null && user.IsApproved)
            {
                user.GeneratePasswordRequest();
                ForgotPasswordPanel.Visible = false;
                EmailSentPanel.Visible = true;
                EmailSentHelpText.Text = string.Format(EmailSentHelpText.Text, user.Email);
            }
            else
            {
                if ((user != null) && (!user.IsApproved)) DisabledUsernameValidator.IsValid = false;
                else ForgotPasswordUserNameValidator.IsValid = false;
            }
        }
    }

    protected void KeepShoppingButton_Click(object sender, EventArgs e)
    {
        Response.Redirect(NavigationHelper.GetLastShoppingUrl());
    }

    protected void ChangePasswordButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            //VERIFY THE GIVEN USERNAME IS VALID
            User user = UserDataSource.LoadForUserName(UserName.Text);
            if ((user != null) && !string.IsNullOrEmpty(UserName.Text) && !string.IsNullOrEmpty(_LastPasswordValue))
            {
                //VERIFY CURRENT PASSWORD IS CORRECT
                if (User.Login(UserName.Text, _LastPasswordValue))
                {
                    //VERIFY THE NEW PASSWORD MEETS POLICY
                    PasswordPolicy policy;
                    if (user.IsAdmin) policy = new MerchantPasswordPolicy();
                    else policy = new CustomerPasswordPolicy();
                    PasswordTestResult result = policy.TestPasswordWithFeedback(user, NewPassword.Text);
                    if ((result & PasswordTestResult.Success) == PasswordTestResult.Success && !NewPassword.Text.Equals(_LastPasswordValue))
                    {
                        user.SetPassword(NewPassword.Text);
                        //LOGIN SUCCEEDED, REDIRECT
                        FormsAuthentication.RedirectFromLoginPage(UserName.Text, false);
                    }
                    else
                    {
                        //REDISPLAY THE PASSWORD REQUIREMENST
                        ShowPasswordExpired(policy, user);

                        //"Your new password did not meet the following minimum requirements:<br>";
                        if ((result & PasswordTestResult.PasswordTooShort) == PasswordTestResult.PasswordTooShort) AddPasswordExpiredValidator(string.Format(PasswordPolicyLength.Text, policy.MinLength));
                        if ((result & PasswordTestResult.RequireLower) == PasswordTestResult.RequireLower) AddPasswordExpiredValidator("New password must contain at least one lowercase letter.<br>");
                        if ((result & PasswordTestResult.RequireUpper) == PasswordTestResult.RequireUpper) AddPasswordExpiredValidator("New password must contain at least one uppercase letter.<br> ");
                        if ((result & PasswordTestResult.RequireNonAlpha) == PasswordTestResult.RequireNonAlpha) AddPasswordExpiredValidator("New password must contain at least one non-letter.<br> ");
                        if ((result & PasswordTestResult.RequireNumber) == PasswordTestResult.RequireNumber) AddPasswordExpiredValidator("New password must contain at least one number.<br> ");
                        if ((result & PasswordTestResult.RequireSymbol) == PasswordTestResult.RequireSymbol) AddPasswordExpiredValidator("New password must contain at least one symbol.<br> ");

                        if ((result & PasswordTestResult.PasswordHistoryLimitation) == PasswordTestResult.PasswordHistoryLimitation)
                        {
                            AddPasswordExpiredValidator("You have recently used this password.<br/>");
                        }
                        if (NewPassword.Text.Equals(_LastPasswordValue))
                        {
                            AddPasswordExpiredValidator("You new password must be different from your current password.<br/>");
                        }
                    }
                }
            }
        }
    }

    private void AddPasswordExpiredValidator(String message)
    {
        CustomValidator validator = new CustomValidator();
        validator.ValidationGroup = "PasswordExpired";
        validator.ErrorMessage = message;
        validator.Text = "*";
        validator.IsValid = false;
        phNewPasswordValidators.Controls.Add(validator);
    }

    private void RefreshCaptcha()
    {
        CaptchaImage.ChallengeText = StringHelper.RandomNumber(6);
    }

    protected void ChangeImageLink_Click(object sender, EventArgs e)
    {
        RefreshCaptcha();
        Password.Attributes.Add("value", Password.Text);
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        SaveCustomViewState();
    }

    private void SaveCustomViewState()
    {
        UrlEncodedDictionary customViewState = new UrlEncodedDictionary();
        customViewState["P"] = _LastPasswordValue;
        VS.Value = EncryptionHelper.EncryptAES(customViewState.ToString());
    }
}
