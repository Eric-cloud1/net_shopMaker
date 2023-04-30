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
using MakerShop.Stores;

public partial class ConLib_ProductReviewsPanel : System.Web.UI.UserControl
{
    private int _ProductId;

    protected void Page_Load(object sender, EventArgs e)
    {
        PageHelper.DisableValidationScrolling(this.Page);
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        ProductReviewForm1.ReviewCancelled += new EventHandler(ProductReviewForm1_ReviewCancelled);
        ProductReviewForm1.ReviewSubmitted += new EventHandler(ProductReviewForm1_ReviewSubmitted);
        ReviewLink.Visible = Store.GetCachedSettings().ProductReviewEnabled != MakerShop.Users.UserAuthFilter.None;
    }

    void ProductReviewForm1_ReviewSubmitted(object sender, EventArgs e)
    {
        HideReviewForm();
        ReviewsGrid.DataBind();
    }

    void ProductReviewForm1_ReviewCancelled(object sender, EventArgs e)
    {
        HideReviewForm();
    }

    private void UpdatePageIndex()
    {
        if (ReviewsGrid.Rows.Count > 0)
        {
            int startIndex = ((ReviewsGrid.PageIndex * ReviewsGrid.PageSize) + 1);
            int endIndex = startIndex + ReviewsGrid.Rows.Count - 1;
            int rowCount = ProductReviewDataSource.SearchCount(_ProductId, BitFieldState.True);
            ReviewsCaptionPanel.Visible = true;
            ReviewCount.Text = string.Format(ReviewCount.Text, rowCount, ((rowCount > 1) ? "s" : ""));
            if (rowCount > ReviewsGrid.PageSize)
            {
                ReviewsCaption.Visible = false;
                PagedReviewsCaption.Visible = true;
                int toIndex = startIndex + ReviewsGrid.PageSize - 1;
                if (toIndex > rowCount) toIndex = rowCount;
                PagedReviewsCaption.Text = string.Format(PagedReviewsCaption.Text, startIndex, toIndex, rowCount);
            }
            else
            {
                ReviewsCaption.Visible = true;
                PagedReviewsCaption.Visible = false;
                ReviewsCaption.Text = string.Format(ReviewsCaption.Text, rowCount, ((rowCount > 1) ? "s" : ""));
            }
            AverageRatingPanel.Visible = true;
            RatingImage.ImageUrl = NavigationHelper.GetRatingImage(ProductReviewDataSource.GetProductRating(_ProductId));
        }
        else
        {
            ReviewsCaptionPanel.Visible = false;
            AverageRatingPanel.Visible = false;
        }
    }

    protected void ReviewLink_Click(object sender, EventArgs e)
    {
        ShowReviewForm();
    }

    private void ShowReviewForm()
    {
        ShowReviewsPanel.Visible = false;
        ReviewProductPanel.Visible = true;
        ProductReviewForm1.InitializeForm();
    }

    private void HideReviewForm()
    {
        ShowReviewsPanel.Visible = true;
        ReviewProductPanel.Visible = false;
    }    

    protected void Page_PreRender(object sender, EventArgs e)
    {
        UpdatePageIndex();
    }
}
