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

public partial class ConLib_ProductRatingImage : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        Product _Product = ProductDataSource.Load(_ProductId);
        if (_Product != null && Token.Instance.Store.Settings.ProductReviewEnabled != MakerShop.Users.UserAuthFilter.None)
        {
            string ratingUrl = NavigationHelper.GetRatingImage(ProductReviewDataSource.GetProductRating(_ProductId));
            this.Controls.Add(new LiteralControl("<img src=\"" + ratingUrl + "\" border=\"0\" />"));
        }
    }
}
