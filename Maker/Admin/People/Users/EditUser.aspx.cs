using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using MakerShop.Common;
using MakerShop.Orders;
using MakerShop.Shipping;
using MakerShop.Stores;
using MakerShop.Users;
using MakerShop.Utility;

public partial class Admin_People_Users_EditUser : MakerShop.Web.UI.MakerShopAdminPage
{
    private int _UserId;
    private User _User;

    protected void Page_Init(object sender, EventArgs e)
    {
        _UserId = AlwaysConvert.ToInt(Request.QueryString["UserId"]);
        _User = UserDataSource.Load(_UserId);

        
        if (_User == null) Response.Redirect("Default.aspx");
        // NON SUPER USERS CANNOT EDIT SUPER USERS ACCOUNT
        if (_User.IsSystemAdmin && !Token.Instance.User.IsSystemAdmin) Response.Redirect("Default.aspx");

        // INITIALIZE CAPTION
        Caption.Text = string.Format(Caption.Text, _User.IsAnonymous ? "anonymous" : _User.UserName);

        if (!_User.IsAnonymous)
        {
            // INITIALIZE THE FORM
            InitializeAccountPage();
        }
        else
        {
            // THE USER IS ANONYMOUS, ONLY CURRENT BASKET AND PAGE VIEWS SHOULD DISPLAY
            AccountTab.Visible = false;
            AddressTab.Visible = false;
            OrderHistoryDialog1.Visible = false;
        }
      //  PageViewsTab.Visible = Store.GetCachedSettings().PageViewTrackingEnabled;

        // ATTEMPT TO RETAIN PAGE STATE
        int pageIndex = AlwaysConvert.ToInt(Request.Form[EditUserPages.ClientID + "_SelectedIndex"]);
        if (pageIndex > -1 && pageIndex < EditUserPages.PageViews.Count)
        {
            EditUserPages.SelectedIndex = pageIndex;
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!_User.IsAnonymous)
        {
            // HANDLE GROUP DISPLAY HERE TO REACT TO CHANGES
            GroupList.Text = GetGroupList();
            GroupListChanged.Value = string.Empty;
            InitializeChangeGroupsDialog();
            SubscriptionsPanel.Visible = (SubscriptionGrid.Rows.Count > 0);
        }
        else
        {
            EditUserTabs.SelectedTab = RelatedOrderTab;
            EditUserPages.SelectPageById("RelatedOrdersPage");
        }



    }

