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
using MakerShop.Users;
using MakerShop.Utility;

public partial class ConLib_PasswordHelpPage : System.Web.UI.UserControl
{
    private int _UserId;
    private User _User;

    protected void Page_Init(object sender, EventArgs e)
    {
        _UserId = AlwaysConvert.ToInt(Request.QueryString["Key"]);
        _User = UserDataSource.Load(_UserId);
        if ((_User == null) || (!_User.IsApproved)) Response.Redirect(NavigationHelper.GetHomeUrl());
        string tempPassword = AlwaysConvert.ToString(Request.QueryString["Check"]);
        if (string.IsNullOrEmpty(tempPassword) || (_User.Comment != tempPassword)) Response.Redirect(NavigationHelper.GetHomeUrl());
        if (!Page.IsPostBack)
        {
            UserName.Text = _User.UserName;
            // PASSWORD POLICY
            PasswordPolicy policy;
            if (_User.IsAdmin) policy = new MerchantPasswordPolicy();
            else policy = new CustomerPasswordPolicy();
            PasswordPolicyLength.Text = string.Format(PasswordPolicyLength.Text, policy.MinLength);
            PasswordPolicyHistoryCount.Visible = (policy.HistoryCount > 0);
            if (PasswordPolicyHistoryCount.Visible) PasswordPolicyHistoryCount.Text = string.Format(PasswordPolicyHistoryCount.Text, policy.HistoryCount);
            PasswordPolicyHistoryDays.Visible = (policy.HistoryDays > 0);
            if (PasswordPolicyHistoryDays.Visible) PasswordPolicyHistoryDays.Text = string.Format(PasswordPolicyHistoryDays.Text, policy.HistoryDays);
            List<string> requirements = new List<string>();
            if (policy.RequireUpper) requirements.Add("uppercase letter");
            if (policy.RequireLower) requirements.Add("lowercase letter");
            if (policy.RequireNumber) requirements.Add("number");
            if (policy.RequireSymbol) requirements.Add("symbol");
            if (!policy.RequireNumber && !policy.RequireSymbol && policy.RequireNonAlpha) requirements.Add("non-letter");
            PasswordPolicyRequired.Visible = (requirements.Count > 0);
            if (PasswordPolicyRequired.Visible)
            {
                if (requirements.Count > 1) requirements[requirements.Count - 1] = "and " + requirements[requirements.Count - 1];
                PasswordPolicyRequired.Text = string.Format(PasswordPolicyRequired.Text, string.Join(", ", requirements.ToArray()));
            }
        }
    }

    protected void SubmitButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            //VERIFY THE NEW PASSWORD MEETS POLICY
            PasswordPolicy policy;
            if (_User.IsAdmin) policy = new MerchantPasswordPolicy();
            else policy = new CustomerPasswordPolicy();            

            PasswordTestResult result = policy.TestPasswordWithFeedback(_User, Password.Text);
            if ((result & PasswordTestResult.Success) == PasswordTestResult.Success)
            {
                _User.SetPassword(Password.Text);
                _User.Comment = string.Empty;
                _User.Save();
                FormsAuthentication.SetAuthCookie(_User.UserName, false);
                Response.Redirect(NavigationHelper.GetHomeUrl());
            }
            else
            {                
                //Your password did not meet the following minimum requirements
                if ((result & PasswordTestResult.PasswordTooShort) == PasswordTestResult.PasswordTooShort) AddPasswordValidator("Password length must be at least " + policy.MinLength.ToString() + " characters.");
                if ((result & PasswordTestResult.RequireLower) == PasswordTestResult.RequireLower) AddPasswordValidator("Password must contain at least one lowercase letter.<br>");
                if ((result & PasswordTestResult.RequireUpper) == PasswordTestResult.RequireUpper) AddPasswordValidator("Password must contain at least one uppercase letter.<br> ");
                if ((result & PasswordTestResult.RequireNonAlpha) == PasswordTestResult.RequireNonAlpha) AddPasswordValidator("Password must contain at least one non-letter.<br> ");
                if ((result & PasswordTestResult.RequireNumber) == PasswordTestResult.RequireNumber) AddPasswordValidator("Password must contain at least one number.<br> ");
                if ((result & PasswordTestResult.RequireSymbol) == PasswordTestResult.RequireSymbol) AddPasswordValidator("Password must contain at least one symbol.<br> ");

                if ((result & PasswordTestResult.PasswordHistoryLimitation) == PasswordTestResult.PasswordHistoryLimitation)
                {
                    AddPasswordValidator("You have recently used this password.<br/>");
                }

            }
        }
    }

    private void AddPasswordValidator(String message)
    {
        CustomValidator validator = new CustomValidator();
        validator.ControlToValidate = "Password";        
        validator.ErrorMessage = message;        
        validator.Text = "*";
        validator.IsValid = false;
        phNewPasswordValidators.Controls.Add(validator);
    }
}
