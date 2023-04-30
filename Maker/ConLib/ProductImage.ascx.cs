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
using MakerShop.Utility;

public partial class ConLib_ProductImage : System.Web.UI.UserControl
{
    //can be icon, thumbnail, or image
    private string _ShowImage = "Thumbnail";

    [Personalizable(), WebBrowsable()]
    public string ShowImage
    {
        get { return _ShowImage; }
        set { _ShowImage = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        int _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        Product _Product = ProductDataSource.Load(_ProductId);
        if (_Product != null)
        {
            string checkImage = this.ShowImage.ToLowerInvariant();
            string imageUrl;
            string imageAltText;
            if (checkImage == "icon")
            {
                imageUrl = _Product.IconUrl;
                imageAltText = _Product.IconAltText;
            }
            else if (checkImage == "thumbnail")
            {
                imageUrl = _Product.ThumbnailUrl;
                imageAltText = _Product.ThumbnailAltText;
            }
            else
            {
                imageUrl = _Product.ImageUrl;
                imageAltText = _Product.ImageAltText;
            }
            if (!string.IsNullOrEmpty(imageUrl))
            {
                phProductImage.Controls.Add(new LiteralControl("<img id=\"ProductImage\" src=\"" + Page.ResolveClientUrl(imageUrl) + "\" border=\"0\" alt=\"" + Server.HtmlEncode(imageAltText) + "\" />"));
            }
        }
    }
}
