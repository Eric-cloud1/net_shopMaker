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
using MakerShop.Catalog;
using MakerShop.Common;
using MakerShop.Messaging;
using MakerShop.Products;
using MakerShop.Utility;

public partial class ConLib_ProductTellAFriend : System.Web.UI.UserControl
{
    private string _Caption = "Email A Friend";
    [Personalizable(), WebBrowsable()]
    public string Caption
    {
        get { return _Caption; }
        set { _Caption = value; }
    }

    protected void SendEmailButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            if ((!trCaptchaImage.Visible) || CaptchaImage.Authenticate(CaptchaInput.Text))
            {
                int productId = PageHelper.GetProductId();
                Product product = ProductDataSource.Load(productId);
                if (product != null)
                {
                    int categoryId = PageHelper.GetCategoryId();
                    Category category = CategoryDataSource.Load(categoryId);
                    EmailTemplate template = EmailTemplateDataSource.Load(Token.Instance.Store.Settings.ProductTellAFriendEmailTemplateId);
                    if (template != null)
                    {
                        //STRIP HTML
                        Name.Text = StringHelper.StripHtml(Name.Text);
                        FromEmail.Text = StringHelper.StripHtml(FromEmail.Text);
                        FriendEmail.Text = StringHelper.StripHtml(FriendEmail.Text);
                        // ADD PARAMETERS
                        template.Parameters["store"] = Token.Instance.Store;
                        template.Parameters["product"] = product;
                        template.Parameters["category"] = category;
                        template.Parameters["fromEmail"] = FromEmail.Text;
                        template.Parameters["fromName"] = Name.Text;
                        template.FromAddress = FromEmail.Text;
                        template.ToAddress = FriendEmail.Text;
                        template.Send();
                        FriendEmail.Text = string.Empty;
                        SentMessage.Visible = true;
                        CaptchaInput.Text = "";
                        CaptchaImage.ChallengeText = StringHelper.RandomNumber(6);
                    }
                    else
                    {
                        FailureMessage.Text = "Email template could not be loaded.";
                        FailureMessage.Visible = true;
                    }
                }
                else
                {
                    FailureMessage.Text = "Product could not be identified.";
                    FailureMessage.Visible = true;
                }
            }
            else
            {
                //CAPTCHA IS VISIBLE AND DID NOT AUTHENTICATE
                CustomValidator invalidInput = new CustomValidator();
                invalidInput.ValidationGroup = "TellAFriend";
                invalidInput.Text = "*";
                invalidInput.ErrorMessage = "You did not input the verification number correctly.";
                invalidInput.IsValid = false;
                phCaptchaValidators.Controls.Add(invalidInput);
                CaptchaInput.Text = "";
                CaptchaImage.ChallengeText = StringHelper.RandomNumber(6);
            }
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        CaptionLabel.Text = this.Caption;
        if (String.IsNullOrEmpty(FromEmail.Text) && !String.IsNullOrEmpty(Token.Instance.User.Email))
        {
            Name.Text = Token.Instance.User.PrimaryAddress.FullName;
            FromEmail.Text = Token.Instance.User.Email;
        }

        bool enableCaptcha = Token.Instance.Store.Settings.ProductTellAFriendCaptcha;
        trCaptchaCaption.Visible = enableCaptcha;
        trCaptchaImage.Visible = enableCaptcha;
        trCaptchaInputLabel.Visible = enableCaptcha;
        trCaptchaInput.Visible = enableCaptcha;
    }    
}