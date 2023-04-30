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
using MakerShop.Marketing;
using MakerShop.Utility;

public partial class ConLib_SubscribeToEmailList : System.Web.UI.UserControl
{
    private int _EmailListId = 0;
    private string _Caption = "Subscribe To Email List";

    [Personalizable(), WebBrowsable()]
    public string Caption
    {
        get { return _Caption; }
        set { _Caption = value; }
    }

    [Personalizable(), WebBrowsable()]
    public int EmailListId
    {
        get { return _EmailListId; }
        set { _EmailListId = value; }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        if (_EmailListId == 0)
        {
            _EmailListId = Token.Instance.Store.Settings.DefaultEmailListId;
        }
    }

    protected void SubscribeButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            if (_EmailListId == 0)
            {
                return;
            }

            EmailList emailList = EmailListDataSource.Load(_EmailListId);
            if (emailList != null)
            {
                string email = UserEmail.Text;
                bool subscribedNow = false;
                bool alreadySubscribed = false;
                if (emailList.IsMember(email))
                {
                    alreadySubscribed = true;
                }
                else
                {
                    //subscribe this user to this email list
                    //emailList.EmailListUsers.Add(new EmailListUser(emailList.EmailListId, userId));
                    //emailList.EmailListUsers.Save();
                    emailList.ProcessSignupRequest(email);
                    subscribedNow = true;
                }
                if (subscribedNow)
                {
                    SubscribedMessage.Text = string.Format(SubscribedMessage.Text, email, emailList.Name);
                    SubscribedMessage.Visible = true;
                }
                else if (alreadySubscribed)
                {
                    SubscribedMessage.Text = string.Format("The email '{0}' is already subscribed to '{1}'.", email, emailList.Name);
                    SubscribedMessage.Visible = true;
                }
            }
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (_EmailListId == 0)
        {
            this.Visible = false;
            return;
        }

        EmailList emailList = EmailListDataSource.Load(_EmailListId);
        if (emailList == null)
        {
            this.Visible = false;
            return;
        }

        CaptionLabel.Text = this.Caption;
        if (!Token.Instance.User.IsAnonymous && !String.IsNullOrEmpty(Token.Instance.User.Email))
        {
            UserEmail.Text = Token.Instance.User.Email;
        }

        InstructionsText.Text = string.Format(InstructionsText.Text, emailList.Name);
        this.Visible = true;
    }
}