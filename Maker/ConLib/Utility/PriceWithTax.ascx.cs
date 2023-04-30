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
using MakerShop.Taxes;
using MakerShop.Utility;

public partial class ConLib_Utility_PriceWithTax : System.Web.UI.UserControl
{
    private LSDecimal _Price = 0;
    private int _TaxCodeId = 0;


    public LSDecimal Price
    {
        set { _Price = AlwaysConvert.ToDecimal(value); }
    }

    public int TaxCodeId
    {
        set { _TaxCodeId = AlwaysConvert.ToInt(value); }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        PriceLiteral.Text = TaxHelper.GetShopPrice(_Price, _TaxCodeId).ToString("ulc");
    }
}