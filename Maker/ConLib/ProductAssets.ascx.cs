using System;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using MakerShop.Products;
using MakerShop.Utility;

public partial class ConLib_ProductAssets : System.Web.UI.UserControl
{
    private string _Caption = "Additional Details";

    [Personalizable(), WebBrowsable()]
    public string Caption
    {
        get { return _Caption; }
        set { _Caption = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        int _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        Product _Product = ProductDataSource.Load(_ProductId);
        if (_Product != null)
        {
            List<ProductAssetWrapper> assetList = ProductHelper.GetAssets(this.Page, _Product, "javascript:window.close()");
            if (assetList.Count > 0)
            {
                CaptionText.Text = this.Caption;
                AssetLinkList.DataSource = assetList;
                AssetLinkList.DataBind();
                ProductAssetsPanel.Visible = true;
            }
        }
    }
}
