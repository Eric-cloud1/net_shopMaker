using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Utility;
using MakerShop.Common;
using MakerShop.Users;
using System.Collections.Generic;

public partial class Admin_UserControls_LoginDialog : System.Web.UI.UserControl
{
    private string _LastPasswordValue = string.Empty;

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
            MerchantPasswordPolicy policy = new MerchantPasswordPolicy();


            string url = Request.Url.Host.ToString();

            if (Request.Url.Host.ToString() == "localhost")
            {
                trCaptchaField.Visible = false;
                trCaptchaImage.Visible = false;
            }
            else
            {
                trCaptchaField.Visible = policy.ImageCaptcha;
                trCaptchaImage.Visible = trCaptchaField.Visible;
            }
            if (trCaptchaField.Visible)
            {
                RefreshCaptcha();
            }
        }
    }

    protected void LoginButton_Click(object sender, EventArgs e)
    {
        _LastPasswordValue = Password.Text;
        if (Page.IsValid)
        {
            //VALIDATE CAPTCHA  
            if (!trCaptchaField.Visible || (CaptchaImage.Authenticate(CaptchaInput.Text)))
            {
                if (User.Login(UserName.Text, Password.Text))
                {
                    //MIGRATE USER IF NEEDED
                    int newUserId = UserDataSource.GetUserId(UserName.Text);
                    if ((Token.Instance.UserId != newUserId) && (newUserId != 0))
                    {
                        User.Migrate(Token.Instance.User, UserDataSource.Load(newUserId));
                        Token.Instance.UserId = newUserId;
                    }
                    //HANDLE LOGIN PROCESSING
                    if (RememberUserName.Checked)
                    {
                        HttpCookie cookie = new HttpCookie("UserName", UserName.Text);
                        cookie.Expires = DateTime.MaxValue;
                        Response.Cookies.Add(cookie);
                    }
                    else
                    {
                        Response.Cookies.Add(new HttpCookie("UserName", ""));
                    }
                    //REDIRECT ADMIN USERS TO THE MERCHANT ADMIN IF NO RETURNURL IS PRESENT
                    User newUser = UserDataSource.Load(newUserId);
                    MerchantPasswordPolicy policy = new MerchantPasswordPolicy();
                    if (policy.IsPasswordExpired(newUser))
                    {
                        ShowPasswordExpired();
                    }
                    else
                    {
                        //REDIRECT TO THE STANDARD PAGE
                        FormsAuthentication.RedirectFromLoginPage(UserName.Text, false);
                    }
                }
                else
                {
                    User user = UserDataSource.LoadForUserName(UserName.Text);
                    if (user != null)
                    {
                        if (!user.IsApproved)
                        {
                            AccountDisabled.IsValid = false;
                        }
                        else
                        {
                            MerchantPasswordPolicy policy = new MerchantPasswordPolicy();
                            int remainingTries = policy.MaxAttempts - user.FailedPasswordAttemptCount;
                            if (!user.IsLockedOut && remainingTries > 0)
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
                    RefreshCaptcha();
                }
            }
            else
            {
                CustomValidator invalidInput = new CustomValidator();
                invalidInput.ValidationGroup = "Login";
                invalidInput.Text = "*";
                invalidInput.ErrorMessage = "You did not input the verification number correctly.";
                invalidInput.IsValid = false;
                phCaptchaValidators.Controls.Add(invalidInput);
                RefreshCaptcha();
            }
        }
    }

    private void ShowPasswordExpired()
    {
        LoginPanel.Visible = false;
        PasswordExpiredPanel.Visible = true;
        MerchantPasswordPolicy policy = new MerchantPasswordPolicy();
        PasswordPolicyLength.Text = string.Format(PasswordPolicyLength.Text, policy.MinLength);
        ppHistoryCount.Visible = (policy.HistoryCount > 0);
        if (ppHistoryCount.Visible) PasswordPolicyHistoryCount.Text = string.Format(PasswordPolicyHistoryCount.Text, policy.HistoryCount);
        ppHistoryDays.Visible = (policy.HistoryDays > 0);
        if (ppHistoryDays.Visible) PasswordPolicyHistoryDays.Text = string.Format(PasswordPolicyHistoryDays.Text, policy.HistoryDays);
        List<string> requirements = new List<string>();
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
            User user = UserDataSource.LoadForUserName(ForgotPasswordUserName.Text);
            if (user != null)
            {
                user.GeneratePasswordRequest();
                ForgotPasswordPanel.Visible = false;
                EmailSentPanel.Visible = true;
                EmailSentHelpText.Text = string.Format(EmailSentHelpText.Text, user.Email);
            }
            else
            {
                ForgotPasswordUserNameValidator.IsValid = false;
            }
        }
    }

    protected void ForgotPasswordContinueButton_Click(object sender, EventArgs e)
    {
        LoginPanel.Visible = true;
        EmailSentPanel.Visible = false;
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
                    MerchantPasswordPolicy policy = new MerchantPasswordPolicy();
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
                        ShowPasswordExpired();

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
        CaptchaInput.Text = String.Empty;
    }

    protected void ChangeImageLink_Click(object sender, EventArgs e)
    {
        RefreshCaptcha();
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
