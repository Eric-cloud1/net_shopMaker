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
using MakerShop.Common;
using MakerShop.Users;

public partial class ConLib_CheckoutLoginDialog : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        InstructionText.Text = string.Format(InstructionText.Text, Token.Instance.Store.Name);
        PageHelper.ConvertEnterToTab(UserName);
        PageHelper.SetDefaultButton(Password, LoginButton.ClientID);
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
            ForgotPasswordPanel.Visible = false;
            EmailSentPanel.Visible = false;
        }
    }

    protected void LoginButton_Click(object sender, EventArgs e)
    {
        if (Membership.ValidateUser(UserName.Text, Password.Text))
        {
            //MIGRATE USER IF NEEDED
            int newUserId = UserDataSource.GetUserId(UserName.Text);
            if ((Token.Instance.UserId != newUserId) && (newUserId != 0))
            {
                User.Migrate(Token.Instance.User, UserDataSource.Load(newUserId));
                Token.Instance.UserId = newUserId;
                Token.Instance.User.Load(newUserId);
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
            //UPDATE AUTHORIZATION COOKIE
            FormsAuthentication.SetAuthCookie(UserName.Text, false);
            //REDIRECT TO CHECKOUT
            Response.Redirect(NavigationHelper.GetCheckoutUrl(true));
        }
        else
        {
            InvalidLogin.IsValid = false;
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

    protected void KeepShoppingButton_Click(object sender, EventArgs e)
    {
        Response.Redirect(NavigationHelper.GetLastShoppingUrl());
    }
}
