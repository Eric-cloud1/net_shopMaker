using System;
using MakerShop.Common;
using MakerShop.Users;
using MakerShop.Messaging;
using MakerShop.Stores;
using MakerShop.Utility;

namespace MakerShop.Marketing
{
    /// <summary>
    /// Class that represents an EmailList object in database
    /// </summary>
    public partial class EmailList
    {
        private const string ConfirmationMessage = "You are receiving this message because you (or someone pretending to be you) has signed up for to receive messages from the list $list.Name.  If you did not intend to sign up for this list, click the link below to remove your address.\r\n\r\n${store.StoreUrl}Subscription.aspx?action=remove&list=${list.EmailListId}&email=${email}";
        private const string VerificationMessage = "You are receiving this message because you (or someone pretending to be you) has asked to receive messages from the list $list.Name.  To activate your subscription, you MUST click the link below to verify your request.\r\n\r\n${store.StoreUrl}Subscription.aspx?action=confirm&list=${list.EmailListId}&email=${email}&key=${signupkey}";

        /// <summary>
        /// The Signup rule for this email list
        /// </summary>
        public EmailListSignupRule SignupRule
        {
            get { return (EmailListSignupRule)this.SignupRuleId; }
            set { this.SignupRuleId = (short)value; }
        }

        /// <summary>
        /// The email template for signup message
        /// </summary>
        public EmailTemplate SignupEmailTemplate
        {
            get { return EmailTemplateDataSource.Load(this.SignupEmailTemplateId,false); }
        }
        
        /// <summary>
        /// Processes a signup request for the given email to this email list
        /// </summary>
        /// <param name="email">The email address to signup</param>
        public void ProcessSignupRequest(string email)
        {
            string loweredEmail = email.ToLowerInvariant();
            //make sure user isn't alredy on list
            if (this.IsMember(loweredEmail)) return;
            //check the signup rule
            EmailTemplate template;
            switch (this.SignupRule)
            {
                case EmailListSignupRule.OptIn:
                    this.Users.Add(new EmailListUser(this.EmailListId, loweredEmail));
                    this.Users.Save();
                    break;
                case EmailListSignupRule.ConfirmedOptIn:
                    this.Users.Add(new EmailListUser(this.EmailListId, loweredEmail));
                    this.Users.Save();
                    //SEND CONFIRMATION MESSAGE
                    template = this.SignupEmailTemplate;
                    if (template == null) {
                        template = new EmailTemplate();
                        template.Body = ConfirmationMessage;
                        template.FromAddress = GetFromAddress();
                        template.IsHTML = false;
                        template.Subject = "Subscription Notice";
                    }
                    template.ToAddress = loweredEmail;
                    template.Parameters.Add("store", Token.Instance.Store);
                    template.Parameters.Add("list", this);
                    template.Parameters.Add("email", loweredEmail);
                    template.Parameters.Add("customer", UserDataSource.LoadMostRecentForEmail(loweredEmail));
                    template.Send();
                    break;
                case EmailListSignupRule.VerifiedOptIn:
                    EmailListSignup signup = EmailListSignupDataSource.Load(this.EmailListId, loweredEmail);
                    if (signup != null) signup.Delete();
                    signup = new EmailListSignup();
                    signup.EmailListId = this.EmailListId;
                    signup.Email = loweredEmail;
                    signup.SignupDate = LocaleHelper.LocalNow;
                    signup.Save();
                    //SEND VERIFICATION MESSAGE
                    template = this.SignupEmailTemplate;
                    if (template == null)
                    {
                        template = new EmailTemplate();
                        template.Body = VerificationMessage;
                        template.FromAddress = GetFromAddress();
                        template.IsHTML = false;
                        template.Subject = "Subscription Notice";
                    }
                    template.ToAddress = loweredEmail;
                    template.Parameters.Add("store", Token.Instance.Store);
                    template.Parameters.Add("list", this);
                    template.Parameters.Add("email", loweredEmail);
                    template.Parameters.Add("customer", UserDataSource.LoadMostRecentForEmail(loweredEmail));
                    template.Parameters.Add("signupkey", signup.SignupKey);
                    template.Send();
                    break;
            }
        }

        private string GetFromAddress()
        {
            StoreSettingCollection settings = Token.Instance.Store.Settings;
            string fromAddress = settings.SubscriptionEmailAddress;
            if (string.IsNullOrEmpty(fromAddress)) fromAddress = settings.DefaultEmailAddress;
            if (string.IsNullOrEmpty(fromAddress)) fromAddress = "admin@domain.xyz";
            return fromAddress;
        }

        /// <summary>
        /// Determines whether the given user is a member of this list.
        /// </summary>
        /// <param name="email">The email address to check for.</param>
        /// <returns>True if the email address is a member; false otherwise.</returns>
        /// <remarks>This is more efficient than Users.IndexOf for checking for membership.</remarks>
        public bool IsMember(string email)
        {
            string loweredEmail = email.ToLowerInvariant();
            return EmailListDataSource.IsMember(this.EmailListId, email);
        }

