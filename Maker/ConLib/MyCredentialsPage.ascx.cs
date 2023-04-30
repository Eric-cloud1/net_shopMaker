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

public partial class ConLib_MyCredentialsPage : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        User user = Token.Instance.User;
        if (!Page.IsPostBack)
        {
            string userName = user.UserName;
            bool isAnonymous = userName.StartsWith("zz_anonymous_");
            if (!isAnonymous)
            {
                UserName.Text = userName;
                Email.Text = user.Email;
                NewAccountText.Visible = false;
                UpdateAccountText.Visible = true;
            }
            else
            {
                //TRY TO DEFAULT THE EMAIL ADDRESS TO LAST ORDER EMAIL
                int orderCount = user.Orders.Count;
                if (orderCount > 0)
                {
                    UserName.Text = user.Orders[orderCount - 1].BillToEmail;
                    Email.Text = UserName.Text;
                }
                //CONFIGURE SCREEN FOR NEW ACCOUNT CREATION
                trCurrentPassword.Visible = false;
                NewAccountText.Visible = true;
                UpdateAccountText.Visible = false;
            }
            ConfirmPanel.Visible = false;
        }

        PasswordPolicy policy;
        if (user.IsAdmin) policy = new MerchantPasswordPolicy();
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

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("MyAccount.aspx");
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            User user = Token.Instance.User;
            string currentUserName = user.UserName;
            bool isAnonymous = currentUserName.StartsWith("zz_anonymous_");

            //VALIDATE THE PASSWORD IF THIS IS NOT AN ANONYMOUS USER
            bool validPassword;
            if (!isAnonymous)
            {
                validPassword = Membership.ValidateUser(currentUserName, CurrentPassword.Text);
                if (!validPassword)
                {
                    InvalidPassword.IsValid = false;
                    return;
                }
            }
            else validPassword = true;

            //VALIDATE NEW PASSWORD AGASINT POLICY
            if (Password.Text.Length > 0)
            {
                PasswordPolicy policy;
                if (user.IsAdmin) policy = new MerchantPasswordPolicy();
                else policy = new CustomerPasswordPolicy();
                PasswordTestResult result = policy.TestPasswordWithFeedback(user, Password.Text);
                if ((result & PasswordTestResult.Success) != PasswordTestResult.Success)
                {
                    PasswordPolicyValidator.ErrorMessage += "<UL>";
                    if ((result & PasswordTestResult.PasswordTooShort) == PasswordTestResult.PasswordTooShort) AddPwdValidationError(string.Format(PasswordPolicyLength.Text, policy.MinLength));
                    if ((result & PasswordTestResult.RequireLower) == PasswordTestResult.RequireLower) AddPwdValidationError("New password must contain at least one lowercase letter.");
                    if ((result & PasswordTestResult.RequireUpper) == PasswordTestResult.RequireUpper) AddPwdValidationError("New password must contain at least one uppercase letter. ");
                    if ((result & PasswordTestResult.RequireNonAlpha) == PasswordTestResult.RequireNonAlpha) AddPwdValidationError("New password must contain at least one non-letter.");
                    if ((result & PasswordTestResult.RequireNumber) == PasswordTestResult.RequireNumber) AddPwdValidationError("New password must contain at least one number.");
                    if ((result & PasswordTestResult.RequireSymbol) == PasswordTestResult.RequireSymbol) AddPwdValidationError("New password must contain at least one symbol.");
                    if ((result & PasswordTestResult.PasswordHistoryLimitation) == PasswordTestResult.PasswordHistoryLimitation)
                    {
                        AddPwdValidationError("You have recently used this password.");
                    }
                    PasswordPolicyValidator.ErrorMessage += "</UL>";
                    PasswordPolicyValidator.IsValid = false;
                    return;
                }
            }
            else if (isAnonymous)
            {
                //PASSWORD IS REQUIRED FOR NEW ANONYMOUS ACCOUNTS
                PasswordRequiredValidator.IsValid = false;
                return;
            }

            // IF USERNAME IS CHANGED, VALIDATE THE NEW NAME IS AVAILABLE
            string newUserName = UserName.Text.Trim();
            bool userNameChanged = (currentUserName != newUserName);
            if (userNameChanged)
            {
                // CHECK IF THERE IS ALREADY A USER WITH DESIRED USERNAME
                if (UserDataSource.GetUserIdByUserName(newUserName) > 0)
                {
                    // A USER ALREADY EXISTS WITH THAT NAME
                    phUserNameUnavailable.Visible = true;
                    return;
                }
            }

            // UPDATE THE USER RECORD WITH NEW VALUES
            user.Email = Email.Text.Trim();
            user.PrimaryAddress.Email = user.Email;
            user.UserName = newUserName;
            user.Save();

            // RESET AUTH COOKIE WITH NEW USERNAME IF NEEDED
            if (userNameChanged) FormsAuthentication.SetAuthCookie(newUserName, false);

            //UPDATE PASSWORD IF INDICATED
            if (Password.Text.Length > 0)
            {
                user.SetPassword(Password.Text);
            }
            EditPanel.Visible = false;
            ConfirmPanel.Visible = true;
        }
    }

    private void AddPwdValidationError(string message)
    {        
        PasswordPolicyValidator.ErrorMessage += message;
    }
}
