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
using MakerShop.Products;
using MakerShop.Common;
using MakerShop.Utility;

public partial class ConLib_EditMyReviewPage : System.Web.UI.UserControl
{
    int _ProductReviewId;
    ProductReview _ProductReview;
    Product _Product;

    protected void Page_Load(object sender, EventArgs e)
    {

        _ProductReviewId = AlwaysConvert.ToInt(Request.QueryString["ReviewId"]);

        _ProductReview = ProductReviewDataSource.Load(_ProductReviewId);
        if (_ProductReview == null) Response.Redirect("~/Members/MyProductReviews.aspx");

        _Product = ProductDataSource.Load(_ProductReview.ProductId);
        if (_Product == null) Response.Redirect("~/Members/MyProductReviews.aspx");
        ProductName.Text = _Product.Name;
        ProductReviewForm1.ReviewCancelled += new EventHandler(ProductReviewForm1_ReviewCancelled);
        ProductReviewForm1.ReviewSubmitted += new EventHandler(ProductReviewForm1_ReviewSubmitted);
    }

    void ProductReviewForm1_ReviewSubmitted(object sender, EventArgs e)
    {
        Response.Redirect("~/Members/MyProductReviews.aspx");
    }

    void ProductReviewForm1_ReviewCancelled(object sender, EventArgs e)
    {
        Response.Redirect("~/Members/MyProductReviews.aspx");
    }
}