    #region Account Page
    protected void InitializeAccountPage()
    {
        // INITIALIZE LEFT COLUMN WITH ADJUSTABLE ACCOUNT SETTINGS
        UserName.Text = _User.UserName;
        Email.Text = _User.Email;
        InitializeChangeGroupsJS();
        IsDisabled.Enabled = (_User.UserId != Token.Instance.UserId);
        IsDisabled.Checked = !_User.IsApproved;

        // INITIALIZE RIGHT COLUMN OF PASSWORD DETAILS
        RegisteredSinceDate.Text = _User.CreateDate.ToString("g");
        if (_User.LastActivityDate > System.DateTime.MinValue)
        {
            LastActiveDate.Text = _User.LastActivityDate.ToString("g");
        }
        FailedLoginCount.Text = _User.FailedPasswordAttemptCount.ToString();
        if (_User.LastLockoutDate > System.DateTime.MinValue)
        {
            LastLockoutDate.Text = _User.LastLockoutDate.ToString("g");
        }
        if (_User.Passwords.Count > 0)
        {
            TimeSpan ts = LocaleHelper.LocalNow - _User.Passwords[0].CreateDate;
            string timeSpanPhrase;
            if (ts.Days > 0) timeSpanPhrase = ts.Days.ToString() + " days";
            else if (ts.Hours > 0) timeSpanPhrase = ts.Hours.ToString() + " hours";
            else timeSpanPhrase = ts.Minutes.ToString() + " minutes";
            PasswordLastChangedText.Text = string.Format(PasswordLastChangedText.Text, timeSpanPhrase);
        }
        else
        {
            PasswordLastChangedText.Visible = false;
        }

        // DISPLAY POLICY ON CHANGE PASSWORD FORM
        PasswordPolicy policy;
        if (_User.IsAdmin) policy = new MerchantPasswordPolicy();
        else policy = new CustomerPasswordPolicy();
        PasswordPolicyLength.Text = string.Format(PasswordPolicyLength.Text, policy.MinLength);
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

    /// <summary>
    /// Initializes javascript required by the change groups dialog
    /// </summary>
    private void InitializeChangeGroupsJS()
    {
        this.Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "selectbox", this.ResolveClientUrl("~/js/selectbox.js"));
        string leftBoxName = AvailableGroups.ClientID;
        string rightBoxName = SelectedGroups.ClientID;
        AvailableGroups.Attributes.Add("onDblClick", "moveSelectedOptions(this.form['" + leftBoxName + "'], this.form['" + rightBoxName + "'], true, '')");
        SelectedGroups.Attributes.Add("onDblClick", "moveSelectedOptions(this.form['" + rightBoxName + "'], this.form['" + leftBoxName + "'], true, '');");
        SelectAllGroups.Attributes.Add("onClick", "moveAllOptions(this.form['" + leftBoxName + "'], this.form['" + rightBoxName + "'], true, ''); return false;");
        SelectGroup.Attributes.Add("onClick", "moveSelectedOptions(this.form['" + leftBoxName + "'], this.form['" + rightBoxName + "'], true, ''); return false;");
        UnselectGroup.Attributes.Add("onClick", "moveSelectedOptions(this.form['" + rightBoxName + "'], this.form['" + leftBoxName + "'], true, ''); return false;");
        UnselectAllGroups.Attributes.Add("onClick", "moveAllOptions(this.form['" + rightBoxName + "'], this.form['" + leftBoxName + "'], true, ''); return false;");
        StringBuilder changeGroupListScript = new StringBuilder();
        changeGroupListScript.AppendLine("function changeGroupList(){");
        changeGroupListScript.AppendLine("\t$get('" + HiddenSelectedGroups.ClientID + "').value=getOptions($get('" + rightBoxName + "'));");
        changeGroupListScript.AppendLine("\t$get('" + GroupList.ClientID + "').innerHTML=getOptionNames($get('" + rightBoxName + "'));");
        changeGroupListScript.AppendLine("}");
        this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "changeGroupList", changeGroupListScript.ToString(), true);
    }

    /// <summary>
    /// Initializes the change groups dialog with current user group settings
    /// </summary>
    private void InitializeChangeGroupsDialog()
    {
        AvailableGroups.Items.Clear();
        SelectedGroups.Items.Clear();
        GroupCollection allGroups = GroupDataSource.LoadForStore("Name");


        // IF THE USER IS A NON-SYSTEM USER REMOVE THE SYSTEM GROUP FROM THE LIST
        if (!Token.Instance.User.IsSystemAdmin)
        {
            int userId = Token.Instance.UserId;
            allGroups = GroupDataSource.LoadForUserRoles(userId);

            MakerShop.Users.GroupCollection systemGroups = new MakerShop.Users.GroupCollection();
            foreach (MakerShop.Users.Group group in allGroups)
            {
                if (group.IsInRole("System")) systemGroups.Add(group);
            }

            // REMOVE THE GROUPS
            foreach (MakerShop.Users.Group group in systemGroups)
            {
                allGroups.Remove(group);
            }
        }

        foreach (Group c in allGroups)
        {
            ListItem newItem = new ListItem(c.Name, c.GroupId.ToString());
            bool groupSelected = (_User.UserGroups.IndexOf(_UserId, c.GroupId) > -1);
            if (groupSelected) SelectedGroups.Items.Add(newItem);
            else AvailableGroups.Items.Add(newItem);
        }
        phMyGroupsWarning.Visible = (_UserId == Token.Instance.UserId);
    }

    /// <summary>
    /// Gets a comma delimited list of assigned group names for the current user
    /// </summary>
    /// <returns>Comma delimited list of group names, or the empty text if no 
    /// groups are assigned to the user</returns>
    protected string GetGroupList()
    {
        List<string> groupNames = new List<string>();
        foreach (UserGroup ur in _User.UserGroups)
        {
            if (ur.Group != null)
            {
                groupNames.Add(ur.Group.Name);
            }
        }
        if (groupNames.Count == 0) return string.Empty;
        return string.Join(", ", groupNames.ToArray());
    }

    protected void ChangeGroupListOKButton_Click(object sender, System.EventArgs e)
    {
        //REMOVE ALL GROUPS ASSOCIATED WITH USER
        _User.UserGroups.DeleteAll();

        // VALIDATE THE SUBMITTED GROUPS AND UPDATE
        int[] selectedGroups = AlwaysConvert.ToIntArray(Request.Form[HiddenSelectedGroups.UniqueID]);
        _User.UserGroups.DeleteAll();
        if (selectedGroups != null && selectedGroups.Length > 0)
        {
            List<int> validGroups = GetValidGroupIds();
            foreach (int groupId in selectedGroups)
            {
                if (validGroups.Contains(groupId))
                {
                    _User.UserGroups.Add(new UserGroup(_UserId, groupId));
                }
            }
            _User.Save();
        }
    }

    private List<int> GetValidGroupIds()
    {
        List<int> allGroupIds = new List<int>();
        GroupCollection allGroups = GroupDataSource.LoadForStore();
        foreach (Group c in allGroups) allGroupIds.Add(c.GroupId);
        return allGroupIds;
    }

    protected void ChangePasswordOKButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid && EnforcePasswordPolicy())
        {
            string password = NewPassword.Text.Trim();
            _User.SetPassword(password, ForceExpiration.Checked);
            ChangePasswordPopup.Hide();
        }
        else
        {
            ChangePasswordPopup.Show();
        }
    }

    protected void ChangePasswordCancelButton_Click(object sender, EventArgs e)
    {
        NewPassword.Text = string.Empty;
        ConfirmNewPassword.Text = string.Empty;
        ChangePasswordPopup.Hide();
    }

    protected bool EnforcePasswordPolicy()
    {
        // DETERMINE THE APPROPRIATE POLICY FOR THE USER
        PasswordPolicy policy;
        if (_User.IsAdmin) policy = new MerchantPasswordPolicy();
        else policy = new CustomerPasswordPolicy();

        // CHECK IF PASSWORD MEETS POLICY
        PasswordTestResult result = policy.TestPasswordWithFeedback(NewPassword.Text.Trim());
        if ((result & PasswordTestResult.Success) == PasswordTestResult.Success) return true;

        // PASSWORD DOES NOT MEET POLICY
        StringBuilder newErrorMessage = new StringBuilder();
        newErrorMessage.Append(PasswordPolicyValidator.ErrorMessage + "<ul>");
        if ((result & PasswordTestResult.PasswordTooShort) == PasswordTestResult.PasswordTooShort) newErrorMessage.Append("<li>New password length must be at least " + policy.MinLength.ToString() + " characters.</li>");
        if ((result & PasswordTestResult.RequireLower) == PasswordTestResult.RequireLower) newErrorMessage.Append("<li>New password must contain at least one lowercase letter.<li>");
        if ((result & PasswordTestResult.RequireUpper) == PasswordTestResult.RequireUpper) newErrorMessage.Append("<li>New password must contain at least one uppercase letter.</li>");
        if ((result & PasswordTestResult.RequireNonAlpha) == PasswordTestResult.RequireNonAlpha) newErrorMessage.Append("<li>New password must contain at least one non-letter.</li>");
        if ((result & PasswordTestResult.RequireNumber) == PasswordTestResult.RequireNumber) newErrorMessage.Append("<li>New password must contain at least one number.</li> ");
        if ((result & PasswordTestResult.RequireSymbol) == PasswordTestResult.RequireSymbol) newErrorMessage.Append("<li>New password must contain at least one symbol.</li>");
        PasswordPolicyValidator.ErrorMessage = newErrorMessage.ToString() + "</ul>";
        PasswordPolicyValidator.IsValid = false;
        return false;
    }

    private bool ValidateNewUserName(string newUserName)
    {
        if (!_User.UserName.Equals(newUserName, StringComparison.InvariantCultureIgnoreCase))
        {
            //user name has been changed. verify if new user name is available
            int existingUserId = UserDataSource.GetUserIdByUserName(newUserName);
            if (existingUserId > 0)
            {
                UserNameAvailableValidator.IsValid = false;
                return false;
            }
        }
        return true;
    }

    protected void SaveAccountButton_Click(object sender, EventArgs e)
    {
        string newUserName = UserName.Text.Trim();
        if (Page.IsValid && ValidateNewUserName(newUserName))
        {
            //UPDATE ACCOUNT SETTINGS
            _User.UserName = newUserName;
            _User.Email = Email.Text;

            // PREVENT DISABLING OF YOUR OWN ACCOUNT
            if (_User.UserId != Token.Instance.UserId)
                _User.IsApproved = !IsDisabled.Checked;

            // SAVE USER SETTINGS
            _User.Save();
            AccountSavedMessage.Text = string.Format(AccountSavedMessage.Text, LocaleHelper.LocalNow);
            AccountSavedMessage.Visible = true;
        }
    }
    #endregion

    protected string GetSubGroupName(object dataItem)
    {
        Subscription s = (Subscription)dataItem;
        Group g = GroupDataSource.Load(s.GroupId);
        if (g != null) return g.Name;
        else return string.Empty;
    }

    protected void SubscriptionGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int subscriptionId = AlwaysConvert.ToInt(e.CommandArgument);
        Subscription subscription = SubscriptionDataSource.Load(subscriptionId);
        switch (e.CommandName)
        {
            case "Activate":
                subscription.Activate();
                SubscriptionGrid.DataBind();
                break;
            case "CancelSubscription":
                subscription.Delete();
                SubscriptionGrid.DataBind();
                break;
        }
    }

    protected void SubscriptionGrid_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        int subscriptionId = (int)SubscriptionGrid.DataKeys[e.RowIndex].Value;
        Subscription subscription = SubscriptionDataSource.Load(subscriptionId);
        if (subscription != null)
        {
            subscription.ExpirationDate = AlwaysConvert.ToDateTime(e.NewValues["ExpirationDate"], DateTime.MinValue);
            subscription.Save();
        }
        SubscriptionGrid.EditIndex = -1;
        e.Cancel = true;
    }
    
    protected void BackButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }
}