        /// <summary>
        /// Adds a given email address to this email list
        /// </summary>
        /// <param name="email">The email address to add</param>
        /// <param name="signupDate">The date for this signup</param>
        /// <param name="signupIP">The IP to save for this signup</param>
        public void AddMember(string email, DateTime signupDate, string signupIP)
        {
            string loweredEmail = email.ToLowerInvariant();
            EmailListUser elu = new EmailListUser();
            elu.EmailListId = _EmailListId;
            elu.Email = loweredEmail;
            elu.SignupDate = signupDate;
            elu.SignupIP = StringHelper.Truncate(signupIP, 39);
            if (_Users != null)
            {
                int index = _Users.IndexOf(this.EmailListId, loweredEmail);
                if (index < 0) _Users.Add(elu);
            }
            elu.Save();
        }

        /// <summary>
        /// Removes the specified member from the list.
        /// </summary>
        /// <param name="email">The email address of the member to remove</param>
        public EmailListUser RemoveMember(string email)
        {
            string loweredEmail = email.ToLowerInvariant();
            if (_Users != null)
            {
                int index = _Users.IndexOf(this.EmailListId, loweredEmail);
                EmailListUser user = null;
                if (index > -1)
                {
                    user = _Users[index];
                    _Users.DeleteAt(index);
                }
                return user;
            }
            else
            {
                return EmailListDataSource.RemoveMember(this.EmailListId, loweredEmail);
            }
        }

        /// <summary>
        /// Sends an email message using the given email template to this email list
        /// </summary>
        /// <param name="template">The email template to use for sending the email</param>
        public void SendMessage(EmailTemplate template)
        {
            bool persistUpdates = (this.EmailListId > 0);
            this.LastSendDate = LocaleHelper.LocalNow;
            if (persistUpdates) this.Save();
            template.Parameters["store"] = Token.Instance.Store;
            template.Parameters["list"] = this;
            foreach (EmailListUser elu in this.Users)
            {
                template.ToAddress = elu.Email;
                //TRY TO GET THE CUSTOMER RECORD
                template.Parameters["customer"] = UserDataSource.LoadMostRecentForEmail(elu.Email);
                try
                {
                    elu.LastSendDate = LocaleHelper.LocalNow;
                    template.Send();
                    elu.FailureCount = 0;
                }
                catch
                {
                    elu.FailureCount += 1;
                }
                if (persistUpdates) elu.Save();
            }
        }

        #region AsyncSendMessage

        /// <summary>
        /// Asynchronously sends an email message using the given email template to this email list
        /// </summary>
        /// <param name="template">The email template to use for sending the email</param>
        public void AsyncSendMessage(EmailTemplate template)
        {
            EmailList.AsyncSendMessage(this, template);
        }

        /// <summary>
        /// Asynchronously sends an email message using the given email template to the given email list
        /// </summary>
        /// <param name="emailListId">Id of the email list to which to send the email</param>
        /// <param name="template">The email template to use for sending the email</param>
        public static void AsyncSendMessage(int emailListId, EmailTemplate template)
        {
            EmailList emailList = EmailListDataSource.Load(emailListId);
            AsyncSendMessage(emailList, template);

        }

        /// <summary>
        /// Asynchronously sends an email message using the given email template to the given email list
        /// </summary>
        /// <param name="emailList">The email list to which to send the email</param>
        /// <param name="template">The email template to use for sending the email</param>
        public static void AsyncSendMessage(EmailList emailList, EmailTemplate template)
        {
            AsyncSendMessageDelegate del = new AsyncSendMessageDelegate(InternalAsyncSendMessage);
            AsyncCallback cb = new AsyncCallback(AsyncSendMessageCallback);
            IAsyncResult ar = del.BeginInvoke(Token.Instance.UserId, Token.Instance.StoreId, emailList, template, cb, null);
        }

        private static void AsyncSendMessageCallback(IAsyncResult ar)
        {
            AsyncSendMessageDelegate del = (AsyncSendMessageDelegate)((System.Runtime.Remoting.Messaging.AsyncResult)ar).AsyncDelegate;
            try
            {
                del.EndInvoke(ar);
            }
            catch (Exception ex)
            {
                Logger.Write("Error", Logger.LogMessageType.Error, ex);
            }
        }

        private delegate void AsyncSendMessageDelegate(int userId, int storeId, EmailList emailList, EmailTemplate template);
        private static void InternalAsyncSendMessage(int userId, int storeId, EmailList emailList, EmailTemplate template)
        {
            //REINITIALIZE THE TOKEN WITH SAVED USER AND STORE CONTEXT
            Store store = StoreDataSource.Load(storeId);
            if (store != null)
            {
                Token.Instance.InitStoreContext(store);
                User user = UserDataSource.Load(userId);
                Token.Instance.InitUserContext(user);
                //CHECK THE LIST INSTANCE
                if (emailList != null)
                {
                    //PROCESS THE SEND REQUEST
                    emailList.SendMessage(template);
                }
            }
        }
        #endregion
    }
}
