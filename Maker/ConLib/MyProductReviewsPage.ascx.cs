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
using MakerShop.Products;
using MakerShop.Utility;

public partial class ConLib_MyProductReviewsPage : System.Web.UI.UserControl
{
    ReviewerProfile _ReviewerProfile;

    protected void Page_Init(object sender, EventArgs e)
    {
        // initialize the user  
        _ReviewerProfile = Token.Instance.User.ReviewerProfile;
        if (_ReviewerProfile != null)
        {
            Email.Text = _ReviewerProfile.Email;
            DisplayName.Text = _ReviewerProfile.DisplayName;
            Location.Text = _ReviewerProfile.Location;
        }
        else
        {
            ProfilePanel.Visible = false;
        }
    }

    private const string ReviewsCaptionText = "Showing {0} Review{1}:";
    private const string PagedReviewsCaptionText = "Showing {0} to {1} of {2} Reviews:";

    private void UpdatePageIndex()
    {
        if (ReviewsGrid.Rows.Count > 0)
        {
            int startIndex = ((ReviewsGrid.PageIndex * ReviewsGrid.PageSize) + 1);
            int endIndex = startIndex + ReviewsGrid.Rows.Count - 1;
            int rowCount = ProductReviewDataSource.CountForReviewerProfile(_ReviewerProfile.ReviewerProfileId);
            if (rowCount > ReviewsGrid.PageSize) ReviewsCaption.Text = string.Format(PagedReviewsCaptionText, startIndex, endIndex, rowCount);
            else ReviewsCaption.Text = string.Format(ReviewsCaptionText, rowCount, ((rowCount > 1) ? "s" : ""));
        }
        else ReviewsCaptionPanel.Visible = false;
    }

    protected void ReviewDs_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        if (_ReviewerProfile != null)
        {
            if (!e.InputParameters.Contains("reviewerProfileId"))
                e.InputParameters.Add("reviewerProfileId", _ReviewerProfile.ReviewerProfileId);
        }
        else
        {
            if (!e.InputParameters.Contains("reviewerProfileId"))
                e.InputParameters.Add("reviewerProfileId", 0);
        }
    }

    protected string GetApprovedText(bool isApproved)
    {
        return (isApproved ? "yes" : "no");
    }

    protected void ReviewsGrid_DataBound(object sender, EventArgs e)
    {
        UpdatePageIndex();
    }
}
