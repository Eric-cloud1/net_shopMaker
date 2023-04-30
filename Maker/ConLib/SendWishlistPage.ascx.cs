using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Common;
using MakerShop.Messaging;
using MakerShop.Utility;
using MakerShop.Users;

public partial class ConLib_SendWishlistPage : System.Web.UI.UserControl
{    

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
            InitSendWishlistForm();
    }

    private void InitSendWishlistForm()
    {
        User user = Token.Instance.User;
        FromAddress.Text = user.Email;
        Message.Text = GetMessage();
    }

    private String GetMessage()
    {
        Wishlist wishlist = Token.Instance.User.PrimaryWishlist;
        String wishlistUrl = Token.Instance.Store.StoreUrl + "ViewWishlist.aspx?WishlistId=" + wishlist.WishlistId;
        String wishlistLink = "<a href=\"" + wishlistUrl + "\">" + wishlistUrl + "</a>";
        StringBuilder messageFormat = new StringBuilder();
        messageFormat.Append("Dear Friend,\n\n");
        messageFormat.Append("I created a WishList at " + Token.Instance.Store.Name + ".\n\n");
        if (wishlist.ViewPassword.Length > 0)
        {
            messageFormat.Append("The password for my WishList is " + wishlist.ViewPassword + ".\n\n");
        }
        messageFormat.Append("Please visit the link below to view the list.\n");
        messageFormat.Append(wishlistUrl);
        messageFormat.Append("\n\nThank you!");
        return messageFormat.ToString();
    }

    protected void BackButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Members/MyWishlist.aspx");
    }

    protected void SendButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
            mailMessage.From = new System.Net.Mail.MailAddress(FromAddress.Text);
            mailMessage.To.Add(SendTo.Text);
            mailMessage.Subject = StringHelper.StripHtml(Subject.Text);
            mailMessage.Body = StringHelper.StripHtml(Message.Text);
            mailMessage.IsBodyHtml = false;
            EmailClient.Send(mailMessage);
            SentMessage.Text = String.Format(SentMessage.Text, SendTo.Text);
            SentMessage.Visible = true;
            SendTo.Text = string.Empty;
        }
    }
}