using MakerShop.Stores;
using MakerShop.Users;
using MakerShop.Common;
using MakerShop.Utility;
using MakerShop.Messaging;

namespace MakerShop.Products
{
    /// <summary>
    /// This class represents a ReviewerProfile object in the database.
    /// </summary>
    public partial class ReviewerProfile
    {
        private const string VerificationMessage = "You are receiving this message because you (or someone pretending to be you) has posted a product review at ${store.Name}.  To verify your address, you must click the link below:\r\n\r\n${store.StoreUrl}VerifyEmail.aspx?Reviewer=${reviewerProfile.ReviewerProfileId}&Code=${reviewerProfile.EmailVerificationCode}\r\n\r\nIf you did not post a review, you can safely ignore this message.\r\n\r\n${store.Name}\r\n${store.StoreUrl}";

        /// <summary>
        /// Gets the user associated with this reviewer profile.
        /// </summary>
        /// <remarks>The property is null if the user cannot be determined.</remarks>
        public User User
        {
            get
            {
                if (_User == null)
                {
                    _User = UserDataSource.LoadMostRecentForEmail(this.Email);
                }
                return _User;
            }
        }
        private User _User;

        /// <summary>
        /// Save this ReviewerProfile object to the database
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public SaveResult Save()
        {
            return this.Save(true);
        }

        /// <summary>
        /// Save this ReviewerProfile object to the database
        /// </summary>
        /// <param name="checkVerified">If <b>true</b> email address is set for verification if it has not already been verified</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public SaveResult Save(bool checkVerified)
        {
            if (checkVerified)
            {
                //IF EMAIL DOES NOT MATCH SAVED VALUE, UPDATE VERIFIED FLAG
                string storedEmail = ReviewerProfileDataSource.GetEmail(this.ReviewerProfileId);
                if (storedEmail != this.Email) this.EmailVerified = false;
            }
            return this.BaseSave();
        }

        private string GetFromAddress()
        {
            StoreSettingCollection settings = Token.Instance.Store.Settings;
            string fromAddress = settings.DefaultEmailAddress;
            if (string.IsNullOrEmpty(fromAddress)) fromAddress = "admin@domain.xyz";
            return fromAddress;
        }

        /// <summary>
        /// Sends a verification email
        /// </summary>
        public void SendVerificationEmail()
        {
            this.SendVerificationEmail(null);
        }

        /// <summary>
        /// Sends a verification email
        /// </summary>
        /// <param name="review">Product review for which verification is being processed</param>
        public void SendVerificationEmail(ProductReview review)
        {
            StoreSettingCollection settings = Token.Instance.Store.Settings;
            EmailTemplate template = EmailTemplateDataSource.Load(settings.ProductReviewEmailVerificationTemplate);
            if (template == null) {
                template = new EmailTemplate();
                template.Body = VerificationMessage;
                template.FromAddress = GetFromAddress();
                template.IsHTML = false;
                template.Subject = "Product Review Verification";
            }
            template.ToAddress = this.Email;
            template.Parameters.Add("store", Token.Instance.Store);
            template.Parameters.Add("reviewerProfile", this);
            template.Parameters.Add("customer", this.User);
            template.Parameters.Add("review", review);
            template.Send();
        }

        /// <summary>
        /// Used to indicate that the email address has been verified.
        /// </summary>
        public void VerifyEmail()
        {
            this.EmailVerificationCode = System.Guid.Empty;
            this.EmailVerified = true;
            this.Save();
            //CHECK WHETHER MANUAL APPROVAL IS REQUIRED FOR THIS USER
            if (!ProductReviewHelper.ApprovalRequired(this.User))
            {
                //APPROVAL NOT REQUIRED, SO AUTOMATICALLY APPROVE ANY REVIEWS BY THIS USER
                foreach (ProductReview review in this.ProductReviews)
                {
                    if (!review.IsApproved)
                    {
                        review.IsApproved = true;
                        review.Save();
                    }
                }
            }
        }
    }
}
